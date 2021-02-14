using System;

namespace Server.Items
{
    public class PlayerMaker : Item
    {
        private Mobile m_User;
        private AccessLevel m_AL;

        [Constructable]
        public PlayerMaker()
            : base(0x103B)
        {
            Hue = 1110;
            LootType = LootType.Blessed;
        }

        public PlayerMaker(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (m_User == from && from.AccessLevel == AccessLevel.Player)
            {
                try
                {
                    from.AccessLevel = m_AL;
                    m_User = null;
                    m_AL = AccessLevel.Player;

                    if ( !from.Alive )
                    {
                        from.Hidden = true;
                        from.Resurrect();
                    }
                }
                catch
                {
                    from.SendMessage("An error occurred... Contact your supervisor or something! Don't stand there. I get so tired of people like you, gosh.");
                }
            }
            else if (m_User == null && from.AccessLevel > AccessLevel.Player)
            {
                m_AL = from.AccessLevel;
                m_User = from;
                from.AccessLevel = AccessLevel.Player;

                if ( from.Backpack != null && (!this.IsChildOf(from) || !this.IsAccessibleTo(from)))
                    from.Backpack.DropItem(this);
            }
            else
            {
                from.SendMessage("You can't use this...");
            }
        }

        public override void OnDoubleClickDead(Mobile from)
        {
            OnDoubleClick( from );
        }

        public override void OnDoubleClickCantSee(Mobile from)
        {
            OnDoubleClick(from);
        }

        public override void OnDoubleClickOutOfRange(Mobile from)
        {
            OnDoubleClick(from);
        }

        public override void OnDoubleClickNotAccessible(Mobile from)
        {
            OnDoubleClick(from);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write(m_User != null);
            if (m_User != null)
            {
                writer.Write(m_User);
                writer.Write((int)m_AL);


            }
        }

        public override void OnSingleClick(Mobile from)
        {
            if (from.AccessLevel > AccessLevel.Player)
                this.LabelTo(from, "a player maker");
            else if (from == m_User)
                this.LabelTo(from, "YOUR player maker");
            else
                this.LabelTo(from, "a bread loaf");
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    {
                        if (reader.ReadBool())
                        {
                            m_User = reader.ReadMobile();
                            m_AL = (AccessLevel)reader.ReadInt();
                        }

                        break;
                    }
            }
        }

    }
}