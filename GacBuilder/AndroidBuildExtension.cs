using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GAppCreator;
using System.IO;
using System.Diagnostics;

namespace GAppCreator
{
    public class AndroidBuildExtension : BuildExtension
    {
        class LibInfo
        {
            public string jarFile = "";
            public string repo = "";
            public bool use = false;
            public LibInfo(string jar, string _repo) { jarFile = jar; repo = _repo; }
        };
        private class ExtraData
        {
            public string AdInitCode = "";
            public string AdLoadCode = "";
            public string ManifestActivities = "";
            public string ManifestMetadate = "";
            public string Permissions = "";
            //public bool GoogleServicesAlreadyAdded = false;

            public Dictionary<string, bool> JavaFiles = new Dictionary<string, bool>();
            public Dictionary<string, bool> ApplyPlugins = new Dictionary<string, bool>();
            public Dictionary<string, string> Files = new Dictionary<string, string>();
            public Dictionary<string, LibInfo> Libs = new Dictionary<string, LibInfo>()
            {
                {"chartboost", new LibInfo("chartboost-6.6.3.jar","") },
                {"play-services-games", new LibInfo("","com.google.android.gms:play-services-games:16.0.0") },
                {"play-services-auth", new LibInfo("","com.google.android.gms:play-services-auth:16.0.0") },
                {"admob",new LibInfo("","com.google.android.gms:play-services-ads:16.0.0") },
                {"firebase-core", new LibInfo("","com.google.firebase:firebase-core:16.0.1") },
                {"firebase-analytics", new LibInfo("","com.google.firebase:firebase-analytics:16.0.1") },
                {"firebase-crashanalytics", new LibInfo("","com.crashlytics.sdk.android:crashlytics:2.9.3") },
            };

        };

        

        private AndroidBuildConfiguration AndroidConfig
        {
            get { return (AndroidBuildConfiguration)Build; }
        }
        private Random r = new Random();
        private ExtraData add;
        private string project_main = "";
       

        private string TransformLoggingCode(string source)
        {
            if (source == null)
            {
                prj.EC.AddError("Null line in TransformLoggingCode(...)");
                return null;
            }
            StringReader sr = new StringReader(source);
            string line;
            string result = "";
            while ((line = sr.ReadLine()) != null)
            {
                if (line.Trim().StartsWith("LOG_"))
                {
                    int i1 = line.IndexOf('(');
                    int i2 = line.LastIndexOf(')');
                    if ((i1 > i2) || (i1 <= 0) || (i2 <= 0))
                    {
                        prj.EC.AddError("Invalid LOG instruction (should be one on each line) in '"+line+"'");
                        return null;
                    }
                    string new_line = "Log.v(\"GAPP\",String.format(\"$$THID:%4d -> %s\",Thread.currentThread().getId(),String.format"+line.Substring(i1,i2-i1+1)+"));";
                    new_line = line.Substring(0,line.IndexOf("LOG_")) + new_line;
                    string log_type = line.Substring(0,i1).Trim();
                    switch (log_type)
                    {
                        case "LOG_INFO":
                            if (Build.EnableInfoLogging)
                                line = new_line.Replace("$$", "[Java INFO] ");
                            else
                                line = null;
                            break;
                        case "LOG_ERROR":
                            if (Build.EnableErrorLogging)
                                line = new_line.Replace("$$", "[JavaERROR] ");
                            else
                                line = null;
                            break;
                        case "LOG_EVENT":
                            if (Build.EnableEventLogging)
                                line = new_line.Replace("$$", "[JavaEVENT] ");
                            else
                                line = null;
                            break;
                        default:
                            prj.EC.AddError("Unknow LOG_xxx directive ("+log_type+") - in '"+line+"'");
                            return null;
                    }
                }
                if (line!=null)
                    result += line + "\n";
            }
            return result;
        }        
        private string GetAndroidSDKPlatform()
        {
            string ver = ((int)AndroidConfig.AndroidSDKVersion).ToString();
            return Path.Combine(prj.Settings.AndroidSDKPlatform, "android-" + ver, "android.jar");
        }
        private string GetSourceJavaFolder()
        {
            return Path.Combine(root, "src", AndroidConfig.Package.Replace(".", "\\"));
        }
        private string GetClassFolder()
        {
            return Path.Combine(root, "obj", AndroidConfig.Package.Replace(".", "\\"));
        }

        private bool AddLibrary(string name)
        {
            if (add.Libs.ContainsKey(name)==false)
            {
                prj.EC.AddError("Unknown library entry: " + name);
                return false;
            }
            add.Libs[name].use = true;
            return true;
        }
        private bool IsLibraryAdded(string name)
        {
            if (add.Libs.ContainsKey(name)==false)
                return false;
            return add.Libs[name].use;
        }

        private string CreateObfuscatedJavaString(string varName, string originalString, int varIDTempName)
        {
            if ((originalString == null) || (originalString.Length == 0))
                return varName + " = \"\";";
            // build
            string tempVarName = "temp_" + varIDTempName.ToString();
            string s = "\t\t//" + varName + " = " + originalString + "\n\t\tint " + tempVarName + ";\n";
            s += "\t\t" + varName + " = \"\";\n";
            

            for (int tr = 0; tr < originalString.Length; tr++)
            {
                int key, key2;

                if (tr == 0)
                {
                    key = r.Next(95) + 32;
                    s += "\t\t" + tempVarName + " = " + key.ToString() + ";";
                }
                else
                {
                    key2 = r.Next(tr);
                    s += "\t\t" + tempVarName + " = " + varName + ".charAt(" + key2.ToString() + ");";
                    key = originalString[key2];
                }
                int count = r.Next(3) + 1;
                while (count > 0)
                {
                    key2 = r.Next(64) + 1;
                    switch (r.Next(5))
                    {
                        case 0: s += tempVarName + " += " + key2.ToString() + ";"; key += key2; break;
                        case 1: if (key > key2) { s += tempVarName + " -= " + key2.ToString() + ";"; key -= key2; } break;
                        case 2: s += tempVarName + " |= " + key2.ToString() + ";"; key |= key2; break;
                        case 3: s += tempVarName + " &= " + key2.ToString() + ";"; key &= key2; break;
                        case 4: s += tempVarName + " ^= " + key2.ToString() + ";"; key ^= key2; break;
                    }
                    count--;
                }
                // compunerea finala // 40 - 50
                key2 = (int)originalString[tr];
                if (r.Next(2) == 0)
                {
                    if (key2 > key)
                    {
                        s += varName + "+= (char)(" + tempVarName + "+" + (key2 - key).ToString() + ");\n";
                    }
                    else
                    {
                        s += varName + "+= (char)(" + tempVarName + "-" + (key - key2).ToString() + ");\n";
                    }
                }
                else
                {
                    s += varName + "+= (char)(" + tempVarName + "^" + (key ^ key2).ToString() + ");\n";
                }
            }
            return s;
        }
        private string CreatePackageNameValidation()
        {
            string s = "\t//Checking for: " + AndroidConfig.Package+"\n";
            foreach (char c in AndroidConfig.Package)
            {
                int code = (int)c;
                int key = r.Next(255);
                s += String.Format("\tif (((*p) ^ {0})!={1}) return false; else p++;\n", key, code ^ key);
            }
            s += "\tif ((*p)!=0) return false;";
            return s;
        }

        private bool PrepareFolders()
        {
            task.CreateSubTask("Creating folders ...");
            while (true)
            {
                if (Disk.CreateFolder(root, prj.EC) == false)
                    break;

                if (Disk.CreateFolder(Path.Combine(root, "gradle"), prj.EC) == false)
                    break;
                if (Disk.CreateFolder(Path.Combine(root, "gradle\\wrapper"), prj.EC) == false)
                    break;


                if (Disk.CreateFolder(Path.Combine(root, "app"), prj.EC) == false)
                    break;
                if (Disk.CreateFolder(Path.Combine(root, "app\\src"), prj.EC) == false)
                    break;

                project_main = Path.Combine(root, "app\\src\\main");

                if (Disk.CreateFolder(project_main, prj.EC) == false)
                    break;



                if (Disk.CreateFolder(Path.Combine(project_main, "assets"), prj.EC) == false)
                    break;
                if (Disk.CreateFolder(Path.Combine(project_main, "libs"), prj.EC) == false)
                    break;
                if (Disk.CreateFolder(Path.Combine(project_main, "lib"), prj.EC) == false)
                    break;
                if (Disk.CreateFolder(Path.Combine(project_main, "jni"), prj.EC) == false)
                    break;
                if (Disk.CreateFolder(Path.Combine(project_main, "java"), prj.EC) == false)
                    break;

                if (Disk.CreateFolder(Path.Combine(project_main, "res"), prj.EC) == false)
                    break;
                if (Disk.CreateFolder(Path.Combine(project_main, "res\\drawable-hdpi"), prj.EC) == false)
                    break;
                if (Disk.CreateFolder(Path.Combine(project_main, "res\\drawable-ldpi"), prj.EC) == false)
                    break;
                if (Disk.CreateFolder(Path.Combine(project_main, "res\\drawable-mdpi"), prj.EC) == false)
                    break;
                if (Disk.CreateFolder(Path.Combine(project_main, "res\\drawable-xhdpi"), prj.EC) == false)
                    break;
                if (Disk.CreateFolder(Path.Combine(project_main, "res\\drawable-xxhdpi"), prj.EC) == false)
                    break;
                if (Disk.CreateFolder(Path.Combine(project_main, "res\\drawable-xxxhdpi"), prj.EC) == false)
                    break;
                if (Disk.CreateFolder(Path.Combine(project_main, "res\\drawable-nodpi"), prj.EC) == false)
                    break;
                if (Disk.CreateFolder(Path.Combine(project_main, "res\\raw"), prj.EC) == false)
                    break;

                if ((AndroidConfig.Package == null) || (AndroidConfig.Package.Length == 0) ||
                    (AndroidConfig.Package.Contains(".") == false) || (AndroidConfig.Package.EndsWith(".")))
                {
                    prj.EC.AddError("Invalid package name (it should be in the form xxx.xxx or xxx.xxx.xxx!");
                    break;
                }
                if ((Disk.CreateFolder(Path.Combine(project_main, "java", AndroidConfig.Package.Replace(".", "\\")), prj.EC)) == false)
                    break;
                task.UpdateSuccessErrorState(true);
                return true;
            }
            task.UpdateSuccessErrorState(false);
            return false;
        }

