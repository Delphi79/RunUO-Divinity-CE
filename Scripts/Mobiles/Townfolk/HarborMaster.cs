using System;
using Server.Items;
using Server;
using Server.Misc;

namespace Server.Mobiles
{
    public class HarborMaster : BaseConvo
	{
		public override bool CanTeach { get { return false; } }

		[Constructable]
		public HarborMaster()
			: base( AIType.AI_Melee, FightMode.Aggressor, 12, 1, 0.5, 0.75 )
        {
            Title = "the harbor master";

            SetStr(86, 100);
            SetDex(66, 80);
            SetInt(71, 85);
            Hue = Utility.RandomSkinHue();
            SpeechHue = Utility.RandomDyedHue();

            Female = Utility.RandomBool();
            Body = 401;
            Name = NameList.RandomName(Female ? "female" : "male");
            Job = JobFragment.master;

            BaseSoundID = 332;
            SetSkill(SkillName.Swords, 15, 37.5);
            SetSkill(SkillName.Fencing, 15, 37.5);
            SetSkill(SkillName.Macing, 15, 37.5);
            SetSkill(SkillName.Parry, 55, 77.5);
            SetSkill(SkillName.Tactics, 45, 67.5);
            SetSkill(SkillName.MagicResist, 55, 77.5);
            SetSkill(SkillName.Wrestling, 15, 37.5);

            Item item = null;

            int hairHue = Utility.RandomHairHue();
            Utility.AssignRandomHair(this, hairHue);

            Utility.AssignRandomFacialHair(this, hairHue);

            item = new Shirt();
            AddItem(item);
            item.Hue = Utility.RandomNondyedHue();

            item = new ShortPants();
            AddItem(item);
            item.Hue = Utility.RandomNondyedHue();

            item = Utility.RandomBool() ? (Item)new Boots() : (Item)new ThighBoots();
            AddItem(item);

            item = new QuarterStaff();
            AddItem(item);

            PackGold(15, 100);
		}

		public override bool ClickTitle { get { return false; } }


		public HarborMaster( Serial serial )
			: base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 ); // version 
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}
