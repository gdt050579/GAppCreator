using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ScintillaNET;
using SharpGL;
using SharpGL.VertexBuffers;
using SharpGL.Shaders;
using SharpGL.SceneGraph.Assets;

namespace GAppCreator
{
    public partial class ShaderEditor : Form
    {
        private class PreviewSettings
        {
            public float imageWidth=1, imageHeight=1, imageCenterX=0.5f, imageCenterY=0.5f;
            public float imageScale=1;
            public bool imageUseScale=true;
            public float rectTop=0.25f, rectLeft=0.25f, rectBottom=0.75f, rectRight=0.75f;
            public float lineX1 = 0, lineY1 = 0, lineX2 = 1, lineY2 = 1;
            public float lineWidth = 1;

            #region Atribute
            [Description("Image Width (in percentages)"), Category("Image"), DisplayName("Width")]
            public float _imageWidth
            {
                get { return imageWidth; }
                set { if (value > 0) imageWidth = value; }
            }
            [Description("Image Height (in percentages)"), Category("Image"), DisplayName("Height")]
            public float _imageHeight
            {
                get { return imageHeight; }
                set { if (value > 0) imageHeight = value; }
            }
            [Description("Center image position (X axes) (in percentages)"), Category("Image"), DisplayName("Center X")]
            public float _centerX
            {
                get { return imageCenterX; }
                set { imageCenterX = value; }
            }
            [Description("Center image position (Y axes) (in percentages)"), Category("Image"), DisplayName("Center Y")]
            public float _centerY
            {
                get { return imageCenterY; }
                set { imageCenterY = value; }
            }
            [Description("Image scale"), Category("Image"), DisplayName("Scale")]
            public float _imageScale
            {
                get { return imageScale; }
                set { if (value > 0) imageScale = value; }
            }
            [Description("Weather to use Scale parmeter or Width and Height parameter"), Category("Image"), DisplayName("Use scale")]
            public bool _imageUseScale
            {
                get { return imageUseScale; }
                set { imageUseScale = value; }
            }
            [Description("Position of the rectangle (in screen percentages)"), Category("Rectangle"), DisplayName("Left")]
            public float _rectLeft
            {
                get { return rectLeft; }
                set { rectLeft = value; }
            }
            [Description("Position of the rectangle (in screen percentages)"), Category("Rectangle"), DisplayName("Top")]
            public float _rectTop
            {
                get { return rectTop; }
                set { rectTop = value; }
            }
            [Description("Position of the rectangle (in screen percentages)"), Category("Rectangle"), DisplayName("Right")]
            public float _rectRight
            {
                get { return rectRight; }
                set { rectRight = value; }
            }
            [Description("Position of the rectangle (in screen percentages)"), Category("Rectangle"), DisplayName("Bottom")]
            public float _rectBottom
            {
                get { return rectBottom; }
                set { rectBottom = value; }
            }
            [Description("Position of the line (in screen percentages)"), Category("Line"), DisplayName("X1")]
            public float _lineX1
            {
                get { return lineX1; }
                set { lineX1 = value; }
            }
            [Description("Position of the line (in screen percentages)"), Category("Line"), DisplayName("Y1")]
            public float _lineY1
            {
                get { return lineY1; }
                set { lineY1 = value; }
            }
            [Description("Position of the line (in screen percentages)"), Category("Line"), DisplayName("X2")]
            public float _lineX2
            {
                get { return lineX2; }
                set { lineX2 = value; }
            }
            [Description("Position of the line (in screen percentages)"), Category("Line"), DisplayName("Y2")]
            public float _lineY2
            {
                get { return lineY2; }
                set { lineY2 = value; }
            }
            [Description("Line width"), Category("Line"), DisplayName("Width")]
            public float _lineWidth
            {
                get { return lineWidth; }
                set { if (value > 0) lineWidth = value; }
            }
            #endregion
        };
        private class OGLPoint
        {
            public float X,Y;
        };
        private class OGLSize
        {
            public float Width,Height;
        };
        enum PreviewShapeType
        {
            None,
            Rectangle,
            Line,
            Image
        };
        private class UniformValue
        {
            public float v1, v2, v3, v4;
            public int uniformIndex;
            public ShaderVariable Variable;

