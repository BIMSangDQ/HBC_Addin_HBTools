#region Namespaces

using System;
using System.Collections.Generic;
using System.Windows;
using Autodesk.Revit.DB;
using Application = Autodesk.Revit.ApplicationServices.Application;
using String = System.String;

#endregion

namespace HoaBinhTools.AutocadToRevit.Utils
{
	internal static class ParameterUtils
	{
		/// <summary>
		/// Lấy về giá trị của parameter p
		/// </summary>
		/// <param name="p">Parameter cần lấy giá trị</param>
		/// <param name="asValueString">asValueString = true: Lấy giá trị parameter được hiện ra màn hình Revit</param>
		/// <returns></returns>
		internal static string GetValue(this Parameter p, bool asValueString = false)
		{
			if (p == null) return string.Empty;
			if (asValueString && p.StorageType != StorageType.String)
				return p.AsValueString();

			switch (p.StorageType)
			{
				case StorageType.Double:
					return p.AsDouble().ToString();

				case StorageType.ElementId:
					try
					{
						return p.AsElementId().IntegerValue.ToString();
					}
					catch (Exception e)
					{
						//MessageBox.Show(e.ToString());
						return string.Empty;
					}

				case StorageType.Integer:
					return p.AsInteger().ToString();

				case StorageType.String:
					return p.AsString();

				case StorageType.None:
					return p.AsValueString();
			}
			return String.Empty;
		}

		/// <summary>
		/// Gán giá trị cho parameter p
		/// </summary>
		/// <param name="p">Parameter cần xét giá trị.</param>
		/// <param name="value">Giá trị cần set cho parameter p.</param>
		internal static void SetValue(this Parameter p, object value)
		{
			// Cannot edit readonly
			if (p.IsReadOnly || value == null) return;

			switch (p.StorageType)
			{
				case StorageType.Double:
					p.Set(Convert.ToDouble(value));
					break;

				case StorageType.ElementId:
					if (value is string)
					{
						try
						{
							int id = Convert.ToInt32(value);
							p.Set(new ElementId(id));
							break;
						}
						catch (Exception e)
						{
							//MessageBox.Show(e.ToString());
						}
					}
					p.Set(value as ElementId);
					break;

				case StorageType.Integer:
					p.Set(Convert.ToInt32(value));
					break;

				case StorageType.String:
					p.Set(value.ToString());
					break;

				case StorageType.None:
					p.SetValueString(value as string);
					break;
			}
		}

		internal static List<string> GetAllParameters(Element el,
			bool isIncludeTypePara = true)
		{
			List<string> result = new List<string>();

			result.Add("ElementId");
			foreach (Parameter p in el.Parameters)
			{
				if (p.Definition == null) continue;
				result.Add(p.Definition.Name);
			}

			if (isIncludeTypePara)
			{
				Element elementType = el.Document.GetElement(el.GetTypeId());

				if (elementType != null)
					foreach (Parameter p in elementType.Parameters)
						result.Add(p.Definition.Name);
			}

			return result;
		}

		internal static List<string> GetAllParametersEditable(Element el)
		{
			List<string> result = new List<string>();
			foreach (Parameter p in el.Parameters)
			{
				if (!p.IsReadOnly)
					result.Add(p.Definition.Name);
			}
			return result;
		}

		#region Shared Parameter

		internal static DefinitionFile GetSharedParamsFile(Application app,
			string sharedParamsPath)
		{
			if (string.IsNullOrEmpty(sharedParamsPath))
			{
				MessageBox.Show("No shared params file be selected");
				return null;
			}

			app.SharedParametersFilename = sharedParamsPath;

			// Lấy về file Shared Parameter
			DefinitionFile sharedParametersFile;
			try
			{
				sharedParametersFile = app.OpenSharedParameterFile();
			}
			catch (Exception ex)
			{
				MessageBox.Show("Can't open shared params file:" + ex.Message, "Error");
				sharedParametersFile = null;
			}
			return sharedParametersFile;
		}

		internal static DefinitionGroup GetOrCreateSharedParamsGroup(DefinitionFile sharedParametersFile, string groupName)
		{
			DefinitionGroup g = sharedParametersFile.Groups.get_Item(groupName);
			if (null == g)
			{
				try
				{
					g = sharedParametersFile.Groups.Create(groupName);
				}
				catch (Exception)
				{
					g = null;
				}
			}
			return g;
		}

