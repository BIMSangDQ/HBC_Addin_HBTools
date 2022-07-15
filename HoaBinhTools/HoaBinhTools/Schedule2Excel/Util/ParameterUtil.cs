using System;
using System.IO;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;

namespace Schedule2Excel2k16.Util
{
	public class ParameterUtil
	{
		public static bool getParameter(Element pEle, String pParamName, out int pVal)
		{
			Parameter p = pEle.LookupParameter(pParamName);

			if (p == null)
			{
				ElementType type = pEle.Document.GetElement(pEle.GetTypeId()) as ElementType;
				p = type.LookupParameter(pParamName);
			}

			pVal = 0;
			if (p != null)
			{
				pVal = p.AsInteger();
				return true;
			}


			return false;

		}

		public static bool getParameter(Element pEle, String pParamName, out double pVal)
		{
			Parameter p = pEle.LookupParameter(pParamName);

			if (p == null)
			{
				ElementType type = pEle.Document.GetElement(pEle.GetTypeId()) as ElementType;
				p = type.LookupParameter(pParamName);
			}

			pVal = 0;
			if (p != null)
			{
				pVal = p.AsDouble();
				return true;
			}

			return false;

		}

		public static bool getParameter(Element pEle, String pParamName, out String pVal, bool pIsStringValue = false)
		{
			Parameter p = pEle.LookupParameter(pParamName);

			if (p == null)
			{
				ElementType type = pEle.Document.GetElement(pEle.GetTypeId()) as ElementType;
				p = type.LookupParameter(pParamName);
			}

			pVal = "";
			if (p != null)
			{
				pVal = p.AsString();

				if (pIsStringValue)
				{
					pVal = p.AsValueString();
				}
				return true;
			}

			return false;

		}


		public static bool getParameter(Element pEle, BuiltInParameter pParamId, out int pVal)
		{
			Parameter p = pEle.get_Parameter(pParamId);

			if (p == null)
			{
				ElementType type = pEle.Document.GetElement(pEle.GetTypeId()) as ElementType;
				p = type.get_Parameter(pParamId);
			}

			pVal = 0;
			if (p != null)
			{
				pVal = p.AsInteger();
				return true;
			}

			return false;

		}

		public static bool getParameter(Element pEle, BuiltInParameter pParamId, out double pVal)
		{
			Parameter p = pEle.get_Parameter(pParamId);

			if (p == null)
			{
				ElementType type = pEle.Document.GetElement(pEle.GetTypeId()) as ElementType;
				p = type.get_Parameter(pParamId);
			}

			pVal = 0;
			if (p != null)
			{
				pVal = p.AsDouble();
				return true;
			}

			return false;

		}

		public static bool getParameter(Element pEle, BuiltInParameter pParamId, out String pVal, bool pIsStringValue = false)
		{
			Parameter p = pEle.get_Parameter(pParamId);

			if (p == null)
			{
				ElementType type = pEle.Document.GetElement(pEle.GetTypeId()) as ElementType;
				p = type.get_Parameter(pParamId);
			}

			pVal = "";
			if (p != null)
			{
				pVal = p.AsString();

				if (pIsStringValue)
				{
					pVal = p.AsValueString();
				}
				return true;
			}

			return false;

		}

		public static bool setParameter(Element pEle, String pParamName, int pVal)
		{
			Parameter p = pEle.LookupParameter(pParamName);
			if (p == null)
			{
				ElementType type = pEle.Document.GetElement(pEle.GetTypeId()) as ElementType;
				p = type.LookupParameter(pParamName);

				if (p == null)
					return false;
			}

			if (p.IsReadOnly)
				return false;

			StorageType parameterType = p.StorageType;
			if (parameterType != StorageType.Integer)
				return false;

			p.Set(pVal);

			return true;
		}

		public static bool setParameter(Element pEle, String pParamName, double pVal)
		{
			Parameter p = pEle.LookupParameter(pParamName);
			if (p == null)
			{
				ElementType type = pEle.Document.GetElement(pEle.GetTypeId()) as ElementType;
				p = type.LookupParameter(pParamName);

				if (p == null)
					return false;
			}

			if (p.IsReadOnly)
				return false;

			StorageType parameterType = p.StorageType;
			if (parameterType != StorageType.Double)
				return false;

			p.Set(pVal);

			return true;
		}

		public static bool setParameter(Element pEle, String pParamName, String pVal)
		{
			Parameter p = pEle.LookupParameter(pParamName);
			if (p == null)
			{
				ElementType type = pEle.Document.GetElement(pEle.GetTypeId()) as ElementType;
				p = type.LookupParameter(pParamName);

				if (p == null)
					return false;
			}

			if (p.IsReadOnly)
				return false;

			StorageType parameterType = p.StorageType;
			if (parameterType != StorageType.String)
				return false;

			p.Set(pVal);

			return true;
		}

		public static void createOrUpdateCustomParameter(Application pApp, Document pDoc, Element pEle, String pParamName, String pDesc, ParameterType pParamType, BuiltInParameterGroup pBuiltInParamGroup, bool pVisible = true, bool pModifiable = false)
		{
			Parameter shapeIdParam = pEle.LookupParameter(pParamName);
			if (shapeIdParam != null)
				return;

			String strFileTemp = Path.Combine(Path.GetTempPath(), "temp" + "_" + GlobalVar.SHARED_PARAM_FILE_NAME + GlobalVar.SHARED_PARAM_FILE_EXT);
			String previousFile = pApp.SharedParametersFilename;
			using (File.Create(strFileTemp)) { }
			pApp.SharedParametersFilename = strFileTemp;

			DefinitionFile sharedFile = pApp.OpenSharedParameterFile();
			Definition myDefinition = null;

			// create a new group in the shared parameters file
			DefinitionGroups myGroups = sharedFile.Groups;
			DefinitionGroup myGroup = myGroups.Create(pParamName);

			ExternalDefinitionCreationOptions options = new ExternalDefinitionCreationOptions(pParamName, pParamType);
			options.Visible = pVisible;
			options.UserModifiable = pModifiable;
			options.Description = pDesc;

			myDefinition = myGroup.Definitions.Create(options);

			// create a category set and insert category of rebar to it
			CategorySet myCategorySet = pApp.Create.NewCategorySet();
			// use BuiltInCategory to get category of wall
			//Category myCategory = Category.GetCategory(pDoc, BuiltInCategory.OST_Stairs);
			//myCategories.Insert(myCategory);
			myCategorySet.Insert(pEle.Category);

			//Create an instance of InstanceBinding
			InstanceBinding instanceBinding = pApp.Create.NewInstanceBinding(myCategorySet);

			// Get the BingdingMap of current document.
			BindingMap bindingMap = pDoc.ParameterBindings;

			bool instanceBindOK = bindingMap.Insert(myDefinition,
											instanceBinding, pBuiltInParamGroup);
			File.Delete(strFileTemp);

			pApp.SharedParametersFilename = previousFile;
		}


	}
}
