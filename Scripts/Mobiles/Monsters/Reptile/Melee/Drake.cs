using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a drake corpse" )]
	public class Drake : BaseCreature
	{
		[Constructable]
        public Drake()
            : base(AIType.AI_Melee, FightMode.Closest, 12, 1, 0.5, 0.75)
        {
            Body = Utility.RandomList(60, 61);
            Name = "a drake";
            SetStr(201, 230);
            SetHits(501, 900);
            SetDex(133, 152);
            SetStam(43, 62);
            SetInt(101, 140);
            SetMana(86, 205);

            Fame = 5500;
            Karma = -5500;

            Tamable = true;
            MinTameSkill = 100;
            BaseSoundID = 362;
            SetSkill(SkillName.Wrestling, 65.1, 80);
            SetSkill(SkillName.Parry, 65.1, 80);
            SetSkill(SkillName.Tactics, 65.1, 90);
            SetSkill(SkillName.MagicResist, 65.1, 80);

            VirtualArmor = 28;
            SetDamage(4, 24);
			BardLevel = 80;
        }

        public override bool CanBeControlledBy(Mobile m)
        {
            return base.CanBeControlledBy(m) && m.Skills.AnimalLore.Value >= 80.0 && m.Skills.AnimalTaming.Value >= 80.0;
        }

        public override void GenerateLoot()
        {
            AddLoot( LootPack.Rich, 2 );
            PackScroll(3, 6);
            PackScroll(3, 6);
        }

        public override int BreathComputeDamage()
        {
            int damage = base.BreathComputeDamage();

            if ( damage > 40 )
                damage = 40;

            return damage;
        }

		public override bool ReacquireOnMovement{ get{ return true; } }
		public override bool HasBreath{ get{ return true; } } // fire breath enabled
		public override int TreasureMapLevel{ get{ return 2; } }
		public override int Meat{ get{ return 10; } }
		public override int Hides{ get{ return 20; } }
		public override HideType HideType{ get{ return HideType.Horned; } }
		public override int Scales{ get{ return 2; } }
		public override ScaleType ScaleType{ get{ return ( Body == 60 ? ScaleType.Yellow : ScaleType.Red ); } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat | FoodType.Fish; } }

		public Drake( Serial serial ) : base( serial )
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