        private bool GenerateCppFiles()
        {
            task.CreateSubTask("Updateing source files ...");
            string cpp_list = "main.cpp ";
            // copii fisierele din framework
            if (CopyDefaultCPPFiles(Sources.FrameworkSources, Path.Combine(project_main, "jni"), ref cpp_list) == false)
                return false;
            // main file
            Dictionary<string, string> d = new Dictionary<string, string>();
            if (prj.UpdateReplaceDictionary(d,Build) == false)
                return false;
            Build.UpdateReplaceDictionaryWithSocialMedia(d, prj);
            d["$$PACKAGE_JNI$$"] = AndroidConfig.Package.Replace(".", "_");
            d["$$PACKAGE_NAME_CHECK$$"] = CreatePackageNameValidation();

            int design_resolution_width = 0;
            int design_resolution_height = 0;
            if (Project.SizeToValues(prj.DesignResolution, ref design_resolution_width, ref design_resolution_height) == false)
            {
                prj.EC.AddError("Unable to parse DesignResolution: " + ((DevelopBuildConfiguration)prj.BuildConfigurations[0]).AppResolution);
                return false;
            }
            d["$$SCREEN.DESIGNHEIGHT$$"] = design_resolution_height.ToString();
            d["$$SCREEN.DESIGNWEIGHT$$"] = design_resolution_width.ToString();

            if (Project.CreateResource("Android","main.cpp", d, Path.Combine(project_main, "jni", "main.cpp"), prj.EC) == false)
                return false;            
                        
            task.Info("CPP List: " + cpp_list);
            d.Clear();
            d["$$SOURCE_FILES$$"] = cpp_list;
            d["$$COMPILER_FLAGS$$"] = AndroidConfig.CompilerParams + " -DPLATFORM_ANDROID ";
            switch (AndroidConfig.OptimizationMode)
            {
                case CompilerOptimizationMode.Debug: d["$$COMPILER_FLAGS$$"] += " -O0 "; break;
                case CompilerOptimizationMode.Normal: d["$$COMPILER_FLAGS$$"] += " -O0 -fvisibility=hidden "; break;
                case CompilerOptimizationMode.Optimize: d["$$COMPILER_FLAGS$$"] += " -O1 -fvisibility=hidden "; break;
                case CompilerOptimizationMode.HighlyOptimize: d["$$COMPILER_FLAGS$$"] += " -O2 -fvisibility=hidden "; break;
            };

            d["$$LIBRARIES_LIST$$"] = "-lGLESv2 -landroid";

            if ((AndroidConfig.EnableErrorLogging) || (AndroidConfig.EnableEventLogging) || (AndroidConfig.EnableInfoLogging))
                d["$$LIBRARIES_LIST$$"] += " -llog";

            if (AndroidConfig.EnableErrorLogging)
                d["$$COMPILER_FLAGS$$"] += " -DENABLE_ERROR_LOGGING";
            if (AndroidConfig.EnableEventLogging)
                d["$$COMPILER_FLAGS$$"] += " -DENABLE_EVENT_LOGGING";
            if (AndroidConfig.EnableInfoLogging)
                d["$$COMPILER_FLAGS$$"] += " -DENABLE_INFO_LOGGING";

            switch (Build.ImageCreateMethod)
            {
                case BuildImageCreateMethod.Dynamic:
                    d["$$COMPILER_FLAGS$$"] += " -DDYNAMIC_IMAGE_RESIZE";
                    break;
                case BuildImageCreateMethod.ResizeAtStartup:
                    d["$$COMPILER_FLAGS$$"] += " -DRESIZE_AT_STARTUP_IMAGE_RESIZE";
                    break;
            }
  

            if (Project.CreateResource("Android","Android.mk",d,Path.Combine(project_main, "jni", "Android.mk"),prj.EC)==false)
                return false;

            d.Clear();
            d["$$ANDROID_VERSION$$"] = "android-14";
            if (Project.CreateResource("Android","Application.mk", d, Path.Combine(project_main, "jni", "Application.mk"), prj.EC) == false)
                return false;

            if ((prj.Settings.AndroidNDKBuildCMD == null) || (prj.Settings.AndroidNDKBuildCMD.Length == 0))
            {
                task.Info("Missing 'Android NDK settings' - execution stopped !");
                prj.EC.AddError("Missing 'Android NDK settings' - execution stopped !");
                return false;
            }

            /*
            if (AndroidConfig.GenerateCompileNDKBatchFile)
            {
                if (Disk.SaveFile(Path.Combine(root, "compile_ndk.bat"), prj.Settings.AndroidNDKBuildCMD, prj.EC) == false)
                    return task.UpdateSuccessErrorState(false);
            }
             */

            task.UpdateSuccessErrorState(true);
            return true;
        }
        private bool BuildCppFiles()
        {
            task.CreateSubTask("Compiling cpp files ...");

            while (true)
            {
                if (prj.Settings.AndroidNDKBuildCMD.Length == 0)
                {
                    prj.EC.AddError("Android NDK-Build path not found ! Check settings file !");
                    break;
                }
                if (File.Exists(prj.Settings.AndroidNDKBuildCMD) == false)
                {
                    prj.EC.AddError("Android NDK-Build path not found ! Check settings file !");
                    break;
                }
                string bat_file = "@" + root.Substring(0, 2) + "\n@cd " + root + "\n@call " + prj.Settings.AndroidNDKBuildCMD + " 1>jni_compile.log 2>jni_error.log\n@exit";
                if (Disk.SaveFile(Path.Combine(root, "compile-jni.bat"), bat_file, prj.EC) == false)
                    break;
                string content = prj.ShellExecute("C:\\Windows\\System32\\cmd.exe","/c \""+Path.Combine(root, "compile-jni.bat")+"\"", root);
                if (content == null)
                    return task.UpdateSuccessErrorState(false);
                task.Info(content);
                if (File.Exists(Path.Combine(root, "libs", "armeabi", "libframework.so")) == false)
                {
                    prj.EC.AddError("'framework.so' was not created !");
                    string s1 = null, s2 = null;
                    if (File.Exists(Path.Combine(root, "jni_compile.log")))
                        s1 = Disk.ReadFileAsString(Path.Combine(root, "jni_compile.log"), prj.EC);
                    if (File.Exists(Path.Combine(root, "jni_error.log")))
                        s2 = Disk.ReadFileAsString(Path.Combine(root, "jni_error.log"), prj.EC);
                    if (s1 == null)
                        s1 = "";
                    if (s2 == null)
                        s2 = "";
                    if (s1.Length > 0)
                        s1 += "\n-------------------------------------------------------------------------------------------------------------------\n";
                    s1 += s2;
                    prj.EC.AddError(s1);
                    break;
                }
                else
                {
                    // for debug purposes
                    /*
                    string s1 = null;
                    if (File.Exists(Path.Combine(root, "jni_compile.log")))
                        s1 = Disk.ReadFileAsString(Path.Combine(root, "jni_compile.log"), prj.EC);
                    if (s1 != null)
                        task.Info(s1);
                    */
                }
                task.UpdateSuccessErrorState(true);
                return true;
            }
            task.UpdateSuccessErrorState(false);
            return false;
        }
        private bool CreateManifest()
        {
            Dictionary<string,string> d = new Dictionary<string,string>()
            {
                {"$$PACKAGE$$",AndroidConfig.Package},
                {"$$VERSIONCODE$$",AndroidConfig.VersionCode.ToString()},
                {"$$VERSIONNAME$$",prj.Version},
                {"$$MINSDKVERSION$$",((int)(AndroidConfig.MinSDKVersion)).ToString()},                
            };
            // permisiuni
            string perm = "";
            if (AndroidConfig.PermInternet)
            {
                perm += "    <uses-permission android:name=\"android.permission.ACCESS_NETWORK_STATE\"/>\n";
                perm += "    <uses-permission android:name=\"android.permission.INTERNET\"/>\n";
            }
            if (AndroidConfig.PermVibrate)
                perm += "    <uses-permission android:name=\"android.permission.VIBRATE\"/>\n";
            if (AndroidConfig.PermExternalStorage)
                perm += "    <uses-permission android:name=\"android.permission.WRITE_EXTERNAL_STORAGE\"/>\n";
            if (AndroidConfig.BillingMarket == AndroidBillingMarket.GooglePlay)
                perm += "    <uses-permission android:name=\"com.android.vending.BILLING\"/>\n";
            if (AndroidConfig.PermSamsung)
                perm += "    <uses-permission android:name=\"com.samsung.android.providers.context.permission.WRITE_USE_APP_FEATURE_SURVEY\"/>\n";

            perm += add.Permissions;

            d["$$PERMISIONS$$"] = perm;


            // alte activitati
            string act = "";
            string metadate = "";

            act += add.ManifestActivities;
            act += add.ManifestMetadate;

            d["$$OTHERACTIVITIES$$"] = act;
            d["$$METADATA$$"] = metadate;




            string content = Project.GetResource("Android","AndroidManifest.xml", d, prj.EC);
            if (prj.EC.HasErrors())
                return false;
            // extra stuff
            if (AndroidConfig.TargetSDKVersion == AndroidVersion.None)
                content = content.Replace("$$TARGETSDKVERSION$$", "");
            else
                content = content.Replace("$$TARGETSDKVERSION$$", "android:targetSdkVersion=\"" + ((int)AndroidConfig.TargetSDKVersion).ToString() + "\"");


            return Disk.SaveFile(Path.Combine(project_main, "AndroidManifest.xml"),content,prj.EC);
        }
        private bool CopyIcon(int size,string folder,Project.ProjectIcons Icons)
        {
            if (Icons.Sizes.ContainsKey(size)==false)
            {
                prj.EC.AddError(String.Format("No icon defined for {0}x{0} size !", size));
                return false;
            }
            if (Disk.Copy(Icons.Sizes[size], Path.Combine(project_main, "res", folder, "app_main_icon.png"), prj.EC) == false)
                return false;
            return true;
        }
        private bool CreateIcons()
        {
            Project.ProjectIcons Icons = prj.GetBuildIcons(this.Build);
            if (CopyIcon(36, "drawable-ldpi", Icons) == false)
                return false;
            if (CopyIcon(48, "drawable-mdpi", Icons) == false)
                return false;
            if (CopyIcon(72, "drawable-hdpi", Icons) == false)
                return false;
            if (CopyIcon(96, "drawable-xhdpi", Icons) == false)
                return false;
            if (CopyIcon(96, "drawable-nodpi", Icons) == false)
                return false;
            if (CopyIcon(144, "drawable-xxhdpi", Icons) == false)
                return false;
            if (CopyIcon(192, "drawable-xxxhdpi", Icons) == false)
                return false;

            return true;
        }
        private bool CreateJavaFile(string resourceFileName, Dictionary<string, string> convert, Dictionary<string, bool> defs, string javaFileName)
        {
            string content = Project.GetResource("Android",resourceFileName, convert, prj.EC);
            if (prj.EC.HasErrors())
                return false;
            content = TransformLoggingCode(Build.EnableDefinitionsCode(content, defs, prj.EC));
            if (prj.EC.HasErrors())
            {
                prj.EC.AddError("Fail to apply define transformation to " + resourceFileName);
                return false;
            }
            string java_folder = Path.Combine(project_main, "java", AndroidConfig.Package.Replace(".", "\\"));
            if (Disk.SaveFile(Path.Combine(java_folder, javaFileName), content, prj.EC) == false)
                return false;
            return true;
        }
        private string GenerateLeaderBoardInitCode()
        {
            string code_name = "";
            string code_id = "";
            List<string> ld = Project.StringListToList(AndroidConfig.GooglePlayLeaderboardsList);
            foreach (string l in ld)
            {
                List<string> w = Project.StringListToList(l,'|');
                if (w.Count != 3)
                {
                    prj.EC.AddError("Invalid leaderboard item: '" + l + "'. It should have 3 components !");
                    return null;
                }
                code_name += "\"" + w[1] + "\",";
                if (w[2] == "A")
                    code_id += "LeaderboardVariant.TIME_SPAN_ALL_TIME,";
                else if (w[2] == "W")
                    code_id += "LeaderboardVariant.TIME_SPAN_WEEKLY,";
                else if (w[2] == "T")
                    code_id += "LeaderboardVariant.TIME_SPAN_DAILY,";
                else
                {
                    prj.EC.AddError("Invalid leaderboard type: '" + w[2] + "' in '"+l+"'. It should be A, W or T !");
                    return null;
                }
            }
            string res = "\n\tLeaderBoardsCount = " + ld.Count.ToString() + ";\n";
            if (ld.Count>0)
            {
                res += "\tLeaderBoardsIDs = new String[] {" + code_name.Substring(0, code_name.Length - 1) + "};\n";
                res += "\tLeaderBoardsTimeSpans = new int[] {" + code_id.Substring(0, code_id.Length - 1) + "};\n";
            }
            return res;
        }
        private bool GenerateJavaFiles()
        {
            Dictionary<string, string> d = new Dictionary<string, string>()
            {
                {"$$PACKAGE$$",AndroidConfig.Package},
                {"$$ORIENTATION$$",AndroidConfig.Orientation.ToString().ToUpper()},
                {"$$ADS_COUNT$$",Build.Ads.Count.ToString()},
                {"$$PRESERVE_EGL_CONTEXT$$",""},
            };
            if (AndroidConfig.PreserveEGLContext)
                d["$$PRESERVE_EGL_CONTEXT$$"] = "\tif (android.os.Build.VERSION.SDK_INT>=11) {\n\t\tLOG_INFO(\"Enable preserve EGL Context\");\n\t\tsetPreserveEGLContextOnPause (true);\n\t}\n";
            task.CreateSubTask("Create Android specific files ...");
            int count = 0;
            if (Build.R.ResourcesCount.ContainsKey(typeof(SoundResource)))
                count = Build.R.ResourcesCount[typeof(SoundResource)];
            // adaug si codul pentru sound
            string content = "";
            if (count == 0)
            {
                d["$$SOUND_ARRAY_VARIABLES$$"] = "\tGSound []Sounds = null;";
                d["$$SOUND_INIT_ARRAY_VARIABLES$$"] = "";
            }
            else
            {
                d["$$SOUND_ARRAY_VARIABLES$$"] = "\tGSound []Sounds = new GSound[" + count.ToString() + "];";
                content = "";
                foreach (GenericResource r in Build.R.List)
                {
                    if (r.GetType() == typeof(SoundResource))
                    {                        
                        content += String.Format("\t\tSounds[{0}] = new GSound(this,R.raw.sound_{0},{1});\n", r.GetResourceIndex(), ((SoundResource)r).BackgroundMusic.ToString().ToLower());
                    }
                }
                d["$$SOUND_INIT_ARRAY_VARIABLES$$"] = content;
            }


            Dictionary<string, bool> defs = Build.CreateDefsDictionary();
            defs["ADS"] = (Build.Ads.Count > 0);
            defs["ADMOB"] = (add.Libs.ContainsKey("admob")) && (add.Libs["admob"].use);
            defs["ANALYTICS"] = (AndroidConfig.AnalyticsFramework != AnalyticsFrameworkType.None);
            defs["GOOGLE_ANALYTICS"] = (AndroidConfig.AnalyticsFramework == AnalyticsFrameworkType.GoogleAnalytics);
            defs["FLURRY_ANALYTICS"] = (AndroidConfig.AnalyticsFramework == AnalyticsFrameworkType.Flurry);
            defs["FIREBASE_ANALYTICS"] = (AndroidConfig.AnalyticsFramework == AnalyticsFrameworkType.FireBase);
            defs["GOOGLE_BILLING"] = (AndroidConfig.BillingMarket == AndroidBillingMarket.GooglePlay);
            defs["HIDE_SOFTWARE_BUTTONS"] = AndroidConfig.HideSoftwareButtons;
            defs["GOOGLE_PLAY_SERVICES"] = AndroidConfig.GooglePlayServices;

            d["$$GOOGLE_PLAY_SERVICES_AUTO_LOGIN$$"] = "";
            if (AndroidConfig.GooglePlayServices)
            {
                string ld_init = GenerateLeaderBoardInitCode();
                if (ld_init == null)
                    return task.UpdateSuccessErrorState(false);
                d["$$LEADERBOARDS_INIT$$"] = ld_init;
            }

            while (true)
            {
                if (defs["ADS"])
                {
                    d["$$ADS_COUNT$$"] = Build.Ads.Count.ToString();
                    d["$$GENERATE_ADS$$"] = add.AdInitCode;
                    d["$$LOAD_ADS_STARTUP$$"] = add.AdLoadCode;
                }
                if (defs["GOOGLE_BILLING"])
                {
                    String buy_token = "";
                    while (buy_token.Length<32)
                    {
                        buy_token += (char)(r.Next(26) + 65);
                    }
                    d["$$BUILD_PURCHASE_VALIDATE_STRING_1$$"] = CreateObfuscatedJavaString("purchaseValidateString", buy_token, 10001);
                    d["$$BUILD_PURCHASE_VALIDATE_STRING_2$$"] = CreateObfuscatedJavaString("purchaseValidateString", buy_token, 10002);
                    d["$$BUILD_GOOGLE_BILLING_KEY$$"] = CreateObfuscatedJavaString("googleBillingKey", AndroidConfig.BillMarketKey, 20000);
                    d["$$BUILD_INAPP_ITEM_LIST$$"] = CreateObfuscatedJavaString("inappItemsList", AndroidConfig.InAppItemsList, 30000);
                }
                if (defs["GOOGLE_ANALYTICS"])
                {
                    d["$$BUILD_GOOGLE_ANALYTICS_TRACKING_ID$$"] = CreateObfuscatedJavaString("trackingId", AndroidConfig.GoogleAnalyticsTrackingID, 40000);
                }
                if (defs["FLURRY_ANALYTICS"])
                {
                    d["$$BUILD_FLURRY_API_KEY$$"] = CreateObfuscatedJavaString("f_apiKey", AndroidConfig.FlurryAPIKey, 40000);
                }
                if (CreateJavaFile("Activity.java", d, defs, "MainActivity.java") == false)
                    break;
                if (CreateJavaFile("OpenGLView.java", d, defs, "OpenGLView.java") == false)
                    break;
                if (CreateJavaFile("GSound.java", d, defs, "GSound.java") == false)
                    break;
                foreach (string ef in add.JavaFiles.Keys)
                {
                    if (CreateJavaFile(ef, d, defs, ef) == false)
                        break;
                }
                // daca am billing adaug si fisierul specifice
                if (AndroidConfig.BillingMarket == AndroidBillingMarket.GooglePlay)
                {
                    string aidl_path = Path.Combine(project_main, "java", "com\\android\\vending\\billing");
                    if (Disk.CreateFolder(aidl_path, prj.EC) == false)
                        break;
                    if (Project.CreateResource("Android","IInAppBillingService.aidl",null,Path.Combine(aidl_path,"IInAppBillingService.aidl"),prj.EC)==false)
                        break;
                    //string[] iab_files = { "Base64.java", "IabException.java", "Base64DecoderException.java", "IabHelper.java", "Inventory.java", "IabResult.java", "Security.java", "Purchase.java", "SkuDetails.java" ,"IInAppBillingService.java"};
                    string[] iab_files = { "GooglePlayBilling.java", "IInAppBillingService.java" };
                    foreach (string name in iab_files)
                    {
                        if (CreateJavaFile(name, d, defs, name) == false)
                            break;
                    }
                }
                // verific imersive-mode
                if (AndroidConfig.HideSoftwareButtons)
                {
                    if (((int)AndroidConfig.AndroidSDKVersion) < ((int)AndroidVersion.KitKat_19))
                    {
                        prj.EC.AddError("Software buttons can be hidden only for API level 19 or higher. Change your 'Android SDK Version' field to API level 19 or hight !");
                        break;
                    }
                }
                return task.UpdateSuccessErrorState(!prj.EC.HasErrors());
            }
            return task.UpdateSuccessErrorState(false);
        }
        private bool CopyExtraLibraries()
        {
            task.CreateSubTask("Adding extra libraries to the project ...");
            foreach (string name in add.Libs.Keys)
            {
                if (add.Libs[name].use == false)
                    continue;
                if (add.Libs[name].jarFile.Length == 0)
                    continue;
                task.Info(name);
                if (Disk.Copy(Project.GetResourceFullPath("Android", add.Libs[name].jarFile), Path.Combine(project_main, "libs", add.Libs[name].jarFile), prj.EC) == false)
                    return task.UpdateSuccessErrorState(false);
            }
            return task.UpdateSuccessErrorState(true);
        }
        private string GetBuildArchitectures()
        {
            List<string> l = Project.StringListToList(AndroidConfig.Architecture);
            string result = "";
            bool first = true;
            foreach (string i in l)
            {
                if (!first)
                    result += " , ";
                result += "'" + i.Replace("_", "-") + "'";
                first = false;
            }
            return result;
        }
        private bool CreateGradleFiles()
        {
            task.CreateSubTask("Create graddle files");
            
            //gradle.properties
            if (Disk.SaveFile(Path.Combine(root, "gradle.properties"), "org.gradle.jvmargs=-Xmx1536m", prj.EC)==false)
                return task.UpdateSuccessErrorState(false);
            //settings.gradle
            if (Disk.SaveFile(Path.Combine(root, "settings.gradle"), "include ':app'", prj.EC) == false)
                return task.UpdateSuccessErrorState(false);
            // gradlew.bat
            if (Disk.Copy(Project.GetResourceFullPath("Android", "Gradle\\gradlew.bat"), Path.Combine(root, "gradlew.bat"), prj.EC) == false)
                return task.UpdateSuccessErrorState(false);
            // local.properties
            if (Disk.Copy(Project.GetResourceFullPath("Android", "Gradle\\local.properties"), Path.Combine(root, "local.properties"), prj.EC) == false)
                return task.UpdateSuccessErrorState(false);
            // gradle-wrapper.jar
            if (Disk.Copy(Project.GetResourceFullPath("Android", "Gradle\\gradle-wrapper.jar"), Path.Combine(root, "gradle", "wrapper", "gradle-wrapper.jar"), prj.EC) == false)
                return task.UpdateSuccessErrorState(false);
            // gradle-wrapper.properties
            if (Disk.Copy(Project.GetResourceFullPath("Android", "Gradle\\gradle-wrapper.properties"), Path.Combine(root, "gradle", "wrapper", "gradle-wrapper.properties"), prj.EC) == false)
                return task.UpdateSuccessErrorState(false);

            // fisierele de build
            Dictionary<string, string> d = new Dictionary<string, string>();
            d["$$COMPILE_SDK_VERSION$$"] = AndroidConfig.AndroidSDKVersion.ToString().Split('_')[1];
            d["$$PACKAGE_NAME$$"] = AndroidConfig.Package;
            d["$$MIN_SDK_VERSION$$"] = AndroidConfig.MinSDKVersion.ToString().Split('_')[1];
            d["$$TARGET_SDK_VERSION$$"] = AndroidConfig.TargetSDKVersion.ToString().Split('_')[1];
            d["$$VERSION_CODE$$"] = AndroidConfig.VersionCode.ToString();
            d["$$VERSION_NAME$$"] = prj.Version;
            d["$$KEY_STORE_FILE$$"] = prj.Settings.AndroidSignKeystore.Replace("\\","/");
            d["$$KEY_STORE_PASSWORD$$"] = prj.Settings.AndroidSignPass;
            d["$$KEY_STORE_ALIAS$$"] = prj.Settings.AndroidKeystoreAlias;
            d["$$ARCHITECTURES$$"] = GetBuildArchitectures();

            // construiesc dependecies list
            string dep = "\n";
            foreach (var l in add.Libs.Keys)
            {
                // exclud ce nu utilizez sau cele care au un jar asociat
                if ((add.Libs[l].use == false) || (add.Libs[l].jarFile.Length > 0))
                    continue;
                dep += "    implementation '" + add.Libs[l].repo + "'\n";
            }
            d["$$DEPENDECIES$$"] = dep;


            // construiesc lista de pluginuri care trebuie aplicate
            string app_plugins = "";
            foreach (var key in add.ApplyPlugins.Keys)
                app_plugins += "    apply plugin: '" + key + "'\n";
            d["$$APPLY.PLUGINS$$"] = app_plugins;



            string content_top = Project.GetResource("Android", "Gradle\\top-level-build.gradle", d, prj.EC);
            if (content_top.Length==0)
            {
                prj.EC.AddError("Unable to process content for Gradle\\top-level-build.gradle");
                return task.UpdateSuccessErrorState(false);
            }

            string content_app = Project.GetResource("Android", "Gradle\\app-build.gradle", d, prj.EC);
            if (content_app.Length == 0)
            {
                prj.EC.AddError("Unable to process content for Gradle\\app-build.gradle");
                return task.UpdateSuccessErrorState(false);
            }

            if (Disk.SaveFile(Path.Combine(root, "build.gradle"), content_top, prj.EC) == false)
                return task.UpdateSuccessErrorState(false);
            if (Disk.SaveFile(Path.Combine(root, "app\\build.gradle"), content_app, prj.EC) == false)
                return task.UpdateSuccessErrorState(false);

            string starter = "@call gradlew.bat clean\n@call gradlew.bat assembleRelease\n";
            if (Disk.SaveFile(Path.Combine(root, "compile.bat"), starter, prj.EC) == false)
                return task.UpdateSuccessErrorState(false);

            return task.UpdateSuccessErrorState(true);
        }
        private bool RunGradle()
        {
            task.CreateSubTask("Running graddle plugin ...");
            // rulam
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.Arguments = "";
                psi.FileName = Path.Combine(root,"compile.bat");
                psi.WorkingDirectory = root;
                psi.UseShellExecute = false;
                Process p = Process.Start(psi);
                if (p == null)
                {
                    prj.EC.AddError("Unable to start process gradle process");
                    return task.UpdateSuccessErrorState(false);
                }
                p.WaitForExit();
            }
            catch (Exception e)
            {
                prj.EC.AddException("Unable to start gradle process !", e);
                return task.UpdateSuccessErrorState(false);
            }


