using System;
using Server;

namespace Server.Mobiles
{
	public class CoMWarHorse : BaseWarHorse
	{
		[Constructable]
		public CoMWarHorse() : base( 0x77, 0x3EB1, AIType.AI_Melee, FightMode.Aggressor, 12, 1, 0.5, 0.75 )
		{
		}

		public CoMWarHorse( Serial serial ) : base( serial )
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
	}
}