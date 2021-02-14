using System;
using System.Collections;
using Server;
using Server.Network;

namespace Server.Items
{
	public class BaseShield : BaseArmor
	{
		public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Plate; } }

		public BaseShield( int itemID ) : base( itemID )
		{
		}

		public BaseShield( Serial serial ) : base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 );//version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( version < 1 )
			{
				if ( this is Aegis )
					return;

				// The 15 bonus points to resistances are not applied to shields on OSI.
				PhysicalBonus = 0;
				FireBonus = 0;
				ColdBonus = 0;
				PoisonBonus = 0;
				EnergyBonus = 0;
			}
		}

        public override double BaseArmorRatingScaled
        {
            get
            {
                Mobile m = this.Parent as Mobile;
                double ar = BaseArmorRating;

                if (m != null)
                    return ((m.Skills[SkillName.Parry].Value * ar) / 200.0) + 1.0;
                else
                    return ar;
            }
        }

        public override double ArmorRatingScaled
        {
            get
            {
                Mobile m = this.Parent as Mobile;
                double ar = UnscaledArmorRating;

                if (m != null)
                    return ((m.Skills[SkillName.Parry].Value * ar) / 200.0) + 1.0;
                else
                    return ar;
            }
		}

        public override int OnHit(BaseWeapon weapon, int damage)
        {
            Mobile owner = this.Parent as Mobile;
            
			if (owner == null)
                return damage;

            if ( Utility.Random(4) == 1 )
                owner.CheckSkill( SkillName.Parry, 0.0, 100.0 );

            bool archery = weapon.Skill == SkillName.Archery;
            double ar = this.ArmorRatingScaled;

			damage = (int)( archery ? (damage*0.35) : (damage*0.85) );

			if (damage < 0)
				damage = 0;

			owner.FixedEffect(0x37B9, 10, 16);

			if (MaxHitPoints > 0 && Utility.Random(100) < 25) // 25% chance to lower durability
			{
				int wear = 1;

				if (weapon.Type == WeaponType.Bashing)
					wear = 3;

				if (wear > 0)
				{
					if (HitPoints > wear && MaxHitPoints > 1)
					{
						HitPoints -= wear;

						if (HitPoints < 10 || ((float)HitPoints / MaxHitPoints) <= 0.1)
						{
							--MaxHitPoints;

							if (Parent is Mobile)
								((Mobile)Parent).LocalOverheadMessage(MessageType.Regular, 0x3B2, 1061121); // Your equipment is severely damaged.
						}
					}
					else
					{
						if (Parent is Mobile)
							((Mobile)Parent).LocalOverheadMessage(MessageType.Regular, 0x3B2, true, "Your equipment has been destroyed!");
						Delete();
					}
				}
			}

/*
            if (Utility.Random(archery ? 300 : 200) < (owner.Skills[SkillName.Parry].Value + 5 - this.BaseArmorRating) )
            {
                damage -= (int)(archery ? ar : ar / 2.0);

                if (damage < 0)
                    damage = 0;

                owner.FixedEffect(0x37B9, 10, 16);

                if (MaxHitPoints > 0 && Utility.Random(100) < 25) // 25% chance to lower durability
                {
                    int wear = 1;

                    if (weapon.Type == WeaponType.Bashing)
                        wear = 3;

                    if (wear > 0)
                    {
                        if (HitPoints > wear && MaxHitPoints > 1)
                        {
                            HitPoints -= wear;

                            if (HitPoints < 10 || ((float)HitPoints / MaxHitPoints) <= 0.1)
                            {
                                --MaxHitPoints;

                                if (Parent is Mobile)
                                    ((Mobile)Parent).LocalOverheadMessage(MessageType.Regular, 0x3B2, 1061121); // Your equipment is severely damaged.
                            }
                        }
                        else
                        {
                            if (Parent is Mobile)
                                ((Mobile)Parent).LocalOverheadMessage(MessageType.Regular, 0x3B2, true, "Your equipment has been destroyed!");
                            Delete();
                        }
                    }
                }
            }
*/
            return damage;
        }
	}
}
