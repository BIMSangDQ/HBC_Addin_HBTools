using Autodesk.Revit.DB;
using Autodesk.Revit.DB.IFC;
using Autodesk.Revit.DB.Structure;
using BeyCons.Core;
using BeyCons.Core.Extensions;
using BeyCons.Core.FormUtils.ControlViews;
using BeyCons.Core.RevitUtils;
using BeyCons.Core.RevitUtils.DataUtils;
using BeyCons.Core.Libraries.Geometries;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using BeyCons.Core.Utils;
using BeyCons.Core.Libraries.Units;
using HoaBinhTools.BIMQAQC.QAQC_Quantity.Models;

namespace HoaBinhTools.BIMQAQC.QAQC_Quantity.ViewModels
{
	public class GenerateModel
	{
		//private JsonUtils<SaveModel> jsonUtilitiesSaveModels;

		public object ElementsOfRevit { get; set; }
		private Random Random { get; set; } = new Random();
		public Category CategorySetup { get; set; }
		public string SelectButton { get; set; }
		public List<SaveModel> SaveModels { get; set; } = new List<SaveModel>();
		//public JsonUtils<SaveModel> JsonUtilitiesSaveModels
		//{
		//	get
		//	{
		//		if (jsonUtilitiesSaveModels == null)
		//		{
		//			jsonUtilitiesSaveModels = new JsonUtils<SaveModel>(FullPath);
		//		}
		//		return jsonUtilitiesSaveModels;
		//	}
		//}
		public string FullPath { get; set; }
		public string FamilyName { get; set; }
		public bool CheckUnionAllElements { get; set; }
		public Material GenerateMaterial { get; set; }

		public void CreateFamilyGenericModel()
		{
			Action action = new Action(() =>
			{
				string pathFamily = CreateFamilyUtils.GetPathDefault();
				if (pathFamily != null)
				{
					ModelSolid modelSolid = new ModelSolid();
					switch (SelectButton)
					{
						case "btSelectRevitLink":
							break;
						default:
							List<Element> elements = ElementsOfRevit as List<Element>;
							List<GuiIdSolid> guiIdSolidsActive = new List<GuiIdSolid>();
							ProgressBarInstance progressBarInstance = new ProgressBarInstance("Creating solid active elements.", elements.Count);
							foreach (Element element in elements)
							{
								progressBarInstance.Start();
								guiIdSolidsActive.Add(new GuiIdSolid() { GuiId = element.UniqueId, Solids = GeometryUtils.GetSolidsFromInstanceElement(element, new Options() { DetailLevel = ViewDetailLevel.Fine }, true).ToUnionSolids() });
							}
							modelSolid = new ModelSolid() { ModelName = "Active", GuiIdSolids = guiIdSolidsActive, GuiIdModel = null };
							break;
					}

					CreateFamily(pathFamily, modelSolid);
				}
				else
				{
					NotifyUtils.LackOfLibrary();
				}
			});
			RevitData.Instance.ExternalEventHandlerSingle.SetAction(action);
			RevitData.Instance.ExternalEventSingle.Raise();
		}

		public double GetVolumeGenericModel()
		{
			double Volume = 0;
			//string pathFamily = CreateFamilyUtils.GetPathDefault();
			//if (pathFamily != null)
			//{
				ModelSolid modelSolid = new ModelSolid();

				List<Element> elements = ElementsOfRevit as List<Element>;
				List<GuiIdSolid> guiIdSolidsActive = new List<GuiIdSolid>();
				ProgressBarInstance progressBarInstance = new ProgressBarInstance("Creating solid active elements.", elements.Count);
				foreach (Element element in elements)
				{
					progressBarInstance.Start();
					guiIdSolidsActive.Add(new GuiIdSolid() { GuiId = element.UniqueId, Solids = GeometryUtils.GetSolidsFromInstanceElement(element, new Options() { DetailLevel = ViewDetailLevel.Fine }, true).ToUnionSolids() });
				}
				modelSolid = new ModelSolid() { ModelName = "Active", GuiIdSolids = guiIdSolidsActive, GuiIdModel = null };

				List<Solid> solids = new List<Solid>();
				modelSolid.GuiIdSolids.ForEach(x => x.Solids.ForEach(y => solids.Add(y)));
				List<Solid> solidsUnion = solids.ToUnionSolids();


				try
				{
					foreach (Solid solidUnion in solidsUnion)
					{
						Volume += solidUnion.Volume.ToVolumeMeter();
					}
				}
				catch
				{
					Volume += 0;
				}
			//}
			//else
			//{
			//	NotifyUtils.LackOfLibrary();
			//}

			return Volume;
		}

