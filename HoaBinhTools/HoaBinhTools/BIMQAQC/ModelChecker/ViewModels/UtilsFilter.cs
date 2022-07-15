using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using HoaBinhTools.AutocadToRevit.Utils;
using HoaBinhTools.BIMQAQC.ModelChecker.Models;
using HoaBinhTools.BIMQAQC.ModelChecker.Views;
using HoaBinhTools.BIMQAQC.ModelChecker.Views.ProgessBar;
using HoaBinhTools.ProjectWarnings.Models;
using HoaBinhTools.PurgeViewsV2.Models;
using HoaBinhTools.SynchronizedData.Db;
using Microsoft.Win32;
using Utils;

namespace HoaBinhTools.BIMQAQC.ModelChecker.ViewModels
{
	public class UtilsFilter
	{
		public const double EPSILON = 0.0001;

		public const int N = 6;
		#region Filter
		public List<ElementId> FilterByCategory(Document doc, string CategoryName)
		{
			List<Element> elements = new List<Element>();
			try
			{
				FilteredElementCollector finalCollector = new FilteredElementCollector(doc);

				Categories categories = doc.Settings.Categories;

				Category Category = null;
				foreach (Category category in categories)
				{
					if (category.Name == CategoryName)
					{
						Category = category;
						break;
					}
				}

				ElementCategoryFilter filter1 = new ElementCategoryFilter(Category.ToBuiltinCategory());

				finalCollector.WherePasses(filter1);

				elements = finalCollector.ToList<Element>();
			}
			catch { }

			return elements.Select(e => e.Id).ToList();
		}

		public List<ElementId> FilterFamilyInPlace(Document doc, bool Is_InPlace)
		{
			List<ElementId> elements = new List<ElementId>();

			try
			{
				if (Is_InPlace)
				{
					List<FamilyInstance> ListFamily = new FilteredElementCollector(doc).OfClass(typeof(FamilyInstance)).Cast<FamilyInstance>().ToList();

					ListFamily = ListFamily.Where(x => x.Symbol.Family.IsInPlace).ToList();
					//List<Family> families = ListFamily.Select(x => x.Symbol.Family).ToList();
					elements = ListFamily.Select(x => x.Id).ToList();
				}
				else
				{
					List<Family> ListFamily = new FilteredElementCollector(doc).OfClass(typeof(Family)).Cast<Family>().ToList();

					ListFamily = ListFamily.Where(x => x.IsInPlace == false).ToList();

					elements = ListFamily.Select(x => x.Id).ToList();
				}
			}
			catch { }
			return elements;
		}

		public List<ElementId> FilterFamilyName(Document doc, string FamilyName, string ConditionUser, List<ElementId> ListelementIds)
		{
			List<Element> elements = new List<Element>();
			List<FamilyInstance> FamilyInstance = new List<FamilyInstance>();
			List<Element> Listelements = new List<Element>();
			foreach (ElementId id in ListelementIds)
			{
				Listelements.Add(doc.GetElement(id));
			}
			Regex rg;

			switch (ConditionUser)
			{
				case "=":
					elements = Listelements.Where(x => GetFamilyName(x) == FamilyName).ToList();

					break;
				case "!=":
					elements = Listelements.Where(x => GetFamilyName(x) != FamilyName).ToList();

					break;
				case "Contains":
					elements = Listelements.Where(x => GetFamilyName(x).Contains(FamilyName)).ToList();

					break;
				case "Does Not Contain":
					elements = Listelements.Where(x => GetFamilyName(x).Contains(FamilyName)== false).ToList();

					break;
				case "Matches Regex": // Value là pattern
					rg = new Regex(FamilyName);

					elements = Listelements.Where(x => rg.Match(GetFamilyName(x)).Success).ToList();

					break;
				case "Does Not Match RegEx": // Value là pattern
					rg = new Regex(FamilyName);

					elements = Listelements.Where(x => rg.Match(GetFamilyName(x)).Success == false).ToList();

					break;
				default:
					break;
			}

			return elements.Select(e => e.Id).ToList();
		}

		public List<ElementId> FilterTypeOrInstance(Document doc, string Condition, string Value, List<ElementId> ListelementIds)
		{
			List<Element> elements = new List<Element>();
			List<ElementId> ListTypeID = new List<ElementId>();

			List<Element> Listelements = new List<Element>();
			foreach (ElementId id in ListelementIds)
			{
				Listelements.Add(doc.GetElement(id));
			}
			try
			{
				if (Condition == "!=")
				{
					FilteredElementCollector finalCollector = (FilteredElementCollector)new FilteredElementCollector(doc)
					.WhereElementIsNotElementType();

					elements = finalCollector.ToList<Element>();
					ListTypeID = elements.Select(e => e.Id).ToList();

					List<ElementId> ele = ListTypeID.Intersect(ListelementIds).ToList();
					return ele;
				}
				else if ((Condition == "="))
				{
					FilteredElementCollector finalCollector = (FilteredElementCollector)new FilteredElementCollector(doc)
					.WhereElementIsElementType();

					elements = finalCollector.ToList<Element>();
					ListTypeID = elements.Select(e => e.Id).ToList();

					List<ElementId> ele = ListTypeID.Intersect(ListelementIds).ToList();
					return ele;
				}
				else if ((Condition == "Type From Instance"))
				{
					FilteredElementCollector finalCollector = (FilteredElementCollector)new FilteredElementCollector(doc)
					.WhereElementIsNotElementType();

					elements = finalCollector.ToList<Element>();
					ListTypeID = elements.Select(e => e.Id).ToList();

					List<ElementId>ele = ListTypeID.Intersect(ListelementIds).ToList();
					elements = new List<Element>();

					foreach (ElementId id in ele)
					{
						elements.Add(doc.GetElement(id));
					}

					List<ElementId> Types = elements.Select(e => e.GetTypeId()).ToList();
					Types = Types.Distinct().ToList();

					return Types;
				}
			}
			catch { }

			return elements.Select(e => e.Id).ToList();
		}

		public List<ElementId> FilterType(Document doc, string Condition, string Value, List<ElementId> ListelementIds)
		{
			List<Element> elements = new List<Element>();
			List<Element> elementsType = new List<Element>();
			List<Element> Listelements = new List<Element>();
			foreach (ElementId id in ListelementIds)
			{
				Listelements.Add(doc.GetElement(id));
			}

			Regex rg;

			try
			{
				switch (Condition) 
				{
					case "=":
						elements = Listelements
						.Where(x => TypeName(doc, x) == Value).ToList();

						elementsType = Listelements
						.Where(x => x.Name == Value).ToList();
						break;
					case "!=":
						elements = Listelements
						.Where(x => TypeName(doc, x) != Value).ToList();

						elementsType = Listelements
						.Where(x => x.Name != Value).ToList();
						break;
					case "Contains":
						elements = Listelements
						.Where(x => TypeName(doc, x).Contains(Value)).ToList();

						elementsType = Listelements
						.Where(x => x.Name.Contains(Value)).ToList();
						break;
					case "Does Not Contain":
						elements = Listelements
						.Where(x => TypeName(doc, x).Contains(Value) == false).ToList();

						elementsType = Listelements
						.Where(x => x.Name.Contains(Value) == false).ToList();
						break;
					case "Matches Regex":
						rg = new Regex(Value);

						elements = Listelements.Where(x => rg.Match(TypeName(doc, x)).Success == true).ToList();

						elementsType = Listelements.Where(x => rg.Match(x.Name).Success == true).ToList();
						break;
					case "Does Not Match RegEx":
						rg = new Regex(Value);

						elements = Listelements.Where(x => rg.Match(TypeName(doc, x)).Success == false).ToList();

						elementsType = Listelements.Where(x => rg.Match(x.Name).Success == false).ToList();
						break;
					default:
						break;
				}
			}
			catch
			{ }

			elements = elements.Union(elementsType).ToList();
			return elements.Select(e => e.Id).ToList();
		}

