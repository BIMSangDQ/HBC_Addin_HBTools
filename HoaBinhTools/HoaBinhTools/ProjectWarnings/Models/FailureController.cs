using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI.Selection;
using HoaBinhTools.ProjectWarnings.Views;
using Utils;

namespace HoaBinhTools.ProjectWarnings.Models
{
	public class FailureController : ViewModelBase
	{
		public ObservableCollection<ObjectWaring> objWarnings;
		public ObservableCollection<ObjectWaring> ObjWarnings
		{
			get
			{
				return objWarnings;
			}
			set
			{
				objWarnings = value;
				OnPropertyChanged(nameof(ObjWarnings));
			}
		}

		public ObservableCollection<ObjectWaring> ObjectWaringsSave { get; set; }

		public ObservableCollection<WarningType> warningType;
		public ObservableCollection<WarningType> WarningType
		{
			get
			{
				return warningType;
			}
			set
			{
				warningType = value;

				OnPropertyChanged(nameof(WarningType));
			}
		}

		public RelayCommand<IList> LvSelectionChangedCommand { get; set; }

		public RelayCommand<IList> LvFixWarningCommand { get; set; }

		public RelayCommand SelectElementWarning { get; set; }

		public RelayCommand LvFilterChangeCommand { get; set; }

		public RelayCommand CheckAllCommand { get; set; }

		public RelayCommand CheckNoneCommand { get; set; }

		public ViewWarning Wmain { get; set; }

		public string noteWarning = "";
		public string NoteWarning
		{
			get
			{
				return noteWarning;
			}
			set
			{
				noteWarning = value;

				OnPropertyChanged(nameof(noteWarning));
			}
		}

		public void Excute()
		{
			ObjWarnings = GetWarings();

			ObjectWaringsSave = ObjWarnings;

			WarningType = GetWaringTypes();

			LvSelectionChangedCommand = new RelayCommand<IList>(DataGridSelectionChangedIsolate);

			LvFixWarningCommand = new RelayCommand<IList>(FixWarning);

			SelectElementWarning = new RelayCommand(SelectElementWarningInProject);

			LvFilterChangeCommand = new RelayCommand(FilterChange);

			CheckAllCommand = new RelayCommand(FilterCheckAll);

			CheckNoneCommand = new RelayCommand(FilterCheckNone);

			Wmain = new ViewWarning(this);

			Wmain.Show();
		}

		//isolate obj
		public void DataGridSelectionChangedIsolate(IList select)
		{

			try
			{
				if (select != null && select[0] is ObjectWaring wr)
				{
					if (wr != null)
					{

						SetWaring(wr);

						List<ElementId> listIdWarning = wr.ElementWarning;

						Action action = new Action(() =>
						{
							using (Transaction trans = new Transaction(ActiveData.Document))
							{
								trans.Start("Transaction Group");
								if (ActiveData.ActiveView.IsTemporaryHideIsolateActive())
								{
									TemporaryViewMode tempView = TemporaryViewMode.TemporaryHideIsolate;

									ActiveData.ActiveView.DisableTemporaryViewMode(tempView);
								}

								ActiveData.ActiveView.IsolateElementsTemporary(listIdWarning);

								// zoom tới telemt
								ActiveData.UIDoc.ShowElements(listIdWarning);

								ActiveData.Document.Regenerate();
								trans.Commit();
							}
						});
						ExternalEventHandler.Instance.SetAction(action);

						ExternalEventHandler.Instance.Run();

					}
				}
			}
			catch
			{
			}
		}

