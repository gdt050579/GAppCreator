using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAppCreator
{
    public class TemplateCodeGenerator
    {
        public class Item
        {
            public string Name = "";
            public string Description = "";
            public bool Use = false;
        }
        private string Code = "";
        public Dictionary<string, Item> Items = new Dictionary<string, Item>();
        public Dictionary<string, string> Translates = new Dictionary<string, string>();
        public void Init(string code)
        {
            Item i = null;
            Items.Clear();
            Translates.Clear();
            Code = code;
            // parcurg linie cu linie si imi scot datele
            foreach (string line in code.Split('\n'))
            {                
                if ((line.StartsWith("###")) && (line.Contains(':')))
                {
                    string key, value;
                    key = line.Substring(3, line.IndexOf(':') - 3).ToLower().Trim();
                    value = line.Substring(line.IndexOf(':') + 1).Trim();
                    if (i == null)
                        i = new Item();
                    switch (key)
                    {
                        case "item": i.Name = value; break;
                        case "description": i.Description = value; break;
                        case "use": i.Use = value.ToLower().StartsWith("t"); break;
                    }
                }
                if ((line.Trim().Equals("###")) && (i!=null))
                {
                    if (i.Name.Length > 0)
                        Items[i.Name] = i;
                    i = null;
                }
            }
        }
        public string CreateCode()
        {
            string s = "";
            bool add = true;
            foreach (string line in Code.Split('\n'))
            {
                if (line.ToLower().StartsWith("###item:"))
                {
                    add = Items[line.Substring(line.IndexOf(':') + 1).Trim()].Use;
                    continue;
                }
                if (line.Trim().Equals("###"))
                    add = true;
                if (line.StartsWith("###"))
                    continue;
                if (add)
                    s += line + "\n";
            }
            // fac si inlocuirile
            foreach (string ss in Translates.Keys)
                s = s.Replace(ss, Translates[ss]);
            return s;
        }
        public List<string> GetItems()
        {
            List<string> l = new List<string>();
            foreach (string s in Items.Keys)
                l.Add(s);
            l.Sort();
            return l;
        }
    }
}
