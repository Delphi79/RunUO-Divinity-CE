using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Guilds;
using Server.Regions;

namespace Server.Battle
{
    public class BattleController : Item
    {
        public const int MaxVirtues = 2; // more later? i think not. be careful changing this

        private List<BattleBuilding> _Buildings;
        private BattleVirtue[] _Virtues = new BattleVirtue[MaxVirtues];
        private int _UnownedHue = 0x03B2;
        private DateTime _EndTime = DateTime.MinValue;
        private DayOfWeek _Day = DayOfWeek.Sunday;
        private TimeSpan _Hour = TimeSpan.FromHours( 17 );
        private TimeSpan _Length = TimeSpan.FromHours(3);
        private TimeSpan _PreBattleLen = TimeSpan.FromMinutes(15);
        private TimeSpan _PostBattleLen = TimeSpan.FromMinutes(5);
        private int _Owner = -1;
        private Timer _PreBattleTimer, _BattleTimer, _BattleEndTimer;
        public bool _Enabled = false, _InCoolDown = false;

        [Constructable()]
        public BattleController()
            : base(0x1B7A)
        {
            Name = "Battle Controller";
            Visible = false;
            Movable = false;

            _Buildings = new List<BattleBuilding>();
        }

        public BattleController(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write(_Owner);
            writer.WriteItemList<BattleBuilding>(_Buildings, true);
            writer.Write(_UnownedHue);

            writer.Write(MaxVirtues);
            for (int i = 0; i < MaxVirtues; i++)
            {
                writer.Write(_Virtues[i]);
            }

            if (InProgress)
                writer.Write(DateTime.Now - _EndTime);
            else
                writer.Write(TimeSpan.Zero);

            writer.Write((int)_Day);
            writer.Write(_Hour);
            writer.Write(_Length);
            writer.Write(_PreBattleLen);
            writer.Write(_PostBattleLen);
            writer.Write(_Enabled);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            TimeSpan duration = TimeSpan.Zero;
            int vCount = MaxVirtues;

            switch (version)
            {
                case 0:
                    _Owner = reader.ReadInt();
                    _Buildings = reader.ReadStrongItemList<BattleBuilding>();
                    _UnownedHue = reader.ReadInt();

                    vCount = reader.ReadInt();
                    for (int i = 0; i < vCount; i++)
                    {
                        BattleVirtue bv = reader.ReadItem<BattleVirtue>();

                        if (i < MaxVirtues)
                            _Virtues[i] = bv;
                    }

                    duration = reader.ReadTimeSpan();
                    _Day = (DayOfWeek)reader.ReadInt();
                    _Hour = reader.ReadTimeSpan();
                    _Length = reader.ReadTimeSpan();
                    _PreBattleLen = reader.ReadTimeSpan();
                    _PostBattleLen = reader.ReadTimeSpan();
                    _Enabled = reader.ReadBool();
                    break;
            }

            if (duration > TimeSpan.Zero)
                _EndTime = DateTime.Now + duration + TimeSpan.FromMinutes(1); // 1 minute accounts for world load delay
            else
                _EndTime = DateTime.MinValue;

            ResetTimers();

            Timer.DelayCall(TimeSpan.Zero, new TimerCallback(OnLoad));
        }

        public void OnLoad()
        {
            // have to add outselves to the region
            Region reg = Region.Find(Location, Map);
            if (reg is VirtueGuardedRegion)
                ((VirtueGuardedRegion)reg).Battle = this;

            /* maybe this isnt really necessary? if we do this then we cant reboot the server ever
             * but if we don't do it, then people can change virtues and get rewards after a reboot.
            Owner = -1;

            foreach (BattleVirtue bv in _Virtues)
            {
                if (bv == null || bv.Deleted)
                    continue;

                bv.HideItems();
            }*/

            foreach (BattleBuilding bb in _Buildings)
            {
                if (bb == null || bb.Deleted)
                    continue;

                bb.RecalcOwner();
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.AccessLevel >= AccessLevel.GameMaster)
                from.SendGump(new Gumps.PropertiesGump(from, this));
        }

        public override void OnMapChange()
        {
            // no way to remove ourselves from the old map?
            Region newReg = Region.Find(Location, Map);
            if (newReg is VirtueGuardedRegion)
                ((VirtueGuardedRegion)newReg).Battle = this;

            base.OnMapChange();
        }

