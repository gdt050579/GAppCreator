using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Xml.Serialization;
using System.ComponentModel;

namespace GAppCreator
{
    public class TextPainter
    {
        public enum FontSizeMethod
        {
            Scale,
            Pixels,
            PercentageOfHeight,
            ShrinkToFitWidth,
            ZoomToFitWidth,
        };
        class GlyphLine
        {
	        public uint	    start,end;
	        public uint	    startFirstWord,endLastWord;
	        public float	size;
        };
        class GlyphLocation
        {
            public float    x,y,w,h;
            public Bitmap   Image;
            public int      Code;
            public bool     Visibility;
        };
        class GlyphWord
        {
	        public uint	start,end;
	        public int		type;
	        public float	width;
	        public float	height;
	        public bool	wordIsTruncated;

	        // in
	        public float scale,charSpace,spaceWidth,wrapWidth;
	        public List<GlyphLocation> text = null;
	        public FontResource font;
	        public uint charactersCount;
        };
        private  const int CHAR_MODE_NORMAL=0;
        private  const int CHAR_MODE_WORD_WRAP=1;
        private  const int CHAR_MODE_JUSTIFY=2;
        private  const int TEXT_PAINT_MODE_DOCK=0x10000;
        private  const uint CHAR_INVALID_POSITION = 0xFFFFFFFF;

        private  const int CHAR_TYPE_SPACE=0x81;
        private  const int CHAR_TYPE_TAB=0x82;
        private  const int CHAR_TYPE_WORD=0x03;
        private  const int CHAR_TYPE_EOL=0x04;
        private  const int CHAR_TYPE_EOW=0x83;
        private  const int CHAR_COMMAND_STOP=0x80;
        private  const int CHAR_TYPE_MASK=0x7F;

        private  const int WORD_TYPE_SPACE =	(CHAR_TYPE_SPACE & CHAR_TYPE_MASK);
        private  const int WORD_TYPE_TAB = (CHAR_TYPE_TAB   & CHAR_TYPE_MASK);
        private  const int WORD_TYPE_WORD	= (CHAR_TYPE_WORD  & CHAR_TYPE_MASK);
        private  const int WORD_TYPE_EOL = (CHAR_TYPE_EOL   & CHAR_TYPE_MASK);



        void ALIGN_COORD(ref float res_x,ref float res_y,float x,float y,float width,float height,Alignament align_type)
        {
            res_x = x;
            res_y = y;
	        switch (align_type) { 
		        case Alignament.TopLeft: res_x = (x); res_y = (y); break; 
		        case Alignament.TopCenter: res_x = (x) - (width)/2.0f; res_y = (y); break; 
		        case Alignament.TopRight: res_x = (x) - (width); res_y = (y); break; 
		        case Alignament.RightCenter: res_x = (x) - (width); res_y = (y) - (height)/2.0f; break; 
		        case Alignament.BottomRight: res_x = (x) - (width); res_y = (y) - (height); break; 
		        case Alignament.BottomCenter: res_x = (x) - (width)/2.0f;res_y = (y) - (height); break; 
		        case Alignament.BottomLeft: res_x = (x); res_y = (y) - (height); break; 
		        case Alignament.LeftCenter: res_x = (x); res_y = (y) - (height)/2.0f; break; 
		        case Alignament.Center: res_x = (x) - (width)/2.0f; res_y = (y) - (height)/2.0f; break; 
		        }; 
        }
        void ALIGN_OBJECT_TO_RECT(ref float res_x,ref float res_y,float left,float top,float right,float bottom,float object_width,float object_height,Alignament align_type) 
        {
            res_x = left;
            res_y = top;
   	        switch (align_type) {
                case Alignament.TopLeft: res_x = (left); res_y = (top); break;
                case Alignament.TopCenter: res_x = ((left) + (right) - (object_width)) / 2.0f; res_y = (top); break;
                case Alignament.TopRight: res_x = (right) - (object_width); res_y = (top); break;
                case Alignament.RightCenter: res_x = (right) - (object_width); res_y = ((top) + (bottom) - (object_height)) / 2.0f; break;
                case Alignament.BottomRight: res_x = (right) - (object_width); res_y = (bottom) - (object_height); break;
                case Alignament.BottomCenter: res_x = ((left) + (right) - (object_width)) / 2.0f; res_y = (bottom) - (object_height); break;
                case Alignament.BottomLeft: res_x = (left); res_y = (bottom) - (object_height); break;
                case Alignament.LeftCenter: res_x = (left); res_y = ((top) + (bottom) - (object_height)) / 2.0f; break;
                case Alignament.Center: res_x = ((left) + (right) - (object_width)) / 2.0f; res_y = ((top) + (bottom) - (object_height)) / 2.0f; break;
		        };
        }