		//Select element in project => warnings
		public void SelectElementWarningInProject()
		{
			Wmain.Hide();
			try
			{
				List<Element> elements = ActiveData.Selection.PickObjects(ObjectType.Element, Utils.SelectionFilter.GetElementFilter(x => !(x is DirectShape)), "Select elements in your project.").Select(x => ActiveData.Document.GetElement(x)).ToList();

				List<ElementId> elementIds = new List<ElementId>();
				foreach (Element e in elements)
				{
					elementIds.Add(e.Id);
				}

				ObservableCollection<ObjectWaring> WaringInProject = new ObservableCollection<ObjectWaring>();

				var Obj = ActiveData.Document.GetWarnings();

				for (int i = 0; i < Obj.Count(); i++)
				{
					FailureMessage fm = Obj[i] as FailureMessage;

					List<ElementId> FailingElements = fm.GetFailingElements().ToList();

					foreach (ElementId id in FailingElements)
					{
						foreach (ElementId v in elementIds)
						{
							if (v == id)
							{
								WaringInProject.Add(new ObjectWaring(WaringInProject.Count + 1, Obj[i]));
								break;
							}
						}
					}
				}

				ObjWarnings = WaringInProject;
				ObjectWaringsSave = ObjWarnings;
				WarningType = GetWaringTypes();
			}
			catch { }
			Wmain.Show();
		}

		//Get list warnings of Project
		public ObservableCollection<ObjectWaring> GetWarings()
		{
			ObservableCollection<ObjectWaring> WaringInProject = new ObservableCollection<ObjectWaring>();

			var Obj = ActiveData.Document.GetWarnings();

			for (int i = 0; i < Obj.Count(); i++)
			{
				WaringInProject.Add(new ObjectWaring(i + 1, Obj[i]));
			}

			return WaringInProject;
		}

		//Get Description of warning => Filter
		public ObservableCollection<WarningType> GetWaringTypes()
		{
			ObservableCollection<WarningType> WarningTypes = new ObservableCollection<WarningType>();

			var Obj = ObjectWaringsSave;

			List<string> WarningList = new List<string>();

			for (int i = 0; i < Obj.Count(); i++)
			{
				string description = Obj[i].Description;
				WarningList.Add(description);
			}

			string[] WarningDescription = WarningList.Distinct().ToArray();

			foreach (string description in WarningDescription)
			{
				int count = ObjectWaringsSave.Where(x => x.Description == description).Count();

				WarningTypes.Add(new WarningType(true, count, description));
			}

			return WarningTypes;
		}

		//Filter obj
		public void FilterChange()
		{
			ObservableCollection<ObjectWaring> WaringInProject = new ObservableCollection<ObjectWaring>();

			var Obj = ObjectWaringsSave;  //==> biến tạm để lưu trữ cả các phần bị ẩn

			for (int i = 0; i < Obj.Count(); i++)
			{
				foreach (WarningType wt in WarningType)
				{
					if (wt.Description == Obj[i].Description && wt.Filter == true)
					{
						WaringInProject.Add(Obj[i]);
						break;
					}
				}
			}

			ObjWarnings = new ObservableCollection<ObjectWaring>();
			ObjWarnings = WaringInProject;

		}

		public void FilterCheckAll()
		{
			ObservableCollection<WarningType> WT = new ObservableCollection<WarningType>();
			WT = WarningType;

			foreach (WarningType wt in WT)
			{
				wt.Filter = true;
			}

			WarningType = new ObservableCollection<WarningType>();
			WarningType = WT;
		}

		public void FilterCheckNone()
		{
			ObservableCollection<WarningType> WT = new ObservableCollection<WarningType>();
			WT = WarningType;

			foreach (WarningType wt in WT)
			{
				wt.Filter = false;
			}

			WarningType = new ObservableCollection<WarningType>();
			WarningType = WT;
		}

		//<<===========================================================================>>
		#region fix warning

