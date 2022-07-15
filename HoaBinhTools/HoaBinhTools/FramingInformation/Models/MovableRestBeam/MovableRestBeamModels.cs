using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Autodesk.Revit.DB;
using Utils;
using HoaBinhTools.FramingInformation.Models.FramingInformationCmd;
using HoaBinhTools.FramingInformation.ViewModels;

namespace HoaBinhTools.FramingInformation.Models
{
    public class MovableRestBeam : ViewModelBase
    {

        public MovableRestBeam(FramingInfoViewModels FraInfoViewModels )
        {
            this.FraInfoViewModels = FraInfoViewModels;

          
        }

        public MovableRestBeam()
        {

        }

        public string Category { get; set; }
     
        public double? Hight { get; set; } 
     
        public double? Width { get; set; }


        public ElementId id;
        public ElementId Id
        {
            get
            {
                return id;
            }
            set
            {
                this.id = value;
                OnPropertyChanged(nameof(Id));
            }
        }


        public List<ElementId> GroupElementID { get; set; }    

        public FramingInfoViewModels FraInfoViewModels { get; set; }

        public TypeAdjSupport type;
        public TypeAdjSupport Type
        {
            get => type;

            set
            {
                this.type = value;

                var Movable = this as MovableRestBeam;

                if(FraInfoViewModels!= null)
                {
                    FraInfoViewModels.OverrideColor(type, Movable);
                }
                 
                OnPropertyChanged(nameof(Type));
            }
        }

        protected double length;

        public double Length
        {
            get => length;

            set
            {
                this.length = value;

                OnPropertyChanged(nameof(Length));
            }
        }

        public Curve Curve { get; set; }

        public string GroupID { get; set; }

        public double? TopElevation { get; set; }

        public double? BottomElevation { get; set; }
       