        int GetCharCodeType(int ch)
        {
	        if ((ch>='A') && (ch<='Z'))
		        return CHAR_TYPE_WORD;
	        if ((ch>='a') && (ch<='z'))
		        return CHAR_TYPE_WORD;
	        if ((ch>='0') && (ch<='9'))
		        return CHAR_TYPE_WORD;
	        if (ch==' ')
		        return CHAR_TYPE_SPACE;
	        if (ch=='\t')
		        return CHAR_TYPE_TAB;
	        if (ch=='\n')
		        return CHAR_TYPE_EOL;
	        if ((ch>32) && (ch<127))
		        return CHAR_TYPE_EOW;
	        return CHAR_TYPE_WORD;
        }

        void FindNextGlyphWord(AppResources Resources, uint start,GlyphWord w)
        {
	        GlyphLocation p = w.text[(int)start];
	        int type = (GetCharCodeType(p.Code) & CHAR_TYPE_MASK);
	        int ctype;
	        float charWidth = 0,charHeight = 0;
            Image img;

	        w.start = w.end = start;
	        w.width = 0;
	        w.height = 0;
	        w.type = type;
	        w.wordIsTruncated = false;
	        do
	        {
                p = w.text[(int)start];
		        ctype = GetCharCodeType(p.Code);
		        if ((ctype & CHAR_TYPE_MASK)!=type)
			        break;
                int index = w.font.CodeToIndex(p.Code);
                img = null;
                if (index >= 0)
                    img = w.font.Glyphs[index].Picture;
		        if (img!=null)
		        {
			        charWidth = img.Width * w.scale;
			        charHeight = img.Height * w.scale;
			        p.Image = (Bitmap)img;
		        } else {
			        charWidth = w.spaceWidth;
			        charHeight = 0;
			        p.Image = null;
			        if (ctype == CHAR_TYPE_TAB)
				        charWidth*=4;
		        }
                p.w = charWidth;
                p.h = charHeight;
		        if (w.end>w.start) // prima litera
			        p.x = w.width+w.charSpace;
		        else
			        p.x = w.width;

		        if (charHeight>w.height)
			        w.height = charHeight;
		        if (w.wrapWidth>=0) // am wrap
		        {
			        if (p.x+charWidth>w.wrapWidth)
			        {
				        w.wordIsTruncated = true;
				        break;
			        }
		        } 		
		        // all ok
		        w.width = p.x+charWidth;

		        w.end++;
		        start++;		        
		        if ((ctype & CHAR_COMMAND_STOP)!=0)
			        break;
	        } while (w.end<w.charactersCount);
	        if (w.end==w.start)
	        {
		        w.width = p.x+charWidth;
		        w.end++; // macar un caracter (asta se poate intampla daca am word wrap
	        }
	        if (w.type == WORD_TYPE_EOL)
		        w.width = 0; // nu am size daca e \n - doar trec pe urmatoarea linie
        }
        void ClearGlyphLine(GlyphLine l)
        {
	        l.start = l.end = CHAR_INVALID_POSITION;
	        l.startFirstWord = l.endLastWord = CHAR_INVALID_POSITION;
	        l.size = 0;
        }
        void AddGlyphWordToGlyphLine(GlyphLine l,GlyphWord w)
        {
	        if (l.start==CHAR_INVALID_POSITION)	
		        l.start = w.start;
	        else
		        l.size+=w.charSpace;

	        l.end = w.end;
	        if (w.type == WORD_TYPE_WORD)
	        {
		        if (l.startFirstWord == CHAR_INVALID_POSITION)
			        l.startFirstWord = w.start;
		        l.endLastWord = w.end;
	        }
	        // recalculez pozitia fiecarui cuvant
	
	        for (uint tr=w.start;tr<w.end;tr++)
		        w.text[(int)tr].x+=l.size;
	        l.size+=w.width;	
        }
        float GetLineSizeWithWord(GlyphLine l,GlyphWord w)
        {
	        if (l.start == CHAR_INVALID_POSITION)
		        return w.width;
	        else
		        return l.size+w.charSpace+w.width;
        }
        float UpdateGlyphLinePosition(GlyphLine l,GlyphWord w,Alignament textAlign,uint wordWrapMode,float y)
        {
	        if (l.startFirstWord == CHAR_INVALID_POSITION)
		        return 0; // nu am nici un caracter in linie - nu am ce align sau repozitionare sa fac
	        float lineWidth = l.size;
	        float addX,addY,extraJustify;
	
	        extraJustify = 0;
	        if (wordWrapMode==CHAR_MODE_JUSTIFY) // JUSTIFY
	        {
		        lineWidth = w.text[(int)(l.endLastWord-1)].x-w.text[(int)(l.startFirstWord)].x;
		        //glyph = ((GlyphInformation*)w.font.GetGlyph(w.text[l.endLastWord-1].Code));
		        //if ((glyph!=NULL) && (glyph.Image!=NULL))
		        //	lineWidth += glyph.Image.Width*w.scale;
                lineWidth += w.text[(int)(l.endLastWord-1)].w;
		        extraJustify = (w.wrapWidth-lineWidth)/(l.endLastWord-l.startFirstWord);
		        lineWidth = w.wrapWidth;
		        // SIGUR nu e bine - trebuie corectat sa evit spatiile de la inceput (va merge bine daca linia chiar incepe cu un cuvant);
	        }
			if (wordWrapMode == CHAR_MODE_WORD_WRAP)
			{
				// scot extra spatiile
                lineWidth = w.text[(int)(l.endLastWord - 1)].x + w.text[(int)(l.endLastWord - 1)].w - w.text[(int)(l.startFirstWord)].x;
				//lineWidth = w->text[l->endLastWord - 1].x - w->text[l->startFirstWord].x;
				//glyph = ((GlyphInformation*)w->font->GetGlyph(w->text[l->endLastWord - 1].Code));
				//if ((glyph != NULL) && (glyph->Image != NULL))
				//	lineWidth += glyph->Image->Width*w->scale;
			}			
            addX = addY = 0;
	        ALIGN_COORD(ref addX,ref addY,0,0,lineWidth,0,textAlign);
	        GlyphLocation p;
	        for (uint tr=l.startFirstWord;tr<l.endLastWord;tr++)
	        {
                p = w.text[(int)tr];
		        p.x+=addX;
		        addX += extraJustify;
		        p.y = y;
	        }
	        return lineWidth;
        }
        void ComputeFontMetrics(FontResource font,AppResources Resources,ref float FontBaseLine,ref float FontDescend, ref float FontSpaceWidth, ref float FontCharacterSpacing)
        {
            float Baseline = 0;
            float Descent = 0;
            float SpaceWidth = 0;
            float CharacterSpacing = 0;
            Glyph gi;
            Image image;
            for (int tr = 0; tr < font.Glyphs.Count; tr++)
            {
                gi = font.Glyphs[tr];
                image = gi.Picture;
                if (image == null)
                    continue;
                Baseline = Math.Max(Baseline, image.Height * (1.0f - gi.BaseLine));
                Descent = Math.Max(Descent, gi.BaseLine * image.Height);
                if (gi.Code == font.WidthCharacter)
                {
                    SpaceWidth = image.Width * font.SpaceWidth;
                    CharacterSpacing = image.Width * font.SpaceBetweenChars;
                }
            }
            FontBaseLine = Baseline;
            FontDescend = Descent;
            if (FontSpaceWidth < 0)
                FontSpaceWidth = SpaceWidth;
            if (FontCharacterSpacing < 0)
                FontCharacterSpacing = CharacterSpacing;

        }
        void RecomputePositions(float forceScale,AppResources Resources)
        {
	        uint cPoz,nrLines;
	        float fontHeight,fontBaseline,usedCharsMaxHeight,tmpSize = 0,lineWidth;
	        uint wordWrap;
	        float charHeight,charDescend,addY = 0;
            float TextFontBaseLine = 0, TextFontDescent = 0, TextFontSpaceWidth = 0, TextFontCharacterSpacing = 0;
	        GlyphLocation g;
		
            if (Text.Length == 0)
                return;
            if (Resources.Fonts.ContainsKey(TextFont) == false)
                return;
            word.charactersCount = (uint)GlyphText.Count;
            word.text = GlyphText;
            word.font = Resources.Fonts[TextFont];
            TextFontSpaceWidth = SpaceWidth;
            TextFontCharacterSpacing = CharacterSpacing;
            ComputeFontMetrics(word.font, Resources, ref TextFontBaseLine, ref TextFontDescent, ref TextFontSpaceWidth, ref TextFontCharacterSpacing);
            if (TextFontSpaceWidth < 0)
                TextFontSpaceWidth = 0;
            if (TextFontCharacterSpacing < 0)
                TextFontCharacterSpacing = 0;
	        switch (fontSizeMethod)
	        {
		        case FontSizeMethod.Scale:
			        word.scale = FontSizeValue;
			        break;
		        case FontSizeMethod.Pixels:
                    word.scale = FontSizeValue / (TextFontBaseLine + TextFontDescent);
			        break;
		        case FontSizeMethod.PercentageOfHeight:
                    word.scale = (FontSizeValue * (ViewBottom - ViewTop)) / (TextFontBaseLine + TextFontDescent);
			        break;
		        case FontSizeMethod.ShrinkToFitWidth:
                    word.scale = FontSizeValue;
                    break;
		        case FontSizeMethod.ZoomToFitWidth:
			        word.scale = 1.0f;
			        break;
		        default:
			        word.scale = 1.0f;
			        break;
	        }
	        if (forceScale>0)
		        word.scale = forceScale;
   
	        word.charSpace = TextFontCharacterSpacing*word.scale;
            word.spaceWidth = TextFontSpaceWidth * word.scale;
	        word.wrapWidth = ViewRight-ViewLeft;
            
	
	        MaxTextWidth = 0;
	        MaxTextHeight = 0;

            fontHeight = (TextFontBaseLine + TextFontDescent) * word.scale * HeightPercentage;
            ComputedFontHeight = fontHeight; // DO NOT REMOVE - important la desenare

            fontBaseline = TextFontBaseLine * word.scale;
	
	        //coordType = (GAC_TYPE_COORDINATES)GET_BITFIELD(tp.Flags,4,4);
            if (Justify)
                wordWrap = CHAR_MODE_JUSTIFY;
            else if (WordWrap)
                wordWrap = CHAR_MODE_WORD_WRAP;
            else
                wordWrap = CHAR_MODE_NORMAL;	
	        //if ((tp.Flags & TEXT_PAINT_MODE_DOCK)!=0)
	        //{
		        ALIGN_OBJECT_TO_RECT(ref TextLeft,ref TextTop,ViewLeft,ViewTop,ViewRight,ViewBottom,0,0,textAlign);
	        //} else {
	        //	TRANSLATE_COORD_TO_PIXELS(TextLeft,TextTop,tp.X,tp.Y,ViewRight-ViewLeft,ViewBottom-ViewTop,coordType);
	        //	TextLeft+=ViewLeft;
	        //	TextTop+=ViewTop;
	        //	wordWrap = CHAR_MODE_NORMAL; // fara ViewRect nu am word wrap
	        //	word.wrapWidth = 0;
	        //}

	        cPoz = 0;
	        nrLines = 0;
	        usedCharsMaxHeight = 0;
	
	        ClearGlyphLine(line);
	        while (cPoz<word.charactersCount)
	        {
		        FindNextGlyphWord(Resources,cPoz,word);
		        usedCharsMaxHeight = Math.Max(usedCharsMaxHeight,word.height);
		        cPoz = word.end;
		        tmpSize = GetLineSizeWithWord(line,word);
		        if (word.type == WORD_TYPE_EOL)
		        {
			        // update la linia curenta			
			        lineWidth = UpdateGlyphLinePosition(line,word,textAlign,wordWrap,(float)nrLines);
                    MaxTextWidth = Math.Max(MaxTextWidth, lineWidth);
			        nrLines++;
			        // clear linia curenta
			        ClearGlyphLine(line);
			        // update			
			        continue;
		        }
		        if ((tmpSize>word.wrapWidth) && (wordWrap!=CHAR_MODE_NORMAL)) // si daca am word wrap
		        {
			        // update linia curenta
			        lineWidth = UpdateGlyphLinePosition(line,word,textAlign,wordWrap,(float)nrLines);
                    MaxTextWidth = Math.Max(MaxTextWidth, lineWidth);
			        nrLines++;
			        // curat linia
			        ClearGlyphLine(line);
			        // pun cuvantul in linia noua - stiu ca e spatiu pentru ca nu are cum sa fie cuvantul mai mare decat linia
			        if (word.type == WORD_TYPE_WORD)
				        AddGlyphWordToGlyphLine(line,word);
			        continue;
		        }
		        // altfel adaug word la linie
		        AddGlyphWordToGlyphLine(line,word);
	        }
	        if (line.start!=CHAR_INVALID_POSITION)
	        {
		        lineWidth = UpdateGlyphLinePosition(line,word,textAlign,wordWrap,(float)nrLines);
                MaxTextWidth = Math.Max(MaxTextWidth, lineWidth);
		        nrLines++;
	        }
	        // in y am acuma numarul de linii
	        MaxTextHeight = 0;
	        if (nrLines==1)
	        {
		        // am o singura linie - height-ul e dat de cel mai inalt caracter
		        MaxTextHeight = usedCharsMaxHeight;
		        // calculez baseline-ul
		        fontBaseline = 0;
		        for (cPoz=0;cPoz<word.charactersCount;cPoz++)
		        {
                    g = word.text[(int)cPoz];
			        if (g.Image!=null)
			        {
                        int index = word.font.CodeToIndex(g.Code);
                        if (index >= 0)
                        {
                            charDescend = word.font.Glyphs[index].BaseLine;
                            charHeight = g.Image.Height * word.scale;
                            tmpSize = (1.0f - charDescend) * charHeight;
                            fontBaseline = Math.Max(tmpSize, fontBaseline);
                        }
			        }
		        }
	        }
	        if (nrLines>1)
	        {
		        // am mai multe 
		        MaxTextHeight = (nrLines-1)*fontHeight+(TextFontBaseLine+TextFontDescent)*word.scale;
	        }

		
	        ALIGN_COORD(ref tmpSize,ref addY,0,TextTop,0,MaxTextHeight,textAlign);

            for (cPoz = 0; cPoz < word.charactersCount; cPoz++)
            {
                g = word.text[(int)cPoz];
                if (g.Image != null)
                {
                        int index = word.font.CodeToIndex(g.Code);
                        if (index >= 0)
                        {
                            charDescend = word.font.Glyphs[index].BaseLine;
                            charHeight = g.Image.Height * word.scale;
                            g.y = (g.y * fontHeight) + fontBaseline - charHeight * (1.0f - charDescend) + addY;
                            g.x += TextLeft;
                        } else {
							g.y = (g.y * fontHeight) + fontBaseline + addY;
							g.x += TextLeft;							
						}
                }
                else
                {
                    g.y = (g.y * fontHeight) + fontBaseline + addY;
                    g.x += TextLeft;	
                }
            }

	        // actualizez si TextLeft si TextUp
	        ALIGN_COORD(ref TextLeft,ref TextTop,TextLeft,TextTop,MaxTextWidth,MaxTextHeight,textAlign);
	        //tp.PaintScale = word.scale;

	        // recalculez daca am un anumit font width
	        switch (fontSizeMethod)
	        {
		        case FontSizeMethod.ShrinkToFitWidth:
			        if (forceScale<0) // am rulat prima data
			        {
				        if ((MaxTextWidth>(ViewRight-ViewLeft)) && (ViewRight>ViewLeft) && (MaxTextWidth>0))
				        {
                            RecomputePositions((ViewRight - ViewLeft) / (MaxTextWidth), Resources);
				        }
			        }
			        break;
		        case FontSizeMethod.ZoomToFitWidth:
			        if (forceScale<0) // am rulat prima data
			        {
				        if ((ViewRight>ViewLeft) && (MaxTextWidth>0))
				        {
					        RecomputePositions((ViewRight-ViewLeft)/(MaxTextWidth),Resources);
				        }
			        }
			        break;
	        }
        }


        
        #region Internal Vars
        private bool Recompute = true;
        private List<GlyphLocation> GlyphText = new List<GlyphLocation>();
        private string Text = "";
        private GlyphWord word = new GlyphWord();
        private GlyphLine line = new GlyphLine();
        private Alignament textAlign = Alignament.TopLeft;
        private string TextFont = "";
        private float HeightPercentage = 1.0f;