		public void FixWarning(IList select)
		{
			var wr = Wmain.lview.SelectedItem as ObjectWaring;

			if (wr.GUIID == GUIService.OverLapWall)
			{
				var ObRemove = WarningService.CompareSolidWall(wr.ElementWarning);

				if (ObRemove.Count > 0)
				{
					Action action = new Action(() =>
					{
						Transaction trans = new Transaction(ActiveData.Document);

						trans.Start("remove wall");

						try
						{
							ActiveData.Document.Delete(ObRemove);
							ActiveData.Document.Regenerate();
						}
						catch
						{

						}
						trans.Commit();
						NoteWarning = "";
						ObjWarnings.Remove(wr);
						ObjectWaringsSave.Remove(wr);
					});

					ExternalEventHandler.Instance.SetAction(action);

					ExternalEventHandler.Instance.Run();
				}
			}

			if (wr.GUIID == GUIService.HighlightedFloorsOverlap)
			{
				var ObRemove = WarningService.CompareSolidFloor(wr.ElementWarning);

				if (ObRemove.Count > 0)
				{
					Action action = new Action(() =>
					{
						Transaction trans = new Transaction(ActiveData.Document);

						trans.Start("remove floor ");

						try
						{
							ActiveData.Document.Delete(ObRemove);
							ActiveData.Document.Regenerate();
						}
						catch
						{

						}
						trans.Commit();
						NoteWarning = "";
						ObjWarnings.Remove(wr);
						ObjectWaringsSave.Remove(wr);
					});

					ExternalEventHandler.Instance.SetAction(action);

					ExternalEventHandler.Instance.Run();
				}
			}

			if (wr.GUIID == GUIService.OverLapRebar)
			{

				Action action = new Action(() =>
				{
					Transaction trans = new Transaction(ActiveData.Document);

					trans.Start("remove rbar");

					try
					{
						for (int i = 0; i < wr.ElementWarning.Count - 1; i++)
						{
							ActiveData.Document.Delete(wr.ElementWarning[i]);


							ActiveData.Document.Regenerate();
						}
					}
					catch
					{

					}
					trans.Commit();
					NoteWarning = "";
					ObjWarnings.Remove(wr);
					ObjectWaringsSave.Remove(wr);
				}
				);

				ExternalEventHandler.Instance.SetAction(action);

				ExternalEventHandler.Instance.Run();

			}

			if (wr.GUIID == GUIService.WallIsSlightly)
			{
				Action action = new Action(() =>
				{
					Transaction trans = new Transaction(ActiveData.Document);

					trans.Start("WallIsSlightly");

					try
					{
						FixWallIsSlightly(wr.ElementWarning);
						ActiveData.Document.Regenerate();
					}
					catch
					{
					}
					trans.Commit();
					NoteWarning = "";
					ObjWarnings.Remove(wr);
					ObjectWaringsSave.Remove(wr);
				}
				);
				ExternalEventHandler.Instance.SetAction(action);

				ExternalEventHandler.Instance.Run();
			}

			if (wr.GUIID == GUIService.BeamOrBraceIsSlightlyOffAxisAndMayCauseInaccuracies)
			{
				Action action = new Action(() =>
				{
					Transaction trans = new Transaction(ActiveData.Document);

					trans.Start("BeamSlightly");

					try
					{
						FixBeamIsSlightly(wr.ElementWarning);
						ActiveData.Document.Regenerate();
					}
					catch
					{
					}
					trans.Commit();
					NoteWarning = "";
					ObjWarnings.Remove(wr);
					ObjectWaringsSave.Remove(wr);
				}
				);
				ExternalEventHandler.Instance.SetAction(action);

				ExternalEventHandler.Instance.Run();
			}

			if (wr.GUIID == GUIService.OverLapElement)
			{
				Action action = new Action(() =>
				{
					Transaction trans = new Transaction(ActiveData.Document);

					trans.Start("OverLap Element");

					try
					{
						FixOverLapElement(wr.ElementWarning);
						ActiveData.Document.Regenerate();
					}
					catch
					{
					}
					trans.Commit();
					NoteWarning = "";
					ObjWarnings.Remove(wr);
					ObjectWaringsSave.Remove(wr);
				}
				);
				ExternalEventHandler.Instance.SetAction(action);

				ExternalEventHandler.Instance.Run();
			}

			if (wr.GUIID == GUIService.HighlightedElementsAreJoined)
			{
				Action action = new Action(() =>
				{
					Transaction trans = new Transaction(ActiveData.Document);

					trans.Start("OverLap Element");

					try
					{
						FixUnjoinElement(wr.ElementWarning);
						ActiveData.Document.Regenerate();
					}
					catch
					{
					}
					trans.Commit();
					NoteWarning = "";
					ObjWarnings.Remove(wr);
					ObjectWaringsSave.Remove(wr);
				}
				);
				ExternalEventHandler.Instance.SetAction(action);

				ExternalEventHandler.Instance.Run();
			}

			if (wr.GUIID == GUIService.InsertConflicts)
			{
				Action action = new Action(() =>
				{
					Transaction trans = new Transaction(ActiveData.Document);

					trans.Start("OverLap Element");

					try
					{
						FixInsertConflicts(wr.ElementWarning);
						ActiveData.Document.Regenerate();
					}
					catch
					{
					}
					trans.Commit();
					NoteWarning = "";
					ObjWarnings.Remove(wr);
					ObjectWaringsSave.Remove(wr);
				}
				);
				ExternalEventHandler.Instance.SetAction(action);

				ExternalEventHandler.Instance.Run();
			}

			if (wr.GUIID == GUIService.HighlightedRoomSeparationLinesOverlap)
			{
				Action action = new Action(() =>
				{
					Transaction trans = new Transaction(ActiveData.Document);

					trans.Start("OverLap Element");

					try
					{
						FixHighlightedRoomSeparationLinesOverlap(wr.ElementWarning);
						ActiveData.Document.Regenerate();
					}
					catch
					{
					}
					trans.Commit();
					NoteWarning = "";
					ObjWarnings.Remove(wr);
					ObjectWaringsSave.Remove(wr);
				}
				);
				ExternalEventHandler.Instance.SetAction(action);

				ExternalEventHandler.Instance.Run();
			}

			if (wr.GUIID == GUIService.RoomTagIsOutsideOfItsRoom)
			{
				Action action = new Action(() =>
				{
					Transaction trans = new Transaction(ActiveData.Document);

					trans.Start("OverLap Element");

					try
					{
						FixRoomTagIsOutsideOfItsRoom(wr.ElementWarning);
						ActiveData.Document.Regenerate();
					}
					catch
					{
					}
					trans.Commit();
					NoteWarning = "";
					ObjWarnings.Remove(wr);
					ObjectWaringsSave.Remove(wr);
				}
				);
				ExternalEventHandler.Instance.SetAction(action);

				ExternalEventHandler.Instance.Run();
			}

			if (wr.GUIID == GUIService.RoomSeparationLineOverlap)
			{
				Action action = new Action(() =>
				{
					Transaction trans = new Transaction(ActiveData.Document);

					trans.Start("OverLap Element");

					try
					{
						FixRoomSeparationLineOverlap(wr.ElementWarning);
						ActiveData.Document.Regenerate();
					}
					catch
					{
					}
					trans.Commit();
					NoteWarning = "";
					ObjWarnings.Remove(wr);
					ObjectWaringsSave.Remove(wr);
				}
				);
				ExternalEventHandler.Instance.SetAction(action);

				ExternalEventHandler.Instance.Run();
			}

			if (wr.GUIID == GUIService.RoomSeparationLineIsSlightlyOffAxisAndMayCauseInaccuracies)
			{
				Action action = new Action(() =>
				{
					Transaction trans = new Transaction(ActiveData.Document);

					trans.Start("OverLap Element");

					try
					{
						FixRoomSeparationLineIsSlightlyOffAxis(wr.ElementWarning);
						ActiveData.Document.Regenerate();
					}
					catch
					{
					}
					trans.Commit();
					NoteWarning = "";
					ObjWarnings.Remove(wr);
					ObjectWaringsSave.Remove(wr);
				}
				);
				ExternalEventHandler.Instance.SetAction(action);

				ExternalEventHandler.Instance.Run();
			}

			if (wr.GUIID == GUIService.HighlightedInsertsOverlapWithOtherInsertsOrCompletelyMissTheirHosts)
			{
				Action action = new Action(() =>
				{
					Transaction trans = new Transaction(ActiveData.Document);

					trans.Start("OverLap Element");

					try
					{
						FixHighlightedInsertsOverlapWithOtherInsertsOrCompletelyMissTheirHosts(wr.ElementWarning);
						ActiveData.Document.Regenerate();
					}
					catch
					{
					}
					trans.Commit();
					NoteWarning = "";
					ObjWarnings.Remove(wr);
					ObjectWaringsSave.Remove(wr);
				}
				);
				ExternalEventHandler.Instance.SetAction(action);

				ExternalEventHandler.Instance.Run();
			}


			ObservableCollection<WarningType> wnt = new ObservableCollection<WarningType>();

			wnt = WarningType;

			foreach (WarningType wn in wnt)
			{
				int count = ObjectWaringsSave.Where(x => x.Description == wn.Description).Count();

				if (count > 1)
				{
					wn.Count = count;
				}
				else
				{
					wnt.Remove(wn);
					break;
				}
			}

			WarningType = new ObservableCollection<WarningType>();
			WarningType = wnt;

		}

