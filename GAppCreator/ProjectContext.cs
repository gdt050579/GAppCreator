using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace GAppCreator
{
    public class ProjectContext
    {
        [XmlType("IDESettings"), XmlRoot("IDESettings")]
        public class IDEProjectSettings
        {
            [XmlAttribute()]
            public bool EnableResourceTab = true;
            [XmlAttribute()]
            public bool EnableCodeTab = true;
            [XmlAttribute()]
            public bool EnableAdvertismentTab = true;
            [XmlAttribute()]
            public bool EnableStringsTab = true;
            [XmlAttribute()]
            public bool EnableMemoryProfileTab = true;
            [XmlAttribute()]
            public bool EnablePublisherTab = true;
            
            public static IDEProjectSettings Load(string fileName, ErrorsContainer EC = null)
            {
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(IDEProjectSettings));
                    TextReader textReader = new StreamReader(fileName);
                    IDEProjectSettings sol = (IDEProjectSettings)serializer.Deserialize(textReader);
                    textReader.Close();
                    if (sol == null)
                    {
                        if (EC != null)
                            EC.AddError("Unable to load gac creator project settings - XML load returns NULL: " + fileName);
                    }
                    return sol;
                }
                catch (Exception e)
                {
                    if (EC != null)
                        EC.AddException("Unable to load gac creator project settings: " + fileName, e);
                    return null;
                }
            }
            public bool Save(string fileName, ErrorsContainer EC = null)
            {
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(IDEProjectSettings));
                    TextWriter textWriter = new StreamWriter(fileName);
                    serializer.Serialize(textWriter, this);
                    textWriter.Close();
                    return true;
                }
                catch (Exception e)
                {
                    if (EC != null)
                        EC.AddError("Unable to save  gac creator project settings  - " + e.ToString());

                    return false;
                }
            }
        }
        public interface CommonFunctions
        {
            bool SaveProject();
            bool LoadProject(string fname, bool newProject);
        };
        public Project Prj;
        public AppResources Resources;
        public GenericBuildConfiguration CurrentBuild = null;
        public ImageList LargeIcons = null;
        public ImageList SmallIcons = null;
        public BackgroundTask Task;
        public SystemSettings Settings;
        public CommonFunctions Functions;
        public ToolStripStatusLabel LabelGeneralInfo,LabelInfo;
        public ListView BackgroundTaskList;
        public PluginList Plugins = new PluginList();
        public IDEProjectSettings IDESettings = new IDEProjectSettings();
        public long RunTime = 0;
        public long ActiveTime = 0;
    }
}
