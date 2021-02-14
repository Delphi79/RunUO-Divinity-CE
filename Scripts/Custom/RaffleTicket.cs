using System;
using Server.Network;

namespace Server.Items
{
	public class RaffleTicket : Item
	{
		private static int m_LastTicket = 0;

		private int m_Number;

		[CommandProperty( AccessLevel.Counselor )]
		public int Number { get { return m_Number; } }

		public RaffleTicket() : base( 0x2aaa )
		{
			Weight = 1.0;
			Hue = 0x482;
			LootType = LootType.Blessed;

			m_Number = ++m_LastTicket;

			Name = String.Format( "a raffle ticket [#{0:D6}]", m_Number ); // a raffle ticket [#005423]
		}

		public RaffleTicket( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			writer.Write( (int) m_Number );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			m_Number = reader.ReadInt();

			if ( m_Number > m_LastTicket )
				m_LastTicket = m_Number;
		}
	}
}


