#region Using
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace BeyCons.Core.Utils
{
    public class JsonUtils<T> where T : new()
    {
        public string FullPath { get; set; }
        public JsonUtils(string fullPath)
        {
            FullPath = fullPath;
        }
        public void WriteJson(object obj)
        {
            if (!Directory.Exists(Path.GetDirectoryName(FullPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(FullPath));
            }
            if (File.Exists(FullPath))
            {
                new FileInfo(FullPath) { IsReadOnly = false };
                File.Delete(FullPath);
            }
            string json = JsonConvert.SerializeObject(obj, Formatting.Indented);
            File.WriteAllText(FullPath, json);
        }
        public T ReadJson()
        {
            if (File.Exists(FullPath))
            {
                string json = File.ReadAllText(FullPath);
                return JsonConvert.DeserializeObject<T>(json);
            }
            else
            {
                return new T();
            }
        }
    }
}