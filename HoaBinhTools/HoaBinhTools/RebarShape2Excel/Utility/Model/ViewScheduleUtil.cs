using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Model.Entity;
using SingleData;

namespace Utility
{
	public static class ViewScheduleUtil
	{
		private static RevitModelData revitModelData
		{
			get { return RevitModelData.Instance; }
		}
		public static ViewSchedule CreateTemporaryScheduleWithElementGuid(this ViewSchedule vs)
		{
			var doc = RevitData.Instance.Document;
			var trans = RevitData.Instance.Transaction;
			var revitModelData = RevitModelData.Instance;

			List<Autodesk.Revit.DB.Element> elems = new FilteredElementCollector(doc, vs.Id).WhereElementIsNotElementType()
				.OfCategory((BuiltInCategory)vs.Definition.CategoryId.IntegerValue).ToList();

			string newParamName = "temp";
			var p = elems.First().LookupParameter(newParamName);
			if (p == null)
			{
				System.Windows.MessageBox.Show("Đối tượng Rebar chưa có tham biến temp kiểu Text. Vui lòng cập nhập và chạy lại add-in.");
				return null;
			}

			ViewSchedule vs1 = doc.GetElement(vs.Duplicate(ViewDuplicateOption.Duplicate)) as ViewSchedule;

			foreach (Autodesk.Revit.DB.Element elem in elems)
			{
				RebarShapeInfo rsi = null;
				try
				{
					rsi = (elem as Autodesk.Revit.DB.Structure.Rebar).GetCurve2DsFromRebar();
				}
				catch
				{
					continue;
				}
				bool isExist = false;
				string guid = null;
				for (int i = 0; i < revitModelData.RebarShapeInfos.Count; i++)
				{
					if (rsi.SimpleCompare(revitModelData.RebarShapeInfos[i]))
					{
						isExist = true;
						guid = revitModelData.RebarShapeInfos[i].GUID;
						break;
					}
				}
				if (!isExist)
				{
					rsi.Initial();
					rsi.Edit();
					guid = rsi.GUID = elem.UniqueId;
					revitModelData.RebarShapeInfos.Add(rsi);
				}
				p = elem.LookupParameter(newParamName);
				p.Set(guid);
			}
			SchedulableField schedulableField = vs1.Definition.GetSchedulableFields().Where(x => x.GetName(doc) == newParamName).First();

			try
			{
				vs1.Definition.AddField(schedulableField);
				vs1.RefreshData();
			}
			catch
			{ }

			return vs1;
		}
	}
}