        public override void OnLocationChange(Point3D oldLocation)
        {
            Region oldReg = Region.Find(oldLocation, Map);
            Region newReg = Region.Find(Location, Map);

            if (oldReg is VirtueGuardedRegion)
                ((VirtueGuardedRegion)oldReg).Battle = null;

            base.OnLocationChange(oldLocation);

            if ( newReg is VirtueGuardedRegion )
                ((VirtueGuardedRegion)newReg).Battle = this;
        }

        public void ResetTimers()
        {
            DateTime beginBattle = DateTime.Now.Date + _Hour + TimeSpan.FromDays( (int)_Day - (int)DateTime.Now.DayOfWeek );
            DateTime beginPreBattle;

            if ( _PreBattleTimer != null )
            {
                _PreBattleTimer.Stop();
                _PreBattleTimer = null;
            }
            if ( _BattleTimer != null )
            {
                _BattleTimer.Stop();
                _BattleTimer = null;
            }
            if ( _BattleEndTimer != null )
            {
                _BattleEndTimer.Stop();
                _BattleEndTimer = null;
            }

            if (!_Enabled)
            {
                _EndTime = DateTime.MinValue;
                return;
            }

            if ( beginBattle < DateTime.Now )
                beginBattle += TimeSpan.FromDays( 7 );

            beginPreBattle = beginBattle - _PreBattleLen;

            if (_EndTime == DateTime.MinValue)
            {
                if (beginPreBattle <= DateTime.Now)
                    _PreBattleTimer = Timer.DelayCall(TimeSpan.Zero, new TimerCallback(OnBeginPreBattle));
                else
                    _PreBattleTimer = Timer.DelayCall(beginPreBattle - DateTime.Now, new TimerCallback(OnBeginPreBattle));
            }

            _BattleTimer = Timer.DelayCall( beginBattle - DateTime.Now, new TimerCallback( OnBeginBattle ) );

            if ( _EndTime >= DateTime.Now )
            {
                _BattleEndTimer = Timer.DelayCall( _EndTime - DateTime.Now, new TimerCallback( OnEndBattle ) );
            }
            else
            {
                _EndTime = DateTime.MinValue;
            }
        }

        public void OnBeginPreBattle()
        {
            if (_PreBattleTimer != null)
                _PreBattleTimer.Stop();
            _PreBattleTimer = null;

            Owner = -1;

            foreach (BattleVirtue bv in _Virtues)
            {
                if (bv == null || bv.Deleted)
                    continue;

                bv.Score = 0;
            }

            foreach (BattleBuilding bb in _Buildings)
            {
                if ( bb == null || bb.Deleted )
                    continue;

                bb.RecalcOwner();
            }

            BroadcastMessage("The battle for {0} will begin shortly. The town is now open.", RegName);
        }

        public void OnBeginBattle()
        {
            Owner = -1;

            foreach (BattleVirtue bv in _Virtues)
            {
                if (bv == null || bv.Deleted)
                    continue;

                bv.Score = 0;
            }

            foreach (BattleBuilding bb in _Buildings)
            {
                if (bb == null || bb.Deleted)
                    continue;

                bb.RecalcOwner();
            }

            if (_BattleTimer != null)
                _BattleTimer.Stop();
            _BattleTimer = null;

            _EndTime = DateTime.Now + _Length;
            _BattleEndTimer = Timer.DelayCall(_Length, new TimerCallback(OnEndBattle));

            foreach (BattleBuilding bb in _Buildings)
            {
                if (bb == null || bb.Deleted)
                    continue;

                bb.BeginBattle();
            }

            BroadcastMessage("The battle for {0} has begun!", RegName);
        }

        public void CoolDownWarning()
        {
            if (InCoolDown)
            {
                Region reg = Region.Find(this.GetWorldLocation(), this.Map);

                int owner = Owner;
                foreach (Mobile m in reg.GetPlayers())
                {
                    int virt = GetVirtue(m);

                    if (virt != -1 && virt != owner)
                    {
                        m.SendMessage(0x25, "This town is now under {0} control.  You should leave at once or be killed!",
                            (GuildType)(owner + 1));
                    }
                }

                Timer.DelayCall(TimeSpan.FromMinutes(1), new TimerCallback(CoolDownWarning));
            }
        }

