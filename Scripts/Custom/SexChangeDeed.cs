using System;
using Server.Gumps;
using Server.Network;
using Server.Prompts;

namespace Server.Items
{
	public class SexChangeDeed : Item
	{
		[Constructable]
		public SexChangeDeed() : base( 0x14F0 )
		{
			Name = "a sex change deed";
			LootType = LootType.Blessed;
		}

		public SexChangeDeed( Serial serial ) : base( serial )
		{
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

		public override void OnDoubleClick( Mobile from )
		{
			if ( this.RootParent == from )
			{
				from.CloseGump( typeof( SexChangeDeedGump ) );
				from.SendGump( new SexChangeDeedGump( this ) );
			}
			else
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
		}
	}

	public class SexChangeDeedGump : Gump
	{
		Item m_Sender;

		public void AddBlackAlpha( int x, int y, int width, int height )
		{
			AddImageTiled( x, y, width, height, 2624 );
			AddAlphaRegion( x, y, width, height );
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

		public SexChangeDeedGump( Item sender ) : base( 50, 50 )
		{
			m_Sender = sender;

			Closable=true;
			Dragable=true;
			Resizable=false;

			AddPage(0);

			AddBlackAlpha( 10, 120, 275, 110 );
			AddHtml( 10, 125, 275, 20, Color( Center( "Sex Change Deed" ), 0xFFFFFF ), false, false );

			AddHtml( 20, 155, 275, 20, Color( "Choose your new sex below:" , 0xFFFFFF ), false, false );

			AddButtonLabeled( 75, 195, 1, "Male" );
			AddButtonLabeled( 150, 195, 2, "Female" );
		}

		public override void OnResponse( NetState sender, RelayInfo info )
		{
			if ( m_Sender == null || m_Sender.Deleted || ( info.ButtonID != 1 && info.ButtonID != 2 ) || m_Sender.RootParent != sender.Mobile )
				return;

			Mobile m = sender.Mobile;
				
			if ( info.ButtonID == 1 )
			{
				if ( !m.Female )
					m.SendMessage( "You are already male!" );
				else
				{
					m.Female = false;
					m.BodyValue = 0x190;
					m.SendMessage( "You are now male!" );
					m_Sender.Delete();
				}
			}
			else if ( info.ButtonID == 2 )
			{
				if ( m.Female )
					m.SendMessage( "You are already female!" );
				else
				{
					m.Female = true;
                    m.FacialHairItemID = 0;
					m.BodyValue = 0x191;
					m.SendMessage( "You are now female!" );
					m_Sender.Delete();
				}
			}
		}
	}
}


