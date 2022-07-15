using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Visual;
using HoaBinhTools.SynchronizedData.Models;
using HoaBinhTools.SynchronizedData.Models.CenterData;
using HoaBinhTools.SynchronizedData.Models.GroupFileModel;
using HoaBinhTools.SynchronizedData.Models.MaterialModel;
using HoaBinhTools.SynchronizedData.Models.WallTypeModel;
using Utils;

namespace HoaBinhTools.SynchronizedData.Db
{
	public class DbUtils
	{
		#region Db of Group
		//Create Centerdata
		public void Create_CenterData(string Type, string TypeName, GroupFileModel CurrentGroup)
		{
			DbConnect db = new DbConnect();
			string idGroup = Get_GroupId(CurrentGroup);
			string query = string.Format("INSERT INTO Addin_CenterData (Type, TypeName,GroupId,IdFiles,Author,Time) VALUES('{0}',N'{1}','{2}','{3}','{4}','{5}');",
							Type, TypeName, Get_GroupId(CurrentGroup), Get_ListIdFile_Group(CurrentGroup).Replace(GetIdCurrentFile() + ".", ""), ActiveData.Username, DateTime.Now.ToString());
			db.Execute_SQL(query);
			db.Close_DB_Connection();
		}

		//Get CenterdataId
		public string Get_CenterId(string Type, string TypeName, GroupFileModel CurrentGroup)
		{
			DbConnect db = new DbConnect();
			string query = string.Format("SELECT * FROM Addin_CenterData WHERE Type = '{0}' and TypeName = N'{1}' and GroupId = '{2}' and IdFiles = '{3}' and Author = '{4}'",
				Type, TypeName, Get_GroupId(CurrentGroup), Get_ListIdFile_Group(CurrentGroup).Replace(GetIdCurrentFile() + ".", ""), ActiveData.Username);
			DataTable table = db.Get_DataTable(query);
			db.Close_DB_Connection();

			return table.Rows[table.Rows.Count - 1]["Id"].ToString();
		}

		//Get GroupId of CurrentGroup
		public string Get_GroupId(GroupFileModel CurrentGroup)
		{
			DbConnect db = new DbConnect();
			string query = string.Format("SELECT * FROM Addin_GroupData WHERE Group_Name = '{0}'", CurrentGroup.Group_Name);
			DataTable table = db.Get_DataTable(query);
			db.Close_DB_Connection();

			return table.Rows[0]["Id_Group"].ToString();
		}

		//Get List<ID> File of Current Group
		public string Get_ListIdFile_Group(GroupFileModel CurrentGroup)
		{
			DbConnect db = new DbConnect();
			string query = string.Format("SELECT * FROM Addin_GroupData WHERE Group_Name = '{0}'", CurrentGroup.Group_Name);
			DataTable table = db.Get_DataTable(query);
			db.Close_DB_Connection();

			return table.Rows[0]["FileIds"].ToString();
		}

		//Get GroupId of File
		public string GetIdCurrentFile()
		{
			string fname = "";
			try
			{
				var modelPath = ActiveData.Document.GetWorksharingCentralModelPath();

				var centralServerPath = ModelPathUtils.ConvertModelPathToUserVisiblePath(modelPath);

				fname = centralServerPath.ToString();
			}
			catch
			{
				fname = ActiveData.Document.PathName;
			}
			fname = fname.Replace(@"\\192.168.1.100\doc_center", "T:");
			DbConnect db = new DbConnect();

			string query = string.Format("SELECT * FROM Addin_FileData WHERE File_Path = '{0}'", fname);
			DataTable table = db.Get_DataTable(query);
			db.Close_DB_Connection();

			return table.Rows[0]["Id_File"].ToString();
		}