        public Color MissingCharacterColor = Color.Red;
        private SolidBrush err = new SolidBrush(Color.Red);

        private float MaxTextWidth = 0, MaxTextHeight = 0,ComputedFontHeight;
        private float TextLeft = 0, TextTop = 0;
        private float ViewLeft = 0, ViewRight = 0, ViewTop = 0, ViewBottom = 0;
        private float FontSizeValue = 1; // era 0 implicit
        private float CharacterSpacing = -1; 
        private float SpaceWidth = -1;
        private bool WordWrap = false, Justify = false;
        private FontSizeMethod fontSizeMethod = FontSizeMethod.Scale;

        private AnimO.RuntimeContext rc = new AnimO.RuntimeContext();
        private BlendingMode blendMode = BlendingMode.None;
        #endregion

        #region Atribute
        [XmlIgnore(), Description(""), Category("General"), DisplayName("Word Wrap")]
        public bool _WordWrap
        {
            get { return WordWrap; }
            set { SetWordWrap(value); }
        }
        [XmlIgnore(), Description(""), Category("General"), DisplayName("Justify")]
        public bool _Justify
        {
            get { return Justify; }
            set { SetJustify(value); }
        }
        [XmlIgnore(), Description(""), Category("General"), DisplayName("Alignament")]
        public Alignament _textAlign
        {
            get { return textAlign; }
            set { SetAlignament(value); }
        }
        [XmlIgnore(), Description(""), Category("General"), DisplayName("Size Method")]
        public FontSizeMethod _fontSizeMethod
        {
            get { return fontSizeMethod; }
            set { SetFontSize(value, FontSizeValue); }
        }
        [XmlIgnore(), Description(""), Category("General"), DisplayName("Size Value")]
        public float _FontSizeValue
        {
            get { return FontSizeValue; }
            set { SetFontSize(fontSizeMethod, value); }
        }
        [XmlIgnore(), Description(""), Category("General"), DisplayName("Lines space")]
        public float _LineSpaces
        {
            get { return HeightPercentage-1; }
            set { SetLineSpace(value); }
        }
        [XmlIgnore(), Description(""), Category("Misc"), DisplayName("Invalid Character")]
        public Color _MissingCharacterColor
        {
            get { return MissingCharacterColor; }
            set { 
                if (MissingCharacterColor != value)
                {
                    MissingCharacterColor = value;
                    Recompute = true; 
                }
            }
        }

