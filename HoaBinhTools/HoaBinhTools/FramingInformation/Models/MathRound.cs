namespace HoaBinhTools.FramingInformation.Models.FramingInformationCmd
{
	public static class MathRound
	{
		public static int Round10(this int toRound)
		{
			if (toRound % 10 == 0) return toRound;

			if ((toRound % 10) >= 5)
			{
				return (10 - toRound % 10) + toRound;
			}
			else
			{
				return toRound - (toRound % 10);
			}
		}

		public static int Round5(this int toRound)
		{
			if (toRound % 5 < 2.5)
			{
				return toRound - toRound % 5;
			}
			else
			{
				return toRound + (5 - toRound % 5);
			}
		}
	}
}
