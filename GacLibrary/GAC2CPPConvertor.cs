using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GAppCreator
{
    public class GAC2CPPConvertor
    {
        private static Project prj;
        private static ITaskNotifier task;
        private static string GACtoCPPFolder = "";


        private static bool UpdateScenesList()
        {
            GACParser.GACFile gf = GACParser.Compiler.GetGACFile("");
            GACParser.Module moduleScenes = GACParser.Compiler.GetModule("Scenes");

            bool hasMainScene = false;
            if (moduleScenes == null)
                return false;
            // curat totul
            foreach (GACParser.Module m in moduleScenes.Modules.Values)
                gf.RemoveModule(m.Name);
            moduleScenes.Modules.Clear();
            moduleScenes.Members.Clear();

            foreach (ProjectFile pf in prj.Files)
            {
                GACParser.GACFile gf_p = GACParser.Compiler.GetGACFile(pf.Name);
                if (gf_p == null)
                    continue;
                if (gf_p.Scenes.Count>0)
                {
                    foreach (string s in gf_p.Scenes)
                    {
                        moduleScenes.Members[s] = new GACParser.Member(moduleScenes, GACParser.MemberType.Constant, s, "GAC_SCENE_ID_" + s, "int", "", null);
                        if (s == "Main")
                            hasMainScene = true;
                    }
                }
            }
            moduleScenes.UpdateAutoCompleteList();
            return hasMainScene;
        }
        private static bool PreprocessDefinitions(bool silent, bool removeDebugCommands)
        {
            bool allOK = true;
            if (!silent)
            {
                task.SetMinMax(0, prj.Files.Count);
                task.CreateSubTask("Analyzing definitions ...");
            }
            GACParser.Compiler.ClearLocals();
            foreach (ProjectFile pf in prj.Files)
            {
                GACParser.ClearError();
                if (pf.Parser.LoadFile(Path.Combine(prj.ProjectPath, "Sources", pf.Name),removeDebugCommands) == false)
                {
                    if (!silent)
                        task.SendCommand(Command.AddCompileError, GACParser.GetError());
                    allOK = false;
                    continue;
                }
                if (pf.Parser.PreprocessDefinitions() == false)
                {
                    if (!silent)
                        task.SendCommand(Command.AddCompileError, GACParser.GetError());
                    allOK = false;
                    continue;
                }
                if (!silent)
                    task.IncrementProgress();
            }
            if (UpdateScenesList()==false)
            {
                if (!silent)
                    task.SendCommand(Command.AddCompileError, "error: Main scene is expected for this app to work !");
                allOK = false;
            }
            if (!silent)
                task.UpdateSuccessErrorState(allOK);
            return allOK;
        }
        private static bool AnalizeClassesMembers(bool silent)
        {
            bool allOK = true;
            if (!silent)
            {
                task.SetMinMax(0, GACParser.Compiler.LocalClasses.Count);
                task.CreateSubTask("Analyzing classes ...");
                task.SendCommand(Command.AddCompileOutput, "Analyzing classes ...");
            }
            Dictionary<string, GACParser> dp = new Dictionary<string, GACParser>();
            foreach (ProjectFile pf in prj.Files)
            {
                dp[pf.Name] = pf.Parser;
                pf.Parser.UpdateTokensForLocalModules();
            }
            foreach (GACParser.Module m in GACParser.Compiler.LocalClasses)
            {
                GACParser.ClearError();
                if (dp[m.FileName].AnalizeClassMembers(m) == false)
                {
                    if (!silent)
                        task.SendCommand(Command.AddCompileError, GACParser.GetError());
                    allOK = false;
                    continue;
                }
                if (!silent)
                    task.IncrementProgress();
            }
            if (!silent)
                task.UpdateSuccessErrorState(allOK);
            return allOK;
        }
        private static bool AnalizeFunctionCode(bool silent)
        {
            bool allOK = true;
            if (!silent)
            {
                task.SetMinMax(0, prj.Files.Count);
                task.CreateSubTask("Analyzing functions ...");
                task.SendCommand(Command.AddCompileOutput, "Analyzing functions ...");
            }
            foreach (ProjectFile pf in prj.Files)
            {
                GACParser.ClearError();
                if (pf.Parser.AnalizeCode() == false)
                {
                    if (!silent)
                        task.SendCommand(Command.AddCompileError, GACParser.GetError());
                    allOK = false;
                    continue;
                }
                // daca am cumva o eroare
                if (GACParser.GetError().Length > 0)
                {
                    if (!silent)
                        task.SendCommand(Command.AddCompileError, GACParser.GetError());
                    allOK = false;
                    continue;
                }
                if (!silent)
                    task.IncrementProgress();
            }
            if (!silent)
                task.UpdateSuccessErrorState(allOK);
            return allOK;
        }
        private static bool CreateCPPFiles()
        {
            bool allOK = true;
            string content = "";
            task.SetMinMax(0, prj.Files.Count);
            task.CreateSubTask("Creating C++ files ...");
            task.SendCommand(Command.AddCompileOutput, "Creating C++ files ...");
            foreach (ProjectFile pf in prj.Files)
            {
                if (pf.Parser.CreateCppFile(pf.Parser.GetFileName(),ref content)==false)
                {
                    task.SendCommand(Command.AddCompileError, GACParser.GetError());
                    task.UpdateSuccessErrorState(false);
                    return false;
                }
                if (Disk.SaveFile(Path.Combine(GACtoCPPFolder, pf.Parser.GetFileName("cpp")), content, prj.EC) == false)
                {
                    allOK = false;
                    continue;
                }
                task.IncrementProgress();
            }
            // animatiile
            content = "#include \"Framework.h\"\n\n";
            foreach (var a in prj.AnimationObjects)
            {
                content += a.CreateCPPWrapperCodeClass(prj);
            }
            if (Disk.SaveFile(Path.Combine(GACtoCPPFolder, "AnimationTemplateWrapper.cpp"), content, prj.EC) == false)
            {
                allOK = false;
            }
            task.UpdateSuccessErrorState(allOK);
            return allOK;
        }
        private static bool CreateFrameworkHFile(Project prj, GenericBuildConfiguration build)
        {
            task.CreateSubTask("Creating headers ...");
            task.SendCommand(Command.AddCompileOutput, "Creating headers ...");
            string hfile = "#ifndef __GAC_FRAMEWORK__\n#define __GAC_FRAMEWORK__\n#include \"Gapp.h\"\n#include \"Resources.h\"\n#include \"IDs.h\"\n";
            // componenta de new si delete
            hfile += "\nvoid* operator new(size_t, const char*,const char*,int,const char*,size_t);\n";
            hfile += "\nvoid operator delete (void* ptr); \n";      
            hfile += GACParser.CreateHFileComponent(prj,build) + "\n#endif\n";
            bool res = Disk.SaveFile(Path.Combine(GACtoCPPFolder, "Framework.h"), hfile, prj.EC);
            res &= Disk.SaveFile(Path.Combine(GACtoCPPFolder, "IDs.h"), GACParser.CreateIDsHFileComponent(prj,build), prj.EC);
            task.UpdateSuccessErrorState(res);
            return res;
        }
        private static bool CreateCppProject(GenericBuildConfiguration build)
        {
            string lib_path = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Libs");
            string prj_include_cpp = "";
            if (prj.AnimationObjects.Count>0)
                prj_include_cpp += "\t<ClCompile Include=\"AnimationTemplateWrapper.cpp\" />\n";

            foreach (ProjectFile pf in prj.Files)
                prj_include_cpp += "\t<ClCompile Include=\"" + pf.Parser.GetFileName("cpp") + "\" />\n";
            int resolution_width = 0;
            int resolution_height = 0;
            int design_resolution_width = 0;
            int design_resolution_height = 0;

            if (Project.SizeToValues(((DevelopBuildConfiguration)prj.BuildConfigurations[0]).AppResolution, ref resolution_width, ref resolution_height) == false)
            {
                prj.EC.AddError("Unable to parse DesignResolution: " + ((DevelopBuildConfiguration)prj.BuildConfigurations[0]).AppResolution);
                return false;
            }
            if (Project.SizeToValues(prj.DesignResolution, ref design_resolution_width, ref design_resolution_height) == false)
            {
                prj.EC.AddError("Unable to parse DesignResolution: " + ((DevelopBuildConfiguration)prj.BuildConfigurations[0]).AppResolution);
                return false;
            }

            Dictionary<string, string> ds = new Dictionary<string, string>()
            {
                { "$$IMAGE_DEFINITIONS$$"           , "" },
                { "$$FONT_DEFINITIONS$$"            , "" },
                { "$$TEXTURE_DEFINITIONS$$"         , "" },
                { "$$RAW_DEFINITIONS$$"             , "" },
                { "$$SOUND_DEFINITIONS$$"           , "" },
                { "$$SHADER_DEFINITIONS$$"          , "" },
                { "$$PRESENTATION_DEFINITIONS$$"    , "" },
                { "$$STRINGS_DEFINITIONS$$"         , "" },
                { "$$IMAGE_INIT_CODE$$"             , "" },
                { "$$FONT_INIT_CODE$$"              , "" },
                { "$$TEXTURE_INIT_CODE$$"           , "" },
                { "$$RAW_INIT_CODE$$"               , "" },
                { "$$SOUND_INIT_CODE$$"             , "" },
                { "$$SHADER_INIT_CODE$$"            , "" },
                { "$$PRESENTATION_INIT_CODE$$"      , "" },
                { "$$STRINGS_INIT_CODE$$"           , "" },
                { "$$INCLUDE_FOLDER$$"              , lib_path },
                { "$$CPP_INCLUDE_IN_PROJECT$$"      , prj_include_cpp },
                { "$$SCREEN.HEIGHT$$"               , resolution_height.ToString() },
                { "$$SCREEN.WEIGHT$$"               , resolution_width.ToString() },
                { "$$SCREEN.DESIGNHEIGHT$$"         , design_resolution_height.ToString() },
                { "$$SCREEN.DESIGNWEIGHT$$"         , design_resolution_width.ToString() },
                { "$$SIMULATEDADS.INIT$$"           , "" },
                { "$$ALARM.CHECKUPDATETICKS$$"      , (build.AlarmCheckUpdateSeconds*60).ToString() },
            };
            if (prj.UpdateReplaceDictionary(ds,prj.BuildConfigurations[0]) == false)
                return false;
            // update la ad-uri
            List<GenericAd> ads = prj.GetAdsUsedInBuild(build);
            if ((ads!=null) && (ads.Count>0))
            {
                string s = "\n";
                foreach (GenericAd ad in ads)
                {
                    System.Drawing.RectangleF poz = ad.GetPositionForSimulatedAds();
                    s += String.Format("\tGApp::DevelopMode::Execution::CreateAd({0},{1},\"{2}\",(float){3},(float){4},(float){5},(float){6},{7},{8},{9},{10});\n", resolution_width, resolution_height, ad.Name, poz.Left, poz.Top, poz.Width, poz.Height, ad.LoadOnStartup.ToString().ToLower(), (int)ad.GetAdTtype(), ad.ReLoadAfterOpen.ToString().ToLower(), ad.MaxAttemptsOnFail);
                }
                ds["$$SIMULATEDADS.INIT$$"] = s;
            }
            if (Project.CreateResource("Develop","main.cpp", ds, Path.Combine(GACtoCPPFolder, "main.cpp"), prj.EC) == false)
                return false;
            // tool version
            ds["$$TOOLS.VERSION$$"] = "4.0";
            ds["$$PLATFORM.VERSION$$"] = "v120";
            if ((prj.Settings.VSToolSet!=null) && (prj.Settings.VSToolSet.Length>0))
            {
                int idx = prj.Settings.VSToolSet.IndexOf('/');
                if (idx>0)
                {
                    ds["$$PLATFORM.VERSION$$"] = prj.Settings.VSToolSet.Substring(0, idx ).Trim();
                    ds["$$TOOLS.VERSION$$"] = prj.Settings.VSToolSet.Substring(idx + 1).Trim();

                }
            }
            if (Project.CreateResource("Develop","Project.vcxproj", ds, Path.Combine(GACtoCPPFolder, prj.GetProjectName() + ".vcxproj"), prj.EC) == false)
                return false;

            return true;
        }
        private static void UpdateDefines(Project proj,GenericBuildConfiguration build)
        {
            GACParser.Builds.Clear();
            foreach (GenericBuildConfiguration b in prj.BuildConfigurations)
                GACParser.Builds[b.Name] = true;
            
            GACParser.Defines.Clear();
            Dictionary<string, string> all = Project.StringListToDict(proj.Defines);
            Dictionary<string, string> fromBuild = Project.StringListToDict(build.Defines);

            foreach (string key in all.Keys)
            {
                GACParser.Defines[all[key]] = fromBuild.ContainsKey(key);
            }

            GACParser.CurrentOS = build.GetOS().ToString();
            GACParser.CurrentBuild = build.Name;
            GACParser.ExtraConstants.Clear();
        }
        public static bool Convert(Project proj, ITaskNotifier tsk, bool createVSProject, ref bool UpdateEditorLocalTypes, GenericBuildConfiguration build, bool removeDebugCommands, bool enableMemoryAnalysis)
        {
            UpdateEditorLocalTypes = false;
            prj = proj;
            task = tsk;
            UpdateDefines(proj, build);
            GACtoCPPFolder = Path.Combine(prj.ProjectPath, "CppProject");
            if (Disk.CreateFolder(GACtoCPPFolder, prj.EC) == false)
                return false;
            GACParser.EnableMemoryHook = enableMemoryAnalysis;
            try
            {

                if (PreprocessDefinitions(false,removeDebugCommands) == false)
                    return false;
                if (GACParser.ComputeClassOrder() == false)
                {
                    task.SendCommand(Command.AddCompileError, GACParser.GetError());
                    return false;
                }

                if (AnalizeClassesMembers(false) == false)
                    return false;
                if (AnalizeFunctionCode(false) == false)
                    return false;

                // update la intellisense
                GACParser.UpdateGlobalAutoComplete(prj, build);
                UpdateEditorLocalTypes = true;
                //GACEditor.UpdateWithLocalTypes();

                if (CreateCPPFiles() == false)
                    return false;
                task.SendCommand(Command.UpdateGacFileList);
                if (CreateFrameworkHFile(proj,build) == false)
                    return false;

                if (createVSProject)
                {
                    task.CreateSubTask("Creating project ...");
                    task.SendCommand(Command.AddCompileOutput, "Creating project ...");
                    bool res = CreateCppProject(build);
                    task.UpdateSuccessErrorState(res);
                    if (res == false)
                        return false;
                }
                task.Info("Compile ok !");
                task.SendCommand(Command.AddCompileOutput, "Compile ok !");
                return true;
            }
            catch (Exception e)
            {
                prj.EC.AddException("Compiler internal error", e);
                return false;
            }
        }
        public static void QuickCompile(Project proj, ITaskNotifier tsk, GenericBuildConfiguration build)
        {
            prj = proj;
            task = tsk;
            UpdateDefines(proj, build);
            task.SendCommand(Command.UpdateGlobalAutoComplete);
            while (true)
            {
                try
                {
                    if (PreprocessDefinitions(false,false) == false)
                        break;
                    if (GACParser.ComputeClassOrder() == false)
                    {
                        task.SendCommand(Command.AddCompileError, GACParser.GetError());
                        break;
                    }
                    if (AnalizeClassesMembers(false) == false)
                        break;
                    if (AnalizeFunctionCode(false) == false)
                        break;
                    break;
                }
                catch (Exception)
                {
                    break;
                }
            }
            task.SendCommand(Command.UpdateGlobalAutoComplete);
            task.SendCommand(Command.UpdateLocalTypes);
            task.SendCommand(Command.UpdateGacFileList);
            task.SendCommand(Command.EnableIntelliSenseTimer);
        }
    }
}