            #region Atribute
            [Description(""), Category("Raw Data"), DisplayName("v1")]
            public float _v1
            {
                get { return v1; }
                set { v1 = value; }
            }
            [Description(""), Category("Raw Data"), DisplayName("v2")]
            public float _v2
            {
                get { return v2; }
                set { v2 = value; }
            }
            [Description(""), Category("Raw Data"), DisplayName("v3")]
            public float _v3
            {
                get { return v3; }
                set { v3 = value; }
            }
            [Description(""), Category("Raw Data"), DisplayName("v4")]
            public float _v4
            {
                get { return v4; }
                set { v4 = value; }
            }
            [Description(""), Category("Color"), DisplayName("Color")]
            public Color _color
            {
                get { return Color.FromArgb(((int)(Math.Abs(v4) * 255)) & 0xFF, ((int)(Math.Abs(v1) * 255)) & 0xFF, ((int)(Math.Abs(v2) * 255)) & 0xFF, ((int)(Math.Abs(v3) * 255)) & 0xFF); }
                set { v4 = value.A / 255.0f; v1 = value.R / 255.0f; v2 = value.G / 255.0f; v3 = value.B / 255.0f; }
            }
            [Description(""), Category("General"), DisplayName("ID")]
            public int _id
            {
                get { return uniformIndex; }
            }
            [Description(""), Category("General"), DisplayName("Type")]
            public string _type
            {
                get { return Variable.OpenGLType; }
            }
            [Description(""), Category("GAC Variable"), DisplayName("Variable")]
            public ShaderVariable _variable
            {
                get { return Variable; }
                set { Variable = value;  }
            }
            #endregion

            public void UpdateValue(OpenGL gl)
            {
                switch (Variable.OpenGLType)
                {
                    case "float": gl.Uniform1(uniformIndex, v4); break;
                    case "vec2": gl.Uniform2(uniformIndex, v1,v2); break;
                    case "vec3": gl.Uniform3(uniformIndex, v1,v2,v3); break;
                    case "vec4": gl.Uniform4(uniformIndex, v1,v2,v3,v4); break;
                }
            }

        };

        Scintilla editVertex, editFragment;
        OpenGLControl preview = new OpenGLControl();
        VertexBufferArray vba = new VertexBufferArray();
        ShaderProgram program = new ShaderProgram();
        OpenGL gl;
        Project prj;
        ShaderResource sr;
        Dictionary<ImageResource, Bitmap> Images;
        Dictionary<string, UniformValue> Uniforms = new Dictionary<string, UniformValue>();
        PreviewSettings psettings = new PreviewSettings();
        PreviewShapeType shapeType = PreviewShapeType.None;
        Texture texImage = new Texture();
        Bitmap previewImage;
        bool showPreview;
        float []textureCoords = new float[]{0,0,0,1,1,1,0,0,1,1,1,0};
        List<ShaderVariable> UniformsList = new List<ShaderVariable>();
                                         // 0,1,2,3,4,5,6,7,8,9,A,B
        Dictionary<uint, string> attrib = new Dictionary<uint, string>();

        public ShaderEditor(Project p,ShaderResource r,ImageList smallImageList)
        {
            InitializeComponent();
            prj = p;
            sr = r;
            showPreview = false;
            Images = new Dictionary<ImageResource, Bitmap>();
            foreach (GenericResource gr in prj.Resources)
            {
                if (gr.GetType() != typeof(ImageResource))
                    continue;
                if (gr.IsBaseResource() == false)
                    continue;
                Bitmap bmp = gr.CreateIcon(-1, -1);
                if (bmp != null)
                    Images[(ImageResource)gr] = bmp;
            }

            lstShapes.SmallImageList = smallImageList;
            propPreviewSettings.SelectedObject = psettings;
            PopulateShapes();

            editVertex = new Scintilla();
            editFragment = new Scintilla();
            tabPageVertexShader.Controls.Add(editVertex);
            tabPageFragmentShader.Controls.Add(editFragment);
            editVertex.Dock = DockStyle.Fill;
            editFragment.Dock = DockStyle.Fill;

            PrepareScintilla(editVertex);
            PrepareScintilla(editFragment);

            pnlPreview.Controls.Add(preview);
            preview.OpenGL.Create(SharpGL.Version.OpenGLVersion.OpenGL2_1, RenderContextType.FBO, pnlPreview.Width, pnlPreview.Height, 32, null);
            preview.Dock = DockStyle.Fill;
            gl = preview.OpenGL;
            preview.OpenGLInitialized += new EventHandler(preview_OpenGLInitialized);
            preview.OpenGLDraw += new RenderEventHandler(preview_OpenGLDraw);

            vba.Create(gl);
            SetShadersCode(sr.GetVertexShader(),sr.GetFragmentShader());

            showPreview = true;
            shapeType = PreviewShapeType.Rectangle;
            InitializeVertexBufferArray();
            Uniforms.Clear();
            foreach (ShaderVariable sv in sr.Uniforms)
            {
                UniformValue uv = new UniformValue();
                uv.Variable = sv;
                Uniforms[sv.Name] = uv;
            }
            UpdateProgram();       
            RefreshPreview();

            OnPreviewSizeChanged(null, null);
            
        }

