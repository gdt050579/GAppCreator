using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GAppCreator;
using System.IO;


namespace GAppCreator
{
    public class MacBuildExtension: BuildExtension
    {
        private bool PrepareFolders()
        {
            task.CreateSubTask("Creating folders ...");
            while (true)
            {
                if (Disk.CreateFolder(root, prj.EC) == false)
                    break;
                if (Disk.CreateFolder(Path.Combine(root, "zip"), prj.EC) == false)
                    break;
                if (Disk.CreateFolder(Path.Combine(root, "sources"), prj.EC) == false)
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
            if (Disk.Copy(Icons.Sizes[size], Path.Combine(root, "zip", String.Format("Icon{0}x{0}.png", size)), prj.EC) == false)
                return false;
            return true;
        }
        private bool CreateIcons()
        {
            Project.ProjectIcons Icons = prj.GetBuildIcons(this.Build);
            if (CopyIcon(57, Icons) == false)
                return false;
            if (CopyIcon(114, Icons) == false)
                return false;

            return true;
        }
        private bool GenerateCppFiles()
        {
            task.CreateSubTask("Updateing source files ...");
            string cpp_list = "main.mm OpenGLView.mm OpenGLWindow.mm IOSInterface.cpp ";
            // copii fisierele din framework
            if (CopyDefaultCPPFiles(Sources.FrameworkSources, Path.Combine(root, "sources"), ref cpp_list) == false)
                return false;
            // IOSInterface files
            Dictionary<string, string> d = new Dictionary<string, string>();
            if (prj.UpdateReplaceDictionary(d,Build) == false)
                return false;
            Build.UpdateReplaceDictionaryWithSocialMedia(d, prj);

            int w=0, h=0;
            if (Project.SizeToValues(((MacBuildConfiguration)Build).WindowSize,ref w,ref h)==false)
            {
                prj.EC.AddError("Window size not set or invalid");
                return false;
            }
            d["$$WINDOW.WIDTH$$"] = w.ToString();
            d["$$WINDOW.HEIGHT$$"] = h.ToString();
            d["$$WINDOW.TITLE$$"] = prj.GetProjectName();

            // il iau din IOS - nu are sens sa mentin o copie duplicata si in MAC
            if (Project.CreateResource("iOS", "IOSInterface.h", d, Path.Combine(root, "sources", "IOSInterface.h"), prj.EC) == false)
                return false;
            if (Project.CreateResource("iOS", "IOSInterface.cpp", d, Path.Combine(root, "sources", "IOSInterface.cpp"), prj.EC) == false)
                return false;

            // se iau specific din proiect
            if (Project.CreateResource("Mac", "OpenGLView.h", d, Path.Combine(root, "sources", "OpenGLView.h"), prj.EC) == false)
                return false;
            if (Project.CreateResource("Mac", "OpenGLView.mm", d, Path.Combine(root, "sources", "OpenGLView.mm"), prj.EC) == false)
                return false;

            if (Project.CreateResource("Mac", "OpenGLWindow.h", d, Path.Combine(root, "sources", "OpenGLWindow.h"), prj.EC) == false)
                return false;
            if (Project.CreateResource("Mac", "OpenGLWindow.mm", d, Path.Combine(root, "sources", "OpenGLWindow.mm"), prj.EC) == false)
                return false;

            if (Project.CreateResource("Mac ", "main.mm", d, Path.Combine(root, "sources", "main.mm"), prj.EC) == false)
                return false;

            // fac si un makefile
            d["$$MAC-SOURCES$$"] = cpp_list;
            d["$$CPP-DEFINES$$"] = "-DPLATFORM_MAC ";
            if (Build.EnableErrorLogging)
                d["$$CPP-DEFINES$$"] += "-DENABLE_ERROR_LOGGING ";
            if (Build.EnableInfoLogging)
                d["$$CPP-DEFINES$$"] += "-DENABLE_INFO_LOGGING ";
            if (Build.EnableEventLogging)
                d["$$CPP-DEFINES$$"] += "-DENABLE_EVENT_LOGGING ";

            if (Project.CreateResource("Mac", "makefile", d, Path.Combine(root, "sources", "makefile"), prj.EC) == false)
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
                d["$$ORIENTATION$$"] = "Portrait";
            else
                d["$$ORIENTATION$$"] = "LandscapeLeft";
            if (Project.CreateResource("IOS", "Info.plist", d, Path.Combine(root, "zip", "Info.plist"), prj.EC) == false)
                return false;



            return true;
        }
        public override void OnBuild()
        {
            if (Disk.CleanDirectory(root, prj.EC) == false)
                return;
            if (PrepareFolders() == false)
                return;
            if (Build.GenerateResources(Path.Combine(root, "zip", "resources.dat"), true) == false)
                return;
            if (Build.GenerateResourceCodeFiles() == false)
                return;
            if (GenerateCppFiles() == false)
                return;
            //if (CreateIcons() == false)
            //    return;
            //if (CreatePListInfo() == false)
            //    return;
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