		//Get List Update of Group
		public ObservableCollection<CenterData> CheckUpdateWithCurrentFile(GroupFileModel CurrentGroup)
		{
			DbConnect db = new DbConnect();
			string IdGroup = Get_GroupId(CurrentGroup);
			string IdCurretFile = GetIdCurrentFile();
			ObservableCollection<CenterData> CenterDb = new ObservableCollection<CenterData>();

			string query = string.Format("SELECT * FROM Addin_CenterData WHERE GroupId = '{0}'", IdGroup);
			DataTable table = db.Get_DataTable(query);
			foreach (DataRow row in table.Rows)
			{
				var ss = row["IdFiles"].ToString().Split('.');
				foreach (string s in ss)
				{
					if (s == IdCurretFile)
					{
						CenterDb.Add(new CenterData
						{
							IsCheck = true,
							Id = row["Id"].ToString(),
							Type = row["Type"].ToString(),
							TypeName = row["TypeName"].ToString(),
							GroupID = row["GroupId"].ToString(),
							IdFiles = row["IdFiles"].ToString(),
							Author = row["Author"].ToString(),
							Time = row["Time"].ToString(),
						});
					}
				}
			}
			db.Close_DB_Connection();
			return CenterDb;
		}
		#endregion

		//Delete
		public void DeleteCenterData(string centerId)
		{
			DbConnect db = new DbConnect();
			string query = string.Format("SELECT * FROM Addin_CenterData WHERE Id = '{0}'", centerId);
			DataTable table = db.Get_DataTable(query);
			string check = table.Rows[0]["IdFiles"].ToString().Replace(" ", "");
			if (check != GetIdCurrentFile() + ".")
			{
				query = string.Format("UPDATE Addin_CenterData SET IdFiles = '{0}' WHERE Id = '{1}'", check.Replace(GetIdCurrentFile() + ".", ""), centerId);
				db.Execute_SQL(query);
			}
			else
			{
				query = string.Format("UPDATE Addin_CenterData SET IdFiles = '{0}' WHERE Id = '{1}'", check.Replace(GetIdCurrentFile() + ".", ""), centerId);
				db.Execute_SQL(query);
				//Xóa các db liên quan nếu là file update cuối cùng
				query = string.Format("DELETE FROM Addin_CenterData WHERE Id = '{0}'", centerId);
				db.Execute_SQL(query);
				query = string.Format("DELETE FROM Addin_WallType_CompoundStructure WHERE IdCenter = '{0}'", centerId);
				db.Execute_SQL(query);
				query = string.Format("DELETE FROM Addin_WallTypeItem WHERE Id_Center = '{0}'", centerId);
				db.Execute_SQL(query);
				query = string.Format("DELETE FROM Addin_Parameter WHERE Id_Center = '{0}'", centerId);
				db.Execute_SQL(query);
				query = string.Format("DELETE FROM Addin_CurtainWallItem WHERE Id_Center = '{0}'", centerId);
				db.Execute_SQL(query);
				query = string.Format("DELETE FROM Addin_MaterialItem WHERE Id_Center = '{0}'", centerId);
				db.Execute_SQL(query);
			}

			db.Close_DB_Connection();
		}

		public int countUpdate(string fileid)
		{
			DbConnect db = new DbConnect();
			string query = string.Format("SELECT * FROM Addin_CenterData");

			int i = 0;

			DataTable table = db.Get_DataTable(query);
			foreach (DataRow row in table.Rows)
			{
				string s = row["IdFiles"].ToString();
				if (s.Contains(fileid + "."))
				{
					i++;
				}
			}

			db.Close_DB_Connection();

			return i;
		}

		public FileModel GetCurrentFileModel()
		{

			string fname = "";
			try
			{
				var modelPath = ActiveData.Document.GetWorksharingCentralModelPath();

				var centralServerPath = ModelPathUtils.ConvertModelPathToUserVisiblePath(modelPath);

				fname = centralServerPath.ToString();
			}
			catch
			{
				fname = ActiveData.Document.PathName;
			}

			var s = fname.Split('\\');
			FileModel fileModel = new FileModel()
			{
				File_Path = fname,
				File_Name = s[s.Length - 1]
			};
			return fileModel;
		}