		public List<ElementId> FilterParameter(Document doc, string Property, string Condition, string Value, string ValueType, List<ElementId> ListelementIds, List<string> Ids,string IdCheck)
		{
			//
			List<Element> elements = new List<Element>();
			List<Element> Listelements = new List<Element>();
			foreach (ElementId id in ListelementIds)
			{
				Listelements.Add(doc.GetElement(id));
			}
			DbConnect db = new DbConnect();
			Regex rg;

			try
			{
				if (ValueType == "Double") // Kiểm tra đổi đơn vị
				{
					if (Condition == "=")
					{
						elements = Listelements.Where(x => x.LookupParameter(Property) != null
						&& Math.Round(double.Parse(LookupParameterValue(x, Property, "Number")), 0) == double.Parse(Value)).ToList();
					}
					else if (Condition == "!=")
					{
						elements = Listelements.Where(x => x.LookupParameter(Property) != null
						&& Math.Round(double.Parse(LookupParameterValue(x, Property, "Number")), 0) != double.Parse(Value)).ToList();
					}
					else if (Condition == ">=")
					{
						elements = Listelements.Where(x => x.LookupParameter(Property) != null
						&& Math.Round(double.Parse(LookupParameterValue(x, Property, "Number")), 0) >= double.Parse(Value)).ToList();
					}
					else if (Condition == "<=")
					{
						elements = Listelements.Where(x => x.LookupParameter(Property) != null
						&& Math.Round(double.Parse(LookupParameterValue(x, Property, "Number")), 0) <= double.Parse(Value)).ToList();
					}
					else if (Condition == ">")
					{
						elements = Listelements.Where(x => x.LookupParameter(Property) != null
						&& Math.Round(double.Parse(LookupParameterValue(x, Property, "Number")), 0) > double.Parse(Value)).ToList();
					}
					else if (Condition == "<")
					{
						elements = Listelements.Where(x => x.LookupParameter(Property) != null
						&& Math.Round(double.Parse(LookupParameterValue(x, Property, "Number")), 0) < double.Parse(Value)).ToList();
					}
				}
				else if (ValueType == "String")
				{
					if (Condition == "Contains")
					{
						elements = Listelements.Where(x => x.LookupParameter(Property) != null && LookupParameterValue(x, Property, "String").Contains(Value)).ToList();
					}
					else if (Condition == "=")
					{
						elements = Listelements.Where(x => x.LookupParameter(Property) != null && LookupParameterValue(x, Property, "String") == Value).ToList();
					}
					else if (Condition == "!=")
					{
						elements = Listelements.Where(x => x.LookupParameter(Property) != null && LookupParameterValue(x, Property, "String") != Value).ToList();
					}
					else if (Condition == "Does Not Contain")
					{
						elements = Listelements.Where(x => x.LookupParameter(Property) != null && LookupParameterValue(x, Property, "String").Contains(Value) == false).ToList();
					}
					else if (Condition == "Has Value")
					{
						elements = Listelements.Where(x => x.LookupParameter(Property) != null && x.LookupParameter(Property).HasValue == true).ToList();
					}
					else if (Condition == "Has No Value")
					{
						elements = Listelements.Where(x => x.LookupParameter(Property) != null && x.LookupParameter(Property).HasValue == false).ToList();
					}
					else if (Condition == "Matches Regex")
					{
						rg = new Regex(Value);

						elements = Listelements.Where(x => x.LookupParameter(Property) != null && rg.Match(LookupParameterValue(x, Property, "String")).Success).ToList();
					}
					else if (Condition == "Does Not Match RegEx")
					{
						rg = new Regex(Value);

						elements = Listelements.Where(x => x.LookupParameter(Property) != null && rg.Match(LookupParameterValue(x, Property, "String")).Success == false).ToList();
					}
					else if (Condition == "Matches with Regex group")
					{
						string CriteriaB = "";
						string PropertyB = "";
						string ValueB = "";
						List<Element> ElementsB = new List<Element>();
						foreach (string Id in Ids)
						{
							string query = $"select * from Addin_QAQC_DesciplineFilter WHERE Id_Filter = '{Id}' AND Id_CheckRow = '{IdCheck}'";

							foreach (DataRow row in db.Get_DataTable(query).Rows)
							{
								if (row["Condition"].ToString() == "Matches Regex" && row["Criteria"].ToString() == "TYPE")
								{
									CriteriaB = row["Criteria"].ToString();
									PropertyB = row["Property"].ToString();
									ValueB = row["Value"].ToString();
								}
								break;
							}
						}

						rg = new Regex(ValueB);
						if (CriteriaB == "TYPE")
						{
							List<Element> resultElement = new List<Element>();
							foreach (Element e in Listelements)
							{
								string resultText = "";
								foreach (Match result in rg.Matches(TypeName(doc, e)))
								{
									resultText = result.Groups[Value].ToString();
								}

								if (resultText == "")
								{
									foreach (Match result in rg.Matches(e.Name))
									{
										resultText = result.Groups[Value].ToString();
									}
								}

								if (e.LookupParameter(Property) != null)
								{
									if (LookupParameterValue(e, Property, "String") == resultText)
									{
										ElementsB.Add(e);
									}
								}
							}

							elements = Listelements.Intersect(ElementsB).ToList();
						}
					}
					else if (Condition == "Does Not Match RegEx group")
					{
						string CriteriaB = "";
						string PropertyB = "";
						string ValueB = "";
						List<Element> ElementsB = new List<Element>();
						foreach (string Id in Ids)
						{
							string query = $"select * from Addin_QAQC_DesciplineFilter WHERE Id_Filter = '{Id}'  AND Id_CheckRow = '{IdCheck}'";

							foreach (DataRow row in db.Get_DataTable(query).Rows)
							{
								if (row["Condition"].ToString() == "Matches Regex" && row["Criteria"].ToString() == "TYPE")
								{
									CriteriaB = row["Criteria"].ToString();
									PropertyB = row["Property"].ToString();
									ValueB = row["Value"].ToString();
								}
								break;
							}
						}

						rg = new Regex(ValueB);
						if (CriteriaB == "TYPE")
						{
							List<Element> resultElement = new List<Element>();
							foreach (Element e in Listelements)
							{
								string resultText = "";
								foreach (Match result in rg.Matches(TypeName(doc, e)))
								{
									resultText = result.Groups[Value].ToString();
								}

								if (resultText == "")
								{
									foreach (Match result in rg.Matches(e.Name))
									{
										resultText = result.Groups[Value].ToString();
									}
								}

								if (e.LookupParameter(Property) != null)
								{
									if (LookupParameterValue(e, Property, "String") != resultText)
									{
										ElementsB.Add(e);
									}
								}
							}

							elements = Listelements.Intersect(ElementsB).ToList();
						}
					}
					else if (Condition == "Defined")
					{
						elements = Listelements.Where(x => x.LookupParameter(Property) != null).ToList();
					}
					else if (Condition == "Undefined")
					{
						elements = Listelements.Where(x => x.LookupParameter(Property) == null).ToList();
					}
					else if (Condition == "Same value, yet different type")
					{
						List<Level> Grids = new FilteredElementCollector(doc).OfClass(typeof(Level)).Cast<Level>().OrderBy(x => x.Elevation).ToList();

						//Lọc luôn bỏ các đối tượng type
						Listelements = Listelements.Where(x => x is ElementType == false).ToList();

						if (Listelements[0].Category.Name == "Structural Columns")
						{
							foreach (Level level in Grids)
							{
								List<string> ListValue = Listelements.
									Where(x => x.get_Parameter(BuiltInParameter.SCHEDULE_BASE_LEVEL_PARAM).AsElementId() == level.Id)
									.Select(x => LookupParameterValue(x, Property, "String")).ToList();

								ListValue = ListValue.Distinct().ToList();

								foreach (string paravalue in ListValue)
								{
									List<Element> elementsSameValue = Listelements.Where(x => x.get_Parameter(BuiltInParameter.SCHEDULE_BASE_LEVEL_PARAM).AsElementId() == level.Id
									&& LookupParameterValue(x, Property, "String") == paravalue).ToList();
									List<string> listTypeofSameValue = elementsSameValue.Select(e => TypeName(doc, e)).ToList();

									listTypeofSameValue = listTypeofSameValue.Distinct().ToList();

									if (listTypeofSameValue.Count > 1)
									{
										//elements = elements.Union(elementsSameValue).ToList();
										List<Element> elements1 = new List<Element>();
										foreach (string st in listTypeofSameValue)
										{
											elements1.Add(elementsSameValue.Where(x => TypeName(doc, x) == st).FirstOrDefault());
										}

										elements = elements.Union(elements1).ToList();
									}
								}
							}
						}
						else if (Listelements[0].Category.Name == "Walls")
						{
							foreach (Level level in Grids)
							{
								List<string> ListValue = Listelements.
									Where(x => x.get_Parameter(BuiltInParameter.WALL_BASE_CONSTRAINT).AsElementId() == level.Id)
									.Select(x => LookupParameterValue(x, Property, "String")).ToList();

								ListValue = ListValue.Distinct().ToList();

								foreach (string paravalue in ListValue)
								{
									List<Element> elementsSameValue = Listelements.Where(x => x.get_Parameter(BuiltInParameter.WALL_BASE_CONSTRAINT).AsElementId() == level.Id
									&& LookupParameterValue(x, Property, "String") == paravalue).ToList();
									List<string> listTypeofSameValue = elementsSameValue.Select(e => TypeName(doc, e)).ToList();

									listTypeofSameValue = listTypeofSameValue.Distinct().ToList();

									if (listTypeofSameValue.Count > 1)
									{
										//elements = elements.Union(elementsSameValue).ToList();
										List<Element> elements1 = new List<Element>();
										foreach (string st in listTypeofSameValue)
										{
											elements1.Add(elementsSameValue.Where(x => TypeName(doc, x) == st).FirstOrDefault());
										}

										elements = elements.Union(elements1).ToList();
									}
								}
							}
						}
						else
						{
							List<string> ListValue = Listelements.Select(x => LookupParameterValue(x, Property, "String")).ToList();

							ListValue = ListValue.Distinct().ToList();

							foreach (string paravalue in ListValue)
							{
								List<Element> elementsSameValue = Listelements.Where(x => LookupParameterValue(x, Property, "String") == paravalue).ToList();
								List<string> listTypeofSameValue = elementsSameValue.Select(e => TypeName(doc, e)).ToList();

								listTypeofSameValue = listTypeofSameValue.Distinct().ToList();

								if (listTypeofSameValue.Count > 1)
								{
									//elements = elements.Union(elementsSameValue).ToList();
									List<Element> elements1 = new List<Element>();
									foreach (string st in listTypeofSameValue)
									{
										elements1.Add(elementsSameValue.Where(x => TypeName(doc, x) == st).FirstOrDefault());
									}

									elements = elements.Union(elements1).ToList();
								}
							}
						}
					}
					else if (Condition == "Same value, yet different para")
					{
						Listelements = Listelements.Where(x => x is ElementType == false).ToList();
						List<string> ListValue = Listelements.Select(x => LookupParameterValue(x, Property, "String")).ToList();

						ListValue = ListValue.Distinct().ToList();

						foreach (string paravalue in ListValue)
						{
							List<Element> elementsSameValue = Listelements.Where(x => LookupParameterValue(x, Property, "String") == paravalue).ToList();
							List<string> listTypeofSameValue = elementsSameValue.Select(e => GetParaAsStringValue(e, Value)).ToList();

							listTypeofSameValue = listTypeofSameValue.Distinct().ToList();

							if (listTypeofSameValue.Count > 1)
							{
								//elements = elements.Union(elementsSameValue).ToList();
								List<Element> elements1 = new List<Element>();
								foreach (string st in listTypeofSameValue)
								{
									elements1.Add(elementsSameValue.Where(x => GetParaAsStringValue(x, Value) == st).FirstOrDefault());
								}

								elements = elements.Union(elements1).ToList();
							}
						}
					}
				}
				else if (ValueType == "Parameter")
				{
					if (Condition == "=")
					{
						elements = Listelements.Where(x => x.LookupParameter(Property) != null
						&& Math.Round(double.Parse(LookupParameterValue(x, Property, "Number")), 0) == Math.Round(double.Parse(GetParaAsStringValue(x, Value)), 0)).ToList();
					}
					else if (Condition == "!=")
					{
						elements = Listelements.Where(x => x.LookupParameter(Property) != null
						&& Math.Round(double.Parse(LookupParameterValue(x, Property, "Number")), 0) != Math.Round(double.Parse(GetParaAsStringValue(x, Value)), 0)).ToList();
					}
					else if (Condition == ">")
					{
						elements = Listelements.Where(x => x.LookupParameter(Property) != null
						&& Math.Round(double.Parse(LookupParameterValue(x, Property, "Number")), 0) > Math.Round(double.Parse(GetParaAsStringValue(x, Value)), 0)).ToList();
					}
					else if (Condition == "<")
					{
						elements = Listelements.Where(x => x.LookupParameter(Property) != null
						&& Math.Round(double.Parse(LookupParameterValue(x, Property, "Number")), 0) < Math.Round(double.Parse(GetParaAsStringValue(x, Value)), 0)).ToList();
					}
					else if (Condition == ">=")
					{
						elements = Listelements.Where(x => x.LookupParameter(Property) != null
						&& Math.Round(double.Parse(LookupParameterValue(x, Property, "Number")), 0) >= Math.Round(double.Parse(GetParaAsStringValue(x, Value)), 0)).ToList();
					}
					else if (Condition == "<=")
					{
						elements = Listelements.Where(x => x.LookupParameter(Property) != null
						&& Math.Round(double.Parse(LookupParameterValue(x, Property, "Number")), 0) <= Math.Round(double.Parse(GetParaAsStringValue(x, Value)), 0)).ToList();
					}
					else if (Condition == "Contains")
					{
						elements = Listelements.Where(x => x.LookupParameter(Property) != null 
						&& LookupParameterValue(x, Property, "String").Contains(GetParaAsStringValue(x, Value))).ToList();
					}
					else if (Condition == "Does Not Contain")
					{
						elements = Listelements.Where(x => x.LookupParameter(Property) != null
						&& LookupParameterValue(x, Property, "String").Contains(GetParaAsStringValue(x, Value)) == false).ToList();
					}
				}
				else
				{ }
			}
			catch { }

			return elements.Select(e => e.Id).ToList();
		}

