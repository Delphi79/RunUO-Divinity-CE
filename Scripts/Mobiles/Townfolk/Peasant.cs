using System;
using Server;
using Server.Items;
using EDI = Server.Mobiles.EscortDestinationInfo;

namespace Server.Mobiles
{
	public class Peasant : BaseEscortable
	{
		[Constructable]
		public Peasant()
		{
			Title = "the peasant";

            BaseSoundID = 332;
            SetSkill(SkillName.Swords, 15, 37.5);
            SetSkill(SkillName.Fencing, 15, 37.5);
            SetSkill(SkillName.Macing, 15, 37.5);
            SetSkill(SkillName.Parry, 15, 37.5);
            SetSkill(SkillName.Tactics, 15, 37.5);
            SetSkill(SkillName.MagicResist, 15, 37.5);
            SetSkill(SkillName.Wrestling, 15, 37.5);

		}

		public override bool CanTeach{ get{ return true; } }
		public override bool ClickTitle{ get{ return false; } } // Do not display 'the peasant' when single-clicking

        public override void InitBody()
        {
            Female = Utility.RandomBool();
            Body = Female ? 401 : 400;
            SpeechHue = Utility.RandomDyedHue();
            Name = NameList.RandomName(Female ? "female" : "male");
            Hue = Utility.RandomSkinHue();
            SetStr(26, 40);
            SetDex(21, 35);
            SetInt(16, 30);
            Job = JobFragment.laborer;
        }

		public override void InitOutfit()
		{
            Item item = null;

            int hairHue = Utility.RandomHairHue();
            Utility.AssignRandomHair(this, hairHue);

            item = new Shirt();
            AddItem(item);
            item.Hue = Utility.RandomNondyedHue();

            AddLoot(LootPack.Poor);

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

		public Peasant( Serial serial ) : base( serial )
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