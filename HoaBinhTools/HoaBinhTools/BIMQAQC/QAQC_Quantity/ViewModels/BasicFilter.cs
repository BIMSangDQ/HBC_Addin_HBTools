using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using BeyCons.Core.Libraries.Geometries;
using BeyCons.Core.RevitUtils.DataUtils;
using Utils;

namespace HoaBinhTools.BIMQAQC.QAQC_Quantity.ViewModels
{
	public class BasicFilter
	{
        #region Variable
        private List<Element> structuralFraming;
        private List<Element> structuralFoundation;
        private List<Element> structuralColumns;
        private List<Element> walls;
        private List<Element> wallSweeps;
        private List<Element> floors;
        private List<Element> slabEdges;
        private List<Element> roofs;
        private List<Element> columns;
        private List<Element> stairs;
        private List<Element> ramps;
        private List<Element> doors;
        private List<Element> windows;
        private List<Element> genericModels;
        private List<Element> elementsAll;
        private List<Element> customElements;
        private List<string> userWorkSet;
        private List<Category> customCategories;
        #endregion

        #region Properties
        public List<Element> StructuralFraming
        {
            get { return structuralFraming ?? (structuralFraming = new List<Element>()); }
            set { structuralFraming = value; }
        }
        public List<Element> StructuralFoundations
        {
            get { return structuralFoundation ?? (structuralFoundation = new List<Element>()); }
            set { structuralFoundation = value; }
        }
        public List<Element> StructuralColumns
        {
            get { return structuralColumns ?? (structuralColumns = new List<Element>()); }
            set { structuralColumns = value; }
        }
        public List<Element> Walls
        {
            get { return walls ?? (walls = new List<Element>()); }
            set { walls = value; }
        }
        public List<Element> WallSweeps
        {
            get { return wallSweeps ?? (wallSweeps = new List<Element>()); }
            set { wallSweeps = value; }
        }
        public List<Element> Floors
        {
            get { return floors ?? (floors = new List<Element>()); }
            set { floors = value; }
        }
        public List<Element> SlabEdges
        {
            get { return slabEdges ?? (slabEdges = new List<Element>()); }
            set { slabEdges = value; }
        }
        public List<Element> Roofs
        {
            get { return roofs ?? (roofs = new List<Element>()); }
            set { roofs = value; }
        }
        public List<Element> Columns
        {
            get { return columns ?? (columns = new List<Element>()); }
            set { columns = value; }
        }
        public List<Element> GenericModels
        {
            get { return genericModels ?? (genericModels = new List<Element>()); }
            set { genericModels = value; }
        }
        public List<Element> Stairs
        {
            get { return stairs ?? (stairs = new List<Element>()); }
            set { stairs = value; }
        }
        public List<Element> Ramps
        {
            get { return ramps ?? (ramps = new List<Element>()); }
            set { ramps = value; }
        }
        public List<Element> Doors
        {
            get { return doors ?? (doors = new List<Element>()); }
            set { doors = value; }
        }
        public List<Element> Windows
        {
            get { return windows ?? (windows = new List<Element>()); }
            set { windows = value; }
        }
        public List<Element> ElementsAll
        {
            get { return elementsAll ?? (elementsAll = new List<Element>()); }
            set { elementsAll = value; }
        }
        public List<Element> CustomElements
        {
            get { return customElements ?? (customElements = new List<Element>()); }
            set { customElements = value; }
        }
        public List<string> UserWorkSet
        {
            get { return userWorkSet ?? (userWorkSet = new List<string>()); }
            set { userWorkSet = value; }
        }
        public List<Category> CustomCategories
        {
            get { return customCategories ?? (customCategories = new List<Category>()); }
            set { customCategories = value; }
        }
        public List<Element> this[string s]
        {
            get
            {
                string nameCategory = Regex.Replace(s, " ", string.Empty);
                return GetType().GetProperty(nameCategory).GetValue(this) as List<Element>;
            }
        }
        #endregion

        public BasicFilter(List<Element> elements)
        {
            CustomFillter(elements);
        }