        void preview_OpenGLInitialized(object sender, EventArgs e)
        {
            gl.Enable(OpenGL.GL_BLEND);
            gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);           
        }
        private void SetShadersCode(string vertex, string fragment)
        {
            editVertex.Text = vertex;
            editFragment.Text = fragment;
            RefreshVariableList();
        }
        private void AddShape(string name, string iconKey, string group,bool select,object tag)
        {
            ListViewItem lvi = new ListViewItem(name);
            if (iconKey!=null)
                lvi.ImageKey = iconKey;
            if (group!=null)
                lvi.Group = lstShapes.Groups[group];
            lvi.Tag = tag;
            lstShapes.Items.Add(lvi);
            if (select)
                lvi.Selected = true;
        }
        private void PopulateShapes()
        {
            lstShapes.Items.Clear();
            AddShape("Rectangle", null, "shapes",true,null);
            AddShape("Line", null, "shapes", false,null);
            foreach (ImageResource image in Images.Keys)
            {
                AddShape(image.GetResourceVariableName(), image.GetIconImageListKey(), "images",false,image);
            }
        }
        void preview_OpenGLDraw(object sender, RenderEventArgs args)
        {
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT | OpenGL.GL_STENCIL_BUFFER_BIT);
            gl.Enable(OpenGL.GL_BLEND);
            gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);

            if (showPreview)
            {
                program.Bind(gl);
                vba.Bind(gl);

                foreach (string name in Uniforms.Keys)
                    Uniforms[name].UpdateValue(gl);
                
                switch (shapeType)
                {
                    case PreviewShapeType.Rectangle:
                        gl.DrawArrays(OpenGL.GL_TRIANGLES, 0, 6);
                        break;
                    case PreviewShapeType.Image:                        
                        gl.BindTexture(OpenGL.GL_TEXTURE_2D, texImage.TextureName);
                        gl.DrawArrays(OpenGL.GL_TRIANGLES, 0, 6);
                        break;
                    case PreviewShapeType.Line:
                        gl.LineWidth(psettings.lineWidth);
                        gl.DrawArrays(OpenGL.GL_LINES, 0, 2);
                        break;
                }
                vba.Unbind(gl);
                program.Unbind(gl);
            }
        }
        private void AddCompileInfo(string text, Color colFore, Color colBack, bool ensureVisible, string group)
        {
            ListViewItem lvi = new ListViewItem(text);
            lvi.ForeColor = colFore;
            lvi.BackColor = colBack;
            lvi.Tag = group;
            lvi.Group = lstCompileInfo.Groups[group];
            if (ensureVisible)
                lvi.EnsureVisible();
            lstCompileInfo.Items.Add(lvi);
            if (ensureVisible)
                lvi.EnsureVisible();
        }
        private void AddErrorList(string text, string group)
        {
            string[] lines = text.Replace("\r", "\n").Replace("\n\n", "\n").Split('\n');
            foreach (string line in lines)
            {
                if (line.Length == 0)
                    continue;
                if (line.ToLower().Contains("warning"))
                {
                    AddCompileInfo("    " + line, Color.Brown, Color.White, false, group);
                    continue;
                }
                if (line.ToLower().Contains("error"))
                {
                    AddCompileInfo("    " + line, Color.Red, Color.White, true, group);
                    continue;
                }
                AddCompileInfo("    " + line, Color.Black, Color.White, false, group);
            }
        }
        private void UpdateProgram()
        {
            lstCompileInfo.Items.Clear();
            Dictionary<string, UniformValue> tmpUniforms = new Dictionary<string, UniformValue>();
            attrib.Clear();
            List<ShaderVariable> l = ShaderResource.CreateVariableList(editVertex.Text, true);
            for (uint tr = 0; tr < l.Count; tr++)
                attrib[tr] = l[(int)tr].Name;
            
            bool res = program.Create2(gl, editVertex.Text, editFragment.Text, attrib);
            AddErrorList(program.VertexInfoLog, "vertex");
            AddErrorList(program.FragmentInfoLog, "fragment");
            if (res==false)
            {
                AddErrorList(program.LinkInfoLog, "linker");
                return;
            }
            // testez si uniformii
            program.Bind(gl);
            l = ShaderResource.CreateVariableList(editFragment.Text, false);
            for (int tr=0;tr<l.Count;tr++)
            {
                int id = gl.GetUniformLocation(program.ShaderProgramObject, l[tr].Name);
                if (id >= 0)
                {
                    UniformValue uv = new UniformValue();
                    uv.uniformIndex = id;
                    uv.Variable = l[tr];
                    tmpUniforms[l[tr].Name] = uv;
                }
                else
                {
                    AddErrorList("(warning): Unable to link uniform '" + l[tr].Name + "' => "+id.ToString(), "linker");
                }
            }
            program.Unbind(gl);
            if (res)
            {
                // updatez so lista de variabile
                foreach (string name in tmpUniforms.Keys)
                {
                    if (Uniforms.ContainsKey(name))
                    {
                        Uniforms[name].uniformIndex = tmpUniforms[name].uniformIndex;
                        if (Uniforms[name].Variable.OpenGLType != tmpUniforms[name].Variable.OpenGLType)
                        {
                            Uniforms[name].Variable = tmpUniforms[name].Variable;
                        }
                    }
                    else
                    {
                        Uniforms[name] = tmpUniforms[name];
                    }
                }
                // sterg ce e in plus
                List<string> toDelete = null;
                foreach (string name in Uniforms.Keys)
                {
                    if (tmpUniforms.ContainsKey(name)==false)
                    {
                        if (toDelete == null)
                            toDelete = new List<string>();
                        toDelete.Add(name);
                    }
                }
                if (toDelete!=null)
                {
                    foreach (string name in toDelete)
                        Uniforms.Remove(name);
                }
                AddCompileInfo("Compile OK !", Color.Green, Color.White, true, "linker");
                RefreshVariableList();
                RefreshPreview();
            }

        }
        private void RefreshPreview()
        {
            if (showPreview)
                preview.DoRender();
        }
        private OGLPoint PositionToOpenGL(float x, float y)
        {
            OGLPoint o = new OGLPoint();
            o.X = x * 2 - 1;
            o.Y = 1 - y * 2;
            return o;
        }
        private OGLSize SizeToOpenGL(float width, float height)
        {
            OGLSize o = new OGLSize();
            o.Width = width; //*2;
            o.Height = height; //*2;
            return o;
        }
        private void InitializeVertexBufferArray()
        {
            if (showPreview == false)
                return;
            vba.Bind(gl);

            VertexBuffer vb = new VertexBuffer();
            vb.Create(gl);
            vb.Bind(gl);

            OGLPoint p1, p2;
            OGLSize sz;
            float[] v;
            switch (shapeType)
            {
                case PreviewShapeType.Line:
                    p1 = PositionToOpenGL(psettings.lineX1, psettings.lineY1);
                    p2 = PositionToOpenGL(psettings.lineX2, psettings.lineY2);
                    v = new float[4];
                    v[0] = p1.X;
                    v[1] = p1.Y;
                    v[2] = p2.X;
                    v[3] = p2.Y;                    
                    vb.SetData(gl, 0, v, false, 2);
                    break;
                case PreviewShapeType.Rectangle:
                    p1 = PositionToOpenGL(psettings.rectLeft, psettings.rectTop);
                    p2 = PositionToOpenGL(psettings.rectRight, psettings.rectBottom);
                    v = new float[12];
                    v[0] = v[2] = v[6] = p1.X;
                    v[1] = v[7] = v[11] = p2.Y;
                    v[3] = v[5] = v[9] = p1.Y;
                    v[4] = v[8] = v[10] = p2.X;
                    vb.SetData(gl, 0, v, false, 2);
                    break;
                case PreviewShapeType.Image:
                    p1 = PositionToOpenGL(psettings.imageCenterX, psettings.imageCenterY);
                    if (psettings.imageUseScale)
                    {
                        sz = SizeToOpenGL((previewImage.Width * psettings.imageScale) / (float)pnlPreview.Width, (previewImage.Height * psettings.imageScale) / (float)pnlPreview.Height);
                    }
                    else
                    {
                        sz = SizeToOpenGL(psettings.imageWidth, psettings.imageHeight);
                    }
                    v = new float[12];
                    v[0] = v[2] = v[6] = p1.X - sz.Width;
                    v[1] = v[7] = v[11] = p1.Y + sz.Height;
                    v[3] = v[5] = v[9] = p1.Y - sz.Height;
                    v[4] = v[8] = v[10] = p1.X + sz.Width;
                    
                    vb.SetData(gl, 0, v, false, 2);
                                   
                    VertexBuffer vb2 = new VertexBuffer();
                    vb2.Create(gl);
                    vb2.Bind(gl);
                    vb2.SetData(gl, 1, textureCoords, false, 2);

                    break;
            }

            vba.Unbind(gl);
        }
        private void PrepareScintilla(Scintilla edit)
        {

            edit.Margins[0].Width = 30;
            edit.Margins[1].Width = 12;
            edit.Margins[1].Type = MarginType.Symbol;
            edit.Margins[2].Width = 20;
            edit.Margins[2].IsFoldMargin = true;
            edit.Margins[2].IsMarkerMargin = false;
            edit.Margins[2].IsClickable = true;


            edit.EndOfLine.Mode = EndOfLineMode.LF;

            //edit.ConfigurationManager.Language = "c#";
            edit.IsBraceMatching = true;

            edit.Indentation.TabWidth = 6;
            edit.Indentation.ShowGuides = true;
            edit.Indentation.SmartIndentType = SmartIndent.CPP;

            edit.Folding.IsEnabled = true;
            edit.Folding.UseCompactFolding = true;

            edit.Lexing.Lexer = ScintillaNET.Lexer.Cpp;


            //Styles[edit.Lexing.StyleNameMap["COMMENT"]].ForeColor = System.Drawing.Color.LightGray;
            edit.Styles[edit.Lexing.StyleNameMap["COMMENT"]].ForeColor = System.Drawing.Color.Gray;
            edit.Styles[edit.Lexing.StyleNameMap["COMMENTLINE"]].ForeColor = System.Drawing.Color.Gray;
            edit.Styles[edit.Lexing.StyleNameMap["WORD"]].ForeColor = System.Drawing.Color.Blue;
            edit.Styles[edit.Lexing.StyleNameMap["WORD"]].Bold = true;
            edit.Styles[edit.Lexing.StyleNameMap["STRING"]].ForeColor = System.Drawing.Color.Red;
            edit.Styles[edit.Lexing.StyleNameMap["CHARACTER"]].ForeColor = System.Drawing.Color.DarkRed;
            edit.Styles[edit.Lexing.StyleNameMap["CONTROLCHAR"]].ForeColor = System.Drawing.Color.DarkRed;
            edit.Styles[edit.Lexing.StyleNameMap["PREPROCESSOR"]].ForeColor = System.Drawing.Color.Green;
            edit.Styles[edit.Lexing.StyleNameMap["OPERATOR"]].ForeColor = System.Drawing.Color.Black;
            edit.Styles[edit.Lexing.StyleNameMap["WORD2"]].ForeColor = System.Drawing.Color.DarkCyan;
            edit.Styles[edit.Lexing.StyleNameMap["NUMBER"]].ForeColor = System.Drawing.Color.DarkMagenta;
            edit.Styles[edit.Lexing.StyleNameMap["NUMBER"]].Bold = true;
            edit.Styles[edit.Lexing.StyleNameMap["BRACEBAD"]].ForeColor = System.Drawing.Color.White;
            edit.Styles[edit.Lexing.StyleNameMap["BRACEBAD"]].BackColor = System.Drawing.Color.DarkRed;
            edit.Styles[edit.Lexing.StyleNameMap["BRACEBAD"]].Bold = true;
            edit.Styles[edit.Lexing.StyleNameMap["BRACELIGHT"]].ForeColor = System.Drawing.Color.Yellow;
            edit.Styles[edit.Lexing.StyleNameMap["BRACELIGHT"]].BackColor = System.Drawing.Color.Green;
            edit.Styles[edit.Lexing.StyleNameMap["GLOBALCLASS"]].ForeColor = System.Drawing.Color.DarkMagenta;
            edit.Styles[edit.Lexing.StyleNameMap["GLOBALCLASS"]].Bold = true;

            System.Drawing.Font fnt = new System.Drawing.Font("Consolas", 10);
            for (int tr = 0; tr < edit.Styles.GetEndStyled(); tr++)
                edit.Styles[tr].Font = fnt;

            edit.Caret.HighlightCurrentLine = true;
            edit.Caret.CurrentLineBackgroundColor = System.Drawing.Color.AliceBlue;

            edit.Lexing.SetKeywords(0, "const,if,for,while,struct,break,case,switch,continue,return,do,else,attribute,lowp,mediump,highp,varying,invariant,uniform,precision".Replace(",", "\r\n"));
            edit.Lexing.SetKeywords(1, "void,int,float,bool,vec2,vec3,vec4,bvec2,bvec3,bvec4,ivec2,ivec3,ivec4,mat2,mac3,mat4,sampler2D".Replace(",", "\r\n"));
            edit.Lexing.SetKeywords(3, "gl_FragColor,gl_FragCoord,gl_Position,smoothstep,distance,normalize,abs,sin,cos,max,min,mod,floor,pow,atan,length,dot".Replace(",", "\r\n"));

            edit.Indicators[0].Style = IndicatorStyle.RoundBox;
            edit.Indicators[0].Color = System.Drawing.Color.Red;
            edit.Indicators[1].Style = IndicatorStyle.Squiggle;
            edit.Indicators[1].Color = System.Drawing.Color.Red;

            edit.Markers[0].Symbol = MarkerSymbol.Circle;
            edit.Markers[0].BackColor = System.Drawing.Color.Red;

            edit.AutoComplete.IsCaseSensitive = false;
            edit.AutoComplete.DropRestOfWord = false;

            edit.SelectionChanged += new EventHandler(Editor_SelectionChanged);
        }
        void Editor_SelectionChanged(object sender, EventArgs e)
        {
            Scintilla edit = (Scintilla)sender;
            edit.GetRange().ClearIndicator(0);
            if (edit.Selection.Start < edit.Selection.End)
            {
                edit.FindReplace.Flags = ScintillaNET.SearchFlags.WholeWord;
                foreach (Range r in edit.FindReplace.FindAll(edit.Selection.Text))
                    r.SetIndicator(0);
            }
        }
        private void OnSelectedVariableChanged(object sender, EventArgs e)
        {
            if (lstVariables.SelectedItems.Count == 1)
            {
                propValues.SelectedObject = (UniformValue)lstVariables.SelectedItems[0].Tag;
            } else {
                propValues.SelectedObject = null;
            }
        }

        private void OnCompile(object sender, EventArgs e)
        {
            UpdateProgram(); 
        }
        private void OnChangeUniformValue(object s, PropertyValueChangedEventArgs e)
        {
            RefreshPreview();
            if (lstVariables.SelectedItems.Count == 1)
                RefreshListViewVariableItem(lstVariables.SelectedItems[0]);
        }
        private void OnUpdateShaderAndExit(object sender, EventArgs e)
        {
            List<ShaderVariable> l= ShaderResource.CreateVariableList(editFragment.Text, false);
            // caut lista sa o am in uniformii mei
            foreach (ShaderVariable sv in l)
            {
                if (Uniforms.ContainsKey(sv.Name) == false)
                {
                    MessageBox.Show("Compile the shader first - local variables are not updated ! - missing: " + sv.Name);
                    return;
                }
                sv.Type = Uniforms[sv.Name].Variable.Type;
                sv.ClearAfterUsage = Uniforms[sv.Name].Variable.ClearAfterUsage;
            }
            if (l.Count!=Uniforms.Count)
            {
                MessageBox.Show("Some variavbles are missing. Recompile the shader !");
                return;
            }

            sr.SetVertexShader(editVertex.Text);
            sr.SetFragmentShader(editFragment.Text);
            sr.Uniforms = l;


            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void OnPreviewPropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            InitializeVertexBufferArray();
            RefreshPreview();
        }

        private void OnSelectShape(object sender, EventArgs e)
        {
            shapeType = PreviewShapeType.None;
            if (lstShapes.SelectedItems.Count == 1)
            {
                if ((lstShapes.SelectedItems[0].Text == "Rectangle") && (lstShapes.SelectedItems[0].Group.Name == "shapes"))
                    shapeType = PreviewShapeType.Rectangle;
                else if ((lstShapes.SelectedItems[0].Text == "Line") && (lstShapes.SelectedItems[0].Group.Name == "shapes"))
                    shapeType = PreviewShapeType.Line;
                else if (lstShapes.SelectedItems[0].Group.Name == "images")
                {
                    shapeType = PreviewShapeType.Image;
                    previewImage = Images[(ImageResource)lstShapes.SelectedItems[0].Tag];
                    Bitmap bmp = new Bitmap(previewImage);
                    texImage.Create(gl,bmp);                    
                }
            }
            InitializeVertexBufferArray();
            RefreshPreview();
        }

        private void RefreshListViewVariableItem(ListViewItem lvi)
        {
            if (lvi.Tag == null)
                return;
            UniformValue uv = (UniformValue)lvi.Tag;
            lvi.SubItems[1].Text = uv.Variable.OpenGLType;
            lvi.SubItems[2].Text = uv.Variable.Type.ToString();
            if (uv.Variable.Type == ShaderVariableType.None)
                lvi.ForeColor = Color.Red;
            else
                lvi.ForeColor = Color.Black;
        }
        private void RefreshVariableList()
        {
            lstVariables.Items.Clear();
            propValues.SelectedObject = null;
            foreach (string k in Uniforms.Keys)
            {
                UniformValue uv = Uniforms[k];
                ListViewItem lvi = new ListViewItem(uv.Variable.Name);
                lvi.SubItems.Add("");
                lvi.SubItems.Add("");
                lvi.Tag = uv;
                lvi.Group = lstVariables.Groups["uniform"];
                RefreshListViewVariableItem(lvi);
                lstVariables.Items.Add(lvi);
            }
        }

        private void OnSetTemplateForRectangle(object sender, EventArgs e)
        {
            SetShadersCode("attribute vec4 pos;\nvoid main() {\n\tgl_Position = pos;\n}", "precision lowp float;\nuniform vec4 color;\nvoid main() {\n\tgl_FragColor = color;\n}");
        }

        private void OnSetTemplateForImage(object sender, EventArgs e)
        {
            SetShadersCode("attribute vec4 pos;\nvarying vec2 imageCoord;\nattribute vec2 positionInImage;\nvoid main() {\n\tgl_Position = pos;\n\timageCoord = positionInImage;\n}", "precision mediump float;\nvarying vec2 imageCoord;\nuniform sampler2D s_texture;\nvoid main() {\n\tgl_FragColor = texture2D( s_texture, imageCoord );\n}");
        }

        private void OnSetTemplateForAlphaTransparency(object sender, EventArgs e)
        {
            SetShadersCode("attribute vec4 pos;\nvarying vec2 imageCoord;\nattribute vec2 positionInImage;\nvoid main() {\n\tgl_Position = pos;\n\timageCoord = positionInImage;\n}", "precision mediump float;\nuniform float alpha;\nvarying vec2 imageCoord;\nuniform sampler2D s_texture;\nvoid main() {\n\tvec4 v= texture2D( s_texture, imageCoord );\n\tv.a *= alpha;\n\tgl_FragColor = v;\n\n}");            
        }

        private void OnSetTemplateForColorMultiplication(object sender, EventArgs e)
        {
            SetShadersCode("attribute vec4 pos;\nvarying vec2 imageCoord;\nattribute vec2 positionInImage;\nvoid main() {\n\tgl_Position = pos;\n\timageCoord = positionInImage;\n}", "precision mediump float;\nuniform vec4 multiplyFactorRGBA;\nvarying vec2 imageCoord;\nuniform sampler2D s_texture;\nvoid main() {\n\tgl_FragColor = texture2D( s_texture, imageCoord )*multiplyFactorRGBA;\n}");            
        }

        private void OnSetTemplateForGrayscale(object sender, EventArgs e)
        {            
            SetShadersCode("attribute vec4 pos;\nvarying vec2 imageCoord;\nattribute vec2 positionInImage;\nvoid main() {\n\tgl_Position = pos;\n\timageCoord = positionInImage;\n}", "precision mediump float;\nvarying vec2 imageCoord;\nuniform sampler2D s_texture;\nvoid main() {\n\tvec4 v = texture2D( s_texture, imageCoord );\n\tfloat average = (v.r+v.g+v.b)/3.0;\n\tgl_FragColor = vec4(average,average,average,v.a);\n}");            
        }

        private void OnSetTemplateForColorTransformation(object sender, EventArgs e)
        {
            SetShadersCode("attribute vec4 pos;\nvarying vec2 imageCoord;\nattribute vec2 positionInImage;\nvoid main() {\n\tgl_Position = pos;\n\timageCoord = positionInImage;\n}", "precision mediump float;\nuniform vec4 multiplyFactorRGBA;\nvarying vec2 imageCoord;\nuniform sampler2D s_texture;\nvoid main() {\n\tvec4 v = texture2D( s_texture, imageCoord );\n\tfloat average = (v.r+v.g+v.b)/3.0;\n\tgl_FragColor = vec4(average,average,average,v.a)*multiplyFactorRGBA;\n}");            
        }

        private void OnSetTemplateVerticalGradientSimple(object sender, EventArgs e)
        {
            SetShadersCode("attribute vec4 pos;\nvarying vec4 screenPosition;\nvoid main() {\n\tscreenPosition = pos;\n\tgl_Position = pos;\n}", "precision lowp float;\nvarying vec4 screenPosition;\nvoid main() {\n\tgl_FragColor = vec4(screenPosition.y,0,0,1);\n}");            
        }

        private void OnSetTemplateVerticalGradient2Colors(object sender, EventArgs e)
        {
            SetShadersCode("attribute vec4 pos;\nvarying vec4 screenPosition;\nvoid main() {\n\tscreenPosition = pos;\n\tgl_Position = pos;\n}", "precision lowp float;\nuniform vec4 color1;\nuniform vec4 color2;\nvarying vec4 screenPosition;\nvoid main() {\n\tgl_FragColor = screenPosition.y*color1+(1-screenPosition.y)*color2;\n}");
        }

        private void OnSetTemplateForFlipVertical(object sender, EventArgs e)
        {
            SetShadersCode("attribute vec4 pos;\nvarying vec2 imageCoord;\nattribute vec2 positionInImage;\nvoid main() {\n\tgl_Position = pos;\n\timageCoord = vec2(positionInImage.x,1-positionInImage.y);\n}", "precision mediump float;\nvarying vec2 imageCoord;\nuniform sampler2D s_texture;\nvoid main() {\n\tgl_FragColor = texture2D( s_texture, imageCoord );\n}");
        }

        private void OnSetTemplateForFlipHorizontal(object sender, EventArgs e)
        {
            SetShadersCode("attribute vec4 pos;\nvarying vec2 imageCoord;\nattribute vec2 positionInImage;\nvoid main() {\n\tgl_Position = pos;\n\timageCoord = vec2(1-positionInImage.x,positionInImage.y);\n}", "precision mediump float;\nvarying vec2 imageCoord;\nuniform sampler2D s_texture;\nvoid main() {\n\tgl_FragColor = texture2D( s_texture, imageCoord );\n}");
        }

        private void OnPreviewSizeChanged(object sender, EventArgs e)
        {
            lbPreviewSize.Text = pnlPreview.Width.ToString() + " x " + pnlPreview.Height.ToString();
        }



    }
}
