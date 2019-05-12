using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GAppCreator
{
    public class PresentationSelection
    {
        public class Punct
        {
            public int X, Y;
            public Alignament Align;
            public System.Windows.Forms.Cursor cursor;
        };
        public IPresentationFrameViewer FrameViewer;
        public GRect rect = new GRect();
        public GRect temp = new GRect();
        GRect margin = new GRect();
        bool Visible = false;
        int ofX, ofY;
        Punct[] Puncte;
        Punct Selected;

        public PresentationSelection()
        {
            Puncte = new Punct[9];
            for (int tr = 0; tr < 9; tr++)
                Puncte[tr] = new Punct();
            Puncte[0].Align = Alignament.TopLeft;
            Puncte[1].Align = Alignament.TopCenter;
            Puncte[2].Align = Alignament.TopRight;
            Puncte[3].Align = Alignament.RightCenter;
            Puncte[4].Align = Alignament.BottomRight;
            Puncte[5].Align = Alignament.BottomCenter;
            Puncte[6].Align = Alignament.BottomLeft;
            Puncte[7].Align = Alignament.LeftCenter;
            Puncte[8].Align = Alignament.Center;

            Puncte[0].cursor = System.Windows.Forms.Cursors.SizeNWSE;
            Puncte[1].cursor = System.Windows.Forms.Cursors.SizeNS;
            Puncte[2].cursor = System.Windows.Forms.Cursors.SizeNESW;
            Puncte[3].cursor = System.Windows.Forms.Cursors.SizeWE;
            Puncte[4].cursor = System.Windows.Forms.Cursors.SizeNWSE;
            Puncte[5].cursor = System.Windows.Forms.Cursors.SizeNS;
            Puncte[6].cursor = System.Windows.Forms.Cursors.SizeNESW;
            Puncte[7].cursor = System.Windows.Forms.Cursors.SizeWE;
            Puncte[8].cursor = System.Windows.Forms.Cursors.SizeAll;

            Selected = null;
        }
        private void UpdateMarginAndPoints()
        {
            margin.Set(rect);
            margin.Left -= 2;
            margin.Right += 2;
            margin.Top -= 2;
            margin.Bottom += 2;

            for (int tr = 0; tr < 8; tr++)
            {
                Puncte[tr].X = margin.GetAnchorX(Puncte[tr].Align);
                Puncte[tr].Y = margin.GetAnchorY(Puncte[tr].Align);
            }
            Puncte[8].X = rect.Left;
            Puncte[8].Y = rect.Top;
        }
        public void SetSelection(int Left, int Top, int Right, int Bottom)
        {
            rect.Set(Left, Top, Right, Bottom);
            UpdateMarginAndPoints();
            Visible = true;
        }
        public void HideSelection()
        {
            Visible = false;
            Selected = null;
        }
        public void ClearSelection()
        {
            Selected = null;
        }
        public bool HitTest(int x, int y)
        {
            if (Visible == false)
                return false;
            int id = PositionToPoint(x, y);
            if (id >= 0)
            {
                Selected = Puncte[id];
                ofX = Selected.X - x;
                ofY = Selected.Y - y;
                return true;
            }
            return false;
        }
        public bool HasSelection()
        {
            return ((Selected != null) && (Visible==true));
        }
        public bool IsVisible()
        {
            return Visible;
        }
        private int PositionToPoint(int x, int y)
        {
            for (int tr = 0; tr < 8; tr++)
            {
                int dist = (x - Puncte[tr].X) * (x - Puncte[tr].X) + (y - Puncte[tr].Y) * (y - Puncte[tr].Y);
                if (dist <= 25)
                    return tr;
            }
            if (rect.Contains(x, y))
                return 8;
            return -1;
        }
        public System.Windows.Forms.Cursor GetCursor(int x,int y)
        {
            int id = PositionToPoint(x, y);
            if (id >= 0)
                return Puncte[id].cursor;
            return null;
        }
        public System.Windows.Forms.Cursor GetSelectedCursor()
        {
            if (Selected != null)
                return Selected.cursor;
            return null;
        }
        public void Paint(AnimO.Canvas canvas)
        {
            if (Visible)
            {
                if (Selected != null)
                {
                    canvas.DrawRectWithPixelsCoordonates(margin.Left,margin.Top,margin.Right,margin.Bottom, Color.Yellow.ToArgb(),0, 2);
                    for (int tr = 0; tr < 8; tr++)
                    {
                        if (Puncte[tr] == Selected)
                        {
                            canvas.DrawCircle(Puncte[tr].X, Puncte[tr].Y, 7, Color.White.ToArgb(), Color.Red.ToArgb(), 1);
                        }
                        else
                        {
                            canvas.DrawCircle(Puncte[tr].X, Puncte[tr].Y, 3, Color.White.ToArgb(), Color.Gray.ToArgb(), 1);
                        }
                    }
                }
                else
                {
                    canvas.DrawRectWithPixelsCoordonates(margin.Left, margin.Top, margin.Right, margin.Bottom, Color.Gray.ToArgb(), 0, 1);
                    for (int tr = 0; tr < 8; tr++)
                    {
                        canvas.DrawCircle(Puncte[tr].X, Puncte[tr].Y, 5, Color.White.ToArgb(), Color.Gray.ToArgb(), 1);
                    }
                }
            }
        }
        public void UpdatePosition(int x, int y, bool add, bool shiftPressed)
        {
            temp.Set(rect);
            int Left = rect.Left;
            int Right = rect.Right;
            int Top = rect.Top;
            int Bottom = rect.Bottom;

            if (Selected == null)
            {
                if (add)
                {
                    if (shiftPressed)
                    {
                        Right += x;
                        Bottom += y;
                    } else {
                        Left += x;
                        Right += x;
                        Top += y;
                        Bottom += y;
                    }
                    shiftPressed = false;
                }
                else
                {
                    Left = x;
                    Top = y;
                    Right = Left + rect.GetWidth();
                    Bottom = Top + rect.GetHeight();
                }
            }
            else
            {
                if (add)
                {
                    x += Selected.X;
                    y += Selected.Y;
                }

                switch (Selected.Align)
                {
                    case Alignament.Center:
                        Left = x + ofX;
                        Top = y + ofY;
                        Right = Left + rect.GetWidth();
                        Bottom = Top + rect.GetHeight();
                        break;
                    case Alignament.TopLeft:
                        Left = x + ofX + 2;
                        Top = y + ofY + 2;
                        break;
                    case Alignament.TopCenter:
                        Top = y + ofY + 2;
                        break;
                    case Alignament.TopRight:
                        Right = x + ofX - 2;
                        Top = y + ofY + 2;
                        break;
                    case Alignament.RightCenter:
                        Right = x + ofX - 2;
                        break;
                    case Alignament.BottomRight:
                        Right = x + ofX - 2;
                        Bottom = y + ofY - 2;
                        break;
                    case Alignament.BottomCenter:
                        Bottom = y + ofY - 2;
                        break;
                    case Alignament.BottomLeft:
                        Bottom = y + ofY - 2;
                        Left = x + ofX + 2;
                        break;
                    case Alignament.LeftCenter:
                        Left = x + ofX + 2;
                        break;

                }
            }
            if (shiftPressed)
            {
                if ((Left == rect.Left) && (Right != rect.Right))
                    Left = rect.Left - (Right - rect.Right);
                if ((Left != rect.Left) && (Right == rect.Right))
                    Right = rect.Right - (Left - rect.Left);
                if ((Top == rect.Top) && (Bottom != rect.Bottom))
                    Top = rect.Top - (Bottom - rect.Bottom);
                if ((Top != rect.Top) && (Bottom == rect.Bottom))
                    Bottom = rect.Bottom - (Top - rect.Top);

            }
            if ((Left <= Right) && (Top <= Bottom))
            {                
                rect.Set(Left, Top, Right, Bottom);
            }
            UpdateMarginAndPoints();
            if (FrameViewer != null)
                FrameViewer.UpdateSelectedObjectsPositions(temp, rect);
        }
    }
}
