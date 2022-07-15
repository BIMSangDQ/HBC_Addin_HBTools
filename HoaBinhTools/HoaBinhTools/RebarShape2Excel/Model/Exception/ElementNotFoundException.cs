namespace Model.Exception
{
	public class ElementNotFoundException : System.Exception
	{
		public override string Message
		{
			get { return "Element not found!"; }
		}
	}
}