		public void FixHighlightedInsertsOverlapWithOtherInsertsOrCompletelyMissTheirHosts(List<ElementId> ElesId)
		{

			foreach (var ele in ElesId.IdsToElements())
			{
				if (ele is FamilyInstance fami)
				{
					if (fami.Host.Id != ElementId.InvalidElementId)
					{
						ActiveData.Document.Delete(ele.Id);
					}
				}
			}

		}

		public void FixRoomSeparationLineIsSlightlyOffAxis(List<ElementId> ElesId)
		{
			var ele = ElesId.First();

			ModelLine modellineSeparation = ActiveData.Document.GetElement(ele) as ModelLine;

			XYZ DirecModelLine = (modellineSeparation.Location as LocationCurve).Curve.Direction();

			if (DirecModelLine.CrossProduct(XYZ.BasisX).DistanceTo(XYZ.Zero) < 0.01)
			{
				EditDirectionRoomSeparation(modellineSeparation, XYZ.BasisX);
			}
			if (DirecModelLine.CrossProduct(XYZ.BasisY).DistanceTo(XYZ.Zero) < 0.01)
			{
				EditDirectionRoomSeparation(modellineSeparation, XYZ.BasisY);
			}
			if (DirecModelLine.CrossProduct(XYZ.BasisZ).DistanceTo(XYZ.Zero) < 0.01)
			{
				EditDirectionRoomSeparation(modellineSeparation, XYZ.BasisZ);
			}


		}
		public void EditDirectionRoomSeparation(ModelLine modellineSeparation, XYZ axis)
		{

			var origin = (modellineSeparation.Location as LocationCurve).Curve.GetEndPoint(0);

			var vec1 = ActiveData.ActiveView.ViewDirection;

			XYZ normal = vec1.CrossProduct(axis);

			Plane plane = Plane.CreateByNormalAndOrigin(normal, origin);

			XYZ p0 = plane.ProjectOnto((modellineSeparation.Location as LocationCurve).Curve.GetEndPoint(0));

			XYZ p1 = plane.ProjectOnto((modellineSeparation.Location as LocationCurve).Curve.GetEndPoint(1));

			(modellineSeparation.Location as LocationCurve).Curve = Line.CreateBound(p0, p1);
		}

