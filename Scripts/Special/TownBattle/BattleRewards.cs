using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Battle
{
    public class BattleRepairReward : Item
    {
        private int _Charges = 1;

        [Constructable()]
        public BattleRepairReward()
            : base(0x14F0)
        {
            Hue = 0x484;
            Name = "a repair deed with 1 charge";
            LootType = LootType.Blessed;
        }

        public BattleRepairReward(Serial s)
            : base(s)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);

            writer.Write(_Charges);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            _Charges = reader.ReadInt();
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (this.RootParent != from)
            {
                from.SendMessage("This must be in your pack to use it.");
            }

            from.SendMessage("Select an item to repair.");
            from.BeginTarget(3, false, TargetFlags.None, new TargetCallback(OnTarget));
        }

        protected void OnTarget(Mobile from, object targeted)
        {
            if (!(targeted is Item))
                return;

            object parent = ((Item)targeted).RootParent;
            if (parent is Mobile && parent != from)
            {
                from.SendMessage("That must be in your pack for you to repair it.");
                return;
            }

            if (targeted is BaseWeapon)
            {
                BaseWeapon wep = targeted as BaseWeapon;
                if (wep.HitPoints >= (wep.MaxHitPoints * 0.95) || wep.MaxHitPoints <= 0)
                {
                    from.SendMessage("That weapon does not need to be repaired.");
                }
                else
                {
                    wep.HitPoints = wep.MaxHitPoints;
                    from.SendMessage("The weapon has been repaired.");
                    if (--Charges <= 0)
                    {
                        from.SendMessage(0x25,"The repair deed has been used up.");
                    }
                }
            }
            else if (targeted is BaseArmor)
            {
                BaseArmor arm = targeted as BaseArmor;

                if (arm.HitPoints >= (arm.MaxHitPoints * 0.95) || arm.MaxHitPoints <= 0)
                {
                    from.SendMessage("That armor does not need to be repaired.");
                }
                else
                {
                    arm.HitPoints = arm.MaxHitPoints;
                    from.SendMessage("The armor has been repaired.");
                    if (--Charges <= 0)
                    {
                        from.SendMessage(0x25,"The repair deed has been used up.");
                    }
                }
            }
            else if (targeted is BaseClothing)
            {
                BaseClothing arm = targeted as BaseClothing;

                if (arm.HitPoints >= (arm.MaxHitPoints * 0.95) || arm.MaxHitPoints <= 0)
                {
                    from.SendMessage("That clothing does not need to be repaired.");
                }
                else
                {
                    arm.HitPoints = arm.MaxHitPoints;
                    from.SendMessage("The clothing has been repaired.");
                    if (--Charges <= 0)
                    {
                        from.SendMessage(0x25,"The repair deed has been used up.");
                    }
                }
            }
            else
            {
                from.SendMessage("This can only be used to repair weapons, armor, and clothing.");
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Charges
        {
            get { return _Charges; }
            set
            {
                if (_Charges != value)
                {
                    _Charges = value;

                    Name = String.Format("a repair deed with {0} charge{1}", _Charges, _Charges != 1 ? "s" : "");
                }

                if (_Charges <= 0)
                {
                    Delete();
                }
            }
        }
    }
}
