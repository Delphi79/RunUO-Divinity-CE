using System;
using System.Collections;
using Server;
using Server.Misc;
using Server.Items;
using Server.Gumps;
using Server.Regions;

namespace Server.Mobiles
{
	public abstract class BaseHealer : BaseVendor
	{
		private ArrayList m_SBInfos = new ArrayList();
		protected override ArrayList SBInfos{ get { return m_SBInfos; } }

		public override bool IsActiveVendor{ get{ return false; } }
		
		public override void InitSBInfo()
		{
		}

		public BaseHealer() : base( null )
		{
			//if ( !IsInvulnerable )
			{
				AI = AIType.AI_Mage;
				RangePerception = BaseCreature.DefaultRangePerception;
				FightMode = FightMode.Aggressor;
			}

            Job = JobFragment.healer;

            SetFameLevel(1);
            Karma = 1000;

            BaseSoundID = 332;
            SetSkill(SkillName.Healing, 55, 77.5);
            SetSkill(SkillName.Wrestling, 15, 37.5);
            SetSkill(SkillName.Fencing, 15, 37.5);
            SetSkill(SkillName.Macing, 15, 37.5);
            SetSkill(SkillName.Swords, 15, 37.5);
            SetSkill(SkillName.SpiritSpeak, 55, 77.5);
            SetSkill(SkillName.Parry, 65, 87.5);
            SetSkill(SkillName.Tactics, 65, 87.5);
            SetSkill(SkillName.MagicResist, 65, 87.5);
            SetSkill(SkillName.Anatomy, 55, 77.5);
            SetSkill(SkillName.Forensics, 35, 57.5);


			PackItem( new Bandage( Utility.RandomMinMax( 5, 10 ) ) );
			PackItem( new HealPotion() );
			PackItem( new CurePotion() );
		}

		public override VendorShoeType ShoeType{ get{ return VendorShoeType.Sandals; } }

        public override void InitBody()
        {
            SetStr(71, 85);
            SetDex(81, 95);
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

            item = new Robe();
            AddItem(item);
            item.Hue = Utility.RandomYellowHue();

            item = new Sandals();
            AddItem(item);

            PackGold(15, 100);

            if (!Female)
                Utility.AssignRandomFacialHair(this, hairHue);
		}

		public virtual bool HealsYoungPlayers{ get{ return true; } }

		public virtual bool CheckResurrect( Mobile m )
		{
			return true;
		}

		private DateTime m_NextResurrect;
		private static TimeSpan ResurrectDelay = TimeSpan.FromSeconds( 2.0 );

		public virtual void OfferResurrection( Mobile m )
		{
			Direction = GetDirectionTo( m );
			Say( 501224 ); // Thou hast strayed from the path of virtue, but thou still deservest a second chance.

			m.PlaySound( 0x214 );
			m.FixedEffect( 0x376A, 10, 16 );
            
			//m.CloseGump( typeof( ResurrectGump ) );
			//m.SendGump( new ResurrectGump( m, ResurrectMessage.Healer ) );
			if ( m.NetState != null )
				new ResurrectMenu( m, this, ResurrectMessage.Healer ).SendTo( m.NetState );
		}

		public virtual void OfferHeal( PlayerMobile m )
		{
			Direction = GetDirectionTo( m );

			if ( m.CheckYoungHealTime() )
			{
				Say( 501229 ); // You look like you need some healing my child.

				m.PlaySound( 0x1F2 );
				m.FixedEffect( 0x376A, 9, 32 );

				m.Hits = m.HitsMax;
			}
			else
			{
				Say( 501228 ); // I can do no more for you at this time.
			}
		}

		public override void OnMovement( Mobile m, Point3D oldLocation )
		{
			if ( !m.Frozen && DateTime.Now >= m_NextResurrect && InRange( m, 4 ) && !InRange( oldLocation, 4 ) && InLOS( m ) )
			{
				if ( !m.Alive )
				{
					m_NextResurrect = DateTime.Now + ResurrectDelay;

					if ( m.Map == null || !m.Map.CanFit( m.Location, 16, false, false ) || m.Region.IsPartOf( typeof( HouseRegion ) ) )
					{
						m.SendLocalizedMessage( 502391 ); // Thou can not be resurrected there!
					}
					else if ( CheckResurrect( m ) )
					{
						OfferResurrection( m );
					}
				}
				else if ( this.HealsYoungPlayers && m.Hits < m.HitsMax && m is PlayerMobile && ((PlayerMobile)m).Young )
				{
					OfferHeal( (PlayerMobile) m );
				}
			}
		}

		public BaseHealer( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			//if ( !IsInvulnerable )
			{
				AI = AIType.AI_Mage;
				RangePerception = BaseCreature.DefaultRangePerception;
				FightMode = FightMode.Aggressor;
			}
		}
	}
}