namespace SingleData
{
	public class Singleton
	{
		#region Variables
		private RevitData revitData;
		private ModelData modelData;
		private IOData ioData;
		private ExcelData excelData;
		private SecurityData securityData;
		private WPFData wpfData;
		#endregion

		#region Properties
		public static Singleton Instance { get; set; }
		public RevitData RevitData
		{
			get
			{
				if (revitData == null) revitData = new RevitData();
				return revitData;
			}
		}
		public ModelData ModelData
		{
			get
			{
				if (modelData == null) modelData = new ModelData();
				return modelData;
			}
		}
		public IOData IOData
		{
			get
			{
				if (ioData == null) ioData = new IOData();
				return ioData;
			}
		}
		public ExcelData ExcelData
		{
			get
			{
				if (excelData == null) excelData = new ExcelData();
				return excelData;
			}
		}
		public SecurityData SecurityData
		{
			get
			{
				if (securityData == null) securityData = new SecurityData();
				return securityData;
			}
		}
		public WPFData WPFData
		{
			get
			{
				if (wpfData == null) wpfData = new WPFData();
				return wpfData;
			}
		}
		#endregion

		private TemplateData templateData;
		public TemplateData TemplateData
		{
			get
			{
				if (templateData == null) templateData = new TemplateData();
				return templateData;
			}
		}

		private FormData formData;
		public FormData FormData
		{
			get
			{
				if (formData == null) formData = new FormData();
				return formData;
			}
		}
	}
}
