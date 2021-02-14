using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a dragon corpse" )]
	public class AncientWyrm : BaseCreature
	{
		[Constructable]
        public AncientWyrm() : base(AIType.AI_Mage, FightMode.Closest, 12, 1, 0.3, 0.5)
        {
            Body = Utility.RandomList(12, 59);
            Name = "an ancient wyrm";
            SetStr(700, 800);
            SetHits(701, 1100);
            SetDex(86, 175);
            SetStam(86, 175);
            SetInt(436, 525);
            SetMana(251, 550);

            if ( Utility.RandomBool() )
                Hue = Utility.RandomMinMax(1105, 1110);
            else
                Hue = Utility.RandomMinMax(34, 39);

            Fame = 22500;
            Karma = -22500;

            BaseSoundID = 362;
            SetSkill(SkillName.Wrestling, 90.1, 92.5);
            SetSkill(SkillName.Parry, 55.1, 95);
            SetSkill(SkillName.Tactics, 97.6, 100);
            SetSkill(SkillName.MagicResist, 99.1, 100);
            SetSkill(SkillName.Magery, 99.1, 100);
            SetSkill(SkillName.EvalInt, 99.1, 100);

            VirtualArmor = 30;
            SetDamage(11, 41);

			//BardImmune = true;
			BardLevel = 97.5;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.UltraRich, 3 );

            if (Utility.Random(2) < 1)
            {
                AddLoot(LootPack.Gems);
            }

            if (Utility.Random(2) < 1)
            {
                AddLoot(LootPack.Gems);
            }

            if (Utility.Random(2) < 1)
            {
                AddLoot(LootPack.Gems);
            }

            if (Utility.Random(2) < 1)
            {
                AddLoot(LootPack.Gems);
            }

            if (Utility.Random(2) < 1)
            {
                AddLoot(LootPack.Gems);
            }

            if (Utility.Random(2) < 1)
            {
                AddLoot(LootPack.Gems);
            }
        }

		public override bool ReacquireOnMovement{ get{ return true; } }
		public override bool HasBreath{ get{ return true; } } // fire breath enabled
		public override bool AutoDispel{ get{ return true; } }
		public override HideType HideType{ get{ return HideType.Barbed; } }
		public override int Hides{ get{ return 40; } }
		public override int Meat{ get{ return 19; } }
		public override int Scales{ get{ return 12; } }
		public override ScaleType ScaleType{ get{ return (ScaleType)Utility.Random( 4 ); } }
		public override Poison PoisonImmune{ get{ return Poison.Regular; } }
		public override Poison HitPoison{ get{ return Utility.RandomBool() ? Poison.Lesser : Poison.Regular; } }
		public override int TreasureMapLevel{ get{ return 5; } }

		public AncientWyrm( Serial serial ) : base( serial )
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