		public void FixRoomTagIsOutsideOfItsRoom(List<ElementId> ElesId)
		{
			RoomTag room_tag = ElesId.FirstOrDefault().ToElement() as RoomTag;

			Room room = room_tag.Room;

			if (!room_tag.IsInRoom)
			{
				var room_tag_pt = (room_tag.Location as LocationPoint).Point;

				var room_pt = (room.Location as LocationPoint).Point;

				var translation = room_pt - room_tag_pt;

				room_tag.Location.Move(translation);
			}
		}

		public void FixInsertConflicts(List<ElementId> ElesId)
		{
			Element el1 = null;

			Wall wall = null;

			if (ActiveData.Document.GetElement(ElesId.First()) is Wall w)
			{
				wall = w;
				el1 = ActiveData.Document.GetElement(ElesId.Last());
			}

			if (ActiveData.Document.GetElement(ElesId.Last()) is Wall w2)
			{
				wall = w2;
				el1 = ActiveData.Document.GetElement(ElesId.First());
			}

			BoundingBoxXYZ e = el1.get_BoundingBox(null);

			var pElement = (e.Max + e.Min) / 2;


			var wallCurve = (wall.Location as LocationCurve).Curve;

			var p0 = wallCurve.GetEndPoint(0);

			var p1 = wallCurve.GetEndPoint(1);


			if (p0.DistanceTo(pElement) < p1.DistanceTo(pElement))
			{
				if (WallUtils.IsWallJoinAllowedAtEnd(wall, 0))
				{
					WallUtils.DisallowWallJoinAtEnd(wall, 0);
				}

			}
			else
			{
				if (WallUtils.IsWallJoinAllowedAtEnd(wall, 1))
				{
					WallUtils.DisallowWallJoinAtEnd(wall, 1);
				}
			}


		}