        public void EndCoolDown()
        {
            _InCoolDown = false;

            GuardedRegion reg = Region.Find(this.GetWorldLocation(), this.Map) as GuardedRegion;

            if (reg == null)
                return;

            int owner = Owner;
            foreach (Mobile m in reg.GetPlayers())
            {
                int virt = GetVirtue(m);

                if (virt != -1 && virt != owner)
                {
                    m.SendMessage(0x25, "This town is now under {0} control!", (GuildType)(owner + 1));
                    reg.CheckGuardCandidate(m);
                }
            }

            ParticipantMessage("The battle is over, and {0} has taken control of the town.", ((GuildType)(owner + 1)));
        }

        public void OnEndBattle()
        {
            _InCoolDown = true;
            Timer.DelayCall(TimeSpan.Zero, new TimerCallback(CoolDownWarning));
            Timer.DelayCall(_PostBattleLen, new TimerCallback(EndCoolDown));

            if ( _BattleEndTimer != null )
                _BattleEndTimer.Stop();
            _BattleEndTimer = null;

            foreach (BattleBuilding bb in _Buildings)
            {
                if (bb == null || bb.Deleted)
                    continue;

                bb.EndBattle();
            }

            // dont end the battle until after the building ends are called
            _EndTime = DateTime.MinValue;
            ResetTimers();

            int winner = -1;
            for (int i = 0; i < _Virtues.Length; i++)
            {
                if (_Virtues[i] == null || _Virtues[i].Deleted)
                    continue;

                if (winner == -1 ||
                    _Virtues[winner].Score < _Virtues[i].Score ||
                    (_Virtues[winner].Score == _Virtues[i].Score && Utility.RandomBool()))
                {
                    winner = i;
                }

                _Virtues[i].HideItems();
            }

            for (int i = 0; i < _Virtues.Length; i++)
            {
                if (_Virtues[i] == null || _Virtues[i].Deleted)
                    continue;

                _Virtues[i].Score = 0;
            }

            if (winner >= 0 && winner < MaxVirtues && _Virtues[winner] != null)
                _Virtues[winner].ShowItems();

            Owner = winner;

            foreach (BattleBuilding bb in _Buildings)
            {
                if (bb == null || bb.Deleted)
                    continue;

                bb.RecalcOwner();
            }

            BroadcastMessage("The battle for {0} is over; {1} is victorious!", RegName, ((GuildType)(winner + 1)));
        }

        public int GetHue(int virtue)
        {
            if (virtue >= 0 && virtue < MaxVirtues && _Virtues[virtue] != null)
                return _Virtues[virtue].ItemHue;
            return _UnownedHue;
        }

        public int GetVirtue(Mobile m)
        {
            if ( m.Guild == null )
                return -1;

            return ((int)m.Guild.Type) - 1; // reg=-1, chaos=0, order=1
        }

        public void Score(int virtue, int score)
        {
            if (!InProgress || virtue < 0 || virtue >= MaxVirtues)
                return;

            if (_Virtues[virtue] != null)
                _Virtues[virtue].Score += score;
        }

        public int GetActualScore(int virtue)
        {
            if (!InProgress || virtue < 0 || virtue >= MaxVirtues)
                return 0;

            int score = _Virtues[virtue] != null ? _Virtues[virtue].Score : 0;

            foreach (BattleBuilding bb in _Buildings)
            {
                int v, s;
                if (bb != null)
                {
                    bb.GetScore(out v, out s);

                    if ( v == virtue )
                        score += s;
                }
            }

            return score;
        }

        public void ParticipantMessage(string format, params object[] args)
        {
            ParticipantMessage(String.Format(format, args));
        }

