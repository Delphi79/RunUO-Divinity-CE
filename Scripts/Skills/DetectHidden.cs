using System;
using Server.Multis;
using Server.Targeting;
using Server.Items;
using Server.Regions;

namespace Server.SkillHandlers
{
    public class DetectHidden
    {
        public static void Initialize()
        {
            SkillInfo.Table[(int)SkillName.DetectHidden].Callback = new SkillUseCallback(OnUse);
        }

        public static TimeSpan OnUse(Mobile src)
        {
            src.SendLocalizedMessage(500819);//Where will you search?
            src.Target = new InternalTarget();
            //src.RevealingAction();

            return TimeSpan.FromSeconds(5);
        }

        private class InternalTarget : Target
        {
            public InternalTarget()
                : base(12, true, TargetFlags.None)
            {
            }

            protected override void OnTarget(Mobile src, object targ)
            {
                bool foundAnything = false;

                double srcSkill = src.Skills[SkillName.DetectHidden].Value;
                Point3D p;
                if (targ is Mobile)
                    p = ((Mobile)targ).Location;
                else if (targ is Item)
                    p = ((Item)targ).Location;
                else if (targ is IPoint3D)
                    p = new Point3D((IPoint3D)targ);
                else
                    p = src.Location;

                double skillLevel = src.Skills.DetectHidden.Value;

                bool success = src.CheckSkill(SkillName.DetectHidden, 0.0, 100.0);
                int range = ((int)Math.Min(100.0, skillLevel)) / 25;

                BaseHouse house = BaseHouse.FindHouseAt(p, src.Map, 16);
                bool inHouse = (house != null && house.IsFriend(src));

                if (success || inHouse)
                {
                    IPooledEnumerable inRange = src.Map.GetMobilesInRange(p, inHouse ? 22 : range);
                    foreach (Mobile trg in inRange)
                    {
                        if (trg.Hidden && src != trg)
                        {
                            double ss = srcSkill + Utility.RandomMinMax(-20, 20);
                            double ts = trg.Skills[SkillName.Hiding].Value + Utility.RandomMinMax(-20, 20);
                            if (src.AccessLevel >= trg.AccessLevel && (!inHouse && ss >= ts || inHouse && house.IsInside(trg)))
                            {
                                trg.RevealingAction();
                                trg.SendLocalizedMessage(500814); // You have been revealed!
                                foundAnything = true;
                            }
                        }
                    }
                    inRange.Free();



                    inRange = src.Map.GetItemsInRange(p, range);
                    foreach (Item trg in inRange)
                    {
                        if (!(trg is TrapableContainer))
                            continue;
                        TrapableContainer cont = (TrapableContainer)trg;
                        if (cont.Visible && cont.Trapped && Utility.RandomDouble() * 100 < skillLevel)
                        {
                            int hue;
                            switch (cont.TrapType)
                            {
                                case TrapType.DartTrap:
                                    hue = 123;
                                    break;
                                case TrapType.ExplosionTrap:
                                case TrapType.MagicTrap:
                                    hue = 0x7D;
                                    break;
                                case TrapType.PoisonTrap:
                                    hue = 90;
                                    break;
                                default:
                                    hue = 0x3B2;
                                    break;
                            }
                            src.Send(new Server.Network.MessageLocalized(cont.Serial, cont.ItemID, Server.Network.MessageType.Regular, hue, 3, 500813, "", "")); // [trapped]
                            foundAnything = true;
                        }
                    }
                    inRange.Free();
                }

                if (!foundAnything)
                    src.SendLocalizedMessage(500817); // You can see nothing hidden there.
            }
        }
    }
}
