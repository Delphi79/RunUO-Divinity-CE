using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Regions;
using Server.Guilds;

namespace Server.Battle
{
    public class BattleVirtue : Item
    {
        private int _Score = 0;
        private List<Item> _Items;
        private Map _HideMap;
        private BattleVirtueRegion _Region;
        private Rectangle2D _Rect;
        private GuildType _GuildType;
        private int _ItemHue;

        [Constructable()]
        public BattleVirtue()
            : base(0x1B7A)
        {
            Name = "Battle Virtue Controller";
            Movable = false;
            Visible = false;

            _Items = new List<Item>();
        }

        public BattleVirtue(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write(_Score);
            writer.Write(_Items, true);
            writer.Write(_HideMap);
            writer.Write(_ItemHue);
            writer.Write((int)_GuildType);
            writer.Write(_Rect);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    _Score = reader.ReadInt();
                    _Items = reader.ReadStrongItemList();
                    _HideMap = reader.ReadMap();
                    _ItemHue = reader.ReadInt();
                    _GuildType = (GuildType)reader.ReadInt();
                    _Rect = reader.ReadRect2D();
                    break;
            }
            Timer.DelayCall(TimeSpan.Zero, new TimerCallback(UpdateRect));
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.AccessLevel >= AccessLevel.GameMaster)
                from.SendGump(new Gumps.PropertiesGump(from, this));
        }

        public void UpdateRect()
        {
            if (_Region != null)
            {
                _Region.Unregister();
                _Region = null;
            }

            if (_Rect.Start != Point2D.Zero && _Rect.End != Point2D.Zero && this.Map != Map.Internal)
                _Region = new BattleVirtueRegion(this, _Rect, this.GetWorldLocation(), this.Map);
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (_Region != null)
                _Region.Unregister();
            _Region = null;
        }

        public override void OnMapChange()
        {
            base.OnMapChange();
            UpdateRect();
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int ItemCount { get { return _Items.Count; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public Rectangle2D Perimeter
        {
            get { return _Rect; }
            set
            {
                _Rect = value;
                UpdateRect();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Map HideMap
        {
            get { return _HideMap; }
            set { _HideMap = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Score
        {
            get { return _Score; }
            set { _Score = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public GuildType GuildType
        {
            get { return _GuildType; }
            set { _GuildType = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int ItemHue
        {
            get { return _ItemHue; }
            set { _ItemHue = value; }
        }

        public List<Item> Decorations { get { return _Items; } }

        public void ShowItems()
        {
            foreach (Item item in _Items)
            {
                if ( item == null )
                    continue;

                if (item is Spawner)
                    ((Spawner)item).RemoveCreatures();

                item.Map = this.Map;
            }

            foreach (Item item in _Items)
            {
                if (item == null)
                    continue;

                if (item is Spawner)
                    ((Spawner)item).Respawn();
            }
        }

        public void HideItems()
        {
            foreach (Item item in _Items)
            {
                if (item == null)
                    continue;

                if (item is Spawner)
                    ((Spawner)item).RemoveCreatures();

                item.Map = _HideMap;
            }
        }
    }

    public class BattleVirtueRegion : VirtueGuardedRegion
    {
        private BattleVirtue _Ctrl;

        public BattleVirtueRegion(BattleVirtue ctrl, Rectangle2D area, Point3D goloc, Map map)
            : base(null, map, 100, area)
        {
            GoLocation = goloc;

            _Ctrl = ctrl;

            Register();
        }

        public override GuildType GetVirtueOwner()
        {
            if (_Ctrl == null || _Ctrl.Deleted)
                return GuildType.Regular;
            else
                return _Ctrl.GuildType;
        }

        public override bool IsConstantCandidate(Mobile m)
        {
            if (m is BaseGuard || !m.Alive || m.AccessLevel > AccessLevel.Player || m.Blessed || IsDisabled())
                return false;

            if (m.Player)
            {
                Guilds.GuildType owner = GetVirtueOwner();
                Guilds.GuildType mtype = m != null && m.Guild != null ? m.Guild.Type : Server.Guilds.GuildType.Regular;

                if (owner != Server.Guilds.GuildType.Regular && owner != mtype)
                    return true;
            }
            return base.IsConstantCandidate(m);
        }

        public override bool IsGuardCandidate(Mobile m)
        {
            if (m is BaseGuard || !m.Alive || m.AccessLevel > AccessLevel.Player || m.Blessed || IsDisabled())
                return false;

            if (m.Player && !m.Blessed)
            {
                Guilds.GuildType owner = GetVirtueOwner();
                Guilds.GuildType mtype = m != null && m.Guild != null ? m.Guild.Type : Server.Guilds.GuildType.Regular;

                if (owner != Server.Guilds.GuildType.Regular && owner != mtype)
                    return true;
            }
            return base.IsGuardCandidate(m);
        }
    }
}
