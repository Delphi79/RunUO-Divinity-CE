using System;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;
using Server.ContextMenus;

namespace Server.Mobiles
{
	[CorpseName("a llama corpse")]
	public class PackLlama : BaseCreature
	{
		[Constructable]
		public PackLlama() : base( AIType.AI_Animal, FightMode.Aggressor, 12, 1, 0.5, 0.75 )
		{
			Body = 292;
			Name = "a pack llama";
			SetStr( 21, 49 );
			SetHits( 21, 49 );
			SetDex( 36, 55 );
			SetStam( 36, 55 );
			SetInt( 16, 30 );
			SetMana( 0 );
			Tamable = true;
			MinTameSkill = 50;
			BaseSoundID = 181;
			SetSkill( SkillName.Wrestling, 19.2, 29 );
			SetSkill( SkillName.Parry, 35.1, 45 );
			SetSkill( SkillName.Tactics, 19.2, 29 );
			SetSkill( SkillName.MagicResist, 15.1, 20 );

			VirtualArmor = 8;
			SetDamage( 2, 6 );

            Fame = 0;
			Karma = 200;

			Container pack = Backpack;

			if ( pack != null )
				pack.Delete();

			pack = new StrongBackpack();
			pack.Movable = false;

			AddItem( pack );
		}

		public override int Meat{ get{ return 1; } }
		public override FoodType FavoriteFood{ get{ return FoodType.FruitsAndVegies | FoodType.GrainsAndHay; } }

		public PackLlama( Serial serial ) : base( serial )
		{
		}

		#region Pack Animal Methods
		public override bool OnBeforeDeath()
		{
			if ( !base.OnBeforeDeath() )
				return false;

			PackAnimal.CombineBackpacks( this );

			return true;
		}

		public override DeathMoveResult GetInventoryMoveResultFor( Item item )
		{
			return DeathMoveResult.MoveToCorpse;
		}

		public override bool IsSnoop( Mobile from )
		{
			if ( PackAnimal.CheckAccess( this, from ) )
				return false;

			return base.IsSnoop( from );
		}

		public override bool OnDragDrop( Mobile from, Item item )
		{
			if ( CheckFeed( from, item ) )
				return true;

			if ( PackAnimal.CheckAccess( this, from ) )
			{
				AddToBackpack( item );
				return true;
			}

			return base.OnDragDrop( from, item );
		}

		public override bool CheckNonlocalDrop( Mobile from, Item item, Item target )
		{
			return PackAnimal.CheckAccess( this, from );
		}

		public override bool CheckNonlocalLift( Mobile from, Item item )
		{
			return PackAnimal.CheckAccess( this, from );
		}

		public override void OnDoubleClick( Mobile from )
		{
			PackAnimal.TryPackOpen( this, from );
		}

		public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
		{
			base.GetContextMenuEntries( from, list );

			PackAnimal.GetContextMenuEntries( this, from, list );
		}
		#endregion

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}