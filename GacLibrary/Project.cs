using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;
using System.ComponentModel;
using System.Threading;
using System.IO.Compression;
using System.Globalization;


namespace GAppCreator
{
    public enum SplashScreenImageResizeMode
    {
        NoResize,
        Fill,
        Fit,
    };
    public class Project
    {
        public class IconInfo
        {
            public string Source;
            public string Output;
            public int Size;
        };
        public class ProjectIcons
        {
            public Dictionary<string, string> Sources = new Dictionary<string, string>();
            public Dictionary<string,IconInfo> OutputFiles = new Dictionary<string,IconInfo>();
            public Dictionary<int, string> Sizes = new Dictionary<int, string>();
            public String ProjectIcon="";
            public String BuildIcon = "";
        };
        [XmlArrayItem(typeof(ImageResource))]        
        [XmlArrayItem(typeof(FontResource))]
        [XmlArrayItem(typeof(RawResource))]
        [XmlArrayItem(typeof(SoundResource))]
        [XmlArrayItem(typeof(ShaderResource))]
        [XmlArrayItem(typeof(PresentationResource))] 
        public List<GenericResource> Resources = new List<GenericResource>();


        [XmlArrayItem(typeof(TextPublish))]
        [XmlArrayItem(typeof(ImagePublish))]
        [XmlArrayItem(typeof(VideoPublish))] 
        public List<PublishObject> PublishData = new List<PublishObject>();

        [XmlArrayItem(typeof(GoogleAdMobBanner))]
        [XmlArrayItem(typeof(GoogleAdMobInterstitial))]
        [XmlArrayItem(typeof(GoogleAdMobRewardable))]
        [XmlArrayItem(typeof(GoogleAdMobNativeExpress))]
        [XmlArrayItem(typeof(ChartboostInterstitial))]
        [XmlArrayItem(typeof(ChartboostRewardable))]
        [XmlArrayItem(typeof(ChartboostInPlay))]
        public List<GenericAd> Ads = new List<GenericAd>();

        public List<Profile> Profiles = new List<Profile>();
        public List<ProjectTask> Tasks = new List<ProjectTask>();

        public List<StringValues> Strings = new List<StringValues>();
        public List<StringTemplatePreview> StringTemplates = new List<StringTemplatePreview>();

        public List<ProjectFile> Files = new List<ProjectFile>();
        public List<Enumeration> Enums = new List<Enumeration>();
        public List<Structure> Structs = new List<Structure>();
        public List<ConstantValue> Constants = new List<ConstantValue>();

        [XmlArrayItem(typeof(DevelopBuildConfiguration))]
        [XmlArrayItem(typeof(AndroidBuildConfiguration))]
        [XmlArrayItem(typeof(IOSBuildConfiguration))]
        [XmlArrayItem(typeof(MacBuildConfiguration))]
        [XmlArrayItem(typeof(WindowsDesktopBuildConfiguration))]
        public List<GenericBuildConfiguration> BuildConfigurations = new List<GenericBuildConfiguration>();

        public ImageResource IconR = new ImageResource();
        public ImageResource SplashScreen = new ImageResource();
        public StringValues ApplicationName = new StringValues();

        public List<DebugCommand> DebugCommands = new List<DebugCommand>();

        public List<ControlID> ControlsIDs = new List<ControlID>();
        public List<ControlID> ObjectEventsIDs = new List<ControlID>();
        public List<Counter> Counters = new List<Counter>();
        public List<CounterGroup> CountersGroups = new List<CounterGroup>();
        public List<Alarm> Alarms = new List<Alarm>();

        public List<AnimO.AnimationObject> AnimationObjects = new List<AnimO.AnimationObject>();

        public DeveloperSettingsSnapshots SettingsSnapshots = new DeveloperSettingsSnapshots();

        [XmlAttribute()]
        public string Version = "1.1.0";
        [XmlAttribute()]
        public string Name = "";
        [XmlAttribute()]
        public string FaceBook = "";
        [XmlAttribute()]
        public string Youtube = "";
        [XmlAttribute()]
        public string Twitter = "";
        [XmlAttribute()]
        public string Instagram = "";
        [XmlAttribute()]
        public string Webpage = "";
        [XmlAttribute()]
        public string DesignResolution = "";
        [XmlAttribute()]
        public Language DefaultLanguage = Language.English;
        [XmlAttribute()]
        public String Icon = "";
        //[XmlAttribute()]
        //public long RunTime = 0;
        //[XmlAttribute()]
        //public long ActiveTime = 0;
        [XmlAttribute()]
        public string Description = "";
        [XmlAttribute()]
        public string Copyright = "";
        [XmlAttribute()]
        public string Company = "";
        [XmlAttribute()]
        public string Defines = "";

        [XmlAttribute()]
        public float SplashScreenWidth = 100.0f, SplashScreenHeight = 100.0f;
        [XmlAttribute()]
        public SplashScreenImageResizeMode SplashScreenResizeMode = SplashScreenImageResizeMode.NoResize;
        [XmlAttribute()]
        public int SplashScreenMinimalShowTime = 2;
        [XmlAttribute()]
        public bool SplashScreenAnimation = false;
        [XmlAttribute()]
        public string SplashScreenAnimationSpriteSize = "";
        [XmlAttribute()]
        public int SplashScreenAnimationSprites = 0;

        
        
        [XmlAttribute()]   
        public string PluginList = "";

          



        [XmlIgnore()]
        public string ProjectPath = "";
        [XmlIgnore()]
        public ErrorsContainer EC = new ErrorsContainer();
        [XmlIgnore()]
        public SystemSettings Settings;
        [XmlIgnore()]
        public Size DesignResolutionSize;
        [XmlIgnore()]
        public static System.Globalization.CultureInfo defaultLocale = null;
        [XmlIgnore()]
        public PluginList Plugins = null;


        protected internal static fnShowError ShowErrorFunction = null;

        #region Atribute
        [XmlIgnore(), Description("Current version (Major.Minor.Build)"), Category("General"), DisplayName("Version")]
        public string _Version
        {
            get { return Version; }
            set {
                if (Project.ValidateStringVersionCorectness(value) == false)
                    MessageBox.Show("Invalid version (should be Major.Minor.Build,where Major is bigger than 1 and smaller than 255, Minor is bigger than 0 and smaller than 255 and Build is bigger than 0 and smaller than 65535");
                else
                    Version = value;
            }
        }
        [XmlIgnore(), Description("Icon source file for the entire project !"), Category("General"), DisplayName("Icon")]
        public string _Icon
        {
            get { return Icon; }
        }
        [XmlIgnore(), Description("Project name"), Category("General"), DisplayName("Name")]
        public string _Name
        {
            get { return GetProjectName(); }            
        }
        [XmlIgnore(), Description("Description of the project"), Category("General"), DisplayName("Description")]
        public string _Description
        {
            get { return Description; }
            set { Description = value; }
        }
        [XmlIgnore(), Description("Copyright"), Category("General"), DisplayName("Copyright")]
        public string _Copyright
        {
            get { return Copyright; }
            set { Copyright = value; }
        }
        [XmlIgnore(), Description("Company"), Category("General"), DisplayName("Company")]
        public string _Company
        {
            get { return Company; }
            set { Company = value; }
        }
        [XmlIgnore(), Description("Facebook"), Category("Social network"), DisplayName("Facebook page id")]
        public string _Facebook
        {
            get { return FaceBook; }
            set { FaceBook = value; }
        }
        [XmlIgnore(), Description("Youtube"), Category("Social network"), DisplayName("Youtube address")]
        public string _Youtube
        {
            get { return Youtube; }
            set { Youtube = value; }
        }
        [XmlIgnore(), Description("Twitter"), Category("Social network"), DisplayName("Twitter acount")]
        public string _Twitter
        {
            get { return Twitter; }
            set { Twitter = value; }
        }
        [XmlIgnore(), Description("Instagram"), Category("Social network"), DisplayName("Instagram acount")]
        public string _Instagram
        {
            get { return Instagram; }
            set { Instagram = value; }
        }
        [XmlIgnore(), Description("Webpage"), Category("Social network"), DisplayName("Webpage address")]
        public string _Webpage
        {
            get { return Webpage; }
            set { Webpage = value; }
        }
        [XmlIgnore(), Description("Percentage of screen width"), Category("Splash Screen"), DisplayName("Screen Width")]
        public float _SplashScreenWidth
        {
            get { return SplashScreenWidth; }
            set { SplashScreenWidth = value; }
        }
        [XmlIgnore(), Description("Percentage of screen height"), Category("Splash Screen"), DisplayName("Screen Height")]
        public float _SplashScreenHeight
        {
            get { return SplashScreenHeight; }
            set { SplashScreenHeight = value; }
        }
        [XmlIgnore(), Description("Resize Method"), Category("Splash Screen"), DisplayName("Resize Method")]
        public SplashScreenImageResizeMode _ResizeMode
        {
            get { return SplashScreenResizeMode; }
            set { SplashScreenResizeMode = value; }
        }
        [XmlIgnore(), Description("Minim time for the splash screen to be visible in seconds (0 or a negative value indicates that the moment the background operaions are completed the splash scrren will be closed)."), Category("Splash Screen"), DisplayName("Show time (seconds)")]
        public int _SplashScreenMinimalShowTime
        {
            get { return SplashScreenMinimalShowTime; }
            set { SplashScreenMinimalShowTime = value; }
        }
        [XmlIgnore(), Description("Enable animation"), Category("Splash Screen"), DisplayName("Enable Animation")]
        public bool _SplashScreenAnimation
        {
            get { return SplashScreenAnimation; }
            set { SplashScreenAnimation = value; }
        }
        [XmlIgnore(), Description("Sets animation sprite size (must be a power of 2 -> 32 x 32, 64 x 64 , 128 x 64, etc)"), Category("Splash Screen"), DisplayName("Animation sprite size")]
        public string _SplashScreenAnimationSpriteSize
        {
            get { return SplashScreenAnimationSpriteSize; }
            set {
                int w = 0, h = 0;
                if (Project.SizeToValues(value, ref w, ref h))
                    SplashScreenAnimationSpriteSize = Project.ResolutionToString(w, h);
                SplashScreenAnimationSpriteSize = value; 
            }
        }
        [XmlIgnore(), Description("Number of sprites"), Category("Splash Screen"), DisplayName("Animation sprites count")]
        public int _SplashScreenAnimationSprites
        {
            get { return SplashScreenAnimationSprites; }
            set { SplashScreenAnimationSprites = value; }
        }

        


        [XmlIgnore(), Description("Design Resolution"), Category("Resources"), DisplayName("Design Resolution")]
        public string _DesignResolution
        {
            get { return DesignResolution; }
        }
        [XmlIgnore(), Description("Default Language"), Category("Resources"), DisplayName("Default Language")]
        public Language _DefaultLanguage
        {
            get { return DefaultLanguage; }
        }
        #endregion

