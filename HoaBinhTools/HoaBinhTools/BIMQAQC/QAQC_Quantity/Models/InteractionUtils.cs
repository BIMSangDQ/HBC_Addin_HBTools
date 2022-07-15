#region Using
using Autodesk.Revit.DB;
using BeyCons.Core.FormUtils.ControlViews;
using BeyCons.Core.FormUtils.Reports;
using BeyCons.Core.Libraries.Geometries;
using BeyCons.Core.Libraries.Units;
using BeyCons.Core.RevitUtils;
using BeyCons.Core.RevitUtils.DataUtils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;
#endregion

namespace HoaBinhTools.BIMQAQC.QAQC_Quantity.Models
{
    public class InteractionUtils
    {
        #region Variable
        private ObservableCollection<Element> elements;
        private ObservableCollection<ObservableCollection<ElementId>> unionErrorElementIds;
        #endregion

        #region Properties
        public Element ElementIntersect { get; set; }
        public ObservableCollection<Element> ElementsStock
        {
            get
            {
                if (elements == null) elements = new ObservableCollection<Element>();
                return elements;
            }
            set
            {
                elements = value;
            }
        }
        public ObservableCollection<ObservableCollection<ElementId>> UnionErrorElementIds
        {
            get
            {
                if (null == unionErrorElementIds) unionErrorElementIds = new ObservableCollection<ObservableCollection<ElementId>>();
                return unionErrorElementIds;
            }
            set
            {
                unionErrorElementIds = value;
            }
        }
        #endregion

