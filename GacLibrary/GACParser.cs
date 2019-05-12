using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

/*
 * 1  - caracter
 * 2  - numar
 * 4  - hexa
 * 8  - operator
 * 16 - virgula
 * 32 - spatiu
 * 64 - new line
 *128 - string 
 *
 * 
 */

namespace GAppCreator
{
    public class GACParser
    {

        public enum ModifierType
        {
            Public,
            Private,
            Protected,
        };
        public enum GACIcon
        {
            Auto = -1,
            Class = 0,
            Namespace = 1,
            Enum = 2,
            Variable = 3,            
            Constant = 4,            
            Function = 5,
            StaticFunction = 6,
            BasicType = 7,
            StaticClass = 8,
            StaticVariable = 9,
            Resources = 10,
            Settings = 11,
            Api = 12,
            Ads = 13,
        };   
        public enum GACFileType
        {
            Application,
            Global,
            Scene,
            Control,
            FrameworkObject,
            Class,
        }

        #region XML Members

        [XmlType("Constant"), XmlRoot("Constant")]
        public class Constant
        {
            [XmlAttribute()]
            public string Name = "";
            [XmlAttribute()]
            public string Description = "";
            [XmlAttribute()]
            public string Cpp = "";
            [XmlAttribute()]
            public GACIcon Icon = GACIcon.Constant;
        }
        [XmlType("Variable"), XmlRoot("Variable")]
        public class Variable
        {
            [XmlAttribute()]
            public string Name = "";
            [XmlAttribute()]
            public string Type = "";
            [XmlAttribute()]
            public ModifierType Access = ModifierType.Public;
            [XmlAttribute()]
            public string Array = "";
            [XmlAttribute()]
            public string Description = "";
            [XmlAttribute()]
            public string Cpp = "";
            [XmlAttribute()]
            public GACIcon Icon = GACIcon.Variable;
        };
        [XmlType("Parameter"), XmlRoot("Parameter")]
        public class Parameter
        {
            [XmlAttribute()]
            public string Name = "";
            [XmlAttribute()]
            public string Type = "";
            [XmlAttribute()]
            public string Array = "";
            [XmlAttribute()]
            public string DefaultValue = "";
            [XmlAttribute()]
            public string Description = "";
            [XmlAttribute()]
            public bool Reference = false;
        };
        [XmlType("Function"), XmlRoot("Function")]
        public class Function
        {
            [XmlAttribute()]
            public string Name = "";
            [XmlAttribute()]
            public string ReturnType = "";
            [XmlAttribute()]
            public ModifierType Access = ModifierType.Public;
            [XmlAttribute()]
            public bool Virtual = false;
            [XmlAttribute()]
            public string Description = "";
            [XmlAttribute()]
            public string Cpp = "";
            [XmlAttribute()]
            public GACIcon Icon = GACIcon.Function;


            public List<Parameter> Parameters = new List<Parameter>();

            public string GetToolTip()
            {
                string s = ReturnType + " " + Name + "(";
                foreach (Parameter v in Parameters)
                {
                    if (v.Reference)
                        s += "ref ";
                    if (v.Type == "None")
                        s += " " + v.Name;
                    else
                        s += v.Type + " " + v.Name;
                    if (v.DefaultValue.Length > 0)
                        s += " = " + v.DefaultValue;
                    s += " , ";
                }
                if (s.EndsWith(" , "))
                    s = s.Substring(0, s.Length - 3);
                s += ")\n";
                if (Description.Length > 0)
                    s += "Description:" + Description + "\n";
                foreach (Parameter v in Parameters)
                {
                    if (v.Description.Length > 0)
                        s += v.Name + " - " + v.Description + "\n";
                }                
                return s;
            }

            protected internal int StartCode = -1, EndCode = -1; // [StartCode .. EndCode] inclusiv
            protected internal string TranslatedCode = "";
        }
        [XmlType("Constructor"), XmlRoot("Constructor")]
        public class Constructor
        {
            [XmlAttribute()]
            public string Description = "";

            public List<Parameter> Parameters = new List<Parameter>();

            protected internal string BaseCall = "";
            protected internal string TranslatedCode = "";
            protected internal int StartCode = -1, EndCode = -1; // [StartCode .. EndCode] inclusiv

            public string GetToolTip(string className)
            {
                string s = className + "(";
                string space = "\n".PadRight(className.Length + 2, ' ');
                bool first = true;
                foreach (Parameter v in Parameters)
                {
                    if (!first)
                        s += space;
                    first = false;
                    if (v.Reference)
                        s += "ref ";
                    if (v.Type == "None")
                        s += " " + v.Name;
                    else
                        s += v.Type + " " + v.Name;
                    if (v.DefaultValue.Length > 0)
                        s += " = " + v.DefaultValue;
                    s += " , ";
                }
                if (s.EndsWith(" , "))
                    s = s.Substring(0, s.Length - 3);
                s += ")\n";
                if (Description.Length > 0)
                    s += "Description:" + Description + "\n";
                foreach (Parameter v in Parameters)
                {
                    if (v.Description.Length > 0)
                        s += v.Name + " - " + v.Description + "\n";
                }
                return s;
            }
        }
        [XmlType("Class"), XmlRoot("Class")]
        public class Class
        {
            [XmlAttribute()]
            public string Name = "";
            [XmlAttribute()]
            public string Cpp = "";
            [XmlAttribute()]
            public string DerivedFrom = "";
            [XmlAttribute()]
            public string Description = "";
            [XmlAttribute()]
            public bool BasicType = false;
            [XmlAttribute()]
            public GACIcon Icon = GACIcon.Class;

            public List<Constant> Constants = new List<Constant>();
            public List<Function> Functions = new List<Function>();
            public List<Function> StaticFunctions = new List<Function>();
            public List<Variable> Variables = new List<Variable>();
            public List<Constructor> Constructors = new List<Constructor>();

            public List<string> AutoCompleteListStatic = new List<string>();
            public List<string> AutoCompleteListNonStatic = new List<string>();

            public Constant GetConstant(string name)
            {
                foreach (Constant c in Constants)
                    if (c.Name.Equals(name))
                        return c;
                return null;
            }
            public Function GetFunction(string name)
            {
                foreach (Function f in Functions)
                    if (f.Name.Equals(name))
                        return f;
                return null;
            }
            public Variable GetVariable(string name)
            {
                foreach (Variable v in Variables)
                    if (v.Name.Equals(name))
                        return v;
                return null;
            }
            public Function GetStaticFunction(string name)
            {
                foreach (Function f in StaticFunctions)
                    if (f.Name.Equals(name))
                        return f;
                return null;
            }

            public string GetGACCPPType(bool pointer)
            {
                if (pointer)
                    return "PTR_GACTYPE_" + Name;
                else
                    return "GACTYPE_" + Name;
            }

            protected internal int Order = 0;
            protected internal Dictionary<string, bool> Dependencies = new Dictionary<string, bool>();

            public void UpdateAutoCompleteList()
            {
                AutoCompleteListStatic.Clear();
                AutoCompleteListNonStatic.Clear();
                Dictionary<string, bool> used = new Dictionary<string, bool>();

                foreach (Constant c in Constants)
                    AutoCompleteListStatic.Add(c.Name + "?" + ((int)c.Icon).ToString());
                foreach (Function f in StaticFunctions)
                    if (used.ContainsKey(f.Name) == false)
                    {
                        AutoCompleteListStatic.Add(f.Name + "?" + ((int)f.Icon).ToString());
                        used[f.Name] = true;
                    }

                foreach (Function f in Functions)
                    if (used.ContainsKey(f.Name) == false)
                    {
                        AutoCompleteListNonStatic.Add(f.Name + "?" + ((int)f.Icon).ToString());
                        used[f.Name] = true;
                    }                    
                foreach (Variable v in Variables)
                    AutoCompleteListNonStatic.Add(v.Name + "?" + ((int)v.Icon).ToString());
                AutoCompleteListNonStatic.Sort(delegate(string s1, string s2) { return string.Compare(s1, s2, StringComparison.OrdinalIgnoreCase); });
                AutoCompleteListStatic.Sort(delegate(string s1, string s2) { return string.Compare(s1, s2, StringComparison.OrdinalIgnoreCase); });
            }
        }
        [XmlType("StaticClass"), XmlRoot("StaticClass")]
        public class StaticClass
        {
            [XmlAttribute()]
            public string Name = "";
            [XmlAttribute()]
            public string Description = "";
            [XmlAttribute()]
            public GACIcon Icon = GACIcon.StaticClass;

            public List<Constant> Constants = new List<Constant>();
            public List<Function> Functions = new List<Function>();
            public List<Variable> Variables = new List<Variable>();

            public List<string> AutoCompleteList = new List<string>();

            public Constant GetConstant(string name)
            {
                foreach (Constant c in Constants)
                    if (c.Name.Equals(name))
                        return c;
                return null;
            }
            public Function GetFunction(string name)
            {
                foreach (Function f in Functions)
                    if (f.Name.Equals(name))
                        return f;
                return null;
            }
            public Variable GetVariable(string name)
            {
                foreach (Variable v in Variables)
                    if (v.Name.Equals(name))
                        return v;
                return null;
            }

            public void ClearLists()
            {
                Constants.Clear();
                Functions.Clear();
                Variables.Clear();
                AutoCompleteList.Clear();
            }  

            public void UpdateAutoCompleteList()
            {
                AutoCompleteList.Clear();
                Dictionary<string, bool> used = new Dictionary<string, bool>();
                foreach (Constant c in Constants)
                    AutoCompleteList.Add(c.Name + "?" + ((int)c.Icon).ToString());
                foreach (Function f in Functions)
                {
                    if (used.ContainsKey(f.Name) == false)
                    {
                        AutoCompleteList.Add(f.Name + "?" + ((int)f.Icon).ToString());
                        used[f.Name] = true;
                    }
                }
                foreach (Variable v in Variables)
                    AutoCompleteList.Add(v.Name + "?" + ((int)v.Icon).ToString());
                AutoCompleteList.Sort(delegate(string s1, string s2) { return string.Compare(s1, s2, StringComparison.OrdinalIgnoreCase); });
            }
        }        
        [XmlType("Enum"), XmlRoot("Enum")]
        public class Enum
        {
            [XmlAttribute()]
            public string Name = "";
            [XmlAttribute()]
            public string Description = "";
            [XmlAttribute()]
            public string Cpp = "";
            [XmlAttribute()]
            public GACIcon Icon = GACIcon.Enum;

            public List<Constant> Constants = new List<Constant>();
            public List<string> AutoCompleteList = new List<string>();

            public Constant GetConstant(string name)
            {
                foreach (Constant c in Constants)
                    if (c.Name.Equals(name))
                        return c;
                return null;
            }
            public void UpdateAutoCompleteList()
            {
                AutoCompleteList.Clear();
                foreach (Constant c in Constants)
                    AutoCompleteList.Add(c.Name + "?" + ((int)c.Icon).ToString());
                AutoCompleteList.Sort(delegate(string s1, string s2) { return string.Compare(s1, s2, StringComparison.OrdinalIgnoreCase); });
            }
        }        
        [XmlType("Namespace"), XmlRoot("Namespace")]
        public class Namespace
        {
            [XmlAttribute()]
            public string Name = "";
            [XmlAttribute()]
            public string Description = "";
            [XmlAttribute()]
            public GACIcon Icon = GACIcon.Namespace;

            public List<Namespace> Namespaces = new List<Namespace>();
            public List<Enum> Enums = new List<Enum>();
            public List<StaticClass> StaticClasses = new List<StaticClass>();
            public List<Class> Classes = new List<Class>();
            public List<string> AutoCompleteList = new List<string>();

            public Class GetClass(string name)
            {
                foreach (Class c in Classes)
                    if (c.Name.Equals(name))
                        return c;
                return null;
            }
            public StaticClass GetStaticClass(string name)
            {
                foreach (StaticClass c in StaticClasses)
                    if (c.Name.Equals(name))
                        return c;
                return null;
            }
            public Namespace GetNamespace(string name)
            {
                foreach (Namespace n in Namespaces)
                    if (n.Name.Equals(name))
                        return n;
                return null;
            }
            public Enum GetEnum(string name)
            {
                foreach (Enum e in Enums)
                    if (e.Name.Equals(name))
                        return e;
                return null;
            }
            public void ClearLists()
            {
                Namespaces.Clear();
                Enums.Clear();
                StaticClasses.Clear();
                Classes.Clear();
            }
            public void UpdateAutoCompleteList()
            {
                AutoCompleteList.Clear();
                foreach (Namespace n in Namespaces)
                    AutoCompleteList.Add(n.Name + "?" + ((int)n.Icon).ToString());
                foreach (Class c in Classes)
                    AutoCompleteList.Add(c.Name + "?" + ((int)c.Icon).ToString());
                foreach (StaticClass c in StaticClasses)
                    AutoCompleteList.Add(c.Name + "?" + ((int)c.Icon).ToString());
                foreach (Enum e in Enums)
                    AutoCompleteList.Add(e.Name + "?" + ((int)e.Icon).ToString());
                AutoCompleteList.Sort(delegate(string s1, string s2) { return string.Compare(s1, s2, StringComparison.OrdinalIgnoreCase); });
            }
        }

        #endregion

        #region Modules and Members
        public enum ModuleType
        {
            Namespace,
            Class,
            StaticClass,
            Enum
        };
        public enum MemberType
        {
            Variable,
            Constant,
            Function,
            Constructor,
            Destructor,
        };
        public enum ModuleTypeInformation
        {
            None,
            IsScene,
            IsApp,
        };
        public enum SerializableType
        {
            None,
            Serializable,
            Persistent
        }
        public enum LocalDefinitionType
        {
            Application,
            Scene,
            Class,
            Method,
            Constructor,
            Destructor,
            Member,
            Parameter,
            LocalVariable
        }
        public class LocalDefinition
        {
            public string Name;
            public string File;
            public string Definition;
            public string Prototype;
            public int Start, End, LineNumber;
            public int SearchScore = 0; // to be used by search module
            public LocalDefinitionType Type;

            public LocalDefinition(string name,string file,string definition,string prototype, int start,int end,int line,LocalDefinitionType type)
            {
                Name = name;
                File = file;
                Definition = definition;
                Start = start;
                end = End;
                LineNumber = line;
                Type = type;
                Prototype = prototype;
            }
            public void CreateFromModule(Module module,LocalDefinitionType type, string prototype)
            {
                this.File = module.FileName;
                this.LineNumber = module.FileLine;
                this.Start = module.FileStartPos;
                this.End = module.FileEndPos;
                this.Name = module.Name;
                this.Type = type;
                this.Prototype = prototype;
            }
        }
        public class ApiDefinition
        {
            public string Name;
            public string Definition;
            public Module module;
            public Member member;
            public int SearchScore = 0;

            public ApiDefinition(string name,string definition,Module _module = null,Member _member = null)
            {
                Name = name;
                Definition = definition;
                module = _module;
                member = _member;
            }
        }
        public class Module
        {
            public string Name = "", Description = "", Cpp = "", DerivedFrom = "", FileName="";
            public int FileStartPos = -1, FileEndPos = -1, FileLine = -1;
            public bool BasicType = false;
            public int StartToken = -1, EndToken = -1, Order = -1;
            public Token ObjectToken = null;
            public GACIcon Icon = GACIcon.Namespace;
            public ModuleType Type = ModuleType.Namespace;
            public ModuleTypeInformation TypeInformation = ModuleTypeInformation.None;
            public Dictionary<string, Member> Members = new Dictionary<string, Member>();
            public Dictionary<string, Module> Modules = new Dictionary<string, Module>();
            public List<string> AutoCompleteListStatic = new List<string>();
            public List<string> AutoCompleteListNonStatic = new List<string>();

            public Module(ModuleType tip, string name, string cpp, string deriveFrom, bool basicType,Token objToken,string fileName)
            {
                Name = name;
                Type = tip;
                DerivedFrom = deriveFrom;
                BasicType = basicType;
                FileName = fileName;
                ObjectToken = objToken;
                if (objToken!=null)
                {
                    FileStartPos = objToken.Start;
                    FileEndPos = objToken.End;
                    FileLine = objToken.Line;
                }
                Cpp = cpp;
                switch (Type)
                {
                    case ModuleType.Class: Icon = GACIcon.Class; break;
                    case ModuleType.Namespace: Icon = GACIcon.Namespace; break;
                    case ModuleType.StaticClass: Icon = GACIcon.StaticClass; break;
                    case ModuleType.Enum: Icon = GACIcon.Enum; break;
                }
            }
            
            public Member GetMember(string name)
            {
                if (Members.ContainsKey(name))
                    return Members[name];
                return null;
            }
            public Module GetModule(string name)
            {
                if (Modules.ContainsKey(name))
                    return Modules[name];
                return null;
            }
            public TokenType GetTokenType()
            {
                switch (Type)
                {
                    case ModuleType.Class: return TokenType.TypeClass;
                    case ModuleType.StaticClass: return TokenType.TypeClassStatic;
                    case ModuleType.Namespace: return TokenType.TypeNamespace;
                    case ModuleType.Enum: return TokenType.TypeEnum;
                }
                return TokenType.Word;
            }
            public string GetGACCPPType(bool pointer)
            {
                if (pointer)
                    return "PTR_GACTYPE_" + Name;
                else
                    return "GACTYPE_" + Name;
            }
            public void UpdateAutoCompleteList()
            {
                AutoCompleteListStatic.Clear();
                AutoCompleteListNonStatic.Clear();
                foreach (Module m in Modules.Values)
                {
                    AutoCompleteListStatic.Add(m.Name + "?" + ((int)m.Icon).ToString());
                }
                foreach (Member mb in Members.Values)
                {
                    if (mb.Static)
                    {
                        switch (mb.Type)
                        {
                            case MemberType.Constant: AutoCompleteListStatic.Add(mb.Name + "?" + ((int)GACIcon.Constant).ToString()); break;
                            case MemberType.Function: AutoCompleteListStatic.Add(mb.Name + "?" + ((int)GACIcon.StaticFunction).ToString()); break;
                            case MemberType.Variable: AutoCompleteListStatic.Add(mb.Name + "?" + ((int)GACIcon.StaticVariable).ToString()); break;
                            default: AutoCompleteListStatic.Add(mb.Name); break;
                        }
                    }
                    else
                    {
                        switch (mb.Type)
                        {
                            case MemberType.Constant: AutoCompleteListNonStatic.Add(mb.Name + "?" + ((int)GACIcon.Constant).ToString()); break;
                            case MemberType.Function: AutoCompleteListNonStatic.Add(mb.Name + "?" + ((int)GACIcon.Function).ToString()); break;
                            case MemberType.Variable: AutoCompleteListNonStatic.Add(mb.Name + "?" + ((int)GACIcon.Variable).ToString()); break;
                            default: AutoCompleteListNonStatic.Add(mb.Name); break;
                        }
                    }

                }
                AutoCompleteListStatic.Sort(delegate(string s1, string s2) { return string.Compare(s1, s2, StringComparison.OrdinalIgnoreCase); });
                AutoCompleteListNonStatic.Sort(delegate(string s1, string s2) { return string.Compare(s1, s2, StringComparison.OrdinalIgnoreCase); });

            }
        }
        public class Member
        {
            public string Name = "", Description = "", Cpp = "", DataType = "";
            public string Translate = "";
            public string BaseCppCallCode = "";
            public string DefaultValue = "";
            public ModifierType Access = ModifierType.Public;
            public bool Static=false, Virtual=false, Reference=false, Override=false;
            public SerializableType Serializable = SerializableType.None;
            public MemberType Type = MemberType.Constant;
            public int StartCode = -1, EndCode = -1, LineNumber = -1;
            //public int StartArray = -1, EndArray = -1;
            public int ArrayCount = 0;
            public string ArrayCppCode = "";
            public string ArrayGacCode = "";
            public Module Parent = null;
            public List<Member> Overrides = null;
            public Dictionary<string, Member> LocalVariables = new Dictionary<string, Member>();
            private List<Member> Parameters = null;
            private Dictionary<string, Member> dParameters = null;
            public int FileStartPos = -1, FileEndPos = -1, FileLine = -1;

            public Member GetParameter(string name)
            {
                if (dParameters == null)
                    return null;
                Member m = null;
                if (dParameters.TryGetValue(name, out m))
                    return m;
                return null;
            }
            public List<Member> GetParameters()
            {
                return Parameters;
            }
            public bool AddParameter(Member m)
            {
                if (Parameters == null)
                {
                    Parameters = new List<Member>();
                    dParameters = new Dictionary<string, Member>();
                }
                if (dParameters.ContainsKey(m.Name))
                    return false;
                dParameters[m.Name] = m;
                Parameters.Add(m);
                return true;
            }
            public string GetFunctionSignature()
            {
                string s;
                if (Type== MemberType.Constructor)
                    s = "constructor(";
                else if (Type == MemberType.Destructor)
                    s = "destructor(";
                else
                    s = Name + "(";
                if (Parameters != null)
                {
                    foreach (Member m in Parameters)
                        s += m.DataType + ",";
                }
                if (s.EndsWith(","))
                    s = s.Substring(0, s.Length - 1);
                s += ")";
                return s;
            }

            public Member(Module parent,MemberType type, string name, string cpp, string dataType, string description,Token tok)
            {
                Parent = parent;
                Type = type;
                Name = name;
                Cpp = cpp;
                DataType = dataType;
                Description = description;
                if (type == MemberType.Constant)
                    Static = true;
                if (tok != null)
                {
                    FileStartPos = tok.Start;
                    FileEndPos = tok.End;
                    FileLine = tok.Line;
                }

            }
            public string GetLocalVarianleDefinition()
            {
                string s = "";
                s += DataType + " " + Name;
                if (ArrayCount > 0)
                    s += " " + ArrayGacCode;
                if (DefaultValue.Length > 0)
                    s += " = " + DefaultValue;
                s += ";  ";
                return s;
            }
            public string GetFunctionParameters()
            {
                string s = "";
                if (Parameters != null)
                {
                    s += "(";
                    foreach (Member v in Parameters)
                    {
                        if (v.Reference)
                            s += "ref ";
                        s += v.DataType + " " + v.Name;
                        if (v.ArrayCount > 0)
                            s += " " + v.ArrayGacCode;
                        if (v.DefaultValue.Length > 0)
                            s += " = " + v.DefaultValue;
                        s += " , ";
                    }
                    if (s.EndsWith(" , "))
                        s = s.Substring(0, s.Length - 3);
                    s += " ) ";
                }
                else
                {
                    s += "( )";
                }
                return s;
            }
            public string GetDefinition(string className)
            {
                string s = "";
                if ((Type == MemberType.Variable) || (Type == MemberType.Constant))
                {
                    s = Access.ToString().ToLower() + " ";
                    if (Serializable != SerializableType.None)
                        s += Serializable.ToString().ToLower() + " ";
                    s += DataType + " " + className;
                    if ((s.EndsWith(".") == false) && (className.Length>0))
                        s += ".";
                    s += Name;
                    if (ArrayCount > 0)
                        s += " "+ArrayGacCode;
                    return s;
                }
                // daca amm o functie / constructor sau destructor
                if (Type == MemberType.Function)
                {
                    s = Access.ToString().ToLower() + " ";
                    if (Override)
                        s += "override ";
                    if (Virtual)
                        s += "virtual ";
                    s += DataType + " " + className;
                    if ((s.EndsWith(".") == false) && (className.Length > 0))
                        s += ".";
                    s += Name;
                }
                if (Type == MemberType.Constructor)
                {
                    s = className;
                    if ((s.EndsWith(".") == false) && (className.Length > 0))
                        s += ".";
                    s += "<constructor>";                    
                }
                if (Type == MemberType.Destructor)
                {
                    s = className;
                    if ((s.EndsWith(".") == false) && (className.Length > 0))
                        s += ".";
                    s += "<destructor>";
                }

                return s + " " + GetFunctionParameters();
            }
            public string GetToolTip(string className)
            {
                string s = "";
                if ((Type == MemberType.Variable) || (Type == MemberType.Constant))
                {
                    return DataType+" "+className+"."+Name;
                }
                string space = "";
                if (Type == MemberType.Constructor)
                {
                    s = "Class:"+className + "[constructor]\n"+className+"(";
                    space = "\n".PadRight(className.Length + 2, ' ');
                }
                else if (Type == MemberType.Destructor)
                {
                    s = "Class:" + className + "[destructor]\n" + className + "(";
                    space = "\n".PadRight(className.Length + 2, ' ');
                }
                else {
                    s = "Class:"+className+"[function]\n"+DataType+" "+Name+"(";
                    space = "\n".PadRight(Name.Length + 3 + DataType.Length, ' ');
                }
                if (Parameters != null)
                {
                    bool first = true;
                    foreach (Member v in Parameters)
                    {
                        if (!first)
                            s += space;
                        first = false;
                        if (v.Reference)
                            s += "ref ";
                        s += v.DataType + " " + v.Name;
                        if (v.ArrayCount > 0)
                        {
                            s += "[" + "?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,".Substring(0, v.ArrayCount * 2 - 1) + "]";

                        }
                        if (v.DefaultValue.Length > 0)
                            s += " = " + v.DefaultValue;
                        s += " , ";
                    }
                    if (s.EndsWith(" , "))
                        s = s.Substring(0, s.Length - 3);
                }
                s += ")\n";
                if (Description.Length > 0)
                    s += "Description:" + Description + "\n";
                if (Parameters != null)
                {
                    foreach (Member v in Parameters)
                    {
                        if (v.Description.Length > 0)
                            s += v.Name + " - " + v.Description + "\n";
                    }
                }
                return s;
            }
            /*
            public void ShowCallTipList(ScintillaNET.Scintilla editor)
            {
                if ((Overrides == null) || (Overrides.Count == 0))
                {
                    editor.CallTip.Message = GetToolTip(Parent.Name);
                    editor.CallTip.Show();
                }
                else
                {
                    if (editor.CallTip.OverloadList == null)
                        editor.CallTip.OverloadList = new ScintillaNET.OverloadList();
                    editor.CallTip.OverloadList.Clear();
                    editor.CallTip.OverloadList.Add(GetToolTip(Parent.Name));
                    foreach (Member mb in Overrides)
                        editor.CallTip.OverloadList.Add(mb.GetToolTip(Parent.Name));
                    editor.CallTip.ShowOverload();
                }
            }
             */
            public Member GetLocalVariableOrParameter(string name)
            {
                Member m = GetParameter(name);
                if (m == null)
                {
                    if (LocalVariables != null)
                    {
                        if (LocalVariables.TryGetValue(name, out m) == false)
                            m = null;
                    }
                }
                return m;
            }
        }
        #endregion