            // verific ca totul a mers ok
            string resultName = Path.Combine(root, "app\\build\\outputs\\apk\\release\\app-release.apk");
            if (File.Exists(resultName) == false)
            {
                prj.EC.AddError("APK '" + resultName + "' was not created !!! Compile error !");
                return task.UpdateSuccessErrorState(false);
            }
            string apk = GetOutputFile(Path.Combine(root, "bin"), Path.Combine(AndroidConfig.Package + "." + prj.Version + ".apk"));
            if (Disk.Copy(resultName,apk,prj.EC)==false)
            {
                prj.EC.AddError("Unable to copy '" + resultName + "' to '" + apk);
                return task.UpdateSuccessErrorState(false);
            }
            task.Info("APK Created succesifully !");
            task.Info("APK:" + apk);
            return task.UpdateSuccessErrorState(true);
        }
        private bool CreateExtraFiles()
        {
            task.CreateSubTask("Adding aditional files to the project ...");
            foreach (string name in add.Files.Keys)
            {
                if (Disk.SaveFile(name,add.Files[name],prj.EC)==false)
                    return task.UpdateSuccessErrorState(false);
            }
            return task.UpdateSuccessErrorState(true);
        }


        private bool CreateStringXML(Language lang)
        {
            bool foundOne = false;
            string s = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n<resources>\n";
            string value = "";
            foreach (StringValues sv in prj.Strings)
            {
                value = sv.Get(lang);
                if ((value != null) && (value.Length > 0))
                {
                    value = value.Replace("'", "\\'");
                    s += "\t<string name=\"" + sv.GetVariableNameWithArrayAndroid() + "\" formatted=\"false\">" + value + "</string>\n";
                    foundOne = true;
                }
            }
            // adaug si intrarile pentru application_name
            value = prj.GetApplicationName(lang, Build);
            if ((value != null) && (value.Length > 0))
            {
                value = value.Replace("'", "\\'").Replace("&","&amp;");
                s += "\t<string name=\"application_name\" formatted=\"false\">" + value + "</string>\n";
                foundOne = true;
            }

            s += "</resources>\n";
            if (foundOne == false)
                return true;
            string fname = "";
            if (lang == prj.DefaultLanguage)
                fname = Path.Combine(project_main, "res", "values", "strings.xml");
            else
                fname = Path.Combine(project_main, "res", "values-" + Build.LanguageToName(lang), "strings.xml");
            if (Disk.CreateFolder(Path.GetDirectoryName(fname), prj.EC) == false)
                return false;
            return Disk.SaveFile(fname, s, prj.EC);
        }
        private bool CreateStringsXML()
        {
            task.CreateSubTask("Creating values xmls ...");
            while (true)
            {
                Dictionary<Language, StringValue> appNameLanguages = this.prj.ApplicationName.GetAvailableLanguages();
                Dictionary<Language, bool> buildLanguages = this.prj.GetBuildAvailableLanguages(this.Build, true);
                foreach (Language l in Enum.GetValues(typeof(Language)))
                {
                    if ((buildLanguages.ContainsKey(l) == false) && (appNameLanguages.ContainsKey(l)==false))
                        continue;
                    if (CreateStringXML(l) == false)
                        break;
                }

                string value = prj.GetApplicationName(prj.DefaultLanguage,Build); 
                if ((value == null) || (value.Length == 0))
                {
                    prj.EC.AddError("Application name is not set for the default language (" + prj.DefaultLanguage.ToString() + ") !");
                    break;
                }
                return task.UpdateSuccessErrorState(true);                
            }
            return task.UpdateSuccessErrorState(false);
        }
        private bool CreateLayouts()
        {
            task.CreateSubTask("Creating layout xmls ...");
            while (true)
            {
                string content = "<?xml version=\"1.0\" encoding=\"utf-8\"?><LinearLayout xmlns:android=\"http://schemas.android.com/apk/res/android\" android:layout_width=\"fill_parent\" android:layout_height=\"fill_parent\" android:orientation=\"vertical\" ></LinearLayout>";
                if (Disk.SaveFile(Path.Combine(project_main, "res", "layout", "main.xml"), content, prj.EC) == false)
                    break;
                if (Disk.SaveFile(Path.Combine(project_main, "res", "layout-large", "main.xml"), content, prj.EC) == false)
                    break;
                if (Disk.SaveFile(Path.Combine(project_main, "res", "layout-xlarge", "main.xml"), content, prj.EC) == false)
                    break;
                if (Disk.SaveFile(Path.Combine(project_main, "res", "layout-sw600dp", "main.xml"), content, prj.EC) == false)
                    break;
                if (Disk.SaveFile(Path.Combine(project_main, "res", "layout-sw720dp", "main.xml"), content, prj.EC) == false)
                    break;
                return task.UpdateSuccessErrorState(true); 
            }
            return task.UpdateSuccessErrorState(false);
        }

