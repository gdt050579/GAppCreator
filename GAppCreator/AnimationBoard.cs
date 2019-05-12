using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GAppCreator
{
    public partial class AnimationBoard : UserControl
    {

        #region Selection Objects
        public enum SelectionRectType
        {
            Resize,
            MoveObject,
        };

        public class SelectionRect
        {
            public float X, Y;
            public float Width, Height;
            public Alignament Align;
            public SelectionRectType Type;
            public bool Selected;

            public SelectionRect()
            {
                Create(0, 0, 1, 1, Alignament.TopLeft, SelectionRectType.Resize);
            }
            public SelectionRect(float _x, float _y, float _w, float _h, Alignament _a, SelectionRectType _t)
            {
                Create(_x, _y, _w, _h, _a, _t);
            }
            public void Create(float _x, float _y, float _w, float _h, Alignament _a, SelectionRectType _t)
            {
                X = _x;
                Y = _y;
                Width = _w;
                Height = _h;
                Type = _t;
                Align = _a;
                Selected = false;
            }
            public static float GetLeft(Alignament Align, float X, float Width)
            {
                switch (Align)
                {
                    case Alignament.TopLeft:
                    case Alignament.LeftCenter:
                    case Alignament.BottomLeft:
                        return X;
                    case Alignament.Center:
                    case Alignament.TopCenter:
                    case Alignament.BottomCenter:
                        return X - Width / 2;
                    case Alignament.BottomRight:
                    case Alignament.TopRight:
                    case Alignament.RightCenter:
                        return X - Width;
                }
                return X;
            }
            public float GetLeft()
            {
                return SelectionRect.GetLeft(Align, X, Width);
            }
            public static float GetTop(Alignament Align, float Y, float Height)
            {
                switch (Align)
                {
                    case Alignament.TopLeft:
                    case Alignament.TopCenter:
                    case Alignament.TopRight:
                        return Y;
                    case Alignament.LeftCenter:
                    case Alignament.Center:
                    case Alignament.RightCenter:
                        return Y - Height / 2;
                    case Alignament.BottomRight:
                    case Alignament.BottomLeft:
                    case Alignament.BottomCenter:
                        return Y - Height;
                }
                return Y;
            }
            public float GetTop()
            {
                return SelectionRect.GetTop(Align, Y, Height);
            }
            public bool IsPointInsideSelectionRect(float xPoz, float yPoz)
            {
                float l = GetLeft();
                float t = GetTop();
                return ((xPoz >= l) && (yPoz >= t) && (xPoz < (l + Width)) && (yPoz < (t + Height)));                
            }
            public void Paint(Graphics g, float previewSize, float x_board, float y_board)
            {
                float xx = x_board + X * previewSize / 100.0f;
                float yy = y_board + Y * previewSize / 100.0f;
                float w = Width * previewSize / 100.0f;
                float h = Height * previewSize / 100.0f;

                if (Type == SelectionRectType.Resize)
                {
                    w = Width;
                    h = Height;
                }

                float l = GetLeft(Align, xx, w);
                float t = GetTop(Align, yy, h);

                switch (Type)
                {
                    case SelectionRectType.MoveObject:
                        if (Selected)
                            g.DrawRectangle(Pens.LightGreen, l, t, w, h);
                        else
                            g.DrawRectangle(Pens.Yellow, l, t, w, h);
                        g.DrawRectangle(Pens.Black, l - 1, t - 1, w + 2, h + 2);
                        break;
                    case SelectionRectType.Resize:
                        if (Selected)
                        {
                            g.FillRectangle(Brushes.LightGreen, l, t, w, h);
                            g.DrawRectangle(Pens.Black, l, t, w, h);
                        }
                        else
                        {
                            g.FillRectangle(Brushes.White, l, t, w, h);
                            g.DrawRectangle(Pens.Black, l, t, w, h);
                        }
                        break;
                }

            }


            private void ComputeX(float left,float right)
            {
                switch (Align)
                {
                    case Alignament.TopLeft:
                    case Alignament.BottomLeft:
                    case Alignament.LeftCenter:
                        X = left;
                        break;
                    case Alignament.BottomRight:
                    case Alignament.RightCenter:
                    case Alignament.TopRight:
                        X = right;
                        break;
                    default:
                        X = (left + right) / 2;
                        break;
                }

                Width = right - left;
            }
            private void ComputeY(float top, float bottom)
            {
                switch (Align)
                {
                    case Alignament.TopLeft:
                    case Alignament.TopRight:
                    case Alignament.TopCenter:
                        Y = top;
                        break;
                    case Alignament.BottomRight:
                    case Alignament.BottomLeft:
                    case Alignament.BottomCenter:
                        Y = bottom;
                        break;
                    default:
                        Y = (top + bottom) / 2;
                        break;
                }
                Height = bottom - top;
            }
            public void SetLeft(float value)
            {
                float l = GetLeft();
                float r = l + Width;
                if (value > r - 1)
                    value = r - 1;
                ComputeX(value, r);
            }
            public void SetRight(float value)
            {
                float l = GetLeft();
                float r = l + Width;
                if (value < l + 1)
                    value = l + 1;
                ComputeX(l, value);
            }
            public void SetTop(float value)
            {
                float t = GetTop();
                float b = t + Height;
                if (value > b - 1)
                    value = b - 1;
                ComputeY(value, b);
            }
            public void SetBottom(float value)
            {
                float t = GetTop();
                float b = t + Height;
                if (value < t + 1)
                    value = t + 1;
                ComputeY(t, value);
            }
        }

        private class SelectionObject
        {
            protected SelectionRect[] Rects = null;
            protected SelectionRect currentRect = null;
            private float x_board, y_board, previewSize;
            private float dif_x, dif_y;

            public virtual void Update() { }
            public virtual void Paint(Graphics g) { }
            public virtual string GetInfo(int deviceWith, int deviceHeight) { return ""; }
            public virtual SelectionRect GetSelectionMainRect() { return null; }

            public void Clear()
            {
                if (Rects != null)
                {
                    foreach (SelectionRect r in Rects)
                        if (r != null)
                            r.Selected = false;
                }
                currentRect = null;
            }
            public void SetBoard(float _x_board, float _y_board, float _previewSize)
            {
                x_board = _x_board;
                y_board = _y_board;
                previewSize = _previewSize;
            }
            public bool OnMouseDown(float x, float y, bool AlignToGrid)
            {
                foreach (var r in Rects)
                    if (r.IsPointInsideSelectionRect(x, y))
                    {
                        r.Selected = true;
                        currentRect = r;
                        dif_x = currentRect.X - x;
                        dif_y = currentRect.Y - y;
                        if (AlignToGrid) { dif_x = dif_y = 0; }
                        return true;
                    }
                return false;
            }
            public bool OnMouseMove(float x, float y)
            {
                if (currentRect == null)
                    return false;
                currentRect.X = x + dif_x;
                currentRect.Y = y + dif_y;
                Update();
                return true;
            }

            protected void CreateRects(int count)
            {
                Rects = new SelectionRect[count];
                for (int tr = 0; tr < Rects.Length; tr++)
                    Rects[tr] = new SelectionRect();
                currentRect = null;
            }
            protected void PaintAllRects(Graphics g)
            {
                foreach (SelectionRect r in Rects)
                    r.Paint(g, previewSize, x_board, y_board);
            }
            protected float TranslateXPoz(float x)
            {
                return x_board + x * previewSize / 100.0f; 
            }
            protected float TranslateYPoz(float y)
            {
                return y_board + y * previewSize / 100.0f;
            }
            protected float TranslateSize(float size)
            {
                return size * previewSize / 100.0f;
            }
        };

        private class ObjectToMoveAndResizeSelection: SelectionObject
        {
            Pen pMargin = new Pen(Color.FromArgb(128, 255, 255, 255));
            public ObjectToMoveAndResizeSelection()
            {
                CreateRects(9);
            }
            public override void Paint(Graphics g)
            {
                g.DrawRectangle(pMargin, TranslateXPoz(Rects[1].X), TranslateYPoz(Rects[1].Y), TranslateSize(Rects[5].X - Rects[1].X), TranslateSize(Rects[5].Y - Rects[1].Y));
                PaintAllRects(g);
                g.FillEllipse(Brushes.Red, TranslateXPoz(Rects[0].X) - 4, TranslateYPoz(Rects[0].Y) - 4, 8, 8);
                g.DrawEllipse(Pens.Black, TranslateXPoz(Rects[0].X) - 4, TranslateYPoz(Rects[0].Y) - 4, 8, 8);
            }
            public override string GetInfo(int deviceWith, int deviceHeight)
            {
                return String.Format("{0:0.00}% , {1:0.00}%", currentRect.X * 100.0f / deviceWith, currentRect.Y * 100.0f / deviceHeight);
            }

            private void ObjectToMoveAndResize_Update(int sourceIndex)
            {
                // daca l-am miscat doar pe cel din centru
                switch (sourceIndex)
                {
                    case 0:
                        float l = Rects[0].GetLeft();
                        float t = Rects[0].GetTop();
                        float width = Rects[0].Width;
                        float height = Rects[0].Height;
                        Rects[1].Create(l - 10, t - 10, 10, 10, Alignament.Center, SelectionRectType.Resize);
                        Rects[2].Create(l + width / 2, t - 10, 10, 10, Alignament.Center, SelectionRectType.Resize);
                        Rects[3].Create(l + width + 10, t - 10, 10, 10, Alignament.Center, SelectionRectType.Resize);
                        Rects[4].Create(l + width + 10, t + height / 2, 10, 10, Alignament.Center, SelectionRectType.Resize);
                        Rects[5].Create(l + width + 10, t + height + 10, 10, 10, Alignament.Center, SelectionRectType.Resize);
                        Rects[6].Create(l + width / 2, t + height + 10, 10, 10, Alignament.Center, SelectionRectType.Resize);
                        Rects[7].Create(l - 10, t + height + 10, 10, 10, Alignament.Center, SelectionRectType.Resize);
                        Rects[8].Create(l - 10, t + height / 2, 10, 10, Alignament.Center, SelectionRectType.Resize);
                        return;
                    case 1:
                        Rects[0].SetLeft(currentRect.X + 5);
                        Rects[0].SetTop(currentRect.Y + 5);
                        break;
                    case 2:
                        Rects[0].SetTop(currentRect.Y + 5);
                        break;
                    case 3:
                        Rects[0].SetRight(currentRect.X - 5);
                        Rects[0].SetTop(currentRect.Y + 5);
                        break;
                    case 4:
                        Rects[0].SetRight(currentRect.X - 5);
                        break;
                    case 5:
                        Rects[0].SetRight(currentRect.X - 5);
                        Rects[0].SetBottom(currentRect.Y - 5);
                        break;
                    case 6:
                        Rects[0].SetBottom(currentRect.Y - 5);
                        break;
                    case 7:
                        Rects[0].SetLeft(currentRect.X + 5);
                        Rects[0].SetBottom(currentRect.Y - 5);
                        break;
                    case 8:
                        Rects[0].SetLeft(currentRect.X + 5);
                        break;

                }
                ObjectToMoveAndResize_Update(0);
            }
            public override void Update()
            {
                for (int tr = 0; tr < Rects.Length; tr++)
                    if (currentRect == Rects[tr])
                    {
                        ObjectToMoveAndResize_Update(tr);
                        return;
                    }             
            }
            public override SelectionRect GetSelectionMainRect()
            {
                return Rects[0];
            }
            public void Create(float x, float y, float width, float height, Alignament _a)
            {
                Rects[0].Create(x, y, width, height, _a, SelectionRectType.MoveObject);
                ObjectToMoveAndResize_Update(0);
            }
        }

        private class Selection
        {
            SelectionObject currentSelectionObject = null;
            ObjectToMoveAndResizeSelection selection_ObjectToMoveAndResizeSelection = null;

            public string Info = "";


            public Selection()
            {
                selection_ObjectToMoveAndResizeSelection = new ObjectToMoveAndResizeSelection();
            }
            public void Clear()
            {
                if (currentSelectionObject != null)
                    currentSelectionObject.Clear();
                currentSelectionObject = null;
            }
            public SelectionRect GetSelectionRect()
            {
                if (currentSelectionObject != null)
                    return currentSelectionObject.GetSelectionMainRect();
                return null;
            }
            public void Create_ObjectToMoveAndResizeSelection(float x, float y, float width, float height, Alignament _a)
            {
                selection_ObjectToMoveAndResizeSelection.Create(x, y, width, height, _a);
                currentSelectionObject = selection_ObjectToMoveAndResizeSelection;
            }
            public void Paint(Graphics g, float previewSize, float x_board, float y_board)
            {
                if (currentSelectionObject != null)
                {
                    currentSelectionObject.SetBoard(x_board, y_board, previewSize);
                    currentSelectionObject.Paint(g);
                }
            }
            public bool OnMouseDown(float x, float y, bool AlignToGrid)
            {
                if (currentSelectionObject != null)
                    return currentSelectionObject.OnMouseDown(x, y, AlignToGrid);
                return false;
            }
            public void OnMouseMove(float x, float y, int deviceWith, int deviceHeight)
            {
                Info = "";
                if (currentSelectionObject != null)
                {
                    if (currentSelectionObject.OnMouseMove(x,y))
                        Info = currentSelectionObject.GetInfo(deviceWith, deviceHeight);                  
                }
            }
            public void OnMouseUp(float x, float y)
            {
                if (currentSelectionObject != null)
                    currentSelectionObject.Clear();
            }
        };

        #endregion

        enum ConvertCoordType
        {
            ScreenXToBoard,
            ScreenXToBoardWithoutAlignament,
            ScreenYToBoard,
            ScreenYToBoardWithoutAlignament,
            BoardXToScreen,
            BoardYToScreen,
            BoardXToBoardPercentage,
            BoardYToBoardPercentage,
        }
        enum MouseLockType
        {
            None,
            OnScreen,
            Select,
        };
        float currentMouseX, currentMouseY;
        float offsetX = 0, offsetY = 0, originalOffsetX = 0, originalOffsetY = 0;
        int screenMouseStartPointX, screenMouseStartPointY;

        int deviceWidth = 100, deviceHeight = 100, lastWidth = -1, lastHeight = -1;

        AnimO.Canvas canvas = new AnimO.Canvas();

        Selection selection = new Selection();


        int previewScale = 100;
        int timerCount = 0;
        string timerMessage = "";
        MouseLockType MouseLock = MouseLockType.None;
        SolidBrush brPanel = new SolidBrush(Color.FromArgb(128, 0,0,128));
        Font fnt = new Font("Arial", 11);
        StringFormat sf = new StringFormat();
        bool ctrlPressed = false;
        SolidBrush brushBackground = new SolidBrush(Color.Black);
        Pen penMargin = new Pen(Color.White);

        // coordonate de ecran
        // - mouse [in]
        // - afisare [out] - tre sa tina cont si de zoom
        // coordonate de board [care pot fi si cu minus / plus / etc]
        // offsetX, offsetY sunt relative la coltul de stanga sus al device-ului (acolo se considera (0,0) fara vreo redimensionare

        #region Propriertati
        public bool AlignToGrid
        {
            set;
            get;
        }
        public int Zoom
        {
            get { return previewScale; }
            set
            {
                if (value < 10)
                    value = 10;
                if (value > 1000)
                    value = 1000;
                float c_x = ConvertCoord(this.Width / 2, ConvertCoordType.ScreenXToBoardWithoutAlignament);
                float c_y = ConvertCoord(this.Height / 2, ConvertCoordType.ScreenYToBoardWithoutAlignament);
                if (previewScale != value)
                {
                    previewScale = value;
                    // value = ((value - offsetX) * ((float)previewScale)) / 100.0f; == this.Width / 2
                    offsetX = c_x - (this.Width * 50.0f) / ((float)previewScale);
                    offsetY = c_y - (this.Height * 50.0f) / ((float)previewScale);
                }
                ActivateTimer(String.Format("Zoom: {0} %", value));

                UpdateCoords();
            }
        }
        #endregion

        #region Delegates and Events

        public delegate void OnPaintCanvasDelegate(AnimO.Canvas canvas);
        public event OnPaintCanvasDelegate OnPaintCanvas = null;

        public delegate void OnSelectionChangedDelegate(AnimO.Canvas canvas);
        public event OnSelectionChangedDelegate OnSelectionChanged = null;

        public delegate void OnSelectionEndsDelegate(AnimO.Canvas canvas);
        public event OnSelectionEndsDelegate OnSelectionEnds = null;

        public delegate void OnSelectionBeginsDelegate(AnimO.Canvas canvas);
        public event OnSelectionBeginsDelegate OnSelectionBegins = null;

        #endregion

        public AnimationBoard()
        {
            InitializeComponent();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            UpdateCoords();
            CenterBoard();
            AlignToGrid = true;
            this.MouseWheel += AnimationBoard_MouseWheel;
            SetBackgroundColor(Color.Black);
        }

        public AnimO.Canvas GetCanvas() { return canvas; }
        public void SetBackgroundColor(Color c)
        {
            brushBackground.Color = c;
            int avg = (((int)c.R) + ((int)c.G) + ((int)c.B)) / 3;
            if (avg > 128)
                avg = 0;
            else
                avg = 255;
            penMargin.Color = Color.FromArgb(avg,avg,avg);
        }

        void AnimationBoard_MouseWheel(object sender, MouseEventArgs e)
        {
            if (ctrlPressed)
            {
                if (e.Delta < 0)
                    Zoom = Zoom + 5;
                else
                    Zoom = Zoom - 5;
            }
        }

        private void UpdateCoords()
        {
            Redraw();
        }
        private void Redraw()
        {
            Invalidate();
        }

        public void SetDeviceSize(int width,int height)
        {
            if (width < 30)
                width = 30;
            if (height < 30)
                height = 30;
            deviceWidth = width;
            deviceHeight = height;
            UpdateCoords();
        }
        public void CenterBoard()
        {
            offsetX = (deviceWidth - this.Width * 100.0f / previewScale) / 2.0f;
            offsetY = (deviceHeight - this.Height * 100.0f / previewScale) / 2.0f;
            Redraw();
        }
        public void RealignOnResize()
        {
            if ((lastWidth != -1) && (lastHeight != -1))
            {
                offsetX += ((lastWidth - this.Width) * 50.0f / (float)previewScale);
                offsetY += ((lastHeight - this.Height) * 50.0f / (float)previewScale);
            }
            lastWidth = this.Width;
            lastHeight = this.Height;
            UpdateCoords();
        }

        private float ConvertCoord(float value,ConvertCoordType type)
        {
            int prc;
            switch (type)
            {
                case ConvertCoordType.BoardXToScreen: 
                    value = ((value - offsetX) * ((float)previewScale)) / 100.0f;
                    return value;
                case ConvertCoordType.BoardYToScreen: 
                    value =  ((value - offsetY) * ((float)previewScale)) / 100.0f;
                    return value;
                case ConvertCoordType.ScreenXToBoard:
                case ConvertCoordType.ScreenXToBoardWithoutAlignament:
                    value = offsetX + (value * 100.0f / (float)previewScale);
                    if ((AlignToGrid) && (type != ConvertCoordType.ScreenXToBoardWithoutAlignament))
                    {
                        prc = deviceWidth / 20;
                        value = (((int)(((int)value) / (prc))) * prc);
                    }
                    return value;
                case ConvertCoordType.ScreenYToBoard: 
                case ConvertCoordType.ScreenYToBoardWithoutAlignament:
                    value = offsetY + (value * 100.0f / (float)previewScale);
                    if ((AlignToGrid) && (type != ConvertCoordType.ScreenYToBoardWithoutAlignament))
                    {
                        prc = deviceHeight / 20;
                        value = (((int)(((int)value) / (prc)))*prc);
                    }
                    return value;
                case ConvertCoordType.BoardXToBoardPercentage:
                    value = value * 100.0f / deviceWidth;
                    return value;
                case ConvertCoordType.BoardYToBoardPercentage:
                    value = value * 100.0f / deviceHeight;
                    return value;
                       
            }
            return 0;
        }

        private void UpdateMousePos(MouseEventArgs e)
        {
            currentMouseX = ConvertCoord(e.X, ConvertCoordType.ScreenXToBoard);
            currentMouseY = ConvertCoord(e.Y, ConvertCoordType.ScreenYToBoard);
        }
        private void AnimationBoard_MouseDown(object sender, MouseEventArgs e)
        {
            UpdateMousePos(e);
            screenMouseStartPointX = e.X;
            screenMouseStartPointY = e.Y;
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                MouseLock = MouseLockType.OnScreen;
                originalOffsetX = offsetX;
                originalOffsetY = offsetY;
                return;
            }
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (selection.OnMouseDown(ConvertCoord(e.X, ConvertCoordType.ScreenXToBoardWithoutAlignament), ConvertCoord(e.Y, ConvertCoordType.ScreenYToBoardWithoutAlignament),AlignToGrid))
                {
                    MouseLock = MouseLockType.Select;
                    Redraw();
                    if (OnSelectionBegins != null)
                        OnSelectionBegins(canvas);
                }
                return;
            }
        }

        private void AnimationBoard_MouseEnter(object sender, EventArgs e)
        {

        }

        private void AnimationBoard_MouseHover(object sender, EventArgs e)
        {

        }

        private void AnimationBoard_MouseLeave(object sender, EventArgs e)
        {

        }

        private void AnimationBoard_MouseMove(object sender, MouseEventArgs e)
        {
            UpdateMousePos(e);
            if (MouseLock == MouseLockType.None)
            {
                ActivateTimer(String.Format("{0:0.00}% , {1:0.00}%", ConvertCoord(currentMouseX, ConvertCoordType.BoardXToBoardPercentage), ConvertCoord(currentMouseY, ConvertCoordType.BoardYToBoardPercentage)));
                Redraw();
                return;
            }

            if (MouseLock == MouseLockType.OnScreen)
            {
                offsetX = originalOffsetX + ((screenMouseStartPointX - e.X) * 100.0f / (float)previewScale);
                offsetY = originalOffsetY + ((screenMouseStartPointY - e.Y) * 100.0f / (float)previewScale);
                ActivateTimer(String.Format("Scroll to {0} : {1} [Start:{2},{3}] [Current:{4},{5}]", offsetX, offsetY, screenMouseStartPointX, screenMouseStartPointY,currentMouseX,currentMouseY));
                Redraw();
                
                return;
            }

            if (MouseLock == MouseLockType.Select)
            {
                UpdateMousePos(e);
                selection.OnMouseMove(currentMouseX, currentMouseY,deviceWidth,deviceHeight);
                ActivateTimer(selection.Info);
                if (OnSelectionChanged != null)
                    OnSelectionChanged(canvas);
                Redraw();

                return;
            }
        }

        private void AnimationBoard_MouseUp(object sender, MouseEventArgs e)
        {
            if (MouseLock == MouseLockType.Select)
            {
                UpdateMousePos(e);
                selection.OnMouseUp(currentMouseX, currentMouseY);
                if (OnSelectionChanged != null)
                    OnSelectionChanged(canvas);
                Redraw();
                if (OnSelectionEnds != null)
                    OnSelectionEnds(canvas);
            }
            MouseLock = MouseLockType.None;
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(brushBackground, 0, 0, this.Width, this.Height);


            // board
            float board_x = ConvertCoord(0, ConvertCoordType.BoardXToScreen);
            float board_y = ConvertCoord(0, ConvertCoordType.BoardYToScreen);

            if (OnPaintCanvas!=null)
            {
                canvas.SetGraphics(e.Graphics);
                canvas.SetScreen((int)board_x, (int)board_y, (int)(deviceWidth * previewScale / 100.0f), (int)(deviceHeight * previewScale / 100.0f), previewScale / 100.0f);
                OnPaintCanvas(canvas);
            }
            
            if (AlignToGrid)
            {
                float step = deviceWidth * previewScale / 2000.0f;
                float x_max = board_x + deviceWidth * previewScale / 100.0f;
                float y_max = board_y + deviceHeight * previewScale / 100.0f;
                float val;
                int tr;
                for (tr = 1, val = board_x+step; tr < 20; tr++, val += step)
                    e.Graphics.DrawLine(Pens.DarkBlue, val, board_y, val, y_max);
                step = deviceHeight * previewScale / 2000.0f;
                for (tr = 1, val = board_y+step; tr < 20; tr++, val += step)
                    e.Graphics.DrawLine(Pens.DarkBlue, board_x, val, x_max, val);
                
            }
            e.Graphics.DrawRectangle(penMargin, board_x, board_y, deviceWidth * previewScale / 100.0f, deviceHeight * previewScale / 100.0f);

            // select
            if (MouseLock == MouseLockType.Select)
            {
                //int sel_x = mouseStartPointX + deviceHeight.W
            }

            selection.Paint(e.Graphics, previewScale, board_x, board_y);

            if (timerCount>0)
            {
                e.Graphics.FillRectangle(brPanel, 0, this.Height - 24, this.Width, this.Height);
                e.Graphics.DrawString(timerMessage, fnt, Brushes.White, this.Width / 2, this.Height - 10, sf);
            }
        }

        private void AnimationBoard_SizeChanged(object sender, EventArgs e)
        {
            RealignOnResize();
        }

        private void ActivateTimer(string msg)
        {
            if ((msg == null) || (msg.Length == 0))
                return;
            timerCount = 2;
            timerMessage = msg;
            timer.Enabled = true;
        }

        private void OnTimer(object sender, EventArgs e)
        {
            timerCount--;
            if (timerCount <= 0)
            {
                timer.Enabled = false;
                Redraw();
            }
        }

        private void AnimationBoard_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Control) && (ctrlPressed==false))
            {
                ctrlPressed = true;
                ActivateTimer("Ctrl pressed !");
                Redraw();
            }
        }

        private void AnimationBoard_KeyUp(object sender, KeyEventArgs e)
        {
            ctrlPressed = e.Control;
        }

        #region Selection

        public void ClearSelection()
        {
            selection.Clear();
            Redraw();
        }
        public void Create_ObjectToMoveAndResizeSelection(float x, float y, float width, float height, Alignament _a)
        {
            selection.Create_ObjectToMoveAndResizeSelection(x, y, width, height, _a);
            Redraw();
        }
        public SelectionRect GetSelectionRect()
        {
            return selection.GetSelectionRect();
        }

        #endregion

        public float GetScale() { return canvas.GetScale(); }
    }
}
