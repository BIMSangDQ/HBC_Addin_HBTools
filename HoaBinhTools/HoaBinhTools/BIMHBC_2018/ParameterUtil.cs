using System;
using System.Collections.Generic;
using System.IO;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;

namespace Geometry
{
	public class ParameterUtil
	{
		public static void AddParameter(Document doc, string name, AddParameterType addType, ParameterType type, BuiltInParameterGroup parameterGroup, params BuiltInCategory[] builtInCategories)
		{
			Application app = doc.Application;
			FilteredElementCollector col = new FilteredElementCollector(doc);
			switch (addType)
			{
				case AddParameterType.Instance: col.WhereElementIsNotElementType(); break;
				case AddParameterType.Type: col.WhereElementIsElementType(); break;
			}
			CategorySet cates = null;
			ICollection<Element> elems = col.ToElements();
			Definition def = null;
			foreach (Element e in elems)
			{
				foreach (Parameter p in e.Parameters)
				{
					try
					{
						if (p.Definition.Name == name)
						{
							def = p.Definition; goto L1;
						}
					}
					catch { }
				}
			}
			L1:
			if (def != null)
			{
				switch (addType)
				{
					case AddParameterType.Instance:
						InstanceBinding ib = doc.ParameterBindings.get_Item(def) as InstanceBinding;
						if (ib == null) throw new Exception("Error when retrieve instance binding!");
						cates = ib.Categories; break;
					case AddParameterType.Type:
						TypeBinding tb = doc.ParameterBindings.get_Item(def) as TypeBinding;
						if (tb == null) throw new Exception("Error when retrieve type binding!");
						cates = tb.Categories; break;
				}
				if (cates == null) throw new Exception("Error when retrieve categories!");
				foreach (BuiltInCategory bic in builtInCategories)
				{
					Category c = Category.GetCategory(doc, bic);
					if (!cates.Contains(c))
					{
						cates.Insert(c);
					}
				}
				switch (addType)
				{
					case AddParameterType.Instance:
						doc.ParameterBindings.ReInsert(def, app.Create.NewInstanceBinding(cates), def.ParameterGroup);
						return;
					case AddParameterType.Type:
						doc.ParameterBindings.ReInsert(def, app.Create.NewTypeBinding(cates), def.ParameterGroup);
						return;
				}
				throw new Exception("Code complier should never be here!");
			}

			string strFileTemp = Path.Combine(Path.GetTempPath(), "temp" + "_AssemblyParameter.txt");
			using (File.Create(strFileTemp)) { }
			app.SharedParametersFilename = strFileTemp;
			DefinitionFile sharedFile = app.OpenSharedParameterFile();
			DefinitionGroups defGroups = sharedFile.Groups;
			DefinitionGroup defGroup = defGroups.Create(name);
			def = defGroup.Definitions.Create(new ExternalDefinitionCreationOptions(name, type) { Visible = true, UserModifiable = true, Description = name });
			cates = app.Create.NewCategorySet();
			foreach (BuiltInCategory bic in builtInCategories)
			{
				Category c = Category.GetCategory(doc, bic);
				cates.Insert(c);
			}
			switch (addType)
			{
				case AddParameterType.Instance:
					doc.ParameterBindings.Insert(def, app.Create.NewInstanceBinding(cates), parameterGroup);
					return;
				case AddParameterType.Type:
					doc.ParameterBindings.Insert(def, app.Create.NewTypeBinding(cates), parameterGroup);
					return;
			}
			File.Delete(strFileTemp);
		}
		public static void AddParameter(Document doc, string name, AddParameterType addType, ParameterType type, BuiltInParameterGroup parameterGroup, List<Category> categories)
		{
			Application app = doc.Application;
			FilteredElementCollector col = new FilteredElementCollector(doc).WhereElementIsNotElementType();
			CategorySet cates = null;
			ICollection<Element> elems = col.ToElements();
			Definition def = null;
			foreach (Element e in elems)
			{
				foreach (Parameter p in e.Parameters)
				{
					try
					{
						if (p.Definition.Name == name)
						{
							def = p.Definition; goto L1;
						}
					}
					catch { }
				}
			}
			L1:
			if (def != null)
			{
				switch (addType)
				{
					case AddParameterType.Instance:
						InstanceBinding ib = doc.ParameterBindings.get_Item(def) as InstanceBinding;
						if (ib == null) throw new Exception("Error when retrieve instance binding!");
						cates = ib.Categories; break;
					case AddParameterType.Type:
						TypeBinding tb = doc.ParameterBindings.get_Item(def) as TypeBinding;
						if (tb == null) throw new Exception("Error when retrieve type binding!");
						cates = tb.Categories; break;
				}
				if (cates == null) throw new Exception("Error when retrieve categories!");
				foreach (Category c in categories)
				{
					if (!cates.Contains(c))
					{
						cates.Insert(c);
					}
				}
				switch (addType)
				{
					case AddParameterType.Instance:
						doc.ParameterBindings.ReInsert(def, app.Create.NewInstanceBinding(cates), parameterGroup);
						return;
					case AddParameterType.Type:
						doc.ParameterBindings.ReInsert(def, app.Create.NewTypeBinding(cates), parameterGroup);
						return;
				}
				throw new Exception("Code complier should never be here!");
			}

			string strFileTemp = Path.Combine(Path.GetTempPath(), "temp" + "_AssemblyParameter.txt");
			using (File.Create(strFileTemp)) { }
			app.SharedParametersFilename = strFileTemp;
			DefinitionFile sharedFile = app.OpenSharedParameterFile();
			DefinitionGroups defGroups = sharedFile.Groups;
			DefinitionGroup defGroup = defGroups.Create(name);
			def = defGroup.Definitions.Create(new ExternalDefinitionCreationOptions(name, type) { Visible = true, UserModifiable = true, Description = name });
			cates = app.Create.NewCategorySet();
			foreach (Category c in categories)
			{
				cates.Insert(c);
			}
			switch (addType)
			{
				case AddParameterType.Instance:
					doc.ParameterBindings.Insert(def, app.Create.NewInstanceBinding(cates), parameterGroup);
					return;
				case AddParameterType.Type:
					doc.ParameterBindings.Insert(def, app.Create.NewTypeBinding(cates), parameterGroup);
					return;
			}
			File.Delete(strFileTemp);
		}
		public static void AddParameter(Document doc, string name, AddParameterType addType, ParameterType type, BuiltInParameterGroup parameterGroup, CategorySet categories)
		{
			Application app = doc.Application;
			FilteredElementCollector col = new FilteredElementCollector(doc).WhereElementIsNotElementType();
			CategorySet cates = null;
			ICollection<Element> elems = col.ToElements();
			Definition def = null;
			foreach (Element e in elems)
			{
				foreach (Parameter p in e.Parameters)
				{
					try
					{
						if (p.Definition.Name == name)
						{
							def = p.Definition; goto L1;
						}
					}
					catch { }
				}
			}
			L1:
			if (def != null)
			{
				switch (addType)
				{
					case AddParameterType.Instance:
						InstanceBinding ib = doc.ParameterBindings.get_Item(def) as InstanceBinding;
						if (ib == null) throw new Exception("Error when retrieve instance binding!");
						cates = ib.Categories; break;
					case AddParameterType.Type:
						TypeBinding tb = doc.ParameterBindings.get_Item(def) as TypeBinding;
						if (tb == null) throw new Exception("Error when retrieve type binding!");
						cates = tb.Categories; break;
				}
				if (cates == null) throw new Exception("Error when retrieve categories!");
				foreach (Category c in categories)
				{
					if (!cates.Contains(c))
					{
						cates.Insert(c);
					}
				}
				switch (addType)
				{
					case AddParameterType.Instance:
						doc.ParameterBindings.ReInsert(def, app.Create.NewInstanceBinding(cates), parameterGroup);
						return;
					case AddParameterType.Type:
						doc.ParameterBindings.ReInsert(def, app.Create.NewTypeBinding(cates), parameterGroup);
						return;
				}
				throw new Exception("Code complier should never be here!");
			}

			string strFileTemp = Path.Combine(Path.GetTempPath(), "temp" + "_AssemblyParameter.txt");
			using (File.Create(strFileTemp)) { }
			app.SharedParametersFilename = strFileTemp;
			DefinitionFile sharedFile = app.OpenSharedParameterFile();
			DefinitionGroups defGroups = sharedFile.Groups;
			DefinitionGroup defGroup = defGroups.Create(name);
			def = defGroup.Definitions.Create(new ExternalDefinitionCreationOptions(name, type) { Visible = true, UserModifiable = true, Description = name });
			cates = app.Create.NewCategorySet();
			foreach (Category c in categories)
			{
				cates.Insert(c);
			}
			switch (addType)
			{
				case AddParameterType.Instance:
					doc.ParameterBindings.Insert(def, app.Create.NewInstanceBinding(cates), parameterGroup);
					return;
				case AddParameterType.Type:
					doc.ParameterBindings.Insert(def, app.Create.NewTypeBinding(cates), parameterGroup);
					return;
			}
			File.Delete(strFileTemp);
		}
	}
	public enum AddParameterType
	{
		Instance, Type
	}
}