        private bool CopySoundFiles()
        {
            task.CreateSubTask("Creating sounds ...");
            foreach (GenericResource r in Build.R.List)
            {
                if (r.GetType() == typeof(SoundResource))
                {
                    string s = Path.Combine(project_main, "res", "raw", "sound_" + r.GetResourceIndex().ToString()+Path.GetExtension(r.Source));
                    if (Disk.Copy(r.GetSourceFullPath(), s, prj.EC) == false)
                        return task.UpdateSuccessErrorState(false);
                }
            }
            return task.UpdateSuccessErrorState(true);
        }

        #region Analytics
        private bool AddGoogleAnalytics()
        {
            prj.EC.AddError("Google analytics not implemented");
            //return AddLib("com.google.android.gms-play-services-analytics");
            return false;
        }
        private bool AddFlurryAnalytics()
        {
            prj.EC.AddError("Flurry not implementeed");
            return false;
        }
        private bool AddFirebaseAnalytics()
        {
            string google_json_file = Project.Base64ToString(AndroidConfig.FirebaseGoogleServicesJSONFile,prj.EC);
            if ((google_json_file == null) || (google_json_file.Length==0))
            {
                prj.EC.AddError("Missing 'google-services.json' content ! Firebase can not be initiated without it !");
                return false;
            }
            add.Files[Path.Combine(root, "app", "google-services.json")] = google_json_file;

            add.JavaFiles["AndroidFirebaseAnalytics.java"] = true;
            add.JavaFiles["IAndroidGenericAnalytics.java"] = true;

            if (AddLibrary("firebase-core") == false)
                return false;
            if (AddLibrary("firebase-analytics") == false)
                return false;

            add.ApplyPlugins["com.google.gms.google-services"] = true;

            return true;
        }
        private bool PrepareAnalytics()
        {
            if (AndroidConfig.AnalyticsFramework == AnalyticsFrameworkType.None)
                return true;
            task.CreateSubTask("Creating analytics ...");
            bool result = false;
            switch (AndroidConfig.AnalyticsFramework)
            {
                case AnalyticsFrameworkType.GoogleAnalytics:
                    result = AddGoogleAnalytics();
                    break;
                case AnalyticsFrameworkType.FireBase:
                    result = AddFirebaseAnalytics();
                    break;
                case AnalyticsFrameworkType.Flurry:
                    result = AddFlurryAnalytics();
                    break;
                default:
                    prj.EC.AddError("Analytics framework: " + AndroidConfig.AnalyticsFramework.ToString() + " is not implemented for Android !");
                    return task.UpdateSuccessErrorState(false);
            }
            if (!result)
            {
                prj.EC.AddError("Fail to add analytics framework: " + AndroidConfig.AnalyticsFramework.ToString() + " !");
                return task.UpdateSuccessErrorState(false);
            }
            return task.UpdateSuccessErrorState(true);
        }
        #endregion

