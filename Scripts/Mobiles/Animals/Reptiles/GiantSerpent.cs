using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "a giant serpent corpse" )]
	[TypeAlias( "Server.Mobiles.Serpant" )]
	public class GiantSerpent : BaseCreature
	{
		[Constructable]
        public GiantSerpent()
            : base(AIType.AI_Melee, FightMode.Aggressor, 12, 1, 0.5, 0.75)
        {
            Body = 21;
            Name = "a giant serpent";
            Hue = Utility.RandomSnakeHue();
            SetStr(186, 215);
            SetHits(186, 215);
            SetDex(56, 80);
            SetStam(56, 75);
            SetInt(66, 85);
            SetMana(66, 90);
            BaseSoundID = 219;
            SetSkill(SkillName.Wrestling, 60.1, 80);
            SetSkill(SkillName.Parry, 45.1, 60);
            SetSkill(SkillName.Tactics, 65.1, 70);
            SetSkill(SkillName.MagicResist, 25.1, 40);

            VirtualArmor = 16;
            SetDamage(5, 19);

            Fame = 2500;
            Karma = -2500;

			BardLevel = 50;
        }

		public override void GenerateLoot()
		{

		}

		public override Poison PoisonImmune{ get{ return Poison.Greater; } }
		public override Poison HitPoison{ get{ return (0.8 >= Utility.RandomDouble() ? Poison.Greater : Poison.Deadly); } }

		public override bool DeathAdderCharmable{ get{ return true; } }

		public override int Meat{ get{ return 4; } }
		public override int Hides{ get{ return 15; } }
		public override HideType HideType{ get{ return HideType.Spined; } }

		public GiantSerpent(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int) 0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			if ( BaseSoundID == -1 )
				BaseSoundID = 219;
		}
	}
}