        public void ParticipantMessage(string msg)
        {
            Region reg = Region.Find(this.GetWorldLocation(), this.Map);

            if (reg != null)
            {
                foreach (Mobile m in reg.GetPlayers())
                {
                    m.SendMessage(0x25, msg);
                }
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public string RegName
        {
            get
            {
                string name = "somewhere";

                Region reg = Region.Find(GetWorldLocation(), Map);
                if (reg.Name != null && reg.Name != "")
                    name = reg.Name;

                return name;
            }
        }

        public void BroadcastMessage(string format, params object[] args)
        {
            BroadcastMessage(String.Format(format, args));
        }

        public void BroadcastMessage(string msg)
        {
            Server.Commands.CommandHandlers.SystemMessage("Battle", msg);
            Server.Misc.IRCBot.SendMessage(Server.Misc.IRCBot.Channel, msg);
        }

        public bool AllowBeneficialAction( Mobile from, Mobile target )
        {
            // don't allow blues or enemies to heal
            return !InProgress || GetVirtue( from ) == GetVirtue( target );
        }

        public bool AllowHarmfulAction(Mobile from, Mobile target)
        {
            if ( !InProgress )
                return true;

            // don't allow team killing unless they aren't on a team
            int fromVirt = GetVirtue(from);
            return fromVirt != GetVirtue(target) || fromVirt == -1;
        }

        public GuildType GetGuardType()
        {
            if (InProgress || InCoolDown)
            {
                return GuildType.Regular;
            }
            else
            {
                return (GuildType)(Owner + 1);
            }
        }

        public bool InCoolDown { get { return _InCoolDown; } }
        public bool InProgress { get { return _Enabled && _EndTime != DateTime.MinValue; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Enabled
        {
            get { return _Enabled; }
            set
            {
                if (_Enabled != value)
                {
                    if (!value)
                    {
                        if ( InProgress )
                            OnEndBattle();
                        Owner = -1;

                        foreach (BattleVirtue bv in _Virtues)
                        {
                            if (bv == null || bv.Deleted)
                                continue;

                            bv.Score = 0;
                        }

                        foreach (BattleBuilding bb in _Buildings)
                        {
                            if (bb == null || bb.Deleted)
                                continue;

                            bb.RecalcOwner();
                        }
                    }

                    _Enabled = value;

                    ResetTimers();
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Owner 
        { 
            get { return _Owner; }
            set
            {
                if (_Owner >= 0 && _Owner < _Virtues.Length && _Virtues[_Owner] != null)
                    _Virtues[_Owner].HideItems();

                _Owner = value;

                if (_Owner >= 0 && _Owner < _Virtues.Length && _Virtues[_Owner] != null)
                    _Virtues[_Owner].ShowItems();

                foreach (BattleBuilding bb in _Buildings)
                {
                    if (bb == null || bb.Deleted)
                        continue;

                    bb.RecalcOwner();
                }
            }
        }

        public List<BattleBuilding> Buildings { get { return _Buildings; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int UnownedItemHue
        {
            get { return _UnownedHue; }
            set { _UnownedHue = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public BattleVirtue Chaos
        {
            get { return _Virtues[((int)GuildType.Chaos - 1)]; }
            set { _Virtues[((int)GuildType.Chaos - 1)] = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public BattleVirtue Order
        {
            get { return _Virtues[((int)GuildType.Order - 1)]; }
            set { _Virtues[((int)GuildType.Order - 1)] = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public DayOfWeek BattleDay
        {
            get { return _Day; }
            set { _Day = value; ResetTimers(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan BattleHour
        {
            get { return _Hour; }
            set { _Hour = value; ResetTimers(); }
        }
        
        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan PreBattlePeriod
        {
            get { return _PreBattleLen; }
            set { _PreBattleLen = value; ResetTimers(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan PostBattlePeriod
        {
            get { return _PostBattleLen; }
            set { _PostBattleLen = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan BattleLength
        {
            get { return _Length; }
            set
            {
                if (_EndTime != DateTime.MinValue)
                    _EndTime -= _Length;
                _Length = value;
                if (_EndTime != DateTime.MinValue)
                {
                    _EndTime += _Length;

                    if (_EndTime <= DateTime.Now)
                    {
                        OnEndBattle();
                        return; // OnEnd does a resettimers() call
                    }
                }
                ResetTimers();
            }
        }

        [CommandProperty(AccessLevel.Counselor)]
        public TimeSpan TimeLeft
        {
            get
            {
                if (_EndTime <= DateTime.Now)
                    return TimeSpan.Zero;

                return _EndTime - DateTime.Now;
            }
        }
    }
}

