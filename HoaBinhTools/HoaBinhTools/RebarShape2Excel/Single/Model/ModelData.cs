namespace SingleData
{
	public partial class ModelData
	{
		public static ModelData Instance { get { return Singleton.Instance.ModelData; } }

		private RevitModelData revitModelData;
		public RevitModelData RevitModelData { get { if (revitModelData == null) revitModelData = new RevitModelData(); return revitModelData; } }
	}
}
