using System;
using System.Collections.ObjectModel;
using Autodesk.Revit.DB;
using Utils;

namespace HoaBinhTools.BIMQAQC.ModelChecker.Models
{
	public class FileInfor : ViewModelBase
	{
		public string FileName { get; set; }

		public string FilePath { get; set; }

		public double FileSize { get; set; }

		public int Warnings { get; set; }

		public int Total_Element { get; set; }

		public int Purable_Elements {get;set;}

		public int Model_Groups { get; set; }

		public int Detail_Groups { get; set; }

		public int In_Place_Families { get; set; }

		public int Duplicate_Intances { get; set; }

		public int Views { get; set; }

		public int Sheets { get; set; }

		public int Hidden_Elements { get; set; }

		public int Cad_Imports { get; set; }

		public int Link_Revit { get; set; }

		public int Linked_Cad { get; set; }

		public int Worksets { get; set; }

		private bool _File_name_Check;
		public bool File_name_Check
		{
			get
			{
				return _File_name_Check;
			}
			set
			{
				_File_name_Check = value;
				OnPropertyChanged("File_name_Check");
			}
		}

		private bool _ProjectInfor_Check;
		public bool ProjectInfor_Check
		{
			get
			{
				return _ProjectInfor_Check;
			}
			set
			{
				_ProjectInfor_Check = value;
				OnPropertyChanged("ProjectInfor_Check");
			}
		}

		private bool _ProjectLocation_Check;
		public bool ProjectLocation_Check 
		{
			get
			{
				return _ProjectLocation_Check;
			}
			set
			{
				_ProjectLocation_Check = value;
				OnPropertyChanged("ProjectLocation_Check");
			}
		}

		private int _Levels_Workset_Check;
		public int Levels_Workset_Check
		{
			get
			{
				return _Levels_Workset_Check;
			}
			set
			{
				_Levels_Workset_Check = value;
				OnPropertyChanged("Levels_Workset_Check");
			}
		}

		private int _WrongElement_Check;
		public int WrongElement_Check
		{
			get
			{
				return _WrongElement_Check;
			}
			set
			{
				_WrongElement_Check = value;
				OnPropertyChanged("WrongElement_Check");
			}
		}
		public string TimeSpan { get; set; }

		public string fileResultsDescription = "";
		public string FileResultsDescription
		{
			get
			{
				return fileResultsDescription;
			}
			set
			{
				fileResultsDescription = value;
				OnPropertyChanged("FileResultsDescription");
			}
		}

		public string fileGeneralResultsDescription = "";
		public string FileGeneralResultsDescription
		{
			get
			{
				return fileGeneralResultsDescription;
			}
			set
			{
				fileGeneralResultsDescription = value;
				OnPropertyChanged("FileGeneralResultsDescription");
			}
		}

		public string fileCheckResultsDescription = "";
		public string FileCheckResultsDescription
		{
			get
			{
				return fileCheckResultsDescription;
			}
			set
			{
				fileCheckResultsDescription = value;
				OnPropertyChanged("FileCheckResultsDescription");
			}
		}

		public ObservableCollection<FileCheckResult> _FileCheckResults;
		public ObservableCollection<FileCheckResult> FileCheckResults
		{
			get
			{
				return _FileCheckResults;
			}
			set
			{
				_FileCheckResults = value;
				OnPropertyChanged("FileCheckResults");
			}
		}

	}

	public class FileCheckResult : ViewModelBase
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string Failure_Message { get; set; }
		public bool Result { get; set; }
		public int ElementCount {get; set; }
		private ObservableCollection<ListElementResult> _Elements { get; set; }
		public ObservableCollection<ListElementResult> Elements 
		{
			get
			{
				return _Elements;
			}
			set
			{
				_Elements = value;
				OnPropertyChanged("Elements");
			}
		}
		public bool IsImportant { get; set; }
		public string SetRange { get; set; }
		public string ListParaResult { get; set; }

		private string para1;
		public string Para1
		{
			get
			{
				return para1;
			}
			set
			{
				para1 = value;
				OnPropertyChanged("Para1");
			}
		}

		private string para2;
		public string Para2
		{
			get
			{
				return para2;
			}
			set
			{
				para2 = value;
				OnPropertyChanged("Para2");
			}
		}

		private string para3;
		public string Para3
		{
			get
			{
				return para3;
			}
			set
			{
				para3 = value;
				OnPropertyChanged("Para3");
			}
		}

	}

	public class ListElementResult : ViewModelBase
	{
		public string Category { get; set; }
		public string Type { get; set; }
		public string Name { get; set; }
		public string HB_ElementName {get; set; }
		public ElementId ElementId { get; set; }
		private string para1;
		public string Para1
		{
			get
			{
				return para1;
			}
			set
			{
				para1 = value;
				OnPropertyChanged("Para1");
			}
		}

		private string para2;
		public string Para2
		{
			get
			{
				return para2;
			}
			set
			{
				para2 = value;
				OnPropertyChanged("Para2");
			}
		}

		private string para3;
		public string Para3
		{
			get
			{
				return para3;
			}
			set
			{
				para3 = value;
				OnPropertyChanged("Para3");
			}
		}
		public string Para1Value { get; set; }

		public string Para2Value { get; set; }

		public string Para3Value { get; set; }

	}
}
