using System;
using Server;
using Server.Gumps;

namespace Server.Mobiles
{
	public class PricedHealer : BaseHealer
	{
		private int m_Price;

		[CommandProperty( AccessLevel.GameMaster )]
		public int Price
		{
			get{ return m_Price; }
			set{ m_Price = value; }
		}

		[Constructable]
		public PricedHealer() : this( 5000 )
		{
		}

		[Constructable]
		public PricedHealer( int price )
		{
			m_Price = price;
		}

		public override void InitSBInfo()
		{
		}

		public override bool HealsYoungPlayers{ get{ return false; } }

		public override void OfferResurrection( Mobile m )
		{
			Direction = GetDirectionTo( m );

			m.PlaySound( 0x214 );
			m.FixedEffect( 0x376A, 10, 16 );

			//m.CloseGump( typeof( ResurrectGump ) );
			//m.SendGump( new ResurrectGump( m, this, m_Price ) );
			if ( m.NetState != null )
				new ResurrectMenu(m, this, ResurrectMessage.Healer).SendTo(m.NetState);
		}

		public override bool CheckResurrect( Mobile m )
		{
			return true;
		}

		public PricedHealer( Serial serial ) : base( serial )
		{
            this.Blessed = true;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			writer.Write( (int) m_Price );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 0:
				{
					m_Price = reader.ReadInt();
					break;
				}
			}
		}
	}
}