        #region Other Services
        private bool PrepareOtherServices()
        {
            bool result = true;
            task.CreateSubTask("Creating config for other services ...");
            if (AndroidConfig.FirebaseCrashAnalytics)
            {
                result &= AddLibrary("firebase-core");
                result &= AddLibrary("firebase-crashanalytics");
                add.ApplyPlugins["io.fabric"] = true;
            }
            if (!result)
            {
                prj.EC.AddError("Fail to add all 3rd party SDKs");
                return task.UpdateSuccessErrorState(false);
            }
            return task.UpdateSuccessErrorState(true);
        }
        #endregion

        private bool PrepareGooglePlayServices()
        {
            if (AndroidConfig.GooglePlayServices == false)
                return true;
            if (AddLibrary("play-services-games") == false)
                return false;
            if (AddLibrary("play-services-auth") == false)
                return false;

            add.JavaFiles["AndroidGooglePlayServices.java"] = true;
            //add.ManifestActivities += "\t<activity android:name=\"com.google.android.gms.auth.api.signin.internal.SignInHubActivity\" android:windowSoftInputMode=\"stateAlwaysHidden|adjustPan\" />\n";
            add.ManifestMetadate += "\t<meta-data android:name=\"com.google.android.gms.games.APP_ID\" android:value=\"@string/app_id\" />\n";

            string res_file = "";
            res_file += "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
            res_file += "<resources>\n";
            res_file += "\t<string name=\"app_id\" translatable=\"false\">" + AndroidConfig.GooglePlayServicesAppID + "</string>\n";
            res_file += "\t<string name=\"package_name\" translatable=\"false\">" + AndroidConfig.Package + "</string>\n";
            res_file += "</resources>\n";
            add.Files[Path.Combine(project_main, "res", "values", "games-ids.xml")] = res_file;

            //AddGoogleServiceesRef();

            return true;
        }


        #region ADS

        private void AddGoogleServiceesRef()
        {
            // treebuie adaugate o data
            //if (add.GoogleServicesAlreadyAdded)
            //    return;
            //string gs_version = "12451000"; // 10260000 for 10.2.6
            //add.Files[Path.Combine(root, "res", "values", "version.xml")] = "<?xml version=\"1.0\" encoding=\"utf-8\"?><resources><integer name=\"google_play_services_version\">" + gs_version + "</integer></resources>";
            //add.ManifestActivities += "\t<activity android:name=\"com.google.android.gms.ads.AdActivity\" android:configChanges=\"keyboard|keyboardHidden|orientation|screenLayout|uiMode|screenSize|smallestScreenSize\" />\n";
            //add.ManifestMetadate += "\t<meta-data android:name=\"com.google.android.gms.version\" android:value=\"@integer/google_play_services_version\" />\n";

            //add.GoogleServicesAlreadyAdded = true;
        }

        private bool AddAdMobLib()
        {
            return AddLibrary("admob");
            //AddLib("com.google.android.gms-play-services-ads", GooglePlayVersion);
            //AddGoogleServiceesRef();
        }

