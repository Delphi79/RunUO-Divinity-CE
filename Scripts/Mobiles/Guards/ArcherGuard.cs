using System;
using System.Collections;
using Server.Misc;
using Server.Items;
using Server.Mobiles;

namespace Server.Mobiles
{
    public class ArcherGuard : BaseGuard
    {
        [Constructable]
        public ArcherGuard()
            : this(null)
        {
        }

        public ArcherGuard(Mobile target)
            : base(target)
        {
            InitStats(100, 125, 25);
            Title = "the guard";

            SpeechHue = Utility.RandomDyedHue();

            Hue = Utility.RandomSkinHue();

            if (Female = Utility.RandomBool())
            {
                Body = 0x191;
                Name = NameList.RandomName("female");
            }
            else
            {
                Body = 0x190;
                Name = NameList.RandomName("male");
            }

            new Horse().Rider = this;

            AddItem(new StuddedChest());
            AddItem(new StuddedArms());
            AddItem(new StuddedGloves());
            AddItem(new StuddedGorget());
            AddItem(new StuddedLegs());
            AddItem(new Boots());
            AddItem(new SkullCap());

            Bow bow = new Bow();

            bow.Movable = false;
            bow.Crafter = this;
            bow.Quality = WeaponQuality.Exceptional;

            AddItem(bow);

            Container pack = new Backpack();

            pack.Movable = false;

            Arrow arrows = new Arrow(250);

            arrows.LootType = LootType.Newbied;

            pack.DropItem(arrows);
            pack.DropItem(new Gold(10, 25));

            AddItem(pack);

            Skills[SkillName.Anatomy].Base = 120.0;
            Skills[SkillName.Tactics].Base = 120.0;
            Skills[SkillName.Archery].Base = 120.0;
            Skills[SkillName.MagicResist].Base = 120.0;
            Skills[SkillName.DetectHidden].Base = 100.0;
        }

        public ArcherGuard(Serial serial)
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
