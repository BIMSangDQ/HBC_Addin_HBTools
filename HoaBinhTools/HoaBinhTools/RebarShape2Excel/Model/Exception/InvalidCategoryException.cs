namespace Model.Exception
{
	public class InvalidCategoryException : System.Exception
	{
		public override string Message => "This category have not been checked!";
	}
}
