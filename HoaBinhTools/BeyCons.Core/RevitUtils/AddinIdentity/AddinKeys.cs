#region Using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace BeyCons.Core.RevitUtils.AddinIdentity
{
    public class AddinKeys
    {
        public static AddinEntity AnnotationCropOffset
        {
            get
            {
                return new AddinEntity()
                {
                    Name = "Annotation Crop Offset",
                    Unique = "E1BC4252-FA09-47DA-B515-7397823A40C3"
                };
            }
        }
        public static AddinEntity AssignRoom
        {
            get
            {
                return new AddinEntity()
                {
                    Name = "Assign Room",
                    Unique = "EE8E9626-CDE1-495C-B493-93726EE38178"
                };
            }
        }
        public static AddinEntity AutoJoin
        {
            get
            {
                return new AddinEntity()
                {
                    Name = "Auto Join",
                    Unique = "F02D73C9-9EE7-48B3-A57A-E71412BB32BA"
                };
            }
        }
        public static AddinEntity ConfigurationWall
        {
            get
            {
                return new AddinEntity()
                {
                    Name = "Configuration Wall Join",
                    Unique = "74C41640-3BB9-4EA4-8DD0-08ABF0E9873D"
                };
            }
        }
        public static AddinEntity ConvertDatum
        {
            get
            {
                return new AddinEntity()
                {
                    Name = "Datum Model To Specific Or Contrary",
                    Unique = "C02E6A49-53F3-4541-A47B-A2A06B79014D"
                };
            }
        }
        public static AddinEntity CountTile
        {
            get
            {
                return new AddinEntity()
                {
                    Name = "Count Tiles",
                    Unique = "CE565695-DECF-4BD8-95C3-54BFE14AA95B"
                };
            }
        }
        public static AddinEntity ElementLocation
        {
            get
            {
                return new AddinEntity()
                {
                    Name = "Element Location",
                    Unique = "11D724C8-6336-4305-8EC1-9A5EA7FD8BD8"
                };
            }
        }
        public static AddinEntity BuiltModel
        {
            get
            {
                return new AddinEntity()
                {
                    Name = "Built Model",
                    Unique = "42F73D03-AA1E-4167-AD8F-F8F70B095B42"
                };
            }
        }
        public static AddinEntity ExcelUtil
        {
            get
            {
                return new AddinEntity()
                {
                    Name = "Excel Utils",
                    Unique = "2090AA7D-A7A7-4F88-AB34-BFFE043525DC"
                };
            }
        }
        public static AddinEntity ExtendConcrete
        {
            get
            {
                return new AddinEntity()
                {
                    Name = "Extend Concrete",
                    Unique = "075F0432-2F5E-4FCD-9E5F-4A1EE366396A"
                };
            }
        }
        public static AddinEntity FastCropView
        {
            get
            {
                return new AddinEntity()
                {
                    Name = "Fast Crop View",
                    Unique = "4763774D-F9BB-44E5-AE9A-29CAC624E4AC"
                };
            }
        }
        public static AddinEntity Formwork
        {
            get
            {
                return new AddinEntity()
                {
                    Name = "Formwork",
                    Unique = "CFA30C0F-84F0-4C32-A55C-FCC774A994C4"
                };
            }
        }
        public static AddinEntity GenerateModel
        {
            get
            {
                return new AddinEntity()
                {
                    Name = "Generate Model",
                    Unique = "616C7400-428E-469B-AAFD-FA00B2E5FD5D"
                };
            }
        }
        public static AddinEntity GridUtil
        {
            get
            {
                return new AddinEntity()
                {
                    Name = "Grid Utils",
                    Unique = "72F00762-09BE-4CEC-A9BE-2BD78DB37B04"
                };
            }
        }
        public static AddinEntity Optimization
        {
            get
            {
                return new AddinEntity()
                {
                    Name = "Optimization",
                    Unique = "9E4E40ED-359F-443B-A5E1-45D7719C4696"
                };
            }
        }
        public static AddinEntity ResetGraphicElement
        {
            get
            {
                return new AddinEntity()
                {
                    Name = "Reset Graphic Element",
                    Unique = "F92BE81E-6C34-446F-A25E-3903825C2473"
                };
            }
        }
        public static AddinEntity RoomFinish
        {
            get
            {
                return new AddinEntity()
                {
                    Name = "Room Finish",
                    Unique = "32020A4D-7CD2-4134-B04D-F0C6E446AA38"
                };
            }
        }
        public static AddinEntity Tracking
        {
            get
            {
                return new AddinEntity()
                {
                    Name = "Tracking",
                    Unique = "DD730319-3BC2-4296-A6CC-E13CA7E2B2FB"
                };
            }
        }
        public static AddinEntity VisibleRebar
        {
            get
            {
                return new AddinEntity()
                {
                    Name = "Visible Rebars",
                    Unique = "A9498DD7-96DF-47B6-9101-105FCA82A915"
                };
            }
        }
        public static AddinEntity SplitFraming
        {
            get
            {
                return new AddinEntity()
                {
                    Name = "Split Framing",
                    Unique = "96C318AC-4889-4DD5-AB96-44751806977A"
                };
            }
        }
        public static AddinEntity PlasterMortar
        {
            get
            {
                return new AddinEntity()
                {
                    Name = "Plaster Mortar",
                    Unique = "96C94AA5-DE55-4B79-8C82-78AEE4E3E3F0"
                };
            }
        }
        public static AddinEntity StructureRebar
        {
            get
            {
                return new AddinEntity()
                {
                    Name = "Structure Rebar",
                    Unique = "790B9979-C766-4184-AEE1-EA9B6CB64362"
                };
            }
        }
        public static AddinEntity Coordinate
        {
            get
            {
                return new AddinEntity()
                {
                    Name = "Coordinate",
                    Unique = "6247CFF6-6905-40A7-AAD1-23BD429DDD38"
                };
            }
        }
        public static AddinEntity ClearanceHeight
        {
            get
            {
                return new AddinEntity()
                {
                    Name = "Clearance Height",
                    Unique = "AC65C323-5D6B-44E6-B805-9D56C1D2A7A1"
                };
            }
        }
        public static AddinEntity BoundaryCropView
        {
            get
            {
                return new AddinEntity()
                {
                    Name = "Boundary Crop View",
                    Unique = "F6F2A57A-0FB3-4375-9512-AA6FF1690911"
                };
            }
        }
        public static AddinEntity DetailingRebar
        {
            get
            {
                return new AddinEntity()
                {
                    Name = "Detailing Rebar",
                    Unique = "CC623DA6-F92C-48C0-8739-9DEBBA8B0016"
                };
            }
        }
        public static AddinEntity AssignContract
        {
            get
            {
                return new AddinEntity()
                {
                    Name = "Assign Contract",
                    Unique = "BAF2CF0F-A5EF-4AA6-BD40-7B32529CD695"
                };
            }
        }
        public static AddinEntity IntersectFraming
        {
            get
            {
                return new AddinEntity()
                {
                    Name = "Intersect Framing",
                    Unique = "6FDDC2D3-5520-4C43-9123-6F1F106BC339"
                };
            }
        }

        public static AddinEntity Topography
        {
            get
            {
                return new AddinEntity()
                {
                    Name = "Topography Surface",
                    Unique = "B1CC2D3E-D3B3-46BD-8DFE-C6CA08C36DE2"
                };
            }
        }

        public static AddinEntity Excavation
        {
            get
            {
                return new AddinEntity()
                {
                    Name = "Excavation",
                    Unique = "3626380D-6BBD-4B96-8DE6-47D95D524D84"
                };
            }
        }

        public static AddinEntity DevidedPart
        {
            get
            {
                return new AddinEntity()
                {
                    Name = "Devided Part",
                    Unique = "db5462a7-5440-40d5-8036-63c1ecb025cf"
                };
            }
        }
    }
}