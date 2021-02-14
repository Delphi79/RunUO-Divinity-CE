using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a dragon corpse" )]
	public class Dragon : BaseCreature
	{
		[Constructable]
        public Dragon()
            : base(AIType.AI_Mage, FightMode.Closest, 12, 1, 0.5, 0.75)
        {
            Body = Utility.RandomList(12, 59);
            Name = "a dragon";
            SetStr(296, 325);
            SetHits(701, 1100);
            SetDex(86, 105);
            SetStam(86, 95);
            SetInt(136, 175);
            SetMana(251, 350);

            Fame = 15000;
            Karma = -15000;

            Tamable = true;
            MinTameSkill = 99;
            BaseSoundID = 362;
            SetSkill(SkillName.Wrestling, 90.1, 92.5);
            SetSkill(SkillName.Parry, 55.1, 95);
            SetSkill(SkillName.Tactics, 97.6, 100);
            SetSkill(SkillName.MagicResist, 99.1, 100);

            VirtualArmor = 30;
            SetDamage(9, 29);
			BardLevel = 85;
        }

        public override bool CanBeControlledBy(Mobile m)
        {
            return base.CanBeControlledBy(m) && m.Skills.AnimalLore.Value >= 80.0 && m.Skills.AnimalTaming.Value >= 80.0;
        }

        public override void GenerateLoot()
        {
			AddLoot( LootPack.FilthyRich, 2 );

			PackGold( 150, 250 );

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


		public override bool ReacquireOnMovement{ get{ return !Controlled; } }
		public override bool HasBreath{ get{ return true; } } // fire breath enabled
		public override bool AutoDispel{ get{ return !Controlled; } }
		public override int TreasureMapLevel{ get{ return 4; } }
		public override int Meat{ get{ return 19; } }
		public override int Hides{ get{ return 20; } }
		public override HideType HideType{ get{ return HideType.Barbed; } }
		public override int Scales{ get{ return 7; } }
		public override ScaleType ScaleType{ get{ return ( Body == 12 ? ScaleType.Yellow : ScaleType.Red ); } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }
		public override bool CanAngerOnTame { get { return true; } }

        public override int BreathComputeDamage()
        {
            int damage = base.BreathComputeDamage();

            if ( damage > 50 )
                damage = 50;

            return damage;
        }

		public Dragon( Serial serial ) : base( serial )
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