        public void SetErrorFunction(fnShowError fnc)
        {
            ShowErrorFunction = fnc;
        }
        public void ShowErrors()
        {
            if (EC.HasErrors())
            {
                if (ShowErrorFunction != null)
                    ShowErrorFunction(EC);
            }
            EC.Reset();
        }
        public void UpdateResourceStatus()
        {
            foreach (GenericResource r in Resources)
            {
                r.prj = this;
            }
            Resources.Sort();
        }
        public void UpdatePublishObjectStatus()
        {
            foreach (PublishObject r in PublishData)
            {
                r.prj = this;
            }
            PublishData.Sort();
        }
        public void AddResources(List<GenericResource> lst)
        {
            foreach (GenericResource r in lst)
            {
                r.prj = this;
                Resources.Add(r);                
            }
            Resources.Sort();
        }
        public void AddResource(GenericResource r)
        {
            r.prj = this;
            if ((r.Builds == null) || (r.Builds.Length == 0))
                r.Builds = BuildConfigurations[0].Name;
            Resources.Add(r);
            Resources.Sort();
        }
        public bool AddResources(string[] names,List<GenericResource> ResourcesAdded)
        {
            if (names == null)
                return false;
            ResourcesAdded.Clear();
            foreach (string name in names)
            {
                switch (FileNameToResourceType(name))
                {
                    case ResourceType.VectorImage:
                    case ResourceType.RasterImage:
                        ImageResource rir = new ImageResource();
                        rir.Create(name, 1.0f, "");
                        rir.DesignResolution = DesignResolution;
                        rir.Lang = DefaultLanguage;
                        rir.Builds = BuildConfigurations[0].Name;
                        Resources.Add(rir);
                        ResourcesAdded.Add(rir);
                        break;
                    case ResourceType.Raw:
                        RawResource raw = new RawResource();
                        raw.Create(name);
                        raw.Lang = DefaultLanguage;
                        raw.Builds = BuildConfigurations[0].Name;
                        Resources.Add(raw);
                        ResourcesAdded.Add(raw);
                        break;
                    case ResourceType.Music:
                        SoundResource sr = new SoundResource();
                        sr.Create(name);
                        sr.Lang = DefaultLanguage;
                        sr.Builds = BuildConfigurations[0].Name;
                        Resources.Add(sr);
                        ResourcesAdded.Add(sr);
                        break;
                    case ResourceType.Presentation:
                        PresentationResource ar = new PresentationResource();
                        ar.Create(name);
                        ar.Lang = DefaultLanguage;
                        ar.Builds = BuildConfigurations[0].Name;
                        Resources.Add(ar);
                        ResourcesAdded.Add(ar);
                        break;
                }
            }
            UpdateResourceStatus();
            ShowErrors();
            return true;
        }
        public bool Save()
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Project));
                TextWriter textWriter = new StreamWriter(Path.Combine(ProjectPath,"project.gappcreator"));
                serializer.Serialize(textWriter, this);
                textWriter.Close();
                return true;
            }
            catch (Exception e)
            {
                EC.AddError("Unable to save project file - " + e.ToString());
                ShowErrors();
                return false;
            }
        }
        public bool RunCommand(string application, string parameters, string applicationName, bool waitForExit,bool hideWindow)
        {
            if ((application == null) || (application.Length == 0) || (application.ToLower().EndsWith(".exe") == false))
            {
                EC.AddError("Unable to run application " + applicationName + " ! Invalid path for an executable !");
                return false;
            }
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.Arguments = parameters;
                psi.FileName = application;
                if (hideWindow)
                    psi.WindowStyle = ProcessWindowStyle.Hidden;
                Process p = Process.Start(psi);
                if (p == null)
                {
                    EC.AddError("Unable to start process: " + application + "\nParams: " + parameters);
                    return false;
                }
                if (waitForExit)
                    p.WaitForExit();
                return true;
            }
            catch (Exception e)
            {
                EC.AddException("Unable to start process: " + application + "\nParams: " + parameters, e);
                return false;
            }
        }
        public string ShellExecute(string command, string parameters, string workingDirectory)
        {
            return ShellExecute(command, parameters, workingDirectory, 5 * 60); // 5 minute
        }
        public string ShellExecute(string command, string parameters, string workingDirectory,int secondsTimeOut)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.Arguments = parameters;
                psi.UseShellExecute = false;                
                psi.RedirectStandardOutput = true;
                if ((workingDirectory != null) && (workingDirectory.Length > 0))
                    psi.WorkingDirectory = workingDirectory;
                psi.RedirectStandardError = true;
                
                psi.CreateNoWindow = true;
                psi.FileName = command;

                Process p = new Process();
                p.StartInfo = psi;
                //Process p = Process.Start(psi);
                if (p == null)
                {
                    EC.AddError("Unable to execute: " + command + "\nParams: " + parameters);
                    return null;
                }

                StringBuilder output = new StringBuilder();
                StringBuilder error = new StringBuilder();

                using (AutoResetEvent outputWaitHandle = new AutoResetEvent(false))
                using (AutoResetEvent errorWaitHandle = new AutoResetEvent(false))
                {
                    p.OutputDataReceived += (sender, e) =>
                    {
                        if (e.Data == null)
                        {
                            outputWaitHandle.Set();
                        }
                        else
                        {
                            output.AppendLine(e.Data);
                        }
                    };
                    p.ErrorDataReceived += (sender, e) =>
                    {
                        if (e.Data == null)
                        {
                            errorWaitHandle.Set();
                        }
                        else
                        {
                            error.AppendLine(e.Data);
                        }
                    };

                    p.Start();

                    p.BeginOutputReadLine();
                    p.BeginErrorReadLine();

                    if (p.WaitForExit(secondsTimeOut*1000) &&
                        outputWaitHandle.WaitOne(secondsTimeOut * 1000) &&
                        errorWaitHandle.WaitOne(secondsTimeOut * 1000))
                    {
                        // Process completed. Check process.ExitCode here.
                        return (output.ToString() + "\r\n" + error.ToString()).Trim();
                    }
                    else
                    {
                        EC.AddError("Time out on execution of: " + command + "\r\n" + (output.ToString() + "\r\n" + error.ToString()).Trim());
                        return null;
                    }
                }
            }
            catch (Exception e)
            {
                EC.AddException("Unable to execute: " + command + "\nParams: " + parameters + "\n",e);
                return null;
            }
        }
        public string GetProjectName()
        {
            return Path.GetFileName(ProjectPath);
        }
        public string GetProjectResourceSourceFullPath(string fileName)
        {
            return GetProjectResourceSourceFullPath(ProjectPath, fileName);
        }
        public string GetProjectResourceOutputFullPath(string fileName)
        {
            return GetProjectResourceOutputFullPath(ProjectPath, fileName);
        }
        public string GetProjectResourceSourceFolder()
        {
            return Path.Combine(ProjectPath, "Resources", "Sources");
        }
        public string GetProjectFontTemplatesFolder()
        {
            return Path.Combine(ProjectPath, "Resources", "FontTemplates");
        }
        public string GetProjectResourcePluginsFolder()
        {
            return Path.Combine(ProjectPath, "Resources", "Plugins");
        }
        public string GetProjectPluginsCodeFolder()
        {
            return Path.Combine(ProjectPath, "PluginsCode");
        }
        public string GetProjectPublishMaterialsWorkGroupFolder()
        {
            return Path.Combine(ProjectPath, "PublishMaterials", "WorkGroup");
        }
        public bool SVGtoPNG(string svgFile, string pngFile, float DPI, int Width, int Height, float Scale, bool waitForExit)
        {
            String procParams = "\"" + svgFile + "\" --export-png=\"" + pngFile + "\"";
            if (DPI > 0)
            {
                procParams += " --export-dpi=" + (DPI * Scale).ToString();
            }
            else
            {
                if (Width > 0)
                    procParams += " --export-width=" + ((int)(Width * Scale)).ToString();
                if (Height > 0)
                    procParams += " --export-height=" + ((int)(Height * Scale)).ToString();
            }
            //procParams += " --export-background-opacity=" + vi._opacity.ToString();
            return RunCommand(Settings.InskapePath, procParams, "Inkscape", waitForExit,true);
        }
        public bool ResizeSVGToDrawing(string svgFile,bool waitForExit)
        {
            return RunCommand(Settings.InskapePath, " --verb=FitCanvasToDrawing --verb=FileSave --verb=FileClose " + svgFile, "Inkscape", waitForExit,true);
        }


        public Dictionary<Language,bool> GetBuildAvailableLanguages(GenericBuildConfiguration build,bool allowAllAvailableLanguagesFlags = true)
        {
            if ((build.AllAvailableLanguages) && (allowAllAvailableLanguagesFlags))
                return GetProjectAvailableLanguages();
            List<string> l = StringListToList(build.BuildLanguages);
            Dictionary<Language, bool> d = new Dictionary<Language, bool>();
            d[this.DefaultLanguage] = true;
            foreach (string lng in l)
            {
                Language _lng = this.DefaultLanguage;
                if (Enum.TryParse<Language>(lng, out _lng))
                    d[_lng] = true;
            }
            return d;
        }
        public Dictionary<Language,bool> GetProjectAvailableLanguages()
        {
            Dictionary<Language, bool> d = new Dictionary<Language, bool>();
            foreach (Language l in Enum.GetValues(typeof(Language)))
            {
                foreach (StringValues sv in this.Strings)
                {
                    if (sv.Get(l).Length > 0)
                    {
                        d[l] = true;
                        break;
                    }
                }
            }
            return d;
        }

        public Dictionary<string, GenericResource> GetBaseResources()
        {
            Dictionary<string, GenericResource> d = new Dictionary<string, GenericResource>();
            foreach (GenericResource r in Resources)
                if (r.IsBaseResource())
                    d[r.GetResourceVariableKey()] = r;
            return d;
        }
        public Dictionary<string,StringValues> GetStringResources()
        {
            Dictionary<string, StringValues> d = new Dictionary<string, StringValues>();
            foreach (StringValues sv in Strings)
            {
                d[sv.GetVariableNameWithArray()] = sv;
            }
            return d;
        }
        public AppResources GetAppResources()
        {
            AppResources d = new AppResources();
            foreach (GenericResource r in Resources)
            {
                if (r.IsBaseResource() == false)
                    continue;
                if (r.GetType() == typeof(ImageResource))
                    d.Images[r.GetResourceVariableName()] = (ImageResource)r;
                if (r.GetType() == typeof(FontResource))
                    d.Fonts[r.GetResourceVariableName()] = (FontResource)r;
                if (r.GetType() == typeof(ShaderResource))
                    d.Shaders[r.GetResourceVariableName()] = (ShaderResource)r;
            }
            // stringuri
            foreach (StringValues sv in Strings)
            {
                d.Strings[sv.GetVariableNameWithArray()] = sv;
            }
            return d;
        }
        
        public int GetBuildIndex(string name)
        {
            for (int tr = 0; tr < BuildConfigurations.Count; tr++)
                if (BuildConfigurations[tr].Name.ToLower() == name.ToLower())
                    return tr;
            return -1;
        }
        public GenericBuildConfiguration GetBuild(string name)
        {
            int index = GetBuildIndex(name);
            if (index < 0)
                return null;
            return BuildConfigurations[index];
        }
        public GZipPackage CreateProjectPackage(bool includeBuildResources,string buildName,bool failIfCriticalFilesAreMissing)
        {
            GZipPackage gzp = new GZipPackage();
            GenericBuildConfiguration gb = null;
            // build
            if ((buildName != null) && (buildName.Length > 0))
            {
                gb = GetBuild(buildName);
                // NU ARE SENS eroarea. Daca creez un build nou, pe server nu exista si o sa dea tot timpul fail.
                //if (gb == null)
                //{
                //    EC.AddError("Invalid build name: " + buildName);
                //    return null;
                //}
            }
            // fisierele
            foreach (ProjectFile pf in Files)
            {
                if (failIfCriticalFilesAreMissing == false)
                {
                    if (File.Exists(Path.Combine(ProjectPath, "Sources", pf.Name))==false)
                        continue;
                }
                if (gzp.Add(Path.Combine(ProjectPath, "Sources", pf.Name), "Sources\\" + pf.Name, EC) == false)
                    return null;
            }
            // resursele
            Dictionary<string, bool> d = new Dictionary<string, bool>();
            Dictionary<string, bool> res = new Dictionary<string, bool>();
            int name_relative_source = Path.Combine(ProjectPath, "Resources", "Sources").Length;
            int name_relative_output = Path.Combine(ProjectPath, "Resources", "Output").Length;
            foreach (GenericResource r in Resources)
            {
                if (r.GetType() == typeof(FontResource))
                {
                    FontResource fnt = (FontResource)r;
                    
                    foreach (Glyph g in fnt.Glyphs)
                    {
                        
                        d[fnt.GetGlyphSourcePath(g.Code).Substring(name_relative_source+1)] = true;
                        Glyph.GlyphVersionInfo vi = g.GetVersion();
                        foreach (string rez in vi.Rezolutions.Keys)
                            res[fnt.GetGlyphOutputPath(g.Code, rez).Substring(name_relative_output+1)] = true;
                    }
                }
                else
                {
                    if ((r.Source != null) && (r.Source.Length > 0))
                        d[r.Source] = true;
                    string s = r.GetOutputFileName();
                    if (s != null)
                        res[Path.GetFileName(s)] = true;
                }
                
            }
            //if ((Icon.Source != null) && (Icon.Source.Length > 0))
            //    d[Icon.Source] = true;
            if (Icon.Length > 0)
                d[Icon] = true;
            foreach (string key in d.Keys)
            {
                if (failIfCriticalFilesAreMissing == false)
                {
                    if (File.Exists(Project.GetProjectResourceSourceFullPath(ProjectPath, key)) == false)
                        continue;
                }
                if (gzp.Add(Project.GetProjectResourceSourceFullPath(ProjectPath, key), "Resources\\Sources\\" + key, EC) == false)
                    return null;
            }
            // resursele builduite
            if (includeBuildResources)
            {
                // cele din output
                foreach (string key in res.Keys)
                {
                    // daca fisierul nu exista nu il pun in arhiva - nu e necesar pentru arhiva
                    if (File.Exists(Project.GetProjectResourceOutputFullPath(ProjectPath,key)))
                    {
                        if (gzp.Add(Project.GetProjectResourceOutputFullPath(ProjectPath, key), "Resources\\Output\\" + key, EC) == false)
                            return null;
                    }
                }
                //  icons
                ProjectIcons Icons;
                if (gb != null)
                    Icons = GetBuildIcons(gb);
                else
                    Icons = GetProjectIcons();
                foreach (string s in Icons.OutputFiles.Keys)
                {
                    if (File.Exists(s))
                    {
                        if (gzp.Add(s, "Resources\\Output\\" + Path.GetFileName(s), EC) == false)
                            return null;
                    }
                }
                // logo
                if (File.Exists(Project.GetProjectResourceOutputFullPath(ProjectPath, "logo.png")))
                {
                    if (gzp.Add(Project.GetProjectResourceOutputFullPath(ProjectPath, "logo.png"), "Resources\\Output\\logo.png", EC) == false)
                        return null;
                }
            }
            // all good - adaug si XML-ul
            if (failIfCriticalFilesAreMissing == false)
            {
                if (File.Exists(Path.Combine(ProjectPath, "project.gappcreator")))
                {
                    if (gzp.Add(Path.Combine(ProjectPath, "project.gappcreator"), "project.gappcreator", EC) == false)
                        return null;
                }
            }
            else
            {
                if (gzp.Add(Path.Combine(ProjectPath, "project.gappcreator"), "project.gappcreator", EC) == false)
                    return null;
            }
            // all ok
            return gzp;
        }
        public List<GenericAd> GetAdsUsedInBuild(string buildName)
        {
            Dictionary<string, string> d = new Dictionary<string, string>();
            buildName = buildName.Trim().ToLower();
            List<GenericAd> lst = new List<GenericAd>();
            foreach (GenericAd ad in Ads)
            {
                Project.StringListToDict(ad.Builds, d);
                if (d.ContainsKey(buildName))
                    lst.Add(ad);
            }
            return lst;
        }
        public List<GenericAd> GetAdsUsedInBuild(GenericBuildConfiguration build)
        {
            return GetAdsUsedInBuild(build.Name);
        }
        public bool UpdateReplaceDictionary(Dictionary<string, string> d, GenericBuildConfiguration build)
        {
            string s;
            if (SplashScreen.Picture == null)
            {
                EC.AddError("Splash Screen needs to be set to run an application");
                return false;
            }
            int v_major = -1, v_minor = -1, v_build = -1;
            Project.StringToVersion(Version, ref v_major, ref v_minor, ref v_build);
            if (v_major < 0)
            {
                EC.AddError("Invalid application version: '" + Version + "'. It should be in the following format: Major.Minor.Build");
                return false;
            }         
            d["$$APLICATION.NAME$$"] = GetProjectName();
            d["$$APLICATION.VERSION.MAJOR$$"] = v_major.ToString();
            d["$$APLICATION.VERSION.MINOR$$"] = v_minor.ToString();
            d["$$APLICATION.VERSION.BUILD$$"] = v_build.ToString();
            d["$$SPLASHSCREEN.START$$"] = SplashScreen.GetResourceFilePosition().ToString();
            d["$$SPLASHSCREEN.SIZE$$"] = SplashScreen.GetResourceFileSize().ToString();
            d["$$SPLASHSCREEN.VIEWWIDTH$$"] = (SplashScreenWidth / 100).ToString();
            d["$$SPLASHSCREEN.VIEWHEIGHT$$"] = (SplashScreenHeight / 100).ToString();
            d["$$SPLASHSCREEN.WAITTIME$$"] = SplashScreenMinimalShowTime.ToString();
            d["$$SPLASHSCREEN.LOGOWIDTH$$"] = SplashScreen.Picture.Width.ToString();
            d["$$SPLASHSCREEN.LOGOHEIGHT$$"] = SplashScreen.Picture.Height.ToString();
            d["$$SPLASHSCREEN.METHOD$$"] = SplashScreenResizeMode.ToString().ToUpper();
            d["$$SNAPSHOT_TYPE$$"] = "GAC_SNAPSHOT_TYPE_"+build.SnapshotType.ToString().ToUpper();
            d["$$ALARM.CHECKUPDATETICKS$$"] = (build.AlarmCheckUpdateSeconds * 60).ToString();

            // pun si rezolutiile available
            d["$$AVAILABLE.RESOLUTIONSCOUNT$$"] = (build.AvailableResolutions.Count*2).ToString();
            if (build.AvailableResolutions.Count>0)
            {
                s = "{";
                foreach (Size sz in build.AvailableResolutions.Keys)
                {
                    s += sz.Width.ToString()+","+sz.Height.ToString()+"  ,   ";
                }
                s = s.Trim();
                if (s.EndsWith(","))
                    s = s.Substring(0, s.Length - 1);
                s = s + "}";
                d["$$AVAILABLE.RESOLUTIONS$$"] = s;
            }
            else
            {
                d["$$AVAILABLE.RESOLUTIONS$$"] = "{0}";
            }       
            return true;
        }
        public bool EditResourceWithExternalApp(string resourceSourceName)
        {
            string fname = this.GetProjectResourceSourceFullPath(resourceSourceName).ToLower();
            if (fname.EndsWith(".svg"))
            {
                if (this.Settings.InskapePath.Length > 0)
                    return this.RunCommand(this.Settings.InskapePath, fname, "Inkscape editor", false, false);
                else
                {
                    MessageBox.Show("Please set the path to Inkscape from the Settings menu !");
                    return false;
                }                
            }
            if ((fname.EndsWith(".png")) || ((fname.EndsWith(".gif"))) || (fname.EndsWith(".jpg")))
            {
                if (this.Settings.ImageEditorPath.Length > 0)
                    return this.RunCommand(this.Settings.ImageEditorPath, fname, "Image editor", false, false);
                else
                {
                    MessageBox.Show("Please set the path to an Image editor tool from the Settings menu!");
                    return false;
                }
            }
            return false;
        }
        public string GetFacebookURL(GenericBuildConfiguration  Build)
        {
            if ((Build.FaceBook != null) && (Build.FaceBook.Trim().Length > 0))
                return Build.FaceBook;
            return FaceBook;
        }
        public string GetYouTubeURL(GenericBuildConfiguration  Build)
        {
            if ((Build.Youtube != null) && (Build.Youtube.Trim().Length > 0))
                return Build.Youtube;
            return Youtube;
        }
        public string GetTwitterURL(GenericBuildConfiguration Build)
        {
            if ((Build.Twitter != null) && (Build.Twitter.Trim().Length > 0))
                return Build.Twitter;
            return Twitter;
        }
        public string GetInstagramURL(GenericBuildConfiguration Build)
        {
            if ((Build.Instagram != null) && (Build.Instagram.Trim().Length > 0))
                return Build.Instagram;
            return Instagram;
        }
        public string GetWebPage(GenericBuildConfiguration Build)
        {
            if ((Build.Webpage != null) && (Build.Webpage.Trim().Length > 0))
                return Build.Webpage;
            return Webpage;
        }

        public int GetStringTemplatePreview(string name)
        {
            name = name.ToLower();
            for (int tr = 0; tr < StringTemplates.Count; tr++)
                if (name.Equals(StringTemplates[tr].Name.ToLower()))
                    return tr;
            return -1;
        }
        public void UpdateBuildsListAttribute()
        {
            string s = "";  // cu toate
            string s2 = ""; // fara develop
            foreach (GenericBuildConfiguration gb in BuildConfigurations)
            {
                if (s.Length>0)
                    s2 += gb.Name + ",";
                s += gb.Name + ",";
            }
            CheckBoxTypeEditor.Builds = s.Substring(0, s.Length - 1);
            if (s2.Length>0)
                CheckBoxTypeEditor.BuildsNoDevelop = s2.Substring(0, s2.Length - 1);
        }

        public bool CheckStringsFontAvailability()
        {
            if (Strings.Count==0)
                return true;
            Dictionary<FontResource, Dictionary<int, Glyph>> fonts = new Dictionary<FontResource, Dictionary<int, Glyph>>();
            foreach (GenericResource g in Resources)
            {
                if (g.GetType() == typeof(FontResource))
                    fonts[(FontResource)g] = ((FontResource)g).GetCharSet();
            }
            if (fonts.Count==0)
            {
                EC.AddError("There are no fonts defined to render the strings that are already defined. Please define some fonts or remove all strings !");
                return false;
            }
            // iau pe rand fiecare string si vad daca poate fi randat
            foreach (StringValues str in Strings)
            {
                if (str.NonDrawable)
                    continue;
                foreach (StringValue sv in str.Values)
                {
                    bool foundOne = false;
                    foreach (FontResource g in fonts.Keys)
                    {
                        if (FontResource.CanRenderString(fonts[g],sv.Value.Replace("\\n","\n")))
                        {
                            foundOne = true;
                            break;
                        }
                    }
                    if (foundOne==false)
                    {
                        EC.AddError("String '" + str.GetVariableNameWithArray() + "' for language: " + sv.Language.ToString() + " - can not be render by any of the existings Fonts !");                        
                    }
                }                
            }
            return !EC.HasErrors();
        }
        public bool CheckCountersIntegrity(GenericBuildConfiguration build,Dictionary<string,Counter> counters,Dictionary<string,List<Counter>> groups = null, List<Counter> list = null, Dictionary<string,Counter> hashes = null)
        {
            string bname = build.Name.ToLower();
            int CounterIndex = 0;
            if (list != null)
                list.Clear();
            if (hashes != null)
                hashes.Clear();
            foreach (Counter gc in this.Counters)
            {
                if (gc.Builds.ToLower().Contains(bname) == false)
                    continue;
                bool err = false;
                if (Project.ValidateVariableNameCorectness(gc.Name, false) == false)
                {
                    err = true;
                    EC.AddError(string.Format("Counter '{0}' from build '{1}' has invalid characters in its name. Format should be [a-zA-Z][a-zA-Z_0-9]*", gc.Name, build.Name));
                }
                if ((gc.Group.Length>0) && (Project.ValidateVariableNameCorectness(gc.Group, false) == false))
                {
                    err = true;
                    EC.AddError(string.Format("Counter group '{0}' from build '{1}' has invalid characters in its name. Format should be [a-zA-Z][a-zA-Z_0-9]*", gc.Group, build.Name));
                }
                if (counters.ContainsKey(gc.Name))
                {
                    err = true;
                    EC.AddError(string.Format("Counter '{0}' from build '{1}' has been define several time !", gc.Name, build.Name));
                }
                if (hashes != null)
                {
                    if (hashes.ContainsKey(gc.Hash))
                    {
                        err = true;
                        EC.AddError(string.Format("Counter '{0}' from build '{1}' has been a hash that was already defined in another Counter !", gc.Name, build.Name));
                    }
                }
                if (err)
                    continue;
                counters[gc.Name] = gc;
                if (hashes != null) 
                    hashes[gc.Hash] = gc;
                gc.CounterIndex = CounterIndex;
                if (list != null)
                    list.Add(gc);
                CounterIndex++;
                if ((gc.Group.Length>0) && (groups != null))
                {
                    if (groups.ContainsKey(gc.Group) == false)
                        groups[gc.Group] = new List<Counter>();
                    groups[gc.Group].Add(gc);
                }
            }
            return !EC.HasErrors();
        }
        public bool CheckCountersIntegrity()
        {
            EC.Reset();
            Dictionary<string,Counter> counters = new Dictionary<string,Counter>();
            Dictionary<string, Counter> hashes = new Dictionary<string, Counter>();
            foreach (GenericBuildConfiguration build in this.BuildConfigurations)
            {
                counters.Clear();
                CheckCountersIntegrity(build, counters, null, null, hashes);
            }
            return !EC.HasErrors();
        }
        public bool CheckAlarmsIntegrity(GenericBuildConfiguration build, Dictionary<string, Alarm> alarms, Dictionary<int, bool> alarms_id)
        {
            string bname = build.Name.ToLower();
            int CounterIndex = 0;
            alarms_id.Clear();
            foreach (Alarm alarm in this.Alarms)
            {
                if (alarm.Builds.ToLower().Contains(bname) == false)
                    continue;
                bool err = false;
                if (Project.ValidateVariableNameCorectness(alarm.Name, true) == false)
                {
                    err = true;
                    EC.AddError(string.Format("Alarm '{0}' from build '{1}' has invalid characters in its name. Format should be [a-zA-Z][a-zA-Z_0-9]* and should start with a capital letter", alarm.Name, build.Name));
                }
                if (alarms.ContainsKey(alarm.Name))
                {
                    err = true;
                    EC.AddError(string.Format("Alarm '{0}' from build '{1}' has been define several time !", alarm.Name, build.Name));
                }
                if (alarms_id.ContainsKey(alarm.UniqueID))
                {
                    err = true;
                    EC.AddError(string.Format("Alarm '{0}' from build '{1}' does not have an unique ID. Delete it and add it again !", alarm.Name, build.Name));
                } else
                {
                    alarms_id[alarm.UniqueID] = true;
                }
                if (alarm.Validate(this) == false)
                    err = true;
                if (err)
                    continue;
                alarms[alarm.Name] = alarm;
                alarm.CounterIndex = CounterIndex;
                CounterIndex++;
            }
            return !EC.HasErrors();
        }
        public bool CheckAlarmsIntegrity()
        {
            EC.Reset();
            Dictionary<string, Alarm> alarms = new Dictionary<string, Alarm>();
            Dictionary<int, bool> alarms_id = new Dictionary<int, bool>();
            foreach (GenericBuildConfiguration build in this.BuildConfigurations)
            {
                alarms.Clear();
                CheckAlarmsIntegrity(build, alarms, alarms_id);
                
            }
            return !EC.HasErrors();
        }
        public bool CheckResourcesIntegrity()
        {
            EC.Reset();
            Dictionary<string, bool> d = new Dictionary<string, bool>();
            Dictionary<string, bool> baseResources = new Dictionary<string, bool>();
            Dictionary<string, bool> builds = new Dictionary<string, bool>();
            Dictionary<string, string> resBuilds = new Dictionary<string,string>();
            foreach (GenericBuildConfiguration gb in BuildConfigurations)
                builds[gb.Name.ToLower()] = true;

            if (ApplicationName.Values.Count==0)
                EC.AddError("No application name defined for the current project !");
            if (ApplicationName.Contains(DefaultLanguage)==false)
                EC.AddError("No application name defined for the default language : "+DefaultLanguage.ToString());
            // verific daca un build are application name specific trebuie sa aiba pentru toate din build-ul de baza
            foreach (GenericBuildConfiguration bld in BuildConfigurations)
            {
                if (bld.ApplicationName.Values.Count == 0)
                    continue;
                foreach (Language l in Enum.GetValues(typeof(Language)))
                {
                    bool projectOK = ApplicationName.Contains(l);
                    bool buildOK = bld.ApplicationName.Contains(l);
                    
                    if ((projectOK) && (!buildOK))
                        EC.AddError(String.Format("Build '{0}' has no application name defined for language '{1}'",bld.Name,l.ToString()));
                    if ((!projectOK) && (buildOK))
                        EC.AddError(String.Format("Build '{0}' has an application name defined for language '{1}' that was not defined in the project as well !", bld.Name, l.ToString()));
                }
            }
            // verific daca toate resursele sunt unice, daca build-urile sunt ok, etc
            foreach (GenericResource g in Resources)
            {
                if (d.ContainsKey(g.GetResourceUniqueKey()))
                    EC.AddError(string.Format("Resource {0} of type {1} for language {2} has been added multiple times !",g.GetResourceVariableName(),g.GetResourceType(),g.Lang));
                else
                    d[g.GetResourceUniqueKey()] = true;
                if (g.IsBaseResource())
                {
                    baseResources[g.GetResourceVariableKey()] = true;
                    if (ValidateVariableNameCorectness(g.Name)==false)
                    {
                        EC.AddError(string.Format("Resource {0} of type {1} for language {2} has an invalid name. Resource names should be in form [A-Za-z][A-Za-z0-9_]+.", g.GetResourceVariableName(), g.GetResourceType(), g.Lang));
                    }
                }
                // verific si build-urile
                Project.StringListToDict(g.Builds, resBuilds);
                foreach (string s in resBuilds.Keys)
                    if (builds.ContainsKey(s)==false)
                        EC.AddError(string.Format("Usage of undefined build ({3}) in resource {0} of type {1} for language {2}", g.GetResourceVariableName(), g.GetResourceType(), g.Lang, resBuilds[s]));
                if (resBuilds.Count==0)
                    EC.AddError(string.Format("Resource {0} of type {1} for language {2} is not used in any build !", g.GetResourceVariableName(), g.GetResourceType(), g.Lang));
            }
            // verific daca toate resursele au resurse de baza si daca au numele ok
            foreach (GenericResource g in Resources)
            {
                if (baseResources.ContainsKey(g.GetResourceVariableKey())==false)
                {
                    EC.AddError(string.Format("Resource {0} of type {1} for language {2} does not exists for the default resolution/language !", g.GetResourceVariableName(), g.GetResourceType(), g.Lang));
                }
            }
            // profile
            if (Profiles.Count == 0)
                EC.AddError("At least one profile should be created for each application !");
            Dictionary<string, bool> usedResources = new Dictionary<string, bool>();
            List<string> usedResourcesKeys = new List<string>(); // altfel nu pot modifica in colectii
            bool globalProfile = false;
            foreach (GenericResource ri in Resources)
            {
                if (Profile.IsAnAccepedResource(ri) == false)
                    continue;
                usedResources[ri.GetResourceVariableKey()] = false;
                usedResourcesKeys.Add(ri.GetResourceVariableKey());
            }
            foreach (Profile p in Profiles)
            {
                if (p.Type == ProfileType.Global)
                    globalProfile = true;
                Dictionary<string, string> profileResources = p.GetAllResources();
                if (profileResources.Count == 0)
                    EC.AddError(string.Format("Profile '{0}' has no resources !", p.Name));

                foreach (string key in usedResourcesKeys)
                    if (profileResources.ContainsKey(key))
                        usedResources[key] = true;
                foreach (string key in profileResources.Keys)
                    if (usedResources.ContainsKey(key) == false)
                        EC.AddError(string.Format("Resource '{0}' from profile '{1}' does not exists !", profileResources[key], p.Name));
            }
            if (globalProfile == false)
                EC.AddError("At least one profile should be declared as global !");
            foreach (GenericResource ri in Resources)
            {
                if (Profile.IsAnAccepedResource(ri) == false)
                    continue;
                if (usedResources[ri.GetResourceVariableKey()] == false)
                {
                    EC.AddError(string.Format("Resource '{0}' of type '{1}' is not used in any profile", ri.GetResourceVariableName(), ri.GetResourceType()));
                }
            }

            // fonturi
            foreach (GenericResource ri in Resources)
            {
                if (ri.GetType() == typeof(FontResource))
                {
                    FontResource gfi = (FontResource)ri;
                    if (gfi.Glyphs.Count == 0)
                        EC.AddError("Font '" + ri.GetResourceVariableName() + "' has no images attached !");
                    foreach (Glyph g in gfi.Glyphs)
                    {
                        g.Validate(false,gfi,this);
                    }
                }
            }
            // shadere
            foreach (GenericResource ri in Resources)
            {
                if (ri.GetType() == typeof(ShaderResource))
                {
                    List<ShaderVariable> l = ShaderResource.CreateVariableList(((ShaderResource)ri).GetFragmentShader(), false);
                    if (l.Count != ((ShaderResource)ri).Uniforms.Count)
                    {
                        EC.AddError(string.Format("Shader '{0}' has {1} variables and none of them are set to a specific type", ri.GetResourceVariableName(), l.Count));
                    }
                }
            }
            // string-uri
            CheckStringsFontAvailability();
            return !EC.HasErrors();
        }
        public bool CheckAnimationsIntegrity()
        {
            EC.Reset();
            Dictionary<string, bool> d = new Dictionary<string, bool>();
            AppResources apres = this.GetAppResources();
            foreach (AnimO.AnimationObject a in this.AnimationObjects)
            {
                if (ValidateVariableNameCorectness(a.Name,false)==false)
                    EC.AddError("Invalid name for AnimationObject: '" + a.Name + "' ! Excepting letters, numbers or '_' character. First character must be a letter !");
                if (a.ValidateAnimation(this, apres)==false)
                    EC.AddError("Invalid animation object: "+a.Name);
            }
            return !EC.HasErrors();
        }

        public int DebugCommandToIndex(string debugCommandName)
        {
            for (int tr = 0; tr < DebugCommands.Count; tr++)
                if (DebugCommands[tr].Name.ToLower().Equals(debugCommandName.ToLower()))
                    return tr;
            return -1;
        }
        public void SetDebugCommand(DebugCommand cmd)
        {
            int index = DebugCommandToIndex(cmd.Name);
            if (index >= 0)
                DebugCommands.RemoveAt(index);
            DebugCommands.Add(cmd);
            DebugCommands.Sort();
        }

        #region Enum/Structs/Constants

        public int GetEnumerationIndex(string name)
        {
            if (name == null)
                return -1;
            for (int tr=0;tr<Enums.Count;tr++)
            {
                if (name.Equals(Enums[tr].Name, StringComparison.InvariantCultureIgnoreCase))
                    return tr;
            }
            return -1;
        }
        public Enumeration GetEnumeration(string name)
        {
            int index = GetEnumerationIndex(name);
            if (index >= 0)
                return Enums[index];
            return null;
        }
        public int GetStructureIndex(string name)
        {
            if (name == null)
                return -1;
            for (int tr = 0; tr < Structs.Count; tr++)
            {
                if (name.Equals(Structs[tr].Name, StringComparison.InvariantCultureIgnoreCase))
                    return tr;
            }
            return -1;
        }
        public Structure GetStructure(string name)
        {
            int index = GetStructureIndex(name);
            if (index >= 0)
                return Structs[index];
            return null;
        }
        public Structure GetStructureForVariableName(string name)
        {
            foreach (Structure s in Structs)
            {
                if (s.GetValueNameIndex(name) >= 0)
                    return s;
            }
            return null;
        }

        public int GetConstantIndex(string name)
        {
            if (name == null)
                return -1;
            for (int tr = 0; tr < Constants.Count; tr++)
            {
                if (name.Equals(Constants[tr].Name, StringComparison.InvariantCultureIgnoreCase))
                    return tr;
            }
            return -1;
        }
        public ConstantValue GetConstant(string name)
        {
            int index = GetConstantIndex(name);
            if (index >= 0)
                return Constants[index];
            return null;
        }
        public string ValidateValueForType(string value, string type, out string error, Dictionary<string, GenericResource> baseResources,Dictionary<string,StringValues> stringResources,bool CanBeNull)
        {
            error = "";
            string result = "";
            if (value==null)
            {
                error = "Value field in ValidateValueForType is NULL";
                return null;
            }
            if (type == null)
            {
                error = "Type field in ValidateValueForType is NULL";
                return null;
            }
            ConstantModeType mode = ConstantHelper.GetConstantMode(type);
            switch (mode)
            {
                case ConstantModeType.BasicTypes:
                    result = ConstantHelper.ValidateBasicTypeValue(value, ConstantHelper.GetBasicTypesType(type));
                    if (result == null)
                            error = String.Format("Value '{0}' is not valid for type '{1}'", value, type);
                    return result;
                case ConstantModeType.Enumerations:
                    Enumeration e = GetEnumeration(ConstantHelper.GetEnumerationType(type));
                    if (e==null)
                    {
                        if (ConstantHelper.GetEnumerationType(type)==null)
                            error = "Invalid type for enumeration : "+type;
                        else
                            error = String.Format("Enumeration '{0}' does not exist !", ConstantHelper.GetEnumerationType(type));
                        return null;
                    }
                    if (e.IsBitSet)
                    {
                        Dictionary<string, string> d = Project.StringListToDict(value);
                        if (d != null)
                        {
                            string sRes = "";
                            foreach (EnumValue ev in e.Values)
                            {
                                if (d.ContainsKey(ev.Name.ToLower()))
                                {
                                    sRes += ev.Name + ", ";
                                }
                            }
                            if (sRes.EndsWith(", "))
                                sRes = sRes.Substring(0, sRes.Length - 2);
                            return sRes;
                        }
                        else
                        {
                            error = String.Format("Value '{0}' is not valid bit set list for the enumeration '{1}' !", value, e.Name);
                            return null;
                        }
                    }
                    else
                    {
                        EnumValue ev = e.FindValue(value);
                        if (ev == null)
                        {
                            error = String.Format("Value '{0}' is not part of the enumeration '{1}' !", value, e.Name);
                            return null;
                        }
                        return ev.Name;
                    }
                    break;
                case ConstantModeType.DataTypes:
                    Structure s = GetStructure(ConstantHelper.GetDataTypesType(type));
                    if (s==null)
                    {
                        if (ConstantHelper.GetDataTypesType(type) == null)
                            error = "Invalid type for data type : "+type;
                        else
                            error = String.Format("DataType '{0}' does not exist !", ConstantHelper.GetDataTypesType(type));
                        return null;
                    }
                    return value;
                case ConstantModeType.Resources:
                    ResourcesConstantType rct = ConstantHelper.GetResourcesType(type);
                    if (rct == ResourcesConstantType.None)
                    {
                        error = "Unknown resource type: " + type;
                        return null;
                    }
                    if ((value.Length == 0) && (CanBeNull == true))
                        return value;
                    if (rct == ResourcesConstantType.String)
                    {
                        if (stringResources == null)
                            stringResources = GetStringResources();
                        if (stringResources.ContainsKey(value)==false)
                        {
                            error = String.Format("String '{0}' does not exist !", value);
                            return null;
                        }
                        return value;
                    }
                    // altfel sunt pe resurse normale
                    Type t = ConstantHelper.ConvertResourcesConstantTypeToResourceType(rct);
                    if (t==null)
                    {
                        error = String.Format("Failed to convert constant resource of type '{0}' to its app resource type", rct.ToString());
                        return null;
                    }                    
                    if (baseResources == null)
                        baseResources = GetBaseResources();
                    if (baseResources.ContainsKey(GenericResource.GetResourceVariableKey(t, value))==false)
                    {
                        error = String.Format("Resource '{0}' of type {1} does not exist !", value,rct.ToString());
                        return null;
                    }
                    return value;
            }
            // pentru toate celelalte cazuri.
            error = "Unimplemented mode: " + mode.ToString();
            return null;
        }

        public bool ValidateValueForType(ColumnInfo cInfo,string oldValue,object newValue,Dictionary<string, GenericResource> baseResources,Dictionary<string, StringValues> stringResources, out string result)
        {
            string error = "";
            result = "";
            switch (cInfo.Mode)
            {
                case ConstantModeType.BasicTypes:
                    if (oldValue.Equals(newValue.ToString()))
                    {
                        result = newValue.ToString();
                        return true;
                    }
                    result = this.ValidateValueForType(newValue.ToString(), cInfo.FullType, out error, null, null, false);
                    if (result == null)
                    {
                        result = error;
                        return false;
                    }
                    return true;
                case ConstantModeType.Enumerations:
                    if (newValue.GetType() == typeof(string))
                    {
                        result = null;
                        return true;
                    }
                    KeyValuePair<string, string> enm = (KeyValuePair<string, string>)newValue;
                    result = this.ValidateValueForType(enm.Value, cInfo.FullType, out error, null, null, false);
                    if (result == null)
                    {
                        result = error;
                        return false;
                    }
                    return true;
                case ConstantModeType.Resources:
                    if (newValue.GetType() == typeof(string))
                    {
                        result = null;
                        return true;
                    }
                    KeyValuePair<ResourcesConstantType, string> res = (KeyValuePair<ResourcesConstantType, string>)newValue;
                    result = this.ValidateValueForType(res.Value, cInfo.FullType, out error, baseResources, stringResources, cInfo.Field.CanBeNull);
                    if (result == null)
                    {
                        MessageBox.Show(error);
                        return false;
                    }
                    return true;
                case ConstantModeType.DataTypes:
                    if (newValue.GetType() == typeof(string))
                    {
                        result = null;
                        return true;
                    }
                    KeyValuePair<string, string> resdt = (KeyValuePair<string, string>)newValue;
                    result = resdt.Value;
                    return true;

            }
            result = "Internal error - don't know how to process this type of field : " + cInfo.Mode.ToString() + " !";
            return false;
        }

        private string GetTypeDefaultValue(string type,ref string error)
        {
            ConstantModeType cmt = ConstantHelper.GetConstantMode(type);
            switch (cmt)
            {
                case ConstantModeType.BasicTypes:
                    BasicTypesConstantType bct = ConstantHelper.GetBasicTypesType(type);
                    if (ConstantHelper.IsNumber(bct))
                        return "0";
                    if (bct == BasicTypesConstantType.Boolean)
                        return "false";
                    if (bct == BasicTypesConstantType.Color)
                        return "0xFF000000";
                    if (bct == BasicTypesConstantType.String)
                        return "";
                    error = String.Format("Basic type {0} is not inplemented !", type);
                    return null;
                case ConstantModeType.Enumerations:
                    Enumeration e = GetEnumeration(ConstantHelper.GetEnumerationType(type));
                    if (e==null) {
                        error = String.Format("Enumeration {0} is not defined !", type);
                        return null;
                    }
                    if (e.Values.Count==0)
                    {
                        error = String.Format("Enumeration {0} has no values !", e.Name);
                        return null;
                    }
                    return e.Values[0].Name;
                case ConstantModeType.Resources:
                case ConstantModeType.DataTypes:
                    return "";

            }
            error = String.Format("Unknwon type: {0} - can not determine the default value !", type);
            return null;
        }

        public bool CheckDataTypesIntegrity()
        {
            Dictionary<string, bool> d = new Dictionary<string, bool>();
            Dictionary<string, bool> d_sv = null;
            Dictionary<string,GenericResource> baseResources = GetBaseResources();
            Dictionary<string,StringValues> stringResources= GetStringResources();
            Dictionary<string, Dictionary<string, bool>> d_linkID = new Dictionary<string, Dictionary<string, bool>>();
            ArrayCounter ac = new ArrayCounter();
            List<string> lst_values = null;
            string error = "";
            // validez enumerarile
            foreach (Enumeration en in Enums)
            {
                if (d.ContainsKey(en.Name))
                    EC.AddError("Enumerations", string.Format("Enum '{0}' name was already used by another enumeration/constant or data type !", en.Name));
                if (ValidateVariableNameCorectness(en.Name,false)==false)
                    EC.AddError("Enumerations", string.Format("Enum '{0}' has an invalid name (should contains [a-z], [A-Z], [0-9], or '_') and must start with a letter !", en.Name));
                if (en.Type == BasicTypesConstantType.None)
                    EC.AddError("Enumerations", string.Format("Enum '{0}' does not have a type !", en.Name));
                if (en.Values.Count==0)
                    EC.AddError("Enumerations", string.Format("Enum '{0}' does not have any value !", en.Name));
                // validez si valorile
                foreach (EnumValue ev in en.Values)
                {
                    if (ValidateVariableNameCorectness(ev.Name,false) == false)
                        EC.AddError("Enumerations", string.Format("Enum '{0}' has an field '{1}' with an invalid name (should contains [a-z], [A-Z], [0-9], or '_') and must start with a letter !", en.Name,ev.Name));
                    if (ConstantHelper.ValidateBasicTypeValue(ev.Value, en.Type)==null)
                        EC.AddError("Enumerations", string.Format("Enum '{0}' -> field '{1}' value '{2}' is not a valid value for type {3} !", en.Name, ev.Name,ev.Value,en.Type.ToString()));
                }
                d[en.Name] = true;
            }
            // validez constanele
            foreach (ConstantValue cv in Constants)
            {
                if (d.ContainsKey(cv.Name))
                    EC.AddError("Constants", string.Format("Constant '{0}' name was already used by another enumeration/constant or data type !", cv.Name));
                if (ValidateVariableNameCorectness(cv.Name,false) == false)
                    EC.AddError("Constants", string.Format("Constant '{0}' has an invalid name (should contains [a-z], [A-Z], [0-9], or '_') and must start with a letter !", cv.Name));
                error = "";
                if (cv.MatrixColumnsCount > 0)
                {
                    if (ConstantHelper.ValidateBasicTypeArrayValues(cv.Value, ConstantHelper.GetBasicTypesType(cv.Type), cv.MatrixColumnsCount, out error) == null)
                        EC.AddError("Constants", error);
                }
                else
                {
                    if (ValidateValueForType(cv.Value, cv.Type, out error, baseResources, stringResources, true) == null)
                        EC.AddError("Constants", error);
                }
                d[cv.Name] = true;
            }
            // validez tipurile de date
            foreach (Structure dt in Structs)
            {
                if (d.ContainsKey(dt.Name))
                    EC.AddError("Data Types", string.Format("DataType '{0}' name was already used by another enumeration/constant or data type !", dt.Name));
                if (ValidateVariableNameCorectness(dt.Name,false) == false)
                    EC.AddError("Data Types", string.Format("DataType '{0}' has an invalid name (should contains [a-z], [A-Z], [0-9], or '_') and must start with a letter !", dt.Name));
                if (dt.Fields.Count == 0)
                    EC.AddError("Data Types", string.Format("DataType '{0}' does not have any field declared !", dt.Name));
                // validez ca field-urile sunt ok
                foreach (StructureField sf in dt.Fields)
                {
                    if (ValidateVariableNameCorectness(sf.Name,false) == false)
                        EC.AddError("Data Types", string.Format("DataType '{0}' has a field '{1}' with an invalid name (should contains [a-z], [A-Z], [0-9], or '_') and must start with a letter !", dt.Name,sf.Name));
                }
                // validez valorile 
                ac.Clear();
                dt.SyncStructureValues();
                Dictionary<string,bool> linkIDs = new Dictionary<string, bool>();
                foreach (StructureValue sv in dt.Values)
                {
                    linkIDs[sv.LinkID.ToString()] = true;
                    if (d_sv!=null)
                        d_sv.Clear();
                    if (sv.Name.Length>0)
                    {
                        if (ValidateVariableNameCorectness(sv.Name,false) == false)
                            EC.AddError("Data Types", string.Format("DataType '{0}' has a value '{1}' with an invalid name (should contains [a-z], [A-Z], [0-9], or '_') and must start with a letter !", dt.Name, sv.Name));
                        if (ac.Add(sv.Name, sv.Array1, sv.Array2) == false)
                        {
                            if (d_sv == null)
                                d_sv = new Dictionary<string, bool>();
                            if (d_sv.ContainsKey(sv.Name) == false)
                            {
                                EC.AddError("Data Types", string.Format("DataType '{0}' has a value '{1}' that has a multiple array scopes ! (the same name is declared as a vector / matrix or simple variable)", dt.Name, sv.Name));
                                d_sv[sv.Name] = true;
                            }
                        }
                    }
                    for (int tr=0;tr<dt.Fields.Count;tr++)
                    {
                        error = "";
                        if (dt.Fields[tr].List)
                        {
                            // validez ca lista e ok
                             if (lst_values == null)
                                lst_values = new List<string>();
                            StringListToList(sv.FieldValues[tr], lst_values, ';', false);
                            foreach (string s in lst_values)
                            {
                                if (ValidateValueForType(s, dt.Fields[tr].Type, out error, baseResources, stringResources, dt.Fields[tr].CanBeNull) == null)
                                    EC.AddError("Data Types", string.Format("DataType '{0}' has a variable field '{1}' (LinkID:{2}) with an invalid value for field {3}: {4}", dt.Name, Project.GetVariableName(sv.Name, sv.Array1, sv.Array2), sv.LinkID, dt.Fields[tr].Name, error));
                            }
                        }
                        else
                        {
                            if (ValidateValueForType(sv.FieldValues[tr], dt.Fields[tr].Type, out error, baseResources, stringResources, dt.Fields[tr].CanBeNull) == null)
                                EC.AddError("Data Types", string.Format("DataType '{0}' has a variable field '{1}' (LinkID:{2}) with an invalid value for field {3}: {4}", dt.Name, Project.GetVariableName(sv.Name, sv.Array1, sv.Array2), sv.LinkID, dt.Fields[tr].Name, error));
                        }
                    }
                }
                d_linkID[dt.Name] = linkIDs;
            }
            // verific si linkurile daca sunt corecte
            foreach (Structure dt in Structs)
            {
                foreach (StructureValue sv in dt.Values)
                {
                    for (int tr = 0; tr < dt.Fields.Count; tr++)
                    {
                        if (dt.Fields[tr].List == false)
                            continue;
                        ConstantModeType cmt = ConstantHelper.GetConstantMode(dt.Fields[tr].Type);
                        if (cmt != ConstantModeType.DataTypes)
                            continue;
                        // verific ca linkurile sa fie ok
                        string dataType = ConstantHelper.GetDataTypesType(dt.Fields[tr].Type);
                        if (d_linkID.ContainsKey(dataType)==false)
                        {
                            EC.AddError("Data Types", string.Format("DataType '{0}' has a variable field '{1}' (LinkID:{2}) with an invalid value for field {3}: They refer to a data type that does not exists {4} !", dt.Name, Project.GetVariableName(sv.Name, sv.Array1, sv.Array2), sv.LinkID, dt.Fields[tr].Name, dataType));
                        }
                        else
                        {
                            Dictionary<string,bool> linkIDs = d_linkID[dataType];
                            StringListToList(sv.FieldValues[tr], lst_values, ';', true);
                            foreach (string s in lst_values)
                            {
                                if (linkIDs.ContainsKey(s)==false)
                                    EC.AddError("Data Types", string.Format("DataType '{0}' has a variable field '{1}' (LinkID:{2}) with an invalid value for field {3}: LinkID {4} described in the list does not exits !", dt.Name, Project.GetVariableName(sv.Name, sv.Array1, sv.Array2), sv.LinkID, dt.Fields[tr].Name, s));
                            }
                        }
                    }
                }
            }

            return !EC.HasErrors();
        }

        #endregion

        #region Application Name/Title
        public string GetApplicationName(Language l, GenericBuildConfiguration bld)
        {
            if (bld.ApplicationName.Contains(l))
                return bld.ApplicationName.Get(l);
            return ApplicationName.Get(l);
        }
        #endregion

        #region Icons
        private void PopulateIconList(GenericBuildConfiguration build, ProjectIcons Icons)
        {
            List<int> sizes = new List<int>();
            string source = "";
            bool buildHasIcon = false;

            if (build != null)
            {
                int []sz = build.GetIconSizes();
                if (build.Icon.Length > 0)
                {
                    source = build.Icon;
                    buildHasIcon = true;
                }
                else
                {
                    source = this.Icon;
                    buildHasIcon = false;
                }
                sizes.AddRange(sz);
            }
            else
            {                
                source = this.Icon;
                buildHasIcon = false;
            }
            sizes.Add(128);
            if (source.Length == 0)
                return;
            // sursele
            string svg_source = GetProjectResourceSourceFullPath(source);
            if (Icons.Sources.ContainsKey(svg_source.ToLower()) == false)
                Icons.Sources[svg_source.ToLower()] = svg_source;
            // pun fisierele output
            foreach (int size in sizes)
            {
                string output = "";
                if (buildHasIcon)
                    output = GetProjectResourceOutputFullPath(String.Format("{0}_icon_{1}x{1}.png",build.Name,size));
                else
                    output = GetProjectResourceOutputFullPath(String.Format("main_icon_{0}x{0}.png",size));
                if (Icons.OutputFiles.ContainsKey(output.ToLower())==false)
                {
                    IconInfo ii = new IconInfo();
                    ii.Output = output;
                    ii.Source = svg_source;
                    ii.Size = size;
                    Icons.OutputFiles[output.ToLower()] = ii;
                }
                if (size==128)
                {
                    if (buildHasIcon)
                        Icons.BuildIcon = output;
                    else
                        Icons.ProjectIcon = output;
                }
                if (Icons.Sizes.ContainsKey(size) == false)
                    Icons.Sizes[size] = output;
            }
        }
        public ProjectIcons GetBuildIcons(GenericBuildConfiguration build)
        {
            ProjectIcons Icons = new ProjectIcons();
            PopulateIconList(build, Icons);
            return Icons;
        }
        public ProjectIcons GetProjectIcons()
        {
            ProjectIcons Icons = new ProjectIcons();
            foreach (GenericBuildConfiguration bld in BuildConfigurations)
            {
                PopulateIconList(bld, Icons);
            }
            return Icons;
        }
        #endregion

        #region Project Vacuum / Zip / Git

        private void ProjectFilesAddProjectXML(Dictionary<string,ZipContentType> d)
        {
            d[Path.Combine(ProjectPath, "project.gappcreator")] = ZipContentType.ProjectXML;
        }
        private void ProjectFilesAddGACSources(Dictionary<string,ZipContentType> d)
        {
            foreach (ProjectFile pf in Files)
                d[Path.Combine(ProjectPath, "Sources", pf.Name)] = ZipContentType.GACSources;
        }
        private void ProjectFilesAddResources(Dictionary<string,ZipContentType> d)
        {
            foreach (GenericResource r in Resources)
            {
                // pentru fonturi adaug alte date
                if (r.GetType() == typeof(FontResource))
                {
                    FontResource f = (FontResource)r;
                    foreach (Glyph g in f.Glyphs)
                    {
                        d[f.GetGlyphSourcePath(g.Code)] = ZipContentType.SourceResources;
                    }

                }
                else
                {
                    if (r.Source.Length > 0)
                        d[r.GetSourceFullPath()] = ZipContentType.SourceResources;
                }
            }
            if (Icon.Length>0)
                d[GetProjectResourceSourceFullPath(Icon)] = ZipContentType.SourceResources;
            // adaug si iconitele specifice - if any
            foreach (GenericBuildConfiguration gb in BuildConfigurations)
                if (gb.Icon.Length>0)
                    d[GetProjectResourceSourceFullPath(gb.Icon)] = ZipContentType.SourceResources;

            d[SplashScreen.GetSourceFullPath()] = ZipContentType.SourceResources;
        }
        private void ProjectFilesAddPublishMaterials(Dictionary<string, ZipContentType> d)
        {
            foreach (PublishObject po in PublishData)
            {
                d[po.GetObjectFile()] = ZipContentType.PublishMaterials;
                string source = po.GetObjectSource();
                if (source.Length > 0)
                    d[source] = ZipContentType.PublishMaterials;
            }
        }
        private void ProjectFilesAddFontTemplates(Dictionary<string,ZipContentType> d)
        {
            Dictionary<string, int> dTemplates = new Dictionary<string, int>();
            try
            {
                string[] fls = Directory.GetFiles(this.GetProjectFontTemplatesFolder());
                foreach (string s in fls)
                {
                    if ((s.ToLower().EndsWith(".png")) || (s.ToLower().EndsWith(".svg")))
                    {
                        string name = s.Substring(0, s.LastIndexOf('.'));
                        if (dTemplates.ContainsKey(name) == false)
                            dTemplates[name] = 0;
                        if (s.ToLower().EndsWith(".png"))
                            dTemplates[name] |= 1;
                        if (s.ToLower().EndsWith(".svg"))
                            dTemplates[name] |= 2;
                    }
                }
            }
            catch (Exception)
            {

            }
            foreach (string source in dTemplates.Keys)
                if (dTemplates[source] == 3)
                {
                    d[source + ".svg"] = ZipContentType.FontTemplates;
                    d[source + ".png"] = ZipContentType.FontTemplates;
                }
        }
        private void ProjectFilesAddBuildResources(Dictionary<string,ZipContentType> d)
        {
            foreach (GenericResource r in Resources)
            {
                // pentru fonturi adaug alte date
                if (r.GetType() == typeof(FontResource))
                {
                    FontResource f = (FontResource)r;
                    foreach (Glyph g in f.Glyphs)
                    {
                        Glyph.GlyphVersionInfo gvi = g.GetVersion();
                        foreach (string rez in gvi.Rezolutions.Keys)
                        {
                            string s = f.GetGlyphOutputPath(g.Code, rez);
                            if ((s != null) && (d.ContainsKey(s) == false))
                                d[s] = ZipContentType.OutputResources;
                        }
                    }

                }
                else
                {
                    string s = r.GetOutputFileName();
                    if ((s != null) && (d.ContainsKey(s)==false))
                        d[s] = ZipContentType.OutputResources;
                }
            }
        }
        private void ProjectFilesAddIconsAndSplashScreen(Dictionary<string, ZipContentType> d)
        {
            // iconitele
            ProjectIcons Icons = GetProjectIcons();
            foreach (string s in Icons.OutputFiles.Keys)
                d[s] = ZipContentType.IconsAndSplashScreen;
            // logo
            d[GetProjectResourceOutputFullPath("logo.png")] = ZipContentType.IconsAndSplashScreen;
        }
        private void ProjectFilesAddPlugins(Dictionary<string, ZipContentType> d)
        {
            Dictionary<string, string> dd = Project.StringListToDict(PluginList);
            foreach (string name in dd.Keys)
                d[Path.Combine(GetProjectResourcePluginsFolder(),dd[name])] = ZipContentType.Plugins;
        }
        private void ProjectFilesAddDirSearch(string dir,Dictionary<string,string> ext,Dictionary<string, ZipContentType> d,ZipContentType tp)
        {
            try
            {
                foreach (string f in Directory.GetFiles(dir))
                {
                    if ((ext != null) && (ext.ContainsKey(Path.GetExtension(f).ToLower()) == false))
                        continue;
                    if (d.ContainsKey(f) == false)
                        d[f] = tp;
                }
                foreach (string dr in Directory.GetDirectories(dir))               
                    ProjectFilesAddDirSearch(dr,ext,d,tp);               
            }
            catch (Exception)
            {                
            }
        }
        private void ProjectFilesAddFilesFromDirectory(string folderPath,string extensionList,Dictionary<string, ZipContentType> d,ZipContentType tp)
        {
            Dictionary<string, string> ext = null;
            if ((extensionList != null) && (extensionList.Trim().Length != 0))
                ext = Project.StringListToDict(extensionList);
            ProjectFilesAddDirSearch(folderPath, ext, d, tp);
        }
        private void ProjectFilesAddBinaries(Dictionary<string, ZipContentType> d)
        {
            ProjectFilesAddFilesFromDirectory(Path.Combine(ProjectPath, "bin"), ".dll,.exe,.res,.apk", d, ZipContentType.Binaries);
        }
        private void ProjectFilesAddPluginSources(Dictionary<string, ZipContentType> d)
        {
            ProjectFilesAddFilesFromDirectory(GetProjectPluginsCodeFolder(), ".cs,.csproj,.resx,.sln,.png,.svg,.config", d, ZipContentType.PluginsSources);
        }
        private void ProjectFilesAddSystemSettingSnapshots(Dictionary<string, ZipContentType> d)
        {
            ProjectFilesAddFilesFromDirectory(Path.Combine(ProjectPath, "bin"), ".snapshot", d, ZipContentType.Binaries);
        }

        public bool ZipProject(string fileName,ZipContentType tp, bool addGacCreatorSettings)
        {
            try
            {
                using (ZipArchive newFile = ZipFile.Open(fileName, ZipArchiveMode.Create))
                {
                    Dictionary<string, ZipContentType> d = new Dictionary<string, ZipContentType>();
                    if (addGacCreatorSettings)
                        d[Path.Combine(ProjectPath, "project.settings")] = ZipContentType.ProjectXML;
                    if ((tp & ZipContentType.ProjectXML) != 0)
                        ProjectFilesAddProjectXML(d);
                    if ((tp & ZipContentType.GACSources) != 0)
                        ProjectFilesAddGACSources(d);
                    if ((tp & ZipContentType.SourceResources) != 0)
                        ProjectFilesAddResources(d);
                    if ((tp & ZipContentType.FontTemplates) != 0)
                        ProjectFilesAddFontTemplates(d);
                    if ((tp & ZipContentType.OutputResources) != 0)
                        ProjectFilesAddBuildResources(d);
                    if ((tp & ZipContentType.IconsAndSplashScreen) != 0)
                        ProjectFilesAddIconsAndSplashScreen(d);
                    if ((tp & ZipContentType.Plugins) != 0)
                        ProjectFilesAddPlugins(d);
                    if ((tp & ZipContentType.PluginsSources) != 0)
                        ProjectFilesAddPluginSources(d);
                    if ((tp & ZipContentType.Binaries) != 0)
                        ProjectFilesAddBinaries(d);
                    if ((tp & ZipContentType.PublishMaterials) != 0)
                        ProjectFilesAddPublishMaterials(d);
                    if ((tp & ZipContentType.SystemSettingSnapshots) != 0)
                        ProjectFilesAddSystemSettingSnapshots(d);
                    // pun in zip
                    string pp = ProjectPath.ToLower();
                    pp = Path.GetDirectoryName(pp);
                    foreach (string k in d.Keys)
                    {
                        if (k.ToLower().StartsWith(pp))
                            newFile.CreateEntryFromFile(k, k.Substring(pp.Length), CompressionLevel.Optimal);
                    }
                }
                return true;
            } 
            catch (Exception e)
            {
                EC.AddException("Unable to create: "+fileName,e);
                return false;
            }
        }
        public Dictionary<string,string> GetProjectFilesThatShouldBeInGitRepo()
        {
            Dictionary<string, ZipContentType> d = new Dictionary<string, ZipContentType>();
            ProjectFilesAddProjectXML(d);
            ProjectFilesAddGACSources(d);
            ProjectFilesAddResources(d);
            ProjectFilesAddFontTemplates(d);
            ProjectFilesAddPublishMaterials(d);
            Dictionary<string, string> l = new Dictionary<string,string>();
            foreach (string fname in d.Keys)
            {
                string res = fname.Substring(ProjectPath.Length).Replace("\\","/").ToLower();
                if (res.StartsWith("/"))
                    res = res.Substring(1);
                l[res] = fname;
            }
            return l;
        }
        #endregion

        #region Static functions
        public static Project Load(string fileName,SystemSettings settings,ErrorsContainer tempEC)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Project));
                TextReader textReader = new StreamReader(fileName);
                Project sol = (Project)serializer.Deserialize(textReader);
                textReader.Close();
                if (sol != null)
                {
                    sol.ProjectPath = Path.GetDirectoryName(fileName);
                    sol.Settings = settings;
                }
                else
                {
                    if (tempEC != null)
                        tempEC.AddError("Unable to load project: " + fileName);
                    return null;
                }
                if (sol.DesignResolution=="")
                {
                    if ((sol.BuildConfigurations.Count>0) && (((DevelopBuildConfiguration)sol.BuildConfigurations[0])._AppResolution.Length>0))
                    {
                        sol.DesignResolution = ((DevelopBuildConfiguration)sol.BuildConfigurations[0])._AppResolution;                              
                    }
                }

                sol.DesignResolutionSize = SizeToValues(sol.DesignResolution);
                if ((sol.DesignResolutionSize.Width<1) || (sol.DesignResolutionSize.Height<1))
                {
                    if (tempEC != null)
                        tempEC.AddError("Missing or Invalid DesignResolution parameter from solution XML file !");
                    return null;
                }
                sol.DesignResolution = string.Format("{0} x {1}", sol.DesignResolutionSize.Width, sol.DesignResolutionSize.Height);
                sol.UpdateResourceStatus();
                sol.UpdatePublishObjectStatus();

                // enumerari
                if (sol.Enums == null)
                    sol.Enums = new List<Enumeration>();
                sol.Enums.Sort();
                if (sol.Structs == null)
                    sol.Structs = new List<Structure>();
                sol.Structs.Sort();
                if (sol.Constants == null)
                    sol.Constants = new List<ConstantValue>();
                sol.Constants.Sort();
                // constante
                // icon
                if (sol.SplashScreen != null)
                    sol.SplashScreen.prj = sol;
                // creez si folderele default
                if (Disk.CreateFolder(Path.Combine(sol.ProjectPath, "Bin"), tempEC) == false)
                    return null;
                if (Disk.CreateFolder(Path.Combine(sol.ProjectPath, "Temp"), tempEC) == false)
                    return null;
                if (Disk.CreateFolder(Path.Combine(sol.ProjectPath, "Sources"), tempEC) == false)
                    return null;
                if (Disk.CreateFolder(Path.Combine(sol.ProjectPath, "CppProject"), tempEC) == false)
                    return null;
                if (Disk.CreateFolder(Path.Combine(sol.ProjectPath, "Resources"), tempEC) == false)
                    return null;
                if (Disk.CreateFolder(Path.Combine(sol.ProjectPath, "Resources", "Sources"), tempEC) == false)
                    return null;
                if (Disk.CreateFolder(Path.Combine(sol.ProjectPath, "Resources", "Output"), tempEC) == false)
                    return null;
                if (Disk.CreateFolder(Path.Combine(sol.ProjectPath, "Resources", "FontTemplates"), tempEC) == false)
                    return null;
                if (Disk.CreateFolder(Path.Combine(sol.ProjectPath, "Resources", "Plugins"), tempEC) == false)
                    return null;
                if (Disk.CreateFolder(Path.Combine(sol.ProjectPath, "PluginsCode"), tempEC) == false)
                    return null;
                if (Disk.CreateFolder(Path.Combine(sol.ProjectPath, "Builds"), tempEC) == false)
                    return null;
                if (Disk.CreateFolder(Path.Combine(sol.ProjectPath, "PublishMaterials"), tempEC) == false)
                    return null;
                if (Disk.CreateFolder(Path.Combine(sol.ProjectPath, "PublishMaterials","WorkGroup"), tempEC) == false)
                    return null;
                return sol;
            }
            catch (Exception e)
            {
                if (tempEC != null)
                    tempEC.AddException("Unable to load project: " + fileName, e);                
                return null;
            }
        }
        public static ResourceType FileNameToResourceType(string fileName)
        {
            string ext = Path.GetExtension(fileName).ToLower();
            switch (ext)
            {
                case ".jpg":
                case ".jpeg":
                case ".png":
                case ".bmp":
                    return ResourceType.RasterImage;
                case ".svg":
                    return ResourceType.VectorImage;
                case ".mp3":
                case ".wav":
                    return ResourceType.Music;
                case ".animxml":
                    return ResourceType.Presentation;
            }
            return ResourceType.Raw;
        }
        public static string GetVariableName(string name,int array1,int array2)
        {
            if ((array1 < 0) && (array2 < 0))
                return name;
            if ((array1 >= 0) && (array2 < 0))
                return string.Format("{0}[{1}]", name, array1);
            return string.Format("{0}[{1}][{2}]", name, array1,array2);
        }        
        public static Bitmap ImageToIcon(ErrorsContainer EC,Bitmap original, int width, int height)
        {
            if ((width < 0) || (height < 0))
                return original;
            try
            {
                Bitmap bmp = new Bitmap(width, height);
                Graphics g = Graphics.FromImage(bmp);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.FillRectangle(Brushes.Transparent, 0, 0, width, height);
                float rapX = (float)original.Width / (float)width;
                float rapY = (float)original.Height / (float)height;
                if (rapX < rapY)
                    rapX = rapY;
                if (rapX < 1)
                    rapX = 1;
                float newW = (original.Width / rapX);
                float newH = (original.Height / rapX);
                float px = (width - newW) / 2;
                float py = (height - newH) / 2;
                g.DrawImage(original, px, py, newW, newH);
                g.Dispose();
                return bmp;
            }
            catch (Exception e)
            {
                EC.AddError("ImageToIcon", "Unable to create Icon - " + e.ToString());
                return null;
            }
        }
        public static Image LoadImage(string path,ErrorsContainer EC = null)
        {
            try
            {
                Image img = Image.FromStream(new MemoryStream(File.ReadAllBytes(path)));
                if ((img == null) && (EC != null))
                    EC.AddError("Unable to load image: " + path + " - Loader returned null !");
                return img;
            }
            catch (Exception e)
            {
                if (EC != null)
                    EC.AddException("Unable to load image: " + path, e);
                return null;
            }
        }
        public static bool SizeToValues(string size, ref int width, ref int height)
        {
            if (size.ToLower().StartsWith("maximized"))
            {
                width = height = ScreenResolutionMaximizedWindow;
                return true;
            }
            if (size.ToLower().StartsWith("full"))
            {
                width = height = ScreenResolutionFullScreen;
                return true;
            }
            if (size.ToLower().StartsWith("best"))
            {
                width = height = ScreenResolutionBestFit;
                return true;
            }
            if (size.ToLower().Contains("x") == false)
                return false;
            string[] words = size.ToLower().Split('x');
            if (words.Length != 2)
                return false;
            int w = 0, h = 0;
            if (int.TryParse(words[0], out w) == false)
                return false;
            if (int.TryParse(words[1], out h) == false)
                return false;
            if ((w < 1) || (h < 1) || (w > 50000) || (h > 50000))
                return false;
            width = w;
            height = h;
            return true;
        }
        public static Size SizeToValues(string size)
        {
            int w = 0, h = 0;
            if (SizeToValues(size, ref w, ref h) == false)
                return new Size(0,0);
            return new Size(w, h);
        }
        public static string ResolutionToString(int width,int height)
        {
            if ((width == height) && (width == ScreenResolutionMaximizedWindow))
                return "MaximizedWindow";
            if ((width == height) && (width == ScreenResolutionFullScreen))
                return "FullScreen";
            if ((width == height) && (width == ScreenResolutionBestFit))
                return "BestFit";
            if ((width > 0) && (height > 0) && (width <= 50000) && (height <= 50000))
                return String.Format("{0} x {1}", width, height);
            return "";
        }
        public static int FirstPow2Bigger(int value)
        {
            int x = 1;
            for (int tr = 0; tr < 30; tr++)
            {
                if (x >= value)
                    return x;
                x = x * 2;
            }
            return -1;
        }
        public static string GetResourceFullPath(string platform,string name)
        {
            return Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Template", platform,name); 
        }
        public static string GetResource(string platform,string name,ErrorsContainer ec)
        {
            try
            {
                string path = GetResourceFullPath(platform,name);
                return File.ReadAllText(path);
            }
            catch (Exception e)
            {
                if (ec==null)
                    MessageBox.Show("Error reading resource: "+platform+"::"+name+"\n"+ e.ToString());
                else
                    ec.AddError("GetResource", "Error reading resource: " + platform+"::"+name + "\n" + e.ToString());
                return "";
            }
        }
        public static bool CreateResource(string platform,string name, Dictionary<string, string> replacements, string outputFile, ErrorsContainer ec)
        {
            string content = GetResource(platform,name, replacements, ec);
            if (ec.HasErrors())
                return false;
            if (Disk.SaveFile(outputFile, content, ec) == false)
                return false;
            return true;
        }
        public static string GetResource(string platform,string name, Dictionary<string, string> replacements, ErrorsContainer ec)
        {
            string s = GetResource(platform,name,ec);
            if (replacements != null)
            {
                foreach (string key in replacements.Keys)
                {
                    s = s.Replace(key,replacements[key]);
                }
            }
            return s;
        }
        public static string ArrayToString(int ArrayIndex1,int ArrayIndex2)
        {
            if (ArrayIndex1 < 0)
                return "-";
            if (ArrayIndex2 < 0)
                return ArrayIndex1.ToString();
            return ArrayIndex1.ToString() + " , " + ArrayIndex2.ToString();
        }
        public static bool StringToArray(string value,ref int ArrayIndex1,ref int ArrayIndex2)
        {
            ArrayIndex1 = -1;
            ArrayIndex2 = -1;
            if (value == null)
                return false;
            if ((value.Length == 0) || (value.Equals("-")))
                return true;
            if (value.Contains(","))
            {
                string[] ss = value.Split(',');
                if ((ss == null) || (ss.Length != 2))
                    return false;
                int v1 = 0, v2 = 0;
                if (Int32.TryParse(ss[0].Trim(), out v1) == false)
                    return false;
                if (Int32.TryParse(ss[1].Trim(), out v2) == false)
                    return false;
                if ((v1 < 0) || (v2 < 0))
                    return false;
                ArrayIndex1 = v1;
                ArrayIndex2 = v2;
                return true;
            }
            else
            {
                if ((Int32.TryParse(value.Trim(), out ArrayIndex1) == false) || (ArrayIndex1 < 0))
                {
                    ArrayIndex1 = -1;
                    return false;
                }
                return true;
            }
        }
        public static string StringToCppString(string value)
        {
            return "\"" + value.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\t", "\\t").Replace("\r", "") + "\"";
        }
        public static string BasicTypeValueToCppValue(string value,BasicTypesConstantType type)
        {
            switch (type)
            {
                case BasicTypesConstantType.Float32:
                    if (value.Contains("."))
                        return value + "f";
                    else
                        return value + ".0f";
                case BasicTypesConstantType.Float64:
                    if (value.Contains("."))
                        return value;
                    else
                        return value + ".0";
                case BasicTypesConstantType.String:
                    return StringToCppString(value);
                default:
                    return value;
            }
        }
        public static string StringToFrameworkString(string value)
        {
            string ss = "";
            value = value.Replace("|~", "?");
            for (int tr = 0; tr < value.Length; tr++)
            {
                int x = value[tr];
                if (x < 127)
                {
                    ss += value[tr];
                }
                else
                {
                    int p1 = x / (32 * 32);
                    x = x % (32 * 32);
                    int p2 = x / 32;
                    int p3 = x % 32;
                    if ((p1 < 32) && (p2 < 32) && (p3 < 32))
                    {
                        ss += "|~";
                        ss += (char)(48 + p1);
                        ss += (char)(48 + p2);
                        ss += (char)(48 + p3);
                    }
                    else
                    {
                        ss += "?";
                    }
                }
            }
            return "\"" + ss.Replace("\"","\\\"") + "\"";
        }
        public static string GetProjectResourceSourceFullPath(string projectPath,string fileName)
        {
            return Path.Combine(projectPath, "Resources", "Sources", fileName);
        }
        public static string GetProjectResourceOutputFullPath(string projectPath, string fileName)
        {
            return Path.Combine(projectPath, "Resources", "Output", fileName);
        }
        public static bool ValidateVariableNameCorectness(string s,bool firstLetterMustBeCapital = true)
        {
            if (s == null)
                return false;
            if (s.Length == 0)
                return false;
            foreach (char ch in s)
            {
                if ((ch >= 'A') && (ch <= 'Z'))
                    continue;
                if ((ch >= '0') && (ch <= '9'))
                    continue;
                if ((ch >= 'a') && (ch <= 'z'))
                    continue;
                if (ch == '_')
                    continue;
                return false;
            }
            // primul caracter trebuie sa fie ok
            char ch2 = s[0];
            if ((ch2 >= 'A') && (ch2 <= 'Z'))
                return true;
            if (firstLetterMustBeCapital==false)
            {
                if ((ch2 >= 'a') && (ch2 <= 'z'))
                    return true;
            }
            return false;
        }
        public static void StringToVersion(string value,ref int Major,ref int Minor,ref int Build)
        {
            Major = Minor = Build = -1;
            if (value == null)
                return;
            if (value.Contains("."))
            {
                string[] ss = value.Split('.');
                if ((ss == null) || (ss.Length != 3))
                    return;
                int v1 = 0, v2 = 0, v3 = 0;
                if (Int32.TryParse(ss[0].Trim(), out v1) == false)
                    return;
                if (Int32.TryParse(ss[1].Trim(), out v2) == false)
                    return;
                if (Int32.TryParse(ss[2].Trim(), out v3) == false)
                    return;
                if ((v1 < 1) || (v2 < 0) || (v3<0))
                    return;
                if ((v1 > 255) || (v2 > 255) || (v3 > 65535))
                    return;
                Major = v1;
                Minor = v2;
                Build = v3;
            }
        }
        public static bool StringToProcent(string value,ref float procent)
        {
            value = value.Trim();
            if (value.EndsWith("%") == true)
                value = value.Substring(0, value.Length - 1).Trim();
            try
            {
                double d = Convert.ToDouble(value);
                d = d / 100.0;
                procent = (float)d;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static float StringToProcent(string value, float minInterval, float maxInterval, float errorValue,bool limitToInterval = false)
        {
            float v = 0;
            if (StringToProcent(value, ref v) == false)
                return errorValue;
            if (v<minInterval)
            {
                if (limitToInterval)
                    return minInterval;
                else
                    return errorValue;
            }
            if (v > maxInterval)
            {
                if (limitToInterval)
                    return maxInterval;
                else
                    return errorValue;
            }
            return v;
        }
        public static string ProcentToString(float value,int decimals = 2)
        {
            return (value*100.0f).ToString("F"+decimals.ToString(),CultureInfo.InvariantCulture)+" %";
        }
        public static bool ValidateStringVersionCorectness(string value)
        {
            int Major = -1;
            int Minor = -1;
            int Build = -1;
            StringToVersion(value, ref Major, ref Minor, ref Build);
            if (Major < 0)
                return false;
            return true;
        }
        public static string LanguageTo2DigitsSymbol(Language lang)
        {
            switch (lang)
            {
                case Language.English: return "en";
                case Language.German: return "de";
                case Language.Romanian: return "ro";
                case Language.Spanish: return "es";
                case Language.French: return "fr";
                case Language.Portuguese: return "pt";
                case Language.Italian: return "it";
                case Language.Russian: return "ru";
                case Language.Arabic: return "ar";
                case Language.Chinese: return "zh";
                case Language.Turkish: return "tr";
                case Language.Serbian: return "sr";
                case Language.Vietnamese: return "vi";
                case Language.Indonesian: return "id";
                case Language.Thai: return "th";
                case Language.Polish: return "pl";
                case Language.Hungarian: return "hu";
                case Language.Dutch: return "nl";
            };
            return "";
        }
        public static Dictionary<string,string> StringListToDict(string strList)
        {
            Dictionary<string, string> d = new Dictionary<string, string>();
            StringListToDict(strList, d);
            return d;
        }
        public static void StringListToDict(string strList, Dictionary<string, string> d, char separator = ',', bool trimWords = true,bool ignoreEmptyWords = true)
        {
            string tmp = "";
            int poz = 0;
            int index = 0;
            d.Clear();
            do
            {
                index = strList.IndexOf(separator, poz);
                if (index < 0)
                    index = strList.Length;
                if (trimWords)
                    tmp = strList.Substring(poz, index - poz).Trim();
                else
                    tmp = strList.Substring(poz, index - poz);
                if (ignoreEmptyWords)
                {
                    if (tmp.Length > 0)
                        d[tmp.ToLower()] = tmp;
                }
                else
                {
                    d[tmp.ToLower()] = tmp;
                }
                poz = index + 1;
            } while (poz < strList.Length);
        }
        public static void StringListToList(string strList,List<string> l,char separator=',',bool trimWords = true)
        {
            string tmp = "";
            int poz = 0;
            int index = 0;
            l.Clear();
            do
            {
                index = strList.IndexOf(separator, poz);
                if (index < 0)
                    index = strList.Length;
                if (trimWords)
                    tmp = strList.Substring(poz, index - poz).Trim();
                else
                    tmp = strList.Substring(poz, index - poz);
                if (tmp.Length > 0)
                    l.Add(tmp);
                poz = index + 1;
            } while (poz < strList.Length);
        }
        public static List<string> StringListToList(string strList, char separator = ',', bool trimWords = true)
        {
            List<string> l = new List<string>();
            StringListToList(strList, l, separator, trimWords);
            return l;
        }
        public static string ListToStringList(List<string> l)
        {
            string s = "";
            for(int tr=0;tr<l.Count;tr++)
            {
                s += l[tr];
                if ((tr + 1) < l.Count)
                    s += " ,";
            }
            return s;
        }
        public static string DeleteFromStringList(string strList,string toDelete)
        {
            toDelete = toDelete.ToLower();
            string tmp = "";
            int poz = 0;
            int index = 0;
            List<string> l = new List<string>();
            do
            {
                index = strList.IndexOf(',', poz);
                if (index < 0)
                    index = strList.Length;
                tmp = strList.Substring(poz, index - poz).Trim();
                if ((tmp.Length > 0) && (tmp.ToLower().Equals(toDelete)==false))
                    l.Add(tmp);
                poz = index + 1;
            } while (poz < strList.Length);
            return ListToStringList(l);
        }
        public static bool StringEnumListToDict(string enumList,Dictionary<string,int> d,out string error)
        {
            d.Clear();
            error = "";
            string[] words = enumList.Split(',');
            if (enumList.Trim().Length==0)
            {
                error = "Missing enum names and value. Use the following format: 'EnumName1 = integer value, EnumName2 = integer value, ...' !";
            }
            if ((words == null) || (words.Length == 0))
            {
                error = "Invalid format - it should be as follows: EnumName1 = integer value, EnumName2 = integer value, ...";
                return false;
            }
            foreach (string word in words)
            {
                int index = word.IndexOf('=');
                if (index < 0)
                {
                    error = "Invalid format - it should be as follows: EnumName1 = integer value, EnumName2 = integer value, ...";
                    return false;
                }
                string key = word.Substring(0, index).Trim();
                if (key.Length == 0)
                {
                    error = "Invalid enum name (empty) in '" + word + "' !";
                    return false;
                }
                string value = word.Substring(index + 1).Trim();
                if (value.Length == 0)
                {
                    error = "Invalid value name (empty) in '" + word + "' !";
                    return false;
                }
                int res = 0;
                if (Int32.TryParse(value,out res)==false)
                {
                    error = "Invalid value (non numerical) '" + value + "' in '" + word + "' !";
                    return false;
                }
                if (d.ContainsKey(key))
                {
                    error = "Enum name '" + key + "' is duplicate ! (in '" + word + "' !";
                    return false;
                }
                d[key] = res;
            }
            return true;
        }
        public static bool StringEnumListToDict(string enumList, Dictionary<string, sbyte> d, out string error)
        {
            d.Clear();
            error = "";
            string[] words = enumList.Split(',');
            if (enumList.Trim().Length == 0)
            {
                error = "Missing enum names and value. Use the following format: 'EnumName1 = integer value, EnumName2 = integer value, ...' !";
            }
            if ((words == null) || (words.Length == 0))
            {
                error = "Invalid format - it should be as follows: EnumName1 = integer value, EnumName2 = integer value, ...";
                return false;
            }
            foreach (string word in words)
            {
                int index = word.IndexOf('=');
                if (index < 0)
                {
                    error = "Invalid format - it should be as follows: EnumName1 = integer value, EnumName2 = integer value, ...";
                    return false;
                }
                string key = word.Substring(0, index).Trim();
                if (key.Length == 0)
                {
                    error = "Invalid enum name (empty) in '" + word + "' !";
                    return false;
                }
                string value = word.Substring(index + 1).Trim();
                if (value.Length == 0)
                {
                    error = "Invalid value name (empty) in '" + word + "' !";
                    return false;
                }
                sbyte res = 0;
                if (sbyte.TryParse(value, out res) == false)
                {
                    error = "Invalid value (non numerical or not an Int8) '" + value + "' in '" + word + "' !";
                    return false;
                }
                if (d.ContainsKey(key))
                {
                    error = "Enum name '" + key + "' is duplicate ! (in '" + word + "' !";
                    return false;
                }
                d[key] = res;
            }
            return true;
        }
        public static float GetResolutionScale(int originalWidth,int originalHeight,int newWidth,int newHeight)
        {
            float r1 = (float)newWidth / (float)originalWidth;
            float r2 = (float)newHeight / (float)originalHeight;
            return Math.Min(r1, r2);
        }
        public static string StringToBase64(string text, ErrorsContainer ec = null)
        {
            if ((text == null) || (text.Length == 0))
                return "";
            try
            {
                return System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(text));
            }
            catch (Exception e)
            {
                if (ec != null)
                    ec.AddException("Unable to convert string to base64", e);
                return null;
            }            
        }
        public static string Base64ToString(string base64Format, ErrorsContainer ec = null)
        {
            if ((base64Format == null) || (base64Format.Length == 0))
                return "";
            try
            {
                return System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(base64Format));
            }
            catch (Exception e)
            {
                if (ec != null)
                    ec.AddException("Unable to convert a Base64 to string", e);
                return null;
            }
        }

        public static byte[] ImageToBuffer(Bitmap Picture)
        {
            byte[] b = new byte[Picture.Width * Picture.Height * 4 + 9];
            if (b == null)
                return null;

            b[0] = (byte)'R';
            b[1] = (byte)'I';
            b[2] = (byte)'M';
            b[3] = (byte)'G';
            b[4] = (byte)(Picture.Width % 256);
            b[5] = (byte)(Picture.Width / 256);
            b[6] = (byte)(Picture.Height % 256);
            b[7] = (byte)(Picture.Height / 256);
            b[8] = 0;
            int index = 9;
            for (int y = 0; y < Picture.Height; y++)
            {
                for (int x = 0; x < Picture.Width; x++)
                {
                    Color c = Picture.GetPixel(x, y);
                    b[index++] = c.R;
                    b[index++] = c.G;
                    b[index++] = c.B;
                    b[index++] = c.A;
                }
            }
            return b;
        }

        public static Size[] Resolutions = new Size[] { 
            new Size(320, 200), new Size(480, 320), new Size(640, 480), new Size(800, 480), new Size(800, 600), new Size(960, 640), new Size(1024,600), new Size(1280,720), new Size(1280,768), new Size(1280, 800), 
            new Size(1920,1080), new Size(2560, 1440)
        };

        public static int ScreenResolutionMaximizedWindow = 100000;
        public static int ScreenResolutionFullScreen = 100001;
        public static int ScreenResolutionBestFit = 100002;

        #endregion
    }
}
