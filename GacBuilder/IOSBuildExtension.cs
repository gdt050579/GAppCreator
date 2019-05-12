using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GAppCreator;
using System.IO;

namespace GAppCreator
{
    public class IOSBuildExtension : BuildExtension
    {
        private static int XCodeHashID = 0;
        private bool PrepareFolders()
        {
            task.CreateSubTask("Creating folders ...");
            while (true)
            {
                if (Disk.CreateFolder(root, prj.EC) == false)
                    break;
                if (Disk.CreateFolder(Path.Combine(root, prj.GetProjectName()), prj.EC) == false)
                    break;
                if (Disk.CreateFolder(Path.Combine(root, prj.GetProjectName(),"Assets.xcassets"), prj.EC) == false)
                    break;
                if (Disk.CreateFolder(Path.Combine(root, prj.GetProjectName(), "Assets.xcassets","AppIcon.appiconset"), prj.EC) == false)
                    break;                
                if (Disk.CreateFolder(Path.Combine(root, prj.GetProjectName()+".xcodeproj"), prj.EC) == false)
                    break;
                if (Disk.SaveFile(Path.Combine(root, prj.GetProjectName(), "Assets.xcassets", "Contents.json"), "{\"info\" : {\"version\" : 1,\"author\" : \"xcode\"} }", prj.EC) == false)
                    break;
                task.UpdateSuccessErrorState(true);
                return true;
            }
            task.UpdateSuccessErrorState(false);
            return false;
        }
        private bool CopyIcon(int size, Project.ProjectIcons Icons)
        {
            if (Icons.Sizes.ContainsKey(size) == false)
            {
                prj.EC.AddError(String.Format("No icon defined for {0}x{0} size !", size));
                return false;
            }
            if (Disk.Copy(Icons.Sizes[size], Path.Combine(root, prj.GetProjectName(), "Assets.xcassets", "AppIcon.appiconset", String.Format("AppIcon_{0}x{0}.png", size)), prj.EC) == false)
                return false;
            return true;
        }
        private bool CreateIcons()
        {
            Project.ProjectIcons Icons = prj.GetBuildIcons(this.Build);
            foreach (int size in Icons.Sizes.Keys)
                if (CopyIcon(size, Icons) == false)
                    return false;
            // creez si fisierul cu iconite - json-ul
            if (Project.CreateResource("iOS", "IconsContents.json", null, Path.Combine(root, prj.GetProjectName(), "Assets.xcassets", "AppIcon.appiconset", "Contents.json"), prj.EC) == false)
                return false;
            return true;
        }
        private bool GenerateCppFiles()
        {
            task.CreateSubTask("Updating source files ...");
            string cpp_list = "main.mm AppDelegate.mm ViewController.mm IOSInterface.cpp ";
            // copii fisierele din framework
            if (CopyDefaultCPPFiles(Sources.FrameworkSources, Path.Combine(root, prj.GetProjectName()), ref cpp_list) == false)
                return false;
            // IOSInterface files
            Dictionary<string, string> d = new Dictionary<string, string>();
            if (prj.UpdateReplaceDictionary(d,Build) == false)
                return false;
            Build.UpdateReplaceDictionaryWithSocialMedia(d, prj);
            int nrSounds = 0;
            foreach (GenericResource r in Build.R.List)
            {
                if (r.GetType() == typeof(SoundResource))
                    nrSounds++;
            }
            d["$$NUMBER_OF_SOUNDS$$"] = nrSounds.ToString();

            if (Project.CreateResource("iOS", "IOSInterface.h", d, Path.Combine(root, prj.GetProjectName(), "IOSInterface.h"), prj.EC) == false)
                return false;
            if (Project.CreateResource("iOS", "IOSInterface.cpp", d, Path.Combine(root, prj.GetProjectName(), "IOSInterface.cpp"), prj.EC) == false)
                return false;

            if (Project.CreateResource("iOS", "AppDelegate.h", d, Path.Combine(root, prj.GetProjectName(), "AppDelegate.h"), prj.EC) == false)
                return false;
            if (Project.CreateResource("iOS", "AppDelegate.mm", d, Path.Combine(root, prj.GetProjectName(), "AppDelegate.mm"), prj.EC) == false)
                return false;

            if (Project.CreateResource("iOS", "ViewController.h", d, Path.Combine(root, prj.GetProjectName(), "ViewController.h"), prj.EC) == false)
                return false;
            if (Project.CreateResource("iOS", "ViewController.mm", d, Path.Combine(root, prj.GetProjectName(), "ViewController.mm"), prj.EC) == false)
                return false;

            if (Project.CreateResource("iOS", "main.mm", d, Path.Combine(root, prj.GetProjectName(), "main.mm"), prj.EC) == false)
                return false;

            // fac si un makefile
            d["$$IOS_SOURCES$$"] = cpp_list;
            d["$$CPP-DEFINES$$"] = "-DPLATFORM_IOS ";
            if (Build.EnableErrorLogging)
                d["$$CPP-DEFINES$$"] += "-DENABLE_ERROR_LOGGING ";
            if (Build.EnableInfoLogging)
                d["$$CPP-DEFINES$$"] += "-DENABLE_INFO_LOGGING ";
            if (Build.EnableEventLogging)
                d["$$CPP-DEFINES$$"] += "-DENABLE_EVENT_LOGGING ";

            if (Project.CreateResource("IOS","makefile", d, Path.Combine(root, "makefile"), prj.EC) == false)
                return false;

            task.UpdateSuccessErrorState(true);
            return true;
        }
        private bool CreatePListInfo()
        {
            Dictionary<string, string> d = new Dictionary<string, string>()
            {
                {"$$APP.NAME$$",prj.GetApplicationName(Language.English,Build)},
                {"$$VERSION$$",prj.Version},
                {"$$PROJECTNAME$$",prj.GetProjectName()},                
            };
            if (((IOSBuildConfiguration)Build).Orientation == ScreenOrientation.Portrait)
                d["$$ORIENTATION$$"]="Portrait";
            else
                d["$$ORIENTATION$$"]="LandscapeLeft";
            if (Project.CreateResource("IOS", "Info.plist", d, Path.Combine(root, prj.GetProjectName(), "Info.plist"), prj.EC) == false)
                return false;

            

            return true;
        }
        private bool CopySoundFiles()
        {
            task.CreateSubTask("Creating sounds ...");
            foreach (GenericResource r in Build.R.List)
            {
                if (r.GetType() == typeof(SoundResource))
                {
                    string s = Path.Combine(root, prj.GetProjectName(), "sound_" + r.GetResourceIndex().ToString() + Path.GetExtension(r.Source));
                    if (Disk.Copy(r.GetSourceFullPath(), s, prj.EC) == false)
                        return task.UpdateSuccessErrorState(false);
                }
            }
            return task.UpdateSuccessErrorState(true);
        }
        private string GetXCodeUniqueHash()
        {
            string s = string.Format("{0:X08}{1:X08}{2:X08}",System.Environment.TickCount,XCodeHashID,System.Environment.TickCount);
            XCodeHashID++;
            return s;
        }
        private bool CreateXCodeProjectFfile()
        {
            task.CreateSubTask("Creating XCode project file ...");
            Dictionary<string, string> d = new Dictionary<string, string>();
            // fill in d
            XCodeHashID = 0;

            string file_refs = "\n";
            string file_info = "\n";
            string source_file_refs = "\n";
            string resource_file_ref = "\n";
            string project_file_list = "\n";
            foreach (string fullFilePath in Directory.EnumerateFiles(Path.Combine(root,prj.GetProjectName())))
            {
                string h1 = GetXCodeUniqueHash();
                string h2 = GetXCodeUniqueHash();
                string fname = Path.GetFileName(fullFilePath);

                if (fname.ToLower().EndsWith(".cpp")) 
                {
                    file_refs += "\t\t" + h1 + " = {isa = PBXBuildFile; fileRef = " + h2 + "; };\n";
                    file_info += "\t\t" + h2 + " = {isa = PBXFileReference; fileEncoding = 4; lastKnownFileType = sourcecode.cpp.cpp; path = " + fname + "; sourceTree = \"<group>\"; };\n";
                    source_file_refs += "\t\t" + h1 + ",\n";
                    project_file_list += "\t\t" + h2 + ",\n";
                    continue;
                }
                if (fname.ToLower().EndsWith(".h"))
                {
                    file_refs += "\t\t" + h1 + " = {isa = PBXBuildFile; fileRef = " + h2 + "; };\n";
                    file_info += "\t\t" + h2 + " = {isa = PBXFileReference; fileEncoding = 4; lastKnownFileType = sourcecode.cpp.h; path = " + fname + "; sourceTree = \"<group>\"; };\n";
                    source_file_refs += "\t\t" + h1 + ",\n";
                    project_file_list += "\t\t" + h2 + ",\n";
                    continue;
                }
                if (fname.ToLower().EndsWith(".mm"))
                {
                    file_refs += "\t\t" + h1 + " = {isa = PBXBuildFile; fileRef = " + h2 + "; };\n";
                    file_info += "\t\t" + h2 + " = {isa = PBXFileReference; fileEncoding = 4; lastKnownFileType = sourcecode.cpp.objc; path = " + fname + "; sourceTree = \"<group>\"; };\n";
                    source_file_refs += "\t\t" + h1 + ",\n";
                    project_file_list += "\t\t" + h2 + ",\n";
                    continue;
                }
            }
            // adaug sunetele
            foreach (GenericResource r in Build.R.List)
            {
                if (r.GetType() == typeof(SoundResource))
                {
                    string fname = "sound_" + r.GetResourceIndex().ToString() + Path.GetExtension(r.Source);
                    string h1 = GetXCodeUniqueHash();
                    string h2 = GetXCodeUniqueHash();
                    file_refs += "\t\t" + h1 + " = {isa = PBXBuildFile; fileRef = " + h2 + "; };\n";
                    file_info += "\t\t" + h2 + " = {isa = PBXFileReference; lastKnownFileType = file; path = " + fname + "; sourceTree = \"<group>\"; };\n";
                    resource_file_ref += "\t\t" + h1 + ",\n";
                    project_file_list += "\t\t" + h2 + ",\n";
                }
            }

            d["$$FILE_REFS$$"] = file_refs;
            d["$$FILE_INFO$$"] = file_info;
            d["$$SOURCE_FILE_REFS$$"] = source_file_refs;
            d["$$RESOURCE_FILE_REF$$"] = resource_file_ref;
            d["$$PROJECT_FILE_LIST$$"] = project_file_list;
            d["$$PROJECT.NAME$$"] = prj.GetProjectName();

            if (Project.CreateResource("IOS", "project.pbxproj", d, Path.Combine(root, prj.GetProjectName()+".xcodeproj", "project.pbxproj"), prj.EC) == false)
                return false;
            return task.UpdateSuccessErrorState(true);
        }
        public override void OnBuild()
        {
            if (Disk.CleanDirectory(root, prj.EC) == false)
                return;
            if (PrepareFolders() == false)
                return;
            if (Build.GenerateResources(Path.Combine(root, prj.GetProjectName(), "resources.dat"), true) == false)
                return;
            if (Build.GenerateResourceCodeFiles() == false)
                return;
            if (GenerateCppFiles() == false)
                return;
            if (CreateIcons() == false)
                return;
            if (CopySoundFiles() == false)
                return;
            if (CreatePListInfo() == false)
                return;
            if (CreateXCodeProjectFfile() == false)
                return;
            /*
            if (CreateStringsXML() == false)
                return;
            if (CreateLayouts() == false)
                return;
            if (GenerateCppFiles() == false)
                return;
            if (GenerateJavaFiles() == false)
                return;
            if (CopyExtraLibraries() == false)
                return;
            if (CreateExtraFiles() == false)
                return;
            if (CreateRFile() == false)
                return;
            if (CompileJavaCode() == false)
                return;
            if (CreateDexFile() == false)
                return;
            if (BuildCppFiles() == false)
                return;
            if (CreateUnsignedAPK() == false)
                return;
            if (CreateSignedAPK() == false)
                return;
            if (ZipAlignAPK() == false)
                return;
             */
        }
    }
}
