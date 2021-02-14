using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a ghostly corpse" )]
	public class Wraith : BaseCreature
	{
		[Constructable]
		public Wraith() : base( AIType.AI_Mage, FightMode.Closest, 12, 1, 0.5, 0.75 )
		{
			Name = "a wraith";
			Body = 26;
			Hue = 0x4001;
			BaseSoundID = 0x482;

			SetStr( 102, 112 );
			SetDex( 76, 95 );
			SetInt( 36, 100 );

			SetHits( 46, 90 );

			SetDamage( 11, 21 );

			SetDamageType( ResistanceType.Physical, 50 );
			SetDamageType( ResistanceType.Cold, 50 );

			SetResistance( ResistanceType.Physical, 25, 30 );
			SetResistance( ResistanceType.Cold, 15, 25 );
			SetResistance( ResistanceType.Poison, 10, 20 );

			SetSkill( SkillName.EvalInt, 55.1, 70.0 );
			SetSkill( SkillName.Magery, 55.1, 80 );
			SetSkill( SkillName.MagicResist, 65, 85 );
			SetSkill( SkillName.Tactics, 45.1, 60.0 );
			SetSkill( SkillName.Wrestling, 45.1, 55.0 );

			Fame = 4000;
			Karma = -4000;
			BardLevel = 80;

			VirtualArmor = 28;

			PackReg( 10 );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average );
		}
		
		public override bool BleedImmune{ get{ return true; } }

		public override OppositionGroup OppositionGroup
		{
			get{ return OppositionGroup.FeyAndUndead; }
		}

		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }

		public Wraith( Serial serial ) : base( serial )
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