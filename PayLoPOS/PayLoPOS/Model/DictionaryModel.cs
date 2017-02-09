using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayLoPOS.Model
{
    public class DictionaryModelObject
    {
        public string key { get; set; }
        public string value { get; set; }
    }

    class DictionaryModel
    {
        private List<DictionaryModelObject> list = new List<DictionaryModelObject>();

        public void add(string key, string value)
        {
            list.Add(new DictionaryModelObject { key = key, value = value });
        }

        public string get(string key)
        {
            foreach(var pair in list)
            {
                if(pair.key == key)
                {
                    return pair.value;
                }
            }
            return "";
        }

        public string getStringUri()
        {
            string uri = "";
            foreach(var pair in list)
            {
                uri += pair.key + "=" + pair.value + "&";
            }
            return uri;
        }

        public string getJSON()
        {
            string json = "{";
            foreach (var pair in list)
            {
                json += "\"" + pair.key + "\":\"" + pair.value + "\",";
            }
            json = json.TrimEnd(',');
            return json + "}";
        }
    }
}
