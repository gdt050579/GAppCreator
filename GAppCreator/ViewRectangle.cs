using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GAppCreator
{
    public class ViewRectangle
    {
        enum MouseStatus
        {
            None,
            Move,
            TopLeft,
            TopCenter,
            TopRight,
            RightCenter,
            BottomRight,
            BottomCenter,
            BottomLeft,
            LeftCenter,           
        };
        public int Left = 0, Top = 0, Right = 0, Bottom = 0;
        int OffsetLeft, OffsetTop;
        bool Initialize = false;
        const int Ray = 5;
        Pen p = new Pen(Color.White, 1);
        SolidBrush b = new SolidBrush(Color.White);
        MouseStatus Status = MouseStatus.None;

        public void Create(int left,int top,int right,int bottom)
        {
            Left = left;
            Top = top;
            Right = Math.Max(right, Ray * 4 + Left);
            Bottom = Math.Max(bottom, Ray * 2 + Top);
            Initialize = true;
        }
        public bool IsInitialize()
        {
            return Initialize;
        }
        private bool IsInCircle(int x,int y,int cx,int cy,int ray)
        {
            int d = (x - cx) * (x - cx) + (y - cy) * (y - cy);
            return (d <= ray * ray);
        }
        private MouseStatus GetStatus(int mouseX, int mouseY)
        {
            if (IsInCircle(mouseX, mouseY, Left, Top, Ray))
                return MouseStatus.TopLeft;
            if (IsInCircle(mouseX, mouseY, (Left + Right) / 2, Top, Ray))
                return MouseStatus.TopCenter;
            if (IsInCircle(mouseX, mouseY, Right, Top, Ray))
                return MouseStatus.TopRight;
            if (IsInCircle(mouseX, mouseY, Right, (Top + Bottom) / 2, Ray))
                return MouseStatus.RightCenter;
            if (IsInCircle(mouseX, mouseY, Right, Bottom, Ray))
                return MouseStatus.BottomRight;
            if (IsInCircle(mouseX, mouseY, (Left + Right) / 2,  Bottom, Ray))
                return MouseStatus.BottomCenter;
            if (IsInCircle(mouseX, mouseY, Left, Bottom, Ray))
                return MouseStatus.BottomLeft;
            if (IsInCircle(mouseX, mouseY, Left, (Top + Bottom) / 2, Ray))
                return MouseStatus.LeftCenter;
            if ((mouseX >= Left) && (mouseX <= Right) && (mouseY >= Top) && (mouseY <= Bottom))
                return MouseStatus.Move;
            return MouseStatus.None;
        }
        private Cursor StatusToCursor(MouseStatus status)
        {
            switch (status)
            {
                case MouseStatus.TopLeft: 
                case MouseStatus.BottomRight:
                    return Cursors.SizeNWSE;
                case MouseStatus.TopCenter: 
                case MouseStatus.BottomCenter:
                    return Cursors.SizeNS;
                case MouseStatus.TopRight:
                case MouseStatus.BottomLeft:
                    return Cursors.SizeNESW;
                case MouseStatus.RightCenter:
                case MouseStatus.LeftCenter:
                    return Cursors.SizeWE;
                case MouseStatus.Move:
                    return Cursors.SizeAll;
                case MouseStatus.None:
                    return Cursors.Arrow;
            }
            return Cursors.Arrow;
        }
        private void UpdateValues(int mouseX,int mouseY)
        {
            int minDist = Ray*4;
            switch (Status)
            {
                case MouseStatus.TopLeft:
                    Left = Math.Min(mouseX,Right-minDist);
                    Top = Math.Min(mouseY,Bottom-minDist);
                    break;
                case MouseStatus.TopCenter:
                    Top = Math.Min(mouseY,Bottom-minDist);
                    break;
                case MouseStatus.TopRight:
                    Right = Math.Max(mouseX,Left+minDist);
                    Top = Math.Min(mouseY,Bottom-minDist);
                    break;
                case MouseStatus.RightCenter:
                    Right = Math.Max(mouseX,Left+minDist);
                    break;
                case MouseStatus.BottomRight:
                    Right = Math.Max(mouseX,Left+minDist);
                    Bottom = Math.Max(mouseY,Top+minDist);
                    break;
                case MouseStatus.BottomCenter:
                    Bottom = Math.Max(mouseY,Top+minDist);
                    break;
                case MouseStatus.BottomLeft:
                    Left = Math.Min(mouseX, Right - minDist);
                    Bottom = Math.Max(mouseY, Top + minDist);
                    break;
                case MouseStatus.LeftCenter:
                    Left = Math.Min(mouseX, Right - minDist);
                    break;
                case MouseStatus.Move:
                    int w = Right - Left;
                    int h = Bottom - Top;
                    Left = mouseX - OffsetLeft;
                    Top = mouseY - OffsetTop;
                    Right = Left + w;
                    Bottom = Top + h;
                    break;
            }
        }
        public bool OnMouseDown(int mouseX,int mouseY)
        {
            Status = GetStatus(mouseX,mouseY);
            if (Status == MouseStatus.Move)
            {
                OffsetTop = mouseY-Top;
                OffsetLeft = mouseX-Left; 
            }
            return Status!=MouseStatus.None;
        }
        public bool OnMouseMove(int mouseX,int mouseY)
        {
            if (Status == MouseStatus.None)
                return false;
            UpdateValues(mouseX, mouseY);
            return true;
        }
        public bool OnMouseUp(int mouseX, int mouseY)
        {
            if (Status == MouseStatus.None)
                return false;
            UpdateValues(mouseX, mouseY);
            Status = MouseStatus.None;
            return true;
        }
        public Cursor GetCursor()
        {
            return StatusToCursor(Status);
        }
        public Cursor GetCursorForPoz(int mouseX,int mouseY)
        {
            return StatusToCursor(GetStatus(mouseX, mouseY));
        }
        private void DrawCircle(System.Drawing.Graphics g,int x,int y)
        {
            g.FillEllipse(b, x - Ray, y - Ray, Ray * 2, Ray * 2);
        }
        public void Paint(System.Drawing.Graphics g,System.Drawing.Color c)
        {
            if (Status == MouseStatus.None)
                p.Color = Color.FromArgb(128, c);
            else
                p.Color = c;
            g.DrawRectangle(p, Left, Top, Right - Left + 1, Bottom - Top + 1);
            b.Color = p.Color;
            DrawCircle(g, Left, Top);
            DrawCircle(g, (Left+Right)/2, Top);
            DrawCircle(g, Right, Top);
            DrawCircle(g, Right, (Top + Bottom) / 2);
            DrawCircle(g, Right, Bottom);
            DrawCircle(g, (Left + Right) / 2, Bottom);
            DrawCircle(g, Left, Bottom);
            DrawCircle(g, Left, (Top + Bottom) / 2);
        }
    }
}