		#region Db of Type

		public string CreateDb_CompoundStructureWallType(CompoundStructureWallType la, string IdCenter)
		{
			DbConnect db = new DbConnect();
			string query = string.Format("INSERT INTO Addin_WallType_CompoundStructure (LayerFunction,Layerid,Layerwidth,MaterialName,MinU," +
				"MinV,MaxU,MaxV,TopExtension,BotExtension,IdCenter) VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}','{9}','{10}')",
				la.LayerFunction,
				la.Layerid,
				la.Layerwidth,
				la.MaterialName,
				la.MinU,
				la.MinV,
				la.MaxU,
				la.MaxV,
				la.TopExtension == true ? 1 : 0,
				la.BotExtension == true ? 1 : 0,
				IdCenter);
			db.Execute_SQL(query);
			query = string.Format("SELECT Id FROM Addin_WallType_CompoundStructure WHERE LayerFunction = '{0}'" +
				"AND Layerid = '{1}' AND Layerwidth = '{2}' AND MaterialName = '{3}' AND MinU = '{4}'" +
				" AND MinV = '{5}' AND MaxU = '{6}' AND MaxV = '{7}' AND TopExtension = '{8}' AND BotExtension = '{9}'" +
				" AND IdCenter = '{10}'",
				la.LayerFunction,
				la.Layerid,
				la.Layerwidth,
				la.MaterialName,
				la.MinU,
				la.MinV,
				la.MaxU,
				la.MaxV,
				la.TopExtension == true ? 1 : 0,
				la.BotExtension == true ? 1 : 0,
				IdCenter);
			DataTable table = db.Get_DataTable(query);
			db.Close_DB_Connection();

			return table.Rows[0]["Id"].ToString();
		}

		public string CreateDb_CompoundStructureFloorType(CompoundStructureFloorType la, string IdCenter)
		{
			DbConnect db = new DbConnect();
			string query = string.Format("INSERT INTO Addin_WallType_CompoundStructure (LayerFunction,Layerid,Layerwidth,MaterialName,IdCenter)" +
				" VALUES('{0}', '{1}', '{2}', '{3}', '{4}')",
				la.LayerFunction,
				la.Layerid,
				la.Layerwidth,
				la.MaterialName,
				IdCenter);
			db.Execute_SQL(query);
			query = string.Format("SELECT Id FROM Addin_WallType_CompoundStructure WHERE LayerFunction = '{0}'" +
				"AND Layerid = '{1}' AND Layerwidth = '{2}' AND MaterialName = '{3}' AND IdCenter = '{4}'",
				la.LayerFunction,
				la.Layerid,
				la.Layerwidth,
				la.MaterialName,
				IdCenter);
			DataTable table = db.Get_DataTable(query);
			db.Close_DB_Connection();

			return table.Rows[0]["Id"].ToString();
		}

		//Create WallTypeItem
		public void CreateWallTypeDb(WallTypeItem WallTypeItem, string IdWallLayer, string IDCenter)
		{
			DbConnect db = new DbConnect();
			string query = string.Format("INSERT INTO Addin_WallTypeItem (Id_Center,WallTypeName,Wallfunction,FamilyName,Exterior,Interior,StructureIndex," +
				"MinLayerV,MaxLayerV,ScaleFillColor,ScaleFillPattern,IdsCompound ) VALUES('{0}',N'{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}')",
				IDCenter,
				WallTypeItem.WallTypeName,
				WallTypeItem.Wallfunction,
				WallTypeItem.FamilyName,
				WallTypeItem.Exterior,
				WallTypeItem.Interior,
				WallTypeItem.StructureIndex,
				WallTypeItem.MinLayerV,
				WallTypeItem.MaxLayerV,
				WallTypeItem.ScaleFillColor,
				WallTypeItem.ScaleFillPattern,
				IdWallLayer);
			db.Execute_SQL(query);
			db.Close_DB_Connection();
		}