        public ObservableCollection<BeamSystemsGroup> GetBeamSystemsGroup(List<List<Element>> EleFramings )
        {       
            ObservableCollection<BeamSystemsGroup> ReturnObs = new ObservableCollection<BeamSystemsGroup>();
       
            foreach (List<Element> Framing in EleFramings)
            {
              

                BeamSystemsGroup BeamSys = new BeamSystemsGroup();

                var GroupID = Guid.NewGuid().ToString();

                HostFraming Host = new HostFraming(Framing, ActiveData.Document);

                List<Element> elesIntersect = Framing.GetElementIntersectFarming(ActiveData.Document);

                List<ElementId> IsolateElement = new List<ElementId>();

                Plane planeWork = Plane.CreateByNormalAndOrigin(XYZ.BasisZ, XYZ.Zero);

                ProgressBarView Goi = new ProgressBarView();

                Goi.Show();

                // check intersect       
                //  doc.DrawingCurve(locaCurve);
                // lấy tọa độ này để sắp sếp 

                Solid SolidAdj = UtilsSolid.GetSolidZero();

               List<MovableRestBeam> ListAdj = new List<MovableRestBeam>();

                // lọc ra các loại gối 

                var SolidHost = Host.SolidOrgin;

                foreach (var eleInter in elesIntersect)
                {
                    try
                    {
                        Solid solid = null;

                        XYZ p1 = SolidHost.GetPointIntersect(eleInter.GetQuickSolidOrigin(ActiveData.Document));

                        if (!Goi.Create(elesIntersect.Count, " Goi ")) break;

                        if (p1 == null)
                        {
                            continue;
                        }

                        solid = eleInter.GetQuickSolidOrigin(ActiveData.Document);

                        MovableRestBeam Adj = new MovableRestBeam(FraInfoViewModels);

                        Curve curveInterscet = Host.TraightCurve.GetcurveAdijusIntersect(p1, solid, planeWork);

                        if (curveInterscet == null) continue;

                        Adj.Curve = curveInterscet;

                        Adj.Id = eleInter.Id;


                        Adj.Length = ((int)Math.Round(curveInterscet.Length.FootToMm(), 0)).Round10();

                        Adj.Category = eleInter.Category.Name;

                        Adj.GroupID = GroupID;

                        IsolateElement.Add(Adj.Id);

                        // nếu là dầm là gối 
                        if (eleInter.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFraming)
                        {
                            var WitdAndHight = (eleInter as FamilyInstance).GetWidthAndHight(ActiveData.Document);

                            Adj.Width = WitdAndHight.Item1;

                            Adj.Hight = WitdAndHight.Item2;

                            var TP = (eleInter as FamilyInstance).GetCrossSectionalAreaFraming(ActiveData.Document);

                            // dầm diện tích bé hơn
                            if (Host.Section - TP > 0.01)
                            {
                                Adj.Type = TypeAdjSupport.DP;
                            }
                            // dầm diện tích lớn hơn
                            else
                            {
                                Adj.Type = TypeAdjSupport.GT;
                            }
                        }
                        // nếu là cột hoặc móng
                        else
                        {
                            // không phải dầm thì chịu khong xac đinh được H và B

                            Adj.Width = null;

                            Adj.Hight = null;

                            Adj.Type = TypeAdjSupport.GT;
                        }

                        ListAdj.Add(Adj);

                        if (curveInterscet != null)
                        {
                            var curloop = CurveLoop.CreateViaThicken(curveInterscet, 100, XYZ.BasisZ);

                            var soild = curloop.GetSolidToCurveloop();

                            BooleanOperationsUtils.ExecuteBooleanOperationModifyingOriginalSolid(SolidAdj, soild, BooleanOperationsType.Union);
                        }

                    }
                     catch (Exception ex)
                    {
                      FramingInfoViewModels.ListErro.Add(new ErrorModels() { ErrorID = eleInter.Id, Category = eleInter.Category.Name, InfoErro = ex.ToString() });
                    }


                }


                Goi.Close();

                // lấy nhịp

                ProgressBarView Nhip = new ProgressBarView();

                Nhip.Show();

                foreach (Element Beam in Host.ListHost)
                {
                    FamilyInstance FamiInstane = (Beam as FamilyInstance);

                    Curve SpanBeam = FamiInstane.GetLocationCurve();

                    var With_Hight = FamiInstane.GetWidthAndHight(ActiveData.Document);

                    List<Curve> Curs = SpanBeam.GetListBeamsSpanIntersect(SolidAdj, Host.OrginPoint, planeWork);

                    // lấy chiều dài và rộng của dầm chủ  

                    foreach (var curBemaPan in Curs)
                    {
                        try
                        {
                            MovableRestBeam Adj = new MovableRestBeam(FraInfoViewModels);

                            Adj.Id = Beam.Id;

                            Adj.Length = ((int)Math.Round(curBemaPan.Length.FootToMm(), 0)).Round5();

                            Adj.Curve = curBemaPan;

                            Adj.Category = string.Format("Host: {0}", Beam.Name);

                            Adj.GroupID = GroupID;

                            Adj.Width = With_Hight.Item1;

                            Adj.Hight = With_Hight.Item2;

                            Adj.TopElevation = Beam.GetBeamElevationTop();

                            Adj.BottomElevation = Beam.GetBeamElevationBottom();

                            Adj.Type = TypeAdjSupport.ND;

                            ListAdj.Add(Adj);

                        }
                        catch (Exception ex)
                        {
                            FramingInfoViewModels.ListErro.Add(new ErrorModels() { ErrorID = Beam.Id, Category = string.Format("Host: {0}", Beam.Name), InfoErro = ex.ToString() });
                        }
                    }
                    if (!Nhip.Create(Host.ListHost.Count, " Nhip ")) break;
                }

                Nhip.Close();

               ListAdj.Sort(new MovableRestBeamSort(Host.OrginPoint));

                BeamSys.Movable = ConverterCollection<MovableRestBeam>.ToObser(ListAdj);

                BeamSys.Name = Host.Name;

                BeamSys.Host = Host;

                BeamSys.GroupID = GroupID; ;

                ReturnObs.Add(BeamSys);

     
                View view_HB = FramingInfoView3D.GetViewHB();

                foreach (var item in ListAdj)
                {
                    if (item.Type == TypeAdjSupport.GT)
                    {
                        ElementOverridesModels.ColorElement(ActiveData.Document, view_HB, item.Id, 255, 0, 0);

                    }
                    if (item.Type == TypeAdjSupport.DP)
                    {
                        ElementOverridesModels.ColorElement(ActiveData.Document, view_HB, item.Id, 030, 011, 204);
        
                    }
                }

                foreach (var host in Host.ListHost)
                {
                    ElementOverridesModels.ColorElement(ActiveData.Document, view_HB, host.Id, 0, 255, 0);
      
                }

                // hiển thị view cuối cùng
                if (EleFramings.LastOrDefault().GetHashCode() == Framing.GetHashCode())
                {
                    view_HB.IsolateFraming(ActiveData.Document, IsolateElement.Concat(Host.ListHost.Select(e => e.Id)).ToList());
                }            
            }
            return ReturnObs;
        }