		public List<Solid> GetSolidGenericModel()
		{
			ModelSolid modelSolid = new ModelSolid();

			List<Element> elements = ElementsOfRevit as List<Element>;
			List<GuiIdSolid> guiIdSolidsActive = new List<GuiIdSolid>();
			ProgressBarInstance progressBarInstance = new ProgressBarInstance("Creating solid active elements.", elements.Count);
			foreach (Element element in elements)
			{
				progressBarInstance.Start();
				guiIdSolidsActive.Add(new GuiIdSolid() { GuiId = element.UniqueId, Solids = GeometryUtils.GetSolidsFromInstanceElement(element, new Options() { DetailLevel = ViewDetailLevel.Fine }, true).ToUnionSolids() });
			}
			modelSolid = new ModelSolid() { ModelName = "Active", GuiIdSolids = guiIdSolidsActive, GuiIdModel = null };

			List<Solid> solids = new List<Solid>();
			modelSolid.GuiIdSolids.ForEach(x => x.Solids.ForEach(y => solids.Add(y)));
			List<Solid> solidsUnion = solids.ToUnionSolids();

			return solids;
		}
		public List<Solid> GetSolids(RevitLinkInstance revitLinkInstance, Element element)
		{
			List<Solid> solidsRevitLink = new List<Solid>();
			if (element.Category.Name == "Doors" || element.Category.Name == "Windows")
			{
				solidsRevitLink.Add(SolidUtils.CreateTransformed(CreateSolidFromDoorAndWindows(revitLinkInstance.GetLinkDocument(), element as FamilyInstance), revitLinkInstance.GetTotalTransform()));
			}
			else
			{
				List<Solid> solids = GeometryUtils.GetSolidsFromInstanceElement(element, new Options() { DetailLevel = ViewDetailLevel.Fine }, true).Select(x => SolidUtils.CreateTransformed(x, revitLinkInstance.GetTotalTransform())).ToList().ToUnionSolids();
				solids.ForEach(x => solidsRevitLink.Add(x));
			}
			return solidsRevitLink;
		}

		private Solid CreateSolidFromDoorAndWindows(Document document, FamilyInstance familyInstance)
		{
			XYZ location = (familyInstance.Location as LocationPoint).Point;
			Wall wall = familyInstance.Host as Wall;
			CurveLoop curveLoop = ExporterIFCUtils.GetInstanceCutoutFromWall(document, wall, familyInstance, out XYZ vector);
			XYZ point = GeometryLib.TranslatingPoint(curveLoop.GetPlane().Origin, curveLoop.GetPlane().Normal, 1);
			if (GeometryLib.IsSameSide(curveLoop.GetPlane(), new List<XYZ>() { location, point }))
			{
				Transform transform = Transform.CreateTranslation(curveLoop.GetPlane().Normal.Negate() * (wall.Width / 2));
				curveLoop.Transform(transform);
				Solid solid = GeometryCreationUtilities.CreateExtrusionGeometry(new List<CurveLoop>() { curveLoop }, curveLoop.GetPlane().Normal, wall.Width * 2);
				return solid;
			}
			else
			{
				Transform transform = Transform.CreateTranslation(curveLoop.GetPlane().Normal * (wall.Width / 2));
				curveLoop.Transform(transform);
				Solid solid = GeometryCreationUtilities.CreateExtrusionGeometry(new List<CurveLoop>() { curveLoop }, curveLoop.GetPlane().Normal.Negate(), wall.Width * 2);
				return solid;
			}
		}