		public List<ElementId> FilterAPIParameter(Document doc, string Property, string Condition, string Value, string ValueType, List<ElementId> ListelementIds, List<string> Ids, string IdCheck)
		{
			List<Element> elements = new List<Element>();
			List<Element> Listelements = new List<Element>();
			foreach (ElementId id in ListelementIds)
			{
				Listelements.Add(doc.GetElement(id));
			}
			DbConnect db = new DbConnect();
			Regex rg;

			var builtInParameter = Enum.Parse(typeof(BuiltInParameter), Property);

			try
			{
				if (ValueType == "Double" || ValueType == "Integer") // Kiểm tra đổi đơn vị
				{
					if (Condition == "=")
					{
						elements = Listelements.Where(x => x.get_Parameter((BuiltInParameter)builtInParameter) != null
						&& Math.Round(double.Parse(GetParameterValue(x, (BuiltInParameter)builtInParameter, "Number")), 0) == double.Parse(Value)).ToList();
					}
					else if (Condition == "!=")
					{
						elements = Listelements.Where(x => x.get_Parameter((BuiltInParameter)builtInParameter) != null
						&& Math.Round(double.Parse(GetParameterValue(x, (BuiltInParameter)builtInParameter, "Number")), 0) != double.Parse(Value)).ToList();
					}
					else if (Condition == ">=")
					{
						elements = Listelements.Where(x => x.get_Parameter((BuiltInParameter)builtInParameter) != null
						&& Math.Round(double.Parse(GetParameterValue(x, (BuiltInParameter)builtInParameter, "Number")), 0) >= double.Parse(Value)).ToList();
					}
					else if (Condition == "<=")
					{
						elements = Listelements.Where(x => x.get_Parameter((BuiltInParameter)builtInParameter) != null &&
						Math.Round(double.Parse(GetParameterValue(x, (BuiltInParameter)builtInParameter, "Number")), 0) <= double.Parse(Value)).ToList();
					}
					else if (Condition == ">")
					{
						elements = Listelements.Where(x => x.get_Parameter((BuiltInParameter)builtInParameter) != null &&
						Math.Round(double.Parse(GetParameterValue(x, (BuiltInParameter)builtInParameter, "Number")), 0) > double.Parse(Value)).ToList();
					}
					else if (Condition == "<")
					{
						elements = Listelements.Where(x => x.get_Parameter((BuiltInParameter)builtInParameter) != null &&
						Math.Round(double.Parse(GetParameterValue(x, (BuiltInParameter)builtInParameter, "Number")), 0) < double.Parse(Value)).ToList();
					}
				}
				else if (ValueType == "String")
				{
					if (Condition == "Contains")
					{
						elements = Listelements.Where(x => x.get_Parameter((BuiltInParameter)builtInParameter) != null
						&& GetParameterValue(x, (BuiltInParameter)builtInParameter, "String").Contains(Value)).ToList();
					}
					else if (Condition == "=")
					{
						elements = Listelements.Where(x => x.get_Parameter((BuiltInParameter)builtInParameter) != null
						&& GetParameterValue(x, (BuiltInParameter)builtInParameter, "String") == (Value)).ToList();
					}
					else if (Condition == "!=")
					{
						elements = Listelements.Where(x => x.get_Parameter((BuiltInParameter)builtInParameter) != null
						&& GetParameterValue(x, (BuiltInParameter)builtInParameter, "String") != (Value)).ToList();
					}
					else if (Condition == "Does Not Contain")
					{
						elements = Listelements.Where(x => x.get_Parameter((BuiltInParameter)builtInParameter) != null
						&& GetParameterValue(x, (BuiltInParameter)builtInParameter, "String").Contains(Value) == false).ToList();
					}
					else if (Condition == "Has Value")
					{
						elements = Listelements.Where(x => x.get_Parameter((BuiltInParameter)builtInParameter) != null
						&& x.get_Parameter((BuiltInParameter)builtInParameter).HasValue == true).ToList();
					}
					else if (Condition == "Has No Value")
					{
						elements = Listelements.Where(x => x.get_Parameter((BuiltInParameter)builtInParameter) != null
						&& x.get_Parameter((BuiltInParameter)builtInParameter).HasValue == false).ToList();
					}
					else if (Condition == "Matches Regex")
					{
						rg = new Regex(Value);

						elements = Listelements.Where(x => x.get_Parameter((BuiltInParameter)builtInParameter) != null
						&& rg.Match(GetParameterValue(x, (BuiltInParameter)builtInParameter, "String")).Success).ToList();
					}
					else if (Condition == "Does Not Match RegEx")
					{
						rg = new Regex(Value);

						elements = Listelements.Where(x => x.get_Parameter((BuiltInParameter)builtInParameter) != null
						&& rg.Match(GetParameterValue(x, (BuiltInParameter)builtInParameter, "String")).Success == false).ToList();
					}
					else if (Condition == "Matches with Regex group")
					{
						string CriteriaB = "";
						string PropertyB = "";
						string ValueB = "";
						List<Element> ElementsB = new List<Element>();
						foreach (string Id in Ids)
						{
							string query = $"select * from Addin_QAQC_DesciplineFilter WHERE Id_Filter = '{Id}'  AND Id_CheckRow = '{IdCheck}'";

							foreach (DataRow row in db.Get_DataTable(query).Rows)
							{
								if (row["Condition"].ToString() == "Matches Regex" && row["Criteria"].ToString() == "TYPE")
								{
									CriteriaB = row["Criteria"].ToString();
									PropertyB = row["Property"].ToString();
									ValueB = row["Value"].ToString();
								}
							}
						}

						rg = new Regex(ValueB);
						if (CriteriaB == "TYPE")
						{
							List<Element> resultElement = new List<Element>();
							foreach (Element e in Listelements)
							{
								string resultText = "";
								foreach (Match result in rg.Matches(TypeName(doc, e)))
								{
									resultText = result.Groups[Value].ToString();
								}

								if (resultText == "")
								{
									foreach (Match result in rg.Matches(e.Name))
									{
										resultText = result.Groups[Value].ToString();
									}
								}


								if (e.get_Parameter((BuiltInParameter)builtInParameter) != null)
								{
									if (GetParameterValue(e, (BuiltInParameter)builtInParameter, "String") == resultText)
									{
										ElementsB.Add(e);
									}
								}
							}

							elements = Listelements.Intersect(ElementsB).ToList();
						}
					}
					else if (Condition == "Contains with Regex group")
					{
						string CriteriaB = "";
						string PropertyB = "";
						string ValueB = "";
						List<Element> ElementsB = new List<Element>();
						foreach (string Id in Ids)
						{
							string query = $"select * from Addin_QAQC_DesciplineFilter WHERE Id_Filter = '{Id}'  AND Id_CheckRow = '{IdCheck}'";

							foreach (DataRow row in db.Get_DataTable(query).Rows)
							{
								if (row["Condition"].ToString() == "Matches Regex" && row["Criteria"].ToString() == "TYPE")
								{
									CriteriaB = row["Criteria"].ToString();
									PropertyB = row["Property"].ToString();
									ValueB = row["Value"].ToString();
								}
							}
						}

						rg = new Regex(ValueB);
						if (CriteriaB == "TYPE")
						{
							List<Element> resultElement = new List<Element>();
							foreach (Element e in Listelements)
							{
								string resultText = "";
								foreach (Match result in rg.Matches(TypeName(doc, e)))
								{
									resultText = result.Groups[Value].ToString();
								}

								if (resultText == "")
								{
									foreach (Match result in rg.Matches(e.Name))
									{
										resultText = result.Groups[Value].ToString();
									}
								}


								if (e.get_Parameter((BuiltInParameter)builtInParameter) != null)
								{
									if (GetParameterValue(e, (BuiltInParameter)builtInParameter, "String").Contains(resultText))
									{
										ElementsB.Add(e);
									}
								}
							}

							elements = Listelements.Intersect(ElementsB).ToList();
						}
					}
					else if (Condition == "Does Not Contains RegEx group")
					{
						string CriteriaB = "";
						string PropertyB = "";
						string ValueB = "";
						List<Element> ElementsB = new List<Element>();
						foreach (string Id in Ids)
						{
							string query = $"select * from Addin_QAQC_DesciplineFilter WHERE Id_Filter = '{Id}'  AND Id_CheckRow = '{IdCheck}'";

							foreach (DataRow row in db.Get_DataTable(query).Rows)
							{
								if (row["Condition"].ToString() == "Matches Regex" && row["Criteria"].ToString() == "TYPE")
								{
									CriteriaB = row["Criteria"].ToString();
									PropertyB = row["Property"].ToString();
									ValueB = row["Value"].ToString();
								}
							}
						}

						rg = new Regex(ValueB);
						if (CriteriaB == "TYPE")
						{
							List<Element> resultElement = new List<Element>();
							foreach (Element e in Listelements)
							{
								string resultText = "";
								foreach (Match result in rg.Matches(TypeName(doc, e)))
								{
									resultText = result.Groups[Value].ToString();
								}

								if (resultText == "")
								{
									foreach (Match result in rg.Matches(e.Name))
									{
										resultText = result.Groups[Value].ToString();
									}
								}


								if (e.get_Parameter((BuiltInParameter)builtInParameter) != null)
								{
									if (GetParameterValue(e, (BuiltInParameter)builtInParameter, "String").Contains(resultText) == false)
									{
										ElementsB.Add(e);
									}
									else
									{
										System.Diagnostics.Debug.WriteLine (resultText);
										System.Diagnostics.Debug.WriteLine (GetParameterValue(e, (BuiltInParameter)builtInParameter, "String"));										
									}
								}
							}

							elements = Listelements.Intersect(ElementsB).ToList();
						}
					}
					else if (Condition == "Does Not Match RegEx group")
					{
						string CriteriaB = "";
						string PropertyB = "";
						string ValueB = "";
						List<Element> ElementsB = new List<Element>();
						foreach (string Id in Ids)
						{
							string query = $"select * from Addin_QAQC_DesciplineFilter WHERE Id_Filter = '{Id}'  AND Id_CheckRow = '{IdCheck}'";

							foreach (DataRow row in db.Get_DataTable(query).Rows)
							{
								if (row["Condition"].ToString() == "Matches Regex" && row["Criteria"].ToString() == "TYPE")
								{
									CriteriaB = row["Criteria"].ToString();
									PropertyB = row["Property"].ToString();
									ValueB = row["Value"].ToString();
								}
								break;
							}
						}

						rg = new Regex(ValueB);
						if (CriteriaB == "TYPE")
						{
							List<Element> resultElement = new List<Element>();
							foreach (Element e in Listelements)
							{
								string resultText = "";
								foreach (Match result in rg.Matches(TypeName(doc, e)))
								{
									resultText = result.Groups[Value].ToString();
								}

								if (resultText == "")
								{
									foreach (Match result in rg.Matches(e.Name))
									{
										resultText = result.Groups[Value].ToString();
									}
								}

								if (e.get_Parameter((BuiltInParameter)builtInParameter) != null)
								{
									if (GetParameterValue(e, (BuiltInParameter)builtInParameter, "String") != resultText)
									{
										ElementsB.Add(e);
									}
								}
							}

							elements = Listelements.Intersect(ElementsB).ToList();
						}
					}
					else if (Condition == "Defined")
					{
						elements = Listelements.Where(x => x.get_Parameter((BuiltInParameter)builtInParameter) != null).ToList();
					}
					else if (Condition == "Unefined")
					{
						elements = Listelements.Where(x => x.get_Parameter((BuiltInParameter)builtInParameter) == null).ToList();
					}
					else if (Condition == "Same value, yet different type")
					{
						List<string> ListValue = Listelements.Select(x => GetParameterValue(x, (BuiltInParameter)builtInParameter, "String")).ToList();
						foreach (string paravalue in ListValue)
						{
							List<Element> elementsSameValue = Listelements.Where(x => GetParameterValue(x, (BuiltInParameter)builtInParameter, "String") == paravalue).ToList();
							List<string> listTypeofSameValue = elementsSameValue.Select(e => TypeName(doc, e)).ToList();
							if (listTypeofSameValue.Count > 1)
							{
								elements = elements.Union(elementsSameValue).ToList();
							}
						}
					}
				}
				else if (ValueType == "Parameter")
				{
					if (Condition == "=")
					{
						elements = Listelements.Where(x => x.get_Parameter((BuiltInParameter)builtInParameter) != null
						&& Math.Round(double.Parse(GetParameterValue(x, (BuiltInParameter)builtInParameter, "Number")), 0) == Math.Round(double.Parse(GetParaAsStringValue(x, Value)), 0)).ToList();
					}
					else if (Condition == "!=")
					{
						elements = Listelements.Where(x => x.get_Parameter((BuiltInParameter)builtInParameter) != null
						&& Math.Round(double.Parse(GetParameterValue(x, (BuiltInParameter)builtInParameter, "Number")), 0) != Math.Round(double.Parse(GetParaAsStringValue(x, Value)), 0)).ToList();
					}
					else if (Condition == ">")
					{
						elements = Listelements.Where(x => x.get_Parameter((BuiltInParameter)builtInParameter) != null
						&& Math.Round(double.Parse(GetParameterValue(x, (BuiltInParameter)builtInParameter, "Number")), 0) > Math.Round(double.Parse(GetParaAsStringValue(x, Value)), 0)).ToList();
					}
					else if (Condition == "<")
					{
						elements = Listelements.Where(x => x.get_Parameter((BuiltInParameter)builtInParameter) != null
						&& Math.Round(double.Parse(GetParameterValue(x, (BuiltInParameter)builtInParameter, "Number")), 0) < Math.Round(double.Parse(GetParaAsStringValue(x, Value)), 0)).ToList();
					}
					else if (Condition == ">=")
					{
						elements = Listelements.Where(x => x.get_Parameter((BuiltInParameter)builtInParameter) != null
						&& Math.Round(double.Parse(GetParameterValue(x, (BuiltInParameter)builtInParameter, "Number")), 0) >= Math.Round(double.Parse(GetParaAsStringValue(x, Value)), 0)).ToList();
					}
					else if (Condition == "<=")
					{
						elements = Listelements.Where(x => x.get_Parameter((BuiltInParameter)builtInParameter) != null
						&& Math.Round(double.Parse(GetParameterValue(x, (BuiltInParameter)builtInParameter, "Number")), 0) <= Math.Round(double.Parse(GetParaAsStringValue(x, Value)), 0)).ToList();
					}
					else if (Condition == "Contains")
					{
						elements = Listelements.Where(x => x.get_Parameter((BuiltInParameter)builtInParameter) != null
						&& GetParameterValue(x, (BuiltInParameter)builtInParameter, "String").Contains(GetParaAsStringValue(x, Value))).ToList();
					}
					else if (Condition == "Does Not Contain")
					{
						elements = Listelements.Where(x => x.get_Parameter((BuiltInParameter)builtInParameter) != null
						&& GetParameterValue(x, (BuiltInParameter)builtInParameter, "String").Contains(GetParaAsStringValue(x, Value)) == false).ToList();
					}
				} 
				else
				{ }
			}
			catch { }

			return elements.Select(e => e.Id).ToList();
		}

