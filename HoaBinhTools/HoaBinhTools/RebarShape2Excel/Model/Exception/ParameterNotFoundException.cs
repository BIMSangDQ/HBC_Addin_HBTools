namespace Model.Exception
{
	public class ParameterNotFoundException : System.Exception
	{
		public override string Message => "Parameter not found!";
	}
}