		internal static Definition GetOrCreateSharedParamsDefinition(DefinitionGroup defGroup,
			string nameOfParameter, ParameterType defType, string description,
			bool visible = true, bool userModifiable = true)
		{
			Definition definition = defGroup.Definitions.get_Item(nameOfParameter);
			if (null == definition)
			{
				try
				{
					ExternalDefinitionCreationOptions opt
						= new ExternalDefinitionCreationOptions(nameOfParameter, defType);

					// True if the parameter is visible to the user, 
					// false if it is hidden and accessible only via the API. The default is true. 
					opt.Visible = visible;
					opt.UserModifiable = userModifiable;
					opt.Description = description;
					definition = defGroup.Definitions.Create(opt);
				}
				catch (Exception)
				{
					definition = null;
				}
			}

			return definition;
		}

		internal static Guid SharedParamGUID(Application app, string defGroup, string defName)
		{
			Guid guid = Guid.Empty;
			try
			{
				DefinitionFile file = app.OpenSharedParameterFile();
				DefinitionGroup group = file.Groups.get_Item(defGroup);
				Definition definition = group.Definitions.get_Item(defName);
				ExternalDefinition externalDefinition = definition as ExternalDefinition;
				guid = externalDefinition.GUID;
			}
			catch (Exception)
			{
			}
			return guid;
		}

		internal static void CreateInstanceBindingParameter(Document doc, List<Category> categories,
			Definition paramDef,
			BuiltInParameterGroup displayGroup)
		{
			// Create the category set for binding and add the category
			CategorySet catSet = doc.Application.Create.NewCategorySet();
			foreach (Category cat in categories)
			{
				if (!cat.AllowsBoundParameters) continue;
				catSet.Insert(cat);
			}

			using (SubTransaction trans = new SubTransaction(doc))
			{
				trans.Start();
				// Bind the param
				try
				{
					Binding binding = doc.Application.Create.NewInstanceBinding(catSet);
					doc.ParameterBindings.Insert(paramDef, binding, displayGroup);
					trans.Commit();
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message);
					trans.RollBack();
				}
			}
		}

		internal static void CreateTypeBindingParameter(Document doc, List<Category> categories,
			Definition paramDef,
			BuiltInParameterGroup displayGroup)
		{
			// Create the category set for binding and add the category
			CategorySet catSet = doc.Application.Create.NewCategorySet();
			foreach (Category cat in categories)
			{
				if (!cat.AllowsBoundParameters) continue;
				catSet.Insert(cat);
			}

			using (SubTransaction trans = new SubTransaction(doc))
			{
				trans.Start();
				// Bind the param
				try
				{
					Binding binding = doc.Application.Create.NewTypeBinding(catSet);
					// We could check if already bound, but looks like Insert will just ignore it in such case
					doc.ParameterBindings.Insert(paramDef, binding, displayGroup);
					trans.Commit();
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message);
					trans.RollBack();
				}
			}
		}

		internal static void CreateSharedParamater(Document doc, Application app,
			string sharedParamsPath, string sharedParamsGroup,
			string nameOfParameter, ParameterType defType,
			BuiltInParameterGroup displayGroup, string description,
			List<Category> categories, bool isInstanceParameter = true,
			bool visible = true, bool userModifiable = true)
		{
			DefinitionFile sharedParameterFile = GetSharedParamsFile(app, sharedParamsPath);
			if (sharedParameterFile == null)
			{
				MessageBox.Show("Error to getting the shared parameter file.");
				return;
			}

			DefinitionGroup group
				= GetOrCreateSharedParamsGroup(sharedParameterFile, sharedParamsGroup);

			if (group == null)
			{
				MessageBox.Show("Error to getting the shared parameter group.");
				return;
			}

			Definition definition = GetOrCreateSharedParamsDefinition(group, nameOfParameter,
				defType, description, visible, userModifiable);

			if (isInstanceParameter)
			{
				CreateInstanceBindingParameter(doc, categories, definition, displayGroup);
			}
			else
			{
				CreateTypeBindingParameter(doc, categories, definition, displayGroup);
			}
		}

		#endregion Shared Parameter
	}
}
