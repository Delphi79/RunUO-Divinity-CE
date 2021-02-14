using System;
using Server;
using Server.Misc;
using Server.Items;
using Server.Guilds;

namespace Server.Mobiles
{
	public abstract class BaseShieldGuard : BaseGuard
	{
        public BaseShieldGuard()
            : this(null)
        {
        }

		public BaseShieldGuard( Mobile target ) : base( target )
		{
            Item item = null;

            Female = Utility.RandomBool();
            Body = Female ? 401 : 400;
            SpeechHue = Utility.RandomDyedHue();
            Name = NameList.RandomName(Female ? "female" : "male");
            Hue = Utility.RandomSkinHue();
            Title = "the guard";

            BaseSoundID = 332;

            int hairHue = Utility.RandomHairHue();
            Utility.AssignRandomHair(this, hairHue);

            item = new Shirt();
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
            if ( Type == GuildType.Order )
                item.Hue = Utility.RandomBlueHue();
            else
                item.Hue = Utility.RandomRedHue();

            item = new VikingSword();
            AddItem(item);

            item = Shield;
            item.Movable = false;
            AddItem(item);

            if (!Female)
            {
                Utility.AssignRandomFacialHair(this, hairHue);

                item = new ShortPants();
                AddItem(item);
                item.Hue = Utility.RandomNondyedHue();
            }
            else
            {
                item = new Skirt();
                AddItem(item);
                item.Hue = Utility.RandomNondyedHue();
            }
		}

		public abstract int Keyword{ get; }
		public abstract BaseShield Shield{ get; }
		public abstract int SignupNumber{ get; }
		public abstract GuildType Type{ get; }

		public override bool HandlesOnSpeech( Mobile from )
		{
			if ( from.InRange( this.Location, 2 ) )
				return true;

			return base.HandlesOnSpeech( from );
		}

		public override void OnSpeech( SpeechEventArgs e )
		{
			if ( !e.Handled && e.HasKeyword( Keyword ) && e.Mobile.InRange( this.Location, 2 ) )
			{
				e.Handled = true;

				Mobile from = e.Mobile;
				Guild g = from.Guild as Guild;

				if ( g == null || g.Type != Type )
				{
					Say( SignupNumber );
				}
				else
				{
					Container pack = from.Backpack;
					BaseShield shield = Shield;
					Item twoHanded = from.FindItemOnLayer( Layer.TwoHanded );

					if ( (pack != null && pack.FindItemByType( shield.GetType() ) != null) || ( twoHanded != null && shield.GetType().IsAssignableFrom( twoHanded.GetType() ) ) )
					{
						Say( 1007110 ); // Why dost thou ask about virtue guards when thou art one?
						shield.Delete();
					}
					else if ( from.PlaceInBackpack( shield ) )
					{
						Say( Utility.Random( 1007101, 5 ) );
						Say( 1007139 ); // I see you are in need of our shield, Here you go.
						from.AddToBackpack( shield );
					}
					else
					{
						from.SendLocalizedMessage( 502868 ); // Your backpack is too full.
						shield.Delete();
					}
				}
			}

			base.OnSpeech( e );
		}

		public BaseShieldGuard( Serial serial ) : base( serial )
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
