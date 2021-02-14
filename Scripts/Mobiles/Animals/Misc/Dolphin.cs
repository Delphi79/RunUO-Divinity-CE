using System;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "a dolphin corpse" )]
	public class Dolphin : BaseCreature
	{
		[Constructable]
		public Dolphin()
			: base( AIType.AI_Animal, FightMode.Aggressor, 12, 1, 0.5, 0.75 )
		{
			Body = 151;
			Name = "a dolphin";
			SetStr( 21, 49 );
			SetHits( 21, 49 );
			SetDex( 66, 85 );
			SetStam( 90, 140 );
			SetInt( 16, 30 );
			SetMana( 0 );
			BaseSoundID = 138;
			SetSkill( SkillName.Wrestling, 19.2, 29 );
			SetSkill( SkillName.Parry, 65.1, 75 );
			SetSkill( SkillName.Tactics, 19.2, 29 );
			SetSkill( SkillName.MagicResist, 15.1, 20 );

			VirtualArmor = 8;
			SetDamage( 3, 6 );

			Fame = 500;
			Karma = 2000;

            CanSwim = true;
			CantWalk = true;
		}

		public override int Meat { get { return 1; } }

		public Dolphin( Serial serial )
			: base( serial )
		{
		}

		public override void OnDoubleClick( Mobile from )
		{
			if( from.AccessLevel >= AccessLevel.GameMaster )
				Jump();
		}

		public virtual void Jump()
		{
			if( Utility.RandomBool() )
				Animate( 3, 16, 1, true, false, 0 );
			else
				Animate( 4, 20, 1, true, false, 0 );
		}

		public override void OnThink()
		{
			if( Utility.RandomDouble() < .005 ) // slim chance to jump
				Jump();

			base.OnThink();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}