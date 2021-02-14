using System;
using System.Collections;
using Server.Multis.Deeds;

namespace Server.Mobiles
{
	public class SBHouseDeed: SBInfo
	{
		private ArrayList m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBHouseDeed()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override ArrayList BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : ArrayList
		{
			public InternalBuyInfo()
			{
                //Add( new GenericBuyInfo( 1041217, typeof( BlueTentDeed ), 16375, 10, 0x14F0, 0 ) );
                //Add( new GenericBuyInfo( 1041218, typeof( GreenTentDeed ), 16375, 10, 0x14F0, 0 ) );

                Add(new GenericBuyInfo(1041211, typeof(StonePlasterHouseDeed), 43800, 10, 0x14F0, 0));
                Add(new GenericBuyInfo(1041212, typeof(FieldStoneHouseDeed), 43800, 10, 0x14F0, 0));
                Add(new GenericBuyInfo(1041213, typeof(SmallBrickHouseDeed), 43800, 10, 0x14F0, 0));
                Add(new GenericBuyInfo(1041214, typeof(WoodHouseDeed), 43800, 10, 0x14F0, 0));
                Add(new GenericBuyInfo(1041215, typeof(WoodPlasterHouseDeed), 43800, 10, 0x14F0, 0));
                Add(new GenericBuyInfo(1041216, typeof(ThatchedRoofCottageDeed), 43800, 10, 0x14F0, 0));

                /* Gone... for now :X
                Add( new GenericBuyInfo( "Deed to a Small Smith's Shop", typeof( SmallForgeHouseDeed ), 16400, 10, 0x14f0, 0 ) );
                Add( new GenericBuyInfo( "Deed to a Weapon Training Hut", typeof( SmallTrainingHouseDeed ), 16400, 10, 0x14f0, 0 ) );
                Add( new GenericBuyInfo( "Deed to a Pickpocket's Den", typeof( SmallPickpocketHouseDeed ), 16400, 10, 0x14f0, 0 ) );
                Add( new GenericBuyInfo( "Deed to a Small Weaver's Shop", typeof( SmallTailorHouseDeed ), 16400, 10, 0x14f0, 0 ) );
                Add( new GenericBuyInfo( "Deed to a Small Baker's Shop", typeof( SmallBakeryHouseDeed ), 16400, 10, 0x14f0, 0 ) );
                */

                //Add( new GenericBuyInfo( "deed to a Marble Workshop", typeof( MarbleWorkshopDeed ), 150000, 10, 0x14f0, 0 ) );
                Add(new GenericBuyInfo( "deed to a small stone workshop", typeof(StoneWorkshopDeed), 225000, 10, 0x14f0, 0));
                Add( new GenericBuyInfo( "deed to a two story villa", typeof( VillaDeed ), 900000, 20, 0x14F0, 0 ) );
                Add(new GenericBuyInfo( "deed to a sandstone house with patio", typeof(SandstonePatioDeed), 300000, 10, 0x14f0, 0));
                Add( new GenericBuyInfo( "deed to a Log Cabin", typeof( LogCabinDeed ), 437500, 10, 0x14f0, 0 ) );
                Add(new GenericBuyInfo("deed to a small stone tower", typeof(SmallTowerDeed), 312500, 10, 0x14f0, 0));
                //Add( new GenericBuyInfo( "deed to a marble house with patio", typeof( LargeMarbleDeed ), 3 * 325000, 10, 0x14f0, 0 ) );


                // Large houses
                Add(new GenericBuyInfo(1041219, typeof(BrickHouseDeed), 310675, 10, 0x14F0, 0));
                Add(new GenericBuyInfo("deed to a large house with patio", typeof(LargePatioDeed), 328520, 20, 0x14F0, 0));
                Add(new GenericBuyInfo(1041220, typeof(TwoStoryWoodPlasterHouseDeed), 413660, 10, 0x14F0, 0));
                Add(new GenericBuyInfo(1041221, typeof(TwoStoryStonePlasterHouseDeed), 413660, 10, 0x14F0, 0));
                Add(new GenericBuyInfo(1041222, typeof(TowerDeed), 641500, 10, 0x14F0, 0));
                Add(new GenericBuyInfo(1041223, typeof(KeepDeed), 2494500, 10, 0x14F0, 0));
                //Add(new GenericBuyInfo(1041224, typeof(CastleDeed), 10228000, 10, 0x14F0, 0));

                /*
                Add(new GenericBuyInfo( "deed to a blue tent", typeof(BlueTentDeed), 16375, 10, 0x14F0, 0));
                Add(new GenericBuyInfo( "deed to a green tent", typeof(GreenTentDeed), 16375, 10, 0x14F0, 0));

				Add( new GenericBuyInfo( "deed to a stone-and-plaster house", typeof( StonePlasterHouseDeed ), 43800, 20, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( "deed to a field stone house", typeof( FieldStoneHouseDeed ), 43800, 20, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( "deed to a small brick house", typeof( SmallBrickHouseDeed), 43800, 20, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( "deed to a wooden house", typeof( WoodHouseDeed ), 43800, 20, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( "deed to a wood-and-plaster house", typeof( WoodPlasterHouseDeed ), 43800, 20, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( "deed to a thatched-roof cottage", typeof( ThatchedRoofCottageDeed ), 43800, 20, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( "deed to a brick house", typeof( BrickHouseDeed ), 144500, 20, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( "deed to a two-story wood-and-plaster house", typeof( TwoStoryWoodPlasterHouseDeed ), 192400, 20, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( "deed to a tower", typeof( TowerDeed ), 433200, 20, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( "deed to a small stone keep", typeof( KeepDeed ), 665200, 20, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( "deed to a castle", typeof( CastleDeed ), 1022800, 20, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( "deed to a large house with patio", typeof( LargePatioDeed ), 152800, 20, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( "deed to a marble house with patio", typeof( LargeMarbleDeed ), 192000, 20, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( "deed to a small stone tower", typeof( SmallTowerDeed ), 88500, 20, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( "deed to a two story log cabin", typeof( LogCabinDeed ), 97800, 20, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( "deed to a sandstone house with patio", typeof( SandstonePatioDeed ), 90900, 20, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( "deed to a two story villa", typeof( VillaDeed ), 136500, 20, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( "deed to a small stone workshop", typeof( StoneWorkshopDeed ), 60600, 20, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( "deed to a small marble workshop", typeof( MarbleWorkshopDeed ), 63000, 20, 0x14F0, 0 ) );
                */
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
            {
                Add(typeof(BlueTentDeed), 16375);
                Add(typeof(GreenTentDeed), 16375);
                
                /*Add(typeof(StonePlasterHouseDeed), 43800);
                Add(typeof(FieldStoneHouseDeed), 43800);
                Add(typeof(SmallBrickHouseDeed), 43800);
                Add(typeof(WoodHouseDeed), 43800);
                Add(typeof(WoodPlasterHouseDeed), 43800);
                Add(typeof(ThatchedRoofCottageDeed), 43800);
                //Add(typeof(SmallForgeHouseDeed), 43800);
                Add(typeof(BrickHouseDeed), 144500);
                //Add(typeof(LargeForgeHouseDeed), 152800);
                Add(typeof(TwoStoryWoodPlasterHouseDeed), 192400);
                Add(typeof(TwoStoryStonePlasterHouseDeed), 192400);
                Add(typeof(TowerDeed), 433200);
                Add(typeof(KeepDeed), 3 * 665200);
                Add(typeof(CastleDeed), 10228000);

                Add(typeof(MarbleWorkshopDeed), 150000);
                Add(typeof(StoneWorkshopDeed), 150000);
                Add(typeof(VillaDeed), 2 * 450000);
                Add(typeof(SandstonePatioDeed), 200000);
                Add(typeof(LogCabinDeed), 250000);
                Add(typeof(SmallTowerDeed), 250000);
                Add(typeof(LargeMarbleDeed), 3 * 325000);*/
            }
		}
	}
}