        public ObservableCollection<BeamSystemsGroup> UpdataBeamSystemsGroup(string IDGroup , ObservableCollection<BeamSystemsGroup> SystemFramings)
        {
            ObservableCollection<BeamSystemsGroup> ReturnObs = new ObservableCollection<BeamSystemsGroup>();

            foreach (var SysFami in SystemFramings)
            {
                if (SysFami.GroupID != IDGroup)
                {
                    ReturnObs.Add(SysFami);
                }

                else
                {
                    BeamSystemsGroup BeamSys = new BeamSystemsGroup();

                    var GroupID = SysFami.GroupID;

                    HostFraming Host = SysFami.Host;

                    List<MovableRestBeam> elesIntersect = SysFami.Movable.Where(e => e.Type != TypeAdjSupport.ND).ToList();

                    List<ElementId> IsolateElement = new List<ElementId>();

                    Plane planeWork = Plane.CreateByNormalAndOrigin(XYZ.BasisZ, XYZ.Zero);

                    Solid SolidAdj = UtilsSolid.GetSolidZero();

                    List<MovableRestBeam> ListAdj = new List<MovableRestBeam>();

                    // Gối
                    foreach (var Support in elesIntersect)
                    {
                        var curloop = CurveLoop.CreateViaThicken(Support.Curve, 100, XYZ.BasisZ);

                        var soild = curloop.GetSolidToCurveloop();

                        BooleanOperationsUtils.ExecuteBooleanOperationModifyingOriginalSolid(SolidAdj, soild, BooleanOperationsType.Union);

                        ListAdj.Add(Support);
                    }


                    foreach (Element Beam in Host.ListHost)
                    {
                        ProgressBarView Uptate = new ProgressBarView();

                        Uptate.Show();

                        FamilyInstance FamiInstane = (Beam as FamilyInstance);

                        Curve SpanBeam = FamiInstane.GetLocationCurve();

                        var With_Hight = FamiInstane.GetWidthAndHight(ActiveData.Document);

                        List<Curve> Curs = SpanBeam.GetListBeamsSpanIntersect(SolidAdj, Host.OrginPoint, planeWork);

                        // lấy chiều dài và rộng của dầm chủ  

                        if (!Uptate.Create(elesIntersect.Count, " Update ")) break;

                        foreach (var curBemaPan in Curs)
                        {
                            try
                            {

                                MovableRestBeam Adj = new MovableRestBeam(FraInfoViewModels);

                                Adj.Id = Beam.Id;

                                Adj.Length = ((int)Math.Round(curBemaPan.Length.FootToMm(), 0)).Round5();

                                Adj.Curve = curBemaPan;

                                Adj.Category = string.Format("Host: {0}", Beam.Name);

                                Adj.GroupID = GroupID;

                                Adj.Width = With_Hight.Item1;

                                Adj.Hight = With_Hight.Item2;

                                Adj.TopElevation = Beam.GetBeamElevationTop();

                                Adj.BottomElevation = Beam.GetBeamElevationBottom();

                                Adj.Type = TypeAdjSupport.ND;

                                ListAdj.Add(Adj);

                            }
                            catch (Exception ex)
                            {
                               FramingInfoViewModels.ListErro.Add(new ErrorModels() { ErrorID = Beam.Id, Category = string.Format("Host: {0}", Beam.Name), InfoErro = ex.ToString() });
                            }



                        }

                        Uptate.Close();
                    }

                    ListAdj.Sort(new MovableRestBeamSort(Host.OrginPoint));

                    BeamSys.Movable = ConverterCollection<MovableRestBeam>.ToObser(ListAdj);

                    BeamSys.Name = Host.Name;

                    BeamSys.Host = Host;

                    BeamSys.GroupID = GroupID; ;

                    ReturnObs.Add(BeamSys);

                    View view_HB = FramingInfoView3D.GetViewHB();

                    foreach (var item in ListAdj)
                    {
                        if (item.Type == TypeAdjSupport.GT)
                        {
                            ElementOverridesModels.ColorElement(ActiveData.Document, view_HB, item.Id, 255, 0, 0);
                        }
                        if (item.Type == TypeAdjSupport.DP)
                        {
                            ElementOverridesModels.ColorElement(ActiveData.Document, view_HB, item.Id, 030, 011, 204);
                        }
                    }

                    foreach (var host in Host.ListHost)
                    {
                        ElementOverridesModels.ColorElement(ActiveData.Document, view_HB, host.Id, 0, 255, 0);
                    }
                }
            }
            return ReturnObs;
        }
    }
}
