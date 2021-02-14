using System;
using Server;
using Server.Items;
using EDI=Server.Mobiles.EscortDestinationInfo;

namespace Server.Mobiles
{
	public class Merchant : BaseEscortable
	{
		[Constructable]
		public Merchant()
		{
            Job = JobFragment.shopkeep;

			Title = "the merchant";

            SetSkill(SkillName.Wrestling, 15, 37.5);
            SetSkill(SkillName.Fencing, 15, 37.5);
            SetSkill(SkillName.Macing, 15, 37.5);
            SetSkill(SkillName.Swords, 15, 37.5);
            SetSkill(SkillName.ItemID, 55, 77.5);
            SetSkill(SkillName.Parry, 27.5, 50);
            SetSkill(SkillName.Tactics, 27.5, 50);
            SetSkill(SkillName.MagicResist, 27.5, 50);
		}

		public override bool CanTeach { get { return true; } }
		public override bool ClickTitle { get { return false; } } // Do not display 'the merchant' when single-clicking

        public override void InitBody()
        {
            SetStr(71, 85);
            SetDex(86, 100);
            SetInt(86, 100);
            Hue = Utility.RandomSkinHue();
            SpeechHue = Utility.RandomDyedHue();

            Female = Utility.RandomBool();
            Body = 401;
            Name = NameList.RandomName(Female ? "female" : "male");
        }

		public override void InitOutfit()
		{
            Item item = null;

            int hairHue = Utility.RandomHairHue();
            Utility.AssignRandomHair(this, hairHue);

            item = new FancyShirt();
            AddItem(item);
            item.Hue = Utility.RandomNondyedHue();

            item = new Shoes();
            AddItem(item);
            item.Hue = Utility.RandomNeutralHue();

            PackGold(15, 100);

            if (!Female)
            {
                Utility.AssignRandomFacialHair(this, hairHue);

                item = new LongPants();
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

		public Merchant( Serial serial )
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