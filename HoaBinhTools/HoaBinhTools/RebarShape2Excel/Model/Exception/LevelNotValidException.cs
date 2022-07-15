namespace Model.Exception
{
	public class LevelNotValidException : System.Exception
	{
		public override string Message => "This level have been invalid!";
	}
}
