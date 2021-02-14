using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.ContextMenus;

namespace Server.Items
{
	public class Ankhs
	{
		public const int ResurrectRange = 2;
		public const int TitheRange = 2;
		public const int LockRange = 2;

		public static void GetContextMenuEntries( Mobile from, Item item, List<ContextMenuEntry> list )
		{
			if ( from is PlayerMobile )
				list.Add( new LockKarmaEntry( (PlayerMobile)from ) );

			list.Add( new ResurrectEntry( from, item ) );

			if ( Core.AOS )
				list.Add( new TitheEntry( from ) );
		}

		public static void Resurrect( Mobile m, Item item, bool chaos )
		{
			if ( m == null || item == null || m.Map == null || m.Alive )
				return;

            if ( !chaos && m.Kills >= 5 )
            {
                m.SendMessage( "Thy deeds are those of a scoundrel; thou shalt not be resurrected here." );
                return;
            }

            Point3D loc = item.GetWorldLocation();

			if ( !m.InRange( loc, ResurrectRange ) || !m.Map.LineOfSight( m, loc ) )
				m.SendLocalizedMessage( 500446 ); // That is too far away.
			else if( m.Map.CanFit( m.Location, 16, false, false ) )
			{
				//m.CloseGump( typeof( ResurrectGump ) );
				//m.SendGump( new ResurrectGump( m, ResurrectMessage.VirtueShrine ) );
				if ( m.NetState != null )
					new ResurrectMenu(m, chaos ? ResurrectMessage.ChaosShrine : ResurrectMessage.VirtueShrine).SendTo(m.NetState);
			}
			else
				m.SendLocalizedMessage( 502391 ); // Thou can not be resurrected there!
		}

		private class ResurrectEntry : ContextMenuEntry
		{
			private Mobile m_Mobile;
			private Item m_Item;

			public ResurrectEntry( Mobile mobile, Item item ) : base( 6195, ResurrectRange )
			{
				m_Mobile = mobile;
				m_Item = item;

				Enabled = !m_Mobile.Alive;
			}

			public override void OnClick()
			{
				Resurrect( m_Mobile, m_Item, false );
			}
		}

		private class LockKarmaEntry : ContextMenuEntry
		{
			private PlayerMobile m_Mobile;

			public LockKarmaEntry( PlayerMobile mobile ) : base( mobile.KarmaLocked ? 6197 : 6196, LockRange )
			{
				m_Mobile = mobile;
			}

			public override void OnClick()
			{
				m_Mobile.KarmaLocked = !m_Mobile.KarmaLocked;

				if ( m_Mobile.KarmaLocked )
					m_Mobile.SendLocalizedMessage( 1060192 ); // Your karma has been locked. Your karma can no longer be raised.
				else
					m_Mobile.SendLocalizedMessage( 1060191 ); // Your karma has been unlocked. Your karma can be raised again.
			}
		}

		private class TitheEntry : ContextMenuEntry
		{
			private Mobile m_Mobile;

			public TitheEntry( Mobile mobile ) : base( 6198, TitheRange )
			{
				m_Mobile = mobile;

				Enabled = m_Mobile.Alive;
			}

			public override void OnClick()
			{
				if ( m_Mobile.CheckAlive() )
					m_Mobile.SendGump( new TithingGump( m_Mobile, 0 ) );
			}
		}

        public static void OnDoubleClick(Mobile from, bool chaos)
        {
            bool good = (from.Karma >= 0 && from.Kills < 5);
            bool heal = false;

            if (chaos)
            {
                if (!good)
                {
                    from.SendMessage("Thy efforts for the resistance are rewarded.");
                    heal = true;
                }
            }
            else
            {
                if (good)
                {
                    from.SendMessage("Strive to continue on the path of benevolence.");
                    heal = true;
                }
            }

            if (heal)
            {
                //from.Hits = from.HitsMax;

                //from.FixedParticles(0x376A, 9, 32, 5030, EffectLayer.Waist);
                //from.PlaySound(0x202);

                if (from.Poison != null)
                {
                    if (from.CurePoison(from))
                    {
                        from.SendLocalizedMessage(1010059); // You have been cured of all poisons.
                        from.FixedParticles(0x373A, 10, 15, 5012, EffectLayer.Waist);
                        from.PlaySound(0x1E0);
                    }
                }
            }
            else
            {
                if (chaos)
                    from.SendMessage("The weak deserve their fate.");
                else
                    from.SendMessage("Do more to help others.");

                if (from.Hits > 1)
                {
                    from.Hits /= 2;

                    from.FixedParticles(0x374A, 10, 15, 5013, EffectLayer.Waist);
                    from.PlaySound(0x1F1);
                }
            }
        }
	}

