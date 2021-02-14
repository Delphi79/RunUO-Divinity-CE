using System;
using Server;
using Server.Items;
using EDI = Server.Mobiles.EscortDestinationInfo;

namespace Server.Mobiles
{
	public class SeekerOfAdventure : BaseEscortable
	{
		private static string[] m_Dungeons = new string[]
			{
				"Covetous", "Deceit", "Despise",
				"Destard", "Hythloth", "Shame",
				"Wrong"
			};

		public override string[] GetPossibleDestinations()
		{
			return m_Dungeons;
		}

		[Constructable]
		public SeekerOfAdventure()
		{
			Title = "the seeker of adventure";

            SetSkill(SkillName.Swords, 15, 37.5);
            SetSkill(SkillName.Fencing, 15, 37.5);
            SetSkill(SkillName.Macing, 15, 37.5);
            SetSkill(SkillName.Parry, 25, 47.5);
            SetSkill(SkillName.Tactics, 25, 47.5);
            SetSkill(SkillName.MagicResist, 25, 47.5);
            SetSkill(SkillName.Wrestling, 15, 37.5);
		}

		public override bool ClickTitle{ get{ return false; } } // Do not display 'the seeker of adventure' when single-clicking

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

            if (Female)
            {
                item = new FancyDress();
                AddItem(item);
                item.Hue = Utility.RandomNeutralHue();
            }
            else
            {
                Utility.AssignRandomFacialHair(this, hairHue);

                item = new FancyShirt();
                AddItem(item);
                item.Hue = Utility.RandomNondyedHue();

                item = new LongPants();
                AddItem(item);
                item.Hue = Utility.RandomNondyedHue();
            }

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
		}

		public SeekerOfAdventure( Serial serial ) : base( serial )
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