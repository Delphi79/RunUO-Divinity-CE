using System;
using Server;
using Server.Items;
using EDI = Server.Mobiles.EscortDestinationInfo;

namespace Server.Mobiles
{
	public class Noble : BaseEscortable
	{
		[Constructable]
		public Noble()
		{
			Title = "the noble";

            BaseSoundID = 332;
            SetSkill(SkillName.Swords, 15, 37.5);
            SetSkill(SkillName.Fencing, 15, 37.5);
            SetSkill(SkillName.Macing, 15, 37.5);
            SetSkill(SkillName.Parry, 25, 47.5);
            SetSkill(SkillName.Tactics, 25, 47.5);
            SetSkill(SkillName.MagicResist, 25, 47.5);
            SetSkill(SkillName.Wrestling, 15, 37.5);
		}

		public override bool CanTeach{ get{ return true; } }
		public override bool ClickTitle{ get{ return false; } } // Do not display 'the noble' when single-clicking

        public override void InitBody()
        {
            Female = Utility.RandomBool();
            Body = Female ? 401 : 400;
            SpeechHue = Utility.RandomDyedHue();
            Name = NameList.RandomName(Female ? "female" : "male");
            Hue = Utility.RandomSkinHue();
            SetStr(31, 45);
            SetDex(41, 55);
            SetInt(51, 65);
            Job = JobFragment.noble;
        }

		public override void InitOutfit()
		{
            Item item = null;

            int hairHue = Utility.RandomHairHue();
            Utility.AssignRandomHair(this, hairHue);

            item = new Cloak();
            AddItem(item);
            item.Hue = Utility.RandomNondyedHue();

            item = new BodySash();
            AddItem(item);
            item.Hue = Utility.RandomNondyedHue();

            item = Utility.RandomBool() ? (Item)new Boots() : (Item)new ThighBoots();
            AddItem(item);

            item = new Longsword();
            AddItem(item);

            AddLoot(LootPack.FilthyRich);

            if (!Female)
            {
                Utility.AssignRandomFacialHair(this, hairHue);

                item = new FancyShirt();
                AddItem(item);
                item.Hue = Utility.RandomNondyedHue();

                item = new LongPants();
                AddItem(item);
                item.Hue = Utility.RandomNondyedHue();
            }
            else
            {
                item = new FancyDress();
                AddItem(item);
                item.Hue = Utility.RandomNeutralHue();
            }
		}

		public Noble( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}