        private bool PrepareGoogleAdMobBanner(GoogleAdMobBanner ad,int index)
        {
            string ad_unit_id = ad.AdUnitID;
            if (AndroidConfig.EnableAdMobTestMode)
                ad_unit_id = "ca-app-pub-3940256099942544/6300978111";
            //initializare
            add.AdInitCode += string.Format("\t\tString uid_{0}=\"\";\n",index);
            add.AdInitCode += string.Format("\t\tString kw_{0}=\"\";\n",index);
            add.AdInitCode += CreateObfuscatedJavaString(string.Format("uid_{0}", index), ad_unit_id, index * 2) + "\n";
            add.AdInitCode += CreateObfuscatedJavaString(string.Format("kw_{0}", index), ad.Keywords, index*2+1)+"\n";
            add.AdInitCode += string.Format("\t\tads[{0}] = new AndroidGoogleAdMobBanner();\n", index);
            add.AdInitCode += string.Format("\t\t((AndroidGoogleAdMobBanner)ads[{0}]).Init(this,{0},{1},{2});\n", index, ad.ReLoadAfterOpen.ToString().ToLower(), ad.MaxAttemptsOnFail);//layout,this,uid_{0},kw_{0},10);\n", index);
            add.AdInitCode += string.Format("\t\t((AndroidGoogleAdMobBanner)ads[{0}]).SetKeywordsList(kw_{0});\n", index);
            add.AdInitCode += string.Format("\t\t((AndroidGoogleAdMobBanner)ads[{0}]).SetGender({1});\n", index, (int)ad.Gender);
            switch (ad.Type)
            {
                case GoogleAdMobBannerType.Banner:
                    add.AdInitCode += string.Format("\t\t((AndroidGoogleAdMobBanner)ads[{0}]).CreateBannerAd(layout,this,uid_{0});\n", index);
                    break;
                case GoogleAdMobBannerType.LargeBanner:
                    add.AdInitCode += string.Format("\t\t((AndroidGoogleAdMobBanner)ads[{0}]).CreateLargeBannerAd(layout,this,uid_{0});\n", index);
                    break;
                case GoogleAdMobBannerType.MediumRectangle:
                    add.AdInitCode += string.Format("\t\t((AndroidGoogleAdMobBanner)ads[{0}]).CreateMediumRectangleAd(layout,this,uid_{0});\n", index);
                    break;
                case GoogleAdMobBannerType.SmartBanner:
                    add.AdInitCode += string.Format("\t\t((AndroidGoogleAdMobBanner)ads[{0}]).CreateSmartBannerAd(layout,this,uid_{0});\n", index);
                    break;          
            }
            add.AdInitCode += "\n";

            // autoload
            add.AdLoadCode += string.Format("\n\t\tads[{0}].SetAlignament({1});\n", index, (int)ad.Align);
            //if (ad.LoadOnStartup)
            //    add.AdLoadCode += string.Format("\t\tads[{0}].Load();\n", index);
            add.AdLoadCode += "\n";

            add.JavaFiles["AndroidGoogleAdMobBanner.java"] = true;
            add.JavaFiles["IAndroidGenericAd.java"] = true;
            add.JavaFiles["GenericAd.java"] = true;

            //AddGooglePlayServicesLib();
            return AddAdMobLib();
        }
        private bool PrepareGoogleAdMobInterstitial(GoogleAdMobInterstitial ad, int index)
        {
            string ad_unit_id = ad.AdUnitID;
            if (AndroidConfig.EnableAdMobTestMode)
                ad_unit_id = "ca-app-pub-3940256099942544/1033173712";
            //initializare
            add.AdInitCode += string.Format("\t\tString uid_{0}=\"\";\n", index);
            add.AdInitCode += string.Format("\t\tString kw_{0}=\"\";\n", index);
            add.AdInitCode += CreateObfuscatedJavaString(string.Format("uid_{0}", index), ad_unit_id, index * 2) + "\n";
            add.AdInitCode += CreateObfuscatedJavaString(string.Format("kw_{0}", index), ad.Keywords, index * 2 + 1) + "\n";
            add.AdInitCode += string.Format("\t\tads[{0}] = new AndroidGoogleAdMobInterstitial();\n", index);
            add.AdInitCode += string.Format("\t\t((AndroidGoogleAdMobInterstitial)ads[{0}]).Init(this,{0},{1},{2});\n", index, ad.ReLoadAfterOpen.ToString().ToLower(), ad.MaxAttemptsOnFail);//layout,this,uid_{0},kw_{0},10);\n", index);
            add.AdInitCode += string.Format("\t\t((AndroidGoogleAdMobInterstitial)ads[{0}]).SetKeywordsList(kw_{0});\n", index);
            add.AdInitCode += string.Format("\t\t((AndroidGoogleAdMobInterstitial)ads[{0}]).SetGender({1});\n", index, (int)ad.Gender);
            add.AdInitCode += string.Format("\t\t((AndroidGoogleAdMobInterstitial)ads[{0}]).Create(uid_{0});\n", index);
            add.AdInitCode += "\n";

            // autoload
            //if (ad.LoadOnStartup)
            //    add.AdLoadCode += string.Format("\t\tads[{0}].Load();\n", index);
            add.AdLoadCode += "\n";

            add.JavaFiles["AndroidGoogleAdMobInterstitial.java"] = true;
            add.JavaFiles["IAndroidGenericAd.java"] = true;
            add.JavaFiles["GenericAd.java"] = true;

            //AddGooglePlayServicesLib();
            return AddAdMobLib();
        }
        private bool PrepareGoogleAdMobRewardable(GoogleAdMobRewardable ad, int index)
        {
            //initializare
            string ad_unit_id = ad.AdUnitID;
            

            if (AndroidConfig.EnableAdMobTestMode)
            {
                ad_unit_id = "ca-app-pub-3940256099942544/5224354917";
            }
            
            add.AdInitCode += string.Format("\t\tString uid_{0}=\"\";\n", index);
            add.AdInitCode += CreateObfuscatedJavaString(string.Format("uid_{0}", index), ad_unit_id, index * 2) + "\n";
            add.AdInitCode += string.Format("\t\tads[{0}] = new AndroidGoogleAdMobRewardable();\n", index);
            add.AdInitCode += string.Format("\t\t((AndroidGoogleAdMobRewardable)ads[{0}]).Init(this,{0},{1},{2});\n", index, ad.ReLoadAfterOpen.ToString().ToLower(), ad.MaxAttemptsOnFail);//layout,this,uid_{0},kw_{0},10);\n", index);
            add.AdInitCode += string.Format("\t\t((AndroidGoogleAdMobRewardable)ads[{0}]).Create(uid_{0});\n", index);
            //if (ad.LoadOnStartup)
            //    add.AdLoadCode += string.Format("\t\tads[{0}].Load();\n", index);
            add.AdLoadCode += "\n";

            add.JavaFiles["AndroidGoogleAdMobRewardable.java"] = true;
            add.JavaFiles["IAndroidGenericAd.java"] = true;
            add.JavaFiles["GenericAd.java"] = true;

            //AddGooglePlayServicesLib();
            return AddAdMobLib();
        }
        private bool PrepareGoogleAdMobNativeExpress(GoogleAdMobNativeExpress ad, int index)
        {
            //initializare
            string ad_unit_id = ad.AdUnitID;


            if (AndroidConfig.EnableAdMobTestMode)
            {
                ad_unit_id = "ca-app-pub-3940256099942544/2793859312";
            }

            if ((ad.leftPercentage >= ad.rightPercentage) || (ad.topPercentage >= ad.bottomPercentage))
            {
                prj.EC.AddError("Invalid native ad position for : " + ad.Name);
                return false;
            }


            add.AdInitCode += string.Format("\t\tString uid_{0}=\"\";\n", index);
            add.AdInitCode += CreateObfuscatedJavaString(string.Format("uid_{0}", index), ad_unit_id, index * 2) + "\n";
            add.AdInitCode += string.Format("\t\tads[{0}] = new AndroidGoogleAdMobNativeExpress();\n", index);
            add.AdInitCode += string.Format("\t\t((AndroidGoogleAdMobNativeExpress)ads[{0}]).Init(this,{0},{1},{2});\n", index, ad.ReLoadAfterOpen.ToString().ToLower(), ad.MaxAttemptsOnFail);//layout,this,uid_{0},kw_{0},10);\n", index);
            add.AdInitCode += string.Format("\t\t((AndroidGoogleAdMobNativeExpress)ads[{0}]).Create(layout,uid_{0},(float){1},(float){2},(float){3},(float){4});\n", index,ad.leftPercentage,ad.topPercentage,ad.rightPercentage,ad.bottomPercentage);
            //if (ad.LoadOnStartup)
            //    add.AdLoadCode += string.Format("\t\tads[{0}].Load();\n", index);
            add.AdLoadCode += "\n";

            add.JavaFiles["AndroidGoogleAdMobNativeExpress.java"] = true;
            add.JavaFiles["IAndroidGenericAd.java"] = true;
            add.JavaFiles["GenericAd.java"] = true;

            //AddGooglePlayServicesLib();
            return AddAdMobLib();
        }
        private void PrepareInitializationDataForGoogleAdMob()
        {
            string app_id = AndroidConfig.AdMobAppID;
            if (AndroidConfig.EnableAdMobTestMode)
            {
                app_id = "ca-app-pub-3940256099942544~3347511713";
            }
            add.AdInitCode += "\n\t\t// --------- Initialization code Google AdMob --------------\n";
            add.AdInitCode += string.Format("\t\tString appid=\"\";\n");
            add.AdInitCode += CreateObfuscatedJavaString("appid", app_id, 50000) + "\n";
            add.AdInitCode += "\t\tMobileAds.initialize(this, appid);\n";
        }
        private bool AddChartboostLib()
        {
            if (IsLibraryAdded("chartboost"))
                return true;
            if (AddLibrary("chartboost")==false)
                return false;
            add.ManifestActivities += "\t<activity android:name=\"com.chartboost.sdk.CBImpressionActivity\" android:excludeFromRecents=\"true\" android:hardwareAccelerated=\"true\" android:theme=\"@android:style/Theme.Translucent.NoTitleBar.Fullscreen\" android:configChanges=\"keyboardHidden|orientation|screenSize\" />\n";
            return true;
        }
        private void PrepareInitializationDataForChartboost()
        {
            add.AdInitCode += "\n\t\t// --------- Initialization code Chartboost --------------\n";
            add.AdInitCode += string.Format("\t\tString CB_appID=\"\";\n");
            add.AdInitCode += string.Format("\t\tString CB_appSignature=\"\";\n");
            add.AdInitCode += CreateObfuscatedJavaString(string.Format("CB_appID"), AndroidConfig.ChartboostAppID, 60000) + "\n";
            add.AdInitCode += CreateObfuscatedJavaString(string.Format("CB_appSignature"), AndroidConfig.ChartboostAppSignature, 60001) + "\n";
            add.AdInitCode += "\t\tChartboostAd.Initialize(this, CB_appID, CB_appSignature);\n";
        }
        private bool PrepareChartboostAd(int index, AdType type, bool ReLoadAfterOpen, int MaxAttemptsOnFail, bool LoadOnStartup)
        {
            //initializare
            add.AdInitCode += string.Format("\t\tads[{0}] = new ChartboostAd();\n", index);
            add.AdInitCode += string.Format("\t\t((ChartboostAd)ads[{0}]).Init(this,{0},{1},{2});\n", index, ReLoadAfterOpen.ToString().ToLower(), MaxAttemptsOnFail);//layout,this,uid_{0},kw_{0},10);\n", index);
            switch (type)
            {
                case AdType.Interstitial:
                    add.AdInitCode += string.Format("\t\t((ChartboostAd)ads[{0}]).Create('I',null);\n", index);
                    break;
                case AdType.Rewardable:
                    add.AdInitCode += string.Format("\t\t((ChartboostAd)ads[{0}]).Create('R',null);\n", index);
                    break;
                case AdType.InPlay:
                    add.AdInitCode += string.Format("\t\t((ChartboostAd)ads[{0}]).Create('P',layout);\n", index);
                    break;
                default:
                    prj.EC.AddError("Chartboost", "No implementation available for ad format: "+type.ToString());
                    return false;
                    
            }
            add.AdInitCode += "\n";

            // autoload
            //if (LoadOnStartup)
            //    add.AdLoadCode += string.Format("\t\tads[{0}].Load();\n", index);
            add.AdLoadCode += "\n";

            add.JavaFiles["ChartboostAd.java"] = true;
            add.JavaFiles["IAndroidGenericAd.java"] = true;
            add.JavaFiles["GenericAd.java"] = true;

            return AddChartboostLib();
        }
 
