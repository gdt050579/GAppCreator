using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAppCreator
{
    public class WindowsDesktopBuildExtension : BuildExtension
    {
        private readonly long[] HashTable = new long[HSIZE];

        private const uint HLOG = 14;
        private const uint HSIZE = (1 << 14);
        private const uint MAX_LIT = (1 << 5);
        private const uint MAX_OFF = (1 << 13);
        private const uint MAX_REF = ((1 << 8) + (1 << 3));

        public int CompressBuffer(byte[] input, int inputLength, byte[] output, int outputLength)
        {
            Array.Clear(HashTable, 0, (int)HSIZE);

            long hslot;
            uint iidx = 0;
            uint oidx = 0;
            long reference;

            uint hval = (uint)(((input[iidx]) << 8) | input[iidx + 1]); // FRST(in_data, iidx);
            long off;
            int lit = 0;

            for (; ; )
            {
                if (iidx < inputLength - 2)
                {
                    hval = (hval << 8) | input[iidx + 2];
                    hslot = ((hval ^ (hval << 5)) >> (int)(((3 * 8 - HLOG)) - hval * 5) & (HSIZE - 1));
                    reference = HashTable[hslot];
                    HashTable[hslot] = (long)iidx;


                    if ((off = iidx - reference - 1) < MAX_OFF
                        && iidx + 4 < inputLength
                        && reference > 0
                        && input[reference + 0] == input[iidx + 0]
                        && input[reference + 1] == input[iidx + 1]
                        && input[reference + 2] == input[iidx + 2]
                        )
                    {
                        /* match found at *reference++ */
                        uint len = 2;
                        uint maxlen = (uint)inputLength - iidx - len;
                        maxlen = maxlen > MAX_REF ? MAX_REF : maxlen;

                        if (oidx + lit + 1 + 3 >= outputLength)
                            return 0;

                        do
                            len++;
                        while (len < maxlen && input[reference + len] == input[iidx + len]);

                        if (lit != 0)
                        {
                            output[oidx++] = (byte)(lit - 1);
                            lit = -lit;
                            do
                                output[oidx++] = input[iidx + lit];
                            while ((++lit) != 0);
                        }

                        len -= 2;
                        iidx++;

                        if (len < 7)
                        {
                            output[oidx++] = (byte)((off >> 8) + (len << 5));
                        }
                        else
                        {
                            output[oidx++] = (byte)((off >> 8) + (7 << 5));
                            output[oidx++] = (byte)(len - 7);
                        }

                        output[oidx++] = (byte)off;

                        iidx += len - 1;
                        hval = (uint)(((input[iidx]) << 8) | input[iidx + 1]);

                        hval = (hval << 8) | input[iidx + 2];
                        HashTable[((hval ^ (hval << 5)) >> (int)(((3 * 8 - HLOG)) - hval * 5) & (HSIZE - 1))] = iidx;
                        iidx++;

                        hval = (hval << 8) | input[iidx + 2];
                        HashTable[((hval ^ (hval << 5)) >> (int)(((3 * 8 - HLOG)) - hval * 5) & (HSIZE - 1))] = iidx;
                        iidx++;
                        continue;
                    }
                }
                else if (iidx == inputLength)
                    break;

                /* one more literal byte we must copy */
                lit++;
                iidx++;

                if (lit == MAX_LIT)
                {
                    if (oidx + 1 + MAX_LIT >= outputLength)
                        return 0;

                    output[oidx++] = (byte)(MAX_LIT - 1);
                    lit = -lit;
                    do
                        output[oidx++] = input[iidx + lit];
                    while ((++lit) != 0);
                }
            }

            if (lit != 0)
            {
                if (oidx + lit + 1 >= outputLength)
                    return 0;

                output[oidx++] = (byte)(lit - 1);
                lit = -lit;
                do
                    output[oidx++] = input[iidx + lit];
                while ((++lit) != 0);
            }

            return (int)oidx;
        }

        private bool CompressResources()
        {
            task.CreateSubTask("Compressing resources ...");
            byte[] b = Disk.ReadFile(Path.Combine(root, "resources.dat"), prj.EC);
            if (b==null)
                return task.UpdateSuccessErrorState(false);
            byte[] output = new byte[b.Length+256];
            int newSize = CompressBuffer(b, b.Length, output, b.Length + 256);
            if ((newSize<0) || ((newSize==0) && (b.Length>0)))
            {
                prj.EC.AddError("Failed to compressed resources of size " + b.Length.ToString() + " bytes");
                return task.UpdateSuccessErrorState(false);
            }
            Array.Resize<byte>(ref output, newSize);
            if (Disk.SaveFile(Path.Combine(root,"resources.compressed"),output,prj.EC)==false)
                return task.UpdateSuccessErrorState(false);
            return task.UpdateSuccessErrorState(true);
        }
        private bool GenerateAndCompile()
        {
            task.CreateSubTask("Compiling source files ...");
            WindowsDesktopBuildConfiguration Config = (WindowsDesktopBuildConfiguration)Build;
            string cpp_list = "main.cpp ";
            // copii fisierele din framework
            if (CopyDefaultCPPFiles(Sources.FrameworkSources, root, ref cpp_list) == false)
                return task.UpdateSuccessErrorState(false);
            // WinOPENFGl
            if (Disk.Copy(Path.Combine(prj.Settings.FrameworkSourcesPath, "WinOpenGL.h"), Path.Combine(root, "WinOpenGL.h"), prj.EC) == false)
                return task.UpdateSuccessErrorState(false);
            // main file
            Dictionary<string, string> d = new Dictionary<string, string>();
            if (prj.UpdateReplaceDictionary(d, Build) == false)
                return task.UpdateSuccessErrorState(false);
            Build.UpdateReplaceDictionaryWithSocialMedia(d, prj);

            // resursele
            byte[] res = Disk.ReadFile(Path.Combine(root, "resources.compressed"), prj.EC);
            if (res==null)
                return task.UpdateSuccessErrorState(false);
            StringBuilder sb = new StringBuilder(res.Length * 6);
            for (int tr = 0; tr < res.Length;tr++)
            {
                if ((tr > 0) && (tr % 20 == 0))
                    sb.Append("\n");
                sb.Append("0x");
                sb.Append(res[tr].ToString("X2"));
                sb.Append(",");
            }
            
            long sz = Disk.GetFileSize(Path.Combine(root,"resources.dat"),prj.EC);
            if (prj.EC.HasErrors())
                return task.UpdateSuccessErrorState(false);
            d["$$COMPRESSED.RESOURCES$$"] = sb.ToString();
            d["$$UNCOMPRESSED.SIZE$$"] = sz.ToString();

            int w = 0, h = 0;
            if (Project.SizeToValues(Config.AppResolution,ref w,ref h)==false)
            {
                prj.EC.AddError("Invalid resolution: " + Config.AppResolution);
                return task.UpdateSuccessErrorState(false);
            }
            d["$$RESOLUTION.HEIGHT$$"] = h.ToString();
            d["$$RESOLUTION.WIDTH$$"] = w.ToString();


            if (Project.CreateResource("WindowsDesktop", "main.cpp", d, Path.Combine(root, "main.cpp"), prj.EC) == false)
                return task.UpdateSuccessErrorState(false);

            string parameters = " -DPLATFORM_WINDOWS_DESKTOP";
            bool hasDebugInfo = false;
            

            if (Config.EnableErrorLogging)
                parameters += " -DENABLE_ERROR_LOGGING";
            if (Config.EnableEventLogging)
                parameters += " -DENABLE_EVENT_LOGGING";
            if (Config.EnableInfoLogging)
                parameters += " -DENABLE_INFO_LOGGING";

            switch (Config.OptimizationMode)
            {
                case CompilerOptimizationMode.None:
                    parameters += " -Od";
                    break;
                case CompilerOptimizationMode.Debug:
                    parameters += " -Od";
                    hasDebugInfo = true;
                    break;
                case CompilerOptimizationMode.Normal:
                    parameters += " -O1";
                    break;
                case CompilerOptimizationMode.Optimize:
                    parameters += " -O2";
                    break;
                case CompilerOptimizationMode.HighlyOptimize:
                    parameters += " -Ox";
                    break;
            }

            parameters += " " + Config.CompilerParams;

            parameters += " /I\""+Path.Combine(GetToolsFolder("cl"),"Include")+"\"";
            parameters += " /I\"" + Path.Combine(GetToolsFolder("cl"), "Windows") + "\"";
            parameters += " /I\"" + Path.Combine(GetToolsFolder("cl"), "shared") + "\"";

            parameters += " " + cpp_list;
            parameters += " /link ";
            parameters += " \"" + Path.Combine(GetToolsFolder("cl"), "Lib", "kernel32.lib")+"\"";
            parameters += " \"" + Path.Combine(GetToolsFolder("cl"), "Lib", "advapi32.lib") + "\"";
            parameters += " \"" + Path.Combine(GetToolsFolder("cl"), "Lib", "user32.lib") + "\"";
            parameters += " \"" + Path.Combine(GetToolsFolder("cl"), "Lib", "gdi32.lib") + "\"";
            parameters += " \"" + Path.Combine(GetToolsFolder("cl"), "Lib", "uuid.lib") + "\"";
            parameters += " \"" + Path.Combine(GetToolsFolder("cl"), "Lib", "libcmt.lib") + "\"";
            parameters += " \"" + Path.Combine(GetToolsFolder("cl"), "Lib", "oldnames.lib") + "\"";
            parameters += " \"" + Path.Combine(GetToolsFolder("cl"), "Lib", "shell32.lib") + "\"";
            parameters += " /OUT:\"" + Path.Combine(root, "application.exe") + "\""; 
            parameters += " /SUBSYSTEM:WINDOWS\",5.01\"";
            parameters += " /VERSION:0.0";
            if (hasDebugInfo) parameters += " /DEBUG";
            parameters += " \""+Path.Combine(root,"resources.res")+"\"";


            task.Info("CPP List: " + cpp_list);
            string batFile = "@cd \"" + root + "\"\n";
            batFile += root[0] + ":\n";
            batFile += "@del /Q *.obj \n";
            batFile += "@\""+GetTool("cl") + "\" " + parameters + " >\""+Path.Combine(root,"compile.log")+"\"\n";
            if (Disk.SaveFile(Path.Combine(root, "compile.bat"), batFile, prj.EC) == false)
                return task.UpdateSuccessErrorState(false);
            if (prj.RunCommand("C:\\Windows\\System32\\cmd.exe", "/c \"" + Path.Combine(root, "compile.bat") + "\"", root,true,true)==false)
            {
                prj.EC.AddError("Unable to run compile.bat file !");
                return task.UpdateSuccessErrorState(false);
            }

            if (File.Exists(Path.Combine(root,"application.exe")) == false)
            {
                string result = Disk.ReadFileAsString(Path.Combine(root,"compile.log"),prj.EC);
                if (result == null)
                    result = "";
                prj.EC.AddError("Application was not created - abording !\n" + result);
                return task.UpdateSuccessErrorState(false); 
            }

            return task.UpdateSuccessErrorState(true);
        }
        private bool GenerateIcon()
        {
            task.CreateSubTask("Creating icons ...");
            string icoPath = Path.Combine(root, "project.ico");
            string parameters = "\"" + icoPath + "\" ";

            Project.ProjectIcons Icons = prj.GetBuildIcons(this.Build);
            foreach (int size in Build.GetIconSizes())
            {

                if (Icons.Sizes.ContainsKey(size) == false)
                {
                    prj.EC.AddError(String.Format("No icon defined for {0}x{0} size !", size));
                    return false;
                }
                parameters += "\"" + Icons.Sizes[size] + "\" ";
            }
            // am toate iconitele
            string result = prj.ShellExecute(GetTool("png2ico"), parameters, "");
            if (File.Exists(icoPath)==false)
            {
                prj.EC.AddError("Icon file was not created - abording !\n"+result);
                return false;
            }
            task.UpdateSuccessErrorState(true);
            return true;
        }
        private bool GenerateResourceSection()
        {
            task.CreateSubTask("Creating resource section ...");
            Dictionary<string, string> d = new Dictionary<string, string>()
            {
                {"$$COMPANY$$",prj.Company},
                {"$$DESCRIPTION$$",prj.Description},
                {"$$COPYRIGHT$$",prj.Copyright},
                {"$$FILENAME$$",prj.GetProjectName()+".exe"},
                {"$$PROJECTNAME$$",prj.GetProjectName()},
                {"$$VERSION$$",prj.Version},
                {"$$VERSION_V$$",prj.Version.Replace('.',',') },
            };
            if (Project.CreateResource("WindowsDesktop", "resources.rc", d, Path.Combine(root, "resources.rc"), prj.EC) == false)
                return false;
            string parameters = "/r /v \"" + Path.Combine(root, "resources.rc");
            string result = prj.ShellExecute(GetTool("rc"), parameters, "");
            if (File.Exists(Path.Combine(root, "resources.res")) == false)
            {
                prj.EC.AddError("'resources.res' was not created. Abording ... !\n"+result);
                return false;
            }

            return task.UpdateSuccessErrorState(true);
        }
        private bool PackApplication()
        {
            task.CreateSubTask("Packing file ...");
            string resultFile = GetOutputFile(root, "application.compressed.exe");
            string parameters = "";
            string result = "";
            string sourceFile = Path.Combine(root, "application.exe");
            switch (((WindowsDesktopBuildConfiguration)Build).PackMethod)
            {
                case WindowsDesktopCodePacker.None:
                    if (Disk.Copy(sourceFile, resultFile, prj.EC) == false)
                        result = "Unable to copy file !";
                    break;
                case WindowsDesktopCodePacker.UPX:
                    parameters = "-9 -o \"" + resultFile + "\" \"" + sourceFile + "\"";
                    result = prj.ShellExecute(GetTool("upx"), parameters, "");
                    break;

            }
            if (File.Exists(resultFile) == false)
            {
                prj.EC.AddError("'"+resultFile+"' was not created. Abording ... !\n" + result);
                return false;
            }            
            return task.UpdateSuccessErrorState(true);
        }
        public override void OnBuild()
        {
            if (Disk.CleanDirectory(root, prj.EC) == false)
                return;
            if (GenerateIcon() == false)
                return;
            if (GenerateResourceSection() == false)
                return;
            if (Build.GenerateResources(Path.Combine(root, "resources.dat"), true) == false)
                return;
            if (CompressResources() == false)
                return;
            if (Build.GenerateResourceCodeFiles() == false)
                return;
            if (GenerateAndCompile() == false)
                return;
            if (PackApplication() == false)
                return;

        }
    }
}
