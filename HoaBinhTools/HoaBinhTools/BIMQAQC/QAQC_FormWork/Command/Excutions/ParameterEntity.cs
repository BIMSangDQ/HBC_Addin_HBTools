using Autodesk.Revit.DB;
using BeyCons.Core.RevitUtils;
using BeyCons.Core.RevitUtils.AddinIdentity;
using BeyCons.Core.RevitUtils.DataUtils;
using BeyCons.Core.RevitUtils.DataUtils.Enums;
using BeyCons.Core.RevitUtils.DataUtils.Models;
using HoaBinhTools.BIMQAQC.QAQC_FormWork.ViewModels;
using System.Collections.Generic;
using System.Linq;
using Utils;

namespace HoaBinhTools.BIMQAQC.QAQC_FormWork.Command.Excutions
{
	public class ParameterEntity
	{
		#region Field
		private static List<string> parameterNames;
		private static List<List<ParameterInformation>> parameterInformationss;
		#endregion

		#region Properties
		public static List<string> ParameterNames()
		{
			parameterNames = new List<string>()
					{
						QAQCCustomParameters.HB_FW_Total.ToString(),
						QAQCCustomParameters.HB_FW_Side.ToString(),
						QAQCCustomParameters.HB_FW_ESide.ToString(),
						QAQCCustomParameters.HB_FW_MSide.ToString(),
						QAQCCustomParameters.HB_FW_BSide.ToString(),
						QAQCCustomParameters.HB_FW_DSide.ToString(),
						QAQCCustomParameters.HB_FW_DESide.ToString(),
						QAQCCustomParameters.HB_FW_DMSide.ToString(),
						QAQCCustomParameters.HB_FW_DBSide.ToString(),
						QAQCCustomParameters.HB_FW_Bottom.ToString(),
						QAQCCustomParameters.HB_FW_DBottom.ToString(),
					};
			return parameterNames;
		}
		public static List<List<ParameterInformation>> ParameterInformationss
		{
			get
			{
				if (null == parameterInformationss)
				{
					parameterInformationss = new List<List<ParameterInformation>>()
					{
						new List<ParameterInformation>()
						{
							new ParameterInformation(){Name = QAQCCustomParameters.HB_FW_MSide.ToString(), ReadOnly = false, Description = "Cốp pha mặt bên được tính theo m dài", Visible = true},
						},
						new List<ParameterInformation>()
						{
							new ParameterInformation(){Name = QAQCCustomParameters.HB_FW_ESide.ToString(), ReadOnly = false, Description = "2 đầu dầm.", Visible = true},
							new ParameterInformation(){Name = QAQCCustomParameters.HB_FW_BSide.ToString(), ReadOnly = false, Description = "Phần cốp pha cột vách bên dưới", Visible = true},
							new ParameterInformation(){Name = QAQCCustomParameters.HB_FW_Side.ToString(), ReadOnly = false, Description = "Total side area formwork.", Visible = true},
							new ParameterInformation(){Name = QAQCCustomParameters.HB_FW_Bottom.ToString(), ReadOnly = false, Description = "Tổng cốp pha đáy", Visible = true},
							new ParameterInformation(){Name = QAQCCustomParameters.HB_FW_Total.ToString(), ReadOnly = false, Description = "Tổng cốp pha", Visible = true},
						},
						new List<ParameterInformation>()
						{
							new ParameterInformation(){Name = QAQCCustomParameters.HB_FW_DSide.ToString(), ReadOnly = false, Description = "Giải trình para Side", Visible = true},
							new ParameterInformation(){Name = QAQCCustomParameters.HB_FW_DESide.ToString(), ReadOnly = false, Description = "Giải trình para ESide", Visible = true},
							new ParameterInformation(){Name = QAQCCustomParameters.HB_FW_DMSide.ToString(), ReadOnly = false, Description = "Giải trình para MSide", Visible = true},
							new ParameterInformation(){Name = QAQCCustomParameters.HB_FW_DBSide.ToString(), ReadOnly = false, Description = "Giải trình para BSide", Visible = true},
							new ParameterInformation(){Name = QAQCCustomParameters.HB_FW_DBottom.ToString(), ReadOnly = false, Description = "Giải trình para Bottom", Visible = true},
						},
						new List<ParameterInformation>()
						{
							new ParameterInformation(){Name = QAQCCustomParameters.HB_FW_Category.ToString(), ReadOnly = true, Description = "Identify formwork.", Visible = true},
							new ParameterInformation(){Name = QAQCCustomParameters.HB_FW_HostId.ToString(), ReadOnly = true, Description = "Use to delete formwork.", Visible = true},
						},
					};
				}
				return parameterInformationss;
			}
		}
		public static string FileShareName { get; set; } = "QAQCFormWork";
		public static bool CheckParameters
		{
			get
			{
				bool result = true;
				if (!BeyCons.Core.RevitUtils.DataUtils.ParameterUtils.AreParametersShareInProject(ParameterNames()))
				{
					result = false;
				}
				if (!result)
				{
					IEnumerable<SharedParameterElement> sharedParameterElements =
						new FilteredElementCollector(ActiveData.Document)
						.OfClass(typeof(SharedParameterElement)).Cast<SharedParameterElement>();

					List<ElementId> shareParameterDelete = new List<ElementId>();

					foreach (SharedParameterElement sharedParameterElement in sharedParameterElements)
					{
						if (ParameterNames().Contains(sharedParameterElement.Name))
						{
							shareParameterDelete.Add(sharedParameterElement.Id);
						}
					}

					if (shareParameterDelete.Count > 0)
					{
						using (Transaction transaction = new Transaction(ActiveData.Document))
						{
							transaction.Start("Delete Parameters");
							ActiveData.Document.Delete(shareParameterDelete);
							transaction.Commit();
						}
					}
				}
				return result;
			}
		}
		#endregion

