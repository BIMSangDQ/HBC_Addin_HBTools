#region Using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
#endregion

namespace BeyCons.Core.RevitUtils.DataUtils.Enums
{
    public enum CustomParameters
    {
        HB_ExtendConcrete_Category = 1,
        HB_ExtendConcrete_Id = 2,
        HB_ExtendConcrete_Volume = 3,
        HB_ExtendConcrete_Height = 4, 
        
        HB_Formwork_Category = 5,
        HB_Formwork_Top = 6,
        HB_Formwork_Side = 7,
        HB_Formwork_Bottom = 8,
        HB_Formwork_Fill = 9,
        HB_Formwork_Precision = 10,
        HB_Formwork_Approx_Side = 11,
        HB_Formwork_Approx_Bottom = 12,
        HB_Formwork_HostId = 13,

        HB_Room_Category = 14,
        HB_Room_Number = 15,
        HB_Room_Name = 16,

        HB_Tile_Category = 17,
        HB_Tile_RoomName = 18,
        HB_Tile_RoomNumber = 19,
        HB_Tile_Area = 20,
        HB_Tile_Perimeter = 21,
        HB_Tile_Dimention = 22,
        HB_Tile_IsCut = 23,
        HB_Tile_IsArcEdge = 24,
        HB_Tile_Combination = 25,
        HB_Tile_HostId = 26,
        HB_Tile_AssemblyCode = 27,
        HB_Tile_Comment = 28,
        HB_Tile_Description = 29,
        HB_Tile_Keynote = 30,
        HB_Tile_TypeComment = 31,
        HB_Tile_TypeMark = 32,
        HB_Tile_TypeName = 33,
        HB_Tile_Complex = 34,
        HB_Tile_HostLevel = 64,

        HB_Mortar_Category = 35,
        HB_Mortar_RoomName = 36,
        HB_Mortar_RoomNumber = 37,
        HB_Mortar_Area = 38,
        HB_Mortar_HostId = 39,

        HB_NationalCoordinate_X = 40,
        HB_NationalCoordinate_Y = 41,
        HB_NationalCoordinate_Z = 42,

        HB_CheckHeight_Category = 43,
        HB_CheckHeight_Host_Id = 44,
        HB_CheckHeight_Host_Category = 45,
        HB_CheckHeight_Host_TypeName = 46,
        HB_CheckHeight_Intersect_Id = 47,
        HB_CheckHeight_Intersect_Category = 48,
        HB_CheckHeight_Intersect_TypeName = 49,
        HB_CheckHeight_Intersect_IsLink = 50,
        HB_CheckHeight_Intersect_Identity = 51,
        HB_CheckHeight_Intersect_RevitFile = 52,
        HB_CheckHeight_Intersect_Reviewed = 53,

        HB_Formwork_Total = 54,

        HB_ExtendConcrete_TOC = 55,

        HB_Framing_Node_Fra = 56,
        HB_Framing_Node_Col = 57,
        HB_Framing_Node_Wal = 58,
        HB_Framing_Node_Fou = 59,
        HB_Framing_Volume_Fra = 60,
        HB_Framing_Volume_Col = 61,
        HB_Framing_Volume_Wal = 62,        
        HB_Framing_Volume_Fou = 63,
        
    }
}