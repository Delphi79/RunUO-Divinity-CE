using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Server.Engines.Help;

namespace Server.Misc
{
	public class IRCBot
	{
		public static string Nickname = "";
		public static string Username = "";
		public static string Realname = "";
		public static string Channel  = "";
		public static string ChanKey  = "";

        public static string StaffChannel = "";

		public static string Server   = "";
		public static int    Port     = 6667;

		public static string NickservPassword = "";
		public static bool   ReconnectOnDisc  = true;
		public static bool   Connected        = false;

        private static void OnlineTimer()
        {
            int count = Admin.AdminNetwork.GetNumOnline();
            SendMessage(Channel, String.Format("There {0} currently {1} user{2} online.", 
                count != 1 ? "are" : "is", count, count != 1 ? "s" : ""));
        }

		public static void Initialize()
		{
			//Thread t = new Thread( new ThreadStart( Connect ) );
			//t.Start();

            //Timer.DelayCall( TimeSpan.FromMinutes( 15 ), TimeSpan.FromMinutes( 15 ), new TimerCallback( OnlineTimer ) );
		}

		private static StreamReader SR;
		private static StreamWriter SW;

		public static void Connect()
		{
			try
			{
				TcpClient C = new TcpClient( Server, Port );
				NetworkStream S = C.GetStream();
				SW = new StreamWriter( S );
				SR = new StreamReader( S );
			}
			catch( Exception )
			{
				Thread.Sleep(60000);
				Connect();
			}

			Connected = true;

			SendRaw( "USER "+Username+" 0 * :"+Realname );

			SetNick( Nickname );
			
			Thread.Sleep( 5000 );

			if (NickservPassword != null)
				Identify( NickservPassword );

			if (ChanKey != null)
				JoinChannel( Channel, ChanKey );
			else
				JoinChannel( Channel );

            SendMessage( "ChanServ", "Invite "+StaffChannel );
            Thread.Sleep(5000);
            JoinChannel( StaffChannel );

			GetInput();
		}

		public static void Stop()
		{
			Connected = false;
			SR.Close();
			SW.Close();
		}

		// Public Methods
		public static void NotifyOps( PageEntry p )
		{
			string pLocation = Region.Find(p.PageLocation, p.PageMap).Name;
            if ( pLocation == null || pLocation == "" )
                pLocation = "wilderness";
			
            SendNotice(StaffChannel, String.Format("[{0}] {1} ({2}): {3}", p.Type, p.Sender.Name, pLocation, p.Message));
		}
		
		// Private Methods
		private static void GetInput()
		{
			try
			{
				while ( Connected && ParseIncoming( SR.ReadLine() ) );
			}
			catch( Exception )
			{
				// Something's Wrong... Probably Disconnected
				Connected = false;
			}
			if ( ReconnectOnDisc )
			{
				Thread.Sleep(60000);
				Connect();
			}
		}

		private static bool ParseIncoming( string inputLine )
		{
			if ( inputLine == null)
				return false;
			if ( inputLine.StartsWith("PING") )
				SendRaw( "PONG "+inputLine.Substring(6) );
			return true;
		}

		private static void SendRaw( string Message )
		{
			try
			{
				if ( Connected )
				{
					SW.WriteLine( Message );
					SW.Flush();
				}
			}
			catch ( Exception )
			{
				// Something's Wrong... Probably Disconnected
				Connected = false;
			}
		}

		private static void Identify( string Password )
		{
			SendMessage( "Nickserv", "Identify "+Password );
		}

		private static void Ghost( string Nickname, string Password )
		{
			SendMessage( "Nickserv", "Ghost "+Nickname+" "+Password );
		}

		private static void JoinChannel( string C )
		{
			SendRaw( "JOIN "+C );
		}

		private static void JoinChannel( string C, string K )
		{
			SendRaw( "JOIN "+C+" "+K );
		}

		private static void PartChannel( string C )
		{
			SendRaw( "PART "+C );
		}

		private static void PartChannel( string C, string Message )
		{
			SendRaw( "PART "+C+" :"+Message  );
		}

		private static void Quit()
		{
			SendRaw( "QUIT" );
			Stop();
		}

		private static void Quit( string Message )
		{
			SendRaw( "QUIT :"+Message );
			Stop();
		}

		public static void SendMessage( string Target, string Message )
		{
			SendRaw( "PRIVMSG "+Target+" :"+Message );
		}

        public static void StaffNotice(string Message)
        {
            SendRaw("NOTICE " + StaffChannel + " :" + Message);
        }

		private static void SendNotice( string Target, string Message )
		{
			SendRaw( "NOTICE "+Target+" :"+Message );
		}

		private static void SetMode( string Target, string Modes )
		{
			SendRaw( "MODE "+Target+" :"+Modes );
		}

		private static void SetNick( string N )
		{
			SendRaw( "NICK "+N );
		}
	}
}