		public List<ElementId> FilterRoom(Document doc, string Property, string Condition, string Value, string ValueType)
		{
			List<Element> elements = new List<Element>();

			try
			{

			}
			catch
			{

			}

			return elements.Select(e => e.Id).ToList();
		}

		public List<ElementId> FilterWorkset(Document doc, string Property, string Condition, string Value)
		{
			List<Element> elements = new List<Element>();
			Regex rg;

			try
			{
				switch (Condition)
				{
					case "=":
						elements = new FilteredElementCollector(doc)
						.WhereElementIsNotElementType()
						.Where(x => x.WorksetId != null && WorksetNameFromId(doc, x) == Value).ToList();

						//elements = finalCollector.ToList<Element>();
						break;
					case "!=":
						elements = new FilteredElementCollector(doc)
						.WhereElementIsNotElementType()
						.Where(x => x.WorksetId != null && WorksetNameFromId(doc, x) != Value).ToList();
						break;
					case "Contains":
						elements = new FilteredElementCollector(doc)
						.WhereElementIsNotElementType()
						.Where(x => x.WorksetId != null && WorksetNameFromId(doc, x).Contains(Value)).ToList();
						break;
					case "Does Not Contain":
						elements = new FilteredElementCollector(doc)
						.WhereElementIsNotElementType()
						.Where(x => x.WorksetId != null && WorksetNameFromId(doc, x).Contains(Value) == false).ToList();
						break;
					case "Matches Regex":
						rg = new Regex(Value);

						elements = new FilteredElementCollector(doc)
						.WhereElementIsNotElementType()
						.Where(x => x.WorksetId != null && rg.Match(WorksetNameFromId(doc, x)).Success == true).ToList();

						break;
					case "Does Not Match RegEx":
						rg = new Regex(Value);

						elements = new FilteredElementCollector(doc)
						.WhereElementIsNotElementType()
						.Where(x => x.WorksetId != null && rg.Match(WorksetNameFromId(doc, x)).Success == false).ToList();
						break;
					default:
						break;
				}
			}
			catch
			{ }

			return elements.Select(e => e.Id).ToList();
		}

