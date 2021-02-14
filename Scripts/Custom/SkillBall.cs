using System;
using System.Collections;
using System.IO;
using Server;
using Server.Gumps;
using Server.Network;

namespace Server.Items
{
	public class SkillBall : Item
	{
		[Constructable]
		public SkillBall() : base( 0xE73 )
		{
			Name = "7x Chooser";
			LootType = LootType.Blessed;
			Hue = 0x482;
		}

		public SkillBall( Serial serial ) : base( serial )
		{
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( this.RootParent == from )
			{
				from.CloseGump( typeof( SevenSkillsGump ) );
				BitArray ba = new BitArray( SkillInfo.Table.Length );
				from.SendGump( new SevenSkillsGump( ba, this ) );
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

	public class SevenSkillsGump : Gump
	{
		BitArray m_Skills;
		Item m_Sender;
		int m_Page;

		public SevenSkillsGump( BitArray ba, Item sender ) : this( ba, sender, 0 )
		{
		}

		public SevenSkillsGump( BitArray ba, Item sender, int page ) : base( 50, 50 )
		{
			if ( page < 0 )
				return;

			m_Page = page;
			m_Skills = ba;
			m_Sender = sender;

			Closable=true;
			Dragable=true;
			Resizable=false;

			AddPage(0);

			AddBackground(10, 10, 225, 425, 9380);
			AddLabel(73, 15, 1152, "Choose 7 Skills" );

			for ( int i = 0; i < 8; i++ )
			{
				int curSkill = i + ( page * 8 );

				if ( curSkill >= SkillInfo.Table.Length || SkillInfo.Table[curSkill] == null || ( !Core.AOS && curSkill > (int)SkillName.RemoveTrap ) )
					break;

				AddCheck(40, 55 + ( 45 * i ), 210, 211, ba[curSkill], i + ( page * 8 ) + 100 );

				AddLabel(70, 55 + ( 45 * i ) , 0, SkillInfo.Table[curSkill].Name );
			}

			AddButton(91, 411, 247, 248, 1, GumpButtonType.Reply, 0);
			//Okay Button ->  # 1

			if ( ( Core.AOS ? SkillInfo.Table.Length : (int)SkillName.RemoveTrap+1 ) - ( page * 8 + 8 ) > 0 )
			{
				AddButton(190, 412, 4005, 4007, 2, GumpButtonType.Reply, 0);
				//Forward button -> #2
			}

			if ( page > 0 )
			{
				AddButton(29, 412, 4014, 4016, 3, GumpButtonType.Reply, 0);
				//Back Button -> #3
			}
		}

		public override void OnResponse( NetState sender, RelayInfo info )
		{
			if ( m_Sender == null || m_Sender.Deleted || m_Sender.RootParent != sender.Mobile )
				return;

			Mobile m = sender.Mobile;

			for ( int i = 0; i < m_Skills.Length; i++ )
			{

				if ( info.IsSwitched( i + 100 ) )
				{
					m_Skills[i] = info.IsSwitched( i + 100 );
				}
			}

			if ( info.ButtonID == 1 )
			{

				ArrayList chosen = new ArrayList();

				for ( int i = 0; i < m_Skills.Length; i++ )
				{
					if ( m_Skills[i] )
						chosen.Add( i );
				}

				if ( chosen.Count != 7 )
				{
					m.SendMessage( "You must choose 7 skills!" );
				}
				else
				{
					for ( int i = 0; i < m.Skills.Length; i++ )
						m.Skills[i].Base = 0.0;

					for ( int i = 0; i < chosen.Count; i++ )
						m.Skills[(int)chosen[i]].Base = 100.0;

					using ( StreamWriter sw = new StreamWriter( "Logs\\Donations\\UsedItems.log", true ) ) {
						sw.WriteLine( "[{4}] NAME: {0} ACCOUNT: {1} IP: {2} used skillball {3}", m, m.Account, m.NetState, m_Sender.Serial, DateTime.Now );
					}

					m.SendMessage( "Your skills have been set!" );
					m_Sender.Delete();
				}
			}
			else if ( info.ButtonID == 2 )
			{
				m.SendGump( new SevenSkillsGump( m_Skills, m_Sender, ++m_Page ) );
			}
			else if ( info.ButtonID == 3 )
			{
				m.SendGump( new SevenSkillsGump( m_Skills, m_Sender, --m_Page ) );
			}
		}
	}
}