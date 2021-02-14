using System;
using Server;
using Server.Targeting;
using Server.Mobiles;

namespace Server.Items
{
	public class ItemIdentification
	{
		public static void Initialize()
		{
			SkillInfo.Table[(int)SkillName.ItemID].Callback = new SkillUseCallback( OnUse );
		}

		public static TimeSpan OnUse( Mobile from )
		{
			from.SendLocalizedMessage( 500343 ); // What do you wish to appraise and identify?
			from.Target = new InternalTarget();

			return TimeSpan.FromSeconds( 10.0 );
		}

		[PlayerVendorTarget]
		public class InternalTarget : Target
		{
            private bool m_CheckSkill;
            private TargetCallback m_Callback;

            public InternalTarget(bool checkSkill, TargetCallback callback)
                : base ( 8, false, TargetFlags.None )
            {
                m_CheckSkill = checkSkill;
                m_Callback = callback;

                AllowNonlocal = true;
            }

			public InternalTarget() : this( true, null )
			{
			}

			protected override void OnTarget( Mobile from, object o )
			{
                bool success = false;
				if ( o is Item )
				{
                    success = !m_CheckSkill;
                    if ( !success )
                        success = from.CheckTargetSkill( SkillName.ItemID, o, 0, 100 );
					if ( success )
					{
                        if (o is BaseWeapon)
                            ((BaseWeapon)o).Identified = true;
                        else if (o is BaseArmor)
                            ((BaseArmor)o).Identified = true;
                        else if (o is BaseClothing)
                            ((BaseClothing)o).Identified = true;

						if ( !Core.AOS )
							((Item)o).OnSingleClick( from );

                        if (m_Callback != null)
                            m_Callback(from, o);
					}
					else
					{
						from.SendLocalizedMessage( 500353 ); // You are not certain...
					}
				}
				else if ( o is Mobile )
				{
					((Mobile)o).OnSingleClick( from );
				}
				else
				{
					from.SendLocalizedMessage( 500353 ); // You are not certain...
                }

                //allows the identify skill to reveal attachments
                Server.Engines.XmlSpawner2.XmlAttach.RevealAttachments(from, o);
			}
		}
	}
}