		#region Method
		public static void CreateParameters()
		{
			RevitData.Instance.Transaction.Start("Create Parameters");

			List<BuiltInCategory> builtInCategories = FormworkViewModels.Categories.Select(c => (BuiltInCategory)(c.Id.IntegerValue)).ToList();
			BeyCons.Core.RevitUtils.DataUtils.ParameterUtils.CreateParameterUseShare(ActiveData.Document, FileShareName, 
												ParameterInformationss[0],ParameterType.Length, builtInCategories, BuiltInParameterGroup.PG_IDENTITY_DATA, CustomGroupParameters.HB_QAQC_Formwork, true);

			BeyCons.Core.RevitUtils.DataUtils.ParameterUtils.AddParameterToFileShare(ActiveData.Document, CustomGroupParameters.HB_QAQC_Formwork,
												ParameterInformationss[1], ParameterType.Area, builtInCategories, BuiltInParameterGroup.PG_IDENTITY_DATA, true);

			BeyCons.Core.RevitUtils.DataUtils.ParameterUtils.AddParameterToFileShare(ActiveData.Document, CustomGroupParameters.HB_QAQC_Formwork,
												ParameterInformationss[2], ParameterType.Text, builtInCategories, BuiltInParameterGroup.PG_IDENTITY_DATA, true);

			BeyCons.Core.RevitUtils.DataUtils.ParameterUtils.AddParameterToFileShare(ActiveData.Document, CustomGroupParameters.HB_QAQC_Formwork,
												ParameterInformationss[3], ParameterType.Text, builtInCategories, BuiltInParameterGroup.PG_IDENTITY_DATA, true);

			RevitData.Instance.Transaction.Commit();
		}
		#endregion
	}

	public enum QAQCCustomParameters
	{
		HB_FW_Total = 1,
		HB_FW_Side = 2,
		HB_FW_ESide = 3,
		HB_FW_MSide = 4,
		HB_FW_BSide = 5,
		HB_FW_DSide = 6,
		HB_FW_DESide = 7,
		HB_FW_DMSide = 8,
		HB_FW_DBSide = 9,

		HB_FW_Bottom = 10,
		HB_FW_DBottom = 11,

		HB_FW_Category = 12,
		HB_FW_HostId = 13,
	}

}
