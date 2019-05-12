using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GAppCreator
{
    public class GRect
    {
        public int Left=0, Right=0, Top=0, Bottom=0;
      
        public void SetWH(int left, int top, int width, int height)
        {
            Left = left;
            Right = Left + width;
            Top = top;
            Bottom = Top + height;
        }
        public void Set(int left, int top, int right, int bottom)
        {
            Left = left;
            Right = right;
            Top = top;
            Bottom = bottom;
        }
        public void Set(int anchorX, int anchorY, Alignament align, int width, int height)
        {
            switch (align)
            {
                case Alignament.TopLeft:
                    Left = anchorX;
                    Top = anchorY;
                    break;
                case Alignament.TopCenter:
                    Left = anchorX - width / 2;
                    Top = anchorY;
                    break;
                case Alignament.TopRight:
                    Left = anchorX - width;
                    Top = anchorY;
                    break;
                case Alignament.LeftCenter:
                    Left = anchorX;
                    Top = anchorY-height/2;
                    break;
                case Alignament.Center:
                    Left = anchorX - width / 2;
                    Top = anchorY - height / 2;
                    break;
                case Alignament.RightCenter:
                    Left = anchorX-width;
                    Top = anchorY - height / 2;
                    break;
                case Alignament.BottomLeft:
                    Left = anchorX;
                    Top = anchorY - height;
                    break;
                case Alignament.BottomCenter:
                    Left = anchorX - width/2;
                    Top = anchorY - height;
                    break;
                case Alignament.BottomRight:
                    Left = anchorX - width;
                    Top = anchorY - height;
                    break;
            }
            Right = Left + width;
            Bottom = Top + height;
        }
        public void Set(int anchorX, int anchorY, Alignament align)
        {
            Set(anchorX, anchorY, align, Right - Left, Bottom - Top);
        }
        public void Set(GRect rect)
        {
            Left = rect.Left;
            Right = rect.Right;
            Top = rect.Top;
            Bottom = rect.Bottom;
        }
        public void Intersect(GRect r)
        {
            if (r.Left > Left)
                Left = r.Left;
            if (r.Right < Right)
                Right = r.Right;
            if (r.Top > Top)
                Top = r.Top;
            if (r.Bottom < Bottom)
                Bottom = r.Bottom;
        }
        public void Intersect(int left,int top,int right,int bottom)
        {
            if (left > Left)
                Left = left;
            if (right < Right)
                Right = right;
            if (top > Top)
                Top = top;
            if (bottom < Bottom)
                Bottom = bottom;
        }
        public void Add(int addX, int addY)
        {
            Left += addX;
            Right += addX;
            Top += addY;
            Bottom += addY;
        }
        public bool Contains(int x, int y)
        {
            return ((x >= Left) && (x <= Right) && (y >= Top) && (y <= Bottom));
        }
        public int GetWidth()
        {
            return Right - Left;
        }
        public int GetHeight()
        {
            return Bottom - Top;
        }
        public int CenterX()
        {
            return (Right + Left) / 2;
        }
        public int CenterY()
        {
            return (Top + Bottom) / 2;
        }
        public void SetWidthFromLeft(int newValue)
        {
            Right = Left + newValue;
        }
        public void SetWidthFromRight(int newValue)
        {
            Left = Right - newValue;
        }
        public void SetHeightFromTop(int newValue)
        {
            Bottom = Top + newValue;
        }
        public void SetHeightFromBottom(int newValue)
        {
            Top = Bottom - newValue;
        }
        public int GetAnchorX(Alignament align)
        {
            switch (align)
            {
                case Alignament.TopLeft: 
                case Alignament.BottomLeft:
                case Alignament.LeftCenter:
                    return Left;
                case Alignament.TopRight:
                case Alignament.BottomRight:
                case Alignament.RightCenter:
                    return Right;
            }
            return (Right + Left) / 2;
        }
        public int GetAnchorY(Alignament align)
        {
            switch (align)
            {
                case Alignament.TopLeft:
                case Alignament.TopCenter:
                case Alignament.TopRight:
                    return Top;
                case Alignament.BottomLeft:
                case Alignament.BottomCenter:
                case Alignament.BottomRight:
                    return Bottom;
            }
            return (Top + Bottom) / 2;
        }
        public int GetDockX(Alignament align, int width)
        {
            switch (align)
            {
                case Alignament.TopLeft:
                case Alignament.BottomLeft:
                case Alignament.LeftCenter:
                    return Left;
                case Alignament.TopRight:
                case Alignament.BottomRight:
                case Alignament.RightCenter:
                    return Right-width;
            }
            return (Right + Left - width) / 2;
        }
        public int GetDockY(Alignament align, int height)
        {
            switch (align)
            {
                case Alignament.TopLeft:
                case Alignament.TopCenter:
                case Alignament.TopRight:
                    return Top;
                case Alignament.BottomLeft:
                case Alignament.BottomCenter:
                case Alignament.BottomRight:
                    return Bottom-height;
            }
            return (Top + Bottom - height) / 2;
        }

    }
}
