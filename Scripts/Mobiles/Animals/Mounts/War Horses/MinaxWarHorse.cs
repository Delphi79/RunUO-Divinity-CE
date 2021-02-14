using System;
using Server;

namespace Server.Mobiles
{
	public class MinaxWarHorse : BaseWarHorse
	{
		[Constructable]
		public MinaxWarHorse() : base( 0x78, 0x3EAF, AIType.AI_Melee, FightMode.Aggressor, 12, 1, 0.5, 0.75 )
		{
		}

		public MinaxWarHorse( Serial serial ) : base( serial )
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