        #region Method
        public void SingleJoin(bool isAdvanceFilter)
        {
            BoundingBoxXYZ boundingBoxXYZ = ElementIntersect.get_BoundingBox(null);
            Outline outline = new Outline(boundingBoxXYZ.Min, boundingBoxXYZ.Max);
            BoundingBoxIntersectsFilter boundingBoxIntersectsFilter = new BoundingBoxIntersectsFilter(outline, 5.0.ToFeet());
            List<Element> elementIntersectsBoundingBox = new FilteredElementCollector(ActiveData.Document, ElementsStock.Select(x => x.Id).ToList()).WherePasses(boundingBoxIntersectsFilter).Excluding(new List<ElementId>() { ElementIntersect.Id }).ToList();

            if (elementIntersectsBoundingBox.Count > 0)
            {
                List<Solid> intersectSolids = GeometryUtils.GetSolidsFromInstanceElement(ElementIntersect, new Options() { DetailLevel = ViewDetailLevel.Fine }, true).ToUnionSolids();

                List<ElementFilter> elementFilters = new List<ElementFilter>();
                foreach (Solid solid in intersectSolids)
                {
                    ElementIntersectsSolidFilter elementIntersectsSolidFilter = new ElementIntersectsSolidFilter(solid);
                    elementFilters.Add(elementIntersectsSolidFilter);
                }
                LogicalOrFilter logicalOrFilter = new LogicalOrFilter(elementFilters);
                List<Element> elementIntersectsSolid = new FilteredElementCollector(ActiveData.Document, elementIntersectsBoundingBox.Select(x => x.Id).ToList()).WherePasses(logicalOrFilter).Excluding(new List<ElementId>() { ElementIntersect.Id }).ToList();

                List<Element> differenceElement = elementIntersectsBoundingBox.Where(p => !elementIntersectsSolid.Any(c => c.Id == p.Id)).ToList();

                ProgressBarInstance progressBarInstance = null;
                if (elementIntersectsBoundingBox.Count > 25)
                {
                    progressBarInstance = new ProgressBarInstance("Joining intersect ...", elementIntersectsBoundingBox.Count);
                }

                foreach (Element element in elementIntersectsSolid)
                {
                    if (elementIntersectsBoundingBox.Count > 25)
                    {
                        progressBarInstance.Start();
                    }

                    if (element.LookupParameter("Volume").AsDouble() > GeometryLib.EPSILON_VOLUME)
                    {
                        if (!JoinGeometryUtils.AreElementsJoined(ActiveData.Document, ElementIntersect, element))
                        {
                            try
                            {
                                JoinGeometryUtils.JoinGeometry(ActiveData.Document, ElementIntersect, element);
                            }
                            catch (Autodesk.Revit.Exceptions.ArgumentException ae)
                            {
                                ObservableCollection<JoinReport> canJoins = new ObservableCollection<JoinReport>();
                                foreach (JoinReport cj in ReportData.Instance.JoinReports)
                                {
                                    canJoins.Add(cj);
                                }
                                JoinReport canJoin = new JoinReport(new List<ElementId>() { ElementIntersect.Id, element.Id })
                                {
                                    Index = canJoins.Count + 1,
                                    DescriptionText = ae.Message
                                };
                                canJoins.Add(canJoin);
                                ReportData.Instance.JoinReports = canJoins;
                            }
                            catch (Autodesk.Revit.Exceptions.InvalidOperationException ai)
                            {
                                ObservableCollection<JoinReport> canJoins = new ObservableCollection<JoinReport>();
                                foreach (JoinReport cj in ReportData.Instance.JoinReports)
                                {
                                    canJoins.Add(cj);
                                }
                                JoinReport canJoin = new JoinReport(new List<ElementId>() { ElementIntersect.Id, element.Id })
                                {
                                    Index = canJoins.Count + 1,
                                    DescriptionText = ai.Message
                                };
                                canJoins.Add(canJoin);
                                ReportData.Instance.JoinReports = canJoins;
                            }
                            ConvertJoin(ElementIntersect, element, isAdvanceFilter);
                        }
                        else
                        {
                            ConvertJoin(ElementIntersect, element, isAdvanceFilter);
                        }
                    }
                }

                foreach (Element element in differenceElement)
                {
                    if (elementIntersectsBoundingBox.Count > 25)
                    {
                        progressBarInstance.Start();
                    }

                    if (element.LookupParameter("Volume").AsDouble() > GeometryLib.EPSILON_VOLUME)
                    {
                        foreach (Solid intersectSolid in intersectSolids)
                        {
                            if (HaveJoin(intersectSolid, element))
                            {
                                bool boo = JoinGeometryUtils.AreElementsJoined(ActiveData.Document, ElementIntersect, element);
                                if (!boo)
                                {
                                    try
                                    {
                                        JoinGeometryUtils.JoinGeometry(ActiveData.Document, ElementIntersect, element);
                                    }
                                    catch (Autodesk.Revit.Exceptions.ArgumentException ae)
                                    {
                                        ObservableCollection<JoinReport> canJoins = new ObservableCollection<JoinReport>();
                                        foreach (JoinReport cj in ReportData.Instance.JoinReports)
                                        {
                                            canJoins.Add(cj);
                                        }
                                        JoinReport canJoin = new JoinReport(new List<ElementId>() { ElementIntersect.Id, element.Id })
                                        {
                                            Index = canJoins.Count + 1,
                                            DescriptionText = ae.Message
                                        };
                                        canJoins.Add(canJoin);
                                        ReportData.Instance.JoinReports = canJoins;
                                    }
                                    catch (Autodesk.Revit.Exceptions.InvalidOperationException ai)
                                    {
                                        ObservableCollection<JoinReport> canJoins = new ObservableCollection<JoinReport>();
                                        foreach (JoinReport cj in ReportData.Instance.JoinReports)
                                        {
                                            canJoins.Add(cj);
                                        }
                                        JoinReport canJoin = new JoinReport(new List<ElementId>() { ElementIntersect.Id, element.Id })
                                        {
                                            Index = canJoins.Count + 1,
                                            DescriptionText = ai.Message
                                        };
                                        canJoins.Add(canJoin);
                                        ReportData.Instance.JoinReports = canJoins;
                                    }
                                    ConvertJoin(ElementIntersect, element, isAdvanceFilter);
                                }
                                else
                                {
                                    ConvertJoin(ElementIntersect, element, isAdvanceFilter);
                                }
                            }
                        }
                    }
                }
            }
        }
        private void ConvertJoin(Element elementOne, Element elementTwo, bool isAdvanceFilter)
        {
            if (isAdvanceFilter)
            {
                if (JoinGeometryUtils.AreElementsJoined(ActiveData.Document, elementOne, elementTwo))
                {
                    if (!JoinGeometryUtils.IsCuttingElementInJoin(ActiveData.Document, elementOne, elementTwo))
                    {
                        try
                        {
                            JoinGeometryUtils.SwitchJoinOrder(ActiveData.Document, elementOne, elementTwo);
                        }
                        catch (Autodesk.Revit.Exceptions.ArgumentException ae)
                        {
                            ObservableCollection<JoinReport> joinReports = new ObservableCollection<JoinReport>();
                            foreach (JoinReport cj in ReportData.Instance.JoinReports)
                            {
                                joinReports.Add(cj);
                            }
                            JoinReport canJoin = new JoinReport(new List<ElementId>() { elementOne.Id, elementTwo.Id })
                            {
                                Index = joinReports.Count + 1,
                                DescriptionText = ae.Message
                            };
                            joinReports.Add(canJoin);
                            ReportData.Instance.JoinReports = joinReports;
                        }
                        catch (Autodesk.Revit.Exceptions.InvalidOperationException ai)
                        {
                            ObservableCollection<JoinReport> joinReports = new ObservableCollection<JoinReport>();
                            foreach (JoinReport cj in ReportData.Instance.JoinReports)
                            {
                                joinReports.Add(cj);
                            }
                            JoinReport canJoin = new JoinReport(new List<ElementId>() { elementOne.Id, elementTwo.Id })
                            {
                                Index = joinReports.Count + 1,
                                DescriptionText = ai.Message
                            };
                            joinReports.Add(canJoin);
                            ReportData.Instance.JoinReports = joinReports;
                        }
                    }
                }
            }
            else
            {
                if (elementOne.Category.Name != elementTwo.Category.Name)
                {
                    if (JoinGeometryUtils.AreElementsJoined(ActiveData.Document, elementOne, elementTwo))
                    {
                        if (!JoinGeometryUtils.IsCuttingElementInJoin(ActiveData.Document, elementOne, elementTwo))
                        {
                            try
                            {
                                JoinGeometryUtils.SwitchJoinOrder(ActiveData.Document, elementOne, elementTwo);
                            }
                            catch (Autodesk.Revit.Exceptions.ArgumentException ae)
                            {
                                ObservableCollection<JoinReport> joinReports = new ObservableCollection<JoinReport>();
                                foreach (JoinReport cj in ReportData.Instance.JoinReports)
                                {
                                    joinReports.Add(cj);
                                }
                                JoinReport canJoin = new JoinReport(new List<ElementId>() { elementOne.Id, elementTwo.Id })
                                {
                                    Index = joinReports.Count + 1,
                                    DescriptionText = ae.Message
                                };
                                joinReports.Add(canJoin);
                                ReportData.Instance.JoinReports = joinReports;
                            }
                            catch (Autodesk.Revit.Exceptions.InvalidOperationException ai)
                            {
                                ObservableCollection<JoinReport> joinReports = new ObservableCollection<JoinReport>();
                                foreach (JoinReport cj in ReportData.Instance.JoinReports)
                                {
                                    joinReports.Add(cj);
                                }
                                JoinReport canJoin = new JoinReport(new List<ElementId>() { elementOne.Id, elementTwo.Id })
                                {
                                    Index = joinReports.Count + 1,
                                    DescriptionText = ai.Message
                                };
                                joinReports.Add(canJoin);
                                ReportData.Instance.JoinReports = joinReports;
                            }
                        }
                    }
                }
            }
        }
        public void SingleUnJoin()
        {
            BoundingBoxXYZ boundingBoxXYZ = ElementIntersect.get_BoundingBox(null);
            Outline outline = new Outline(boundingBoxXYZ.Min, boundingBoxXYZ.Max);
            BoundingBoxIntersectsFilter boundingBoxIntersectsFilter = new BoundingBoxIntersectsFilter(outline, 5.0.ToFeet());
            List<Element> elementIntersects = new FilteredElementCollector(ActiveData.Document, ElementsStock.Select(x => x.Id).ToList()).WherePasses(boundingBoxIntersectsFilter).Excluding(new List<ElementId>() { ElementIntersect.Id }).ToList();

            ProgressBarInstance progressBarInstance = null;
            if (elementIntersects.Count > 25)
            {
                progressBarInstance = new ProgressBarInstance("Unjoining intersect ...", elementIntersects.Count);
            }

            foreach (Element element in elementIntersects)
            {
                if (elementIntersects.Count > 25)
                {
                    progressBarInstance.Start();
                }

                if (JoinGeometryUtils.AreElementsJoined(ActiveData.Document, ElementIntersect, element))
                {
                    try
                    {
                        JoinGeometryUtils.UnjoinGeometry(ActiveData.Document, ElementIntersect, element);
                    }
                    catch (Autodesk.Revit.Exceptions.ArgumentException ae)
                    {
                        ObservableCollection<JoinReport> joinReports = new ObservableCollection<JoinReport>();
                        foreach (JoinReport jr in ReportData.Instance.JoinReports)
                        {
                            joinReports.Add(jr);
                        }
                        JoinReport joinReport = new JoinReport(new List<ElementId>() { ElementIntersect.Id, element.Id })
                        {
                            Index = joinReports.Count + 1,
                            DescriptionText = ae.Message
                        };
                        joinReports.Add(joinReport);
                        ReportData.Instance.JoinReports = joinReports;
                    }
                    catch (Autodesk.Revit.Exceptions.InvalidOperationException ai)
                    {
                        ObservableCollection<JoinReport> joinReports = new ObservableCollection<JoinReport>();
                        foreach (JoinReport jr in ReportData.Instance.JoinReports)
                        {
                            joinReports.Add(jr);
                        }
                        JoinReport joinReport = new JoinReport(new List<ElementId>() { ElementIntersect.Id, element.Id })
                        {
                            Index = joinReports.Count + 1,
                            DescriptionText = ai.Message
                        };
                        joinReports.Add(joinReport);
                        ReportData.Instance.JoinReports = joinReports;
                    }
                }
            }
        }
        public void SingleSwitchJoinOrder(bool isAdvanceFilter)
        {
            BoundingBoxXYZ boundingBoxXYZ = ElementIntersect.get_BoundingBox(null);
            Outline outline = new Outline(boundingBoxXYZ.Min, boundingBoxXYZ.Max);
            BoundingBoxIntersectsFilter boundingBoxIntersectsFilter = new BoundingBoxIntersectsFilter(outline, 5.0.ToFeet());
            List<Element> elementIntersects = new FilteredElementCollector(ActiveData.Document, ElementsStock.Select(x => x.Id).ToList()).WherePasses(boundingBoxIntersectsFilter).Excluding(new List<ElementId>() { ElementIntersect.Id }).ToList();

            ProgressBarInstance progressBarInstance = null;
            if (elementIntersects.Count > 25)
            {
                progressBarInstance = new ProgressBarInstance("Switch joining intersect ...", elementIntersects.Count);
            }

            foreach (Element element in elementIntersects)
            {
                if (elementIntersects.Count > 25)
                {
                    progressBarInstance.Start();
                }

                if (JoinGeometryUtils.AreElementsJoined(ActiveData.Document, ElementIntersect, element))
                {
                    ConvertJoin(ElementIntersect, element, isAdvanceFilter);
                }
            }
        }
        public bool HaveJoin(Solid intersectSolid, Element elementTwo)
        {
            List<Solid> solidsTwo = GeometryUtils.GetSolidsFromInstanceElement(elementTwo, new Options() { DetailLevel = ViewDetailLevel.Fine }, true).ToUnionSolids();
            foreach (Solid solidTwo in solidsTwo)
            {
                if (IntersectUtils.DoesIntersect(intersectSolid, solidTwo))
                {
                    return true;
                }
            }
            return false;
        }
        #endregion
    }
}
