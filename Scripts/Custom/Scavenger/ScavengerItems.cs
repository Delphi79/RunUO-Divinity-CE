//*********************************************************************
//*	Scavenger Hunt File: ScavengerItems.cs
//*
//*	Author: FrayedString
//*
//*
//*
//*	Description: Item script used to create the items players will
//*		search for during the scavenger hunt. They are randomly
//*		assigned an itemid of a monster statue from a list,
//*		followed by a hue from a list.
//*
//*	Scavenger Hunt includes the following files:
//*		- ScavengerBasket.cs		- ScavengerCmd.cs
//*		- ScavengerItemCounter.cs	- ScangerItems.cs
//*		- ScavengerSignup.cs		- ScavengerSignupGump.cs
//*     - ScavengerREADME.txt       - ScavengerLicense.txt
//*     - ScavengerCHANGELOG.txt
//*
//*********************************************************************

using System;
using Server;

namespace Server.Items
{
	public class ScavengerItemInfo
	{
		private static ScavengerItemInfo[] m_Table = new ScavengerItemInfo[]
			{
				new ScavengerItemInfo( 8438, "a Llama" ),
				new ScavengerItemInfo( 8490, "a Terathan Warrior" ),
				new ScavengerItemInfo( 8492, "a Terathan Matriarch" ),
				new ScavengerItemInfo( 8472, "a Bear" ),
				new ScavengerItemInfo( 8477, "an Eagle" ),
				new ScavengerItemInfo( 8496, "ASayre" ),
				new ScavengerItemInfo( 8479, "a Horse" ),
				new ScavengerItemInfo( 8501, "an Ostard" ),
				new ScavengerItemInfo( 9600, "Billy Goat Gruff" ),
				new ScavengerItemInfo( 9601, "a Centuar" ),
				new ScavengerItemInfo( 8429, "an Elemental" ),
				new ScavengerItemInfo( 9671, "a Ghost" ),
				new ScavengerItemInfo( 9763, "a Devourer" ),
				new ScavengerItemInfo( 9775, "a Flesh Renderer" ),
				new ScavengerItemInfo( 9745, "a Horde Demon" ),
				new ScavengerItemInfo( 11664, "a Satyr" ),
				new ScavengerItemInfo( 11672, "a Ferris" ),
				new ScavengerItemInfo( 9762, "a Skittering Hopper" ),
				new ScavengerItemInfo( 11676, "a Warhorse" ),
				new ScavengerItemInfo( 10083, "a Bake Kitsune" ),
				new ScavengerItemInfo( 9657, "a Giant Scorpion" ),
				new ScavengerItemInfo( 10084, "a Crane" ),
				new ScavengerItemInfo( 11669, "a Chimera" ),
				new ScavengerItemInfo( 10090, "a Hiyru" ),
				new ScavengerItemInfo( 10098, "Yomoto" ),
				new ScavengerItemInfo( 9654, "a Pixie" ),
				new ScavengerItemInfo( 9638, "a Mongbat" ),
				new ScavengerItemInfo( 10088, "a Gaman" ),
				new ScavengerItemInfo( 9609, "an Ethereal Warrior" ),
				new ScavengerItemInfo( 8415, "an Ogre" )
			};

		public static ScavengerItemInfo[] Table { get { return m_Table; } }

		private int m_ItemID;

		public int ItemID
		{
			get { return m_ItemID; }
			set { m_ItemID = value; }
		}

		private string m_Name;

		public string Name
		{
			get { return m_Name; }
			set { m_Name = value; }
		}

		public ScavengerItemInfo( int itemID, string name )
		{
			m_ItemID = itemID;
			m_Name = name;
		}
	}

	public class ScavengerItem : Item
	{
		[Constructable]
		public ScavengerItem()
			: base( 8438 )
		{
			Weight = 0;
			Movable = false;

			Visible = ScavengerSignup.Started; //true if event in progress, false if not

			ScavengerItemInfo info = ScavengerItemInfo.Table[Utility.Random(ScavengerItemInfo.Table.Length)];

			ItemID = info.ItemID;
			Name = Utility.Intern( String.Format( "A Scavenger Statue of {0}", info.Name ) );

			Hue = m_Hues[ Utility.Random( m_Hues.Length ) ];

			ScavengerSignup.ScavengerItems.Add( this ); //Add it to the sexy collection
		}

		private static int[] m_Hues = new int[] {
			1261, 1266, 1272, 1275, 1283, 1286, 1288, 1366, 
			1260, 1150, 1151, 1194, 2125, 37, 391 };

		[CommandProperty( AccessLevel.GameMaster )]
		public override bool Decays { get { return false; } }

		public override void OnDelete()
		{
			if( ScavengerSignup.ScavengerItems.Contains( this ) )
				ScavengerSignup.ScavengerItems.Remove( this );
		}

		public ScavengerItem( Serial serial )
			: base( serial )
		{
			//No need for the conditional since we KNOW the List isn't persisted across restarts.
			//if (!ScavengerSignup.ScavengerItems.Contains(this))
			ScavengerSignup.ScavengerItems.Add( this );
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
