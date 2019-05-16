using System.Xml.Serialization;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System;
using System.Collections.Generic;
//using System.Windows.Forms;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.Reflection;
using PluginInterface;
using System.Drawing.Imaging;

namespace GAppCreator
{
    public delegate void fnShowError(ErrorsContainer EC);
    public enum ResourceType
    {
        RasterImage,
        VectorImage,
        Music,
        Presentation,
        Raw,
    };
    public enum Command
    {
        Copy,
        Cut,
        Paste,
        Undo,
        Redo,
        SelectAll,
        Delete,

        ExpandAll,
        CollapseAll,
        CollapseToDefinitions,

        BuildSelectedResources,

        Find,
        FindWindow,
        Replace,
        ReplaceWindow,
        FindAllInCurrentDocument,
        FindAllInCurrentProject,

        GoToLine,
        GoToNextError,
        GoToDefinition,
        OpenGoToDefinitionDialog,
        SearchDefinitionInCurrentFile,

        Comment,
        UnComment,

        ESCPressed,

        Run,
        Compile,
        QuickRun,
        RunInVisualStudio,

        ApiBrowser,

        DisableResourceList,
        EnableResourceList,
        UpdateResourceList,
        UpdateResourceIcon,
        UpdateApplicationIcon,
        UpdateApplicationSplashScreen,
        UpdateGacFileList,
        OpenSourceCodeFile,
        AddCompileError,
        AddCompileOutput,
        AddIntelliSenseError,
        UpdateGlobalAutoComplete,
        UpdateLocalTypes,
        EnableIntelliSenseTimer,
        UpdatePreview,
        RunFile,
    };
    public enum ZipContentType : int
    {
        None = 0,
        GACSources = 0x00000001,
        SourceResources = 0x00000002,
        FontTemplates = 0x00000004,
        ProjectXML = 0x00000008,
        OutputResources = 0x00000010,
        Plugins = 0x00000020,
        PluginsSources = 0x00000040,
        Binaries = 0x00000080,
        IconsAndSplashScreen = 0x00000100,
        PublishMaterials = 0x00000200,
        Advertisments = 0x00000400,
        Strings = 0x00000800,
        SystemSettingSnapshots = 0x00001000,
    }
    public class AppResources
    {
        public Dictionary<string, ImageResource> Images = new Dictionary<string, ImageResource>();
        public Dictionary<string, FontResource> Fonts = new Dictionary<string, FontResource>();
        public Dictionary<string, ShaderResource> Shaders = new Dictionary<string, ShaderResource>();
        public Dictionary<string, StringValues> Strings = new Dictionary<string, StringValues>();
        public Language Lang = Language.English;
    }
    public class PluginAssemblyLoader : MarshalByRefObject
    {
        public void Load(string path)
        {
            ValidatePath(path);
            Assembly.Load(path);
        }

        public void LoadFrom(string path)
        {
            ValidatePath(path);
            Assembly.LoadFrom(path);
        }

        private void ValidatePath(string path)
        {
            if (path == null) throw new ArgumentNullException("path");
            if (!System.IO.File.Exists(path))
                throw new ArgumentException(String.Format("path \"{0}\" does not exist", path));
        }
    }
    public class ResourcePluginData
    {
        public PluginInterface.ResourcePlugin Interface = null;
        public Assembly assembly = null;
        public AppDomain Domain = null;
        public string ModuleName = "";  
        public void Unload()
        {
            assembly = null;
            Interface = null;
            ModuleName = null;
            if (Domain!=null)
            {
                AppDomain.Unload(Domain);
            }
        }
    };
    public class PluginList
    {
        public Dictionary<string, ResourcePluginData> Plugins = new Dictionary<string, ResourcePluginData>();
        public Dictionary<string, string> ModuleToExtension = new Dictionary<string, string>(); // relatie 
        public List<string> AvailableModules = new List<string>();
        public Project prj = null;
        
        public void RefreshAvailablePlugins()
        {
            AvailableModules.Clear();
            if (prj==null)
                return;
            foreach (string fname in Directory.GetFiles(prj.GetProjectResourcePluginsFolder()))
            {
                if (fname.ToLower().EndsWith(".plugin.dll") == false)
                    continue;
                string name = Path.GetFileName(fname.ToLower());
                if (ModuleToExtension.ContainsKey(name))
                    continue;
                AvailableModules.Add(name);
            }
            AvailableModules.Sort();
        }
        private ResourcePluginData LoadModule(string name)
        {
            if (prj == null)
                return null;
            Assembly assembly = null;
            AppDomain domain = null;
            try
            {
                string fname = Path.Combine(prj.GetProjectResourcePluginsFolder(), name);
                assembly = Assembly.Load(File.ReadAllBytes(fname));
                AppDomainSetup setup = new AppDomainSetup();
                setup.ApplicationBase = prj.GetProjectResourcePluginsFolder();
                domain = AppDomain.CreateDomain(Guid.NewGuid().ToString(), null, setup);
                Type[] types = assembly.GetTypes();
                Type pluginType = typeof(PluginInterface.ResourcePlugin); 
                foreach (Type type in types)
                {
                    if ((type.IsInterface) || (type.IsAbstract))
                        continue;
                    if (type.GetInterface(pluginType.FullName) != null)
                    {
                        // am gasit un obiect - il returnez\
                        ResourcePluginData rpd = new ResourcePluginData();
                        rpd.ModuleName = name.ToLower();
                        rpd.Domain = domain;
                        rpd.assembly = assembly;
                        rpd.Interface = (PluginInterface.ResourcePlugin)assembly.CreateInstance(type.ToString());

                        if (rpd.Interface==null)
                        {
                            prj.EC.AddError("Unable to create an instance from type: " + type.ToString());
                            if (domain != null)
                            {
                                assembly = null;
                                AppDomain.Unload(domain);
                            }                            
                            return null;
                        }
                        return rpd;
                    }
                }                
                prj.EC.AddError("None of the exports from " + name + " implements 'ResourcePlugin' interface !");
                if (domain != null)
                {
                    assembly = null;
                    AppDomain.Unload(domain);
                }
                return null;
            }
            catch (Exception e)
            {
                if (domain!=null)
                {
                    assembly = null;
                    AppDomain.Unload(domain);
                }
                prj.EC.AddException("Unable to load: " + name + "\r\nFile:" + Path.Combine(prj.GetProjectResourcePluginsFolder(), name)+"\r\n", e);
                return null;
            }
        }
        public bool ReloadPlugins()
        {
            foreach (string key in Plugins.Keys)
            {
                Plugins[key].Unload();
            }
            Plugins.Clear();
            ModuleToExtension.Clear();
            Dictionary<string, string> lst = Project.StringListToDict(prj.PluginList);
            bool allOK = true;
            foreach (string s in lst.Keys)
            {
                string name = s;
                if (name.EndsWith(".plugin.dll")==false)
                    name+=".plugin.dll";
                if ((s!=name) && (lst.ContainsKey(name)))
                    continue; // se gaseste de doua ori - o data fara si o data cu extensie
                ResourcePluginData p = LoadModule(name);
                if (p==null)
                {
                    allOK = false;
                    continue;
                }
                if (Plugins.ContainsKey(p.Interface.GetResourceTypeExtension().ToLower()))
                {
                    if (prj!=null)
                        prj.EC.AddError("Plugin Manager","There is already a plugin that is associated to '"+p.Interface.GetResourceTypeExtension()+"' extension !");
                    allOK = false;
                    p.Unload();
                    continue;
                }
                Plugins[p.Interface.GetResourceTypeExtension().ToLower()] = p;
                ModuleToExtension[name] = p.Interface.GetResourceTypeExtension().ToLower();
            }
            return allOK;
        }
        public void UpdateProjectPlugins()
        {
            string s = "";
            foreach (string m in ModuleToExtension.Keys)
                s += m + ",";
            s = s.Trim();
            if (s.EndsWith(","))
                s = s.Substring(0, s.Length - 1);
            prj.PluginList = s.Trim();
        }
        public void AddPlugin(string name)
        {
            UpdateProjectPlugins();
            if (prj.PluginList.Length == 0)
                prj.PluginList = name.Trim();
            else
                prj.PluginList += ","+name.Trim();
        }
        public void DeletePlugin(string name)
        {
            UpdateProjectPlugins();
            name = name.ToLower();
            Dictionary<string,string> d = Project.StringListToDict(prj.PluginList);
            if (d.ContainsKey(name))
                d.Remove(name);
            string s = "";
            foreach (string m in d.Keys)
                s += d[m]+ ",";
            s = s.Trim();
            if (s.EndsWith(","))
                s = s.Substring(0, s.Length - 1);
            prj.PluginList = s.Trim();
        }
    }


    public class PreviewData
    {
        public string PreviewDataType = "";
        public Object Data = null;
    };

    public class SystemSettings
    {
        [XmlAttribute()]
        public string InskapePath = "";
        [XmlAttribute()]
        public string ImageEditorPath = "";
        [XmlAttribute()]
        public string FrameworkSourcesPath = "";
        [XmlAttribute()]
        public string AndroidSDKFolder = "";
        [XmlAttribute()]
        public string JavaFolder = "";
        [XmlAttribute()]
        public string AndroidNDKBuildCMD = "ndk-build.cmd";
        [XmlAttribute()]
        public string AndroidSignKeystore = "";
        [XmlAttribute()]
        public string AndroidKeystoreAlias = "";
        [XmlAttribute()]
        public string AndroidSignPass = "";
        [XmlAttribute()]
        public string AndroidAssetPackagingTool = "";
        [XmlAttribute()]
        public string AndroidADB = "adb.exe";
        [XmlAttribute()]
        public string AndroidDexTool = "";
        [XmlAttribute()]
        public string AndroidZipAlign = "";
        [XmlAttribute()]
        public string AndroidSDKPlatform = "";
        [XmlAttribute()]
        public string VSVarsBat = "";
        [XmlAttribute()]
        public string CL = "";
        [XmlAttribute()]
        public string VSDevEnv = "";
        [XmlAttribute()]
        public string VSToolSet = "";
        [XmlAttribute()]
        public string RepoFolder = "";
        [XmlAttribute()]
        public string BuildFolder = "";
        [XmlAttribute()]
        public string BuildServerAddress = "";
        [XmlAttribute()]
        public string UserName = "";
        [XmlAttribute()]
        public string Password = "";
        [XmlAttribute()]
        public string ProguardPath = "";
        [XmlAttribute()]
        public string GitPath = "git.exe";
        [XmlAttribute()]
        public string GitUserName = "";
        [XmlAttribute()]
        public string GitPassword = "";

        public void Save()
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(SystemSettings));
                TextWriter textWriter = new StreamWriter(Application.ExecutablePath + ".settings");
                serializer.Serialize(textWriter, this);
                textWriter.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show("Unable to save settings !\n" + e.ToString());
            }
        }
        public static SystemSettings Load()
        {
            if (File.Exists(Application.ExecutablePath + ".settings") == false)
                return new SystemSettings();
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(SystemSettings));
                TextReader textReader = new StreamReader(Application.ExecutablePath + ".settings");
                SystemSettings sol = (SystemSettings)serializer.Deserialize(textReader);
                textReader.Close();
                if (sol != null)
                    return sol;
                MessageBox.Show("Unable to load settings !");
                return new SystemSettings();
            }
            catch (Exception e)
            {
                MessageBox.Show("Unable to load settings !\n" + e.ToString());
                return new SystemSettings();
            }
        }

    };
    public enum OSType
    {
        None,
        WindowsDesktop,
        Android,
        IOS,
        Mac,
        Linux,
    };
    public enum TaskType
    {
        ToDo,
        Bug,
        Feature,
        NextUpdate,
        Ideea,
    };
    [XmlType("Task"), XmlRoot("Task")]
    public class ProjectTask
    {
        [XmlAttribute()]
        public string Text = "";
        [XmlAttribute()]
        public string AddedOn = "";
        [XmlAttribute()]
        public string CompletedOn = "";
        [XmlAttribute()]
        public TaskType Type = TaskType.ToDo;

        public void SetNow(bool toAddedOnField)
        {
            DateTime dt = DateTime.Now;
            string s = string.Format("{0:D4}-{1:D2}-{2:D2}  {3:D2}:{4:D2}:{5:D2}", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
            if (toAddedOnField)
                AddedOn = s;
            else
                CompletedOn = s;
        }
    }

    [XmlType("File"), XmlRoot("File")]
    public class ProjectFile
    {
        [XmlAttribute()]
        public string Name = "";
        [XmlAttribute()]
        public bool Opened = false;

        //protected internal GACEditor Editor = null;
        [XmlIgnore()]
        public Object Editor = null;
        [XmlIgnore()]
        public GACParser Parser = new GACParser();
        [XmlIgnore()]
        public int ErrorsCount = 0;
    }
    public class LineErrorAnalyzer
    {
        public static string FileName;
        public static int Line;
        public static int StartPosition, EndPosition;
        public static string Content;

        public static void Analyze(string error)
        {
            int start, middle, end;
            FileName = "";
            Line = -1;
            StartPosition = EndPosition = -1;
            Content = error.Trim();

            start = error.IndexOf("(");
            if (start > 0)
            {
                FileName = error.Substring(0, start).Trim();
                end = error.IndexOf(")", start);
                if (end > 0)
                {
                    if (int.TryParse(error.Substring(start + 1, end - (start + 1)), out Line) == false)
                        Line = -1;
                    Content = error.Substring(end + 1).Trim();
                }
            }

            start = error.IndexOf(")[");
            if (start > 0)
            {
                start += 2;
                middle = error.IndexOf('-', start);
                if (middle > start)
                {
                    end = error.IndexOf(']', middle + 1);
                    if (end > middle)
                    {
                        if (int.TryParse(error.Substring(start, middle - start), out StartPosition) == false)
                            StartPosition = -1;
                        if (int.TryParse(error.Substring(middle + 1, end - (middle + 1)), out EndPosition) == false)
                            EndPosition = -1;
                        if ((EndPosition <= StartPosition) || (StartPosition < 0))
                        {
                            StartPosition = EndPosition = -1;
                        }
                        Content = error.Substring(end + 1).Trim();
                    }
                }
            }
        }
    }
    public class ArrayCounter
    {
        protected class ArrayInfo
        {
            public int max1 = -1, max2 = -1,count = 0;
        };
        protected Dictionary<string, ArrayInfo> array = new Dictionary<string, ArrayInfo>();
        public bool Add(string name, int array1, int array2)
        {
            ArrayInfo ai;
            if (array.ContainsKey(name) == false)
            {
                ai = new ArrayInfo();
                array[name] = ai;
            }
            else
            {
                ai = array[name];
            }
            bool result = true;
            if (ai.count>0)
            {
                if ((array1<0) && (ai.max1>=0))
                    result = false;
                if ((array1>=0) && (ai.max1<0))
                    result = false;
                if ((array2 < 0) && (ai.max2 >= 0))
                    result = false;
                if ((array2 >= 0) && (ai.max2 < 0))
                    result = false;
            }
            if (array1 > ai.max1)
                ai.max1 = array1;
            if (array2 > ai.max2)
                ai.max2 = array2;
            ai.count++;
            return result;
        }
        public void Add(GenericResource r)
        {
            Add(r.Name, r.Array1, r.Array2);
        }
        public int GetArray1(string name)
        {
            if (array.ContainsKey(name))
            {
                if (array[name].max1 == -1)
                    return -1;
                return array[name].max1 + 1;
            }
            return -1;
        }
        public int GetArray2(string name)
        {
            if (array.ContainsKey(name))
            {
                if (array[name].max2 == -1)
                    return -1;
                return array[name].max2 + 1;
            }
            return -1;
        }
        public void Add(List<GenericResource> l, Type t)
        {
            foreach (GenericResource r in l)
                if (r.GetType() == t)
                    Add(r);
        }
        public string[] Variables
        {
            get
            {
                string[] s = new string[array.Keys.Count];
                array.Keys.CopyTo(s, 0);
                return s;
            }
        }
        public string GetVariableName(string name)
        {
            if (array.ContainsKey(name))
                return Project.GetVariableName(name, GetArray1(name), GetArray2(name));
            return "";
        }
        public void Clear()
        {
            array.Clear();
        }
    }

    public enum CheckBoxAttributeType
    {
        Builds,
        BuildsNoDevelop,
    };
    public class CheckBoxTypeEditor : UITypeEditor
    {
        public static string Builds = null;
        public static string BuildsNoDevelop = null;
        public class Source : Attribute
        {
            public string sursa = "";
            public CheckBoxAttributeType attrType;
            public Source(string value) { sursa = value; }
            public Source(CheckBoxAttributeType type) { attrType = type; sursa = null; }
            public Dictionary<string, string> GetOptions()
            {
                if (sursa != null)
                    return Project.StringListToDict(sursa);
                switch (attrType)
                {
                    case CheckBoxAttributeType.Builds:
                        return Project.StringListToDict(CheckBoxTypeEditor.Builds);
                    case CheckBoxAttributeType.BuildsNoDevelop:
                        return Project.StringListToDict(CheckBoxTypeEditor.BuildsNoDevelop);
                }
                return new Dictionary<string, string>();
            }
        }
        private string result = "";

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }
        public override bool IsDropDownResizable
        {
            get
            {
                return false;
            }
        }
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            result = "";        
            Source s = (Source)context.PropertyDescriptor.Attributes[typeof(Source)];
            if (s==null)
                return "";
            Dictionary<string, string> options = s.GetOptions();
            IWindowsFormsEditorService es = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            if ((es != null) && (s != null))
            {
                CheckedListBox cb = new CheckedListBox();
                cb.BeginUpdate();
                Dictionary<string, string> values = Project.StringListToDict(value.ToString());
                foreach (string opt in options.Keys)
                {
                    cb.Items.Add(options[opt]);
                    cb.SetItemChecked(cb.Items.Count - 1, values.ContainsKey(opt));
                }
                cb.Sorted = true;
                cb.CheckOnClick = true;
                cb.EndUpdate();
    
                cb.Leave += cb_Leave;
                es.DropDownControl(cb);

            }
            return result;
        }

        void cb_Leave(object sender, EventArgs e)
        {
            CheckedListBox cb = (CheckedListBox)sender;
            string s = "";
            for (int tr = 0; tr < cb.Items.Count; tr++)
                if (cb.GetItemChecked(tr))
                    s += cb.Items[tr].ToString() + " , ";
            if (s.Length > 0)
                s = s.Substring(0, s.Length - 3);
            result = s;
        }

    }
    class GlyphVersionsEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
        public override object EditValue(ITypeDescriptorContext context, System.IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService svc = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
            string tmp = value as string;
            if ((svc != null) && (tmp != null))
            {
                FontVersionEditor dlg = new FontVersionEditor(tmp);
                dlg.TopMost = true;
                if (dlg.ShowDialog() == DialogResult.OK)
                    value = dlg.ResultVersion;
            }
            return value; // can also replace the wrapper object here
        }
    }
    class ResolutionEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
        public override object EditValue(ITypeDescriptorContext context, System.IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService svc = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
            string tmp = value as string;
            if ((svc != null) && (tmp != null))
            {
                ResolutionEditorDialog dlg = new ResolutionEditorDialog(tmp);
                dlg.TopMost = true;
                if (dlg.ShowDialog() == DialogResult.OK)
                    value = dlg.SelectedResolution;
            }
            return value; // can also replace the wrapper object here
        }
    }
    class InAppItemsEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
        public override object EditValue(ITypeDescriptorContext context, System.IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService svc = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
            string tmp = value as string;
            if ((svc != null) && (tmp != null))
            {
                InAppItemsEditDialog dlg = new InAppItemsEditDialog(tmp);
                dlg.TopMost = true;
                if (dlg.ShowDialog() == DialogResult.OK)
                    value = dlg.InAppItemList;
            }
            return value; // can also replace the wrapper object here
        }
    }
    class SettingsSnapshotEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
        public override object EditValue(ITypeDescriptorContext context, System.IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService svc = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
            string tmp = value as string;
            if ((svc != null) && (tmp != null))
            {
                SettingsSnapshotEditDialog dlg = new SettingsSnapshotEditDialog(tmp);
                dlg.TopMost = true;
                if (dlg.ShowDialog() == DialogResult.OK)
                    value = dlg.Result;
            }
            return value; // can also replace the wrapper object here
        }
    }
    class MarketStringEditor: UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
        public override object EditValue(ITypeDescriptorContext context, System.IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService svc = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
            string tmp = value as string;
            if ((svc != null) && (tmp != null))
            {
                MarketStringEditorDialog dlg = new MarketStringEditorDialog(tmp);
                dlg.TopMost = true;
                if (dlg.ShowDialog() == DialogResult.OK)
                    value = dlg.ResultedURL;
            }
            return value; // can also replace the wrapper object here
        }
    }
    public class ImageSelectorEditor : UITypeEditor
    {
        public static Form EditForm = null;

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
        public override object EditValue(ITypeDescriptorContext context, System.IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService svc = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
            string tmp = value as string;
            if ((svc != null) && (tmp != null))
            {
                if (EditForm != null)
                {
                    if (EditForm.ShowDialog() == DialogResult.OK)
                    {
                        if (EditForm.Tag != null)
                            value = (EditForm.Tag).ToString();
                    }
                }
            }
            return value; // can also replace the wrapper object here
        }
    }
    public class SoundSelectorEditor : UITypeEditor
    {
        public static Form EditForm = null;

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
        public override object EditValue(ITypeDescriptorContext context, System.IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService svc = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
            string tmp = value as string;
            if ((svc != null) && (tmp != null))
            {
                if (EditForm != null)
                {
                    if (EditForm.ShowDialog() == DialogResult.OK)
                    {
                        if (EditForm.Tag != null)
                            value = (EditForm.Tag).ToString();
                    }
                }
            }
            return value; // can also replace the wrapper object here
        }
    }
    public class FontSelectorEditor : UITypeEditor
    {
        public static Form EditForm = null;
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
        public override object EditValue(ITypeDescriptorContext context, System.IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService svc = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
            string tmp = value as string;
            if ((svc != null) && (tmp != null))
            {
                if (EditForm != null)
                {
                    if (EditForm.ShowDialog() == DialogResult.OK)
                    {
                        if (EditForm.Tag != null)
                            value = (EditForm.Tag).ToString();
                    }
                }
            }
            return value; // can also replace the wrapper object here
        }
    }
    public class ShaderSelectorEditor : UITypeEditor
    {
        public static Form EditForm = null;
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
        public override object EditValue(ITypeDescriptorContext context, System.IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService svc = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
            string tmp = value as string;
            if ((svc != null) && (tmp != null))
            {
                if (EditForm != null)
                {
                    if (EditForm.ShowDialog() == DialogResult.OK)
                    {
                        if (EditForm.Tag != null)
                            value = (EditForm.Tag).ToString();
                    }
                }
            }
            return value; // can also replace the wrapper object here
        }
    }
    public class StringSelectorEditor : UITypeEditor
    {
        public static Form EditForm = null;
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
        public override object EditValue(ITypeDescriptorContext context, System.IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService svc = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
            string tmp = value as string;
            if ((svc != null) && (tmp != null))
            {
                if (EditForm != null)
                {
                    if (EditForm.ShowDialog() == DialogResult.OK)
                    {
                        if (EditForm.Tag != null)
                            value = (EditForm.Tag).ToString();
                    }
                }
            }
            return value; // can also replace the wrapper object here
        }
    }
    public class CounterGroupSelectorEditor : UITypeEditor
    {
        public static Project prj = null;
        private string result = "";
        private IWindowsFormsEditorService es = null;
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }
        public override bool IsDropDownResizable
        {
            get
            {
                return false;
            }
        }
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            es = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            result = "";
            if (es != null)
            {
                ListBox cb = new ListBox();
                cb.BeginUpdate();
                cb.Items.Add("<None>");
                int index = 0;
                if (prj!=null)
                {         
                    int tr=1;
                    foreach (CounterGroup gcg in prj.CountersGroups) {
                        cb.Items.Add(gcg.Name);
                        if ((value!=null) && (gcg.Name.Equals(value.ToString())))
                            index = tr;
                        tr++;
                    }
                }
                cb.SelectedIndex = index;
                cb.Sorted = true;
                cb.EndUpdate();
    
                cb.Leave += cb_Leave;
                cb.SelectionMode = SelectionMode.One;
                cb.SelectedValueChanged += cb_SelectedValueChanged;
                es.DropDownControl(cb);
            }
            return result;
        }

        void cb_SelectedValueChanged(object sender, EventArgs e)
        {
            cb_Leave(sender, e);
            if (es!=null)
                es.CloseDropDown();
        }

        void cb_Leave(object sender, EventArgs e)
        {
            ListBox cb = (ListBox)sender;
            if (cb.SelectedIndex <= 0)
                result = "";
            else
                result = cb.Items[cb.SelectedIndex].ToString();
        }
    }
    public class SceneSelectorEditor : UITypeEditor
    {
        private string result = "";
        private IWindowsFormsEditorService es = null;
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }
        public override bool IsDropDownResizable
        {
            get
            {
                return false;
            }
        }
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            es = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            result = "";
            if (es != null)
            {
                ListBox cb = new ListBox();
                cb.BeginUpdate();
                cb.Items.Add("<None>");
                int index = 0;

                GACParser.Module moduleScenes = GACParser.Compiler.GetModule("Scenes");
                if (moduleScenes != null)
                {
                    int tr = 1;
                    foreach (string sceneName in moduleScenes.Members.Keys)
                    {
                        cb.Items.Add(sceneName);
                        if ((value != null) && (sceneName.Equals(value.ToString())))
                            index = tr;
                        tr++;
                    }
                }


                cb.SelectedIndex = index;
                cb.Sorted = true;
                cb.EndUpdate();

                cb.Leave += cb_Leave;
                cb.SelectionMode = SelectionMode.One;
                cb.SelectedValueChanged += cb_SelectedValueChanged;
                es.DropDownControl(cb);
            }
            return result;
        }

        void cb_SelectedValueChanged(object sender, EventArgs e)
        {
            cb_Leave(sender, e);
            if (es != null)
                es.CloseDropDown();
        }

        void cb_Leave(object sender, EventArgs e)
        {
            ListBox cb = (ListBox)sender;
            if (cb.SelectedIndex <= 0)
                result = "";
            else
                result = cb.Items[cb.SelectedIndex].ToString();
        }
    }
    public class EventIDSelectorEditor : UITypeEditor
    {
        public static Project prj = null;
        private string result = "";
        private IWindowsFormsEditorService es = null;
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }
        public override bool IsDropDownResizable
        {
            get
            {
                return false;
            }
        }
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            es = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            result = "";
            if (es != null)
            {
                ListBox cb = new ListBox();
                cb.BeginUpdate();
                int index = -1;
                if (prj!=null)
                {
                    int tr = 0;
                    foreach (var e in prj.ObjectEventsIDs)
                    {
                        if ((value != null) && (e.Name.Equals(value)))
                            index = tr;
                        cb.Items.Add(e.Name);
                        tr++;
                    }
                }
                
                cb.SelectedIndex = index;
                cb.Sorted = true;
                cb.EndUpdate();

                cb.Leave += cb_Leave;
                cb.SelectionMode = SelectionMode.One;
                cb.SelectedValueChanged += cb_SelectedValueChanged;
                es.DropDownControl(cb);
            }
            return result;
        }

        void cb_SelectedValueChanged(object sender, EventArgs e)
        {
            cb_Leave(sender, e);
            if (es != null)
                es.CloseDropDown();
        }

        void cb_Leave(object sender, EventArgs e)
        {
            ListBox cb = (ListBox)sender;
            if (cb.SelectedIndex < 0)
                result = "";
            else
                result = cb.Items[cb.SelectedIndex].ToString();
        }
    }
    public class CounterEnableConditionEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService svc = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
            string tmp = value as string;
            if ((svc != null) && (tmp != null))
            {
                CounterAuttoEnableStateEditor dlg = new CounterAuttoEnableStateEditor(tmp);
                dlg.TopMost = true;
                if (dlg.ShowDialog() == DialogResult.OK)
                    value = dlg.ResultVersion;
            }
            return value; // can also replace the wrapper object here
        }
    }
    public interface UITypeEditorAnimationZOrderWrapper
    {
        string Edit(string currentValue);
    };
    public class AnimationZOrderEditor : UITypeEditor
    {
        public static UITypeEditorAnimationZOrderWrapper editor = null;
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
        public override object EditValue(ITypeDescriptorContext context, System.IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService svc = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
            string tmp = value as string;
            if ((svc != null) && (tmp != null))
            {
                if (editor != null)
                    value = editor.Edit(tmp);
            }
            return value;
        }
    }
    public interface UITypeEditorStringWrapper
    {
        string Edit(string currentValue);
    };
    public class ImageListSelectorEditor : UITypeEditor
    {
        public static UITypeEditorStringWrapper editor = null;
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
        public override object EditValue(ITypeDescriptorContext context, System.IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService svc = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
            string tmp = value as string;
            if ((svc != null) && (tmp != null))
            {
                if (editor != null)
                    value = editor.Edit(tmp);
            }
            return value; 
        }
    }
    public interface UITypeEditorButtonStateWrapper
    {
        string Edit(string currentValue);
    };
    public class ButtonStateSelectorEditor : UITypeEditor
    {
        public static UITypeEditorButtonStateWrapper editor = null;
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
        public override object EditValue(ITypeDescriptorContext context, System.IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService svc = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
            string tmp = value as string;
            if ((svc != null) && (tmp != null))
            {
                if (editor != null)
                    value = editor.Edit(tmp);
            }
            return value;
        }
    }
    public class AnimationElementEditor : UITypeEditor
    {
        public static AnimO.AnimationObject currentAnimationObject = null;
        public static AnimO.GenericElementTransformation currentTransformation = null;
        private string result = "";
        private IWindowsFormsEditorService es = null;
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }
        public override bool IsDropDownResizable
        {
            get
            {
                return false;
            }
        }
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            es = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            result = "";
            if (es != null)
            {
                ListBox cb = new ListBox();
                cb.BeginUpdate();
                int index = -1;
                if ((currentAnimationObject != null) && (currentTransformation!=null))
                {
                    Type t = currentTransformation.GetType();
                    int tr = 0;
                    foreach (AnimO.GenericElement e in currentAnimationObject.Elements)
                    {
                        if (AnimO.Factory.IsTransformationCompatible(t,e))
                        {
                            cb.Items.Add(e.Name);
                            if ((value != null) && (e.Name.Equals(value.ToString())))
                                index = tr;
                            tr++;
                        }
                    }
                    cb.SelectedIndex = index;
                    cb.Sorted = true;
                }
                cb.EndUpdate();

                cb.Leave += cb_Leave;
                cb.SelectionMode = SelectionMode.One;
                cb.SelectedValueChanged += cb_SelectedValueChanged;
                es.DropDownControl(cb);
            }
            return result;
        }

        void cb_SelectedValueChanged(object sender, EventArgs e)
        {
            cb_Leave(sender, e);
            if (es != null)
                es.CloseDropDown();
        }

        void cb_Leave(object sender, EventArgs e)
        {
            ListBox cb = (ListBox)sender;
            if (cb.SelectedIndex < 0)
                result = "";
            else
                result = cb.Items[cb.SelectedIndex].ToString();
        }
    }

    public class AnimationElementRelativePositionEditor : UITypeEditor
    {
        public static AnimO.AnimationObject currentAnimationObject = null;
        private string result = "";
        private IWindowsFormsEditorService es = null;
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }
        public override bool IsDropDownResizable
        {
            get
            {
                return false;
            }
        }
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            es = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            result = "";
            if (es != null)
            {
                ListBox cb = new ListBox();
                cb.BeginUpdate();
                int index = -1;
                int tr = 1;
                cb.Items.Add("<None>");
                foreach (AnimO.GenericElement e in currentAnimationObject.Elements)
                {
                    cb.Items.Add(e.Name);
                    if ((value != null) && (e.Name.Equals(value.ToString())))
                            index = tr;
                    tr++;
                }
                if (index == -1)
                {
                    if ((value != null) && ((value.Equals("")) || (value.Equals("<None>"))))
                        index = 0;
                }
                cb.SelectedIndex = index;
                cb.Sorted = true;
                cb.EndUpdate();

                cb.Leave += cb_Leave;
                cb.SelectionMode = SelectionMode.One;
                cb.SelectedValueChanged += cb_SelectedValueChanged;
                es.DropDownControl(cb);
            }
            return result;
        }

        void cb_SelectedValueChanged(object sender, EventArgs e)
        {
            cb_Leave(sender, e);
            if (es != null)
                es.CloseDropDown();
        }

        void cb_Leave(object sender, EventArgs e)
        {
            ListBox cb = (ListBox)sender;
            if (cb.SelectedIndex < 0)
                result = "";
            else
                result = cb.Items[cb.SelectedIndex].ToString();
        }
    }

    #region Constants, Structures and Enums

    public enum BasicTypesConstantType
    {
        None,
        Boolean,
        Int8,
        Int16,
        Int32,
        Int64,
        UInt8,
        UInt16,
        UInt32,
        UInt64,      
        Float32,
        Float64,
        String,
        Color
    };
    public enum ResourcesConstantType
    {
        None,
        Presentation,
        Font,
        Image,
        Shader,
        Sound,
        String,
        Raw,
    };
    public enum ConstantModeType
    {
        BasicTypes,
        Resources,
        Enumerations,
        DataTypes
    };

    public class ConstantHelper
    {
        public static ConstantModeType GetConstantMode(string type)
        {
            if (type == null)
                return ConstantModeType.BasicTypes;
            if (type.StartsWith("Resources::"))
                return ConstantModeType.Resources;
            if (type.StartsWith("Enumerations::"))
                return ConstantModeType.Enumerations;
            if (type.StartsWith("DataTypes::"))
                return ConstantModeType.DataTypes;
            return ConstantModeType.BasicTypes;
        }
        public static string GetConstantTypeName(string type)
        {
            if (type == null)
                return null;
            if (type.StartsWith("Resources::"))
                return type.Substring(11);
            if (type.StartsWith("Enumerations::"))
                return type.Substring(14);
            if (type.StartsWith("DataTypes::"))
                return type.Substring(11);
            if (type.StartsWith("BasicTypes::"))
                return type.Substring(12);
            return type;
        }
        // returns null if the format is not valid
        public static string GetEnumerationType(string type)
        {
            if (type == null)
                return null;
            if (type.StartsWith("Enumerations::"))
                return type.Substring(14);
            return null;
        }
        // returns null if the format is not valid
        public static string GetDataTypesType(string type)
        {
            if (type == null)
                return null;
            if (type.StartsWith("DataTypes::"))
                return type.Substring(11);
            return null;
        }
        // returneaza BasicTypesConstantType.None in caz de string invalid
        public static BasicTypesConstantType GetBasicTypesType(string type)
        {
            if (type == null)
                return BasicTypesConstantType.None;
            if (type.StartsWith("BasicTypes::"))
                type = type.Substring(12);
            BasicTypesConstantType res = BasicTypesConstantType.None;
            if (Enum.TryParse<BasicTypesConstantType>(type, out res))
                return res;
            return BasicTypesConstantType.None;
        }
        // returneaza ResourcesConstantType.None in caz de string invalid
        public static ResourcesConstantType GetResourcesType(string type)
        {
            if (type == null)
                return ResourcesConstantType.None;
            if (type.StartsWith("Resources::") == false)
                return ResourcesConstantType.None;
            ResourcesConstantType res = ResourcesConstantType.None;
            if (Enum.TryParse<ResourcesConstantType>(type.Substring(11), out res))
                return res;
            return ResourcesConstantType.None;
        }
        public static bool CanConvertBasicTypeTo(BasicTypesConstantType fromType, BasicTypesConstantType toType)
        {
            if (fromType == toType)
                return true;
            if ((fromType == BasicTypesConstantType.Int8) && ((toType == BasicTypesConstantType.Int16) || (toType == BasicTypesConstantType.Int32) || (toType == BasicTypesConstantType.Int64) || (toType == BasicTypesConstantType.Float32) || (toType == BasicTypesConstantType.Float64)))
                return true;
            if ((fromType == BasicTypesConstantType.Int16) && ((toType == BasicTypesConstantType.Int32) || (toType == BasicTypesConstantType.Int64) || (toType == BasicTypesConstantType.Float32) || (toType == BasicTypesConstantType.Float64)))
                return true;
            if ((fromType == BasicTypesConstantType.Int32) && ((toType == BasicTypesConstantType.Int64)))
                return true;
            if ((fromType == BasicTypesConstantType.UInt8) && ((toType == BasicTypesConstantType.Int16) || (toType == BasicTypesConstantType.Int32) || (toType == BasicTypesConstantType.Int64) || (toType == BasicTypesConstantType.UInt16) || (toType == BasicTypesConstantType.UInt32) || (toType == BasicTypesConstantType.UInt64) || (toType == BasicTypesConstantType.Float32) || (toType == BasicTypesConstantType.Float64)))
                return true;
            if ((fromType == BasicTypesConstantType.UInt16) && ((toType == BasicTypesConstantType.Int32) || (toType == BasicTypesConstantType.Int64) || (toType == BasicTypesConstantType.UInt32) || (toType == BasicTypesConstantType.UInt64) || (toType == BasicTypesConstantType.Float32) || (toType == BasicTypesConstantType.Float64)))
                return true;
            if ((fromType == BasicTypesConstantType.UInt32) && ((toType == BasicTypesConstantType.Int64) || (toType == BasicTypesConstantType.UInt64)))
                return true;
            if ((fromType == BasicTypesConstantType.Float32) && ((toType == BasicTypesConstantType.Float64)))
                return true;
            return false;
        }
        public static bool IsUnsignedNumber(BasicTypesConstantType type)
        {
            return (type == BasicTypesConstantType.UInt8) || (type == BasicTypesConstantType.UInt16) || (type == BasicTypesConstantType.UInt32) || (type == BasicTypesConstantType.UInt64);
        }
        public static bool IsSignedNumber(BasicTypesConstantType type)
        {
            return (type == BasicTypesConstantType.Int8) || (type == BasicTypesConstantType.Int16) || (type == BasicTypesConstantType.Int32) || (type == BasicTypesConstantType.Int64);
        }
        public static bool IsInteger(BasicTypesConstantType type)
        {
            return IsUnsignedNumber(type) || IsSignedNumber(type);
        }
        public static bool IsFloat(BasicTypesConstantType type)
        {
            return (type == BasicTypesConstantType.Float32) || (type == BasicTypesConstantType.Float64);
        }
        public static bool IsNumber(BasicTypesConstantType type)
        {
            return IsInteger(type) || IsFloat(type);
        }
        // returneaza null daca valoarea nu este corecta, sau un string cu valoarea formatata ok
        public static string ValidateBasicTypeValue(string value, BasicTypesConstantType type)
        {
            Int16 i16 = 0;
            Int32 i32 = 0;
            Int64 i64 = 0;
            UInt16 u16 = 0;
            UInt32 u32 = 0;
            UInt64 u64 = 0;
            byte u8 = 0;
            sbyte i8 = 0;
            float f32 = 0;
            double f64 = 0;

            switch (type)
            {
                case BasicTypesConstantType.Int8:
                    if (sbyte.TryParse(value, out i8) == false)
                        return null;
                    return i8.ToString();
                case BasicTypesConstantType.Int16:
                    if (Int16.TryParse(value, out i16) == false)
                        return null;
                    return i16.ToString();
                case BasicTypesConstantType.Int32:
                    if (Int32.TryParse(value, out i32) == false)
                        return null;
                    return i32.ToString();
                case BasicTypesConstantType.Int64:
                    if (Int64.TryParse(value, out i64) == false)
                        return null;
                    return i64.ToString();
                case BasicTypesConstantType.UInt8:
                    if (byte.TryParse(value, out u8) == false)
                        return null;
                    return u8.ToString();
                case BasicTypesConstantType.UInt16:
                    if (UInt16.TryParse(value, out u16) == false)
                        return null;
                    return u16.ToString();
                case BasicTypesConstantType.UInt32:
                    if (UInt32.TryParse(value, out u32) == false)
                        return null;
                    return u32.ToString();
                case BasicTypesConstantType.UInt64:
                    if (UInt64.TryParse(value, out u64) == false)
                        return null;
                    return u64.ToString();
                case BasicTypesConstantType.Float32:
                    if (float.TryParse(value, out f32) == false)
                        return null;
                    return f32.ToString();
                case BasicTypesConstantType.Float64:
                    if (double.TryParse(value, out f64) == false)
                        return null;
                    return f64.ToString();
                case BasicTypesConstantType.String:
                    return value;
                case BasicTypesConstantType.Boolean:
                    if (value.ToLower().StartsWith("t"))
                        return "true";
                    if (value.ToLower().StartsWith("f"))
                        return "false";
                    return null;
                case BasicTypesConstantType.Color:
                    if (value.StartsWith("0x"))
                    {
                        if (UInt32.TryParse(value.Substring(2),NumberStyles.HexNumber,CultureInfo.CurrentCulture,out u32)==false)
                            return null;
                    } else {
                        if (UInt32.TryParse(value, out u32) == false)
                            return null;
                    }
                    return "0x" + u32.ToString("X8");
            }
            return null;
        }
        public static string ValidateBasicTypeArrayValues(string value, BasicTypesConstantType type, int MatrixColumnsCount,out string error)
        {
            List<string> values = null;
            error = "";
            string Result = "";
            if (type == BasicTypesConstantType.String)
                values = Project.StringListToList(value, ';', false);
            else
                values = Project.StringListToList(value, ',');
            if ((values.Count % MatrixColumnsCount) != 0)
            {
                error = "Invalid number of values (" + values.Count.ToString() + "). It should be a multiple of your columns: " + MatrixColumnsCount.ToString();
                return null;
            }
            if (values.Count == 0)
            {
                error = "An array must contains at least one value !";
                return null;
            }
            foreach (string v in values)
            {
                string res = ConstantHelper.ValidateBasicTypeValue(v, type);
                if (res == null)
                {
                    error = "Value '" + v + "' is not of type " + type.ToString();
                    return null;
                }
                if ((type == BasicTypesConstantType.String) && (res.Contains(";")))
                {
                    error = "String '" + v + "' contains a semicolumn (;) that is an invalid character for basic type string arrays !";
                    return null;
                }
                Result += res;
                if (type == BasicTypesConstantType.String)
                    Result += ";";
                else
                    Result += ',';
            }
            return Result.Substring(0, Result.Length - 1);
        }
        // returneaza o conversie de la resurse de un anumit tip la tipul resurse
        public static Type ConvertResourcesConstantTypeToResourceType(ResourcesConstantType resourceType)
        {
            Type t = null;
            switch (resourceType)
            {
                case ResourcesConstantType.Presentation: t = typeof(PresentationResource); break;
                case ResourcesConstantType.Font: t = typeof(FontResource); break;
                case ResourcesConstantType.Image: t = typeof(ImageResource); break;
                case ResourcesConstantType.Raw: t = typeof(RawResource); break;
                case ResourcesConstantType.Shader: t = typeof(ShaderResource); break;
                case ResourcesConstantType.Sound: t = typeof(SoundResource); break;
            }
            return t;
        }
        public static string GetBasicTypeCppReprezentation(BasicTypesConstantType bct)
        {
            switch (bct)
            {
                case BasicTypesConstantType.Int8:
                case BasicTypesConstantType.Int16:
                case BasicTypesConstantType.Int32:
                case BasicTypesConstantType.Int64:
                case BasicTypesConstantType.UInt8:
                case BasicTypesConstantType.UInt16:
                case BasicTypesConstantType.UInt32:
                case BasicTypesConstantType.UInt64:
                    return bct.ToString();
                case BasicTypesConstantType.Float32:
                case BasicTypesConstantType.Float64:
                    return bct.ToString();
                case BasicTypesConstantType.Boolean:
                    return "bool";
                case BasicTypesConstantType.String:
                    return "const char*";
                case BasicTypesConstantType.Color:
                    return "UInt32";
            }
            return "?";
        }
    }

    [XmlType("EnumValue"), XmlRoot("EnumValue")]
    public class EnumValue
    {
        [XmlAttribute()]
        public string Name = "", Value = "", Description = "";

        #region List View Props
        [XmlIgnore()]
        public string propName { get { return Name; } }
        [XmlIgnore()]
        public string propValue { get { return Value; } }
        [XmlIgnore()]
        public string propValueBitSet64 
        {
            get { return GetBinFormat(64); }
        }
        [XmlIgnore()]
        public string propValueBitSet32
        {
            get { return GetBinFormat(32); }
        }
        [XmlIgnore()]
        public string propValueBitSet16
        {
            get { return GetBinFormat(16); }
        }
        [XmlIgnore()]
        public string propValueBitSet8
        {
            get { return GetBinFormat(8); }
        }
        [XmlIgnore()]
        public string propValueHex
        {
            get
            {
                Int64 v = 0;
                if (Int64.TryParse(Value, out v))
                {
                    return String.Format("0x{0:X}", v);
                }
                return "?";
            }
        }
        [XmlIgnore()]
        public string propValueNormal
        {
            get { return Value; }
        }
        [XmlIgnore()]
        public bool propBool
        {
            get { return (Value != null) && ((Value[0] == 'T') || (Value[0] == 't')); }
        }
        [XmlIgnore()]
        public string propDescription { get { return Description; } }
        #endregion

        private string GetBinFormat(int nrBits)
        {
            Int64 v = 0;
            if (Int64.TryParse(Value, out v))
            {
                string s1 = Convert.ToString(v, 2).PadLeft(nrBits, '0').Replace('0','.').Replace("1","X");
                string res = "";
                for (int tr = 0; tr < nrBits/4; tr++)
                    res += s1.Substring(tr * 4, 4) + "  ";
                return res;
            }
            return "Failed to convert: '" + Value + "'";
        }        
    }



    [XmlType("Enumeration"), XmlRoot("Enumeration")]
    public class Enumeration : IComparable
    {
        [XmlAttribute()]
        public string Name = "", Description = "";
        [XmlAttribute()]
        public bool IsBitSet = false;
        [XmlAttribute()]
        public BasicTypesConstantType Type = BasicTypesConstantType.Int32;
        public List<EnumValue> Values = new List<EnumValue>();

        #region List View Props
        [XmlIgnore()]
        public string propName { get { return Name; } }
        [XmlIgnore()]
        public string propType { get { return Type.ToString(); } }
        [XmlIgnore()]
        public int propValues { get { return Values.Count; } }
        [XmlIgnore()]
        public string propDescription { get { return Description; } }
        [XmlIgnore()]
        public bool propIsBitSet { get { return IsBitSet; } }
        #endregion

        public int CompareTo(object obj)
        {
            Enumeration o = (Enumeration)obj;
            int r = Name.CompareTo(o.Name);
            return r;
        }
        public EnumValue FindValue(string name)
        {
            foreach (EnumValue ev in Values)
                if (ev.Name.Equals(name,StringComparison.InvariantCultureIgnoreCase))
                    return ev;
            return null;
        }

    }

    public enum ArrayType
    {
        None,
        List,
        Vector,
        Matrix
    }
    public enum FieldProperties
    {
        None,
        IsNull,
        IsConstant
    };

    [XmlType("DataTypeField"), XmlRoot("DataTypeField")]
    public class StructureField
    {
        [XmlAttribute()]
        public string Name = "", Type = "", Description = "", DefaultValue = "";
        [XmlAttribute()]
        public bool List = false, CanBeNull = false;
        [XmlAttribute()]
        public int ColumnWidth = 0, DisplayIndex = -1;

        #region List View Props
        [XmlIgnore()]
        public string propName { get { return Name; } }
        [XmlIgnore()]
        public string propDefaultValue { get { return DefaultValue; } }
        [XmlIgnore()]
        public string propType { get { return Type; } }
        [XmlIgnore()]
        public string propDescription { get { return Description; } }
        [XmlIgnore()]
        public bool propCanBeNull { get { return CanBeNull; } }
        [XmlIgnore()]
        public bool propIsList { get { return List; } }

        #endregion

        public string GetTypeName()
        {
            if (Type.Contains("::"))
                return Type.Substring(Type.IndexOf("::") + 2);
            return Type;
        }
    }
    [XmlType("StructureValue"), XmlRoot("StructureValue")]
    public class StructureValue : IComparable
    {
        [XmlAttribute()]
        public string Name = "";
        [XmlAttribute()]
        public int Array1 = -1, Array2 = -1,LinkID=-1;
        [XmlAttribute()]
        public bool IsNull = false;

        public List<string> FieldValues = new List<string>();

        public int CompareTo(object obj)
        {
            StructureValue o = (StructureValue)obj;
            int res = Name.CompareTo(o.Name);
            if (res != 0)
            {
                if ((res < 0) && (Name.Length == 0))
                    return 1;
                if ((res > 0) && (o.Name.Length == 0))
                    return -1;
                return res;
            }
            res = Array1.CompareTo(o.Array1);
            if (res != 0)
                return res;
            res = Array2.CompareTo(o.Array2);
            if (res != 0)
                return res;
            return LinkID.CompareTo(o.LinkID);
        }
    }

    [XmlType("Structure"), XmlRoot("Structure")]
    public class Structure : IComparable
    {
        [XmlAttribute()]
        public string Name = "", Description = "";
        [XmlAttribute()]
        public int LinkID = 0;
        [XmlAttribute()]
        public string Filter = "";

        public List<StructureField> Fields = new List<StructureField>();
        public List<StructureValue> Values = new List<StructureValue>();

        #region List View Props
        [XmlIgnore()]
        public string propName { get { return Name; } }
        [XmlIgnore()]
        public int propFields { get { return Fields.Count; } }
        [XmlIgnore()]
        public int propValues { get { return Values.Count; } }
        [XmlIgnore()]
        public string propDescription { get { return Description; } }
        #endregion

        public void AddField(StructureField field)
        {
            Fields.Add(field);
            SyncStructureValues();
        }
        public void RemoveField(StructureField field)
        {
            RemoveField(field.Name);
        }
        public void RemoveField(string name)
        {
            int index = GetFieldIndex(name);
            if (index < 0)
                return;
            foreach (StructureValue sv in Values)
            {
                if (index < sv.FieldValues.Count)
                    sv.FieldValues.RemoveAt(index);
            }
            Fields.RemoveAt(index);
        }
        public int CompareTo(object obj)
        {
            Structure o = (Structure)obj;
            int r = Name.CompareTo(o.Name);
            return r;
        }
        public int GetFieldIndex(string name)
        {
            for (int tr=0;tr<Fields.Count;tr++)
                if (Fields[tr].Name.Equals(name,StringComparison.InvariantCultureIgnoreCase))
                    return tr;
            return -1;
        }
        public StructureField FindField(string name)
        {
            foreach (StructureField field in Fields)
                if (field.Name.Equals(name,StringComparison.InvariantCultureIgnoreCase))
                    return field;
            return null;
        }
        private void SetDefaultValues(StructureValue sv)
        {
            sv.FieldValues.Clear();
            foreach (StructureField sf in Fields)
                sv.FieldValues.Add(sf.DefaultValue);
        }
        public int GetValueNameIndex(string name)
        {
            for (int tr=0;tr<Values.Count;tr++)
            {
                if (Values[tr].Name.Equals(name,StringComparison.InvariantCultureIgnoreCase))
                    return tr;            
            }
            return -1;
        }
        private void AddValue(string name,int array1,int array2,bool nullField)
        {
            StructureValue sv = new StructureValue();
            sv.Name = name;
            sv.LinkID = LinkID;
            sv.IsNull = nullField;
            sv.Array1 = array1;
            sv.Array2 = array2;
            LinkID++;
            SetDefaultValues(sv);
            Values.Add(sv);
        }
        public int AddValues(string name, int array1, int array2,bool nullFields)
        {
            int cLink = LinkID;
            if ((name.Length==0) || (name==null))
            {
                for (int tr=0;tr<array1;tr++)
                {
                    AddValue(name, -1, -1, nullFields);
                }
            }
            else
            {
                if (array1<=0)
                {
                    AddValue(name, -1, -1, nullFields);
                }
                if ((array1>0) && (array2<=0))
                {
                    for (int tr = 0; tr < array1; tr++)
                    {
                        AddValue(name, tr, -1, nullFields);
                    }
                }
                if ((array1 > 0) && (array2 > 0))
                {
                    for (int tr = 0; tr < array1; tr++)
                    {
                        for (int gr = 0; gr < array2; gr++)
                        {
                            AddValue(name, tr, gr, nullFields);
                        }
                    }
                }
            }
            Values.Sort();
            return cLink + 1;
        }
        public void ClearField(string name,string newValue)
        {
            int index = GetFieldIndex(name);
            if (index < 0)
                return;
            foreach (StructureValue sv in Values)
            {
                if (index < sv.FieldValues.Count)
                    sv.FieldValues[index] = newValue;
            }
        }
        public StructureValue FindStructureValue(string name,int array1,int array2)
        {
            foreach (StructureValue sv in Values)
            {
                if ((sv.Array1 != array1) && (array1 >= 0))
                    continue;
                if ((sv.Array2 != array2) && (array2 >= 0))
                    continue;
                if (sv.Name.Equals(name))
                    return sv;
            }
            return null;
        }
        public void ResizeValuesVariable(string name,int oldArray1,int oldArray2,int newArray1,int newArray2,bool NewFieldsAreNull)
        {
            char newCase = 'n';
            char oldCase = 'n';
            if ((oldArray1 > 0) && (oldArray2 > 0))
                oldCase = 'm';
            if ((oldArray1 > 0) && (oldArray2 <= 0))
                oldCase = 'v';
            if ((newArray1 > 0) && (newArray2 > 0))
                newCase = 'm';
            if ((newArray1 > 0) && (newArray2 <= 0))
                newCase = 'v';

            List<StructureValue> toDelete = new List<StructureValue>();

            // caz 1: (none -> vector, none-> array)
            if (oldCase == 'n')
            {
                StructureValue sv = FindStructureValue(name, -1, -1);
                if (sv!=null)
                {
                    sv.Array1 = 0;
                    if (newCase == 'm')
                    {
                        sv.Array2 = 0;
                        for (int tr = 0; tr < newArray1; tr++)
                            for (int gr = 0; gr < newArray2; gr++)
                                if ((tr == 0) && (gr == 0)) { } else { AddValue(name, tr, gr, NewFieldsAreNull); }
                    } else {
                        for (int tr = 1; tr < newArray1; tr++)
                            AddValue(name, tr, -1, NewFieldsAreNull);
                    }
                }
                Values.Sort();
                return;
            }
            // caz 2: vector->none sau matrix->none
            if (newCase == 'n')
            {
                // cel mai mic devine none - restul sunt sterse
                StructureValue found = null;
                if (oldCase=='m')
                    found = FindStructureValue(name,0,0);
                else
                    found = FindStructureValue(name,0,-1);

                foreach (StructureValue sv in Values)
                {
                    if (sv.Name.Equals(name) == false)
                        continue;
                    if (sv != found)
                        toDelete.Add(sv);
                }
                found.Array1 = found.Array2 = -1;
                foreach (StructureValue sv in toDelete)
                    Values.Remove(sv);
                if (found == null)
                    AddValue(name, -1, -1, NewFieldsAreNull);
                Values.Sort();
                return;
            }            
            Dictionary<string, StructureValue> d = new Dictionary<string, StructureValue>();
            // caz 3: vector -> vector
            if ((oldCase == 'v') && (newCase == 'v'))
            {
                foreach (StructureValue sv in Values)
                {
                    if (sv.Name.Equals(name) == false)
                        continue;
                    d[Project.GetVariableName(name, sv.Array1, -1)] = sv;
                }
                int maxIndex1 = Math.Max(newArray1, oldArray1);
                for (int tr = 0; tr < maxIndex1; tr++)
                {
                    string key = Project.GetVariableName(name, tr, -1);
                    bool exists = d.ContainsKey(key);
                    if (tr < newArray1)
                    {
                        if (!exists)
                            AddValue(name, tr, -1, NewFieldsAreNull);
                    }
                    else
                    {
                        if (exists)
                            Values.Remove(d[key]);
                    }
                }
                Values.Sort();
                return;
            }
            // caz 4: matrix -> matrix
            if ((oldCase == 'm') && (newCase == 'm'))
            {
                foreach (StructureValue sv in Values)
                {
                    if (sv.Name.Equals(name) == false)
                        continue;
                    d[Project.GetVariableName(name, sv.Array1, sv.Array2)] = sv;
                }
                int maxIndex1 = Math.Max(newArray1, oldArray1);
                int maxIndex2 = Math.Max(newArray2, oldArray2);
                for (int tr = 0; tr < maxIndex1; tr++)
                {
                    for (int gr = 0; gr < maxIndex2; gr++)
                    {
                        string key = Project.GetVariableName(name, tr, gr);
                        bool exists = d.ContainsKey(key);
                        if ((tr < newArray1) && (gr<newArray2))
                        {
                            if (!exists)
                                AddValue(name, tr, gr, NewFieldsAreNull);
                        }
                        else
                        {
                            if (exists)
                                Values.Remove(d[key]);
                        }
                    }
                }
                Values.Sort();
                return;
            }
            // caz 5: vector->matrix
            if ((oldCase == 'v') && (newCase == 'm'))
            {
                foreach (StructureValue sv in Values)
                {
                    if (sv.Name.Equals(name) == false)
                        continue;
                    sv.Array2 = 0;
                    d[Project.GetVariableName(name, sv.Array1, 0)] = sv;
                }
                int maxIndex1 = Math.Max(newArray1, oldArray1);
                int maxIndex2 = Math.Max(newArray2, oldArray2);
                for (int tr = 0; tr < maxIndex1; tr++)
                {
                    for (int gr = 0; gr < maxIndex2; gr++)
                    {
                        string key = Project.GetVariableName(name, tr, gr);
                        bool exists = d.ContainsKey(key);
                        if ((tr < newArray1) && (gr < newArray2))
                        {
                            if (!exists)
                                AddValue(name, tr, gr, NewFieldsAreNull);
                        }
                        else
                        {
                            if (exists)
                                Values.Remove(d[key]);
                        }
                    }
                }
                Values.Sort();
                return;
            }
            // caz 6: vector->matrix
            if ((oldCase == 'm') && (newCase == 'v'))
            {
                foreach (StructureValue sv in Values)
                {
                    if (sv.Name.Equals(name) == false)
                        continue;
                    d[Project.GetVariableName(name, sv.Array1, sv.Array2)] = sv;
                }
                int maxIndex1 = Math.Max(newArray1, oldArray1);
                int maxIndex2 = Math.Max(newArray2, oldArray2);
                for (int tr = 0; tr < maxIndex1; tr++)
                {
                    for (int gr = 0; gr < maxIndex2; gr++)
                    {
                        string key = Project.GetVariableName(name, tr, gr);
                        bool exists = d.ContainsKey(key);
                        if ((tr < newArray1) && (gr < 1))
                        {
                            if (!exists)
                                AddValue(name, tr, -1, NewFieldsAreNull);
                            else
                                d[key].Array2 = -1;
                        }
                        else
                        {
                            if (exists)
                                Values.Remove(d[key]);
                        }
                    }
                }
                Values.Sort();
                return;
            }          
        }
        public void SyncStructureValues()
        {
            int count = Fields.Count;
            foreach (StructureValue sv in Values)
            {
                if (sv.FieldValues.Count<count)
                {
                    for (int tr=sv.FieldValues.Count;tr<count;tr++)
                    {
                        sv.FieldValues.Add(Fields[tr].DefaultValue);
                    }
                }
            }
        }
    }

    [XmlType("ConstantValue"), XmlRoot("ConstantValue")]
    public class ConstantValue: IComparable
    {
        [XmlAttribute()]
        public string Name = "", Type = "", Description = "", Value = "";
        [XmlAttribute()]
        public int MatrixColumnsCount = 0;

        #region List View Props
        [XmlIgnore()]
        public string propName { get { return Name; } }
        [XmlIgnore()]
        public string propValue { get { return Value; } }
        [XmlIgnore()]
        public string propType { get { return ConstantHelper.GetConstantTypeName(Type); } }
        [XmlIgnore()]
        public ConstantModeType propMode { get { return ConstantHelper.GetConstantMode(Type); } }
        [XmlIgnore()]
        public string propDescription { get { return Description; } }
        #endregion

        public int CompareTo(object obj)
        {
            ConstantValue o = (ConstantValue)obj;
            int r = Name.CompareTo(o.Name);
            return r;
        }
    }

    public enum ColumnEditType
    {
        None,
        Boolean,
        NullField,
        Text,
        Enums,
        BitSets,
        Resources,
        DataType,
        List,
        VariableName,
        VariableArray,
        UnknownType,
    };
    public class ColumnInfo
    {
        public ConstantModeType Mode = ConstantModeType.BasicTypes;
        public BasicTypesConstantType BasicType = BasicTypesConstantType.None;
        public ResourcesConstantType ResType = ResourcesConstantType.None;
        public string Type = null;
        public Enumeration E = null;
        public Structure DataType = null;
        public StructureField Field = null;
        public string FullType = "";
        public ColumnEditType ColumnType = ColumnEditType.None;

        public ColumnInfo(StructureField _field, Project prj)
        {
            Field = _field;
            Init(Field.Type, prj);
        }
        public ColumnInfo(string FieldType, Project prj)
        {
            Init(FieldType, prj);
        }
        private void Init(string FieldType, Project prj)
        {
            FullType = FieldType;
            Mode = ConstantHelper.GetConstantMode(FieldType);
            switch (Mode)
            {
                case ConstantModeType.BasicTypes:
                    BasicType = ConstantHelper.GetBasicTypesType(FieldType);
                    if (BasicType != BasicTypesConstantType.None)
                        Type = BasicType.ToString();
                    break;
                case ConstantModeType.Enumerations:
                    Type = ConstantHelper.GetEnumerationType(FieldType);
                    E = prj.GetEnumeration(Type);
                    break;
                case ConstantModeType.DataTypes:
                    Type = ConstantHelper.GetDataTypesType(FieldType);
                    DataType = prj.GetStructure(Type);
                    break;
                case ConstantModeType.Resources:
                    ResType = ConstantHelper.GetResourcesType(FieldType);
                    if (ResType != ResourcesConstantType.None)
                        Type = ResType.ToString();
                    break;
            }
            if (Type.Length == 0)
                Type = null;
        }

    }

    

    #endregion

    #region Char Set
    public class CharacterSet
    {
        public enum CharacterType
        {
            None,
            UpperCase,
            LowerCase,
            Digit,
            Punctuation,
        };
        public enum AlignamentRelation
        {
            None,
            Top,
            Bottom,
            Center,
        };
        public class Info: IComparable
        {
            public List<Language> Languages = new List<Language>();
            public CharacterType Type = CharacterType.None;
            public AlignamentRelation AlignRelation = AlignamentRelation.None;
            public bool DefaultForEveryLanguage = false;
            public int CharCode = 0;
            public int AlignCharCode = 0;
            public int CompareTo(object o)
            {
                return CharCode.CompareTo(((Info)o).CharCode);
            }
        };
        private Dictionary<int, Info> CharSet = new Dictionary<int, Info>();
        private Dictionary<char, string> AsciiTranslate = new Dictionary<char, string>();
        private Info GetInfo(int charCode)
        {
            if (CharSet.ContainsKey(charCode) == false)
            {
                CharSet[charCode] = new Info();
                CharSet[charCode].CharCode = charCode;
            }
            return CharSet[charCode];
        }
        private void Set(int charCode,Language l)
        {
            GetInfo(charCode).Languages.Add(l);
        }
        private void Set(int charCode, Language []l)
        {
            GetInfo(charCode).Languages.AddRange(l);
        }
        private void Set(int charCode,CharacterType type)
        {
            GetInfo(charCode).Type = type;
        }
        private void Set(int charCode, bool defaultForEveryLanguage)
        {
            GetInfo(charCode).DefaultForEveryLanguage = defaultForEveryLanguage;
        }
        private void SetRange(int start,int end,Language []l)
        {
            while (start <= end)
            {
                Set(start, l);
                start++;
            }
        }
        private void SetRange(int start, int end, Language l)
        {
            while (start <= end)
            {
                Set(start, l);
                start++;
            }
        }
        private void SetRange(int start, int end, CharacterType type)
        {
            while (start <= end)
            {
                Set(start, type);
                start++;
            }
        }
        private void SetRange(int start, int end, bool defaultForEveryLanguage)
        {
            while (start <= end)
            {
                Set(start, defaultForEveryLanguage);
                start++;
            }
        }
        private void SetList(int[] list, Language[] l)
        {
            foreach (int i in list)
                Set(i, l);
        }
        private void SetList(int[] list, Language l)
        {
            foreach (int i in list)
                Set(i, l);
        }
        private void SetList(int[] list, CharacterType type)
        {
            foreach (int i in list)
                Set(i, type);
        }
        private void SetList(int[] list, bool defaultForEveryLanguage)
        {
            foreach (int i in list)
                Set(i, defaultForEveryLanguage);
        }
        private void SetText(string text, Language[] l)
        {
            foreach (char i in text)
                Set(i, l);
        }
        private void SetText(string text, Language l)
        {
            foreach (char i in text)
                Set(i, l);
        }
        private void SetText(string text, CharacterType type)
        {
            foreach (char i in text)
                Set(i, type);
        }
        private void SetText(string text, bool defaultForEveryLanguage)
        {
            foreach (char i in text)
                Set(i, defaultForEveryLanguage);
        }
        private void SetAlignamentRelation(int charCode,AlignamentRelation rel,int referenceCharacter)
        {
            GetInfo(charCode).AlignRelation = rel;
            GetInfo(charCode).AlignCharCode = referenceCharacter;
        }
        public CharacterSet()
        {
            SetRange('A', 'Z', CharacterType.UpperCase);
            SetRange('a', 'z', CharacterType.LowerCase);
            SetRange('0', '9', CharacterType.Digit);
            SetText("~!@#$%^&*()_+{}|:\"<>?[]\\;',./-=`", CharacterType.Punctuation);
            SetRange('A', 'Z', new Language[] { Language.English, Language.French, Language.Romanian, Language.Spanish, Language.German, Language.Portuguese, Language.Italian});
            SetRange('a', 'z', new Language[] { Language.English, Language.French, Language.Romanian, Language.Spanish, Language.German, Language.Portuguese, Language.Italian });
            SetRange('0', '9', new Language[] { Language.English, Language.French, Language.Romanian, Language.Spanish, Language.German, Language.Portuguese, Language.Italian });
            SetRange(33, 127, true);

            // German
            SetList(new int[] { 196, 201, 214, 220, 223 }, CharacterType.UpperCase);
            SetList(new int[] { 196, 201, 214, 220, 223 }, Language.German);
            SetList(new int[] { 228, 233, 246, 252 }, CharacterType.LowerCase);
            SetList(new int[] { 228, 233, 246, 252 }, Language.German);

            // French
            SetList(new int[] { 192, 194, 198, 199, 200, 201, 202, 203, 206, 207, 212, 338, 217, 219, 220, 376 }, CharacterType.UpperCase);
            SetList(new int[] { 192, 194, 198, 199, 200, 201, 202, 203, 206, 207, 212, 338, 217, 219, 220, 376 }, Language.French);
            SetList(new int[] { 224, 226, 230, 231, 232, 233, 234, 235, 238, 239, 244, 339, 249, 251, 252, 255 }, CharacterType.LowerCase);
            SetList(new int[] { 224, 226, 230, 231, 232, 233, 234, 235, 238, 239, 244, 339, 249, 251, 252, 255 }, Language.French);

            // Croatian
            SetList(new int[] { 262, 268, 272, 352, 381 }, CharacterType.UpperCase);
            SetList(new int[] { 262, 268, 272, 352, 381 }, Language.Croatian);
            SetList(new int[] { 263, 269, 273, 353, 382 }, CharacterType.LowerCase);
            SetList(new int[] { 263, 269, 273, 353, 382 }, Language.Croatian);

            // Italian
            SetList(new int[] { 192, 200, 201, 204, 210, 217 }, CharacterType.UpperCase);
            SetList(new int[] { 192, 200, 201, 204, 210, 217 }, Language.Italian);
            SetList(new int[] { 224, 232, 233, 236, 242, 249 }, CharacterType.LowerCase);
            SetList(new int[] { 224, 232, 233, 236, 242, 249 }, Language.Italian);

            // Spanish
            SetList(new int[] { 193, 201, 205, 209, 211, 218, 220, 191 }, CharacterType.UpperCase);
            SetList(new int[] { 193, 201, 205, 209, 211, 218, 220, 191 }, Language.Spanish);
            SetList(new int[] { 225, 233, 237, 241, 243, 250, 252, 161 }, CharacterType.LowerCase);
            SetList(new int[] { 225, 233, 237, 241, 243, 250, 252, 161 }, Language.Spanish);  

            // Portuguese
            SetList(new int[] { 192, 193, 194, 195, 201, 202, 205, 211, 212, 213, 218, 220, 199 }, CharacterType.UpperCase);
            SetList(new int[] { 192, 193, 194, 195, 201, 202, 205, 211, 212, 213, 218, 220, 199 }, Language.Portuguese);
            SetList(new int[] { 224, 225, 226, 227, 233, 234, 237, 243, 244, 245, 250, 252, 231 }, CharacterType.LowerCase);
            SetList(new int[] { 224, 225, 226, 227, 233, 234, 237, 243, 244, 245, 250, 252, 231 }, Language.Portuguese);
            SetList(new int[] { 170, 171, 186, 187 }, Language.Portuguese);

            // Greek
            SetRange(913, 929, CharacterType.UpperCase);
            SetRange(913, 929, Language.Greek);
            SetRange(931, 937, CharacterType.UpperCase);
            SetRange(931, 937, Language.Greek);
            SetRange(945, 961, CharacterType.LowerCase);
            SetRange(945, 961, Language.Greek);
            SetRange(963, 969, CharacterType.LowerCase);
            SetRange(963, 969, Language.Greek);

            // Polish
            SetList(new int[] { 260, 262, 280, 321, 323, 211, 346, 377, 379 }, CharacterType.UpperCase);
            SetList(new int[] { 260, 262, 280, 321, 323, 211, 346, 377, 379 }, Language.Polish);
            SetList(new int[] { 261, 263, 281, 322, 324, 242, 347, 378, 380 }, CharacterType.LowerCase);
            SetList(new int[] { 261, 263, 281, 322, 324, 242, 347, 378, 380 }, Language.Polish);

            // Turkish
            SetList(new int[] { 286, 304, 350 }, CharacterType.UpperCase);
            SetList(new int[] { 286, 304, 350 }, Language.Turkish);
            SetList(new int[] { 287, 305, 351 }, CharacterType.LowerCase);
            SetList(new int[] { 287, 305, 351 }, Language.Turkish);

            // Romanian
            SetList(new int[] { 258, 194, 206, 536, 350, 538, 354 }, CharacterType.UpperCase);
            SetList(new int[] { 258, 194, 206, 536, 350, 538, 354 }, Language.Romanian);
            SetList(new int[] { 259, 226, 238, 537, 351, 539, 355 }, CharacterType.LowerCase);
            SetList(new int[] { 259, 226, 238, 537, 351, 539, 355 }, Language.Romanian);

            // relatii
            SetAlignamentRelation('Ş', AlignamentRelation.Top, 'S');
            SetAlignamentRelation('Ș', AlignamentRelation.Top, 'S');
            SetAlignamentRelation('ş', AlignamentRelation.Top, 's');
            SetAlignamentRelation('ș', AlignamentRelation.Top, 's');
            SetAlignamentRelation('Ţ', AlignamentRelation.Top, 'T');
            SetAlignamentRelation('Ț', AlignamentRelation.Top, 'T');
            SetAlignamentRelation('ţ', AlignamentRelation.Top, 't');
            SetAlignamentRelation('ț', AlignamentRelation.Top, 't');
            SetAlignamentRelation('Ç', AlignamentRelation.Top, 'C');

            SetAlignamentRelation('g', AlignamentRelation.Top, 'a');
            SetAlignamentRelation('p', AlignamentRelation.Top, 'a');
            SetAlignamentRelation('q', AlignamentRelation.Top, 'a');
            SetAlignamentRelation('y', AlignamentRelation.Top, 'a');
            SetAlignamentRelation('Q', AlignamentRelation.Top, 'O');
            SetAlignamentRelation('j', AlignamentRelation.Top, 'i');
            SetAlignamentRelation('ç', AlignamentRelation.Top, 'c');

            SetAlignamentRelation('-', AlignamentRelation.Center, '0');
            SetAlignamentRelation('+', AlignamentRelation.Center, '0');
            SetAlignamentRelation('*', AlignamentRelation.Center, '0');


            // traduceri
            AsciiTranslate['Æ'] = "AE";
            AsciiTranslate['À'] = "A";
            AsciiTranslate['Á'] = "A";
            AsciiTranslate['Â'] = "A";
            AsciiTranslate['Ã'] = "A";
            AsciiTranslate['Ä'] = "A";
            AsciiTranslate['Ă'] = "A";
            AsciiTranslate['Ą'] = "A";
            AsciiTranslate['ß'] = "B";
            AsciiTranslate['Ç'] = "C";
            AsciiTranslate['Ć'] = "C";
            AsciiTranslate['Č'] = "C";
            AsciiTranslate['Đ'] = "D";
            AsciiTranslate['È'] = "E";
            AsciiTranslate['É'] = "E";
            AsciiTranslate['Ê'] = "E";
            AsciiTranslate['Ë'] = "E";
            AsciiTranslate['Ę'] = "E";
            AsciiTranslate['Ğ'] = "G";
            AsciiTranslate['Ì'] = "I";
            AsciiTranslate['Í'] = "I";
            AsciiTranslate['Î'] = "I";
            AsciiTranslate['Ï'] = "I";
            AsciiTranslate['Ñ'] = "N";
            AsciiTranslate['Ń'] = "N";
            AsciiTranslate['Ò'] = "O";
            AsciiTranslate['Ó'] = "O";
            AsciiTranslate['Ô'] = "O";
            AsciiTranslate['Õ'] = "O";
            AsciiTranslate['Ö'] = "O";
            AsciiTranslate['Ù'] = "U";
            AsciiTranslate['Ú'] = "U";
            AsciiTranslate['Û'] = "U";
            AsciiTranslate['Ü'] = "U";            
            AsciiTranslate['Š'] = "S";
            AsciiTranslate['Ş'] = "S";
            AsciiTranslate['Ș'] = "S";
            AsciiTranslate['Ś'] = "S";
            AsciiTranslate['Ţ'] = "T";
            AsciiTranslate['Ț'] = "T";
            AsciiTranslate['Ÿ'] = "Y";
            AsciiTranslate['Ź'] = "Z";
            AsciiTranslate['Ž'] = "Z";
            AsciiTranslate['Ż'] = "Z";           
            
            AsciiTranslate['à'] = "a";
            AsciiTranslate['á'] = "a";
            AsciiTranslate['â'] = "a";
            AsciiTranslate['ã'] = "a";
            AsciiTranslate['ä'] = "a";
            AsciiTranslate['ă'] = "a";
            AsciiTranslate['ą'] = "a";
            AsciiTranslate['ç'] = "c";
            AsciiTranslate['ć'] = "c";
            AsciiTranslate['č'] = "c";
            AsciiTranslate['đ'] = "d";
            AsciiTranslate['è'] = "e";
            AsciiTranslate['é'] = "e";
            AsciiTranslate['ê'] = "e";
            AsciiTranslate['ë'] = "e";
            AsciiTranslate['ę'] = "e";
            AsciiTranslate['ğ'] = "g";
            AsciiTranslate['ì'] = "i";
            AsciiTranslate['í'] = "i";
            AsciiTranslate['î'] = "i";
            AsciiTranslate['ï'] = "i";
            AsciiTranslate['ñ'] = "n";
            AsciiTranslate['ń'] = "n";
            AsciiTranslate['ò'] = "o";
            AsciiTranslate['ó'] = "o";
            AsciiTranslate['ô'] = "o";
            AsciiTranslate['õ'] = "o";
            AsciiTranslate['ö'] = "o";
            AsciiTranslate['ś'] = "s";
            AsciiTranslate['ş'] = "s";
            AsciiTranslate['š'] = "s";
            AsciiTranslate['ș'] = "s";
            AsciiTranslate['ț'] = "t";
            AsciiTranslate['ţ'] = "t";
            AsciiTranslate['ù'] = "u";
            AsciiTranslate['ú'] = "u";
            AsciiTranslate['û'] = "u";
            AsciiTranslate['ü'] = "u";
            AsciiTranslate['ÿ'] = "y";
            AsciiTranslate['ź'] = "z";
            AsciiTranslate['ż'] = "z";
            AsciiTranslate['ž'] = "z";           
        }
        public Info Get(int charCode)
        {
            if (CharSet.ContainsKey(charCode) == false)
                return null;
            return CharSet[charCode];
        }
        public List<Info> GetCharacterTable()
        {
            List<Info> l = new List<Info>(CharSet.Count);
            foreach (Info i in CharSet.Values)
                l.Add(i);
            l.Sort();
            return l;
        }
        public string GetNonDiacriticalChar(char ch)
        {
            if (AsciiTranslate.ContainsKey(ch))
                return AsciiTranslate[ch];
            return ""+ch;
        }
    }
    #endregion

    #region Obiecte de tip Resource

    [XmlInclude(typeof(ImageResource))]
    [XmlInclude(typeof(FontResource))]
    [XmlInclude(typeof(RawResource))]
    [XmlInclude(typeof(SoundResource))]
    [XmlInclude(typeof(ShaderResource))]
    [XmlInclude(typeof(PresentationResource))]


    [XmlType("Resource"), XmlRoot("Resource")]
    public class GenericResource: IComparable
    {
        [XmlAttribute()]
        public string Name = "";
        [XmlAttribute()]
        public string Builds = "";
        [XmlAttribute()]
        public int Array1 = -1, Array2 = -1;
        [XmlAttribute()]
        public string Source = "";
        [XmlAttribute()]
        public Language Lang = Language.English;

        [XmlIgnore()]
        public Project prj = null;

        #region Internal vars
        protected internal long ResourceFilePosition = -1;
        protected internal long ResourceFileSize = -1;
        protected internal string OutputName = null;
        protected internal int ResourceIndex = -1;
        private bool ResourceLoaded = false;
        #endregion

        #region Atribute
        [XmlIgnore(), Description("Variable Name"), Category("General"), DisplayName("Variable Name")]
        public virtual string _Name
        {
            get { return Name; }
            set {
                if (Project.ValidateVariableNameCorectness(value) == false)
                {
                    MessageBox.Show("Invalid variable name - it should contain only characters from the following set [a-z], [A-Z], [0-9] and '_'. It must start with a letter !");
                }
                else
                {
                    Name = value;
                }
            }
        }
        [XmlIgnore(), Description("Source File"), Category("General"), DisplayName("Source File")]
        public virtual string _Source
        {
            get { return Source; }
            set { Source = value; }
        }
        [XmlIgnore(), Description("Source File"), Category("General"), DisplayName("Type")]
        public virtual string _Type
        {
            get { return GetResourceType(); }
        }
        [XmlIgnore(), Description("Source File"), Category("General"), DisplayName("Array 1")]
        public virtual int _Array1
        {
            get { return Array1; }
            set { Array1 = value; if (Array1 < 0) Array1 = -1;  }
        }
        [XmlIgnore(), Description("Source File"), Category("General"), DisplayName("Array 2")]
        public virtual int _Array2
        {
            get { return Array2; }
            set { Array2 = value; if (Array2 < 0) Array2 = -1; }
        }
        [XmlIgnore(), Description("Specifies what builds will use this resource"), Category("Target"), DisplayName("Builds"), Editor(typeof(CheckBoxTypeEditor), typeof(UITypeEditor)), CheckBoxTypeEditor.Source(CheckBoxAttributeType.Builds)]
        public virtual string _Builds
        {
            get { return Builds; }
            set { Builds = value; }
        }
        [XmlIgnore(), Description("Specifies for what laguage this resource will be used"), Category("Target"), DisplayName("Language")]
        public virtual Language _Lang
        {
            get { return Lang; }
            set { Lang = value; }
        }
        #endregion

        #region List View Props
        [XmlIgnore(), Browsable(false)]
        public string propName { get { return GetResourceVariableName(); } }
        [XmlIgnore(), Browsable(false)]
        public GenericResource propMe { get { return this; } }
        [XmlIgnore(), Browsable(false)]
        public string propType { get { return GetResourceType(); } }
        [XmlIgnore(), Browsable(false)]
        public string propBuilds { get { return Builds; } }
        [XmlIgnore(), Browsable(false)]
        public string propSource{ get { return Source; } }
        [XmlIgnore(), Browsable(false)]
        public string propLanguage { get { return Lang.ToString(); } }
        [XmlIgnore(), Browsable(false)]
        public string propIcon { get { return GetIconImageListKey(); } }
        [XmlIgnore(), Browsable(false)]
        public string propDesignResolution { get { return GetDesignResolution(); } }
        [XmlIgnore(), Browsable(false)]
        public string propDescription { get { return GetResourceInformation(); } }
        #endregion

        #region General Functions

        public bool IsBaseResource()
        {
            if (prj == null)
                return false;
            if (prj.DefaultLanguage != Lang)
                return false;
            if (GetType()==typeof(ImageResource))
            {
                int w = 0, h = 0;
                if (Project.SizeToValues(((ImageResource)this).DesignResolution, ref w, ref h) == false)
                    return false;
                if ((w != prj.DesignResolutionSize.Width) || (h != prj.DesignResolutionSize.Height))
                    return false;
            }
            return true;
        }

        protected void EnableAttr(string name, bool value)
        {
            PropertyDescriptor descriptor = TypeDescriptor.GetProperties(this.GetType())[name];
            ReadOnlyAttribute attribute = (ReadOnlyAttribute)descriptor.Attributes[typeof(ReadOnlyAttribute)];
            FieldInfo fieldToChange = attribute.GetType().GetField("isReadOnly", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            fieldToChange.SetValue(attribute, !value);
        }

        public void OverrideDefaultOutputName(string newName)
        {
            OutputName = newName;
        }
        public string GetFromSource(string source, string key, string defaultValue)
        {
            string lsource = source.ToLower();
            key = "[" + key.ToLower() + "_";
            if (lsource.Contains(key) == false)
                return defaultValue;
            int index = lsource.IndexOf(key) + key.Length;
            source = source.Substring(index);
            if (source.Contains("]") == false)
                return defaultValue;
            return source.Substring(0, source.IndexOf("]"));
        }
        public int GetFromSource(string source, string key, int defaultValue)
        {
            string r = GetFromSource(source, key, "");
            if (r.Length == 0)
                return defaultValue;
            int val = 0;
            if (int.TryParse(r, out val) == false)
                return defaultValue;
            return val;
        }
        protected float GetFromSource(string source, string key, float defaultValue)
        {
            string r = GetFromSource(source, key, "");
            if (r.Length == 0)
                return defaultValue;
            int val = 0;
            if (int.TryParse(r, out val) == false)
                return defaultValue;
            return (float)((float)val / 100.0f);
        }


        protected void CreateNameFromSources(string source)
        {
            // iau numele din sursa - precum si indicele de la array
            Array1 = -1;
            Array2 = -1;
            Name = "";
            int value = 0;

            string temp = "";
            string psource = source.ToLower();
            psource = psource.Replace("_none_none_english_", "_");
            psource = psource.Replace("_none_english_", "_");
            foreach (char ch in psource)
            {

                if ((ch >= '0') && (ch <= '9'))
                    temp += ch;
                else if (((ch >= 'a') && (ch <= 'z')) || ((ch >= 'A') && (ch <= 'Z')))
                    temp += ch;
                else
                {
                    // adaug caracterul
                    temp = temp.ToLower();
                    if (temp.Length > 0)
                        temp = temp.Substring(0, 1).ToUpper() + temp.Substring(1);
                    if (int.TryParse(temp, out value))
                    {
                        if ((value >= 0) && (value < 64000))
                        {
                            if (Array1 < 0)
                                Array1 = value;
                            else if (Array2 < 0)
                                Array2 = value;
                            else
                                break;
                            temp = "";
                        }
                    }
                    else
                    {
                        if (Array1 >= 0) // dupa ce am gasit un numar nu mai iau alte texte in nume
                            break;
                        if ((temp.Length > 0) && (temp[0] >= '0') && (temp[0] <= '9'))
                            break; // nu e un numar valid
                        Name += temp;
                        temp = "";
                    }
                    if ((ch == '.') || (ch == '['))
                        break;
                    temp = "";
                }
            }

            Array1 = GetFromSource(source, "A", Array1);
            Array2 = GetFromSource(source, "A2", Array2);
        }
        public string GetSourceFullPath()
        {
            return prj.GetProjectResourceSourceFullPath(Source);
        }
        protected string ComputeOutputFileName(string extension)
        {
            if ((OutputName != null) && (OutputName.Length > 0))
                return prj.GetProjectResourceOutputFullPath(OutputName);
            if (extension.Length>0)
                return prj.GetProjectResourceOutputFullPath(GetResourceUniqueKey()+ "." + extension);            
            else
                return prj.GetProjectResourceOutputFullPath(GetResourceUniqueKey());            
        }
        public string GetSourceType()
        {
            if ((Source == null) || (Source.Contains(".") == false))
                return "";
            return Path.GetExtension(Source).ToLower().Substring(1);
        }

        public long GetResourceFilePosition()
        {
            return ResourceFilePosition;
        }
        public long GetResourceFileSize()
        {
            return ResourceFileSize;
        }
        public int GetResourceIndex()
        {
            return ResourceIndex;
        }
        
        public bool IsFilteredBy(string text)
        {
            if (text.Length == 0)
                return true;
            return (Name.IndexOf(text, StringComparison.InvariantCultureIgnoreCase) >= 0) || (Source.IndexOf(text, StringComparison.InvariantCultureIgnoreCase) >= 0);
        }

        public bool IsLoaded()
        {
            return ResourceLoaded;
        }
        public bool Load()
        {
            ResourceLoaded = LoadResource();
            return ResourceLoaded;
        }

        #endregion

        #region Resource name and keys
        public string GetResourceVariableName()
        {
            return Project.GetVariableName(Name, Array1, Array2);
        }
        public string GetResourceType()
        {
            return GetResourceType(GetType());
        }
        private static string GetResourceType(Type t)
        {
            if (t == typeof(ImageResource))
                return "Image";
            if (t == typeof(FontResource))
                return "Font";
            if (t == typeof(RawResource))
                return "Raw";
            if (t == typeof(SoundResource))
                return "Sound";
            if (t == typeof(ShaderResource))
                return "Shader";
            if (t == typeof(PresentationResource))
                return "Presentation";
            throw new Exception("Unknwown type ('" + t.ToString() + "') - Update your GenericResource::GetResourceType() Functione ");
        }
        public string GetResourceVariableKey()
        {
            return GetResourceVariableKey(GetType(), GetResourceVariableName());
        }
        public static string GetResourceVariableKey(Type t, string variableName)
        {
            return GetResourceType(t) + "_" + variableName;
        }
        public virtual string GetResourceUniqueKey()
        {
            return GetResourceType() + "_" + GetResourceVariableName() + "_" + Lang.ToString();
        }
        #endregion

        #region Icon
        public virtual Bitmap CreateIcon(int width, int height)
        {
            return null;
        }
        protected virtual bool HasCustomIcon()
        {
            return false;
        }
        public string GetIconImageListKey()
        {
            if (IsBaseResource()==false)
                return "__Derived__";
            else
            {
                if (HasCustomIcon())
                    return GetResourceVariableKey();
                else
                    return GetDefaultIconImageListKey();
            }                
        }
        private string GetDefaultIconImageListKey()
        {
            return "__" + GetResourceType() + "__";
        }
        #endregion

        #region Virtual Functions

        public virtual string GetOutputFileName()
        {
            return null;
        }
        public virtual string GetResourceInformation()
        {
            return "";
        }
        protected virtual bool LoadResource()
        {
            return false;
        }
        public virtual bool Build()
        {
            return false;
        }

        public virtual void GetPreviewData( PreviewData data)
        {
            data.Data = null;
            data.PreviewDataType = "";
        }
        public virtual byte[] GetContent( ref bool hasContent)
        {
            hasContent = true;
            return null;
        }
        public virtual bool IsDeriveFromOtherResources()
        {
            return false;
        }
        public virtual bool DuplicateFrom(GenericResource r)
        {
            Name = r.Name;
            Builds = r.Builds;
            Array1 = r.Array1;
            Array2 = r.Array2;
            Source = r.Source;
            Lang = r.Lang;
            return false; // implicit da false - true se da doar din clasele derivate
        }

        public virtual void RemoveBuild(string buildName)
        {
            Dictionary<string, string> d = Project.StringListToDict(Builds);
            if (d.ContainsKey(buildName))
                d.Remove(buildName);
            string ss = "";
            foreach (string k in d.Keys)
                ss += d[k] + ",";
            if (ss.EndsWith(","))
                ss = ss.Substring(0, ss.Length - 1);
            Builds = ss;
        }
        public virtual void AddBuild(string buildName)
        {
            Dictionary<string, string> d = Project.StringListToDict(Builds);
            if (d.ContainsKey(buildName.ToLower()))
                return;
            List<string> l = Project.StringListToList(Builds);
            l.Add(buildName);
            l.Sort();
            Builds = Project.ListToStringList(l);
        }
        public virtual void DuplicateBuildFrom(string existingBuild,string newBuild)
        {
            existingBuild = existingBuild.ToLower();
            Dictionary<string, string> d = Project.StringListToDict(Builds);
            if (d.ContainsKey(existingBuild)==false)
                return;
            AddBuild(newBuild);
        }

        public virtual string GetDesignResolution() { return ""; }
        #endregion

        #region Comparable
        public int CompareTo(object o)
        {
            if (o.GetType()!=GetType())
            {
                return GenericResource.GetResourceType(GetType()).CompareTo(GenericResource.GetResourceType(o.GetType()));
            }
            GenericResource r = (GenericResource)o;
            int res = Name.CompareTo(r.Name);
            if (res != 0)
                return res;
            res = Array1.CompareTo(r.Array1);
            if (res != 0)
                return res;
            res = Array2.CompareTo(r.Array2);
            if (res != 0)
                return res;
            bool me = IsBaseResource();
            bool ome = ((GenericResource)o).IsBaseResource();
            if ((me) && (!ome))
                return -1;
            if ((!me) && (ome))
                return 1;
            // daca sunt egale
            return 0;
        }
        #endregion
    };

    public enum VectorToRawGenerateMethod
    {
        UseDPI,
        UseWidth,
        UseHeight,
        UseWidthAndHeight
    };
    public enum ImageCreationResizeMethod: uint
    {
        Dynamic = 0x00000000,
        Bilinear = 0x01000000,
    };
    [XmlType("Image"), XmlRoot("Image")]
    public class ImageResource : GenericResource
    {
        [XmlAttribute()]
        public string DesignResolution;
        [XmlAttribute()]
        public float Scale = 1.0f;
        [XmlAttribute()]
        public float DPI = 90f;
        [XmlAttribute()]
        public int Width = 1, Height = 1;
        [XmlAttribute()]
        public VectorToRawGenerateMethod RawGenMethod = VectorToRawGenerateMethod.UseDPI;
        [XmlAttribute()]
        public ImageCreationResizeMethod ResizeMethod = ImageCreationResizeMethod.Bilinear;

        [XmlIgnore()]
        public Bitmap Picture;

        #region Atribute
        [XmlIgnore(), Description("Sets the resolution of the target device/platform where this image will be used"), Category("Target"), DisplayName("Resolution")]
        public string _DesignResolution
        {
            get { return DesignResolution; }
            set
            {
                int w = 0, h = 0;
                if (Project.SizeToValues(value, ref w, ref h))
                    DesignResolution = string.Format("{0} x {1}", w, h);
            }
        }
     

        [XmlIgnore(), Description("Specifies what paraameters to use when generating raw images from vector images"), Category("Raw image generation"), DisplayName("Method")]
        public VectorToRawGenerateMethod _RawGenMethod
        {
            get { return RawGenMethod; }
            set { 
                RawGenMethod = value;
                EnableAttr("_DPI", RawGenMethod == VectorToRawGenerateMethod.UseDPI);
                EnableAttr("_Width", (RawGenMethod == VectorToRawGenerateMethod.UseWidth) || (RawGenMethod == VectorToRawGenerateMethod.UseWidthAndHeight));
                EnableAttr("_Height", (RawGenMethod == VectorToRawGenerateMethod.UseHeight) || (RawGenMethod == VectorToRawGenerateMethod.UseWidthAndHeight));
            }
        }
        [XmlIgnore(), Description("Specifies the scale to be applied after the image generation is Completed."), Category("Raw image generation"), DisplayName("Scale")]
        public string _Scale
        {
            get { return Project.ProcentToString(Scale); }
            set { 
                float v = 0;
                if (Project.StringToProcent(value, ref v) == false)
                    MessageBox.Show("Invalid percentage value");
                if (v > 0)
                    Scale = v;
            }
        } 
        [XmlIgnore(), Description("DPI"), Category("Raw image generation"), DisplayName("DPI")]
        public float _DPI
        {
            get { return DPI; }
            set { if (value >= 0) DPI = value; }
        }

        [XmlIgnore(), Description("Sets the final width of the raw image"), Category("Raw image generation"), DisplayName("Desired Width")]
        public int _Width
        {
            get { return Width; }
            set { if (value >= 1) Width = value; }
        }

        [XmlIgnore(), Description("Sets the final height of the raw image"), Category("Raw image generation"), DisplayName("Desired Height")]
        public int _Height
        {
            get { return Height; }
            set { if (value >= 1) Height = value; }
        }

        [XmlIgnore(), Description("Specifies how the image will be created when it is loaded into the application.\nDynamic means that the image will be resize dynamically."), Category("Raw image generation"), DisplayName("Resize Method")]
        public ImageCreationResizeMethod _ResizeMethod
        {
            get { return ResizeMethod; }
            set { ResizeMethod = value; }
        }
        [XmlIgnore(), Description("Scale"), Category("Information"), DisplayName("Image size")]
        public string _ImageSize
        {
            get
            {
                if (Picture != null)
                    return string.Format("{0} x {1}", Picture.Width, Picture.Height);
                return "?";
            }
        }
        #endregion

        public bool Create(string source, float scale, string designResolution)
        {
            DesignResolution = designResolution;
            Scale = scale;
            Source = source;
            CreateNameFromSources(source);
            DPI = GetFromSource(source, "DPI", DPI);
            Width = GetFromSource(source, "Width", Width);
            Height = GetFromSource(source, "Height", Height);
            if (DPI < 1)
                DPI = -1;
            return true;
        }
        private bool BuildRasterImage()
        {
            if ((Scale == 1) && (RawGenMethod== VectorToRawGenerateMethod.UseDPI))
            {
                if (Disk.Copy(GetSourceFullPath(), GetOutputFileName(), prj.EC) == false)
                {
                    prj.EC.AddError("RasterImage::OnBuild", "Unable to copy image for '" + GetResourceVariableName() + "' to " + GetOutputFileName());
                    return false;
                }
                return true;
            }
            try
            {
                prj.EC.AddError("RasterImage::OnBuild", "Raster images can only be copied - not transform. This options has not been implemented yet !");
                return false;
                //Bitmap img = (Bitmap)Project.LoadImage(GetSourceFullPath(),prj.EC);
                //if (img == null)
                //{
                //    prj.EC.AddError("Unable to load image for '" + GetResourceVariableName() + "' !");
                //    return false;
                //}
                //int w = (int)(img.Width * Scale);
                //int h = (int)(img.Height * Scale);

                //if (Width > 0)
                //    w = (int)(Width * Scale);
                //if (Height > 0)
                //    h = (int)(Height * Scale);

                //Bitmap res = Project.ImageToIcon(prj.EC, img, w, h);
                //if (res == null)
                //    return false;
                //res.Save(GetOutputFileName());
                //return true;
            }
            catch (Exception e)
            {
                prj.EC.AddException("RasterImage::OnBuild", "Unable to create image for '" + GetResourceVariableName() + "'", e);
                return false;
            }
        }
        private bool BuildSVGImage()
        {
            string outputFile = GetOutputFileName();
            try
            {
                if (File.Exists(outputFile))
                    File.Delete(outputFile);
            }
            catch (Exception e)
            {
                prj.EC.AddException("Unable to delete PNG file: " + outputFile, e);
                return false;
            }
            return prj.SVGtoPNG(GetSourceFullPath(), outputFile, DPI, Width, Height, Scale, true);
        }
        public override bool Build()
        {
            if (GetSourceType() == "svg")
                return BuildSVGImage();
            return BuildRasterImage();
        }
        protected override bool LoadResource()
        {
            Bitmap bPicture = (Bitmap)Project.LoadImage(GetOutputFileName());
            if (bPicture!=null)
            {
                Picture = bPicture;
                return true;
            }
            prj.EC.AddError("RasterImage::OnLoad", "Unable to load image for '" + GetResourceVariableName() + "'");
            return false;
        }
        public override string GetResourceInformation()
        {
            //if (Picture == null)
            //    return String.Format("<not loaded> [Target:{0},Language:{1}]", DesignResolution, Lang);
            //return String.Format("{0} x {1} [Target:{2},{3}]", Picture.Width, Picture.Height, DesignResolution,Lang);
            if (Picture == null)
                return "<not loaded>";
            return String.Format("{0} x {1}", Picture.Width, Picture.Height);
        }
        public override Bitmap CreateIcon( int width, int height)
        {
            if (Picture != null)
                return Project.ImageToIcon(prj.EC, Picture, width, height);
            return null;
        }
        protected override bool HasCustomIcon()
        {
            return true;
        }
        public override void GetPreviewData( PreviewData data)
        {
            data.Data = Picture;
            data.PreviewDataType = "Image";
        }
        public override byte[] GetContent( ref bool hasContent)
        {
            hasContent = true;
            if (Load() == false)
            {
                prj.EC.AddError("RasterImageResource::GetContent()", "Error loading raster image !");
                return null;
            }
            byte[] b = Project.ImageToBuffer(Picture);
            if (b == null)
            {
                prj.EC.AddError("RasterImageResource::GetContent()", "Unable to allocate memory for image rasterization");
                return null;
            }
            return b;
        }

        public override string GetOutputFileName()
        {
            return ComputeOutputFileName("png");
        }
        public override string GetResourceUniqueKey()
        {
            return base.GetResourceUniqueKey() + "_" + DesignResolution.Replace(" ", "");
        }
        public override bool DuplicateFrom(GenericResource r)
        {
            base.DuplicateFrom(r);
            if (r.GetType() != typeof(ImageResource))
                return false;
            ImageResource rr = (ImageResource)r;
            DesignResolution = rr.DesignResolution;
            Scale = rr.Scale;
            DPI = rr.DPI;
            Width = rr.Width;
            Height = rr.Height;
            RawGenMethod = rr.RawGenMethod;
            return true;
        }

        public override string GetDesignResolution()
        {
            return DesignResolution.Replace(" ","").Replace("x"," x ");
        }
    }


    [XmlType("Glyph"), XmlRoot("Glyph")]
    public class Glyph
    {
        public class ImageInfo
        {
            public Image Picture = null;
            public long ResourceFilePoz = -1;
            public int ResourceSize = -1;
        };
        public class GlyphVersionInfo
        {
            public Dictionary<string, List<string>> Rezolutions = new Dictionary<string, List<string>>();
            public Dictionary<string, List<string>> Builds = new Dictionary<string, List<string>>();
            public Dictionary<string, string> BuildNames = new Dictionary<string, string>();
        };
        [XmlAttribute()]
        public int Code = 0;
        [XmlAttribute()]
        public float DPI = 90;
        [XmlAttribute()]
        public float Scale = 1;
        [XmlAttribute()]
        public float BaseLine = 0;
        [XmlAttribute()]
        public string Versions = "";
        [XmlAttribute()]
        public string Template = "";
        
        [XmlIgnore()]
        public Image Picture = null;
        [XmlIgnore()]
        public Dictionary<string, ImageInfo> PicturesForResolutions = new Dictionary<string,ImageInfo>();

        #region Atribute
        [XmlIgnore(), Description("DPI"), Category("Image"), DisplayName("DPI")]
        public float _DPI
        {
            get { return DPI; }
            set { DPI = value; }
        }
        [XmlIgnore(), Description("Scale for image"), Category("Image"), DisplayName("Scale")]
        public string _Scale
        {
            get { return Project.ProcentToString(Scale); }
            set
            {
                float v = 0;
                if (Project.StringToProcent(value, ref v) == false)
                    MessageBox.Show("Invalid percentage value");
                if (v > 0)
                    Scale = v;
            }
        }
        [XmlIgnore(), Description("Image size"), Category("Image"), DisplayName("Image")]
        public string _ImageSize
        {
            get
            {
                if (Picture != null)
                    return String.Format("{0} x {1}", Picture.Width, Picture.Height);
                else
                    return "Not loaded !";
            }

        }
        [XmlIgnore(), Description("Character code"), Category("General"), DisplayName("Caracter")]
        public string _CharCode
        {
            get { return ""+(char)Code+" Dec="+Code.ToString()+" Hex=0x"+Code.ToString("X4"); }
        }
        [XmlIgnore(), Description("Template used to create this glyph"), Category("General"), DisplayName("Template")]
        public string _Template
        {
            get { return Template; }
            set { Template = value; }
        }

        [XmlIgnore(), Description("Percentage of height for baseline"), Category("General"), DisplayName("BaseLine")]
        public string _Baseline
        {
            get { return Project.ProcentToString(BaseLine); }
            set
            {
                float v = 0;
                if (Project.StringToProcent(value, ref v) == false)
                    MessageBox.Show("Invalid percentage value");
                BaseLine = v;
            }
        }
        [XmlIgnore(), Description("Rezolutions and builds corelations for this glyph"), Category("Target"), DisplayName("Versions"), Editor(typeof(GlyphVersionsEditor), typeof(UITypeEditor))]
        public string _Versions
        {
            get { return Versions; }
            set { Versions = value; }
        }
        #endregion

        public string GetCharString()
        {
            if (Code < 32)
                return "Dec:" + Code.ToString() + " Hex:" + Code.ToString("X4");
            else
                return "Char:"+(char)Code+" Dec:" + Code.ToString() + " Hex:" + Code.ToString("X4");
        }
        public Glyph MakeACopy()
        {
            Glyph g = new Glyph();
            g.Code = this.Code;
            g.DPI = this.DPI;
            g.Scale = this.Scale;
            g.BaseLine = this.BaseLine;
            g.Versions = this.Versions;
            g.Picture = this.Picture;
            g.Template = this.Template;
            g.PicturesForResolutions.Clear();
            foreach (string rez in this.PicturesForResolutions.Keys)
            {
                if (this.PicturesForResolutions[rez]==null)
                    g.PicturesForResolutions[rez] = null;
                else
                {
                    Glyph.ImageInfo gi = new ImageInfo();
                    gi.Picture =  this.PicturesForResolutions[rez].Picture;
                    gi.ResourceSize =  this.PicturesForResolutions[rez].ResourceSize;
                    gi.ResourceFilePoz =  this.PicturesForResolutions[rez].ResourceFilePoz;
                    g.PicturesForResolutions[rez] = gi;
                }
            }
            return g;
        }
        public GlyphVersionInfo GetVersion()
        {
            return GetVersion(Versions);
        }
        public void DuplicateBuildFrom(string existingBuild, string newBuild)
        {
            int start,end;

            if (Versions.Length > 0)
            {
                start = Versions.LastIndexOf('[', Versions.Length - 1);
                end = Versions.LastIndexOf(']', Versions.Length - 1);
                existingBuild = existingBuild.ToLower();
                Dictionary<string, string> d = new Dictionary<string, string>();
                List<string> l = new List<string>();

                while ((start >= 0) && (end >= 0))
                {
                    string ss = Versions.Substring(start+1, (end - (start+1)));
                    Project.StringListToDict(ss, d);
                    if (d.ContainsKey(existingBuild))
                    {
                        Project.StringListToList(ss, l);
                        l.Add(newBuild);
                        l.Sort();
                        ss = Project.ListToStringList(l);
                        Versions = Versions.Substring(0, start + 1) + ss + Versions.Substring(end);
                    }
                    end = Versions.LastIndexOf(']', start);
                    if (end >= 0)
                        start = Versions.LastIndexOf('[', end);
                    else
                        start = -1;
                }
            }
        }
        public static GlyphVersionInfo GetVersion(string Versions)
        {
            string tmp = "";
            GlyphVersionInfo gvi = new GlyphVersionInfo();
            // formatul e rezolutie[build],rezolutie[build],...
            bool onRezolutie = true;
            string lastRezolutie = "";
            Versions += ",";
            Dictionary<string, bool> Pairs = new Dictionary<string, bool>();

            foreach (char ch in Versions)
            {
                if (((ch >= 'A') && (ch <= 'Z')) || ((ch >= 'a') && (ch <= 'z')) || ((ch >= '0') && (ch <= '9')))
                {
                    tmp += ch;
                    continue;
                }
                if (ch==' ')
                {
                    if (onRezolutie == false)
                        tmp += ' ';
                    continue;
                }
                if (tmp.Length>0)
                {
                    if (onRezolutie)
                    {
                        int w = 0,h = 0;
                        if ((Project.SizeToValues(tmp, ref w, ref h)) && (w > 0) && (h > 0))
                        {

                            if (gvi.Rezolutions.ContainsKey(tmp) == false)
                                gvi.Rezolutions[tmp] = new List<string>();
                            lastRezolutie = tmp;
                        }
                        else
                        {
                            lastRezolutie = "";
                        }
                    }
                    else
                    {
                        if (lastRezolutie.Length > 0)
                        {
                            tmp = tmp.Trim();
                            string k = tmp.ToLower();
                            string kp = lastRezolutie + "|" + k;
                            if (Pairs.ContainsKey(kp) == false)
                            {
                                Pairs[kp] = true;
                                gvi.Rezolutions[lastRezolutie].Add(k);
                                if (gvi.Builds.ContainsKey(k) == false)
                                {
                                    gvi.Builds[k] = new List<string>();
                                    gvi.BuildNames[k] = tmp;
                                }
                                gvi.Builds[k].Add(lastRezolutie);
                            }
                        }
                    }
                    tmp = "";
                }
                if (ch=='[')
                {
                    onRezolutie = false;
                    continue;
                }
                if (ch==']')
                {
                    onRezolutie = true;
                    continue;
                }
            }
            return gvi;
        }
        private bool ShouldHavePositiveBaseline()
        {
            if ((Code == 'q') || (Code=='Q') || (Code == 'j') || (Code == 'y') || (Code == 'p') || (Code == 'g') || (Code == ','))
                return true;
            return false;
        }
        private bool ShouldHaveNegativeBaseline()
        {
            if ((Code == '-') || (Code == '*') || (Code == '~') || (Code == '`') || (Code == '"') || (Code == '+') || (Code == '<') || (Code == '>') || (Code == '=') || (Code == '\''))
                return true;
            return false;
        }
        public bool Validate(bool includeWarnings, FontResource fnt,Project prj)
        {
            // verific sa fie incarcata imaginea
            if (Picture == null)
            {
                prj.EC.AddError("Image was not loaded for character: " + GetCharString());
                return false;
            }
            if (DPI<1)
            {
                prj.EC.AddError("Invalid DPI value ("+DPI.ToString()+") for character: " + GetCharString());
                return false;
            }
            if (Scale <=0 )
            {
                prj.EC.AddError("Invalid Scale value (" + Project.ProcentToString(Scale) + ") for character: " + GetCharString());
                return false;
            }
            GlyphVersionInfo vi = GetVersion();
            if (vi.Rezolutions.Count==0)
            {
                prj.EC.AddError("No resolution set for character: " + GetCharString());
                return false;
            }
            if (vi.Builds.Count==0)
            {
                prj.EC.AddError("No builds selected for character: " + GetCharString());
                return false;
            }
            // verific ca toate imaginile din rezolutiile specificate sa existe
            foreach (string rez in vi.Rezolutions.Keys)
            {
                string p = fnt.GetGlyphOutputPath(Code, rez);
                if (p == null)
                {
                    prj.EC.AddError("Invalid resolution (" + rez + ") specified for character: " + GetCharString());
                    return false;
                }
                if (File.Exists(p)==false)
                {
                    prj.EC.AddError("Missing image for resolution ("+rez+") for character: " + GetCharString());
                    return false;
                }
            }
            if (includeWarnings)
            {
                if ((BaseLine<=0) && (ShouldHavePositiveBaseline()))
                {
                    prj.EC.AddWarning("Character "+GetCharString()+" usually has a positive base line (current baseline is "+Project.ProcentToString(BaseLine)+") !");
                }
                if ((BaseLine >= 0) && (ShouldHaveNegativeBaseline()))
                {
                    prj.EC.AddWarning("Character " + GetCharString() + " usually has a negative base line (current baseline is " + Project.ProcentToString(BaseLine) + ") !");
                }
            }
            return true;
        }
    }
    [XmlType("Font"), XmlRoot("Font")]
    public class FontResource : GenericResource
    {
        [XmlAttribute()]
        public int WidthCharacter = 0, HeightCharacter = 0;
        [XmlAttribute()]
        public float SpaceBetweenChars = 0.1f, SpaceWidth = 1f, Height = 1f;
        [XmlAttribute()]
        public string FontID = "";


        public List<Glyph> Glyphs = new List<Glyph>();

        #region Atribute virtuale
        [XmlIgnore(), Description("Specifies what builds will use this resource"), Category("Target"), DisplayName("Builds"), Editor(typeof(CheckBoxTypeEditor), typeof(UITypeEditor)), CheckBoxTypeEditor.Source(CheckBoxAttributeType.Builds)]
        public override string _Builds
        {
            get { return Builds; }
            set { MessageBox.Show("Build will be automatically computed from the glyphs"); }
        }
        [XmlIgnore(), Description("Specifies for what laguage this resource will be used"), Category("Target"), DisplayName("Language")]
        public override Language _Lang
        {
            get { return prj.DefaultLanguage; }
            set { MessageBox.Show("A font resource is available for every language."); }
        }
        #endregion

        #region Atribute
        [XmlIgnore(), Description("Character to be used as reference for Space character"), Category("Font"), DisplayName("Width Character Reference")]
        public string _FontWidthCar
        {
            get {
                if (WidthCharacter <= 32)
                    return "None";
                return "" + (char)WidthCharacter + " Dec=" + WidthCharacter.ToString() + " Hex=0x" + WidthCharacter.ToString("X4"); 
            }
        }
        [XmlIgnore(), Description("Character to be used as reference for font Height"), Category("Font"), DisplayName("Height Character Reference")]
        public string _HeightCharacter
        {
            get
            {
                if (HeightCharacter <= 32)
                    return "None";
                return "" + (char)HeightCharacter + " Dec=" + HeightCharacter.ToString() + " Hex=0x" + HeightCharacter.ToString("X4");
            }
        }
        [XmlIgnore(), Description("Internal ID for this font object"), Category("General"), DisplayName("Font ID")]
        public string _FontID
        {
            get { return FontID; }
        }
        [XmlIgnore(), Description("Space betweeen characters"), Category("Font"), DisplayName("Space betweeen characters")]
        public string _SpaceBetweenChars
        {
            get { return Project.ProcentToString(SpaceBetweenChars); }
            set
            {
                float v = 0;
                if (Project.StringToProcent(value, ref v) == false)
                    MessageBox.Show("Invalid percentage value");
                if (v > 0)
                    SpaceBetweenChars = v;
            }
        }
        [XmlIgnore(), Description("Space width"), Category("Font"), DisplayName("Space width")]
        public string _SpaceWidth
        {
            get { return Project.ProcentToString(SpaceWidth); }
            set
            {
                float v = 0;
                if (Project.StringToProcent(value, ref v) == false)
                    MessageBox.Show("Invalid percentage value");
                if (v >= 0.05f)
                    SpaceWidth = v;
            }
        }
        [XmlIgnore(), Description("Font height (percentage of heght reference character)"), Category("Font"), DisplayName("Height")]
        public string _Height
        {
            get { return Project.ProcentToString(Height); }
            set
            {
                float v = 0;
                if (Project.StringToProcent(value, ref v) == false)
                    MessageBox.Show("Invalid percentage value");
                if (v >= 0.10f)
                    Height = v;
            }
        }
        #endregion


        private string GetFontGlyphsSourceFolder()
        {
            return Path.Combine(prj.GetProjectResourceSourceFolder(), GetResourceType() + "-" + FontID);
        }
        private string GetFontGlyphsOutputFolder()
        {
            return prj.GetProjectResourceOutputFullPath(GetResourceType() + "-" + FontID);
        }
        public bool Create(Project p)
        {
            prj = p;
            Random r = new Random(System.Environment.TickCount);
            do
            {
                FontID = System.Environment.TickCount.ToString("X8") + "-" + r.Next(65535).ToString("X4");
            } while ((Directory.Exists(GetFontGlyphsSourceFolder()) == true) || (Directory.Exists(GetFontGlyphsOutputFolder()) == true));

            Name = "NewFont" + System.Environment.TickCount.ToString("X8");
            return true;
        }
        public bool CheckFontFolders()
        {
            bool res = true;
            res &= Disk.CreateFolder(GetFontGlyphsSourceFolder(), prj.EC);
            res &= Disk.CreateFolder(GetFontGlyphsOutputFolder(), prj.EC);
            return res;
        }
        public override string GetOutputFileName()
        {
            return ComputeOutputFileName("");
        }
        public string GetGlyphSourcePath(int code)
        {
            return Path.Combine(GetFontGlyphsSourceFolder(), "Char_" + code.ToString() + ".svg");
        }
        public string GetGlyphOutputPath(int code,string rezolution)
        {
            int w = 0, h = 0;
            if (Project.SizeToValues(rezolution, ref w, ref h) == false)
                return null;
            if ((w < 1) || (h < 1))
                return null;
            return Path.Combine(GetFontGlyphsOutputFolder(), "Char_" + code.ToString() + "_" + w.ToString() + "x" + h.ToString() + ".png");
        }
        public string GetGlyphImageVariableName(Glyph g)
        {
            return GetGlyphImageVariableName(g.Code);
        }
        public string GetGlyphImageVariableName(int code)
        {
            return "Image_" + FontID.Replace("-", "") + "_" + code.ToString();
        }
        public override string GetResourceInformation()
        {
            return String.Format("{0} Glyphs", Glyphs.Count);
        }
        public int CodeToIndex(int code)
        {
            for (int tr = 0; tr < Glyphs.Count; tr++)
                if (code == Glyphs[tr].Code)
                    return tr;
            return -1;
        }
        public bool BuildGlyph(Glyph g,string rezolution)
        {
            Size sz = Project.SizeToValues(rezolution);
            if ((sz.Height<=0) || (sz.Width<=0))
            {
                prj.EC.AddError("Invalid resolution: " + rezolution);
                return false;
            }
            float newScale = g.Scale * Project.GetResolutionScale(prj.DesignResolutionSize.Width, prj.DesignResolutionSize.Height, sz.Width, sz.Height);
            return prj.SVGtoPNG(GetGlyphSourcePath(g.Code), GetGlyphOutputPath(g.Code, rezolution), g.DPI, -1, -1, newScale, true);
        }
        public bool LoadGlyph(Glyph g)
        {
            bool errors = false;
            Glyph.GlyphVersionInfo vi = g.GetVersion();
            g.PicturesForResolutions.Clear();
            g.Picture = null;
            foreach (string rez in vi.Rezolutions.Keys)
            {
                try
                {
                    Image i = Project.LoadImage(GetGlyphOutputPath(g.Code,rez));
                    if (i==null)
                    {
                        prj.EC.AddError("Unable to load image for glyph: " + g.Code.ToString());
                        errors = true;
                    }
                    Glyph.ImageInfo gi = new Glyph.ImageInfo();
                    gi.Picture = i;
                    Size sz = Project.SizeToValues(rez);
                    if ((sz.Width == prj.DesignResolutionSize.Width) && (sz.Height == prj.DesignResolutionSize.Height))
                        g.Picture = i;
                    g.PicturesForResolutions[rez] = gi;
                }
                catch (Exception e)
                {
                    g.PicturesForResolutions[rez] = null;
                    prj.EC.AddException("Unable to load image for glyph: " + g.Code.ToString()+" -> "+rez,e);
                    errors = true;
                }
                    
            }
            if (g.Picture==null)
            {
                prj.EC.AddError("Unable to find image for design resolutioln: " + prj.DesignResolution + " for character: " + g.GetCharString());
                errors = true;
            }
            return !errors;
        }
        public bool LoadGlyphs(List<Glyph> glyphs)
        {
            bool result = true;
            foreach (Glyph g in glyphs)
            {
                result &= LoadGlyph(g);
            }
            return result;
        }
        public bool LoadGlyphs(List<int> codes)
        {
            Dictionary<int,Glyph> d = GetCharSet();
            bool result = true;
            foreach (int code in codes)
            {
                if (d.ContainsKey(code))
                    result &= LoadGlyph(d[code]);
            }
            return result;
        }
        public bool AddGlyph(int code,string templateName)
        {
            if (code <= 32)
                return true;
            int index = CodeToIndex(code);
            if (index == -1)
            {
                Glyph g = new Glyph();
                g.Code = code;
                g.BaseLine = 0;
                g.Versions = prj.DesignResolution + "[" + prj.BuildConfigurations[0].Name + "]";
                g.Template = templateName;
                Glyphs.Add(g);
                return true;
            }
            return false;
        }
        public void SortGlyphs()
        {
            Glyphs.Sort(delegate(Glyph i1, Glyph i2) { return i1.Code.CompareTo(i2.Code); });
        }
        public override byte[] GetContent(ref bool hasContent)
        {
            hasContent = false;
            return null;
        }

        public override void GetPreviewData(PreviewData data)
        {
            data.Data = this;
            data.PreviewDataType = "Font";
        }
        
        protected override bool LoadResource()
        {
            bool result = true;
            foreach (Glyph g in Glyphs)
            {
                result &= LoadGlyph(g);
            }
            return result;
        }
        public override bool Build()
        {
            if (CheckFontFolders() == false)
                return false;
            bool result = true;
            foreach (Glyph g in Glyphs)
            {
                Glyph.GlyphVersionInfo vi = g.GetVersion();
                foreach (string rez in vi.Rezolutions.Keys)
                {
                    result &= BuildGlyph(g, rez);
                }
            }    
            return result;
        }
        public override bool IsDeriveFromOtherResources() { return true; }
        public override void RemoveBuild(string buildName)
        {
            foreach (Glyph g in Glyphs)
            {
                // sterge build-ul pentru fiecare glyph in parte
            }
            base.RemoveBuild(buildName);
        }

        public Dictionary<int, Glyph> GetCharSet()
        {
            Dictionary<int, Glyph> d = new Dictionary<int, Glyph>();
            foreach (Glyph g in Glyphs)
            {
                d[g.Code] = g;
            }
            return d;
        }
        public static bool CanRenderString(Dictionary<int, Glyph> glyphs, string s)
        {
            foreach (char ch in s)
            {
                if ((ch == ' ') || (ch == '\t') || (ch == '\n') || (ch == '\r'))
                    continue;
                if (glyphs.ContainsKey(ch) == false)
                    return false;
            }
            return true;
        }
        public bool CanRenderString(string s)
        {
            return CanRenderString(GetCharSet(), s);
        }
        public bool ValidateFont(bool includeWarnings)
        {
            if (WidthCharacter < 32)
                prj.EC.AddError("'Width Character Reference' property was not set correctly !");
            if (HeightCharacter < 32)
                prj.EC.AddError("'Height Character Reference' property was not set correctly !");
            Dictionary<int, Glyph> d = GetCharSet();
            if (d.ContainsKey(WidthCharacter) == false)
            {
                if (WidthCharacter<=32)
                    prj.EC.AddError(String.Format("'Width Character Reference' (Code:{0}) is not present in the current character set", WidthCharacter));
                else
                    prj.EC.AddError(String.Format("'Width Character Reference' ({0}, Code:{1}) is not present in the current character set", (char)WidthCharacter, WidthCharacter));
            }
            if (d.ContainsKey(HeightCharacter) == false)
            {
                if (HeightCharacter<=32)
                    prj.EC.AddError(String.Format("'Height Character Reference' (Code:{0}) is not present in the current character set", HeightCharacter));
                else
                    prj.EC.AddError(String.Format("'Height Character Reference' ({0}, Code:{1}) is not present in the current character set", (char)HeightCharacter, HeightCharacter));
            }
            foreach (Glyph g in Glyphs)
                g.Validate(includeWarnings,this, prj);

            return !(prj.EC.HasErrors());
        }
        public override bool DuplicateFrom(GenericResource r)
        {
            base.DuplicateFrom(r);
            if (r.GetType()==typeof(FontResource))
            {
                FontResource f = (FontResource)r;
                this.WidthCharacter = f.WidthCharacter;
                this.HeightCharacter = f.HeightCharacter;
                this.SpaceBetweenChars = f.SpaceBetweenChars;
                this.SpaceWidth = f.SpaceWidth;                
                this.FontID = f.FontID;
                this.prj = f.prj;
                this.Glyphs.Clear();
                foreach (Glyph g in f.Glyphs)
                {
                    this.Glyphs.Add(g.MakeACopy());
                }
                this.SortGlyphs();
                return true;
            }
            return false;
        }
        public override void DuplicateBuildFrom(string existingBuild, string newBuild)
        {
            existingBuild = existingBuild.ToLower();
            Dictionary<string, string> d = Project.StringListToDict(Builds);
            if (d.ContainsKey(existingBuild) == false)
                return;
            foreach (Glyph g in Glyphs)
                g.DuplicateBuildFrom(existingBuild, newBuild);
            AddBuild(newBuild);
        }
        public List<int> GetCharacterCodesAvailableForABuild(string buildName)
        {
            List<int> l = new List<int>();
            buildName = buildName.ToLower();
            foreach (Glyph g in Glyphs)
            {
                Glyph.GlyphVersionInfo vi = g.GetVersion();
                if (vi.Builds.ContainsKey(buildName) == false)
                    continue;
                l.Add(g.Code);
            }
            return l;
        }

        public void UpdateFontBuilds()
        {
            Dictionary<string, bool> d = new Dictionary<string, bool>();
            Dictionary<string, string> BuildsNames = new Dictionary<string, string>();
            foreach (Glyph g in Glyphs)
            {
                Glyph.GlyphVersionInfo vi = g.GetVersion();
                foreach (string bld in vi.Builds.Keys)
                    d[bld] = true;
            }
            foreach (GenericBuildConfiguration gb in prj.BuildConfigurations)
                BuildsNames[gb.Name.ToLower()] = gb.Name;
            string s = "";
            foreach (string k in d.Keys)
            {
                if (BuildsNames.ContainsKey(k))
                    s += BuildsNames[k] + " , ";
            }
            if (s.EndsWith(" , "))
                s = s.Substring(0, s.Length - 3);
            Builds = s;
        }
    }
 

    [XmlType("Raw"), XmlRoot("Raw")]
    public class RawResource : GenericResource
    {
        private byte[] data;
        private object PluginObject = null;
        private string PluginExtension = null;

        [XmlAttribute()]
        public bool LocalCopy = false;

        #region Atribute
        [XmlIgnore(), Description("If set creates a local copy of the data. If not set it uses framework buffers and the data is loaded everytime is needed."), Category("Data Management"), DisplayName("Store Data locally")]
        public bool _LocalCopy
        {
            get { return LocalCopy; }
            set { LocalCopy = value; }
        }
        [XmlIgnore(), Description("Source File"), Category("General"), DisplayName("Source File")]
        public override string _Source
        {
            get { return Source; }
            set { Source = value; UpdatePluginExtension(); }
        }
        #endregion

        private void UpdatePluginExtension()
        {
            PluginExtension = "";
            if (Source == null)
                return;
            int index = Source.LastIndexOf('.');            
            if (index >= 0)
                PluginExtension = Source.Substring(index + 1).ToLower();
        }
        public ResourcePluginData GetPlugin()
        {
            if ((prj==null) || (prj.Plugins==null))
                return null;
            if (PluginExtension == null)
                UpdatePluginExtension();
            ResourcePluginData result = null;
            if (prj.Plugins.Plugins.TryGetValue(PluginExtension, out result))
                return result;
            return null;
        }
        public object GetPluginObject()
        {
            return PluginObject;
        }
        public void Create(string source)
        {
            Source = source;
            CreateNameFromSources(source);
        }
        public override string GetResourceInformation()
        {
            ResourcePluginData rpd = GetPlugin();
            if (rpd == null)
            {
                return "Binary data from: " + Source;
            }
            if (PluginObject==null)
            {
                return "<Not loaded !>";
            }
            return rpd.Interface.GetResourceInformation(PluginObject);
        }
        public override string GetOutputFileName()
        {
            if (PluginExtension == null)
                UpdatePluginExtension();
            return ComputeOutputFileName(PluginExtension+".binary");
        }
        public override byte[] GetContent( ref bool hasContent)
        {
            hasContent = true;
            data = Disk.ReadFile(GetOutputFileName(), prj.EC);
            if (data == null)
                prj.EC.AddError("RawResource::GetContent", "Unable to load raw data for " + GetResourceVariableName());
            return data;
        }
        protected override bool LoadResource()
        {
            ResourcePluginData rpd = GetPlugin();
            PluginObject = null;
            if (rpd == null)
            {
                // nu am plugin - nici nu am ce sa incarc - asa ca dau true
                return true;
            }
            string error = "";
            PluginObject = rpd.Interface.Load(GetSourceFullPath(),out error);
            if (PluginObject==null)
            {
                prj.EC.AddError("Unable to load object for '" + GetResourceVariableName() + "' -> using plugin: '"+rpd.Interface.GetResourceTypeDescription()+"'\r\tError:"+error);
                return false;
            }
            return true;
        }
        public override bool Build()
        {
            ResourcePluginData rpd = GetPlugin();
            PluginObject = null;
            if (rpd == null)
            {
                // nu am plugin - copii fisierul sursa
                return Disk.Copy(GetSourceFullPath(), GetOutputFileName(), prj.EC);
            }
            if ((Load()==false) || (PluginObject==null))
            {
                prj.EC.AddError("Raw resource '"+GetResourceVariableName()+"' was not loaded !");
                return false;
            }
            string error = "";
            if (rpd.Interface.Build(PluginObject,GetOutputFileName(),out error)==false)
            {
                prj.EC.AddError("Unable to build resource for '" + GetResourceVariableName() + "' !\r\nError:"+error);
                return false;
            }
            return true;
        }
        public override void GetPreviewData(PreviewData data)
        {
            if (GetPlugin()==null)
            {
                data.Data = null;
                data.PreviewDataType = "";
                return;
            }
            if (PluginExtension == null)
                UpdatePluginExtension();
            data.Data = PluginObject;
            data.PreviewDataType = "_" + PluginExtension + "_";
        }
        protected override bool HasCustomIcon()
        {
            ResourcePluginData rpd = GetPlugin();
            return ((rpd != null) && (PluginObject != null));
        }
        public override Bitmap CreateIcon(int width, int height)
        {
            ResourcePluginData rpd = GetPlugin();
            if ((rpd == null) || (PluginObject==null))
                return null;
            string error = "";
            Image i = rpd.Interface.GetIcon(PluginObject, width, height,out error);
            if (i != null)
                return (Bitmap)i;
            return null;
        }
    }

    [XmlType("Sound"), XmlRoot("Sound")]
    public class SoundResource : GenericResource
    {
        [XmlAttribute()]
        public bool BackgroundMusic = false;

        #region Atribute
        [XmlIgnore(), Description("Background Music"), Category("Music"), DisplayName("Background Music")]
        public bool _BackgroundMusic
        {
            get { return BackgroundMusic; }
            set { BackgroundMusic = value; }
        }
        #endregion

        private byte[] data;
        public void Create(string source)
        {
            Source = source;
            CreateNameFromSources(source);
        }
        public override string GetResourceInformation()
        {
            return Source;
        }
        public override byte[] GetContent( ref bool hasContent)
        {
            hasContent = true;
            data = Disk.ReadFile(GetSourceFullPath(), prj.EC);
            if (data == null)
                prj.EC.AddError("SoundResource::GetContent", "Unable to load source file for " + GetResourceVariableName());
            return data;
        }
        public override void GetPreviewData(PreviewData data)
        {
            data.Data = GetSourceFullPath();
            data.PreviewDataType = "Sound";
        }
        protected override bool LoadResource()
        {
            return true;
        }
        public override bool Build()
        {
            return true;
        }
    }

    public enum ShaderVariableType
    {
        None,
        Color,
        Float,
        ColorChannel,
    };
    public class ShaderVariableConverter : ExpandableObjectConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context,System.Type destinationType)
        {
            if (destinationType == typeof(ShaderVariable))
                return true;
            return base.CanConvertTo(context, destinationType);
        }
        public override object ConvertTo(ITypeDescriptorContext context,CultureInfo culture,object value,System.Type destinationType)
        {
            if (destinationType == typeof(System.String) && value is ShaderVariable)
            {
                ShaderVariable so = (ShaderVariable)value;
                return so.Name + " of type " + so.Type.ToString();
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
        public override bool CanConvertFrom(ITypeDescriptorContext context,System.Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;

            return base.CanConvertFrom(context, sourceType);
        }
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return base.ConvertFrom(context, culture, value);
        }
    }

    [TypeConverterAttribute(typeof(ShaderVariableConverter)), XmlType("ShaderVariable"), XmlRoot("ShaderVariable")]
    public class ShaderVariable
    {
        [XmlAttribute()]
        public string Name = "";
        [XmlAttribute()]
        public string OpenGLType = "";
        [XmlAttribute()]
        public bool ClearAfterUsage = false;
        [XmlAttribute()]
        public ShaderVariableType Type = ShaderVariableType.None;
        
        #region Atribute
        [XmlIgnore(), Description("Name of the variable that will be used in GAC code"), DisplayName("Name")]
        public string _Name
        {
            get { return Name; }
        }
        [XmlIgnore(), Description("Type of the variable that will be used in GAC code"), DisplayName("Type")]
        public ShaderVariableType _Type
        {
            get { return Type; }
            set { Type = value; }
        }
        [XmlIgnore(), Description("Specifies wheather this variable holds its value on subsequent OpenGL calls or it is deleted after the first one."), DisplayName("Permanent")]
        public bool _Permanent
        {
            get { return !ClearAfterUsage; }
            set { ClearAfterUsage = !value; }
        }
        #endregion
    };

    [XmlType("Shader"), XmlRoot("Shader")]
    public class ShaderResource : GenericResource
    {
        [XmlAttribute()]
        public string VertexShader = "", FragmentShader = "";
        public List<ShaderVariable> Uniforms = new List<ShaderVariable>();

        private void AddString(List<byte> l, string text)
        {
            foreach (char ch in text)
                l.Add((byte)ch);
            l.Add(0);
        }
        private void AddSize(List<byte> l, int size)
        {
            l.Add((byte)(size % 256));
            l.Add((byte)(size / 256));
        }
        private string UTF8ToBase64(string utf8Text)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(utf8Text));
        }
        private string Base64ToUTF8(string base64Text)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(base64Text));
        }
        public void SetVertexShader(string text)
        {
            VertexShader = UTF8ToBase64(text);
        }
        public void SetFragmentShader(string text)
        {
            FragmentShader = UTF8ToBase64(text);
        }
        public string GetVertexShader()
        {
            return Base64ToUTF8(VertexShader);
        }
        public string GetFragmentShader()
        {
            return Base64ToUTF8(FragmentShader);
        }
        public void Create()
        {
            Name = "NewShader";
            SetVertexShader("attribute vec4 pos;\r\nvoid main()\r\n{\r\n\tgl_Position = pos;\r\n}");
            SetFragmentShader("void main()\r\n{\r\n\tgl_FragColor = vec4(1,1,1,1);\r\n}");
        }
        public override byte[] GetContent( ref bool hasContent)
        {
            hasContent = true;
            List<byte> buffer = new List<byte>();
            // ordinea datelor
            // marker 
            // dimensiunea
            AddString(buffer, "GSHD");
            string v = GetVertexShader();
            string f = GetFragmentShader();
            if (v.Length > 64000)
            {
                prj.EC.AddError("Vertex shader should be less than 64000 characters !");
                return null;
            }
            if (f.Length > 64000)
            {
                prj.EC.AddError("Fragment shader should be less than 64000 characters !");
                return null;
            }
            AddSize(buffer, v.Length);
            AddString(buffer, v);
            AddSize(buffer, f.Length);
            AddString(buffer, f);

            string tmp = CreateVariableListAsString(v, true);
            AddSize(buffer, tmp.Length);
            AddString(buffer, tmp);

            tmp = CreateVariableListAsString(f, false);
            AddSize(buffer, tmp.Length);
            AddString(buffer, tmp);

            return buffer.ToArray();
        }
        protected override bool LoadResource()
        {
            return true;
        }
        public override bool Build()
        {
            return true;
        }
        public static List<ShaderVariable> CreateVariableList(string shader, bool vertex)
        {
            if (shader.Contains("{"))
                shader = shader.Substring(0, shader.IndexOf('{'));
            string[] lines = shader.Replace("\r", "\n").Replace("\n\n", "\n").Split('\n');
            List<ShaderVariable> l = new List<ShaderVariable>();
            foreach (string line in lines)
            {
                string ln = line.Trim().Replace("\t", " ");
                while (ln.Contains("  "))
                    ln = ln.Replace("  ", " ");
                ln = ln.Replace(";", "").Trim();
                string[] word = ln.Split(' ');
                if ((vertex) && (ln.StartsWith("attribute ")) && (word.Length == 3))
                {
                    ShaderVariable sv = new ShaderVariable();
                    sv.Name = word[2];
                    sv.OpenGLType = word[1];
                    if (word[2] == "pos")
                        l.Insert(0, sv);
                    else
                        l.Add(sv);
                }
                if ((!vertex) && (ln.StartsWith("uniform ")) && (word.Length == 3))
                {
                    if (word[1] == "sampler2D")
                        continue; 
                    ShaderVariable sv = new ShaderVariable();
                    sv.Name = word[2];
                    sv.OpenGLType = word[1];
                    if (word[2] == "color")
                        l.Insert(0, sv);
                    else
                        l.Add(sv);
                }
            }
            return l;
        }
        public static string CreateVariableListAsString(string shader, bool vertex)
        {
            List<ShaderVariable> l = CreateVariableList(shader, vertex);
            string tmp = "";
            foreach (ShaderVariable sv in l)
                tmp += sv.Name + ",";
            if (tmp.EndsWith(","))
                tmp = tmp.Substring(0, tmp.Length - 1);
            return tmp;
        }
        public override void GetPreviewData(PreviewData data)
        {
            data.Data = this;
            data.PreviewDataType = "Shader";
        }
    }


    [XmlType("Presentation"), XmlRoot("Presentation")]
    public class PresentationResource : GenericResource
    {

        #region Atribute
        #endregion

        [XmlIgnore()]
        public Presentation anim = null;

        private byte[] data;
        public void Create(string source)
        {
            Source = source;
            CreateNameFromSources(source);
        }
        public override string GetResourceInformation()
        {
            if (anim != null)
            {
                return string.Format("{0} x {1}, {2} frames", anim.Width, anim.Height, anim.Frames.Count);
            }
            return "";
        }
        public override byte[] GetContent( ref bool hasContent)
        {
            hasContent = true;
            data = Disk.ReadFile(GetOutputFileName(), prj.EC);
            if (data == null)
                prj.EC.AddError("PresentationnResource::GetContent", "Unable to load source file for " + GetResourceVariableName());
            return data;
        }
        protected override bool LoadResource()
        {
            anim = Presentation.LoadFromXML(GetSourceFullPath(), prj.EC);
            if (anim != null)
                anim.SetProject(prj, prj.GetAppResources());
            return (anim != null);
        }
        public override bool Build()
        {
            string outputFile = GetOutputFileName();
            if (Load() == false)
                return false;

            try
            {
                if (File.Exists(outputFile))
                    File.Delete(outputFile);
            }
            catch (Exception e)
            {
                prj.EC.AddException("Unable to delete anim file: " + outputFile, e);
                return false;
            }
            return anim.ExportToBinaryFormat(outputFile, prj.EC);
        }
        public override bool IsDeriveFromOtherResources() { return true; }
        public override string GetOutputFileName()
        {
            return ComputeOutputFileName("anim");
        }
        public override string GetDesignResolution()
        {
            if (anim != null)
                return anim._Resolution;
            return "";
        }
    }
    #endregion

    #region Profile
    public enum ProfileType
    {
        Global,
        Scene,
        Local,
    };
    [XmlType("Profile"), XmlRoot("Profile")]
    public class Profile
    {
        [XmlAttribute()]
        public string Name = "";
        [XmlAttribute()]
        public int Array1 = -1, Array2 = -1;
        [XmlAttribute()]
        public ProfileType Type;

        public List<string> Images = new List<string>();
        public List<string> Shaders = new List<string>();
        public List<string> Sounds = new List<string>();
        public List<string> Raw = new List<string>();
        public List<string> Presentations = new List<string>();
        public List<string> Fonts = new List<string>();

        #region Atribute
        [XmlIgnore(), Description("Profile Name"), Category("General"), DisplayName("Profile Name")]
        public string _Name
        {
            get { return Name; }
            set { Name = value; }
        }

        [XmlIgnore(), Description("Profile Array Index"), Category("General"), DisplayName("Array 1")]
        public int _Array1
        {
            get { return Array1; }
            set { Array1 = value; }
        }
        [XmlIgnore(), Description("Profile Array Index"), Category("General"), DisplayName("Array 2")]
        public int _Array2
        {
            get { return Array2; }
            set { Array2 = value; }
        }
        [XmlIgnore(), Description("Profile Type"), Category("General"), DisplayName("Type")]
        public ProfileType _Type
        {
            get { return Type; }
            set { Type = value; }
        }
        #endregion

        public static bool IsAnAccepedResource(GenericResource res)
        {
            if ((res.GetType() == typeof(RawResource)) && (((RawResource)res).LocalCopy == false))
                return false;
            return ((res.GetType() == typeof(ImageResource)) ||
                    (res.GetType() == typeof(SoundResource)) ||
                    (res.GetType() == typeof(ShaderResource)) ||
                    (res.GetType() == typeof(RawResource)) ||
                    (res.GetType() == typeof(FontResource)) ||
                    (res.GetType() == typeof(PresentationResource)));
        }
        public string GetProperties()
        {
            string ss = "";
            if (Images.Count > 0)
                ss += "Images:" + Images.Count.ToString() + "  ";
            if (Sounds.Count > 0)
                ss += "Sounds:" + Sounds.Count.ToString() + "  ";
            if (Shaders.Count > 0)
                ss += "Shaders:" + Shaders.Count.ToString() + "  ";
            if (Presentations.Count > 0)
                ss += "Presentations:" + Presentations.Count.ToString() + "  ";
            if (Raw.Count > 0)
                ss += "Raw:" + Raw.Count.ToString() + "  ";
            return ss;
        }
        public Dictionary<string, string> GetAllResources()
        {
            Dictionary<string, string> d = new Dictionary<string, string>();
            foreach (string s in Images)
                d[GenericResource.GetResourceVariableKey(typeof(ImageResource), s)] = s;
            foreach (string s in Sounds)
                d[GenericResource.GetResourceVariableKey(typeof(SoundResource), s)] = s;
            foreach (string s in Shaders)
                d[GenericResource.GetResourceVariableKey(typeof(ShaderResource), s)] = s;
            foreach (string s in Presentations)
                d[GenericResource.GetResourceVariableKey(typeof(PresentationResource), s)] = s;
            foreach (string s in Raw)
                d[GenericResource.GetResourceVariableKey(typeof(RawResource), s)] = s;
            foreach (string s in Fonts)
                d[GenericResource.GetResourceVariableKey(typeof(FontResource), s)] = s;
            return d;
        }
        public void AddResource(GenericResource r)
        {
            if (IsAnAccepedResource(r) == false)
                return;
            if (r.GetType() == typeof(ImageResource))
                Images.Add(r.GetResourceVariableName());
            if (r.GetType() == typeof(SoundResource))
                Sounds.Add(r.GetResourceVariableName());
            if (r.GetType() == typeof(ShaderResource))
                Shaders.Add(r.GetResourceVariableName());
            if (r.GetType() == typeof(PresentationResource))
                Presentations.Add(r.GetResourceVariableName());
            if (r.GetType() == typeof(RawResource))
                Raw.Add(r.GetResourceVariableName());
            if (r.GetType() == typeof(FontResource))
                Fonts.Add(r.GetResourceVariableName());
        }
        public void RemoveResource(Type resourceType, string resourceVariableName)
        {
            if (resourceType == typeof(ImageResource))
                Images.Remove(resourceVariableName);
            if (resourceType == typeof(SoundResource))
                Sounds.Remove(resourceVariableName);
            if (resourceType == typeof(ShaderResource))
                Shaders.Remove(resourceVariableName);
            if (resourceType == typeof(PresentationResource))
                Presentations.Remove(resourceVariableName);
            if (resourceType == typeof(RawResource))
                Raw.Remove(resourceVariableName);
            if (resourceType == typeof(FontResource))
                Fonts.Remove(resourceVariableName);
        }
        public void RemoveMissedResource(string key,string resourceVariableName)
        {
            if (GenericResource.GetResourceVariableKey(typeof(ImageResource), resourceVariableName) == key)
                Images.Remove(resourceVariableName);
            if (GenericResource.GetResourceVariableKey(typeof(SoundResource), resourceVariableName) == key)
                Sounds.Remove(resourceVariableName);
            if (GenericResource.GetResourceVariableKey(typeof(ShaderResource), resourceVariableName) == key)
                Shaders.Remove(resourceVariableName);
            if (GenericResource.GetResourceVariableKey(typeof(PresentationResource), resourceVariableName) == key)
                Presentations.Remove(resourceVariableName);
            if (GenericResource.GetResourceVariableKey(typeof(RawResource), resourceVariableName) == key)
                Raw.Remove(resourceVariableName);
            if (GenericResource.GetResourceVariableKey(typeof(FontResource), resourceVariableName) == key)
                Fonts.Remove(resourceVariableName);
        }
        public string CreateInitializationCode(Project p, BuildResources R, string buildName)
        {
            string s = "";
            int index = 0;
            string vname = Project.GetVariableName(Name, Array1, Array2);
            // calculez cate imagini am in profil
            List<string> FontImages = new List<string>();
            Dictionary<string, FontResource> fonts = new Dictionary<string, FontResource>();
            
            foreach (GenericResource gr in R.List)
            {
                if (gr.GetType() == typeof(FontResource))
                    fonts[gr.GetResourceVariableName()] = (FontResource)gr;
            }
            int count = 0;
            foreach (string i in Fonts)
            {
                // la fonturi se adauga doar imaginile din fonturi
                if (fonts.ContainsKey(i)==false)
                {
                    p.EC.AddError("Font '" + i + "' is declared in profile: " + Name + " but is not available for current build: '" + buildName + "' !");
                    return "";
                }
                FontResource fnt = fonts[i];
                List<int> codes = fnt.GetCharacterCodesAvailableForABuild(buildName);
                count += codes.Count;
                foreach (int code in codes)
                    FontImages.Add(fnt.GetGlyphImageVariableName(code));
            }
            count += Images.Count+Sounds.Count+Shaders.Count+Raw.Count+Presentations.Count;
            s = string.Format("\tCHECK(Profiles.{0}->Create(Context,GAC_PROFILE_{1},{2}),false,\"\");\n", vname, this.Type.ToString().ToUpper(), count);
            foreach (string i in Images)
            {
                s += string.Format("\tCHECK(Profiles.{0}->SetResource({1},Images.{2}),false,\"\");\n", vname, index, i);
                index++;
            }
            // pun imaginile din fonturi
            foreach (string f_gname in FontImages)
            {
                s += string.Format("\tCHECK(Profiles.{0}->SetResource({1},&Fonts.{2}),false,\"\");\n", vname, index, f_gname);
                index++;
            }
            foreach (string i in Sounds)
            {
                s += string.Format("\tCHECK(Profiles.{0}->SetResource({1},Sounds.{2}),false,\"\");\n", vname, index, i);
                index++;
            }
            foreach (string i in Shaders)
            {
                s += string.Format("\tCHECK(Profiles.{0}->SetResource({1},Shaders.{2}),false,\"\");\n", vname, index, i);
                index++;
            }
            foreach (string i in Raw)
            {
                s += string.Format("\tCHECK(Profiles.{0}->SetResource({1},RawData.{2}),false,\"\");\n", vname, index, i);
                index++;
            }
            foreach (string i in Presentations)
            {
                s += string.Format("\tCHECK(Profiles.{0}->SetResource({1},Presentations.{2}),false,\"\");\n", vname, index, i);
                index++;
            }
            foreach (string i in Fonts)
            {
                // la fonturi se adauga doar imaginile din fonturi
            }
            return s;
        }
    }
    #endregion

    #region Stringuri
    public enum Language
    {
        Afrikaans = 0,
        Albanian = 1,
        Arabic = 2,
        Armenian = 3,
        Azerbaijani = 4,
        Basque = 5,
        Belarusian = 6,
        Bengali = 7,
        Bosnian = 8,
        Bulgarian = 9,
        Catalan = 10,
        Cebuanto = 11,
        Chinese = 12,
        Croatian = 13,
        Czech = 14,
        Danish = 15,
        Dutch = 16,
        English = 17,
        Esperanto = 18,
        Estonian = 19,
        Filipino = 20,
        Finnish = 21,
        French = 22,
        Galician = 23,
        Georgian = 24,
        German = 25,
        Greek = 26,
        Gujarati = 27,
        HaitianCreole = 28,
        Hebrew = 29,
        Hindi = 30,
        Hmong = 31,
        Hungarian = 32,
        Icelandic = 33,
        Indonesian = 34,
        Irish = 35,
        Italian = 36,
        Japanese = 37,
        Javanese = 38,
        Kannada = 39,
        Khmer = 40,
        Lao = 41,
        Latin = 42,
        Latvian = 43,
        Lithuanian = 44,
        Mecedonian = 45,
        Malay = 46,
        Maltese = 47,
        Marathi = 48,
        Norwegian = 49,
        Persian = 50,
        Polish = 51,
        Portuguese = 52,
        Romanian = 53,
        Russian = 54,
        Serbian = 55,
        Slovak = 56,
        Slovenian = 57,
        Spanish = 58,
        Swahili = 59,
        Swedish = 60,
        Tamil = 61,
        Telegu = 62,
        Thai = 63,
        Turkish = 64,
        Ukrainian = 65,
        Urdu = 66,
        Vietnamese = 67,
        Welsh = 68,
        Yiddish = 69,
    }
    public enum ExtendedLanguage
    {
        Afrikaans = 0,
        Albanian = 1,
        Arabic = 2,
        Armenian = 3,
        Azerbaijani = 4,
        Basque = 5,
        Belarusian = 6,
        Bengali = 7,
        Bosnian = 8,
        Bulgarian = 9,
        Catalan = 10,
        Cebuanto = 11,
        Chinese = 12,
        Croatian = 13,
        Czech = 14,
        Danish = 15,
        Dutch = 16,
        English = 17,
        Esperanto = 18,
        Estonian = 19,
        Filipino = 20,
        Finnish = 21,
        French = 22,
        Galician = 23,
        Georgian = 24,
        German = 25,
        Greek = 26,
        Gujarati = 27,
        HaitianCreole = 28,
        Hebrew = 29,
        Hindi = 30,
        Hmong = 31,
        Hungarian = 32,
        Icelandic = 33,
        Indonesian = 34,
        Irish = 35,
        Italian = 36,
        Japanese = 37,
        Javanese = 38,
        Kannada = 39,
        Khmer = 40,
        Lao = 41,
        Latin = 42,
        Latvian = 43,
        Lithuanian = 44,
        Mecedonian = 45,
        Malay = 46,
        Maltese = 47,
        Marathi = 48,
        Norwegian = 49,
        Persian = 50,
        Polish = 51,
        Portuguese = 52,
        Romanian = 53,
        Russian = 54,
        Serbian = 55,
        Slovak = 56,
        Slovenian = 57,
        Spanish = 58,
        Swahili = 59,
        Swedish = 60,
        Tamil = 61,
        Telegu = 62,
        Thai = 63,
        Turkish = 64,
        Ukrainian = 65,
        Urdu = 66,
        Vietnamese = 67,
        Welsh = 68,
        Yiddish = 69,
        All = 200,
    }
    public class StringValue
    {
        [XmlAttribute()]
        public Language Language;
        [XmlAttribute()]
        public String Value;

        public StringValue()
        {
            Language = Language.English;
            Value = "";
        }
        public StringValue(Language lang, String value)
        {
            Language = lang;
            Value = value;
        }
    }
    public class StringValues
    {
        [XmlAttribute()]
        public String VariableName = "";
        [XmlAttribute()]
        public String PreviewTemplate = "";
        [XmlAttribute()]
        public int Array1 = -1, Array2 = -1;
        [XmlAttribute()]
        public bool NonDrawable = false;
        public List<StringValue> Values = new List<StringValue>();



        public StringValues()
        {
            VariableName = "";
            Values = new List<StringValue>();
        }
        private StringValue Find(Language lang)
        {
            for (int tr = 0; tr < Values.Count; tr++)
                if (lang == Values[tr].Language)
                    return Values[tr];
            return null;
        }
        public Dictionary<Language,StringValue> GetAvailableLanguages()
        {
            Dictionary<Language, StringValue> d = new Dictionary<Language, StringValue>();
            foreach (StringValue sv in Values)
            {
                if ((sv.Value != null) && (sv.Value.Length > 0))
                    d[sv.Language] = sv;
            }
            return d;
        }
        public void Set(Language Lang, String value)
        {
            StringValue sv = Find(Lang);
            if (sv == null)
                Values.Add(new StringValue(Lang, value));
            else
                sv.Value = value;
        }
        public void Set(Dictionary<Language,string> values)
        {
            foreach (Language l in values.Keys)
            {
                Set(l, values[l]);
            }
        }
        public bool Contains(Language Lang)
        {
            return (Find(Lang) != null);
        }
        public String Get(Language Lang)
        {
            StringValue sv = Find(Lang);
            if (sv != null)
                return sv.Value;
            return "";
        }
        public StringValues(StringValues svalues)
        {
            VariableName = svalues.VariableName;
            Values = new List<StringValue>();
            foreach (StringValue sv in svalues.Values)
                Values.Add(new StringValue(sv.Language, sv.Value));
        }
        public string GetVariableNameWithArrayAndroid()
        {
            // tre sa ramana asa ca e folosit in XML-ul de android
            string s = VariableName;
            if (Array1 >= 0)
                s += "_" + Array1.ToString();
            if (Array2 >= 0)
                s += "_" + Array2.ToString();
            return s;
        }
        public string GetVariableNameWithArray()
        {
            return Project.GetVariableName(VariableName, Array1, Array2);
        }
        public string GetSortName()
        {
            string s = VariableName;
            if (Array1 >= 0)
            {
                s += String.Format("_{0:D9}", Array1);
                if (Array2 >= 0)
                    s += String.Format("_{0:D9}", Array2);
            }
            return s;
        }
        public void Delete(Language Lang)
        {
            StringValue sv = Find(Lang);
            if (sv != null)
                Values.Remove(sv);
        }
    };

    public class StringTemplatePreview: IComparable
    {
        [XmlAttribute()]
        public string Name = "";

        [XmlAttribute()]
        public int imageLeft, imageRight, imageTop, imageBottom;
        [XmlAttribute()]
        public string BitmapName = "";
        

        [XmlAttribute()]
        public int textLeft, textRight, textTop, textBottom;
        [XmlAttribute()]
        public string FontName = "";
        [XmlAttribute()]
        public bool WordWrap = false,Justify = false;
        [XmlAttribute()]
        public Alignament Align = Alignament.Center;
        [XmlAttribute()]
        public TextPainter.FontSizeMethod SizeMethod = TextPainter.FontSizeMethod.Scale;
        [XmlAttribute()]
        public float FontSizeValue = 0.0f,LineSpace = 0.0f;
        [XmlAttribute()]
        public int MissingCharColor = Color.Red.ToArgb();

        [XmlAttribute()]
        public int BackgroundColor = Color.Black.ToArgb();

        public int CompareTo(object obj)
        {
            return Name.ToLower().CompareTo(((StringTemplatePreview)obj).Name.ToLower());
        }
    }
    #endregion

    #region Ads Types

    public enum AdProvider
    {
        GoogleAdMob,
        Chartboost
    };
    public enum AdType: int
    {
        Banner = 0,
        Interstitial = 1,
        Rewardable = 2,
        Native = 3,
        InPlay = 4,
    };


    [XmlInclude(typeof(GoogleAdMobBanner))]
    [XmlInclude(typeof(GoogleAdMobInterstitial))]
    [XmlInclude(typeof(GoogleAdMobRewardable))]
    [XmlInclude(typeof(GoogleAdMobNativeExpress))]
    [XmlInclude(typeof(ChartboostInterstitial))]
    [XmlInclude(typeof(ChartboostRewardable))]
    [XmlInclude(typeof(ChartboostInPlay))]
    [XmlType("Ad"), XmlRoot("Ad")]
    public class GenericAd
    {
        [XmlAttribute()]
        public string Name = "";
        [XmlAttribute()]
        public AdProvider Provider = AdProvider.GoogleAdMob;
        [XmlAttribute()]
        public string Builds = "";
        [XmlAttribute()]
        public bool LoadOnStartup = false;
        [XmlAttribute()]
        public bool ReLoadAfterOpen = false;
        [XmlAttribute()]
        public int MaxAttemptsOnFail = 5;


        #region Atribute
        [XmlIgnore(), Description("Variable Name"), Category("General"), DisplayName("Variable Name")]
        public string _Name
        {
            get { return Name; }
            set { Name = value; }
        }
        [XmlIgnore(), Description("Provider"), Category("General"), DisplayName("Ad provider")]
        public AdProvider _Provider
        {
            get { return Provider; }
        }
        [XmlIgnore(), Description("Specifies what builds will use this ad"), Category("Settings"), DisplayName("Builds"), Editor(typeof(CheckBoxTypeEditor), typeof(UITypeEditor)), CheckBoxTypeEditor.Source(CheckBoxAttributeType.BuildsNoDevelop)]
        public string _Builds
        {
            get { return Builds; }
            set { Builds = value; }
        }
        [XmlIgnore(), Description("Load ad when app starts"), Category("Loading"), DisplayName("Load at startup")]
        public bool _LoadOnStartup
        {
            get { return LoadOnStartup; }
            set { LoadOnStartup = value; }
        }
        [XmlIgnore(), Description("Automatically reload an ad after it is opened"), Category("Loading"), DisplayName("Auto reload after open")]
        public bool _ReLoadAfterOpen
        {
            get { return ReLoadAfterOpen; }
            set { ReLoadAfterOpen = value; }
        }
        [XmlIgnore(), Description("The number of times system attempts to load an ad. Use 0 for no attempt."), Category("Loading"), DisplayName("Max attempts on fail")]
        public int _MaxAttemptsOnFail
        {
            get { return MaxAttemptsOnFail; }
            set { MaxAttemptsOnFail = value; }
        }
        #endregion

        #region List View Props
        [XmlIgnore(), Browsable(false)]
        public string propName { get { return Name; } }
        [XmlIgnore(), Browsable(false)]
        public string propProvider { get { return Provider.ToString(); } }
        [XmlIgnore(), Browsable(false)]
        public string propBuilds { get { return Builds; } }
        [XmlIgnore(), Browsable(false)]
        public string propDescription { get { return GetDescription(); } }
        [XmlIgnore(), Browsable(false)]
        public string propType { get { return GetAdTtype().ToString(); } }
        [XmlIgnore(), Browsable(false)]
        public bool propLoadOnStartup { get { return LoadOnStartup; } }
        [XmlIgnore(), Browsable(false)]
        public bool propReLoadAfterOpen { get { return ReLoadAfterOpen; } }
        #endregion

        #region Functii virtuale
        public virtual string GetProperties()
        {
            return "";
        }
        public virtual bool IsAvailableForOS(OSType os)
        {
            return false;
        }
        public virtual RectangleF GetPositionForSimulatedAds()
        {
            return new RectangleF(0.0f,0.0f,1.0f,1.0f);
        }
        public virtual GenericAd Duplicate() { return null;  }
        public virtual string GetDescription() { return ""; }
        public virtual AdType GetAdTtype() { return AdType.Banner; }
        #endregion

        protected void CopyTo(GenericAd ad)
        {
            ad.Name = Name;
            ad.LoadOnStartup = LoadOnStartup;
            ad.ReLoadAfterOpen = ReLoadAfterOpen;
            ad.MaxAttemptsOnFail = MaxAttemptsOnFail;
            ad.Builds = Builds;
            
        }
    };

    public enum GoogleAdMobBannerType
    {
        Banner,
        LargeBanner,
        MediumRectangle,
        SmartBanner,
    };
    public enum GoogleAdMobGender: int
    {
        Unknwon = 0,
        Male = 1,
        Female = 2,
    };

    [XmlType("GoogleAdMobBanner"), XmlRoot("GoogleAdMobBanner")]
    public class GoogleAdMobBanner : GenericAd
    {
        [XmlAttribute()]
        public string AdUnitID = "";
        [XmlAttribute()]
        public string Keywords = "";
        [XmlAttribute()]
        public GoogleAdMobBannerType Type = GoogleAdMobBannerType.Banner;
        [XmlAttribute()]
        public GoogleAdMobGender Gender = GoogleAdMobGender.Unknwon;
        [XmlAttribute()]
        public Alignament Align = Alignament.TopCenter;

        #region Atribute
        [XmlIgnore(), Description(""), Category("Settings"), DisplayName("Ad unit ID")]
        public string _AdUnitID
        {
            get { return AdUnitID; }
            set { AdUnitID = value; }
        }
        [XmlIgnore(), Description("List of keywords separated throug ','"), Category("Settings"), DisplayName("Keywords")]
        public string _Keywords
        {
            get { return Keywords; }
            set { Keywords = value; }
        }
        [XmlIgnore(), Description(""), Category("Settings"), DisplayName("Type")]
        public GoogleAdMobBannerType _Type
        {
            get { return Type; }
            set { Type = value; }
        }
        [XmlIgnore(), Description(""), Category("Settings"), DisplayName("Gender")]
        public GoogleAdMobGender _Gender
        {
            get { return Gender; }
            set { Gender = value; }
        }
        [XmlIgnore(), Description(""), Category("Settings"), DisplayName("Alignament")]
        public Alignament _Align
        {
            get { return Align; }
            set { Align = value; }
        }
        #endregion

        public override string GetProperties()
        {
            return String.Format("Type:{0}, AdUnitID:{1}, Keywords:{2}", Type.ToString(), AdUnitID, Keywords);
        }
        public override bool IsAvailableForOS(OSType os)
        {
            return (os == OSType.Android);
        }
        public override RectangleF GetPositionForSimulatedAds()
        {
            float w = 0, h = 0;
            switch (Type)
            {
                case GoogleAdMobBannerType.Banner: w=0.66f; h=0.0625f;break;
                case GoogleAdMobBannerType.LargeBanner: w=0.66f; h=0.125f;break;
                case GoogleAdMobBannerType.MediumRectangle: w=0.66f; h=0.3125f;break;
                case GoogleAdMobBannerType.SmartBanner: w = 1; h = 0.125f; break;
            }
            float x = 0, y = 0;
            switch (Align)
            {
                case Alignament.TopLeft: x = y = 0; break;
                case Alignament.TopCenter: x = 0.5f-w/2.0f; y = 0; break;
                case Alignament.TopRight: x = 1.0f - w; y = 0; break;
                case Alignament.RightCenter: x = 1.0f - w; y = 0.5f-h/2.0f; break;
                case Alignament.BottomRight: x = 1.0f - w; y = 1-h; break;
                case Alignament.BottomCenter: x = 0.5f - w / 2.0f; y = 1 - h; break;
                case Alignament.BottomLeft: x = 0; y = 1 - h; break;
                case Alignament.LeftCenter: x = 0; y = 0.5f - h / 2.0f; break;
                case Alignament.Center: x = 0.5f - w / 2.0f; y = 0.5f - h / 2.0f; break;
            };
            return new RectangleF(x, y, w, h);
        }
        public override GenericAd Duplicate()
        {
            GoogleAdMobBanner ad = new GoogleAdMobBanner();
            CopyTo(ad);
            ad.AdUnitID = AdUnitID;
            ad.Keywords = Keywords;
            ad.Type = Type;
            ad.Gender = Gender;
            ad.Align = Align;
            return ad;
        }
        public override string GetDescription() { return AdUnitID+"\n"+Keywords; }
        public override AdType GetAdTtype() { return AdType.Banner; }
    };

    [XmlType("GoogleAdMobInterstitial"), XmlRoot("GoogleAdMobInterstitial")]
    public class GoogleAdMobInterstitial : GenericAd
    {
        [XmlAttribute()]
        public string AdUnitID = "";
        [XmlAttribute()]
        public string Keywords = "";
        [XmlAttribute()]
        public GoogleAdMobGender Gender = GoogleAdMobGender.Unknwon;

        #region Atribute
        [XmlIgnore(), Description(""), Category("Settings"), DisplayName("Ad unit ID")]
        public string _AdUnitID
        {
            get { return AdUnitID; }
            set { AdUnitID = value; }
        }
        [XmlIgnore(), Description("List of keywords separated throug ','"), Category("Settings"), DisplayName("Keywords")]
        public string _Keywords
        {
            get { return Keywords; }
            set { Keywords = value; }
        }
        [XmlIgnore(), Description(""), Category("Settings"), DisplayName("Gender")]
        public GoogleAdMobGender _Gender
        {
            get { return Gender; }
            set { Gender = value; }
        }        
        #endregion

        public override string GetProperties()
        {
            return String.Format("Type:Interstitial, AdUnitID:{0}, Keywords:{1}", AdUnitID, Keywords);
        }
        public override bool IsAvailableForOS(OSType os)
        {
            return (os == OSType.Android);
        }
        public override RectangleF GetPositionForSimulatedAds()
        {
            return new RectangleF(0,0,1,1);
        }
        public override GenericAd Duplicate()
        {
            GoogleAdMobInterstitial ad = new GoogleAdMobInterstitial();
            CopyTo(ad);
            ad.AdUnitID = AdUnitID;
            ad.Keywords = Keywords;
            ad.Gender = Gender;
            return ad;
        }
        public override string GetDescription() { return AdUnitID + "\n" + Keywords; }
        public override AdType GetAdTtype() { return AdType.Interstitial; }
    };

    [XmlType("GoogleAdMobRewardable"), XmlRoot("GoogleAdMobRewardable")]
    public class GoogleAdMobRewardable : GenericAd
    {
        [XmlAttribute()]
        public string AdUnitID = "";

        #region Atribute
        [XmlIgnore(), Description(""), Category("Settings"), DisplayName("Ad unit ID")]
        public string _AdUnitID
        {
            get { return AdUnitID; }
            set { AdUnitID = value; }
        }
        #endregion

        public override string GetProperties()
        {
            return "AdUnitID:" + AdUnitID;
        }
        public override bool IsAvailableForOS(OSType os)
        {
            return (os == OSType.Android);
        }
        public override RectangleF GetPositionForSimulatedAds()
        {
            return new RectangleF(0, 0, 1, 1);
        }
        public override GenericAd Duplicate()
        {
            GoogleAdMobRewardable ad = new GoogleAdMobRewardable();
            CopyTo(ad);
            ad.AdUnitID = AdUnitID;
            return ad;
        }
        public override string GetDescription() { return AdUnitID; }
        public override AdType GetAdTtype() { return AdType.Rewardable; }
    };

    [XmlType("GoogleAdMobNativeExpress"), XmlRoot("GoogleAdMobNativeExpress")]
    public class GoogleAdMobNativeExpress : GenericAd
    {
        [XmlAttribute()]
        public string AdUnitID = "";
        [XmlAttribute()]
        public float leftPercentage = 0;
        public float topPercentage = 0;
        public float rightPercentage = 1;
        public float bottomPercentage = 0.25f;

        #region Atribute
        [XmlIgnore(), Description(""), Category("Settings"), DisplayName("Ad unit ID")]
        public string _AdUnitID
        {
            get { return AdUnitID; }
            set { AdUnitID = value; }
        }
        [XmlIgnore(), Description("Left position in percentages of the ad"), Category("Location"), DisplayName("Left")]
        public string _Left
        {
            get { return Project.ProcentToString(leftPercentage); }
            set { float v = 0;
                if ((Project.StringToProcent(value, ref v)) && (v >= 0) && (v <= 1))
                    leftPercentage = v;
            }
        }
        [XmlIgnore(), Description("Top position in percentages of the ad"), Category("Location"), DisplayName("Top")]
        public string _Top
        {
            get { return Project.ProcentToString(topPercentage); }
            set
            {
                float v = 0;
                if ((Project.StringToProcent(value, ref v)) && (v >= 0) && (v <= 1))
                    topPercentage = v;
            }
        }
        [XmlIgnore(), Description("Right position in percentages of the ad"), Category("Location"), DisplayName("Right")]
        public string _Right
        {
            get { return Project.ProcentToString(rightPercentage); }
            set
            {
                float v = 0;
                if ((Project.StringToProcent(value, ref v)) && (v >= 0) && (v <= 1))
                    rightPercentage = v;
            }
        }
        [XmlIgnore(), Description("Bottom position in percentages of the ad"), Category("Location"), DisplayName("Bottom")]
        public string _Bottom
        {
            get { return Project.ProcentToString(bottomPercentage); }
            set
            {
                float v = 0;
                if ((Project.StringToProcent(value, ref v)) && (v >= 0) && (v <= 1))
                    bottomPercentage = v;
            }
        }
        #endregion

        public override string GetProperties()
        {
            return "AdUnitID:" + AdUnitID;
        }
        public override bool IsAvailableForOS(OSType os)
        {
            return (os == OSType.Android);
        }
        public override RectangleF GetPositionForSimulatedAds()
        {
            return new RectangleF(leftPercentage,topPercentage,rightPercentage-leftPercentage,bottomPercentage-topPercentage);
        }
        public override GenericAd Duplicate()
        {
            GoogleAdMobNativeExpress ad = new GoogleAdMobNativeExpress();
            CopyTo(ad);
            ad.AdUnitID = AdUnitID;
            return ad;
        }
        public override string GetDescription() { return AdUnitID + String.Format("\nLocation:({0},{1}) - ({2},{3}) [Width:{4}, Height:{5}]", Project.ProcentToString(leftPercentage), Project.ProcentToString(topPercentage), Project.ProcentToString(rightPercentage), Project.ProcentToString(bottomPercentage), Project.ProcentToString(rightPercentage-leftPercentage), Project.ProcentToString(bottomPercentage-topPercentage)); }
        public override AdType GetAdTtype() { return AdType.Native; }
    };

    [XmlType("ChartboostInterstitial"), XmlRoot("ChartboostInterstitial")]
    public class ChartboostInterstitial : GenericAd
    {
        public ChartboostInterstitial()
        {
            this.Provider = AdProvider.Chartboost;
        }
        public override string GetProperties()
        {
            return "Type:Interstitial";
        }
        public override bool IsAvailableForOS(OSType os)
        {
            return (os == OSType.Android);
        }
        public override RectangleF GetPositionForSimulatedAds()
        {
            return new RectangleF(0, 0, 1, 1);
        }
        public override GenericAd Duplicate()
        {
            ChartboostInterstitial ad = new ChartboostInterstitial();
            CopyTo(ad);
            return ad;
        }
        public override string GetDescription() { return ""; ; }
        public override AdType GetAdTtype() { return AdType.Interstitial; }
    };
    
    [XmlType("ChartboostRewardable"), XmlRoot("ChartboostRewardable")]
    public class ChartboostRewardable : GenericAd
    {
        public ChartboostRewardable()
        {
            this.Provider = AdProvider.Chartboost;
        }
        public override string GetProperties()
        {
            return "Type:Rewardable";
        }
        public override bool IsAvailableForOS(OSType os)
        {
            return (os == OSType.Android);
        }
        public override RectangleF GetPositionForSimulatedAds()
        {
            return new RectangleF(0, 0, 1, 1);
        }
        public override GenericAd Duplicate()
        {
            ChartboostInterstitial ad = new ChartboostInterstitial();
            CopyTo(ad);
            return ad;
        }
        public override string GetDescription() { return ""; }
        public override AdType GetAdTtype() { return AdType.Rewardable; }
    };

    [XmlType("ChartboostInPlay"), XmlRoot("ChartboostInPlay")]
    public class ChartboostInPlay : GenericAd
    {
        public ChartboostInPlay()
        {
            this.Provider = AdProvider.Chartboost;
        }
        public override string GetProperties()
        {
            return "Type:InPlay ad";
        }
        public override bool IsAvailableForOS(OSType os)
        {
            return (os == OSType.Android);
        }
        public override RectangleF GetPositionForSimulatedAds()
        {
            return new RectangleF(0, 0, 1, 1);
        }
        public override GenericAd Duplicate()
        {
            ChartboostInterstitial ad = new ChartboostInterstitial();
            CopyTo(ad);
            return ad;
        }
        public override string GetDescription() { return ""; }
        public override AdType GetAdTtype() { return AdType.InPlay; }
    };

    #endregion

    #region Global alarms and counters

    [XmlType("Alarm"), XmlRoot("Alarm")]
    public class Alarm: IComparable
    {
        [XmlAttribute()]
        public string Name = "", Builds = "Develop";
        [XmlAttribute()]
        public bool Enabled = true;
        [XmlAttribute()]
        public bool OneTimeOnly = false;
        [XmlAttribute()]
        public int AlarmType = 0, Duration = 5;
        [XmlAttribute()]
        public int Year = 0; // different values depending on the AlarmType
        [XmlAttribute()]
        public int Month = 0; // different values depending on the AlarmType
        [XmlAttribute()]
        public int Day = 0; // different values depending on the AlarmType
        [XmlAttribute()]
        public int Hour = 0; // different values depending on the AlarmType
        [XmlAttribute()]
        public int Minute = 0; // different values depending on the AlarmType
        [XmlAttribute()]
        public bool PushNotification = true;
        [XmlAttribute()]
        public string PushNotificationString = "";
        [XmlAttribute()]
        public int UniqueID = 0; // latest ID

        [XmlIgnore()]
        public int CounterIndex = -1; // internal for C/C++ conversion

        #region List View Props
        [XmlIgnore(), Browsable(false)]
        public string propName { get { return Name; } }
        [XmlIgnore(), Browsable(false)]
        public string propBuilds { get { return Builds; } }
        [XmlIgnore(), Browsable(false)]
        public bool propEnabled { get { return Enabled; } set { Enabled = value; } }
        [XmlIgnore(), Browsable(false)]
        public bool propOneTimeOnly { get { return OneTimeOnly; } set { OneTimeOnly = value; } }
        [XmlIgnore(), Browsable(false)]
        public string propDuration { get { return ConvertValueToStringDuration(Duration); } }
        [XmlIgnore(), Browsable(false)]
        public string propPushNotification 
        { 
            get {
                if (PushNotification == false)
                    return "No";
                return "Yes (using string '" + PushNotificationString + "')";
            } 
        }
        [XmlIgnore(), Browsable(false)]
        public string propDescription 
        { 
            get 
            { 
                switch (AlarmType)
                {
                    case 0:
                        if ((Month >= 1) && (Month <= 12))
                            return string.Format("On {0:D4}-{1:D2}-{2:D2}  {3:D2}:{4:D2}", Year, months[Month - 1], Day, Hour, Minute);
                        else
                            return "Invalid value for exact date (Month=" + Month.ToString() + ")";
                    case 1:
                        if ((Month >= 1) && (Month <= 12))
                            return string.Format("Every year on {0:D2}-{1:D2}  {2:D2}:{3:D2}", months[Month-1], Day, Hour, Minute);
                        else
                            return "Invalid value for specific date (Month=" + Month.ToString() + ")";
                    case 2:
                        if (Day == 1)
                            return "On the first day of every month";
                        if (Day == 2)
                            return "On the second day of every mobth";
                        if (Day == 3)
                            return "On the third day of every mobth";
                        return "On the " + Day.ToString() + "th day of every month";
                    case 3:
                        if (Day<daysOfWeek.Length)
                            return "On "+daysOfWeek[Day];
                        return "Invalid value for day of the week: "+Day.ToString();
                    case 4:
                        return string.Format("Every day at {0:D2}:{1:D2}", Hour, Minute);
                    case 5:
                        return "Every " + Minute.ToString() + " minutes";
                    case 6:
                        return "Every " + Hour.ToString() + " hours";
                    case 7:
                        return "Every " + Day.ToString() + " days";
                    default:
                        return "? (unknown alert type: " + AlarmType.ToString() + " )";
                }
            } 
        }
        #endregion

        #region Static functions / data
        private static string[] daysOfWeek = { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
        private static string[] months = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Nov", "Dec" };

        public static string[] alarmTypes = {
            // GDT - IMPORTANT - ORDINEA NU TREBUIE MODIFICATA
        /*  0 */                                "Exact date (Year / Month / Day / Hour / Minute)",
        /*  1 */                                "On a specific date (Month / Day / Hour / Minute )",
        /*  2 */                                "On a specific day of the month (from 00:00 to 23:59)",
        /*  3 */                                "On a specific day of a week (from 00:00 to 23:59)",
        /*  4 */                                "Daily at a specific time (Hour / Minute)",
        /*  5 */                                "Every \"x\" minutes (starting from the moment the alarm gets activated)",
        /*  6 */                                "Every \"x\" hours (starting from the moment the alarm gets activated)",
        /*  7 */                                "Every \"x\" days (starting from the moment the alarm gets activated)",
                                            };

        private static List<Tuple<int, string>> durationsInMinutes = new List<Tuple<int, string>>()
        {
            new Tuple<int,string>(0,"Always / Continue"),
            new Tuple<int,string>(1,"One minute"),
            new Tuple<int,string>(5,"5 minutes"),
            new Tuple<int,string>(10,"10 minutes"),
            new Tuple<int,string>(15,"15 minutes"),
            new Tuple<int,string>(30,"30 minutes"),
            new Tuple<int,string>(60,"One hour"),
            new Tuple<int,string>(120,"2 hours"),
            new Tuple<int,string>(180,"3 hours"),
            new Tuple<int,string>(240,"4 hours"),
            new Tuple<int,string>(300,"5 hours"),
            new Tuple<int,string>(360,"6 hours"),
            new Tuple<int,string>(420,"7 hours"),
            new Tuple<int,string>(480,"8 hours"),
            new Tuple<int,string>(720,"12 hours"),
            new Tuple<int,string>(960,"16 hours"),
            new Tuple<int,string>(1440,"One day"),
            new Tuple<int,string>(1440*2,"2 days"),
            new Tuple<int,string>(1440*3,"3 days"),
            new Tuple<int,string>(1440*4,"4 days"),
            new Tuple<int,string>(1440*5,"5 days"),
            new Tuple<int,string>(1440*6,"6 days"),
        };


        public static int ConvertStringDurationToValue(string value)
        {
            foreach (Tuple<int, string> t in durationsInMinutes)
            {
                if (t.Item2 == value)
                    return t.Item1;
            }
            return 0; // None
        }
        public static string ConvertValueToStringDuration(int value)
        {
            foreach (Tuple<int, string> t in durationsInMinutes)
            {
                if (t.Item1 == value)
                    return t.Item2;
            }
            return "? (" + value.ToString() + ")";
        }
        public static List<string> GetDurationValues()
        {
            List<string> lst = new List<string>();
            foreach (Tuple<int, string> t in durationsInMinutes)
                lst.Add(t.Item2);
            return lst;
        }

        #endregion

        public void CopyFrom(Alarm a)
        {
            this.Name = a.Name;
            this.Builds = a.Builds;
            this.Enabled = a.Enabled;
            this.OneTimeOnly = a.OneTimeOnly;
            this.AlarmType = a.AlarmType;
            this.Duration = a.Duration;
            this.Year = a.Year;
            this.Month = a.Month;
            this.Day = a.Day;
            this.Hour = a.Hour;
            this.Minute = a.Minute;
            this.PushNotification = a.PushNotification;
            this.PushNotificationString = a.PushNotificationString;
        }


        int IComparable.CompareTo(object obj)
        {
            return Name.CompareTo(((Alarm)obj).Name);
        }

        public string GetCppInitCode(bool debugMode)
        {
            string s = "";
            s += CounterIndex.ToString() + ","+UniqueID.ToString()+",";
            s += Enabled.ToString().ToLower() + ",";
            s += OneTimeOnly.ToString().ToLower() + ",";
            if (Duration <= 0) s += "0xFFFFFFFF,"; else s += Duration.ToString() + ",";
            if (PushNotification)
                s += "NULL,";  // PushNotificationString (resursa)
            else
                s += "NULL,";
            s += AlarmType.ToString() + ",";
            // in functie de tipul de alarma - setez alte lucruri
            s += string.Format("{0},{1},{2},{3},{4}", Year, Month, Day, Hour, Minute);
            return s;
        }

        public bool Validate(Project prj)
        {
            switch (AlarmType)
            {
                case 0: break;
                case 1: // "On a specific date (Month / Day / Hour / Minute )"
                    if ((OneTimeOnly == false) && ((Duration > 525600) || (Duration == 0)))
                    {
                        prj.EC.AddError(string.Format("Alarm {0} has to be triggered every year but its duration is bigger than one year !", Name));
                        return false;
                    }
                    break;
                case 2: break;
                case 3: break;
                case 4: //"Daily day at a specific time (Hour / Minute)"
                    if ((OneTimeOnly == false) && ((Duration > 1440) || (Duration == 0)))
                    {
                        prj.EC.AddError(string.Format("Alarm {0} has to be triggered daily but its duration is bigger than one day !", Name));
                        return false;
                    }
                    break;
                case 5: // "Every \"x\" minutes (starting from the moment the alarm gets activated)"
                    if ((OneTimeOnly==false) && ((Duration> Minute) || (Duration == 0)))
                    {
                        prj.EC.AddError(string.Format("Alarm {0} has an invalid duration time in minutes: ({1}) but it is set to be activated every {2} minutes continuously !",Name,Duration,Minute));
                        return false;
                    }
                    break;
                case 6: // "Every \"x\" hours (starting from the moment the alarm gets activated)"
                    if ((OneTimeOnly == false) && ((Duration > Hour*60) || (Duration == 0)))
                    {
                        prj.EC.AddError(string.Format("Alarm {0} has an invalid duration time in hours: ({1}) but it is set to be activated every {2} hours continuously !", Name, Duration/60, Hour));
                        return false;
                    }
                    break;
                case 7: // "Every \"x\" days (starting from the moment the alarm gets activated)"
                    if ((OneTimeOnly == false) && ((Duration > Day * 60 * 60) || (Duration == 0)))
                    {
                        prj.EC.AddError(string.Format("Alarm {0} has an invalid duration time in days: ({1}) but it is set to be activated every {2} day(s) continuously !", Name, Duration / (60*60), Day));
                        return false;
                    }
                    break;
                default:
                    prj.EC.AddError("Unknown alarm type: " + AlarmType.ToString() + " for Alarm '" + Name + "' !");
                    return false;
            }
            return true;
        }
    };

    [XmlType("CounterGroup"), XmlRoot("CounterGroup")]
    public class CounterGroup: IComparable
    {
        [XmlAttribute()]
        public string Name = "";
        [XmlAttribute()]
        public int UpdateMethod = 0;
        [XmlAttribute()]
        public int MinimTimeInterval = 0;
        [XmlAttribute()]
        public int StartTimerMethod = 0;
        [XmlAttribute()]
        public string StartTimerScene = "";
        [XmlAttribute()]
        public int AfterUpdateBehavior = 0;
        [XmlAttribute()]
        public bool UseEnableConditionProperty = false;

        #region List View Props
        [XmlIgnore(), Browsable(false)]
        public string propGroup { get { return Name; } }
        [XmlIgnore(), Browsable(false)]
        public string propDescription 
        { 
            get 
            {
                string s = GetUpdateMethod(UpdateMethod);
                if (MinimTimeInterval > 0)
                    s += " [Minim time limit: " + MinimTimeInterval.ToString() + " seconds]";
                return s;
            } 
        }
        #endregion

        public int CompareTo(object obj)
        {
            CounterGroup o = (CounterGroup)obj;
            return Name.CompareTo(o.Name);
        }

        private static string[] UpdateMethodsNames = 
        {
            "Update all counters",
            "Update only one counter based on priority",
            "Update only one counter chosen randomnly (the counter has to be enabled)",
        };
        private static string[] AfterUpdateTimerStatus = 
        {
            "Stop the timer (the timer will have to be started manually)",
            "Start the timer",
            "Keep the timer in the exact state before the update call (preserve)",
        };

        public static int GetUpdateMethodsCount() 
        { 
            return UpdateMethodsNames.Length; 
        }
        public static string GetUpdateMethod(int index)
        {
            if ((index < 0) || (index >= UpdateMethodsNames.Length))
                return "";
            return UpdateMethodsNames[index];
        }
        public static int GetAfterUpdateTimerStatusCount()
        { 
            return AfterUpdateTimerStatus.Length; 
        }
        public static string GetAfterUpdateTimerStatus(int index)
        {
            if ((index < 0) || (index >= AfterUpdateTimerStatus.Length))
                return "";
            return AfterUpdateTimerStatus[index];
        }
    }

    public class EnableStateCondition
    {
        public int conditionID;
        public string strValue;
        public bool useAND;

        public EnableStateCondition(int c, string v, bool a) { conditionID = c; strValue = v; useAND = a; }
    };

    [XmlType("Counter"), XmlRoot("Counter")]
    public class Counter
    {
        [XmlAttribute()]
        public string Name = "", Group = "", Builds = "Develop", Hash = "", AssociatedScene = "", EnableCondition = "";
        
        [XmlAttribute()]
        public bool Enabled = true, Persistent = true;

        [XmlAttribute()]
        public int Interval = 1, MaxTimes = 0, Priority = 1;

        [XmlIgnore()]
        public int CounterIndex = -1; // internal for C/C++ conversion

        #region Atribute
        [XmlIgnore(), Description("Internal unique ID"), Category("General"), DisplayName("ID")]
        public virtual string _Hash
        {
            get { return Hash; }
        }
        [XmlIgnore(), Description("Name"), Category("General"), DisplayName("Name")]
        public virtual string _Name
        {
            get { return Name; }
            set {
                if (Project.ValidateVariableNameCorectness(value, false) == false)
                {
                    MessageBox.Show("Invalid name - use letters or number to set the name correctly ");
                }
                else
                {
                    Name = value;
                }
            }
        }
        [XmlIgnore(), Description("Group"), Category("General"), DisplayName("Group"), Editor(typeof(CounterGroupSelectorEditor), typeof(UITypeEditor))]
        public virtual string _Group
        {
            get { return Group; }
            set {
                if ((Project.ValidateVariableNameCorectness(value, false) == false) && (value.Length>0))
                {
                    MessageBox.Show("Invalid group - use letters or number to set the name correctly ");
                }
                else
                {
                    Group = value; 
                }                
            }
        }
        [XmlIgnore(), Description("Enable/Disable status"), Category("General"), DisplayName("Enabled")]
        public virtual bool _Enabled
        {
            get { return Enabled; }
            set { Enabled = value; }
        }
        [XmlIgnore(), Description("If set to true this counter will preserve its value through several app executions."), Category("General"), DisplayName("Persistent")]
        public virtual bool _Persistent
        {
            get { return Persistent; }
            set { Persistent = value; }
        }
        [XmlIgnore(), Description("Specifies what builds will use this publish object"), Category("Target"), DisplayName("Builds"), Editor(typeof(CheckBoxTypeEditor), typeof(UITypeEditor)), CheckBoxTypeEditor.Source(CheckBoxAttributeType.Builds)]
        public virtual string _Builds
        {
            get { return Builds; }
            set { Builds = value; }
        }
        [XmlIgnore(), Description("Number of times this counter has to be called before it is triggered"), Category("Settings"), DisplayName("Interval")]
        public virtual int _Interval
        {
            get { return Interval; }
            set { if (value < 1) Interval = 1; else { if (value > 0x7fff) Interval = 0x7fff; else Interval = value; } }
        }
        [XmlIgnore(), Description("Priority of the counter. Less is better (counters with a smaller priority number will be called before other ones"), Category("Settings"), DisplayName("Priority")]
        public virtual int _Priority
        {
            get { return Priority; }
            set { if (value < 1) Priority = 1; else { if (value > 99) Priority = 99; else Priority = value; } }
        }
        [XmlIgnore(), Description("Maximum nunber of times that counter can be triggered. After this maximum number is reached the counter will be disabled !"), Category("Settings"), DisplayName("Max Times")]
        public virtual string _MaxTimes
        {
            get { return propMaxTimes; }
            set {  
                int newValue = 0;
                if ((int.TryParse(value, out newValue) == false) || (newValue < 1))
                    MaxTimes = 0;
                else
                {
                    if (newValue > 0x7FFF)
                        MaxTimes = 0x7FFF;
                    else
                        MaxTimes = newValue;
                }
            }
        }
        [XmlIgnore(), Description("Scene to goto in case of group update trigger"), Category("Settings"), DisplayName("Associated Scene"), Editor(typeof(SceneSelectorEditor), typeof(UITypeEditor))]
        public virtual string _AssociatedScene
        {
            get { return AssociatedScene; }
            set
            {
                AssociatedScene = value;
            }
        }
        [XmlIgnore(), Description("Rules used to automatically enable this condition !"), Category("Settings"), DisplayName("Enable condition"), Editor(typeof(CounterEnableConditionEditor), typeof(UITypeEditor))]
        public virtual string _EnableCondition
        {
            get { return EnableCondition; }
            set
            {
                string res = ConditionListToStringRepresentation(StringRepresentationToConditionList(value));
                if (res != null)
                    EnableCondition = res;
                else
                    MessageBox.Show("Invalid format: " + value);
            }
        }
        #endregion

        #region List View Props
        [XmlIgnore(), Browsable(false)]
        public string propName { get { return Name; } }
        [XmlIgnore(), Browsable(false)]
        public string propGroup { get { return Group; } }
        [XmlIgnore(), Browsable(false)]
        public string propBuilds { get { return Builds; } }
        [XmlIgnore(), Browsable(false)]
        public bool propEnabled { get { return Enabled; } set { Enabled = value; } }
        [XmlIgnore(), Browsable(false)]
        public bool propPersistent { get { return Persistent; } set { Persistent = value; } }
        [XmlIgnore(), Browsable(false)]
        public int propInterval { get { return Interval; } }
        [XmlIgnore(), Browsable(false)]
        public int propPriority { get { return Priority; } }
        [XmlIgnore(), Browsable(false)]
        public string propMaxTimes { get { if (MaxTimes >= 1) return MaxTimes.ToString(); else return "Unlimited"; } }
        [XmlIgnore(), Browsable(false)]
        public string propScene { get { return AssociatedScene; } }
        #endregion

        public string GetDescription()
        {
            return "Group: '" + Group + "' Interval:" + Interval.ToString() + " MaxTimes:" + MaxTimes.ToString() + " Priority:" + Priority.ToString() + " Builds:" + Builds;
        }
        // fb AND ac < 1
        // ac < 5
        // ad AdMobInterstitial OR ad CBIntesrtiail

        #region Condition functions
        public static string[] ConditionsNames = 
        {
            "Facebook application is installed",    // 0
            "ActionCounter is less than",           // 1
            "ActionCounter is bigger than",         // 2
            "The following Ad unit is loaded:",     // 3
            "Ads are enabled",                      // 4
            "Ads are disabled",                     // 5
        };
        public static string[] ConditionsCPPIds = 
        {
            "AUTO_ENABLE_STATE_FB_APP_EXISTS",
            "AUTO_ENABLE_STATE_ACTIONS_LESS",
            "AUTO_ENABLE_STATE_ACTIONS_BIGGER",
            "AUTO_ENABLE_STATE_AD_IS_LOADED",
            "AUTO_ENABLE_STATE_ADS_ARE_ENABLED",
            "AUTO_ENABLE_STATE_ADS_ARE_DISABLED",
        };
        private static int GetSplitterCharType(char ch)
        {
            if ((ch >= 'A') && (ch <= 'Z'))
                return 1;
            if ((ch >= 'a') && (ch <= 'z'))
                return 1;
            if ((ch >= '0') && (ch <= '9'))
                return 1;
            if (ch == '_')
                return 1;
            if ((ch == '<') || (ch == '>') || (ch == '='))
                return 2;
            return 0;
        }
        private static List<string> Splitter(string compactFormat)
        {
            List<string> s = new List<string>();
            int type = -1;
            string res = "";
            compactFormat += " "; // ca sa fiu sigur ca le adaug pe toate
            foreach (char ch in compactFormat)
            {
                int ctype = GetSplitterCharType(ch);
                if (ctype!=type)
                {
                    if ((type > 0) && (res.Length > 0))
                        s.Add(res);
                    if ((res.Equals("and", StringComparison.InvariantCultureIgnoreCase)) || (res.Equals("or", StringComparison.InvariantCultureIgnoreCase)))
                        s.Add("###");
                    res = "";
                }
                type = ctype;
                res += ch;
            }
            if ((s.Count > 0) && (s[s.Count - 1] != "###"))
                s.Add("###");
            return s;
        }
        private static EnableStateCondition GetCondition(List<string> ls, int start, int end, Project prj = null)
        {
            bool useAnd = ls[end].Equals("and",StringComparison.InvariantCultureIgnoreCase);
            // fb
            if (ls[start].Equals("fb",StringComparison.InvariantCultureIgnoreCase))
                return new EnableStateCondition(0,"",useAnd);
            // ac
            if (ls[start].Equals("ac",StringComparison.InvariantCultureIgnoreCase))
            {
                // trebuie sa am minim 2 parametri
                if (start + 2 > end)
                {
                    if (prj != null)
                        prj.EC.AddError("expecting at least 2 operands for 'ac' command in counter auto enable status !");
                    return null;
                }
                int result = 0;
                if ((ls[start + 1] == "<") && (int.TryParse(ls[start + 2], out result)))
                    return new EnableStateCondition(1, ls[start + 2], useAnd);
                if ((ls[start + 1] == ">") && (int.TryParse(ls[start + 2], out result)))
                    return new EnableStateCondition(2, ls[start + 2], useAnd);
                // nu e ceva cunoscut pentru formatul "ac"
                if (prj != null)
                    prj.EC.AddError("unkwnon format for 'ac' command in counter auto enable status !");
                return null;
            }
            // ad
            if (ls[start].Equals("ad", StringComparison.InvariantCultureIgnoreCase))
            {
                // trebuie sa am minim 1 parametri
                if (start + 1 > end)
                {
                    if (prj != null)
                        prj.EC.AddError("expecting at least one operand for 'ad' command in counter auto enable status !");
                    return null;
                }
                return new EnableStateCondition(3, ls[start + 1], useAnd);
            }
            // ads
            if (ls[start].Equals("ads", StringComparison.InvariantCultureIgnoreCase))
            {
                // trebuie sa am minim 1 parametri
                if (start + 1 > end)
                {
                    if (prj != null)
                        prj.EC.AddError("expecting at least one operand for 'ads' command in counter auto enable status !");
                    return null;
                }
                if (ls[start + 1].ToLower().StartsWith("enable"))
                    return new EnableStateCondition(4, "", useAnd);
                if (ls[start + 1].ToLower().StartsWith("disable"))
                    return new EnableStateCondition(5, "", useAnd);
                if (prj != null)
                    prj.EC.AddError("unknown operand: '" + ls[start+1] + "' for 'ads' command !");
                return null;
            }
            if (prj != null)
                prj.EC.AddError("unknown command: '"+ls[start]+"' for counter auto enable status");
            // altfel nu stiu ce e !
            return null;
        }
        private static string GetCondition(EnableStateCondition esc)
        {
            esc.strValue = esc.strValue.Trim();
            int result = 0;
            switch (esc.conditionID)
            {
                case 0: return "fb"; 
                case 1:                    
                    if (int.TryParse(esc.strValue, out result) == false)
                        return null;
                    return "ac < " + esc.strValue;
                case 2:
                    if (int.TryParse(esc.strValue, out result) == false)
                        return null;
                    return "ac > " + esc.strValue;
                case 3:
                    if (esc.strValue.Length == 0)
                        return null;
                    return "ad " + esc.strValue;
                case 4: return "ads enabled";
                case 5: return "ads disabled";
            }
            // daca e necunoscut
            return null;
        }
        public static List<EnableStateCondition> StringRepresentationToConditionList(string compactFormat, Project prj = null)
        {
            List<string> ls = Splitter(compactFormat);
            List<EnableStateCondition> ens = new List<EnableStateCondition>();
            int start = 0;
            while (start<ls.Count)
            {
                int found = -1;
                for (int tr=start;tr<ls.Count;tr++)
                    if (ls[tr]=="###")
                    {
                        found = tr;
                        break;
                    }
                if (found>start)
                {
                    EnableStateCondition cond = GetCondition(ls, start, found - 1,prj);
                    if (cond!=null)
                    {
                        ens.Add(cond);
                    }
                    start = found + 1;
                }
                else
                {
                    break;
                }
            }
            return ens;
        }
        public static string ConditionListToStringRepresentation(List<EnableStateCondition> lst)
        {
            string result = "";
            for (int tr=0;tr<lst.Count;tr++)
            {
                string r = GetCondition(lst[tr]);
                if (r == null)
                    return null;
                result += r;
                if ((tr+1)<lst.Count)
                {
                    if (lst[tr].useAND)
                        result += " AND ";
                    else
                        result += " OR ";
                }
            }
            return result;
        }
        #endregion
    };
    #endregion

    #region Builder configurations
    public enum ScreenOrientation
    {
        Portrait,
        Landscape
    };

    public enum AndroidVersion : int
    {
        None = 0,
        Froyo_8 = 8,
        Gingerbread_9 = 9,
        Gingerbread_10 = 10,
        Honeycomb_11 = 11,
        Honeycomb_12 = 12,
        Honeycomb_13 = 13,
        IceCreamSandwich_14 = 14,
        IceCreamSandwich_15 = 15,
        JellyBean_16 = 16,
        JellyBean_17 = 17,
        JellyBean_18 = 18,
        KitKat_19 = 19,
        KitKat_20 = 20,
        Lollipop_21 = 21,
        Lollipop_22 = 22,
        Marshmallow_23 = 23,
        Nougat_24 = 24,
        Nougat_25 = 25,
        Oreo_26 = 26,
        Oreo_27 = 27,
    };
    public enum AndroidJavaCodeObfuscation
    {
        None,
        Proguard
    };
    public enum AndroidBillingMarket
    {
        None,
        GooglePlay,
    };
    public enum WindowsDesktopCodePacker
    {
        None,
        UPX,
    };
    public enum CompilerOptimizationMode
    {
        None,
        Debug,
        Normal,
        Optimize,
        HighlyOptimize,
    };
    public enum BuildImageCreateMethod
    {
        Dynamic,
        ResizeAtStartup,
        CustomizeForEachImage,
    };
    public enum SnapshotTypeMethod
    {
        OnlyGlobalData,
        Full,
    };
    public enum AnalyticsFrameworkType
    {
        None,
        GoogleAnalytics,
        FireBase,
        Flurry,
    };

    public interface ITaskNotifier
    {
        void CreateSubTask(string name);
        void SetMinMax(int min, int max);
        bool UpdateSuccessErrorState(bool isSuccesiful);
        void IncrementProgress();
        void Info(string text);
        void SendCommand(Command cmd);
        void SendCommand(Command cmd, Object param);
    };

    public class BuildExtension
    {
        public Project prj;
        public ITaskNotifier task;
        public string root;
        private string outputFile = "";
        public GenericBuildConfiguration Build;
        public virtual void OnBuild() { }

        public void SetOutputFile(string value)
        {
            outputFile = value;
        }
        protected string GetOutputFile(string defaultPath,string defaultName)
        {
            DateTime dt = DateTime.Now;
            string currentDateTime = string.Format("{0:D4}-{1:D2}-{2:D2}---{3:D2}-{4:D2}-{5:D2}", dt.Year, dt.Month, dt.Day,dt.Hour,dt.Minute,dt.Second);
            if ((outputFile == null) || (outputFile.Length == 0))
                return Path.Combine(defaultPath, defaultName);
            string result = outputFile;
            result = result.Replace("$$DATE$$", currentDateTime);
            result = result.Replace("$$APP$$", defaultName);
            result = result.Replace("$$PATH$$", defaultPath);
            return result;
        }
        protected bool CopyDefaultCPPFiles(string[] FrameworkSources, string destinationFolder,ref string cpp_list)
        {
            foreach (string name in FrameworkSources)
            {
                if (Disk.Copy(Path.Combine(prj.Settings.FrameworkSourcesPath, name), Path.Combine(destinationFolder, name), prj.EC) == false)
                    return false;
                if (name.ToLower().EndsWith(".cpp"))
                    cpp_list += name + " ";
            }
            // copii fisierele din aplicatie
            foreach (string fname in Directory.EnumerateFiles(Path.Combine(prj.ProjectPath, "CppProject")))
            {
                if ((fname.ToLower().EndsWith(".cpp")) || (fname.ToLower().EndsWith(".h")))
                {
                    string name = Path.GetFileName(fname);
                    if (name.ToLower() == "main.cpp")
                        continue;
                    if (Disk.Copy(fname, Path.Combine(destinationFolder, name), prj.EC) == false)
                        return false;
                    if (name.ToLower().EndsWith(".cpp"))
                        cpp_list += name + " ";
                }
            }
            return true;
        }
        protected string GetTool(string name)
        {
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            return Path.Combine(path, "Tools",name, name + ".exe");
        }
        protected string GetToolsFolder(string name)
        {
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            return Path.Combine(path, "Tools", name);
        }
    };

    public class BuildResources
    {
        public class ArrayCounter
        {
            private Dictionary<string, bool> Resources = new Dictionary<string, bool>();
            private int Max1 = -1;
            private int Max2 = -1;
            private GenericResource res = null;
            public void Add(GenericResource r)
            {
                string k = r.GetResourceVariableName();
                if (res == null)
                    res = r;
                if (Resources.ContainsKey(k)==false)
                {
                    Resources[k] = true;
                    Max1 = Math.Max(r.Array1, Max1);
                    Max2 = Math.Max(r.Array2, Max2);
                }
                else
                {
                    // cazul imposibil - teoretic in acest punct nu am o variabila care sa fie de mai multe ori
                }
            }
            public string GetVariableNameDefinition()
            {
                if ((Max1 >= 0) && (Max2 >= 0))
                    return Project.GetVariableName(res.Name, Max1 + 1, Max2 + 1);
                if ((Max1 >=0 ) && (Max2 == -1))
                    return Project.GetVariableName(res.Name, Max1 + 1, -1);
                return Project.GetVariableName(res.Name, -1, -1);
            }
            public int GetElementsCount()
            {
                if ((Max1>=0) && (Max2>=0))
                    return (Max1+1)*(Max2+1);
                if ((Max1>=0) && (Max2<0))
                    return (Max1+1);
                return 1;
            }
        };
        public class ResourcesByLanguage
        {            
            public Dictionary<Language, List<GenericResource>> Lang = new Dictionary<Language,List<GenericResource>>();
            public Type ResourceType = null;
            public string Name = "";
            public int Index = 0;
        };
        public Dictionary<Type, int> ResourcesCount = new Dictionary<Type, int>();
        public Dictionary<Type, int> VariablesCount = new Dictionary<Type, int>();
        public List<GenericResource> List = new List<GenericResource>();
        public Dictionary<string, ResourcesByLanguage> Resources = new Dictionary<string, ResourcesByLanguage>();
        public Dictionary<Type, Dictionary<string, ArrayCounter>> Array = new Dictionary<Type, Dictionary<string, ArrayCounter>>();
        public void Add(GenericResource gr)
        {
            List.Add(gr);
            if (VariablesCount.ContainsKey(gr.GetType()) == false)
            {
                VariablesCount[gr.GetType()] = 0;
                ResourcesCount[gr.GetType()] = 0;
            }
            gr.ResourceIndex = ResourcesCount[gr.GetType()];
            ResourcesCount[gr.GetType()]++;
            string k = gr.GetResourceVariableKey();
            if (Resources.ContainsKey(k) == false)
            {
                Resources[k] = new ResourcesByLanguage();
                Resources[k].ResourceType = gr.GetType();
                Resources[k].Name = gr.Name;
                Resources[k].Index = VariablesCount[gr.GetType()];
                VariablesCount[gr.GetType()]++;
            }
            if (Resources[k].Lang.ContainsKey(gr.Lang)==false)
                Resources[k].Lang[gr.Lang] = new List<GenericResource>();
            Resources[k].Lang[gr.Lang].Add(gr);         
            if (Array.ContainsKey(gr.GetType()) == false)
                Array[gr.GetType()] = new Dictionary<string, ArrayCounter>();            
            if (Array[gr.GetType()].ContainsKey(gr.Name) == false)
                Array[gr.GetType()][gr.Name] = new ArrayCounter();
            ArrayCounter ac = Array[gr.GetType()][gr.Name];
            ac.Add(gr);
        }
    }

    [XmlInclude(typeof(DevelopBuildConfiguration))]
    [XmlInclude(typeof(AndroidBuildConfiguration))]
    [XmlInclude(typeof(IOSBuildConfiguration))]
    [XmlInclude(typeof(MacBuildConfiguration))]
    [XmlInclude(typeof(WindowsDesktopBuildConfiguration))]
    [XmlType("BuildConfiguration"), XmlRoot("BuildConfiguration")]
    public class GenericBuildConfiguration
    {
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
        public string Market = "";
        [XmlAttribute()]
        public string Webpage = "";
        [XmlAttribute()]
        public string DeveloperMarket = "";
        [XmlAttribute()]
        public string Defines = "";
        [XmlAttribute()]
        public bool EnableErrorLogging = false, EnableEventLogging = false, EnableInfoLogging = false;
        [XmlAttribute()]
        public BuildImageCreateMethod ImageCreateMethod = BuildImageCreateMethod.ResizeAtStartup;
        [XmlAttribute()]
        public String Icon = "";
        [XmlAttribute()]
        public SnapshotTypeMethod SnapshotType = SnapshotTypeMethod.OnlyGlobalData;
        [XmlAttribute()]
        public String BuildLanguages = "";
        [XmlAttribute()]
        public bool AllAvailableLanguages = true;
        [XmlAttribute()]
        public int AlarmCheckUpdateSeconds = 60; // secunde

        public StringValues ApplicationName = new StringValues();

        [XmlIgnore()]
        public List<GenericAd> Ads = new List<GenericAd>();
        [XmlIgnore()]
        public BuildResources R = new BuildResources();
        [XmlIgnore()]
        public Dictionary<Size, bool> AvailableResolutions = new Dictionary<Size, bool>();

        #region Internal vars
        protected Project prj;
        protected ITaskNotifier task;
        protected string root;
        protected BuildExtension extension = null;
        private static Dictionary<ResourcesConstantType, string> resConstTypeCppRepresentation = new Dictionary<ResourcesConstantType, string>()
            {
                {ResourcesConstantType.Presentation,    "GApp::Resources::Presentation*"},
                {ResourcesConstantType.Font,            "GApp::Resources::Font*"},
                {ResourcesConstantType.Image,           "GApp::Resources::Bitmap*"},
                {ResourcesConstantType.Raw,             "GApp::Resources::RawResource*"},
                {ResourcesConstantType.Shader,          "GApp::Resources::Shader*"},
                {ResourcesConstantType.Sound,           "GApp::Resources::Sound*"},
                {ResourcesConstantType.String,          "UnicodeString"},
            };
        #endregion

        #region Atribute
        [XmlIgnore(), Description("Build Name"), Category("General"), DisplayName("Build Name")]
        public string _Name
        {
            get { return Name; }
            set {
                if (GetType() == typeof(DevelopBuildConfiguration))
                {
                    MessageBox.Show("Develop build can not be changed !");
                }
                else
                {
                    if (Project.ValidateVariableNameCorectness(value) == true)
                    {
                        Name = value;
                    }
                    else
                    {
                        MessageBox.Show("Invalid name - Build name can only contain the following characters: A-Z, a-z, 0-9 and '_");
                    }
                }
            }
        }
        [XmlIgnore(), Description("Icon source file for the current build !"), Category("General"), DisplayName("Icon")]
        public string _Icon
        {
            get { return Icon; }
        }
        [XmlIgnore(), Description("Operation system"), Category("General"), DisplayName("OS")]
        public OSType _OS
        {
            get { return GetOS(); }
        }
        [XmlIgnore(), Description("Specifies how snapshot should be made.\n'OnlyGlobalData' means that only persisten variables from Global will be saved.\n'Full' means that all persisten variables will be saved."), Category("General"), DisplayName("Snapshot type")]
        public SnapshotTypeMethod _SnapshotType
        {
            get { return SnapshotType; }
            set { SnapshotType = value; }
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
        [XmlIgnore(), Description("Market"), Category("Social network"), DisplayName("App Market address"), Editor(typeof(MarketStringEditor), typeof(UITypeEditor))]
        public string _Market
        {
            get { return Market; }
            set { Market = value; }
        }
        [XmlIgnore(), Description("Market"), Category("Social network"), DisplayName("Developer Market address"), Editor(typeof(MarketStringEditor), typeof(UITypeEditor))]
        public string _DevMarket
        {
            get { return DeveloperMarket; }
            set { DeveloperMarket = value; }
        }
        [XmlIgnore(), Description("Webpage"), Category("Social network"), DisplayName("Webpage address")]
        public string _Webpage
        {
            get { return Webpage; }
            set { Webpage = value; }
        }
        [XmlIgnore(), Description("Enable/Disable error logging"), Category("Debug"), DisplayName("Error logging")]
        public virtual bool _EnableErrorLogging
        {
            get { return EnableErrorLogging; }
            set { EnableErrorLogging = value; }
        }
        [XmlIgnore(), Description("Enable/Disable event logging"), Category("Debug"), DisplayName("Event logging")]
        public virtual bool _EnableEventLogging
        {
            get { return EnableEventLogging; }
            set { EnableEventLogging = value; }
        }
        [XmlIgnore(), Description("Enable/Disable information logging"), Category("Debug"), DisplayName("Information logging")]
        public virtual bool _EnableInfoLogging
        {
            get { return EnableInfoLogging; }
            set { EnableInfoLogging = value; }
        }

        [XmlIgnore(), Description("Specifies how the image should be created.\nDynamic means that every image will be resize dynamically.\nResizeAtStartup means that the images will be resized at startup.\nCustomizeForEachImage means that each image will be resize according to its 'Resize Method' property."), Category("Video Card"), DisplayName("Image Create Method")]
        public virtual BuildImageCreateMethod _ImageCreateMethod
        {
            get { return ImageCreateMethod; }
            set { ImageCreateMethod = value; }
        }

        [XmlIgnore(), Description("Time spam to check the alarms (in seconds). Must be at least one second."), Category("Alarms"), DisplayName("Alarm re-check (sec)")]
        public int _AlarmCheckUpdateSeconds
        {
            get { return AlarmCheckUpdateSeconds; }
            set { if (value >= 1) AlarmCheckUpdateSeconds = value; }
        }
        #endregion

        #region Functii interne
        public void Prepare(Project p, ITaskNotifier t)
        {
            prj = p;
            task = t;
            root = Path.Combine(prj.ProjectPath, "Builds", Name);
        }
        protected void EnableAttr(string name, bool value)
        {
            PropertyDescriptor descriptor = TypeDescriptor.GetProperties(this.GetType())[name];
            ReadOnlyAttribute attribute = (ReadOnlyAttribute)descriptor.Attributes[typeof(ReadOnlyAttribute)];
            FieldInfo fieldToChange = attribute.GetType().GetField("isReadOnly", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            fieldToChange.SetValue(attribute, !value);
        }
        protected string GenerateProfileHeaderDefinition()
        {
            string s = "";
            int count = 0;
            ArrayCounter ac = new ArrayCounter();
            foreach (Profile p in prj.Profiles)
            {
                ac.Add(p.Name, p.Array1, p.Array2);
            }

            foreach (string varName in ac.Variables)
            {
                s += string.Format("\tGApp::Resources::Profile* {0};\n", ac.GetVariableName(varName));
                int a1 = ac.GetArray1(varName);
                int a2 = ac.GetArray2(varName);
                if (a1<=0)
                {
                    count++;
                }
                else
                {
                    if (a2 <= 0)
                        count += a1;
                    else
                        count += a1 * a2;
                }
            }
            s += string.Format("\tGApp::Resources::Profile  List[{0}];\n", count);
            return s;
        }
        protected string GenerateHeaderDefinitions(string varType, Type tip, out int resCount)
        {
            string s = "";
            int count = 0;
            resCount = 0;
            if (R.Array.ContainsKey(tip)==false)
                return "";
            foreach (string varName in R.Array[tip].Keys)
            {
                s += string.Format("\t{0}* {1};\n", varType, R.Array[tip][varName].GetVariableNameDefinition());
                count += R.Array[tip][varName].GetElementsCount();
            }
            if (count>0)
                s += string.Format("\t{0} List[{1}];\n", varType, count);
            resCount = count;
            return s;
        }
        protected string GenerateHeaderDefinitionsForFonts(string varType)
        {
            int count = 0;
            string s = GenerateHeaderDefinitions(varType, typeof(FontResource),out count);
            foreach (GenericResource r in R.List)
            {
                if (r.GetType() != typeof(FontResource))
                    continue;
                FontResource f = (FontResource)r;
                foreach (Glyph g in f.Glyphs)
                {
                    s += string.Format("\tGApp::Resources::Bitmap {0};\n", f.GetGlyphImageVariableName(g));
                }
            }
            return s;
        }
        protected string GenerateShaderDefinitions()
        {
            string s = "";
            if (R.Array.ContainsKey(typeof(ShaderResource)) == false)
                return "";
            foreach (string varName in R.Array[typeof(ShaderResource)].Keys)
            {
                s += string.Format("\t{0}* {1};\n", "WrapperClass" + varName + "Type", R.Array[typeof(ShaderResource)][varName].GetVariableNameDefinition());
                s += string.Format("\t{0}  {1};\n", "WrapperClass" + varName + "Type", "__Linked__" + R.Array[typeof(ShaderResource)][varName].GetVariableNameDefinition());
            }
            return s;
        }
        protected string GenerateShaderWrapperClassesHeaders()
        {
            string s = "";
            Dictionary<string, bool> d = new Dictionary<string,bool>();
            foreach (GenericResource r in R.List)
            {
                if (r.GetType() != typeof(ShaderResource))
                    continue;
                if (d.ContainsKey(r.Name))
                    continue;
                s += "class WrapperClass" + r.Name + "Type: public GApp::Resources::Shader {\n\tpublic:\n\tbool OnUpdate();\n";
                ShaderResource sr = (ShaderResource)r;
                foreach (ShaderVariable sv in sr.Uniforms)
                {
                    switch (sv.Type)
                    {
                        case ShaderVariableType.Color:
                            s+="\t\tunsigned int "+sv.Name+";\n";
                            break;
                        case ShaderVariableType.Float:
                            s += "\t\tfloat " + sv.Name + ";\n";
                            break;
                        case ShaderVariableType.ColorChannel:
                            s += "\t\tint " + sv.Name + ";\n";
                            break;
                    }
                }                
                s+="};\n";
            }
            return s;
        }
        protected string GenerateShaderWrapperClassesCode()
        {
            string s = "";
            Dictionary<string, bool> d = new Dictionary<string, bool>();
            foreach (GenericResource r in R.List)
            {
                if (r.GetType() != typeof(ShaderResource))
                    continue;
                if (d.ContainsKey(r.Name))
                    continue;
                s += "bool WrapperClass" + r.Name + "Type::OnUpdate() {\n";
                ShaderResource sr = (ShaderResource)r;
                for (int tr = 0; tr < sr.Uniforms.Count;tr++ )
                {
                    switch (sr.Uniforms[tr].Type)
                    {
                        case ShaderVariableType.Color:
                            s += String.Format("\tSetVariableColor({0},{1},{2});\n", tr, sr.Uniforms[tr].Name, sr.Uniforms[tr].ClearAfterUsage.ToString().ToLower());
                            break;
                        case ShaderVariableType.Float:
                            s += String.Format("\tSetVariableValue({0},{1},{2});\n", tr, sr.Uniforms[tr].Name, sr.Uniforms[tr].ClearAfterUsage.ToString().ToLower());
                            break;
                        case ShaderVariableType.ColorChannel:
                            s += String.Format("\t{0}=MAX(255,{0});\n\t{0}=MIN(0,{0});\n", sr.Uniforms[tr].Name);
                            s += String.Format("\tSetVariableValue({0},(float)({1})/255.0f,{2});\n", tr, sr.Uniforms[tr].Name, sr.Uniforms[tr].ClearAfterUsage.ToString().ToLower());
                            break;
                    }
                }
                s += "\treturn(true);\n";
                s += "};\n";
            }
            return s;
        }
        protected Dictionary<string, GenericAd> GetAdsForCurrentBuild()
        {
            List<GenericAd> ads = null;
            if (this.GetType() == typeof(DevelopBuildConfiguration))
            {
                if (((DevelopBuildConfiguration)this).BuildToUseForCodeSettings == null)
                    ads = prj.GetAdsUsedInBuild(this);
                else
                    ads = prj.GetAdsUsedInBuild(((DevelopBuildConfiguration)this).BuildToUseForCodeSettings);
            }
            else
                ads = prj.GetAdsUsedInBuild(this);
            Dictionary<string, GenericAd> d = new Dictionary<string, GenericAd>();
            if ((ads == null) || (ads.Count == 0))
                return d;           
            foreach (GenericAd ad in ads)
            {
                if (d.ContainsKey(ad.Name))
                    prj.EC.AddError("Ad '" + ad.Name + "' is defined multipe times in current build !");
                d[ad.Name] = ad;
            }
            return d;
        }
        protected string GenerateAdsHeaderDefinition(Dictionary<string, GenericAd> ads)
        {
            string s = "";

            if ((ads == null) || (ads.Count == 0))
                return s;
            foreach (string adName in ads.Keys)
            {
                s += string.Format("\tGApp::Platform::AdInterface* {0};\n", adName);
            }
            s += string.Format("\tGApp::Platform::AdInterface  List[{0}];\n", ads.Count);
            return s;
        }
        protected string GenerateAdsInitCode(Dictionary<string, GenericAd> ads)
        {
            string s = "";

            if ((ads == null) || (ads.Count == 0))
                return s;
            int tr = 0;
            foreach (string adName in ads.Keys)
            {
                s += string.Format("\tAds.{0} = &Ads.List[{1}];\n", adName, tr);
                s += string.Format("\tCHECK(Ads.{0}->Init(Context,{1},{2},GAC_ADTYPE_{3}),false,\"\");\n", adName, tr, ads[adName].LoadOnStartup.ToString().ToLower(), ads[adName].GetAdTtype().ToString().ToUpper());
                tr++;
            }
            return s;
        }
        protected string GenerateAdsGetterCode()
        {
            List<GenericAd> ads = null;
            if (this.GetType() == typeof(DevelopBuildConfiguration))
            {
                if (((DevelopBuildConfiguration)this).BuildToUseForCodeSettings == null)
                    ads = prj.GetAdsUsedInBuild(this);
                else
                    ads = prj.GetAdsUsedInBuild(((DevelopBuildConfiguration)this).BuildToUseForCodeSettings);
            } else
                ads = prj.GetAdsUsedInBuild(this);
            if ((ads == null) || (ads.Count == 0))
                return "return NULL;";
            return "if (index < "+ads.Count.ToString()+") return &Ads.List[index]; else return NULL;";
        }

        private string ConvertResourceNameToCString(string s)
        {
            if (String.IsNullOrEmpty(s))
                return "NULL";
            return "\"" + s + "\"";
        }

        protected string GenerateProfileInitCode()
        {
            int index = 0;
            string s = "";
            foreach (Profile p in prj.Profiles)
            {
                string vname = Project.GetVariableName(p.Name, p.Array1, p.Array2);
                Dictionary<string, string> profileResources = p.GetAllResources();
                s += string.Format("\tProfiles.{0} = &Profiles.List[{1}];\n", vname, index);
                if ((EnableErrorLogging) || (EnableInfoLogging))
                    s += string.Format("\tProfiles.{0}->Name = \"{1}\";\n", vname, vname);
                
                s += p.CreateInitializationCode(prj,R,Name);
                s += "\n";
                index = index + 1;
            }
            return s;
        }

        private string GetVariableNameSetUpString(string className,GenericResource r)
        {
            return "" + className + "." + r.GetResourceVariableName() + "->Name=\"" + r.GetResourceVariableName() + "\";\n\t";
        }
        private string GenerateInitializationCodeForImages(BuildResources.ResourcesByLanguage RL, Language l, bool resourceHasName)
        {

            string s = "\tImages." + RL.Lang[l][0].GetResourceVariableName() + " = &Images.List[" + RL.Index.ToString() + "];\n\t";
            // pregatesc vectorul
            int poz = 0;
            foreach (GenericResource gr in RL.Lang[l])
            {
                ImageResource r = (ImageResource)gr;
                if (r.Picture == null)
                {
                    prj.EC.AddError("ImageResource::GenerateInitializationCodeForImages", string.Format("Image was not loaded for {0}, for resolution {1} and language {2} !", r.GetResourceVariableName(),r.DesignResolution,r.Lang));
                    return "";
                }
                if (r.ResourceFilePosition < 0)
                {
                    prj.EC.AddError("ImageResource::GenerateInitializationCodeForImages", string.Format("Position in resource is undefined (-1) for {0}, for resolution {1} and language {2} !", r.GetResourceVariableName(), r.DesignResolution, r.Lang));
                    return "";
                }
                if (r.ResourceFileSize < 0)
                {
                    prj.EC.AddError("ImageResource::GenerateInitializationCodeForImages", string.Format("Size of content for resource is undefined (-1) for {0}, for resolution {1} and language {2} !", r.GetResourceVariableName(), r.DesignResolution, r.Lang));
                    return "";
                }
                if (r.ResourceFileSize > 0xFFFFFF)
                {
                    prj.EC.AddError("ImageResource::GenerateInitializationCodeForImages", string.Format("Size of content for resource is too larger (bigger than 0xFFFFFF) for {0}, for resolution {1} and language {2} !", r.GetResourceVariableName(), r.DesignResolution, r.Lang));
                    return "";
                }
                int design_w = 0, design_h = 0;
                if (Project.SizeToValues(r.DesignResolution, ref design_w, ref design_h) == false)
                {
                    prj.EC.AddError("ImageResource::GenerateInitializationCodeForImages", string.Format("Design resolution is not set for for {0}. Current value is '{2}'. Language is {3} !", r.GetResourceVariableName(), r.DesignResolution, r.Lang));
                    return "";
                }
                // datele sunt ok - le adaugam in buffer
                // formatul este:
                // * rezolutie - w,h
                // * dimensiune imagine - w,h
                // * de unde din resurser - offset,[size+resize_method]
                s += string.Format("tmp[{0}]={1};tmp[{2}]={3};tmp[{4}]={5};tmp[{6}]={7};tmp[{8}]={9};tmp[{10}]={11};\n\t", poz, design_w, poz + 1, design_h, poz + 2, r.Picture.Width, poz + 3, r.Picture.Height, poz + 4, r.ResourceFilePosition, poz + 5, (((uint)r.ResourceFileSize) & 0xFFFFFF)|((uint)r.ResizeMethod));
                poz += 6;
                if (poz>32)
                {
                    prj.EC.AddError("unsigned int tmp[32*6] = maximum allowd for image analisys - curently is " + poz.ToString() + ". This is an internal error !!!!");
                    return "";
                }
            }
            if (resourceHasName)
                s += GetVariableNameSetUpString("Images", RL.Lang[l][0]);
            s += string.Format("\tCHECK(Images.{0}->Create(/*Context,*/((GApp::UI::CoreSystem*)Context)->Width,((GApp::UI::CoreSystem*)Context)->Height,tmp,{1}),false,\"Failed to initialized image {0}\");\n\t", RL.Lang[l][0].GetResourceVariableName(), poz/6);          
           // s += "CHECK(Images." + GetResourceVariableName() + string.Format("->Create(Context,{0},{1},{2},{3},((GApp::UI::CoreSystem*)Context)->Width,((GApp::UI::CoreSystem*)Context)->Height,0,{4},{5})", Picture.Width, Picture.Height, design_w, design_h, ResourceFilePosition, ResourceFileSize) + ",false,\"\");";

            return s+"\n";
        }
        private string GenerateInitializationCodeForFonts(BuildResources.ResourcesByLanguage RL, Language l, bool resourceHasName)
        {
            if (RL.Lang[l].Count != 1)
            {
                prj.EC.AddError("Font resources don't have support for multiple variation ! (Resource: " + RL.Name + ")");
                return "";
            }
            FontResource r = (FontResource)RL.Lang[l][0];
            string s = "\tFonts." + r.GetResourceVariableName() + " = &Fonts.List[" + RL.Index.ToString() + "];\n\t";

            if (r.Glyphs.Count == 0)
            {
                prj.EC.AddError("GlyphFontResource::CreateInitializationCode", "No images added to the font " + r.GetResourceVariableName());
                return "";
            }
            Glyph width = null;
            foreach (Glyph g in r.Glyphs)
            {
                if (g.Code == r.WidthCharacter)
                    width = g;
            }
            if (width == null)
            {
                prj.EC.AddError("GlyphFontResource::CreateInitializationCode", "No Glyph selected for width refernce in " + r.GetResourceVariableName());
                return "";
            }
            s += string.Format("\n\tCHECK(Fonts.{0}->Create({1}),false,\"\");\n", r.GetResourceVariableName(), r.Glyphs.Count);
            int count = 0;
            foreach (Glyph g in r.Glyphs)
            {
                // fac imaginea
                int poz = 0;
                Glyph.GlyphVersionInfo gi = g.GetVersion();
                if (gi.Builds.ContainsKey(this.Name.ToLower()) == false) // imaginea nu e disponibila
                    continue;
                List<string> resolutions = gi.Builds[this.Name.ToLower()];
                // datele sunt ok - le adaugam in buffer
                // formatul este:
                // * rezolutie - w,h
                // * dimensiune imagine - w,h
                // * de unde din resurser - offset,[size+resize_method]
                foreach (string res in resolutions)
                {
                    Size sz = Project.SizeToValues(res);
                    if (g.PicturesForResolutions.ContainsKey(res)==false)
                    {
                        prj.EC.AddError("Unable to find picture key for resolution "+res+" - for character: "+g.GetCharString()+". This is an internal error !!!!");
                        return "";
                    }
                    Glyph.ImageInfo ii = g.PicturesForResolutions[res];
                    if (ii.Picture==null)
                    {
                        prj.EC.AddError("Glyph image for character "+g.GetCharString()+" for resolution '"+res+"' was not loaded !");
                        return "";
                    }
                    if ((ii.ResourceFilePoz<0) || (ii.ResourceSize<=0))
                    {
                        prj.EC.AddError("Glyph image for character " + g.GetCharString() + " for resolution '" + res + "' was not added to the resource buffer for font '"+r.GetResourceVariableName()+"'. This is an internal error !!!!\nIf you are running from GACCreator and using a specific build, make sure that all font are availabel for the same resolutions as Develop Mode.");
                        return "";
                    }

                    s += string.Format("\ttmp[{0}]={1};tmp[{2}]={3};tmp[{4}]={5};tmp[{6}]={7};tmp[{8}]={9};tmp[{10}]={11};\n", poz, sz.Width, poz + 1, sz.Height, poz + 2, ii.Picture.Width, poz + 3, ii.Picture.Height, poz + 4, ii.ResourceFilePoz, poz + 5, ii.ResourceSize & 0xFFFFFF); //(((uint)r.ResourceFileSize) & 0xFFFFFF) | ((uint)r.ResizeMethod));
                    poz += 6;
                    if (poz > 32)
                    {
                        prj.EC.AddError("unsigned int tmp[32*6] = maximum allowd for image analisys - curently is " + poz.ToString() + ". This is an internal error !!!!");
                        return "";
                    }
                }
                s += string.Format("\tCHECK(Fonts.{0}.Create(/*Context,*/((GApp::UI::CoreSystem*)Context)->Width,((GApp::UI::CoreSystem*)Context)->Height,tmp,{1}),false,\"Failed to initialized image {0}\");\n", r.GetGlyphImageVariableName(g), poz / 6);          
                // o atasez la font
                s += string.Format("\tCHECK(Fonts.{0}->Add({1},&Fonts.{2},{3}),false,\"\");\n",r.GetResourceVariableName(), g.Code, r.GetGlyphImageVariableName(g), g.BaseLine);
                count++;
            }
            s += string.Format("\tCHECK(Fonts.{0}->Prepare({1},{2},{3}),false,\"\");\n\t", r.GetResourceVariableName(), r.WidthCharacter, r.SpaceWidth, r.SpaceBetweenChars);

            if (resourceHasName)
                s += GetVariableNameSetUpString("Fonts", r);
            return s;

        }
        private string GenerateInitializationCodeForSounds(BuildResources.ResourcesByLanguage RL, Language l, bool resourceHasName)
        {
            if (RL.Lang[l].Count != 1)
            {
                prj.EC.AddError("Sound resources don't have support for multiple variation ! (Resource: " + RL.Name + ")");
                return "";
            }
            SoundResource r = (SoundResource)RL.Lang[l][0];
            string s = "\tSounds." + r.GetResourceVariableName() + " = &Sounds.List[" + RL.Index.ToString() + "];\n";
            s += string.Format("\tCHECK(Sounds.{0}->Create(Context,{1},{2},{3}),false,\"\");\n\t", r.GetResourceVariableName(), r.ResourceIndex, r.ResourceFilePosition, r.ResourceFileSize);
            if (resourceHasName)
                s += GetVariableNameSetUpString("Sounds", r);
            return s;
        }
        private string GenerateInitializationCodeForRaw(BuildResources.ResourcesByLanguage RL, Language l, bool resourceHasName)
        {
            if (RL.Lang[l].Count!=1)
            {
                prj.EC.AddError("Raw resources don't have support for multiple variation ! (Resource: "+RL.Name+")");
                return "";
            }
            RawResource r = (RawResource)RL.Lang[l][0];
            string s = "\tRawData." + r.GetResourceVariableName() + " = &RawData.List[" + RL.Index.ToString() + "];\n";
            s += string.Format("\tCHECK(RawData.{0}->Create(Context,{1},{2},{3}),false,\"\");\n\t", r.GetResourceVariableName(), r.ResourceFilePosition, r.ResourceFileSize, r.LocalCopy.ToString().ToLower());
            if (resourceHasName)
                s += GetVariableNameSetUpString("RawData",r);
            return s;
        }
        private string GenerateInitializationCodeForShader(BuildResources.ResourcesByLanguage RL, Language l, bool resourceHasName)
        {
            if (RL.Lang[l].Count != 1)
            {
                prj.EC.AddError("Shader resources don't have support for multiple variation ! (Resource: " + RL.Name + ")");
                return "";
            }
            ShaderResource r = (ShaderResource)RL.Lang[l][0];
            string s = "\tShaders." + r.GetResourceVariableName() + " = &Shaders.__Linked__" + r.GetResourceVariableName() + ";\n\t";
            s += string.Format("CHECK(Shaders.{0}->Create(Context,{1},{2}),false,\"\");\n\t", r.GetResourceVariableName(), r.ResourceFilePosition, r.ResourceFileSize);
            if (resourceHasName)
                s += GetVariableNameSetUpString("Shaders",r);
            return s;
        }
        private string GenerateInitializationCodeForPresentation(BuildResources.ResourcesByLanguage RL, Language l, bool resourceHasName)
        {
            if (RL.Lang[l].Count != 1)
            {
                prj.EC.AddError("Presentation resources don't have support for multiple variation ! (Resource: " + RL.Name + ")");
                return "";
            }
            PresentationResource r = (PresentationResource)RL.Lang[l][0];
            string ss = "\nPresentations." + r.GetResourceVariableName() + " = &Presentations.List[" + RL.Index.ToString() + "];\n\t";
            if (r.anim == null)
            {
                prj.EC.AddError("Presentation was not loaded for " + r.GetResourceVariableName());
                return ss;
            }
            ResourcesIndexes ri = r.anim.CreateResourcesIndexes();
            ss += string.Format("CHECK(Presentations.{0}->Create(Context,{1},{2},{3},{4},{5},{6},{7}),false,\"\");\n\t", r.GetResourceVariableName(), r.ResourceFilePosition, r.ResourceFileSize, ri.ImageIndex.Count, ri.FontsIndex.Count, ri.ShaderIndex.Count, ri.StringIndex.Count,ri.StringValueIndex.Count);
            foreach (string name in ri.ImageIndex.Keys)
                ss += string.Format("CHECK(Presentations.{0}->SetImage({1},Images.{2},{3}),false,\"\");\n\t", r.GetResourceVariableName(), ri.ImageIndex[name].Index, name, ConvertResourceNameToCString(ri.ImageIndex[name].Name));       
            foreach (string name in ri.FontsIndex.Keys)
                ss += string.Format("CHECK(Presentations.{0}->SetFont({1},Fonts.{2},{3}),false,\"\");\n\t", r.GetResourceVariableName(), ri.FontsIndex[name].Index, name, ConvertResourceNameToCString(ri.FontsIndex[name].Name));
            foreach (string name in ri.StringIndex.Keys)
                ss += string.Format("CHECK(Presentations.{0}->SetString({1},Strings.{2},{3}),false,\"\");\n\t", r.GetResourceVariableName(), ri.StringIndex[name].Index, name, ConvertResourceNameToCString(ri.StringIndex[name].Name));
            foreach (string name in ri.ShaderIndex.Keys)
                ss += string.Format("CHECK(Presentations.{0}->SetShader({1},Shaders.{2},{3}),false,\"\");\n\t", r.GetResourceVariableName(), ri.ShaderIndex[name].Index, name, ConvertResourceNameToCString(ri.ShaderIndex[name].Name));
            foreach (string name in ri.StringValueIndex.Keys)
                ss += string.Format("CHECK(Presentations.{0}->CreateStringValue(\"{1}\",{2}),false,\"\");\n\t", r.GetResourceVariableName(), name, ri.StringValueIndex[name]);
            if (resourceHasName)
                ss += GetVariableNameSetUpString("Presentations", r);
            return ss;
        }
        private string GenerateInitializationCode(BuildResources.ResourcesByLanguage RL, Language l, bool resourceHasName)
        {
            if (RL.ResourceType == typeof(ImageResource)) 
                return GenerateInitializationCodeForImages(RL,l, resourceHasName);
            if (RL.ResourceType == typeof(FontResource)) 
                return GenerateInitializationCodeForFonts(RL,l, resourceHasName);
            if (RL.ResourceType == typeof(SoundResource)) 
                return GenerateInitializationCodeForSounds(RL,l, resourceHasName);
            if (RL.ResourceType == typeof(RawResource)) 
                return GenerateInitializationCodeForRaw(RL,l, resourceHasName);
            if (RL.ResourceType == typeof(ShaderResource)) 
                return GenerateInitializationCodeForShader(RL,l, resourceHasName);
            if (RL.ResourceType == typeof(PresentationResource)) 
                return GenerateInitializationCodeForPresentation(RL,l, resourceHasName);
            prj.EC.AddError("Unable to add initialization code for resources of type " + RL.ResourceType.ToString());
            return "";
        }
        protected string GenerateSourceInitCode(Type t)
        {
            string s = "";
            foreach (string k in R.Resources.Keys)
            {
                BuildResources.ResourcesByLanguage RL = R.Resources[k];
                if (RL.ResourceType != t)
                    continue;
                if (RL.Lang.Count==1)
                {
                    // foreach-ul e formal - avem un singur element
                    foreach (Language l in RL.Lang.Keys)
                        s += GenerateInitializationCode(RL, l, ((EnableErrorLogging) || (EnableInfoLogging)))+"\n";
                }
                else
                {
                    s += "\tswitch (Lang) {\n";
                    foreach (Language l in RL.Lang.Keys)
                    {
                        if (l!=prj.DefaultLanguage)
                        {
                            s += "\t\tcase GAC_LANGUAGE_" + l.ToString().ToUpper() + ":\n";
                            s += GenerateInitializationCode(RL, l, ((EnableErrorLogging) || (EnableInfoLogging)))+"\n";
                            // genereaza codul pentru RL.Lang[l]
                            s += "\t\t\tbreak;\n";
                        }
                    }
                    // pun si default
                    s += "\t\tdefault:\n";
                    // genereaza codul pentru RL.Lang[prj.DefaultLanguage]
                    if (RL.Lang.ContainsKey(prj.DefaultLanguage))
                    {
                        s += GenerateInitializationCode(RL, prj.DefaultLanguage, ((EnableErrorLogging) || (EnableInfoLogging)))+"\n";
                    }
                    else
                    {
                        prj.EC.AddError("Resource " + RL.Name + " does not have any form for the default language: " + prj.DefaultLanguage.ToString());
                    }
                    s += "\t\t\tbreak;\n";
                    s += "\t}\n";
                }
            }
            return s;
        }
        protected string GenerateStringsDefinitions()
        {
            string s = "";
            ArrayCounter ac = new ArrayCounter();
            foreach (StringValues sv in prj.Strings)
                ac.Add(sv.VariableName, sv.Array1, sv.Array2);

            foreach (string varName in ac.Variables)
            {
                s += "\tUnicodeString " + ac.GetVariableName(varName) + ";\n";
            }
            return s;
        }
        protected string GenerateStringsInitCode()
        {
            string ss = "";

            Dictionary<Language, bool> buildLanguages = prj.GetBuildAvailableLanguages(this, true);
            Dictionary<Language, string> d = new Dictionary<Language, string>();
            foreach (StringValues sv in prj.Strings)
            {
                d.Clear();
                foreach (Language l in Enum.GetValues(typeof(Language)))
                {
                    if (buildLanguages.ContainsKey(l) == false)
                        continue;
                    string v = sv.Get(l);
                    if (v.Length == 0)
                        continue;
                    d[l] = v;
                }
                string varName = Project.GetVariableName(sv.VariableName, sv.Array1, sv.Array2);
                // am variabila
                if (d.ContainsKey(prj.DefaultLanguage) == false)
                {
                    prj.EC.AddError("Missing '" + prj.DefaultLanguage.ToString() + "' language configuration for " + varName);
                    return ss;
                }
                if (d.Keys.Count == 1)
                {
                    ss += String.Format("\tStrings.{0} = (char *){1};\n", varName, Project.StringToFrameworkString(sv.Get(prj.DefaultLanguage)));
                }
                else
                {
                    if (d.Keys.Count > 1)
                        ss += "\tswitch (Lang)\n\t{\n";
                    foreach (Language lang in d.Keys)
                    {
                        if (lang == prj.DefaultLanguage)
                            continue;
                        ss += String.Format("\t\tcase GAC_LANGUAGE_{0}: Strings.{1} = (char *){2}; break;\n", lang.ToString().ToUpper(), varName, Project.StringToFrameworkString(sv.Get(lang)));
                    }
                    if (d.Keys.Count > 1)
                        ss += "\t\tdefault: Strings." + varName + " = (char *)" + Project.StringToFrameworkString(sv.Get(prj.DefaultLanguage)) + ";break;\n\t};\n";
                }
            }



            return ss;
        }
        private string GenerateEnumerationDefinitions()
        {
            string s = "\n";
            foreach (Enumeration en in prj.Enums)
            {
                foreach (EnumValue ev in en.Values)
                {
                    s += "\n#define GAC_ENUM_" + en.Name + "_" + ev.Name + " (" + Project.BasicTypeValueToCppValue(ev.Value, en.Type) + ")";
                }
            }
            return s + "\n";
        }
        private string GetCppValueConstantValueRepresentation(string type, string value, Dictionary<string, Dictionary<string, int>> d_linkID,bool UseResMacro)
        {
            string s = "";
            ConstantModeType mode = ConstantHelper.GetConstantMode(type);
            switch (mode)
            {
                case ConstantModeType.BasicTypes:
                    s = "("+Project.BasicTypeValueToCppValue(value, ConstantHelper.GetBasicTypesType(type)) + ")";
                    break;
                case ConstantModeType.Enumerations:
                    s = "(";
                    Enumeration E = prj.GetEnumeration(ConstantHelper.GetEnumerationType(type));
                    if (E != null)
                    {
                        if (E.IsBitSet)
                        {
                            s += " 0 ";
                            List<string> l = Project.StringListToList(value);
                            foreach (string bitValue in l)
                            {
                                EnumValue ev = E.FindValue(bitValue);
                                if (ev != null)
                                {
                                    s += "| (" + ev.Value + ")";
                                }
                                else
                                {
                                    s += "| (void) /*unknwon enumeration value: " + bitValue + " */";
                                }
                            }
                            s += " )";
                        }
                        else
                        {
                            EnumValue ev = E.FindValue(value);
                            if (ev != null)
                            {
                                s += ev.Value + " )";
                            }
                            else
                            {
                                s += "void) /*unknwon enumeration value: " + value + " */";
                            }
                        }
                    }
                    else
                    {
                        s += "void) /*unknwon enumeration : " + ConstantHelper.GetEnumerationType(type) + " */";
                    }
                    break;
                case ConstantModeType.Resources:
                    s = "(";
                    ResourcesConstantType rct = ConstantHelper.GetResourcesType(type);
                    if (value.Trim().Length == 0)
                    {
                        s += "NULL)";
                    }
                    else
                    {
                        if (UseResMacro)
                            s += "Res.";
                        switch (rct)
                        {
                            case ResourcesConstantType.Image: s += "Images." + value.Trim() + ")"; break;
                            case ResourcesConstantType.Presentation: s += "Presentations." + value.Trim() + ")"; break;
                            case ResourcesConstantType.Font: s += "Fonts." + value.Trim() + ")"; break;
                            case ResourcesConstantType.Raw: s += "RawData." + value.Trim() + ")"; break;
                            case ResourcesConstantType.Sound: s += "Sounds." + value.Trim() + ")"; break;
                            case ResourcesConstantType.Shader: s += "Shaders." + value.Trim() + ")"; break;
                            case ResourcesConstantType.String: s += "Strings." + value.Trim() + ")"; break;
                            default: s += "void) /*unknwon resource : " + value + " */"; break;
                        }
                    }
                    break;
                case ConstantModeType.DataTypes:
                    if (d_linkID == null)
                        break;
                    s = "(";
                    if (value.Trim().Length == 0)
                    {
                        s += "NULL)";
                    }
                    else
                    {
                        string dataType = ConstantHelper.GetDataTypesType(type);
                        if (d_linkID.ContainsKey(dataType) == false)
                        {
                            s += "void )// Unknwon data type: " + dataType;
                        }
                        else
                        {
                            if (d_linkID[dataType].ContainsKey(value) == false)
                            {
                                s += "void )// Unknwon link ID :" + value;
                            }
                            else
                            {
                                s += string.Format("&ConstantValues.__{0}__values__[{1}] )", dataType, d_linkID[dataType][value]);
                            }
                        }
                    }
                    break;
            }
            return s;
        }
        private string GenerateConstantsDefinitions()
        {
            string s = "\n";
            foreach (ConstantValue Const in prj.Constants)
            {
                if (Const.MatrixColumnsCount == 0)
                {
                    s += "\n#define GAC_CONSTANTS_" + Const.Name + " ";
                    s += GetCppValueConstantValueRepresentation(Const.Type, Const.Value, null, true);
                }
                else
                {
                    s += "\nextern const " + ConstantHelper.GetBasicTypeCppReprezentation(ConstantHelper.GetBasicTypesType(Const.Type)) + " GAC_CONSTANTS_" + Const.Name;
                    if (Const.MatrixColumnsCount==1)
                        s += "[]";
                    else
                        s += "[]["+Const.MatrixColumnsCount.ToString()+"]";
                    s += ";";
                }
                
            }
            return s + "\n";
        }
        private string GetCppDataTypeForConstant(string Type)
        {
            ConstantModeType cmt = ConstantHelper.GetConstantMode(Type);
            string typeName;
            string cppFormat = "";
            switch (cmt)
            {
                case ConstantModeType.BasicTypes:
                    cppFormat = ConstantHelper.GetBasicTypeCppReprezentation(ConstantHelper.GetBasicTypesType(Type));
                    break;
                case ConstantModeType.Resources:
                    ResourcesConstantType rct = ConstantHelper.GetResourcesType(Type);
                    if (resConstTypeCppRepresentation.ContainsKey(rct))
                        cppFormat = resConstTypeCppRepresentation[rct];
                    break;
                case ConstantModeType.DataTypes:
                    typeName = ConstantHelper.GetDataTypesType(Type);
                    cppFormat = "GAC_DATATYPE_" + typeName + "*";
                    break;
                case ConstantModeType.Enumerations:
                    typeName = ConstantHelper.GetEnumerationType(Type);
                    Enumeration e = prj.GetEnumeration(typeName);
                    if (e != null)
                    {
                        cppFormat = ConstantHelper.GetBasicTypeCppReprezentation(e.Type);
                    }
                    break;
            }
            return cppFormat;
        }
        private string GenerateDataTypesDefinitions()
        {
            string ss = "";

            foreach (Structure s in prj.Structs)
                ss += "class GAC_DATATYPE_" + s.Name + ";\n";
            foreach (Structure s in prj.Structs)
            {
                ss += "class GAC_DATATYPE_" + s.Name + " {\npublic:";
                ss += "\n\tGAC_DATATYPE_" + s.Name + "() { unsigned char* p = (unsigned char *)this;unsigned char* e = p+sizeof(GAC_DATATYPE_" + s.Name + ");while (p<e) { (*p)=0;p++; } }";
                foreach (StructureField sf in s.Fields)
                {
                    string cppFormat = GetCppDataTypeForConstant(sf.Type);
                    if (sf.List)
                        cppFormat += "*";
                    ss += "\n\t" + cppFormat + " " + sf.Name;
                    ss += ";";
                    if (sf.List)
                    {
                        ss += "\n\tint " + sf.Name+"Count;";
                    }
                }
                ss += "\n};\n";
            }
            return ss;
        }
        private string GenerateConstantValuesDefinitions()
        {
            string ss = "";
            ArrayCounter ac = new ArrayCounter();
            foreach (Structure dt in prj.Structs)
            {
                if (dt.Values.Count == 0)
                    continue;
                ss += "\tGAC_DATATYPE_" + dt.Name + " __"+dt.Name+"__values__["+dt.Values.Count.ToString()+"];\n";
                ac.Clear();
                foreach (StructureValue sv in dt.Values)
                    if (sv.Name.Length>0)
                        ac.Add(sv.Name, sv.Array1, sv.Array2);
                string[] vars = ac.Variables;
                foreach (string varName in vars)
                {
                    ss += "\tGAC_DATATYPE_" + dt.Name + " *" + ac.GetVariableName(varName) + ";\n";
                }
            }
            return ss;
        }
        private string GenerateConstantLinkIDLinksCode()
        {
            string ss = "";

            ArrayCounter ac = new ArrayCounter();
            foreach (Structure dt in prj.Structs)
            {
                if (dt.Values.Count == 0)
                    continue;
                ac.Clear();
                foreach (StructureValue sv in dt.Values)
                    if (sv.Name.Length > 0)
                        ac.Add(sv.Name, sv.Array1, sv.Array2);
                string[] vars = ac.Variables;
                // curat datele
                foreach (string varName in vars)
                {
                    int a1 = ac.GetArray1(varName);
                    int a2 = ac.GetArray2(varName);
                    if ((a1<=0) && (a2<=0))
                    {
                        ss += "\t\tConstantValues." + varName + " = NULL;\n";
                        continue;
                    }
                    if ((a1>0) && (a2<=0))
                    {
                        ss += string.Format("\t\tfor (int index=0;index<{0};index++) ConstantValues.{1}[index] = NULL;\n", a1, varName);
                        continue;
                    }
                    if ((a1 > 0) && (a2 > 0))
                    {
                        ss += string.Format("\t\tfor (int i1=0;i1<{0};i1++) {{ for (int i2=0;i2<{1};i2++) {{ ConstantValues.{2}[i1][i2] = NULL; }} }}\n", a1, a2, varName);
                        continue;
                    }
                }
                // fac linkurile
                for (int tr=0;tr<dt.Values.Count;tr++)
                {
                    if (dt.Values[tr].Name.Length == 0)
                        continue;
                    if (dt.Values[tr].IsNull)
                        continue;
                    ss += string.Format("\t\tConstantValues.{0} = &ConstantValues.__{1}__values__[{2}];\n", Project.GetVariableName(dt.Values[tr].Name, dt.Values[tr].Array1, dt.Values[tr].Array2), dt.Name, tr);
                }
            }
            // am creat toate linkurile posibile
            return ss;
        }
        private string GenerateConstantValuesInitCode()
        {
            string ss = GenerateConstantLinkIDLinksCode();
            ss += "\n";

            // setez toate valorile
            ArrayCounter ac = new ArrayCounter();
            Dictionary<string, Dictionary<string, int>> d_linkID = new Dictionary<string, Dictionary<string, int>>();
            // validez tipurile de date
            foreach (Structure dt in prj.Structs)
            {
                ac.Clear();
                Dictionary<string, int> linkIDs = new Dictionary<string, int>();
                for (int tr = 0; tr < dt.Values.Count; tr++)
                    linkIDs[dt.Values[tr].LinkID.ToString()] = tr;
                d_linkID[dt.Name] = linkIDs;
            }
            List<string> listValue = new List<string>();
            foreach (Structure dt in prj.Structs)
            {
                for (int gr=0;gr<dt.Values.Count;gr++)
                {
                    StructureValue sv = dt.Values[gr];
                    if (sv.IsNull)
                        continue;
                    for (int tr = 0; tr < dt.Fields.Count;tr++)
                    {
                        if (dt.Fields[tr].List)
                        {
                            Project.StringListToList(sv.FieldValues[tr],listValue,';',false);
                            ss += string.Format("\t\tConstantValues.__{0}__values__[{1}].{2}Count = {3};\n", dt.Name, gr, dt.Fields[tr].Name, listValue.Count);
                            if (listValue.Count==0)
                            {
                                ss += string.Format("\t\tConstantValues.__{0}__values__[{1}].{2} = NULL;\n", dt.Name, gr, dt.Fields[tr].Name);
                            }
                            else
                            {
                                ss += string.Format("\t\tConstantValues.__{0}__values__[{1}].{2} = new {3}[{4}];\n", dt.Name, gr, dt.Fields[tr].Name,GetCppDataTypeForConstant(dt.Fields[tr].Type),listValue.Count);
                                for (int hr=0;hr<listValue.Count;hr++)
                                    ss += string.Format("\t\tConstantValues.__{0}__values__[{1}].{2}[{3}] = {4};\n", dt.Name, gr, dt.Fields[tr].Name, hr, GetCppValueConstantValueRepresentation(dt.Fields[tr].Type, listValue[hr], d_linkID, false));
                            }
                        }
                        else
                        {
                            ss += string.Format("\t\tConstantValues.__{0}__values__[{1}].{2} = {3};\n", dt.Name, gr, dt.Fields[tr].Name, GetCppValueConstantValueRepresentation(dt.Fields[tr].Type, sv.FieldValues[tr], d_linkID,false));
                        }
                    }
                }
            }

            return ss;
        }
        private string GenerateConstantValuesArrayInitCode()
        {
            string s = "\n";
            foreach (ConstantValue Const in prj.Constants)
            {
                if (Const.MatrixColumnsCount == 0)
                    continue; // a fost definit ca si #define
                BasicTypesConstantType bct = ConstantHelper.GetBasicTypesType(Const.Type);
                s += "\nconst " + ConstantHelper.GetBasicTypeCppReprezentation(bct) + " GAC_CONSTANTS_" + Const.Name;
                if (Const.MatrixColumnsCount == 1)
                    s += "[]";
                else
                    s += "[][" + Const.MatrixColumnsCount.ToString() + "]";
                s += " = {";
                List<string> l;
                if (bct == BasicTypesConstantType.String)
                    l = Project.StringListToList(Const.Value, ';', false);
                else
                    l = Project.StringListToList(Const.Value);
                foreach (string val in l)
                {
                    s += Project.BasicTypeValueToCppValue(val, bct);
                    s += ",";
                }
                if (l.Count > 0)
                    s = s.Substring(0, s.Length - 1);
                s += "};";
            }
            return s + "\n";
        }
        private string GenerateCountersDefinitions(Dictionary<string,Counter> counters)
        {
            if (counters.Keys.Count == 0)
                return "";
            string s = "\n\tGApp::UI::Counter List[" + counters.Keys.Count.ToString() + "];\n";
            foreach (string name in counters.Keys)
            {
                s += "\tGApp::UI::Counter* " + name + ";\n";
            }
            return s + "\n";
        }
        private string GenerateCountersGroupsDefinitions(Dictionary<string, List<Counter>> groups)
        {
            if (groups.Keys.Count == 0)
                return "";
            int count = 0;
            string s = "\n";
            foreach (string name in groups.Keys)
            {
                if (groups[name].Count == 0)
                    continue;
                s += "\tGApp::UI::CountersGroup* " + name + ";\n";
                count++;
            }
            if (count == 0)
                return "";

            return s + "\tGApp::UI::CountersGroup List[" + count.ToString() + "];\n";
        }
        private string GenerateCountersInitCode(Dictionary<string, Counter> counters, Dictionary<string, GenericAd> ads)
        {
            if (counters.Keys.Count == 0)
                return "";
            string s = "\n";
            string scene_id = "";
            foreach (string name in counters.Keys)
            {
                Counter gc = counters[name];
                s += "\tCounters." + name + " = &Counters.List[" + gc.CounterIndex.ToString() + "];\n";
                if (gc.AssociatedScene.Trim().Length == 0)
                    scene_id = "0xFFFFFFFF";
                else
                    scene_id = "GAC_SCENE_ID_" + gc.AssociatedScene.Trim();
                // void			Init(unsigned int _index, unsigned int _interval, unsigned int _maxTimes, unsigned int _hash, bool _enabled, const char *_name, const char * _group);
                if ((this.EnableInfoLogging) || (this.EnableErrorLogging))
                    s += string.Format("\tCounters.{0}->Init({1},{2},{3},0x{4},{5},{6},\"{7}\",\"{8}\",{9});\n", name, gc.CounterIndex, gc.Interval, gc.MaxTimes, gc.Hash, gc.Priority, gc.Enabled.ToString().ToLower(), gc.Name, gc.Group,scene_id);
                else
                    s += string.Format("\tCounters.{0}->Init({1},{2},{3},0x{4},{5},{6},NULL,NULL,{7});\n", name, gc.CounterIndex, gc.Interval, gc.MaxTimes, gc.Hash, gc.Priority, gc.Enabled.ToString().ToLower(),scene_id);
                // verific si autoupdate
                if (gc.EnableCondition.Trim().Length>0)
                {
                    List<EnableStateCondition> lst= Counter.StringRepresentationToConditionList(gc.EnableCondition);
                    foreach (EnableStateCondition esc in lst)
                    {
                        if ((esc.conditionID<0) || (esc.conditionID>=Counter.ConditionsCPPIds.Length))
                        {
                            prj.EC.AddError("Invalid counter condition ID: " + esc.conditionID.ToString() + " => unable to translate it to C++");
                            continue;
                        }
                        string cpp_value = "";
                        string cpp_ptr = "";
                        switch (esc.conditionID)
                        {
                            case 0: cpp_value = "0"; cpp_ptr = "NULL"; break; // FB
                            case 1:
                            case 2: cpp_value = esc.strValue; cpp_ptr = "NULL"; break;
                            case 3:
                                cpp_value = "0"; // dummy value
                                if ((ads != null) && (ads.ContainsKey(esc.strValue)))
                                    cpp_ptr = "Ads."+esc.strValue;
                                else
                                    cpp_ptr = "NULL";
                                break;
                            case 4:
                            case 5: cpp_value = "0"; cpp_ptr = "NULL"; break; // ads enabled / disabled
                        }
                        s += string.Format("\tCounters.{0}->AddAutoEnableStateCondition({1},{2},{3},{4});\n",name,Counter.ConditionsCPPIds[esc.conditionID],cpp_value, cpp_ptr, esc.useAND.ToString().ToLower());
                    }
                }
            }
            return s + "\n";
        }
        private string GenerateCountersGroupInitCode(Dictionary<string, List<Counter>> groups)
        {
            if (groups.Keys.Count == 0)
                return "";
            string s = "\n";
            string nm,scname;
            int index = 0;
            Dictionary<string,CounterGroup> grp = new Dictionary<string,CounterGroup>();
            foreach (CounterGroup gcg in prj.CountersGroups)
                grp[gcg.Name] = gcg;

            foreach (string name in groups.Keys)
            {
                if (groups[name].Count == 0)
                    continue;
                List<Counter> lst = groups[name];
                s += "\tCountersGroups." + name + " = &CountersGroups.List[" + index.ToString() + "];\n";
                if ((this.EnableInfoLogging) || (this.EnableErrorLogging))
                    nm = "\"" + name + "\"";
                else
                    nm = "NULL";
                if (grp[name].StartTimerScene.Trim().Length == 0)
                    scname = "Main";
                else
                    scname = grp[name].StartTimerScene;
                s += string.Format("\tCountersGroups.{0}->Init(Context, {1},{2},{3},{4},{5},{6},{7},GAC_SCENE_ID_{8},{9});\n", name, index, nm, grp[name].UpdateMethod, lst.Count, grp[name].MinimTimeInterval * 1000, grp[name].StartTimerMethod, grp[name].AfterUpdateBehavior, scname, grp[name].UseEnableConditionProperty.ToString().ToLower());
                foreach (Counter gc in lst)
                {
                    s += string.Format("\tCountersGroups.{0}->Add(Counters.{1});\n", name, gc.Name);
                }
                index++;
            }
            return s + "\n";
        }
        private string GenerateAlarmsDefinitions(Dictionary<string, Alarm> alarms)
        {
            if (alarms.Keys.Count == 0)
                return "";
            string s = "\n\tGApp::UI::Alarm List[" + alarms.Keys.Count.ToString() + "];\n";
            foreach (string name in alarms.Keys)
            {
                s += "\tGApp::UI::Alarm* " + name + ";\n";
            }
            return s + "\n";
        }
        private string GenerateAlarmsInitCode(Dictionary<string, Alarm> alarms, bool debugMode)
        {
            if (alarms.Keys.Count == 0)
                return "";
            string s = "\n";
            foreach (string name in alarms.Keys)
            {
                Alarm alarm = alarms[name];
                s += "\tAlarms." + name + " = &Alarms.List[" + alarm.CounterIndex.ToString() + "];\n";
                s += string.Format("\tAlarms.{0}->Init({1});\n", alarm.Name, alarm.GetCppInitCode(debugMode));
            }
            return s + "\n";
        }

        public string EnableDefinitionsCode(string source, Dictionary<string, bool> defs, ErrorsContainer EC)
        {
            List<string> content = new List<string>();
            StringReader sr = new StringReader(source);
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                if (line.Trim().StartsWith("#"))
                    line = line.Trim();
                content.Add(line);
            }
            // parcurc totul si sterg
            bool keep = true;
            Stack<bool> stack = new Stack<bool>();
            stack.Push(true);
            int depth = 0;
            for (int tr = 0; tr < content.Count; tr++)
            {
                if (content[tr].StartsWith("#"))
                {
                    if (content[tr].StartsWith("#ifdef "))
                    {
                        string key = content[tr].Split(' ')[1].Trim();
                        if (defs.ContainsKey(key) == false)
                        {
                            EC.AddError("Unknown precompile definition : " + key);
                            return null;
                        }
                        depth++;
                        stack.Push(keep);
                        keep = defs[key] & keep;                        
                        content[tr] = null;
                        continue;
                    }
                    if (content[tr].StartsWith("#ifndef "))
                    {
                        string key = content[tr].Split(' ')[1].Trim();
                        if (defs.ContainsKey(key) == false)
                        {
                            EC.AddError("Unknown precompile definition : " + key);
                            return null;
                        }
                        stack.Push(keep);
                        keep = (!defs[key]) & keep;
                        content[tr] = null;
                        continue;
                    }
                    if (content[tr].StartsWith("#ifdefoneof "))
                    {
                        string[] keys = content[tr].Split(' ')[1].Trim().Split(',');
                        bool one_of = false;
                        foreach (string key in keys)
                        {
                            if (defs.ContainsKey(key) == false)
                            {
                                EC.AddError("Unknown precompile definition : " + key);
                                return null;
                            }
                            one_of |= defs[key];
                        }
                        stack.Push(keep);
                        keep = keep & one_of;
                        content[tr] = null;
                        continue;
                    }
                    if (content[tr].StartsWith("#endif"))
                    {
                        if (stack.Count==0)
                        {
                            EC.AddError("Too many #endif macros !");
                            return null;
                        }
                        keep = stack.Pop();
                        content[tr] = null;
                        continue;
                    }
                    EC.AddError("Unknown precompile macro in line '" + content[tr] + "'");
                    return null;
                }
                if (!keep)
                    content[tr] = null;
            }
            // build inapoi
            source = "";
            foreach (string s in content)
                if (s != null)
                    source += s + "\n";
            return source;
        }

        public string LanguageToName(Language lang)
        {
            string s = Project.LanguageTo2DigitsSymbol(lang);
            if ((s.Length==0) && (prj!=null))
                prj.EC.AddError("Unable to find a short translation (two letters translation) for " + lang.ToString());
            return s;
        }

        public bool GenerateResourceCodeFiles()
        {
            GenericBuildConfiguration usedBuild = this;

            if ((this.GetType() == typeof(DevelopBuildConfiguration)) && (((DevelopBuildConfiguration)this).BuildToUseForCodeSettings != null))
            {
                usedBuild = ((DevelopBuildConfiguration)this).BuildToUseForCodeSettings;
            }

            string h = Project.GetResource("All","Resources.h", prj.EC);
            string cpp = Project.GetResource("All","Resources.cpp", prj.EC);


            int count = 0;
            task.CreateSubTask("Creating resource.h and resource.cpp files ...");           
            // Imagini
            h = h.Replace("$$IMAGE_DEFINITIONS$$", GenerateHeaderDefinitions("GApp::Resources::Bitmap", typeof(ImageResource),out count));
            cpp = cpp.Replace("$$IMAGE_INIT_CODE$$", GenerateSourceInitCode(typeof(ImageResource)));

            // Texturi
            //h = h.Replace("$$TEXTURE_DEFINITIONS$$", GenerateHeaderDefinitions("GApp::Graphics::Texture", typeof(TextureAtlasResource)));
            //cpp = cpp.Replace("$$TEXTURE_INIT_CODE$$", GenerateSourceInitCode(typeof(TextureAtlasResource)));

            // Fonturi
            h = h.Replace("$$FONT_DEFINITIONS$$", GenerateHeaderDefinitionsForFonts("GApp::Resources::Font"));
            cpp = cpp.Replace("$$FONT_INIT_CODE$$", GenerateSourceInitCode(typeof(FontResource)));

            // Raw
            h = h.Replace("$$RAW_DEFINITIONS$$", GenerateHeaderDefinitions("GApp::Resources::RawResource", typeof(RawResource), out count));
            cpp = cpp.Replace("$$RAW_INIT_CODE$$", GenerateSourceInitCode(typeof(RawResource)));

            // Sound
            h = h.Replace("$$SOUND_DEFINITIONS$$", GenerateHeaderDefinitions("GApp::Resources::Sound", typeof(SoundResource), out count));
            cpp = cpp.Replace("$$SOUND_INIT_CODE$$", GenerateSourceInitCode(typeof(SoundResource)));
            if (count>0)
            {
                cpp = cpp.Replace("$$NUMBER_OF_SOUNDS$$", count.ToString());
                cpp = cpp.Replace("$$LIST_OF_SOUNDS$$", "Sounds.List");
            }
            else
            {
                cpp = cpp.Replace("$$NUMBER_OF_SOUNDS$$", "0");
                cpp = cpp.Replace("$$LIST_OF_SOUNDS$$", "NULL");
            }

            // Shader
            h = h.Replace("$$SHADER_DEFINITIONS$$", GenerateShaderDefinitions());
            h = h.Replace("$$SHADER_WRAPPER_CLASSES$$", GenerateShaderWrapperClassesHeaders());
            cpp = cpp.Replace("$$SHADER_INIT_CODE$$", GenerateSourceInitCode(typeof(ShaderResource)));
            cpp = cpp.Replace("$$SHADER_WRAPPER_CLASSES$$", GenerateShaderWrapperClassesCode());

            // strings
            h = h.Replace("$$STRINGS_DEFINITIONS$$", GenerateStringsDefinitions());
            cpp = cpp.Replace("$$STRINGS_INIT_CODE$$", GenerateStringsInitCode());

            // Presentations
            h = h.Replace("$$PRESENTATION_DEFINITIONS$$", GenerateHeaderDefinitions("GApp::Resources::Presentation", typeof(PresentationResource), out count));
            cpp = cpp.Replace("$$PRESENTATION_INIT_CODE$$", GenerateSourceInitCode(typeof(PresentationResource)));


            // Profilele
            h = h.Replace("$$PROFILES_DEFINITIONS$$", GenerateProfileHeaderDefinition());
            cpp = cpp.Replace("$$PROFILE_INIT_CODE$$", GenerateProfileInitCode());

            // ad-urile
            Dictionary<string, GenericAd> ads = GetAdsForCurrentBuild();
            h = h.Replace("$$ADS_DEFINITIONS$$", GenerateAdsHeaderDefinition(ads));
            cpp = cpp.Replace("$$ADS_INIT_CODE$$", GenerateAdsInitCode(ads));
            cpp = cpp.Replace("$$ADS_GETTEER_CODE$$", GenerateAdsGetterCode());

            // enumeratii
            h = h.Replace("$$ENUMERATIONS$$", GenerateEnumerationDefinitions());

            // Constante
            h = h.Replace("$$CONSTANTS$$", GenerateConstantsDefinitions());

            // DataTypes
            h = h.Replace("$$DATATYPES$$", GenerateDataTypesDefinitions());

            // constantele cu valori
            h = h.Replace("$$DATATYPESVALUES$$", GenerateConstantValuesDefinitions());
            cpp = cpp.Replace("$$CONSTANTS_INIT_CODE$$", GenerateConstantValuesInitCode());
            cpp = cpp.Replace("$$CONSTANTS_ARRAYS_INIT_CODE$$", GenerateConstantValuesArrayInitCode());

            // countere
            Dictionary<string, Counter> counters = new Dictionary<string, Counter>();
            Dictionary<string, List<Counter>> groups = new Dictionary<string, List<Counter>>();
            if (prj.CheckCountersIntegrity(usedBuild, counters ,groups) == false)
            {
                prj.EC.AddError("Unable to update counters !");
            }
            else
            {
                h = h.Replace("$$GLOBAL_COUNTERS_DEFINITIONS$$", GenerateCountersDefinitions(counters));
                h = h.Replace("$$GLOBAL_COUNTERS_GROUPS_DEFINITIONS$$", GenerateCountersGroupsDefinitions(groups));
                cpp = cpp.Replace("$$GLOBAL_COUNTERS_INIT_CODE$$", GenerateCountersInitCode(counters,ads));
                cpp = cpp.Replace("$$GLOBAL_COUNTERS_GROUPS_INIT_CODE$$", GenerateCountersGroupInitCode(groups));
                if (counters.Keys.Count==0)
                {
                    cpp = cpp.Replace("$$GLOBAL_COUNTER_LIST_GETTER$$", "NULL");
                    cpp = cpp.Replace("$$GLOBAL_COUNTER_LIST_COUNT_GETTER$$", "0");
                }
                else
                {
                    cpp = cpp.Replace("$$GLOBAL_COUNTER_LIST_GETTER$$", "&Counters.List[0]");
                    cpp = cpp.Replace("$$GLOBAL_COUNTER_LIST_COUNT_GETTER$$", counters.Keys.Count.ToString());
                }
                if (groups.Keys.Count==0)
                {
                    cpp = cpp.Replace("$$GLOBAL_COUNTER_GROUP_LIST_GETTER$$", "NULL");
                    cpp = cpp.Replace("$$GLOBAL_COUNTER_GROUP_LIST_COUNT_GETTER$$", "0");
                }
                else
                {
                    cpp = cpp.Replace("$$GLOBAL_COUNTER_GROUP_LIST_GETTER$$", "&CountersGroups.List[0]");
                    cpp = cpp.Replace("$$GLOBAL_COUNTER_GROUP_LIST_COUNT_GETTER$$", groups.Keys.Count.ToString());
                }
            }
            
            // alarme
            Dictionary<string, Alarm> alarms = new Dictionary<string, Alarm>();
            Dictionary<int, bool> alarms_id = new Dictionary<int, bool>();
            if (prj.CheckAlarmsIntegrity(usedBuild, alarms, alarms_id) == false)
            {
                prj.EC.AddError("Unable to update alarmss !");
            }
            else
            {
                h = h.Replace("$$GLOBAL_ALARMS_DEFINITIONS$$", GenerateAlarmsDefinitions(alarms));
                cpp = cpp.Replace("$$GLOBAL_ALARMS_INIT_CODE$$", GenerateAlarmsInitCode(alarms,usedBuild.EnableInfoLogging));
                if (alarms.Keys.Count == 0)
                {
                    cpp = cpp.Replace("$$GLOBAL_ALARM_LIST_GETTER$$", "NULL");
                    cpp = cpp.Replace("$$GLOBAL_ALARM_LIST_COUNT_GETTER$$", "0");
                }
                else
                {
                    cpp = cpp.Replace("$$GLOBAL_ALARM_LIST_GETTER$$", "&Alarms.List[0]");
                    cpp = cpp.Replace("$$GLOBAL_ALARM_LIST_COUNT_GETTER$$", alarms.Keys.Count.ToString());
                }
            }


            Disk.SaveFile(prj.ProjectPath + "\\CppProject\\Resources.h", h, prj.EC);
            Disk.SaveFile(prj.ProjectPath + "\\CppProject\\Resources.cpp", cpp, prj.EC);

            return task.UpdateSuccessErrorState(!prj.EC.HasErrors());
        }
        public Dictionary<string, bool> CreateDefsDictionary()
        {
            Dictionary<string, bool> defs = new Dictionary<string, bool>();
            defs["ENABLE_ERROR_LOGGING"] = EnableErrorLogging;
            defs["ENABLE_INFO_LOGGING"] = EnableInfoLogging;
            defs["ENABLE_EVENT_LOGGING"] = EnableEventLogging;
            return defs;
        }

        public bool GenerateResources(string outputName, bool includeSound)
        {
            prj.EC.Reset();
            AvailableResolutions.Clear();
            long pos = 0;
            task.SetMinMax(0, prj.Resources.Count);
            task.CreateSubTask("Creating resource file [" + prj.Resources.Count.ToString() + " resources]");
            string bldName = Name.ToLower();
            Dictionary<string, string> builds = new Dictionary<string, string>();
            R = new BuildResources();
            if (GetType()==typeof(DevelopBuildConfiguration))
            {
                bldName = ((DevelopBuildConfiguration)this).BuildToUseForCodeSettings.Name.ToLower().ToString();
            }
            try
            {

                FileStream f = File.Open(outputName, FileMode.Create);
                bool hasContent;
                byte[] buf;
                foreach (GenericResource g in prj.Resources)
                {
                    // nu pun in resurse fisierele care nu apartin acestui build
                    Project.StringListToDict(g.Builds, builds);
                    if (builds.ContainsKey(bldName) == false)
                        continue;
                    R.Add(g);
                    if ((g.GetType() == typeof(SoundResource)) && (includeSound == false))
                        continue;
                    if (g.GetType() == typeof(FontResource))
                    {
                        if (g.Load() == false)
                            continue;
                        foreach (Glyph glp in ((FontResource)g).Glyphs)
                        {
                            Glyph.GlyphVersionInfo vi = glp.GetVersion();
                            // nu pun in resurse fisierele care nu apartin acestui build

                            if (vi.Builds.ContainsKey(bldName)==false)
                                continue;
                            List<string> resolutions = vi.Builds[bldName];
                            // pentru fiecare rezolutie disponibila adaug imaginea
                            foreach (string res in resolutions)
                            {
                                AvailableResolutions[Project.SizeToValues(res)] = true;
                                if (glp.PicturesForResolutions.ContainsKey(res) == false)
                                {
                                    prj.EC.AddError("GenerateResource", "Image for character code " + glp.GetCharString() + " for resolution '" + res + "' does not exists !");
                                    continue;
                                }
                                if (glp.PicturesForResolutions[res] == null)
                                {
                                    prj.EC.AddError("GenerateResource", "Image for character code " + glp.GetCharString() + " for resolution '" + res + "' was not loaded !");
                                    continue;
                                }
                                Glyph.ImageInfo ii = glp.PicturesForResolutions[res];
                                buf = Project.ImageToBuffer((Bitmap)ii.Picture);
                                if (buf==null)
                                {
                                    prj.EC.AddError("GenerateResource", "Image for character code " + glp.GetCharString() + " for resolution '" + res + "' could not be converted to byte buffer !");
                                    continue;
                                }
                                f.Write(buf, 0, buf.Length);
                                ii.ResourceFilePoz = pos;
                                ii.ResourceSize = buf.Length;
                                pos += buf.Length;
                            }
                        }
                    }
                    else
                    {
                        if (g.GetType() == typeof(ImageResource))
                        {
                            ImageResource ir = (ImageResource)g;
                            AvailableResolutions[Project.SizeToValues(ir.DesignResolution)] = true;
                        }
                        hasContent = false;
                        buf = g.GetContent(ref hasContent);
                        if ((buf == null) && (hasContent))
                        {
                            task.Info("Unable to add " + g.GetResourceVariableName() + " of type " + g._Type);
                            task.IncrementProgress();
                            prj.EC.AddError("GenerateResource", "GetContent returned NULL for " + g.GetResourceVariableName() + " [" + g._Type + "]");
                            continue;
                        }
                        if (buf != null)
                        {
                            f.Write(buf, 0, buf.Length);
                            g.ResourceFilePosition = pos;
                            g.ResourceFileSize = buf.Length;
                            pos += buf.Length;
                        }
                    }
                    // totul e ok - adaug resursa in lista mea de resurse                                        
                    task.IncrementProgress();
                }
                hasContent = false;
                prj.SplashScreen.OverrideDefaultOutputName("logo.png");
                buf = prj.SplashScreen.GetContent(ref hasContent);
                if (buf == null)
                {
                    task.Info("Unable to add splash image !");
                    prj.EC.AddError("GenerateResource", "GetContent returned NULL for splash image");
                }
                else
                {
                    f.Write(buf, 0, buf.Length);
                    prj.SplashScreen.ResourceFilePosition = pos;
                    prj.SplashScreen.ResourceFileSize = buf.Length;
                    pos += buf.Length;
                }
                f.Close();
                return task.UpdateSuccessErrorState(!prj.EC.HasErrors());
            }
            catch (Exception e)
            {
                task.UpdateSuccessErrorState(false);
                prj.EC.AddException("GenerateResources", e.ToString(), e);
                return false;
            }
        }

        private string CreateObfuscatedStringFunction(string functionName, string buffer, string textToObfuscate)
        {
            string s = String.Format("char* {0} () {{\n", functionName);
            Random r = new Random();

            for (int tr = 0; tr < textToObfuscate.Length; tr++)
            {
                int key, key2;
                string tempVarName = String.Format("{0}[{1}]", buffer, tr);
                if (tr == 0)
                {
                    key = r.Next(95) + 32;
                    s += "\t" + tempVarName + " = " + key.ToString() + ";";
                }
                else
                {
                    key2 = r.Next(tr);
                    s += "\t" + tempVarName + " = " + String.Format("{0}[{1}];", buffer, key2);
                    key = textToObfuscate[key2];
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
                key2 = (int)textToObfuscate[tr];
                if (r.Next(2) == 0)
                {
                    if (key2 > key)
                    {
                        s += tempVarName + "= (char)(" + tempVarName + "+" + (key2 - key).ToString() + ");\n";
                    }
                    else
                    {
                        s += tempVarName + "= (char)(" + tempVarName + "-" + (key - key2).ToString() + ");\n";
                    }
                }
                else
                {
                    s += tempVarName + "= (char)(" + tempVarName + "^" + (key ^ key2).ToString() + ");\n";
                }
            }
            s += "\treturn (char*)&" + buffer + "[0];\n}\n";
            return s;
        }
        protected void CreateObfuscatedStringInit(Dictionary<string, string> d, string key, string text)
        {
            if (text.Length > 0)
            {
                d["$$" + key.ToUpper() + "_STRING_CREATE$$"] = String.Format("char {0}String[{1}];\n", key, text.Length + 1) + CreateObfuscatedStringFunction("Create" + key + "StringFunction", key + "String", text);
                d["$$" + key.ToUpper() + "_STRING_INIT$$"] = "Create" + key + "StringFunction()";
            }
            else
            {
                d["$$" + key.ToUpper() + "_STRING_CREATE$$"] = "";
                d["$$" + key.ToUpper() + "_STRING_INIT$$"] = "\"\"";
            }
        }
        public void UpdateReplaceDictionaryWithSocialMedia(Dictionary<string, string> d,Project prj)
        {
            CreateObfuscatedStringInit(d, "Facebook", prj.GetFacebookURL(this));
            CreateObfuscatedStringInit(d, "Youtube", prj.GetYouTubeURL(this));
            CreateObfuscatedStringInit(d, "Twitter", prj.GetTwitterURL(this));
            CreateObfuscatedStringInit(d, "Instagram", prj.GetInstagramURL(this));
            CreateObfuscatedStringInit(d, "AppMarket", Market);
            CreateObfuscatedStringInit(d, "DevMarket", DeveloperMarket);
            CreateObfuscatedStringInit(d, "DevWebPage", prj.GetWebPage(this));
        }
        public void Build()
        {
            if (prj.CheckResourcesIntegrity() == false)
                return;
            if (prj.CheckDataTypesIntegrity() == false)
                return;
            if (prj.CheckAnimationsIntegrity() == false)
                return;
            OnBuild();
        }

        public void SetBuildExtension(BuildExtension be)
        {
            extension = be;
            if (extension == null)
                return;
            extension.Build = this;
        }
        public BuildExtension GetBuildExtension()
        {
            return extension;
        }
        public void Duplicate(GenericBuildConfiguration bld)
        {
            FaceBook = bld.FaceBook;
            Youtube = bld.Youtube;
            Market = bld.Market;
            Webpage = bld.Webpage;
            DeveloperMarket = bld.DeveloperMarket;
            EnableErrorLogging = bld.EnableErrorLogging;
            EnableEventLogging = bld.EnableEventLogging;
            EnableInfoLogging = bld.EnableInfoLogging;
            if (bld.GetType() == this.GetType())
                DuplicateFromType(bld);

        }
        #endregion



        public virtual OSType GetOS()
        {
            return OSType.None;
        }
        public virtual int[] GetIconSizes()
        {
            return null;
        }
        public virtual string GetOutputApplicationFileName(Project prj)
        {
            return "output"+prj.Version+".bin";
        }
        protected virtual void OnBuild()
        {
            if (extension != null)
            {
                extension.prj = prj;
                extension.root = root;
                extension.task = task;
                // fac update si la lista de ad-uri
                Ads = prj.GetAdsUsedInBuild(this);
                // verific si daca se pot instala o data cu OS-ul
                foreach (GenericAd ad in Ads)
                {
                    if (ad.IsAvailableForOS(this.GetOS())==false)
                    {
                        prj.EC.AddError("Ad '" + ad.Name + "' is not available for '" + this.GetOS().ToString() + "' operating system");
                    }
                }
                if (prj.EC.HasErrors()==false)
                    extension.OnBuild();
            }
            else
            {
                task.Info("No extension available for " + Name);
                task.UpdateSuccessErrorState(false);
                prj.EC.AddError("No build extension available for " + Name);
            }
        }
        protected virtual void DuplicateFromType(GenericBuildConfiguration bld)
        {

        }


    }

    // Trebuie sa fie in raport de 1:1 cu DevelopBuildConfiguration in ceea ce priveste parametri de executie
    public enum ControlConsolePosition
    {
        Default,
        OnLeft,
        OnTop,
        Overlap,
    }
    public enum InAppSimulatedCurencyType
    {
        EUR,
        USD,
        RON,
    };
    public class ExecutionSettings
    {
        public string AppResolution = "480x800";
        public UInt32 MaxTextureSize = 0;
        public GAppCreator.Language AppLanguage = GAppCreator.Language.English;
        public bool BillingServiceAvailable = true;
        public bool AllowPurchases = true;
        public bool ShowControls = true;
        public bool ShowBoudaryRectangle = true;
        public bool EnableAdService = true;
        public int StartSnapshot = 0;
        public float Speed = 1.0f;
        public ControlConsolePosition ConsolePosition = ControlConsolePosition.OnLeft;
        public bool ConsoleIsMaximized = true;
        public bool ConsoleAlwaysOnTop = true;
        public bool EnableSounds = true;
        public string InAppItems = "";
        public InAppSimulatedCurencyType Currency = InAppSimulatedCurencyType.USD;
        public bool EnableSecondaryTouch = false;
        public bool FaceBookHandlerAvailable = true;
        public bool TwitterHandlerAvailable = true;
        public bool InstagramHandlerAvailable = true;
        public bool YouTubeHandlerAvailable = true;

        #region Atribute
        [XmlIgnore(), Description("Resolution of the application window"), Category("App Settings"), DisplayName("Resolution"), Editor(typeof(ResolutionEditor), typeof(UITypeEditor))]
        public string _AppResolution
        {
            get { return AppResolution; }
            set
            {
                int w = 0, h = 0;
                if (Project.SizeToValues(value, ref w, ref h))
                    AppResolution = Project.ResolutionToString(w, h);
            }
        }
        [XmlIgnore(), Description("Game execution speed (in percentage)"), Category("App Settings"), DisplayName("Speed")]
        public string _Speed
        {
            get { return Project.ProcentToString(Speed); }
            set { 
                float res = 0.0f;
                if (Project.StringToProcent(value,ref res))
                {
                    if (res < 0.1)
                        res = 0.1f;
                    if (res > 1)
                        res = 1;
                    Speed = res;
                }
            }
        }
        [XmlIgnore(), Description("Maximum texture size for OpenGL context. If 0, the current system value will be used.Shold be at least 64."), Category("App Settings"), DisplayName("Texture Size")]
        public UInt32 _MaxTextureSize
        {
            get { return MaxTextureSize; }
            set
            {
                MaxTextureSize = 0;
                for (UInt32 tr = 1; tr < (1 << 25); tr *= 2)
                {
                    if (tr >= value)
                    {
                        MaxTextureSize = tr;
                        break;
                    }
                }
                if (MaxTextureSize < 64)
                    MaxTextureSize = 0;
            }
        }
        [XmlIgnore(), Description("Language to be used on the app"), Category("App Settings"), DisplayName("Language")]
        public GAppCreator.Language _AppLanguage
        {
            get { return AppLanguage; }
            set { AppLanguage = value; }
        }
        [XmlIgnore(), Description("Enable sounds when the application is started."), Category("App Settings"), DisplayName("Enable sounds")]
        public bool _EnableSounds
        {
            get { return EnableSounds; }
            set { EnableSounds = value; }
        }        
        [XmlIgnore(), Description("Enables a secondary touch with a cursor that can be controled using arrows. Keep 'CTRL' key press to simulate touch."), Category("App Settings"), DisplayName("Enable secondary touch")]
        public bool _EnableSecondaryTouch
        {
            get { return EnableSecondaryTouch; }
            set { EnableSecondaryTouch = value; }
        }
        [XmlIgnore(), Description("Specifies if the billing services are available or not !"), Category("Billing"), DisplayName("Billing Service")]
        public bool _BillingServiceAvailable
        {
            get { return BillingServiceAvailable; }
            set { BillingServiceAvailable = value; }
        }
        [XmlIgnore(), Description("Specifies if purchases are allowed or not !"), Category("Billing"), DisplayName("Allow purchases")]
        public bool _AllowPurchases
        {
            get { return AllowPurchases; }
            set { AllowPurchases = value; }
        }
        [XmlIgnore(), Description("In-App items simulator (items and their prices)"), Category("Billing"), DisplayName("In-app items"), Editor(typeof(InAppItemsEditor), typeof(UITypeEditor))]
        public string _InAppItems
        {
            get { return InAppItems; }
            set { InAppItems = value; }
        }
        [XmlIgnore(), Description("In-App items currency"), Category("Billing"), DisplayName("In-app Currency")]
        public InAppSimulatedCurencyType _Currency
        {
            get { return Currency; }
            set { Currency = value; }
        }
        [XmlIgnore(), Description("Show controls from the current scene !"), Category("Misc"), DisplayName("Show Controls")]
        public bool _ShowControls
        {
            get { return ShowControls; }
            set { ShowControls = value; }
        }
        [XmlIgnore(), Description("Show touch boundary if any !"), Category("Misc"), DisplayName("Show touch boundary")]
        public bool _ShowBoudaryRectangle
        {
            get { return ShowBoudaryRectangle; }
            set { ShowBoudaryRectangle = value; }
        }
        [XmlIgnore(), Description("Start from a specific snapshot when the application is starting"), Category("App Settings"), DisplayName("Start snapshot"), Editor(typeof(SettingsSnapshotEditor), typeof(UITypeEditor))]
        public string _StartSnapshot
        {
            get { if (SettingsSnapshotControl.CurrentProject != null) return SettingsSnapshotControl.CurrentProject.SettingsSnapshots.GetStringRepresentation(StartSnapshot); else return "?"; }
            set { if (SettingsSnapshotControl.CurrentProject != null) StartSnapshot = SettingsSnapshotControl.CurrentProject.SettingsSnapshots.GetSnapshotID(value); }
        }
        [XmlIgnore(), Description("Enable or disable simulated ads !"), Category("Ads"), DisplayName("Enable ad service")]
        public bool _EnableAdService
        {
            get { return EnableAdService; }
            set { EnableAdService = value; }
        }
        [XmlIgnore(), Description("Control console is expanded (maximized)"), Category("Control Console"), DisplayName("Expanded")]
        public bool _ConsoleIsMaximized
        {
            get { return ConsoleIsMaximized; }
            set { ConsoleIsMaximized = value; }
        }
        [XmlIgnore(), Description("Position of the control console in relation with the game window"), Category("Control Console"), DisplayName("Position")]
        public ControlConsolePosition _ConsolePosition
        {
            get { return ConsolePosition; }
            set { ConsolePosition = value; }
        }
        [XmlIgnore(), Description("True if facebook app is installed into the device"), Category("Social Networks"), DisplayName("Facebook App")]
        public bool _FaceBookHandlerAvailable
        {
            get { return FaceBookHandlerAvailable; }
            set { FaceBookHandlerAvailable = value; }
        }
        [XmlIgnore(), Description("True if YouTUbe app is installed into the device"), Category("Social Networks"), DisplayName("YouTube App")]
        public bool _YouTubeHandlerAvailable
        {
            get { return YouTubeHandlerAvailable; }
            set { YouTubeHandlerAvailable = value; }
        }
        [XmlIgnore(), Description("True if instagram app is installed into the device"), Category("Social Networks"), DisplayName("Instagram App")]
        public bool _InstagramHandlerAvailable
        {
            get { return InstagramHandlerAvailable; }
            set { InstagramHandlerAvailable = value; }
        }
        [XmlIgnore(), Description("True if twitter app is installed into the device"), Category("Social Networks"), DisplayName("Twitter App")]
        public bool _TwitterHandlerAvailable
        {
            get { return TwitterHandlerAvailable; }
            set { TwitterHandlerAvailable = value; }
        }
        #endregion

        public bool CreateSettings(Project prj,int x,int y)
        {
            int w = 0, h = 0;
            if (Project.SizeToValues(this.AppResolution, ref w, ref h) == false)
            {
                prj.EC.AddError("Invalid resolution: " + this.AppResolution);
                return false;
            }
            string s = "";
            s += string.Format("Width={0}\n", w);
            s += string.Format("Height={0}\n", h);
            s += string.Format("MaxTextureSize={0}\n", this.MaxTextureSize);
            s += string.Format("Language={0}\n", this.AppLanguage.ToString());
            s += string.Format("BillingService={0}\n", this.BillingServiceAvailable);
            s += string.Format("AllowPurchases={0}\n", this.AllowPurchases);
            s += string.Format("ShowControls={0}\n", this.ShowControls);
            s += string.Format("ShowBoudaryRectangle={0}\n", this.ShowBoudaryRectangle);
            s += string.Format("LoadSettings={0}\n", this.StartSnapshot);
            s += string.Format("EnableAdService={0}\n", this.EnableAdService);
            s += string.Format("InAppItems={0}\n", this.InAppItems);
            s += string.Format("Currency={0}\n", this.Currency);
            s += string.Format("Speed={0}\n", (int)(this.Speed * 100));
            s += string.Format("EnableSounds={0}\n", this.EnableSounds);
            s += string.Format("EnableSecondaryTouch={0}\n", this.EnableSecondaryTouch);
            s += string.Format("TwitterHandlerAvailable={0}\n", this.TwitterHandlerAvailable);
            s += string.Format("InstagramHandlerAvailable={0}\n", this.InstagramHandlerAvailable);
            s += string.Format("YouTubeHandlerAvailable={0}\n", this.YouTubeHandlerAvailable);
            s += string.Format("FaceBookHandlerAvailable={0}\n", this.FaceBookHandlerAvailable);
            s += string.Format("SnapshotType={0}\n", prj.BuildConfigurations[0].SnapshotType); // valoarea din develop
            if ((x>=0) && (y>=0))
            {
                s += string.Format("X={0}\n", x);
                s += string.Format("Y={0}\n", y);
            }
            return Disk.SaveFile(Path.Combine(prj.ProjectPath, "Bin", "settings.ini"), s, prj.EC);
        }
    }
    public class DevelopBuildConfiguration : GenericBuildConfiguration
    {
        [XmlAttribute()]
        public string AppResolution = "480x800";
        [XmlAttribute()]
        public UInt32 MaxTextureSize = 0;
        [XmlAttribute()]
        public GAppCreator.Language AppLanguage = GAppCreator.Language.English;
        [XmlAttribute()]
        public string InAppItems = "";
        [XmlAttribute()]
        public InAppSimulatedCurencyType Currency = InAppSimulatedCurencyType.USD;
        [XmlAttribute()]
        public ControlConsolePosition ConsolePosition = ControlConsolePosition.OnLeft;
        [XmlAttribute()]
        public bool ConsoleIsMaximized = true;
        [XmlAttribute()]
        public bool ConsoleAlwaysOnTop = true;
        [XmlAttribute()]
        public int StartSnapshot = 0;
        [XmlAttribute()]
        public bool EnableSounds = true;
        [XmlAttribute()]
        public bool EnableSecondaryTouch = false;

        [XmlIgnore()]
        public GenericBuildConfiguration BuildToUseForCodeSettings = null;


        #region Atribute
        [XmlIgnore(), Description("Resolution of the application window"), Category("App Settings"), DisplayName("Resolution"), Editor(typeof(ResolutionEditor), typeof(UITypeEditor))]
        public string _AppResolution
        {
            get { return AppResolution; }
            set
            {
                int w = 0, h = 0;
                if (Project.SizeToValues(value, ref w, ref h))
                    AppResolution = Project.ResolutionToString(w, h);
            }
        }
        [XmlIgnore(), Description("In-App items simulator (items and their prices)"), Category("App Settings"), DisplayName("In-app items"), Editor(typeof(InAppItemsEditor), typeof(UITypeEditor))]
        public string _InAppItems
        {
            get { return InAppItems; }
            set { InAppItems = value; }
        }
        [XmlIgnore(), Description("In-App items currency"), Category("App Settings"), DisplayName("In-app Currency")]
        public InAppSimulatedCurencyType _Currency
        {
            get { return Currency; }
            set { Currency = value; }
        }
        [XmlIgnore(), Description("Maximum texture size for OpenGL context. If 0, the current system value will be used.Shold be at least 64."), Category("App Settings"), DisplayName("Texture Size")]
        public UInt32 _MaxTextureSize
        {
            get { return MaxTextureSize; }
            set
            {
                MaxTextureSize = 0;
                for (UInt32 tr = 1; tr < (1 << 25); tr *= 2)
                {
                    if (tr >= value)
                    {
                        MaxTextureSize = tr;
                        break;
                    }
                }
                if (MaxTextureSize < 64)
                    MaxTextureSize = 0;
            }
        }
        [XmlIgnore(), Description("Language to be used on the app"), Category("App Settings"), DisplayName("Language")]
        public GAppCreator.Language _AppLanguage
        {
            get { return AppLanguage; }
            set { AppLanguage = value; }
        }
        [XmlIgnore(), Description("Start from a specific snapshot when the application is starting"), Category("App Settings"), DisplayName("Start snapshot"), Editor(typeof(SettingsSnapshotEditor), typeof(UITypeEditor))]
        public string _StartSnapshot
        {
            get { if (SettingsSnapshotControl.CurrentProject != null) return SettingsSnapshotControl.CurrentProject.SettingsSnapshots.GetStringRepresentation(StartSnapshot); else return "?"; }
            set { if (SettingsSnapshotControl.CurrentProject != null) StartSnapshot = SettingsSnapshotControl.CurrentProject.SettingsSnapshots.GetSnapshotID(value); }
        }
        [XmlIgnore(), Description("Enable sounds when the application is started."), Category("App Settings"), DisplayName("Enable sounds")]
        public bool _EnableSounds
        {
            get { return EnableSounds; }
            set { EnableSounds = value; }
        }
        [XmlIgnore(), Description("Enables a secondary touch with a cursor that can be controled using arrows. Keep 'CTRL' key press to simulate touch."), Category("App Settings"), DisplayName("Enable secondary touch")]
        public bool _EnableSecondaryTouch
        {
            get { return EnableSecondaryTouch; }
            set { EnableSecondaryTouch = value; }
        }

        [XmlIgnore(), Description("Enable/Disable error logging"), Category("Debug"), DisplayName("Error logging")]
        public override bool _EnableErrorLogging
        {
            get { EnableErrorLogging = true; return true; }
            set { EnableErrorLogging = true; MessageBox.Show("Error logging is always set in developer mode !"); }
        }
        [XmlIgnore(), Description("Enable/Disable event logging"), Category("Debug"), DisplayName("Event logging")]
        public override bool _EnableEventLogging
        {
            get { EnableEventLogging = false; return false; }
            set { EnableEventLogging = false; MessageBox.Show("Event logging is never set in developer mode !"); }
        }
        [XmlIgnore(), Description("Enable/Disable information logging"), Category("Debug"), DisplayName("Information logging")]
        public override bool _EnableInfoLogging
        {
            get { EnableInfoLogging = true; return true; }
            set { EnableInfoLogging = true; MessageBox.Show("Info logging is always set in developer mode !"); }
        }
        [XmlIgnore(), Description("Specifies how the image should be created.\nDynamic means that every image will be resize dynamically.\nResizeAtStartup means that the images will be resized at startup.\nCustomizeForEachImage means that each image will be resize according to its 'Resize Method' property."), Category("Video Card"), DisplayName("Image Create Method")]
        public override BuildImageCreateMethod _ImageCreateMethod
        {
            get { return ImageCreateMethod; }
            set { ImageCreateMethod = BuildImageCreateMethod.ResizeAtStartup;  MessageBox.Show("Develop mode only uses ResizeAtStartup option.");}
        }
        [XmlIgnore(), Description("Control console is expanded (maximized)"), Category("Control Console"), DisplayName("Expanded")]
        public bool _ConsoleIsMaximized
        {
            get { return ConsoleIsMaximized; }
            set { ConsoleIsMaximized = value; }
        }
        [XmlIgnore(), Description("Control console is always on top of other windows"), Category("Control Console"), DisplayName("Always on Top")]
        public bool _ConsoleAlwaysOnTop
        {
            get { return ConsoleAlwaysOnTop; }
            set { ConsoleAlwaysOnTop = value; }
        }
        [XmlIgnore(), Description("Position of the control console in relation with the game window"), Category("Control Console"), DisplayName("Position")]
        public ControlConsolePosition _ConsolePosition
        {
            get { return ConsolePosition; }
            set { ConsolePosition = value; }
        }
        #endregion

        public override OSType GetOS()
        {
            return OSType.WindowsDesktop;
        }
        protected override void OnBuild()
        {
            // genereaza doar resursele
            if (GenerateResources(prj.ProjectPath + "\\Bin\\resources.res", true) == false)
                return;
            if (GenerateResourceCodeFiles() == false)
                return;
            task.Info("All OK !");
        }
        public override int[] GetIconSizes()
        {
            return new int[] { 96, 128 };
        }

        public void UpdateRunSettings(ExecutionSettings settings)
        {
            settings.AppLanguage = this.AppLanguage;
            settings.AppResolution = this.AppResolution;
            settings.MaxTextureSize = this.MaxTextureSize;
            settings.ConsolePosition = this.ConsolePosition;
            settings.ConsoleIsMaximized = this.ConsoleIsMaximized;
            settings.ConsoleAlwaysOnTop = this.ConsoleAlwaysOnTop;
            settings.InAppItems = this.InAppItems;
            settings.Currency = this.Currency;
            settings.StartSnapshot = this.StartSnapshot;
            settings.EnableSounds = this.EnableSounds;
            settings.EnableSecondaryTouch = this.EnableSecondaryTouch;
        }
    }
    public class AndroidBuildConfiguration : GenericBuildConfiguration
    {
        [XmlAttribute()]
        public string Package = "";
        [XmlAttribute()]
        public int VersionCode = 1;
        [XmlAttribute()]
        public AndroidVersion MinSDKVersion = AndroidVersion.Froyo_8, TargetSDKVersion = AndroidVersion.None;
        [XmlAttribute()]
        public bool PermInternet, PermVibrate, PermExternalStorage,PermSamsung;
        [XmlAttribute()]
        public ScreenOrientation Orientation = ScreenOrientation.Landscape;
        [XmlAttribute()]
        public string CompilerParams = "-Werror";
        [XmlAttribute()]
        public CompilerOptimizationMode OptimizationMode = CompilerOptimizationMode.Debug;
        [XmlAttribute()]
        public AndroidVersion AndroidSDKVersion = AndroidVersion.JellyBean_17;
        [XmlAttribute()]
        public bool PreserveEGLContext = true;
        [XmlAttribute()]
        public AndroidJavaCodeObfuscation JavaCodeObfuscation = AndroidJavaCodeObfuscation.None;
        [XmlAttribute()]
        public AndroidBillingMarket BillingMarket = AndroidBillingMarket.None;
        [XmlAttribute()]
        public string BillMarketKey = "",InAppItemsList="";
        [XmlAttribute()]
        public bool HideSoftwareButtons;
        [XmlAttribute()]
        public bool EnableAdMobTestMode = false;
        [XmlAttribute()]
        public string AdMobAppID = "";
        [XmlAttribute()]
        public string GoogleAnalyticsTrackingID = "";
        [XmlAttribute()]
        public string FlurryAPIKey = "";
        [XmlAttribute()]
        public AnalyticsFrameworkType AnalyticsFramework = AnalyticsFrameworkType.None;
        [XmlAttribute()]
        public string ChartboostAppID = "";
        [XmlAttribute()]
        public string ChartboostAppSignature = "";
        [XmlAttribute()]
        public bool GooglePlayServices = false;
        [XmlAttribute()]
        public string GooglePlayServicesAppID = "";
        [XmlAttribute()]
        public string GooglePlayLeaderboardsList = "";
        [XmlAttribute()]
        public string FirebaseGoogleServicesJSONFile = "";
        [XmlAttribute()]
        public bool FirebaseCrashAnalytics = false;

        #region Atribute
        [XmlIgnore(), Description("Screen Orientation"), Category("Application"), DisplayName("Screen Orientation")]
        public ScreenOrientation _Orientation
        {
            get { return Orientation; }
            set { Orientation = value; }
        }
        [XmlIgnore(), Description("Package name"), Category("Application"), DisplayName("Package Name")]
        public string _Package
        {
            get { return Package; }
            set { Package = value; }
        }
        [XmlIgnore(), Description("Version Code"), Category("Application"), DisplayName("Version Code")]
        public int _VersionCode
        {
            get { return VersionCode; }
            set { if (value > 0) VersionCode = value; }
        }
        [XmlIgnore(), Description("Hide software buttons bar for Android 4.1+"), Category("Application"), DisplayName("Hide Software Buttons")]
        public bool _HideSoftwareButtons
        {
            get { return HideSoftwareButtons; }
            set { /*MessageBox.Show("Not implemented yet ! Modify Activity template"); */ HideSoftwareButtons = value; }
        }
        [XmlIgnore(), Description("Minim SDK Version"), Category("Application"), DisplayName("Minim SDK Version")]
        public AndroidVersion _MinSDKVerison
        {
            get { return MinSDKVersion; }
            set { if (value > 0) MinSDKVersion = value; }
        }
        [XmlIgnore(), Description("Target SDK Version"), Category("Application"), DisplayName("Target SDK Version")]
        public AndroidVersion _TargetSDKVersion
        {
            get { return TargetSDKVersion; }
            set { if (value >= 0) TargetSDKVersion = value; }
        }
        [XmlIgnore(), Description("Compiler params"), Category("Build"), DisplayName("Compile params")]
        public string _CompilerParams
        {
            get { return CompilerParams; }
            set { CompilerParams = value; }
        }
        [XmlIgnore(), Description("None - default compiler setup.\nDebug - no optimization and debug information.\nNormal - no optimization and no debug information.\nOptimize - default optimization and no debug information.\nHighlyOptimized - best speed optimization and no debug information."), Category("Build"), DisplayName("Compiler optimization mode")]
        public CompilerOptimizationMode _OptimizationMode
        {
            get { return OptimizationMode; }
            set { 
                OptimizationMode = value;
                if ((OptimizationMode == CompilerOptimizationMode.Optimize) || (OptimizationMode == CompilerOptimizationMode.HighlyOptimize))
                    MessageBox.Show("Optimize and HighlyOptimize are not fully implemented yet - the application will most likely crush !");
            }
        }
        [XmlIgnore(), Description("Specifies the SDK verision of Android.jar that will be user"), Category("Build"), DisplayName("Android SDK Version")]
        public AndroidVersion _AndroidSDKVersion
        {
            get { return AndroidSDKVersion; }
            set { AndroidSDKVersion = value; }
        }
        [XmlIgnore(), Description("Access Internet"), Category("Permissions"), DisplayName("Access Internet")]
        public bool _PermInternet
        {
            get { return PermInternet; }
            set { PermInternet = value; }
        }
        [XmlIgnore(), Description("Vibrate"), Category("Permissions"), DisplayName("Vibrate")]
        public bool _PermVibrate
        {
            get { return PermVibrate; }
            set { PermVibrate = value; }
        }
        [XmlIgnore(), Description("External Storage"), Category("Permissions"), DisplayName("External Storage")]
        public bool _PermExternalStorage
        {
            get { return PermExternalStorage; }
            set { PermExternalStorage = value; }
        }
        [XmlIgnore(), Description("Required only if Samsung Galaxy the app needs to be register to Samsung Galaxy market"), Category("Permissions"), DisplayName("Samsung Galaxy")]
        public bool _PermSamsung
        {
            get { return PermSamsung; }
            set { PermSamsung = value; }
        }
        [XmlIgnore(), Description("Specify what kind of obfuscation to use for Java wrapper classes !"), Category("Debug"), DisplayName("Java Obfuscation")]
        public virtual AndroidJavaCodeObfuscation _JavaCodeObfuscation
        {
            get { return JavaCodeObfuscation; }
            set { JavaCodeObfuscation = value; }
        }
        [XmlIgnore(), Description("Specifies the market for witch the billing system is designed for."), Category("Billing"), DisplayName("Market")]
        public virtual AndroidBillingMarket _BillingMarket
        {
            get { return BillingMarket; }
            set { BillingMarket = value; }
        }
        [XmlIgnore(), Description("Specifies security key to use for billing in market (a security key that identifies your account)."), Category("Billing"), DisplayName("Key")]
        public virtual string _BillMarketKey
        {
            get { return BillMarketKey; }
            set { BillMarketKey = value; }
        }
        [XmlIgnore(), Description("Specifies a list of in-app items separated by comma (,)."), Category("Billing"), DisplayName("In-App Items")]
        public virtual string _InAppItemsList
        {
            get { return InAppItemsList; }
            set { InAppItemsList = value; }
        }
        [XmlIgnore(), Description("Preserve EGL Context on Android 3.0 or higher after onPause is called !"), Category("Video Card"), DisplayName("Preserve EGL Context")]
        public virtual bool _PreserveEGLContext
        {
            get { return PreserveEGLContext; }
            set { PreserveEGLContext = value; }
        }

        [XmlIgnore(), Description("Analytics framework"), Category("Analytics"), DisplayName("Analytics framework")]
        public virtual AnalyticsFrameworkType _AnalyticsFramework
        {
            get { return AnalyticsFramework; }
            set { AnalyticsFramework = value; }
        }
        [XmlIgnore(), Description("Flurry API key"), Category("Analytics"), DisplayName("Flurry API Key")]
        public virtual string _FlurryAPIKey
        {
            get { return FlurryAPIKey; }
            set { FlurryAPIKey = value; }
        }

        #endregion

        public override OSType GetOS()
        {
            return OSType.Android;
        }
        public override string GetOutputApplicationFileName(Project prj)
        {
            return Package + "." + prj.Version + ".apk";
        }
        public override int[] GetIconSizes()
        {
            return new int[] { 36, 48, 72, 96, 144, 192 };
        }
        protected override void DuplicateFromType(GenericBuildConfiguration bld)
        {
            AndroidBuildConfiguration b = (AndroidBuildConfiguration)bld;
            Package = b.Package;
            VersionCode = b.VersionCode;
            MinSDKVersion = b.MinSDKVersion;
            TargetSDKVersion = b.TargetSDKVersion;
            PermExternalStorage = b.PermExternalStorage;
            PermInternet = b.PermInternet;
            PermVibrate = b.PermVibrate;
            Orientation = b.Orientation;
            CompilerParams = b.CompilerParams;
            AndroidSDKVersion = b.AndroidSDKVersion;
            BillingMarket = b.BillingMarket;
            OptimizationMode = b.OptimizationMode;
            PreserveEGLContext = b.PreserveEGLContext;
            JavaCodeObfuscation = b.JavaCodeObfuscation;
            BillMarketKey = b.BillMarketKey;
            HideSoftwareButtons = b.HideSoftwareButtons;
            ImageCreateMethod = b.ImageCreateMethod;
            Defines = b.Defines;

        }
    }
    public class IOSBuildConfiguration : GenericBuildConfiguration
    {
        [XmlAttribute()]
        public ScreenOrientation Orientation = ScreenOrientation.Landscape;



        #region Atribute
        [XmlIgnore(), Description("Screen Orientation"), Category("Application"), DisplayName("Screen Orientation")]
        public ScreenOrientation _Orientation
        {
            get { return Orientation; }
            set { Orientation = value; }
        }


        #endregion

        public override OSType GetOS()
        {
            return OSType.IOS;
        }
        public override string GetOutputApplicationFileName(Project prj)
        {
            return prj.GetProjectName() + "." + prj.Version + ".ipa";
        }
        public override int[] GetIconSizes()
        {
            return new int[] { 20,29,40,50,57,58,60,72,76,80,87,100,114,120,144,152,180};
        }
        protected override void DuplicateFromType(GenericBuildConfiguration bld)
        {
            IOSBuildConfiguration b = (IOSBuildConfiguration)bld;
            Orientation = b.Orientation;
        }
    }
    public class MacBuildConfiguration : GenericBuildConfiguration
    {
        [XmlAttribute()]
        public string WindowSize = "";



        #region Atribute
        [XmlIgnore(), Description("Window Size"), Category("Application"), DisplayName("Window size")]
        public string _WindowSize
        {
            get { return WindowSize; }
            set { WindowSize = value; }
        }


        #endregion

        public override OSType GetOS()
        {
            return OSType.Mac;
        }
        public override string GetOutputApplicationFileName(Project prj)
        {
            return prj.GetProjectName() + "." + prj.Version + ".mac";
        }
        public override int[] GetIconSizes()
        {
            return new int[] { 72 };
        }
        protected override void DuplicateFromType(GenericBuildConfiguration bld)
        {
            MacBuildConfiguration b = (MacBuildConfiguration)bld;
            WindowSize = b.WindowSize;
            ImageCreateMethod = b.ImageCreateMethod;
            Defines = b.Defines;
        }
    }


    public class WindowsDesktopBuildConfiguration : GenericBuildConfiguration
    {
        [XmlAttribute()]
        public string AppResolution = "480 x 800";
        [XmlAttribute()]
        public CompilerOptimizationMode OptimizationMode = CompilerOptimizationMode.Debug;
        [XmlAttribute()]
        public string CompilerParams = "";
        [XmlAttribute()]
        public WindowsDesktopCodePacker PackMethod = WindowsDesktopCodePacker.UPX;

        #region Atribute

        [XmlIgnore(), Description("Compiler params"), Category("Build"), DisplayName("Compile params")]
        public string _CompilerParams
        {
            get { return CompilerParams; }
            set { CompilerParams = value; }
        }
        [XmlIgnore(), Description("None - no optimizations added.\nDebug - no optimization and debug information.\nNormal - Optimize for size and no debug informations.\nOptimize - Optimized for speed no debug information.\nHighlyOptimized - Optimized for both speed and size and no debug information."), Category("Build"), DisplayName("Compiler optimization mode")]
        public CompilerOptimizationMode _OptimizationMode
        {
            get { return OptimizationMode; }
            set
            {
                OptimizationMode = value;
            }
        }
        [XmlIgnore(), Description("Code packing method"), Category("Build"), DisplayName("Code pack")]
        public WindowsDesktopCodePacker _PackMethod
        {
            get { return PackMethod; }
            set { PackMethod = value; }
        }
        [XmlIgnore(), Description("Resolution of the application window"), Category("General"), DisplayName("Resolution"), Editor(typeof(ResolutionEditor), typeof(UITypeEditor))]
        public string _AppResolution
        {
            get { return AppResolution; }
            set
            {
                int w = 0, h = 0;
                if (Project.SizeToValues(value, ref w, ref h))
                    AppResolution = Project.ResolutionToString(w, h);
            }
        }

        #endregion

        public override OSType GetOS()
        {
            return OSType.WindowsDesktop;
        }
        public override string GetOutputApplicationFileName(Project prj)
        {
            return prj.GetProjectName() + "." + prj.Version + ".exe";
        }
        public override int[] GetIconSizes()
        {
            return new int[] { 16, 32, 48, 64};
        }
        protected override void DuplicateFromType(GenericBuildConfiguration bld)
        {
            WindowsDesktopBuildConfiguration b = (WindowsDesktopBuildConfiguration)bld;
            AppResolution = b.AppResolution;
            OptimizationMode = b.OptimizationMode;
            PackMethod = b.PackMethod;
            ImageCreateMethod = b.ImageCreateMethod;
            Defines = b.Defines;
        }
    }    
    #endregion

    #region Publish Material Objects
    [XmlInclude(typeof(TextPublish))]
    [XmlInclude(typeof(ImagePublish))]
    [XmlInclude(typeof(VideoPublish))]
    [XmlType("PublishObject"), XmlRoot("PublishObject")]
    public class PublishObject : IComparable
    {
        [XmlAttribute()]
        public string Name = "";
        [XmlAttribute()]
        public string FileID = "";
        [XmlAttribute()]
        public string Builds = "";
        [XmlAttribute()]
        public ExtendedLanguage Lang = ExtendedLanguage.All;

        [XmlIgnore()]
        public Project prj = null;

        #region Atribute
        [XmlIgnore(), Description("Name"), Category("General"), DisplayName("Name")]
        public virtual string _Name
        {
            get { return Name; }
            set { Name = value; }
        }
        [XmlIgnore(), Description("Type"), Category("General"), DisplayName("Type")]
        public virtual string _Type
        {
            get { return GetObjectType(); }
        }
        [XmlIgnore(), Description("ID"), Category("General"), DisplayName("ID")]
        public virtual string _FileID
        {
            get { return FileID; }
        }
        [XmlIgnore(), Description("Specifies what builds will use this publish object"), Category("Target"), DisplayName("Builds"), Editor(typeof(CheckBoxTypeEditor), typeof(UITypeEditor)), CheckBoxTypeEditor.Source(CheckBoxAttributeType.Builds)]
        public virtual string _Builds
        {
            get { return Builds; }
            set { Builds = value; }
        }
        [XmlIgnore(), Description("Specifies for what laguage this publish object will used"), Category("Target"), DisplayName("Language")]
        public virtual ExtendedLanguage _Lang
        {
            get { return Lang; }
            set { Lang = value; }
        }
        #endregion

        #region General Functions

        public string GetObjectFolder()
        {
            return Path.Combine(prj.ProjectPath, "PublishMaterials", FileID);
        }
        public bool Create(Project p)
        {
            prj = p;
            Random r = new Random(System.Environment.TickCount);
            do
            {
                FileID = "Obj-"+System.Environment.TickCount.ToString("X8") + "-" + r.Next(65535).ToString("X4");
            } while (Directory.Exists(GetObjectFolder()) == true);
            return Disk.CleanDirectory(GetObjectFolder(), p.EC);
        }
        public virtual string GetObjectFile()
        {
            return "";          
        }
        public virtual string GetObjectSource()
        {
            return ""; // daca nu are sursa - returnez ""
        }

        #endregion


        #region Virtual Functions
        public virtual string GetObjectType()
        {
            return "?";
        }
        public virtual string GetInformation()
        {
            return "";
        }
        public virtual bool Build()
        {
            return true;
        }


        public virtual void RemoveBuild(string buildName)
        {
            Dictionary<string, string> d = Project.StringListToDict(Builds);
            if (d.ContainsKey(buildName))
                d.Remove(buildName);
            string ss = "";
            foreach (string k in d.Keys)
                ss += d[k] + ",";
            if (ss.EndsWith(","))
                ss = ss.Substring(0, ss.Length - 1);
            Builds = ss;
        }
        #endregion

        #region Comparable
        public int CompareTo(object o)
        {
            if (o.GetType() != GetType())
            {
                return GetObjectType().CompareTo(((PublishObject)o).GetObjectType());
            }
            return Name.CompareTo(((PublishObject)o).Name);
        }
        #endregion
    };

    public enum TextPublishScope
    {
        General,
        Title,
        Description,
        ShortDescription,
        WhatsNew,
    };
    [XmlType("PublisherText"), XmlRoot("PublisherText")]
    public class TextPublish: PublishObject
    {
        [XmlAttribute()]
        public TextPublishScope Scope = TextPublishScope.General;
        [XmlAttribute()]
        public string Keywords = "";


        [XmlIgnore(), Description("The scope for which this publish object will be used"), Category("Target"), DisplayName("Scope")]
        public virtual TextPublishScope _Scope
        {
            get { return Scope; }
            set { Scope = value; }
        }

        public override string GetObjectType() { return "Text"; }
        public override string GetObjectFile() { return Path.Combine(GetObjectFolder(), "content.txt"); }
        public override string GetInformation(){ return "Scope:" + Scope.ToString() + " Keywords:" + Keywords;  }
    }


    public enum ImagePublishScope
    {
        General,
        Icon,
        Phone,
        Tablet7Inch,
        Tablet10Inch,
        TV,
        Publicity,
        Ads,
    };
    [XmlType("PublisherImage"), XmlRoot("PublisherImage")]
    public class ImagePublish : PublishObject
    {
        [XmlAttribute()]
        public ImagePublishScope Scope = ImagePublishScope.General;
        [XmlAttribute()]
        public string Size = "";


        [XmlIgnore(), Description("The scope for which this publish object will be used"), Category("Target"), DisplayName("Scope")]
        public virtual ImagePublishScope _Scope
        {
            get { return Scope; }
            set { Scope = value; }
        }

        public override string GetObjectType()  { return "Image"; }
        public override string GetObjectFile()  { return Path.Combine(GetObjectFolder(), "image.png"); }
        public override string GetInformation() { return "Scope:" + Scope.ToString() + " Size:" + Size; }

        public override string GetObjectSource()
        {
            string source = Path.Combine(GetObjectFolder(), "source.svg");
            if (File.Exists(source))
                return source;
            return "";
        }
    }

    public enum VideoPublishScope
    {
        General,
        Trailer,
        HowTo,
    };
    [XmlType("PublisherVideo"), XmlRoot("PublisherVideo")]
    public class VideoPublish : PublishObject
    {
        [XmlAttribute()]
        public VideoPublishScope Scope = VideoPublishScope.General;
        [XmlAttribute()]
        public string Size = "";


        [XmlIgnore(), Description("The scope for which this publish object will be used"), Category("Target"), DisplayName("Scope")]
        public virtual VideoPublishScope _Scope
        {
            get { return Scope; }
            set { Scope = value; }
        }

        public override string GetObjectType() { return "Video"; }
        public override string GetObjectFile() { return Path.Combine(GetObjectFolder(), "video.mp4"); }
        public override string GetInformation() { return "Scope:" + Scope.ToString() + " Size:" + Size; }

    }


    #endregion

    #region Debug Commands
    public enum DebugCommandParamType
    {
        None,
        Boolean,
        Int8,
        UInt8,
        Int16,
        UInt16,
        Int32,
        UInt32,
        Float32,
        Enum,
        Color,
    };
    [XmlType("DebugCommandParameter"), XmlRoot("DebugCommandParameter")]
    public class DebugCommandParam
    {
        [XmlAttribute()]
        public string Name = "";
        [XmlAttribute()]
        public string Description = "";
        [XmlAttribute()]
        public DebugCommandParamType Type = DebugCommandParamType.None;
        [XmlAttribute()]
        public string EnumValues = "";
    }
    [XmlType("DebugCommand"), XmlRoot("DebugCommand")]
    public class DebugCommand: IComparable
    {
        [XmlAttribute()]
        public string Name = "";
        [XmlAttribute()]
        public string Description = "";

        public List<DebugCommandParam> Parameters = new List<DebugCommandParam>();

        public int GetParametersPackedMemorySize()
        {
            int size = 0;
            foreach (DebugCommandParam p in Parameters)
            {
                switch (p.Type)
                {
                    case DebugCommandParamType.Boolean: 
                    case DebugCommandParamType.Int8:
                    case DebugCommandParamType.UInt8:
                    case DebugCommandParamType.Enum:
                        size++; 
                        break;
                    case DebugCommandParamType.Int16:
                    case DebugCommandParamType.UInt16:
                        size += 2;
                        break;
                    case DebugCommandParamType.Int32:
                    case DebugCommandParamType.UInt32:
                    case DebugCommandParamType.Float32:   
                    case DebugCommandParamType.Color:
                        size += 4;
                        break;
                    default:
                        size += 100;
                        break;
                }
            }
            return size;
        }

        public int CompareTo(object obj)
        {
            return Name.CompareTo(((DebugCommand)obj).Name);
        }
    }
    #endregion

    #region Develop settings snapshots
    [XmlType("SettingsSnapshot"), XmlRoot("SettingsSnapshot")]
    public class SettingsSnapshot
    {
        [XmlAttribute()]
        public int ID = 0, ParentID = 0;
        [XmlAttribute()]
        public string Info = "";
        [XmlAttribute]
        public DateTime Added = DateTime.Now;
    };
    public class SettingsSnapshotChildrenList
    {
        public SettingsSnapshot Snapshot = null;
        public List<SettingsSnapshot> Children = new List<SettingsSnapshot>();
    };
    [XmlType("DeveloperSettingsSnapshots"), XmlRoot("DeveloperSettingsSnapshots")]
    public class DeveloperSettingsSnapshots
    {
        public List<SettingsSnapshot> Snapshots = new List<SettingsSnapshot>();

        public int GetNewID()
        {
            int biggest = 0;
            foreach (SettingsSnapshot ds in Snapshots)
            {
                if (ds.ID>biggest)
                    biggest = ds.ID;
            }
            return biggest+1;
        }
        public void AddNewSnapshot(int id,int parentID,string message)
        {
            SettingsSnapshot ds = new SettingsSnapshot();
            ds.ID = id;
            ds.ParentID = parentID;
            ds.Info = message;
            Snapshots.Add(ds);
        }
        public SettingsSnapshot GetSnapshotFromID(int snapshotID)
        {
            foreach (SettingsSnapshot ds in Snapshots)
            {
                if (ds.ID == snapshotID)
                    return ds;
            }
            return null;
        }
        public string GetStringRepresentation(SettingsSnapshot ds)
        {
            return ds.Info + " <ID:" + ds.ID.ToString() + ">";
        }
        public string GetStringRepresentation(int snapshotID)
        {
            if (snapshotID == 0)
                return "Last state";
            if (snapshotID < 0)
                return "None";
            SettingsSnapshot ds = GetSnapshotFromID(snapshotID);
            if (ds != null)
                return GetStringRepresentation(ds);
            return "Last state";
        }
        public int GetSnapshotID(string stringRepresentation)
        {
            if (stringRepresentation.Equals("None", StringComparison.InvariantCultureIgnoreCase))
                return -1;
            if (stringRepresentation.Equals("Last state", StringComparison.InvariantCultureIgnoreCase))
                return 0;
            int index = stringRepresentation.LastIndexOf("<ID:");
            if (index < 0)
                return 0; // ultima stare
            index+=4;
            int ind2 = stringRepresentation.IndexOf(">", index);
            if (ind2 < 0)
                return 0; // ultima stare
            int snapshotID = 0;
            if (int.TryParse(stringRepresentation.Substring(index,ind2-index),out snapshotID))
            {
                if (GetSnapshotFromID(snapshotID) != null)
                    return snapshotID;
                // daca nu il am in lista
                return 0; // ultima stare
            }
            // altfel returnez tot ultima stare
            return 0;
        }
        public Dictionary<int, SettingsSnapshotChildrenList> GetRelations()
        {
            Dictionary<int, SettingsSnapshotChildrenList> d = new Dictionary<int, SettingsSnapshotChildrenList>();

            foreach (SettingsSnapshot sd in Snapshots)
            {
                if (d.ContainsKey(sd.ID) == false)
                    d[sd.ID] = new SettingsSnapshotChildrenList();
                d[sd.ID].Snapshot = sd;
                // link la parinte
                if (d.ContainsKey(sd.ParentID) == false)
                    d[sd.ParentID] = new SettingsSnapshotChildrenList();
                d[sd.ParentID].Children.Add(sd);
            }

            return d;
        }
    }
    #endregion

    #region Controls IDs
    [XmlType("ControlID"), XmlRoot("ControlID")]
    public class ControlID : IComparable
    {
        [XmlAttribute()]
        public string Name = "";
        [XmlAttribute()]
        public int ID = 0;
        [XmlAttribute()]
        public string Description = "";

        public int CompareTo(object obj)
        {
            return Name.CompareTo(((ControlID)obj).Name);
        }
    }

    #endregion

    #region Animation Object
    namespace AnimO
    {
        public enum BoardViewMode
        {
            Design,
            Play
        };
        public interface IRefreshDesign
        {
            void Refresh();
        }
        public enum Coordinates
        {
            Pixels,
            Percentage,
        }
        public class RuntimeContext
        {
            // toate informatiile despre un element care vor fi necesare in timpul transformarii
            public float ScaleWidth = 1.0f;
            public float ScaleHeight = 1.0f;
            public float X_Percentage = 0;
            public float Y_Percentage = 0;
            public float WidthInPixels = 0;
            public float HeightInPixels = 0;
            public Alignament Align = Alignament.TopLeft;
            public UInt32 ColorBlending = 0xFFFFFFFF;
            public Bitmap Image = null;
            public bool Visible = true;
            public RectangleF boundRect_ = new RectangleF();
            public RectangleF ScreenRect = new RectangleF();
            public static uint BlendModeToColor(BlendingMode mode, int color, float alpha)
            {
                switch (mode)
                {
                    case BlendingMode.ColorBlending: return (uint)color;
                    case BlendingMode.AlphaBlending: return ((uint)(alpha * 255)) << 24 | (((uint)0xFFFFFF) & ((uint)color));
                    default: return 0xFFFFFFFF;
                }
            }
        }

        public class Canvas
        {
            private Pen tempPen = new Pen(System.Drawing.Color.Black, 1);
            private SolidBrush tempBrush = new SolidBrush(System.Drawing.Color.Black);

            int left, top, width, height;
            float scale;
            ColorMatrix cm = new ColorMatrix();
            ImageAttributes imageAttributes = new ImageAttributes();
            Rectangle imageRect = new Rectangle();
            Graphics internalGraphics = null;
            SolidBrush brush = new SolidBrush(Color.White);
            RectangleF tempRectF = new RectangleF();


            public float ConvertXAxisToPercentage(int x)
            {
                return ((float)x) / ((float)width);
            }
            public float ConvertYAxisToPercentage(int y)
            {
                return ((float)y) / ((float)height);
            }
            public float ConvertXAxisToPercentage(float x)
            {
                return x / ((float)width);
            }
            public float ConvertYAxisToPercentage(float y)
            {
                return y / ((float)height);
            }
            private void ComputeRectOnScreen(float x, float y, float objWidthInPixels, float objHeightInPixels, Alignament align, ref RectangleF resultedRectInPixels, bool fixedSize = false, float scaleWidth = 1.0f, float scaleHeight = 1.0f)
            {
                float xPoz = x * width + left;
                float yPoz = y * height + top;
                float w, h;
                if (fixedSize)
                {
                    w = objWidthInPixels;
                    h = objHeightInPixels;
                }
                else
                {
                    w = objWidthInPixels * scale * scaleWidth;
                    h = objHeightInPixels * scale * scaleHeight;
                }
                switch (align)
                {
                    case Alignament.Center: xPoz -= w / 2; yPoz -= h / 2; break;
                    case Alignament.TopLeft: break;
                    case Alignament.TopCenter: xPoz -= w / 2; break;
                    case Alignament.TopRight: xPoz -= w; break;
                    case Alignament.RightCenter: xPoz -= w; yPoz -= h / 2; break;
                    case Alignament.BottomRight: xPoz -= w; yPoz -= h; break;
                    case Alignament.BottomCenter: xPoz -= w / 2; yPoz -= h; break;
                    case Alignament.BottomLeft: yPoz -= h; break;
                    case Alignament.LeftCenter: yPoz -= h / 2; break;
                }
                resultedRectInPixels.X = xPoz;
                resultedRectInPixels.Y = yPoz;
                resultedRectInPixels.Width = w;
                resultedRectInPixels.Height = h;
            }
            private void TranslateScreenRectToScaledScreenRect(RectangleF screenRect, ref RectangleF result, bool fixedSize = false)
            {              
                if (fixedSize)
                {
                    result.Width = screenRect.Width;
                    result.Height = screenRect.Height;
                } else
                {
                    result.Width = screenRect.Width * scale;
                    result.Height = screenRect.Height * scale;
                }
                result.X = screenRect.X * scale + left;
                result.Y = screenRect.Y * scale + top;
            }
            private void TranslateScreenRectToScaledScreenRect(RuntimeContext rContext, ref RectangleF result, bool fixedSize = false)
            {
                TranslateScreenRectToScaledScreenRect(rContext.ScreenRect, ref result, fixedSize);
            }
            private void ComputeRectOnScreen(RuntimeContext rContext, bool useImageMemberForWidth, ref RectangleF resultedRectInPixels)
            {
                if (useImageMemberForWidth)
                    ComputeRectOnScreen(rContext.X_Percentage, rContext.Y_Percentage, rContext.Image.Width, rContext.Image.Height, rContext.Align, ref resultedRectInPixels, false, rContext.ScaleWidth, rContext.ScaleHeight);
                else
                    ComputeRectOnScreen(rContext.X_Percentage, rContext.Y_Percentage, rContext.WidthInPixels, rContext.HeightInPixels, rContext.Align, ref resultedRectInPixels, false, rContext.ScaleWidth, rContext.ScaleHeight);
            }
            private void ComputeRectInPercentages(float x, float y, float objWidthInPixels, float objHeightInPixels, Alignament align, ref RectangleF resultedRectInPixels, bool fixedSize = false, float scaleWidth = 1.0f, float scaleHeight = 1.0f)
            {
                float xPoz = x;
                float yPoz = y;
                float w, h;
                if (fixedSize)
                {
                    w = objWidthInPixels;
                    h = objHeightInPixels;
                }
                else
                {
                    // calculez pozitia in procente - nu ma intereseaza scale-ul in acest punct
                    w = objWidthInPixels * scaleWidth;
                    h = objHeightInPixels * scaleHeight;
                }
                w = w / ((float)width/scale);
                h = h / ((float)height/scale);
                switch (align)
                {
                    case Alignament.Center: xPoz -= w / 2; yPoz -= h / 2; break;
                    case Alignament.TopLeft: break;
                    case Alignament.TopCenter: xPoz -= w / 2; break;
                    case Alignament.TopRight: xPoz -= w; break;
                    case Alignament.RightCenter: xPoz -= w; yPoz -= h / 2; break;
                    case Alignament.BottomRight: xPoz -= w; yPoz -= h; break;
                    case Alignament.BottomCenter: xPoz -= w / 2; yPoz -= h; break;
                    case Alignament.BottomLeft: yPoz -= h; break;
                    case Alignament.LeftCenter: yPoz -= h / 2; break;
                }
                resultedRectInPixels.X = xPoz;
                resultedRectInPixels.Y = yPoz;
                resultedRectInPixels.Width = w;
                resultedRectInPixels.Height = h;
            }
            public void ComputeRectInPercentages(RuntimeContext rContext, bool useImageMemberForWidth, ref RectangleF resultedRectInPercentages)
            {
                if (useImageMemberForWidth)
                {
                    float w = 0;
                    float h = 0;
                    if (rContext.Image!=null)
                    {
                        w = rContext.Image.Width;
                        h = rContext.Image.Height;
                    }
                    ComputeRectInPercentages(rContext.X_Percentage, rContext.Y_Percentage, w, h, rContext.Align, ref resultedRectInPercentages, false, rContext.ScaleWidth, rContext.ScaleHeight);
                }
                else
                    ComputeRectInPercentages(rContext.X_Percentage, rContext.Y_Percentage, rContext.WidthInPixels, rContext.HeightInPixels, rContext.Align, ref resultedRectInPercentages, false, rContext.ScaleWidth, rContext.ScaleHeight);
            }



            #region General Settings
            public void SetGraphics(Graphics g)
            {
                internalGraphics = g;
            }
            public void SetScreen(int _left, int _top, int _width, int _height, float _scale)
            {
                left = _left;
                top = _top;
                width = _width;
                height = _height;
                scale = _scale;
            }
            public int GetWidth() { return width; }
            public int GetHeight() { return height; }
            public float GetScale() { return scale; }
            public float GetLeft() { return left; }
            public float GetTop() { return top; }
            #endregion

            #region Image
            //public void GetLastImageRect(ref Rectangle resultedImageRect)
            //{
            //    resultedImageRect.X = (int)(lastXPoz);
            //    resultedImageRect.Y = (int)(lastYPoz);
            //    resultedImageRect.Width = (int)(imageRect.Width / scale);
            //    resultedImageRect.Height = (int)(imageRect.Height / scale);
            //}
            //public void DrawImage_(Image img, float x, float y, Alignament align, uint ColorBlending = 0xFFFFFFFF, float scaleWidth = 1.0f, float scaleHeight = 1.0f)
            //{
            //    if (internalGraphics == null)
            //        return;
            //    if (img == null)
            //        return;
            //    ComputeRectOnScreen(x, y, img.Width, img.Height, align, ref tempRectF, false, scaleWidth, scaleHeight);
            //    cm.Matrix00 = ((float)((ColorBlending >> 16) & 0xFF)) / 255.0f; // RED
            //    cm.Matrix11 = ((float)((ColorBlending >> 8) & 0xFF)) / 255.0f; // GREEN
            //    cm.Matrix22 = ((float)((ColorBlending >> 0) & 0xFF)) / 255.0f; // BLUE 
            //    cm.Matrix33 = ((float)((ColorBlending >> 24) & 0xFF)) / 255.0f; // ALPHA                
            //    imageAttributes.SetColorMatrix(cm, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            //    imageRect.X = (int)tempRectF.X;
            //    imageRect.Y = (int)tempRectF.Y;
            //    imageRect.Width = (int)tempRectF.Width;
            //    imageRect.Height = (int)tempRectF.Height;
            //    internalGraphics.DrawImage(img, imageRect, 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, imageAttributes);
            //}
            public void DrawImage(Image img, RectangleF screenPozition, uint ColorBlending = 0xFFFFFFFF)
            {
                if (internalGraphics == null)
                    return;
                if (img == null)
                    return;
                TranslateScreenRectToScaledScreenRect(screenPozition, ref tempRectF, false);
                imageRect.X = (int)tempRectF.X;
                imageRect.Y = (int)tempRectF.Y;
                imageRect.Width = (int)tempRectF.Width;
                imageRect.Height = (int)tempRectF.Height;
                cm.Matrix00 = ((float)((ColorBlending >> 16) & 0xFF)) / 255.0f; // RED
                cm.Matrix11 = ((float)((ColorBlending >> 8) & 0xFF)) / 255.0f; // GREEN
                cm.Matrix22 = ((float)((ColorBlending >> 0) & 0xFF)) / 255.0f; // BLUE 
                cm.Matrix33 = ((float)((ColorBlending >> 24) & 0xFF)) / 255.0f; // ALPHA                
                imageAttributes.SetColorMatrix(cm, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                internalGraphics.DrawImage(img, imageRect, 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, imageAttributes);
            }
            public void DrawImage(RuntimeContext rContext)
            {
                DrawImage(rContext.Image, rContext.ScreenRect, rContext.ColorBlending);
                //DrawImage(rContext.Image, rContext.X_Percentage, rContext.Y_Percentage, rContext.Align, rContext.ColorBlending, rContext.ScaleWidth, rContext.ScaleHeight);
            }
            public void FillScreen(Image img)
            {
                ClearScreen((int)0x7FFF8040);
            }
            #endregion


            #region Clipping
            public void EnableClipping(RuntimeContext rContext)
            {
                if (internalGraphics == null)
                    return;
                ComputeRectOnScreen(rContext, false, ref tempRectF);
                internalGraphics.SetClip(tempRectF);
            }
            public void ClearClipping(Graphics g)
            {
                g.ResetClip();
            }
            public void ClearClipping()
            {
                ClearClipping(internalGraphics);
            }
            #endregion

            #region Rectangle Functions
            public void ClearScreen(int aRGB)
            {
                DrawRect(0, 0, 1, 1, Alignament.TopLeft, 0, aRGB, 0);
            }


            public void DrawRect(RectangleF screenPozition, int borderColor, int fillColor, float penWidth, bool fixedSize = false)
            {
                if (internalGraphics == null)
                    return;
                TranslateScreenRectToScaledScreenRect(screenPozition, ref tempRectF, fixedSize);

                if (fillColor != 0) // color.Transparent
                {
                    tempBrush.Color = System.Drawing.Color.FromArgb(fillColor);
                    internalGraphics.FillRectangle(tempBrush, tempRectF);
                }
                if ((borderColor != 0) && (penWidth > 0))
                {
                    tempPen.Color = System.Drawing.Color.FromArgb(borderColor);
                    tempPen.Width = penWidth;
                    internalGraphics.DrawRectangle(tempPen, tempRectF.X, tempRectF.Y, tempRectF.Width, tempRectF.Height);
                }
            }
            public void DrawRect(RuntimeContext rContext, int borderColor, int fillColor, float penWidth, bool fixedSize = false)
            {
                DrawRect(rContext.ScreenRect, borderColor, fillColor, penWidth, fixedSize);
            }

            public void DrawRect(float x, float y, float rectWithInPixels, float rectHeightInPixels, Alignament align, int borderColor, int fillColor, float penWidth, float scaleWidth = 1, float scaleHeight = 1, bool fixedSize = false)
            {
                if (internalGraphics == null)
                    return;
                ComputeRectOnScreen(x, y, rectWithInPixels, rectHeightInPixels, align, ref tempRectF, fixedSize, scaleWidth, scaleHeight);

                if (fillColor != 0) // color.Transparent
                {
                    tempBrush.Color = System.Drawing.Color.FromArgb(fillColor);
                    internalGraphics.FillRectangle(tempBrush, tempRectF);
                }
                if ((borderColor != 0) && (penWidth > 0))
                {
                    tempPen.Color = System.Drawing.Color.FromArgb(borderColor);
                    tempPen.Width = penWidth;
                    internalGraphics.DrawRectangle(tempPen, tempRectF.X, tempRectF.Y, tempRectF.Width, tempRectF.Height);
                }
            }
            public void DrawRectWithPixelsCoordonates(int left,int top, int right, int bottom, int borderColor, int fillColor, int penWidth)
            {
                DrawRect(ConvertXAxisToPercentage(left), ConvertYAxisToPercentage(top), right-left, bottom-top, Alignament.TopLeft, borderColor, fillColor, penWidth);
            }
            public void DrawObjectRect(RuntimeContext rContext, bool useImageForWidth, int borderColor)
            {
                if ((useImageForWidth) && (rContext.Image != null))
                    this.DrawRect(rContext.X_Percentage, rContext.Y_Percentage, rContext.Image.Width, rContext.Image.Height, rContext.Align, borderColor, 0, 1, rContext.ScaleWidth, rContext.ScaleHeight);
                else
                    this.DrawRect(rContext.X_Percentage, rContext.Y_Percentage, rContext.WidthInPixels, rContext.HeightInPixels, rContext.Align, borderColor, 0, 1, rContext.ScaleWidth, rContext.ScaleHeight);
            }

            public void FillRect(RuntimeContext rContext)
            {
                DrawRect(rContext, 0, (int)rContext.ColorBlending, 0);
            }

            public void FillExclusionRect(RuntimeContext rContext)
            {
                if (internalGraphics == null)
                    return;
                //ComputeRectOnScreen(rContext, false, ref tempRectF);
                TranslateScreenRectToScaledScreenRect(rContext, ref tempRectF, false);

                brush.Color = Color.FromArgb((int)rContext.ColorBlending);
                int l = (int)tempRectF.Left;
                int t = (int)tempRectF.Top;
                int r = (int)tempRectF.Right;
                int b = (int)tempRectF.Bottom;

                if ((l > left + this.width) || (r < left) || (t > top + this.height) || (b < top))
                {
                    // fac un fill la ecran si gata
                    internalGraphics.FillRectangle(brush, left, top, width, height);
                    return;
                }
                // altfel am o zona comuna
                if (t < top)
                    t = top;
                if (l < left)
                    l = left;
                if (b > top + height)
                    b = top + height;
                if (r > left + width)
                    r = left + width;

                internalGraphics.FillRectangle(brush, left, top, width, t - top);
                internalGraphics.FillRectangle(brush, left, b, width, top + height - b);
                if (l > left)
                {
                    internalGraphics.FillRectangle(brush, left, t, l - left, b - t);
                }
                if (r < left + width)
                {
                    internalGraphics.FillRectangle(brush, r, t, left + width - r, b - t);
                }

            }
            #endregion

            #region Line function
            private void DrawLine(float x1, float y1, float x2, float y2, int lineColor, int lineWidth)
            {
                if (internalGraphics == null)
                    return;
                ComputeRectOnScreen(x1, y1, 0, 0, Alignament.TopLeft, ref tempRectF, true);
                float _x1 = tempRectF.X;
                float _y1 = tempRectF.Y;
                ComputeRectOnScreen(x2, y2, 0, 0, Alignament.TopLeft, ref tempRectF, true);
                float _x2 = tempRectF.X;
                float _y2 = tempRectF.Y;
                tempPen.Color = System.Drawing.Color.FromArgb(lineColor);
                tempPen.Width = lineWidth;
                internalGraphics.DrawLine(tempPen, _x1, _y1, _x2, _y2);
            }
            public void DrawLineWithPixelsCoordonates(int x1,int y1,int x2,int y2,int lineColor,int lineWidth)
            {
                DrawLine(ConvertXAxisToPercentage(x1), ConvertYAxisToPercentage(y1), ConvertXAxisToPercentage(x2), ConvertYAxisToPercentage(y2), lineColor, lineWidth);
            }
            #endregion

            #region Ellipse Functions
            public void DrawEllipse(float x, float y, float rectWidth, float rectHeight, Alignament align, int borderColor, int fillColor, float penWidth, float scaleWidth = 1, float scaleHeight = 1, bool fixedSize = false)
            {
                if (internalGraphics == null)
                    return;
                ComputeRectOnScreen(x, y, rectWidth, rectHeight, align, ref tempRectF, fixedSize, scaleWidth, scaleHeight);

                if (fillColor != 0) // color.Transparent
                {
                    tempBrush.Color = System.Drawing.Color.FromArgb(fillColor);
                    internalGraphics.FillEllipse(tempBrush, tempRectF);
                }
                if ((borderColor != 0) && (penWidth > 0))
                {
                    tempPen.Color = System.Drawing.Color.FromArgb(borderColor);
                    tempPen.Width = penWidth;
                    internalGraphics.DrawEllipse(tempPen, tempRectF.X, tempRectF.Y, tempRectF.Width, tempRectF.Height);
                }
            }

            //public void DrawEllipse(int left, int top, int right, int bottom, int borderColor)
            //{
            //    g_DrawEllipse(left, top, right - left, bottom - top, borderColor, 0, 1);
            //}
            //public void DrawEllipseWH(int left, int top, int width, int height, int borderColor)
            //{
            //    g_DrawEllipse(left, top, width, height, borderColor, 0, 1);
            //}
            //public void DrawEllipse(GRect rect, int borderColor)
            //{
            //    g_DrawEllipse(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top, borderColor, 0, 1);
            //}
            //public void DrawEllipse(int left, int top, int right, int bottom, int borderColor, int fillColor, int penWidth)
            //{
            //    g_DrawEllipse(left, top, right - left, bottom - top, borderColor, fillColor, penWidth);
            //}
            //public void DrawEllipseWH(int left, int top, int width, int height, int borderColor, int fillColor, int penWidth)
            //{
            //    g_DrawEllipse(left, top, width, height, borderColor, fillColor, penWidth);
            //}
            //public void DrawEllipse(GRect rect, int borderColor, int fillColor, int penWidth)
            //{
            //    g_DrawEllipse(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top, borderColor, fillColor, penWidth);
            //}

            #endregion

            #region Circle Functions
            public void DrawCircle(float x, float y, float ray, int borderColor, bool fixedSize = true)
            {
                DrawEllipse(x, y, ray * 2, ray * 2, Alignament.Center, borderColor, 0, 1, 1.0f, 1.0f, fixedSize);
            }
            public void DrawCircle(int x, int y, int ray, int borderColor, int fillColor, int penWidth, bool fixedSize = true)
            {
                DrawEllipse(x, y, ray * 2, ray * 2, Alignament.Center, borderColor, fillColor, penWidth, 1.0f, 1.0f, fixedSize);
            }
            #endregion

        }


        #region Transformations

        public interface ITransformationEvent
        {
            void OnEvent(GenericTransformation trans);
        }

        [XmlInclude(typeof(TouchBoundaryTransformation))]
        [XmlInclude(typeof(TouchStatusTransformation))]
        [XmlInclude(typeof(CountDown))]
        [XmlInclude(typeof(NumberIncreaseTransformation))]
        [XmlInclude(typeof(NumericFormatterTransformation))]
        [XmlInclude(typeof(TextCharacterVisibilityTransformation))]
        [XmlInclude(typeof(AlphaBlendingStateTransformation))]
        [XmlInclude(typeof(FontSizeTransformation))]
        [XmlInclude(typeof(SetNewRelativePositionTransformation))]
        [XmlInclude(typeof(SetNewAbsolutePositionTransformation))]
        [XmlInclude(typeof(Stopper))]
        [XmlInclude(typeof(BranchBlock))]
        [XmlInclude(typeof(IfElseBlock))]
        [XmlInclude(typeof(TextCenterFlowTransformation))]
        [XmlInclude(typeof(TextFlowTransformation))]
        [XmlInclude(typeof(SetNewImageTransformation))]
        [XmlInclude(typeof(SetImageIndexTransformation))]
        [XmlInclude(typeof(ImageIndexLinearTransformation))]
        [XmlInclude(typeof(SetNewTextTransformation))]
        [XmlInclude(typeof(ColorBlendStateTransformation))]
        [XmlInclude(typeof(ZOrderTransformation))]
        [XmlInclude(typeof(VisibleStateTransformation))]
        [XmlInclude(typeof(ButtonEnableTransformation))]        
        [XmlInclude(typeof(QuadraticBezierTransformation))]
        [XmlInclude(typeof(MoveAbsoluteLinearTransformation))]
        [XmlInclude(typeof(MoveRelativeLinearTransformation))]
        [XmlInclude(typeof(ContinousBlock))]
        [XmlInclude(typeof(ColorBlendForwardAndBackTransformation))]
        [XmlInclude(typeof(ColorBlendLinearTransformation))]
        [XmlInclude(typeof(ScaleHeightForwardAndBackTransformation))]
        [XmlInclude(typeof(ScaleHeightLinearTransformation))]
        [XmlInclude(typeof(ScaleWidthForwardAndBackTransformation))]
        [XmlInclude(typeof(ScaleWidthLinearTransformation))]
        [XmlInclude(typeof(ScaleForwardAndBackTransformation))]
        [XmlInclude(typeof(ScaleLinearTransformation))]
        [XmlInclude(typeof(AlphaBlendingForwardAndBackTransformation))]
        [XmlInclude(typeof(AlphaBlendingLinearTransformation))]
        [XmlInclude(typeof(TransformationBlock))]
        [XmlInclude(typeof(RepeatBlock))]
        [XmlInclude(typeof(RepeatUntil))]
        [XmlInclude(typeof(DoOnceUntil))]
        [XmlInclude(typeof(PopupLoop))]
        [XmlInclude(typeof(DoOncePopupLoop))]
        [XmlInclude(typeof(WaitUntil))]
        [XmlInclude(typeof(SoundTransformation))]
        [XmlInclude(typeof(EventTransformation))]
        [XmlInclude(typeof(TimerTransformation))]
        [XmlInclude(typeof(GenericElementTransformation))]
        [XmlType("Transformation"), XmlRoot("Transformation")]
        public class GenericTransformation
        {
            [XmlAttribute()]
            public string BranchName = "";

            [XmlIgnore()]
            public string CppName = "";

            [XmlAttribute()]
            public bool Collapsed = false;

            [XmlIgnore()]
            public static ITransformationEvent TransEvent = null;
            [XmlIgnore()]
            public static Project Prj = null;

            #region Atribute
            [XmlIgnore(), Description("Name of the branch (only required if the transformation is part of a branch)"), Category("Branch"), DisplayName("Branch Name")]
            public string _BranchName
            {
                get { return BranchName; }
                set
                {
                    if ((Project.ValidateVariableNameCorectness(value, false) == false) && (value.Length>0))
                    {
                        MessageBox.Show("THis field must contain only letter, numbers and character '_'. The first character must be a letter !");
                        return;
                    }
                    BranchName = value;
                }
            }
            #endregion

            #region Virtual functions
            public virtual List<GenericTransformation> GetBlockTransformations()
            {
                return null;
            }
            public virtual string GetName()
            {
                return Factory.GetName(GetType());
            }
            public virtual string GetIconKey()
            {
                return Factory.GetIconKey(GetType());
            }
            #endregion

            #region Dynamic Execution
            protected GenericElement ElementObject = null;
            protected bool Started = false;
            public void SetElement(GenericElement element) { ElementObject = element; }
            public void Init() 
            {
                OnInit();
                Started = true;
            }
            public bool Update() 
            { 
                if (Started==false)
                    return false;
                if (OnUpdate()) // continuu
                    return true;
                Started = false;
                return false;
            }
            protected virtual bool OnUpdate() { return false; }
            protected virtual void OnInit() { }
            protected string GetLocationValue(float value)
            {
                return Project.ProcentToString(value);
            }
            protected float SetLocationValue(string strRepresentation, float currentValue)
            {
                return Project.StringToProcent(strRepresentation, -1000, 1000, currentValue);
            }
            protected string GetSizeInPixels(float value)
            {
                return value.ToString() + " px";
            }
            protected float SetSizeInPixels(string strRepresentation, float currentValue)
            {
                float result = 0.0f;
                if (float.TryParse(strRepresentation.ToLower().Replace("px", "").Trim(), out result))
                {
                    if (result >= 0)
                        return result;
                }
                return currentValue;
            }
            protected string GetOnStartFieldInit(string parameter,string localName)
            {
                if (parameter.Length > 0)
                    return "\n\t" + CppName + "." + localName + " = param_" + parameter + ";";
                return "";
            }

            public virtual string GetCPPClassName() { return "?"; }
            public virtual string CreateInitializationCPPCode() { return "\n???? - missing initialization code for: " + this.GetType().ToString() + "\n"; }
            public virtual string CreateOnStartCPPCode() 
            {
                return "";
            }
            public virtual void PopulateParameters(AnimationParameters p) 
            {
            }
            public virtual void Validate(Project prj, string animationName, AppResources resources, Dictionary<string, GenericElement> elements) { }
            public virtual void AddAnimationFunction(GACParser.Module m) { }
            public virtual string GetAnimationFunctionCPPImplementation(string className) { return ""; }
            public virtual string GetAnimationFunctionCPPHeaderDefinition() { return ""; }
            public virtual void CreateGACEnums(Dictionary<string, List<string>> enums) { }
            #endregion

            public string ToXMLString()
            {
                try
                {
                    var stringwriter = new System.IO.StringWriter();
                    var serializer = new XmlSerializer(typeof(GenericTransformation));
                    serializer.Serialize(stringwriter, this);
                    return stringwriter.ToString();
                }
                catch (Exception e)
                {
                    MessageBox.Show("Unable to convert frame to XML: \n" + e.ToString());
                    return "";
                }
            }
            public static GenericTransformation FromXMLString(string xmlText)
            {
                try
                {
                    var stringReader = new System.IO.StringReader(xmlText);
                    var serializer = new XmlSerializer(typeof(GenericTransformation));
                    GenericTransformation af = serializer.Deserialize(stringReader) as GenericTransformation;
                    if (af == null)
                    {
                        MessageBox.Show("Unable to create transformation from XML: \n" + xmlText);
                    }
                    return af;
                }
                catch (Exception e)
                {
                    MessageBox.Show("Unable to create transformation from XML: \n" + e.ToString());
                    return null;
                }
            }
        }

        [XmlType("Timer"), XmlRoot("Timer")]
        public class TimerTransformation : GenericTransformation
        {
            [XmlAttribute()]
            public int Times = 1;
            [XmlAttribute()]
            public string UserValue_Times = "";

            #region Atribute
            [XmlIgnore(), Description("Number of times to wait until the next transformation starts"), Category("Timer"), DisplayName("Times")]
            public int _Times
            {
                get { return Times; }
                set { if (value > 0) Times = value; }
            }

            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Times")]
            public string _UserValue_Times
            {
                get { return UserValue_Times; }
                set { UserValue_Times = value; }
            }
            #endregion

            #region Virtual functions
            public override string GetName()
            {
                return "Timer (Waits for "+Times.ToString()+" frames)";
            }
            public override string GetIconKey()
            {
                return "timer";
            }
            #endregion

            #region Dynamic Execution
            private int index;
            protected override void OnInit()
            {
                index = Times;
            }
            protected override bool OnUpdate()
            {
                index--;
                return index>0;
            }
            public override string GetCPPClassName() { return "Timer"; }
            public override string CreateInitializationCPPCode()
            {
                return "\n\t//Timer transformation\n\t" + this.CppName + ".NumberOfTimes = " + Times.ToString() + ";\n";
            }
            public override void PopulateParameters(AnimationParameters p)
            {
                if (UserValue_Times.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Times, "int"));
                base.PopulateParameters(p);
            }
            public override string CreateOnStartCPPCode()
            {
                return GetOnStartFieldInit(UserValue_Times, "NumberOfTimes") + base.CreateOnStartCPPCode();
            }
            #endregion
        }

        [XmlType("ZOrder"), XmlRoot("ZOrder")]
        public class ZOrderTransformation : GenericTransformation
        {
            [XmlAttribute()]
            public string ZOrder = "";
            [XmlIgnore()]
            public int ZOrderID = -1;

            #region Atribute
            [XmlIgnore(), Description("Sets a new ZOrder among existing element"), Category("Z-Order"), DisplayName("Z-Order"), Editor(typeof(AnimationZOrderEditor), typeof(UITypeEditor))]
            public string _ZOrder
            {
                get { return ZOrder; }
                set { ZOrder = value; }
            }
            #endregion

            #region Virtual functions
            public override string GetName()
            {
                return "ZOrder (" + ZOrder + ")";
            }
            public override string GetIconKey()
            {
                return "z_order";
            }
            #endregion

            #region Dynamic Execution
            protected override void OnInit()
            {
                if (TransEvent != null)
                    TransEvent.OnEvent(this);
            }
            protected override bool OnUpdate()
            {
                return false;
            }
            public override string GetCPPClassName() { return "ZOrder"; }
            public override string CreateInitializationCPPCode()
            {
                return "\n\t//ZOrder transformation\n\t" + this.CppName + ".ZOrderID = " + ZOrderID.ToString() + ";\n";
            }
            #endregion
        }

        [XmlType("TriggerEvent"), XmlRoot("TriggerEvent")]
        public class EventTransformation : GenericTransformation
        {
            [XmlAttribute()]
            public string EventID = "";
            [XmlAttribute()]
            public string UserValue_Event = "";

            [XmlIgnore()]
            public int EventIDValue = -1;

            #region Atribute
            [XmlIgnore(), Description("Event that will be triggered"), Category("Event"), DisplayName("Event"), Editor(typeof(EventIDSelectorEditor), typeof(UITypeEditor))]
            public string _EventID
            {
                get { return EventID; }
                set
                {
                    if (Project.ValidateVariableNameCorectness(value, false) == false)
                    {
                        MessageBox.Show("Invalid name for event - you can only use letters and numbers and character '_'. The first character must be a letter !");
                        return;
                    }
                    EventID = value;
                }
            }

            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Event")]
            public string _UserValue_Event
            {
                get { return UserValue_Event; }
                set { UserValue_Event = value; }
            }
            #endregion

            #region Virtual functions
            public override string GetName()
            {
                return "Trigger Event ("+EventID+")";
            }
            public override string GetIconKey()
            {
                return "event";
            }
            public override void Validate(Project prj, string animationName, AppResources resources, Dictionary<string, GenericElement> elements)
            {
                EventIDValue = -1;
                if (EventID.Length == 0)
                {
                    prj.EC.AddError("Animation - '"+animationName+"' : You have to add an event ID to an Event Transformation !");
                    return;
                }
                foreach (var e in prj.ObjectEventsIDs)
                    if (e.Name.Equals(EventID))
                    {
                        EventIDValue = e.ID;
                        return;
                    }
                prj.EC.AddError("Animation - '" + animationName + "' : Event '" + EventID + "' is not defined !");
            }
            #endregion

            #region Dynamic Execution
            protected override void OnInit()
            {
            }
            protected override bool OnUpdate()
            {
                return false;
            }
            public override string GetCPPClassName() { return "Event"; }
            public override string CreateInitializationCPPCode()
            {
                return "\n\t//Event transformation\n\t" + this.CppName + ".EventID = " + EventIDValue.ToString() + ";\n";
            }
            public override void PopulateParameters(AnimationParameters p)
            {
                if (UserValue_Event.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Event, "int"));
                base.PopulateParameters(p);
            }
            public override string CreateOnStartCPPCode()
            {
                return GetOnStartFieldInit(UserValue_Event, "EventID") + base.CreateOnStartCPPCode();
            }
            #endregion
        }

        [XmlType("TouchStatus"), XmlRoot("TouchStatus")]
        public class TouchStatusTransformation : GenericTransformation
        {
            [XmlAttribute()]
            public bool Enable = true;

            #region Atribute
            [XmlIgnore(), Description("Weather or not touch is enabled (application wise)"), Category("Touch"), DisplayName("Touch enabled")]
            public bool _Enable
            {
                get { return Enable; }
                set { Enable = value;}
            }
            #endregion

            #region Virtual functions
            public override string GetName()
            {
                if (Enable)
                    return "Application touch: enabled";
                else
                    return "Application touch: disabled";
            }
            public override string GetIconKey()
            {
                return "touch_status";
            }
            #endregion

            #region Dynamic Execution
            protected override void OnInit()
            {
            }
            protected override bool OnUpdate()
            {
                return false;
            }
            public override string GetCPPClassName() { return "TouchStatus"; }
            public override string CreateInitializationCPPCode()
            {
                return "\n\t//Touch status transformation\n\t" + this.CppName + ".TouchEnabled = " + Enable.ToString().ToLower() + ";\n";
            }
            #endregion
        }

        [XmlType("TouchBoundary"), XmlRoot("TouchBoundary")]
        public class TouchBoundaryTransformation : GenericTransformation
        {
            [XmlAttribute()]
            public Alignament Align = Alignament.Center;
            [XmlAttribute()]
            public float X = 0.5f;
            [XmlAttribute()]
            public float Y = 0.5f;
            [XmlAttribute()]
            public float Width = 50;
            [XmlAttribute()]
            public float Height = 50;
            [XmlAttribute()]
            public string UserValue_X = "", UserValue_Y = "", UserValue_ScaleWidth = "", UserValue_ScaleHeight = "", UserValue_Align = "", UserValue_ColorBlending = "", UserValue_Width = "", UserValue_Height = "";


            #region Atribute
            [XmlIgnore(), Description("Rectangle Aliganment"), Category("Layout"), DisplayName("Alignament")]
            public Alignament _Align
            {
                get { return Align; }
                set { Align = value; }
            }
            [XmlIgnore(), Description("Object initial location"), Category("Layout"), DisplayName("X")]
            public string _X
            {
                get { return GetLocationValue(X); }
                set { X = SetLocationValue(value, X); }
            }
            [XmlIgnore(), Description("Object initial location"), Category("Layout"), DisplayName("Y")]
            public string _Y
            {
                get { return GetLocationValue(Y); }
                set { Y = SetLocationValue(value, Y); }
            }
            [XmlIgnore(), Description("Scale width"), Category("Layout"), DisplayName("Width")]
            public string _Width
            {
                get { return GetSizeInPixels(Width); }
                set { Width = SetSizeInPixels(value, Width); }
            }
            [XmlIgnore(), Description("Scale height"), Category("Layout"), DisplayName("Height")]
            public string _Height
            {
                get { return GetSizeInPixels(Height); }
                set { Height = SetSizeInPixels(value, Height); }
            }

            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("X")]
            public string _UserValue_X
            {
                get { return UserValue_X; }
                set { UserValue_X = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Y")]
            public string _UserValue_Y
            {
                get { return UserValue_Y; }
                set { UserValue_Y = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Alignament")]
            public string _UserValue_Align
            {
                get { return UserValue_Align; }
                set { UserValue_Align = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Width")]
            public string _UserValue_Width
            {
                get { return UserValue_Width; }
                set { UserValue_Width = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Height")]
            public string _UserValue_Height
            {
                get { return UserValue_Height; }
                set { UserValue_Height = value; }
            }
            #endregion

            #region Virtual functions
            public override string GetName()
            {
                return "Touch boundary: "+ Align.ToString() + "(" + _X + "," + _Y + ") Size = [" + _Width + "px x " + _Height + "px]";
            }
            public override string GetIconKey()
            {
                return "touch_boundary";
            }
            #endregion

            #region Dynamic Execution
            protected override void OnInit()
            {
            }
            protected override bool OnUpdate()
            {
                return false;
            }
            public override string GetCPPClassName() { return "TouchBoundary"; }
            public override string CreateInitializationCPPCode()
            {
                string s = "\n\t//Touch boundary transformation\n\t";
                s += "\t" + this.CppName + ".X = " + X.ToString() + ";\n";
                s += "\t" + this.CppName + ".Y = " + Y.ToString() + ";\n";
                s += "\t" + this.CppName + ".Width = " + Width.ToString() + ";\n";
                s += "\t" + this.CppName + ".Height = " + Height.ToString() + ";\n";
                s += "\t" + this.CppName + ".Alignament = GAC_ALIGNAMENT_" + this.Align.ToString().ToUpper() + ";\n";
                return s;
            }
            public override void PopulateParameters(AnimationParameters p)
            {
                if (UserValue_X.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_X, "float"));
                if (UserValue_Y.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Y, "float"));
                if (UserValue_Width.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Width, "float"));
                if (UserValue_Height.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Height, "float"));
                if (UserValue_Align.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Align, "unsigned int", "Alignament"));
                base.PopulateParameters(p);
            }
            public override string CreateOnStartCPPCode()
            {
                return GetOnStartFieldInit(UserValue_X, "X") + GetOnStartFieldInit(UserValue_Y, "Y") + GetOnStartFieldInit(UserValue_Align, "Alignament") + GetOnStartFieldInit(UserValue_Width, "Width") + GetOnStartFieldInit(UserValue_Height, "Height") + base.CreateOnStartCPPCode();
            }
            #endregion
        }

        [XmlType("PlaySound"), XmlRoot("PlaySound")]
        public class SoundTransformation : GenericTransformation
        {
            [XmlAttribute()]
            public string SoundName = "";
            [XmlAttribute()]
            public string UserValue_Sound = "";


            #region Atribute
            [XmlIgnore(), Description("Sound that will be played"), Category("Sound"), DisplayName("Sound"), Editor(typeof(SoundSelectorEditor), typeof(UITypeEditor))]
            public string _SoundName
            {
                get { return SoundName; }
                set { SoundName = value; }
            }

            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Sound")]
            public string _UserValue_Sound
            {
                get { return UserValue_Sound; }
                set { UserValue_Sound = value; }
            }
            #endregion

            #region Virtual functions
            public override string GetName()
            {
                return "Play sound (" + SoundName + ")";
            }
            public override string GetIconKey()
            {
                return "sound";
            }
            public override void Validate(Project prj, string animationName, AppResources resources, Dictionary<string, GenericElement> elements)
            {
                if (SoundName.Length == 0)
                {
                    prj.EC.AddError("Animation - '" + animationName + "' : You have to add a sound to be played !");
                    return;
                }
            }
            #endregion

            #region Dynamic Execution
            protected override void OnInit()
            {
            }
            protected override bool OnUpdate()
            {
                return false;
            }
            public override string GetCPPClassName() { return "SoundPlay"; }
            public override string CreateInitializationCPPCode()
            {
                return "\n\t//Sound transformation\n\t" + this.CppName + ".sound = Res.Sounds." + SoundName + ";\n";
            }
            public override void PopulateParameters(AnimationParameters p)
            {
                if (UserValue_Sound.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Sound, "GApp::Resources::Sound*", "Sound"));
                base.PopulateParameters(p);
            }
            public override string CreateOnStartCPPCode()
            {
                return GetOnStartFieldInit(UserValue_Sound, "sound") + base.CreateOnStartCPPCode();
            }
            #endregion
        }

        [XmlType("ElementTransformation"), XmlRoot("EelementTransformation")]
        public class GenericElementTransformation: GenericTransformation
        {
            [XmlAttribute()]
            public string Element = ""; // numele elementului pentru care se aplica transformarea AnimationElementEditor

            [XmlIgnore(), Description("Element to wich this transformation applies for."), Category("General"), DisplayName("Element"), Editor(typeof(AnimationElementEditor), typeof(UITypeEditor))]
            public string _Element
            {
                get { return Element; }
                set { Element = value; }
            }

            public override string GetName()
            {
                string s = GetTransformationDescription();
                if (s.Length==0)
                    return Factory.GetName(GetType()) +" (" + Element + ")";
                else
                    return Factory.GetName(GetType()) + " (" + Element + " => "+s+")";
            }
            protected virtual string GetTransformationDescription()
            {
                return "";
            }
            public override string GetCPPClassName()
            {
                string transformationName = this.GetType().ToString();
                int idx = transformationName.LastIndexOf('.');
                if (idx >= 0)
                    transformationName = transformationName.Substring(idx + 1);
                transformationName = transformationName.Replace("Transformation", "");
                return transformationName;
            }
        }

        [XmlType("Repeat"), XmlRoot("Repeat")]
        public class RepeatBlock : GenericTransformation
        {
            public List<GenericTransformation> Transformations = new List<GenericTransformation>(); // lista de transformari daca e un block
            [XmlAttribute()]
            public int Times = 1;
            [XmlAttribute()]
            public string UserValue_Times = "";

            #region Atribute
            [XmlIgnore(), Description("Number of times the content of this block will be repeated"), Category("Block"), DisplayName("Times")]
            public int _Times
            {
                get { return Times; }
                set { if (value>0) Times = value; }
            }

            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Times")]
            public string _UserValue_Times
            {
                get { return UserValue_Times; }
                set { UserValue_Times = value; }
            }
            #endregion

            #region Virtual functions
            public override List<GenericTransformation> GetBlockTransformations()
            {
                return Transformations;
            }
            public override string GetName()
            {
                return "Repeat block";
            }
            public override string GetIconKey()
            {
                return "repeat_block";
            }
            public override void Validate(Project prj, string animationName, AppResources resources, Dictionary<string, GenericElement> elements)
            {
                if (Transformations.Count != 1)
                    prj.EC.AddError("Animation - '" + animationName + "' : Repeat blocks within an animation object must only have one child !");
            }
            #endregion

            #region Dynamic Execution
            private int index;
            private GenericTransformation transToRepeat;
            protected override void OnInit()
            {
                index = Times;
                if (Transformations.Count > 0)
                    transToRepeat = Transformations[0];
                else
                    transToRepeat = null;
                if (transToRepeat != null)
                    transToRepeat.Init();
            }
            protected override bool OnUpdate()
            {
                if (transToRepeat == null)
                    return false; // nu continue
                if (transToRepeat.Update()==false)
                {
                    index--;
                    if (index<=0)
                        return false;
                    // altfel o iau de la capat
                    transToRepeat.Init();
                }
                return true;
            }
            public override string GetCPPClassName() { return "Repeat"; }
            public override string CreateInitializationCPPCode()
            {
                return "\n\t//Repeat transformation\n\t" + this.CppName + ".NumberOfTimes = " + Times.ToString() + ";\n\t" + this.CppName + ".trans = &" + Transformations[0].CppName + ";\n";
            }
            public override void PopulateParameters(AnimationParameters p)
            {
                if (UserValue_Times.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Times, "int"));
                base.PopulateParameters(p);
            }
            public override string CreateOnStartCPPCode()
            {
                return GetOnStartFieldInit(UserValue_Times, "NumberOfTimes") + base.CreateOnStartCPPCode(); 
            }
            #endregion
        }

        [XmlType("Continous"), XmlRoot("Continous")]
        public class ContinousBlock : GenericTransformation
        {
            public List<GenericTransformation> Transformations = new List<GenericTransformation>(); // lista de transformari daca e un block

            #region Virtual functions
            public override List<GenericTransformation> GetBlockTransformations()
            {
                return Transformations;
            }
            public override string GetName()
            {
                return "Continous block";
            }
            public override string GetIconKey()
            {
                return "infinite_loop_block";
            }
            public override void Validate(Project prj, string animationName, AppResources resources, Dictionary<string, GenericElement> elements)
            {
                if (Transformations.Count != 1)
                    prj.EC.AddError("Animation - '" + animationName + "' : Continous/Infinitee loops within an animation object must only have one child !");
            }
            #endregion

            #region Dynamic Execution
            private GenericTransformation transToRepeat;
            protected override void OnInit()
            {
                if (Transformations.Count > 0)
                    transToRepeat = Transformations[0];
                else
                    transToRepeat = null;
                if (transToRepeat != null)
                    transToRepeat.Init();
            }
            protected override bool OnUpdate()
            {
                if (transToRepeat == null)
                    return false; // nu continue
                if (transToRepeat.Update() == false)
                {
                    // altfel o iau de la capat
                    transToRepeat.Init();
                }
                return true;
            }
            public override string GetCPPClassName() { return "Continous"; }
            public override string CreateInitializationCPPCode()
            {
                return "\n\t//Continous transformation\n\t" + this.CppName + ".trans = &" + Transformations[0].CppName + ";\n";
            }
            #endregion
        }

        [XmlType("Stopper"), XmlRoot("Stopper")]
        public class Stopper : GenericTransformation
        {

            #region Virtual functions
            public override string GetName()
            {
                return "Stopper";
            }
            public override string GetIconKey()
            {
                return "stop";
            }
            #endregion

            #region Dynamic Execution
            protected override void OnInit()
            {
            }
            protected override bool OnUpdate()
            {
                return true;
            }
            public override string GetCPPClassName() { return "Stopper"; }
            public override string CreateInitializationCPPCode()
            {
                return "";
            }
            #endregion
        }

        [XmlType("RepeatUntil"), XmlRoot("RepeatUntil")]
        public class RepeatUntil : GenericTransformation
        {
            public List<GenericTransformation> Transformations = new List<GenericTransformation>(); // lista de transformari daca e un block

            [XmlAttribute()]
            public string LoopExitTriggerName = "";
            [XmlAttribute()]
            public bool ForceExitOnTrigger = false;

            #region Atribute
            [XmlIgnore(), Description("Name of the trigger that if set the loop ends"), Category("Exit"), DisplayName("Exit trigger")]
            public string _LoopExitTriggerName
            {
                get { return LoopExitTriggerName; }
                set
                {
                    if (Project.ValidateVariableNameCorectness(value, false) == false)
                    {
                        MessageBox.Show("THis field must contain only letter, numbers and character '_'. The first character must be a letter !");
                        return;
                    }
                    LoopExitTriggerName = value;
                }
            }
            [XmlIgnore(), Description("If set to true, the Repeat...Until loop exits imediatelly when the trigger is set. Otherwise the loop will exit when the child transformation from within the loop ends."), Category("Exit"), DisplayName("Force exit on trigger")]
            public bool _ForceExitOnTrigger
            {
                get { return ForceExitOnTrigger; }
                set { ForceExitOnTrigger = value; }
            }
            #endregion

            #region Virtual functions
            public override List<GenericTransformation> GetBlockTransformations()
            {
                return Transformations;
            }
            public override string GetName()
            {
                return "Repeat ... Until block (Trigger: " + LoopExitTriggerName+")";
            }
            public override string GetIconKey()
            {
                return "repeat_until";
            }
            public override void Validate(Project prj, string animationName, AppResources resources, Dictionary<string, GenericElement> elements)
            {
                if (Transformations.Count != 1)
                    prj.EC.AddError("Animation - '" + animationName + "' : RepeatUntil loops within an animation object must only have one child !");
                if (Project.ValidateVariableNameCorectness(LoopExitTriggerName, false) == false)
                    prj.EC.AddError("Animation - '" + animationName + "' : RepeatUntil loop with invalid trigger name - should contain letter, numbers and character '_' and the first character must be a letter !");
                    
            }
            #endregion

            #region Dynamic Execution
            private GenericTransformation transToRepeat;
            protected override void OnInit()
            {
                if (Transformations.Count > 0)
                    transToRepeat = Transformations[0];
                else
                    transToRepeat = null;
                if (transToRepeat != null)
                    transToRepeat.Init();
            }
            protected override bool OnUpdate()
            {
                if (transToRepeat == null)
                    return false; // nu continue
                if (transToRepeat.Update() == false)
                {
                    // altfel o iau de la capat
                    //transToRepeat.Init();
                    return false; // in simulator fac o singura tura
                }
                return true;
            }
            public override string GetCPPClassName() { return "RepeatUntil"; }
            public override string CreateInitializationCPPCode()
            {
                return "\n\t//RepeatUntil transformation\n\t" + this.CppName + ".trans = &" + Transformations[0].CppName + ";\n\t" + this.CppName + ".ExitCondition = false;\n\t" + this.CppName + ".ForceExit = " + ForceExitOnTrigger.ToString().ToLower() + ";\n";
            }
            public override void AddAnimationFunction(GACParser.Module m)
            {
                string nm = "Trigger" + LoopExitTriggerName;
                m.Members[nm] = new GACParser.Member(m, GACParser.MemberType.Function, nm, nm, "void", "", null);
            }
            public override string GetAnimationFunctionCPPHeaderDefinition()
            {
                return "\n\tvoid Trigger" + LoopExitTriggerName + " ();\n";
            }
            public override string GetAnimationFunctionCPPImplementation(string className)
            {
                return "\nvoid " + className + "::Trigger" + LoopExitTriggerName + "() { this->" + this.CppName + ".ExitCondition = true; } \n";
            }
            public override string CreateOnStartCPPCode()
            {
                return "\n\t" + this.CppName + ".ExitCondition = false;\n";
            }
            #endregion
        }

        [XmlType("DoOnceUntil"), XmlRoot("DoOnceUntil")]
        public class DoOnceUntil : GenericTransformation
        {
            public List<GenericTransformation> Transformations = new List<GenericTransformation>(); // lista de transformari daca e un block

            [XmlAttribute()]
            public string LoopExitTriggerName = "";
            [XmlAttribute()]
            public bool ForceExitOnTrigger = true;

            #region Atribute
            [XmlIgnore(), Description("Name of the trigger that if set the loop ends"), Category("Exit"), DisplayName("Exit trigger")]
            public string _LoopExitTriggerName
            {
                get { return LoopExitTriggerName; }
                set
                {
                    if (Project.ValidateVariableNameCorectness(value, false) == false)
                    {
                        MessageBox.Show("This field must contain only letter, numbers and character '_'. The first character must be a letter !");
                        return;
                    }
                    LoopExitTriggerName = value;
                }
            }
            [XmlIgnore(), Description("If set to true, the Do...Until loop exits imediatelly when the trigger is set. Otherwise the loop will exit when the child transformation from within the loop ends."), Category("Exit"), DisplayName("Force exit on trigger")]
            public bool _ForceExitOnTrigger
            {
                get { return ForceExitOnTrigger; }
                set { ForceExitOnTrigger = value; }
            }
            #endregion

            #region Virtual functions
            public override List<GenericTransformation> GetBlockTransformations()
            {
                return Transformations;
            }
            public override string GetName()
            {
                return "Do Once... Until (Trigger: " + LoopExitTriggerName + ")";
            }
            public override string GetIconKey()
            {
                return "do_until";
            }
            public override void Validate(Project prj, string animationName, AppResources resources, Dictionary<string, GenericElement> elements)
            {
                if (Transformations.Count != 1)
                    prj.EC.AddError("Animation - '" + animationName + "' : DoOnceUntil within an animation object must only have one child !");
                if (Project.ValidateVariableNameCorectness(LoopExitTriggerName, false) == false)
                    prj.EC.AddError("Animation - '" + animationName + "' : DoOnceUntil with invalid trigger name - should contain letter, numbers and character '_' and the first character must be a letter !");

            }
            #endregion

            #region Dynamic Execution
            private GenericTransformation transToRepeat;
            protected override void OnInit()
            {
                if (Transformations.Count > 0)
                    transToRepeat = Transformations[0];
                else
                    transToRepeat = null;
                if (transToRepeat != null)
                    transToRepeat.Init();
            }
            protected override bool OnUpdate()
            {
                if (transToRepeat == null)
                    return false; // nu continue
                if (transToRepeat.Update() == false)
                {
                    // altfel o iau de la capat
                    //transToRepeat.Init();
                    return false; // in simulator fac o singura tura
                }
                return true;
            }
            public override string GetCPPClassName() { return "DoOnceUntil"; }
            public override string CreateInitializationCPPCode()
            {
                return "\n\t//DoOnceUntil transformation\n\t" + this.CppName + ".trans = &" + Transformations[0].CppName + ";\n\t" + this.CppName + ".ExitCondition = false;\n\t" + this.CppName + ".ForceExit = " + ForceExitOnTrigger.ToString().ToLower() + ";\n";
            }
            public override void AddAnimationFunction(GACParser.Module m)
            {
                string nm = "Trigger" + LoopExitTriggerName;
                m.Members[nm] = new GACParser.Member(m, GACParser.MemberType.Function, nm, nm, "void", "", null);
            }
            public override string GetAnimationFunctionCPPHeaderDefinition()
            {
                return "\n\tvoid Trigger" + LoopExitTriggerName + " ();\n";
            }
            public override string GetAnimationFunctionCPPImplementation(string className)
            {
                return "\nvoid " + className + "::Trigger" + LoopExitTriggerName + "() { this->" + this.CppName + ".ExitCondition = true; } \n";
            }
            public override string CreateOnStartCPPCode()
            {
                return "\n\t" + this.CppName + ".ExitCondition = false;\n";
            }
            #endregion
        }

        [XmlType("PopupLoop"), XmlRoot("PopupLoop")]
        public class PopupLoop : GenericTransformation
        {
            public List<GenericTransformation> Transformations = new List<GenericTransformation>(); // lista de transformari daca e un block

            [XmlAttribute()]
            public bool ForceExitOnTrigger = true;

            #region Atribute
            [XmlIgnore(), Description("If set to true, the Popup loop exits imediatelly when the trigger is set. Otherwise the loop will exit when the child transformation from within the loop ends."), Category("Exit"), DisplayName("Force exit on trigger")]
            public bool _ForceExitOnTrigger
            {
                get { return ForceExitOnTrigger; }
                set { ForceExitOnTrigger = value; }
            }
            #endregion

            #region Virtual functions
            public override List<GenericTransformation> GetBlockTransformations()
            {
                return Transformations;
            }
            public override string GetName()
            {
                return "Popup Loop";
            }
            public override string GetIconKey()
            {
                return "popup_loop";
            }
            public override void Validate(Project prj, string animationName, AppResources resources, Dictionary<string, GenericElement> elements)
            {
                if (Transformations.Count != 1)
                    prj.EC.AddError("Animation - '" + animationName + "' : PopupLoop within an animation object must only have one child !");                
            }
            #endregion

            #region Dynamic Execution
            private GenericTransformation transToRepeat;
            protected override void OnInit()
            {
                if (Transformations.Count > 0)
                    transToRepeat = Transformations[0];
                else
                    transToRepeat = null;
                if (transToRepeat != null)
                    transToRepeat.Init();
            }
            protected override bool OnUpdate()
            {
                if (transToRepeat == null)
                    return false; // nu continue
                if (transToRepeat.Update() == false)
                {
                    // altfel o iau de la capat
                    //transToRepeat.Init();
                    return false; // in simulator fac o singura tura
                }
                return true;
            }
            public override string GetCPPClassName() { return "PopupLoop"; }
            public override string CreateInitializationCPPCode()
            {
                return "\n\t//PopupLoop transformation\n\t" + this.CppName + ".trans = &" + Transformations[0].CppName + ";\n\t" + this.CppName + ".ForceExit = " + ForceExitOnTrigger.ToString().ToLower() + ";\n";
            }
            #endregion
        }

        [XmlType("DoOncePopupLoop"), XmlRoot("DoOncePopupLoop")]
        public class DoOncePopupLoop : GenericTransformation
        {
            public List<GenericTransformation> Transformations = new List<GenericTransformation>(); // lista de transformari daca e un block

            [XmlAttribute()]
            public bool ForceExitOnTrigger = true;

            #region Atribute
            [XmlIgnore(), Description("If set to true, the Popup loop exits imediatelly when the trigger is set. Otherwise the loop will exit when the child transformation from within the loop ends."), Category("Exit"), DisplayName("Force exit on trigger")]
            public bool _ForceExitOnTrigger
            {
                get { return ForceExitOnTrigger; }
                set { ForceExitOnTrigger = value; }
            }
            #endregion

            #region Virtual functions
            public override List<GenericTransformation> GetBlockTransformations()
            {
                return Transformations;
            }
            public override string GetName()
            {
                return "Do Once Popup Loop";
            }
            public override string GetIconKey()
            {
                return "do_once_popup_loop";
            }
            public override void Validate(Project prj, string animationName, AppResources resources, Dictionary<string, GenericElement> elements)
            {
                if (Transformations.Count != 1)
                    prj.EC.AddError("Animation - '" + animationName + "' : Do once PopupLoop within an animation object must only have one child !");
            }
            #endregion

            #region Dynamic Execution
            private GenericTransformation transToRepeat;
            protected override void OnInit()
            {
                if (Transformations.Count > 0)
                    transToRepeat = Transformations[0];
                else
                    transToRepeat = null;
                if (transToRepeat != null)
                    transToRepeat.Init();
            }
            protected override bool OnUpdate()
            {
                if (transToRepeat == null)
                    return false; // nu continue
                if (transToRepeat.Update() == false)
                {
                    // altfel o iau de la capat
                    //transToRepeat.Init();
                    return false; // in simulator fac o singura tura
                }
                return true;
            }
            public override string GetCPPClassName() { return "DoOncePopupLoop"; }
            public override string CreateInitializationCPPCode()
            {
                return "\n\t//DoOncePopupLoop transformation\n\t" + this.CppName + ".trans = &" + Transformations[0].CppName + ";\n\t" + this.CppName + ".ForceExit = " + ForceExitOnTrigger.ToString().ToLower() + ";\n";
            }
            #endregion
        }

        [XmlType("WaitUntil"), XmlRoot("WaitUntil")]
        public class WaitUntil : GenericTransformation
        {
            [XmlAttribute()]
            public string LoopExitTriggerName = "";

            #region Atribute
            [XmlIgnore(), Description("Name of the trigger that if set wait ends"), Category("Exit trigger"), DisplayName("Exit trigger")]
            public string _LoopExitTriggerName
            {
                get { return LoopExitTriggerName; }
                set
                {
                    if (Project.ValidateVariableNameCorectness(value, false) == false)
                    {
                        MessageBox.Show("This field must contain only letter, numbers and character '_'. The first character must be a letter !");
                        return;
                    }
                    LoopExitTriggerName = value;
                }
            }
            #endregion

            #region Virtual functions
            public override string GetName()
            {
                return "Wait Until (Trigger: " + LoopExitTriggerName + ")";
            }
            public override string GetIconKey()
            {
                return "wait_until";
            }
            public override void Validate(Project prj, string animationName, AppResources resources, Dictionary<string, GenericElement> elements)
            {
                if (Project.ValidateVariableNameCorectness(LoopExitTriggerName, false) == false)
                    prj.EC.AddError("Animation - '" + animationName + "' : WaitUntil with invalid trigger name - should contain letter, numbers and character '_' and the first character must be a letter !");

            }
            #endregion

            #region Dynamic Execution
            protected override void OnInit()
            {
            }
            protected override bool OnUpdate()
            {
                return false; // nu continue
            }
            public override string GetCPPClassName() { return "WaitUntil"; }
            public override string CreateInitializationCPPCode()
            {
                return "\n\t//WaitUntil transformation\n\t" + this.CppName + ".ExitCondition = false;\n";
            }
            public override void AddAnimationFunction(GACParser.Module m)
            {
                string nm = "Trigger" + LoopExitTriggerName;
                m.Members[nm] = new GACParser.Member(m, GACParser.MemberType.Function, nm, nm, "void", "", null);
            }
            public override string GetAnimationFunctionCPPHeaderDefinition()
            {
                return "\n\tvoid Trigger" + LoopExitTriggerName + " ();\n";
            }
            public override string GetAnimationFunctionCPPImplementation(string className)
            {
                return "\nvoid " + className + "::Trigger" + LoopExitTriggerName + "() { this->" + this.CppName + ".ExitCondition = true; } \n";
            }
            public override string CreateOnStartCPPCode()
            {
                return "\n\t" + this.CppName + ".ExitCondition = false;\n";
            }
            #endregion
        }

        [XmlType("IfElse"), XmlRoot("IfElse")]
        public class IfElseBlock : GenericTransformation
        {
            public List<GenericTransformation> Transformations = new List<GenericTransformation>(); // lista de transformari daca e un block

            [XmlAttribute()]
            public string ConditionParameterName = "";

            [XmlAttribute()]
            public bool EditModeConditionValue = true;

            #region Atribute
            [XmlIgnore(), Description("Name of the condition parameter"), Category("Branch"), DisplayName("Condition parameter name")]
            public string _ConditionParameterName
            {
                get { return ConditionParameterName; }
                set
                {
                    if (Project.ValidateVariableNameCorectness(value, false) == false)
                    {
                        MessageBox.Show("THis field must contain only letter, numbers and character '_'. The first character must be a letter !");
                        return;
                    }
                    ConditionParameterName = value;
                }
            }
            [XmlIgnore(), Description("Specify if in the edit mode condition should be considered 'True' or 'False'"), Category("Branch"), DisplayName("Edit mode condition value")]
            public bool _EditModeConditionValue
            {
                get { return EditModeConditionValue; }
                set { EditModeConditionValue = value; }
            }
            #endregion

            #region Virtual functions
            public override List<GenericTransformation> GetBlockTransformations()
            {
                return Transformations;
            }
            public override string GetName()
            {
                return "If..Else (Condition: " + EditModeConditionValue.ToString() + ")";
            }
            public override string GetIconKey()
            {
                return "if_else";
            }
            public override void Validate(Project prj, string animationName, AppResources resources, Dictionary<string, GenericElement> elements)
            {
                if (Transformations.Count == 0)
                    prj.EC.AddError("Animation - '" + animationName + "' : If..Else blocks  must contain at least one child !");
                if (Transformations.Count > 2)
                    prj.EC.AddError("Animation - '" + animationName + "' : If..Else blocks  cannot have more than two children ('then' branch and 'else' branch)");
                if ((Transformations.Count > 0) && (Transformations[0].BranchName != "Then"))
                    prj.EC.AddError("Animation - '" + animationName + "' : First brach from the If..Else block must be named 'Then' !");
                if ((Transformations.Count > 1) && (Transformations[1].BranchName != "Else"))
                    prj.EC.AddError("Animation - '" + animationName + "' : Second brach from the If..Else block must be named 'Else' !");
                if (Project.ValidateVariableNameCorectness(ConditionParameterName, false) == false)
                    prj.EC.AddError("Animation - '" + animationName + "' : Invalid condition parameter name '"+ConditionParameterName+ "'. This field must contain only letter, numbers and character '_'. The first character must be a letter !");
            }
            #endregion

            #region Dynamic Execution
            private GenericTransformation transToRepeat;
            protected override void OnInit()
            {
                transToRepeat = null;
                if ((EditModeConditionValue) && (Transformations.Count > 0))
                    transToRepeat = Transformations[0];
                if ((!EditModeConditionValue) && (Transformations.Count > 1))
                    transToRepeat = Transformations[1];
                if (transToRepeat != null)
                    transToRepeat.Init();
            }
            protected override bool OnUpdate()
            {
                if (transToRepeat == null)
                    return false; // nu continue
                if (transToRepeat.Update() == false)
                    return false; // in simulator fac o singura tura
                return true;
            }
            public override string GetCPPClassName() { return "IfElseBlock"; }
            public override string CreateInitializationCPPCode()
            {
                string s = "\n\t//If..Else transformation";
                if (Transformations.Count>0)
                    s += "\n\t" + this.CppName + ".then_branch = &" + Transformations[0].CppName + ";";
                else
                    s += "\n\t" + this.CppName + ".then_branch = NULL;";
                if (Transformations.Count > 1)
                    s += "\n\t" + this.CppName + ".else_branch = &" + Transformations[1].CppName + ";";
                else
                    s += "\n\t" + this.CppName + ".else_branch = NULL;";

                s += "\n\t" + this.CppName + ".IfElseCondition = true;\n";
                return s;
            }

            public override string CreateOnStartCPPCode()
            {
                return GetOnStartFieldInit(ConditionParameterName, "IfElseCondition") + base.CreateOnStartCPPCode();
            }
            public override void PopulateParameters(AnimationParameters p)
            {
                p.ParametersList.Add(new ParameterInformation(ConditionParameterName, "bool"));
                base.PopulateParameters(p);
            }

            #endregion
        }

        [XmlType("Branch"), XmlRoot("Branch")]
        public class BranchBlock : GenericTransformation
        {
            public List<GenericTransformation> Transformations = new List<GenericTransformation>(); // lista de transformari daca e un block

            [XmlAttribute()]
            public string BranchConditionsGroup = "";

            [XmlAttribute()]
            public string EditModeBranchExecution = "";

            #region Atribute
            [XmlIgnore(), Description("Name of branch group name"), Category("Branch"), DisplayName("Branch conditions group")]
            public string _BranchConditionsGroup
            {
                get { return BranchConditionsGroup; }
                set
                {
                    if (Project.ValidateVariableNameCorectness(value, false) == false)
                    {
                        MessageBox.Show("THis field must contain only letter, numbers and character '_'. The first character must be a letter !");
                        return;
                    }
                    BranchConditionsGroup = value;
                }
            }
            [XmlIgnore(), Description("Name of branch to follow on edit mode"), Category("Branch"), DisplayName("Edit mode branch to execute")]
            public string _EditModeBranchExecution
            {
                get { return EditModeBranchExecution; }
                set { EditModeBranchExecution = value; }
            }
            #endregion

            #region Virtual functions
            public override List<GenericTransformation> GetBlockTransformations()
            {
                return Transformations;
            }
            public override string GetName()
            {
                return "Branch (Class: " + BranchConditionsGroup + ") - Execute: "+EditModeBranchExecution;
            }
            public override string GetIconKey()
            {
                return "branch_block";
            }
            public override void Validate(Project prj, string animationName, AppResources resources, Dictionary<string, GenericElement> elements)
            {
                if (Transformations.Count == 0)
                    prj.EC.AddError("Animation - '" + animationName + "' : Branch blocks  must contain at least one child !");
                bool found_one = false;
                foreach (var t in Transformations)
                {
                    if (Project.ValidateVariableNameCorectness(t.BranchName, false) == false)
                        prj.EC.AddError("Animation - '" + animationName + "' : Branch block with a branch with an invalid name: '" + t.BranchName + "' - should contain letter, numbers and character '_' and the first character must be a letter !");
                    if (t.BranchName == EditModeBranchExecution)
                        found_one = true;
                }
                if (found_one == false)
                {
                    string res = "";
                    foreach (var t in Transformations)
                        res += t.BranchName + ",";
                    prj.EC.AddError("Animation - '" + animationName + "' : Branch block with invalid 'Edit modee branch to execute' field - should be one of the following: '"+res+"' !");
                }
                if (Project.ValidateVariableNameCorectness(BranchConditionsGroup, false) == false)
                    prj.EC.AddError("Animation - '" + animationName + "' : Branch block with invalid 'Branch conditions group' name - should contain letter, numbers and character '_' and the first character must be a letter !");
            }
            #endregion

            #region Dynamic Execution
            private GenericTransformation transToRepeat;
            protected override void OnInit()
            {
                transToRepeat = null;
                foreach (var t in Transformations)
                    if (t.BranchName == EditModeBranchExecution)
                    {
                        transToRepeat = t;
                        break;
                    }
                if (transToRepeat != null)
                    transToRepeat.Init();
            }
            protected override bool OnUpdate()
            {
                if (transToRepeat == null)
                    return false; // nu continue
                if (transToRepeat.Update() == false)
                    return false; // in simulator fac o singura tura
                return true;
            }
            public override string GetCPPClassName() { return "BranchBlock"; }
            public override string CreateInitializationCPPCode()
            {
                string s = "\n\t//Branch transformation\n\t" + this.CppName + ".list = new GApp::Animations::Transformations::Transformation* [" + Transformations.Count.ToString() + "];";
                s += "\n\t" + this.CppName + ".NumberOfTransformations = " + Transformations.Count.ToString() + ";";
                int index = 0;
                foreach (var t in Transformations)
                {
                    s += "\n\t" + this.CppName + ".list[" + index.ToString() + "] = &" + t.CppName + ";";
                    index++;
                }
                s += "\n\t" + this.CppName + ".BranchIndex = -1;\n\t"+this.CppName+".trans = NULL;\n";
                return s;
            }

            public override string CreateOnStartCPPCode()
            {
                return GetOnStartFieldInit(BranchConditionsGroup, "BranchIndex") + base.CreateOnStartCPPCode();
            }
            public override void PopulateParameters(AnimationParameters p)
            {
                p.ParametersList.Add(new ParameterInformation(BranchConditionsGroup, "int", BranchConditionsGroup+"Enum"));
                base.PopulateParameters(p);
            }
            public override void CreateGACEnums(Dictionary<string, List<string>> enums)
            {
                List<string> l = new List<string>();
                foreach (var t in Transformations)
                    l.Add(t.BranchName);
                enums[this.BranchConditionsGroup] = l;
            }

            #endregion
        }

        public enum TransformationBlockType
        {
            Sequance,
            Parallel,
        };

        [XmlType("Block"), XmlRoot("Block")]
        public class TransformationBlock:  GenericTransformation
        {
            public List<GenericTransformation> Transformations = new List<GenericTransformation>(); // lista de transformari daca e un block
            [XmlAttribute()]
            public TransformationBlockType BlockType = TransformationBlockType.Sequance;            

            #region Atribute
            [XmlIgnore(), Description("Describes how the elements within the block will be executed"), Category("Block"), DisplayName("Block type")]
            public TransformationBlockType _BlockType
            {
                get { return BlockType; }
                set { BlockType = value; }
            }
            #endregion

            #region Virtual functions
            public override List<GenericTransformation> GetBlockTransformations()
            {
                return Transformations;
            }
            public override string GetName()
            {
                if (BlockType == TransformationBlockType.Parallel)
                    return "Parallel block";
                else
                    return "Sequance block";
            }
            public override string GetIconKey()
            {
                if (BlockType == TransformationBlockType.Parallel)
                    return "parralel_block";
                else
                    return "sequance_block";
            }
            #endregion

            #region Dynamic Execution
            private int transformation_index, finished;
            private bool[] useInParalelMode = new bool[128];
            protected override void OnInit()
            {
                if (BlockType == TransformationBlockType.Parallel)
                {
                    finished = Transformations.Count;
                    for (int tr = 0; tr < finished; tr++)
                    {
                        useInParalelMode[tr] = true;
                        Transformations[tr].Init();
                    }
                }
                else
                {
                    transformation_index = 0;
                    Transformations[transformation_index].Init();
                }
            }
            protected override bool OnUpdate()
            {
                if (BlockType == TransformationBlockType.Parallel)
                {
                    if (finished>0)
                    {
                        for (int tr=0;tr<Transformations.Count;tr++)
                        {
                            if ((useInParalelMode[tr]) && (Transformations[tr].Update() == false))
                            {
                                useInParalelMode[tr] = false;
                                finished--;
                            }
                        }
                    }
                    if (finished <= 0)
                        return false;
                }
                else
                {
                    if (Transformations[transformation_index].Update()==false)
                    {
                        transformation_index++;
                        if (transformation_index >= Transformations.Count)
                            return false;
                        Transformations[transformation_index].Init();
                    }
                }
                return true;
            }
            public override string GetCPPClassName() { if (BlockType == TransformationBlockType.Parallel) return "Parallel"; else return "Sequance"; }
            public override string CreateInitializationCPPCode()
            {
                string s = "\n\t//" + BlockType.ToString() + " transformation\n\t" + this.CppName + ".list = new GApp::Animations::Transformations::Transformation* [" + Transformations.Count.ToString() + "];";
                s += "\n\t" + this.CppName + ".NumberOfTransformations = " + Transformations.Count.ToString() + ";";
                int index = 0;
                foreach (var t in Transformations) {
                    s+="\n\t"+this.CppName+".list["+index.ToString()+"] = &"+t.CppName+";";
                    index++;
                }
                return s+"\n"; 
            }
            public override void Validate(Project prj, string animationName, AppResources resources, Dictionary<string, GenericElement> elements)
            {
                if (Transformations.Count == 0 )
                    prj.EC.AddError("Animation - '" + animationName + "' : Parralel or Sequence blocks have to have at least one child !");
            }
            #endregion
        }

        [XmlType("LinearTransformation"), XmlRoot("LinearTransformation")]
        public class LinearTransformation : GenericElementTransformation
        {
            [XmlAttribute()]
            public float Start = 0.0f;
            [XmlAttribute()]
            public float End = 1.0f;
            [XmlAttribute()]
            public int Steps = 10;
            [XmlAttribute()]
            public string UserValue_Start = "", UserValue_End = "", UserValue_Steps = "";

            #region Atribute
            [XmlIgnore(), Description("Start value (percentage)"), Category("Transformation"), DisplayName("Start")]
            public string _Start
            {
                get { return Project.ProcentToString(Start); }
                set { Start = Project.StringToProcent(value, 0,100,Start,true); }
            }
            [XmlIgnore(), Description("End value (percentage)"), Category("Transformation"), DisplayName("End")]
            public string _End
            {
                get { return Project.ProcentToString(End); }
                set { End = Project.StringToProcent(value, 0, 100, End, true); }
            }
            [XmlIgnore(), Description("Nummber of steps"), Category("Transformation"), DisplayName("Steps")]
            public int _Steps
            {
                get { return Steps; }
                set { if (value >= 1) Steps = value; }
            }

            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Start")]
            public string _UserValue_Start
            {
                get { return UserValue_Start; }
                set { UserValue_Start = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("End")]
            public string _UserValue_End
            {
                get { return UserValue_End; }
                set { UserValue_End = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Steps")]
            public string _UserValue_Steps
            {
                get { return UserValue_Steps; }
                set { UserValue_Steps = value; }
            }
            #endregion

            #region Dynamic Execution
            protected int steps_count;
            protected float step_value, current_value;
            protected bool BackAndForward = false;
            protected override void OnInit()
            {
                steps_count = Steps;
                if (BackAndForward)
                    steps_count *= 2;
                step_value = (End - Start) / Steps;
                current_value = Start;
                OnUpdateElement(ElementObject);
            }
            protected override bool OnUpdate()
            {
                if (BackAndForward)
                {
                    if (steps_count > Steps)
                        current_value += step_value;
                    else
                        current_value -= step_value;
                } else {
                    current_value += step_value;
                }
                steps_count--;
                OnUpdateElement(ElementObject);
                return steps_count > 0;
            }
            protected virtual void OnUpdateElement(GenericElement e) {}
            #endregion

            protected override string GetTransformationDescription()
            {
                return "From " + Project.ProcentToString(Start) + " to " + Project.ProcentToString(End) + " in " + Steps.ToString() + " steps";
            }
            public override string CreateInitializationCPPCode()
            {
                string s = "\n\t//"+GetType().ToString()+" transformation";
                s += "\n\t" + this.CppName + ".Start = " + this.Start.ToString() + ";";
                s += "\n\t" + this.CppName + ".End = " + this.End.ToString() + ";";
                s += "\n\t" + this.CppName + ".Steps = " + this.Steps.ToString() + ";";
                s += "\n\t" + this.CppName + ".Element = &this->" + this.Element + ";";
                return s + "\n";
            }
            public override string CreateOnStartCPPCode()
            {
                return GetOnStartFieldInit(UserValue_Start, "Start") + GetOnStartFieldInit(UserValue_End, "End") + GetOnStartFieldInit(UserValue_Steps, "Steps") + base.CreateOnStartCPPCode();
            }
            public override void PopulateParameters(AnimationParameters p) 
            {
                if (UserValue_Start.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Start, "float"));
                if (UserValue_End.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_End, "float"));
                if (UserValue_Steps.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Steps, "int"));
                base.PopulateParameters(p);
            }
        }

        [XmlType("AlphaBlendingLinear"), XmlRoot("AlphaBlendingLinear")]
        public class AlphaBlendingLinearTransformation : LinearTransformation
        {
            protected override void OnUpdateElement(GenericElement e) 
            {
                ElementObject.ExecutionContext.ColorBlending = RuntimeContext.BlendModeToColor(BlendingMode.AlphaBlending, (int)ElementObject.ExecutionContext.ColorBlending, current_value); 
            }
        }
        
        [XmlType("AlphaBlendingForwardAndBack"), XmlRoot("AlphaBlendingForwardAndBack")]
        public class AlphaBlendingForwardAndBackTransformation : LinearTransformation
        {
            public AlphaBlendingForwardAndBackTransformation() { BackAndForward = true; }
            protected override void OnUpdateElement(GenericElement e)
            {
                ElementObject.ExecutionContext.ColorBlending = RuntimeContext.BlendModeToColor(BlendingMode.AlphaBlending, (int)ElementObject.ExecutionContext.ColorBlending, current_value);
            }
        }

        [XmlType("ScaleLinear"), XmlRoot("ScaleLinear")]
        public class ScaleLinearTransformation : LinearTransformation
        {
            protected override void OnUpdateElement(GenericElement e)
            {
                ElementObject.ExecutionContext.ScaleWidth = current_value;
                ElementObject.ExecutionContext.ScaleHeight = current_value;                 
            }
        }

        [XmlType("ScaleForwardAndBack"), XmlRoot("ScaleForwardAndBack")]
        public class ScaleForwardAndBackTransformation : LinearTransformation
        {
            public ScaleForwardAndBackTransformation() { BackAndForward = true; }
            protected override void OnUpdateElement(GenericElement e)
            {
                ElementObject.ExecutionContext.ScaleWidth = current_value;
                ElementObject.ExecutionContext.ScaleHeight = current_value;
            }
        }

        [XmlType("ScaleWidthLinear"), XmlRoot("ScaleWidthLinear")]
        public class ScaleWidthLinearTransformation : LinearTransformation
        {
            protected override void OnUpdateElement(GenericElement e)
            {
                ElementObject.ExecutionContext.ScaleWidth = current_value;
            }
        }

        [XmlType("ScaleWidthForwardAndBack"), XmlRoot("ScaleWidthForwardAndBack")]
        public class ScaleWidthForwardAndBackTransformation : LinearTransformation
        {
            public ScaleWidthForwardAndBackTransformation() { BackAndForward = true; }
            protected override void OnUpdateElement(GenericElement e)
            {
                ElementObject.ExecutionContext.ScaleWidth = current_value;
            }
        }

        [XmlType("ScaleHeightLinear"), XmlRoot("ScaleHeightLinear")]
        public class ScaleHeightLinearTransformation : LinearTransformation
        {
            protected override void OnUpdateElement(GenericElement e)
            {
                ElementObject.ExecutionContext.ScaleHeight = current_value;
            }
        }

        [XmlType("ScaleHeightForwardAndBack"), XmlRoot("ScaleHeightForwardAndBack")]
        public class ScaleHeightForwardAndBackTransformation : LinearTransformation
        {
            public ScaleHeightForwardAndBackTransformation() { BackAndForward = true; }
            protected override void OnUpdateElement(GenericElement e)
            {
                ElementObject.ExecutionContext.ScaleHeight = current_value;
            }
        }

        [XmlType("ColorBlendingGenericLinearTransformation"), XmlRoot("ColorBlendingGenericLinearTransformation")]
        public class ColorBlendingGenericLinearTransformation : GenericElementTransformation
        {
            [XmlAttribute()]
            public int StartColor = -1;
            [XmlAttribute()]
            public int EndColor = 0;
            [XmlAttribute()]
            public int Steps = 10;
            [XmlAttribute()]
            public string UserValue_StartColor = "", UserValue_EndColor = "", UserValue_Steps = "";

            #region Atribute
            [XmlIgnore(), Description("Start color"), Category("Transformation"), DisplayName("Start color")]
            public Color _StartColor
            {
                get { return System.Drawing.Color.FromArgb(StartColor); }
                set { StartColor = value.ToArgb(); }
            }
            [XmlIgnore(), Description("End color"), Category("Transformation"), DisplayName("End color")]
            public Color _EndColor
            {
                get { return System.Drawing.Color.FromArgb(EndColor); }
                set { EndColor = value.ToArgb(); }
            }
            [XmlIgnore(), Description("Nummber of steps"), Category("Transformation"), DisplayName("Steps")]
            public int _Steps
            {
                get { return Steps; }
                set { if (value >= 1) Steps = value; }
            }

            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Start color")]
            public string _UserValue_StartColor
            {
                get { return UserValue_StartColor; }
                set { UserValue_StartColor = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("End color")]
            public string _UserValue_EndColor
            {
                get { return UserValue_EndColor; }
                set { UserValue_EndColor = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Steps")]
            public string _UserValue_Steps
            {
                get { return UserValue_Steps; }
                set { UserValue_Steps = value; }
            }
            #endregion

            #region Dynamic Execution
            protected int steps_count;
            protected Color c_work;
            protected bool BackAndForward = false;
            private int GetChannel(float start,float end,int index)
            {
                return (int)(((end - start) / Steps) * index + start);
            }
            private Color ComputeColor(int index)
            {
                Color s = Color.FromArgb(StartColor);
                Color e = Color.FromArgb(EndColor);
                return Color.FromArgb(GetChannel(s.A, e.A, index), GetChannel(s.R, e.R, index), GetChannel(s.G, e.G, index), GetChannel(s.B, e.B, index));
            }
            protected override void OnInit()
            {
                steps_count = Steps;
                if (BackAndForward)
                    steps_count *= 2;
                ElementObject.ExecutionContext.ColorBlending = (uint)StartColor;
            }
            protected override bool OnUpdate()
            {
                if (BackAndForward)
                {
                    if (steps_count > Steps)
                        c_work = ComputeColor(Steps*2 - steps_count);
                    else
                        c_work = ComputeColor(steps_count);
                }
                else
                {
                    c_work = ComputeColor(Steps - steps_count);
                }
                steps_count--;
                ElementObject.ExecutionContext.ColorBlending = (uint)c_work.ToArgb();

                return steps_count > 0;
            }
            #endregion

            protected override string GetTransformationDescription()
            {
                return "From " + StartColor.ToString() + " to " + EndColor.ToString() + " in " + Steps.ToString() + " steps";
            }
            public override string CreateInitializationCPPCode()
            {
                string s = "\n\t//" + GetType().ToString() + " transformation";
                s += "\n\t" + this.CppName + ".Start = 0x" + this.StartColor.ToString("X8") + ";";
                s += "\n\t" + this.CppName + ".End = 0x" + this.EndColor.ToString("X8") + ";";
                s += "\n\t" + this.CppName + ".Steps = " + this.Steps.ToString() + ";";
                s += "\n\t" + this.CppName + ".Element = &this->" + this.Element + ";";
                return s + "\n";
            }
            public override string CreateOnStartCPPCode()
            {
                return GetOnStartFieldInit(UserValue_StartColor, "Start") + GetOnStartFieldInit(UserValue_EndColor, "End") + GetOnStartFieldInit(UserValue_Steps, "Steps") + base.CreateOnStartCPPCode();
            }
            public override void PopulateParameters(AnimationParameters p)
            {
                if (UserValue_StartColor.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_StartColor, "unsigned int", "Color"));
                if (UserValue_EndColor.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_EndColor, "unsigned int", "Color"));
                if (UserValue_Steps.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Steps, "int"));
                base.PopulateParameters(p);
            }
        }

        [XmlType("ColorBlendLinear"), XmlRoot("ColorBlendLinear")]
        public class ColorBlendLinearTransformation : ColorBlendingGenericLinearTransformation
        {
        }

        [XmlType("ColorBlendForwardAndBack"), XmlRoot("ColorBlendForwardAndBack")]
        public class ColorBlendForwardAndBackTransformation : ColorBlendingGenericLinearTransformation
        {
            public ColorBlendForwardAndBackTransformation() { BackAndForward = true; }
        }

        [XmlType("MoveRelativeLinear"), XmlRoot("MoveRelativeLinear")]
        public class MoveRelativeLinearTransformation : GenericElementTransformation
        {
            [XmlAttribute()]
            public float OffsetX = 0.0f;
            [XmlAttribute()]
            public float OffsetY = 1.0f;
            [XmlAttribute()]
            public int Steps = 10;
            [XmlAttribute()]
            public string UserValue_OffsetX = "", UserValue_OffsetY = "", UserValue_Steps = "";

            #region Atribute
            [XmlIgnore(), Description("Start value (percentage)"), Category("Transformation"), DisplayName("Offset X")]
            public string _OffsetX
            {
                get { return GetLocationValue(OffsetX); }
                set { OffsetX = SetLocationValue(value, OffsetX); }
            }
            [XmlIgnore(), Description("End value (percentage)"), Category("Transformation"), DisplayName("Offset Y")]
            public string _OffsetY
            {
                get { return GetLocationValue(OffsetY); }
                set { OffsetY = SetLocationValue(value, OffsetY); }
            }
            [XmlIgnore(), Description("Nummber of steps"), Category("Transformation"), DisplayName("Steps")]
            public int _Steps
            {
                get { return Steps; }
                set { if (value >= 1) Steps = value; }
            }

            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Offset X")]
            public string _UserValue_OffsetX
            {
                get { return UserValue_OffsetX; }
                set { UserValue_OffsetX = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Offset Y")]
            public string _UserValue_OffsetY
            {
                get { return UserValue_OffsetY; }
                set { UserValue_OffsetY = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Steps")]
            public string _UserValue_Steps
            {
                get { return UserValue_Steps; }
                set { UserValue_Steps = value; }
            }
            #endregion

            #region Dynamic Execution
            protected int steps_count;
            protected override void OnInit()
            {
                steps_count = Steps;
            }
            protected override bool OnUpdate()
            {
                steps_count--;
                ElementObject.ExecutionContext.X_Percentage += (OffsetX / (float)Steps);
                ElementObject.ExecutionContext.Y_Percentage += (OffsetY / (float)Steps);
                return steps_count > 0;
            }
            #endregion

            protected override string GetTransformationDescription()
            {
                return "Move to (" + _OffsetX+","+_OffsetY+") " + " in " + Steps.ToString() + " steps";
            }
            public override string CreateInitializationCPPCode()
            {
                string s = "\n\t//" + GetType().ToString() + " transformation";
                s += "\n\t" + this.CppName + ".OffsetX = " + this.OffsetX.ToString() + ";";
                s += "\n\t" + this.CppName + ".OffsetY = " + this.OffsetY.ToString() + ";";
                s += "\n\t" + this.CppName + ".Steps = " + this.Steps.ToString() + ";";
                s += "\n\t" + this.CppName + ".Element = &this->" + this.Element + ";";
                return s + "\n";
            }
            public override string CreateOnStartCPPCode()
            {
                return GetOnStartFieldInit(UserValue_OffsetX, "OffsetX") + GetOnStartFieldInit(UserValue_OffsetY, "OffsetY") + GetOnStartFieldInit(UserValue_Steps, "Steps") + base.CreateOnStartCPPCode();
            }
            public override void PopulateParameters(AnimationParameters p)
            {
                if (UserValue_OffsetX.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_OffsetX, "float"));
                if (UserValue_OffsetY.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_OffsetY, "float"));
                if (UserValue_Steps.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Steps, "int"));
                base.PopulateParameters(p);
            }
        }

        [XmlType("MoveAbsoluteLinear"), XmlRoot("MoveAbsoluteLinear")]
        public class MoveAbsoluteLinearTransformation : GenericElementTransformation
        {
            [XmlAttribute()]
            public float X = 0.5f;
            [XmlAttribute()]
            public float Y = 0.5f;
            [XmlAttribute()]
            public int StepSize = 1;
            [XmlAttribute()]
            public string UserValue_X = "", UserValue_Y = "", UserValue_StepSize = "";

            #region Atribute
            [XmlIgnore(), Description("Start value (percentage)"), Category("Transformation"), DisplayName("X")]
            public string _X
            {
                get { return GetLocationValue(X); }
                set { X = SetLocationValue(value, X); }
            }
            [XmlIgnore(), Description("End value (percentage)"), Category("Transformation"), DisplayName("Y")]
            public string _Y
            {
                get { return GetLocationValue(Y); }
                set { Y = SetLocationValue(value, Y); }
            }
            [XmlIgnore(), Description("Set size (units)"), Category("Transformation"), DisplayName("Step size")]
            public string _StepSize
            {
                get { return StepSize.ToString() + " units"; }
                set {
                    string ss = "";
                    for (int tr = 0; tr < value.Length; tr++)
                    {
                        if ((value[tr] < '0') || (value[tr] > '9'))
                            break;
                        ss += value[tr];
                    }
                    int res = 0;
                    if (int.TryParse(ss,out res)==false)
                    {
                        MessageBox.Show("Unable to translate '" + value + "' to a valid number !");
                        return;
                    }
                    if (res<1)
                    {
                        MessageBox.Show("Step size must be at least one unit !");
                        return;
                    }
                    StepSize = res;
                }
            }



            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("X")]
            public string _UserValue_X
            {
                get { return UserValue_X; }
                set { UserValue_X = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Y")]
            public string _UserValue_Y
            {
                get { return UserValue_Y; }
                set { UserValue_Y = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Step size")]
            public string _UserValue_StepSize
            {
                get { return UserValue_StepSize; }
                set { UserValue_StepSize     = value; }
            }
            #endregion

            #region Dynamic Execution
            protected int steps_count;
            protected float addX, addY;
            protected override void OnInit()
            {
                float dist, f_steps_count;
                //float dist = (float)Math.Sqrt((X - ElementObject.ExecutionContext.X) * (X - ElementObject.ExecutionContext.X) + (Y - ElementObject.ExecutionContext.Y) * (Y - ElementObject.ExecutionContext.Y));

                int design_resolution_width = 1;
                int design_resolution_height = 1;

                Project.SizeToValues(GenericTransformation.Prj.DesignResolution, ref design_resolution_width, ref design_resolution_height);
                dist = (float)Math.Sqrt((X - ElementObject.ExecutionContext.X_Percentage) * (float)design_resolution_width * (X - ElementObject.ExecutionContext.X_Percentage) * (float)design_resolution_width + (Y - ElementObject.ExecutionContext.Y_Percentage) * (float)design_resolution_height * (Y - ElementObject.ExecutionContext.Y_Percentage) * (float)design_resolution_height);

                f_steps_count = dist / (float)StepSize;                
                addX = (X - ElementObject.ExecutionContext.X_Percentage) / f_steps_count;
                addY = (Y - ElementObject.ExecutionContext.Y_Percentage) / f_steps_count;
                steps_count = (int)f_steps_count;
                
            }
            protected override bool OnUpdate()
            {
                steps_count--;
                if (steps_count<=0)
                {
                    ElementObject.ExecutionContext.X_Percentage = X;
                    ElementObject.ExecutionContext.Y_Percentage = Y;
                }
                else
                {
                    ElementObject.ExecutionContext.X_Percentage += addX;
                    ElementObject.ExecutionContext.Y_Percentage += addY;
                }
                return steps_count > 0;
            }
            #endregion

            protected override string GetTransformationDescription()
            {
                return "Move to (" + _X + "," + _Y + ") " + " StepSize = " + _StepSize;
            }
            public override string CreateInitializationCPPCode()
            {
                string s = "\n\t//" + GetType().ToString() + " transformation";
                s += "\n\t" + this.CppName + ".X = " + this.X.ToString() + ";";
                s += "\n\t" + this.CppName + ".Y = " + this.Y.ToString() + ";";
                s += "\n\t" + this.CppName + ".StepSize = " + this.StepSize.ToString() + ";";
                s += "\n\t" + this.CppName + ".Element = &this->" + this.Element + ";";
                return s + "\n";
            }
            public override string CreateOnStartCPPCode()
            {
                return GetOnStartFieldInit(UserValue_X, "X") + GetOnStartFieldInit(UserValue_Y, "Y") + GetOnStartFieldInit(UserValue_StepSize, "StepSize")  + base.CreateOnStartCPPCode();
            }
            public override void PopulateParameters(AnimationParameters p)
            {
                if (UserValue_X.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_X, "float"));
                if (UserValue_Y.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Y, "float"));
                if (UserValue_StepSize.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_StepSize, "int"));

                base.PopulateParameters(p);
            }
        }

        [XmlType("QuadraticBezier"), XmlRoot("QuadraticBezier")]
        public class QuadraticBezierTransformation : GenericElementTransformation
        {
            [XmlAttribute()]
            public float StartPointX = 0.0f;
            [XmlAttribute()]
            public float StartPointY = 1.0f;
            [XmlAttribute()]
            public float EndPointX = 1.0f;
            [XmlAttribute()]
            public float EndPointY = 1.0f;
            [XmlAttribute()]
            public float ControlPointX = 0.5f;
            [XmlAttribute()]
            public float ControlPointY = 0.5f;
            [XmlAttribute()]
            public int Steps = 10;
            [XmlAttribute()]
            public string UserValue_StartPointX = "", UserValue_StartPointY = "", UserValue_EndPointX = "", UserValue_EndPointY = "", UserValue_ControlPointX = "", UserValue_ControlPointY = "", UserValue_Steps = "";

            #region Atribute
            [XmlIgnore(), Description("Start value"), Category("Transformation"), DisplayName("Start Point X")]
            public string _StartPointX
            {
                get { return GetLocationValue(StartPointX); }
                set { StartPointX = SetLocationValue(value, StartPointX); }
            }
            [XmlIgnore(), Description("Start value"), Category("Transformation"), DisplayName("Start Point Y")]
            public string _StartPointY
            {
                get { return GetLocationValue(StartPointY); }
                set { StartPointY = SetLocationValue(value, StartPointY); }
            }
            [XmlIgnore(), Description("Control value"), Category("Transformation"), DisplayName("Control Point X")]
            public string _ControlPointX
            {
                get { return GetLocationValue(ControlPointX); }
                set { ControlPointX = SetLocationValue(value, ControlPointX); }
            }
            [XmlIgnore(), Description("Control value"), Category("Transformation"), DisplayName("Control Point Y")]
            public string _ControlPointY
            {
                get { return GetLocationValue(ControlPointY); }
                set { ControlPointY = SetLocationValue(value, ControlPointY); }
            }
            [XmlIgnore(), Description("End value"), Category("Transformation"), DisplayName("End Point X")]
            public string _EndPointX
            {
                get { return GetLocationValue(EndPointX); }
                set { EndPointX = SetLocationValue(value, EndPointX); }
            }
            [XmlIgnore(), Description("End value"), Category("Transformation"), DisplayName("End Point Y")]
            public string _EndlPointY
            {
                get { return GetLocationValue(EndPointY); }
                set { EndPointY = SetLocationValue(value, EndPointY); }
            }

            [XmlIgnore(), Description("Nummber of steps"), Category("Transformation"), DisplayName("Steps")]
            public int _Steps
            {
                get { return Steps; }
                set { if (value >= 1) Steps = value; }
            }

            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Start Point X")]
            public string _UserValue_StartPointX
            {
                get { return UserValue_StartPointX; }
                set { UserValue_StartPointX = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Start Point Y")]
            public string _UserValue_StartPointY
            {
                get { return UserValue_StartPointY; }
                set { UserValue_StartPointY = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Control Point X")]
            public string _UserValue_ControlPointX
            {
                get { return UserValue_ControlPointX; }
                set { UserValue_ControlPointX = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Control Point Y")]
            public string _UserValue_ControlPointY
            {
                get { return UserValue_ControlPointY; }
                set { UserValue_ControlPointY = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("End Point X")]
            public string _UserValue_EndPointX
            {
                get { return UserValue_EndPointX; }
                set { UserValue_EndPointX = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("End Point Y")]
            public string _UserValue_EndPointY
            {
                get { return UserValue_EndPointY; }
                set { UserValue_EndPointY = value; }
            }


            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Steps")]
            public string _UserValue_Steps
            {
                get { return UserValue_Steps; }
                set { UserValue_Steps = value; }
            }
            #endregion

            #region Dynamic Execution
            protected int steps_count;
            protected override void OnInit()
            {
                steps_count = 0;
            }
            protected override bool OnUpdate()
            {
                steps_count++;
                //x = (1 - t) * (1 - t) * p[0].x + 2 * (1 - t) * t * p[1].x + t * t * p[2].x;
                //y = (1 - t) * (1 - t) * p[0].y + 2 * (1 - t) * t * p[1].y + t * t * p[2].y;
                float t = ((float)steps_count) / ((float)Steps);
                ElementObject.ExecutionContext.X_Percentage = (1 - t) * (1 - t) * StartPointX + 2 * (1 - t) * t * ControlPointX + t * t * EndPointX;
                ElementObject.ExecutionContext.Y_Percentage = (1 - t) * (1 - t) * StartPointY + 2 * (1 - t) * t * ControlPointY + t * t * EndPointY;
                
                return steps_count < Steps;
            }
            #endregion

            protected override string GetTransformationDescription()
            {
                return "Bezier Move from (" + _StartPointX + "," + _StartPointY + ") to (" + _EndPointX + "," + _EndlPointY + ") in " + Steps.ToString() + " steps";
            }
            public override string CreateInitializationCPPCode()
            {
                string s = "\n\t//" + GetType().ToString() + " transformation";
                s += "\n\t" + this.CppName + ".StartPointX = " + this.StartPointX.ToString() + ";";
                s += "\n\t" + this.CppName + ".StartPointY = " + this.StartPointY.ToString() + ";";
                s += "\n\t" + this.CppName + ".ControlPointX = " + this.ControlPointX.ToString() + ";";
                s += "\n\t" + this.CppName + ".ControlPointY = " + this.ControlPointY.ToString() + ";";
                s += "\n\t" + this.CppName + ".EndPointX = " + this.EndPointX.ToString() + ";";
                s += "\n\t" + this.CppName + ".EndPointY = " + this.EndPointY.ToString() + ";";
                s += "\n\t" + this.CppName + ".Steps = " + this.Steps.ToString() + ";";
                s += "\n\t" + this.CppName + ".Element = &this->" + this.Element + ";";
                return s + "\n";
            }
            public override string CreateOnStartCPPCode()
            {
                return GetOnStartFieldInit(UserValue_StartPointX, "StartPointX") + GetOnStartFieldInit(UserValue_StartPointY, "StartPointY") +
                       GetOnStartFieldInit(UserValue_ControlPointX, "ControlPointX") + GetOnStartFieldInit(UserValue_ControlPointY, "ControlPointY") +
                       GetOnStartFieldInit(UserValue_EndPointX, "StartEndX") + GetOnStartFieldInit(UserValue_EndPointY, "EndPointY") +
                       GetOnStartFieldInit(UserValue_Steps, "Steps") + base.CreateOnStartCPPCode();
            }
            public override void PopulateParameters(AnimationParameters p)
            {
                if (UserValue_StartPointX.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_StartPointX, "float"));
                if (UserValue_StartPointY.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_StartPointY, "float"));
                if (UserValue_ControlPointX.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_ControlPointX, "float"));
                if (UserValue_ControlPointY.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_ControlPointY, "float"));
                if (UserValue_EndPointX.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_EndPointX, "float"));
                if (UserValue_EndPointY.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_EndPointY, "float"));
                if (UserValue_Steps.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Steps, "int"));
                base.PopulateParameters(p);
            }
        }

        [XmlType("VisibleState"), XmlRoot("VisibleState")]
        public class VisibleStateTransformation : GenericElementTransformation
        {
            [XmlAttribute()]
            public bool Visible = true;
            [XmlAttribute()]
            public string UserValue_Visible = "";

            #region Atribute
            [XmlIgnore(), Description("Start value (percentage)"), Category("Transformation"), DisplayName("Visible")]
            public bool _Visible
            {
                get { return Visible; }
                set { Visible = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Visible")]
            public string _UserValue_Visible
            {
                get { return UserValue_Visible; }
                set { UserValue_Visible = value; }
            }
            #endregion

            #region Dynamic Execution
            protected int steps_count;
            protected float step_value, current_value;
            protected bool BackAndForward = false;
            protected override void OnInit()
            {
                ElementObject.ExecutionContext.Visible = Visible;
            }
            protected override bool OnUpdate()
            {
                return false;
            }
            protected virtual void OnUpdateElement(GenericElement e) { }
            #endregion

            protected override string GetTransformationDescription()
            {
                return "Set visible state: " + Visible.ToString();
            }
            public override string CreateInitializationCPPCode()
            {
                string s = "\n\t//" + GetType().ToString() + " transformation";
                s += "\n\t" + this.CppName + ".Visible = " + this.Visible.ToString().ToLower() +";";
                s += "\n\t" + this.CppName + ".Element = &this->" + this.Element + ";";
                return s + "\n";
            }
            public override string CreateOnStartCPPCode()
            {
                return GetOnStartFieldInit(UserValue_Visible, "Visible") + base.CreateOnStartCPPCode();
            }
            public override void PopulateParameters(AnimationParameters p)
            {
                if (UserValue_Visible.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Visible, "bool"));
                base.PopulateParameters(p);
            }
        }

        [XmlType("ButtonEnableState"), XmlRoot("ButtonEnableState")]
        public class ButtonEnableTransformation : GenericElementTransformation
        {
            [XmlAttribute()]
            public bool Enabled = true;
            [XmlAttribute()]
            public string UserValue_Enabled = "";

            #region Atribute
            [XmlIgnore(), Description("Indicates if the button state should be set to enable or not"), Category("Button"), DisplayName("Enabled")]
            public bool _Visible
            {
                get { return Enabled; }
                set { Enabled = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Enabled")]
            public string _UserValue_Visible
            {
                get { return UserValue_Enabled; }
                set { UserValue_Enabled = value; }
            }
            #endregion

            #region Dynamic Execution
            protected override void OnInit()
            {
                SimpleButtonElement sbe = ElementObject as SimpleButtonElement;
                if (sbe!=null)
                {
                    sbe.Enabled = Enabled;
                }
            }
            protected override bool OnUpdate()
            {
                return false;
            }
            protected virtual void OnUpdateElement(GenericElement e) { }
            #endregion

            protected override string GetTransformationDescription()
            {
                return "Set button enable state: " + Enabled.ToString();
            }
            public override string CreateInitializationCPPCode()
            {
                string s = "\n\t//" + GetType().ToString() + " transformation";
                s += "\n\t" + this.CppName + ".Enabled = " + this.Enabled.ToString().ToLower() + ";";
                s += "\n\t" + this.CppName + ".Element = &this->" + this.Element + ";";
                return s + "\n";
            }
            public override string CreateOnStartCPPCode()
            {
                return GetOnStartFieldInit(UserValue_Enabled, "Enabled") + base.CreateOnStartCPPCode();
            }
            public override void PopulateParameters(AnimationParameters p)
            {
                if (UserValue_Enabled.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Enabled, "bool"));
                base.PopulateParameters(p);
            }
        }

        [XmlType("SetNewAbsolutePosition"), XmlRoot("SetNewAbsolutePosition")]
        public class SetNewAbsolutePositionTransformation : GenericElementTransformation
        {
            [XmlAttribute()]
            public float X = 0;
            [XmlAttribute()]
            public float Y = 0;

            [XmlAttribute()]
            public string UserValue_X = "", UserValue_Y = "";

            #region Atribute
            [XmlIgnore(), Description("X-axes"), Category("Transformation"), DisplayName("X")]
            public string _X
            {
                get { return GetLocationValue(X); }
                set { X = SetLocationValue(value, X); }
            }
            [XmlIgnore(), Description("Y-axes"), Category("Transformation"), DisplayName("Y")]
            public string _Y
            {
                get { return GetLocationValue(Y); }
                set { Y = SetLocationValue(value, Y); }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("X")]
            public string _UserValue_X
            {
                get { return UserValue_X; }
                set { UserValue_X = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Y")]
            public string _UserValue_Y
            {
                get { return UserValue_Y; }
                set { UserValue_Y = value; }
            }               
            #endregion

            #region Dynamic Execution
            protected override void OnInit()
            {
                ElementObject.ExecutionContext.X_Percentage = X;
                ElementObject.ExecutionContext.Y_Percentage = Y;
            }
            protected override bool OnUpdate()
            {
                return false;
            }
            protected virtual void OnUpdateElement(GenericElement e) { }
            #endregion

            protected override string GetTransformationDescription()
            {
                return "Set new position (" + _X + "," + _Y + ")";
            }
            public override string CreateInitializationCPPCode()
            {
                string s = "\n\t//" + GetType().ToString() + " transformation";
                s += "\n\t" + this.CppName + ".X = " + this.X.ToString() + ";";
                s += "\n\t" + this.CppName + ".Y = " + this.Y.ToString() + ";";
                s += "\n\t" + this.CppName + ".Element = &this->" + this.Element + ";";
                return s + "\n";
            }
            public override string CreateOnStartCPPCode()
            {
                return GetOnStartFieldInit(UserValue_X, "X") + GetOnStartFieldInit(UserValue_Y, "Y") + base.CreateOnStartCPPCode();
            }
            public override void PopulateParameters(AnimationParameters p)
            {
                if (UserValue_X.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_X, "float"));
                if (UserValue_Y.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Y, "float"));
                base.PopulateParameters(p);
            }
        }

        [XmlType("SetNewRelativePosition"), XmlRoot("SetNewRelativePosition")]
        public class SetNewRelativePositionTransformation : GenericElementTransformation
        {
            [XmlAttribute()]
            public float OffsetX = 0;
            [XmlAttribute()]
            public float OffsetY = 0;

            [XmlAttribute()]
            public string UserValue_OffsetX = "", UserValue_OffsetY = "";

            #region Atribute
            [XmlIgnore(), Description("X-axes"), Category("Transformation"), DisplayName("Offset X")]
            public string _X
            {
                get { return GetLocationValue(OffsetX); }
                set { OffsetX = SetLocationValue(value, OffsetX); }
            }
            [XmlIgnore(), Description("Y-axes"), Category("Transformation"), DisplayName("Offset Y")]
            public string _Y
            {
                get { return GetLocationValue(OffsetY); }
                set { OffsetY = SetLocationValue(value, OffsetY); }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Offset X")]
            public string _UserValue_X
            {
                get { return UserValue_OffsetX; }
                set { UserValue_OffsetX = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Offset Y")]
            public string _UserValue_Y
            {
                get { return UserValue_OffsetY; }
                set { UserValue_OffsetY = value; }
            }
            #endregion

            #region Dynamic Execution
            protected override void OnInit()
            {
                ElementObject.ExecutionContext.X_Percentage += OffsetX;
                ElementObject.ExecutionContext.Y_Percentage += OffsetY;
            }
            protected override bool OnUpdate()
            {
                return false;
            }
            protected virtual void OnUpdateElement(GenericElement e) { }
            #endregion

            protected override string GetTransformationDescription()
            {
                return "Move to offset (" + _X + "," + _Y + ")";
            }
            public override string CreateInitializationCPPCode()
            {
                string s = "\n\t//" + GetType().ToString() + " transformation";
                s += "\n\t" + this.CppName + ".OffsetX = " + this.OffsetX.ToString() + ";";
                s += "\n\t" + this.CppName + ".OffsetY = " + this.OffsetY.ToString() + ";";
                s += "\n\t" + this.CppName + ".Element = &this->" + this.Element + ";";
                return s + "\n";
            }
            public override string CreateOnStartCPPCode()
            {
                return GetOnStartFieldInit(UserValue_OffsetX, "OffsetX") + GetOnStartFieldInit(UserValue_OffsetY, "OffsetY") + base.CreateOnStartCPPCode();
            }
            public override void PopulateParameters(AnimationParameters p)
            {
                if (UserValue_OffsetX.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_OffsetX, "float"));
                if (UserValue_OffsetY.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_OffsetY, "float"));
                base.PopulateParameters(p);
            }
        }


        [XmlType("ColorBlendState"), XmlRoot("ColorBlendState")]
        public class ColorBlendStateTransformation : GenericElementTransformation
        {
            [XmlAttribute()]
            public int NewColor = -1;
            [XmlAttribute()]
            public string UserValue_Color = "";

            #region Atribute
            [XmlIgnore(), Description("Color)"), Category("Transformation"), DisplayName("Color")]
            public Color _Color
            {
                get { return System.Drawing.Color.FromArgb(NewColor); }
                set { NewColor = value.ToArgb(); }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Color")]
            public string _UserValue_Visible
            {
                get { return UserValue_Color; }
                set { UserValue_Color = value; }
            }
            #endregion

            #region Dynamic Execution
            protected override void OnInit()
            {
                ElementObject.ExecutionContext.ColorBlending = (uint)NewColor;
            }
            protected override bool OnUpdate()
            {
                return false;
            }
            protected virtual void OnUpdateElement(GenericElement e) { }
            #endregion

            protected override string GetTransformationDescription()
            {
                return "Set color: " + Color.FromArgb(NewColor).ToString();
            }
            public override string CreateInitializationCPPCode()
            {
                string s = "\n\t//" + GetType().ToString() + " transformation";
                s += "\n\t" + this.CppName + ".Color = " + this.NewColor.ToString().ToLower() + ";";
                s += "\n\t" + this.CppName + ".Element = &this->" + this.Element + ";";
                return s + "\n";
            }
            public override string CreateOnStartCPPCode()
            {
                return GetOnStartFieldInit(UserValue_Color, "Color") + base.CreateOnStartCPPCode();
            }
            public override void PopulateParameters(AnimationParameters p)
            {
                if (UserValue_Color.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Color, "unsigned int", "Color"));
                base.PopulateParameters(p);
            }
        }

        [XmlType("AlphaBlendingState"), XmlRoot("AlphaBlendingState")]
        public class AlphaBlendingStateTransformation : GenericElementTransformation
        {
            [XmlAttribute()]
            public float NewAlpha = 1.0f;
            [XmlAttribute()]
            public string UserValue_Alpha = "";

            #region Atribute
            [XmlIgnore(), Description("Alpha"), Category("Transformation"), DisplayName("Alpha")]
            public string _Alpha
            {
                get { return Project.ProcentToString(NewAlpha); }
                set { NewAlpha = Project.StringToProcent(value, 0, 100, NewAlpha, true); }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Alpha")]
            public string _UserValue_Alpha
            {
                get { return UserValue_Alpha; }
                set { UserValue_Alpha = value; }
            }
            #endregion

            #region Dynamic Execution
            protected override void OnInit()
            {
                ElementObject.ExecutionContext.ColorBlending = RuntimeContext.BlendModeToColor(BlendingMode.AlphaBlending, (int)ElementObject.ExecutionContext.ColorBlending, NewAlpha);
            }
            protected override bool OnUpdate()
            {
                return false;
            }
            #endregion

            protected override string GetTransformationDescription()
            {
                return "Set alpha: " + _Alpha;
            }
            public override string CreateInitializationCPPCode()
            {
                string s = "\n\t//" + GetType().ToString() + " transformation";
                s += "\n\t" + this.CppName + ".Alpha = " + this.NewAlpha.ToString() + ";";
                s += "\n\t" + this.CppName + ".Element = &this->" + this.Element + ";";
                return s + "\n";
            }
            public override string CreateOnStartCPPCode()
            {
                return GetOnStartFieldInit(UserValue_Alpha, "Alpha") + base.CreateOnStartCPPCode();
            }
            public override void PopulateParameters(AnimationParameters p)
            {
                if (UserValue_Alpha.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Alpha, "float"));
                base.PopulateParameters(p);
            }
        }

        [XmlType("SetNewText"), XmlRoot("SetNewText")]
        public class SetNewTextTransformation : GenericElementTransformation
        {
            [XmlAttribute()]
            public string Text = "";
            [XmlAttribute()]
            public string StringResource = "";
            [XmlAttribute()]
            public bool UseStringResource = false;
            [XmlAttribute()]
            public string UserValue_Text = "";

            #region Atribute
            [XmlIgnore(), Category("Text"), DisplayName("Text")]
            public string _Text
            {
                get { return Text; }
                set { Text = value; }
            }
            [XmlIgnore(), Category("Text"), DisplayName("Resource"), Editor(typeof(StringSelectorEditor), typeof(UITypeEditor))]
            public string _StringResource
            {
                get { return StringResource; }
                set { StringResource = value; }
            }
            [XmlIgnore(), Category("Text"), DisplayName("Use string resources")]
            public bool _UseStringResource
            {
                get { return UseStringResource; }
                set { UseStringResource = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Text")]
            public string _UserValue_Text
            {
                get { return UserValue_Text; }
                set { UserValue_Text = value; }
            }
            #endregion

            #region Dynamic Execution
            protected override void OnInit()
            {
                if (ElementObject.GetType() == typeof(AnimO.TextElement))
                {
                    if (UseStringResource)
                        ((AnimO.TextElement)ElementObject).tp.SetText(StringResource,true, GenericElement.CurrentAppResources);
                    else
                        ((AnimO.TextElement)ElementObject).tp.SetText(Text, false, GenericElement.CurrentAppResources);
                }
            }
            protected override bool OnUpdate()
            {
                return false;
            }
            protected virtual void OnUpdateElement(GenericElement e) { }
            #endregion

            protected override string GetTransformationDescription()
            {
                if (UseStringResource)
                    return "From resources: " + StringResource;
                else
                    return Text;
            }
            public override string CreateInitializationCPPCode()
            {
                string s = "\n\t//" + GetType().ToString() + " transformation";
                s += "\n\t" + this.CppName + ".Element = &this->" + this.Element + ";";
                if (UserValue_Text.Length > 0)
                {
                    s += "\n\t" + CppName + ".Text.Set(param_" + UserValue_Text + ");";
                }
                else
                {
                    if (this.UseStringResource)
                        s += "\n\t" + CppName + ".Text.Set(Res.Strings." + this.StringResource + ");";
                    else
                        s += "\n\t" + CppName + ".Text.Set(\"" + this.Text + "\");";
                }
                return s + "\n";
            }
            public override string CreateOnStartCPPCode()
            {
                return GetOnStartFieldInit(UserValue_Text, "Text") + base.CreateOnStartCPPCode();
            }
            public override void PopulateParameters(AnimationParameters p)
            {
                if (UserValue_Text.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Text, "GApp::Utils::String", "string"));
                base.PopulateParameters(p);
            }
            public override void Validate(Project prj, string animationName, AppResources resources, Dictionary<string, GenericElement> elements)
            {
                if (this.UseStringResource)
                {
                    if (resources.Strings.ContainsKey(this.StringResource) == false)
                        prj.EC.AddError("Animation '" + animationName + "' has a SetText transformation with an invalid string: '" + StringResource + "' !");
                }                
            }
        }

        [XmlType("SetNewImage"), XmlRoot("SetNewImage")]
        public class SetNewImageTransformation : GenericElementTransformation
        {
            [XmlAttribute()]
            public string Image = "";
            [XmlAttribute()]
            public string UserValue_Image = "";

            #region Atribute
            [XmlIgnore(), Category("Image"), DisplayName("Image"), Editor(typeof(ImageSelectorEditor), typeof(UITypeEditor))]
            public string _Image
            {
                get { return Image; }
                set { Image = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Image")]
            public string _UserValue_Image
            {
                get { return UserValue_Image; }
                set { UserValue_Image = value; }
            }
            #endregion

            #region Dynamic Execution
            protected override void OnInit()
            {
                if (TransEvent != null)
                    TransEvent.OnEvent(this);
            }
            protected override bool OnUpdate()
            {
                return false;
            }
            #endregion

            protected override string GetTransformationDescription()
            {
                    return Image;
            }
            public override string CreateInitializationCPPCode()
            {
                string s = "\n\t//" + GetType().ToString() + " transformation";
                s += "\n\t" + this.CppName + ".Image = Res.Images." + this.Image + ";";
                s += "\n\t" + this.CppName + ".Element = &this->" + this.Element + ";";
                return s + "\n";
            }
            public override string CreateOnStartCPPCode()
            {
                return GetOnStartFieldInit(UserValue_Image, "Image") + base.CreateOnStartCPPCode();
            }
            public override void PopulateParameters(AnimationParameters p)
            {
                if (UserValue_Image.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Image, "GApp::Resources::Bitmap *", "Bitmap"));
                base.PopulateParameters(p);
            }
            public override void Validate(Project prj, string animationName, AppResources resources, Dictionary<string, GenericElement> elements)
            {
                if (resources.Images.ContainsKey(this.Image) == false)
                    prj.EC.AddError("Animation '" + animationName + "' has an unknwon image: '" + Image + "' !");
            }
            public void SetImage(Bitmap bmp)
            {
                if (ElementObject.GetType() == typeof(AnimO.ImageElement))
                {
                    ((AnimO.ImageElement)ElementObject).ExecutionContext.Image = bmp;
                }
            }
        }

        [XmlType("ImageIndexGenericTransformation"), XmlRoot("ImageIndexGenericTransformation")]
        public class ImageIndexGenericTransformation: GenericElementTransformation
        {
            protected bool SetImageIndex(int idx)
            {
                if (ElementObject.GetType() != typeof(AnimO.ImageElement))
                    return false;
                
                AnimO.ImageElement img = ((AnimO.ImageElement)ElementObject);
                List<string> images = Project.StringListToList(img.Images, ';');
                if ((idx >= 0) && (idx < images.Count))
                {
                    string imgName = images[idx];
                    if (AnimO.GenericElement.CurrentAppResources.Images.ContainsKey(imgName))
                    {
                        img.ExecutionContext.Image = AnimO.GenericElement.CurrentAppResources.Images[imgName].Picture;
                        return true;
                    }
                    return false;
                }
                return false;
            }
            public override void Validate(Project prj, string animationName, AppResources resources, Dictionary<string, GenericElement> elements)
            {
                if (elements.ContainsKey(this.Element))
                {
                    GenericElement el = elements[this.Element];
                    if (el.GetType() != typeof(AnimO.ImageElement))
                    {
                        prj.EC.AddError("Animation '" + animationName + "' has an 'SetImageIndex' transformation for a non Image element: '" + this.Element + "' !");
                        return;
                    }
                    if (((ImageElement)el).HasMultipleImages() == false)
                    {
                        prj.EC.AddError("Set image index requires a multi image elements : '" + this.Element + "' - in animation: " + animationName + " !");
                        return;
                    }
                }
            }
        }

        [XmlType("SetImageIndex"), XmlRoot("SetImageIndex")]
        public class SetImageIndexTransformation : ImageIndexGenericTransformation
        {
            [XmlAttribute()]
            public int ImageIndex = 0;
            [XmlAttribute()]
            public string UserValue_ImageIndex = "";

            #region Atribute
            [XmlIgnore(), Category("Image"), DisplayName("Image Index")]
            public int _ImageIndex
            {
                get { return ImageIndex; }
                set { if (value>=0) ImageIndex = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Image Index")]
            public string _UserValue_ImageIndex
            {
                get { return UserValue_ImageIndex; }
                set { UserValue_ImageIndex = value; }
            }
            #endregion

            #region Dynamic Execution
            protected override void OnInit()
            {
                SetImageIndex(ImageIndex);
            }
            protected override bool OnUpdate()
            {
                return false;
            }
            #endregion

            protected override string GetTransformationDescription()
            {
                return ImageIndex.ToString();
            }
            public override string CreateInitializationCPPCode()
            {
                string s = "\n\t//" + GetType().ToString() + " transformation";
                s += "\n\t" + this.CppName + ".ImageIndex = " + this.ImageIndex.ToString() + ";";
                s += "\n\t" + this.CppName + ".Element = &this->" + this.Element + ";";
                return s + "\n";
            }
            public override string CreateOnStartCPPCode()
            {
                return GetOnStartFieldInit(UserValue_ImageIndex, "ImageIndex") + base.CreateOnStartCPPCode();
            }
            public override void PopulateParameters(AnimationParameters p)
            {
                if (UserValue_ImageIndex.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_ImageIndex, "int", "int"));
                base.PopulateParameters(p);
            }

        }

        
        [XmlType("ImageIndexGenericLinear"), XmlRoot("ImageIndexGenericLinear")]
        public class ImageIndexLinearGenericTransformation : ImageIndexGenericTransformation
        {
            [XmlAttribute()]
            public int StartIndex = 0;
            [XmlAttribute()]
            public int EndIndex = 0;
            [XmlAttribute()]
            public int Step = 1;
            [XmlAttribute()]
            public int FramesInterval = 1;

            [XmlAttribute()]
            public string UserValue_StartIndex = "", UserValue_EndIndex = "", UserValue_Step = "", UserValue_FramesInterval = "";

            #region Atribute
            [XmlIgnore(), Category("Image"), DisplayName("Start Index"), Description("Start index (image index)")]
            public int _StartIndex
            {
                get { return StartIndex; }
                set { if (value >= 0) StartIndex = value; }
            }
            [XmlIgnore(), Category("Image"), DisplayName("End Index"), Description("End index (image index)")]
            public int _EndIndex
            {
                get { return EndIndex; }
                set { if (value >= 0) EndIndex = value; }
            }
            [XmlIgnore(), Category("Image"), DisplayName("Frames Interval"), Description("Number of frames that need to pas before changing an image index")]
            public int _FramesInterval
            {
                get { return FramesInterval; }
                set { if (value >= 1) FramesInterval = value; }
            }

            [XmlIgnore(), Category("Image"), DisplayName("Step"), Description("Value to increase/decrease image index (from start to end)")]
            public int _Step
            {
                get { return Step; }
                set { if (value >= 1) Step = value; }
            }

            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Start Index")]
            public string _UserValue_StartIndex
            {
                get { return UserValue_StartIndex; }
                set { UserValue_StartIndex = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("End Index")]
            public string _UserValue_EndIndex
            {
                get { return UserValue_EndIndex; }
                set { UserValue_EndIndex = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Step")]
            public string _UserValue_Step
            {
                get { return UserValue_Step; }
                set { UserValue_Step = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Frames Interval")]
            public string _UserValue_FramesInterval
            {
                get { return UserValue_FramesInterval; }
                set { UserValue_FramesInterval = value; }
            }
            #endregion

            protected override string GetTransformationDescription()
            {
                return "From " + StartIndex.ToString() + " to " + EndIndex.ToString() + ", Step:" + Step.ToString() + ", FramesInterval:" + FramesInterval.ToString();
            }
            public override string CreateInitializationCPPCode()
            {
                string s = "\n\t//" + GetType().ToString() + " transformation";
                s += "\n\t" + this.CppName + ".StartIndex = " + this.StartIndex.ToString() + ";";
                s += "\n\t" + this.CppName + ".EndIndex = " + this.EndIndex.ToString() + ";";
                s += "\n\t" + this.CppName + ".Step = " + this.Step.ToString() + ";";
                s += "\n\t" + this.CppName + ".FramesInterval = " + this.FramesInterval.ToString() + ";";
                s += "\n\t" + this.CppName + ".Element = &this->" + this.Element + ";";
                return s + "\n";
            }
            public override string CreateOnStartCPPCode()
            {
                return  GetOnStartFieldInit(UserValue_StartIndex, "StartIndex") +
                        GetOnStartFieldInit(UserValue_EndIndex, "EndIndex") +
                        GetOnStartFieldInit(UserValue_Step, "Step") +
                        GetOnStartFieldInit(UserValue_FramesInterval, "FramesInterval") + 
                        base.CreateOnStartCPPCode();
            }
            public override void PopulateParameters(AnimationParameters p)
            {
                if (UserValue_StartIndex.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_StartIndex, "int", "int"));
                if (UserValue_EndIndex.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_EndIndex, "int", "int"));
                if (UserValue_Step.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Step, "int", "int"));
                if (UserValue_FramesInterval.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_FramesInterval, "int", "int"));
                base.PopulateParameters(p);
            }
        }

        [XmlType("ImageIndexLinear"), XmlRoot("ImageIndexLinear")]
        public class ImageIndexLinearTransformation : ImageIndexLinearGenericTransformation
        {
            int currentIndex,timer;
            #region Dynamic Execution
            protected override void OnInit()
            {
                SetImageIndex(StartIndex);
                currentIndex = StartIndex;
                timer = FramesInterval;
            }
            protected override bool OnUpdate()
            {
                timer--;
                if (timer > 0)
                    return true;
                timer = FramesInterval;
                if (currentIndex==EndIndex)
                    return false;
                if (currentIndex<EndIndex)
                {
                    currentIndex += Step;
                    if (currentIndex > EndIndex)
                        currentIndex = EndIndex;
                } else
                {
                    currentIndex -= Step;
                    if (currentIndex < EndIndex)
                        currentIndex = EndIndex;
                }
                SetImageIndex(currentIndex);
                return true;
            }
            #endregion
        }

        [XmlType("TextFlow"), XmlRoot("TextFlow")]
        public class TextFlowTransformation : GenericElementTransformation
        {
            [XmlAttribute()]
            public int FramesUpdate = 1;
            [XmlAttribute()]
            public string UserValue_FramesUpdate = "";

            #region Atribute
            [XmlIgnore(), Category("Flow"), DisplayName("Update time"), Description("Number of frames that need to pass to display another character from the text (minimal value is 1)")]
            public int _FramesUpdate
            {
                get { return FramesUpdate; }
                set { if (value >= 1) FramesUpdate = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Update time")]
            public string _UserValue_FramesUpdate
            {
                get { return UserValue_FramesUpdate; }
                set { UserValue_FramesUpdate = value; }
            }
            #endregion

            #region Dynamic Execution
            private int time, poz;
            protected override void OnInit()
            {
                if (ElementObject.GetType() == typeof(AnimO.TextElement))
                {
                    ((AnimO.TextElement)ElementObject).tp.SetCharactesVisibility(0, 0x7fffffff, false);
                    time = FramesUpdate;
                    poz = 0;
                }
            }
            protected override bool OnUpdate()
            {
                if (ElementObject.GetType() == typeof(AnimO.TextElement))
                {
                    time--;
                    if (time==0)
                    {
                        time = FramesUpdate;
                        TextPainter tp = ((AnimO.TextElement)ElementObject).tp;
                        poz++;
                        tp.SetCharactesVisibility(0,poz,true);
                        if (poz>=tp.GetTextLength())
                            return false;
                        return true;
                    }
                    return true;
                }
                return false;
            }
            protected virtual void OnUpdateElement(GenericElement e) { }
            #endregion

            protected override string GetTransformationDescription()
            {
                return "Every " + FramesUpdate.ToString() + " frame(s)";
            }
            public override string CreateInitializationCPPCode()
            {
                string s = "\n\t//" + GetType().ToString() + " transformation";
                s += "\n\t" + this.CppName + ".Element = &this->" + this.Element + ";";
                s += "\n\t" + this.CppName + ".FramesUpdate = " + FramesUpdate.ToString() + ";";
                return s + "\n";
            }
            public override string CreateOnStartCPPCode()
            {
                return GetOnStartFieldInit(UserValue_FramesUpdate, "FramesUpdate") + base.CreateOnStartCPPCode();
            }
            public override void PopulateParameters(AnimationParameters p)
            {
                if (UserValue_FramesUpdate.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_FramesUpdate, "int"));
                base.PopulateParameters(p);
            }

        }

        [XmlType("TextCharacterVisibility"), XmlRoot("TextCharacterVisibility")]
        public class TextCharacterVisibilityTransformation : GenericElementTransformation
        {
            [XmlAttribute()]
            public int Start = 0;
            [XmlAttribute()]
            public int End = -1;
            [XmlAttribute()]
            public int Step = 1;
            [XmlAttribute()]
            public bool Visibility = true;

            [XmlAttribute()]
            public string UserValue_Start = "", UserValue_End = "", UserValue_Step = "", UserValue_Visibility = "";

            #region Atribute
            [XmlIgnore(), Category("Characters"), DisplayName("Start character index"), Description("Index of the first character that will be changed ! It must be bigger or equal to 0.")]
            public int _Start
            {
                get { return Start; }
                set { if (value >= 0) Start = value; }
            }
            [XmlIgnore(), Category("Characters"), DisplayName("End character index"), Description("Index of the last character that will be changed ! If negative it is consider to be an index from the right side of the string (-1 means last character)")]
            public int _End
            {
                get { return End; }
                set { End = value; }
            }
            [XmlIgnore(), Category("Characters"), DisplayName("Step"), Description("Step to jump to the next character staring from 'Start' until 'End'. Must be bigger than 0.")]
            public int _Step
            {
                get { return Step; }
                set { if (value > 0) Step = value; }
            }
            [XmlIgnore(), Category("Characters"), DisplayName("Visibility"), Description("If characters that matches the rule should be made visible or not.")]
            public bool _Visibility
            {
                get { return Visibility; }
                set { Visibility = value; }
            }

            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Start character index")]
            public string _UserValue_Start
            {
                get { return UserValue_Start; }
                set { UserValue_Start = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("End character index")]
            public string _UserValue_End
            {
                get { return UserValue_End; }
                set { UserValue_End = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Step")]
            public string _UserValue_Step
            {
                get { return UserValue_Step; }
                set { UserValue_Step = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Visibility")]
            public string _UserValue_Visibility
            {
                get { return UserValue_Visibility; }
                set { UserValue_Visibility = value; }
            }
            #endregion

            #region Dynamic Execution
            protected override void OnInit()
            {
                if (ElementObject.GetType() == typeof(AnimO.TextElement))
                {
                    TextPainter tp = ((AnimO.TextElement)ElementObject).tp;
                    int sz = tp.GetTextLength();
                    int _end = End;
                    if (_end<0)
                        _end = sz+End;
                    for (int tr=Start;tr<=_end;tr+=Step)
                        tp.SetCharactesVisibility(tr,tr+1, Visibility);
                }
            }
            protected override bool OnUpdate()
            {
                return false;
            }

            #endregion

            protected override string GetTransformationDescription()
            {
                return "Character visibility (Start:" + Start.ToString() + " End:" + End.ToString() + " Step:" + Step.ToString() + " Visible:" + Visibility.ToString() + ")";
            }
            public override string CreateInitializationCPPCode()
            {
                string s = "\n\t//" + GetType().ToString() + " transformation";
                s += "\n\t" + this.CppName + ".Element = &this->" + this.Element + ";";
                s += "\n\t" + this.CppName + ".Start = " + Start.ToString() + ";";
                s += "\n\t" + this.CppName + ".End = " + End.ToString() + ";";
                s += "\n\t" + this.CppName + ".Step = " + Step.ToString() + ";";
                s += "\n\t" + this.CppName + ".Visibility = " + Visibility.ToString().ToLower() + ";";

                return s + "\n";
            }
            public override string CreateOnStartCPPCode()
            {
                return GetOnStartFieldInit(UserValue_Start, "Start") + GetOnStartFieldInit(UserValue_End, "End") + GetOnStartFieldInit(UserValue_Step, "Step") + GetOnStartFieldInit(UserValue_Visibility, "Visibility") + base.CreateOnStartCPPCode();
            }
            public override void PopulateParameters(AnimationParameters p)
            {
                if (UserValue_Start.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Start, "int"));
                if (UserValue_End.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_End, "int"));
                if (UserValue_Step.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Step, "int"));
                if (UserValue_Visibility.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Visibility, "bool"));
                base.PopulateParameters(p);
            }

        }

        [XmlType("TextCenterFlow"), XmlRoot("TextCenterFlow")]
        public class TextCenterFlowTransformation : GenericElementTransformation
        {
            [XmlAttribute()]
            public int FramesUpdate = 1;
            [XmlAttribute()]
            public string UserValue_FramesUpdate = "";

            #region Atribute
            [XmlIgnore(), Category("Flow"), DisplayName("Update time"), Description("Number of frames that need to pass to display another character from the text (minimal value is 1)")]
            public int _FramesUpdate
            {
                get { return FramesUpdate; }
                set { if (value >= 1) FramesUpdate = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Update time")]
            public string _UserValue_FramesUpdate
            {
                get { return UserValue_FramesUpdate; }
                set { UserValue_FramesUpdate = value; }
            }
            #endregion

            #region Dynamic Execution
            private int time, poz;
            protected override void OnInit()
            {
                if (ElementObject.GetType() == typeof(AnimO.TextElement))
                {
                    ((AnimO.TextElement)ElementObject).tp.SetCharactesVisibility(0, 0x7fffffff, false);
                    time = FramesUpdate;
                    poz = ((AnimO.TextElement)ElementObject).tp.GetTextLength() / 2;
                }
            }
            protected override bool OnUpdate()
            {
                if (ElementObject.GetType() == typeof(AnimO.TextElement))
                {
                    time--;
                    if (time == 0)
                    {
                        time = FramesUpdate;
                        TextPainter tp = ((AnimO.TextElement)ElementObject).tp;
                        poz--;
                        tp.SetCharactesVisibility(poz, (tp.GetTextLength() - poz), true);
                        if (poz < 0)
                        {
                            tp.SetCharactesVisibility(0, 0x7fffffff, true);
                            return false;
                        }
                        return true;
                    }
                    return true;
                }
                return false;
            }
            protected virtual void OnUpdateElement(GenericElement e) { }
            #endregion

            protected override string GetTransformationDescription()
            {
                return "Every " + FramesUpdate.ToString() + " frame(s)";
            }
            public override string CreateInitializationCPPCode()
            {
                string s = "\n\t//" + GetType().ToString() + " transformation";
                s += "\n\t" + this.CppName + ".Element = &this->" + this.Element + ";";
                s += "\n\t" + this.CppName + ".FramesUpdate = " + FramesUpdate.ToString() + ";";
                return s + "\n";
            }
            public override string CreateOnStartCPPCode()
            {
                return GetOnStartFieldInit(UserValue_FramesUpdate, "FramesUpdate") + base.CreateOnStartCPPCode();
            }
            public override void PopulateParameters(AnimationParameters p)
            {
                if (UserValue_FramesUpdate.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_FramesUpdate, "int"));
                base.PopulateParameters(p);
            }

        }

        [XmlType("FontSize"), XmlRoot("FontSize")]
        public class FontSizeTransformation : GenericElementTransformation
        {
            [XmlAttribute()]
            public float Size = 1.0f;
            [XmlAttribute()]
            public string UserValue_Size = "";

            #region Atribute
            [XmlIgnore(), Description("Start value (percentage)"), Category("Transformation"), DisplayName("Font size")]
            public string _Size
            {
                get { return Project.ProcentToString(Size); }
                set { Size = Project.StringToProcent(value, 0, 100, Size, true); }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Font size")]
            public string _UserValue_Size
            {
                get { return UserValue_Size; }
                set { UserValue_Size = value; }
            }
            #endregion

            #region Dynamic Execution
            protected override void OnInit()
            {
                if (ElementObject.GetType() == typeof(AnimO.TextElement))
                {
                    ((AnimO.TextElement)ElementObject).ExecutionContext.ScaleWidth = Size;
                }
            }
            protected override bool OnUpdate()
            {
                return false;
            }
            protected virtual void OnUpdateElement(GenericElement e) { }
            #endregion

            protected override string GetTransformationDescription()
            {
                return "Font size : " + _Size;
            }
            public override string CreateInitializationCPPCode()
            {
                string s = "\n\t//" + GetType().ToString() + " transformation";
                s += "\n\t" + this.CppName + ".Size = " + this.Size.ToString() + ";";
                s += "\n\t" + this.CppName + ".Element = &this->" + this.Element + ";";
                return s + "\n";
            }
            public override string CreateOnStartCPPCode()
            {
                return GetOnStartFieldInit(UserValue_Size, "Size") +  base.CreateOnStartCPPCode();
            }
            public override void PopulateParameters(AnimationParameters p)
            {
                if (UserValue_Size.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Size, "float"));
                base.PopulateParameters(p);
            }
        }

        public enum NumberFormatMethod: int
        {
            Integer = 0,
            IntegerWithDigitGrouping = 1,
            FloatWith2DecimalPlaces = 2,
            FloatWith3DecimalPlaces = 3,
            Percentage = 4,
            Percentage2DecimalPlaces = 5,
        };

        public class NumericFormatValidator
        {
            public int intValue = 0;
            public float floatValue = 0;
            public string cppType = "";
            public string cppFormatRepresentation = "";
            public string csFormatedValue = "";
            public string strRepresentation = "";
            public string error = "";
            public bool FloatNumber;

            private bool UpdateCPPFormat(NumberFormatMethod m)
            {
                switch (m)
                {
                    case NumberFormatMethod.Integer: cppType = "int"; return true;
                    case NumberFormatMethod.IntegerWithDigitGrouping: cppType = "int"; return true;
                    case NumberFormatMethod.FloatWith2DecimalPlaces: cppType = "float"; return true;
                    case NumberFormatMethod.FloatWith3DecimalPlaces: cppType = "float"; return true;
                    case NumberFormatMethod.Percentage: cppType = "float"; return true;
                    case NumberFormatMethod.Percentage2DecimalPlaces: cppType = "float"; return true;
                }
                error = "Unable to convert values of type '" + m.ToString() + "' to a C++ format !";
                return false;
            }

            private bool UpdateCSFormatedValue(NumberFormatMethod m)
            {
                switch (m)
                {
                    case NumberFormatMethod.Integer: csFormatedValue = intValue.ToString(); return true;
                    case NumberFormatMethod.IntegerWithDigitGrouping: csFormatedValue = intValue.ToString("N0", CultureInfo.CreateSpecificCulture("en-US")); return true;
                    case NumberFormatMethod.FloatWith2DecimalPlaces: csFormatedValue = floatValue.ToString("F2"); return true;
                    case NumberFormatMethod.FloatWith3DecimalPlaces: csFormatedValue = floatValue.ToString("F3"); return true;
                    case NumberFormatMethod.Percentage: csFormatedValue = ((int)(floatValue * 100.0f)).ToString() + "%"; return true;
                    case NumberFormatMethod.Percentage2DecimalPlaces: csFormatedValue = floatValue.ToString("P2", CultureInfo.InvariantCulture).Replace(" ", "") ; return true;
                }
                error = "Unable to convert values of type '" + m.ToString() + "' to a C-Sharp format !";
                return false;
            }

            public bool ValidateData(string value, NumberFormatMethod m)
            {
                int iValue = 0;
                float fValue = 0;
                error = "";
                strRepresentation = "";
                cppFormatRepresentation = "";                
                cppType = "";
                csFormatedValue = "";
                FloatNumber = true;
                if (UpdateCPPFormat(m) == false)
                    return false;

                switch (m)
                {
                    case NumberFormatMethod.Integer:
                    case NumberFormatMethod.IntegerWithDigitGrouping:
                        if (int.TryParse(value, out iValue) == false)
                        {
                            error = "Value '" + value + "' is not valid for method: '" + m.ToString() + "' !";
                            return false;
                        }
                        intValue = iValue;
                        strRepresentation = iValue.ToString();
                        cppFormatRepresentation = strRepresentation;
                        FloatNumber = false;
                        break;
                    case NumberFormatMethod.FloatWith2DecimalPlaces:
                    case NumberFormatMethod.FloatWith3DecimalPlaces:
                        if (float.TryParse(value, out fValue) == false)
                        {
                            error = "Value '" + value + "' is not valid for method: '" + m.ToString() + "' !";
                            return false;
                        }
                        floatValue = fValue;
                        strRepresentation = fValue.ToString();
                        cppFormatRepresentation = strRepresentation;
                        break;
                    case NumberFormatMethod.Percentage:
                    case NumberFormatMethod.Percentage2DecimalPlaces:
                        if (Project.StringToProcent(value, ref fValue)==false)
                        {
                            error = "Value '" + value + "' is not valid for method: '" + m.ToString() + "' !";
                            return false;
                        }
                        floatValue = fValue;
                        strRepresentation = Project.ProcentToString(floatValue);
                        cppFormatRepresentation = floatValue.ToString();
                        break;
                }
                if (strRepresentation.Length>0)
                {
                    return UpdateCSFormatedValue(m);
                }
                error = "No translation available for value '" + value + "' as method: '" + m.ToString() + "' !";
                return false;
            }
        }

        [XmlType("NumericFormatter"), XmlRoot("NumericFormatter")]
        public class NumericFormatterTransformation : GenericElementTransformation
        {
            [XmlAttribute()]
            public string Value = "12345";
            [XmlAttribute()]
            public NumberFormatMethod Method = NumberFormatMethod.Integer;

            [XmlAttribute()]
            public string UserValue_Value = "";

            #region Atribute
            [XmlIgnore(), Category("Format"), DisplayName("Value"), Description("Number to be formated")]
            public string _FramesUpdate
            {
                get { return Value; }
                set { if (nv.ValidateData(value, Method)) Value = nv.strRepresentation; }
            }
            [XmlIgnore(), Category("Format"), DisplayName("Method"), Description("Format method")]
            public NumberFormatMethod _Method
            {
                get { return Method; }
                set { Method = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Value")]
            public string _UserValue_Value
            {
                get { return UserValue_Value; }
                set { UserValue_Value = value; }
            }
            #endregion

            #region Dynamic Execution
            private NumericFormatValidator nv = new NumericFormatValidator();

            protected override void OnInit()
            {
                if (ElementObject.GetType() == typeof(AnimO.TextElement))
                {
                    TextPainter tp = ((AnimO.TextElement)ElementObject).tp;
                    if (nv.ValidateData(Value, Method))
                        tp.SetText(nv.csFormatedValue, false, null);
                    else
                        tp.SetText("???", false, null);
                }
            }
            protected override bool OnUpdate()
            {                
                return false;
            }
            #endregion

            

            protected override string GetTransformationDescription()
            {
                return "Number format " + Value + " as "+Method.ToString();
            }
            public override string CreateInitializationCPPCode()
            {
                string s = "\n\t//" + GetType().ToString() + " transformation";
                s += "\n\t" + this.CppName + ".Element = &this->" + this.Element + ";";
                s += "\n\t" + this.CppName + "." + nv.cppType + "Value = " + nv.cppFormatRepresentation + ";";
                s += "\n\t" + this.CppName + ".FormatType = " + ((int)Method).ToString() + " ;";
                return s + "\n";
            }
            public override void Validate(Project prj, string animationName, AppResources resources, Dictionary<string, GenericElement> elements)
            {
                if (nv.ValidateData(Value, Method) == false)
                    prj.EC.AddError("Animation '" + animationName + "' has an invalid NumericFormat transformation: => " + nv.error);
            }
            public override string CreateOnStartCPPCode()
            {
                return GetOnStartFieldInit(UserValue_Value, nv.cppType+"Value") + base.CreateOnStartCPPCode();
            }
            public override void PopulateParameters(AnimationParameters p)
            {
                if (nv.ValidateData(Value, Method) == false)
                    nv.cppType = "???";
                if (UserValue_Value.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Value, nv.cppType));
                base.PopulateParameters(p);
            }

        }

        [XmlType("NumberIncrease"), XmlRoot("NumberIncrease")]
        public class NumberIncreaseTransformation : GenericElementTransformation
        {
            [XmlAttribute()]
            public string Start = "0";
            [XmlAttribute()]
            public string End = "0";
            [XmlAttribute()]
            public int Steps = 20;
            [XmlAttribute()]
            public NumberFormatMethod Method = NumberFormatMethod.Integer;

            [XmlAttribute()]
            public string UserValue_Start = "", UserValue_End = "", UserValue_Steps = "";

            #region Atribute
            [XmlIgnore(), Category("Format"), DisplayName("Start value"), Description("Number to be formated")]
            public string _Start
            {
                get { return Start; }
                set { if (nvStart.ValidateData(value, Method)) Start = nvStart.strRepresentation; }
            }
            [XmlIgnore(), Category("Format"), DisplayName("End value"), Description("Number to be formated")]
            public string _End
            {
                get { return End; }
                set { if (nvStart.ValidateData(value, Method)) End = nvStart.strRepresentation; }
            }
            [XmlIgnore(), Category("Format"), DisplayName("Steps"), Description("Number steps to go from 'Start' to 'End'")]
            public int _Steps
            {
                get { return Steps; }
                set { if (value > 0) Steps = value; }
            }
            [XmlIgnore(), Category("Format"), DisplayName("Method"), Description("Format method")]
            public NumberFormatMethod _Method
            {
                get { return Method; }
                set { Method = value; }
            }

            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Start value")]
            public string _UserValue_Start
            {
                get { return UserValue_Start; }
                set { UserValue_Start = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("End value")]
            public string _UserValue_End
            {
                get { return UserValue_End; }
                set { UserValue_End = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Steps")]
            public string _UserValue_Steps
            {
                get { return UserValue_Steps; }
                set { UserValue_Steps = value; }
            }
            #endregion

            #region Dynamic Execution
            private NumericFormatValidator nvStart = new NumericFormatValidator();
            private NumericFormatValidator nvEnd = new NumericFormatValidator();
            private NumericFormatValidator nvCurrent = new NumericFormatValidator();
            private int step;

            protected override void OnInit()
            {
                if (ElementObject.GetType() == typeof(AnimO.TextElement))
                {
                    TextPainter tp = ((AnimO.TextElement)ElementObject).tp;
                    if (nvStart.ValidateData(Start, Method))
                        tp.SetText(nvStart.csFormatedValue, false, null);
                    else
                        tp.SetText("???", false, null);
                    nvEnd.ValidateData(End, Method);
                    step = 0;
                }
            }
            protected override bool OnUpdate()
            {
                step++;
                bool result_ok = false;
                if (nvStart.FloatNumber)
                {
                    float dif = nvEnd.floatValue - nvStart.floatValue;
                    float cValue = nvStart.floatValue + (dif * step) / Steps;
                    if (cValue > nvEnd.floatValue)
                        cValue = nvEnd.floatValue;
                    if ((Method == NumberFormatMethod.Percentage) || (Method == NumberFormatMethod.Percentage2DecimalPlaces))
                        result_ok = nvCurrent.ValidateData(Project.ProcentToString(cValue), Method);
                    else
                        result_ok = nvCurrent.ValidateData(cValue.ToString(), Method);
                }
                else
                {
                    int dif = nvEnd.intValue - nvStart.intValue;
                    int cValue = nvStart.intValue + (dif * step) / Steps;
                    if (cValue > nvEnd.intValue)
                        cValue = nvEnd.intValue;
                    result_ok = nvCurrent.ValidateData(cValue.ToString(), Method);
                }
                TextPainter tp = ((AnimO.TextElement)ElementObject).tp;
                if (result_ok)
                    tp.SetText(nvCurrent.csFormatedValue, false, null);

                return step < Steps;
            }
            #endregion



            protected override string GetTransformationDescription()
            {
                return "Number increase from " + _Start + " to " + _End + " in " + Steps.ToString() + " steps";
            }
            public override string CreateInitializationCPPCode()
            {
                string s = "\n\t//" + GetType().ToString() + " transformation";
                s += "\n\t" + this.CppName + ".Element = &this->" + this.Element + ";";
                s += "\n\t" + this.CppName + "." + nvStart.cppType + "Start = " + nvStart.cppFormatRepresentation + ";";
                s += "\n\t" + this.CppName + "." + nvEnd.cppType + "End = " + nvEnd.cppFormatRepresentation + ";";
                s += "\n\t" + this.CppName + ".Steps = " + Steps.ToString() + " ;";
                s += "\n\t" + this.CppName + ".FormatType = " + ((int)Method).ToString() + " ;";
                return s + "\n";
            }
            public override void Validate(Project prj, string animationName, AppResources resources, Dictionary<string, GenericElement> elements)
            {
                if (nvStart.ValidateData(Start, Method) == false)
                    prj.EC.AddError("Animation '" + animationName + "' has an invalid NumericFormat transformation: => " + nvStart.error);
                if (nvEnd.ValidateData(End, Method) == false)
                    prj.EC.AddError("Animation '" + animationName + "' has an invalid NumericFormat transformation: => " + nvEnd.error);
            }
            public override string CreateOnStartCPPCode()
            {
                return GetOnStartFieldInit(UserValue_Start, nvStart.cppType + "Start") + GetOnStartFieldInit(UserValue_End, nvEnd.cppType + "End") + GetOnStartFieldInit(UserValue_Steps, "Steps") + base.CreateOnStartCPPCode();
            }
            public override void PopulateParameters(AnimationParameters p)
            {
                if (nvStart.ValidateData(Start, Method) == false)
                    nvStart.cppType = "???";
                if (UserValue_Start.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Start, nvStart.cppType));
                if (nvEnd.ValidateData(End, Method) == false)
                    nvEnd.cppType = "???";
                if (UserValue_End.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_End, nvEnd.cppType));
                if (UserValue_Steps.Length>0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Steps, "int"));
                base.PopulateParameters(p);
            }

        }

        public enum CountDownFormat : int
        {
            Auto = 0,
            Seconds = 1,
            MinutesAndSeconds = 2,
            HoursMinutesSeconds = 3,
            DaysHourMinutesSeconds = 4,
        };

        [XmlType("CountDown"), XmlRoot("CountDown")]
        public class CountDown : GenericElementTransformation
        {
            [XmlAttribute()]
            public int SecondsLeft = 30;
            [XmlAttribute()]
            public CountDownFormat Method = CountDownFormat.Auto;

            [XmlAttribute()]
            public string UserValue_SecondsLeft = "";

            #region Atribute
            [XmlIgnore(), Category("Format"), DisplayName("Seconds left"), Description("Number of seconds left from the countdown")]
            public int _SecondsLeft
            {
                get { return SecondsLeft; }
                set { if (value >= 0) SecondsLeft = value; }
            }

            [XmlIgnore(), Category("Format"), DisplayName("Method"), Description("Format method")]
            public CountDownFormat _Method
            {
                get { return Method; }
                set { Method = value; }
            }

            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Seconds left")]
            public string _UserValue_SecondsLeft
            {
                get { return UserValue_SecondsLeft; }
                set { UserValue_SecondsLeft = value; }
            }
            #endregion

            #region Dynamic Execution
            private int counter,cValue;

            private void UpdateSeconds()
            {
                int d, h, r;
                if (ElementObject.GetType() == typeof(AnimO.TextElement))
                {
                    TextPainter tp = ((AnimO.TextElement)ElementObject).tp;
                    string res = "";
                    CountDownFormat cd = Method;
                    if (cd == CountDownFormat.Auto)
                    {
                        if (cValue < 60)
                            cd = CountDownFormat.Seconds;
                        else if (cValue < (60 * 60))
                            cd = CountDownFormat.MinutesAndSeconds;
                        else if (cValue < (60 * 60 * 24))
                            cd = CountDownFormat.HoursMinutesSeconds;
                        else
                            cd = CountDownFormat.DaysHourMinutesSeconds;
                    }
                    switch (cd)
                    {
                        case CountDownFormat.Seconds:
                            res = String.Format(":{0:00}", cValue);
                            break;
                        case CountDownFormat.MinutesAndSeconds:
                            res = String.Format("{0:00}:{1:00}", cValue / 60, cValue % 60);
                            break;
                        case CountDownFormat.HoursMinutesSeconds:
                            h = cValue / 3600;r = cValue % 3600;
                            res = String.Format("{0:00}:{1:00}:{2:00}", h, r / 60, r % 60);
                            break;
                        case CountDownFormat.DaysHourMinutesSeconds:
                            d = cValue / (3600 * 24);r = cValue % (3600 * 24);
                            h = r / 3600; r = r % 3600;
                            res = String.Format("{0}d {1:00}:{2:00}:{3:00}", d, h, r / 60, r % 60);
                            break;
                    }
                    tp.SetText(res, false, null);
                }
            }

            protected override void OnInit()
            {
                cValue = SecondsLeft;
                counter = 0;
                UpdateSeconds();
            }
            protected override bool OnUpdate()
            {
                if (cValue <= 0)
                    return false;
                counter++;
                if (counter>=60)
                {
                    counter = 0;
                    cValue--;
                    UpdateSeconds();
                }
                return true;
            }
            #endregion



            protected override string GetTransformationDescription()
            {
                return "Count down from " + SecondsLeft.ToString() + " seconds";
            }
            public override string CreateInitializationCPPCode()
            {
                string s = "\n\t//" + GetType().ToString() + " transformation";
                s += "\n\t" + this.CppName + ".Element = &this->" + this.Element + ";";
                s += "\n\t" + this.CppName + ".SecondsLeft = " + SecondsLeft.ToString() + ";";
                s += "\n\t" + this.CppName + ".FormatType = " + ((int)Method).ToString() + " ;";
                return s + "\n";
            }
            public override string CreateOnStartCPPCode()
            {
                return GetOnStartFieldInit(UserValue_SecondsLeft, "SecondsLeft") + base.CreateOnStartCPPCode();
            }
            public override void PopulateParameters(AnimationParameters p)
            {
                if (UserValue_SecondsLeft.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_SecondsLeft, "int"));
                base.PopulateParameters(p);
            }

        }


        public class TransformationDescriptor
        {
            public Type type;
            public String Name, Icon, Description;
            public HashSet<Type> CompatibilityElements;
            public TransformationDescriptor(Type _type, string name,string icon,string compatibility, string description)
            {
                type = _type;
                Name = name;
                Icon = icon;
                Description = description;
                CompatibilityElements = new HashSet<Type>();
                foreach (char ch in compatibility)
                {
                    switch (ch)
                    {
                        case 'I': CompatibilityElements.Add(typeof(AnimO.ImageElement)); break;
                        case 'T': CompatibilityElements.Add(typeof(AnimO.TextElement)); break;
                        case 'S': CompatibilityElements.Add(typeof(AnimO.EntireSurfaceElement)); break;
                        case 'R': CompatibilityElements.Add(typeof(AnimO.RectangleElement)); break;
                        case 'E': CompatibilityElements.Add(typeof(AnimO.ExclusionRectangleElement)); break;
                        case 'C': CompatibilityElements.Add(typeof(AnimO.ClipRectangleElement)); break;
                        case 'B': CompatibilityElements.Add(typeof(AnimO.SimpleButtonElement)); break;
                    }
                }
            }

            #region List View Props
            [XmlIgnore(), Browsable(false)]
            public string propName { get { return Name; } }
            [XmlIgnore(), Browsable(false)]
            public string propDescription { get { return Description; } }
            [XmlIgnore(), Browsable(false)]
            public string propIcon { get { return Icon; } }
            #endregion
        };
        public class Factory
        {
            private static List<TransformationDescriptor> transformation_list = new List<TransformationDescriptor>()
            {
                // alpha
                new TransformationDescriptor(typeof(AlphaBlendingLinearTransformation),"Alpha blending linear transform","alpha_blending_linear","ITSRE","Increses/Decreses alpha value within a specific interval"),
                new TransformationDescriptor(typeof(AlphaBlendingForwardAndBackTransformation),"Back and Forward aplha blending","alpha_blending_forward_and_back","ITSRE","Increses/Decreses alpha value within a specific interval from start to and and then back to start"),
                // scale
                new TransformationDescriptor(typeof(ScaleLinearTransformation),"Scale linear transform","scale_linear","ITRECB","Increses/Decreses image or text scale within a specific interval"),
                new TransformationDescriptor(typeof(ScaleForwardAndBackTransformation),"Back and Forward scale transformation","scale_forrward_and_back","ITRECB","Scale an image/text to a specific value and then go back to the original one"),
                new TransformationDescriptor(typeof(ScaleWidthLinearTransformation),"Width Scale linear transform","scale_width_linear","IRECB","Increses/Decreses image or text width scale within a specific interval"),
                new TransformationDescriptor(typeof(ScaleWidthForwardAndBackTransformation),"Back and Forward Width scale transformation","scale_width_forward_and_back","IRECB","Scale the width for an image/text to a specific value and then go back to the original one"),
                new TransformationDescriptor(typeof(ScaleHeightLinearTransformation),"Height Scale linear transform","scale_height_linear","IRECB","Increses/Decreses image or text height scale within a specific interval"),
                new TransformationDescriptor(typeof(ScaleHeightForwardAndBackTransformation),"Back and Forward Height scale transformation","scale_height_forward_and_back","IRECB","Scale the height for an image/text to a specific value and then go back to the original one"),
                // color
                new TransformationDescriptor(typeof(ColorBlendLinearTransformation),"Color blending linear transformation","color_linear","ITSRE","Change the blending color from one color to another !"),
                new TransformationDescriptor(typeof(ColorBlendForwardAndBackTransformation),"Color blending back and forward transformation","color_forward_and_back","ITSRE","Change the blending color from one color to another and back !"),
                new TransformationDescriptor(typeof(ColorBlendStateTransformation),"Blend color","color","ITSRE","Sets the blending color for an element"),
                new TransformationDescriptor(typeof(AlphaBlendingStateTransformation),"Alpha blending","alpha_blending","ITSRE","Sets the alpha channel for an element"),
                // move
                new TransformationDescriptor(typeof(MoveRelativeLinearTransformation),"Move element relative to its position","move_relative_linear","ITRECB","Move element relative to its position with an X offset and an Y offset "),
                new TransformationDescriptor(typeof(MoveAbsoluteLinearTransformation),"Move element to a position","move_absolute_linear","ITRECB","Move element to an absolute position."),
                new TransformationDescriptor(typeof(QuadraticBezierTransformation),"Quadratic Bezier move","move_bezier","ITRECB","Move element quadratic bezier curves"),
                new TransformationDescriptor(typeof(SetNewAbsolutePositionTransformation),"Set position","absolute_position","ITRECB","Move element to an absolute position"),
                new TransformationDescriptor(typeof(SetNewRelativePositionTransformation),"Move to position","relative_position","ITRECB","Move element relative to its current position"),
                // state
                new TransformationDescriptor(typeof(VisibleStateTransformation),"Visibility state","visible","ITSRECB","Changes the visibility state of an element"),
                // specific texte
                new TransformationDescriptor(typeof(SetNewTextTransformation),"Set text","set_text","T","Set a new text for a text element"),
                new TransformationDescriptor(typeof(TextFlowTransformation),"Text flow","text_flow","T","Display a text character by character"),
                new TransformationDescriptor(typeof(TextCenterFlowTransformation),"Text Center flow","text_center_flow","T","Display a text character by character starting from the center and moving to margins"),
                new TransformationDescriptor(typeof(FontSizeTransformation),"Font size","font_size","T","Sets the size of the font used by a text element."),
                new TransformationDescriptor(typeof(TextCharacterVisibilityTransformation),"Character visibility","char_visibility","T","Sets the visibility state of characters from a text !"),
                new TransformationDescriptor(typeof(NumericFormatterTransformation),"Numeric formatter","numeric_formatter","T","Formats a number"),
                new TransformationDescriptor(typeof(NumberIncreaseTransformation),"Number increase","numeric_formatter_linear","T","Increases/Decreases a number from a start value to an end value."),
                new TransformationDescriptor(typeof(CountDown),"Count down","countdown","T","Creates a count down for a specific number of seconds/ticks !"),

                // specific imagini
                new TransformationDescriptor(typeof(SetNewImageTransformation),"Set image","set_image","I","Set a new image to an image element"),
                new TransformationDescriptor(typeof(SetImageIndexTransformation),"Set image index","set_image_index","I","Set the index of the image from the multi image element"),
                new TransformationDescriptor(typeof(ImageIndexLinearTransformation),"Image index linear transformation","image_index_linear","I","Changes the index of the image from the multi image element within a specific interval"),                

                // specific butoane
                new TransformationDescriptor(typeof(ButtonEnableTransformation),"Set button enable state","set_button_enable_state","B","Set the state of a button - enabled or not"),
            };
            private static Dictionary<Type, TransformationDescriptor> transformations_dict = null;
            private static void CreateTransformationDictionary()
            {
                if (transformations_dict != null)
                    return;
                transformations_dict = new Dictionary<Type, TransformationDescriptor>();
                foreach (TransformationDescriptor td in transformation_list)
                    transformations_dict[td.type] = td;
            }

            public static string GetName(Type transformationType)
            {
                CreateTransformationDictionary();
                if (transformations_dict.ContainsKey(transformationType))
                    return transformations_dict[transformationType].Name;
                return "??? (" + transformationType.ToString() + ")";
            }
            public static string GetIconKey(Type transformationType)
            {
                CreateTransformationDictionary();
                if (transformations_dict.ContainsKey(transformationType))
                    return transformations_dict[transformationType].Icon;
                return "";
            }
            public static bool IsTransformationCompatible(Type transformationType, AnimO.GenericElement elem)
            {
                return IsTransformationCompatible(transformationType, elem.GetType());
            }
            public static bool IsTransformationCompatible(Type transformationType, Type elementType)
            {
                CreateTransformationDictionary();
                if (transformations_dict.ContainsKey(transformationType))
                    return transformations_dict[transformationType].CompatibilityElements.Contains(elementType);
                return false;
            }
            public static List<TransformationDescriptor>  GetTransformationList()
            {
                return transformation_list;
            }
            public static GenericTransformation CreateTransformation(Type transformationType, GenericElement element)
            {
                GenericElementTransformation trans = (GenericElementTransformation)Activator.CreateInstance(transformationType);
                if (trans!=null)
                {
                    trans.Element = element.Name;
                }
                return trans;
            }
        }

        #endregion

        #region Elements
        public enum YesNo
        {            
            No,
            Yes,
        };
        public enum SimpleButtonSetter
        {
            No,
            OneForEachState,
            OneForAll,
        };
        public enum PositionGetterType
        {
            No,
            Percentages,
            Pixels
        };

        public class ElementRectangle
        {
            public float X_Percentage;
            public float Y_Percentage;
            public float WidthInPixels;
            public float HeightInPixels;
            public Alignament Align;
            public bool FullScreen;
        };

        [XmlInclude(typeof(EntireSurfaceElement))]
        [XmlInclude(typeof(TextElement))]
        [XmlInclude(typeof(ImageElement))]
        [XmlInclude(typeof(RectangleElement))]
        [XmlInclude(typeof(ExclusionRectangleElement))]
        [XmlInclude(typeof(ClipRectangleElement))]
        [XmlInclude(typeof(DisableClippingElement))]
        [XmlInclude(typeof(SimpleButtonElement))]
        [XmlInclude(typeof(GenericElementWithPosition))]
        [XmlInclude(typeof(GenericElementWithPositionAndSize))]
        [XmlType("Element"), XmlRoot("Element")]
        public class GenericElement
        {            
            [XmlAttribute()]
            public string Name = "";
            [XmlAttribute()]
            public bool Visible = true;
            [XmlAttribute()]
            public string Parent = "<None>";
            [XmlAttribute()]
            public string UserValue_Visible = "";
            [XmlIgnore()]
            public RuntimeContext ExecutionContext = new RuntimeContext();

            [XmlIgnore()]
            public static AppResources CurrentAppResources = null;
            [XmlIgnore()]
            public static IRefreshDesign RefreshDesignCallback = null;

            [XmlIgnore()]
            public bool _ShowInBoardAnimation_ = true;
            [XmlIgnore()]
            public bool _FoundInZOrder_ = false;
            [XmlIgnore()]
            public GenericElement ParentElement = null;


            #region Atribute
            [XmlIgnore(), Description("Name"), Category("General"), DisplayName("Name")]
            public string _Name
            {
                get { return Name; }
                set { Name = value; }
            }
            [XmlIgnore(), Description("Initial Visibility"), Category("General"), DisplayName("Visible")]
            public bool _Visible
            {
                get { return Visible; }
                set { Visible = value; }
            }
            [XmlIgnore(), Description("Parent element (to compute the relative position)"), Category("General"), DisplayName("Parent"), Editor(typeof(AnimationElementRelativePositionEditor), typeof(UITypeEditor))]
            public string _RelativeToElement
            {
                get { return Parent; }
                set
                {
                    if ((value.Equals("")) || (value.Equals("<None>", StringComparison.InvariantCultureIgnoreCase)))
                        Parent = "<None>";
                    else
                        Parent = value;
                }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Visible")]
            public string _UserValue_Visible
            {
                get { return UserValue_Visible; }
                set { UserValue_Visible = value; }
            }    
            
            #endregion

            #region List View Props
            [XmlIgnore(), Browsable(false)]
            public string propName { get { return Name; } }
            [XmlIgnore(), Browsable(false)]
            public string propDescription { get { return GetDescription(); } }
            [XmlIgnore(), Browsable(false)]
            public string propIcon { get { return GetIconKey(); } }
            [XmlIgnore(), Browsable(false)]
            public bool propVisible 
            {
                set { _ShowInBoardAnimation_ = value; if (RefreshDesignCallback != null) RefreshDesignCallback.Refresh(); }
                get { return _ShowInBoardAnimation_; } 
            }            
            #endregion

            #region Internal functions
            protected string GetPercentageValue(float value)
            {
                return Project.ProcentToString(value);
            }
            protected float SetPercentageValue(string strRepresentation,float currentValue)
            {
                return Project.StringToProcent(strRepresentation, 0, 1000, currentValue);
            }
            protected string GetLocationValue(float value)
            {
                return Project.ProcentToString(value);
            }
            protected float SetLocationValue(string strRepresentation, float currentValue)
            {
                return Project.StringToProcent(strRepresentation, -1000, 1000, currentValue);
            }
            public static string GetSizeInPixels(float value)
            {
                return value.ToString() + " px";
            }
            public static float SetSizeInPixels(string strRepresentation, float currentValue)
            {
                float result = 0.0f;
                if (float.TryParse(strRepresentation.ToLower().Replace("px","").Trim(),out result))
                {
                    if (result >= 0)
                        return result;
                }
                return currentValue;
            }
            protected string GetParamOrDefaultValue(string defaultValue, string paramName)
            {
                if (paramName.Length > 0)
                    return "param_" + paramName;
                return defaultValue;
            }
            protected void CreateGetterForPosition(GACParser.Module m)
            {
                string nm = "Get" + Name + "X";
                m.Members[nm] = new GACParser.Member(m, GACParser.MemberType.Function, nm, nm, "float", "", null);
                nm = "Get" + Name + "Y";
                m.Members[nm] = new GACParser.Member(m, GACParser.MemberType.Function, nm, nm, "float", "", null);
            }
            protected string CreateGetPositionCPPHeaderDefinition()
            {
                return "\n\tfloat Get" + Name + "X ();\n\tfloat Get" + Name + "Y ();";
            }
            protected string CreateGetPositionCPPImplementation(string className, PositionGetterType pType)
            {
                return "\nfloat " + className + "::Get" + Name + "X () { return " + Name + ".GetX(this," + (pType == PositionGetterType.Pixels).ToString().ToLower() + "); }\nfloat " + className + "::Get" + Name + "Y (){ return " + Name + ".GetY(this," + (pType == PositionGetterType.Pixels).ToString().ToLower() + "); }";
            }
            protected void CreateSetterForColor(GACParser.Module m,string setterName)
            {
                string nm = "Set" + Name + setterName;
                GACParser.Member mb = new GACParser.Member(m, GACParser.MemberType.Function, nm, nm, "void", "", null);
                mb.AddParameter(new GACParser.Member(m, GACParser.MemberType.Variable, "color", "color", "Color", "", null));
                m.Members[nm] = mb;
            }
            protected string CreateSetColorCPPImplementation(string className, string setterName)
            {
                return "\nvoid " + className + "::Set" + Name + setterName + " (unsigned int __color__) { " + Name + ".ColorTransform = __color__; if (" + Name + ".CallVirtualFunction) " + Name + ".OnUpdateBlendColor(this);  };";
            }
            protected string CreateSetColorCPPHeaderDefinition(string setterName)
            {
                return "\n\tvoid Set" + Name + setterName+" (unsigned int __color__);";
            }

            #endregion


            #region Virtual Functions
            protected virtual float GetWidthInPixels() { return 0; }
            protected virtual float GetHeightInPixels() { return 0; }
            protected virtual void SetPositionAndSize(float x_percentage, float y_percantage, float unscaled_widthInPixels, float unscaled_heightInPixels, float scaledWidth, float scaleHeight)
            {

            }
            public virtual bool IsFullScreen() { return false; }
            public static void ComputeScreenRect(float percentage_X, float percentage_y, float widthInPixels, float heightInPixels, float scaleWidth, float scaleHeight, Alignament align, float parentLeftInPixels, float parentTopInPixels, float parentWidth, float parentHeight, ref RectangleF result)
            {
                float x = percentage_X * parentWidth + parentLeftInPixels;
                float y = percentage_y * parentHeight + parentTopInPixels;
                widthInPixels *= scaleWidth;
                heightInPixels *= scaleHeight;
                switch (align)
                {
                    case Alignament.Center: x -= widthInPixels / 2.0f; y -= heightInPixels / 2.0f; break;
                    case Alignament.TopLeft: break;
                    case Alignament.TopCenter: x -= widthInPixels / 2.0f; break;
                    case Alignament.TopRight: x -= widthInPixels; break;
                    case Alignament.RightCenter: x -= widthInPixels; y -= heightInPixels / 2.0f; break;
                    case Alignament.BottomRight: x -= widthInPixels; y -= heightInPixels; break;
                    case Alignament.BottomCenter: x -= widthInPixels / 2.0f; y -= heightInPixels; break;
                    case Alignament.BottomLeft: y -= heightInPixels; break;
                    case Alignament.LeftCenter: y -= heightInPixels / 2.0f; break;
                }
                result.X = x;
                result.Y = y;
                result.Width = widthInPixels;
                result.Height = heightInPixels;
            }
            public static void ComputeScreenRect(float percentage_X, float percentage_y, float widthInPixels, float heightInPixels, float scaleWidth, float scaleHeight, Alignament align, RectangleF parent, ref RectangleF result)
            {
                ComputeScreenRect(percentage_X, percentage_y, widthInPixels, heightInPixels, scaleWidth, scaleHeight, align, parent.Left, parent.Top, parent.Width, parent.Height, ref result);
            }
            public static void ComputeScreenRect(RuntimeContext rContext, bool useImage, RectangleF parent)
            {
                float w, h;
                if (useImage)
                {
                    if (rContext.Image != null)
                    {
                        w = rContext.Image.Width;
                        h = rContext.Image.Height;
                    } else
                    {
                        w = 0;
                        h = 0;
                    }
                } else
                {
                    w = rContext.WidthInPixels;
                    h = rContext.HeightInPixels;
                }
                ComputeScreenRect(rContext.X_Percentage, rContext.Y_Percentage, w, h, rContext.ScaleWidth, rContext.ScaleHeight, rContext.Align, parent.Left, parent.Top, parent.Width, parent.Height, ref rContext.ScreenRect);
            }

            public void ComputeScreenRect(float screenWidthInPixels,float screenHeightInPixels)
            {
                float w, h, s_w, s_h;
                if (IsFullScreen())
                {
                    w = screenWidthInPixels;
                    h = screenHeightInPixels;
                    s_w = s_h = 1.0f;
                }
                else
                {
                    w = GetWidthInPixels(); s_w = this.ExecutionContext.ScaleWidth;
                    h = GetHeightInPixels(); s_h = this.ExecutionContext.ScaleHeight;
                }
                if (this.ParentElement != null)
                    ComputeScreenRect(ExecutionContext.X_Percentage, ExecutionContext.Y_Percentage, w, h, s_w, s_h, ExecutionContext.Align, this.ParentElement.ExecutionContext.ScreenRect, ref ExecutionContext.ScreenRect);
                else
                    ComputeScreenRect(ExecutionContext.X_Percentage, ExecutionContext.Y_Percentage, w, h, s_w, s_h, ExecutionContext.Align, 0, 0, screenWidthInPixels, screenHeightInPixels, ref ExecutionContext.ScreenRect);
            }
            public void UpdateFromScreenRect(float leftInPixels, float topInPixels, float widthInPixels, float HeightInPixels, float screenWidthInPixels, float screenHeightInPixels)
            {
                float x_p = 0, y_p = 0; // x_p = leftInPixels + widthInPixels/ 2.0f;

                //
                //

                switch (this.ExecutionContext.Align)
                {
                    case Alignament.Center:  
                    case Alignament.RightCenter: 
                    case Alignament.LeftCenter: y_p = topInPixels + HeightInPixels / 2.0f; break;

                    case Alignament.TopLeft:
                    case Alignament.TopCenter:
                    case Alignament.TopRight: y_p = topInPixels; break;

                    case Alignament.BottomRight:  
                    case Alignament.BottomCenter: 
                    case Alignament.BottomLeft: y_p = topInPixels + HeightInPixels; break;
                }
                switch (this.ExecutionContext.Align)
                {
                    case Alignament.TopLeft:
                    case Alignament.BottomLeft:
                    case Alignament.LeftCenter: x_p = leftInPixels; break;

                    case Alignament.BottomRight:
                    case Alignament.RightCenter:
                    case Alignament.TopRight: x_p = leftInPixels+widthInPixels; break;

                    case Alignament.Center:
                    case Alignament.TopCenter:
                    case Alignament.BottomCenter: x_p = leftInPixels + widthInPixels / 2.0f; break;
                }
                if (this.ParentElement==null)
                {
                    x_p = x_p / screenWidthInPixels;
                    y_p = y_p / screenHeightInPixels;
                } else
                {
                    x_p = (x_p - this.ParentElement.ExecutionContext.ScreenRect.Left) / this.ParentElement.ExecutionContext.ScreenRect.Width;
                    y_p = (y_p - this.ParentElement.ExecutionContext.ScreenRect.Top) / this.ParentElement.ExecutionContext.ScreenRect.Height;
                }
                // am recalculat pozitiile
                // calculez si sclaraile
                float scale_w = 0;
                float scale_h = 0;
                float w_pixels = 0;
                float h_pixels = 0;
                // pot sa am 2 cazuri
                // a) modific scale-ul
                // b) modific size-ul

                // a) modific scale-ul
                scale_w = widthInPixels / this.GetWidthInPixels();
                scale_h = HeightInPixels / this.GetHeightInPixels();
                w_pixels = widthInPixels / this.ExecutionContext.ScaleWidth;
                h_pixels = HeightInPixels / this.ExecutionContext.ScaleHeight;
                SetPositionAndSize(x_p, y_p, w_pixels, h_pixels, scale_w, scale_h);
            }

            protected virtual string GetDescription()
            {
                return "<None>";
            }
            public virtual string GetTypeName()
            {
                return "";
            }
            public virtual string GetIconKey()
            {
                return "";
            }
            public virtual void InitRuntimeContext()
            {
                ExecutionContext.Visible = Visible;
            }
            public virtual void OnPaint(Canvas c, float deviceWidth, float deviceHeight, AnimO.BoardViewMode viewMode) { }
            public void Paint(Canvas c,float deviceWidth, float deviceHeight, AnimO.BoardViewMode viewMode)
            {
                switch (viewMode)
                {
                    case BoardViewMode.Design:
                        if (_ShowInBoardAnimation_ == false)
                            return;
                        OnPaint(c, deviceWidth, deviceHeight, viewMode);
                        break;
                    case BoardViewMode.Play:
                        if (ExecutionContext.Visible)
                            OnPaint(c, deviceWidth, deviceHeight, viewMode);
                        break;
                }
            }
            public virtual string Validate(Project prj, AppResources resources)
            {
                return "Missing implementation for this element type: "+GetType().ToString();
            }
            public virtual string GetCPPClassName() { return "?"; }
            public virtual string CreateOnStartCPPCode(AnimationObject animObj)
            {
                string s = "";
                s += "\n\t" + Name + ".Visible = " + GetParamOrDefaultValue(this.Visible.ToString().ToLower(), UserValue_Visible) + ";";
                if (this.Parent.Equals("<None>"))
                    s += "\n\t" + Name + ".Parent = NULL;";
                else
                    s += "\n\t" + Name + ".Parent = &"+Parent+";";
                return s;
            }
            public virtual void PopulateParameters(AnimationParameters p)
            {
                if (UserValue_Visible.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Visible, "bool"));
            }
            public virtual void AddAnimationFunction(GACParser.Module m) { }
            public virtual string GetAnimationFunctionCPPHeaderDefinition() { return ""; }
            public virtual string GetAnimationFunctionCPPImplementation(string className) { return ""; }
            #endregion

            public string ToXMLString()
            {
                try
                {
                    var stringwriter = new System.IO.StringWriter();
                    var serializer = new XmlSerializer(typeof(GenericElement));
                    serializer.Serialize(stringwriter, this);
                    return stringwriter.ToString();
                }
                catch (Exception e)
                {
                    MessageBox.Show("Unable to convert element to XML: \n" + e.ToString());
                    return "";
                }
            }

            public static GenericElement FromXMLString(string xmlText)
            {
                try
                {
                    var stringReader = new System.IO.StringReader(xmlText);
                    var serializer = new XmlSerializer(typeof(GenericElement));
                    GenericElement af = serializer.Deserialize(stringReader) as GenericElement;
                    if (af == null)
                    {
                        MessageBox.Show("Unable to create element from XML: \n" + xmlText);
                    }
                    return af;
                }
                catch (Exception e)
                {
                    MessageBox.Show("Unable to create element from XML: \n" + e.ToString());
                    return null;
                }
            }

            public GenericElement MakeCopy()
            {
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(GenericElement));
                    StringWriter textWriter = new StringWriter();
                    serializer.Serialize(textWriter, this);
                    StringReader textReader = new StringReader(textWriter.ToString());
                    return (GenericElement)serializer.Deserialize(textReader);
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }
        public class GenericElementWithPosition: GenericElement
        {
            [XmlAttribute()]
            public Alignament Align = Alignament.Center;
            [XmlAttribute()]
            public float ScaleWidth = 1.0f;
            [XmlAttribute()]
            public float ScaleHeight = 1.0f;
            [XmlAttribute()]
            public float X = 0.5f;
            [XmlAttribute()]
            public float Y = 0.5f;
            [XmlAttribute()]
            public string UserValue_X = "", UserValue_Y = "", UserValue_ScaleWidth = "", UserValue_ScaleHeight = "", UserValue_Align = "";
            [XmlAttribute()]
            public PositionGetterType Getter_Position = PositionGetterType.No;


            #region Atribute
            [XmlIgnore(), Description("Rectangle Aliganment"), Category("Layout"), DisplayName("Alignament")]
            public Alignament _Align
            {
                get { return Align; }
                set { Align = value; }
            }
            [XmlIgnore(), Description("Scale width"), Category("Zoom"), DisplayName("Scale Width")]
            public string _ScaleWidth
            {
                get { return GetPercentageValue(ScaleWidth); }
                set { ScaleWidth = SetPercentageValue(value, ScaleWidth); }
            }
            [XmlIgnore(), Description("Scale height"), Category("Zoom"), DisplayName("Scale Height")]
            public string _ScaleHeight
            {
                get { return GetPercentageValue(ScaleHeight); }
                set { ScaleHeight = SetPercentageValue(value, ScaleHeight); }
            }
            [XmlIgnore(), Description("Object initial location"), Category("Layout"), DisplayName("X")]
            public string _X
            {
                get { return GetLocationValue(X); }
                set { X = SetLocationValue(value,  X); }
            }
            [XmlIgnore(), Description("Object initial location"), Category("Layout"), DisplayName("Y")]
            public string _Y
            {
                get { return GetLocationValue(Y); }
                set { Y = SetLocationValue(value,  Y); }
            }

            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("X")]
            public string _UserValue_X
            {
                get { return UserValue_X; }
                set { UserValue_X = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Y")]
            public string _UserValue_Y
            {
                get { return UserValue_Y; }
                set { UserValue_Y = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Alignament")]
            public string _UserValue_Align
            {
                get { return UserValue_Align; }
                set { UserValue_Align = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Scale Width")]
            public string _UserValue_ScaleWidth
            {
                get { return UserValue_ScaleWidth; }
                set { UserValue_ScaleWidth = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Scale Height")]
            public string _UserValue_ScaleHeight
            {
                get { return UserValue_ScaleHeight; }
                set { UserValue_ScaleHeight = value; }
            }
            #endregion

            #region Getters
            [XmlIgnore(), Description("Specifies if a getter for element position is to be created."), Category("Getters"), DisplayName("Position")]
            public PositionGetterType _Getter_Position
            {
                get { return Getter_Position; }
                set { Getter_Position = value; }
            }
            #endregion

            #region Virtual functions

            public override void PopulateParameters(AnimationParameters p)
            {
                base.PopulateParameters(p);
                if (UserValue_X.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_X, "float"));
                if (UserValue_Y.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Y, "float"));
                if (UserValue_ScaleWidth.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_ScaleWidth, "float"));
                if (UserValue_ScaleHeight.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_ScaleHeight, "float"));
                if (UserValue_Align.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Align, "unsigned int", "Alignament"));
            }
            public override string CreateOnStartCPPCode(AnimationObject animObj)
            {
                string s = base.CreateOnStartCPPCode(animObj);

                s += "\n\t" + Name + ".X = " + GetParamOrDefaultValue(this.X.ToString(), UserValue_X) + ";";
                s += "\n\t" + Name + ".Y = " + GetParamOrDefaultValue(this.Y.ToString(), UserValue_Y) + ";";
                s += "\n\t" + Name + ".ScaleWidth = " + GetParamOrDefaultValue(this.ScaleWidth.ToString(), UserValue_ScaleWidth) + ";";
                s += "\n\t" + Name + ".ScaleHeight = " + GetParamOrDefaultValue(this.ScaleHeight.ToString(), UserValue_ScaleHeight) + ";";
                s += "\n\t" + Name + ".Alignament = " + GetParamOrDefaultValue("GAC_ALIGNAMENT_" + this.Align.ToString().ToUpper(), UserValue_Align) + ";";
                return s;
            }
            public override void InitRuntimeContext()
            {
                base.InitRuntimeContext();
                ExecutionContext.ScaleWidth = ScaleWidth;
                ExecutionContext.ScaleHeight = ScaleHeight;
                ExecutionContext.X_Percentage = X;
                ExecutionContext.Y_Percentage = Y;
                ExecutionContext.Align = Align;
            }
            public override void AddAnimationFunction(GACParser.Module m)
            {
                base.AddAnimationFunction(m);
                if (Getter_Position != PositionGetterType.No)
                    CreateGetterForPosition(m);
            }
            public override string GetAnimationFunctionCPPHeaderDefinition()
            {
                string s = base.GetAnimationFunctionCPPHeaderDefinition();
                if (Getter_Position != PositionGetterType.No)
                    s += CreateGetPositionCPPHeaderDefinition();
                return s;
            }
            public override string GetAnimationFunctionCPPImplementation(string className)
            {
                string s = base.GetAnimationFunctionCPPImplementation(className);
                if (Getter_Position != PositionGetterType.No)
                    s += CreateGetPositionCPPImplementation(className, Getter_Position);
                return s;
            }
            public override string Validate(Project prj, AppResources resources)
            {

                return null;
            }
            #endregion
        }
        public class GenericElementWithPositionAndSize : GenericElementWithPosition
        {
            [XmlAttribute()]
            public float Width = 100;
            [XmlAttribute()]
            public float Height = 100;
            [XmlAttribute()]
            public string UserValue_Width = "", UserValue_Height = "";

            #region Atribute
            [XmlIgnore(), Description("Width of the object"), Category("Layout"), DisplayName("Width")]
            public string _Width
            {
                get { return GetSizeInPixels(Width); }
                set { Width = SetSizeInPixels(value,  Width); }
            }
            [XmlIgnore(), Description("Height of the object"), Category("Layout"), DisplayName("Height")]
            public string _Height
            {
                get { return GetSizeInPixels(Height); }
                set { Height = SetSizeInPixels(value,  Height); }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Width")]
            public string _UserValue_Width
            {
                get { return UserValue_Width; }
                set { UserValue_Width = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Height")]
            public string _UserValue_Height
            {
                get { return UserValue_Height; }
                set { UserValue_Height = value; }
            }
            #endregion

            #region Virtual functions
            protected override float GetWidthInPixels() { return this.Width; }
            protected override float GetHeightInPixels() { return this.Height; }
            protected override string GetDescription()
            {
                return Align.ToString() + "(" + GetLocationValue(X) + "," + GetLocationValue(Y) + ") Size = [" + GetLocationValue(Width) + " x " + GetLocationValue(Height) + " pixels]";
            }
            public override void PopulateParameters(AnimationParameters p)
            {
                base.PopulateParameters(p);
                if (UserValue_Width.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Width, "float"));
                if (UserValue_Height.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Height, "float"));
            }
            public override string CreateOnStartCPPCode(AnimationObject animObj)
            {
                string s = base.CreateOnStartCPPCode(animObj);
                s += "\n\t" + Name + ".WidthInPixels = " + GetParamOrDefaultValue(this.Width.ToString()+ "* (Core.ResolutionAspectRatio)", UserValue_Width) + ";";
                s += "\n\t" + Name + ".HeightInPixels = " + GetParamOrDefaultValue(this.Height.ToString() + "* (Core.ResolutionAspectRatio)", UserValue_Height) + ";";
                return s;
            }
            public override void InitRuntimeContext()
            {
                base.InitRuntimeContext();
                ExecutionContext.WidthInPixels = Width;
                ExecutionContext.HeightInPixels = Height;
            }
            #endregion
        }
        public class ImageElement: GenericElementWithPosition
        {
            [XmlAttribute()]
            public string UserValue_ColorBlending = "", UserValue_Images = "";
            [XmlAttribute()]
            public int ColorBlending = -1;
            [XmlAttribute()]
            public string Images = "";
            [XmlAttribute()]
            public int ImageIndex = 0;
            [XmlAttribute()]
            public YesNo Getter_Image = YesNo.No;
            [XmlAttribute()]
            public YesNo Setter_Image = YesNo.No;
            [XmlAttribute()]
            public YesNo Setter_Color = YesNo.No;

            #region Atribute
            [XmlIgnore(), Category("Image"), DisplayName("Images"), Description("List of images used"), Editor(typeof(ImageListSelectorEditor), typeof(UITypeEditor))]
            public string _Images
            {
                get { return Images; }
                set { Images = value; }
            }
            [XmlIgnore(), Category("Image"), DisplayName("Image Index"), Description("Image index from the list of indexes")]
            public int _ImageIndex
            {
                get { return ImageIndex; }
                set {
                    if (value < 0)
                        ImageIndex = 0;
                    else
                    {
                        int count = Project.StringListToList(Images, ';').Count;
                        if (value >= count)
                            ImageIndex = count - 1;
                        else
                            ImageIndex = value;
                        if (ImageIndex < 0)
                            ImageIndex = 0;
                    }
                }
            }                       
            [XmlIgnore(), Category("Image"), DisplayName("Blend Color")]
            public System.Drawing.Color _ColorBlending
            {
                get { return System.Drawing.Color.FromArgb(ColorBlending); }
                set { ColorBlending = value.ToArgb(); }
            }
            #endregion

            #region Initialization parameters (user dependent)
            public string _UserValue_ColorBlending
            {
                get { return UserValue_ColorBlending; }
                set { UserValue_ColorBlending = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Image/Images")]
            public string _UserValue_Images
            {
                get { return UserValue_Images; }
                set { UserValue_Images = value; }
            }
            #endregion

            #region Getters and Setters
            [XmlIgnore(), Description("Specifies if a getter for element image is to be created."), Category("Getters"), DisplayName("Image")]
            public YesNo _Getter_Image
            {
                get { return Getter_Image; }
                set { Getter_Image = value; }
            }
            [XmlIgnore(), Description("Sets the image for this specific element"), Category("Setters"), DisplayName("Image")]
            public YesNo _Setter_Image
            {
                get { return Setter_Image; }
                set { Setter_Image = value; }
            }
            [XmlIgnore(), Description("Sets the image for this specific element"), Category("Setters"), DisplayName("Blend Color")]
            public YesNo _Setter_Color
            {
                get { return Setter_Color; }
                set { Setter_Color = value; }
            }
            #endregion

            #region Virtual Functions
            private List<Bitmap> internalImages = new List<Bitmap>();

            protected override float GetWidthInPixels()
            {
                if (ImageIndex < internalImages.Count)
                    return internalImages[ImageIndex].Width;
                return 0;
            }
            protected override float GetHeightInPixels()
            {
                if (ImageIndex < internalImages.Count)
                    return internalImages[ImageIndex].Height;
                return 0;
            }
            protected override void SetPositionAndSize(float x_percentage, float y_percantage, float unscaled_widthInPixels, float unscaled_heightInPixels, float scaledWidth, float scaleHeight)
            {
                this.X = x_percentage;
                this.Y = y_percantage;
                this.ScaleWidth = scaledWidth;
                this.ScaleHeight = scaleHeight;
            }
            protected override string GetDescription()
            {
                int count = Project.StringListToList(Images, ';').Count;
                if (count<=1)
                {
                    if (Images.Trim().Length == 0)
                        return ("<no image/images selected>");
                    return Images;
                }
                else
                {
                    return count.ToString() + " images (" + Images + ")";
                }                
            }
            public override string GetIconKey()
            {
                List<string> imgLst = Project.StringListToList(Images, ';');
                if ((imgLst != null) && (imgLst.Count > ImageIndex) && (ImageIndex>=0))
                    return GenericResource.GetResourceVariableKey(typeof(ImageResource), imgLst[ImageIndex]);
 
                return "";
            }
            public override string GetTypeName()
            {
                return "Image";
            }
            public override void InitRuntimeContext()
            {
                base.InitRuntimeContext();
                ExecutionContext.ColorBlending = RuntimeContext.BlendModeToColor(BlendingMode.ColorBlending, ColorBlending, 1.0f);
                ExecutionContext.Image = internalImages[ImageIndex];
            }
            public override void OnPaint(Canvas c, float deviceWidth, float deviceHeight, BoardViewMode viewMode)
            {
                c.DrawImage(this.ExecutionContext);
            }
            public override string Validate(Project prj, AppResources resources)
            {
                List<String> lst = Project.StringListToList(Images, ';');
                if ((lst == null) || (lst.Count == 0))
                    return "Expecting at least one image !";
                internalImages.Clear();
                foreach (string s in lst)
                {
                    if (resources.Images.ContainsKey(s) == false)
                        return "Image resource with name '" + s + "' does not exists !";
                    internalImages.Add(resources.Images[s].Picture);
                }
                if (ImageIndex < 0)
                    return "Expecting a valid image index - between 0 and " + (internalImages.Count - 1).ToString();
                if (ImageIndex >= internalImages.Count)
                    return "Expecting a valid image index - between 0 and " + (internalImages.Count - 1).ToString();
                return null;
            }
            public override string CreateOnStartCPPCode(AnimationObject animObj)
            {
                string s = base.CreateOnStartCPPCode(animObj);
                s += "\n\t" + Name + ".ColorTransform = " + GetParamOrDefaultValue("0x" + this.ColorBlending.ToString("X8"), UserValue_ColorBlending) + ";";
                if (internalImages.Count == 1)
                {
                    List<String> lst = Project.StringListToList(Images, ';');
                    s += "\n\t" + Name + ".Image = " + GetParamOrDefaultValue("Res.Images." + lst[0],UserValue_Images) + ";";
                    return s;
                }
                else
                {
                    List<String> lst = Project.StringListToList(Images, ';');
                    s += "\n\t" + Name + ".ImageList = new GApp::Resources::Bitmap *[" + lst.Count.ToString() + "];";
                    s += "\n\t" + Name + ".ImageIndex = " + this.ImageIndex.ToString() + ";";
                    s += "\n\t" + Name + ".Count = " + lst.Count.ToString() + ";";
                    for (int tr = 0; tr < lst.Count; tr++)
                        s += "\n\t" + Name + ".ImageList["+tr.ToString()+"] = Res.Images." + lst[tr] + ";";
                    s += "\n\t" + Name + ".Image = " + GetParamOrDefaultValue(Name+".ImageList["+Name+".ImageIndex]", UserValue_Images) + ";";
                    return s;
                }
            }
            public override string GetCPPClassName()
            {
                if (internalImages.Count == 1)
                    return "SingleImageElement";
                return "MultipleImageElement";
            }
            public override void PopulateParameters(AnimationParameters p) 
            {
                base.PopulateParameters(p);
                if (UserValue_ColorBlending.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_ColorBlending, "unsigned int", "Color"));
                if (UserValue_Images.Length > 0)
                {
                    if (HasMultipleImages()==false)
                        p.ParametersList.Add(new ParameterInformation(UserValue_Images, "GApp::Resources::Bitmap *", "Bitmap"));
                }
            }
            public override void AddAnimationFunction(GACParser.Module m)
            {
                base.AddAnimationFunction(m);
                if (Setter_Image != YesNo.No)
                {
                    string nm = "Set" + Name + "Image";
                    GACParser.Member mb = new GACParser.Member(m, GACParser.MemberType.Function, nm, nm, "void", "", null);
                    mb.AddParameter(new GACParser.Member(m, GACParser.MemberType.Variable, "image", "image", "Bitmap", "", null));
                    m.Members[nm] = mb;
                }
                if (Getter_Image != YesNo.No)
                {
                    string nm = "Get" + Name + "Image";
                    m.Members[nm] = new GACParser.Member(m, GACParser.MemberType.Function, nm, nm, "Bitmap", "", null);
                }
                if (Setter_Color != YesNo.No)
                    CreateSetterForColor(m, "BlendColor");
            }
            public override string GetAnimationFunctionCPPHeaderDefinition()
            {
                string s = base.GetAnimationFunctionCPPHeaderDefinition();
                if (Setter_Image != YesNo.No)
                    s += "\n\tvoid Set" + Name + "Image (GApp::Resources::Bitmap* __image__);";
                if (Getter_Image != YesNo.No)
                    s += "\nGApp::Resources::Bitmap* Get" + Name + "Image ();";
                if (Setter_Color != YesNo.No)
                    s += CreateSetColorCPPHeaderDefinition("BlendColor");
                return s;
            }
            public override string GetAnimationFunctionCPPImplementation(string className)
            {
                string s = base.GetAnimationFunctionCPPImplementation(className);
                if (Setter_Image != YesNo.No)
                    s += "\nvoid " + className + "::Set" + Name + "Image(GApp::Resources::Bitmap* __image__) { ((GApp::Animations::Elements::SingleImageElement*)(&" + Name + "))->Image = __image__; };";
                if (Getter_Image != YesNo.No)
                    s += "\nGApp::Resources::Bitmap* " + className + "::Get" + Name + "Image() { return (((GApp::Animations::Elements::SingleImageElement*)(&" + Name + "))->Image); };";
                if (Setter_Color != YesNo.No)
                    s += CreateSetColorCPPImplementation(className, "BlendColor");
                return s;                
            }
            #endregion

            public bool HasMultipleImages()
            {
                return internalImages.Count > 1;
            }
        }
        public class TextElement: GenericElementWithPositionAndSize
        {
            [XmlAttribute()]
            public string Text = "";
            [XmlAttribute()]
            public string StringResource = "";
            [XmlAttribute()]
            public bool WordWrap = false, Justify = false, UseStringResource = false;
            [XmlAttribute()]
            public string Font = "";
            [XmlAttribute()]
            public Alignament TextAlignament = Alignament.Center;
            [XmlAttribute()]
            public float LineSpace = 0.0f, Size = 1f;
            [XmlAttribute()]
            public int ColorBlending = -1;
            [XmlAttribute()]
            public string UserValue_Font = "", UserValue_Size = "", UserValue_TextAlign = "", UserValue_Text = "", UserValue_Color = "";
            [XmlAttribute()]
            public YesNo Getter_Text = YesNo.No;
            [XmlAttribute()]
            public YesNo Setter_Text = YesNo.No;
            [XmlAttribute()]
            public YesNo Setter_Color = YesNo.No;

            [XmlIgnore()]
            public TextPainter tp = new TextPainter();

            #region Atribute
            [XmlIgnore(), Category("Text"), DisplayName("Text")]
            public string _Text
            {
                get { return Text; }
                set { Text = value; }
            }
            [XmlIgnore(), Category("Text"), DisplayName("Resource"), Editor(typeof(StringSelectorEditor), typeof(UITypeEditor))]
            public string _StringResource
            {
                get { return StringResource; }
                set { StringResource = value; }
            }
            [XmlIgnore(), Category("Text"), DisplayName("Use string resources")]
            public bool _UseStringResource
            {
                get { return UseStringResource; }
                set { UseStringResource = value; }
            }

            [XmlIgnore(), Category("Font"), DisplayName("Color")]
            public System.Drawing.Color _ColorBlending
            {
                get { return System.Drawing.Color.FromArgb(ColorBlending); }
                set { ColorBlending = value.ToArgb(); }
            }
            [XmlIgnore(), Category("Font"), DisplayName("Word Wrap")]
            public bool _WordWrap
            {
                get { return WordWrap; }
                set { WordWrap = value; }
            }
            [XmlIgnore(), Category("Font"), DisplayName("Justify")]
            public bool _Jusity
            {
                get { return Justify; }
                set { Justify = value; }
            }
            [XmlIgnore(), Category("Font"), DisplayName("Font"), Description("Use '' string for the default font"), Editor(typeof(FontSelectorEditor), typeof(UITypeEditor))]
            public string _Font
            {
                get { return Font; }
                set { Font = value; }
            }
            [XmlIgnore(), Category("Font"), DisplayName("Text Alignament")]
            public Alignament _TextAlignament
            {
                get { return TextAlignament; }
                set { TextAlignament = value; }
            }
            [XmlIgnore(), Category("Font"), DisplayName("Line spaces")]
            public string _LineSpace
            {
                get { return Project.ProcentToString(LineSpace); }
                set
                {
                    float v = 0;
                    if (Project.StringToProcent(value, ref v) == false)
                        MessageBox.Show("Invalid percentage value");
                    LineSpace = v;
                }
            }
            [XmlIgnore(), Category("Font"), DisplayName("Size")]
            public string _Size
            {
                get { return GetPercentageValue(Size); }
                set { Size = SetPercentageValue(value, Size); }
            }


            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Font")]
            public string _UserValue_Font
            {
                get { return UserValue_Font; }
                set { UserValue_Font = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Size")]
            public string _UserValue_Size
            {
                get { return UserValue_Size; }
                set { UserValue_Size = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Text Alignament")]
            public string _UserValue_TextAlign
            {
                get { return UserValue_TextAlign; }
                set { UserValue_TextAlign = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Text")]
            public string _UserValue_Text
            {
                get { return UserValue_Text; }
                set { UserValue_Text = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Color")]
            public string _UserValue_Color
            {
                get { return UserValue_Color; }
                set { UserValue_Color = value; }
            }

            #endregion

            #region Getters
            [XmlIgnore(), Description("Specifies if a getter for element text is to be created."), Category("Getters"), DisplayName("Text")]
            public YesNo _Getter_Text
            {
                get { return Getter_Text; }
                set { Getter_Text = value; }
            }
            [XmlIgnore(), Description("Sets the text for this specific element"), Category("Setters"), DisplayName("Text")]
            public YesNo _Setter_Text
            {
                get { return Setter_Text; }
                set { Setter_Text = value; }
            }
            [XmlIgnore(), Description("Sets the image for this specific element"), Category("Setters"), DisplayName("Color")]
            public YesNo _Setter_Color
            {
                get { return Setter_Color; }
                set { Setter_Color = value; }
            }
            #endregion

            #region Virtual Functions
            protected override void SetPositionAndSize(float x_percentage, float y_percantage, float unscaled_widthInPixels, float unscaled_heightInPixels, float scaledWidth, float scaleHeight)
            {
                this.X = x_percentage;
                this.Y = y_percantage;
                this.Width = unscaled_widthInPixels;
                this.Height = unscaled_heightInPixels;
            }
            protected override string GetDescription()
            {
                if (UseStringResource)
                    return "Resource: " + this.StringResource;
                else
                    return "Text:" + this.Text;
            }
            public override string GetIconKey()
            {
                return "__Font__";
            }
            public override string GetTypeName()
            {
                return "Text";
            }
            public override void InitRuntimeContext()
            {
                base.InitRuntimeContext();
                ExecutionContext.ColorBlending = RuntimeContext.BlendModeToColor(BlendingMode.ColorBlending, ColorBlending, 1.0f);
                if (UseStringResource)
                    tp.SetText(this.StringResource, true, CurrentAppResources);
                else
                    tp.SetText(Text, false, CurrentAppResources);
                tp.SetSpaceWidth(-1);
                tp.SetCharacterSpacing(-1);
                tp.SetLineSpace(LineSpace);
                tp.SetAlignament(TextAlignament);
                tp.SetFont(Font);
                tp.SetWordWrap(WordWrap);
                tp.SetJustify(Justify);
                tp.SetCharactesVisibility(0, 0xFFFFFF, true);
                tp.ForceRecompute();                
            }

            public override void OnPaint(Canvas c, float deviceWidth, float deviceHeight, BoardViewMode viewMode)
            {
                tp.SetPosition(ExecutionContext.ScreenRect.Left, ExecutionContext.ScreenRect.Top, ExecutionContext.ScreenRect.Right, ExecutionContext.ScreenRect.Bottom);
                float fontsz = Size;
                if (ExecutionContext.ScaleWidth < ExecutionContext.ScaleHeight)
                    fontsz *= ExecutionContext.ScaleWidth;
                else
                    fontsz *= ExecutionContext.ScaleHeight;
                tp.SetFontSize(TextPainter.FontSizeMethod.Scale, fontsz);
                tp.SetBlending(BlendingMode.ColorBlending, (int)ExecutionContext.ColorBlending, 1.0f);
                tp.Paint(c, CurrentAppResources);
            }
            public override string Validate(Project prj, AppResources resources)
            {
                if (resources.Fonts.ContainsKey(Font) == false)
                    return "Font '" + Font + "' does not exists !";
                if (this.UseStringResource)
                {
                    if (resources.Strings.ContainsKey(StringResource) == false)
                        return "String resource: '" + StringResource + "' does not exists !";
                }
                return null;
            }
            public override string CreateOnStartCPPCode(AnimationObject animObj)
            {
                string s = base.CreateOnStartCPPCode(animObj);
                s += "\n\t" + Name + ".CallVirtualFunction = true;";
                s += "\n\t" + Name + ".fontSize = " + GetParamOrDefaultValue(this.Size.ToString(), UserValue_Size) + ";";
                s += "\n\t" + Name + ".TP.SetFont(Res.Fonts." + GetParamOrDefaultValue(this.Font,UserValue_Font) + ");";
                s += "\n\t" + Name + ".TP.SetFontSize(GAC_FONTSIZETYPE_SCALE, " + GetParamOrDefaultValue(this.Size.ToString(), UserValue_Size) + ");";
                s += "\n\t" + Name + ".TP.SetLineSpace(" + this.LineSpace.ToString() + ");";
                s += "\n\t" + Name + ".TP.SetTextJustify(" + this.Justify.ToString().ToLower() + ");";
                s += "\n\t" + Name + ".TP.SetWordWrap(" + this.WordWrap.ToString().ToLower() + ");";
                s += "\n\t" + Name + ".TP.SetColorBlending(" + GetParamOrDefaultValue("0x"+this.ColorBlending.ToString("X8"), UserValue_Color) + ");";
                s += "\n\t" + Name + ".ColorTransform = "+GetParamOrDefaultValue("0x"+this.ColorBlending.ToString("X8"), UserValue_Color)+";";
                s += "\n\t" + Name + ".TP.SetViewRectWH(" + Name + ".X * this->Width," + Name + ".Y * this->Height," + Name + ".Alignament," + Name + ".WidthInPixels * this->Width," + Name + ".HeightInPixels * this->Height);";
                s += "\n\t" + Name + ".TP.SetDockPosition(" + GetParamOrDefaultValue("GAC_ALIGNAMENT_" + this.TextAlignament.ToString().ToUpper(), UserValue_TextAlign) + ");";  
                if (UserValue_Text.Length>0)
                {
                    s += "\n\t" + Name + ".TP.SetText(param_" + UserValue_Text + ");";
                }
                else
                {
                    if (this.UseStringResource)
                        s += "\n\t" + Name + ".TP.SetText(Res.Strings." + this.StringResource + ");";
                    else
                        s += "\n\t" + Name + ".TP.SetText(\"" + this.Text+ "\");";
                }
                return s;
            }
            public override string GetCPPClassName()
            {
                return "TextElement";
            }
            public override void PopulateParameters(AnimationParameters p)
            {
                base.PopulateParameters(p);
                if (UserValue_Font.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Font, "GApp::Resources::Font*", "Font"));
                if (UserValue_Size.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Size, "float"));
                if (UserValue_TextAlign.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_TextAlign, "unsigned int", "Alignament"));
                if (UserValue_Color.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Color, "unsigned int", "Color"));
                if (UserValue_Text.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Text, "GApp::Utils::String", "string"));
            }
            public override void AddAnimationFunction(GACParser.Module m)
            {
                base.AddAnimationFunction(m);
                if (Getter_Position != PositionGetterType.No)
                    CreateGetterForPosition(m);
                if (Setter_Text != YesNo.No)
                {
                    string nm = "Set" + Name + "Text";
                    GACParser.Member mb = new GACParser.Member(m, GACParser.MemberType.Function, nm, nm, "void", "", null);
                    mb.AddParameter(new GACParser.Member(m, GACParser.MemberType.Variable, "text","text","string","",null));
                    m.Members[nm] = mb;
                }
                if (Getter_Text != YesNo.No)
                {
                    string nm = "Get" + Name + "Text";
                    GACParser.Member mb = new GACParser.Member(m, GACParser.MemberType.Function, nm, nm, "bool", "", null);
                    mb.AddParameter(new GACParser.Member(m, GACParser.MemberType.Variable, "text", "text", "String", "", null));
                    m.Members[nm] = mb;
                }
                if (Setter_Color != YesNo.No)
                    CreateSetterForColor(m, "Color");
            }
            public override string GetAnimationFunctionCPPHeaderDefinition()
            {
                string s = "";
                if (Getter_Position != PositionGetterType.No)
                    s += CreateGetPositionCPPHeaderDefinition();
                if (Setter_Text != YesNo.No)
                    s += "\n\tvoid Set" + Name + "Text (const char* __text__);";
                if (Getter_Text != YesNo.No)
                    s += "\n\tbool Get" + Name + "Text (GApp::Utils::String *copy);";
                if (Setter_Color != YesNo.No)
                    s += CreateSetColorCPPHeaderDefinition("Color");
                return s;
            }
            public override string GetAnimationFunctionCPPImplementation(string className)
            {
                string s = "";
                if (Getter_Position != PositionGetterType.No)
                    s += CreateGetPositionCPPImplementation(className, Getter_Position);
                if (Setter_Text != YesNo.No)
                    s += "\nvoid " + className + "::Set" + Name + "Text(const char* __text__) { ((GApp::Animations::Elements::TextElement*)(&" + Name + "))->TP.SetText(__text__); };";
                if (Getter_Text != YesNo.No)
                    s += "\nbool " + className + "::Get" + Name + "Text(GApp::Utils::String *__copy__) { return ((GApp::Animations::Elements::TextElement*)(&" + Name + "))->TP.CopyText(__copy__); };";
                if (Setter_Color != YesNo.No)
                    s += CreateSetColorCPPImplementation(className, "Color");
                return s;
            }
            #endregion
        }
        public class EntireSurfaceElement: GenericElement
        {
            [XmlAttribute()]
            public int ColorBlending = 0x7F000000;
            [XmlAttribute()]
            public string UserValue_Color = "";
            [XmlAttribute()]
            public YesNo Setter_Color = YesNo.No;

            #region Atribute
            [XmlIgnore(), Category("Surface"), DisplayName("Color")]
            public System.Drawing.Color _ColorBlending
            {
                get { return System.Drawing.Color.FromArgb(ColorBlending); }
                set { ColorBlending = value.ToArgb(); }
            }

            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Color")]
            public string _UserValue_Color
            {
                get { return UserValue_Color; }
                set { UserValue_Color = value; }
            }
            #endregion

            #region Getters
            [XmlIgnore(), Description("Sets the image for this specific element"), Category("Setters"), DisplayName("Color")]
            public YesNo _Setter_Color
            {
                get { return Setter_Color; }
                set { Setter_Color = value; }
            }
            #endregion

            #region Virtual Functions
            private List<Bitmap> internalImages = new List<Bitmap>();
            protected override string GetDescription()
            {
                return "Surface color: " + Color.FromArgb(ColorBlending).ToString();
            }
            public override string GetIconKey()
            {
                return "__EntireSurface__";
            }
            public override string GetTypeName()
            {
                return "Entire Surface";
            }
            public override void InitRuntimeContext()
            {
                ExecutionContext.ColorBlending = RuntimeContext.BlendModeToColor(BlendingMode.ColorBlending, ColorBlending, 1.0f);
                ExecutionContext.Visible = Visible;
            }
            public override void OnPaint(Canvas c, float deviceWidth, float deviceHeight, BoardViewMode viewMode)
            {
                c.DrawRect(0, 0, c.GetWidth(),c.GetHeight(), Alignament.TopLeft, 0, (int)ExecutionContext.ColorBlending, 0);
            }
            public override string Validate(Project prj, AppResources resources)
            {
                return null;
            }
            public override string CreateOnStartCPPCode(AnimationObject animObj)
            {
                string s = "";
                s += "\n\t" + Name + ".ColorTransform = " + GetParamOrDefaultValue("0x"+ColorBlending.ToString("X8"), UserValue_Color) + ";";
                s += "\n\t" + Name + ".Visible = " + GetParamOrDefaultValue(this.Visible.ToString().ToLower(), UserValue_Visible) + ";";
                return s;
            }
            public override string GetCPPClassName()
            {
                return "EntireSurfaceElement";
            }
            public override void PopulateParameters(AnimationParameters p)
            {
                if (UserValue_Color.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Color, "unsigned int", "Color"));
            }

            public override void AddAnimationFunction(GACParser.Module m)
            {
                if (Setter_Color != YesNo.No)
                    CreateSetterForColor(m, "Color");
            }
            public override string GetAnimationFunctionCPPHeaderDefinition()
            {
                string s = "";
                if (Setter_Color != YesNo.No)
                    s += CreateSetColorCPPHeaderDefinition("Color");
                return s;
            }
            public override string GetAnimationFunctionCPPImplementation(string className)
            {
                string s = "";
                if (Setter_Color != YesNo.No)
                    s += CreateSetColorCPPImplementation(className, "Color");
                return s;
            }
            #endregion
        }
        public class RectangleElement : GenericElementWithPositionAndSize
        {

            [XmlAttribute()]
            public string UserValue_ColorBlending = "";
            [XmlAttribute()]
            public int ColorBlending = -1;
            [XmlAttribute()]
            public YesNo Setter_Color = YesNo.No;

            #region Atribute
            [XmlIgnore(), Category("Color"), DisplayName("Color")]
            public System.Drawing.Color _ColorBlending
            {
                get { return System.Drawing.Color.FromArgb(ColorBlending); }
                set { ColorBlending = value.ToArgb(); }
            }           
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Color")]
            public string _UserValue_ColorBlending
            {
                get { return UserValue_ColorBlending; }
                set { UserValue_ColorBlending = value; }
            }
            #endregion

            #region Getters
            [XmlIgnore(), Description("Sets the image for this specific element"), Category("Setters"), DisplayName("Color")]
            public YesNo _Setter_Color
            {
                get { return Setter_Color; }
                set { Setter_Color = value; }
            }
            #endregion

            #region Virtual Functions
            protected override void SetPositionAndSize(float x_percentage, float y_percantage, float unscaled_widthInPixels, float unscaled_heightInPixels, float scaledWidth, float scaleHeight)
            {
                this.X = x_percentage;
                this.Y = y_percantage;
                this.Width = unscaled_widthInPixels;
                this.Height = unscaled_heightInPixels;
            }
            public override string GetIconKey()
            {
                return "__Rectangle__";
            }
            public override string GetTypeName()
            {
                return "Rectangle";
            }
            public override void InitRuntimeContext()
            {
                base.InitRuntimeContext();                
                ExecutionContext.ColorBlending = RuntimeContext.BlendModeToColor(BlendingMode.ColorBlending, ColorBlending, 1.0f);
            }
            public override void OnPaint(Canvas c, float deviceWidth, float deviceHeight, BoardViewMode viewMode)
            {
                c.FillRect(this.ExecutionContext);
            }
            public override string Validate(Project prj, AppResources resources)
            {
                return null;
            }
            public override string CreateOnStartCPPCode(AnimationObject animObj)
            {
                string s = base.CreateOnStartCPPCode(animObj);
                s += "\n\t" + Name + ".ColorTransform = " + GetParamOrDefaultValue("0x" + this.ColorBlending.ToString("X8"), UserValue_ColorBlending) + ";"; 
                return s;

            }
            public override string GetCPPClassName()
            {
                return "RectangleElement";
            }
            public override void PopulateParameters(AnimationParameters p)
            {
                base.PopulateParameters(p);
                if (UserValue_ColorBlending.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_ColorBlending, "unsigned int", "Color"));
            }
            public override void AddAnimationFunction(GACParser.Module m)
            {
                base.AddAnimationFunction(m);
                if (Setter_Color != YesNo.No)
                    CreateSetterForColor(m, "Color");
            }
            public override string GetAnimationFunctionCPPHeaderDefinition()
            {
                string s = base.GetAnimationFunctionCPPHeaderDefinition();
                if (Setter_Color != YesNo.No)
                    s += CreateSetColorCPPHeaderDefinition("Color");
                return s;
            }
            public override string GetAnimationFunctionCPPImplementation(string className)
            {
                string s = base.GetAnimationFunctionCPPImplementation(className);
                if (Setter_Color != YesNo.No)
                    s += CreateSetColorCPPImplementation(className, "Color");
                return s;
            }
            #endregion

        }
        public class ExclusionRectangleElement : RectangleElement
        {
            #region Virtual Functions
            public override string GetIconKey()
            {
                return "__ExclusionRect__";
            }
            public override string GetTypeName()
            {
                return "Exclusion Rectangle";
            }
            public override void OnPaint(Canvas c, float deviceWidth, float deviceHeight, BoardViewMode viewMode)
            {
                c.FillExclusionRect(this.ExecutionContext);
            }          
            public override string GetCPPClassName()
            {
                return "ExclusionRectElement";
            }
            #endregion

        }
        public class ClipRectangleElement : GenericElementWithPositionAndSize
        {
            #region Virtual Functions
            public override string GetIconKey()
            {
                return "__ClipRect__";
            }
            public override string GetTypeName()
            {
                return "Clipping";
            }
            public override void OnPaint(Canvas c, float deviceWidth, float deviceHeight, BoardViewMode viewMode)
            {
                c.EnableClipping(this.ExecutionContext);
            }
            public override string GetCPPClassName()
            {
                return "ClippingElement";
            }
            #endregion

        }
        public class DisableClippingElement : GenericElement
        {
            #region Virtual Functions
            protected override string GetDescription()
            {
                return "Disable clipping";
            }
            public override string GetIconKey()
            {
                return "__DisableClipping__";
            }
            public override string GetTypeName()
            {
                return "Disable Clipping";
            }
            public override void InitRuntimeContext()
            {
                ExecutionContext.Visible = Visible;
            }
            public override void OnPaint(Canvas c, float deviceWidth, float deviceHeight, BoardViewMode viewMode)
            {
                c.ClearClipping();
            }
            public override string Validate(Project prj, AppResources resources)
            {
                return null;
            }
            public override string GetCPPClassName()
            {
                return "DisableClippingElement";
            }
            #endregion
        }
        public enum SimpleButtonBackgroundStyle
        {
            Image,
            Rectangle,
        }
        public class ButtonFaceContainer
        {
            public string BackgroundImage = "";
            public int BackgroundColorBlending = -1;

            public string SymbolImage = "";
            public Alignament SymbolAlign = Alignament.Center;
            public float SymbolScaleWidth = 1.0f;
            public float SymbolScaleHeight = 1.0f;
            public float SymbolX = 0.5f;
            public float SymbolY = 0.5f;
            public int SymbolColorBlending = -1;

            public string Text = "";
            public string TextStringResource = "";
            public bool TextWordWrap = false, TextJustify = false, TextUseStringResource = false;
            public string TextFont = "";
            public Alignament TextAlignament = Alignament.Center;
            public float TextLineSpace = 0.0f, TextSize = 1f;
            public int TextColorBlending = -1;
            public float TextX = 0.5f;
            public float TextY = 0.5f;
            public float TextRectangleWidth = 100;
            public float TextRectangleHeight = 100;
            public Alignament TextRectangleAlignament = Alignament.Center;

            public string UserValue_BackgroundImage = "", UserValue_BackgroundColorBlending = "";
            public string UserValue_SymbolImage = "", UserValue_SymbolColorBlending = "", UserValue_SymbolAlign = "";
            public string UserValue_SymbolScaleWidth = "", UserValue_SymbolScaleHeight= "", UserValue_SymbolX = "", UserValue_SymbolY = "";
            public string UserValue_TextColorBlending = "", UserValue_Text = "";

            public Bitmap Background = null;
            public Bitmap Symbol = null;
            public TextPainter tp = new TextPainter();

            #region Atribute background
            [XmlIgnore(), Category("Background"), DisplayName("Image"), Editor(typeof(ImageSelectorEditor), typeof(UITypeEditor))]
            public string _BackgroundImage
            {
                get { return BackgroundImage; }
                set { BackgroundImage = value; }
            }
            [XmlIgnore(), Category("Background"), DisplayName("Blend Color")]
            public System.Drawing.Color _BackgroundColorBlending
            {
                get { return System.Drawing.Color.FromArgb(BackgroundColorBlending); }
                set { BackgroundColorBlending = value.ToArgb(); }
            }
            #endregion

            #region Atribute Simbol
            [XmlIgnore(), Category("Symbol"), DisplayName("Image"), Editor(typeof(ImageSelectorEditor), typeof(UITypeEditor))]
            public string _SymbolImage
            {
                get { return SymbolImage; }
                set { SymbolImage = value; }
            }
            [XmlIgnore(), Description("Aliganment"), Category("Symbol"), DisplayName("Alignament")]
            public Alignament _SymbolAlign
            {
                get { return SymbolAlign; }
                set { SymbolAlign = value; }
            }
            [XmlIgnore(), Description("Scale width"), Category("Symbol"), DisplayName("Scale Width")]
            public string _SymbolScaleWidth
            {
                get { return Project.ProcentToString(SymbolScaleWidth); }
                set { SymbolScaleWidth = Project.StringToProcent(value, 0, 1000, SymbolScaleWidth); }
            }
            [XmlIgnore(), Description("Scale height"), Category("Symbol"), DisplayName("Scale Height")]
            public string _SymbolScaleHeight
            {
                get { return Project.ProcentToString(SymbolScaleHeight); }
                set { SymbolScaleHeight = Project.StringToProcent(value, 0, 1000, SymbolScaleHeight); }
            }
            [XmlIgnore(), Description("Object initial location"), Category("Symbol"), DisplayName("X")]
            public string _SymbolX
            {
                get { return Project.ProcentToString(this.SymbolX); }
                set { SymbolX = Project.StringToProcent(value, -1000, 1000, SymbolX); }
            }
            [XmlIgnore(), Description("Object initial location"), Category("Symbol"), DisplayName("Y")]
            public string _SymbolY
            {
                get { return Project.ProcentToString(this.SymbolY); }
                set { SymbolY = Project.StringToProcent(value, -1000, 1000, SymbolY); }
            }
            [XmlIgnore(), Category("Symbol"), DisplayName("Blend Color")]
            public System.Drawing.Color _SymbolColorBlending
            {
                get { return System.Drawing.Color.FromArgb(SymbolColorBlending); }
                set { SymbolColorBlending = value.ToArgb(); }
            }
            #endregion

            #region Atribute Text
            [XmlIgnore(), Category("Text"), DisplayName("Text")]
            public string _Text
            {
                get { return Text; }
                set { Text = value; }
            }
            [XmlIgnore(), Category("Text"), DisplayName("Resource"), Editor(typeof(StringSelectorEditor), typeof(UITypeEditor))]
            public string _StringResource
            {
                get { return TextStringResource; }
                set { TextStringResource = value; }
            }
            [XmlIgnore(), Category("Text"), DisplayName("Use string resources")]
            public bool _UseStringResource
            {
                get { return TextUseStringResource; }
                set { TextUseStringResource = value; }
            }

            [XmlIgnore(), Category("Text"), DisplayName("Color")]
            public System.Drawing.Color _ColorBlending
            {
                get { return System.Drawing.Color.FromArgb(TextColorBlending); }
                set { TextColorBlending = value.ToArgb(); }
            }
            [XmlIgnore(), Category("Text"), DisplayName("Word Wrap")]
            public bool _WordWrap
            {
                get { return TextWordWrap; }
                set { TextWordWrap = value; }
            }
            [XmlIgnore(), Category("Text"), DisplayName("Justify")]
            public bool _Jusity
            {
                get { return TextJustify; }
                set { TextJustify = value; }
            }
            [XmlIgnore(), Category("Text"), DisplayName("Font"), Description("Use '' string for the default font"), Editor(typeof(FontSelectorEditor), typeof(UITypeEditor))]
            public string _Font
            {
                get { return TextFont; }
                set { TextFont = value; }
            }
            [XmlIgnore(), Category("Text"), DisplayName("Text Alignament")]
            public Alignament _TextAlignament
            {
                get { return TextAlignament; }
                set { TextAlignament = value; }
            }
            [XmlIgnore(), Category("Text"), DisplayName("Line spaces")]
            public string _LineSpace
            {
                get { return Project.ProcentToString(TextLineSpace); }
                set
                {
                    float v = 0;
                    if (Project.StringToProcent(value, ref v) == false)
                        MessageBox.Show("Invalid percentage value");
                    TextLineSpace = v;
                }
            }
            [XmlIgnore(), Category("Text"), DisplayName("Size")]
            public string _Size
            {
                get { return Project.ProcentToString(TextSize); }
                set { TextSize = Project.StringToProcent(value, 0, 1000, TextSize); }
            }
            [XmlIgnore(), Description("Object initial location"), Category("Text Layout"), DisplayName("X")]
            public string _TextX
            {
                get { return Project.ProcentToString(this.TextX); }
                set { TextX = Project.StringToProcent(value, -1000, 1000, TextX); }
            }
            [XmlIgnore(), Description("Object initial location"), Category("Text Layout"), DisplayName("Y")]
            public string _TextY
            {
                get { return Project.ProcentToString(this.TextY); }
                set { TextY = Project.StringToProcent(value, -1000, 1000, TextY); }
            }
            [XmlIgnore(), Category("Text Layout"), DisplayName("Text Rectangle Alignament")]
            public Alignament _TextRectangleAlignament
            {
                get { return TextRectangleAlignament; }
                set { TextRectangleAlignament = value; }
            }
            [XmlIgnore(), Description("Text rectangle with"), Category("Text Layout"), DisplayName("Width")]
            public string _TextRectangleWidth
            {
                get { return AnimO.GenericElement.GetSizeInPixels(TextRectangleWidth); }
                set { TextRectangleWidth = AnimO.GenericElement.SetSizeInPixels(value, TextRectangleWidth); }
            }
            [XmlIgnore(), Description("Text rectangle  height"), Category("Text Layout"), DisplayName("Height")]
            public string _TextRectangleHeight
            {
                get { return AnimO.GenericElement.GetSizeInPixels(TextRectangleHeight); }
                set { TextRectangleHeight = AnimO.GenericElement.SetSizeInPixels(value, TextRectangleHeight); }
            }
            #endregion

            #region Initialization parameters (user dependent)
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent) - Background"), DisplayName("Image")]
            public string _UserValue_BackgroundImage
            {
                get { return UserValue_BackgroundImage; }
                set { UserValue_BackgroundImage = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent) - Background"), DisplayName("Blend Color")]
            public string _UserValue_BackgroundColorBlending
            {
                get { return UserValue_BackgroundColorBlending; }
                set { UserValue_BackgroundColorBlending = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent) - Symbol"), DisplayName("Image")]
            public string _UserValue_SymbolImage
            {
                get { return UserValue_SymbolImage; }
                set { UserValue_SymbolImage = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent) - Symbol"), DisplayName("Blend Color")]
            public string _UserValue_SymbolColorBlending
            {
                get { return UserValue_SymbolColorBlending; }
                set { UserValue_SymbolColorBlending = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent) - Symbol"), DisplayName("Alignament")]
            public string _UserValue_SymbolAlign
            {
                get { return UserValue_SymbolAlign; }
                set { UserValue_SymbolAlign = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent) - Symbol"), DisplayName("Scale Width")]
            public string _UserValue_SymbolScaleWidth
            {
                get { return UserValue_SymbolScaleWidth; }
                set { UserValue_SymbolScaleWidth = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent) - Symbol"), DisplayName("Scale Height")]
            public string _UserValue_SymbolScaleHeight
            {
                get { return UserValue_SymbolScaleHeight; }
                set { UserValue_SymbolScaleHeight = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent) - Symbol"), DisplayName("X")]
            public string _UserValue_SymbolX
            {
                get { return UserValue_SymbolX; }
                set { UserValue_SymbolX = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent) - Symbol"), DisplayName("Y")]
            public string _UserValue_SymbolY
            {
                get { return UserValue_SymbolY; }
                set { UserValue_SymbolY = value; }
            }

            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent) - Text"), DisplayName("Blend Color")]
            public string _UserValue_TextColorBlending
            {
                get { return UserValue_TextColorBlending; }
                set { UserValue_TextColorBlending = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent) - Text"), DisplayName("Text")]
            public string _UserValue_Text
            {
                get { return UserValue_Text; }
                set { UserValue_Text = value; }
            }
            #endregion

            private string GetValue(Dictionary<string,string> d,string key,string defaultValue)
            {
                string val;
                if (d.TryGetValue(key, out val))
                    return val;
                return defaultValue;
            }
            private int GetValue(Dictionary<string, string> d, string key, int defaultValue)
            {
                string val;
                int iVal;
                if (d.TryGetValue(key, out val))
                {
                    if (int.TryParse(val, out iVal))
                        return iVal;
                    return defaultValue;
                }
                return defaultValue;
            }
            private bool GetValue(Dictionary<string, string> d, string key, bool defaultValue)
            {
                string val;
                if (d.TryGetValue(key, out val))
                {
                    return (val.Equals("true", StringComparison.InvariantCultureIgnoreCase));
                }
                return defaultValue;
            }
            private Alignament GetValue(Dictionary<string, string> d, string key, Alignament defaultValue)
            {
                string val;
                Alignament aVal;
                if (d.TryGetValue(key, out val))
                {
                    if (Enum.TryParse<Alignament>(val, out aVal))
                        return aVal;
                    return defaultValue;
                }
                return defaultValue;
            }
            public bool CreateFromString(string format)
            {
                Dictionary<string, string> d = new Dictionary<string, string>();
                List<string> lst = Project.StringListToList(format, ';');
                if (lst == null)
                    return false;
                foreach (string item in lst)
                {
                    int idx = item.IndexOf(':');
                    if (idx>=0)
                    {
                        d[item.Substring(0, idx)] = item.Substring(idx + 1).Trim();
                    }
                }
                // fac asignarile
                BackgroundImage = GetValue(d, "BackgroundImage", BackgroundImage);
                BackgroundColorBlending = GetValue(d, "BackgroundColorBlending", BackgroundColorBlending);

                SymbolImage = GetValue(d, "SymbolImage", SymbolImage);
                SymbolAlign = GetValue(d, "SymbolAlign", SymbolAlign);
                _SymbolScaleWidth = GetValue(d, "SymbolScaleWidth", _SymbolScaleWidth);
                _SymbolScaleHeight = GetValue(d, "SymbolScaleHeight", _SymbolScaleHeight);
                _SymbolX = GetValue(d, "SymbolX", _SymbolX);
                _SymbolY = GetValue(d, "SymbolY", _SymbolY);
                SymbolColorBlending = GetValue(d, "SymbolColorBlending", SymbolColorBlending);

                Text = GetValue(d, "Text", Text);
                TextStringResource = GetValue(d, "TextStringResource", TextStringResource);
                TextWordWrap = GetValue(d, "TextWordWrap", TextWordWrap);
                TextJustify = GetValue(d, "TextJustify", TextJustify);
                TextUseStringResource = GetValue(d, "TextUseStringResource", TextUseStringResource);
                TextFont = GetValue(d, "TextFont", TextFont);
                TextAlignament = GetValue(d, "TextAlignament", TextAlignament);
                _LineSpace = GetValue(d, "TextLineSpace", _LineSpace);
                _Size = GetValue(d, "TextSize", _Size);
                TextColorBlending = GetValue(d, "TextColorBlending", TextColorBlending);
                _TextX = GetValue(d, "TextX", _TextX);
                _TextY = GetValue(d, "TextY", _TextY);
                _TextRectangleWidth = GetValue(d, "TextRectangleWidth", _TextRectangleWidth);
                _TextRectangleHeight = GetValue(d, "TextRectangleHeight", _TextRectangleHeight);
                TextRectangleAlignament = GetValue(d, "TextRectangleAlignament", TextRectangleAlignament);

                UserValue_BackgroundImage = GetValue(d, "UserValue_BackgroundImage", UserValue_BackgroundImage);
                UserValue_BackgroundColorBlending = GetValue(d, "UserValue_BackgroundColorBlending", UserValue_BackgroundColorBlending);

                UserValue_SymbolImage = GetValue(d, "UserValue_SymbolImage", UserValue_SymbolImage);
                UserValue_SymbolColorBlending = GetValue(d, "UserValue_SymbolColorBlending", UserValue_SymbolColorBlending);
                UserValue_SymbolAlign = GetValue(d, "UserValue_SymbolAlign", UserValue_SymbolAlign);
                UserValue_SymbolScaleWidth = GetValue(d, "UserValue_SymbolScaleWidth", UserValue_SymbolScaleWidth);
                UserValue_SymbolScaleHeight = GetValue(d, "UserValue_SymbolScaleHeight", UserValue_SymbolScaleHeight);
                UserValue_SymbolX = GetValue(d, "UserValue_SymbolX", UserValue_SymbolX);
                UserValue_SymbolY = GetValue(d, "UserValue_SymbolY", UserValue_SymbolY);

                UserValue_TextColorBlending = GetValue(d, "UserValue_TextColorBlending", UserValue_TextColorBlending);
                UserValue_Text = GetValue(d, "UserValue_Text", UserValue_Text);
                return true;
            }
            public string CreateString()
            {
                string s = "";
                s += "BackgroundImage:" + BackgroundImage + ";";
                s += "BackgroundColorBlending:" + BackgroundColorBlending.ToString() + ";";

                s += "SymbolImage:" + SymbolImage + ";";
                s += "SymbolAlign:" + SymbolAlign.ToString() + ";";
                s += "SymbolScaleWidth:" + _SymbolScaleWidth + ";";
                s += "SymbolScaleHeight:" + _SymbolScaleHeight + ";";
                s += "SymbolX:" + _SymbolX + ";";
                s += "SymbolY:" + _SymbolY + ";";
                s += "SymbolColorBlending:" + SymbolColorBlending.ToString() + ";";

                s += "Text:" + Text + ";";
                s += "TextStringResource:" + TextStringResource + ";";
                s += "TextWordWrap:" + TextWordWrap.ToString() + ";";
                s += "TextJustify:" + TextJustify.ToString() + ";";
                s += "TextUseStringResource:" + TextUseStringResource.ToString() + ";";
                s += "TextFont:" + TextFont + ";";
                s += "TextAlignament:" + TextAlignament.ToString() + ";";
                s += "TextLineSpace:" + _LineSpace + ";";
                s += "TextSize:" + _Size + ";";
                s += "TextColorBlending:" + TextColorBlending.ToString() + ";";
                s += "TextX:" + _TextX + ";";
                s += "TextY:" + _TextY+ ";";
                s += "TextRectangleWidth:" + _TextRectangleWidth + ";";
                s += "TextRectangleHeight:" + _TextRectangleHeight+ ";";
                s += "TextRectangleAlignament:" + TextRectangleAlignament.ToString() + ";";


                s += "UserValue_BackgroundImage:" + UserValue_BackgroundImage + ";";
                s += "UserValue_BackgroundColorBlending:" + UserValue_BackgroundColorBlending + ";";

                s += "UserValue_SymbolImage:" + UserValue_SymbolImage + ";";
                s += "UserValue_SymbolColorBlending:" + UserValue_SymbolColorBlending + ";";
                s += "UserValue_SymbolAlign:" + UserValue_SymbolAlign + ";";
                s += "UserValue_SymbolScaleWidth:" + UserValue_SymbolScaleWidth + ";";
                s += "UserValue_SymbolScaleHeight:" + UserValue_SymbolScaleHeight + ";";
                s += "UserValue_SymbolX:" + UserValue_SymbolX + ";";
                s += "UserValue_SymbolY:" + UserValue_SymbolY + ";";

                s += "UserValue_TextColorBlending:" + UserValue_TextColorBlending + ";";
                s += "UserValue_Text:" + UserValue_Text + ";";
                return s;
            }
            public string Validate(AppResources resources, SimpleButtonBackgroundStyle mode)
            {
                Background = null;
                Symbol = null;
                if (mode == SimpleButtonBackgroundStyle.Image)
                {
                    if (BackgroundImage.Length == 0)
                        return "Background image is mandatory for a button !";
                    if (resources.Images.ContainsKey(BackgroundImage) == false)
                        return "Unknwon bakcground image: " + BackgroundImage;
                    Background = resources.Images[BackgroundImage].Picture;
                } else
                {
                    if (BackgroundImage.Length > 0)
                    {
                        if (resources.Images.ContainsKey(BackgroundImage) == false)
                            return "Unknwon bakcground image: " + BackgroundImage;
                        Background = resources.Images[BackgroundImage].Picture;
                    }
                }
                if (SymbolImage.Length>0)
                {
                    if (resources.Images.ContainsKey(SymbolImage) == false)
                        return "Unknwon symbol image: " + SymbolImage;
                    Symbol = resources.Images[SymbolImage].Picture;
                }
                if (TextFont.Length>0)
                {
                    if (resources.Fonts.ContainsKey(TextFont) == false)
                        return "Unknwon font: " + TextFont;
                    if ((TextUseStringResource) && (resources.Strings.ContainsKey(TextStringResource)==false))
                        return "Unknwon string: " + TextStringResource;
                }               
                return null;
            }
            public void InitRuntimeContext(RuntimeContext ExecutionContext, RuntimeContext symbolExecutionContext, RuntimeContext textExecutionContext,  SimpleButtonBackgroundStyle BackgroundStyle)
            {
                if (BackgroundStyle == SimpleButtonBackgroundStyle.Image)
                {
                    if (Background != null) {
                        ExecutionContext.WidthInPixels = Background.Width;
                        ExecutionContext.HeightInPixels = Background.Height;
                    } else
                    {
                        ExecutionContext.WidthInPixels = 0;
                        ExecutionContext.HeightInPixels = 0;
                    }
                } else
                {
                    // el vine deja setat cu dimensiunea din base.Init(...) - dar e dimensiunea pentru dreptunghi
                }
                ExecutionContext.ColorBlending = RuntimeContext.BlendModeToColor(BlendingMode.ColorBlending, this.BackgroundColorBlending, 1.0f);
                ExecutionContext.Image = this.Background;

                symbolExecutionContext.Image = this.Symbol;
                symbolExecutionContext.X_Percentage = this.SymbolX;
                symbolExecutionContext.Y_Percentage = this.SymbolY;
                symbolExecutionContext.Align = this.SymbolAlign;
                symbolExecutionContext.ScaleWidth = this.SymbolScaleWidth;
                symbolExecutionContext.ScaleHeight = this.SymbolScaleHeight;
                symbolExecutionContext.Visible = true;
                symbolExecutionContext.ColorBlending = RuntimeContext.BlendModeToColor(BlendingMode.ColorBlending, this.SymbolColorBlending, 1.0f);

                // text
                textExecutionContext.X_Percentage = this.TextX;
                textExecutionContext.Y_Percentage = this.TextY;
                textExecutionContext.Align = this.TextRectangleAlignament;
                textExecutionContext.ScaleWidth = this.TextSize;
                textExecutionContext.ScaleHeight = this.TextSize;
                textExecutionContext.Visible = true;
                textExecutionContext.WidthInPixels = this.TextRectangleWidth;
                textExecutionContext.HeightInPixels = this.TextRectangleHeight;
                textExecutionContext.ColorBlending = RuntimeContext.BlendModeToColor(BlendingMode.ColorBlending, this.TextColorBlending, 1.0f);
                if (TextUseStringResource)
                    tp.SetText(this.TextStringResource, true, GenericElement.CurrentAppResources);
                else
                    tp.SetText(Text, false, GenericElement.CurrentAppResources);
                tp.SetSpaceWidth(-1);
                tp.SetCharacterSpacing(-1);
                tp.SetLineSpace(TextLineSpace);
                tp.SetAlignament(TextAlignament);
                tp.SetFont(TextFont);
                tp.SetWordWrap(TextWordWrap);
                tp.SetJustify(TextJustify);
                tp.SetCharactesVisibility(0, 0xFFFFFF, true);
                tp.ForceRecompute();

            }
            public static void Paint(Canvas c, RectangleF tempRect, RuntimeContext ExecutionContext, RuntimeContext symbolExecutionContext, RuntimeContext textExecutionContext, TextPainter tp, SimpleButtonBackgroundStyle style, int compRectColor = 0)
            {
                if (style == SimpleButtonBackgroundStyle.Image) {
                    c.DrawImage(ExecutionContext);
                    c.ComputeRectInPercentages(ExecutionContext, true, ref tempRect);
                    if (compRectColor != 0)
                        c.DrawObjectRect(ExecutionContext, true, compRectColor);
                }
                else {
                    c.FillRect(ExecutionContext);
                    c.ComputeRectInPercentages(ExecutionContext, false, ref tempRect);
                    if (compRectColor != 0)
                        c.DrawObjectRect(ExecutionContext, false, compRectColor);
                }


                // simbol
                GenericElement.ComputeScreenRect(symbolExecutionContext, true, ExecutionContext.ScreenRect);
                c.DrawImage(symbolExecutionContext);
                if (compRectColor != 0)
                    c.DrawObjectRect(symbolExecutionContext, true, compRectColor);

                GenericElement.ComputeScreenRect(textExecutionContext, true, ExecutionContext.ScreenRect);
                tp.SetPosition(textExecutionContext.ScreenRect.Left, textExecutionContext.ScreenRect.Top, textExecutionContext.ScreenRect.Right, textExecutionContext.ScreenRect.Bottom);
                tp.SetFontSize(TextPainter.FontSizeMethod.Scale, textExecutionContext.ScaleWidth);
                tp.SetBlending(BlendingMode.ColorBlending, (int)textExecutionContext.ColorBlending, 1.0f);
                tp.Paint(c, GenericElement.CurrentAppResources);
            }
            private string GetParamOrDefaultValue(string defaultValue, string paramName)
            {
                if (paramName.Length > 0)
                    return "param_" + paramName;
                return defaultValue;
            }
            public string CreateOnStartCPPCode(AnimationObject animObj,string Name)
            {
                string s = "";
                s += "\n\t//------ Code for face: " + Name;
                if (BackgroundImage.Length == 0)
                    s += "\n\t" + Name + ".Background = " + GetParamOrDefaultValue("NULL", UserValue_BackgroundImage) + ";";
                else
                    s += "\n\t" + Name + ".Background = " + GetParamOrDefaultValue("Res.Images." + BackgroundImage, UserValue_BackgroundImage) + ";";
                s += "\n\t" + Name + ".BackgroundColorBlending = " + GetParamOrDefaultValue("0x" + this.BackgroundColorBlending.ToString("X8"), UserValue_BackgroundColorBlending) + ";";

                if (SymbolImage.Length==0)
                    s += "\n\t" + Name + ".Symbol = " + GetParamOrDefaultValue("NULL", UserValue_SymbolImage) + ";";
                else
                    s += "\n\t" + Name + ".Symbol = " + GetParamOrDefaultValue("Res.Images." + SymbolImage, UserValue_SymbolImage) + ";";
                s += "\n\t" + Name + ".SymbolColorBlending = " + GetParamOrDefaultValue("0x" + this.SymbolColorBlending.ToString("X8"), UserValue_SymbolColorBlending) + ";";
                s += "\n\t" + Name + ".SymbolScaleWidth = " + GetParamOrDefaultValue(SymbolScaleWidth.ToString(), UserValue_SymbolScaleWidth) + ";";
                s += "\n\t" + Name + ".SymbolScaleHeight = " + GetParamOrDefaultValue(SymbolScaleHeight.ToString(), UserValue_SymbolScaleHeight) + ";";
                s += "\n\t" + Name + ".SymbolX = " + GetParamOrDefaultValue(SymbolX.ToString(), UserValue_SymbolX) + ";";
                s += "\n\t" + Name + ".SymbolY = " + GetParamOrDefaultValue(SymbolY.ToString(), UserValue_SymbolY) + ";";
                s += "\n\t" + Name + ".SymbolAlign = " + GetParamOrDefaultValue("GAC_ALIGNAMENT_" + this.SymbolAlign.ToString().ToUpper(), UserValue_SymbolAlign) + ";";
                // Text
                if (TextFont.Length==0)
                    s += "\n\t" + Name + ".TP.SetFont(NULL);";
                else
                    s += "\n\t" + Name + ".TP.SetFont(Res.Fonts."+this.TextFont + ");";
                s += "\n\t" + Name + ".TP.SetFontSize(GAC_FONTSIZETYPE_SCALE, " + this.TextSize.ToString() + ");";
                s += "\n\t" + Name + ".TP.SetLineSpace(" + this.TextLineSpace.ToString() + ");";
                s += "\n\t" + Name + ".TP.SetTextJustify(" + this.TextJustify.ToString().ToLower() + ");";
                s += "\n\t" + Name + ".TP.SetWordWrap(" + this.TextWordWrap.ToString().ToLower() + ");";
                s += "\n\t" + Name + ".TP.SetColorBlending(" + GetParamOrDefaultValue("0x" + this.TextColorBlending.ToString("X8"), UserValue_TextColorBlending) + ");";
                s += "\n\t" + Name + ".TextColorBlending = " + GetParamOrDefaultValue("0x" + this.TextColorBlending.ToString("X8"), UserValue_TextColorBlending) + ";";
                s += "\n\t" + Name + ".TP.SetDockPosition(GAC_ALIGNAMENT_" + this.TextAlignament.ToString().ToUpper() + ");";
                //s += "\n\t" + Name + ".TP.SetViewRectWH(" + Name + ".X * this->Width," + Name + ".Y * this->Height," + Name + ".Alignament," + Name + ".WidthInPixels * this->Width," + Name + ".HeightInPixels * this->Height);";
                s += "\n\t" + Name + ".TextRectX = " + TextX.ToString() + ";";
                s += "\n\t" + Name + ".TextRectY = " + TextY.ToString() + ";";
                s += "\n\t" + Name + ".TextRectAlign = GAC_ALIGNAMENT_" + this.TextRectangleAlignament.ToString().ToUpper() + ";";
                s += "\n\t" + Name + ".TextRectWidth = " + TextRectangleWidth.ToString() + " * (Core.ResolutionAspectRatio);";
                s += "\n\t" + Name + ".TextRectHeight = " + TextRectangleHeight.ToString() + " * (Core.ResolutionAspectRatio);";

                if (UserValue_Text.Length > 0)
                {
                    s += "\n\t" + Name + ".TP.SetText(param_" + UserValue_Text + ");";
                }
                else
                {
                    if (this.TextUseStringResource)
                        s += "\n\t" + Name + ".TP.SetText(Res.Strings." + this.TextStringResource + ");";
                    else
                        s += "\n\t" + Name + ".TP.SetText(\"" + this.Text + "\");";
                }
                return s;
            }
            public void PopulateParameters(AnimationParameters p)
            {
                if (UserValue_SymbolX.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_SymbolX, "float"));
                if (UserValue_SymbolY.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_SymbolY, "float"));
                if (UserValue_SymbolScaleWidth.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_SymbolScaleWidth, "float"));
                if (UserValue_SymbolScaleHeight.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_SymbolScaleHeight, "float"));
                if (UserValue_SymbolAlign.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_SymbolAlign, "unsigned int", "Alignament"));
                if (UserValue_BackgroundColorBlending.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_BackgroundColorBlending, "unsigned int", "Color"));
                if (UserValue_SymbolColorBlending.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_SymbolColorBlending, "unsigned int", "Color"));
                if (UserValue_TextColorBlending.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_TextColorBlending, "unsigned int", "Color"));
                if (UserValue_BackgroundImage.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_BackgroundImage, "GApp::Resources::Bitmap *", "Bitmap"));
                if (UserValue_SymbolImage.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_SymbolImage, "GApp::Resources::Bitmap *", "Bitmap"));
                if (UserValue_Text.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Text, "GApp::Utils::String", "string"));
            }
        }
        public class ButtonAction
        {
            public enum ActionType
            {
                None,
                SendEvent,
                OpenRatePage,
                OpenDeveloperPage,
                OpenFacebookPage,
                OpenBrowser,
                GoToScene,
                CloseApplication,
                ChangeSoundSettingsOnOff,
                ChangeSoundSettingsOnSfxOff,
                ShowRewardableAd,
            };
            public string ActionName = null;
            public bool PerformActionAfterAnimationEnds = false;
            public ActionType ActionID = ActionType.None;
            public static ButtonAction CreateFromString(string strFormat)
            {
                return null;
            }
            public static string[] GetActionsList()
            {
                return Enum.GetNames(typeof(ActionType));
            }
        }
        public class SimpleButtonElement : GenericElementWithPositionAndSize
        {
            [XmlAttribute()]
            public string Normal = "";
            [XmlAttribute()]
            public string Pressed = "";
            [XmlAttribute()]
            public string Inactive = "";
            [XmlAttribute()]
            public bool Enabled = true;
            [XmlAttribute()]
            public bool SendEventWhenAnimationEnds = false;
            [XmlAttribute()]
            public string ClickEvent = "";
            [XmlAttribute()]
            public SimpleButtonBackgroundStyle BackgroundStyle = SimpleButtonBackgroundStyle.Image;
            [XmlAttribute()]
            public string SoundName = "";


            [XmlIgnore()]
            public int EventIDValue = -1;



            [XmlAttribute()]
            public string UserValue_Enabled = "", UserValue_Event="";
            [XmlAttribute()]
            public string UserValue_Sound = "";

            [XmlAttribute()]
            public SimpleButtonSetter Setter_Background_Image = SimpleButtonSetter.No;
            [XmlAttribute()]
            public SimpleButtonSetter Setter_Symbol_Image = SimpleButtonSetter.No;
            [XmlAttribute()]
            public SimpleButtonSetter Setter_Background_Color = SimpleButtonSetter.No;
            [XmlAttribute()]
            public SimpleButtonSetter Setter_Symbol_Color = SimpleButtonSetter.No;
            [XmlAttribute()]
            public SimpleButtonSetter Setter_Text_Color = SimpleButtonSetter.No;
            [XmlAttribute()]
            public SimpleButtonSetter Setter_Text = SimpleButtonSetter.No;
            [XmlAttribute()]
            public YesNo Getter_Enabled = YesNo.No;
            [XmlAttribute()]
            public YesNo Setter_Enabled = YesNo.No;

            #region Atribute
            [XmlIgnore(), Category("Button"), DisplayName("Normal state"), Description("Aspect for normal button state"), Editor(typeof(ButtonStateSelectorEditor), typeof(UITypeEditor))]
            public string _Normal
            {
                get { return Normal; }
                set { Normal = value; }
            }
            [XmlIgnore(), Category("Button"), DisplayName("Pressed state"), Description("Aspect for pressed button state"), Editor(typeof(ButtonStateSelectorEditor), typeof(UITypeEditor))]
            public string _Pressed
            {
                get { return Pressed; }
                set { Pressed = value; }
            }
            [XmlIgnore(), Category("Button"), DisplayName("Inactive state"), Description("Aspect for inactive button state"), Editor(typeof(ButtonStateSelectorEditor), typeof(UITypeEditor))]
            public string _Inactive
            {
                get { return Inactive; }
                set { Inactive = value; }
            }
            [XmlIgnore(), Category("Button"), DisplayName("Enabled")]
            public bool _Enabled
            {
                get { return Enabled; }
                set { Enabled = value; }
            }
            [XmlIgnore(), Category("Button"), DisplayName("Send click event when animation ends")]
            public bool _SendEventWhenAnimationEnds
            {
                get { return SendEventWhenAnimationEnds; }
                set { SendEventWhenAnimationEnds = value; }
            }
            [XmlIgnore(), Description("Event that will be triggered"), Category("Button"), DisplayName("Click Event"), Editor(typeof(EventIDSelectorEditor), typeof(UITypeEditor))]
            public string _ClickEvent
            {
                get { return ClickEvent; }
                set
                {
                    if (Project.ValidateVariableNameCorectness(value, false) == false)
                    {
                        MessageBox.Show("Invalid name for event - you can only use letters and numbers and character '_'. The first character must be a letter !");
                        return;
                    }
                    ClickEvent = value;
                }
            }
            [XmlIgnore(), Description("Button Aliganment"), Category("Button"), DisplayName("Background Style")]
            public SimpleButtonBackgroundStyle _Mode
            {
                get { return BackgroundStyle; }
                set { BackgroundStyle = value; }
            }
            [XmlIgnore(), Description("Sound that will be played when the button is clicked"), Category("Button"), DisplayName("Sound"), Editor(typeof(SoundSelectorEditor), typeof(UITypeEditor))]
            public string _SoundName
            {
                get { return SoundName; }
                set { SoundName = value; }
            }
            #endregion

            #region Initialization parameters (user dependent)
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Enabled")]
            public string _UserValue_Enabled
            {
                get { return UserValue_Enabled; }
                set { UserValue_Enabled = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Click Event")]
            public string _UserValue_Event
            {
                get { return UserValue_Event; }
                set { UserValue_Event = value; }
            }
            [XmlIgnore(), Description("Parameter name"), Category("Initialization parameters (user dependent)"), DisplayName("Sound")]
            public string _UserValue_Sound
            {
                get { return UserValue_Sound; }
                set { UserValue_Sound = value; }
            }
            #endregion

            #region Getters and Setters
            [XmlIgnore(), Description("Specifies if a getter for element position is to be created."), Category("Getters"), DisplayName("Enabled")]
            public YesNo _Getter_Enabled
            {
                get { return Getter_Enabled; }
                set { Getter_Enabled = value; }
            }

            [XmlIgnore(), Description("Sets the background image for this specific element"), Category("Setters"), DisplayName("Background Image")]
            public SimpleButtonSetter _Setter_Background_Image
            {
                get { return Setter_Background_Image; }
                set { Setter_Background_Image = value; }
            }
            [XmlIgnore(), Description("Sets the symbol image for this specific element"), Category("Setters"), DisplayName("Symbol Image")]
            public SimpleButtonSetter _Setter_Symbol_Image
            {
                get { return Setter_Symbol_Image; }
                set { Setter_Symbol_Image = value; }
            }
            [XmlIgnore(), Description("Sets the background image for this specific element"), Category("Setters"), DisplayName("Background Color")]
            public SimpleButtonSetter _Setter_Background_Color
            {
                get { return Setter_Background_Color; }
                set { Setter_Background_Color = value; }
            }
            [XmlIgnore(), Description("Sets the symbol color for this specific element"), Category("Setters"), DisplayName("Symbol Color")]
            public SimpleButtonSetter _Setter_Symbol_Color
            {
                get { return Setter_Symbol_Color; }
                set { Setter_Symbol_Color = value; }
            }
            [XmlIgnore(), Description("Sets the symbol image for this specific element"), Category("Setters"), DisplayName("Enabled")]
            public YesNo _Setter_Enabled
            {
                get { return Setter_Enabled; }
                set { Setter_Enabled = value; }
            }
            [XmlIgnore(), Description("Sets the text color for this specific element"), Category("Setters"), DisplayName("Text Color")]
            public SimpleButtonSetter _Setter_Text_Color
            {
                get { return Setter_Text_Color; }
                set { Setter_Text_Color = value; }
            }
            [XmlIgnore(), Description("Sets the text color for this specific element"), Category("Setters"), DisplayName("Text")]
            public SimpleButtonSetter _Setter_Text
            {
                get { return Setter_Text; }
                set { Setter_Text = value; }
            }
            #endregion

            #region Virtual Functions
            private ButtonFaceContainer FaceNormal = new ButtonFaceContainer();
            private ButtonFaceContainer FacePressed = new ButtonFaceContainer();
            private ButtonFaceContainer FaceInactive = new ButtonFaceContainer();
            private RuntimeContext symbolExecutionContext = new RuntimeContext();
            private RuntimeContext textExecutionContext = new RuntimeContext();

            private RectangleF tempRect = new RectangleF();

            protected override float GetWidthInPixels()
            {
                if (BackgroundStyle == SimpleButtonBackgroundStyle.Image)
                {
                    if (FaceNormal.Background != null)
                        return FaceNormal.Background.Width;
                    else
                        return 0;
                }
                else
                    return this.Width;
            }
            protected override float GetHeightInPixels()
            {
                if (BackgroundStyle == SimpleButtonBackgroundStyle.Image)
                {
                    if (FaceNormal.Background != null)
                        return FaceNormal.Background.Height;
                    else
                        return 0;
                }
                else
                    return this.Height;
            }

            protected override void SetPositionAndSize(float x_percentage, float y_percantage, float unscaled_widthInPixels, float unscaled_heightInPixels, float scaledWidth, float scaleHeight)
            {
                this.X = x_percentage;
                this.Y = y_percantage;
                if (BackgroundStyle == SimpleButtonBackgroundStyle.Image)
                {
                    this.ScaleWidth = scaledWidth;
                    this.ScaleHeight = scaleHeight;
                } else
                {
                    this.Width = unscaled_widthInPixels;
                    this.Height = unscaled_heightInPixels;
                }
            }

            protected override string GetDescription()
            {
                return "Event:"+ClickEvent+" , Enable:" + Enabled.ToString() + " , Visible:" + Visible.ToString();
            }
            public override string GetIconKey()
            {
                return "__SimpleButton__";
            }
            public override string GetTypeName()
            {
                return "SimpleButton";
            }
            public ButtonFaceContainer GetCurrentFace()
            {
                ButtonFaceContainer face = FaceNormal;
                if (Enabled == false)
                    face = FaceInactive;
                return face;
            }
            public override void InitRuntimeContext()
            {
                base.InitRuntimeContext();
                GetCurrentFace().InitRuntimeContext(ExecutionContext,symbolExecutionContext,textExecutionContext, BackgroundStyle);
            }
            public override void OnPaint(Canvas c, float deviceWidth, float deviceHeight, BoardViewMode viewMode)
            {
                ButtonFaceContainer.Paint(c, tempRect, ExecutionContext, symbolExecutionContext,textExecutionContext,GetCurrentFace().tp, BackgroundStyle);
            }
            public override string Validate(Project prj, AppResources resources)
            {
                string result = "";
                string res;

                EventIDValue = -1;
                if (ClickEvent.Length == 0)
                    result += "SimpleButton '" + Name + "' : You have to add a click event ID !\n";
                foreach (var e in prj.ObjectEventsIDs)
                    if (e.Name.Equals(ClickEvent))
                    {
                        EventIDValue = e.ID;
                        break;
                    }
                if (EventIDValue == -1)
                    result += "Unknwown event: '" + ClickEvent + "' in simple button : '" + Name + "' !\n";

                if (FaceNormal.CreateFromString(Normal) == false)
                {
                    result += "Fail to create normal face for button\n";
                }
                else
                {
                    res = FaceNormal.Validate(resources,BackgroundStyle);
                    if (res != null)
                        result += "Unable to validate normal face: " + res + "\n";
                }

                if (FacePressed.CreateFromString(Pressed) == false)
                {
                    result += "Fail to create normal face for button\n";
                }
                else
                {
                    res = FacePressed.Validate(resources,BackgroundStyle);
                    if (res != null)
                        result += "Unable to validate pressed face: " + res + "\n";
                }

                if (FaceInactive.CreateFromString(Inactive) == false)
                {
                    result += "Fail to create normal face for button\n";
                }
                else
                {
                    res = FaceInactive.Validate(resources,BackgroundStyle);
                    if (res != null)
                        result += "Unable to validate inactive face: " + res + "\n";
                }
                if (result.Length==0)
                    return null;

                return result;
            }
            public override string CreateOnStartCPPCode(AnimationObject animObj)
            {
                string s = base.CreateOnStartCPPCode(animObj);
                s += "\n\t" + Name + ".Enabled = " + GetParamOrDefaultValue(this.Enabled.ToString().ToLower(), UserValue_Enabled) + ";";
                s += "\n\t" + Name + ".SendEventWhenAnimationEnds = " + this.SendEventWhenAnimationEnds.ToString().ToLower() + ";";
                s += "\n\t" + Name + ".ClickEvent = " + GetParamOrDefaultValue(this.EventIDValue.ToString(), UserValue_Event) + ";";
                s += "\n\t" + Name + ".UseBackgoundImage = " + (this.BackgroundStyle == SimpleButtonBackgroundStyle.Image).ToString().ToLower() + ";";
                s += "\n\t" + Name + ".IsPressed = false;";
                s += "\n\t" + Name + ".CanProcessTouchEvents = true;";
                if (SoundName.Length>0)
                    s += "\n\t" + Name + ".ClickSound = " + GetParamOrDefaultValue("Res.Sounds."+this.SoundName, UserValue_Sound) + ";";
                else
                    s += "\n\t" + Name + ".ClickSound = " + GetParamOrDefaultValue("NULL", UserValue_Sound) + ";";

                s += FaceNormal.CreateOnStartCPPCode(animObj, Name+".Normal");
                s += FacePressed.CreateOnStartCPPCode(animObj, Name+".Pressed");
                s += FaceInactive.CreateOnStartCPPCode(animObj, Name+".Inactive");
                return s;
            }
            public override string GetCPPClassName()
            {
                return "SimpleButtonElement";
            }
            public override void PopulateParameters(AnimationParameters p)
            {
                base.PopulateParameters(p);
                if (UserValue_Enabled.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Enabled, "bool"));
                if (UserValue_Event.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Event, "int"));
                if (UserValue_Sound.Length > 0)
                    p.ParametersList.Add(new ParameterInformation(UserValue_Sound, "GApp::Resources::Sound*", "Sound"));
                FaceNormal.PopulateParameters(p);
                FacePressed.PopulateParameters(p);
                FaceInactive.PopulateParameters(p);
            }
            private void CreateGetterForButtonSetter(GACParser.Module m,string name, SimpleButtonSetter style,string paramName,string paramType)
            {                
                GACParser.Member mb = new GACParser.Member(m, GACParser.MemberType.Function, name, name, "void", "", null);
                if (style == SimpleButtonSetter.OneForAll)
                    mb.AddParameter(new GACParser.Member(m, GACParser.MemberType.Variable, paramName, paramName, paramType, "", null));
                else
                {
                    mb.AddParameter(new GACParser.Member(m, GACParser.MemberType.Variable, paramName + "Normal", paramName + "Normal", paramType, "", null));
                    mb.AddParameter(new GACParser.Member(m, GACParser.MemberType.Variable, paramName + "Pressed", paramName + "Pressed", paramType, "", null));
                    mb.AddParameter(new GACParser.Member(m, GACParser.MemberType.Variable, paramName + "Inactive", paramName + "Inactive", paramType, "", null));
                }
                m.Members[name] = mb;
            }
            private string CreateCPPHeaderSetterForButtonSetter(string name,SimpleButtonSetter style, string paramName, string cppParamType)
            {
                if (style == SimpleButtonSetter.OneForAll)
                    return "\n\tvoid "+name+" ("+cppParamType+" "+ paramName+"); ";
                else
                    return "\n\tvoid " + name + " (" + cppParamType + " " + paramName + "Normal, "+ cppParamType + " " + paramName + "Pressed, "+cppParamType + " " + paramName + "Inactive); ";
            }
            private string CreateCPPCodeSetterForButtonSetter(string className, string name, string objName, string objParam, SimpleButtonSetter style, string paramName, string cppParamType)
            {
                string s = "";
                if (style == SimpleButtonSetter.OneForAll)
                {
                    s = "\n\tvoid " + className + "::" + name + " (" + cppParamType + " " + paramName + ") {";
                    s += "\n\t\tGApp::Animations::Elements::SimpleButtonElement * el = ((GApp::Animations::Elements::SimpleButtonElement*)(&" + objName + "));";
                    s += "\n\t\tel->Normal." + objParam + " = " + paramName + ";";
                    s += "\n\t\tel->Pressed." + objParam + " = " + paramName + ";";
                    s += "\n\t\tel->Inactive." + objParam + " = " + paramName + ";";
                }
                else
                {
                    s = "\n\tvoid " + className + "::" + name + " (" + cppParamType + " " + paramName + "Normal, " + cppParamType + " " + paramName + "Pressed, " + cppParamType + " " + paramName + "Inactive) {";
                    s += "\n\t\tGApp::Animations::Elements::SimpleButtonElement * el = ((GApp::Animations::Elements::SimpleButtonElement*)(&" + objName + "));";
                    s += "\n\t\tel->Normal." + objParam + " = " + paramName + "Normal;";
                    s += "\n\t\tel->Pressed." + objParam + " = " + paramName + "Pressed;";
                    s += "\n\t\tel->Inactive." + objParam + " = " + paramName + "Inactive;";
                }
                return s+"\n\t}\n";
            }
            private string CreateCPPCodeSetterForButtonTextSetter(string className, string name, string objName, SimpleButtonSetter style, string paramName, string cppParamType)
            {
                string s = "";
                if (style == SimpleButtonSetter.OneForAll)
                {
                    s = "\n\tvoid " + className + "::" + name + " (" + cppParamType + " " + paramName + ") {";
                    s += "\n\t\tGApp::Animations::Elements::SimpleButtonElement * el = ((GApp::Animations::Elements::SimpleButtonElement*)(&" + objName + "));";
                    s += "\n\t\tel->Normal.TP.SetText(" + paramName + ");";
                    s += "\n\t\tel->Pressed.TP.SetText(" + paramName + ");";
                    s += "\n\t\tel->Inactive.TP.SetText(" + paramName + ");";
                }
                else
                {
                    s = "\n\tvoid " + className + "::" + name + " (" + cppParamType + " " + paramName + "Normal, " + cppParamType + " " + paramName + "Pressed, " + cppParamType + " " + paramName + "Inactive) {";
                    s += "\n\t\tGApp::Animations::Elements::SimpleButtonElement * el = ((GApp::Animations::Elements::SimpleButtonElement*)(&" + objName + "));";
                    s += "\n\t\tel->Normal.TP.SetText(" + paramName + "Normal);";
                    s += "\n\t\tel->Pressed.TP.SetText(" + paramName + "Pressed);";
                    s += "\n\t\tel->Inactive.TP.SetText(" + paramName + "Inactive);";
                }
                return s + "\n\t}\n";
            }
            public override void AddAnimationFunction(GACParser.Module m)
            {
                base.AddAnimationFunction(m);
                if (Getter_Enabled != YesNo.No)
                {
                    string nm = "Is" + Name + "Enabled";
                    m.Members[nm] = new GACParser.Member(m, GACParser.MemberType.Function, nm, nm, "bool", "", null);
                }
                if (Setter_Symbol_Image != SimpleButtonSetter.No)
                    CreateGetterForButtonSetter(m, "Set" + Name + "SymbolImage", Setter_Symbol_Image, "symbolImage", "Bitmap");
                if (Setter_Background_Image != SimpleButtonSetter.No)
                    CreateGetterForButtonSetter(m, "Set" + Name + "BackgroundImage", Setter_Background_Image, "backgroundImage", "Bitmap");

                if (Setter_Symbol_Color != SimpleButtonSetter.No)
                    CreateGetterForButtonSetter(m, "Set" + Name + "SymbolColor", Setter_Symbol_Color, "symbolColor", "Color");
                if (Setter_Background_Color != SimpleButtonSetter.No)
                    CreateGetterForButtonSetter(m, "Set" + Name + "BackgroundColor", Setter_Background_Color, "backgroundColor", "Color");
                if (Setter_Text_Color != SimpleButtonSetter.No)
                    CreateGetterForButtonSetter(m, "Set" + Name + "TextColor", Setter_Text_Color, "textColor", "Color");

                if (Setter_Text != SimpleButtonSetter.No)
                    CreateGetterForButtonSetter(m, "Set" + Name + "Text", Setter_Text, "text", "string");

                if (Setter_Enabled != YesNo.No)
                {
                    string nm = "Set" + Name + "Enable";
                    GACParser.Member mb = new GACParser.Member(m, GACParser.MemberType.Function, nm, nm, "void", "", null);
                    mb.AddParameter(new GACParser.Member(m, GACParser.MemberType.Variable, "enableState", "enableState", "bool", "", null));
                    m.Members[nm] = mb;
                }
            }
            public override string GetAnimationFunctionCPPHeaderDefinition()
            {
                string s = base.GetAnimationFunctionCPPHeaderDefinition();
                if (Setter_Symbol_Image != SimpleButtonSetter.No)
                    s += CreateCPPHeaderSetterForButtonSetter("Set"+Name+"SymbolImage", Setter_Symbol_Image, "symbolImage", "GApp::Resources::Bitmap*");
                if (Setter_Background_Image != SimpleButtonSetter.No)
                    s += CreateCPPHeaderSetterForButtonSetter("Set" + Name + "BackgroundImage", Setter_Background_Image, "backgroundImage", "GApp::Resources::Bitmap*");
                if (Setter_Symbol_Color != SimpleButtonSetter.No)
                    s += CreateCPPHeaderSetterForButtonSetter("Set" + Name + "SymbolColor", Setter_Symbol_Color, "symbolColor", "unsigned int");
                if (Setter_Background_Color != SimpleButtonSetter.No)
                    s += CreateCPPHeaderSetterForButtonSetter("Set" + Name + "BackgroundColor", Setter_Background_Color, "backgroundColor", "unsigned int");
                if (Setter_Text_Color != SimpleButtonSetter.No)
                    s += CreateCPPHeaderSetterForButtonSetter("Set" + Name + "TextColor", Setter_Text_Color, "textColor", "unsigned int");
                if (Setter_Text != SimpleButtonSetter.No)
                    s += CreateCPPHeaderSetterForButtonSetter("Set" + Name + "Text", Setter_Text, "text", "const char *");
                if (Getter_Enabled != YesNo.No)
                    s += "\n\tbool Is" + Name + "Enabled ();";
                if (Setter_Enabled != YesNo.No)
                    s += "\n\tvoid Set" + Name + "Enable (bool enableState);";

                return s;
            }
            public override string GetAnimationFunctionCPPImplementation(string className)
            {
                string s = base.GetAnimationFunctionCPPImplementation(className);
                if (Setter_Symbol_Image != SimpleButtonSetter.No)
                    s += CreateCPPCodeSetterForButtonSetter(className, "Set" + Name + "SymbolImage",Name,"Symbol", Setter_Symbol_Image, "symbolImage", "GApp::Resources::Bitmap*");
                if (Setter_Background_Image != SimpleButtonSetter.No)
                    s += CreateCPPCodeSetterForButtonSetter(className, "Set" + Name + "BackgroundImage", Name, "Background", Setter_Background_Image, "backgroundImage", "GApp::Resources::Bitmap*");
                if (Setter_Symbol_Color != SimpleButtonSetter.No)
                    s += CreateCPPCodeSetterForButtonSetter(className, "Set" + Name + "SymbolColor", Name, "SymbolColorBlending", Setter_Symbol_Color, "symbolColor", "unsigned int");
                if (Setter_Background_Color != SimpleButtonSetter.No)
                    s += CreateCPPCodeSetterForButtonSetter(className, "Set" + Name + "BackgroundColor", Name, "BackgroundlColorBlending", Setter_Background_Color, "backgroundColor", "unsigned int");
                if (Setter_Text_Color != SimpleButtonSetter.No)
                    s += CreateCPPCodeSetterForButtonSetter(className, "Set" + Name + "TextColor", Name, "TextColorBlending", Setter_Text_Color, "textColor", "unsigned int");
                if (Setter_Text != SimpleButtonSetter.No)
                    s += CreateCPPCodeSetterForButtonTextSetter(className, "Set" + Name + "Text", Name, Setter_Text, "text", "const char *");

                if (Getter_Enabled != YesNo.No)
                {
                    s += "\n\tbool " + className + "::Is" + Name + "Enabled () {";
                    s += "\n\t\tGApp::Animations::Elements::SimpleButtonElement * el = ((GApp::Animations::Elements::SimpleButtonElement*)(&" + Name + "));";
                    s += "\n\t\treturn el->Enabled;";
                    s += "\n\t}";
                }
                if (Setter_Enabled != YesNo.No)
                {
                    s += "\n\tvoid " + className + "::Set" + Name + "Enable (bool enableState) {";
                    s += "\n\t\tGApp::Animations::Elements::SimpleButtonElement * el = ((GApp::Animations::Elements::SimpleButtonElement*)(&" + Name + "));";
                    s += "\n\t\tel->IsPressed = false;";
                    s += "\n\t\tel->Enabled = enableState;";
                    s += "\n\t}";
                }
                return s;
            }
            #endregion
        }


        #endregion

        #region Animation Object
        public class ParameterInformation
        {
            public string Name = "";
            public string CppType = "";
            public string GACType = "";
            public bool UseAsParameter = true;
            public ParameterInformation(string name, string cppType) { Name = name; CppType = cppType; GACType = cppType; }
            public ParameterInformation(string name, string cppType, string gacType) { Name = name; CppType = cppType; GACType = gacType; }
        }
        public class AnimationParameters
        {
            public Dictionary<string, ParameterInformation> Parameters = new Dictionary<string, ParameterInformation>();
            public List<ParameterInformation> ParametersList = new List<ParameterInformation>();

            public void Clear()
            {
                ParametersList.Clear();
                Parameters.Clear();
            }
        };
        public enum AnimationDesignMode
        {
            Screen,
            Control,
            Button
        }

        [XmlType("AnimationObject"), XmlRoot("AnimationObject")]
        public class AnimationObject
        {
            [XmlAttribute()]
            public string Name;
            [XmlAttribute()]
            public Coordinates Coord = Coordinates.Percentage;
            [XmlAttribute()]
            public AnimationDesignMode DesignMode = AnimationDesignMode.Screen;
            [XmlAttribute()]
            public int ControlWidth = 100;
            [XmlAttribute()]
            public int ControlHeight = 50;
            [XmlAttribute()]
            public int BackgroundColor = Color.Black.ToArgb();
            [XmlAttribute()]
            public string ZOrder = "";
            [XmlAttribute()]
            public bool AutoStart = false;

            [XmlArrayItem(typeof(EntireSurfaceElement))]
            [XmlArrayItem(typeof(ImageElement))]
            [XmlArrayItem(typeof(TextElement))]
            [XmlArrayItem(typeof(RectangleElement))]
            [XmlArrayItem(typeof(ExclusionRectangleElement))]
            [XmlArrayItem(typeof(ClipRectangleElement))]
            [XmlArrayItem(typeof(DisableClippingElement))]
            [XmlArrayItem(typeof(SimpleButtonElement))]
            [XmlArrayItem(typeof(GenericElementWithPosition))]
            [XmlArrayItem(typeof(GenericElementWithPositionAndSize))]
            public List<GenericElement> Elements = new List<GenericElement>();

            #region List View Props
            [XmlIgnore(), Browsable(false)]
            public string propName { get { return Name; } }
            [XmlIgnore(), Browsable(false)]
            public string propDescription { get { return "Elements: "+Elements.Count.ToString() +" ("+ZOrder+")"; } }
            [XmlIgnore(), Browsable(false)]
            public string propDesignMode { get { return DesignMode.ToString(); } }
            [XmlIgnore(), Browsable(false)]
            public bool propAutoStart { get { return AutoStart; } set { AutoStart = value; } }
            [XmlIgnore(), Browsable(false)]
            public string propCoord { get { return Coord.ToString(); }  }
            #endregion

            #region Atribute
            [XmlIgnore(), Description("Name"), Category("General"), DisplayName("Name")]
            public string _Name
            {
                get { return Name; }
                set
                {
                    if (Project.ValidateVariableNameCorectness(value, false) == false)
                    {
                        MessageBox.Show("Name should only contain letters, numbers and '_' character. The first character of the name must be a letter : 'A' - 'Z' !");
                        return;
                    }
                    Name = value;
                }
            }
            [XmlIgnore(), Description("Elements"), Category("General"), DisplayName("Elements")]
            public int _Elements
            {
                get { return Elements.Count; }
            }
            [XmlIgnore(), Description("Elements"), Category("General"), DisplayName("Z-Order")]
            public string _Z_Order
            {
                get { return ZOrder; }
            }
            [XmlIgnore(), Description("Automatically start animation when object is created"), Category("General"), DisplayName("Auto start")]
            public bool _AutoStart
            {
                get { return AutoStart; }
                set { AutoStart = value; }
            }
            [XmlIgnore(), Description("Coordinates type"), Category("Layout"), DisplayName("Coordinates")]
            public Coordinates _Coord
            {
                get { return Coord; }
                set { Coord = value; }
            }
            [XmlIgnore(), Description("Select the purpose of the animation. It could be for Screen animations or Control animations."), Category("Layout"), DisplayName("Design Mode")]
            public AnimationDesignMode _DesignMode
            {
                get { return DesignMode; }
                set { DesignMode = value; }
            }
            [XmlIgnore(), Description("Size of the control that holds the animation"), Category("Layout"), DisplayName("Control Size")]
            public string _Size
            {
                get { return string.Format("{0} x {1}", ControlWidth, ControlHeight); ; }
                set
                {
                    int w = 0, h = 0;
                    if (Project.SizeToValues(value, ref w, ref h)) { ControlWidth = w; ControlHeight = h; }
                }
            }
            [XmlIgnore(), Category("Layout"), DisplayName("Design background color")]
            public System.Drawing.Color _BackgroundColor
            {
                get { return System.Drawing.Color.FromArgb(BackgroundColor); }
                set { BackgroundColor = value.ToArgb(); }
            }
            #endregion

            public GenericTransformation Main = null;
            private int ZOrderID = -1;
            private AnimationParameters animParams = new AnimationParameters();

            [XmlIgnore()]
            public List<GenericElement> ParentsPositionOrder = new List<GenericElement>();

            private void ComputeAllTransformationsListRecursivelly(AnimO.GenericTransformation root, List<GenericTransformation> lst)
            {
                if (root == null)
                    return;
                root.CppName = "transformation_" + lst.Count.ToString();
                if (root.GetType() == typeof(ZOrderTransformation))
                {
                    ((ZOrderTransformation)root).ZOrderID = ZOrderID;
                    ZOrderID++;
                }
                lst.Add(root);
                var lst_trans = root.GetBlockTransformations();
                if (lst_trans != null)
                {
                    foreach (var t in lst_trans)
                        ComputeAllTransformationsListRecursivelly(t,lst);
                }
            }
            List<GenericTransformation> GetAllTransformations()
            {
                List<GenericTransformation> lst = new List<GenericTransformation>();
                ZOrderID = 1;
                ComputeAllTransformationsListRecursivelly(Main, lst);
                return lst;
            }

            private void ProcessTransformation(Dictionary<string, GenericElement> elements, AnimO.GenericTransformation root, Project prj, AppResources resources)
            {
                if (root == null)
                    return;
                root.Validate(prj, Name, resources,elements);
                // doar pentru ZOrdeer
                if (root.GetType() == typeof(ZOrderTransformation))
                    ValidateZOrder(((ZOrderTransformation)root).ZOrder, elements, prj);
                var lst = root.GetBlockTransformations();
                root.SetElement(null);
                var et = root as GenericElementTransformation;
                if (et!=null)
                {
                    if (elements.ContainsKey(et.Element)==false)
                    {
                        prj.EC.AddError("Animation '" + Name + "' has a transformation that refers to an unexisting element : '" + et.Element + "' !");
                    }
                    else
                    {
                        et.SetElement(elements[et.Element]);
                    }
                }
                if (lst!=null)
                {
                    foreach (var t in lst)
                        ProcessTransformation(elements, t, prj, resources);
                }
            }
            private void CreateParametersForTransformations(AnimO.GenericTransformation root, AnimationParameters p)
            {
                if (root == null)
                    return;
                root.PopulateParameters(p);
                var lst = root.GetBlockTransformations();
                if (lst != null)
                {
                    foreach (var t in lst)
                    {
                        CreateParametersForTransformations(t, p);
                    }
                }
            }
            private bool CreateParameters(AnimationParameters p, Project prj)
            {
                bool ok = true;
                foreach (var e in Elements)
                {
                    e.PopulateParameters(p);
                }
                CreateParametersForTransformations(Main, p);
                // fac si dictionarul
                foreach (ParameterInformation pi in p.ParametersList)
                {
                    if (p.Parameters.ContainsKey(pi.Name))
                    {
                        ParameterInformation current = p.Parameters[pi.Name];
                        if (pi.GACType != current.GACType)
                        {
                            if (prj != null)
                                prj.EC.AddError("Parameter '" + pi.Name + "' from animation :'" + this.Name + "' has been defined with two different types ("+pi.GACType+" and "+current.GACType+") !");
                            ok = false;
                        }
                        if (pi.CppType != current.CppType)
                        {
                            if (prj != null)
                                prj.EC.AddError("Parameter '" + pi.Name + "' from animation :'" + this.Name + "' has been defined with two different cpp translations (" + pi.CppType + " and " + current.CppType + ") !");
                            ok = false;
                        }
                        pi.UseAsParameter = false;
                    }
                    else
                    {
                        p.Parameters[pi.Name] = pi;
                        pi.UseAsParameter = true;
                    }
                }
                return ok;
            }

            private string ProcessNode(string name, int depth, Dictionary<string, List<string>> nodes, Dictionary<string, int> nodes_order)
            {
                if (depth > 0)
                {
                    if ((nodes_order[name] & 0x10000) != 0)
                        return "Element '" + name + "' is part of a circular reference !";
                    nodes_order[name] = 0x10000 | depth;
                }
                if (nodes.ContainsKey(name))
                {
                    depth++;
                    foreach (string copil in nodes[name])
                    {
                        string s = ProcessNode(copil, depth, nodes, nodes_order);
                        if (s != null)
                            return s;
                    }
                }
                return null;
            }
            public bool ValidateElementOrder(Project prj, Dictionary<string, GenericElement> elements)
            {
                Dictionary<string, List<string>> nodes = new Dictionary<string, List<string>>();
                Dictionary<string, int> nodes_order = new Dictionary<string, int>();
                ParentsPositionOrder.Clear();
                foreach (GenericElement el in this.Elements)
                {
                    if (el.Parent.Equals("<None>"))
                        el.ParentElement = null;
                    else
                    {
                        if (elements.TryGetValue(el.Parent, out el.ParentElement) == false)
                        {
                            prj.EC.AddError("Element '" + el.Name + "' has an invalid parent: '" + el.Parent + "' in animation: '" + this.Name + "'");
                            return false;
                        }
                    }                    
                    nodes_order[el.Name] = 0;
                    if (nodes.ContainsKey(el.Parent) == false)
                        nodes[el.Parent] = new List<string>();
                    nodes[el.Parent].Add(el.Name);
                }
                if (nodes.ContainsKey("<None>") == false)
                {
                    prj.EC.AddError("Animation '" + this.Name + "' does not have any element relative to animation (that have a parent <None>)");
                    return false;
                }
                string res;
                res = ProcessNode("<None>", 0, nodes, nodes_order);
                if (res != null)
                {
                    prj.EC.AddError(res + " in animation '" + this.Name + "'");
                    return false;
                }
                // verific daca toate nodurile au fost procesate
                foreach (string nodeName in nodes_order.Keys)
                    if ((nodes_order[nodeName] & 0x10000) == 0) 
                        prj.EC.AddError("Element '" + nodeName + "' from Animation '" + this.Name + "' does not have a valid parent !");
                if (prj.EC.HasErrors())
                    return false;
                // all good - fac si lista cu ordinea de procesare
                List<KeyValuePair<string, int>> all_nodes = new List<KeyValuePair<string, int>>();
                foreach (var item in nodes_order)
                    all_nodes.Add(item);
                all_nodes.Sort((firstValue, nextValue) => { return firstValue.Value.CompareTo(nextValue.Value); });
                // pun datele
                
                foreach (var item in all_nodes)
                {
                    ParentsPositionOrder.Add(elements[item.Key]);
                }
                return true;
            }
            private void ValidateZOrder(string z_order, Dictionary<string, GenericElement> elements, Project prj)
            {
                List<string> l = Project.StringListToList(z_order);
                foreach (string k in elements.Keys)
                    elements[k]._FoundInZOrder_ = false;
                foreach (string name in l)
                {
                    if (elements.ContainsKey(name) == false)
                    {
                        prj.EC.AddError("Animation '" + Name + "' has an invalid/unknwon element in the Z-Order list: '" + name + "' !");
                    }
                    else
                    {
                        elements[name]._FoundInZOrder_ = true;
                    }
                }
                // verific daca toate sunt
                foreach (string k in elements.Keys)
                    if (elements[k]._FoundInZOrder_ == false)
                    {
                        prj.EC.AddError("Animation '" + Name + "' does not have element '"+k+"' added in the Z-Order list !");
                    }
            }

            public bool ValidateAnimation(Project prj,AppResources resources)
            {
                if ((DesignMode == AnimationDesignMode.Control) || (DesignMode == AnimationDesignMode.Button))
                {
                    if (ControlWidth<10)
                        prj.EC.AddError("Animation '" + Name + "' has a Control/Button Width too small: " + ControlWidth.ToString() + " !");
                    if (ControlHeight < 10)
                        prj.EC.AddError("Animation '" + Name + "' has a Control/Button Height too small: " + ControlHeight.ToString() + " !");
                }
                Dictionary<string, GenericElement> elements = new Dictionary<string, GenericElement>();
                foreach (var e in Elements)
                {
                    if (elements.ContainsKey(e.Name))
                        prj.EC.AddError("Animation '" + Name + "' has a duplicate element: '" + e.Name + "' !");
                    else
                        elements[e.Name] = e;  
                    string s = e.Validate(prj,resources);
                    if (s != null)
                        prj.EC.AddError("Animation '" + Name + "' has an incorecet set up element: '" + e.Name + "' => Error: "+s);
                }
                // validez ZOrder
                ValidateZOrder(ZOrder, elements, prj);
                // validez relatia element - parinte
                ValidateElementOrder(prj, elements);
                // setez si eelementele in transformari
                ProcessTransformation(elements, Main, prj, resources);
                // validez si parametri
                animParams.Clear();
                if (CreateParameters(animParams,prj)==false)
                    prj.EC.AddError("Unable to valide all parameters !");
                // ies
                return !prj.EC.HasErrors();
            }

            public AnimationObject MakeCopy()
            {
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(AnimationObject));
                    StringWriter textWriter = new StringWriter();
                    serializer.Serialize(textWriter, this);
                    StringReader textReader = new StringReader(textWriter.ToString());
                    return (AnimationObject)serializer.Deserialize(textReader);
                }
                catch (Exception e)
                {
                    return null;
                }
            }

            private void AddCreateParameters(GACParser.Member fnc,GACParser.Module m)
            {
                foreach (var p in this.animParams.ParametersList)
                {
                    if (p.UseAsParameter)
                        fnc.AddParameter(new GACParser.Member(m, GACParser.MemberType.Variable, p.Name, p.Name, p.GACType, "", null));
                }
            }
            public void BuildGACParameters(GACParser.Module m, Dictionary<string, List<string>> enums)
            {
                m.Members["Start"] = new GACParser.Member(m, GACParser.MemberType.Function, "Start", "Start", "bool", "", null);
                m.Members["Stop"] = new GACParser.Member(m, GACParser.MemberType.Function, "Stop", "Stop", "bool", "", null);
                m.Members["IsRunning"] = new GACParser.Member(m, GACParser.MemberType.Function, "IsRunning", "IsRunning", "bool", "", null);
                m.Members["Pause"] = new GACParser.Member(m, GACParser.MemberType.Function, "Pause", "Pause", "bool", "", null);
                m.Members["Resume"] = new GACParser.Member(m, GACParser.MemberType.Function, "Resume", "Resume", "bool", "", null);
                m.Members["IsPaused"] = new GACParser.Member(m, GACParser.MemberType.Function, "IsPaused", "IsPaused", "bool", "", null);
                m.Members["Paint"] = new GACParser.Member(m, GACParser.MemberType.Function, "Paint", "Paint", "void", "", null);
                m.Members["ExitPopupLoop"] = new GACParser.Member(m, GACParser.MemberType.Function, "ExitPopupLoop", "ExitPopupLoop", "void", "", null);
                m.Members["ResetMovementOffsets"] = new GACParser.Member(m, GACParser.MemberType.Function, "ResetMovementOffsets", "ResetMovementOffsets", "void", "", null);
                GACParser.Member MoveWithOffset = new GACParser.Member(m, GACParser.MemberType.Function, "MoveWithOffset", "MoveWithOffset", "void", "", null);
                MoveWithOffset.AddParameter(new GACParser.Member(m, GACParser.MemberType.Variable,"offsetX","offsetX","float","",null));
                MoveWithOffset.AddParameter(new GACParser.Member(m, GACParser.MemberType.Variable,"offsetY","offsetY","float","",null));
                MoveWithOffset.AddParameter(new GACParser.Member(m, GACParser.MemberType.Variable, "coord", "coord", "Coordinates", "", null));
                m.Members["MoveWithOffset"] = MoveWithOffset;
                GACParser.Member OnTouchEvent = new GACParser.Member(m, GACParser.MemberType.Function, "OnTouchEvent", "OnTouchEvent", "bool", "", null);
                OnTouchEvent.AddParameter(new GACParser.Member(m, GACParser.MemberType.Variable, "touchEvent", "touchEvent", "TouchEvent", "", null));
                m.Members["OnTouchEvent"] = OnTouchEvent;
                // update parametri
                animParams.Clear();
                CreateParameters(animParams, null);
                // functia de create
                GACParser.Member mCreate = new GACParser.Member(m, GACParser.MemberType.Function, "Create", "Create", "void", "", null);
                AddCreateParameters(mCreate, m);
                m.Members["Create"] = mCreate;
                // constructorii
                GACParser.Member mConstrScene = new GACParser.Member(m, GACParser.MemberType.Constructor, "", "", "", "", null);
                mConstrScene.AddParameter(new GACParser.Member(m, GACParser.MemberType.Variable, "scene", "scene", "Scene", "", null));
                AddCreateParameters(mConstrScene, m);
                m.Members[""] = mConstrScene;

                GACParser.Member mConstrApp = new GACParser.Member(m, GACParser.MemberType.Constructor, "", "", "", "", null);
                mConstrApp.AddParameter(new GACParser.Member(m, GACParser.MemberType.Variable, "app", "app", "Application", "", null));
                AddCreateParameters(mConstrApp, m);
                m.Members[""].Overrides = new List<GACParser.Member>();
                m.Members[""].Overrides.Add(mConstrApp);

                GACParser.Member mConstrObj = new GACParser.Member(m, GACParser.MemberType.Constructor, "", "", "", "", null);
                mConstrObj.AddParameter(new GACParser.Member(m, GACParser.MemberType.Variable, "obj", "__obj__", "Object", "", null));
                AddCreateParameters(mConstrObj, m);
                m.Members[""].Overrides.Add(mConstrObj);

                var l = GetAllTransformations();
                foreach (var t in l)
                {
                    t.AddAnimationFunction(m);
                    t.CreateGACEnums(enums);
                }
                foreach (var e in Elements)
                    e.AddAnimationFunction(m);
            }

            public string GetCPPClassName()
            {
                return "GAC_" + Name;
            }
            private string CreateParameterListPrototype()
            {
                string lst = "";
                foreach (var p in this.animParams.ParametersList)
                {
                    if (p.UseAsParameter)
                        lst += p.CppType + " " + p.Name + ", ";
                }
                if (lst.Length>0)
                {
                    lst = lst.Substring(0, lst.Length - 2);
                }
                return lst;
            }
            private string CreateParameterListForCall()
            {
                string lst = "";
                foreach (var p in this.animParams.ParametersList)
                {
                    if (p.UseAsParameter)
                        lst += p.Name + ", ";
                }
                if (lst.Length > 0)
                {
                    lst = lst.Substring(0, lst.Length - 2);
                }
                return lst;
            }
            public string CreateCPPWrapperHeaderClass()
            {
                string s = "\n// ====================== Animation wrapper class for " + Name + " ===========================================\n";
                s += "\nclass " + GetCPPClassName() + " : public GApp::Animations::AnimationObject {";
                // elementele
                s += "\n\tGApp::Animations::Elements::Element *ZOrder[" + Elements.Count.ToString() + "];";
                s += "\n\tGApp::Animations::Elements::Element *RelativeOrder[" + Elements.Count.ToString() + "];";

                foreach (GenericElement e in Elements)
                {
                    s += "\n\tGApp::Animations::Elements::" + e.GetCPPClassName() + " " + e.Name + ";";
                }
                // transformarile
                var l = GetAllTransformations();
                foreach (var t in l)
                {
                    s += "\n\tGApp::Animations::Transformations::" + t.GetCPPClassName() + " " + t.CppName + ";";
                }
                // parametri             
                Dictionary<string, bool> p_d = new Dictionary<string, bool>();
                foreach (var p in this.animParams.ParametersList)
                {
                    //if (p_d.ContainsKey(p.Name) == false)
                    //{
                    if (p.UseAsParameter)
                        s += "\n\t" + p.CppType + " param_" + p.Name + ";";
                    //    p_d[p.Name] = true;
                    //}
                }
                string param_list = CreateParameterListPrototype();

                s += "\n\n\tvoid InitializeTransformations();";
                s += "\npublic:";
                if (param_list.Length > 0)
                {
                    s += "\n\t" + GetCPPClassName() + "(GApp::UI::Scene *scene, "+param_list+");";
                    s += "\n\t" + GetCPPClassName() + "(GApp::UI::Application *application, "+param_list+");";
                    s += "\n\t" + GetCPPClassName() + "(GApp::UI::FrameworkObject *frameworkObject, "+param_list+");";
                    s += "\n\t void Create (" + param_list + ");";
                }
                else
                {
                    s += "\n\t" + GetCPPClassName() + "(GApp::UI::Scene *scene);";
                    s += "\n\t" + GetCPPClassName() + "(GApp::UI::Application *a);";
                    s += "\n\t" + GetCPPClassName() + "(GApp::UI::FrameworkObject *frameworkObject);";
                    s += "\n\t void Create ();";
                }
                foreach (var t in l)
                {
                    s += t.GetAnimationFunctionCPPHeaderDefinition();
                }
                foreach (GenericElement e in Elements)
                {
                    s += e.GetAnimationFunctionCPPHeaderDefinition();
                }
                s += "\n\tvirtual void Paint();";
                s += "\n\tvirtual void ButtonPaint(bool visible);";
                s += "\n\tvirtual bool ControlPaint(bool loop) ;";
                s += "\n\tvirtual bool OnStart();";
                s += "\n\tvirtual void SetZOrder(int index);";
                s += "\n\tvirtual bool OnTouchEvent(GApp::Controls::TouchEvent *te);";
                return s + "\n};\n";
            }

            public string CreateCPPWrapperCodeClass()
            {
                string s = "\n// ====================== Animation wrapper class for " + Name + " ===========================================\n";
                // constructorii
                string param_list = CreateParameterListPrototype();
                string param_list_for_call = CreateParameterListForCall();
                bool hasClipping = false;
                // constructor componeent
                string constr_code = "\n\tCoreContext = __init_object__->CoreContext;";
                if ((DesignMode == AnimationDesignMode.Control) || (DesignMode == AnimationDesignMode.Button))
                {
                    constr_code += "\n\tWidth = " + ControlWidth.ToString() + " * (Core.ResolutionAspectRatio);";
                    constr_code += "\n\tHeight = " + ControlHeight.ToString() + " * (Core.ResolutionAspectRatio);";
                }
                else
                {
                    constr_code += "\n\tWidth = (float)(Core.Width);";
                    constr_code += "\n\tHeight = (float)(Core.Height);";
                }
                //if (this.Coord == Coordinates.Pixels)
                //    constr_code += "\n\tFlags = GAC_COORDINATES_PIXELS << 16;";
                //else
                // COORDONATELE SUNT TOT TIMPUL IN PROCENTE
                constr_code += "\n\tFlags = GAC_COORDINATES_PERCENTAGE << 16;";
                for (int tr = 0; tr < this.ParentsPositionOrder.Count; tr++)
                    constr_code += "\n\tthis->RelativeOrder[" + tr.ToString() + "] = &" + this.ParentsPositionOrder[tr].Name + ";";
                constr_code += "\n\tInitializeTransformations();";
                constr_code += "\n\tCreate(" + param_list_for_call + ");";
                constr_code += "\n}";
                
                // constructor pentru SCENA =======================================================================================================================================
                if (param_list.Length > 0)
                    s += "\n" + GetCPPClassName() + "::" + GetCPPClassName() + "(GApp::UI::Scene *__init_object__, " + param_list + ") {";
                else
                    s += "\n" + GetCPPClassName() + "::" + GetCPPClassName() + "(GApp::UI::Scene *__init_object__) {";
                s += constr_code;

                // constructor pentru Applicaction =======================================================================================================================================
                if (param_list.Length > 0)
                    s += "\n" + GetCPPClassName() + "::" + GetCPPClassName() + "(GApp::UI::Application *__init_object__, "+param_list+") {";
                else
                    s += "\n" + GetCPPClassName() + "::" + GetCPPClassName() + "(GApp::UI::Application *__init_object__) {";
                s += constr_code;
                
                // constructor pentru FrameworkObject =======================================================================================================================================
                if (param_list.Length > 0)
                    s += "\n" + GetCPPClassName() + "::" + GetCPPClassName() + "(GApp::UI::FrameworkObject *__init_object__, " + param_list + ") {";
                else
                    s += "\n" + GetCPPClassName() + "::" + GetCPPClassName() + "(GApp::UI::FrameworkObject *__init_object__) {";
                s += constr_code;

                // functia de create
                s += "\nvoid " + GetCPPClassName() + "::Create("+param_list+") {";
                s += "\n\tthis->OffsetX = this->OffsetY = 0.0f;";
                if ((AutoStart) && (DesignMode == AnimationDesignMode.Screen))
                    s += "\n\tStop();";
                foreach (var p in this.animParams.ParametersList)
                    if (p.UseAsParameter)
                        s += "\n\tparam_" + p.Name + " = " + p.Name + ";";
                if ((AutoStart) && (DesignMode == AnimationDesignMode.Screen))
                    s += "\n\tStart();";
                s += "\n}";

                s += "\nvoid " + GetCPPClassName() + "::InitializeTransformations() {";
                var transformations_list = GetAllTransformations();
                foreach (var t in transformations_list)
                {
                    s += t.CreateInitializationCPPCode();
                }
                s += "\n}";

                // codul pentru OnStart
                s += "\nbool " + GetCPPClassName() + "::OnStart() {";
                foreach (var elem in this.Elements)
                {
                    s += elem.CreateOnStartCPPCode(this);
                    if (elem.GetType() == typeof(AnimO.ClipRectangleElement))
                        hasClipping = true;
                }
                foreach (var t in transformations_list)
                {
                    s += t.CreateOnStartCPPCode();
                }
                // ZOrder
                s += "\n\tSetZOrder(0);";
                // pornesc prima transformare
                if (Main != null)
                    s += "\n\ttransformation_0.Init(this);";
                s += "\n\treturn true;";
                s += "\n}";

                string paint_code = "";
                paint_code += "\n\t\tGApp::Animations::Elements::Element **z = &RelativeOrder[0];";
                for (int tr = 0; tr < (Elements.Count) - 1; tr++)
                {
                    paint_code += "\n\t\t(*z)->UpdateScreenRect(this);z++;";
                }
                paint_code += "\n\t\t(*z)->UpdateScreenRect(this);";
                paint_code += "\n\t\tz = &ZOrder[0];";
                for (int tr = 0; tr < (Elements.Count) - 1; tr++)
                {
                    paint_code += "\n\t\t(*z)->Paint(this);z++;";
                }
                paint_code += "\n\t\t(*z)->Paint(this);";
                // codul pentru Paint
                switch (this.DesignMode)
                {
                    case AnimationDesignMode.Screen:
                        s += "\nbool " + GetCPPClassName() + "::ControlPaint(bool loop)  { return true; };";
                        s += "\nvoid " + GetCPPClassName() + "::ButtonPaint(bool visible) { };";
                        s += "\nvoid " + GetCPPClassName() + "::Paint() {";
                        s += "\n\tif ((Flags & 3)!=1) return;";
                        if (Main != null)
                            s += "\n\tif (transformation_0.Update(this)==false) { Stop(); return; }";
                        // paint
                        s += paint_code;
                        // sterg clipping-ul (daca am)
                        if (hasClipping)
                            s += "\n\tG.ClearClip();";
                        s += "\n}";
                        break;
                    case AnimationDesignMode.Button:
                        s += "\nvoid " + GetCPPClassName() + "::Paint() { };";
                        s += "\nbool " + GetCPPClassName() + "::ControlPaint(bool loop)  { return true; };";
                        s += "\nvoid " + GetCPPClassName() + "::ButtonPaint(bool visible) {";
                        s += "\n\tif ((Flags & 1)!=1) Start();";
                        if (Main != null)
                            s += "\n\tif (transformation_0.Update(this)==false) { Stop(); Start(); }";
                        // cazul 1 (fara ordine)
                        s += "\n\tif (visible) {";
                        s += paint_code;
                        s += "\n\t};";
                        s += "\n}";
                        break;
                    case AnimationDesignMode.Control:
                        s += "\nvoid " + GetCPPClassName() + "::Paint() { };";
                        s += "\nvoid " + GetCPPClassName() + "::ButtonPaint(bool visible) { };";
                        s += "\nbool " + GetCPPClassName() + "::ControlPaint(bool loop) {";
                        s += "\n\tif ((Flags & 3)!=1) return true;";
                        if (Main != null)
                        {
                            s += "\n\tbool ret_code = true;";
                            s += "\n\tif (transformation_0.Update(this)==false) { if (loop) Start(); else { Stop();ret_code=false;} } ";
                            s += paint_code;
                            s += "\n\treturn ret_code;";
                        }
                        else
                        {
                            s += paint_code;
                            s += "\n\treturn true;";
                        }
                        s += "\n}";
                        break;
                }


                // codul pentru alte funcii
                string class_name = GetCPPClassName();
                foreach (var t in transformations_list)
                {
                    s += t.GetAnimationFunctionCPPImplementation(class_name);
                }
                foreach (var e in Elements)
                {
                    s += e.GetAnimationFunctionCPPImplementation(class_name);
                }
                // ZOrder
                s += "\nvoid " + GetCPPClassName() + "::SetZOrder(int index) {";
                s += "\n\tswitch (index) {";
                s += CreateZOrderSetUpCppCode(ZOrder, 0); // default
                foreach (var t in transformations_list)
                {
                    if (t.GetType() == typeof(ZOrderTransformation))
                        s += CreateZOrderSetUpCppCode(((ZOrderTransformation)t).ZOrder, ((ZOrderTransformation)t).ZOrderID);
                }
                s += "\n\t}";
                s += "\n}";

                // OnTouchEvent
                s += "\nbool " + GetCPPClassName() + "::OnTouchEvent(GApp::Controls::TouchEvent *te) {";
                s += "\n\treturn ProcessTouchEvents(te,ZOrder," + Elements.Count.ToString() + ");";
                s += "\n}";
                return s +"\n";
            }

            private string CreateZOrderSetUpCppCode(string z_order,int index)
            {
                List<string> l = Project.StringListToList(z_order);
                l.Reverse();
                string s = "\n\t\tcase "+index.ToString()+":";
                for (int tr=0;tr<l.Count;tr++)
                {
                    s += "\n\t\t\tZOrder["+tr.ToString()+"] = &"+l[tr]+";";
                }
                s += "\n\t\t\tbreak;";
                return s;
            }

            public void AddElement(AnimO.GenericElement e)
            {
                Elements.Add(e);
                if (ZOrder.Length == 0)
                    ZOrder = e.Name;
                else
                    ZOrder = e.Name + ","+ZOrder;
            }
            public void RemoveElement(AnimO.GenericElement e)
            {
                Elements.Remove(e);
                List<string> l = Project.StringListToList(ZOrder);
                for (int tr=0;tr<l.Count;tr++)
                {
                    if (l[tr] == e.Name)
                    {
                        l.RemoveAt(tr);
                        break;
                    }
                }
                ZOrder = Project.ListToStringList(l);
            }


        }


        #endregion 
    }
    #endregion


}