		private void CreateFamily(string pathFamily, ModelSolid modelSolid)
		{
			if (CheckUnionAllElements == true)
			{
				SaveModel saveModel = new SaveModel() { GuiIdDocument = modelSolid.GuiIdModel };

				string path = Path.Combine(Path.GetTempPath(), $"{FamilyName}.rfa");
				Document familyDoc = RevitData.Instance.Application.NewFamilyDocument(pathFamily);
				FamilyParameter familyParameter = null;
				using (Transaction transaction = new Transaction(familyDoc, "Add Parameter & Set Cayegory"))
				{
					transaction.Start();
					familyParameter = familyDoc.FamilyManager.AddParameter("Material", BuiltInParameterGroup.PG_MATERIALS, ParameterType.Material, false);
					familyDoc.OwnerFamily.FamilyCategory = CategorySetup;
					transaction.Commit();
				}

				using (Transaction transactionFamily = new Transaction(familyDoc, "Add Element"))
				{
					transactionFamily.Start();
					List<Solid> solids = new List<Solid>();
					modelSolid.GuiIdSolids.ForEach(x => x.Solids.ForEach(y => solids.Add(y)));
					List<Solid> solidsUnion = solids.ToUnionSolids();

					foreach (Solid solid in solidsUnion)
					{
						FreeFormElement freeFormElement = FreeFormElement.Create(familyDoc, solid);
						Parameter parameterFreeForm = freeFormElement.get_Parameter(BuiltInParameter.MATERIAL_ID_PARAM);
						familyDoc.FamilyManager.AssociateElementParameterToFamilyParameter(parameterFreeForm, familyParameter);
					}
					saveModel.GuiIdElements = modelSolid.GuiIdSolids.Select(x => x.GuiId).ToList();
					transactionFamily.Commit();
				}

				familyDoc.SaveAs(path, new SaveAsOptions() { OverwriteExistingFile = true, MaximumBackups = 1 });
				familyDoc.Close(true);

				SaveModels.Add(PlaceInstance(path, saveModel));
			}
			else
			{
				ProgressBarInstance progressBarInstance = new ProgressBarInstance("Creating family ...", modelSolid.GuiIdSolids.Count);
				foreach (GuiIdSolid guiIdSolid in modelSolid.GuiIdSolids)
				{
					foreach (Solid solid in guiIdSolid.Solids)
					{
						SaveModel saveModel = new SaveModel() { GuiIdDocument = modelSolid.GuiIdModel, GuiIdElements = new List<string>() { guiIdSolid.GuiId } };
						progressBarInstance.Start();
						string path = Path.Combine(Path.GetTempPath(), $"Model {modelSolid.ModelName} {Random.Next(100000, 999999)}.rfa");
						Document familyDoc = RevitData.Instance.Application.NewFamilyDocument(pathFamily);
						FamilyParameter familyParameter = null;
						using (Transaction transaction = new Transaction(familyDoc, "Add Parameter & Set Cayegory"))
						{
							transaction.Start();
							familyParameter = familyDoc.FamilyManager.AddParameter("Material", BuiltInParameterGroup.PG_MATERIALS, ParameterType.Material, false);
							familyDoc.OwnerFamily.FamilyCategory = CategorySetup;
							transaction.Commit();
						}

						using (Transaction transactionFamily = new Transaction(familyDoc, "Add Element"))
						{
							transactionFamily.Start();
							FreeFormElement freeFormElement = FreeFormElement.Create(familyDoc, solid);
							Parameter parameterFreeForm = freeFormElement.get_Parameter(BuiltInParameter.MATERIAL_ID_PARAM);
							familyDoc.FamilyManager.AssociateElementParameterToFamilyParameter(parameterFreeForm, familyParameter);
							transactionFamily.Commit();
						}

						familyDoc.SaveAs(path, new SaveAsOptions() { OverwriteExistingFile = true, MaximumBackups = 1 });
						familyDoc.Close(true);

						SaveModels.Add(PlaceInstance(path, saveModel));
					}
				}
			}
		}

		public SaveModel PlaceInstance(string path, SaveModel saveModel)
		{
			using (Transaction transaction = new Transaction(RevitData.Instance.Document, "Place Instance"))
			{
				WarningUtils ignoreWarning = new WarningUtils();
				FailureHandlingOptions options = ignoreWarning.GetFailureHandling(transaction);

				transaction.Start();
				RevitData.Instance.Document.LoadFamily(path, new LoadFamilyUtils(), out Family family);
				FamilySymbol familySymbol = new FilteredElementCollector(RevitData.Instance.Document).WherePasses(new FamilySymbolFilter(family.Id)).FirstElement() as FamilySymbol;
				familySymbol.Activate();
				FamilyInstance familyInstance = RevitData.Instance.Document.Create.NewFamilyInstance(XYZ.Zero, familySymbol, StructuralType.NonStructural);
				RevitData.Instance.Document.GetElement(familyInstance.GetTypeId()).LookupParameter("Material").Set(GenerateMaterial.Id);
				saveModel.GuiIDFamilySymbol = familySymbol.UniqueId;
				transaction.Commit(options);
				File.Delete(path); File.Delete(Path.Combine(Path.GetTempPath(), $"{family.Name}.0001.rfa"));
				return saveModel;
			}
		}