        private bool PrepareAdsData()
        {
            Dictionary<AdProvider, bool> providers = new Dictionary<AdProvider, bool>();
            for (int tr = 0; tr < Build.Ads.Count; tr++)
                providers[Build.Ads[tr].Provider] = true;

            if (providers.ContainsKey(AdProvider.GoogleAdMob))
                PrepareInitializationDataForGoogleAdMob();

            if (providers.ContainsKey(AdProvider.Chartboost))
                PrepareInitializationDataForChartboost();

            for (int tr=0;tr<Build.Ads.Count;tr++)
            {
                add.AdInitCode += "\n\t\t// --------- Initialization code for : " + Build.Ads[tr].Name + "\n";

                bool result = false;
                if (Build.Ads[tr].GetType()==typeof(GoogleAdMobBanner))
                    result = PrepareGoogleAdMobBanner((GoogleAdMobBanner)Build.Ads[tr],tr); 
                if (Build.Ads[tr].GetType() == typeof(GoogleAdMobInterstitial))
                    result = PrepareGoogleAdMobInterstitial((GoogleAdMobInterstitial)Build.Ads[tr], tr);
                if (Build.Ads[tr].GetType() == typeof(GoogleAdMobRewardable))
                    result = PrepareGoogleAdMobRewardable((GoogleAdMobRewardable)Build.Ads[tr], tr);
                if (Build.Ads[tr].GetType() == typeof(GoogleAdMobNativeExpress))
                    result = PrepareGoogleAdMobNativeExpress((GoogleAdMobNativeExpress)Build.Ads[tr], tr);
                if (Build.Ads[tr].GetType() == typeof(ChartboostInterstitial))
                    result = PrepareChartboostAd(tr, Build.Ads[tr].GetAdTtype(), Build.Ads[tr].ReLoadAfterOpen, Build.Ads[tr].MaxAttemptsOnFail, Build.Ads[tr].LoadOnStartup);
                if (Build.Ads[tr].GetType() == typeof(ChartboostRewardable))
                    result = PrepareChartboostAd(tr, Build.Ads[tr].GetAdTtype(), Build.Ads[tr].ReLoadAfterOpen, Build.Ads[tr].MaxAttemptsOnFail, Build.Ads[tr].LoadOnStartup);
                if (Build.Ads[tr].GetType() == typeof(ChartboostInPlay))
                    result = PrepareChartboostAd(tr, Build.Ads[tr].GetAdTtype(), Build.Ads[tr].ReLoadAfterOpen, Build.Ads[tr].MaxAttemptsOnFail, Build.Ads[tr].LoadOnStartup);

                // verific rezultatul
                if (result==false)
                {
                    prj.EC.AddError("Error processing ad: " + Build.Ads[tr].Name + " of type: " + Build.Ads[tr].GetType().ToString());
                    return false;
                }
            }

            return true;
        }
        #endregion

        private bool PrepareInAppBilling()
        {
            if (AndroidConfig.BillingMarket != AndroidBillingMarket.None)
            {
                if (AndroidConfig.InAppItemsList.Trim().Length == 0)
                {
                    prj.EC.AddError("Billing", "You have a billing market set up but no in-app items. Fill out 'In-App items' field !");
                    return false;
                }
            }
            //if (AndroidConfig.BillingMarket == AndroidBillingMarket.GooglePlay)
            //    AddGooglePlayServicesLib();
            return true;
        }

        public override void OnBuild()
        {
            if (Disk.CleanDirectory(root, prj.EC) == false)
                return;
            if (PrepareFolders() == false)
                return;
            add = new ExtraData();
            if (PrepareAdsData() == false)
                return;
            if (PrepareAnalytics() == false)
                return;
            if (PrepareOtherServices() == false)
                return;
            if (PrepareGooglePlayServices() == false)
                return;
            if (PrepareInAppBilling() == false)
                return;
            if (Build.GenerateResources(Path.Combine(project_main, "assets", "resources.dat"), false) == false)
                return;
            if (Build.GenerateResourceCodeFiles() == false)
                return;
            if (CopySoundFiles() == false)
                return;
            if (CreateManifest() == false)
                return;
            if (CreateIcons() == false)
                return;
            if (CreateStringsXML() == false)
                return;
            if (CreateLayouts() == false)
                return;
            if (GenerateCppFiles() == false)
                return;
            if (GenerateJavaFiles() == false)
                return;
            if (CreateGradleFiles() == false)
                return;
            if (CopyExtraLibraries() == false)
                return;
            if (CreateExtraFiles() == false)
                return;
            // ultima parte a codului - care ruleaza pluginul de gradle
            if (RunGradle() == false)
                return;

            //if (CreateRFile() == false)
            //    return;
            //if (CompileJavaCode() == false)
            //    return;
            //if (AndroidConfig.JavaCodeObfuscation == AndroidJavaCodeObfuscation.Proguard)
            //{
            //    if (ObfuscateWithProguard() == false)
            //        return;
            //}
            //if (CreateDexFile() == false)
            //    return;
            //if (BuildCppFiles() == false)
            //    return;
            //if (CreateUnsignedAPK() == false)
            //    return;
            //if (CreateSignedAPK() == false)
            //    return;
            //if (ZipAlignAPK() == false)
            //    return;

        }
    }
}

// OLD code
//private bool CreateEclipseProject()
//{
//    //if (!AndroidConfig.GenerateEclipseProject)
//    //    return true;            
//    /*
//    task.CreateSubTask("Creating Eclipse project ...");
//    while (true)
//    {
//        if (Project.CreateResource("EclipseAndroidClasspathFile.xml", null, Path.Combine(root, ".classpath"), prj.EC) == false)
//            break;
//        if (Project.CreateResource("EclipseProjectFile.xml", new Dictionary<string, string>() { { "$$PROJECTNAME$$", prj.GetProjectName() } }, Path.Combine(root, ".project"), prj.EC) == false)
//            break;
//        if (Disk.SaveFile(Path.Combine(root, "prject.properties"), "target = android-" + AndroidConfig.TargetSDKVersion.ToString(), prj.EC) == false)
//            break;
//        return task.UpdateSuccessErrorState(true); 
//    }
//    return task.UpdateSuccessErrorState(false);
//     */
//    return true;
//}
//private bool CreateRFile()
//{
//    task.CreateSubTask("Create R.Java file ...");            
//    string param = "package -v -f -m -S ./res -J ./src -M ./AndroidManifest.xml -I " + "\"" + GetAndroidSDKPlatform() + "\"";
//    string res = prj.ShellExecute(prj.Settings.AndroidAssetPackagingTool, param, root);

