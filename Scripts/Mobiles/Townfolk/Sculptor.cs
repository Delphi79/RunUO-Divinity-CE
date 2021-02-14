using System;
using Server.Items;
using Server;
using Server.Misc;

namespace Server.Mobiles
{
	public class Sculptor : BaseConvo
	{
		[Constructable]
		public Sculptor()
			: base( AIType.AI_Melee, FightMode.Aggressor, 12, 1, 0.5, 0.75 )
		{
            Female = Utility.RandomBool();
            Body = Female ? 401 : 400;
            SpeechHue = Utility.RandomDyedHue();
            Name = NameList.RandomName(Female ? "female" : "male");
            Hue = Utility.RandomSkinHue();
            SetStr(16, 30);
            SetDex(26, 40);
            SetInt(21, 35);
            Job = JobFragment.sculptor;
            Title = "the sculptor";

            BaseSoundID = 332;
            SetSkill(SkillName.Wrestling, 15, 37.5);
            SetSkill(SkillName.Fencing, 15, 37.5);
            SetSkill(SkillName.Macing, 15, 37.5);
            SetSkill(SkillName.Swords, 15, 37.5);
            SetSkill(SkillName.Parry, 15, 37.5);
            SetSkill(SkillName.Tactics, 15, 37.5);
            SetSkill(SkillName.MagicResist, 15, 37.5);
            SetSkill(SkillName.Anatomy, 25, 47.5);

            Item item = null;

            int hairHue = Utility.RandomHairHue();
            Utility.AssignRandomHair(this, hairHue);

            item = new Shirt();
            AddItem(item);
            item.Hue = Utility.RandomNondyedHue();

            item = new Shoes();
            AddItem(item);
            item.Hue = Utility.RandomNeutralHue();

            item = new HalfApron();
            AddItem(item);
            item.Hue = 2301;

            PackGold(15, 100);

            if (!Female)
            {
                Utility.AssignRandomFacialHair(this, hairHue);

                item = new ShortPants();
                AddItem(item);
                item.Hue = Utility.RandomNondyedHue();
            }
            else
            {
                item = new Skirt();
                AddItem(item);
                item.Hue = Utility.RandomNondyedHue();
            }
        }

		public override bool ClickTitle { get { return false; } }

		public Sculptor( Serial serial )
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