		public List<ElementId> FilterLocation(Document doc, string Property, string Condition, string Value, List<ElementId> ListelementIds)
		{
			List<Element> elements = new List<Element>();
			List<Element> Listelements = new List<Element>();
			foreach (ElementId id in ListelementIds)
			{
				Listelements.Add(doc.GetElement(id));
			}

			List<Grid> Grids = new FilteredElementCollector(doc).OfClass(typeof(Grid)).Cast<Grid>().Where(x => x.Curve is Line).OrderBy(x => x.Name).ToList();

			try
			{
				foreach (Element element in Listelements)
				{
					try
					{
						if (element.Category != null)
						{
							if (element.Category.Name == "Structural Framing" || element.Category.Name == "Walls")
							{
								double ElementWidth = 0;
								try
								{
									ElementWidth = element.get_Parameter(BuiltInParameter.WALL_ATTR_WIDTH_PARAM).AsDouble();
								}
								catch
								{
									ElementWidth = 0;
								}

								LocationCurve locationCurve = element.Location as LocationCurve;
								if (locationCurve.Curve is Line locationLine)
								{
									if (element.Category.Name == "Walls")
									{
										if (element.get_Parameter(BuiltInParameter.WALL_KEY_REF_PARAM).AsValueString() == "Wall Centerline")
											ElementWidth = ElementWidth / 2;
									}

									bool IsFalse = true;
									foreach (Grid grid in Grids)
									{
										Line lineGrid = grid.Curve as Line;
										if (RelativePositionBetweenLineAndLine(locationLine, lineGrid) == BetweenLineAndLine.Parallel)
										{
											XYZ checkpoint = new XYZ(locationLine.GetEndPoint(0).X, locationLine.GetEndPoint(0).Y, lineGrid.GetEndPoint(0).Z);
											double distancett = Math.Round((CurveUtils.DistancePoint2Line(checkpoint, lineGrid)+ ElementWidth) * 304.8, 3);

											if (Value != "")
											{
												if (distancett > double.Parse(Value)) continue;
											}

											if ((distancett - Math.Round(distancett, 0) == 0))
											{
												IsFalse = true;
												break;
											}
											else
											{
												IsFalse = false;
												break;
											}
										}
									}
									if (IsFalse == false) elements.Add(element);
								}
							}
							else if (element.Category.Name == "Grids")
							{
								Grid gridelement = element as Grid;
								Line locationLine = gridelement.Curve as Line;

								bool IsFalse = true;
								foreach (Grid grid in Grids)
								{
									Line lineGrid = grid.Curve as Line;
									if (RelativePositionBetweenLineAndLine(locationLine, lineGrid) == BetweenLineAndLine.Parallel)
									{
										XYZ checkpoint = new XYZ(locationLine.GetEndPoint(0).X, locationLine.GetEndPoint(0).Y, lineGrid.GetEndPoint(0).Z);
										double distancett = Math.Round(CurveUtils.DistancePoint2Line(checkpoint, lineGrid) * 304.8, 3);

										if (Value != "")
										{
											if (distancett > double.Parse(Value) || (Distance2Point(checkpoint, lineGrid.GetEndPoint(0)) * 304.8 > double.Parse(Value)
												&& Distance2Point(checkpoint, lineGrid.GetEndPoint(1)) * 304.8 > double.Parse(Value))) continue;
										}

										if ((distancett - Math.Round(distancett, 0) != 0))
										{
											IsFalse = false;
										}
										else IsFalse = true;
									}
								}
								if (IsFalse == false) elements.Add(element);
							}
							else if (element.Category.Name == "Structural Columns")
							{
								if (element.Location is LocationPoint locationPoint)
								{
									XYZ location = locationPoint.Point;
									bool IsFalse = false;
									foreach (Grid grid in Grids)
									{
										Line lineGrid = grid.Curve as Line;
										location = new XYZ(location.X, location.Y, lineGrid.GetEndPoint(0).Z);

										double distancett = Math.Round(CurveUtils.DistancePoint2Line(location, lineGrid) * 304.8, 3);

										if (Value != "")
										{
											if (distancett > double.Parse(Value)) continue;
										}

										if ((distancett - Math.Round(distancett, 0) == 0))
										{
											IsFalse = true;
											break;
										}
									}

									if (IsFalse == false) elements.Add(element);

								}

							}
						}
					}
					catch { }
				}
			}
			catch { }
			return elements.Select(e => e.Id).ToList();
		}

