#region Using
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks; 
#endregion

namespace BeyCons.Core.Extensions
{
    public class ClassUtils
    {
        public static List<string> GetSubClassNameInheritedFromParentClass(Type type, string fullName = "RevitAPI, Version=19.0.0.0, Culture=neutral, PublicKeyToken=null")
        {
            Assembly assemblie = AppDomain.CurrentDomain.GetAssemblies().Where(x => x.FullName == fullName).First();
            Type[] types = assemblie.GetTypes();
            IEnumerable<Type> subclasses = types.Where(t => t.IsSubclassOf(type));
            return subclasses.Select(x => x.Name).ToList();
        }
    }
}