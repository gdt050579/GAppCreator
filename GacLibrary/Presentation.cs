using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Drawing;

namespace GAppCreator
{
    public interface IPresentationFrameViewer
    {
        void OnUpdateAnimationSize();
        void UpdateSelectedObjectsPositions(GRect oldRect, GRect newRect);
    }
    public class Presentation
    {
        class HashCountItem
        {
            public string md5;
            public int count;
        };

        [XmlAttribute()]
        public int DesignWidth, DesignHeight,Width = 100, Height = 100;
        
        public List<PresentationFrame> Frames;
        public List<ResourceKeyName> ImageTemplateNames = new List<ResourceKeyName>();
        public List<ResourceKeyName> FontTemplateNames = new List<ResourceKeyName>();
        public List<ResourceKeyName> ShaderTemplateNames = new List<ResourceKeyName>();
        public List<ResourceKeyName> StringTemplateNames = new List<ResourceKeyName>();
        public List<ResourceKeyName> SoundTemplateNames = new List<ResourceKeyName>();


        #region Variabile interne
        [XmlIgnore()]
        private GBitmap screenBitmap;
        [XmlIgnore()]
        private AnimO.Canvas screenBitmapCanvas;
        [XmlIgnore()]
        public IPresentationFrameViewer animViewer = null;
        [XmlIgnore()]
        public string FilePath;
        [XmlIgnore()]
        public GBitmap BackgroundImage = null;
        [XmlIgnore()]
        public int BackgroundColor = Color.Black.ToArgb();
        #endregion

        #region Atribute
        [XmlIgnore(), Description("Sets the resolution of the target device/platform where this animation will be used"), Category("Layout"), DisplayName("Resolution")]
        public string _Resolution
        {
            get { return string.Format("{0} x {1}", DesignWidth, DesignHeight); ; }
            set
            {
                int w = 0, h = 0;
                if (Project.SizeToValues(value, ref w, ref h)) { DesignWidth = w; DesignHeight = h; }
            }
        }
        [XmlIgnore(), Description("Presentation size"), Category("Layout"), DisplayName("Size")]
        public string _Size
        {
            get { return string.Format("{0} x {1}", Width, Height); ; }
            set
            {
                int w = 0, h = 0;
                if (Project.SizeToValues(value, ref w, ref h)) { Width = w; Height = h; }
            }
        }    
        [XmlIgnore(), Category("Informations"), DisplayName("Frame Count")]
        public int _FrameCount
        {
            get { return Frames.Count; }            
        }
        [XmlIgnore(), Category("Informations"), DisplayName("File Name")]
        public String _FileName
        {
            get { return System.IO.Path.GetFileName(FilePath); }
        }
        #endregion

        private Project prj = null;
        [XmlIgnore()]
        public AppResources Resources = null;