	public class AnkhWest : Item
	{
		private InternalItem m_Item;


        private bool m_Chaos;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Chaos { get { return m_Chaos; } set { m_Chaos = value; } }

		[Constructable]
		public AnkhWest() : this( false )
		{
		}

		[Constructable]
		public AnkhWest( bool bloodied ) : base( bloodied ? 0x1D98 : 0x3 )
		{
			Movable = false;

			m_Item = new InternalItem( bloodied, this );
		}

		public AnkhWest( Serial serial ) : base( serial )
		{
		}

		public override bool HandlesOnMovement{ get{ return true; } } // Tell the core that we implement OnMovement

		public override void OnMovement( Mobile m, Point3D oldLocation )
		{
			if ( Parent == null && Utility.InRange( Location, m.Location, Ankhs.ResurrectRange ) && !Utility.InRange( Location, oldLocation, Ankhs.ResurrectRange ) )
				Ankhs.Resurrect( m, this, m_Chaos );
		}

		public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
		{
			base.GetContextMenuEntries( from, list );
			Ankhs.GetContextMenuEntries( from, this, list );
		}

		[Hue, CommandProperty( AccessLevel.GameMaster )]
		public override int Hue
		{
			get{ return base.Hue; }
			set{ base.Hue = value; if ( m_Item.Hue != value ) m_Item.Hue = value; }
		}

		public override void OnDoubleClickDead( Mobile m )
		{
			Ankhs.Resurrect( m, this, m_Chaos );
		}

		public override void OnLocationChange( Point3D oldLocation )
		{
			if ( m_Item != null )
				m_Item.Location = new Point3D( X, Y + 1, Z );
		}

		public override void OnMapChange()
		{
			if ( m_Item != null )
				m_Item.Map = Map;
		}

		public override void OnAfterDelete()
		{
			base.OnAfterDelete();

			if ( m_Item != null )
				m_Item.Delete();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version

            writer.Write(m_Chaos);
			writer.Write( m_Item );
		}

        public override void OnDoubleClick(Mobile from)
        {
            Ankhs.OnDoubleClick(from, m_Chaos);
        }

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    m_Chaos = reader.ReadBool();
                    break;
            }