//    if (File.Exists(Path.Combine(GetSourceJavaFolder(), "R.java")) == false)
//    {
//        prj.EC.AddError("R.Java was not created !!!");
//        prj.EC.AddError(res);
//        return task.UpdateSuccessErrorState(false);                
//    }
//    return task.UpdateSuccessErrorState(true);            
//}
//private bool CompileJavaCode()
//{
//    //-sourcepath .\src ".\src\com\test\abc\GSound.java" ".\src\com\test\abc\OpenGLView.java" ".\src\com\test\abc\MainActivity.java" ".\src\com\test\abc\R.java"
//    task.CreateSubTask("Compiling Java Code ...");
//    string param = "-Xmaxerrs 50000  -d \"" + Path.Combine(root, "obj") + "\" -classpath \"";
//    param += GetAndroidSDKPlatform() + ";" + Path.Combine(root, "obj");
//    foreach (string name in add.Libs.Keys)
//        param += ";" + Path.Combine(root, "libs", add.Libs[name]);
//    param += "\" -sourcepath .\\src ";
//    foreach (string fname in Directory.EnumerateFiles(GetSourceJavaFolder()))
//    {
//        if (fname.ToLower().EndsWith(".java"))
//            param += "\"" + fname + "\" ";
//        task.Info(fname);
//    }
//    //if (AndroidConfig.BillingMarket == AndroidBillingMarket.GooglePlay)
//    //{
//    //    param += "\"" + Path.Combine(root, "src", "com\\android\\vending\\billing\\IInAppBillingService.aidl") +"\" ";
//    //    task.Info(Path.Combine(root, "src", "com\\android\\vending\\billing\\IInAppBillingService.aidl"));
//    //}
//    task.Info("Running: javac.exe " + param);
//    string res = prj.ShellExecute(Path.Combine(prj.Settings.JavaFolder,"javac.exe"), param, root);
//    // verific daca s-au facut fisiere .class
//    // return 
//    foreach (string fname in Directory.EnumerateFiles(GetSourceJavaFolder()))
//    {
//        if (fname.ToLower().EndsWith("iinappbillingservice.java"))
//            continue; // skip la billing
//        if (fname.ToLower().EndsWith(".java"))
//        {
//            string className = Path.Combine(GetClassFolder(), Path.GetFileNameWithoutExtension(fname) + ".class");
//            if (File.Exists(className) == false)
//            {
//                prj.EC.AddError("File: " + fname + " was not compiled into a .class file\n"+res);
//                task.UpdateSuccessErrorState(false);
//                return false;
//            }
//        }
//    }
//    task.UpdateSuccessErrorState(true);
//    return true;
//}
//private bool ObfuscateWithProguard()
//{
//    task.CreateSubTask("Obfuscating java classes (Proguard)");
//    Dictionary<string, string> d = new Dictionary<string, string>();
//    d["$$PACKAGE$$"] = AndroidConfig.Package;
//    d["$$PROJECT_OBJ_FOLDER$$"] = "\"" + Path.Combine(root, "obj") + "\"";
//    d["$$PROJECT_OBFUS_FOLDER$$"] = "\""+Path.Combine(root, "proguard") + "\"";
//    d["$$LIBRARIES$$"] = "-libraryjars \"" + GetAndroidSDKPlatform()+"\"\n";
//    foreach (string name in add.Libs.Keys)
//        d["$$LIBRARIES$$"] += "-libraryjars \"" + Path.Combine(root, "libs", add.Libs[name]) + "\"\n";

//    if (Project.CreateResource("Android","proguard.txt",d,Path.Combine(root,"proguard.txt"),prj.EC)==false)
//        return task.UpdateSuccessErrorState(false);
//    string content = prj.ShellExecute(Path.Combine(prj.Settings.JavaFolder, "java.exe"), "-jar \"" + Path.Combine(prj.Settings.ProguardPath, "lib", "proguard.jar") + "\" @proguard.txt", root);
//    if (content == null)
//        return task.UpdateSuccessErrorState(false);
//    // verific daca proguard a functionat
//    if (File.Exists(Path.Combine(root, "proguard", AndroidConfig.Package.Replace(".", "\\"),"MainActivity.class"))==false)
//    {
//        prj.EC.AddError("Proguard failed:\n" + content);
//        return task.UpdateSuccessErrorState(false);
//    }
//    // daca da - mut clasele noi in locul celor vechi
//    // sterg tot ce e in folderul initial
//    List<string> files = new List<string>();
//    if (Disk.GetAllFiles(GetClassFolder(),false,files,prj.EC)==false)
//        return task.UpdateSuccessErrorState(false);
//    foreach (string fname in files)
//    {
//        //Console.WriteLine(fname);
//        if (fname.ToLower().EndsWith(".class"))
//            if (Disk.DeleteFile(fname, prj.EC) == false)
//                return task.UpdateSuccessErrorState(false);
//    }         
//    // copii clasele noi
//    if (Disk.GetAllFiles(Path.Combine(root,"proguard"), true, files, prj.EC) == false)
//        return task.UpdateSuccessErrorState(false);
//    // le copii in sursa
//    foreach (string fname in files)
//    {                
//        if (fname.ToLower().EndsWith(".class") == false)
//            continue;
//        string resName = Path.Combine(GetClassFolder(), Path.GetFileName(fname));
//        if (File.Exists(resName))
//        {
//            prj.EC.AddError("Obfuscation naming error - duplicate class: " + resName);
//            return task.UpdateSuccessErrorState(false);
//        }
//        if (Disk.Copy(fname,resName,prj.EC)==false)
//            return task.UpdateSuccessErrorState(false);
//    }
//    // all good - am iesit
//    return task.UpdateSuccessErrorState(true);
//}
//private bool CreateDexFile()
//{
//    task.CreateSubTask("Create 'classes.dex' ");
//    string param = "--dex --no-strict --verbose --output=\"" + Path.Combine(root, "bin", "classes.dex") + "\" \"" + GetClassFolder() + "\" ";
//    if (AndroidConfig.BillingMarket == AndroidBillingMarket.GooglePlay)
//    {
//        param += " \""+Path.Combine(root,"obj\\com\\android\\vending\\billing")+"\" ";
//    }
//    foreach (string name in add.Libs.Keys)
//        param += " \"" + Path.Combine(root, "libs", add.Libs[name]) + "\"  ";
//    string bat_file = "@" + root.Substring(0, 2) + "\n@cd " + root + "\n@call \"" + prj.Settings.AndroidDexTool + "\" "+param+" \n@exit";
//    if (Disk.SaveFile(Path.Combine(root, "create-dex.bat"), bat_file, prj.EC) == false)
//        return task.UpdateSuccessErrorState(false);
//    string content = prj.ShellExecute("C:\\Windows\\System32\\cmd.exe", "/c \"" + Path.Combine(root, "create-dex.bat") + "\"", root);
//    if (content == null)
//        return task.UpdateSuccessErrorState(false);
//    if (File.Exists(Path.Combine(root, "bin", "classes.dex")) == false)
//    {
//        prj.EC.AddError("classes.dex was not created ...");
//        prj.EC.AddError(content);
//        return task.UpdateSuccessErrorState(false);
//    }
//    return task.UpdateSuccessErrorState(true);
//}
//private bool CreateUnsignedAPK()
//{
//    task.CreateSubTask("Create APK ...");
//    string param = "package -v -f -M .\\AndroidManifest.xml -S .\\res -A .\\assets -I " + "\"" + GetAndroidSDKPlatform() + "\" -F .\\bin\\unsigned.apk .\\bin ";
//    string res = prj.ShellExecute(prj.Settings.AndroidAssetPackagingTool, param, root);
//    string apk = Path.Combine(root, "bin", "unsigned.apk");
//    task.Info("Unsigned APK File:" + apk);
//    if (File.Exists(apk) == false)
//    {
//        prj.EC.AddError("unsigned.apk was not created !!!");
//        prj.EC.AddError(res);
//        return task.UpdateSuccessErrorState(false);
//    }
//    // totul e ok - trebuie sa bag si libraria
//    if (Disk.CreateFolder(Path.Combine(root, "lib", "armeabi"), prj.EC) == false)
//        return task.UpdateSuccessErrorState(false);
//    if (Disk.Copy(Path.Combine(root, "libs", "armeabi", "libframework.so"), Path.Combine(root, "lib", "armeabi", "libframework.so"),prj.EC) == false)
//        return task.UpdateSuccessErrorState(false);
//    param = "a -v .\\bin\\unsigned.apk lib/armeabi/libframework.so ";
//    long sz1 = Disk.GetFileSize(apk, prj.EC);
//    if (prj.EC.HasErrors())
//        return task.UpdateSuccessErrorState(false);
//    res = prj.ShellExecute(prj.Settings.AndroidAssetPackagingTool, param, root);
//    long sz2 = Disk.GetFileSize(apk, prj.EC);
//    if (prj.EC.HasErrors())
//        return task.UpdateSuccessErrorState(false);
//    if (sz2 <= sz1)
//    {
//        prj.EC.AddError("libframework.so was not added to unsigned.apk !!!");
//        prj.EC.AddError(res);
//        return task.UpdateSuccessErrorState(false);
//    }
//    return task.UpdateSuccessErrorState(true); 
//}
//private bool CreateSignedAPK()
//{
//    task.CreateSubTask("Signing APK");
//    string param = "-verbose -sigalg SHA1withRSA -digestalg SHA1 -keystore \"" + prj.Settings.AndroidSignKeystore + "\" -storepass " + prj.Settings.AndroidSignPass + " -keypass " + prj.Settings.AndroidSignPass + " -signedjar .\\bin\\signed.apk .\\bin\\unsigned.apk " + prj.Settings.AndroidKeystoreAlias;
//    string res = prj.ShellExecute(Path.Combine(prj.Settings.JavaFolder,"jarsigner.exe"), param, root);
//    string apk = Path.Combine(root, "bin", "signed.apk");
//    task.Info("Sign APK File:" + apk);
//    if (File.Exists(apk) == false)
//    {
//        prj.EC.AddError("Signed.apk was not created - the APK was not signed!!!");
//        prj.EC.AddError(res);
//        return task.UpdateSuccessErrorState(false);
//    }
//    return task.UpdateSuccessErrorState(true);
//}
//private bool ZipAlignAPK()
//{
//    task.CreateSubTask("Optimizing the APK");
//    string apk =  GetOutputFile(Path.Combine(root, "bin"), Path.Combine(AndroidConfig.Package+"."+prj.Version+".apk"));
//    task.Info("APK File:" + apk);
//    string param = "-v -f 4 .\\bin\\signed.apk \"" + apk + "\"";
//    string res = prj.ShellExecute(prj.Settings.AndroidZipAlign, param, root);

//    if (File.Exists(apk) == false)
//    {
//        prj.EC.AddError(apk + " was not created!!!");
//        prj.EC.AddError(res);
//        return task.UpdateSuccessErrorState(false);
//    }
//    return task.UpdateSuccessErrorState(true);
//}