        public Presentation()
        {
            Frames =  new List<PresentationFrame>();
            animViewer = null;
            screenBitmap = null;
            screenBitmapCanvas = new AnimO.Canvas();
            UpdateScreenBitmap();
        }
        public void SetProject(Project p, AppResources r)
        {
            prj = p;
            Resources = r;
        }
        private void UpdateScreenBitmap()
        {
            screenBitmap = new GBitmap();
            screenBitmap.Create(Width, Height);
            screenBitmapCanvas.SetScreen(0, 0, Width, Height, 1.0f);
        }
        private void UpdateScreenBitmapAndIcons()
        {
            UpdateFrameIcons();
            if ((animViewer != null))
                animViewer.OnUpdateAnimationSize();            
        }
        public void UpdateFrameIcon(PresentationFrame frame)
        {
            if ((Width!=screenBitmap.Width) || (Height != screenBitmap.Height))
            {
                screenBitmap.Dispose();
                screenBitmap.Create(Width, Height);
                screenBitmapCanvas.SetScreen(0, 0, Width, Height, 1.0f);
            }
            screenBitmap.GetGraphicContext().ClearScreen(BackgroundColor);
            if (BackgroundImage != null)
                screenBitmap.GetGraphicContext().FillScreen(BackgroundImage);
            screenBitmapCanvas.SetGraphics(screenBitmap.GetGraphicContext().g);
            frame.Paint(screenBitmapCanvas, Resources);
            frame.Icon.GetGraphicContext().ClearScreen(Color.White.ToArgb());
            frame.Icon.GetGraphicContext().DrawImage(screenBitmap, 0, 0, frame.Icon.Width, frame.Icon.Height, Alignament.Center, ImageResizeMode.Shrink);
            if (frame.Bookmark)
            {
                frame.Icon.GetGraphicContext().DrawCircle(5, 5, 4, Color.Yellow.ToArgb(),Color.Red.ToArgb(),1);
            }
        }
        public void UpdateFrameIcons()
        {
            foreach (PresentationFrame frame in Frames)
                UpdateFrameIcon(frame);
        }
        public void MoveZOrder(int index, int steps)
        {
            if ((index < 0) || (index >= Frames.Count))
                return;
            int newIndex = index + steps;
            if (newIndex < 0)
                newIndex = 0;
            if (newIndex > Frames.Count - 1)
                newIndex = Frames.Count - 1;
            if (newIndex == index)
                return;
            // mut datele
            PresentationFrame aux = Frames[index];
            if (newIndex < index)
            {
                for (int tr = index - 1; tr >= newIndex; tr--)
                    Frames[tr + 1] = Frames[tr];
            }
            else
            {
                for (int tr = index + 1; tr <= newIndex; tr++)
                    Frames[tr - 1] = Frames[tr];
            }
            Frames[newIndex] = aux;
        }
        public bool SaveToXML()
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Presentation));
                TextWriter textWriter = new StreamWriter(FilePath);
                serializer.Serialize(textWriter, this);
                textWriter.Close();
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show("Unable to save: " + FilePath + "\n" + e.ToString());
                return false;
            }
        }
        public static Presentation LoadFromXML(string fileName,ErrorsContainer ec)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Presentation));
                TextReader textReader = new StreamReader(fileName);
                Presentation an = (Presentation)serializer.Deserialize(textReader);
                textReader.Close();
                if (an == null)
                {
                    ec.AddError("Unable to create XML from " + fileName);                    
                    return null;
                }
                an.FilePath = fileName;
                an.UpdateScreenBitmap();
                return an;
            }
            catch (Exception e)
            {
                ec.AddException("Unable to load: " + fileName,e);                
                return null;
            }
        }
        public ResourcesIndexes CreateResourcesIndexes()
        {
            ResourcesIndexes ri = new ResourcesIndexes();
            Dictionary<string, ResourceAnimationItem> dict;
            string name = "";
            foreach (PresentationFrame Frame in Frames)
            {
                foreach (PresentationObject obj in Frame.Objects)
                {
                    dict = null;
                    if (obj.GetType() == typeof(ImagePresentationObject))
                    {
                        dict = ri.ImageIndex;
                        name = ((ImagePresentationObject)obj).ImageName;
                    }
                    if (obj.GetType() == typeof(TextPresentationObject))
                    {
                        if (((TextPresentationObject)obj).TextMode == TextModeType.FromResources)
                        {
                            dict = ri.StringIndex;
                            name = ((TextPresentationObject)obj).StringResource;
                        }
                        if (((TextPresentationObject)obj).TextMode == TextModeType.UserValue)
                        {
                            if (ri.StringValueIndex.ContainsKey(((TextPresentationObject)obj).ValueName) == false)
                                ri.StringValueIndex[((TextPresentationObject)obj).ValueName] = ri.StringValueIndex.Count;
                        }
                    }
                    if (dict != null)
                    {
                        if (dict.ContainsKey(name) == false)
                        {
                            dict[name] = new ResourceAnimationItem(dict.Count);
                        }
                    }


                    // fonturi
                    dict = null;
                    if (obj.GetType() == typeof(TextPresentationObject))
                    {
                        dict = ri.FontsIndex;
                        name = ((TextPresentationObject)obj).Font;
                    }
                    if (dict != null)
                    {
                        if (dict.ContainsKey(name) == false)
                            dict[name] = new ResourceAnimationItem(dict.Count);
                    }

                    // shadere
                    dict = null;
                    if ((obj != null) && (obj is BasicBlenderObject))
                    {
                        dict = ri.ShaderIndex;
                        string shd = ((BasicBlenderObject)obj).ShaderName;
                        if ((shd != null) && (shd.Length > 0))
                        {
                            if (dict.ContainsKey(shd) == false)
                                dict[shd] = new ResourceAnimationItem(dict.Count);
                        }
                    }
                }
            }
            // pun si numele - pentru imagini
            foreach (ResourceKeyName rti in ImageTemplateNames)
                if (ri.ImageIndex.ContainsKey(rti.ResourceName))
                    ri.ImageIndex[rti.ResourceName].Name = rti.KeyName;
            // pun si numele - pentru Fonturi
            foreach (ResourceKeyName rti in FontTemplateNames)
                if (ri.FontsIndex.ContainsKey(rti.ResourceName))
                    ri.FontsIndex[rti.ResourceName].Name = rti.KeyName;
            // pun si numele - pentru Stringuri
            foreach (ResourceKeyName rti in StringTemplateNames)
                if (ri.StringIndex.ContainsKey(rti.ResourceName))
                    ri.StringIndex[rti.ResourceName].Name = rti.KeyName;
            // pun si numele - pentru Shadere
            foreach (ResourceKeyName rti in ShaderTemplateNames)
                if (ri.ShaderIndex.ContainsKey(rti.ResourceName))
                    ri.ShaderIndex[rti.ResourceName].Name = rti.KeyName;
            // pun si numele - pentru Sound
            foreach (ResourceKeyName rti in SoundTemplateNames)
                if (ri.SoundsIndex.ContainsKey(rti.ResourceName))
                    ri.SoundsIndex[rti.ResourceName].Name = rti.KeyName;

            return ri;
        }
        private int FindClipElement(List<PresentationObject> originalList,int index)
        {
            for (int tr = index + 1; tr < originalList.Count; tr++)
                if (originalList[tr] is ClipPresentationObject)
                    return tr;
            return -1;
        }
        private List<PresentationObject> ClipReorder(List<PresentationObject> originalList,List<PresentationObject> newList)
        {
            // verific daca am un clip
            int index = FindClipElement(originalList, -1);
            if (index < 0)
                return originalList;
            newList.Clear();
            ClearClipAnimationObject cclip = new ClearClipAnimationObject();
            int cPoz = 0;
            do
            {
                newList.Add(originalList[index]);
                for (int tr = cPoz; tr < index; tr++)
                    newList.Add(originalList[tr]);
                newList.Add(cclip);
                cPoz = index + 1;
                index = FindClipElement(originalList, index);
            } while (index >= 0);
            for (int tr = cPoz; tr < originalList.Count; tr++)
                newList.Add(originalList[tr]);
            return newList;
        }
        public bool ExportToBinaryFormat(string fileName, ErrorsContainer ec)
        {
            ec.SetDefaultModule("Presentation::Export(" + _FileName + ")");
            ByteBuffer bb = new ByteBuffer(ec);
            ByteBuffer bbIndexFrame = new ByteBuffer(ec);
            ByteBuffer bbHead = new ByteBuffer(ec);
            ByteBuffer bbObj = new ByteBuffer(ec);
            ByteBuffer bbtmp = new ByteBuffer(ec);
            Dictionary<string, PresentationObject> objectsHashes = new Dictionary<string, PresentationObject>();
            Dictionary<string, int> objectsPoz = new Dictionary<string, int>();
            Dictionary<string, int> objectsCount = new Dictionary<string, int>();
            Dictionary<string, bool> stringValueNames = new Dictionary<string, bool>();
            FileStream fs = null;
            GRect tempRect = new GRect();
            List<PresentationObject> tmpObjList = new List<PresentationObject>();
            
            ByteBuffer bb_ccao = new ByteBuffer(ec);
            ClearClipAnimationObject ccao = new ClearClipAnimationObject();
            ccao.AddBinaryBuffer(bb_ccao, null, null, null);
            string ClearClipMD5 = bb_ccao.ComputeMD5();

            ResourcesIndexes ri = CreateResourcesIndexes();
            try
            {
                foreach (PresentationFrame frm in Frames)
                {
                    foreach (PresentationObject obj in frm.Objects)
                    {
                        bb.Clear();                        
                        obj.ComputeRect(tempRect);
                        obj.AddBinaryBuffer(bb, ri, tempRect,Resources);
                        if (obj is BasicBlenderObject)
                        {
                            if (((BasicBlenderObject)obj).CheckShaderValidity(Resources) == false)
                                ec.AddError(String.Format("Shader '{0}' in frame {1}, object {2} does not exist !", ((BasicBlenderObject)obj).ShaderName, obj.GetName(), Frames.IndexOf(frm)));
                        }
                        string md5 = bb.ComputeMD5();
                        if (objectsHashes.ContainsKey(md5) == false)
                            objectsHashes[md5] = obj;
                        if (objectsCount.ContainsKey(md5) == false)
                            objectsCount[md5] = 1;
                        else
                            objectsCount[md5] += 1;
                        if (bb.Count() == 0)
                        {
                            ec.AddError(String.Format("Object '{0}' in frame {1} can not be converted into binary mode !", obj.GetName(), Frames.IndexOf(frm)));
                        }
                        if (obj is ClipPresentationObject)
                        {
                            if (objectsHashes.ContainsKey(ClearClipMD5) == false)
                                objectsHashes[ClearClipMD5] = ccao;
                            if (objectsCount.ContainsKey(ClearClipMD5) == false)
                                objectsCount[ClearClipMD5] = 1;
                            else
                                objectsCount[ClearClipMD5] += 1;
                        }
                    }
                }
                if (objectsHashes.Count > 0xFFFF)                
                    ec.AddError("Too many different objects (more than 0xFFFF objects !)");
                if (objectsHashes.Count == 0)
                    ec.AddError("No objects defined or no frames !");
                if (DesignWidth<=0)
                    ec.AddError("Invalid 'Design Width' property. Should be bigger than 0 !");
                if (DesignHeight<=0)
                    ec.AddError("Invalid 'Design Height' property. Should be bigger than 0 !");
                if (Width<=0)
                    ec.AddError("Invalid 'Width' property. Should be bigger than 0 !");
                if (Height<=0)
                    ec.AddError("Invalid 'Height' property. Should be bigger than 0 !");
                if (Frames.Count>0xFFFF)
                    ec.AddError("Too many frames (more than 0xFFFF frames !)");
                // verific sa nu am vreun frame cu timp 0
                foreach (PresentationFrame frm in Frames)
                {
                    if (frm.Time<1)
                        ec.AddError(String.Format("Frame {0} has a invalid 'Time' property. It should be bigger than 0",Frames.IndexOf(frm)));
                    if (frm.Time > 0x7FFF)
                        ec.AddError(String.Format("Frame {0} has a invalid 'Time' property. It should be smaller than 0x7FFF", Frames.IndexOf(frm)));
                    if (frm.Objects.Count>0xFFFF)
                        ec.AddError(String.Format("Frame {0} has too many objects ({1}) ! (max allowed is 0xFFFF)", Frames.IndexOf(frm),frm.Objects.Count));
                }
                if (ec.HasErrors())
                    return false;
            // all good - scriu datele
                    
                bbHead.Clear();
                bbHead.AddString("GANI", false);
                bbHead.AddByte(6); // versiunea
                bbHead.AddWord(Width);
                bbHead.AddWord(Height);
                bbHead.AddWord(DesignWidth);
                bbHead.AddWord(DesignHeight);
                bbHead.AddWord(Frames.Count);                
                // adaug obiectele
                bbObj.Clear();
                // sortare dupa count
                HashCountItem[] objItems = new HashCountItem[objectsHashes.Count];
                HashCountItem temp = new HashCountItem();
                int index = 0;
                foreach (string hash in objectsCount.Keys)
                {
                    objItems[index] = new HashCountItem();
                    objItems[index].count = objectsCount[hash];
                    objItems[index].md5 = hash;
                    index++;
                }
                // sortare
                bool sorted = false;
                int newLast = objItems.Length - 1;
                while (sorted == false)
                {
                    int last = newLast;
                    sorted = true;
                    for (int tr = 0; tr < last; tr++)
                    {
                        if (objItems[tr].count < objItems[tr + 1].count)
                        {
                            temp = objItems[tr];
                            objItems[tr] = objItems[tr + 1];
                            objItems[tr + 1] = temp;
                            newLast = tr;
                            sorted = false;
                        }
                    }
                }
                // l-am sortat - creez ID-urile
                for (int tr=0;tr<objItems.Length;tr++)
                {
                    objectsPoz[objItems[tr].md5] = bbObj.Count();
                    objectsHashes[objItems[tr].md5].ComputeRect(tempRect);
                    objectsHashes[objItems[tr].md5].AddBinaryBuffer(bbObj, ri, tempRect,Resources);
                }
                bb.Clear();
                bbIndexFrame.Clear();
                // adaug frame-urile
                int bookmarkIndex = 0;
                foreach (PresentationFrame frm in Frames)
                {
                    bbIndexFrame.Add24BitUint(bb.Count()+Frames.Count*3+bbHead.Count()+3);
                    bb.AddCompactValue(frm.Time);
                    if (frm.Bookmark)
                    {
                        bookmarkIndex++;
                        bb.AddCompactValue(bookmarkIndex);
                    }
                    else
                    {
                        bb.AddCompactValue(0);
                    }                    
                    // adaug si ID-urile frameurilor
                    List<PresentationObject> frameObjects = ClipReorder(frm.Objects, tmpObjList);
                    bb.AddCompactValue(frameObjects.Count);
                    foreach (PresentationObject obj in frameObjects)
                    {
                        bbtmp.Clear();
                        obj.ComputeRect(tempRect);
                        obj.AddBinaryBuffer(bbtmp,ri, tempRect,Resources);
                        string md5 = bbtmp.ComputeMD5();
                        bb.AddCompactValue(objectsPoz[md5]);
                    }
                }
                // ultimele informatii din hedere
                bbHead.Add24BitUint(bbHead.Count() + 3 + bb.Count()+bbIndexFrame.Count());
                // adaug si ultimele date
                if (fileName != null)
                {
                    fs = File.Create(fileName);
                    fs.Write(bbHead.GetBuffer(), 0, bbHead.Count());
                    fs.Write(bbIndexFrame.GetBuffer(), 0, bbIndexFrame.Count());
                    fs.Write(bb.GetBuffer(), 0, bb.Count());
                    fs.Write(bbObj.GetBuffer(), 0, bbObj.Count());
                    fs.Close();
                }
                // all ok
                return !ec.HasErrors();
            }
            catch (Exception e)
            {
                ec.AddException("Unable to save '" + fileName + "'", e);
                return false;
            }            
        }

    }
}