		public List<ElementId> FilterView(Document doc, string Property, string Condition, string Value)
		{
			List<Element> elements = new List<Element>();
			Regex rg;

			try
			{
				if (Property == "Name")
				{
					switch (Condition)
					{
						case "==":
							elements = new FilteredElementCollector(doc)
							.OfCategory(BuiltInCategory.OST_Views)
							.Where(x => x.Name == Value).ToList();
							break;
						case "!=":
							elements = new FilteredElementCollector(doc)
							.OfCategory(BuiltInCategory.OST_Views)
							.Where(x => x.Name != Value).ToList();
							break;
						case "Contains":
							elements = new FilteredElementCollector(doc)
							.OfCategory(BuiltInCategory.OST_Views)
							.Where(x => x.Name.Contains(Value)).ToList();
							break;
						case "Does Not Contain":
							elements = new FilteredElementCollector(doc)
							.OfCategory(BuiltInCategory.OST_Views)
							.Where(x => x.Name.Contains(Value) == false).ToList();
							break;
						case "Matches Regex":
							rg = new Regex(Value);

							elements = new FilteredElementCollector(doc)
							.OfCategory(BuiltInCategory.OST_Views)
							.Where(x => rg.Match(x.Name).Success).ToList();
							break;
						case "Does Not Match RegEx":
							rg = new Regex(Value);

							elements = new FilteredElementCollector(doc)
							.OfCategory(BuiltInCategory.OST_Views)
							.Where(x => rg.Match(x.Name).Success == false).ToList();

							break;
						default:
							break;
					}

				}
				else
				{ }
			}
			catch
			{ }

			return elements.Select(x => x.Id).ToList();
		}