		//Create CurtainWallItem
		public void CreateCurtainWallTypeDb(CurtainWallItem curtainWallItem, string IDCenter)
		{
			DbConnect db = new DbConnect();
			string query = string.Format("INSERT INTO Addin_CurtainWallItem (Id_Center,WallTypeName,Wallfunction,AutomaticallyEmbed," +
				"StructuralMaterial,LayoutVert,SpacingVert,AdjustBorderVert,LayoutHoriz,SpacingHoriz,AdjustBorderHoriz,InteriorTypeVert," +
				"Border1Vert,Border2Vert,InteriorTypeHoriz,Border1Horiz,Border2Horiz,CurtainPanel,JoinCondition) VALUES('{0}',N'{1}','{2}','{3}','{4}','{5}','{6}','{7}'," +
				"'{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}')",
				IDCenter,
				curtainWallItem.WallTypeName,
				curtainWallItem.Wallfunction,
				curtainWallItem.Wall_AutomaticallyEmbed,
				curtainWallItem.StructuralMaterial,
				curtainWallItem.LayoutVert,
				curtainWallItem.SpacingVert,
				curtainWallItem.AdjustBorderVert,
				curtainWallItem.LayoutHoriz,
				curtainWallItem.SpacingHoriz,
				curtainWallItem.AdjustBorderHoriz,
				curtainWallItem.InteriorTypeVert,
				curtainWallItem.Border1Vert,
				curtainWallItem.Border2Vert,
				curtainWallItem.InteriorTypeHoriz,
				curtainWallItem.Border1Horiz,
				curtainWallItem.Border2Horiz,
				curtainWallItem.CurtainPanel,
				curtainWallItem.JoinCondition
				);

			db.Execute_SQL(query);
			db.Close_DB_Connection();
		}

		//Create FloorTypeItem
		public void CreateFloorTypeDb(FloorTypeItem FloorTypeItem, string IdWallLayer, string IDCenter)
		{
			DbConnect db = new DbConnect();
			string query = string.Format("INSERT INTO Addin_WallTypeItem (Id_Center,WallTypeName,FamilyName,Exterior,Interior,StructureIndex," +
				"ScaleFillColor,ScaleFillPattern,Wallfunction,IdsCompound) VALUES('{0}',N'{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}')",
				IDCenter,
				FloorTypeItem.FloorTypeName,
				FloorTypeItem.FamilyName,
				FloorTypeItem.Exterior,
				FloorTypeItem.Interior,
				FloorTypeItem.StructureIndex,
				FloorTypeItem.ScaleFillColor,
				FloorTypeItem.ScaleFillPattern,
				FloorTypeItem.FloorFunction,
								IdWallLayer);
			db.Execute_SQL(query);
			db.Close_DB_Connection();
		}

		//Create MaterialItem
		public void CreateMaterialDb(MaterialModel Mat, string IDCenter)
		{
			DbConnect db = new DbConnect();
			string query = string.Format("INSERT INTO Addin_MaterialItem (Id_Center,Name,Description,KeyNote,Mark," +
				"UserRenderAppearance,Color,Tranferancy,CutBackgroundPatternColor,CutBackgroundPatternName," +
				"CutForegroundPatternColor,CutForegroundPatternName,SurfaceBackgroundPatternColor,SurfaceBackgroundPatternName," +
				"SurfaceForegroundPatternColor,SurfaceForegroundPatternName,MaterialCategory,MaterialClass,Shininess," +
				"Smoothness,AppearanceName,StructuralAssetName,ThemalAssetName) " +
				"VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}'," +
				"'{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}')",
				IDCenter,
				Mat.Name,
				Mat.Description,
				Mat.KeyNote,
				Mat.Mark,
				Mat.UserRenderAppearance,
				Mat.Color,
				Mat.Tranferancy,
				Mat.CutBackgroundPatternColor,
				Mat.CutBackgroundPatternName,
				Mat.CutForegroundPatternColor,
				Mat.CutForegroundPatternName,
				Mat.SurfaceBackgroundPatternColor,
				Mat.SurfaceBackgroundPatternName,
				Mat.SurfaceForegroundPatternColor,
				Mat.SurfaceForegroundPatternName,
				Mat.MaterialCategory,
				Mat.MaterialClass,
				Mat.Shininess,
				Mat.Smoothness,
				Mat.AppearanceName,
				Mat.StructuralAssetName,
				Mat.ThemalAssetName
				);

			db.Execute_SQL(query);
			db.Close_DB_Connection();
		}