			m_Item = reader.ReadItem() as InternalItem;
		}

		private class InternalItem : Item
		{
			private AnkhWest m_Item;

			public InternalItem( bool bloodied, AnkhWest item ) : base( bloodied ? 0x1D97 : 0x2 )
			{
				Movable = false;

				m_Item = item;
			}

			public InternalItem( Serial serial ) : base( serial )
			{
			}

			public override void OnLocationChange( Point3D oldLocation )
			{
				if ( m_Item != null )
					m_Item.Location = new Point3D( X, Y - 1, Z );
			}

			public override void OnMapChange()
			{
				if ( m_Item != null )
					m_Item.Map = Map;
			}

			public override void OnAfterDelete()
			{
				base.OnAfterDelete();

				if ( m_Item != null )
					m_Item.Delete();
			}

            public override void OnDoubleClick(Mobile from)
            {
                Ankhs.OnDoubleClick(from, m_Item != null ? m_Item.Chaos : false);
            }

            /*public override bool HandlesOnMovement{ get{ return true; } } // Tell the core that we implement OnMovement

            public override void OnMovement( Mobile m, Point3D oldLocation )
            {
                if ( Parent == null && Utility.InRange( Location, m.Location, Ankhs.ResurrectRange ) && !Utility.InRange( Location, oldLocation, Ankhs.ResurrectRange ) )
                    Ankhs.Resurrect(m, this, m_Item != null ? m_Item.Chaos : false);
            }*/

            public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
			{
				base.GetContextMenuEntries( from, list );
				Ankhs.GetContextMenuEntries( from, this, list );
			}

			[Hue, CommandProperty( AccessLevel.GameMaster )]
			public override int Hue
			{
				get{ return base.Hue; }
				set{ base.Hue = value; if ( m_Item.Hue != value ) m_Item.Hue = value; }
			}

			public override void OnDoubleClickDead( Mobile m )
			{
                Ankhs.Resurrect(m, this, m_Item != null ? m_Item.Chaos : false);
			}

			public override void Serialize( GenericWriter writer )
			{
				base.Serialize( writer );

				writer.Write( (int) 0 ); // version

				writer.Write( m_Item );
			}

			public override void Deserialize( GenericReader reader )
			{
				base.Deserialize( reader );

				int version = reader.ReadInt();

				m_Item = reader.ReadItem() as AnkhWest;
			}
		}
	}

	[TypeAlias( "Server.Items.AnkhEast" )]
	public class AnkhNorth : Item
	{
		private InternalItem m_Item;

        private bool m_Chaos;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Chaos { get { return m_Chaos; } set { m_Chaos = value; } }

		[Constructable]
		public AnkhNorth() : this( false )
		{
		}

		[Constructable]
		public AnkhNorth( bool bloodied ) : base( bloodied ? 0x1E5D : 0x4 )
		{
			Movable = false;

			m_Item = new InternalItem( bloodied, this );
		}

		public AnkhNorth( Serial serial )
			: base( serial )
		{
		}

		public override bool HandlesOnMovement{ get{ return true; } } // Tell the core that we implement OnMovement

		public override void OnMovement( Mobile m, Point3D oldLocation )
		{
			if ( Parent == null && Utility.InRange( Location, m.Location, Ankhs.ResurrectRange ) && !Utility.InRange( Location, oldLocation, Ankhs.ResurrectRange ) )
				Ankhs.Resurrect( m, this, m_Chaos );
		}

		public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
		{
			base.GetContextMenuEntries( from, list );
			Ankhs.GetContextMenuEntries( from, this, list );
		}

		[Hue, CommandProperty( AccessLevel.GameMaster )]
		public override int Hue
		{
			get{ return base.Hue; }
			set{ base.Hue = value; if ( m_Item.Hue != value ) m_Item.Hue = value; }
		}

		public override void OnDoubleClickDead( Mobile m )
		{
			Ankhs.Resurrect( m, this, m_Chaos );
		}

        public override void OnDoubleClick(Mobile from)
        {
            Ankhs.OnDoubleClick(from, m_Chaos);
        }

		public override void OnLocationChange( Point3D oldLocation )
		{
			if ( m_Item != null )
				m_Item.Location = new Point3D( X + 1, Y, Z );
		}

		public override void OnMapChange()
		{
			if ( m_Item != null )
				m_Item.Map = Map;
		}

		public override void OnAfterDelete()
		{
			base.OnAfterDelete();

			if ( m_Item != null )
				m_Item.Delete();
		}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version

            writer.Write(m_Chaos);
            writer.Write(m_Item);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    m_Chaos = reader.ReadBool();
                    break;
            }

            m_Item = reader.ReadItem() as InternalItem;
        }

		[TypeAlias( "Server.Items.AnkhEast+InternalItem" )]
		private class InternalItem : Item
		{
			private AnkhNorth m_Item;

			public InternalItem( bool bloodied, AnkhNorth item )
				: base( bloodied ? 0x1E5C : 0x5 )
			{
				Movable = false;

				m_Item = item;
			}

			public InternalItem( Serial serial ) : base( serial )
			{
			}

			public override void OnLocationChange( Point3D oldLocation )
			{
				if ( m_Item != null )
					m_Item.Location = new Point3D( X - 1, Y, Z );
			}

			public override void OnMapChange()
			{
				if ( m_Item != null )
					m_Item.Map = Map;
			}

			public override void OnAfterDelete()
			{
				base.OnAfterDelete();

				if ( m_Item != null )
					m_Item.Delete();
			}

            /*public override bool HandlesOnMovement{ get{ return true; } } // Tell the core that we implement OnMovement

            public override void OnMovement( Mobile m, Point3D oldLocation )
            {
                if ( Parent == null && Utility.InRange( Location, m.Location, Ankhs.ResurrectRange ) && !Utility.InRange( Location, oldLocation, Ankhs.ResurrectRange ) )
                    Ankhs.Resurrect(m, this, m_Item != null ? m_Item.Chaos : false);
            }*/

            public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
			{
				base.GetContextMenuEntries( from, list );
				Ankhs.GetContextMenuEntries( from, this, list );
			}

			[Hue, CommandProperty( AccessLevel.GameMaster )]
			public override int Hue
			{
				get{ return base.Hue; }
				set{ base.Hue = value; if ( m_Item.Hue != value ) m_Item.Hue = value; }
			}

			public override void OnDoubleClickDead( Mobile m )
			{
				Ankhs.Resurrect( m, this, m_Item != null ? m_Item.Chaos : false );
			}

            public override void OnDoubleClick(Mobile from)
            {
                Ankhs.OnDoubleClick(from, m_Item != null ? m_Item.Chaos : false);
            }

			public override void Serialize( GenericWriter writer )
			{
				base.Serialize( writer );

				writer.Write( (int) 0 ); // version

				writer.Write( m_Item );
			}

			public override void Deserialize( GenericReader reader )
			{
				base.Deserialize( reader );

				int version = reader.ReadInt();

				m_Item = reader.ReadItem() as AnkhNorth;
			}
		}
	}
}
