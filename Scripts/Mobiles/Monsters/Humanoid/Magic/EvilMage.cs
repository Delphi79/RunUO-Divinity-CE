using System;
using Server;
using Server.Misc;
using Server.Items;

namespace Server.Mobiles 
{ 
	[CorpseName( "an evil mage corpse" )] 
	public class EvilMage : BaseCreature 
	{ 
		[Constructable]
        public EvilMage()
            : base(AIType.AI_Mage, FightMode.Closest, 12, 1, 0.5, 0.75)
        {
            Female = Utility.RandomBool();
            Body = Female ? 401 : 400;
            SpeechHue = Utility.RandomDyedHue();
            Name = NameList.RandomName(Female ? "female" : "male");
            Hue = Utility.RandomSkinHue();
            SetStr(81, 155);
            SetDex(91, 115);
            SetInt(96, 220);

            Fame = 5000;
            Karma = -5000;

            SetSkill(SkillName.Wrestling, 20.2, 60);
            SetSkill(SkillName.EvalInt, 55, 77.5);
            SetSkill(SkillName.Parry, 65, 87.5);
            SetSkill(SkillName.Tactics, 65, 87.5);
            SetSkill(SkillName.MagicResist, 75, 97.5);
            SetSkill(SkillName.Magery, 95, 100);
            SetSkill(SkillName.Inscribe, 75.1, 90);

            VirtualArmor = 8;
            SetDamage(3, 12);

            int hairHue = Utility.RandomHairHue();
            Utility.AssignRandomHair(this, hairHue);

            Utility.AssignRandomFacialHair(this, hairHue);

            Item item = new Robe();
            AddItem(item);
            item.Hue = Utility.RandomRedHue();

            item = new Sandals();
            AddItem(item);

			//BardImmune = true;
			BardLevel = 85.5;
        }

        public override void GenerateLoot()
        {
            PackGold(50, 60);

            PackScroll(4, 8);
            PackScroll(4, 8);
        }

		public override bool CanRummageCorpses{ get{ return true; } }
		public override bool AlwaysMurderer{ get{ return true; } }
		public override int Meat{ get{ return 1; } }
		public override int TreasureMapLevel{ get{ return Core.AOS ? 1 : 0; } }

		public EvilMage( Serial serial ) : base( serial )
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