using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "an earth elemental corpse" )]
	public class EarthElemental : BaseCreature
	{
		public override double DispelDifficulty{ get{ return 117.5; } }
		public override double DispelFocus{ get{ return 45.0; } }

		[Constructable]
        public EarthElemental()
            : base( AIType.AI_Melee, FightMode.Closest, 12, 1, 0.65, 0.85 )
        {
            Body = 14;
            Name = "an earth elemental";
            SetStr(130, 150);
            SetHits(100, 150);
            SetDex(66, 85);
            SetStam(66, 85);
            SetInt(71, 92);
            SetMana(71, 95);

            Fame = 3500;
            Karma = -2500;

            BaseSoundID = 268;
            SetSkill(SkillName.Wrestling, 60.1, 100);
            SetSkill(SkillName.Parry, 40.2, 65);
            SetSkill(SkillName.Tactics, 60.1, 100);
            SetSkill(SkillName.MagicResist, 50.1, 75);

            VirtualArmor = 17;
            SetDamage(5, 20);

			BardLevel = 45;
        }

        public override void GenerateLoot()
        {
            int count = Utility.Random(1, 3);

            IronOre ore = new IronOre();
            ore.Amount = count;
            PackItem(ore);

            count = Utility.Random(1, 2);

            FertileDirt dirt = new FertileDirt();
            dirt.Amount = count;
            PackItem(dirt);

            PackGold(150, 200);
        }

        public override bool BleedImmune { get { return true; } }
		public override int TreasureMapLevel{ get{ return 1; } }

		public EarthElemental( Serial serial ) : base( serial )
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