		public void UpdateForFamily(FamilySymbol familySymbolSelected)
		{
			Action action = new Action(() =>
			{
				string pathFamily = CreateFamilyUtils.GetPathDefault();
				if (pathFamily != null)
				{
					ModelSolid modelSolid = new ModelSolid();
					switch (SelectButton)
					{
						case "btSelectRevitLink":
							break;
						default:
							List<Element> elements = ElementsOfRevit as List<Element>;
							List<GuiIdSolid> guiIdSolidsActive = new List<GuiIdSolid>();
							ProgressBarInstance progressBarInstance = new ProgressBarInstance("Creating solid active elements.", elements.Count);
							foreach (Element element in elements)
							{
								progressBarInstance.Start();
								guiIdSolidsActive.Add(new GuiIdSolid() { GuiId = element.UniqueId, Solids = GeometryUtils.GetSolidsFromInstanceElement(element, new Options() { DetailLevel = ViewDetailLevel.Fine }, true).ToUnionSolids() });
							}
							modelSolid = new ModelSolid() { ModelName = "Active", GuiIdSolids = guiIdSolidsActive, GuiIdModel = null };
							break;
					}

					string path = Path.Combine(Path.GetTempPath(), $"{familySymbolSelected.Name}.rfa");
					Document familyDoc = RevitData.Instance.Application.NewFamilyDocument(pathFamily);
					FamilyParameter familyParameter = null;
					using (Transaction transaction = new Transaction(familyDoc, "Add Parameter & Set Cayegory"))
					{
						transaction.Start();
						familyParameter = familyDoc.FamilyManager.AddParameter("Material", BuiltInParameterGroup.PG_MATERIALS, ParameterType.Material, false);
						familyDoc.OwnerFamily.FamilyCategory = CategorySetup;
						transaction.Commit();
					}

					using (Transaction transactionFamily = new Transaction(familyDoc, "Add Element"))
					{
						transactionFamily.Start();
						List<Solid> solids = new List<Solid>();
						modelSolid.GuiIdSolids.ForEach(x => x.Solids.ForEach(y => solids.Add(y)));
						List<Solid> solidsUnion = solids.ToUnionSolids();
						foreach (Solid solid in solidsUnion)
						{
							FreeFormElement freeFormElement = FreeFormElement.Create(familyDoc, solid);
							Parameter parameterFreeForm = freeFormElement.get_Parameter(BuiltInParameter.MATERIAL_ID_PARAM);
							familyDoc.FamilyManager.AssociateElementParameterToFamilyParameter(parameterFreeForm, familyParameter);
						}
						transactionFamily.Commit();
					}

					familyDoc.SaveAs(path, new SaveAsOptions() { OverwriteExistingFile = true, MaximumBackups = 1 });
					familyDoc.Close(true);

					using (Transaction transaction = new Transaction(RevitData.Instance.Document, "Place Instance"))
					{
						WarningUtils ignoreWarning = new WarningUtils();
						FailureHandlingOptions options = ignoreWarning.GetFailureHandling(transaction);

						transaction.Start();
						RevitData.Instance.Document.LoadFamily(path, new LoadFamilyUtils(), out Family family);
						FamilySymbol newFamilySymbol = new FilteredElementCollector(RevitData.Instance.Document).WherePasses(new FamilySymbolFilter(family.Id)).FirstElement() as FamilySymbol;
						newFamilySymbol.Activate();
						newFamilySymbol.LookupParameter("Material").Set(GenerateMaterial.Id);

						List<FamilyInstance> familyInstances = new FilteredElementCollector(RevitData.Instance.Document).OfClass(typeof(FamilyInstance)).Cast<FamilyInstance>().Where(x => x.Name == familySymbolSelected.Name).ToList();
						familyInstances.ForEach(x => x.Symbol = newFamilySymbol);

						transaction.Commit(options);
						File.Delete(path); File.Delete(Path.Combine(Path.GetTempPath(), $"{family.Name}.0001.rfa"));
					}
				}
				else
				{
					NotifyUtils.LackOfLibrary();
				}
				NotifyUtils.Update();
			});
			RevitData.Instance.ExternalEventHandlerSingle.SetAction(action);
			RevitData.Instance.ExternalEventSingle.Raise();
		}
	}
}

