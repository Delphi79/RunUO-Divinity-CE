using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a skeletal corpse" )]
	public class BoneKnight : BaseCreature
	{
		[Constructable]
        public BoneKnight()
            : base(AIType.AI_Melee, FightMode.Closest, 12, 1, 0.5, 0.75)
        {
            Body = 57;
            Name = "a bone knight";
            SetStr(125, 125);
            SetHits(96, 200);
            SetDex(76, 95);
            SetStam(76, 95);
            SetInt(36, 60);
            SetMana(0);

            Fame = 3000;
            Karma = -3000;

            BaseSoundID = 451;
            SetSkill(SkillName.Wrestling, 85.1, 95);
            SetSkill(SkillName.Parry, 85.1, 95);
            SetSkill(SkillName.Tactics, 85.1, 100);
            SetSkill(SkillName.MagicResist, 65.1, 90);

            VirtualArmor = 18;
            SetDamage(18, 20);
			BardLevel = 65;
        }

        public override void GenerateLoot()
        {
            Item item = null;

            item = new PlateChest();
            PackItem(item);

            AddLoot(LootPack.Average);
        }

		public override bool BleedImmune{ get{ return true; } }

		public BoneKnight( Serial serial ) : base( serial )
		{
		}

		public override OppositionGroup OppositionGroup
		{
			get{ return OppositionGroup.FeyAndUndead; }
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