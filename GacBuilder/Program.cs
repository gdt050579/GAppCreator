using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace GAppCreator
{
    class Notifier : ITaskNotifier
    {
        int minValue = 0;
        int maxValue = 0;
        int progress = 0;
        int lastProc = -1;
        private void ShowTime()
        {
            Program.CPrint("#{10}" + DateTime.Now.ToString() + " |");
        }
        public void Info(string text)
        {
            ShowTime();
            Program.CPrint("#{7}[ INFO  ] "+text+"\n");
        }
        public void CreateSubTask(string text)
        {
            ShowTime();
            Program.CPrint("#{11}[ TASK  ] " + text + "\n");
            Program.Log("T:" + text + "\n");
            lastProc = -1;
        }
        public void IncrementProgress()
        {
            progress++;
            if (maxValue > 0)
            {
                int proc = (progress * 100 / maxValue);
                if (proc > lastProc)
                {
                    Program.Log("P:" + proc.ToString() + "\n");
                    lastProc = proc;
                }
            }
        }
        public void SetMinMax(int min, int max)
        {
            progress = min;
            minValue = min;
            maxValue = max;
            lastProc = -1;
        }
        public bool UpdateSuccessErrorState(bool value)
        {
            ShowTime();
            if (value)
                Program.CPrint("#{2}-------------[ ALLOK ]-------------\n");
            else
                Program.CPrint("#{12}-------------[ ERROR ]-------------\n");                        
            return value;
        }
        public void SendCommand(Command cmd)
        {
            SendCommand(cmd, null);
        }
        public void SendCommand(Command cmd, Object param)
        {
            string n = "None";
            if (param != null)
                n = param.ToString();

            switch (cmd)
            {
                case Command.AddCompileError:
                    ShowTime();
                    Program.CPrint("#{12}[ ERROR ] " + n.Trim() + "\n");
                    Program.Log("E:" + n.Trim() + "\n");
                    break;
                default:
                    ShowTime();
                    Program.CPrint("#{11}[ACTION ] " + cmd.ToString() + "(" + n + ") \n");
                    break;
            }
            
        }
    }
    class Program
    {
        static FileStream logFile = null;
        static SystemSettings Settings;
        static ErrorsContainer EC = new ErrorsContainer();
        static Dictionary<string, string> parameters;
        static ConsoleColor[] colors = { ConsoleColor.Black, ConsoleColor.DarkBlue, ConsoleColor.DarkGreen, ConsoleColor.DarkCyan, ConsoleColor.DarkRed, ConsoleColor.DarkMagenta, ConsoleColor.DarkYellow, ConsoleColor.Gray, ConsoleColor.Gray, ConsoleColor.Blue, ConsoleColor.Green, ConsoleColor.Cyan, ConsoleColor.Red, ConsoleColor.Magenta, ConsoleColor.Yellow, ConsoleColor.White };
        static public void CPrint(string s)
        {
            if (s.Length > 20000)
                s = s.Substring(0, 20000)+" ..... too many errors .... ";
            string s2 = s + "     ";
            string buf = "";

            Console.ResetColor();
            for (int tr = 0; tr < s.Length; tr++)
            {
                if ((s[tr] == '#') && (s[tr + 1] == '{'))
                {
                    if (s[tr + 3] == '}')
                    {
                        Console.Write(buf);
                        Console.ForegroundColor = colors[s[tr + 2] - 48];
                        buf = "";
                        tr = tr + 3;
                        continue;
                    }
                    if (s[tr + 4] == '}')
                    {
                        Console.Write(buf);
                        Console.ForegroundColor = colors[(s[tr + 2] - 48) * 10 + (s[tr + 3] - 48)];
                        buf = "";
                        tr = tr + 4;
                        continue;
                    }
                }
                buf += s[tr];
            }
            Console.Write(buf);
        }
        static public void Log(string str)
        {
            if (logFile == null)
                return;
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(str);
                logFile.Write(bytes, 0, bytes.Length);
                logFile.Flush(true);
            }
            catch (Exception e)
            {
                CPrint("#{12}[ ERROR ] Unable to write to log file.\n" + e.ToString());
            }
        }
        static void Help()
        {
            CPrint("Use #{15}GacBuilder #{13}command #{7}[-options]\n");
            CPrint("Where command is\n");
            CPrint("   #{13}build                      #{7}builds the hole package completely\n");
            CPrint("   #{13}quickbuild                 #{7}builds the hole package skipping resource generation\n");            
            CPrint("   #{13}packfile                   #{7}packs and encrypts one file\n");
            CPrint("   #{13}hashlist                   #{7}create project hash list file\n");
            CPrint("   #{13}updateproject              #{7}Update files from a specific project\n");
            CPrint("   #{13}clean                      #{7}Cleans up the current project file from the work folder.\n");
            CPrint("   #{13}createupdatepack           #{7}Create an update pack for GACCreator.\n");
            CPrint("   #{13}listusers                  #{7}List all users\n");
            CPrint("   #{13}listprojects               #{7}List all projects (from a specific user - optional)\n");
            CPrint("and options are\n");
            CPrint("   #{11}-buildname:<name>          #{7}specifies the name of the build\n");
            CPrint("   #{11}-project:<file>            #{7}Name of the project\n");
            CPrint("   #{11}-projectpath:<folder>      #{7}Path to a project. Only for DEBUG purposes.\n");
            CPrint("   #{11}-settings:<file>           #{7}Full path to settings xml file\n");
            CPrint("   #{11}-gacxml:<file>             #{7}Full path to gac xml definition file\n");
            CPrint("   #{11}-pass:<password>           #{7}Password to be used when packing a file\n");
            CPrint("   #{11}-in:<file>                 #{7}File to be packed\n");
            CPrint("   #{11}-out:<file>                #{7}Result file after packing\n");
            CPrint("   #{11}                           #{7}Use the following macros:\n");
            CPrint("   #{11}                           #{7}- $$APP$$ - default file name of the application\n");
            CPrint("   #{11}                           #{7}- $$DATE$$ - date time in format YYYY-MM-DD---HH-MM-SS\n");
            CPrint("   #{11}                           #{7}- $$PATH$$ - default path of the resulted file\n");
            CPrint("   #{11}-user:<name>               #{7}Name of the user\n");
            CPrint("   #{11}-updatepack:<file>         #{7}Path to diff file to be used for update\n");
            CPrint("   #{11}-includeBuildResources     #{7}Add to package resources that were already build\n");
            CPrint("   #{11}-gacfolder:<folder>        #{7}Path to GAC Creator folder.\n");
            CPrint("   #{11}-skipcleanup               #{7}Skip any folder clean up after build !\n");



        }
        static void Error(string s)
        {
            CPrint("#{12}[ ERROR ] " + s); Log("E:" + s.Replace("\n","\\n").Replace("\r","") + "\n");
        }
        static void ShowErrorContainer(ErrorsContainer ec)
        {
            if (ec.HasErrors())
            {
                int count = ec.GetCount();
                for (int tr = 0; tr < count; tr++)
                {
                    ErrorsContainer.ErrorInfo e = ec.Get(tr);
                    string s = e.Module;
                    if (s.Length>0)
                        s+=":";
                    s+=e.Error;
                    if (e.Type == ErrorsContainer.ErrorType.Exception)
                        s += "\n" + e.Exception;
                    s = s.Trim() + "\n";
                    Error(s);

                }
                ec.Reset();
            }
        }
        static string GetBuildName()
        {
            if (parameters.ContainsKey("buildname") == false)
                return null;
            return parameters["buildname"];
        }
        static bool CheckParameter(string name, string message)
        {
            if (parameters.ContainsKey(name) == false)
            {
                Error("Missing '-" + name + "' parameter: " + message);
                return false;
            }
            return true;
        }
        static SystemSettings CreateSystemSettings()
        {
            if (parameters.ContainsKey("settings") == false)
            {
                Error("Missing '-Settings' parameters");
                return null;
            }
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(SystemSettings));
                TextReader textReader = new StreamReader(parameters["settings"]);
                SystemSettings sol = (SystemSettings)serializer.Deserialize(textReader);
                textReader.Close();
                if (sol == null)                    
                {
                    Error("Unable to load settings from " + parameters["settings"]);
                    return null;
                }
                if (Directory.Exists(sol.RepoFolder) == false)
                {
                    Error("Repository folder not specified or invalid (" + sol.RepoFolder + ")");
                    return null;
                }
                if (Directory.Exists(sol.BuildFolder) == false)
                {
                    Error("Build folder not specified or invalid (" + sol.BuildFolder + ")");
                    return null;
                }
                if (File.Exists(sol.InskapePath) == false)
                {
                    Error("Path to inkscape.exe file is invalid or missing ("+sol.InskapePath+")");
                    return null;
                }
                return sol;
            }
            catch (Exception e)
            {
                Error("Unable to load settings from " + parameters["settings"] + "\n" + e.ToString());
                return null;
            }
        }
        static bool BuildIcons(Project prj, Notifier task)
        {
            string bn = GetBuildName();
            if (bn == null)
            {
                Error("Missing -buildname parameters");
                return task.UpdateSuccessErrorState(false);
            }
            GenericBuildConfiguration bg = prj.GetBuild(bn);
            if (bg == null)
            {
                Error("Configuration '" + bn + "' does not exist");
                return task.UpdateSuccessErrorState(false);
            }
            Project.ProjectIcons Icons = prj.GetBuildIcons(bg);
            task.SetMinMax(0, Icons.OutputFiles.Count);
            task.CreateSubTask("Building icons");
            foreach (string path in Icons.OutputFiles.Keys)
            {
                task.Info(String.Format("Creating icon: {0}x{0} => {1}", Icons.OutputFiles[path].Size, Path.GetFileName(Icons.OutputFiles[path].Output)));
                task.UpdateSuccessErrorState(prj.SVGtoPNG(Icons.OutputFiles[path].Source, Icons.OutputFiles[path].Output, 0, Icons.OutputFiles[path].Size, Icons.OutputFiles[path].Size, 1.0f, true));
                task.IncrementProgress();
                if (prj.EC.HasErrors())
                {
                    prj.ShowErrors();
                    return task.UpdateSuccessErrorState(false);
                }
            }

            return task.UpdateSuccessErrorState(true);
        }
        static bool BuildAndLoadResources(Project prj,Notifier task,bool build)
        {
            task.CreateSubTask("Create resources");
            task.SetMinMax(0, prj.Resources.Count);
            foreach (GenericResource res in prj.Resources)
            {
                if (res.IsDeriveFromOtherResources() == true)
                    continue;
                if (build)
                {
                    if (res.Build() == false)
                    {
                        Error("Unable to build resource: " + res.GetResourceVariableName() + "\n");
                        prj.ShowErrors();
                        return task.UpdateSuccessErrorState(false);
                    }
                }
                if (res.Load() == false)
                {
                    Error("Unable to load resource: " + res.GetResourceVariableName() + "\n");
                    prj.ShowErrors();
                    return task.UpdateSuccessErrorState(false);
                }
                task.IncrementProgress();
            }
            foreach (GenericResource res in prj.Resources)
            {
                if (res.IsDeriveFromOtherResources() == false)
                    continue;
                if (build)
                {
                    if (res.Build() == false)
                    {
                        Error("Unable to build resource: " + res.GetResourceVariableName() + "\n");
                        prj.ShowErrors();
                        return task.UpdateSuccessErrorState(false);
                    }
                }
                if (res.Load() == false)
                {
                    Error("Unable to load resource: " + res.GetResourceVariableName() + "\n");
                    prj.ShowErrors();
                    return task.UpdateSuccessErrorState(false);
                }
                task.IncrementProgress();
            }
            return task.UpdateSuccessErrorState(true);
        }
        static BuildExtension GetBuildExtension(GenericBuildConfiguration bld)
        {
            if (bld.GetType() == typeof(AndroidBuildConfiguration))
                return new AndroidBuildExtension();
            if (bld.GetType() == typeof(IOSBuildConfiguration))
                return new IOSBuildExtension();
            if (bld.GetType() == typeof(MacBuildConfiguration))
                return new MacBuildExtension();
            if (bld.GetType() == typeof(WindowsDesktopBuildConfiguration))
                return new WindowsDesktopBuildExtension();
            Error("No extension available for " + bld.GetType().ToString());
            return null;
        }
        static Project GetProject()
        {
            if (!CheckParameter("project", ""))
                return null;
            if (!CheckParameter("user", ""))
                return null;
            Project prj = Project.Load(Path.Combine(Settings.RepoFolder, parameters["user"], parameters["project"], "Current","project.gappcreator"), Settings,EC);
            if (prj == null)
            {
                ShowErrorContainer(EC);
                return null;
            }
            prj.SetErrorFunction(ShowErrorContainer);
            return prj;
        }
        static bool ClearBuildLock()
        {
            string buildLock = Path.Combine(Settings.BuildFolder, parameters["user"] , parameters["project"],"build.lock");
            if (File.Exists(buildLock))
            {
                if (Disk.DeleteFile(buildLock, EC) == false)
                {
                    Error("Unable to delete build lock ...");
                    ShowErrorContainer(EC);
                    return false;
                }
            }
            return true;
        }
        static bool CopyToWorkingPath(bool includeBuildResources)
        {
            if (!CheckParameter("project", ""))
                return false;
            if (!CheckParameter("user", ""))
                return false;
            string workFolder = Path.Combine(Settings.BuildFolder, parameters["user"], parameters["project"]);
            if (File.Exists(Path.Combine(workFolder,"build.lock")))
            {
                Error("A build is currently executing ... cannot perform another build until the current one finishes!");
                return false;
            }
            if (Disk.SaveFile(Path.Combine(workFolder, "build.lock"), " ", EC) == false)
            {
                Error("Unable to create build.lock...");
                ShowErrorContainer(EC);
                return false;
            }
            Project prj = GetProject();
            if (prj == null)
            {
                ClearBuildLock();
                return false;
            }
            GZipPackage gzp = prj.CreateProjectPackage(includeBuildResources, GetBuildName(),true);
            if (gzp == null)
            {
                Error("Unable to create package for : '" + parameters["project"] + "' (2)\n");
                prj.ShowErrors();
                ClearBuildLock();
                return false;
            }
            if (gzp.CopyTo(workFolder, EC) == false)
            {
                Error("Unable to copy project '" + parameters["project"] + "' to work folder.\n");
                ShowErrorContainer(EC);
                ClearBuildLock();
                return false;
            }
            return true;
        }
        static bool BuildApp(bool quick)
        {
            if (!CheckParameter("buildname", ""))
                return false;
            if (!CheckParameter("settings", ""))
                return false;
            if (!CheckParameter("gacxml", ""))
                return false;

            Project prj;
            bool useFullPathToProjectOption = parameters.ContainsKey("projectpath");
            if (useFullPathToProjectOption == false)
            {
                if (!CheckParameter("project", ""))
                    return false;
                if (CopyToWorkingPath(quick) == false)
                    return false;
                prj = Project.Load(Path.Combine(Settings.BuildFolder, parameters["user"],parameters["project"], "project.gappcreator"), Settings,EC);
            } else {
                prj = Project.Load(Path.Combine(parameters["projectpath"], "project.gappcreator"), Settings, EC);
            }

            if (prj == null)
            {
                ShowErrorContainer(EC);
                return false;
            }
            prj.SetErrorFunction(ShowErrorContainer);
            Console.WriteLine("Project Folder: " + prj.ProjectPath);
            // caut buildul
            GenericBuildConfiguration bld = null;
            for (int tr = 1; tr < prj.BuildConfigurations.Count; tr++)
            {
                if (prj.BuildConfigurations[tr].Name.ToLower().Equals(parameters["buildname"].ToLower()))
                {
                    bld = prj.BuildConfigurations[tr];
                    break;
                }
            }
            if (bld == null)
            {
                Error("There is no buld configuration with name: '" + parameters["buildname"] + "'\n");
                return false;
            }
            Notifier task = new Notifier();

            // fac folderul resurselor - daca nu este
            if (useFullPathToProjectOption == false)
            {
                if (Disk.CreateFolder(Path.Combine(Settings.BuildFolder, parameters["user"] , parameters["project"], "Resources", "Output"), EC) == false)
                {
                    Error("Unable to create resource folder ...");
                    ShowErrorContainer(EC);
                    return false;
                }
            }
            if (quick)
            {
                // builduiesc resursele
                if (BuildAndLoadResources(prj, task, false) == false)
                    return false;
            }
            else
            {
                // iconitele
                if (BuildIcons(prj, task) == false)
                    return false;
                // builduiesc resursele
                if (BuildAndLoadResources(prj, task,true) == false)
                    return false;
            }
            // compilez codul
            bool u = false;
            try
            {
                if (GACParser.LoadGacDefinitions(parameters["gacxml"]) == false)
                {
                    Error("Unable to load gac definitions: " + parameters["gacxml"] + "\n");
                    return false;
                }
            }
            catch (Exception e)
            {
                Error(e.ToString());
                return false;
            }            
            GACParser.UpdateGlobalAutoComplete(prj, bld);            
            GACParser.CreatePreprocessAutoCompleteList();
            if (prj.CheckAnimationsIntegrity() == false)
            {
                prj.ShowErrors();
                return false;
            }
            if (GAC2CPPConvertor.Convert(prj, task, false, ref u,bld,true,false) == false)
            {
                Error("Unable compile gac sources ...\n");
                prj.ShowErrors();
                return false;
            }
            // am si un build - il fac
            task.CreateSubTask("Prepare");
            bld.Prepare(prj, task);
            bld.SetBuildExtension(GetBuildExtension(bld));
            if (parameters.ContainsKey("out"))
            {
                if (File.Exists(parameters["out"]))
                {
                    if (Disk.DeleteFile(parameters["out"],EC)==false)
                    {
                        ShowErrorContainer(EC);
                        return false;
                    }
                }
                bld.GetBuildExtension().SetOutputFile(parameters["out"]);
            }
            bld.Build();
            if (prj.EC.HasErrors())
            {
                prj.ShowErrors();
                return false;
            }
            return true;
        }
        static bool CleanUpWorkingProject()
        {
            //return true;
            if (!CheckParameter("project", ""))
                return false;
            if (!CheckParameter("user", ""))
                return false;
            if (parameters.ContainsKey("skipcleanup"))
                return true;
            string path = Path.Combine(Settings.BuildFolder, parameters["user"] , parameters["project"]);
            if (Disk.CleanDirectory(path, EC) == false)
            {
                ShowErrorContainer(EC);
                return false;
            }
            return true;
        }
        static bool PackFile()
        {
            if (!CheckParameter("pass", ""))
                return false;
            if (!CheckParameter("in", ""))
                return false;
            if (!CheckParameter("out", ""))
                return false;
            GZipPackage p = new GZipPackage();
            ErrorsContainer ec = new ErrorsContainer();
            if (p.Add(parameters["in"], Path.GetFileName(parameters["in"]), ec) == false)
            {
                ShowErrorContainer(ec);
                return false;
            }
            byte[] res = p.Compress(parameters["pass"], ec);
            if (res == null)
            {
                ShowErrorContainer(ec);
                return false;
            }
            if (Disk.SaveFile(parameters["out"], res, ec) == false)
            {
                ShowErrorContainer(ec);
                return false;
            }
            return true;
        }
        static bool CreateProjectHashListFile()
        {
            if (!CheckParameter("out", ""))
                return false;
            Project prj = GetProject();
            if (prj == null)
                return false;
            string buildName = GetBuildName();
            // daca nu am acel build
            if (prj.GetBuild(buildName)==null)
            {

            }
            GZipPackage zp = prj.CreateProjectPackage(parameters.ContainsKey("includebuildresources"), GetBuildName(),false);
            if (zp == null)
            {
                Error("Unable to create package for : '" + parameters["project"] + "' (1)\n");
                prj.ShowErrors();
                return false;
            }
            if (parameters.ContainsKey("pass") == false)
            {
                if (zp.ExportHashList(parameters["out"], prj.EC) == false)
                {
                    Error("Unable to export hash file list for : '" + parameters["project"] + "'\n");
                    prj.ShowErrors();
                    return false;
                }
            }
            else
            {
                if (zp.ExportHashList(parameters["out"] + ".unpacked", prj.EC) == false)
                {
                    Error("Unable to export hash file list for : '" + parameters["project"] + "'\n");
                    prj.ShowErrors();
                    return false;
                }
                GZipPackage zp2 = new GZipPackage();
                if (zp2.Add(parameters["out"] + ".unpacked", Path.GetFileName(parameters["out"]), prj.EC) == false)
                {
                    Error("Unable to add unpack file " + Path.GetFileName(parameters["out"]) + "\n");
                    prj.ShowErrors();
                    return false;
                }
                byte[] res = zp2.Compress(parameters["pass"], prj.EC);
                if (res == null)
                {
                    Error("Unable to create pack file " + Path.GetFileName(parameters["out"]) + "\n");
                    prj.ShowErrors();
                    return false;
                }
                if (Disk.SaveFile(parameters["out"], res, prj.EC) == false)
                {
                    Error("Unable to save pack file " + Path.GetFileName(parameters["out"]) + "\n");
                    prj.ShowErrors();
                    return false;
                }
            }
            return true;
        }
        static bool CheckIfUserExists(string name)
        {
            if (Directory.Exists(Path.Combine(Settings.RepoFolder, name)) == false)
            {
                Error("User '" + name + "' is not registered as a valid user !");
                return false;
            }
            return true;
        }
        static bool CreateGACCreatorUpdatePack()
        {
            if (!CheckParameter("pass", ""))
                return false;
            if (!CheckParameter("out", ""))
                return false;
            if (!CheckParameter("gacfolder", ""))
                return false;
            GZipPackage zp = new GZipPackage();
            if (zp.AddFolder(Path.Combine(parameters["gacfolder"], "Libs"), "Libs", null, EC) == false)
            {
                ShowErrorContainer(EC);
                return false;
            }
            if (zp.AddFolder(Path.Combine(parameters["gacfolder"], "Template"), "Template", null, EC) == false)
            {
                ShowErrorContainer(EC);
                return false;
            }
            string[] files = { "GAppCreator.exe.gacxml","GAppCreator.exe","ScintillaNET.dll","SharpGL.dll","SharpGL.SceneGraph.dll","SharpGL.WinForms.dll","SciLexer.dll","GacLibrary.dll","Updater.exe" };
            foreach (string fname in files)
            {
                if (zp.Add(Path.Combine(parameters["gacfolder"], fname),fname,EC)==false)
                {
                    ShowErrorContainer(EC);
                    return false;
                }
            }
            if (zp.Compress(parameters["out"], parameters["pass"], EC) == false)
            {
                ShowErrorContainer(EC);
                return false;
            }            
            return true;
        }
        static bool UpdateProject()
        {
            if (!CheckParameter("pass", ""))
                return false;
            if (!CheckParameter("user", ""))
                return false;
            if (!CheckParameter("project", ""))
                return false;
            if (!CheckParameter("updatepack", ""))
                return false;
            if (CheckIfUserExists(parameters["user"])==false)
                return false;
            // fac folderele proiectului
            string prjFolder = Path.Combine(Settings.RepoFolder,parameters["user"],parameters["project"],"Current");
            if (Disk.CreateFolder(prjFolder,EC)==false)
            {
                ShowErrorContainer(EC);
                return false;
            }
            // despachetez
            GZipPackage gzp = new GZipPackage();
            if (gzp.Uncompress(parameters["pass"], parameters["updatepack"], prjFolder, EC) == false)
            {
                ShowErrorContainer(EC);
                return false;
            }            
            return true;
        }
        static bool test()
        {
            GZipPackage p = new GZipPackage();
            p.AddFolder(@"E:\a\rm\Untangle\Sources", "Sources", "gac", null);
            p.AddFolder(@"E:\a\rm\Untangle\Resources\Sources", "Resources\\Sources", null, null);
            p.Add(@"E:\a\rm\Untangle\project.gappcreator", "project.gappcreator", null);
            //p.ExportHashList(@"E:\Imagini\Share\collab\test\myRep\res.gzip.md5", null);
            Console.WriteLine("Total files: " + p.GetFilesCount().ToString());
            //p.FilterFileList(@"E:\Imagini\Share\collab\test\myRep\res.gzip.md5", null);
            //Console.WriteLine("Filtered files: " + p.GetFilesCount().ToString());
            byte[] b = p.Compress("pass", null);
            Disk.SaveFile(@"E:\Imagini\Share\collab\test\myRep\untangle.gzip", b, null);
            return true;
        }
        static bool test2()
        {
            GZipPackage p = new GZipPackage();
            p.Uncompress("pass2", @"E:\Imagini\Share\collab\test\myRep\res.gzip", @"E:\Imagini\Share\collab\test\myRep\tmp", null);
            return true;
        }
        static void Main(string[] args)
        {
            //if (test())
            //    return;
            if (args.Length == 0)
            {
                Help();
                return;
            }
            bool result = false;
            parameters = new Dictionary<string, string>();
            foreach (string a in args)
            {
                if (a.StartsWith("-"))
                {
                    string k = "", v = "";
                    if (a.Contains(':'))
                    {
                        k = a.Substring(1, a.IndexOf(':') - 1).ToLower();
                        v = a.Substring(a.IndexOf(':') + 1);
                    }
                    else
                    {
                        k = a.Substring(1).ToLower();
                    }
                    parameters[k] = v;
                }
            }
            if (parameters.ContainsKey("log"))
            {
                try
                {
                    logFile = File.Open(parameters["log"], FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
                }
                catch (Exception e)
                {
                    logFile = null;
                    Error("Unable to open " + parameters["log"] + " for writing\n" + e.ToString());
                }
            }
            // incarc si testez setarile
            Settings = CreateSystemSettings();
            if (Settings == null)
                return;
            switch (args[0])
            {
                case "build":           
                    result = BuildApp(false); 
                    if (parameters.ContainsKey("projectpath") == false) 
                        result &= CleanUpWorkingProject(); 
                    break;
                case "quickbuild":      
                    result = BuildApp(true);
                    if (parameters.ContainsKey("projectpath") == false) 
                        result &= CleanUpWorkingProject(); 
                    break;
                case "packfile":        
                    result = PackFile(); 
                    break;
                case "hashlist":        
                    result = CreateProjectHashListFile(); 
                    break;
                case "updateproject":   
                    result = UpdateProject(); 
                    break;
                case "clean":           
                    result = CleanUpWorkingProject(); 
                    break;
                case "createupdatepack":
                    result = CreateGACCreatorUpdatePack();
                    break;
                default:
                    Error("Unknwon commmand: '" + args[0] + "'");
                    break;
            }
            if (result)
            {
                Console.WriteLine("[EXECUTION-OK]");
                Log("O:\n");
            } else {
                Log("X:\n");
            }
            if (logFile != null)
                logFile.Close();
        }
    }
}
