using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a water elemental corpse" )]
	public class SummonedWaterElemental : BaseCreature
	{
		public override double DispelDifficulty{ get{ return 117.5; } }
		public override double DispelFocus{ get{ return 45.0; } }

		[Constructable]
		public SummonedWaterElemental () : base( AIType.AI_Mage, FightMode.Closest, 12, 1, 0.5, 0.75 )
		{
			Body = 16;
			Name = "a water elemental";
			SetStr( 116, 135 );
			SetHits( 116, 135 );
			SetDex( 56, 65 );
			SetStam( 56, 65 );
			SetInt( 61, 75 );
			SetMana( 61, 75 );

			SetSkill( SkillName.Wrestling, 70.1, 90 );
			SetSkill( SkillName.Parry, 55.1, 65 );
			SetSkill( SkillName.Tactics, 80.1, 100 );
			SetSkill( SkillName.MagicResist, 60.1, 75 );
			SetSkill( SkillName.Magery, 60.1, 75 );

			VirtualArmor = 19;
			SetDamage( 4, 12 );
		
			CanSwim = true;
		}

		public SummonedWaterElemental( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}