        private void CustomFillter(List<Element> elements)
        {
            foreach (Element element in elements)
            {
                if (element.Category != null)
                {
                    switch (element.Category.Id.IntegerValue)
                    {
                        case (int)BuiltInCategory.OST_StructuralFraming:
                            if (element.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED).AsDouble() > GeometryLib.EPSILON_VOLUME)
                            {
                                StructuralFraming.Add(element);
                                CustomElements.Add(element);
                                CustomCategories.Add(element.Category);
                            }
                            break;
                        case (int)BuiltInCategory.OST_StructuralFoundation:
                            if (element.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED).AsDouble() > GeometryLib.EPSILON_VOLUME)
                            {
                                StructuralFoundations.Add(element);
                                CustomElements.Add(element);
                                CustomCategories.Add(element.Category);
                            }
                            break;
                        case (int)BuiltInCategory.OST_StructuralColumns:
                            if (element.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED).AsDouble() > GeometryLib.EPSILON_VOLUME)
                            {
                                StructuralColumns.Add(element);
                                CustomElements.Add(element);
                                CustomCategories.Add(element.Category);
                            }
                            break;
                        case (int)BuiltInCategory.OST_Walls:
                            if (element.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED).AsDouble() > GeometryLib.EPSILON_VOLUME)
                            {
                                Walls.Add(element);
                                CustomElements.Add(element);
                                CustomCategories.Add(element.Category);
                            }
                            break;
                        case (int)BuiltInCategory.OST_Cornices:
                            if (element.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED).AsDouble() > GeometryLib.EPSILON_VOLUME)
                            {
                                WallSweeps.Add(element);
                                CustomElements.Add(element);
                                CustomCategories.Add(element.Category);
                            }
                            break;
                        case (int)BuiltInCategory.OST_Floors:
                            if (element.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED).AsDouble() > GeometryLib.EPSILON_VOLUME)
                            {
                                Floors.Add(element);
                                CustomElements.Add(element);
                                CustomCategories.Add(element.Category);
                            }
                            break;
                        case (int)BuiltInCategory.OST_EdgeSlab:
                            if (element.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED).AsDouble() > GeometryLib.EPSILON_VOLUME)
                            {
                                SlabEdges.Add(element);
                                CustomElements.Add(element);
                                CustomCategories.Add(element.Category);
                            }
                            break;
                        case (int)BuiltInCategory.OST_Roofs:
                            if (element.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED).AsDouble() > GeometryLib.EPSILON_VOLUME)
                            {
                                Roofs.Add(element);
                                CustomElements.Add(element);
                                CustomCategories.Add(element.Category);
                            }
                            break;
                        case (int)BuiltInCategory.OST_Columns:
                            if (element.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED).AsDouble() > GeometryLib.EPSILON_VOLUME)
                            {
                                Columns.Add(element);
                                CustomElements.Add(element);
                                CustomCategories.Add(element.Category);
                            }
                            break;
                        case (int)BuiltInCategory.OST_GenericModel:
                            if (element is FamilyInstance && element.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED).AsDouble() > GeometryLib.EPSILON_VOLUME)
                            {
                                GenericModels.Add(element);
                                CustomElements.Add(element);
                                CustomCategories.Add(element.Category);
                            }
                            break;
                        case (int)BuiltInCategory.OST_Stairs:
                            Stairs.Add(element);
                            CustomElements.Add(element);
                            CustomCategories.Add(element.Category);
                            break;
                        case (int)BuiltInCategory.OST_Ramps:
                            Ramps.Add(element);
                            CustomElements.Add(element);
                            CustomCategories.Add(element.Category);
                            break;
                        case (int)BuiltInCategory.OST_Doors:
                            Doors.Add(element);
                            CustomElements.Add(element);
                            CustomCategories.Add(element.Category);
                            break;
                        case (int)BuiltInCategory.OST_Windows:
                            Windows.Add(element);
                            CustomElements.Add(element);
                            CustomCategories.Add(element.Category);
                            break;
                        case (int)BuiltInCategory.OST_IOSModelGroups:
                            List<Element> elementsGroup = (element as Autodesk.Revit.DB.Group).GetMemberIds().Select(x => ActiveData.Document.GetElement(x)).ToList();
                            CustomFillter(elementsGroup);
                            break;
                    }
                    ElementsAll.Add(element);
                    try
                    {
                        UserWorkSet.Add(element.ToUserWorkset());
                    }
                    catch { }
                }
            }
        }

        public static void FindElement(List<ElementId> listId)
        {
            
        }
    }
}
