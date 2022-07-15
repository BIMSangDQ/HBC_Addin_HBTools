using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using HoaBinhTools.AutocadToRevit.Utils;
using Utils;

namespace HoaBinhTools.BIMQAQC.ModelChecker.Models.GeneralCheckSet
{
	public class ParaProjectInfors
	{
		
		public string Name { get; set; }

		public List<string> Params { get; set; }

		public ParaProjectInfors(List<string> p, string name)
		{
			Name = name;
			Params = p;
		}
	}

	public class CategoryInfors
	{

		public string Name { get; set; }

		public List<string> Categories { get; set; }

		public CategoryInfors(List<string> p, string name)
		{
			Name = name;
			Categories = p;
		}
	}
}
