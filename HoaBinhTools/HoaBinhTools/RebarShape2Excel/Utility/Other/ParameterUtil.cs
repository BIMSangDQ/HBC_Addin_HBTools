using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Model;
using SingleData;

namespace Utility
{
	public static partial class ParameterUtil
	{
		private static RevitData revitData
		{
			get
			{
				return RevitData.Instance;
			}
		}
		public static Model.ParameterValue AsValue(this Autodesk.Revit.DB.Element element, string param)
		{
			return element.LookupParameter(param).AsValue();
		}
		public static Model.ParameterValue AsValue(this Autodesk.Revit.DB.Parameter p)
		{
			string text = null; double number = 0;
			switch (p.StorageType)
			{
				case Autodesk.Revit.DB.StorageType.String:
					text = p.AsString();
					return new Model.ParameterValue { ParameterValueType = ParameterValueType.Text, ValueText = text };
				case Autodesk.Revit.DB.StorageType.ElementId:
					text = revitData.Document.GetElement(p.AsElementId()).Name;
					return new Model.ParameterValue { ParameterValueType = ParameterValueType.Text, ValueText = text };
				case Autodesk.Revit.DB.StorageType.Integer:
					number = p.AsInteger();
					return new Model.ParameterValue { ParameterValueType = ParameterValueType.Number, ValueNumber = number };
				case Autodesk.Revit.DB.StorageType.Double:
					number = p.AsDouble().GetConvertValue(p.DisplayUnitType);
					return new Model.ParameterValue { ParameterValueType = ParameterValueType.Number, ValueNumber = number };
			}
			throw new Exception("Đoạn code này không được đọc tới.");
		}
		public static void SetValue(this Autodesk.Revit.DB.Element e, string param, object obj)
		{
			e.LookupParameter(param).SetValue(obj);
		}
		public static void SetValue(this Autodesk.Revit.DB.Parameter p, object obj)
		{
			var invalidTypeError = "Giá trị không đúng kiểu dữ liệu.";
			switch (p.StorageType)
			{
				case StorageType.Double:
					if (!(obj is double)) throw new Exception(invalidTypeError);
					p.Set((double)obj);
					break;
				case StorageType.Integer:
					if (!(obj is int)) throw new Exception(invalidTypeError);
					p.Set((int)obj);
					break;
				case StorageType.String:
					if (!(obj is string)) throw new Exception(invalidTypeError);
					p.Set((string)obj);
					break;
				case StorageType.ElementId:
					if (obj is Autodesk.Revit.DB.Element)
					{
						p.Set(((Autodesk.Revit.DB.Element)obj).Id);
					}
					else if (obj is Autodesk.Revit.DB.ElementId)
					{
						p.Set((Autodesk.Revit.DB.ElementId)obj);
					}
					else throw new Exception(invalidTypeError);
					break;
			}
		}

		public static double GetConvertValue(this double value, Autodesk.Revit.DB.DisplayUnitType dut)
		{
			switch (dut)
			{
				case DisplayUnitType.DUT_MILLIMETERS:
				case DisplayUnitType.DUT_METERS:
					return UnitUtils.Convert(value, DisplayUnitType.DUT_DECIMAL_FEET, dut);
				case DisplayUnitType.DUT_SQUARE_MILLIMETERS:
				case DisplayUnitType.DUT_SQUARE_METERS:
					return UnitUtils.Convert(value, DisplayUnitType.DUT_SQUARE_FEET, dut);
				case DisplayUnitType.DUT_CUBIC_MILLIMETERS:
				case DisplayUnitType.DUT_CUBIC_METERS:
					return UnitUtils.Convert(value, DisplayUnitType.DUT_CUBIC_FEET, dut);
				case DisplayUnitType.DUT_DECIMAL_DEGREES:
					return UnitUtils.Convert(value, DisplayUnitType.DUT_RADIANS, dut);
				default:
					throw new Exception("Trường hợp này chưa xét tới");
			}
		}

		public static double ConvertValue(this double value, string unit)
		{
			switch (unit)
			{
				case "m3":
					return UnitUtils.Convert(value, DisplayUnitType.DUT_CUBIC_FEET, DisplayUnitType.DUT_CUBIC_METERS);
				case "m2":
					return UnitUtils.Convert(value, DisplayUnitType.DUT_SQUARE_FEET, DisplayUnitType.DUT_SQUARE_METERS);
			}
			throw new Exception("This code should not have been reached!");
		}

		public static CategorySet GetCategories(this Parameter p)
		{
			var def = p.Definition;
			var bindingMap = revitData.BindingMap;
			var binding = bindingMap.get_Item(def) as ElementBinding;

			return binding?.Categories;
		}
		public static List<Category> ConvertList(this CategorySet cateSet)
		{
			var cates = new List<Category>();
			foreach (Category category in cateSet)
			{
				cates.Add(category);
			}
			return cates;
		}

		public static Autodesk.Revit.DB.Parameter LookupParameter
			(this BuiltInCategory bic, string name,
			bool isElementType = false)
		{
			var elems = isElementType ? revitData.TypeElements : revitData.InstanceElements;
			var elem = elems.Where(x => x.Category?.Id.IntegerValue == (int)bic)
				.FirstOrDefault();
			if (elem == null)
			{
				throw new Exception("Element is null");
			}
			var p = elem.LookupParameter(name);
			if (p == null) throw new Exception("Không tìm thấy Parameter");
			return p;
		}

		public static Autodesk.Revit.DB.Parameter LookupParameter
			(this IEnumerable<BuiltInCategory> bics, string name, bool isElementType = false)
		{
			Parameter p = null;
			foreach (var bic in bics)
			{
				p = bic.LookupParameter(name, isElementType);
			}
			return p;
		}
	}
}
