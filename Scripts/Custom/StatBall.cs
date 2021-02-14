using System;
using System.IO;
using Server;
using Server.Gumps;
using Server.Network;

namespace Server.Items
{
	public class StatBall : Item
	{
		[Constructable]
		public StatBall() : base( 0xE73 )
		{
			Name = "225 Chooser";
			LootType = LootType.Blessed;
			Hue = 0x20;
		}

		public StatBall( Serial serial ) : base( serial )
		{
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( this.RootParent == from )
			{
				from.CloseGump( typeof( StatBallGump ) );
				from.SendGump( new StatBallGump( this ) );
			}
			else
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class StatBallGump : Gump
	{
		Item m_Sender;

		public void AddBlackAlpha( int x, int y, int width, int height )
		{
			AddImageTiled( x, y, width, height, 2624 );
			AddAlphaRegion( x, y, width, height );
		}

		public void AddTextField( int x, int y, int width, int height, int index )
		{
			AddBackground( x - 2, y - 2, width + 4, height + 4, 0x2486 );
			AddTextEntry( x + 2, y + 2, width - 4, height - 4, 0, index, "" );
		}

		public string Center( string text )
		{
			return String.Format( "<CENTER>{0}</CENTER>", text );
		}

		public string Color( string text, int color )
		{
			return String.Format( "<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", color, text );
		}

		public void AddButtonLabeled( int x, int y, int buttonID, string text )
		{
			AddButton( x, y - 1, 4005, 4007, buttonID, GumpButtonType.Reply, 0 );
			AddHtml( x + 35, y, 240, 20, Color( text, 0xFFFFFF ), false, false );
		}

		public StatBallGump( Item sender ) : base( 50, 50 )
		{
			m_Sender = sender;

			Closable=true;
			Dragable=true;
			Resizable=false;

			AddPage(0);

			AddBlackAlpha( 10, 120, 275, 150 );
			AddHtml( 10, 125, 275, 20, Color( Center( "Distribute 225 Stat Points" ), 0xFFFFFF ), false, false );

			AddLabel( 73, 15, 1152, "" );
			AddLabel( 20, 150, 0x480, "Strength:" );
			AddTextField( 150, 150, 40, 20, 0 );

			AddLabel( 20, 180, 0x480, "Dexterity:" );
			AddTextField( 150, 180, 40, 20, 1 );

			AddLabel( 20, 210, 0x480, "Intelligence:" );
			AddTextField( 150, 210, 40, 20, 2 );

			AddButtonLabeled( 75, 240, 1, "Submit" );
		}

		public override void OnResponse( NetState sender, RelayInfo info )
		{
			if ( m_Sender == null || m_Sender.Deleted || info.ButtonID != 1 || m_Sender.RootParent != sender.Mobile )
				return;

			Mobile m = sender.Mobile;
			TextRelay strEntry = info.GetTextEntry( 0 );
			TextRelay dexEntry = info.GetTextEntry( 1 );
			TextRelay intEntry = info.GetTextEntry( 2 );

			int Str, Dex, Int;

			try
			{
				Str = Int32.Parse( strEntry == null ? null : strEntry.Text.Trim() );
				Dex = Int32.Parse( dexEntry == null ? null : dexEntry.Text.Trim() );
				Int = Int32.Parse( intEntry == null ? null : intEntry.Text.Trim() );
			}
			catch ( Exception )
			{
				m.SendMessage( "You must complete the form with numeric values before clicking submit." );
				return;
			}

				
			if ( (Str+Dex+Int) != 225 )
			{
				m.SendMessage( "Your stats must total 225." );
               			m.SendGump( new StatBallGump( m_Sender ) );
			}
			else if ( Str > 100 || Dex > 100 || Int > 100 )
			{
				m.SendMessage( "You cannot exceed 100 in any stat." );
                		m.SendGump( new StatBallGump( m_Sender ) );
			}
			else if ( Str < 10 || Dex < 10 || Int < 10 )
			{
				m.SendMessage( "You must have more than 10 in each stat." );
                		m.SendGump( new StatBallGump( m_Sender ) );
			}
			else
			{
				m.RawStr = 0;
				m.RawDex = 0;
				m.RawInt = 0;

				m.RawStr = Str;
				m.RawDex = Dex;
				m.RawInt = Int;

				m.Hits = m.HitsMax;
				m.Stam = m.Dex;
				m.Mana = m.Int;

				using ( StreamWriter sw = new StreamWriter( "Logs\\Donations\\UsedItems.log", true ) ) {
					sw.WriteLine( "[{4}] NAME: {0} ACCOUNT: {1} IP: {2} used statball {3}", m, m.Account, m.NetState, m_Sender.Serial, DateTime.Now );
				}

				m.SendMessage( "Your stats have been set!" );
				m_Sender.Delete();
			}
		}
	}
}