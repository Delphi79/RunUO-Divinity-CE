using System;
using System.Reflection;
using Server;
using System.Collections.Generic;

namespace Server.Items
{
    public class CollectionBarrel : Barrel
    {
        private int m_Needed, m_Collected;
        private string m_TypeStr;
        private Type m_Type;

        private Dictionary<Mobile, int> m_Record = new Dictionary<Mobile,int>();

        [CommandProperty(AccessLevel.GameMaster)]
        public string CollectionType
        {
            get
            {
                return m_TypeStr;
            }
            set
            {
                m_TypeStr = value;
                m_Type = null;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Needed
        {
            get
            {
                return m_Needed;
            }
            set
            {
                m_Needed = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Collected
        {
            get
            {
                return m_Collected;
            }
            set
            {
                m_Collected = value;
            }
        }

	public void GetTops(out Mobile f, out Mobile s, out Mobile t, out int fc, out int sc, out int tc)
        {
            f = s = t = null;
            fc = sc = tc = 0;

            foreach (KeyValuePair<Mobile, int> kvp in m_Record)
            {
                if (kvp.Value >= fc)
                {
                    tc = sc;
                    t = s;

                    sc = fc;
                    s = f;

                    fc = kvp.Value;
                    f = kvp.Key;
                }
		else if (kvp.Value >= sc)
                {
                    tc = sc;
                    t = s;
                    
                    sc = kvp.Value;
                    s = kvp.Key;
                }
		else if (kvp.Value >= tc)
                {
                    tc = kvp.Value;
                    t = kvp.Key;
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile FirstDonator
        {
            get
            {
                Mobile f, s, t;
                int fc, sc, tc;

                GetTops( out f, out s, out t, out fc, out sc, out tc );

                return f;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile SecondtDonator
        {
            get
            {
                Mobile f, s, t;
                int fc, sc, tc;

                GetTops(out f, out s, out t, out fc, out sc, out tc);

                return s;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile ThirdDonator
        {
            get
            {
                Mobile f, s, t;
                int fc, sc, tc;

                GetTops(out f, out s, out t, out fc, out sc, out tc);

                return t;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int FirstAmount
        {
            get
            {
                Mobile f, s, t;
                int fc, sc, tc;

                GetTops(out f, out s, out t, out fc, out sc, out tc);

                return fc;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SecondAmount
        {
            get
            {
                Mobile f, s, t;
                int fc, sc, tc;

                GetTops(out f, out s, out t, out fc, out sc, out tc);

                return sc;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int ThirdAmount
        {
            get
            {
                Mobile f, s, t;
                int fc, sc, tc;

                GetTops(out f, out s, out t, out fc, out sc, out tc);

                return tc;
            }
        }

        [Constructable]
        public CollectionBarrel(string type, int needed) : base()
        {
            Name = "a collection barrel";
            Movable = false;

            m_TypeStr = type;
            m_Needed = needed;
            m_Collected = 0;
        }

        public CollectionBarrel(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version

            writer.Write((int)m_Record.Count);
            foreach ( KeyValuePair<Mobile,int> kvp in m_Record )
            {
                writer.Write( kvp.Key );
                writer.Write( kvp.Value );
            }

            writer.Write(m_TypeStr);
            writer.Write(m_Collected);
            writer.Write(m_Needed);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    {
                        int count = reader.ReadInt();

                        for (int i = 0; i < count; i++)
                        {
                            Mobile m = reader.ReadMobile();
                            int total = reader.ReadInt();

				if ( m != null )
					m_Record.Add(m, total);
                        }
                        goto case 0;
                    }
                case 0:
                    {
                        m_TypeStr = reader.ReadString();
                        m_Collected = reader.ReadInt();
                        m_Needed = reader.ReadInt();
                        break;
                    }
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            OnSingleClick( from );
        }

        public override void OnSingleClick(Mobile from)
        {
            //LabelTo( from, "{0} collection barrel", m_TypeStr != null ? m_TypeStr.ToString() : "" );
            if ( this.Name == null )
                this.Name = "a collection barrel";
            LabelTo( from, this.Name );
            LabelTo( from, "{0} of {1} collected so far", Collected, Needed );
        }

        public override bool TryDropItem(Mobile from, Item dropped, bool sendFullMessage)
        {
            return HandleDrop(from, dropped);
        }

        public override bool OnDragDropInto(Mobile from, Item item, Point3D p)
        {
            return HandleDrop(from, item);
        }

        public virtual bool CheckItem( Item dropped )
        {
            return dropped.GetType().IsSubclassOf(m_Type) || m_Type.IsAssignableFrom(dropped.GetType());
        }

        public virtual bool HandleDrop( Mobile from, Item dropped )
        {
            if (m_Collected >= m_Needed)
            {
                from.SendAsciiMessage("We have received all that we need of that already!  Thanks anyways!");
                return false;
            }

            if (m_Type == null)
            {
                if (CollectionType == null)
                {
                    from.SendAsciiMessage("That does not belong in this collection barrel!");
                    return false;
                }

                try
                {
                    m_Type = ScriptCompiler.FindTypeByName(CollectionType, true);
                }
                catch
                {
                }

                if (m_Type == null)
                {
                    from.SendAsciiMessage("That does not belong in this collection barrel!");
                    return false;
                }
            }

            if (CheckItem(dropped))
            {
                from.SendAsciiMessage("Thank you for your donation to our cause!");
                from.SendSound(GetDroppedSound(dropped), GetWorldLocation());

                m_Collected += dropped.Amount;

                if ( m_Record.ContainsKey( from ) )
                {
                    m_Record[from] += dropped.Amount;
                }
                else
                {
                    m_Record.Add( from, dropped.Amount );
                }

                dropped.Delete();

                return true;
            }
            else
            {
                from.SendAsciiMessage("That does not belong in this collection barrel!");
                return false;
            }
        }
    }

    public class MagicCollectionBarrel : CollectionBarrel
    {
        private int m_MinLevel = 1;

        public int MinLevel
        {
            get { return m_MinLevel; }
            set { m_MinLevel = value; }
        }

        [Constructable]
        public MagicCollectionBarrel(string type, int needed)
            : base(type, needed)
        {
        }

        public MagicCollectionBarrel(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write(m_MinLevel);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    {
                        m_MinLevel = reader.ReadInt();
                        break;
                    }
            }
        }

        public override bool CheckItem(Item dropped)
        {
            if ( base.CheckItem(dropped) )
            {
                if ( dropped is BaseWeapon )
                {
                    return (int)((BaseWeapon)dropped).DamageLevel >= m_MinLevel;
                }
                else if ( dropped is BaseArmor )
                {
                    return (int)((BaseArmor)dropped).ProtectionLevel >= m_MinLevel;
                }
            }

            return false;
        }
    }
}