        public class GACFile
        {
            public Dictionary<string, Module> Modules = new Dictionary<string, Module>();
            public List<string> PreprocessCommands = new List<string>();
            public List<string> AutoCompleteList = new List<string>();
            public List<string> Scenes = new List<string>();
            public GACFileType FileType = GACFileType.Class;

            public void Clear()
            {
                Modules.Clear();
                PreprocessCommands.Clear();
            }
            public Module GetModule(string Name)
            {
                Module m = null;
                if (Modules.TryGetValue(Name, out m))
                    return m;
                return null;
            }
            public void RemoveModule(string Name)
            {
                if (Modules.ContainsKey(Name))
                    Modules.Remove(Name);
            }
            private bool AddModule(Module Parent,Module m)
            {
                if (Modules.ContainsKey(m.Name))
                {
                    MessageBox.Show(m.Type.ToString() + " "+m.Name + " was already added !");
                    return false;
                }
                if (Parent != null)
                    Parent.Modules[m.Name] = m;
                Modules[m.Name] = m;
                return true;
            }
            private bool AddMember(Module Parent, Member m)
            {
                if (Parent.Members.ContainsKey(m.Name))
                {
                    if (((Parent.Members[m.Name].Type == MemberType.Function) && (m.Type == MemberType.Function)) || 
                        ((Parent.Members[m.Name].Type == MemberType.Constructor) && (m.Type == MemberType.Constructor)) ||
                        ((Parent.Members[m.Name].Type == MemberType.Destructor) && (m.Type == MemberType.Destructor)))
                    {
                        if (Parent.Members[m.Name].GetFunctionSignature().Equals(m.GetFunctionSignature()))
                        {
                            if (m.DataType != Parent.Members[m.Name].DataType)
                            {
                                MessageBox.Show(Parent.Name+"."+m.GetFunctionSignature()+" has different return type!.");
                                return false;
                            }
                        }
                        if (Parent.Members[m.Name].Overrides==null)
                            Parent.Members[m.Name].Overrides = new List<Member>();
                        Parent.Members[m.Name].Overrides.Add(m);
                        return true;
                    }
                    else
                    {
                        MessageBox.Show(m.Name + " was already defined in " + Parent.Name);
                        return false;
                    }
                }
                Parent.Members[m.Name] = m;
                return true;
            }
            public bool Add(Module Parent, Namespace n)
            {
                Module m = new Module(ModuleType.Namespace, n.Name, "", "",false,null,"");
                if (AddModule(Parent,m) == false)
                    return false;
                foreach (Enum e in n.Enums)
                    Add(m,e);
                foreach (Class c in n.Classes)
                    Add(m,c);
                foreach (StaticClass c in n.StaticClasses)
                    Add(m,c);
                foreach (Namespace nn in n.Namespaces)
                    Add(m,nn);
                return true;
            }
            private bool Add(Module Parent, Enum e)
            {
                Module m = new Module(ModuleType.Enum, e.Name, e.Cpp, "", true,null,"");
                if (AddModule(Parent,m) == false)
                    return false;
                foreach (Constant c in e.Constants)
                    if (Add(m, c) == false)
                        return false;
                return true;
            }
            private bool Add(Module Parent, Class c)
            {
                Module m = new Module(ModuleType.Class, c.Name, c.Cpp, c.DerivedFrom, c.BasicType,null,"");
                if (AddModule(Parent, m) == false)
                    return false;

                foreach (Function f in c.Functions)
                {
                    if (Add(m, f,false) == false)
                        return false;                                       
                }
                foreach (Function f in c.StaticFunctions)
                {
                    if (Add(m, f,true) == false)
                        return false;
                }
                foreach (Constructor f in c.Constructors)
                {
                    if (Add(m, f) == false)
                        return false;
                }
                foreach (Variable v in c.Variables)
                {
                    if (Add(m, v) == false)
                        return false;
                }
                foreach (Constant ct in c.Constants)
                {
                    if (Add(m, ct) == false)
                        return false;
                }
                return true;
            }
            private bool Add(Module Parent, StaticClass c)
            {
                Module m = new Module(ModuleType.StaticClass, c.Name, "", "", false,null,"");
                if (AddModule(Parent, m) == false)
                    return false;
                foreach (Function f in c.Functions)
                {
                    if (Add(m, f, true) == false)
                        return false;
                }
                foreach (Variable v in c.Variables)
                {
                    if (Add(m, v) == false)
                        return false;
                }
                foreach (Constant ct in c.Constants)
                {
                    if (Add(m, ct) == false)
                        return false;
                }
                return true;
            }
            private bool Add(Module Parent, Function f,bool statica)
            {
                Member mb = new Member(Parent, MemberType.Function, f.Name, f.Cpp, f.ReturnType, f.Description, null);
                mb.Static = statica;
                mb.Virtual = f.Virtual;
                foreach (Parameter p in f.Parameters)
                {
                    Member param = new Member(Parent, MemberType.Variable, p.Name, "", p.Type, p.Description, null);
                    param.DefaultValue = p.DefaultValue;
                    mb.AddParameter(param);
                }
                return AddMember(Parent, mb);
            }
            private bool Add(Module Parent, Constructor f)
            {
                Member mb = new Member(Parent,MemberType.Constructor, "", "", "", f.Description, null);
                foreach (Parameter p in f.Parameters)
                {
                    Member param = new Member(Parent,MemberType.Variable, p.Name, "", p.Type, p.Description, null);
                    param.DefaultValue = p.DefaultValue;
                    mb.AddParameter(param);
                }
                return AddMember(Parent, mb);
            }
            private bool Add(Module Parent, Variable v)
            {
                Member mb = new Member(Parent,MemberType.Variable, v.Name, v.Cpp, v.Type, v.Description, null);
                return AddMember(Parent, mb);
            }
            private bool Add(Module Parent, Constant c)
            {
                Member mb = new Member(Parent, MemberType.Constant, c.Name, c.Cpp, Parent.Cpp, c.Description, null);
                return AddMember(Parent, mb);
            }
            public void UpdateAutoCompleteList()
            {
                AutoCompleteList.Clear();
                foreach (Module m in Modules.Values)
                {
                    m.UpdateAutoCompleteList();
                    AutoCompleteList.Add(m.Name + "?" + ((int)m.Icon).ToString());
                }
                // nu trebuie sortate pentru ca le sortez in Compiler
            }
        }
        public class GACCompiler
        {
            public Dictionary<string, GACFile> Files = new Dictionary<string, GACFile>();
            public List<Module> LocalClasses = new List<Module>();
            public List<string> AutoCompleteList = new List<string>();

            public GACFile GetGACFile(string fileName)
            {
                if (Files.ContainsKey(fileName)==false)
                    Files[fileName] = new GACFile();
                return Files[fileName];
            }
            public Module GetModule(string name)
            {
                foreach (GACFile gf in Files.Values)
                {
                    if (gf.Modules.ContainsKey(name) == true)
                        return gf.Modules[name];
                }
                return null;
            }

            public Member GetMemberFromBaseClasses(Module m, string memberName)
            {
                if (m.DerivedFrom.Length == 0)
                    return null;
                Module baseClass = GetModule(m.DerivedFrom);
                if (baseClass == null)
                    return null;
                Member mb = baseClass.GetMember(memberName);
                if (mb != null)
                    return mb;
                return GetMemberFromBaseClasses(baseClass, memberName);
            }

            public void ClearLocals()
            {
                foreach (string fileName in Files.Keys)
                {
                    if (fileName.Length == 0)
                        continue;
                    Files[fileName].Clear();
                }
            }

            protected internal string GetCPPName(string typeName,bool addReference)
            {
                Module m = GetModule(typeName);
                if (m != null)
                {
                    if (m.Type == ModuleType.Class)
                    {
                        if (m.BasicType)
                        {
                            if (addReference)
                                return m.GetGACCPPType(false) + "&";
                            else
                                return m.GetGACCPPType(false);
                        }
                        else
                        {
                            return m.GetGACCPPType(true);
                        }
                    }
                    if (m.Type == ModuleType.Enum)
                    {
                        if (m.Cpp.Length > 0)
                            return m.Cpp;
                        else
                            return m.Name;
                    }
                }
                return "<<INVALID_TOKEN_NAME_" + typeName + ">>";
            }
            public Dictionary<string,Class> CreateLocalClassList()
            {
                Dictionary<string, Class> l = new Dictionary<string, Class>();
                /*
                foreach (string fileName in Files.Keys)
                {
                    // skip la cele din framework
                    if (fileName.Length == 0)
                        continue;
                    GACFile gf = Files[fileName];                    
                    foreach (Class c in gf.Classes.Values)
                        l[c.Name] = c;
                }
                 */ 
                return l;
            }
            public Dictionary<string,Enum> CreateLocalEnumList()
            {
                Dictionary<string, Enum> l = new Dictionary<string, Enum>();
                /*
                foreach (string fileName in Files.Keys)
                {
                    // skip la cele din framework
                    if (fileName.Length == 0)
                        continue;
                    GACFile gf = Files[fileName];
                    foreach (Enum c in gf.Enums.Values)
                        l[c.Name] = c;
                }
                 */
                return l;
            }
            public void UpdateGlobalAutoCompleteList()
            {
                AutoCompleteList.Clear();
                foreach (GACFile gFile in Files.Values)
                {
                    gFile.UpdateAutoCompleteList();
                    AutoCompleteList.AddRange(gFile.AutoCompleteList);
                }
                // adaug si tot ce este in gacKeywords
                foreach (string name in gacKeywords.Keys)
                {
                    if ((gacKeywords[name] == TokenType.Null) || (gacKeywords[name] == TokenType.Constant))
                        AutoCompleteList.Add(name + "?4");
                    else
                        AutoCompleteList.Add(name + "?14");
                }
                AutoCompleteList.Sort(delegate(string s1, string s2) { return string.Compare(s1, s2, StringComparison.OrdinalIgnoreCase); });
            }
            public string GetConstantsList()
            {
                string s = "";
                foreach (GACFile gFile in Files.Values)
                {
                    foreach (Module m in gFile.Modules.Values)
                    {
                        foreach (Member mb in m.Members.Values)
                        {
                            if (mb.Type == MemberType.Constant)
                                s += mb.Name + "\r\n";
                        }
                    }
                }
                foreach (string name in gacKeywords.Keys)
                {
                    switch (gacKeywords[name])
                    {
                        case TokenType.Constant:
                        case TokenType.Null:
                            s += name + "\r\n";
                            break;
                    }
                }
                return s;
            }
            public string GetTypesList()
            {
                string s = "";
                foreach (GACFile gFile in Files.Values)
                {
                    foreach (string moduleName in gFile.Modules.Keys)
                        s += moduleName + "\r\n";
                }
                return s;
            }
            public string GetKeywordsList()
            {
                string s = "";
                foreach (string name in gacKeywords.Keys)
                {
                    switch (gacKeywords[name])
                    {
                        case TokenType.Class:
                        case TokenType.Enum:                        
                        case TokenType.AccessModifier:
                        case TokenType.Keyword:
                        case TokenType.Constructor:
                        case TokenType.Destructor:
                        case TokenType.Base:
                        case TokenType.Reference:
                        case TokenType.New:
                        case TokenType.Delete:
                        case TokenType.Override:
                        case TokenType.Serializable:
                        case TokenType.Persistent:
                        case TokenType.Global:
                        case TokenType.Local:
                        case TokenType.Scene:
                        case TokenType.Application:
                        case TokenType.This:
                            s += name + "\r\n";
                            break;
                    }
                }
                return s;
            }
            public string GetPreprocesorData()
            {
                string s = "";
                foreach (GACFile gf in Files.Values)
                {
                    foreach (string ss in gf.PreprocessCommands)
                        s += ss + "\n";
                }
                return s;
            }
            public bool CheckDefinitions(Project prj)
            {
                GACFile system = GetGACFile("");
                /*
                if (system.Classes.Count == 0)
                    prj.EC.AddError("GAC XML", "No clases found on GAC xml !");
                if (system.Enums.Count == 0)
                    prj.EC.AddError("GAC XML", "No enum found on GAC xml !");
                if (system.Namespaces.Count == 0)
                    prj.EC.AddError("GAC XML", "No namespaces found on GAC xml !");
                if (system.StaticClasses.Count == 0)
                    prj.EC.AddError("GAC XML", "No static clases found on GAC xml !");


                foreach (Class c in system.Classes.Values)
                {
                    if (c.Cpp.Length == 0)
                        prj.EC.AddError("GAC XML", String.Format("Class {0} does not have a Cpp definition!", c.Name));
                    foreach (Constant cst in c.Constants)
                        if (cst.Cpp.Length == 0)
                            prj.EC.AddError("GAC XML", String.Format("Constant {0} from class {1} does not have a Cpp definition !", cst.Name, c.Name));
                    foreach (Function f in c.StaticFunctions)
                        if (f.Cpp.Length == 0)
                            prj.EC.AddError("GAC XML", String.Format("Function {0} from class {1} does not have a Cpp definition !", f.Name, c.Name));
                }
                foreach (StaticClass sc in system.StaticClasses.Values)
                {
                    foreach (Constant cst in sc.Constants)
                        if (cst.Cpp.Length == 0)
                            prj.EC.AddError("GAC XML", String.Format("Constant {0} from class {1} does not have a Cpp definition !", cst.Name, sc.Name));
                    foreach (Function f in sc.Functions)
                        if (f.Cpp.Length == 0)
                            prj.EC.AddError("GAC XML", String.Format("Function {0} from class {1} does not have a Cpp definition !", f.Name, sc.Name));
                    foreach (Variable v in sc.Variables)
                        if (v.Cpp.Length == 0)
                            prj.EC.AddError("GAC XML", String.Format("Member {0} from class {1} does not have a Cpp definition !", v.Name, sc.Name));
                }
                foreach (Enum e in system.Enums.Values)
                {
                    if (e.Cpp.Length == 0)
                        prj.EC.AddError("GAC XML", String.Format("Enum {0} does not have a Cpp definition  (a cpp type)!", e.Name));
                    foreach (Constant c in e.Constants)
                        if (c.Cpp.Length == 0)
                            prj.EC.AddError("GAC XML", String.Format("Member {0} from enum {1} does not have a Cpp definition !", c.Name, e.Name));
                }
                 */ 
                return !prj.EC.HasErrors();
            }
        }
        private class PreprocesorIF
        {
            public int Line, Start, End;
            public bool Else;
            public bool DoNotAdd;
            public bool Condition;
            public PreprocesorIF()
            {
                Line = Start = End = -1;
                Else = false;
                DoNotAdd = false;
                Condition = false;
            }
            public void Set(int line,int start,int end,bool condition,bool add)
            {
                Line = line;
                Start = start;
                End = end;
                Else = false;
                Condition = condition;
                DoNotAdd = add;
            }
        };
        public class Location
        {
            public Module module;
            public Member member;
            public int LineNumber;
            public string Line;
            public int CursorPositionInLine;
        };

        static public GACCompiler Compiler = new GACCompiler();
        static public Dictionary<string, bool> Builds = new Dictionary<string, bool>();
        static public Dictionary<string, bool> Defines = new Dictionary<string, bool>();
        static public Dictionary<string, bool> OS = new Dictionary<string, bool>()
        {
            {OSType.None.ToString(),true},
            {OSType.Android.ToString(),true},
            {OSType.IOS.ToString(),true},
            {OSType.WindowsDesktop.ToString(),true},
        };
        static public Dictionary<string, bool> BuildDefines = new Dictionary<string, bool>();
        static public Dictionary<string, Dictionary<string, string>> ExtraConstants = new Dictionary<string, Dictionary<string, string>>();
        static public string CurrentOS = "";
        static public string CurrentBuild = "";
        static public bool EnableMemoryHook = false;

        #region Preprocessor
        public static List<string> preprocessAutoCompleteList = new List<string>();
        public static Dictionary<string, string> preprocessCommands = new Dictionary<string, string>()
        {
            {"#addConstant",""},
            {"#define",""},
            {"#else",""},
            {"#endif",""},
            {"#ifdef",""},
            {"#ifdefoneof",""},
            {"#ifndef",""},
            {"#ifbuild",""},
            {"#ifbuilds",""},
            {"#ifplatform",""},
            {"#ifplatforms",""},
        };
        public static void CreatePreprocessAutoCompleteList()
        {
            foreach (string key in preprocessCommands.Keys)
                preprocessAutoCompleteList.Add(key + "?13");
            preprocessAutoCompleteList.Sort();
        }
        #endregion

        #region Internal Parse Object
        public enum TokenType
        {
            Keyword,
            AccessModifier,
            Operator,
            Number,
            String,
            Comment,

            SemiColumn,
            Column,
            Class,
            Enum,
            Point,
            TwoPoints,
            Word,

            Preprocesor,
            Override,
            Serializable,
            Persistent,
            Reference,
            TypeClass,
            TypeClassStatic,
            TypeEnum,
            Constant,
            TypeNamespace,

            BreketOpen,
            BreketClose,
            RoundBreketOpen,
            RoundBreketClose,
            SquareBreketOpen,
            SquareBreketClose,

            Equal,
            Minus,

            Constructor,
            Destructor,
            Base,

            This,
            New,
            Delete,
            Null,
            Global,
            Local,
            Scene,
            Application,
        };
        public class Token
        {
            public int Start, End, Link, Line;
            public TokenType Type;
            public string Text = "";
            public string Translate = "";
            public Module Obj=null;
            public void Create(Token t)
            {
                Start = t.Start;
                End = t.End;
                Type = t.Type;
                Link = t.Link;
                Line = t.Line;
                Text = t.Text;
                Obj = t.Obj;
            }
            public void CopyContentToTranslate(string preText,string postText)
            {
                Translate = preText + Text + postText;
            }
        };
        #endregion

