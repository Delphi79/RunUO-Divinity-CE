using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Regions;

namespace Server.Battle
{
    public class BattleBuilding : Item
    {
        private Rectangle2D _Rect;
        private List<Item> _Items;
        private BattleBuildingRegion _Region = null;
        private BattleController _Battle = null;
        private int _Owner = -1;
        private DateTime _OwnershipChange = DateTime.MinValue;
        private sbyte _ScoreFactor = 1;
        private Timer _CheckTimer;

        [Constructable()]
        public BattleBuilding()
            : base(0x1B7A)
        {
            Name = "Battle Building Controller";
            Visible = false;
            Movable = false;

            _Rect = new Rectangle2D();
            _Items = new List<Item>();

            _CheckTimer = Timer.DelayCall(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), new TimerCallback(CheckOwner));
            _CheckTimer.Priority = TimerPriority.FiveSeconds;
        }

        public BattleBuilding(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write(_Rect);
            writer.Write(_Items, true);
            writer.Write(_Battle);
            writer.Write(_Owner);
            writer.Write(_ScoreFactor);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 0:
                    _Rect = reader.ReadRect2D();
                    _Items = reader.ReadStrongItemList();
                    _Battle = reader.ReadItem<BattleController>();
                    _Owner = reader.ReadInt();
                    _ScoreFactor = reader.ReadSByte();
                    break;
            }

            Timer.DelayCall(TimeSpan.Zero, new TimerCallback(UpdateRect));

            _CheckTimer = Timer.DelayCall(TimeSpan.Zero, TimeSpan.FromSeconds(1.0), new TimerCallback(CheckOwner));
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
            {
                Point3D thisLoc = this.GetWorldLocation();
                Region parent = Region.Find(thisLoc, this.Map);

                if (parent == this.Map.DefaultRegion)
                    parent = null;

                _Region = new BattleBuildingRegion(this, parent, _Rect, thisLoc, this.Map);
            }
        }

        public override void OnMapChange()
        {
            base.OnMapChange();
            UpdateRect();
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (_Region != null)
                _Region.Unregister();
            _Region = null;

            if (_CheckTimer != null)
                _CheckTimer.Stop();
        }

        private void CheckOwner()
        {
            if (_Region == null || _Battle == null || _Battle.Deleted || !_Battle.InProgress)
                return;

            List<Mobile> list = _Region.GetPlayers();

            int[] counts = new int[BattleController.MaxVirtues];

            for (int i = 0; i < counts.Length; i++)
                counts[i] = 0;

            for (int i = 0; i < list.Count; i++)
            {
                if ( !list[i].Alive )
                    continue;

                int idx = _Battle.GetVirtue(list[i]);
                if (idx >= 0 && idx < counts.Length)
                    counts[idx]++;
            }

            int newOwner = -1;
            int first = 0, second = 1;

            if (counts[1] > counts[0])
            {
                first = 1;
                second = 0;
            }

            for (int i = 2; i < counts.Length; i++)
            {
                if (counts[i] > counts[first])
                {
                    second = first;
                    first = i;
                }
                else if (counts[i] > counts[second])
                {
                    second = i;
                }
            }

            if (counts[second] == 0 && counts[first] > 0)
                newOwner = first;

            if (counts[first] > (counts[second] + 1))
                newOwner = first;

            if (newOwner != _Owner)
            {
                OnOwnerChange(newOwner);

                _Owner = newOwner;
            }
        }

        public List<Item> Decorations { get { return _Items; } }

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

        [CommandProperty( AccessLevel.GameMaster )]
        public int ItemCount { get { return _Items.Count; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public sbyte ScoreFactor 
        { 
            get { return _ScoreFactor; }
            set 
            { 
                _ScoreFactor = value;
                if (_ScoreFactor == 0)
                    _ScoreFactor = 1;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public BattleController Battle
        {
            get { return _Battle; }
            set 
            {
                if (_Battle != null)
                    _Battle.Buildings.Remove(this);
                _Battle = value;
                if (_Battle != null)
                    _Battle.Buildings.Add(this);

                // force update the owner and the display
                _OwnershipChange = DateTime.MinValue;
                _Owner = -1;
                OnOwnerChange(-1);
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Owner 
        { 
            get { return _Owner; } 
        }

        public void BeginBattle()
        {
            _OwnershipChange = DateTime.MinValue;
        }

        public void EndBattle()
        {
            OnOwnerChange( -1 );

            _Owner = -1;
            _OwnershipChange = DateTime.MinValue;
        }

        public void OnEnter( Mobile m )
        {
            if ( m.Player && m.Alive )
                CheckOwner();
        }

        public void OnExit( Mobile m )
        {
            if ( m.Player )
                CheckOwner();
        }

        public void RecalcOwner()
        {
            if (_Battle != null && !_Battle.InProgress)
            {
                if (_Battle.Owner != _Owner)
                {
                    OnOwnerChange(_Battle.Owner);

                    _Owner = _Battle.Owner;
                }
            }
        }

        private void OnOwnerChange(int newOwner)
        {
            int score, hue;

            if (_Battle == null)
                return;

            score = GetScore();

            if (_Owner != -1 && score > 0)
                _Battle.Score( _Owner, score);

            hue = _Battle.GetHue(newOwner);
            foreach ( Item item in _Items )
                item.Hue = hue;

            if (newOwner != -1)
                _OwnershipChange = DateTime.Now;
            else
                _OwnershipChange = DateTime.MinValue;
        }

        private int GetScore()
        {
            int sc = 0;

            if (_OwnershipChange != DateTime.MinValue)
                sc = (int)((DateTime.Now - _OwnershipChange).TotalSeconds) * _ScoreFactor;

            return sc;
        }

        public void GetScore(out int team, out int score)
        {
            team = _Owner;
            score = GetScore();
        }
    }

    public class BattleBuildingRegion : Region
	{
        private BattleBuilding _Ctrl;
		public BattleBuildingRegion( BattleBuilding ctrl, Region parent, Rectangle2D area, Point3D goloc, Map map ) : base( null, map, parent, area )
		{
			GoLocation = goloc;

            _Ctrl = ctrl;

			Register();
		}

		public override void OnEnter( Mobile m )
		{
			if ( _Ctrl != null )
                _Ctrl.OnEnter( m );
		}

		public override void OnExit( Mobile m )
		{
			if ( _Ctrl != null )
                _Ctrl.OnExit( m );
		}
	}
}