		public void CreateMaterialAssetProperties(List<AssetProperty> assets, string IDCenter)
		{
			DbConnect db = new DbConnect();
			string Type;
			string Name;
			string Property;
			string query = "";
			assets = assets.OrderBy(ap => ap.Name).ToList();
			for (int idx = 0; idx < assets.Count; idx++)
			{
				AssetProperty ap = assets[idx];
				Type type = ap.GetType();

				object apVal = null;
				try
				{
					var prop = type.GetProperty("Value");
					if (prop != null &&	prop.GetIndexParameters().Length == 0)
					{
						apVal = prop.GetValue(ap);
					}
					else
					{
						apVal = "<No Value Property>";
					}
				}
				catch (Exception ex)
				{
					apVal = ex.GetType().Name + "-" + ex.Message;
				}
				if (apVal is DoubleArray)
				{
					var doubles = apVal as DoubleArray;
					apVal = doubles.Cast<double>().Aggregate("", (s, d) => s + Math.Round(d, 5) + ",");
				}

				query = string.Format("INSERT INTO Addin_Material_AssetProperties (Id_Center,Type,Name,Property) " +
					"VALUES ('{0}','{1}','{2}','{3}')",
					IDCenter,
					ap.Type,
					ap.Name,
					apVal);
				db.Execute_SQL(query);

			}
			db.Close_DB_Connection();
		}

		public void CreateFamilySymbolDb(FamilySymbol fs, string IDCenter)
		{
			DbConnect db = new DbConnect();
			try
			{
				ParameterSet ps = fs.Parameters;
				foreach (Parameter p in ps)
				{
					string paraname = p.Definition.Name;
					double paraAsDouble = p.AsDouble();
					int paraAsInt = p.AsInteger();
					string paraAsStr = p.AsString();
					ElementId paraElementId = p.AsElementId();
					
					if (paraAsDouble != 0
						|| paraAsInt != 0
						|| (paraAsStr != "" && paraAsStr != null)
						|| paraElementId != new ElementId(-1))
					{
						Element e = null;
						string categoryName = "";
						if (paraElementId != new ElementId(-1))
						{
							e = ActiveData.Document.GetElement(paraElementId);
							if (e != null)
							{
								if (e.Category != null)
								{
									categoryName = e.Category.Name + " -- " + e.Name;
								}
								else
								{
									categoryName = ((Autodesk.Revit.DB.ElementType)e).FamilyName + " -- " + e.Name;
								}
								
							}
							else
							{
								categoryName = "BuiltInCategory" + " -- " + paraElementId;
							}
						}
						string query = string.Format("INSERT INTO Addin_Parameter (Id_Center,ParaName,AsDouble,AsInt,AsStr,AsEleId,BuiltInParameter) " +
						"VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}')",
						IDCenter,
						paraname,
						paraAsDouble,
						paraAsInt,
						paraAsStr,
						categoryName == "" ? paraElementId.ToString() : categoryName,
						p.Id);
						db.Execute_SQL(query);
					}
				}
			}
			catch (Exception ex)
			{ }

			db.Close_DB_Connection();
		}
		#endregion

	}
}
