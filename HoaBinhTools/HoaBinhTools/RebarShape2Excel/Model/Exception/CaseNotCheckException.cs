namespace Model.Exception
{
	public class CaseNotCheckException : System.Exception
	{
		public override string Message => "This case have not been checked yet!";
	}
}