        [XmlIgnore(), Description("Layout rectangle Width"), Category("Preview"), DisplayName("Width")]
        public int _Width
        {
            get { return (int)(ViewRight - ViewLeft); }
        }
        [XmlIgnore(), Description("Layout rectangle Width"), Category("Preview"), DisplayName("Height")]
        public int _Height
        {
            get { return (int)(ViewBottom - ViewTop); }
        }
        [XmlIgnore(), Description(""), Category("Preview"), DisplayName("Left")]
        public int _Left
        {
            get { return (int)(ViewLeft); }
            set { ViewLeft = value; }
        }
        [XmlIgnore(), Description(""), Category("Preview"), DisplayName("Top")]
        public int _Top
        {
            get { return (int)(ViewTop); }
            set { ViewTop = value; }
        }
        [XmlIgnore(), Description(""), Category("Preview"), DisplayName("Right")]
        public int _Right
        {
            get { return (int)(ViewRight); }
            set { ViewRight = value; }
        }
        [XmlIgnore(), Description(""), Category("Preview"), DisplayName("Bottom")]
        public int _Bottom
        {
            get { return (int)(ViewBottom); }
            set { ViewBottom = value; }
        }
        #endregion

        public void SetPosition(float left, float top, float right, float bottom)
        {
            if ((left != ViewLeft) || (top != ViewTop) || (right != ViewRight) || (bottom != ViewBottom))
            {
                ViewLeft = left;
                ViewTop = top;
                ViewRight = right;
                ViewBottom = bottom;
                Recompute = true;
            }
        }
        public void SetAlignament(Alignament align)
        {
            if (align != textAlign)
            {
                textAlign = align;
                Recompute = true;
            }
        }
        public void SetFont(string name)
        {
            if (name != TextFont)
            {
                TextFont = name;
                Recompute = true;
            }
        }
        public void SetWordWrap(bool value)
        {
            if (value != WordWrap)
            {
                WordWrap = value;
                Recompute = true;
            }
        }
        public void SetJustify(bool value)
        {
            if (value != Justify)
            {
                Justify = value;
                Recompute = true;
            }
        }
        public void SetFontSize(FontSizeMethod method, float value)
        {
            if ((method != fontSizeMethod) || (value != FontSizeValue))
            {
                fontSizeMethod = method;
                FontSizeValue = value;
                Recompute = true;
            }
        }
        public void SetCharacterSpacing(float value)
        {
            if (value != CharacterSpacing)
            {
                CharacterSpacing = value;
                Recompute = true;
            }
        }
        public void SetSpaceWidth(float value)
        {
            if (value != SpaceWidth)
            {
                SpaceWidth = value;
                Recompute = true;
            }
        }
        public void SetText(string text, bool useResources, AppResources Resources)
        {
            string ss = text;
            if (useResources)
            {
                if (Resources.Strings.ContainsKey(ss))
                    ss = Resources.Strings[ss].Get(Resources.Lang).Replace("\\n", "\n");
                else
                    ss = "";
            }
            if (ss != Text)
            {
                Text = ss;                
                GlyphText.Clear();
                foreach (char ch in ss)
                {
                    GlyphLocation item = new GlyphLocation();
                    item.Code = ch;
                    item.Visibility = true;
                    GlyphText.Add(item);
                }
                Recompute = true;
            }
        }
        public void SetLineSpace(float value)
        {
            if (value + 1 != HeightPercentage)
            {
                HeightPercentage = value + 1;
                Recompute = true;
            }
        }
        public void SetBlending(BlendingMode mode, int color, float alpha)
        {
            blendMode = mode;
            rc.ColorBlending = AnimO.RuntimeContext.BlendModeToColor(mode, color, alpha);
        }
        public void ForceRecompute()
        {
            Recompute = true;
        }
        public void Paint(AnimO.Canvas canvas,AppResources Resources)
        {
            if (Recompute)
            {
                RecomputePositions(-1.0f, Resources);
            }
            Recompute = false;
            err.Color = MissingCharacterColor;
            //float canvas_scale = canvas.GetScale();
            foreach (GlyphLocation gl in GlyphText)
            {
                if ((gl.Code == ' ') || (gl.Code == '\t') || (gl.Code == '\n') || (gl.Code == '\r'))
                    continue;
                if (gl.Visibility == false)
                    continue;
                if (gl.Image != null)
                {
                    //g.g.DrawImage(gl.Image, gl.x, gl.y, gl.w, gl.h);
                    rc.Image = gl.Image;
                    //rc.X_Percentage = canvas.ConvertXAxisToPercentage(gl.x) * canvas.GetScale();
                    //rc.Y_Percentage = canvas.ConvertYAxisToPercentage(gl.y) * canvas.GetScale();
                    rc.Align = Alignament.TopLeft;

                    //if (this.fontSizeMethod == FontSizeMethod.Scale)
                    //{
                    //    rc.ScaleWidth = rc.ScaleHeight = this.FontSizeValue;
                    //}
                    //else
                    //{
                    //    rc.ScaleWidth = gl.w / (float)rc.Image.Width;
                    //    rc.ScaleHeight = gl.h / (float)rc.Image.Height;
                    //}
                    rc.ScreenRect.X = gl.x;
                    rc.ScreenRect.Y = gl.y;
                    rc.ScreenRect.Width = gl.w;
                    rc.ScreenRect.Height = gl.h;
                    rc.ScaleWidth = rc.ScaleHeight = 1.0f; // safety

                    canvas.DrawImage(rc);
                }
                else
                {
                    //g.g.FillRectangle(err, gl.x, gl.y - ComputedFontHeight, gl.w, ComputedFontHeight);
                }
            }
        }
        public void Paint_Old(GraphicContext g, AppResources Resources)
        {
            if (Recompute)
            {
                //GlyphText.Clear();
                //string ss = Text;
                //if (UseResources)
                //{
                //    if (Resources.Strings.ContainsKey(ss))
                //        ss = Resources.Strings[ss].Get(Resources.Lang);
                //    else
                //        ss = "";
                //}
                //GlyphText.Clear();
                //foreach (char ch in ss)
                //{
                //    GlyphLocation item = new GlyphLocation();
                //    item.Code = ch;
                //    GlyphText.Add(item);
                //}

                RecomputePositions(-1.0f, Resources);
            }
            Recompute = false;
            err.Color = MissingCharacterColor;
            foreach (GlyphLocation gl in GlyphText)
            {
                if ((gl.Code == ' ') || (gl.Code == '\t') || (gl.Code == '\n') || (gl.Code == '\r'))
                    continue;
                if (gl.Image != null)
                {
                    g.g.DrawImage(gl.Image, gl.x, gl.y, gl.w, gl.h);
                }
                else
                {
                    g.g.FillRectangle(err, gl.x, gl.y - ComputedFontHeight, gl.w, ComputedFontHeight);
                }
            }
        }


