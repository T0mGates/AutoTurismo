using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;

[System.Serializable]
public class GameData
{
    public static int                                                                                                                       STARTING_MONEY = 50000;

    // GameManager
    public string                                                                                                                           driverName;
    public int                                                                                                                              money;
    public int                                                                                                                              fame;
    public int                                                                                                                              maxFame;
    public int                                                                                                                              level;
    public int                                                                                                                              renown;
    public string                                                                                                                           pathToJsons;
    [field:SerializeField]
    public SerializableDictionary<SerializableType, SerializableList<Dealer>>                                                               unlockedDealersDict;
    [field:SerializeField]
    public SerializableDictionary<SerializableType, SerializableList<Purchasable>>                                                          ownedProductsDict;
    [field:SerializeField]
    public SerializableDictionary<Region.ClickableRegion, SerializableList<EventSeries.SeriesTier>>                                         unlockedRegionTiers;
    [field:SerializeField]

    // Region
    public SerializableDictionary<Region.ClickableRegion, SerializableDictionary<EventSeries.SeriesTier, SerializableList<EventSeries>>>    regionTiersToSeries;

    // Values defined here are the 'new game' values
    public GameData(){
        driverName          = "";
        money               = STARTING_MONEY;
        fame                = 0;
        maxFame             = 0;
        level               = 1;
        renown              = 11;
        pathToJsons         = "";

        // Initialize our dealers dict
        unlockedDealersDict = new SerializableDictionary<SerializableType, SerializableList<Dealer>>();
        foreach(SerializableType dealerType in Dealers.dealerTypes){
            unlockedDealersDict[dealerType]         = new SerializableList<Dealer>();
        }

        // Initialize our owned products dict
        ownedProductsDict   = new SerializableDictionary<SerializableType, SerializableList<Purchasable>>();
        foreach(SerializableType productType in Purchasable.productTypes){
            ownedProductsDict[productType]          = new SerializableList<Purchasable>();
        }

        // Initialize our unlocked tiers dict
        unlockedRegionTiers = new SerializableDictionary<Region.ClickableRegion, SerializableList<EventSeries.SeriesTier>>();
        foreach(Region.ClickableRegion region in Enum.GetValues(typeof(Region.ClickableRegion))){
            unlockedRegionTiers[region]             = new SerializableList<EventSeries.SeriesTier>();
        }

        regionTiersToSeries = new SerializableDictionary<Region.ClickableRegion, SerializableDictionary<EventSeries.SeriesTier, SerializableList<EventSeries>>>();

        foreach(Region.ClickableRegion region in Enum.GetValues(typeof(Region.ClickableRegion))){
            regionTiersToSeries[region]             = new SerializableDictionary<EventSeries.SeriesTier, SerializableList<EventSeries>>();

            foreach(EventSeries.SeriesTier tier in Enum.GetValues(typeof(EventSeries.SeriesTier))){
                regionTiersToSeries[region][tier]   = new SerializableList<EventSeries>();
            }
        }
    }
}
