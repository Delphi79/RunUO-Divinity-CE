using System;
using System.Collections;
using Server.Items;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;

namespace Server.Mobiles
{
	public class Brigand : BaseConvo
	{
		public override bool ClickTitle{ get{ return false; } }

		[Constructable]
        public Brigand()
            : base(AIType.AI_Melee, FightMode.Closest, 12, 1, 0.5, 0.75)
        {
            Female = Utility.RandomBool();
            Body = Female ? 401 : 400;
            SpeechHue = Utility.RandomDyedHue();
            Name = NameList.RandomName(Female ? "female" : "male");
            Hue = Utility.RandomSkinHue();
            SetStr(66, 100);
            SetDex(81, 95);
            SetInt(61, 75);
            Job = JobFragment.brigand;

            Fame = 1000;
            Karma = -1000;

			BardLevel = 55;

            BaseSoundID = 332;
            SetSkill(SkillName.Hiding, 45, 67.5);
            SetSkill(SkillName.Anatomy, 55, 65.1);
            SetSkill(SkillName.Wrestling, 50, 75);
            SetSkill(SkillName.Fencing, 50, 75 );
            SetSkill(SkillName.Macing, 50, 75 );
            SetSkill(SkillName.Swords, 50, 75 );
            SetSkill(SkillName.Camping, 45, 67.5);
            SetSkill(SkillName.Stealing, 45, 67.5);
            SetSkill(SkillName.Snooping, 35, 57.5);
            SetSkill(SkillName.Poisoning, 35, 57.5);
            SetSkill(SkillName.Parry, 55, 77.5);
            SetSkill(SkillName.Tactics, 55, 77.5);
            SetSkill(SkillName.MagicResist, 55, 77.5);
            SetSkill(SkillName.Lockpicking, 35, 57.5);

            int hairHue = Utility.RandomHairHue();
            Utility.AssignRandomHair(this, hairHue);

            Item item = new Shirt();
            AddItem(item);
            item.Hue = Utility.RandomNondyedHue();

            switch (Utility.Random(4))
            {
                case 0: item = new Boots(); break;
                case 1: item = new ThighBoots(); break;
                case 2: item = new Shoes(); break;
                case 3:
                default: item = new Sandals(); break;
            }
            AddItem(item);

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

            item = Loot.RandomWeapon();
            EquipItem(item);
        }

        public override void GenerateLoot()
        {
            PackGold(30, 75);
        }

		public override bool AlwaysMurderer{ get{ return true; } }

		public Brigand( Serial serial ) : base( serial )
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