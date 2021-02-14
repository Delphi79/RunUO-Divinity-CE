using System;
using Server;
using Server.Accounting;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;
using Server.Commands;

namespace Server.Commands
{
	public class ManageEmail
	{
		public static void Initialize()
		{
			CommandSystem.Register( "EMail", AccessLevel.Administrator, new CommandEventHandler( EMail_OnCommand ) );
		}

		[Usage( "EMail" )]
		[Description( "View and modify the targeted player's account e-mail." )]
		public static void EMail_OnCommand( CommandEventArgs e )
		{
			e.Mobile.BeginTarget( -1, false, TargetFlags.None, new TargetCallback( EMail_OnTarget ) );
			e.Mobile.SendMessage( "Target a player to view or modify their account e-mail." );
		}

		public static void EMail_OnTarget( Mobile from, object obj )
		{
			if ( obj is Mobile && ((Mobile)obj).Player )
			{
				Mobile m = (Mobile)obj;
				Account acct = m.Account as Account;

				if ( acct == null )
				{
					from.SendMessage( "The targeted player does not have an account." );
					return;
				}

				from.SendGump( new ViewMailGump( m, acct ) );
			}
			else
			{
				from.BeginTarget( -1, false, TargetFlags.None, new TargetCallback( EMail_OnTarget ) );
				from.SendMessage( "That is not a player. Try again." );
			}
		}
	}

	public class ViewMailGump : Gump
	{
		private Mobile m_Mob;
		private Account m_Acct;
		
		public void AddBlackAlpha( int x, int y, int width, int height )
		{
			AddImageTiled( x, y, width, height, 2624 );
			AddAlphaRegion( x, y, width, height );
		}

		public void AddButtonLabeled( int x, int y, int buttonID, string text )
		{
			AddButton( x, y - 1, 4005, 4007, buttonID, GumpButtonType.Reply, 0 );
			AddHtml( x + 35, y, 240, 20, Color( text, LabelColor32 ), false, false );
		}

		public void AddTextField( int x, int y, int width, int height, int index )
		{
			AddBackground( x - 2, y - 2, width + 4, height + 4, 0x2486 );
			AddTextEntry( x + 2, y + 2, width - 4, height - 4, 0, index, "" );
		}
		
		public int GetButtonID( int type, int index )
		{
			return 1 + (index * 10) + type;
		}

		public string Center( string text )
		{
			return String.Format( "<CENTER>{0}</CENTER>", text );
		}

		public string Color( string text, int color )
		{
			return String.Format( "<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", color, text );
		}

		private const int LabelColor32 = 0xFFFFFF;
		private const int LabelHue = 0x480;

		public ViewMailGump( Mobile m, Account a ) : base( 20, 30 )
		{
			m_Mob = m;
			m_Acct = a;
			
			AddPage( 0 );

			AddBlackAlpha( 10, 120, 360, 180 );
			AddHtml( 10, 125, 400, 20, Color( Center( "Account E-Mail" ), LabelColor32 ), false, false );

			AddLabel(  20, 150, LabelHue, "Username:" );
			AddLabel( 200, 150, LabelHue, a.Username );

			AddLabel(  20, 180, LabelHue, "Current E-Mail:" );
			AddLabel( 200, 180, LabelHue, a.GetTag( "EMail" ) );

			AddLabel( 20, 210, LabelHue, "New E-Mail:" );
			AddTextField( 200, 210, 160, 20, 0 );

			AddLabel( 20, 240, LabelHue, "Confirm:" );
			AddTextField( 200, 240, 160, 20, 1 );

			AddButtonLabeled( 50, 270, GetButtonID( 5, 12 ), "Submit Change" );
		}

		public override void OnResponse( NetState state, RelayInfo info )
		{
			Mobile from = state.Mobile;
			TextRelay emEntry = info.GetTextEntry( 0 );
			TextRelay confirmEntry = info.GetTextEntry( 1 );

			string em = ( emEntry == null ? null : emEntry.Text.Trim() );
			string confirm = ( confirmEntry == null ? null : confirmEntry.Text.Trim() );

			if ( em == null || em.Length == 0 )
			{
				if ( confirm == null || confirm.Length == 0 )
				{
					from.SendMessage( "The account's e-mail address has been cleared." );
					m_Acct.RemoveTag( "EMail" );
				}
			}
			else if ( confirm != em )
			{
				from.SendMessage( "You must confirm the e-mail. The field must match exactly. Try again." );
				from.SendGump( new ViewMailGump( m_Mob, m_Acct ) );
			}
			else
			{
				from.SendMessage( "The account's e-mail address has been successfully changed." );
				m_Mob.SendMessage( "Your account's e-mail has been changed to {0}.", em );
				m_Acct.SetTag( "EMail", em );
			}
		}
	}
}