		public void FixUnjoinElement(List<ElementId> ElesId)
		{
			try
			{
				Autodesk.Revit.DB.JoinGeometryUtils.UnjoinGeometry(ActiveData.Document, ActiveData.Document.GetElement(ElesId.First()), ActiveData.Document.GetElement(ElesId.Last()));
			}
			catch
			{
			}

			try
			{
				Autodesk.Revit.DB.JoinGeometryUtils.UnjoinGeometry(ActiveData.Document, ActiveData.Document.GetElement(ElesId.Last()), ActiveData.Document.GetElement(ElesId.First()));
			}
			catch
			{
			}
		}

		public void FixOverLapElement(List<ElementId> ElesId)
		{
			for (int i = 0; i < ElesId.Count - 1; i++)
			{
				ActiveData.Document.Delete(ElesId[i]);

				ActiveData.Document.Regenerate();
			}
		}

		public void FixRoomSeparationLineOverlap(List<ElementId> ElesId)
		{
			foreach (var i in ElesId)
			{

				if ((i.ToElement() is Wall)) continue;


				ActiveData.Document.Delete(i);

				ActiveData.Document.Regenerate();
			}
		}

		public void FixHighlightedRoomSeparationLinesOverlap(List<ElementId> ElesId)
		{
			for (int i = 0; i < ElesId.Count - 1; i++)
			{
				ActiveData.Document.Delete(ElesId[i]);

				ActiveData.Document.Regenerate();
			}

		}

		public void FixWallIsSlightly(List<ElementId> ElesId)
		{
			foreach (var el in ElesId)
			{
				var wall = ActiveData.Document.GetElement(el) as Wall;

				Curve cur = (wall.Location as LocationCurve).Curve;

				var vec = GetVectorInParallelGrid(cur.Direction());

				if (vec == null) continue;

				var plan = Plane.CreateByNormalAndOrigin(vec.CrossProduct(XYZ.BasisZ), cur.GetEndPoint(0));

				var p1 = cur.GetEndPoint(0).ProjectOnto(plan);

				var p2 = cur.GetEndPoint(1).ProjectOnto(plan);

				Line lnew = Line.CreateBound(p1, p2);

				(wall.Location as LocationCurve).Curve = lnew;
			}
		}

		public void FixBeamIsSlightly(List<ElementId> ElesId)
		{
			foreach (var el in ElesId)
			{
				var beam = ActiveData.Document.GetElement(el) as FamilyInstance;

				Curve cur = (beam.Location as LocationCurve).Curve;

				var vec = GetVectorInParallelGrid(cur.Direction());

				if (vec == null) continue;

				var plan = Plane.CreateByNormalAndOrigin(vec.CrossProduct(XYZ.BasisZ), cur.GetEndPoint(0));

				var p1 = cur.GetEndPoint(0).ProjectOnto(plan);

				var p2 = cur.GetEndPoint(1).ProjectOnto(plan);

				Line lnew = Line.CreateBound(p1, p2);

				(beam.Location as LocationCurve).Curve = lnew;
			}
		}

