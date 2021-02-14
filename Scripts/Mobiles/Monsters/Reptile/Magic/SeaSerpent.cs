using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a sea serpents corpse" )]
	[TypeAlias( "Server.Mobiles.Seaserpant" )]
	public class SeaSerpent : BaseCreature
	{
		[Constructable]
        public SeaSerpent()
            : base(AIType.AI_Mage, FightMode.Closest, 12, 1, 0.5, 0.75)
        {
            Body = 150;
            Name = "a sea serpent";
            Hue = Utility.RandomBlueHue();

            CantWalk = true;
            CanSwim = true;

            SetStr(168, 225);
            SetHits(168, 225);
            SetDex(58, 85);
            SetStam(58, 85);
            SetInt(53, 95);
            SetMana(53, 95);

            Fame = 6000;
            Karma = -6000;

            BaseSoundID = 446;
            SetSkill(SkillName.Wrestling, 60.1, 70);
            SetSkill(SkillName.Parry, 65.1, 75);
            SetSkill(SkillName.Tactics, 60.1, 70);
            SetSkill(SkillName.MagicResist, 60.1, 75);

            VirtualArmor = 15;
            SetDamage(5, 15);
        }

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Meager );
		}

		public override bool HasBreath{ get{ return true; } }
		public override int TreasureMapLevel{ get{ return 2; } }

		public override int Hides{ get{ return 10; } }
		public override HideType HideType{ get{ return HideType.Horned; } }
		public override int Scales{ get{ return 8; } }
		public override ScaleType ScaleType{ get{ return ScaleType.Blue; } }

		public SeaSerpent( Serial serial ) : base( serial )
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