        public void Check(AppResources Resources,Project prj,string variableName,string Language )
        {
            GlyphText.Clear();
            string ss = Text;
            foreach (char ch in ss)
            {
                GlyphLocation item = new GlyphLocation();
                item.Code = ch;
                GlyphText.Add(item);
            }
            bool outside = false;
            RecomputePositions(-1.0f, Resources);
            foreach (GlyphLocation gl in GlyphText)
            {
                if ((gl.Code == ' ') || (gl.Code == '\t') || (gl.Code == '\n') || (gl.Code == '\r'))
                    continue;
                if (gl.Image == null)
                    prj.EC.AddError(String.Format("String '{0}' can not render character '{1}' for language '{2}' and font '{3}' !",variableName,(char)gl.Code,Language,TextFont));
                else
                {
                    if ((gl.x>=ViewLeft) && (gl.x+gl.w<=ViewRight) && (gl.y>=ViewTop) && (gl.y+gl.h<=ViewBottom))
                    {

                    }
                    else
                    {
                        outside = true;
                    }
                }
            }
            if (outside)
                prj.EC.AddError(String.Format("String '{0}' can not be fit into the preview rectangle for language '{1}' and font '{2}' !", variableName, Language, TextFont));
        }
        public void Create(StringTemplatePreview stp)
        {
            SetPosition(stp.textLeft, stp.textTop, stp.textRight, stp.textBottom);
            SetAlignament(stp.Align);
            SetWordWrap(stp.WordWrap);
            SetJustify(stp.Justify);
            _LineSpaces = stp.LineSpace;
            SetFontSize(stp.SizeMethod, stp.FontSizeValue);
            _MissingCharacterColor = Color.FromArgb(stp.MissingCharColor);
            SetFont(stp.FontName);
        }

        public void SetCharactesVisibility(int startIndex, int endIndex, bool visibility)
        {
            if (endIndex>GlyphText.Count)
                endIndex = GlyphText.Count;
            if (startIndex < 0)
                startIndex = 0;
            for (int tr=startIndex;tr<endIndex;tr++)
            {
                GlyphText[tr].Visibility = visibility;
            }
        }

        public int GetTextLength() { return GlyphText.Count; }
    }
}