		public List<ElementId> FilterStructuralType(Document doc, string Value)
		{
			List<Element> elements = new List<Element>();

			var StructuralType = Enum.Parse(typeof(StructuralType), Value);

			try
			{
				ElementStructuralTypeFilter elementStructuralTypeFilter = new ElementStructuralTypeFilter((StructuralType)StructuralType);

				elements = new FilteredElementCollector(doc).OfClass(typeof(FamilyInstance))
					.WherePasses(elementStructuralTypeFilter).ToList();
			}
			catch { }

			return elements.Select(x => x.Id).ToList();
		}

		public List<ElementId> FilterWarning(Document doc, string Value)
		{
			List<ElementId> elements = new List<ElementId>();

			ObservableCollection<ObjectWaring> WaringInProject = new ObservableCollection<ObjectWaring>();
			try
			{
				var Obj = doc.GetWarnings();

				for (int i = 0; i < Obj.Count(); i++)
				{
					if (Obj[i].GetDescriptionText() == Value)
					{
						WaringInProject.Add(new ObjectWaring(i + 1, Obj[i]));
					}
					else if (Obj[i].GetDescriptionText().Contains(Value))
					{
						WaringInProject.Add(new ObjectWaring(i + 1, Obj[i]));
					}
				}
			}
			catch { }

			List<List<ElementId>> a = WaringInProject.Select(x => x.ElementWarning).ToList();

			foreach (List<ElementId> elementwarning in a)
			{
				elements = elements.Union(elementwarning).ToList();
			}
			return elements;
		}

		public List<ElementId> FilterbaseLevel(Document doc, string Property, string Condition, string Value, List<ElementId> ListelementIds)
		{
			List<Element> elements = new List<Element>();
			List<Element> Listelements = new List<Element>();
			foreach (ElementId id in ListelementIds)
			{
				Listelements.Add(doc.GetElement(id));
			}

			List<Level> Grids = new FilteredElementCollector(doc).OfClass(typeof(Level)).Cast<Level>().OrderBy(x => x.Elevation).ToList();
			List<ElementId> GridIds = Grids.Select(x => x.Id).ToList();

			try
			{
				foreach (Element element in Listelements)
				{
					ElementId BaseId = new ElementId(-1);
					ElementId TopId = new ElementId(-1);
					try
					{
						if (element.Category != null)
						{
							if (element.Category.Name == "Walls")
							{
								BaseId = element.get_Parameter(BuiltInParameter.WALL_BASE_CONSTRAINT).AsElementId();
								TopId = element.get_Parameter(BuiltInParameter.WALL_HEIGHT_TYPE).AsElementId();
							}
							else if (element.Category.Name == "Structural Columns")
							{
								BaseId = element.get_Parameter(BuiltInParameter.SCHEDULE_BASE_LEVEL_PARAM).AsElementId();
								TopId = element.get_Parameter(BuiltInParameter.SCHEDULE_TOP_LEVEL_PARAM).AsElementId();
							}
						}
					}
					catch { }

					if (Condition == "=")
					{
						if (GridIds.IndexOf(TopId) - GridIds.IndexOf(BaseId) != 1)
						{
							int start = GridIds.IndexOf(BaseId);
							int end = GridIds.IndexOf(TopId);
							bool isSub = true;
							foreach (Level level in Grids)
							{
								if (GridIds.IndexOf(level.Id) > start && GridIds.IndexOf(level.Id) < end)
								{
									if (level.Name.Contains(" Sub") == false)
									{
										isSub = false;
										break;
									}
								}
							}

							if (isSub == false) elements.Add(element);
						}

					}
					else if (Condition == "!=")
					{
						if (GridIds.IndexOf(TopId) - GridIds.IndexOf(BaseId) != 0) elements.Add(element);
					}
				}
			}
			catch { }
			return elements.Select(e => e.Id).ToList();
		}

		public List<ElementId> FilterIsMonitor(Document doc, string Property, List<ElementId> ListelementIds)
		{
			List<Element> elements = new List<Element>();
			List<Element> Listelements = new List<Element>();
			foreach (ElementId id in ListelementIds)
			{
				Listelements.Add(doc.GetElement(id));
			}

			try
			{
				if (Property == "True")
				{
					elements = Listelements.Where(e => e.IsMonitoringLinkElement() == true).ToList();
				}
				else
				{
					elements = Listelements.Where(e => e.IsMonitoringLinkElement() == false).ToList();
				}
			}
			catch
			{
			}

			return elements.Select(e => e.Id).ToList();
		}

		#endregion
		#region Utils
		public string TypeName(Document doc, Element element)
		{
			try
			{
				Element elemType = doc.GetElement(element.GetTypeId());
				return elemType.Name;
			}
			catch
			{
				try { return element.Name; }
				catch { return ""; }
			}
		}

		public string WorksetNameFromId(Document doc, Element e)
		{
			IList<Workset> worksetList = new FilteredWorksetCollector(doc).OfKind(WorksetKind.UserWorkset).ToWorksets();
			foreach (Workset ws in worksetList)
			{
				if (ws.Id == e.WorksetId) return ws.Name;
			}
			return "";
		}

		public BetweenLineAndLine RelativePositionBetweenLineAndLine(Line lineOne, Line lineTwo)
		{
			BetweenLineAndLine result = BetweenLineAndLine.None;
			XYZ pointOne = lineOne.Origin;
			XYZ vectorDirectionOne = lineOne.Direction;
			XYZ pointTwo = lineTwo.Origin;
			XYZ vectorDirectionTwo = lineTwo.Direction;
			XYZ m1m2 = Vector(pointOne, pointTwo);
			XYZ crossProductBetweenVecotrDirectionOneAndVectorDirectionTwo = CrossProduct(vectorDirectionOne, vectorDirectionTwo);
			XYZ crossProductBetweenVecotrDirectionOneAndVectorM1M2 = CrossProduct(vectorDirectionOne, m1m2);
			if (Math.Abs(DotProduct(crossProductBetweenVecotrDirectionOneAndVectorDirectionTwo, m1m2)) < EPSILON && !AreVectorsEqual(crossProductBetweenVecotrDirectionOneAndVectorDirectionTwo, XYZ.Zero))
			{
				result = BetweenLineAndLine.Intersecting;
				if (Math.Abs(AngleBetweenTwoVectors(vectorDirectionOne, vectorDirectionTwo, true) - 90) <= EPSILON)
				{
					result = BetweenLineAndLine.IntersectPerpendicular;
				}
			}
			else if (AreVectorsEqual(crossProductBetweenVecotrDirectionOneAndVectorDirectionTwo, XYZ.Zero) && !AreVectorsEqual(crossProductBetweenVecotrDirectionOneAndVectorM1M2, XYZ.Zero))
			{
				result = BetweenLineAndLine.Parallel;
			}
			else if (AreVectorsEqual(crossProductBetweenVecotrDirectionOneAndVectorDirectionTwo, XYZ.Zero) && AreVectorsEqual(crossProductBetweenVecotrDirectionOneAndVectorM1M2, XYZ.Zero))
			{
				result = BetweenLineAndLine.Overlap;
			}
			else if (!AreVectorsEqual(crossProductBetweenVecotrDirectionOneAndVectorDirectionTwo, XYZ.Zero) && Math.Abs(DotProduct(crossProductBetweenVecotrDirectionOneAndVectorDirectionTwo, m1m2)) > EPSILON)
			{
				result = BetweenLineAndLine.CrossEachOther;
				if (Math.Abs(AngleBetweenTwoVectors(vectorDirectionOne, vectorDirectionTwo, true) - 90) <= EPSILON)
				{
					result = BetweenLineAndLine.CrossEachOtherPerpendicular;
				}
			}
			return result;
		}

