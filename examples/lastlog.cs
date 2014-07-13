
namespace Linux.User.Examples
{
	using System;
	using Linux.User;
	class LastLogExample
	{
		public static void Main()
		{
			LastLog ll = new LastLog();

			Password pw	= new Password();

			LastLogEntry llent;
			Console.WriteLine("Username         Port     From             Latest");
			while((llent = ll.GetLlEnt()) != null )
			{
				PasswordEntry pwent;
			
				if ((pwent = pw.GetPWUid((int)llent.Uid)) != null)
				{
					String dt;
					if ( llent.ll_time != 0 )
					{
						dt = llent.Time.ToString("F");
					}
					else
					{
						dt = "**Never logged in**";
					}
					Console.WriteLine("{0,-17}{1,-9}{2,-17}{3}", pwent.Name, llent.Line, llent.Host, dt);
				}
			}

		}
	}
}
