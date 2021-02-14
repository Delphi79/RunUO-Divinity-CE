using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a scorpion corpse" )]
	public class Scorpion : BaseCreature
	{
		[Constructable]
        public Scorpion()
            : base(AIType.AI_Melee, FightMode.Closest, 12, 1, 0.5, 0.75)
        {
            Body = 48;
            Name = "a scorpion";
            SetStr(73, 115);
            SetHits(73, 115);
            SetDex(76, 95);
            SetStam(151, 170);
            SetInt(16, 30);
            SetMana(0);

            Fame = 2000;
            Karma = -2000;

            Tamable = true;
            MinTameSkill = 60;
            BaseSoundID = 397;
            SetSkill(SkillName.Wrestling, 50.3, 65);
            SetSkill(SkillName.Parry, 60.1, 70);
            SetSkill(SkillName.Tactics, 60.3, 75);
            SetSkill(SkillName.MagicResist, 30.1, 35);

            VirtualArmor = 14;
            SetDamage(3, 12);
			BardLevel = 35;
        }

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Poor );
		}

		public override int Meat{ get{ return 1; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }
		public override PackInstinct PackInstinct{ get{ return PackInstinct.Arachnid; } }
		public override Poison PoisonImmune{ get{ return Poison.Greater; } }
		public override Poison HitPoison{ get{ return (0.8 >= Utility.RandomDouble() ? Poison.Greater : Poison.Deadly); } }

		public Scorpion( Serial serial ) : base( serial )
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