using System;
using System.Collections;
using Server.Misc;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Mobiles
{
    public class WarriorGuard : BaseGuard
    {
        [Constructable]
        public WarriorGuard()
            : this(null)
        {
        }

        public WarriorGuard(Mobile target)
            : base(target)
        {
            SpeechHue = Utility.RandomDyedHue();
            Hue = Utility.RandomSkinHue();
            Title = "the guard";

            if (Female = Utility.RandomBool())
            {
                Body = 0x191;
                Name = NameList.RandomName("female");

                switch (Utility.Random(2))
                {
                    case 0: AddItem(new LeatherSkirt()); break;
                    case 1: AddItem(new LeatherShorts()); break;
                }

                AddItem(new FemalePlateChest());
            }
            else
            {
                Body = 0x190;
                Name = NameList.RandomName("male");

                Item item = new Shirt();
                AddItem(item);
                item.Hue = Utility.RandomNondyedHue();

                item = new ShortPants();
                AddItem(item);
                item.Hue = Utility.RandomNondyedHue();

                item = new PlateChest();
                AddItem(item);

                item = new PlateLegs();
                AddItem(item);

                item = new PlateArms();
                AddItem(item);

                item = new Tunic();
                AddItem(item);
                item.Hue = Utility.RandomNondyedHue();
            }
            Utility.AssignRandomHair(this);

            if (Female)
                Utility.AssignRandomFacialHair(this, HairHue);

            Halberd weapon = new Halberd();

            weapon.Movable = false;
            weapon.Quality = WeaponQuality.Exceptional;

            AddItem(weapon);

            Container pack = new Backpack();

            pack.Movable = false;

            pack.DropItem(new Gold(10, 25));

            AddItem(pack);
        }

        public WarriorGuard(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}