		public XYZ GetVectorInParallelGrid(XYZ vec)
		{
			foreach (var grid in new FilteredElementCollector(ActiveData.Document).WhereElementIsNotElementType().OfCategory(BuiltInCategory.OST_Grids).Cast<Grid>().ToList())
			{
				var gr = grid.Curve.Direction().DotProduct(vec);

				if ((1 - Math.Abs(gr)) < 0.001)
				{
					return grid.Curve.Direction();
				}
			}

			NoteWarning = "";
			return null;
		}

		public void SetWaring(ObjectWaring wr)
		{
			if (wr.GUIID == GUIService.OverLapWall)
			{
				int countEle = WarningService.CompareSolidWall(wr.ElementWarning).Count;

				if (countEle > 0)
				{
					NoteWarning = "Có " + countEle.ToString() + " vách trùng hoàn toàn lên nhau bạn có muốn xóa các vách có thể tích nhỏ hơn không ?";
				}
				else
				{
					NoteWarning = "";
				}
			}
			else if (wr.GUIID == GUIService.HighlightedFloorsOverlap)
			{
				int countEle = WarningService.CompareSolidFloor(wr.ElementWarning).Count;

				if (countEle > 0)
				{
					NoteWarning = "Có " + countEle.ToString() + " sàn trùng hoàn toàn lên nhau bạn có muốn xóa các vách có thể tích nhỏ hơn không ?";
				}
				else
				{
					NoteWarning = "";
				}
			}
			else if (wr.GUIID == GUIService.OverLapRebar)
			{
				NoteWarning = "Bạn có muốn xóa các thanh thép trùng nhau ? ";
			}
			else if (wr.GUIID == GUIService.WallIsSlightly)
			{
				NoteWarning = "Bạn có muốn Align ? ";
			}
			else if (wr.GUIID == GUIService.BeamOrBraceIsSlightlyOffAxisAndMayCauseInaccuracies)
			{
				NoteWarning = "Bạn có muốn Align ? ";
			}
			else if (wr.GUIID == GUIService.OverLapElement)
			{
				NoteWarning = "Có các đối tượng đang trùng nhau, chúng sẽ được đếm lặp lại trong bảng thống kê . Bạn có muốn loại bỏ 1 trong 2 đối tượng ? ";
			}
			else if (wr.GUIID == GUIService.HighlightedElementsAreJoined)
			{
				NoteWarning = "Bạn có muốn Unjoin để phá vỡ liên kết Join? ";
			}
			else if (wr.GUIID == GUIService.InsertConflicts)
			{
				NoteWarning = "Bạn có muốn Disallow Join với cấu kiện Tường đang va chạm? ";
			}
			else if (wr.GUIID == GUIService.HighlightedRoomSeparationLinesOverlap)
			{
				NoteWarning = "Có các đường phân tách phòng đang trùng nhau, Bạn có muốn loại bỏ 1 trong số chúng ?";
			}
			else if (wr.GUIID == GUIService.RoomTagIsOutsideOfItsRoom)
			{
				NoteWarning = " Bạn có dời Room Tag vào bên trong Room ?";
			}
			else if (wr.GUIID == GUIService.RoomSeparationLineOverlap)
			{
				NoteWarning = " Bạn có muốn xóa Room Separation line ?";
			}
			else if (wr.GUIID == GUIService.RoomSeparationLineIsSlightlyOffAxisAndMayCauseInaccuracies)
			{
				NoteWarning = " Bạn có muốn Align với trục không ?";
			}
			else if (wr.GUIID == GUIService.RoomSeparationLineIsSlightlyOffAxisAndMayCauseInaccuracies)
			{
				NoteWarning = " Bạn có muốn Align với trục không ?";
			}
			else if (wr.GUIID == GUIService.HighlightedInsertsOverlapWithOtherInsertsOrCompletelyMissTheirHosts)
			{
				NoteWarning = " Bạn có muốn xóa đối tượng bắt host bị trùng không ?";
			}
			else
			{
				NoteWarning = "";
			}
		}


		#endregion
	}
}
