#region Using
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using System.Collections.Generic;
using System.IO;
using System.Linq;
#endregion

namespace BeyCons.Core.RevitUtils.DataUtils
{
    public static class CreateFamilyUtils
    {
        public static string GetPathDefault(string fullNameFamilyTemplate = "Metric Generic Model.rft")
        {
            try
            {
                string familyPath = RevitData.Instance.Document.Application.FamilyTemplatePath;
                IEnumerable<string> files = Directory.EnumerateFiles(familyPath, fullNameFamilyTemplate, SearchOption.AllDirectories);
                if (files.Count() > 0)
                {
                    return files.FirstOrDefault();
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }
        public static FamilyInstance CreateFamilyFreeFormNonOriginal(string fullPathFamily, Solid solid, string nameFamily, Category category)
        {
            Document familyDoc = RevitData.Instance.Application.NewFamilyDocument(fullPathFamily);
            string path = Path.Combine(Path.GetTempPath(), $"{nameFamily}.rfa");

            Transform transform = Transform.CreateTranslation(XYZ.Zero - solid.ComputeCentroid());

            FamilyParameter familyParameter = null;
            using (Transaction transaction = new Transaction(familyDoc, "Add Parameter & Set Category"))
            {
                transaction.Start();
                familyParameter = familyDoc.FamilyManager.AddParameter("Material", BuiltInParameterGroup.PG_MATERIALS, ParameterType.Material, false);
                familyDoc.OwnerFamily.FamilyCategory = category ?? Category.GetCategory(RevitData.Instance.Document, BuiltInCategory.OST_GenericModel);
                transaction.Commit();
            }

            using (Transaction transactionFamily = new Transaction(familyDoc, "Add Elements"))
            {
                transactionFamily.Start();
                FreeFormElement freeFormElement = FreeFormElement.Create(familyDoc, SolidUtils.CreateTransformed(solid, transform));
                Parameter parameterFreeForm = freeFormElement.get_Parameter(BuiltInParameter.MATERIAL_ID_PARAM);
                familyDoc.FamilyManager.AssociateElementParameterToFamilyParameter(parameterFreeForm, familyParameter);
                transactionFamily.Commit();
            }

            familyDoc.SaveAs(path, new SaveAsOptions() { OverwriteExistingFile = true, MaximumBackups = 1 });
            familyDoc.Close(true);

            FamilyInstance familyInstance = PlaceInstance(transform.Inverse, path);

            File.Delete(path); File.Delete(Path.Combine(Path.GetTempPath(), $"{nameFamily}.0001.rfa"));

            return familyInstance;
        }
        public static FamilyInstance CreateFamilyFreeFormOriginal(string fullPathFamily, Solid solid, string nameFamily, Category category)
        {
            Document familyDoc = RevitData.Instance.Application.NewFamilyDocument(fullPathFamily);
            string path = Path.Combine(Path.GetTempPath(), $"{nameFamily}.rfa");

            FamilyParameter familyParameter = null;
            using (Transaction transaction = new Transaction(familyDoc, "Add Parameter & Set Category"))
            {
                transaction.Start();
                familyParameter = familyDoc.FamilyManager.AddParameter("Material", BuiltInParameterGroup.PG_MATERIALS, ParameterType.Material, false);
                familyDoc.OwnerFamily.FamilyCategory = category ?? Category.GetCategory(RevitData.Instance.Document, BuiltInCategory.OST_GenericModel);
                transaction.Commit();
            }

            using (Transaction transactionFamily = new Transaction(familyDoc, "Add Elements"))
            {
                transactionFamily.Start();
                FreeFormElement freeFormElement = FreeFormElement.Create(familyDoc, solid);
                Parameter parameterFreeForm = freeFormElement.get_Parameter(BuiltInParameter.MATERIAL_ID_PARAM);
                familyDoc.FamilyManager.AssociateElementParameterToFamilyParameter(parameterFreeForm, familyParameter);
                transactionFamily.Commit();
            }

            familyDoc.SaveAs(path, new SaveAsOptions() { OverwriteExistingFile = true, MaximumBackups = 1 });
            familyDoc.Close(true);

            FamilyInstance familyInstance = PlaceInstance(path);
            File.Delete(path); File.Delete(Path.Combine(Path.GetTempPath(), $"{nameFamily}.0001.rfa"));

            return familyInstance;
        }
        public static FamilyInstance CreateFamilyFreeFormOriginal(string fullPathFamily, List<Solid> solids, string nameFamily, Category category)
        {
            Document familyDoc = RevitData.Instance.Application.NewFamilyDocument(fullPathFamily);
            string path = Path.Combine(Path.GetTempPath(), $"{nameFamily}.rfa");

            FamilyParameter familyParameter = null;
            using (Transaction transaction = new Transaction(familyDoc, "Add Parameter & Set Category"))
            {
                transaction.Start();
                familyParameter = familyDoc.FamilyManager.AddParameter("Material", BuiltInParameterGroup.PG_MATERIALS, ParameterType.Material, false);
                familyDoc.OwnerFamily.FamilyCategory = category ?? Category.GetCategory(RevitData.Instance.Document, BuiltInCategory.OST_GenericModel);
                transaction.Commit();
            }

            using (Transaction transactionFamily = new Transaction(familyDoc, "Add Elements"))
            {
                transactionFamily.Start();
                foreach (Solid solid in solids)
                {
                    FreeFormElement freeFormElement = FreeFormElement.Create(familyDoc, solid);
                    Parameter parameterFreeForm = freeFormElement.get_Parameter(BuiltInParameter.MATERIAL_ID_PARAM);
                    familyDoc.FamilyManager.AssociateElementParameterToFamilyParameter(parameterFreeForm, familyParameter);
                }                
                transactionFamily.Commit();
            }

            familyDoc.SaveAs(path, new SaveAsOptions() { OverwriteExistingFile = true, MaximumBackups = 1 });
            familyDoc.Close(true);

            FamilyInstance familyInstance = PlaceInstance(path);

            File.Delete(path); File.Delete(Path.Combine(Path.GetTempPath(), $"{nameFamily}.0001.rfa"));

            return familyInstance;
        }
        public static FamilyInstance CreateFamilyExtrusion(string fullPathFamily, CurveLoop curveLoop, string nameFamily, double startOffset, double endOffset)
        {
            Document familyDoc = RevitData.Instance.Application.NewFamilyDocument(fullPathFamily);
            string path = Path.Combine(Path.GetTempPath(), $"{nameFamily}.rfa");
            Transform transform = Transform.CreateTranslation(XYZ.Zero - curveLoop.GetPlane().Origin);
            CurveLoop curveLoopClone = curveLoop.Clone();
            CreateExtrusion(familyDoc, curveLoopClone, startOffset, endOffset);
            familyDoc.SaveAs(path, new SaveAsOptions() { OverwriteExistingFile = true, MaximumBackups = 1 });
            familyDoc.Close(true);
            FamilyInstance familyInstance = PlaceInstance(transform.Inverse, path);
            File.Delete(path);
            return familyInstance;
        }
        public static Extrusion CreateExtrusion(Document familyDoc, CurveLoop curveLoop, double startOffset, double endOffset)
        {
            using (Transaction transaction = new Transaction(familyDoc, "Create Extrusion"))
            {
                WarningUtils ignoreWarning = new WarningUtils();
                FailureHandlingOptions options = ignoreWarning.GetFailureHandling(transaction);
                transaction.Start();

                CurveArrArray curveArrArray = new CurveArrArray();
                curveArrArray.Append(curveLoop.ToCurveArray());
                Autodesk.Revit.Creation.FamilyItemFactory familyItemFactory = familyDoc.FamilyCreate;
                Extrusion extrusion = familyItemFactory.NewExtrusion(true, curveArrArray, SketchPlane.Create(familyDoc, curveLoop.GetPlane()), startOffset);
                extrusion.EndOffset = -endOffset;
                extrusion.StartOffset = startOffset;
                curveArrArray.Dispose();

                transaction.Commit(options);
                return extrusion;
            }
        }
        private static FamilyInstance PlaceInstance(Transform transform, string path)
        {
            using (Transaction transactionDocument = new Transaction(RevitData.Instance.Document, "Place Instance"))
            {
                WarningUtils ignoreWarning = new WarningUtils();
                FailureHandlingOptions options = ignoreWarning.GetFailureHandling(transactionDocument);
                transactionDocument.Start();
                RevitData.Instance.Document.LoadFamily(path, new LoadFamilyUtils(), out Family family);
                FamilySymbol familySymbol = new FilteredElementCollector(RevitData.Instance.Document).WherePasses(new FamilySymbolFilter(family.Id)).FirstElement() as FamilySymbol;
                familySymbol.Activate();
                List<FamilyInstance> familyInstances = new FilteredElementCollector(RevitData.Instance.Document).OfClass(typeof(FamilyInstance)).Cast<FamilyInstance>().Where(x => x.Symbol.Name == familySymbol.Name).ToList();
                if (familyInstances.Count > 0)
                {
                    foreach (FamilyInstance familyInstanceOld in familyInstances)
                    {
                        RevitData.Instance.Document.Delete(familyInstanceOld.Id);
                    }
                }
                FamilyInstance familyInstance = RevitData.Instance.Document.Create.NewFamilyInstance(transform.Origin, familySymbol, StructuralType.NonStructural);
                transactionDocument.Commit(options);
                return familyInstance;
            }
        }
        private static FamilyInstance PlaceInstance(string path)
        {
            using (Transaction transactionDocument = new Transaction(RevitData.Instance.Document, "Place Instance"))
            {
                WarningUtils ignoreWarning = new WarningUtils();
                FailureHandlingOptions options = ignoreWarning.GetFailureHandling(transactionDocument);
                transactionDocument.Start();
                RevitData.Instance.Document.LoadFamily(path, new LoadFamilyUtils(), out Family family);
                FamilySymbol familySymbol = new FilteredElementCollector(RevitData.Instance.Document).WherePasses(new FamilySymbolFilter(family.Id)).FirstElement() as FamilySymbol;
                familySymbol.Activate();
                List<FamilyInstance> familyInstances = new FilteredElementCollector(RevitData.Instance.Document).OfClass(typeof(FamilyInstance)).Cast<FamilyInstance>().Where(x => x.Symbol.Name == familySymbol.Name).ToList();
                if (familyInstances.Count > 0)
                {
                    foreach (FamilyInstance familyInstanceOld in familyInstances)
                    {
                        RevitData.Instance.Document.Delete(familyInstanceOld.Id);
                    }
                }
                FamilyInstance familyInstance = RevitData.Instance.Document.Create.NewFamilyInstance(XYZ.Zero, familySymbol, StructuralType.NonStructural);
                transactionDocument.Commit(options);
                return familyInstance;
            }
        }
    }
}