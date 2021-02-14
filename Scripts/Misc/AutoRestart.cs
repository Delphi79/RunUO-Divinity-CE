using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using Server;
using Server.Commands;

namespace Server.Misc
{
	public class AutoRestart : Timer
	{
		public static bool Enabled = true; // is the script enabled?
		private static bool Weekly = true;

		private static DayOfWeek RestartDay = DayOfWeek.Sunday;
		private static TimeSpan RestartTime = TimeSpan.FromMinutes( 5.0 ); // time of day at which to restart
		private static TimeSpan RestartDelay = TimeSpan.FromMinutes( 30.0 ); // how long the server should remain active before restart (period of 'server wars')

		private static TimeSpan WarningDelay = TimeSpan.FromMinutes( 1.0 ); // at what interval should the shutdown message be displayed?

		private static bool m_Restarting;
		private static DateTime m_RestartTime;

		public static bool Restarting
		{
			get{ return m_Restarting; }
		}

		public static void Initialize()
		{
			CommandSystem.Register( "Restart", AccessLevel.Administrator, new CommandEventHandler( Restart_OnCommand ) );

			new AutoRestart().Start();
		}

		private static string FormatDirectory( string root, string name, string timeStamp )
		{
			return Path.Combine( root, String.Format( "{0} ({1})", name, timeStamp ) );
		}

		private static string GetTimeStamp()
		{
			DateTime now = DateTime.Now;

			return String.Format( "{0}-{1}-{2} {3}-{4:D2}-{5:D2}",
					now.Day,
					now.Month,
					now.Year,
					now.Hour,
					now.Minute,
					now.Second
				);
		}

		public static void Restart_OnCommand( CommandEventArgs e )
		{
			if ( m_Restarting )
			{
				e.Mobile.SendMessage( "The server is already restarting." );
			}
			else
			{
				e.Mobile.SendMessage( "You have initiated server shutdown." );
				Enabled = true;
				m_RestartTime = DateTime.Now;
			}
		}

		public AutoRestart() : base( TimeSpan.FromSeconds( 1.0 ), TimeSpan.FromSeconds( 1.0 ) )
		{
			Priority = TimerPriority.FiveSeconds;
			
			DateTime current = DateTime.Now;

			if ( Weekly ) {
				m_RestartTime = current.Date + RestartTime + TimeSpan.FromDays( (int)RestartDay - (int)current.DayOfWeek );

				if ( m_RestartTime < current )
					m_RestartTime += TimeSpan.FromDays( 7.0 );
			} else {
				m_RestartTime = current.Date + RestartTime;

				if ( m_RestartTime < current )
					m_RestartTime += TimeSpan.FromDays( 1.0 );
			}
		}

		private void Warning_Callback()
		{
			World.Broadcast( 0x25, true, "The server is going down shortly." );
		}

		private void Restart_Callback()
		{
            Warning_Callback(); // one final message
			Core.Kill( true ); // shutdown now.
		}

		protected override void OnTick()
		{
			if ( m_Restarting || !Enabled )
				return;

			if ( DateTime.Now < m_RestartTime )
				return;

			if ( WarningDelay > TimeSpan.Zero )
			{
				Warning_Callback();
				Timer.DelayCall( WarningDelay, WarningDelay, new TimerCallback( Warning_Callback ) );
			}

            Warning_Callback(); // warn immediately
            AutoSave.Save(); // perform the final save

			m_Restarting = true;

			Timer.DelayCall( RestartDelay, new TimerCallback( Restart_Callback ) );
		}
	}
}