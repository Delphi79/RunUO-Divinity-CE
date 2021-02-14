using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	public class WanderingHealer : BaseHealer
	{
		public override bool CanTeach{ get{ return true; } }

		public override bool CheckTeach( SkillName skill, Mobile from )
		{
			if ( !base.CheckTeach( skill, from ) )
				return false;

			return ( skill == SkillName.Anatomy )
				|| ( skill == SkillName.Camping )
				|| ( skill == SkillName.Forensics )
				|| ( skill == SkillName.Healing )
				|| ( skill == SkillName.SpiritSpeak );
		}

		[Constructable]
		public WanderingHealer()
		{
			Title = "the wandering healer";

            Female = Utility.RandomBool();
            Body = Female ? 401 : 400;
            SpeechHue = Utility.RandomDyedHue();
            Name = NameList.RandomName(Female ? "female" : "male");
            Hue = Utility.RandomSkinHue();
            SetStr(71, 85);
            SetDex(81, 95);
            SetInt(86, 100);
            Job = JobFragment.healer;

            BaseSoundID = 332;
            SetSkill(SkillName.Fishing, 35, 57.5);
            SetSkill(SkillName.Healing, 55, 77.5);
            SetSkill(SkillName.Wrestling, 15, 37.5);
            SetSkill(SkillName.Fencing, 15, 37.5);
            SetSkill(SkillName.Macing, 15, 37.5);
            SetSkill(SkillName.Swords, 15, 37.5);
            SetSkill(SkillName.Camping, 35, 57.5);
            SetSkill(SkillName.SpiritSpeak, 55, 77.5);
            SetSkill(SkillName.Parry, 65, 87.5);
            SetSkill(SkillName.Tactics, 65, 87.5);
            SetSkill(SkillName.MagicResist, 65, 87.5);
            SetSkill(SkillName.Anatomy, 55, 77.5);
            SetSkill(SkillName.Forensics, 35, 57.5);
		}

		public override bool ClickTitle{ get{ return false; } } // Do not display title in OnSingleClick

		public override bool CheckResurrect( Mobile m )
		{
			if ( m.Criminal )
			{
				Say( 501222 ); // Thou art a criminal.  I shall not resurrect thee.
				return false;
			}
			else if ( m.Kills >= 5 )
			{
				Say( 501223 ); // Thou'rt not a decent and good person. I shall not resurrect thee.
				return false;
			}

			return true;
		}

		public WanderingHealer( Serial serial ) : base( serial )
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