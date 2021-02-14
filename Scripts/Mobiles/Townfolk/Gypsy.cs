using System;
using Server.Items;
using Server;
using Server.Misc;

namespace Server.Mobiles
{
    public class Gypsy : BaseConvo
	{
		[Constructable]
		public Gypsy()
			: base( AIType.AI_Melee, FightMode.Aggressor, 12, 1, 0.5, 0.75 )
		{
            Female = Utility.RandomBool();
            Body = Female ? 401 : 400;
            SpeechHue = Utility.RandomDyedHue();
            Name = NameList.RandomName(Female ? "female" : "male");
            Hue = Utility.RandomSkinHue();
            SetStr(41, 55);
            SetDex(51, 65);
            SetInt(61, 75);
            Job = JobFragment.gypsy;

            Title = "the gypsy";

            BaseSoundID = 332;
            SetSkill(SkillName.Hiding, 45, 67.5);
            SetSkill(SkillName.Wrestling, 15, 37.5);
            SetSkill(SkillName.Fencing, 15, 37.5);
            SetSkill(SkillName.Macing, 15, 37.5);
            SetSkill(SkillName.Swords, 15, 37.5);
            SetSkill(SkillName.Camping, 45, 67.5);
            SetSkill(SkillName.Stealing, 45, 67.5);
            SetSkill(SkillName.Snooping, 45, 67.5);
            SetSkill(SkillName.ItemID, 45, 67.5);
            SetSkill(SkillName.Begging, 45, 67.5);
            SetSkill(SkillName.Parry, 35, 57.5);
            SetSkill(SkillName.Tactics, 35, 57.5);
            SetSkill(SkillName.MagicResist, 35, 57.5);
            SetSkill(SkillName.Lockpicking, 45, 67.5);
        
            Item item = null;

			int hairHue = Utility.RandomHairHue();
			Utility.AssignRandomHair( this, hairHue );

			item = new Shirt();
			AddItem( item );
			item.Hue = Utility.RandomNondyedHue();

			item = new Bandana();
			AddItem( item );
			item.Hue = Utility.RandomBlueHue();

			switch ( Utility.Random( 4 ) )
			{
				case 0: item = new Boots(); break;
				case 1: item = new ThighBoots(); break;
				case 2: item = new Shoes(); break;
				case 3: default: item = new Sandals(); break;
			}
			AddItem( item );

			PackGold( 15, 100 );

			if ( !Female )
			{
				Utility.AssignRandomFacialHair( this, hairHue );

				item = new LongPants();
				AddItem( item );
				item.Hue = Utility.RandomNondyedHue();
			} else {
				item = new Skirt();
				AddItem( item );
				item.Hue = Utility.RandomNondyedHue();
			}
		}

		public override bool CanTeach { get { return true; } }
		public override bool ClickTitle { get { return false; } }

		public Gypsy( Serial serial )
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
