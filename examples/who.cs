namespace Linux.User.Examples
{
	using System;
	using Linux.User;

	class	WhoExample
	{
		public static void Main()
		{
			Utmp ut	= new Utmp();


			while(ut.MoreRecords())
			{
				Utent utent = ut.GetUtent();
				if (utent.ut_type == UtType.UserProcess)
				{
					Console.WriteLine("{0,-9}{1,-12}{2,-20}({3})",utent.ut_user, utent.ut_line,utent.ut_tv.Date, utent.ut_host);
				}
			}
		}
	}
}
