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


}