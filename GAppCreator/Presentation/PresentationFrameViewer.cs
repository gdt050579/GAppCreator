using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace GAppCreator
{
    public partial class PresentationFrameViewer : UserControl,IPresentationFrameViewer
    {
        public enum SelectionMode
        {
            Select,
            Rectangle,
            Line,
            Text,
            Image,
            Clipping,
        };
        public enum RelativeTo
        {
            First,
            Last,
            Median,
            Minim,
            Maxim
        };

        Presentation Anim;
        PresentationFrame frame= null;
        List<PresentationObject> SelectedObjectsList = new List<PresentationObject>();
        List<PresentationFrame> SelectedFramesList = new List<PresentationFrame>();
        string[] ClipboardSlots = new string[10];
        System.Drawing.Bitmap screenBitmap;
        System.Drawing.Graphics screenGraphcs;
        AnimO.Canvas canvas;
        public PresentationSelection sel;        
        int startX, startY;
        int endX, endY;
        int frameIndex;
        GRect selectionRect = new GRect();
        GRect tempRect = new GRect();
        ImageList resImagesSmall, resImagesLarge;
        Project prj;
        AppResources Resources;
        ProjectContext pContext;

        public ListView ObjectsList;
        public ToolStripComboBox comboRelativeTo;
        public PropertyGrid ObjectsProperties;
        public SelectionMode mode;        
        public PropertyGrid AnimationProperties;
        public PropertyGrid FrameProperties;
        public ListView FramesList;
        public ToolStripStatusLabel LabelInfo;


        public PresentationFrameViewer(ProjectContext _pContext,AppResources r)
        {
            InitializeComponent();

            pContext = _pContext;
            prj = pContext.Prj;
            resImagesLarge = pContext.LargeIcons;
            resImagesSmall = pContext.SmallIcons;
            Resources = r;

            canvas = new AnimO.Canvas();

            sel = new PresentationSelection();
            sel.FrameViewer = this;
            mode = SelectionMode.Select;            
            sel.HideSelection();
            ClearCursorSelection();
        }

        public void Log(string s)
        {
            if (LabelInfo != null)
                LabelInfo.Text = DateTime.Now.ToLongTimeString() + " - " + s;
        }
        private void ClearCursorSelection()
        {
            startX = startY = -1;
            endX = endY = -1;
        }
        private void PresentationFrameViewer_Paint(object sender, PaintEventArgs e)
        {
            if ((frame != null) && (Anim!=null))
            {
                canvas.SetScreen(0, 0, Width, Height, 1.0f);
                canvas.ClearScreen(Anim.BackgroundColor);
                if (Anim.BackgroundImage != null)
                    canvas.FillScreen(Anim.BackgroundImage.__internal__image);
                frame.Paint(canvas,Resources);
                sel.Paint(canvas);
                if ((startX >= 0) && (startY >= 0))
                {
                    UpdateSelectionRect();
                    canvas.DrawRectWithPixelsCoordonates(selectionRect.Left, selectionRect.Top, selectionRect.Right, selectionRect.Bottom, Color.White.ToArgb(),0,0);
                }
                //if (!Focused)
                //    g.ClearScreen(GavApps.Color.ARGB(128, 0, 0, 0));
                e.Graphics.DrawImage(screenBitmap, 0, 0);
            }
        }

        #region Mouse events

        private void PresentationFrameViewer_MouseDown(object sender, MouseEventArgs e)
        {
            if (frame == null)
                return;
            ClearCursorSelection();
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (sel.HitTest(e.X, e.Y))
                {
                    Invalidate();
                    return;
                }
            }

            startX = endX = e.X;
            startY = endY = e.Y;            
            Invalidate();
        }

        private void PresentationFrameViewer_MouseMove(object sender, MouseEventArgs e)
        {
            if (frame == null)
                return;
            if (sel.IsVisible())
                SetCursor(sel.GetCursor(e.X,e.Y));
            else
                SetCursor(null);

            if (sel.HasSelection())
            {
                sel.UpdatePosition(e.X, e.Y, false, (Control.ModifierKeys & System.Windows.Forms.Keys.Shift) != System.Windows.Forms.Keys.None);
                SetCursor(sel.GetSelectedCursor());
                Invalidate();
                Update();
                return;
            }

            if ((startX >= 0) && (startY >= 0))
            {
                endX = e.X;
                endY = e.Y;
                Invalidate();
                return;
            }

            

        }

        private void PresentationFrameViewer_MouseUp(object sender, MouseEventArgs e)
        {
            if (frame == null)
                return;
            bool isClick = false;
            sel.ClearSelection();

            endX = e.X;
            endY = e.Y;
            UpdateSelectionRect();
            if ((startX >= 0) && (startY >= 0))
                isClick = (selectionRect.GetWidth() < 3) && (selectionRect.GetHeight() < 3);

            if ((isClick == false) && (startX>=0) && (startY>=0))
            {
                PresentationObject newObj = null;
                switch (mode)
                {
                    case SelectionMode.Select:
                        foreach (PresentationObject obj in frame.Objects)
                        {
                            obj.ComputeRect(tempRect);
                            if ((selectionRect.Contains(tempRect.Left, tempRect.Top)) && (selectionRect.Contains(tempRect.Right, tempRect.Bottom)))
                                SelectObject(obj, true);
                        }
                        break;
                    case SelectionMode.Rectangle: 
                        newObj = new RectanglePresentationObject(); 
                        break; 
                    case SelectionMode.Image: 
                        newObj = new ImagePresentationObject(); 
                        break;
                    case SelectionMode.Line:
                        newObj = new LinePresentationObject();
                        ((LinePresentationObject)newObj).SetLine(startX, startY, endX, endY);
                        break;
                    case SelectionMode.Text:
                        newObj = new TextPresentationObject();
                        break;
                    case SelectionMode.Clipping:
                        newObj = new ClipPresentationObject();
                        break;
                }
                if (newObj!=null)
                {
                    newObj.SetRectangle(selectionRect);
                    frame.Add(newObj);
                    UpdateObjectListFromFrameObjecs();
                    SelectObject(newObj,false);
                }
            }
            if ((isClick == true) && (startX >= 0) && (startY >= 0))
            {
                PresentationObject obj = frame.HitTest(e.X, e.Y,e.Button != System.Windows.Forms.MouseButtons.Left);
                if (obj != null)
                {
                    SelectObject(obj, (Control.ModifierKeys & System.Windows.Forms.Keys.Shift) != System.Windows.Forms.Keys.None);
                    Invalidate();
                    Update();
                }
                else
                {
                    // am facut click in afara unei zone - curat toate obiectele
                    ClearSelectedObjects();
                }
                
                UpdateSelectionFromObjectList();                
            }


            ClearCursorSelection();
            Invalidate();
        }

        #endregion


        private void SetCursor(Cursor c)
        {
            if (c == null)
                this.Cursor = Cursors.Arrow;
            else
                this.Cursor = c;
        }

        #region Animation Functions
        public void PlayAnimation()
        {
            PresentationPlayerDialog dlg = new PresentationPlayerDialog(Anim,Resources);
            dlg.ShowDialog();
        }
        private void UpdateAnim()
        {
            int tmp = frameIndex;
            Anim.UpdateFrameIcons();
            UpdateFrameList();
            if (tmp >= 0)
                FramesList.Items[tmp].Selected = true;
            SetFrame(tmp);
            this.Update();
            this.Invalidate();
        }
        public void SetBackgroundColor(int color)
        {
            if (Anim != null)
            {
                Anim.BackgroundColor = color;
                UpdateAnim();
            }
        }
        public void SetBackgroundImage(GBitmap img)
        {
            if (Anim != null)
            {
                Anim.BackgroundImage = img;
                UpdateAnim();
            }
        }
        public void SetLanguage(Language lang)
        {
            /*
            if (Anim != null)
            {
                gb.UpdateResourcesContext(gb._designWidth, gb._designHeight, lang);
                UpdateAnim();
            }
             */ 
        }
        public void SaveToXML()
        {
            if (Anim != null)
            {
                Anim.SaveToXML();
                Log("Save animation to XML");
            }
        }
        public void UpdateFrameSize()
        {
            screenBitmap = new System.Drawing.Bitmap(Anim.Width, Anim.Height);
            screenGraphcs = System.Drawing.Graphics.FromImage(screenBitmap);
            screenGraphcs.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            screenGraphcs.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            canvas.SetGraphics(screenGraphcs);
            Width = Anim.Width;
            Height = Anim.Height;
        }
        public void UpdateFrameList()
        {
            FramesList.LargeImageList.Images.Clear();
            FramesList.Items.Clear();
            int index = 0;
            foreach (PresentationFrame frame in Anim.Frames)
            {
                FramesList.LargeImageList.Images.Add(frame.Icon.__internal__image);
                ListViewItem lvi = new ListViewItem((index + 1).ToString());
                lvi.Tag = frame;
                lvi.ImageIndex = (index);
                FramesList.Items.Add(lvi);
                index++;
            }
        }
        public void OnUpdateAnimationSize()
        {
            UpdateFrameSize();

            // refac toate iconitele
            for (int tr = 0; tr < Anim.Frames.Count; tr++)
                FramesList.LargeImageList.Images[tr] = Anim.Frames[tr].Icon.__internal__image;
            FramesList.Update();
            FramesList.Invalidate();

        }
        public int GetCurrentFrameIndex()
        {
            if ((FramesList.SelectedIndices == null) || (FramesList.SelectedIndices.Count != 1))
                return -1;
            return FramesList.SelectedIndices[0];
        }
        public void SetAnimation(Presentation animObject)
        {
            SetFrame(-1);
            Anim = animObject;
            Anim.animViewer = this;            
            UpdateFrameList();
            UpdateFrameSize();
            Anim.UpdateFrameIcons();
            AnimationProperties.SelectedObject = Anim;
    
        }
        #endregion

        #region Frame Functions
        public void SetFrame(int newFrameIndex)
        {
            PresentationFrame newFrame = null;
            if (newFrameIndex >= 0)
                newFrame = Anim.Frames[newFrameIndex];
            
            if (frame != null)
            {
                ClearSelectedObjects();
                UpdateFrameIcon(frameIndex);
            }
            ObjectsProperties.SelectedObjects = null;
            ObjectsProperties.Update();
            ObjectsList.Items.Clear();

            frame = newFrame;
            frameIndex = newFrameIndex;
            Enabled = (frame != null);
            Visible = (frame != null);
            ClearCursorSelection();
            sel.HideSelection();
            if (frame != null)
                UpdateObjectListFromFrameObjecs();
            UpdateSelectionFromObjectList();
            
            // updatez si frame-urile
            if (FramesList.SelectedItems.Count == 0)
            {                
                FrameProperties.SelectedObjects = null;
            }
            else
            {
                PresentationFrame[] aframes = new PresentationFrame[FramesList.SelectedItems.Count];
                for (int tr = 0; tr < FramesList.SelectedItems.Count; tr++)
                    aframes[tr] = (PresentationFrame)FramesList.SelectedItems[tr].Tag;
                FrameProperties.SelectedObjects = aframes;
            }
            if (frameIndex >= 0)
                FramesList.Items[frameIndex].EnsureVisible();
            this.Update();
            this.Invalidate();
        }
        private void InsertNewFrame(PresentationFrame af)
        {
            int index = GetCurrentFrameIndex();  
            Anim.UpdateFrameIcon(af);
            if (index < 0)
            {
                Anim.Frames.Add(af);
                index = Anim.Frames.Count - 1;
            }
            else
            {
                index++;
                Anim.Frames.Insert(index, af);
            }
            UpdateFrameList();
            AnimationProperties.Update();
            AnimationProperties.Invalidate();

            // selectez acel frame
            FramesList.Items[index].Selected = true;
            FramesList.Items[index].EnsureVisible();
            FramesList.Update();
            FramesList.Invalidate();
        }
        public void AddNewFrame()
        {
            InsertNewFrame(new PresentationFrame());
        }
        public void DuplicateCurentFrame()
        {
            if ((frame == null) || (Anim==null))
            {
                MessageBox.Show("You have to select one frame to duplicate it !");
                return;
            }
            string res = frame.ToXMLString();
            if (res.Length == 0)
                return;
            PresentationFrame newFrame = PresentationFrame.FromXMLString(res);
            if (newFrame == null)
                return;
            InsertNewFrame(newFrame);            
        }
        public void DeleteSelectedFrames()
        {
            if ((FramesList != null) && (FramesList.SelectedItems != null) && (FramesList.SelectedItems.Count > 0))
            {
                if (MessageBox.Show("Are you sure do you want to delete " + FramesList.SelectedItems.Count.ToString() + " frames ?", "Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    foreach (ListViewItem item in FramesList.SelectedItems)
                    {
                        Anim.Frames.Remove((PresentationFrame)item.Tag);
                    }
                    SetFrame(-1);
                    UpdateFrameList();
                    FrameProperties.SelectedObjects = null;
                    FrameProperties.SelectedObject = null;
                }
            }
            else
            {
                MessageBox.Show("No frame selected !", "Delete");
            }
        }
        public void OnNewFrameSelected()
        {
            int index = GetCurrentFrameIndex();
            if (index < 0)
                SetFrame(-1);
            else
                SetFrame(index);
        }
        public void UpdateFrameIcon(int index)
        {
            if ((index >= 0) && (index<Anim.Frames.Count))
            {
                Anim.UpdateFrameIcon(Anim.Frames[index]);
                FramesList.LargeImageList.Images[index] = Anim.Frames[index].Icon.__internal__image;
                FramesList.Invalidate();
                FramesList.Update();
            }
        }
        public void UpdateCurentFrameIcon()
        {
            UpdateFrameIcon(GetCurrentFrameIndex());
            FrameProperties.Update();
            FrameProperties.Invalidate();
        }
        public void UpdateSelectedFramesIcon()
        {
            for (int tr = 0; tr < FramesList.Items.Count; tr++)
            {
                if (FramesList.Items[tr].Selected)
                    UpdateFrameIcon(tr);
            }
        }
        #endregion

        #region Alignament functions

        public void AlignObjects(AlignComponent ac, RelativeTo rel)
        {
            ComputeSelectedObjectsList();
            if (SelectedObjectsList.Count == 0)
                return;
            // template part
            int value = SelectedObjectsList[0].GetValue(ac,tempRect);
            int index = SelectedObjectsList[0].SelectionIndex;
            int sum = 0;
            foreach (PresentationObject obj in SelectedObjectsList)
            {
                int c_val = obj.GetValue(ac,tempRect);
                switch (rel)
                {
                    case RelativeTo.First:
                        if (obj.SelectionIndex < index)
                        {
                            index = obj.SelectionIndex;
                            value = c_val;
                        }
                        break;
                    case RelativeTo.Last:
                        if (obj.SelectionIndex > index)
                        {
                            index = obj.SelectionIndex;
                            value = c_val;
                        }
                        break;
                    case RelativeTo.Median:
                        sum += c_val;
                        break;
                    case RelativeTo.Minim:
                        if (c_val < value)
                            value = c_val;
                        break;
                    case RelativeTo.Maxim:
                        if (c_val > value)
                            value = c_val;
                        break;
                }
            }
            // setez valorile
            if (rel == RelativeTo.Median)
                value = sum / SelectedObjectsList.Count;
            foreach (PresentationObject obj in SelectedObjectsList)
            {
                obj.SetValue(ac, value,tempRect);
            }
            UpdateSelectionFromObjectList();
        }
        #endregion

        #region Animation Objects
        private void UpdateSelectionRect()
        {
            selectionRect.Left = startX;
            if (endX < startX)
                selectionRect.Left = endX;
            selectionRect.Right = endX;
            if (endX < startX)
                selectionRect.Right = startX;
            selectionRect.Top = startY;
            if (endY<startY)
                selectionRect.Top = endY;
            selectionRect.Bottom = endY;
            if (endY < startY)
                selectionRect.Bottom = startY;
        }
        public void ClearSelectedObjects()
        {
            sel.HideSelection();
            foreach (ListViewItem item in ObjectsList.Items)
            {
                item.Selected = false;
                PresentationObject obj = (PresentationObject)item.Tag;
                if (obj != null)
                {
                    if (obj.SelectionIndex >= 0)
                    {
                        obj.SelectionIndex = -1;
                        item.SubItems[1].Text = "-";
                    }
                }
            }
        }
        public void SelectObject(PresentationObject obj, bool AddToExistingSelection)
        {
            if (!AddToExistingSelection)
                ClearSelectedObjects();
            ListViewItem itm = AnimationObjectToItem(obj);
            if ((itm != null) && (obj!=null))
            {
                itm.Selected = true;
                itm.EnsureVisible();
            }
        }
        public void UpdateSelectionFromObjectList()
        {
            if ((ObjectsList.SelectedItems==null) || (ObjectsList.SelectedItems.Count==0))
            {
                ClearSelectedObjects();
                sel.HideSelection();
                ObjectsList.Update();
                ObjectsProperties.SelectedObjects = null;
                ObjectsProperties.Update();
                this.Update();
                this.Invalidate();
                return;
            }
            int minX = 0, maxX = 0, minY = 0, maxY = 0;
            bool firstTime = true;
            object[] selected = new object[ObjectsList.SelectedItems.Count];
            int index = 0;
            int sindex = ObjectsList.SelectedItems.Count - 1;
            foreach (ListViewItem Item in ObjectsList.Items)
            {
                PresentationObject obj = (PresentationObject)Item.Tag;
                if (obj==null)
                    continue;
                if (Item.Selected)
                {
                    if (obj.SelectionIndex < 0)
                    {
                        obj.SelectionIndex = sindex;
                        sindex++;
                    }
                    selected[index++] = obj;
                    Item.SubItems[2].Text = obj.GetName();
                    Item.SubItems[1].Text = (obj.SelectionIndex + 1).ToString();
                    obj.ComputeRect(tempRect);
                    if (firstTime)
                    {
                        minX = tempRect.Left;
                        minY = tempRect.Top;
                        maxX = tempRect.Right;
                        maxY = tempRect.Bottom;
                        firstTime = false;
                    }
                    // updatez
                    if (tempRect.Left < minX)
                        minX = tempRect.Left;
                    if (tempRect.Top < minY)
                        minY = tempRect.Top;
                    if (tempRect.Right > maxX)
                        maxX = tempRect.Right;
                    if (tempRect.Bottom > maxY)
                        maxY = tempRect.Bottom;
                }
                else
                {
                    if (obj.SelectionIndex >= 0)
                    {
                        obj.SelectionIndex = -1;
                        Item.SubItems[1].Text = "-";
                    }
                }
            }
            sel.SetSelection(minX, minY, maxX, maxY);
            ObjectsProperties.SelectedObjects = selected;
            ObjectsProperties.Update();
            ObjectsList.Update();
            UpdateCurentFrameIcon();
            this.Update();
            Invalidate();
            // updatez si iconita curenta
            
        }
        public void UpdateObjectListFromFrameObjecs()
        {
            ObjectsList.Items.Clear();
            // sunt puse invers ca sa facem ZOrder
            for (int tr = frame.Objects.Count - 1; tr >= 0; tr--)
            {
                ListViewItem itm = new ListViewItem("   "+(ObjectsList.Items.Count + 1).ToString());
                if (frame.Objects[tr].SelectionIndex >= 0)
                    itm.SubItems.Add((frame.Objects[tr].SelectionIndex + 1).ToString());
                else
                    itm.SubItems.Add("-");
                itm.SubItems.Add(frame.Objects[tr].GetName());
                itm.Tag = frame.Objects[tr];
                itm.ImageKey = frame.Objects[tr].GetIconName();
                ObjectsList.Items.Add(itm);
            }
        }
        public void UpdateSelectedObjectsPositions(GRect oldRect,GRect newRect)
        {
            float rapWidth=1,rapHeight=1;

            if ((oldRect.GetWidth() != 0) && (oldRect.GetHeight() != 0) && (newRect.GetHeight() != 0) && (newRect.GetWidth() != 0))
            {
                rapWidth = (float)newRect.GetWidth() / (float)oldRect.GetWidth();
                rapHeight = (float)newRect.GetHeight() / (float)oldRect.GetHeight();
            }
            if (ObjectsList.SelectedItems.Count <= 1)
            {
                PresentationObject obj = (PresentationObject)ObjectsList.SelectedItems[0].Tag;
                obj.SetRectangle(newRect);
            }
            else
            {
                foreach (ListViewItem Item in ObjectsList.SelectedItems)
                {
                    PresentationObject obj = (PresentationObject)Item.Tag;
                    if ((Item.Selected) && (obj != null))
                    {
                        obj.ComputeRect(tempRect);
                        int dx = tempRect.Left - oldRect.Left;
                        int dy = tempRect.Top - oldRect.Top;
                        int w = tempRect.GetWidth();
                        int h = tempRect.GetHeight();
                        tempRect.Left = (int)(dx * rapWidth) + newRect.Left;
                        tempRect.Top = (int)(dy * rapHeight) + newRect.Top;
                        tempRect.SetWidthFromLeft((int)(w * rapWidth));
                        tempRect.SetHeightFromTop((int)(h *rapHeight));
                        obj.SetRectangle(tempRect);
                    }
                }
            }
            UpdateSelectionFromObjectList();
            Invalidate();
        }
        public ListViewItem AnimationObjectToItem(PresentationObject obj)
        {
            foreach (ListViewItem item in ObjectsList.Items)
                if (item.Tag == obj)
                    return item;
            return null;
        }
        public void SetImageForObjects()
        {
            if ((ObjectsProperties.SelectedObjects != null) && (ObjectsProperties.SelectedObjects.Length > 0))
            {
                bool hasOneImage = false;
                foreach (object o in ObjectsProperties.SelectedObjects)
                {
                    if (o.GetType() == typeof(ImagePresentationObject))
                    {
                        hasOneImage = true;
                        break;
                    }
                }
                if (hasOneImage == false)
                {
                    MessageBox.Show("You have to have at least one image selected !");
                }
                else
                {

                    ResourceSelectDialog dlg = new ResourceSelectDialog(pContext, ResourcesConstantType.Image,true,false);
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        foreach (object o in ObjectsProperties.SelectedObjects)
                        {
                            if (o.GetType() == typeof(ImagePresentationObject))
                            {
                                ImagePresentationObject img = (ImagePresentationObject)o;
                                img.ImageName = dlg.SelectedResource;
                            }
                        }
                        UpdateSelectionFromObjectList();
                        ObjectsProperties.Update();
                        ObjectsProperties.Invalidate();
                    }
                }
            }
        }
        public void SetStringForObjects()
        {
            if ((ObjectsProperties.SelectedObjects != null) && (ObjectsProperties.SelectedObjects.Length > 0))
            {
                bool hasOne = false;
                foreach (object o in ObjectsProperties.SelectedObjects)
                {
                    if (o.GetType() == typeof(TextPresentationObject))
                    {
                        hasOne = true;
                        break;
                    }
                }
                if (hasOne == false)
                {
                    MessageBox.Show("You have to have at least one text objects or glyph text object selected !");
                }
                else
                {
                    ResourceSelectDialog dlg = new ResourceSelectDialog(pContext, ResourcesConstantType.String,true,false);                    
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        foreach (object o in ObjectsProperties.SelectedObjects)
                        {
                            if (o.GetType() == typeof(TextPresentationObject))
                            {
                                TextPresentationObject obj = (TextPresentationObject)o;
                                obj.StringResource = dlg.SelectedResource;
                                obj.TextMode = TextModeType.FromResources;
                            }
                        }
                        UpdateSelectionFromObjectList();
                        ObjectsProperties.Update();
                        ObjectsProperties.Invalidate();
                    }
                }
            }
        }
        public void SetFontForObjects()
        {
            if ((ObjectsProperties.SelectedObjects != null) && (ObjectsProperties.SelectedObjects.Length > 0))
            {
                bool hasOne = false;
                foreach (object o in ObjectsProperties.SelectedObjects)
                {
                    if (o.GetType() == typeof(TextPresentationObject))
                    {
                        hasOne = true;
                        break;
                    }
                }
                if (hasOne == false)
                {
                    MessageBox.Show("You have to have at least one text objects or glyph text object selected !");
                }
                else
                {
                    ResourceSelectDialog dlg = new ResourceSelectDialog(pContext, ResourcesConstantType.Font,true,false);                    
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        foreach (object o in ObjectsProperties.SelectedObjects)
                        {
                            if (o.GetType() == typeof(TextPresentationObject))
                            {
                                TextPresentationObject obj = (TextPresentationObject)o;
                                obj.Font = dlg.SelectedResource;
                            }
                        }
                        UpdateSelectionFromObjectList();
                        ObjectsProperties.Update();
                        ObjectsProperties.Invalidate();
                    }
                }
            }
        }

        public void SetShaderForObjects()
        {
            if ((ObjectsProperties.SelectedObjects != null) && (ObjectsProperties.SelectedObjects.Length > 0))
            {
                ResourceSelectDialog dlg = new ResourceSelectDialog(pContext, ResourcesConstantType.Shader,true,false);
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    foreach (object o in ObjectsProperties.SelectedObjects)
                    {
                        if (o is BasicBlenderObject)
                            ((BasicBlenderObject)o).ShaderName = dlg.SelectedResource;
                    }
                    UpdateSelectionFromObjectList();
                    ObjectsProperties.Update();
                    ObjectsProperties.Invalidate();
                }
            }
        }

        public void ResizeSelectedImagesToOriginalSize()
        {
            if ((ObjectsProperties.SelectedObjects != null) && (ObjectsProperties.SelectedObjects.Length > 0))
            {
                bool hasOneImage = false;
                foreach (object o in ObjectsProperties.SelectedObjects)
                {
                    if (o.GetType() == typeof(ImagePresentationObject))
                    {
                        hasOneImage = true;
                        break;
                    }
                }
                if (hasOneImage == false)
                {
                    MessageBox.Show("You have to have at least one image selected !");
                }
                else
                {
                    foreach (object o in ObjectsProperties.SelectedObjects)
                    {
                        if (o.GetType() == typeof(ImagePresentationObject))
                        {
                            ImagePresentationObject img = (ImagePresentationObject)o;
                            if (Resources.Images.ContainsKey(img.ImageName))
                            {
                                if (Resources.Images[img.ImageName].Picture == null)
                                {
                                    MessageBox.Show("Image " + img.ImageName + " is not loaded !");
                                }
                                else
                                {
                                    img.Width = Resources.Images[img.ImageName].Picture.Width;
                                    img.Height = Resources.Images[img.ImageName].Picture.Height;
                                }
                            }
                        }
                    }
                    UpdateSelectionFromObjectList();
                    ObjectsProperties.Update();
                    ObjectsProperties.Invalidate();
                }
            }
        }
        public void OnChangeLanguage()
        {
            if (frame != null)
            {
                foreach (PresentationObject aobj in frame.Objects)
                    if (aobj.GetType() == typeof(TextPresentationObject))
                    {
                        ((TextPresentationObject)aobj).Refresh();
                    }
            }
        }
        #endregion

        #region ZOrder Functions
        private void ComputeSelectedFramesList()        
        {
            SelectedFramesList.Clear();
            foreach (ListViewItem item in FramesList.Items)
                if (item.Selected)
                    SelectedFramesList.Add((PresentationFrame)item.Tag);
        }
        private void ComputeSelectedObjectsList()
        {
            SelectedObjectsList.Clear();
            foreach (ListViewItem item in ObjectsList.Items)
                if (item.Selected)
                    SelectedObjectsList.Add((PresentationObject)item.Tag);
        }
        public void MoveObjectsInZOrder(int steps)
        {
            if ((frame==null) || (steps==0))
                return;
            ComputeSelectedObjectsList();
            if (SelectedObjectsList.Count == 0)
                return;
            int index, newIndex;
            if (steps>0)
                index = frame.Objects.IndexOf(SelectedObjectsList[0]);
            else
                index = frame.Objects.IndexOf(SelectedObjectsList[SelectedObjectsList.Count - 1]);
            newIndex = index + steps;
            if (newIndex < 0)
                newIndex = 0;
            if (newIndex >= frame.Objects.Count - 1)
                newIndex = frame.Objects.Count - 1;
            if (newIndex == index)
                return;
            steps = newIndex - index;
            if (steps > 0)
            {
                foreach (PresentationObject obj in SelectedObjectsList)
                {
                    index = frame.Objects.IndexOf(obj);
                    frame.MoveZOrder(index, steps);
                }
            }
            else
            {
                for (int tr = SelectedObjectsList.Count - 1; tr >= 0; tr--)
                {
                    index = frame.Objects.IndexOf(SelectedObjectsList[tr]);
                    frame.MoveZOrder(index, steps);
                }
            }
            // am mutat ordinea - trebuie sa refac lista + selectiile
            UpdateObjectListFromFrameObjecs();
            // comanda precdenta a sters selectia
            foreach (PresentationObject obj in SelectedObjectsList)
            {
                SelectObject(obj, true);                
            }
            UpdateSelectionFromObjectList();
            ObjectsList.Update();
            ObjectsList.Invalidate();
            this.Update();
            this.Invalidate();
        }
        public void MoveFramesInZOrder(int steps)
        {
            if ((steps == 0) || (Anim==null))
                return;
            ComputeSelectedFramesList();
            if (SelectedFramesList.Count == 0)
                return;
            int index, newIndex;
            if (steps < 0)
                index = Anim.Frames.IndexOf(SelectedFramesList[0]);
            else
                index = Anim.Frames.IndexOf(SelectedFramesList[SelectedFramesList.Count - 1]);
            newIndex = index + steps;
            if (newIndex < 0)
                newIndex = 0;
            if (newIndex >= Anim.Frames.Count - 1)
                newIndex = Anim.Frames.Count - 1;
            if (newIndex == index)
                return;
            steps = newIndex - index;
            if (steps > 0)
            {
                foreach (PresentationFrame frm in SelectedFramesList)
                {
                    index = Anim.Frames.IndexOf(frm);
                    Anim.MoveZOrder(index, steps);
                }
            }
            else
            {
                for (int tr = SelectedFramesList.Count - 1; tr >= 0; tr--)
                {
                    index = Anim.Frames.IndexOf(SelectedFramesList[tr]);
                    Anim.MoveZOrder(index, steps);
                }
            }
            UpdateFrameList();
            // comanda precdenta a sters selectia
            foreach (PresentationFrame frm in SelectedFramesList)
            {
                index = Anim.Frames.IndexOf(frm);
                FramesList.Items[index].Selected = true;                
            }
            /*
            UpdateSelectionFromObjectList();
            ObjectsList.Update();
            ObjectsList.Invalidate();
            this.Update();
            this.Invalidate();
             */
        }
        #endregion

        #region Delete Stuff
        public void DeleteSelected()
        {
            if ((frame == null) || (sel.IsVisible()==false))
            {
                MessageBox.Show("No objects to delete !","Delete");
                return;
            }
            ComputeSelectedObjectsList();
            if (SelectedObjectsList.Count == 0)
            {
                MessageBox.Show("No objects to delete !","Delete");
                return;
            }
            if (MessageBox.Show("Are you sure do you want to delete " + SelectedObjectsList.Count.ToString() + " objects ?", "Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                foreach (PresentationObject obj in SelectedObjectsList)
                    frame.Objects.Remove(obj);
                UpdateObjectListFromFrameObjecs();
                UpdateSelectionFromObjectList();
                this.Update();
                this.Invalidate();
            }
        }
        #endregion

        #region Keyboard

        private void AnimationFrameViewer_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (sel.IsVisible())
            {
                int pas = 10;
                if (e.Alt)
                    pas = 1;
                switch (e.KeyCode)
                {
                    case System.Windows.Forms.Keys.Left: sel.UpdatePosition(-pas,0,true,e.Shift); break;
                    case System.Windows.Forms.Keys.Right: sel.UpdatePosition(pas, 0, true, e.Shift); break;
                    case System.Windows.Forms.Keys.Up: sel.UpdatePosition(0, -pas, true, e.Shift); break;
                    case System.Windows.Forms.Keys.Down: sel.UpdatePosition(0, pas, true, e.Shift); break;
                }

                SetCursor(sel.GetSelectedCursor());
                Invalidate();
                Update();
                return;
            }
        }

        private void AnimationFrameViewer_KeyUp(object sender, KeyEventArgs e)
        {

        }

        #endregion

        #region Copy-Paste
        private string GetSelectedObjectsXMLString()
        {
            ComputeSelectedObjectsList();
            if (SelectedObjectsList.Count > 0)
            {
                string res = "OBJECTS:";
                foreach (PresentationObject obj in SelectedObjectsList)
                {
                    string xml = obj.ToXMLString();
                    if (xml.Length == 0)
                        return "";
                    res += xml + "<GANIMARKER/>";
                }
                return res;
            }
            return "";
        }
        public void CopyObjectsToClipboard(int clipboardID)
        {
            string res = GetSelectedObjectsXMLString();
            if (res.Length>0)
            {
                if (clipboardID == -1)
                {
                    Clipboard.SetText(res);
                    Log(String.Format("Copy {0} objects to clipboard", SelectedObjectsList.Count));
                }
                else
                {
                    if (clipboardID < 10)
                    {
                        ClipboardSlots[clipboardID] = res;
                        Log(String.Format("Copy {0} objects to local clipboard {1}", SelectedObjectsList.Count, clipboardID));
                    }
                }
            }            
        }
        private string GetSelectedFramesXMLString()
        {
            ComputeSelectedFramesList();
            if (SelectedFramesList.Count > 0)
            {
                string res = "FRAMES:";
                foreach (PresentationFrame obj in SelectedFramesList)
                {
                    string xml = obj.ToXMLString();
                    if (xml.Length == 0)
                        return "";
                    res += xml + "<GANIMARKER/>";
                }
                return res;
            }
            return "";
        }
        public void CopyFramesToClipboard(int clipboardID)
        {
            string res = GetSelectedFramesXMLString();
            if (res.Length > 0)
            {
                if (clipboardID == -1)
                {
                    Clipboard.SetText(res);
                    Log(String.Format("Copy {0} frames to clipboard",SelectedFramesList.Count));
                }
                else
                {
                    if (clipboardID < 10)
                    {
                        ClipboardSlots[clipboardID] = res;
                        Log(String.Format("Copy {0} frames to local clipboard {1}", SelectedFramesList.Count,clipboardID));
                    }
                }
            }
        }
        public void PasteFromClipboard(int clipboardID)
        {
            string res = null;
            if (clipboardID == -1)
                res = Clipboard.GetText();
            else if (clipboardID<10)
                res = ClipboardSlots[clipboardID];
            if ((res == null) || ((res.StartsWith("OBJECTS:") == false) && (res.StartsWith("FRAMES:") == false)))
            {
                MessageBox.Show("Nothing to paste !\nNo valid string found in clipboard");
                return;
            }
            bool isobj = res.StartsWith("OBJECTS:");
            res = res.Substring(res.IndexOf(':') + 1);
            string[] xmls = res.Split(new string[] {"<GANIMARKER/>"}, StringSplitOptions.None);
            if (isobj)
                ClearSelectedObjects();
            foreach (string xml in xmls)
            {
                string newXml = xml.Trim();
                if (newXml.Length == 0)
                    continue;
                if (isobj)
                {
                    PresentationObject obj = null;
                    Type objType = null;
                    if (newXml.Contains("<Rectangle"))
                        objType = typeof(RectanglePresentationObject);
                    if (newXml.Contains("<Image"))
                        objType = typeof(ImagePresentationObject);
                    if (newXml.Contains("<Line"))
                        objType = typeof(LinePresentationObject);
                    if (newXml.Contains("<Text"))
                        objType = typeof(TextPresentationObject);

                    if (objType == null)
                    {
                        MessageBox.Show("Unknonw AnimationObject type\n" + newXml);
                        return;
                    }
                    obj = PresentationObject.FromXMLString(newXml,objType);
                    if (obj == null)
                        return;
                    frame.Add(obj);
                    UpdateObjectListFromFrameObjecs();
                    SelectObject(obj, true);
                }
                else
                {
                    PresentationFrame newFrame = PresentationFrame.FromXMLString(newXml);
                    if (newFrame == null)
                        return;
                    InsertNewFrame(newFrame);
                }
            }
        }
        #endregion
    }
}