        #region Specific Keywords
                
        
        public static bool LoadGacDefinitions(string gacXMLDefinitionsFile)
        {
            GACFile c = Compiler.GetGACFile("");
            c.Clear();
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Namespace));
                TextReader textReader = new StreamReader(gacXMLDefinitionsFile);
                Namespace n = (Namespace)serializer.Deserialize(textReader);
                if (n == null)
                    return false;                
                c.Clear();
                if (c.Add(null, n) == false)
                    return false;
                // fac si derivarile
                /*
                foreach (Module m in c.Modules.Values)
                {
                    if ((m.DerivedFrom != null) && (m.DerivedFrom.Length > 0))
                    {
                        Module deriv = Compiler.GetModule(m.DerivedFrom);
                        if (m.DerivedFrom == null)
                        {
                            MessageBox.Show("Class " + m.Name + " is derived from an unknwon class " + m.DerivedFrom);
                            return false;
                        }
                        foreach (Member mb in deriv.Members.Values)
                        {
                            if (mb.Type != MemberType.Function)
                                continue;
                            if (m.Members.ContainsKey(mb.Name))
                            {
                                MessageBox.Show("Class " + m.Name + " has a function ("+mb.Name+") that is already defined in " + m.DerivedFrom);
                                return false;
                            }
                            m.Members[mb.Name] = mb;
                        }
                    }
                }
                // */ 
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show("Unable to load GAC definitions ... GAC language will not work !"+"\n"+e.ToString());
                return false;
            }
        }
        public static List<string> LanguageKeywords = new List<string>();
        public static List<string> LanguageConstants = new List<string>();
        private static Dictionary<string, TokenType> gacKeywords = new Dictionary<string, TokenType>()
        {          
            {"public",TokenType.AccessModifier},
            {"private",TokenType.AccessModifier},
            {"protected",TokenType.AccessModifier},

            {"override",TokenType.Override},
            {"serializable",TokenType.Serializable},
            {"persistent",TokenType.Persistent},

            {"ref",TokenType.Reference},            

            {"class",TokenType.Class}, 
            {"scene",TokenType.Scene}, 
            {"application",TokenType.Application},

            {"enum",TokenType.Enum},
           
            {"if",TokenType.Keyword},
            {"while",TokenType.Keyword},
            {"else",TokenType.Keyword},
            {"do",TokenType.Keyword},
            {"continue",TokenType.Keyword},
            {"break",TokenType.Keyword},
            {"for",TokenType.Keyword},
            {"case",TokenType.Keyword},
            {"switch",TokenType.Keyword},
            {"return",TokenType.Keyword},               
            {"default",TokenType.Keyword},

            {"this",TokenType.This},

            // trebuiesc revizuite
            {"true",TokenType.Constant},
            {"false",TokenType.Constant},
            {"null",TokenType.Null},

            {"constructor",TokenType.Constructor},
            {"destructor", TokenType.Destructor},
            {"base",TokenType.Base},

            {"new",TokenType.New},
            {"delete",TokenType.Delete},
            {"global",TokenType.Global},
            {"local",TokenType.Local},
        };
        private static Dictionary<string, ModifierType> gacModifType = new Dictionary<string, ModifierType>()
        {
            {"public",ModifierType.Public},
            {"private",ModifierType.Private},
            {"protected",ModifierType.Protected},
        };
        private int[] CharType = new int[256];
        #endregion

        #region Internal vars
        private string Text;
        private int TextLength = 0;
        private int line = 1;        
        private string FileName = "";
        private bool doNotAddToken = false;
        private static string[] PreprocessWords = new string[16];
        private static int ArrayElementsCount = 0,ArrayStartToken = -1,ArrayEndToken = -1;
        
        #endregion

        #region Tokens
        Token[] Tokens = null;
        PreprocesorIF[] PreprocessIFStack = null;
        int tokensCount;
        int[] Stack;
        char[] StackExpectedChar;
        int topStack;
        int topPreprocesorIF;
        bool ReportAllErrors;

        void Resize(int newSize)
        {
            newSize = ((newSize / 2) + 1) * 2;
            Token[] newT = new Token[newSize];
            for (int tr = 0; tr < newT.Length; tr++)
                newT[tr] = new Token();
            for (int tr = 0; tr < tokensCount; tr++)
                newT[tr].Create(Tokens[tr]);
            Tokens = newT;
        }

        void ClearTokensList()
        {
            tokensCount = 0;
            topStack = 0;
            topPreprocesorIF = 0;
        }
        void Push(int start, int end)
        {
            if (doNotAddToken)
                return;
            if (tokensCount == Tokens.Length)
                Resize(tokensCount*2);
            Token tkn = Tokens[tokensCount];
            tkn.Start = start;
            tkn.End = end;
            tkn.Link = -1;
            tkn.Line = line;
            tkn.Text = Text.Substring(start, end - start);
            tkn.Translate = "";
            tkn.Obj = null;
            if (gacKeywords.TryGetValue(tkn.Text, out tkn.Type) == false)
            {
                tkn.Obj = Compiler.GetModule(tkn.Text);
                if (tkn.Obj != null)
                    tkn.Type = tkn.Obj.GetTokenType();
                else
                    tkn.Type = TokenType.Word;
            }
            tokensCount++;
        }
        void Push(int start, int end,TokenType t)
        {
            if (doNotAddToken)
                return;
            if (tokensCount == Tokens.Length)
                Resize(tokensCount*2);
            Token tkn = Tokens[tokensCount];
            tkn.Start = start;
            tkn.End = end;
            tkn.Type = t;
            tkn.Link = -1;
            tkn.Line = line;
            tkn.Text = Text.Substring(start, end - start);
            tkn.Translate = "";
            tkn.Obj = null;
            tokensCount++;
        }
        void PushOperator(int start, int end)
        {
            if (doNotAddToken)
                return;
            if (tokensCount == Tokens.Length)
                Resize(tokensCount*2);
            Token tkn = Tokens[tokensCount];
            tkn.Start = start;
            tkn.End = end;            
            tkn.Link = -1;
            tkn.Line = line;
            tkn.Text = Text.Substring(start, end - start);
            tkn.Obj = null;
            tkn.Translate = "";
            if (tkn.Text.Equals("="))
                tkn.Type = TokenType.Equal;
            else if (tkn.Text.Equals("-"))
                tkn.Type = TokenType.Minus;
            else
                tkn.Type = TokenType.Operator;
            tokensCount++;
        }
        void PushBraket(int start, TokenType t)
        {
            if (doNotAddToken)
                return;
            if (tokensCount == Tokens.Length)
                Resize(tokensCount*2);
            int id;
            Token tkn = Tokens[tokensCount];
            tkn.Start = start;
            tkn.End = start + 1;
            tkn.Type = t;
            tkn.Link = -1;
            tkn.Line = line;
            tkn.Text = "";
            tkn.Text += Text[start];
            tkn.Obj = null;
            tkn.Translate = "";
            

            switch (t)
            {
                case TokenType.SquareBreketOpen: topStack++; Stack[topStack] = tokensCount; StackExpectedChar[topStack] = ']'; break;
                case TokenType.RoundBreketOpen: topStack++; Stack[topStack] = tokensCount; StackExpectedChar[topStack] = ')'; break;
                case TokenType.BreketOpen: topStack++; Stack[topStack] = tokensCount; StackExpectedChar[topStack] = '}'; break;
                case TokenType.SquareBreketClose:
                case TokenType.RoundBreketClose:
                case TokenType.BreketClose:
                    if (topStack > 0)
                    {
                        if (StackExpectedChar[topStack] != Text[start])
                        {
                            AddError(line, start, start + 1, "Breket mismatch - expecting '"+StackExpectedChar[topStack]+"' !");
                        } else {
                            id = Stack[topStack];
                            Tokens[id].Link = tokensCount;
                            tkn.Link = id;
                        }
                        topStack--;
                    }
                    else
                    {
                        AddError(line, start, start+1, "Breket mismatch (Closed breket without opened breket) !");
                    }
                    break;
            }
            tokensCount++;
        }

        private string GetTokenText_(int index, bool normalize)
        {
            if (index < tokensCount)
                return GetTokenText_(Tokens[index], normalize);
            return "";
        }
        private string GetTokenText_(Token t, bool normalize)
        {
            string s = Text.Substring(t.Start, t.End - t.Start);
            if (normalize)
                s = s.Replace("\n", "").Replace("\r", "").Replace("\t", " ");
            return s;
        }
        private string GetTextBetweenTokens_(int index1, int index2)
        {
            if (index2 < index1)
                return "";
            return Text.Substring(Tokens[index1].Start, Tokens[index2].End - Tokens[index1].Start);
        }
        private string GetLine(int index)
        {
            int first, last;
            first = -1;
            last = -1;
            for (int tr = 0; tr < tokensCount; tr++)
                if (Tokens[tr].Line == index)
                {
                    last = tr;
                    if (first == -1)
                        first = tr;
                }
            if (first != -1)
                return GetTextBetweenTokens_(first, last);
            return "";
        }
        protected string GetTextFromTokens(Token[] list,int start,int end)
        {
            string s="";
            while (start<=end)
            {
                Token tmp = list[start];
                if (tmp.Type != TokenType.Comment)
                {
                    s += tmp.Text;
                    if (tmp.Type != TokenType.Point)
                    {
                        if ((start + 1 <= end) && (list[start + 1].Type == TokenType.Point)) { }
                        else { s += ' '; }
                    }
                }
                start++;
            }
            return s;
        }

        public void UpdateTokensForLocalModules()
        {
            for (int tr = 0; tr < tokensCount; tr++)
            {
                if (Tokens[tr].Type == TokenType.Word)
                {
                    Tokens[tr].Obj = Compiler.GetModule(Tokens[tr].Text);
                    if (Tokens[tr].Obj != null)
                        Tokens[tr].Type = Tokens[tr].Obj.GetTokenType();
                }
            }
        }
        #endregion

        #region Errors
        private static string Error = "";
        protected internal static string CreateErrorString(string fileName,int lineNumber,int start,int end,string text)
        {
            return String.Format("{0}({1})[{2}-{3}] Error: \"{4}\"", fileName, lineNumber, start, end, text);  
        }
        protected internal static string CreateErrorString(string fileName,Token t, string text)
        {
            if (t == null)
                return CreateErrorString(fileName, 0, 0,0, "INTERNAL_NULL_TOKEN:"+text);
            return CreateErrorString(fileName, t.Line, t.Start, t.End, text);
        }
        protected internal static void AddError(string fileName, Token t, string text)
        {
            if (t==null)
                Error += CreateErrorString(fileName, 0, 0, 0, text) + "\n";
            else
                Error += CreateErrorString(fileName, t, text) + "\n";
        }
        protected internal static void AddError(string fileName, int start, int end, string text)
        {
            Error += CreateErrorString(fileName, 0, start, end, text) + "\n";
        }
        protected internal static void AddError(string fileName, Member mb, string text)
        {
            Error += CreateErrorString(fileName,mb.LineNumber, mb.StartCode, mb.EndCode, text) + "\n";
        }
        private void AddError(int lineNumber,int start,int end, string text)
        {
            if (ReportAllErrors)
                Error += String.Format("{0} in line \"{1}\"\n", CreateErrorString(FileName, lineNumber, start, end, text), GetLine(lineNumber));                
        }
        private void AddError(Token t, string text)
        {
            AddError(t.Line, t.Start,t.End, text);
        }
        public static string GetError()
        {
            return Error;
        }
        public static void ClearError()
        {
            Error = "";
        }
        #endregion

        #region Constructor
        private void SetCharType(string text, int bit)
        {
            foreach (char ch in text)
                CharType[ch] |= bit;
        }
        public GACParser()
        {
            for (int tr = 0; tr < 256; tr++)
                CharType[tr] = 0;
            SetCharType("abcdefghijklmnopqrstuvwxyz_", 1);
            SetCharType("ABCDEFGHIJKLMNOPQRSTUVWXYZ", 1);
            SetCharType("0123456789", 1 | 2);
            SetCharType("0123456789abcdefABCDEF", 4);
            SetCharType("+-*/=%&|!~<>?", 8);
            SetCharType(",", 16);
            SetCharType(" \t", 32);
            SetCharType("\"'", 128);

            ReportAllErrors = true;
            tokensCount = 0;
            Resize(1024);
            Stack = new int[64];
            PreprocessIFStack = new PreprocesorIF[16];
            for (int tr = 0; tr < PreprocessIFStack.Length; tr++)
                PreprocessIFStack[tr] = new PreprocesorIF();
            StackExpectedChar = new char[64];
        }
        #endregion

        #region Parser
        public static string GetExtraConstant(string className,string valueName)
        {
            if (ExtraConstants.ContainsKey(className) == false)
                return null;
            if (ExtraConstants[className].ContainsKey(valueName) == false)
                return null;
            return ExtraConstants[className][valueName];
        }
        public bool LoadFile(string fileName, bool removeDebugCommands)
        {
            FileName = "";
            Text = Disk.ReadFileAsString(fileName, null);
            if (Text == null)
            {
                Text = "";
                TextLength = 0;
                return false;
            }
            Text = Text.Replace("\r\n", "\n").Replace("\r", "\n");
            TextLength = Text.Length;
            FileName = Path.GetFileName(fileName);
            bool res = Parse();
            if ((res) && (removeDebugCommands))
            {
                string temp = Text;
                if (RemoveOnDebugCommand())
                    res = Parse();
                Text = temp;
            }
            return res;
        }
        public bool Parse(string text,string fileName,bool removeDebugCommands)
        {
            ReportAllErrors = true;
            Text = text;
            Text = Text.Replace("\r\n", "\n").Replace("\r", "\n");
            TextLength = text.Length;
            FileName = Path.GetFileName(fileName);
            bool res = Parse();
            if ((res) && (removeDebugCommands))
            {
                string temp = Text;
                if (RemoveOnDebugCommand())
                    res = Parse();
                Text = temp;
            }
            return res;
        }
        public void QuickParse(string text)
        {
            ReportAllErrors = false;
            Text = text;
            TextLength = text.Length;
            FileName = null;
            Parse();
            ClearError();
        }
        private bool RemoveOnDebugCommand()
        {
            // caut in lista de tokeni on debug command
            for (int tr=0;tr<this.tokensCount-14;tr++)
            {
                if ((Tokens[tr+0].Type == TokenType.AccessModifier) &&
                    (Tokens[tr+1].Type == TokenType.Override) &&
                    (Tokens[tr+2].Type == TokenType.TypeClass) && (Tokens[tr+2].Text == "void") &&
                    (Tokens[tr+3].Type == TokenType.Word) && (Tokens[tr+3].Text == "OnDebugCommand") && 
                    (Tokens[tr+4].Type == TokenType.RoundBreketOpen)
                   )
                {
                    // am gasit o posibila combinatie
                    int cod = Tokens[tr + 4].Link + 1;
                    if ((cod<tokensCount) && (cod>=0) && (Tokens[cod].Type == TokenType.BreketOpen))
                    {
                        cod = Tokens[cod].Link;
                        if ((cod>tr) && (cod<tokensCount))
                        {
                            // sterg caracterele
                            StringBuilder sb = new StringBuilder(Text);
                            for (int poz = Tokens[tr].Start; poz < Tokens[cod].End;poz++)
                            {
                                if (sb[poz] != '\n')
                                    sb[poz] = ' ';
                            }
                            Text = sb.ToString();
                            return true;
                        }
                    }

                }
            }
            return false;
        }
        private int ParsePreprocesaor(int start)
        {
            //char lastChar = '\0';
            //char ch = '\0';
            while (start < TextLength)
            {
                if (Text[start] == '\n')
                    break;
                //lastChar = ch;
                //ch = Text[start];
                //if (ch == '\n')
                //    break;
                //if ((ch == '\n') && (lastChar != '\\'))
                //    break;
                //if (ch == '\n')
                //    line++;
                start++;
            }
            return start;
        }
        private int ParseText(int start)
        {
            char ch;
            while (start < TextLength)
            {
                ch = Text[start];
                if ((CharType[ch] & 1) == 0) break;
                start++;
            }
            return start;
        }
        private int ParseOperator(int start)
        {
            char ch;
            while (start < TextLength)
            {
                ch = Text[start];
                if ((CharType[ch] & 8) == 0) break;
                start++;
            }
            return start;
        }
        private int ParseNumber(int start)
        {
            char ch;
            int flag = 2;
            if ((Text[start] == '0') && (start + 2 < TextLength) && (Text[start + 1] == 'x'))
            {
                start += 2;
                flag = 4;
            }
            
            while (start < TextLength)
            {
                ch = Text[start];
                if ((CharType[ch] & flag) == 0) break;
                start++;
            }
            if ((flag==2) && (start<TextLength+1))
            {
                if (Text[start] == '.')
                {
                    start++;
                    while (start < TextLength)
                    {
                        ch = Text[start];
                        if ((CharType[ch] & 2) == 0) break;
                        start++;
                    }
                    if ((start < TextLength) && (Text[start] == 'f'))
                    {
                        start++;
                    }
                }
            }
            return start;
        }
        private int SkipOfType(int start, int tip)
        {
            char ch;
            while (start < TextLength)
            {
                ch = Text[start];
                if ((CharType[ch] & tip) == 0)
                    break;
                if (ch == '\n')
                    line++;
                start++;
            }
            return start;
        }
        private int ParseString(int start)
        {
            char ch, chs;
            chs = Text[start];
            int poz = start + 1;
            
            while (poz < TextLength)
            {
                ch = Text[poz];
                if (ch == '\\')
                {
                    poz += 2;
                    continue;
                }
                if (ch == chs)
                {
                    poz++;
                    break;
                }
                if (ch == '\n')
                {
                    AddError(line,start,poz, "Incomplete string ("+Text.Substring(start,poz-start)+") !");
                    break;
                }
                poz++;
            }
            return poz;
        }
        private int ParseComment(int start)
        {
            char ch;
            // se preupune ca incep cu //
            if (start + 1 == TextLength)
                return -1;
            ch = Text[start + 1];
            if (ch == '/')
            {
                start += 2;
                while ((start < TextLength) && (Text[start] != '\n'))
                    start++;
                return start;
            }
            if (ch == '*')
            {
                int startComent=start;
                start += 2;
                while (start < TextLength)
                {
                    ch = Text[start];
                    if ((ch == '*') && (start + 1 < TextLength) && (Text[start + 1] == '/'))
                        return start + 2;
                    if (ch == '\n')
                        line++;
                    start++;
                }
                AddError(line, startComent, startComent + 2, "Multi line comment not closed !");
                return start;
            }
            return -1;
        }
        private bool AddPreprocesorIF(int line, int start, int end,bool conditionIsTrue)
        {
            if (topPreprocesorIF+1==PreprocessIFStack.Length)
            {
                AddError(line, start, end, "Too many preprocessor commands - maximum depth allowed for preprocess commands is 16");
                return false;
            }
            topPreprocesorIF++;
            PreprocesorIF p = PreprocessIFStack[topPreprocesorIF];
            p.Set(line, start, end, conditionIsTrue, doNotAddToken);
            doNotAddToken |= (!conditionIsTrue);
            return true;
        }
        private bool AddExtraConstant(GACFile gf, int line, int start, int end, string className, string name, string value)
        {
            if (ExtraConstants.ContainsKey(className) == false)
                ExtraConstants[className] = new Dictionary<string, string>();
            int size = ExtraConstants[className].Count;
            if (className == "Scenes")
            {
                if (ExtraConstants[className].ContainsKey(name) == false)
                    value = (32 + ExtraConstants[className].Count).ToString();
                else
                    value = ExtraConstants[className][name];
            }
            if (ExtraConstants[className].ContainsKey(name)==false)
            {
                ExtraConstants[className][name] = value;
            }
            else
            {
                if (ExtraConstants[className][name]!=value)
                {
                    AddError(line, start, end, "Value '"+className+"."+name+"' has already been defined with value ('"+value+"') !");
                    return false; 
                }
            }
            gf.PreprocessCommands.Add("#define GAC_ADDED_CONSTANT_" + className + "_" + name + " " + value);
            return true;
        }
        private bool AnalizePreprocesorCommand_addConstant(GACFile gf,int line, int start, int end, int count)
        {
            if (count != 4)
            {
                AddError(line, start, end, "Incorect number of parameters for #addConstant -> expecting #addConstant(enum/class,name,value)");
                return false;
            }
            return AddExtraConstant(gf, line, start, end, PreprocessWords[1], PreprocessWords[2], PreprocessWords[3]);
        }
        
        private bool AnalizePreprocesorCommand_ifdef(int line, int start, int end, int count)
        {
            if (count != 2)
            {
                AddError(line, start, end, "Incorect number of parameters for #ifdef -> expecting ##ifdef define");
                return false;
            }
            if (Defines.ContainsKey(PreprocessWords[1]) == false)
            {
                AddError(line, start, end, "Define '" + PreprocessWords[1] + "' was not defined for this project !");
                return false;
            }
            AddPreprocesorIF(line, start, end, Defines[PreprocessWords[1]]);
            // all good
            return true;
        }
        private bool AnalizePreprocesorCommand_ifndef(int line, int start, int end, int count)
        {
            if (count != 2)
            {
                AddError(line, start, end, "Incorect number of parameters for #ifndef -> expecting #ifndef define");
                return false;
            }
            if (Defines.ContainsKey(PreprocessWords[1]) == false)
            {
                AddError(line, start, end, "Define '" + PreprocessWords[1] + "' was not defined for this project !");
                return false;
            }
            AddPreprocesorIF(line, start, end, !Defines[PreprocessWords[1]]);
            // all good
            return true;
        }
        private bool AnalizePreprocesorCommand_ifdefoneof(int line, int start, int end, int count)
        {
            if (count < 2)
            {
                AddError(line, start, end, "Incorect number of parameters for #ifdefoneof -> expecting #ifdefoneof define_1,[define_2,define_3, ... define_n]");
                return false;
            }
            for (int tr = 1; tr < count; tr++)
            {
                if (Defines.ContainsKey(PreprocessWords[tr]) == false)
                {
                    AddError(line, start, end, "Define '" + PreprocessWords[tr] + "' was not defined for this project !");
                    return false;
                }
            }
            bool res = false;
            for (int tr = 1; tr < count; tr++)
            {
                res |= Defines[PreprocessWords[tr]];
            }
            AddPreprocesorIF(line, start, end, res);
            // all good
            return true;
        }
        private bool AnalizePreprocesorCommand_ifbuild(int line, int start, int end, int count)
        {
            if (count != 2)
            {
                AddError(line, start, end, "Incorect number of parameters for #ifbuild -> expecting #ifbuild build_name");
                return false;
            }
            if (Builds.ContainsKey(PreprocessWords[1]) == false)
            {
                AddError(line, start, end, "Build '" + PreprocessWords[1] + "' is not a valid build !");
                return false;
            }
            AddPreprocesorIF(line, start, end, PreprocessWords[1] == CurrentBuild);
            // all good
            return true;
        }
        private bool AnalizePreprocesorCommand_ifbuilds(int line, int start, int end, int count)
        {
            if (count < 2)
            {
                AddError(line, start, end, "Incorect number of parameters for #ifbuilds -> expecting #ifbuild build_name_1,[build_name_2,build_name_3, ... build_name_n]");
                return false;
            }
            for (int tr = 1; tr < count; tr++)
            {
                if (Builds.ContainsKey(PreprocessWords[tr]) == false)
                {
                    AddError(line, start, end, "Build '" + PreprocessWords[tr] + "' is not a valid build !");
                    return false;
                }
            }
            bool res = false;
            for (int tr = 1; tr < count; tr++)
            {
                res |= (PreprocessWords[tr] == CurrentBuild);                    
            }
            AddPreprocesorIF(line, start, end, res);
            // all good
            return true;
        }
        private bool AnalizePreprocesorCommand_ifplatform(int line, int start, int end, int count)
        {
            if (count != 2)
            {
                AddError(line, start, end, "Incorect number of parameters for #ifplatform -> expecting #ifplatform build_name");
                return false;
            }
            if (OS.ContainsKey(PreprocessWords[1]) == false)
            {
                AddError(line, start, end, "Platform '" + PreprocessWords[1] + "' is not a valid platform !");
                return false;
            }
            AddPreprocesorIF(line, start, end, PreprocessWords[1] == CurrentOS);
            // all good
            return true;
        }
        private bool AnalizePreprocesorCommand_else(int line, int start, int end, int count)
        {
            if (topPreprocesorIF == 0)
            {
                AddError(line, start, end, "#else preprocessor definition without #if");
                return false;
            }
            PreprocesorIF p = PreprocessIFStack[topPreprocesorIF];
            if (p.Else)
            {
                AddError(line, start, end, "There is already a #else preprocessor definition for current #if");
                return false;
            }
            p.Else = true;
            doNotAddToken = (p.Condition) | (p.DoNotAdd);
            return true;
        }
        private bool AnalizePreprocesorCommand_endif(int line, int start, int end, int count)
        {
            if (topPreprocesorIF == 0)
            {
                AddError(line, start, end, "#endif preprocessor definition without #if");
                return false;
            }
            doNotAddToken = PreprocessIFStack[topPreprocesorIF].DoNotAdd;
            topPreprocesorIF--;
            return true;
        }
        private bool AnalizePreprocesorCommand(GACFile gf,int line, int start, int end)
        {
            int count = 0;
            PreprocessWords[0] = "";
            bool inString = false;
            char ch;
            for (int tr = start; tr < end;tr++ )
            {
                ch = Text[tr];
                if (((ch == ' ') || (ch == '\t')) && (!inString))
                {
                    if (PreprocessWords[count].Length == 0)
                        continue;
                    count++;
                    if (count >= PreprocessWords.Length)
                    {
                        AddError(line, start, end, "Too many parameters for preprocessor definition !");
                        return false;
                    }
                    PreprocessWords[count] = "";
                    continue;

                }
                if (((ch == '(') || (ch == ')') || (ch == ',')) && (!inString))
                {
                    if (PreprocessWords[count].Length == 0)
                    {
                        AddError(line,start,end, "Empty parameter for preprocessor definition !");
                        return false;
                    }
                    count++;
                    if (count >= PreprocessWords.Length)
                    {
                        AddError(line, start, end, "Too many parameters for preprocessor definition !");
                        return false;
                    }
                    PreprocessWords[count] = "";
                    continue;
                }
                if (ch == '"')
                    inString = !inString;
                if ((ch == ' ') && (inString == false))
                    continue;
                PreprocessWords[count] += ch;
            }
            if (PreprocessWords[count].Length > 0)
                count++;

            // totul e ok - analizez 
            if (PreprocessWords[0] == "#addConstant") // #addConstant(enum/clasa,nume,valoare)
                return AnalizePreprocesorCommand_addConstant(gf, line, start, end, count);
            //if (PreprocessWords[0] == "#addNewScene") // #addNewScene(numee)
            //    return AnalizePreprocesorCommand_addNewScene(gf, line, start, end, count);
            // conditii
            if (PreprocessWords[0] == "#ifdef")
                return AnalizePreprocesorCommand_ifdef(line, start, end, count);
            if (PreprocessWords[0] == "#ifndef")
                return AnalizePreprocesorCommand_ifndef(line, start, end, count);
            if (PreprocessWords[0] == "#ifdefoneof")
                return AnalizePreprocesorCommand_ifdefoneof(line, start, end, count);
            // pentru platforma
            if (PreprocessWords[0] == "#ifbuild")
                return AnalizePreprocesorCommand_ifbuild(line, start, end, count);
            if (PreprocessWords[0] == "#ifbuilds")
                return AnalizePreprocesorCommand_ifbuilds(line, start, end, count);
            if (PreprocessWords[0] == "#ifplatform")
                return AnalizePreprocesorCommand_ifplatform(line, start, end, count);
            if (PreprocessWords[0] == "#else")
                return AnalizePreprocesorCommand_else(line, start, end, count);
            if (PreprocessWords[0] == "#endif")
                return AnalizePreprocesorCommand_endif(line, start, end, count);
            AddError(line, start, end, "Unknown preprocessor definition ("+PreprocessWords[0]+") !");
            return false; 
        }
        private bool Parse()
        {
            ClearTokensList();
            int start = 0;
            char ch;
            int tip, end = 0;
            bool firstInLine = true;
            line = 1;
            Error = "";
            doNotAddToken = false;
            GACFile gf = null;
            if (FileName != null)
                gf = Compiler.GetGACFile(FileName);
            if (gf!=null)
                gf.Clear();
            
            while (end < TextLength)
            {
                start = end;
                ch = Text[start];
                if ((firstInLine) && (ch > 32) && (ch != '#'))
                    firstInLine = false;
                tip = CharType[ch];
                if (ch == '/')
                {
                    end = ParseComment(start);
                    if (end > start)
                        continue;
                }
                // verific numar
                if ((tip & 2) != 0)
                {
                    end = ParseNumber(start);
                    Push(start, end, TokenType.Number);
                    continue;
                }
                // verific operator
                if ((tip & 8) != 0)
                {
                    end = ParseOperator(start);
                    PushOperator(start, end);
                    continue;
                }
                // daca am string
                if ((tip & 128) != 0)
                {
                    end = ParseString(start);
                    Push(start, end, TokenType.String);
                    continue;
                }
                switch (ch)
                {
                    case ',': end = start + 1; Push(start, end,  TokenType.Column); continue;
                    case ';': end = start + 1; Push(start, end,  TokenType.SemiColumn); continue;
                    case '.': end = start + 1; Push(start, end,  TokenType.Point); continue;
                    case ':': end = start + 1; Push(start, end,  TokenType.TwoPoints); continue;
                    case '{': end = start + 1; PushBraket(start,  TokenType.BreketOpen); continue;
                    case '}': end = start + 1; PushBraket(start, TokenType.BreketClose); continue;
                    case '(': end = start + 1; PushBraket(start, TokenType.RoundBreketOpen); continue;
                    case ')': end = start + 1; PushBraket(start, TokenType.RoundBreketClose); continue;
                    case '[': end = start + 1; PushBraket(start, TokenType.SquareBreketOpen); continue;
                    case ']': end = start + 1; PushBraket(start, TokenType.SquareBreketClose); continue;
                    case '#':
                        end = ParsePreprocesaor(start);
                        if ((!firstInLine) && (ReportAllErrors))
                            AddError(line, start, end, "Preprocesor commands should be first on a new line !");
                        if (gf != null)
                            AnalizePreprocesorCommand(gf,line, start, end);
                        //Push(start, end, TokenType.Preprocesor);
                        firstInLine = false;
                        continue;
                    case '\n': end = start + 1; line++; firstInLine = true; continue;
                }

                // daca am un text
                if ((tip & 1) != 0)
                {
                    end = ParseText(start);
                    Push(start, end);
                    continue;
                }
                // skip la spatii
                if (tip != 0)
                {
                    end = SkipOfType(start, tip);
                    continue;
                }
                if (ReportAllErrors)
                {
                    if (Text[start] < 32)
                        AddError(line, start, start + 1, String.Format("Unknown character with code 0x{0:X} found !", (int)Text[start]));
                    else
                        AddError(line, start, start + 1, String.Format("Unknown character with code 0x{0:X} '{1}' found !", (int)Text[start], Text[start]));
                }
                end = start + 1;
            }
            if (ReportAllErrors)
            {
                // topAcolada
                for (int tr = 0; tr < topStack; tr++)
                    AddError(Tokens[Stack[tr]], "Missmatch breket (opened and not closed) !");
                // topPreprocess
                if (topPreprocesorIF > 0)
                {
                    AddError(PreprocessIFStack[1].Line, PreprocessIFStack[1].Start, PreprocessIFStack[1].End, "#if without #end");
                }
                return (Error.Length == 0);
            }
            else
            {
                return true;
            }
            
        }        
        #endregion

        #region Compile
        
        private bool SkipNamespaceToken(ref int start)
        {
            int sstart = start;
            if (start >= tokensCount)
                return true;
            // daca nu incep cu un namespace - nu prea are sens continu cautarea
            if (Tokens[start].Type != TokenType.TypeNamespace)
                return true;
            // verific si daca namespace-urile sunt ok (semantic)
            Module last = null;
            while (start < tokensCount)
            {
                switch (Tokens[start].Type)
                {
                    case TokenType.TypeClass:
                        if (last.Modules.ContainsKey(Tokens[start].Text)==false)                        
                        {
                            AddError(Tokens[start], String.Format("Class '{0}' is not defined in namespace '{1}'", Tokens[start].Text, last.Name));
                            return false;
                        }
                        return true;
                    case TokenType.TypeClassStatic:
                        if (last.Modules.ContainsKey(Tokens[start].Text) == false)  
                        {
                            AddError(Tokens[start], String.Format("Static class '{0}' is not defined in namespace '{1}'", Tokens[start].Text, last.Name));
                            return false;
                        }
                        return true;
                    case TokenType.TypeEnum:
                        if (last.Modules.ContainsKey(Tokens[start].Text) == false) 
                        {
                            AddError(Tokens[start], String.Format("Enumeration '{0}' is not defined in namespace '{1}'", Tokens[start].Text, last.Name));
                            return false;
                        }
                        return true;
                    case TokenType.TypeNamespace:
                        if (last != null)
                        {
                            if (last.Modules.ContainsKey(Tokens[start].Text) == false) 
                            {
                                AddError(Tokens[start], String.Format("Namespace '{0}' is not defined in namespace '{1}'", Tokens[start].Text, last.Name));
                                return false;
                            }
                        }
                        last = Tokens[start].Obj;
                        break;
                    default:
                        AddError(Tokens[start], String.Format("Unexpected word '{0}' in namespace '{1}'", Tokens[start].Text, last.Name));
                        return false;
                }
                start++;
                if (start < tokensCount)
                {
                    if (Tokens[start].Type == TokenType.Point)
                    {
                        start++;
                        continue;
                    }
                    // am o structura de genu Namespace <altceva> - returnez pozitia namespace-ului
                    AddError(Tokens[start-1],String.Format("Invalid usage of namespace '{0}'. It should have been preceded by a '.' operator !",last.Name));
                    start--;
                    return false;
                }
            }
            AddError(Tokens[sstart], String.Format("Invalid usage of namespace '{0}' - used until end of file !", Tokens[sstart].Text));
            start = tokensCount - 1;
            return false;            
        }
        private int FindNextInList(int start, int end)
        {
            // merg pana cand dau de o virgula, sau pana cand am depasit end
            while (start < end)
            {
                if (Tokens[start].Type == TokenType.Column)
                    return start;
                if ((Tokens[start].Type == TokenType.BreketOpen) || (Tokens[start].Type == TokenType.RoundBreketOpen) || (Tokens[start].Type == TokenType.SquareBreketOpen))
                {
                    start = Tokens[start].Link + 1;
                }
                else
                {
                    start++;
                }
            }
            return start;
        }
        private int FindFirstSemiColumn(int start, int end)
        {
            while (start < end)
            {
                if (Tokens[start].Type == TokenType.SemiColumn)
                    return start;
                start++;
            }
            return -1;
        }

        private int AnalizeApplicationDefinition(GACFile gf, int poz)
        {
            if (CheckToken(poz + 2) == false)
                return -1;
            if (Tokens[poz + 1].Type != TokenType.BreketOpen)
            {
                AddError(Tokens[poz + 1], "Expecting '{' in class definition !");
                return -1;
            }
            Module m = new Module(ModuleType.Class, "App", "", "Application", false, Tokens[poz], FileName);
            m.TypeInformation = ModuleTypeInformation.IsApp;
            m.StartToken = poz + 2;
            m.EndToken = Tokens[poz+1].Link - 1;
            gf.Modules[m.Name] = m;
            gf.FileType = GACFileType.Application;
            return Tokens[poz+1].Link + 1;
        }
        private int AnalizeClassDefinition(GACFile gf, int poz)
        {
            int start, derivate, name;
            TokenType typeDef = Tokens[poz].Type;
            while (true)
            {
                if (CheckToken(poz + 3) == false)
                    return -1;
                if ((Tokens[poz + 1].Type != TokenType.Word) && (Tokens[poz + 1].Type != TokenType.TypeClass))
                {
                    AddError(Tokens[poz + 1], "Invalid qualifier after keyword class (expecting a name)");
                    return -1;
                }
                name = poz+1;
                if (Tokens[name].Text == "App")
                {
                    AddError(Tokens[name], "'App' is a special name and can not be used to declare a class/scene !");
                    return -1;
                }
                if (Tokens[poz + 2].Type == TokenType.BreketOpen)
                {
                    start = poz + 2;
                    derivate = -1;
                    break;
                }
                if (Tokens[poz + 2].Type == TokenType.TwoPoints)
                {
                    // fac skip la namespace-uri
                    poz += 3;
                    if (SkipNamespaceToken(ref poz) == false)
                        return -1;
                    if (Tokens[poz + 1].Type != TokenType.BreketOpen)
                    {
                        AddError(Tokens[poz + 1], "Expecting '{' in class definition !");
                        return -1;
                    }
                    derivate = poz;
                    start = poz + 1;
                    if ((Tokens[poz].Type != TokenType.TypeClass) && (Tokens[poz].Type != TokenType.Word))
                    {
                        AddError(Tokens[poz], String.Format("Invalid type ({0}) for class derivation ! Expecting a class !", Tokens[poz].Text));
                        return Tokens[start].Link + 1;
                    }
                    break;
                }
                AddError(Tokens[poz + 2], "Expecting '{' in class definition !");
                return -1;
            }
            if (IsDeclared(Tokens[name]))
                return Tokens[start].Link + 1;
            Module m = new Module(ModuleType.Class, Tokens[name].Text, "", "", false,Tokens[name],FileName);
            if (derivate >= 0)
            {                
                m.DerivedFrom = Tokens[derivate].Text;
            }
            if (typeDef == TokenType.Scene) 
            {
                if (m.DerivedFrom.Length == 0)
                    m.DerivedFrom = "Scene";
                gf.Scenes.Add(m.Name);
                gf.FileType = GACFileType.Scene;
                m.TypeInformation = ModuleTypeInformation.IsScene;
            }              
            if (m.DerivedFrom == "Application")
            {
                AddError(Tokens[name], "A class/scene can not be derived from an application !");
                return -1;
            }
            m.StartToken = start + 1;
            m.EndToken = Tokens[start].Link - 1;
            gf.Modules[m.Name] = m;
            return Tokens[start].Link + 1;
        }
        private int AnalizeEnumDefinition(GACFile gf, int start)
        {
            // se subintelege ca la Tokens[start] am un enum
            // enum <word> : <tip> {..}
            if (start + 5 >= tokensCount)
            {
                AddError(Tokens[start], "Incomplete usage of enum definition !");
                return -1;
            }
            if (Tokens[start + 1].Type != TokenType.Word)
            {
                AddError(Tokens[start + 1], "Expecting a name for Enum definition (" + Tokens[start + 1].Text + ") !");
                return -1;
            }
            if (Tokens[start + 2].Type != TokenType.TwoPoints)
            {
                AddError(Tokens[start + 2], "Expecting a ':' for Enum definition  !");
                return -1;
            }
            if (Tokens[start + 4].Type != TokenType.BreketOpen)
            {
                AddError(Tokens[start + 4], "Expecting a '{' for enum declaration");
                return -1;
            }
            int end_enum = Tokens[start + 4].Link;
            if (Tokens[start + 3].Type != TokenType.TypeClass)
            {
                AddError(Tokens[start + 3], "Expecting a basic type (int, byte) for Enum type !");
                return end_enum+1;
            }
            if (Tokens[start + 3].Obj.BasicType == false)
            {
                AddError(Tokens[start + 3], "A enum should be of a basic type (int, float, ...) and not '" + Tokens[start + 3].Text + "' ");
                return end_enum + 1;
            }
            // totul e ok
            if (IsDeclared(Tokens[start + 1]))
                return end_enum + 1;
            Module m = new Module(ModuleType.Enum, Tokens[start + 1].Text, Tokens[start + 3].Obj.Cpp, "", true,Tokens[start + 1],FileName);
            gf.Modules[m.Name] = m;
            m.Icon = GACIcon.Enum;
            m.StartToken = start + 5;
            m.EndToken = end_enum - 1;
            
            // caut definitiile (word = ... ,)
            int poz = start + 5;
            while (poz < end_enum)
            {
                // formatul este word = [string | numar]
                if (poz + 3 > end_enum)
                {
                    AddError(Tokens[poz], "Incorect deffinition for enumeration constant !");
                    return end_enum + 1;
                }
                if (Tokens[poz].Type != TokenType.Word)
                {
                    AddError(Tokens[poz], "Expecting a word in enum constants (<word> = value) !");
                    return end_enum + 1;
                }
                if (Tokens[poz+1].Type != TokenType.Equal)
                {
                    AddError(Tokens[poz+1], "Expecting equal operator in enum constants (<word> = value) !");
                    return end_enum + 1;
                }
                int txName = poz;
                poz += 2;
                string defineValue = null;
                if (Tokens[poz].Type == TokenType.String)
                {
                    defineValue = Tokens[poz].Text;
                    poz++;
                }
                else if (Tokens[poz].Type == TokenType.Number)
                {
                    defineValue = "(" + Tokens[poz].Text + ")";
                    poz++;
                }
                else if ((Tokens[poz].Type == TokenType.Minus) && (Tokens[poz + 1].Type == TokenType.Number))
                {
                    defineValue = "(-" + Tokens[poz+1].Text + ")";
                    poz += 2;
                }

                if (defineValue == null)
                {
                    AddError(Tokens[poz + 2], "Expecting a constant (number or string) !");
                    return end_enum + 1;
                }
                if ((poz < end_enum) && (Tokens[poz].Type == TokenType.Column))
                    poz++;

                if (m.Members.ContainsKey(Tokens[txName].Text) == true)
                {
                    AddError(Tokens[txName], String.Format("Constant {0} was already defined in {1}", Tokens[txName].Text, m.Name));
                    return end_enum + 1;
                }
                m.Members[Tokens[txName].Text] = new Member(m, MemberType.Constant, Tokens[txName].Text, "GAC_ENUM_" + m.Name.ToUpper() + "_" + Tokens[txName].Text.ToUpper(), m.Cpp, "", Tokens[txName]);
                gf.PreprocessCommands.Add("#define GAC_ENUM_" + m.Name.ToUpper() + "_" + Tokens[txName].Text.ToUpper() + " " + defineValue);
                
            }
            m.UpdateAutoCompleteList();
            

            return end_enum + 1;
        }
        private int SkipGlobalDefinition(int start)
        {
            // se intelege ca primul token e "global"
            if (CheckToken(start + 1) == false)
                return -1;
            if (Tokens[start + 1].Type != TokenType.BreketOpen)
            {
                AddError(Tokens[start + 1], "Expecting '{' in global definition !");
                return -1;
            }
            Module m = Compiler.GetModule("Global");
            m.Modules.Clear();
            m.Members.Clear();
            m.Order = 1000;
            m.FileName = FileName;
            m.StartToken = start + 2;
            m.EndToken = Tokens[start + 1].Link - 1;
            return Tokens[start + 1].Link + 1;
        }
        private int AnalizeArray(Module cls, Member currentFunction, int start)
        {
            if (Tokens[start].Type != TokenType.SquareBreketOpen)
            {
                ArrayElementsCount = 0;
                return start;
            }
            int count = 0;            
            int next, poz = start+1;
            int end = Tokens[start].Link;

            if (poz == end) // sunt pe cazul []
            {
                count = 1;
            }
            else
            {
                while (poz < end)
                {
                    next = FindNextInList(poz, end);
                    if (poz <= next)
                        AnalizeCode(poz, next - 1, cls, currentFunction, 0);
                    count++;
                    if (Tokens[next].Type == TokenType.Column)
                        Tokens[next].Translate = "][";
                    poz = next + 1;
                }
            }
            ArrayStartToken = start;
            ArrayEndToken = Tokens[start].Link;
            Tokens[ArrayStartToken].CopyContentToTranslate("", "");
            Tokens[ArrayEndToken].CopyContentToTranslate("", "");
            ArrayElementsCount = count;
            return ArrayEndToken + 1;            
        }
        private bool AnalizeFunctionParameter(Module c,Member f,int poz)
        {
            bool reference = false;
            if (Tokens[poz].Type == TokenType.Reference)
            {
                reference = true;
                poz++;
            }
            if (f.Name == "Sum2")
            {
                f.Name.Trim();
            }
            if (SkipNamespaceToken(ref poz) == false)
                return false;
            if ((Tokens[poz].Type != TokenType.TypeClass) && (Tokens[poz].Type != TokenType.TypeEnum))
            {
                AddError(Tokens[poz], String.Format("Invalid qualifier ({0}) - expecting a type or enum !", Tokens[poz].Text));
                return false;
            }
            if (Tokens[poz + 1].Type != TokenType.Word)
            {
                AddError(Tokens[poz + 1], String.Format("Invalid qualifier ({0}) - expecting a name for function parameter !", Tokens[poz + 1].Text));
                return false;
            }
            // daca e array
            int next = AnalizeArray(c,f,poz+2);
            // trebuie sa fiu urmat de , sau )
            if ((Tokens[next].Type != TokenType.RoundBreketClose) && (Tokens[next].Type != TokenType.Column))
            {
                AddError(Tokens[next], String.Format("Invalid qualifier ({0}) in a function definition !", Tokens[next].Text));
                return false;
            }
            Member ex = c.GetMember(Tokens[poz + 1].Text);
            if (ex != null)
            {
                AddError(Tokens[poz + 1], String.Format("{0} is already defined in class {1} or one of its base classses !", Tokens[poz + 1].Text,c.Name));
                return false;
            }
            ex = f.GetParameter(Tokens[poz + 1].Text);
            if (ex != null)
            {
                AddError(Tokens[poz + 1], String.Format("{0} is already defined as a parameter !", Tokens[poz + 1].Text));
                return false;
            }
            Module vtype = Compiler.GetModule(Tokens[poz].Text);
            if ((reference) && (vtype.BasicType == false))
            {
                AddError(Tokens[poz], String.Format("Reference can only be applied to a basic type (int,char,...) !"));
                return false;
            }
            Member mb = new Member(c, MemberType.Variable, Tokens[poz + 1].Text, "", Tokens[poz].Text, "", Tokens[poz + 1]);
            mb.Reference = reference;
            if (ArrayElementsCount > 0)
            {
                mb.ArrayCount = ArrayElementsCount;
                mb.ArrayCppCode = CreateCppCode(ArrayStartToken, ArrayEndToken);
                mb.ArrayGacCode = GetTextFromTokens(Tokens, ArrayStartToken, ArrayEndToken);
            }
            return f.AddParameter(mb);
        }
        private int AnalizeModuleFunction(Module c, ModifierType access, bool overRide, int poz)
        {
            // stiu ca de la poz am type nume (...) { ... }   
            int nameToken = poz + 1;
            Member f = new Member(c, MemberType.Function, Tokens[nameToken].Text, "", Tokens[poz].Text, "", Tokens[nameToken]);
            f.Override = overRide;
            f.Access = access;
            int end_p = Tokens[poz + 2].Link;
            poz += 3;
            while (poz < end_p)
            {
                AnalizeFunctionParameter(c, f, poz);
                poz = FindNextInList(poz, end_p) + 1; // ca sa fac skip peste virgula
            }
            f.StartCode = end_p + 2;
            f.EndCode = Tokens[end_p + 1].Link - 1;
            int next = Tokens[end_p + 1].Link + 1;
            // extra verificari
            Member ex = c.GetMember(f.Name);
            if (ex != null)
            {
                if (ex.Type != MemberType.Function)
                {
                    AddError(Tokens[nameToken], String.Format("Class {0} or one of its base classes already has a non-function member (variable,constant) named {1}", c.Name, f.Name));
                    return next;
                }
                // este functie
                if (ex.GetFunctionSignature().Equals(f.GetFunctionSignature()))
                {
                    // este exact acceasi functie
                    if (ex.Virtual == false)
                    {
                        AddError(Tokens[nameToken], String.Format("Function {0} was not defined as virtual in the base class and cannot be overridden", f.GetFunctionSignature()));
                        return next;
                    }
                    if (f.Override == false)
                    {
                        AddError(Tokens[nameToken], String.Format("Function {0} does not have the override specifier in its definition !", f.GetFunctionSignature()));
                        return next;
                    }
                    if (f.DataType != ex.DataType)
                    {
                        AddError(Tokens[nameToken], String.Format("Function {0} has a different return type ({1}) than the function that it overrides ({2}", f.GetFunctionSignature(), f.DataType, ex.DataType));
                        return next;
                    }
                    f.Virtual = true;
                    c.Members[f.Name] = f;
                    return next;
                }
                else
                {
                    if (ex.Parent != f.Parent)
                    {
                        AddError(Tokens[nameToken], String.Format("Function {0} has already been declared in base class {1} with the signare {2}", f.GetFunctionSignature(), ex.Parent.Name, ex.GetFunctionSignature()));
                        return next;
                    }
                    // e vorba de alta functie
                    if (ex.Overrides == null)
                        ex.Overrides = new List<Member>();
                    ex.Overrides.Add(f);                    
                    //AddError(Tokens[nameToken], String.Format("Function {0} was already defined as {1} in {2} or one of its base classes", f.GetFunctionSignature(), ex.GetFunctionSignature(), c.Name));
                    return next;
                }
            }
            // functie noua - verific sa nu fi pus override
            if (f.Override == true)
            {
                AddError(Tokens[nameToken], String.Format("Function {0} can not override a function that was not declared in the base class", f.GetFunctionSignature()));
                return next;
            }
            c.Members[f.Name] = f;
            return next;
        }
        private int AnalizeModuleVariable(Module c, ModifierType access, int poz, SerializableType serializable)
        {
            // pot sa am urmatoarele formate
            // type nume
            // type nume[,,,],

            string type = Tokens[poz].Text;
            
            int namePoz = -1;
            poz++;
            while (poz < tokensCount)
            {
                if (Tokens[poz].Type == TokenType.SemiColumn)
                {
                    // totul e ok - am gasit ;
                    return poz + 1;
                }
                if (Tokens[poz].Type != TokenType.Word)
                {
                    AddError(Tokens[poz], String.Format("Invalid name '{0}' for a variable (either already defined or invalid token)", Tokens[poz].Text));
                    return -1;
                }
                namePoz = poz;                
                poz++;
                poz = AnalizeArray(c, null, poz);
                // verifica sa nu are ,
                if (poz < tokensCount)
                {
                    if (Tokens[poz].Type == TokenType.Column)
                        poz++;
                }
                Member ex = c.GetMember(Tokens[namePoz].Text);
                if (ex != null)
                {
                    AddError(Tokens[namePoz],String.Format("Member variable {0} is already defined in {1} or in one of its base classes !",Tokens[namePoz].Text,c.Name));
                    continue;
                }
                // ok - variabila nu exista
                Member newVar = new Member(c, MemberType.Variable, Tokens[namePoz].Text, "", type, "", Tokens[namePoz]);
                newVar.Serializable = serializable;
                newVar.StartCode = Tokens[namePoz].Start;
                newVar.EndCode = Tokens[namePoz].End;
                newVar.LineNumber = Tokens[namePoz].Line;
                if (ArrayElementsCount > 0)
                {
                    newVar.ArrayCount = ArrayElementsCount;
                    newVar.ArrayCppCode = CreateCppCode(ArrayStartToken, ArrayEndToken);
                    newVar.ArrayGacCode = GetTextFromTokens(Tokens, ArrayStartToken, ArrayEndToken);
                }
                c.Members[newVar.Name] = newVar;
            }

            return -1;
        }
        private int AnalizeClassMember(Module c, int poz)
        {
            ModifierType mtype = ModifierType.Private;
            bool over_ride = false;
            SerializableType seralizable = SerializableType.None;
            // [modificator de access] [overide] type nume (..) { ... }
            int startPoz = poz;
            if (Tokens[poz].Type == TokenType.AccessModifier)
            {
                mtype = gacModifType[Tokens[poz].Text];
                poz++;
                if (poz >= tokensCount)
                {
                    AddError(Tokens[poz - 1], String.Format("Improper usage of {0} access modifier !", mtype.ToString()));
                    return -1;
                }
            }
            switch (Tokens[poz].Type)
            {
                case TokenType.Override:
                    over_ride = true;
                    poz++;
                    if (poz >= tokensCount)
                    {
                        AddError(Tokens[poz - 1], String.Format("Improper usage of override modifier !"));
                        return -1;
                    }
                    break;
                case TokenType.Persistent:
                    seralizable = SerializableType.Persistent;
                    poz++;
                    if (poz >= tokensCount)
                    {
                        AddError(Tokens[poz - 1], String.Format("Improper usage of persistent modifier !"));
                        return -1;
                    }
                    break;
                case TokenType.Serializable:
                    seralizable = SerializableType.Serializable;
                    poz++;
                    if (poz >= tokensCount)
                    {
                        AddError(Tokens[poz - 1], String.Format("Improper usage of seralizable modifier !"));
                        return -1;
                    }
                    break;
            }
            // nu ar trebui sa urmeze inca un specificator
            switch (Tokens[poz].Type)
            {
                case TokenType.Override:
                case TokenType.Persistent:
                case TokenType.Serializable:
                    if ((over_ride) || (seralizable!= SerializableType.None))
                    {
                        AddError(Tokens[poz], String.Format("You can only use one of the following modifiers in a definition: 'override','serializable' or 'persistent'."));
                        return -1;
                    }
                    break;
            }
            if (SkipNamespaceToken(ref poz) == false)
                return -1;
            if ((Tokens[poz].Type != TokenType.TypeClass) && (Tokens[poz].Type != TokenType.TypeEnum))
            {
                AddError(Tokens[poz], String.Format("Invalid qualifer ({0}) - expecting a type or enum !", Tokens[poz].Text));
                return -1;
            }
            // tip nume ()
            // tip nume ;
            if (poz + 2 >= tokensCount)
            {
                AddError(Tokens[poz], String.Format("EOF reached in member definition ({0})", Tokens[poz].Text));
                return -1;
            }
            if (Tokens[poz + 1].Type != TokenType.Word)
            {
                AddError(Tokens[poz + 1], String.Format("Invalid qualifer ({0}) - expecting a name for class memeber !", Tokens[poz + 1].Text));
                return -1;
            }
            if (Tokens[poz + 2].Type == TokenType.RoundBreketOpen)
            {
                int tmp = Tokens[poz + 2].Link;
                if (tmp + 2 >= tokensCount)
                {
                    AddError(Tokens[poz + 2], "Incorect definition for member function (breket matches outside class scope) !");
                    return -1;
                }
                if (Tokens[tmp + 1].Type != TokenType.BreketOpen)
                {
                    AddError(Tokens[tmp + 1], "Invalid function definion - expecting '{'");
                    return -1;
                }
                if (seralizable!=SerializableType.None)
                {
                    AddError(Tokens[poz + 1], "A function can not be serializable !");
                }
                return AnalizeModuleFunction(c, mtype, over_ride, poz);
            }
            if (over_ride)
            {
                AddError(Tokens[poz + 1], "A variable can not be overwritten (remove the 'override' specifier) !");
            }
            int rezult = AnalizeModuleVariable(c, mtype, poz, seralizable);
            if (rezult < 0)
            {
                // am o eroare la analiza variabile - incerc sa fac skip peste linia curenta
                int startLine = Tokens[startPoz].Line;
                while ((startPoz < tokensCount) && (Tokens[startPoz].Line == startLine))
                    startPoz++;
                if (startPoz >= tokensCount)
                    return -1; // nu am peste ce sa trec
                return startPoz;
            }
            return rezult;
        }        
        private int AnalizeConstructor(Module c, int poz)
        {
            // daca sunt in clasa globala - ies
            if (c.Name == "Global")
            {
                AddError(Tokens[poz], "Global data can not have a constructor. Use Application.OnInit() to initialize global data");
                return -1;
            }
            // stiu ca la poz am un constructor
            if (Tokens[poz + 1].Type != TokenType.RoundBreketOpen)
            {
                AddError(Tokens[poz + 1], "Expecting '(' in constructor definition !");
                return -1;
            }
            int end_p = Tokens[poz + 1].Link;
            int b_start = -1, b_end = -1;
            int s_code = -1;
            int e_code = -1;

            if (Tokens[end_p + 1].Type == TokenType.TwoPoints)
            {
                if (Tokens[end_p + 2].Type != TokenType.Base)
                {
                    AddError(Tokens[end_p + 2], "Expecting 'base' after constructor definition !");
                    return -1;
                }
                if (Tokens[end_p + 3].Type != TokenType.RoundBreketOpen)
                {
                    AddError(Tokens[end_p + 3], "Expecting '(' after base !");
                    return -1;
                }
                if ((c.DerivedFrom == null) || (c.DerivedFrom.Length == 0))
                {
                    AddError(Tokens[end_p + 2], String.Format("Class '{0}' is not derived from another class. You can not use 'base' without derivation !",c.Name));
                    return -1;
                }

                b_start = end_p + 4;
                b_end = Tokens[end_p + 3].Link;
                s_code = b_end + 1;
            }
            else
            {
                s_code = end_p + 1;
            }
            if (Tokens[s_code].Type != TokenType.BreketOpen)
            {
                AddError(Tokens[s_code], "Expecting '{' after contructor definition !");
                return -1;
            }
            e_code = Tokens[s_code].Link;
            s_code++;
            // am un constructor valid
            Member ctor = new Member(c,MemberType.Constructor, "", "", "", "", Tokens[poz]);            
            if (b_start > 0)
                ctor.BaseCppCallCode = GetTextBetweenTokens_(b_start, b_end-1);
            ctor.StartCode = s_code;
            ctor.EndCode = e_code-1;
            // parametri
            poz = poz + 2;
            while (poz < end_p)
            {
                AnalizeFunctionParameter(c, ctor, poz);
                poz = FindNextInList(poz, end_p) + 1; // ca sa fac skip peste virgula
            }
            Member ctormember = c.GetMember("");
            if (ctormember == null)
            {
                ctor.Overrides = new List<Member>();
                c.Members[""] = ctor;
                return e_code + 1;
            }
            ctormember.Overrides.Add(ctor);
            return e_code + 1;

        }
        private int AnalizeDestructor(Module c, int poz)
        {
            // daca sunt in clasa globala - ies
            if (c.Name == "Global")
            {
                AddError(Tokens[poz], "Global data can not have a destructor. Use Application.OnTerminate() to initialize global data");
                return -1;
            }
            // stiu ca la poz am un constructor
            if (Tokens[poz + 1].Type != TokenType.RoundBreketOpen)
            {
                AddError(Tokens[poz + 1], "Expecting '(' in constructor definition !");
                return -1;
            }
            int end_p = Tokens[poz + 1].Link;
            if (end_p!=poz+2)
            {
                AddError(Tokens[poz + 1], "Destructors can not have any parameters !");
                return -1;
            }
            int s_code = end_p + 1;

            if (Tokens[s_code].Type != TokenType.BreketOpen)
            {
                AddError(Tokens[s_code], "Expecting '{' after destructor definition !");
                return -1;
            }
            int e_code = Tokens[s_code].Link;
            s_code++;
            // am un constructor valid
            Member dtor = new Member(c, MemberType.Destructor, "", "", "", "",Tokens[poz]);
            dtor.StartCode = s_code;
            dtor.EndCode = e_code - 1;
            // parametri
            poz = poz + 2;
            while (poz < end_p)
            {
                AnalizeFunctionParameter(c, dtor, poz);
                poz = FindNextInList(poz, end_p) + 1; // ca sa fac skip peste virgula
            }
            Member dtormember = c.GetMember("~");
            if (dtormember != null)
            {
                AddError(Tokens[poz], "Class '"+c.Name+"' can only have one destructor !");
                return -1;
            }
            c.Members["~"] = dtor;
            return e_code + 1;

        }

        public bool AnalizeClassMembers(Module m)
        {
            int start = m.StartToken;
            Error = "";
            while ((start <= m.EndToken) && (start >= 0))
            {
                if (Tokens[start].Type == TokenType.SemiColumn)
                {
                    start++;
                    continue;
                }
                if (Tokens[start].Type == TokenType.Constructor)
                {
                    start = AnalizeConstructor(m, start);
                } else
                if (Tokens[start].Type == TokenType.Destructor)
                {
                    start = AnalizeDestructor(m, start);
                }
                else
                {
                    start = AnalizeClassMember(m, start);
                }
            }
            if (m.Name == "Global")
            {
                // adaug si traducerile
                foreach (Member mb in m.Members.Values)
                {
                    mb.Cpp = "GD." + mb.Name;
                }
            }
            return (Error.Length == 0);
        }
        
        //private bool AnalizePreprocessorCommand_old(GACFile gFile,int poz)
        //{
        //    string s = Tokens[poz].Text.Replace("\t", " ").Trim();
        //    foreach (string key in preprocessCommands.Keys)
        //    {
        //        if ((preprocessCommands[key].Length==0) && (s.StartsWith(key + " ")))
        //        {
        //            gFile.PreprocessCommands.Add(s);
        //            return true;
        //        }
        //    }
        //    // altfel avem ceva de forma #command(p1,p2,p3,...pn)
        //    if ((s.EndsWith(")") == false) || (s.Contains("(")==false))
        //    {
        //        AddError(Tokens[poz], "Incorrect usage of preprocessor commands (should be #preprocessorCommand(param1,param2,...paramX) )");
        //        return false;
        //    }
        //    int count = 0;
        //    PreprocessWords[0] = "";
        //    bool inString = false;
        //    foreach (char ch in s)
        //    {
        //        if (((ch == '(') || (ch == ')') || (ch == ',')) && (!inString))
        //        {
        //            if (PreprocessWords[count].Length==0)
        //            {
        //                AddError(Tokens[poz], "Empty parameter for preprocessor definition !");
        //                return false;
        //            }
        //            count++;
        //            if (count>=PreprocessWords.Length)
        //            {
        //                AddError(Tokens[poz], "Too many parameters for preprocessor definition !");
        //                return false;
        //            }
        //            PreprocessWords[count] = "";
        //            continue;
        //        }
        //        if (ch == '"')
        //            inString = !inString;
        //        if ((ch==' ') && (inString==false))
        //            continue;
        //        PreprocessWords[count] += ch;
        //    }
        //    if (PreprocessWords[count].Length > 0)
        //    {
        //        AddError(Tokens[poz], "Incorect format for preprocessor word (not finished - '" + PreprocessWords[count]+"' )");
        //        return false;
        //    }
        //    // totul e ok - analizez 
        //    if (PreprocessWords[0] == "#addConstant") // #addConstant(enum/clasa,nume,valoare)
        //    {
        //        if (count != 4)
        //        {
        //            AddError(Tokens[poz], "Incorect number of parameters for #addConstant -> expecting #addConstant(enum/class,name,value)");
        //            return false;
        //        }
        //        gFile.PreprocessCommands.Add("#define GAC_ADDED_CONSTANT_"+PreprocessWords[1]+"_"+PreprocessWords[2]+" "+PreprocessWords[3]);
        //        return true;
        //    }


        //    AddError(Tokens[poz], "Unknown preprocessor definition !");
        //    return false;
        //}

        public bool PreprocessDefinitions()
        {
            int start = 0;
            GACFile gf = Compiler.GetGACFile(FileName);
            gf.FileType = GACFileType.Class;
            gf.Scenes.Clear();
            //gf.Clear(); sterge definitiile de la enum si addConstant
            while ((start < tokensCount) && (start>=0))
            {
                switch (Tokens[start].Type)
                {
                    case TokenType.Class:
                    case TokenType.Scene:
                        start = AnalizeClassDefinition(gf, start);
                        break;
                    case TokenType.Application:
                        start = AnalizeApplicationDefinition(gf, start);
                        break;
                    case TokenType.Enum:
                        start = AnalizeEnumDefinition(gf, start);
                        break;
                    case TokenType.Global:
                        start = SkipGlobalDefinition(start);
                        gf.FileType = GACFileType.Global;
                        break;
                    //case TokenType.Preprocesor:
                    //    AnalizePreprocessorCommand(gf, start);
                    //    start += 1;
                    //    break;
                    default:
                        AddError(Tokens[start], String.Format("Invalid qualifier ({0}) - outside any enum or class definition !", Tokens[start].Text));
                        return false;
                }
            }
            return (Error.Length == 0);
        }


        private static bool CreateOrderAndDerive(Module m)
        {
            if (m.DerivedFrom.Length == 0)
            {
                m.Order = 0;
                return true;
            }
            Module d = Compiler.GetModule(m.DerivedFrom);
            if (d == null)
            {
                AddError(m.FileName, m.ObjectToken, String.Format("Unknown class used for derivation {0}", m.DerivedFrom));
                return false;
            }
            if (d.Type != ModuleType.Class)
            {
                AddError(m.FileName, m.ObjectToken, String.Format("{0} is not a class. {1} cannot be derived from it !", m.DerivedFrom, m.Name));
                return false;
            }
            if (d.BasicType == true)
            {
                AddError(m.FileName, m.ObjectToken, String.Format("{0} is a basic type. {1} cannot be derived from it !", m.DerivedFrom, m.Name));
                return false;
            }
            if (d.Order == -2)
            {
                AddError(d.FileName, d.ObjectToken, String.Format("{0} is used in a circular derivation !", d.Name));
                return false;
            }
            // derivarea este in regula
            if (d.Order < 0)
            {
                m.Order = -2;
                if (CreateOrderAndDerive(d) == false)
                    return false;
            }
            // clasa de baza este si ea rezolvata - stabilim ordinea
            m.Order = d.Order + 1;
            // copii functiile din clasa de baza in clasa mea
            foreach (Member mb in d.Members.Values)
            {
                if ((mb.Type== MemberType.Variable) || (mb.Type== MemberType.Function))
                {
                    if (m.Members.ContainsKey(mb.Name))
                    {
                        AddError(m.FileName,m.ObjectToken,String.Format("Member {0} from class {1} is already defined in derived class {2}",mb.Name,m.Name,d.Name));
                        return false;
                    }
                    m.Members[mb.Name] = mb;
                }                
            }
            return true;
        }
        public static bool ComputeClassOrder()
        {
            Error = "";
            bool res = true;
            Compiler.LocalClasses.Clear();
            foreach (GACFile gf in Compiler.Files.Values)
            {
                foreach (Module m in gf.Modules.Values)
                {
                    if (m.Type != ModuleType.Class)
                        continue;
                    if (m.FileName.Length > 0)
                        Compiler.LocalClasses.Add(m);
                    if (m.Order < 0)
                    {
                        res &= CreateOrderAndDerive(m);
                    }
                }
            }
            Compiler.LocalClasses.Add(Compiler.GetModule("Global"));
            Compiler.LocalClasses.Sort(delegate(Module m1, Module m2) 
            {
                if ((m1 == null) && (m2 == null))
                    return 0;
                if ((m1 == null) && (m2 != null))
                    return -1;
                if ((m1 != null) && (m2 == null))
                    return 1;
                return m1.Order.CompareTo(m2.Order); 
            });
            return res;
        }


        private bool CheckToken(int id)
        {
            if (id >= tokensCount)
            {
                AddError(Tokens[tokensCount - 1], "Premature end of code !");
                return false;
            }
            return true;
        }
        private bool IsDeclared(Token t)
        {
            Module m = Compiler.GetModule(t.Text);
            if (m != null)
            {
                AddError(t, String.Format("{0} was already defined !", t.Text));
                return true;
            }
            return false;
        }
        private void AnalizeList(int start, int end, Module cls, Member currentFunction)
        {
            // analizeaza o lista separata prin virgule
            AnalizeCode(start, end, cls, currentFunction, 0);
        }
        private int AnalizeStackLocalVariableDefinition(int start, Module cls, Member currentFunction)
        {
            // local tip(params) nume [];
            // la start am cuvantul cheie local            
            int end_d = FindFirstSemiColumn(start, tokensCount);
            if (end_d < 0)
            {
                AddError(Tokens[start + 1], "Missing ';' at the end of the local variable definition");
                return start + 1;
            }           
            if (Tokens[start + 1].Type != TokenType.TypeClass)
            {
                AddError(Tokens[start + 1], "Expecting a type after 'local' keyword !");
                return start + 1;
            }
            if (Tokens[start + 1].Obj.BasicType == true)
            {
                AddError(Tokens[start + 1], "A basic type should not be used with 'local' keyword ! Remove the 'local' keyword !!!");
                return start + 1;
            }
            // verific daca tipul nu are parametri
            int namePoz = start + 2;
            if (Tokens[start+2].Type == TokenType.RoundBreketOpen)
            {
                namePoz = Tokens[start + 2].Link + 1;
            }
            if (Tokens[namePoz].Type != TokenType.Word)
            {
                AddError(Tokens[namePoz], "Expecting a variable name !");
                return namePoz;
            }
            int next = AnalizeArray(cls, currentFunction, namePoz+1);
            if (next != namePoz+1)
            {
                AddError(Tokens[namePoz+1], "Array are not permited in a local variable definition !");
                return next;
            }
            if (next != end_d)
            {
                AddError(Tokens[next], "Invalid word in a local variable definition (expecting ';') !");
                return next;
            }
            Tokens[start].Translate = ""; // 'local' nu apare in definitia din CPP
            Tokens[start + 1].Translate = Tokens[start + 1].Obj.Cpp + " ";
            Tokens[namePoz].CopyContentToTranslate("__local__", " ");
            for (int tr = start + 2; tr < namePoz; tr++)
            {
                Tokens[tr].Translate = "";
            }


            // verific daca am variabila
            if (cls.GetMember(Tokens[namePoz].Text) != null)
            {
                AddError(Tokens[namePoz], String.Format("Local variable {0} was already defined in class {1} or one of its base classes", Tokens[namePoz].Text, cls.Name));
                return end_d;
            }
            if (currentFunction.GetParameter(Tokens[namePoz].Text) != null)
            {
                AddError(Tokens[namePoz], String.Format("Local variable {0} was already defined as a parameter in function {1}", Tokens[namePoz].Text, currentFunction.Name));
                return end_d;
            }
            if (currentFunction.LocalVariables.ContainsKey(Tokens[namePoz].Text))
            {
                AddError(Tokens[namePoz], String.Format("Local variable {0} was previously declared as a local variable in functione {1}", Tokens[namePoz].Text, currentFunction.Name));
                return end_d;
            }
            Member mb = new Member(cls, MemberType.Variable, Tokens[namePoz].Text, "", Tokens[start + 1].Text, "", Tokens[namePoz]);
            /*
            if (ArrayElementsCount > 0)
            {
                mb.ArrayCount = ArrayElementsCount;
                mb.ArrayCppCode = CreateCppCode(ArrayStartToken, ArrayEndToken);
            }
             */
            currentFunction.LocalVariables[mb.Name] = mb;
            string tmp = ";"+Compiler.GetCPPName(Tokens[start+1].Obj.Name, false)+" "+Tokens[namePoz].Text;
            /*
            if (ArrayElementsCount > 0)
                tmp+=" "+mb.ArrayCppCode+" ";
             */
            tmp += " = &__local__"+Tokens[namePoz].Text+";";
            if (start+2!=namePoz)
            {
                // are parametri specifici pentru tip de date
                switch (Tokens[start+1].Text)
                {
                    case "String":
                        if (namePoz!=start+5)
                        {
                            AddError(Tokens[start + 2], String.Format("Type {0} only supports one parameter for type initialization !", Tokens[start + 1].Text));
                            return end_d;
                        }
                        if (Tokens[start+3].Type != TokenType.Number)
                        {
                            AddError(Tokens[start + 3], String.Format("Type {0} requires a numeric value to be used for type initalization !", Tokens[start + 1].Text));
                            return end_d;
                        }
                        int size = 0;
                        if ((int.TryParse(Tokens[start+3].Text,out size)==false) || (size<8))
                        {
                            AddError(Tokens[start + 3], String.Format("Type {0} requires an unsigned integer value bigger than 8 to be used for type initalization !", Tokens[start + 1].Text));
                            return end_d;
                        }
                        // totul e ok - modificam tmp
                        tmp += ";char __local__string_buffer_" + Tokens[namePoz].Text+"["+Tokens[start+3].Text+"]";
                        tmp += ";" + Tokens[namePoz].Text + "->Create(__local__string_buffer_" + Tokens[namePoz].Text + "," + Tokens[start + 3].Text + ",true);";
                        break;
                    default:
                        AddError(Tokens[start + 2], String.Format("Type {0} does not support type initialization !", Tokens[start + 1].Text));
                        return end_d;
                }
            }

            Tokens[end_d - 1].Translate += tmp;
            return end_d;
        }
        private int AnalizeLocalVariableDefinition(int start, Module cls,Member currentFunction)
        {
            // tip nume [] = [...],
            // la start am tipul clasei
            int localVariableType = start;
            int end_d = FindFirstSemiColumn(start, tokensCount);
            int virgula = 0;

            if (end_d < 0)
            {
                AddError(Tokens[start + 1], "Missing ';' at the end of the local variable definition");
                return start+1;
            }
            int next = start + 1;
            Tokens[start].Translate = Compiler.GetCPPName(Tokens[start].Obj.Name, false) + " ";
            while (next < end_d)
            {
                start = next;
                if (Tokens[next].Type != TokenType.Word)
                {
                    AddError(Tokens[next], "Expecting a word for local variable definition");
                    return end_d;
                }
                Tokens[next].CopyContentToTranslate("", " ");
                next = AnalizeArray(cls, currentFunction, next+1);
                // verific daca am variabila
                if (cls.GetMember(Tokens[start].Text) != null)
                {
                    AddError(Tokens[start], String.Format("Local variable {0} was already defined in class {1} or one of its base classes", Tokens[start].Text, cls.Name));
                    return end_d;
                }
                if (currentFunction.GetParameter(Tokens[start].Text) != null)
                {
                    AddError(Tokens[start], String.Format("Local variable {0} was already defined as a parameter in function {1}", Tokens[start].Text, currentFunction.Name));
                    return end_d;
                }
                if (currentFunction.LocalVariables.ContainsKey(Tokens[start].Text))
                {
                    AddError(Tokens[start], String.Format("Local variable {0} was previously declared as a local variable in functione {1}", Tokens[start].Text, currentFunction.Name));
                    return end_d;
                }
                Member mb = new Member(cls, MemberType.Variable, Tokens[start].Text, "", Tokens[localVariableType].Text, "", Tokens[start]);
                if (ArrayElementsCount > 0)
                {
                    mb.ArrayCount = ArrayElementsCount;
                    mb.ArrayCppCode = CreateCppCode(ArrayStartToken, ArrayEndToken);
                    mb.ArrayGacCode = GetTextFromTokens(Tokens, ArrayStartToken, ArrayEndToken);
                }
                currentFunction.LocalVariables[mb.Name] = mb;
                virgula = FindNextInList(next, end_d);
                if ((next < virgula) && (Tokens[next].Type != TokenType.Equal))
                {
                    AddError(Tokens[next], "Expecting '=' or ',' or ';' in local variable definition");
                    return end_d;
                }
                if (next + 1 < virgula)
                {
                    // am cod de initializare
                    AnalizeCode(next, virgula - 1, cls, currentFunction, 0);
                }
                Tokens[virgula].CopyContentToTranslate(" ", " ");
                next = virgula + 1;
            }
            return end_d;
        }
        private int AnalizeCodeAfterPoint(int start, Module cls,Member currentFunction,Module tip)
        {
            // start puncteaza exact la un "."
            if (tip.BasicType)
                Tokens[start].Translate = ".";
            else
                Tokens[start].Translate = "->";
            if (Tokens[start + 1].Type != TokenType.Word)
            {
                AddError(Tokens[start + 1], String.Format("Expecting a member of {0} ", tip.Name));
                return start + 2;
            }
            Member mb = tip.GetMember(Tokens[start + 1].Text);
            if (mb == null) 
                mb = Compiler.GetMemberFromBaseClasses(tip, Tokens[start + 1].Text);

            if (mb == null)
            {
                AddError(Tokens[start + 1], String.Format("{0} is not a member of {1} ", Tokens[start + 1].Text, tip.Name));
                return start + 2;
            }
            if (mb.Static == true)
            {
                AddError(Tokens[start + 1], String.Format("{0} is a static member of {1} and can not be used in an object instance", Tokens[start + 1].Text, tip.Name));
                return start + 2;
            }
            if (mb.Type == MemberType.Function)
                return AnalizeFunctionCall(start + 1, cls,currentFunction, mb);
            else
                return AnalizeVariableUsage(start + 1, cls,currentFunction, mb);
        }
        private int AnalizeVariableUsage(int start, Module cls,Member currentFunction,Member m)
        {
            int next = start+1;
            Module tip = Compiler.GetModule(m.DataType);
            if (tip == null)
            {
                AddError(Tokens[start],String.Format("Unknwon type {0} - Internal error !",m.DataType));
                return next;
            }
            // daca am un array - trec peste
            next = AnalizeArray(cls, currentFunction, next);
            // verific daca apelez cum trebuie array-ul
            if (ArrayElementsCount > 0)
            {
                // am un array - compar cu ce are variabila mea
                if (m.ArrayCount != ArrayElementsCount)
                {
                    if (m.ArrayCount <= 0)
                        AddError(Tokens[start], String.Format("Variable {0} was not declared as an array. It can not be used as one.", m.Name));
                    else
                        AddError(Tokens[start], String.Format("Variable {0} was declared as an array of {1} dimensions. It is used as it would have {2} dimensions.", m.Name, m.ArrayCount, ArrayElementsCount));
                    return next;
                }
            }

            if (CheckToken(next) == false)
                return next - 1;
            
            if (Tokens[next].Type == TokenType.Point)
            {
                if (m.Cpp.Length > 0)
                    Tokens[start].Translate = m.Cpp;
                else
                    Tokens[start].CopyContentToTranslate("", ""); // urmeaza un punct - fara spatiu
                return AnalizeCodeAfterPoint(next, cls, currentFunction, tip);
            }
            else
            {
                if (m.Cpp.Length > 0)
                    Tokens[start].Translate = m.Cpp + " ";
                else
                    Tokens[start].CopyContentToTranslate("", " "); // fara punct - adaugam spatiu
            }
            return next;
        }
        private int AnalizeFunctionCall(int start, Module cls,Member currentFunction,Member f)
        {
            // start puncteaza la apelul efectiv x.f(5) - start va puncta la f
            // trebuie sa am ()
            //if (f.Name=="SetLocation")
            //    f.Name.Trim();

            if (f.Cpp.Length > 0)
                Tokens[start].Translate = f.Cpp;
            else
                Tokens[start].CopyContentToTranslate("", "");
            if (CheckToken(start + 2) == false)
                return -1;
            if (Tokens[start + 1].Type != TokenType.RoundBreketOpen)
            {
                AddError(Tokens[start + 1], String.Format("Expecting '(' for the function call {0} ", f.Name));
                return start + 1;
            }
            AnalizeList(start + 2, Tokens[start + 1].Link - 1, cls, currentFunction);
            Tokens[start + 1].Translate = "(";
            Tokens[Tokens[start + 1].Link].Translate = ")";
            start = Tokens[start + 1].Link+1;
            if (CheckToken(start) == false)
                return -1;
            if (Tokens[start].Type == TokenType.Point)
                return AnalizeCodeAfterPoint(start, cls, currentFunction, Compiler.GetModule(f.DataType));
            // daca valoarea de return e un tip de date, si este precedat de . atunci ar trebui sa apelam AliazeVariableUsage
            return start;
        }
        
        private int AnalizeEnumCall(int start, Module m)
        {
            // implicit la Tokens[start] este enum-ul
            if (Tokens[start + 1].Type != TokenType.Point)
            {
                if (m.Cpp.Length > 0)
                    Tokens[start].Translate = m.Cpp;
                else
                    Tokens[start].Translate = m.Name;
                return start + 1;
            }
            // am un punct
            if (start + 2 >= tokensCount)
            {
                AddError(Tokens[start + 1], String.Format("Incorect use of Enum {0} (expecting a constnat after a '.') ", m.Name));
                return start + 1;
            }
            Member mb = m.GetMember(Tokens[start + 2].Text);
            string cppTranslation = "";
            if (mb==null)
            {
                // verific daca nu este in extra valori
                cppTranslation = GACParser.GetExtraConstant(m.Name, Tokens[start + 2].Text);
                if (cppTranslation == null)
                {
                    AddError(Tokens[start + 2], String.Format("Enum {0} does not have a member {1}", m.Name, Tokens[start + 2].Text));
                    return start + 3;
                } 
            }
            else
            {
                cppTranslation = mb.Cpp;
            }

            if (cppTranslation.Length > 0)
            {
                Tokens[start + 2].Translate = cppTranslation;
                return start + 3;
            }
            AddError(Tokens[start + 2], String.Format("Constant {0} from Enum {1} does not have a Cpp translation !", mb.Name, m.Name));
            return start + 3;
        }
        private int AnalizeStaticClassCall(int start, Module cls,Member currentFunction,Module sc)
        {
            // implicit la Tokens[start] este static class-ul
            if (CheckToken(start + 2) == false)
                return -1;
            if (Tokens[start + 1].Type != TokenType.Point)
            {
                AddError(Tokens[start + 1], String.Format("Incorect use of static class {0} (expecting a '.') after {0} ", sc.Name));
                return start+1;
            }            
            start+=2;
            Member mb = sc.GetMember(Tokens[start].Text);
            if (mb==null)
            {
                AddError(Tokens[start], String.Format("Static class {0} does not have a member {1}", sc.Name, Tokens[start].Text));
                return start+1;
            }
            if (mb.Type == MemberType.Constant)
            {
                if (mb.Cpp.Length > 0)
                    Tokens[start].Translate = mb.Cpp + " ";
                else
                    AddError(Tokens[start], String.Format("Constant {0} from static class {1} does not have a Cpp translation !", mb.Name, sc.Name));
                return start+1;
            }
            if (mb.Type == MemberType.Function)
            {
                if (mb.Cpp.Length > 0)
                {
                    Tokens[start].Translate = mb.Cpp + " ";
                    return AnalizeFunctionCall(start, cls,currentFunction, mb);
                }
                else
                    AddError(Tokens[start], String.Format("Function {0} from static class {1} does not have a Cpp translation !", mb.Name, sc.Name));
                return start+1;
            }
            if (mb.Type == MemberType.Variable)
            {
                Module varType = Compiler.GetModule(mb.DataType);
                if (varType == null)
                {
                    AddError(Tokens[start], String.Format("Unknown type ({0}) for variable {1}.{2}", mb.DataType, sc.Name, mb.Name));
                    return start+1;
                }
                if (mb.Cpp.Length > 0)
                {
                    Tokens[start].Translate = mb.Cpp;
                    return AnalizeVariableUsage(start, cls,currentFunction, mb);// trebuie revizuit pentru Resurse
                }
                else
                {
                    AddError(Tokens[start], String.Format("Variable {0} from static class {1} does not have a Cpp translation !", mb.Name, sc.Name));
                }
                return start+1;
            }
            AddError(Tokens[start], String.Format("Unknwon member {0} from static class {1}. It should be a function, constant or variable !", mb.Name, sc.Name));
            return start+1;
        }
        private int AnalizeClassCall(int start, Module cls, Module c,Member currentFunction)
        {
            if (CheckToken(start + 1) == false)
                return -1;
            if (Tokens[start + 1].Type == TokenType.Point)
            {
                // am cazul Class.<constanta> sau <functie> statica
                if (CheckToken(start + 2) == false)
                    return -1;
                Member mb = Tokens[start].Obj.GetMember(Tokens[start + 2].Text);
                if (mb == null)
                {
                    AddError(Tokens[start+2], String.Format("Class {0} does not have a static member {1}", Tokens[start].Text, Tokens[start + 2].Text));
                    return start + 3;
                }
                if ((mb.Type != MemberType.Constant) && (mb.Type != MemberType.Function))
                {
                    AddError(Tokens[start+2], String.Format("Expecting a constant or a static function as a member of Class {0}", Tokens[start].Text));
                    return start + 3;
                }
                if (mb.Static == false)
                {
                    AddError(Tokens[start+2], String.Format("{1} from class {0} is not defined as static", Tokens[start].Text, Tokens[start + 2].Text));
                    return start + 3;
                }
                if (mb.Type == MemberType.Constant)
                {
                    if (mb.Cpp.Length == 0)
                    {
                        AddError(Tokens[start + 2], String.Format("Constant {1} from class {0} does not have a CPP translation !", Tokens[start].Text, Tokens[start + 2].Text));
                        return start + 3;
                    }
                    Tokens[start + 2].Translate = mb.Cpp;
                    return start + 3;
                }
                // altfel e o functie
                return AnalizeFunctionCall(start + 2, cls,currentFunction, mb);
            }
            // daca am new class
            if ((start > 0) && (Tokens[start - 1].Type == TokenType.New))
            {
                
                if (Tokens[start+1].Type == TokenType.SquareBreketOpen)
                {
                    Module m = Compiler.GetModule(Tokens[start].Text);                    
                    Tokens[start].Translate = Tokens[start].Obj.GetGACCPPType(!m.BasicType);
                    Tokens[start + 1].CopyContentToTranslate(" ", "");
                    AnalizeList(start + 2, Tokens[start + 1].Link - 1, cls, currentFunction);
                    Tokens[Tokens[start + 1].Link].CopyContentToTranslate("", " ");
                    return Tokens[start + 1].Link + 1;
                }
                Tokens[start].Translate = Tokens[start].Obj.GetGACCPPType(false);
                return start + 1;
            }
            // daca am un cast
            if ((start > 0) && (Tokens[start - 1].Type == TokenType.RoundBreketOpen) && (Tokens[start - 1].Link == start + 1))
            {
                Tokens[start - 1].Translate = "(";
                Tokens[start + 1].Translate = ")";
                Tokens[start].Translate = Compiler.GetCPPName(Tokens[start].Obj.Name, false);
                return start + 2;
            }
            // altfel am o definitie de variabila    
            return AnalizeLocalVariableDefinition(start, cls, currentFunction);            
        }
        private int AnalizeWordCall(int start, Module cls, Member currentFunction)
        {
            Member m = cls.GetMember(Tokens[start].Text);
            if ((m == null) && (currentFunction!=null))
                m = currentFunction.GetLocalVariableOrParameter(Tokens[start].Text);
            if (m == null)
            {
                AddError(Tokens[start], String.Format("Undeclared identifier {0}", Tokens[start].Text));
                return start + 1;
            }
            if (m.Type == MemberType.Function)
                return AnalizeFunctionCall(start, cls,currentFunction, m);
            else
                return AnalizeVariableUsage(start, cls, currentFunction, m);
        }

        private int AnalizeThis(int start, Module cls, Member currentFunction)
        {
            // start e la "this"
            if (Tokens[start + 1].Type == TokenType.Point)
            {
                Tokens[start].CopyContentToTranslate("", "");
                return AnalizeCodeAfterPoint(start + 1, cls, currentFunction, cls);
            }
            Tokens[start].CopyContentToTranslate("", " ");
            return start + 1;
        }
        private int AnalizeBaseKeyword(int start,Module cls, Member currentFunction)
        {
            if (CheckToken(start + 2) == false)
                return -1;
            if (Tokens[start + 1].Type != TokenType.Point)
            {
                AddError(Tokens[start + 1], String.Format("Incorect use of base keyword (expecting a '.') after base "));
                return start+1;
            }
            if (Tokens[start + 2].Type != TokenType.Word)
            {
                AddError(Tokens[start + 2], String.Format("Incorect use of base keyword (expecting a function name) !"));
                return start + 2;
            }
            Module baseClass = Compiler.GetModule(cls.DerivedFrom);
            if (baseClass == null)
            {
                AddError(Tokens[start], String.Format("Current class is derived from an unexisting module: "+cls.DerivedFrom));
                return start + 2;
            }
            Member mb = baseClass.GetMember(Tokens[start + 2].Text);
            if (mb==null)
            {
                AddError(Tokens[start + 2], String.Format("Function {0} is not a member of base class {1}", Tokens[start + 2].Text, cls.DerivedFrom));
                return start + 2;
            }
            // altfel - totul e ok , trebuie sa fac traducerile
            Tokens[start].Translate = cls.DerivedFrom + "::" + Tokens[start + 2].Text;
            Tokens[start + 1].Translate = "";
            Tokens[start + 2].Translate = "";
            return start + 3;
            //AddError(Tokens[start], "Base used in " + cls.Name + " derived from " + cls.DerivedFrom+" of type "+Tokens[start+2].Type.ToString());
            //return -1;
        }
        private int AnalizeNewKeyword(int start, Module cls, Member currentFunction)
        {
            if (CheckToken(start + 2) == false)
                return -1;
            if (Tokens[start + 1].Type != TokenType.TypeClass)
            {
                AddError(Tokens[start + 1], String.Format("'new' keyword should be followed by a type !"));
                return start + 1;
            }
            if (!EnableMemoryHook)
            {
                Tokens[start].Translate = " new ";
                return start + 1;
            }
            bool isArray = (Tokens[start + 2].Type == TokenType.SquareBreketOpen);
            string currentLine = "";
            int ln = Tokens[start].Line;
            int lStart = start;
            int lEnd = start;
            // start
            while ((lStart >= 0) && (Tokens[lStart].Line == ln) && (Tokens[lStart].Type != TokenType.SemiColumn))
                lStart--;
            lStart++;
            // end
            while ((lEnd < tokensCount) && (Tokens[lEnd].Line == ln) && (Tokens[lEnd].Type != TokenType.SemiColumn))
                lEnd++;
            // construiesc linie
            while (lStart<lEnd)
            {
                if (Tokens[lStart].Type == TokenType.String)
                    currentLine += Tokens[lStart].Text.Replace("\"", "\\\"");
                else
                    currentLine += Tokens[lStart].Text;
                switch (Tokens[lStart].Type)
                {
                    case TokenType.SquareBreketClose:
                    case TokenType.SquareBreketOpen:
                    case TokenType.TwoPoints:
                    case TokenType.BreketClose:
                    case TokenType.BreketOpen:
                    case TokenType.Column:
                    case TokenType.Point:
                    case TokenType.SemiColumn:
                        break;
                    default:
                        currentLine += " ";
                        break;
                }
                lStart++;
            }
            // numele tipului
            Module m = Compiler.GetModule(Tokens[start + 1].Text);
            if (m==null)
            {
                AddError(Tokens[start + 1], String.Format("Unknown module '{0}' !!!",Tokens[start+1].Text));
                return start + 1;
            }
            //if (m.Cpp.Length==0)
            //{
            //    AddError(Tokens[start + 1], String.Format("Translation for '{0}' is missing !!! This is an internal error !!!", Tokens[start + 1].Text));
            //    return start + 1;
            //}
            string nType = m.Cpp;
            if (nType.Length == 0)
                nType = "GACTYPE_"+Tokens[start + 1].Text;
            if ((isArray) && (m.BasicType == false))
                nType = "PTR_" + nType;
            string cfname = currentFunction.Name;
            if (cfname.Trim().Length==0)
                cfname = "(constructor)";
            if (cfname.StartsWith("~"))
                cfname = "(destructor)";
            if (isArray)
            {
                Tokens[start].Translate = String.Format(" new(\"{0}[]\",\"{1}::{2}\",{3},\"{4}\",sizeof({5})) ", Tokens[start + 1].Text, cls.Name, cfname, ln, currentLine, nType);
            } else {
                Tokens[start].Translate = String.Format(" new(\"{0}\",\"{1}::{2}\",{3},\"{4}\",sizeof({5})) ", Tokens[start + 1].Text, cls.Name, cfname, ln, currentLine, nType);
            }
            return start + 1;
        }
        private int AnalizeDeleteKeyword(int start, Module cls, Member currentFunction)
        {
            if (CheckToken(start + 2) == false)
                return -1;
            //if (Tokens[start+1].Type != TokenType.RoundBreketOpen)
            //{
            //    AddError(Tokens[start + 1], String.Format("'delete' keyword should be preceded by '('."));
            //    return start + 1;
            //}
            if (!EnableMemoryHook)
            {
                Tokens[start].Translate = " delete ";
                return start + 1;
            }        
            string currentLine = "";
            int ln = Tokens[start].Line;
            int lStart = start;
            int lEnd = start;
            // start
            while ((lStart >= 0) && (Tokens[lStart].Line == ln) && (Tokens[lStart].Type != TokenType.SemiColumn))
                lStart--;
            lStart++;
            // end
            while ((lEnd < tokensCount) && (Tokens[lEnd].Line == ln) && (Tokens[lEnd].Type != TokenType.SemiColumn))
                lEnd++;
            // construiesc linie
            while (lStart < lEnd)
            {
                if (Tokens[lStart].Type == TokenType.String)
                    currentLine += Tokens[lStart].Text.Replace("\"", "\\\"");
                else
                    currentLine += Tokens[lStart].Text;
                switch (Tokens[lStart].Type)
                {
                    case TokenType.SquareBreketClose:
                    case TokenType.SquareBreketOpen:
                    case TokenType.TwoPoints:
                    case TokenType.BreketClose:
                    case TokenType.BreketOpen:
                    case TokenType.Column:
                    case TokenType.Point:
                    case TokenType.SemiColumn:
                        break;
                    default:
                        currentLine += " ";
                        break;
                }
                lStart++;
            }
            string cfname = currentFunction.Name;
            if (cfname.Trim().Length == 0)
                cfname = "(constructor)";
            if (cfname.StartsWith("~"))
                cfname = "(destructor)";
            //Tokens[start].Translate = 
            Tokens[start].Translate = String.Format("DEBUGMSG(\"DeAllocMetaData|{0}::{1}|{2}|{3}\"); delete ", cls.Name, cfname, ln, currentLine);
            //Tokens[start + 1].Translate = String.Format("(\"{0}::{1}\",{2},\"{3}\",", cls.Name, cfname, ln, currentLine);
            return start + 1;
        }
        private bool AnalizeCode(int start, int end, Module cls, Member currentFunction, int depth)
        {
            string value = "";
            //ss += Tabs(depth);
            int last = -1;

            while (start <= end)
            {
                if (start <= last)
                {
                    if (last >= 0)
                        AddError(Tokens[last], "Too many parsing errors - halting !");
                    return false;
                }
                last = start;
                value = Tokens[start].Text;
                switch (Tokens[start].Type)
                {
                    case TokenType.Word:
                        start = AnalizeWordCall(start, cls, currentFunction);
                        break;
                    case TokenType.TypeNamespace:
                        SkipNamespaceToken(ref start);
                        break;
                    case TokenType.TypeEnum:
                        start = AnalizeEnumCall(start, Tokens[start].Obj);                        
                        break;
                    case TokenType.Local:
                        start = AnalizeStackLocalVariableDefinition(start, cls, currentFunction);
                        break;
                    case TokenType.TypeClass:
                        start = AnalizeClassCall(start, cls, Tokens[start].Obj, currentFunction);                        
                        break;
                    case TokenType.TypeClassStatic:
                        start = AnalizeStaticClassCall(start,cls,currentFunction, Tokens[start].Obj);
                        break;
                    case TokenType.SemiColumn:
                        Tokens[start].Translate = ";\n" + Tabs(depth);
                        start++;
                        break;
                    case TokenType.BreketOpen:
                        Tokens[start].Translate = "\n" + Tabs(depth) + value;
                        depth++;
                        Tokens[start].Translate += "\n" + Tabs(depth);
                        start++;
                        break;
                    case TokenType.BreketClose:
                        depth--;
                        Tokens[start].Translate = "\n" + Tabs(depth) + value;
                        Tokens[start].Translate += "\n" + Tabs(depth);
                        start++;
                        break;
                    case TokenType.Operator:
                        Tokens[start].Translate = value;
                        if ((start + 1 <= end) && (Tokens[start + 1].Type == TokenType.Operator))
                            Tokens[start].Translate += " ";
                        start++;
                        break;
                    case TokenType.RoundBreketOpen:
                    case TokenType.RoundBreketClose:
                        if (Tokens[start].Translate.Length==0)
                            Tokens[start].Translate = value;
                        start++;
                        break;
                    case TokenType.Point:
                        AddError(Tokens[start], "Incorrect usage of '.' operator !");
                        start++;
                        break;
                    case TokenType.SquareBreketOpen:
                    case TokenType.SquareBreketClose:
                        AddError(Tokens[start], "Incorrect usage of array opertor !");
                        start++;
                        break;
                    case TokenType.This:
                        start = AnalizeThis(start, cls, currentFunction);
                        break;
                    case TokenType.Keyword:
                        Tokens[start].CopyContentToTranslate(""," ");
                        start++;
                        break;
                    case TokenType.Null:
                        Tokens[start].Translate = "(NULL)";
                        start++;
                        break;
                    case TokenType.New:
                        start = AnalizeNewKeyword(start, cls, currentFunction);  
                        break;
                    case TokenType.Delete:
                        start = AnalizeDeleteKeyword(start, cls, currentFunction);
                        break;
                    case TokenType.Class:
                    case TokenType.Scene:
                    case TokenType.Application:
                    case TokenType.Enum:
                    case TokenType.AccessModifier:
                    case TokenType.Constructor:
                    case TokenType.Destructor:
                    case TokenType.Override:
                    case TokenType.Reference:     
                    case TokenType.Serializable:
                    case TokenType.Persistent:
                        AddError(Tokens[start], "Invalid keyword to be used within a function code !");
                        break;
                    case TokenType.Base:
                        start = AnalizeBaseKeyword(start, cls, currentFunction);                        
                        break;
                    default:
                        Tokens[start].CopyContentToTranslate("", " ");
                        start++;
                        break;
                }
            }
            return (Error.Length == 0);
        }
        private void AnalizeModuleMembersCode(Module m)
        {
            foreach (Member mb in m.Members.Values)
            {
                if (((mb.Type == MemberType.Function) || (mb.Type == MemberType.Constructor) || (mb.Type == MemberType.Destructor)) && (mb.StartCode >= 0))
                {
                    mb.LocalVariables.Clear();
                    AnalizeCode(mb.StartCode, mb.EndCode, m, mb, 1);
                    if ((mb.Overrides != null) && (mb.Overrides.Count > 0))
                    {
                        foreach (Member overrideMB in mb.Overrides)
                        {
                            overrideMB.LocalVariables.Clear();
                            AnalizeCode(overrideMB.StartCode, overrideMB.EndCode, m, overrideMB, 1);
                        }
                    }
                }
            }
        }
        public bool AnalizeCode()
        {
            GACFile gf = Compiler.GetGACFile(FileName);

            foreach (Module m in gf.Modules.Values)
            {
                if (m.Type == ModuleType.Class)
                    AnalizeModuleMembersCode(m);
            }
            if (FileName.ToLower().Equals("global.gac"))
            {
                Module m = Compiler.GetModule("Global");
                foreach (Member mb in m.Members.Values)
                    mb.Cpp = mb.Name; 
                if (m != null)
                    AnalizeModuleMembersCode(m);
                foreach (Member mb in m.Members.Values)
                    mb.Cpp = "GD." + mb.Name;
            }
            return (Error.Length == 0);
        }
        #endregion

        #region H and CPP files
        private Module GetTopMostBaseClass(Module m)
        {
            if (m.DerivedFrom.Length == 0)
                return m;
            Module mBase = Compiler.GetModule(m.DerivedFrom);
            return GetTopMostBaseClass(mBase);
        }
        private string Tabs(int depth)
        {
            string s = "";
            while (depth > 0)
            {
                s += "\t";
                depth--;
            }
            return s;
        }
        private string CreateArrayCode(int start)
        {
            // start incepe de la un Array [
            int end = Tokens[start].Link;
            string s = "";
            start++;
            while (start < end)
            {
                int next = FindNextInList(start, end);
                //s += "(" + CreateCPPCode(start, next-1, 0) + ") ][ ";
                start = next + 1;
            }
            if (s.EndsWith(" ][ "))
                s = s.Substring(0, s.Length - 4);
            return "["+s+"]";
        }
        private static string CreateFunctionDefinition(Module classObject, Member functionMember,bool forHeader)
        {
            string s = Compiler.GetCPPName(functionMember.DataType, false);
            if (forHeader)
                s += " " + functionMember.Name + " (";
            else
                s += " " + classObject.Name + "::" +functionMember.Name + " (";

            List<Member> p = functionMember.GetParameters();
            if (p != null)
            {
                foreach (Member v in p)
                {
                    s += Compiler.GetCPPName(v.DataType, v.Reference);
                    if (v.ArrayCount > 0)
                    {
                        if (v.ArrayCppCode == "[]")
                            s += "* " + v.Name;
                        else
                            s += " " + v.Name+v.ArrayCppCode;
                    }
                    else
                    {
                        s += " " + v.Name;
                    }
                    s += ",";
                }
                if (s.EndsWith(","))
                    s = s.Substring(0, s.Length - 1);
            }
            s += ")";
            return s;                        
        }
        private static string CreateConstructorDefinition(Module classObject, Member constructorMember, bool forHeader)
        {
            string s = "";
            if (forHeader)
                s = classObject.Name + " (";
            else
                s = classObject.Name + "::" + classObject.Name + " (";
            List<Member> p = constructorMember.GetParameters();
            if (p != null)
            {
                foreach (Member v in p)
                {
                    s += Compiler.GetCPPName(v.DataType, v.Reference);
                    if (v.ArrayCount > 0)
                    {
                        if (v.ArrayCppCode == "[]")
                            s += "* " + v.Name;
                        else
                            s += " " + v.Name + v.ArrayCppCode;
                    }
                    else
                    {
                        s += " " + v.Name;
                    }
                    s += ",";
                }
                if (s.EndsWith(","))
                    s = s.Substring(0, s.Length - 1);
            }
            s += ")";
            if ((forHeader == false) && (constructorMember.BaseCppCallCode.Length>0))
            {
                s += " : ";
                Module derived = Compiler.GetModule(classObject.DerivedFrom);
                if (derived.Cpp.Length > 0)
                    s += derived.Cpp;
                else
                    s += derived.Name;
                s += " ("+constructorMember.BaseCppCallCode+") ";
            }
            return s;
        }
        private static string CreateDestructorDefinition(Module classObject, Member destructorMember, bool forHeader)
        {
            string s = "";
            if (forHeader)
                s = "virtual ~"+classObject.Name + " ()";
            else
                s = classObject.Name + "::~" + classObject.Name + " ()";
            return s;
        }
        private static string CreateVariableDefinition(Member v)
        {
            string s = Compiler.GetCPPName(v.DataType, false);
            if (v.ArrayCount > 0)
            {
                if (v.ArrayCppCode == "[]")
                    s += "* " + v.Name;
                else
                    s += " " + v.Name + v.ArrayCppCode;
            }
            else
            {
                s += " " + v.Name;
            }                    
            s += ";";
            return s; 
        }
        private string CreateCppCode(int tokenStart, int tokenEnd)
        {
            string ss = "";
            while (tokenStart <= tokenEnd)
            {
                ss += Tokens[tokenStart].Translate;
                if (Tokens[tokenStart].Translate.Length == 0)
                {
                    ss += " /*" + Tokens[tokenStart].Text + "*/ ";
                }
                tokenStart++;
            }
            return ss;
        }
        private static Dictionary<string, string> basicSerializableTypes = new Dictionary<string, string>()
        {
	        {"uint32","UInt32"},
	        {"int16","Int16"},
	        {"int32","Int32"},
	        {"int","Int32"},
	        {"Double","Double"},
	        {"float","Float"},
	        {"UInt8","UInt8"},
	        {"uint8","UInt8"},
	        {"qword","UInt64"},
	        {"uint64","UInt64"},
	        {"short","Int16"},
	        {"Int64","Int64"},
	        {"boolean","Bool"},
	        {"byte","UInt8"},
	        {"bool","Bool"},
	        {"int8","Int8"},
	        {"Float32","Float"},
	        {"Float64","double"},
	        {"UInt64","UInt64"},
	        {"Int32","Int32"},
	        {"char","Int8"},
	        {"Int16","Int16"},
	        {"word","UInt16"},
	        {"float64","Double"},
	        {"double","Double"},
	        {"Float","Float"},
	        {"uint","UInt32"},
	        {"long","Int64"},
	        {"Int8","Int8"},
	        {"uint16","UInt16"},
	        {"UInt16","UInt16"},
	        {"int64","Int64"},
	        {"dword","UInt32"},
	        {"UInt32","UInt32"},
	        {"float32","Float"},
        };
        public static Dictionary<string, string> StandardScenes = new Dictionary<string, string>()
        {
            {"Main","GAC_SCENES_MAIN"},
            {"SelectSeason","GAC_SCENES_SELECTSEASON"},		
            {"SelectLevel","GAC_SCENES_SELECTLEVEL"},
            {"Rate","GAC_SCENES_RATE"},	
            {"Final","GAC_SCENES_FINAL"},		
            {"Game","GAC_SCENES_GAME"},		
            {"Tutorial","GAC_SCENES_TUTORIAL"},			
            {"Credentials","GAC_SCENES_CREDENTIALS"},		
            {"WhatsNew","GAC_SCENES_WHATSNEW"},	
            {"Options","GAC_SCENES_OPTIONS"},	
            {"Comercial","GAC_SCENES_COMERCIAL"},		
            {"FirstTimeRun","GAC_SCENES_FIRSTTIMERUN"},		
            {"ApplicationError","GAC_SCENES_APP_ERROR"},
        };
        private string CreateSerializableBasicTypeCode(string prefix,Member mb,string dataType)
        {

            if (mb.ArrayCount>0)
            {
                return string.Format("\tCore.TempString.Add(\"{2}\");Settings.Set{0}Vector(Core.TempString.GetText(),({1}*)&{2},sizeof({2})/sizeof({1}));Core.TempString.Truncate(tmpSize);\n", basicSerializableTypes[dataType], dataType, mb.Name);
            }
            else
            {
                return string.Format("\tCore.TempString.Add(\"{1}\");Settings.Set{0}(Core.TempString.GetText(),{1});Core.TempString.Truncate(tmpSize);\n", basicSerializableTypes[dataType], mb.Name);
            }
        }
        private string CreateDeSerializableBasicTypeCode(string prefix, Member mb, string dataType)
        {
            if (mb.ArrayCount > 0)
            {
                return string.Format("\tCore.TempString.Add(\"{2}\");Settings.Copy{0}Vector(Core.TempString.GetText(),({1}*)&{2},sizeof({2})/sizeof({1}));Core.TempString.Truncate(tmpSize);\n", basicSerializableTypes[dataType], dataType, mb.Name);
            }
            else
            {
                return string.Format("\tCore.TempString.Add(\"{1}\");Settings.Copy{0}(Core.TempString.GetText(),{1});Core.TempString.Truncate(tmpSize);\n", basicSerializableTypes[dataType], mb.Name);
            }
        }
        private string GenerateSerializationCode(string fileName,Module cls,Member mb,bool codeForSerialization)
        {
            string s = "";
            // basic types
            if (basicSerializableTypes.ContainsKey(mb.DataType))
            {
                if (codeForSerialization)
                    return CreateSerializableBasicTypeCode(cls.Name, mb,mb.DataType);
                else
                    return CreateDeSerializableBasicTypeCode(cls.Name, mb,mb.DataType);
            }
            
            // iau tipul 
            Module mType = Compiler.GetModule(mb.DataType);
            if (mType==null)
            {
                AddError(fileName, mb, string.Format("Type '{0}' from member '{1}' from class '{2}' is invalid (NULL) and can not be serialized/deserialized !",mb.DataType,mb.Name,cls.Name));
                return "";
            }

            // daca este de tipul String
            if (mType.Name == "String")
            {
                if (codeForSerialization)
                {
                    if (mb.ArrayCount > 0)
                    {
                        s += "\tfor (unsigned int tr=0;tr<sizeof(" + mb.Name + ")/sizeof(void*);tr++) {\n";
                        s += String.Format("\t\tCore.TempString.AddFormatedEx(\"{0}[%{{ui32}}].\",tr);\n", mb.Name);
                        s += String.Format("\t\tSettings.SetString(Core.TempString.GetText()," + mb.Name + "[tr]->GetText());\n");
                        s += String.Format("\t\tCore.TempString.Truncate(tmpSize);\n");
                        s += "\t};\n";
                    }
                    else
                    {
                        return String.Format("\tCore.TempString.Add(\"{0}\");Settings.SetString(Core.TempString.GetText(),{0}->GetText());Core.TempString.Truncate(tmpSize);\n", mb.Name);
                    }
                }
                else
                {
                    if (mb.ArrayCount > 0)
                    {
                        s += "\tfor (unsigned int tr=0;tr<sizeof(" + mb.Name + ")/sizeof(void*);tr++) {\n";
                        s += String.Format("\t\tCore.TempString.AddFormatedEx(\"{0}[%{{ui32}}].\",tr);\n", mb.Name);
                        s += String.Format("\t\tSettings.CopyString(Core.TempString.GetText(),(" + mb.Name + "[tr]));\n");
                        s += String.Format("\t\tCore.TempString.Truncate(tmpSize);\n");
                        s += "\t};\n";
                    }
                    else
                    {
                        return String.Format("\tCore.TempString.Add(\"{0}\");Settings.CopyString(Core.TempString.GetText(),({0}));Core.TempString.Truncate(tmpSize);\n", mb.Name);
                    }
                }
                return s;
            }

            // enumerari
            if (mType.Type == ModuleType.Enum)
            {
                if (basicSerializableTypes.ContainsKey(mType.Cpp)==false)
                {
                    AddError(fileName, mb, string.Format("Member '{1}' of type '{0}' from class '{2}' can not be serialized/deserialized !. A Enum should be derived from a basic type in order to be serialized/deserialized !", mb.DataType, mb.Name, cls.Name));
                    return "";
                }
                if (codeForSerialization)
                    return CreateSerializableBasicTypeCode(cls.Name, mb, mType.Cpp);
                else
                    return CreateDeSerializableBasicTypeCode(cls.Name, mb, mType.Cpp);
            }
            // daca e o clasa
            if (mType.Type == ModuleType.Class)
            {
                if (codeForSerialization)
                {
                    if (mb.ArrayCount > 0)
                    {                        
                        s += "\tfor (unsigned int tr=0;tr<sizeof("+mb.Name+")/sizeof(void*);tr++) {\n";
                        s += String.Format("\t\tCore.TempString.AddFormatedEx(\"{0}[%{{ui32}}].\",tr);\n", mb.Name);
                        s += String.Format("\t\t(({0}*)((({0}**){1})[tr]))->__internal__Serialize(false,onlyPersistent);Core.TempString.Truncate(tmpSize);\n", mType.Name, mb.Name);
                        s += "\t};\n";
                    }
                    else
                    {
                        return String.Format("\tCore.TempString.Add(\"{0}.\");{0}->__internal__Serialize(false,onlyPersistent);Core.TempString.Truncate(tmpSize);\n", mb.Name);
                    }
                }
                else
                {
                    if (mb.ArrayCount > 0)
                    {
                        s += "\tfor (unsigned int tr=0;tr<sizeof(" + mb.Name + ")/sizeof(void*);tr++) {\n";
                        s += String.Format("\t\tCore.TempString.AddFormatedEx(\"{0}[%{{ui32}}].\",tr);\n", mb.Name);
                        s += String.Format("\t\t(({0}*)((({0}**){1})[tr]))->__internal__Deserialize(false,onlyPersistent);Core.TempString.Truncate(tmpSize);\n", mType.Name, mb.Name);
                        s += "\t};\n";
                    }
                    else
                    {
                        return String.Format("\tCore.TempString.Add(\"{0}.\");{0}->__internal__Deserialize(false,onlyPersistent);Core.TempString.Truncate(tmpSize);\n", mb.Name);
                    }
                }
                return s;
            }
            AddError(fileName, mb, string.Format("Type '{0}' from member '{1}' from class '{2}' can not be serialized/deserialized !", mb.DataType, mb.Name, cls.Name));
            return "";
        }

        private static string GetSceneIDsList()
        {
            string s = "";
            Module moduleScenes = Compiler.GetModule("Scenes");
            int index = 32;
            foreach (string sceneName in moduleScenes.Members.Keys)
            {
                if (StandardScenes.ContainsKey(sceneName)) {
                    s += "#define GAC_SCENE_ID_" + sceneName + " (" + StandardScenes[sceneName] + ")\n";
                } else {
                    s += "#define GAC_SCENE_ID_" + sceneName + " ("+index.ToString()+")\n";
                    index++;
                }
            }
            return s+"\n\n";
        }
        
        private string CreateCppCodeForModule(string fileName,Module m)
        {
            string s = "";
            string serializable = "";
            string deserializable = "";
            string serializable_persistent = "";
            string deserializable_persistent = "";
            Module mbase = GetTopMostBaseClass(m);
            foreach (Member mb in m.Members.Values)
            {
                if (mb.Parent != m) // e o functie derivata
                    continue;
                if (mb.Type == MemberType.Constructor)
                {
                    s += CreateConstructorDefinition(m, mb, false) + " {\n";
                    s += "\t" + CreateCppCode(mb.StartCode, mb.EndCode);
                    s += "\n}\n";
                    if (mb.Overrides != null)
                    {
                        foreach (Member mbo in mb.Overrides)
                        {
                            s += CreateConstructorDefinition(m, mbo, false) + " {\n";
                            s += "\t" + CreateCppCode(mbo.StartCode, mbo.EndCode);
                            s += "\n}\n";
                        }
                    }
                }
                if (mb.Type == MemberType.Destructor)
                {
                    s += CreateDestructorDefinition(m, mb, false) + " {\n";
                    s += "\t" + CreateCppCode(mb.StartCode, mb.EndCode);
                    s += "\n}\n";
                }
                if (mb.Type == MemberType.Function)
                {
                    s += CreateFunctionDefinition(m, mb, false) + " {\n";
                    s += "\t" + CreateCppCode(mb.StartCode, mb.EndCode);
                    s += "\n}\n";
                    if (mb.Overrides != null)
                    {
                        foreach (Member mbo in mb.Overrides)
                        {
                            s += CreateFunctionDefinition(m, mbo, false) + " {\n";
                            s += "\t" + CreateCppCode(mbo.StartCode, mbo.EndCode);
                            s += "\n}\n";
                        }
                    }
                }
                if ((mb.Type == MemberType.Variable) && (mb.Serializable!= SerializableType.None))
                {
                    if (mb.Serializable == SerializableType.Persistent)
                    {
                        serializable_persistent += GenerateSerializationCode(fileName, m, mb,true);
                        deserializable_persistent += GenerateSerializationCode(fileName, m, mb,false);
                    }
                    else
                    {
                        serializable += GenerateSerializationCode(fileName, m, mb,true);
                        deserializable += GenerateSerializationCode(fileName, m, mb,false);
                    }
                }
            }
            // verificari de derivare
            if (m.TypeInformation == ModuleTypeInformation.IsScene)
            {
                if ((mbase==null) || (mbase.Name!="Scene"))
                {
                    AddError(fileName, m.ObjectToken, "Scene object '"+m.Name+"' must be derived from a Scene or a Scene derived class.");
                    return "";
                }
            }
            if (m.TypeInformation == ModuleTypeInformation.IsApp)
            {
                foreach (Member mb in m.Members.Values)
                {
                    switch (mb.Type)
                    {
                        case MemberType.Constant:
                            AddError(fileName, m.ObjectToken, "Application interface can not contain constants. Define them externaly !");
                            return "";
                        case MemberType.Constructor:
                        case MemberType.Destructor:
                            AddError(fileName, m.ObjectToken, "Application interface can not have a constructor and/or destructor !");
                            return "";
                        case MemberType.Variable:
                            AddError(fileName, m.ObjectToken, "Application interface can not contain any variable. Move them to the Global object !");
                            return "";
                    }
                }
            }
            if (mbase.Name == "Control")
            {
                Compiler.GetGACFile(fileName).FileType = GACFileType.Control;
            }
            // pun functiile de serializare si deserializare
            if ((serializable.Length > 0) || (deserializable.Length > 0) || (serializable_persistent.Length > 0) || (deserializable_persistent.Length > 0))
            {
                
                if (mbase==null)
                {
                    AddError(fileName, m.ObjectToken, "Internal error - unable to locate the derivation class for " + m.Name);
                    return "";
                }
                if (mbase.Name == "Application")
                {
                    AddError(fileName, m.ObjectToken, "App class or Application derived class can not contain serializable or persistent variables. Move those variables to Global class.");
                    return "";
                }
                if ((mbase.Name != "Scene") && (mbase.Name != "Object") && (mbase.Name != "Global"))
                {
                    AddError(fileName, m.ObjectToken, "Class '" + m.Name + "' needs to be derived from Object or Scene to be able to have serializable or persistent variables !");
                    return "";
                }
                if ((mbase.Name == "Scene") && (serializable_persistent.Length>0))
                {
                    AddError(fileName, m.ObjectToken, "Scene '" + m.Name + "' can not have persistent variables. A scene can only have serializable variables. Only Global can have persistent variables !");
                    return "";
                }
                if ((mbase.Name == "Global") && (serializable.Length > 0))
                {
                    AddError(fileName, m.ObjectToken, "'Global' can not have serializable variables - only persistent. Move serializable variables into a scene !");
                    return "";
                }

                s += "\nvoid " + m.Name + "::__internal__Serialize(bool firstCall, bool onlyPersistent) {\n\tint tmpSize;\n";
                s += "\tif (firstCall) Core.TempString.Set(\"" + m.Name + ".\");\n\ttmpSize = Core.TempString.Len();\n";
                s += serializable_persistent;
                s += "\tif (onlyPersistent) return;\n\n";
                s += serializable;
                s += "}\n";
                s += "\nvoid " + m.Name + "::__internal__Deserialize(bool firstCall, bool onlyPersistent) {\n\tint tmpSize;\n";
                s += "\tif (firstCall) Core.TempString.Set(\"" + m.Name + ".\");\n\ttmpSize = Core.TempString.Len();\n";
                s += deserializable_persistent;
                s += "\tif (onlyPersistent) return;\n\n";
                s += deserializable;
                s += "}\n";
            }
            // adaug si crearea de scene
            if (m.TypeInformation == ModuleTypeInformation.IsApp)
            {
                s += "\nbool " + m.Name + "::OnCreateScenes() {";
                Module moduleScenes = Compiler.GetModule("Scenes");
                foreach (string sceneName in moduleScenes.Members.Keys)
                {
                    s += "\n\tCHECK(SetScene(" + moduleScenes.Members[sceneName].Cpp + ",new " + sceneName + "()),false,\"Unblea to add scene: " + sceneName + "\");";
                }
                s += "\n\treturn (true);";
                s += "\n}\n";
            }
            return s;
        }
        public bool CreateCppFile(string fileName,ref string content)
        {
            string s = "#include \"Framework.h\"\n";
            
            GACFile gf = Compiler.GetGACFile(fileName);
            foreach (Module m in gf.Modules.Values)
            {
                if (m.Type == ModuleType.Class)
                    s += CreateCppCodeForModule(fileName,m);
            }
            if (FileName.ToLower().Equals("global.gac"))
            {
                Module m = Compiler.GetModule("Global");
                if (m != null)
                {
                    s += CreateCppCodeForModule(fileName,m);
                }
            }          
            content = s;
            return (Error.Length == 0);
        }
        public string GetFileName(string extension)
        {
            if (extension == null)
                return FileName;
            if (extension.StartsWith(".") == false)
                extension = "." + extension;
            if (extension == ".")
                extension = "";
            return FileName.Replace(".gac", extension);
        }
        public string GetFileName()
        {
            return GetFileName(null);
        }
        public static string CreateIDsHFileComponent(Project prj, GenericBuildConfiguration build)
        {
            string s = "#ifndef __GAC_IDs_H_FILE__\n#define __GAC_IDs_H_FILE__\n\n";
            // ID-urile scenelor
            s += "\n//----------- SCENES ID ------------\n";
            s += GACParser.GetSceneIDsList();
            // end
            s += "\n#endif\n";
            return s;
        }
        public static string CreateHFileComponent(Project prj, GenericBuildConfiguration build)
        {
            Dictionary<string,Class> LocalClasses = Compiler.CreateLocalClassList();
            string s = "";
            //*
            string lastAccess;

            s += "\n//----------- DEBUG COMMANDDS PARAMS --------\n";
            s += GACParser.GetDebugCommandParams(prj);

            s += "\n//----------- CLASS LIST --------------------\n";
            foreach (Module cls in Compiler.LocalClasses)
                s += "class " + cls.Name + ";\n";
            s += "class Global;\n";
            foreach (var a in prj.AnimationObjects)
                s += "class " + a.GetCPPClassName() + ";\n";


            s += "\n//----------- TYPE POINTER LIST -------------\n";
            foreach (Module cls in Compiler.LocalClasses)
            {
                if (cls.Type != ModuleType.Class)
                    continue;
                s += "typedef " + cls.Name + " * " + cls.GetGACCPPType(true) + ";\n";
                s += "typedef " + cls.Name + " " + cls.GetGACCPPType(false) + ";\n";
            }
            GACFile system = Compiler.GetGACFile("");
            foreach (Module cls in system.Modules.Values)
            {
                if (cls.Type != ModuleType.Class)
                    continue;
                if (cls.BasicType == false)
                    s += "typedef " + cls.Cpp + " * " + cls.GetGACCPPType(true) + ";\n";
                s += "typedef " + cls.Cpp + " " + cls.GetGACCPPType(false) + ";\n";
            }   

            // adaug definitiile
            s += "\n//----------- ENUMS and DEFINES ------------\n";
            s += Compiler.GetPreprocesorData();


            s += "\n//----------- CLASS DEFINITIONS ------------\n";
            foreach (Module cls in Compiler.LocalClasses)
            {
                bool isSerializable = false;
                lastAccess = "";
                if (cls.Name == "Global")
                {
                    s += "class Global: public GApp::UI::GlobalDataContainer {\n";
                }
                else
                {
                    s += "class " + cls.Name;
                    if ((cls.DerivedFrom != null) && (cls.DerivedFrom.Length > 0))
                    {
                        // trebuie numele complet cand facem derivarea
                        Module derive = Compiler.GetModule(cls.DerivedFrom);
                        if (derive.Cpp.Length > 0)
                            s += " : public " + derive.Cpp;
                        else
                            s += " : public " + derive.Name;
                    }
                    s += " {\n";
                }
                foreach (Member v in cls.Members.Values)
                {
                    if (v.Type != MemberType.Variable)
                        continue;
                    if (v.Access.ToString() != lastAccess)
                    {
                        s += "\t" + v.Access.ToString().ToLower() + ":\n";
                        lastAccess = v.Access.ToString();
                    }
                    s += "\t\t" + CreateVariableDefinition(v) + "\n";
                    if (v.Serializable != SerializableType.None)
                        isSerializable = true;
                }
                foreach (Member f in cls.Members.Values)
                {
                    if (f.Type != MemberType.Function)
                        continue;
                    if (f.Parent != cls) // e o functie din clasa de baza
                        continue;
                    if (f.Access.ToString() != lastAccess)
                    {
                        s += "\t" + f.Access.ToString().ToLower() + ":\n";
                        lastAccess = f.Access.ToString();
                    }
                    s += "\t\t" + CreateFunctionDefinition(cls, f, true) + ";\n";
                    if (f.Overrides != null)
                    {
                        foreach (Member over_f in f.Overrides)
                        {
                            if (over_f.Access.ToString() != lastAccess)
                            {
                                s += "\t" + over_f.Access.ToString().ToLower() + ":\n";
                                lastAccess = over_f.Access.ToString();
                            }
                            s += "\t\t" + CreateFunctionDefinition(cls, over_f, true) + ";\n";
                        }
                    }
                }
                if (lastAccess != "public")
                {
                    s += "\tpublic:\n";
                    lastAccess = "public";
                }
                foreach (Member ctor in cls.Members.Values)
                {
                    if (ctor.Type != MemberType.Constructor)
                        continue;
                    if (ctor.Parent != cls) // e o functie din clasa de baza
                        continue;
                    s += "\t\t" + CreateConstructorDefinition(cls, ctor, true) +";\n";
                    if (ctor.Overrides != null)
                    {
                        foreach (Member over_f in ctor.Overrides)
                        {
                            if (over_f.Access.ToString() != lastAccess)
                            {
                                s += "\t" + over_f.Access.ToString().ToLower() + ":\n";
                                lastAccess = over_f.Access.ToString();
                            }
                            s += "\t\t" + CreateConstructorDefinition(cls, over_f, true) + ";\n";
                        }
                    }
                }
                foreach (Member ctor in cls.Members.Values)
                {
                    if (ctor.Type != MemberType.Destructor)
                        continue;
                    s += "\t\t" + CreateDestructorDefinition(cls, ctor, true) + ";\n";
                }
                // functiile de sincronizare
                if (isSerializable)
                {
                    s += "\t\tvoid __internal__Serialize(bool firstTime,bool onlyPersistent);\n";
                    s += "\t\tvoid __internal__Deserialize(bool firstTime,bool onlyPersistent);\n";
                }
                if (cls.TypeInformation == ModuleTypeInformation.IsApp)
                {
                    s += "public:\n\t\tbool OnCreateScenes();\n";                    
                }
                s += "};\n";
            }
            s += "\n//----------- ANIMATION WRAPPER CLASSES ------------\n";
            // clasele pentru animatii
            foreach (var a in prj.AnimationObjects)
                s += a.CreateCPPWrapperHeaderClass();

            return s;
        }
        #endregion

        #region IntelliSense

        
        public static ImageList AutocompleteIcons = new ImageList();
        // 0 - Clasa
        // 1 - Clasa Statica
        // 2 - Tip
        // 3 - Enumerare
        // 4 - Metoda
        // 5 - Variabila
        // 6 - Membru Enumerare
        // 7 - Constanta
        // 8 - Resources
         

        private static Dictionary<Type, string[]> resTypeInfo = new Dictionary<Type, string[]>()
        {
            {typeof(ImageResource),new string[]{"Images","Bitmap"}},
            {typeof(RawResource),new string[]{"RawData","RawResource"}},
            {typeof(ShaderResource),new string[]{"Shaders","Shader"}},
            {typeof(FontResource),new string[]{"Fonts","Font"}},
            {typeof(SoundResource),new string[]{"Sounds","int"}},
            {typeof(PresentationResource),new string[]{"Presentations","Presentation"}},
        };
        private static Dictionary<ResourcesConstantType, string> dataTypeToGACRepresentation = new Dictionary<ResourcesConstantType, string>()
        {
            {ResourcesConstantType.Presentation,    "Presentation"},
            {ResourcesConstantType.Font,            "Font"},
            {ResourcesConstantType.Image,           "Bitmap"},
            {ResourcesConstantType.Raw,             "RawResource"},
            {ResourcesConstantType.Shader,          "Shader"},
            {ResourcesConstantType.Sound,           "Sound"},
            {ResourcesConstantType.String,          "string"},
        };
        private static Dictionary<BasicTypesConstantType, string> basicTypeToGACRepresentation = new Dictionary<BasicTypesConstantType, string>()
        {
            {BasicTypesConstantType.Boolean,        "bool"},
            {BasicTypesConstantType.Color,          "Color"},
            {BasicTypesConstantType.Float32,        "float"},
            {BasicTypesConstantType.Float64,        "Float64"},
            {BasicTypesConstantType.Int8,           "Int8"},
            {BasicTypesConstantType.Int16,          "Int16"},
            {BasicTypesConstantType.Int32,          "Int32"},
            {BasicTypesConstantType.Int64,          "Int64"},
            {BasicTypesConstantType.UInt8,          "UInt8"},
            {BasicTypesConstantType.UInt16,         "UInt16"},
            {BasicTypesConstantType.UInt32,         "UInt32"},
            {BasicTypesConstantType.UInt64,         "UInt64"},
            {BasicTypesConstantType.String,         "string"},
        };
        private static string ConvertDataTypToGACType(string type)
        {
            ConstantModeType cmt = ConstantHelper.GetConstantMode(type);
            switch (cmt)
            {
                case ConstantModeType.Resources: return dataTypeToGACRepresentation[ConstantHelper.GetResourcesType(type)];
                case ConstantModeType.Enumerations: return ConstantHelper.GetEnumerationType(type);
                case ConstantModeType.DataTypes: return ConstantHelper.GetDataTypesType(type);
                case ConstantModeType.BasicTypes: return basicTypeToGACRepresentation[ConstantHelper.GetBasicTypesType(type)];
            }
            return "void??";
        }

        private static string GetDebugCommandParams(Project prj)
        {
            string s="\n#ifdef PLATFORM_DEVELOP\n#pragma pack(push,align_1_octet,1)\n#endif\n";
            foreach (DebugCommand cmd in prj.DebugCommands)
            {
                if (cmd.Parameters.Count == 0)
                    continue;
                s += "\tclass GAC_DEBUG_" + cmd.Name + "Params {\n\tpublic:\n";
                foreach (DebugCommandParam dcp in cmd.Parameters)
                {
                    switch (dcp.Type)
                    {
                        case DebugCommandParamType.Int8:
                        case DebugCommandParamType.UInt8:
                        case DebugCommandParamType.Int32:
                        case DebugCommandParamType.UInt32:
                        case DebugCommandParamType.Int16:
                        case DebugCommandParamType.UInt16:
                            s += "\t\t" + dcp.Type.ToString() + " " + dcp.Name + ";\n";
                            break;
                        case DebugCommandParamType.Float32:
                            s += "\t\tfloat " + dcp.Name + ";\n";
                            break;
                        case DebugCommandParamType.Enum:
                               s += "\t\tchar " + dcp.Name + ";\n";
                            break;
                        case DebugCommandParamType.Color:
                            s += "\t\tunsigned int " + dcp.Name + ";\n";
                            break;
                        case DebugCommandParamType.Boolean:
                            s += "\t\tunsigned char " + dcp.Name + ";\n";
                            break;                   
                    }
                }
                // operatori + constructori
                s += "\t\tGAC_DEBUG_" + cmd.Name + "Params () { memset(this, 0, sizeof(GAC_DEBUG_" + cmd.Name + "Params)); }\n";
                s += "\t\tGAC_DEBUG_" + cmd.Name + "Params (UInt64 value) { memcpy(this, &value, sizeof(GAC_DEBUG_" + cmd.Name + "Params)); }\n";
                s += "\t\tUInt64 operator = (UInt64 value) { memcpy(this, &value, sizeof(GAC_DEBUG_" + cmd.Name + "Params)); return value; }\n";
                s += "\t\tGAC_DEBUG_" + cmd.Name + "Params* operator -> () { return this; }\n";
                s += "\t};\n";
            }
            s += "#ifdef PLATFORM_DEVELOP\n#pragma pack(pop,align_1_octet)\n#endif\n\n"; 
            return s;
        }
        private static void UpdateEnums(Project prj)
        {
            GACFile gf = Compiler.GetGACFile("");
            Module moduleEnums = Compiler.GetModule("Enums");
            Module cls;

            if (moduleEnums == null)
                return;
            // curat totul
            foreach (Module m in moduleEnums.Modules.Values)
                gf.RemoveModule(m.Name);
            moduleEnums.Modules.Clear();
            moduleEnums.Members.Clear();

            foreach (Enumeration en in prj.Enums)
            {
                cls = new Module(ModuleType.Enum, en.Name, en.Type.ToString(), "", true, null, "");
                cls.Description = en.Description;
                if (gf.Modules.ContainsKey(cls.Name))
                {
                    AddError("", 0, 0, "Enum '" + en.Name + "' already exists !");
                    continue;
                }
                gf.Modules[cls.Name] = cls;
                moduleEnums.Modules[cls.Name] = cls;
                foreach (EnumValue ev in en.Values)
                {
                    cls.Members[ev.Name] = new Member(cls, MemberType.Constant, ev.Name, "GAC_ENUM_"+en.Name+"_"+ev.Name, en.Type.ToString(), ev.Description,null);
                }
                cls.UpdateAutoCompleteList();
            }
            
            moduleEnums.UpdateAutoCompleteList();
        }
        private static void UpdateAnimations(Project prj)
        {
            GACFile gf = Compiler.GetGACFile("");
            Module moduleAnimations = Compiler.GetModule("Animations");
            Module cls;

            if (moduleAnimations == null)
                return;
            // curat totul
            foreach (Module m in moduleAnimations.Modules.Values)
                gf.RemoveModule(m.Name);
            moduleAnimations.Modules.Clear();
            moduleAnimations.Members.Clear();

            Dictionary<string, List<string>> enums = new Dictionary<string, List<string>>();

            foreach (var ao in prj.AnimationObjects)
            {
                cls = new Module(ModuleType.Class, ao.Name, ao.GetCPPClassName(), "", false, null, "");
                if (gf.Modules.ContainsKey(cls.Name))
                {
                    AddError("", 0, 0, "Animation '" + ao.Name + "' already exists !");
                    continue;
                }
                gf.Modules[cls.Name] = cls;
                moduleAnimations.Modules[cls.Name] = cls;
                enums.Clear();
                ao.BuildGACParameters(cls,enums);
                if (enums.Count>0)
                {
                    foreach (string ename in enums.Keys)
                    {
                        if (gf.Modules.ContainsKey(ename))
                        {
                            AddError("", 0, 0, "Animation '" + ao.Name + "' has an enumeration '"+ename+"' that was already defined !");
                            continue;
                        }
                        Module m_enum = new Module(ModuleType.Enum, ename, "int", "", true, null, null);
                        List<string> l_v_enum = enums[ename];
                        for (int tr=0;tr<l_v_enum.Count;tr++)
                        {
                            m_enum.Members[l_v_enum[tr]] = new Member(m_enum, MemberType.Constant, l_v_enum[tr], tr.ToString(), "int", "", null);
                        }
                        gf.Modules[m_enum.Name] = m_enum;
                        moduleAnimations.Modules[m_enum.Name] = m_enum;
                        m_enum.UpdateAutoCompleteList();
                    }
                }
                cls.UpdateAutoCompleteList();
            }
            moduleAnimations.UpdateAutoCompleteList();
        }
        private static void UpdateConstantsAndDataTypeValues(Project prj)
        {
            GACFile gf = Compiler.GetGACFile("");
            Module classConstants = Compiler.GetModule("Constants");

            if (classConstants == null)
                return;
            // curat totul
            classConstants.Modules.Clear();
            classConstants.Members.Clear();

            foreach (ConstantValue Const in prj.Constants)
            {
                if (Const.MatrixColumnsCount == 0)
                {
                    classConstants.Members[Const.Name] = new Member(classConstants, MemberType.Constant, Const.Name, "GAC_CONSTANTS_" + Const.Name, "", Const.Description, null);
                }
                else
                {
                    Member m = new Member(classConstants, MemberType.Variable, Const.Name, "GAC_CONSTANTS_" + Const.Name, ConvertDataTypToGACType(Const.Type), Const.Description, null);
                    m.Static = true;
                    m.ArrayCount = 1;
                    if (Const.MatrixColumnsCount > 1)
                        m.ArrayCount = 2;
                    classConstants.Members[Const.Name] = m;
                }
            }

            // pentru fiecare DataTypeValue iau valorile
            ArrayCounter ac = new ArrayCounter();
            foreach (Structure s in prj.Structs)
            {
                ac.Clear();
                foreach (StructureValue sv in s.Values)
                {
                    if (sv.Name.Length == 0)
                        continue;
                    ac.Add(sv.Name, sv.Array1, sv.Array2);
                }
                string[] vars = ac.Variables;
                foreach (string v in vars)
                {
                    Member m = new Member(classConstants, MemberType.Variable, v, "Res.ConstantValues." + v, s.Name, "",null);                    
                    m.Static = true;
                    m.ArrayCount = 0;
                    int a1 = ac.GetArray1(v);
                    int a2 = ac.GetArray2(v);
                    if ((a1 > 0) && (a2 <= 0))
                        m.ArrayCount = 1;
                    if ((a1 > 0) && (a2 > 0))
                        m.ArrayCount = 2;
                    classConstants.Members[v] = m;
                }
            }

            classConstants.UpdateAutoCompleteList();
        }
        private static void UpdateDataTypes(Project prj)
        {
            GACFile gf = Compiler.GetGACFile("");
            Module moduleDataTypes = Compiler.GetModule("DataTypes");
            Module cls;

            if (moduleDataTypes == null)
                return;
            // curat totul
            foreach (Module m in moduleDataTypes.Modules.Values)
                gf.RemoveModule(m.Name);
            moduleDataTypes.Modules.Clear();
            moduleDataTypes.Members.Clear();

            foreach (Structure s in prj.Structs)
            {
                cls = new Module(ModuleType.Class, s.Name, "GAC_DATATYPE_"+s.Name, "", false, null, "");
                cls.Description = s.Description;
                if (gf.Modules.ContainsKey(cls.Name))
                {
                    AddError("", 0, 0, "Structure/DataType '" + s.Name + "' already exists !");
                    continue;
                }
                gf.Modules[cls.Name] = cls;
                moduleDataTypes.Modules[cls.Name] = cls;
                foreach (StructureField sf in s.Fields)
                {
                    Member m = new Member(cls, MemberType.Variable, sf.Name, "", ConvertDataTypToGACType(sf.Type), sf.Description,null);
                    m.Access = ModifierType.Public;
                    if (sf.List)
                        m.ArrayCount = 1;
                    cls.Members[sf.Name] = m;
                    if (sf.List)
                    {
                        // adaug si counterul
                        m = new Member(cls, MemberType.Variable, sf.Name+"Count", "", ConvertDataTypToGACType("Int32"), "Number of elements from "+sf.Name,null);
                        m.Access = ModifierType.Public;
                        cls.Members[sf.Name + "Count"] = m;
                    }
                }
                cls.UpdateAutoCompleteList();
            }

            moduleDataTypes.UpdateAutoCompleteList();
        }
        private static void UpdateResources(Project prj, GenericBuildConfiguration currentBuild)
        {
            //*
            GACFile gf = Compiler.GetGACFile("");
            Module Resources = Compiler.GetModule("Resources");
            Module TShader = Compiler.GetModule("Shader");

            if ((prj != null) && (Resources != null))
            {
                foreach (Module m in Resources.Modules.Values)
                    gf.RemoveModule(m.Name);
                Resources.Modules.Clear();
                Resources.Members.Clear();
                Module cls;
                Dictionary<string, Module> addedResources = new Dictionary<string, Module>();
                Dictionary<string, bool> usedResNames = new Dictionary<string, bool>();
                // resurse
                foreach (GenericResource r in prj.Resources)
                {
                    if (resTypeInfo.ContainsKey(r.GetType()))
                    {
                        string tname = resTypeInfo[r.GetType()][0];
                        if (addedResources.ContainsKey(tname) == false)
                        {
                            cls = new Module(ModuleType.StaticClass, tname, "", "", false, null, "");
                            gf.Modules[cls.Name] = cls;
                            Resources.Modules[cls.Name] = cls;
                            addedResources[tname] = cls;
                        }
                        else
                        {
                            cls = addedResources[tname];
                        }
                        string key = r.GetType().ToString() + "_" + r.Name;
                        if (usedResNames.ContainsKey(key))
                            continue;
                        usedResNames[key] = true;
                        string dataType = resTypeInfo[r.GetType()][1];
                        if (r.GetType() == typeof(ShaderResource))
                        {
                            dataType = "ShaderType" + r.Name;
                            if (addedResources.ContainsKey(dataType) == false)
                            {
                                Module ShaderCls = new Module(ModuleType.StaticClass, dataType, "", "Shader", false, null, "");
                                foreach (ShaderVariable sv in ((ShaderResource)r).Uniforms)
                                {
                                    if (sv.Type == ShaderVariableType.None)
                                        continue;
                                    Member mshd = null;
                                    switch (sv.Type)
                                    {
                                        case ShaderVariableType.Color:
                                            mshd = new Member(ShaderCls, MemberType.Variable, sv.Name, "", "UInt32", "",null);
                                            break;
                                        case ShaderVariableType.Float:
                                            mshd = new Member(ShaderCls, MemberType.Variable, sv.Name, "", "Float32", "",null);
                                            break;
                                        case ShaderVariableType.ColorChannel:
                                            mshd = new Member(ShaderCls, MemberType.Variable, sv.Name, "", "Int32", "",null);
                                            break;
                                    }
                                    if (mshd != null)
                                        ShaderCls.Members[mshd.Name] = mshd;
                                }
                                // copii si membrii din shader - momentan nefunctional pentru ca OnUpdate suprascrie ce pun eu
                                /*
                                foreach(Member mshd in TShader.Members.Values)
                                {
                                    ShaderCls.Members[mshd.Name] = mshd;
                                }
                                */
                                gf.Modules[ShaderCls.Name] = ShaderCls;
                                Resources.Modules[ShaderCls.Name] = ShaderCls;
                                addedResources[dataType] = ShaderCls;
                            }
                        }
                        Member mb = new Member(cls, MemberType.Variable, r.Name, "Res." + cls.Name + "." + r.Name, dataType, "",null);
                        if (r.Array2 >= 0)
                            mb.ArrayCount = 2;
                        else if (r.Array1 >= 0)
                            mb.ArrayCount = 1;
                        mb.Static = true;
                        cls.Members[mb.Name] = mb;
                    }
                }
                // stringuri
                foreach (StringValues sv in prj.Strings)
                {
                    string tname = "Strings";
                    if (addedResources.ContainsKey(tname) == false)
                    {
                        cls = new Module(ModuleType.StaticClass, tname, "", "", false, null, "");
                        gf.Modules[cls.Name] = cls;
                        Resources.Modules[cls.Name] = cls;
                        addedResources[tname] = cls;
                    }
                    else
                    {
                        cls = addedResources[tname];
                    }
                    string key = "__strings__" + sv.VariableName;
                    if (usedResNames.ContainsKey(key))
                        continue;
                    usedResNames[key] = true;
                    Member mb = new Member(cls, MemberType.Variable, sv.VariableName, "Res." + cls.Name + "." + sv.VariableName, "string", "",null);
                    if (sv.Array2 >= 0)
                        mb.ArrayCount = 2;
                    else if (sv.Array1 >= 0)
                        mb.ArrayCount = 1;
                    mb.Static = true;
                    cls.Members[mb.Name] = mb;
                }
                // profile
                foreach (Profile p in prj.Profiles)
                {
                    string tname = "Profiles";
                    if (addedResources.ContainsKey(tname) == false)
                    {
                        cls = new Module(ModuleType.StaticClass, tname, "", "", false, null, "");
                        gf.Modules[cls.Name] = cls;
                        Resources.Modules[cls.Name] = cls;
                        addedResources[tname] = cls;
                    }
                    else
                    {
                        cls = addedResources[tname];
                    }
                    string key = "__profiles__" + p.Name;
                    if (usedResNames.ContainsKey(key))
                        continue;
                    usedResNames[key] = true;
                    Member mb = new Member(cls, MemberType.Variable, p.Name, "Res." + cls.Name + "." + p.Name, "Profile", "", null);
                    if (p.Array2 >= 0)
                        mb.ArrayCount = 2;
                    else if (p.Array1 >= 0)
                        mb.ArrayCount = 1;
                    mb.Static = true;
                    cls.Members[mb.Name] = mb;
                }
            }
            // AD-urile
            Module Ads = Compiler.GetModule("Ads");
            if ((Ads != null) && (prj != null))
            {
                foreach (GenericAd ad in prj.Ads)
                {
                    if (Ads.Members.ContainsKey(ad.Name))
                        continue;
                    Member mb = new Member(Ads, MemberType.Variable, ad.Name, "ADS." + ad.Name, "AdInterface", "", null);
                    mb.Static = true;
                    Ads.Members[mb.Name] = mb;
                }
            }
            
            // alte chestii automate
            if (prj != null) {
                GACParser.UpdateControlIDs(prj);
                GACParser.UpdateObjectEventsIDs(prj);
                GACParser.UpdateLeaderboards(currentBuild);
                GACParser.UpdateCountersAndAlarms(prj);
            }
            Compiler.UpdateGlobalAutoCompleteList();
        }
        public static void UpdateCountersAndAlarms(Project prj)
        {
            GACFile gf = Compiler.GetGACFile("");
            Module module;

            // ----------------------------------------- Counters ----------------------------------------------------
            module = Compiler.GetModule("Counters");
            // curat totul
            foreach (Module m in module.Modules.Values)
                gf.RemoveModule(m.Name);
            module.Modules.Clear();
            module.Members.Clear();

            foreach (Counter gc in prj.Counters)
            {
                if (module.Members.ContainsKey(gc.Name))
                    continue;
                Member mb = new Member(module, MemberType.Variable, gc.Name, "Res.Counters." + gc.Name, "Counter", gc.GetDescription(), null);
                mb.Static = true;
                module.Members[mb.Name] = mb;
            }
            module.UpdateAutoCompleteList();


            // ----------------------------------------- Counters Groups ----------------------------------------------------
            module = Compiler.GetModule("CountersGroups");
            // curat totul
            foreach (Module m in module.Modules.Values)
                gf.RemoveModule(m.Name);
            module.Modules.Clear();
            module.Members.Clear();

            foreach (CounterGroup gc in prj.CountersGroups)
            {
                if (module.Members.ContainsKey(gc.Name))
                    continue;
                Member mb = new Member(module, MemberType.Variable, gc.Name, "Res.CountersGroups." + gc.Name, "CountersGroup", "", null);
                mb.Static = true;
                module.Members[mb.Name] = mb;
            }
            module.UpdateAutoCompleteList();

            // ----------------------------------------- Counters ----------------------------------------------------
            module = Compiler.GetModule("Alarms");
            // curat totul
            foreach (Module m in module.Modules.Values)
                gf.RemoveModule(m.Name);
            module.Modules.Clear();
            module.Members.Clear();

            foreach (Alarm ac in prj.Alarms)
            {
                if (module.Members.ContainsKey(ac.Name))
                    continue;
                Member mb = new Member(module, MemberType.Variable, ac.Name, "Res.Alarms." + ac.Name, "Alarm", ac.propDescription, null);
                mb.Static = true;
                module.Members[mb.Name] = mb;
            }
            module.UpdateAutoCompleteList();

        }
        public static void UpdateControlIDs(Project prj)
        {
            UpdateIDsValue(prj, prj.ControlsIDs, "ControlID");
        }
        public static void UpdateObjectEventsIDs(Project prj)
        {
            UpdateIDsValue(prj, prj.ObjectEventsIDs, "Events");
        }
        public static void UpdateIDsValue(Project prj,List<ControlID> lst,string className)
        {
            GACFile gf = Compiler.GetGACFile("");
            Module module;

            // adaug si ID-urile controalelor
            module = Compiler.GetModule(className);
            // curat totul
            foreach (Module m in module.Modules.Values)
                gf.RemoveModule(m.Name);
            module.Modules.Clear();
            module.Members.Clear();

            foreach (ControlID cmd in lst)
            {
                module.Members[cmd.Name] = new Member(module, MemberType.Constant, cmd.Name, cmd.ID.ToString(), "int", cmd.Description, null);
            }
            module.UpdateAutoCompleteList();
        }
        public static void UpdateLeaderboards(GenericBuildConfiguration currentBuild)
        {
            GACFile gf = Compiler.GetGACFile("");
            Module module;

            // adaug si ID-urile controalelor
            module = Compiler.GetModule("Leaderboards");
            // curat totul
            foreach (Module m in module.Modules.Values)
                gf.RemoveModule(m.Name);
            module.Modules.Clear();
            module.Members.Clear();

            if (currentBuild != null)
            {
                List<string> ld = null;
                if (currentBuild.GetType() == typeof(AndroidBuildConfiguration))
                    ld = Project.StringListToList(((AndroidBuildConfiguration)currentBuild).GooglePlayLeaderboardsList);
                if (ld != null)
                {
                    for (int index = 0;index<ld.Count;index++)
                    {
                        List<string> w = Project.StringListToList(ld[index],'|');
                        if (w.Count != 3)
                            continue;
                        module.Members[w[0]] = new Member(module, MemberType.Constant, w[0], index.ToString(), "int", "", null);
                    }
                }
            }
            module.UpdateAutoCompleteList();
        }

        public static void UpdateDebugCommands(Project prj)
        {
            GACFile gf = Compiler.GetGACFile("");
            Module module = Compiler.GetModule("Debug");
            Module cls,param;

            // curat totul
            foreach (Module m in module.Modules.Values)
                gf.RemoveModule(m.Name);
            module.Modules.Clear();
            module.Members.Clear();

            // creez enumerarile si structurile pentru parametri
            cls = new Module(ModuleType.Enum, "DebugCommand", "UInt32", "", true, null, "");
            gf.Modules[cls.Name] = cls;
            module.Modules[cls.Name] = cls;
            prj.DebugCommands.Sort();
            int index = 0;
            foreach (DebugCommand cmd in prj.DebugCommands)
            {
                if (cls.GetMember(cmd.Name) != null)
                {
                    MessageBox.Show("Duplicate ID: -> Internal Error (invalid debug command) : " + cmd.Name);
                    continue;
                }
                cls.Members[cmd.Name] = new Member(cls, MemberType.Constant, cmd.Name, index.ToString(), "uint32", cmd.Description, null);
                // parametri
                if (cmd.Parameters.Count > 0)
                {
                    param = new Module(ModuleType.Class, cmd.Name + "Params", "GAC_DEBUG_" + cmd.Name + "Params", "", true, null, "");
                    gf.Modules[param.Name] = param;
                    module.Modules[param.Name] = param;
                    foreach (DebugCommandParam dcp in cmd.Parameters)
                    {
                        param.Members[dcp.Name] = new Member(param, MemberType.Variable, dcp.Name, "", "uint32", dcp.Description, null);
                    }
                }
                index++;
            }
            cls.UpdateAutoCompleteList();
            module.UpdateAutoCompleteList();



        }
        public static void UpdateGlobalAutoComplete(Project prj, GenericBuildConfiguration currentBuild)
        {
            Compiler.AutoCompleteList.Clear();
            // updatez alte elemente
            UpdateEnums(prj);
            UpdateConstantsAndDataTypeValues(prj);
            UpdateDataTypes(prj);
            UpdateAnimations(prj);
            UpdateResources(prj, currentBuild);
        }
        public bool CursorToFunction(int poz,out Module cls, out Member fnc)
        {            
            cls = null;
            fnc = null;
            //*
            GACFile gf = Compiler.GetGACFile(FileName);
            int dist = int.MaxValue;
            foreach (Module c in gf.Modules.Values)
            {
                foreach (Member f in c.Members.Values)
                {
                    if ((f.Type != MemberType.Constructor) && (f.Type != MemberType.Function) && (f.Type != MemberType.Destructor))
                        continue;
                    if ((f.StartCode >= 0) && (f.EndCode >= 0) && (f.Parent==c))
                    {
                        if ((poz >= Tokens[f.StartCode].Start) && ((poz - Tokens[f.StartCode].Start) < dist))
                        {
                            cls = c;
                            fnc = f;
                            dist = poz - Tokens[f.StartCode].Start;
                        }
                        if (f.Overrides != null)
                        {
                            foreach (Member fov in f.Overrides)
                            {
                                if ((poz >= Tokens[fov.StartCode].Start) && ((poz - Tokens[fov.StartCode].Start) < dist) && (fov.Parent == c))
                                {
                                    cls = c;
                                    fnc = fov;
                                    dist = poz - Tokens[fov.StartCode].Start;
                                }
                            }
                        }
                    }
                }
            }
            if (FileName.ToLower().Equals("global.gac"))
            {
                Module c = Compiler.GetModule("Global");
                if (c != null)
                {
                    foreach (Member f in c.Members.Values)
                    {
                        if ((f.Type != MemberType.Constructor) && (f.Type != MemberType.Function) && (f.Type != MemberType.Destructor))
                            continue;
                        if ((f.StartCode >= 0) && (f.EndCode >= 0) && (f.Parent == c))
                        {
                            if ((poz >= Tokens[f.StartCode].Start) && ((poz - Tokens[f.StartCode].Start) < dist))
                            {
                                cls = c;
                                fnc = f;
                                dist = poz - Tokens[f.StartCode].Start;
                            }
                            if (f.Overrides != null)
                            {
                                foreach (Member fov in f.Overrides)
                                {
                                    if ((poz >= Tokens[fov.StartCode].Start) && ((poz - Tokens[fov.StartCode].Start) < dist) && (fov.Parent == c))
                                    {
                                        cls = c;
                                        fnc = fov;
                                        dist = poz - Tokens[fov.StartCode].Start;
                                    }
                                }
                            }
                        }
                    }
                }
            }


            if (dist < int.MaxValue)
                return true;
            // */
            return false;
        }
        private static void AddFunctionParameters(List<LocalDefinition> l, Module module,Member function, string fileName, string functionDef)
        {
            List<Member> prms = function.GetParameters();
            if (prms == null)
                return;
            foreach (Member m in prms)
            {
                l.Add(new LocalDefinition(m.Name, fileName, m.GetLocalVarianleDefinition() + " parameter from " + functionDef, module.Name+"."+function.Name + "=>" + m.Name, m.FileStartPos, m.FileEndPos, m.FileLine, LocalDefinitionType.Parameter));
            }
        }
        private static void AddLocalDefinitionMember(List<LocalDefinition> l, Member m, string fileName, string parentPath, Module parent)
        {
            string def = m.GetDefinition(parentPath);
            if (m.FileStartPos < 0)
                return;
            switch (m.Type)
            {
                case MemberType.Variable:
                case MemberType.Constant:
                    l.Add(new LocalDefinition(m.Name, fileName, def, parentPath+m.Name, m.FileStartPos, m.FileEndPos, m.FileLine, LocalDefinitionType.Member));
                    break;
                case MemberType.Function:
                    l.Add(new LocalDefinition(m.Name, fileName, def, parentPath + m.Name, m.FileStartPos, m.FileEndPos, m.FileLine, LocalDefinitionType.Method));
                    AddFunctionParameters(l, parent, m, fileName, def);
                    break;
                case MemberType.Constructor:
                    l.Add(new LocalDefinition(parent.Name, fileName, def, parentPath + "<constructor>", m.FileStartPos, m.FileEndPos, m.FileLine, LocalDefinitionType.Constructor));
                    AddFunctionParameters(l, parent, m, fileName, def);
                    break;
                case MemberType.Destructor:
                    l.Add(new LocalDefinition(parent.Name, fileName, def, parentPath + "<destructor>", m.FileStartPos, m.FileEndPos, m.FileLine, LocalDefinitionType.Destructor));
                    break;
            }            
        }
        private static void AddLocaDefinitionForLocalVariable(List<LocalDefinition> l, Module module, Member function,Member m, string fileName,string functionName)
        {
            if (m.FileStartPos < 0)
                return;
            string prototype = module.Name + "." + function.Name + "=>" + m.Name;
            l.Add(new LocalDefinition(m.Name, fileName, m.GetLocalVarianleDefinition() + " from { " + functionName + " }", prototype, m.FileStartPos, m.FileEndPos, m.FileLine, LocalDefinitionType.LocalVariable));
        }
        private static void AddLocalDefinitionModule(List<LocalDefinition> l, Module m, string parentPath)
        {
            if (m.FileName.Length == 0)
                return;
            if (m.FileStartPos >= 0)
            {
                LocalDefinitionType type = LocalDefinitionType.Class;
                if (m.TypeInformation == ModuleTypeInformation.IsApp)
                    type = LocalDefinitionType.Application;
                if (m.TypeInformation == ModuleTypeInformation.IsScene)
                    type = LocalDefinitionType.Scene;
                l.Add(new LocalDefinition(m.Name, m.FileName, parentPath + m.Name, parentPath + m.Name, m.FileStartPos, m.FileEndPos, m.FileLine, type));
            }
            foreach (string key in m.Modules.Keys)
                AddLocalDefinitionModule(l,m.Modules[key],parentPath+m.Name+".");
            foreach (string key in m.Members.Keys)
            {
                AddLocalDefinitionMember(l, m.Members[key], m.FileName, parentPath + m.Name + ".", m);
                if (m.Members[key].Overrides != null)
                {
                    foreach (Member mm in m.Members[key].Overrides)
                        AddLocalDefinitionMember(l, mm, m.FileName, parentPath + m.Name + ".", m);
                }
                if (m.Members[key].LocalVariables!=null)
                {
                    string fdef = m.Members[key].GetDefinition(parentPath + m.Name + ".");
                    foreach (string localVar in m.Members[key].LocalVariables.Keys)
                    {
                        AddLocaDefinitionForLocalVariable(l, m, m.Members[key], m.Members[key].LocalVariables[localVar], m.FileName, fdef);
                    }
                }
            }
        }
        public static List<LocalDefinition> GetLocalDefinitions()
        {
            List<LocalDefinition> l = new List<LocalDefinition>();
            foreach (GACParser.Module m in GACParser.Compiler.LocalClasses)
            {
                AddLocalDefinitionModule(l, m, "");
            }
            return l;
        }
        private static void AddApiDefinitionMember(List<ApiDefinition> l, Member m, string parentPath, Module parent)
        {
            string def = m.GetDefinition(parentPath);
            switch (m.Type)
            {
                case MemberType.Variable:
                case MemberType.Constant:
                    l.Add(new ApiDefinition(parentPath + m.Name, def, parent, m));
                    break;
                case MemberType.Function:
                    l.Add(new ApiDefinition(parentPath + m.Name, def, parent, m));
                    //AddFunctionParameters(l, parent, m, fileName, def);
                    break;
                case MemberType.Constructor:
                    l.Add(new ApiDefinition(parentPath + "<constructor>", def, parent, m));                    
                    //AddFunctionParameters(l, parent, m, fileName, def);
                    break;
                case MemberType.Destructor:
                    l.Add(new ApiDefinition(parentPath + "<destructor>", def, parent, m)); 
                    break;
            }
        }
        private static void AddApiDefinitionModule(List<ApiDefinition> l, Module m)
        {
            l.Add(new ApiDefinition(m.Name, m.Name, m, null));

            foreach (string key in m.Members.Keys)
            {
                AddApiDefinitionMember(l, m.Members[key], m.Name + ".", m);
                if (m.Members[key].Overrides != null)
                {
                    foreach (Member mm in m.Members[key].Overrides)
                        AddApiDefinitionMember(l, mm, m.Name + ".", m);
                }
            }
        }
        private static void UpdateTranslations(Module m,Dictionary<string,string> translations,string parentPath)
        {
            translations[(parentPath + m.Name).ToLower()] = m.Name;
            foreach (string key in m.Modules.Keys)
                UpdateTranslations(m.Modules[key], translations, parentPath + m.Name + ".");  
        }
        public static void GetApiDefinitions(List<ApiDefinition> l, Dictionary<string,string> translations)
        {
            GACFile system = GACParser.Compiler.GetGACFile("");
            foreach (string moduleName in system.Modules.Keys)
            {
                AddApiDefinitionModule(l, system.Modules[moduleName]);
                UpdateTranslations(system.Modules[moduleName], translations, "");
            }
        
        }

        public class Intellisense
        {
            public enum InfoType
            {
                None,
                VariableOrParameter,
                Images,
                Sounds,
                Shaders,
                Fonts,
                Strings,
                Scenes,
                Global,
            }
            private static Dictionary<string, InfoType> intelliSenseInfoTypes = new Dictionary<string,InfoType>()
            {
                {"System.Resources.Images",InfoType.Images},
                {"Resources.Images",InfoType.Images},
                {"Images",InfoType.Images},

                {"System.Resources.Sounds",InfoType.Sounds},
                {"Resources.Sounds",InfoType.Sounds},
                {"Sounds",InfoType.Sounds},

                {"System.Resources.Shaders",InfoType.Shaders},
                {"Resources.Shaders",InfoType.Shaders},
                {"Shaders",InfoType.Shaders},

                {"System.Resources.Fonts",InfoType.Fonts},
                {"Resources.Fonts",InfoType.Fonts},
                {"Fonts",InfoType.Fonts},

                {"System.Resources.Strings",InfoType.Strings},
                {"Resources.Strings",InfoType.Strings},
                {"Strings",InfoType.Strings},

                {"System.UI.Scenes",InfoType.Scenes},
                {"UI.Scenes",InfoType.Scenes},
                {"Scenes",InfoType.Scenes},
                
            };
            public class Info
            {
                public Token token;
                public int functionParamOrder; // -1 daca nu este un parametru al unei functii
                public Info function;
                public string prototype;
                public Module module;
                public LocalDefinition localDef;
                public InfoType Type;
                public void Clear()
                {
                    token = null;
                    functionParamOrder = -1;
                    function = null;
                    prototype = "";
                    module = null;
                    Type = InfoType.None;
                }
            }
            private GACParser p;
            private Info []inf;
            private int infCount;
            public Info CurrentWordInfo;
            public Intellisense()
            {
                p = new GACParser();
                inf = new Info[1000];
                for (int tr = 0; tr < inf.Length; tr++)
                {
                    inf[tr] = new Info();
                    inf[tr].localDef = new LocalDefinition("","","","",0,0,0, LocalDefinitionType.Member);
                    inf[tr].Clear();
                }
                infCount = 0;
            }
            private int Process(Info function,int start,int end,Location location)
            {
                int functionParamOrder = 0;
                if (end < 0)
                    end = infCount;
                string prototype = "";
                string toAdd;
                Module lastModule = null;
                Member m = null;
                bool resetPrototype;
                bool clearModuleAndPrototype;
                for (int tr = start; tr < end; tr++)
                {
                    Token t = p.Tokens[tr];
                    Info i = inf[tr];
                    resetPrototype = false;
                    clearModuleAndPrototype = false;
                    i.token = t;
                    i.function = function;
                    i.Type = InfoType.None;
                    if (function != null)
                        i.functionParamOrder = functionParamOrder;
                    else
                        i.functionParamOrder = -1;
                    i.module = t.Obj;
                    toAdd = null;
                    if (i.module != null)
                        toAdd = i.module.Name;
                    switch (t.Type)
                    {
                        case TokenType.Column: 
                            functionParamOrder++;
                            break;
                        case TokenType.RoundBreketOpen:
                            i.prototype = "";
                            if ((tr > 0) && ((p.Tokens[tr - 1].Type == TokenType.Word) || (p.Tokens[tr - 1].Type == TokenType.TypeClass)))
                            {
                                i.function = inf[tr - 1];
                                i.functionParamOrder = 0;
                                tr = Process(inf[tr - 1], tr + 1, t.Link, location);                               
                            }
                            else
                            {
                                tr = Process(null, tr + 1, t.Link, location);
                            }
                            t = p.Tokens[tr];
                            i = inf[tr];
                            i.token = t;
                            i.function = null;
                            i.Type = InfoType.None;
                            i.functionParamOrder = -1;
                            i.module = lastModule;
                            toAdd = ""; // nu NULL ci "" ca sa se poata adauga dar sa nu modifice ceva
                            clearModuleAndPrototype = true;
                            break;
                        case TokenType.SquareBreketOpen:
                            i.prototype = "";
                            tr = Process(null, tr + 1, t.Link,location);
                            t = p.Tokens[tr];
                            i = inf[tr];
                            i.token = t;
                            i.function = function;
                            i.Type = InfoType.None;
                            if (function != null)
                                i.functionParamOrder = functionParamOrder;
                            else
                                i.functionParamOrder = -1;
                            i.module = lastModule;
                            toAdd = ""; // nu NULL ci "" ca sa se poata adauga dar sa nu modifice ceva
                            clearModuleAndPrototype = true;
                            break;
                        case TokenType.TypeClass:
                            // caz de constructor
                            if ((tr+1 < end) && (toAdd!=null) && (p.Tokens[tr+1].Type == TokenType.RoundBreketOpen))
                            {
                                toAdd += ".<constructor>";
                            }
                            break;
                        case TokenType.BreketOpen:
                            i.prototype = "";
                            tr = Process(null, tr + 1, t.Link,location)-1;
                            break;
                        case TokenType.Point:
                            i.module = lastModule;
                            toAdd = ".";
                            break;
                        case TokenType.This:
                            i.module = location.module;
                            if (i.module != null)
                                toAdd = i.module.Name;
                            break;
                        case TokenType.Word:                            
                            if (lastModule == null)
                            {
                                // am urmatoarele alternative - fie e o variabila a clasei, fie e o variabilaa locala, fie e un parametru
                                if (location.member!=null)
                                {
                                    m = location.member.GetLocalVariableOrParameter(t.Text);
                                    if (m != null)
                                    {
                                        // variabila locala sau argument
                                        prototype = location.module.Name + "." + location.member.Name + "=>";
                                        toAdd = t.Text;
                                        lastModule = Compiler.GetModule(m.DataType);
                                        if (lastModule != null)
                                            i.module = lastModule;
                                        i.Type = InfoType.VariableOrParameter;
                                        i.localDef.LineNumber = m.FileLine;
                                        i.localDef.Start = m.FileStartPos;
                                        i.localDef.End = m.FileEndPos;
                                        i.localDef.File = location.module.FileName;
                                        resetPrototype = true;
                                    }
                                    else
                                    {
                                        // meembru sau metoda
                                        m = location.module.GetMember(t.Text);
                                        if (m != null)
                                        {
                                            prototype = location.module.Name + ".";
                                            toAdd = t.Text;
                                            lastModule = Compiler.GetModule(m.DataType);
                                            if (lastModule != null)
                                                i.module = lastModule;
                                            resetPrototype = true;
                                        }
                                    }
                                }
                            }
                            else 
                            {
                                m = lastModule.GetMember(t.Text);
                                if (m != null)
                                {
                                    lastModule = Compiler.GetModule(m.DataType);
                                    toAdd = t.Text;
                                    if (lastModule != null)    
                                        i.module = lastModule;
                                    resetPrototype = true;
                                }
                            }
                            break;
                    }
                    if (toAdd != null)
                        prototype += toAdd;
                    else
                        prototype = "";
                    
                    i.prototype = prototype;
                    lastModule = i.module;
                    if ((location.CursorPositionInLine >= t.Start) && (location.CursorPositionInLine < t.End))
                        CurrentWordInfo = i;
                    if (resetPrototype)
                    {
                        if (lastModule != null)
                            prototype = lastModule.Name;
                        else
                            prototype = "";
                    }
                    if (clearModuleAndPrototype)
                    {
                        i.module = null;
                        i.prototype = "";
                    }
                }
                return end;
            }
            private void AnalyzeCurrentWordType()
            {
                if (CurrentWordInfo == null)
                    return;
                if ((CurrentWordInfo.module != null) && (CurrentWordInfo.module.Name == "Global"))
                {
                    CurrentWordInfo.localDef.CreateFromModule(CurrentWordInfo.module, LocalDefinitionType.Scene, "Global");
                    CurrentWordInfo.Type = InfoType.Global;
                    CurrentWordInfo.prototype = "Global";
                    return;
                }
                if (CurrentWordInfo.prototype.Length == 0)
                    return;
                int idx = CurrentWordInfo.prototype.LastIndexOf('.');
                if (idx < 0)
                    return;
                string mainClasss = CurrentWordInfo.prototype.Substring(0, idx);
                InfoType itype = InfoType.None;
                if (intelliSenseInfoTypes.TryGetValue(mainClasss, out itype))
                    CurrentWordInfo.Type = itype;
                if (CurrentWordInfo.Type == InfoType.Scenes)
                {
                    CurrentWordInfo.localDef.CreateFromModule(CurrentWordInfo.module, LocalDefinitionType.Scene, CurrentWordInfo.prototype);
                }
            }
            public void Analize(Location location)
            {
                // curat pe cei deja existenti
                for (int tr = 0; tr < infCount; tr++)
                    inf[tr].Clear();
                CurrentWordInfo = null;
                p.QuickParse(location.Line);
                infCount = p.tokensCount;
                if (infCount > inf.Length)
                    infCount = inf.Length;
                Process(null, 0, infCount,location);
                AnalyzeCurrentWordType();
            }

        }
        #endregion
    }
}