		public XYZ Vector(XYZ pointOne, XYZ pointTwo)
		{
			return new XYZ(pointTwo.X - pointOne.X, pointTwo.Y - pointOne.Y, pointTwo.Z - pointOne.Z);
		}

		public XYZ CrossProduct(XYZ vectorOne, XYZ vectorTwo)
		{
			double toaDoX = vectorOne.Y * vectorTwo.Z - vectorTwo.Y * vectorOne.Z;
			double toaDoY = vectorOne.Z * vectorTwo.X - vectorTwo.Z * vectorOne.X;
			double toaDoZ = vectorOne.X * vectorTwo.Y - vectorTwo.X * vectorOne.Y;
			return new XYZ(toaDoX, toaDoY, toaDoZ);
		}

		public double DotProduct(XYZ vectorOne, XYZ vectorTwo)
		{
			return vectorOne.X * vectorTwo.X + vectorOne.Y * vectorTwo.Y + vectorOne.Z * vectorTwo.Z;
		}

		public bool AreVectorsEqual(XYZ vectorOne, XYZ vectorTwo)
		{
			bool result = false;
			bool compareX = Math.Abs(vectorOne.X - vectorTwo.X) <= EPSILON;
			bool compareY = Math.Abs(vectorOne.Y - vectorTwo.Y) <= EPSILON;
			bool compareZ = Math.Abs(vectorOne.Z - vectorTwo.Z) <= EPSILON;
			if (compareX && compareY && compareZ)
			{
				result = true;
			}
			return result;
		}

		public double AngleBetweenTwoVectors(XYZ vectorOne, XYZ vectorTwo, bool absolute)
		{
			double numerator = vectorOne.X * vectorTwo.X + vectorOne.Y * vectorTwo.Y + vectorOne.Z * vectorTwo.Z;
			double denominator = VectorLength(vectorOne) * VectorLength(vectorTwo);
			if (absolute)
			{
				return (Math.Acos(Math.Round(numerator / denominator, N))) * (180 / (Math.PI));
			}
			else
			{
				return (Math.Acos(Math.Round(Math.Abs(numerator) / denominator, N))) * (180 / (Math.PI));
			}
		}

		public double VectorLength(XYZ vector)
		{
			return Math.Sqrt(Math.Pow(vector.X, 2) + Math.Pow(vector.Y, 2) + Math.Pow(vector.Z, 2));
		}

		public static string GetParameterValue(Element element, BuiltInParameter builtInParameter, string valuetype)
		{
			try
			{
				if (valuetype != "String")
				{
					StorageType a = element.get_Parameter((BuiltInParameter)builtInParameter).StorageType;

					if (a == StorageType.String)
					{
						if (element.get_Parameter((BuiltInParameter)builtInParameter).AsValueString() is null)
						{
							return "";
						}
						else
						{
							return element.get_Parameter((BuiltInParameter)builtInParameter).AsValueString();
						}
					}
					else if (a == StorageType.Double)
					{
						if (element.get_Parameter((BuiltInParameter)builtInParameter).AsDouble() == 0)
						{
							return "0";
						}
						else
						{
							return MathUtils.FootToMm(element.get_Parameter((BuiltInParameter)builtInParameter).AsDouble()).ToString();
						}
					}
					else if (a == StorageType.Integer)
					{
						return element.get_Parameter((BuiltInParameter)builtInParameter).AsInteger().ToString();
					}
					else
					{
						return element.get_Parameter((BuiltInParameter)builtInParameter).AsElementId().ToString();
					}
				}
				else
				{
					if (element.get_Parameter((BuiltInParameter)builtInParameter).AsValueString() is null)
					{
						return "";
					}
					else
					{
						return element.get_Parameter((BuiltInParameter)builtInParameter).AsValueString();
					}
				}
			}
			catch
			{
				return "";
			}
		}

		public static string GetParaAsStringValue(Element element, string value)
		{
			BuiltInParameter builtInParameter;
			string valuePara = "";
			try
			{
				bool isBuiltInParameter = Enum.TryParse<BuiltInParameter>(value, out builtInParameter);

				if (isBuiltInParameter == true)
				{
					valuePara = element.get_Parameter((BuiltInParameter)builtInParameter).AsValueString();
				}
				else
				{
					valuePara = element.LookupParameter(value).AsValueString();
				}
			}
			catch { }
			return valuePara;
		}

		public string LookupParameterValue(Element element, string parameterName, string Valuetype)
		{
			try
			{
				if (Valuetype != "String")
				{
					StorageType a = element.LookupParameter(parameterName).StorageType;

					if (a == StorageType.String)
					{
						if (element.LookupParameter(parameterName).AsValueString() is null)
						{
							return "";
						}
						else
						{
							if (element.LookupParameter(parameterName).AsString() == ""
								|| element.LookupParameter(parameterName).AsString() == null)
							{
								return element.LookupParameter(parameterName).AsValueString();
							}
							else
							{
								return element.LookupParameter(parameterName).AsString();
							}
						}
					}
					else if (a == StorageType.Double)
					{
						if (element.LookupParameter(parameterName).AsDouble() == 0)
						{
							return "0";
						}
						else
						{
							return MathUtils.FootToMm(element.LookupParameter(parameterName).AsDouble()).ToString();
						}
					}
					else if (a == StorageType.Integer)
					{
						return element.LookupParameter(parameterName).AsInteger().ToString();
					}
					else
					{
						return element.LookupParameter(parameterName).AsElementId().ToString();
					}
				}
				else
				{
					if (element.LookupParameter(parameterName).AsValueString() is null)
					{
						return "";
					}
					else
					{
						return element.LookupParameter(parameterName).AsValueString();
					}
				}
			}
			catch
			{
				return "";
			}
		}

		public double Distance2Point(XYZ p1, XYZ p2)
		{
			double d = Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y));
			return d;
		}

		public static string GetFamilyName(Element element)
		{
			string familyname = "";

				try
				{
					familyname= GetParameterValue(element, BuiltInParameter.ELEM_FAMILY_PARAM, "String");
					if (familyname == "")
					{
						familyname = GetParameterValue(element, BuiltInParameter.ALL_MODEL_FAMILY_NAME, "String");
					}
				}
				catch
				{
				familyname = "";
				}
			return familyname;
		}
		#endregion
	}

	public enum BetweenLineAndLine
	{
		None,
		Intersecting,
		Parallel,
		Overlap,
		CrossEachOther,
		IntersectPerpendicular,
		CrossEachOtherPerpendicular
	}
}
