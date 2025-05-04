using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class PlayerData{
    public string                                                               driverName;
    public int                                                                  money;
    public int                                                                  fame;
    public int                                                                  maxFame;
    public int                                                                  level;
    public int                                                                  renown;

    // unlockedDealersDict
    public List<string>                                                         unlockedCarDealers;
    public List<string>                                                         unlockedEntryPassDealers;


    // ownedProductsDict
    public List<string>                                                         ownedCars;
    public List<string>                                                         ownedEntryPasses;


    // unlockedRegionTiers
    // Parallel Lists
    public List<Region.ClickableRegion>                                         regions;
    public List<SerializableList<EventSeries.SeriesTier>>                       tiers;

    public List<SerializableEventSeries>                                        series;


    //public Dictionary<Type, List<Dealer>>                                       unlockedDealersDict;
    //public Dictionary<Type, List<Purchasable>>                                  ownedProductsDict;
    //public Dictionary<Region.ClickableRegion, List<EventSeries.SeriesTier>>     unlockedRegionTiers;

    public PlayerData(Profile profile)
    {
        unlockedCarDealers                  = new List<string>();
        unlockedEntryPassDealers            = new List<string>();

        ownedEntryPasses                    = new List<string>();
        ownedCars                           = new List<string>();

        regions                             = new List<Region.ClickableRegion>();
        tiers                               = new List<SerializableList<EventSeries.SeriesTier>>();

        series                              = new List<SerializableEventSeries>();


        driverName                          = profile.GetName();
        money                               = profile.GetMoney();
        fame                                = profile.GetCurrentFame();
        maxFame                             = profile.GetMaxFame();
        level                               = profile.GetLevel();
        renown                              = profile.GetRenown();

        List<Dealer>    entryPassDealers   = profile.GetUnlockedDealers(typeof(EntryPassDealer));
        List<Dealer>    carDealers         = profile.GetUnlockedDealers(typeof(CarDealer));

        foreach(Dealer dealer in entryPassDealers){
            unlockedEntryPassDealers.Add(dealer.name);
        }

        foreach(Dealer dealer in carDealers){
            unlockedCarDealers.Add(dealer.name);
        }

        List<Purchasable>   cars        = profile.GetOwnedProducts(typeof(Car));
        List<Purchasable>   passes      = profile.GetOwnedProducts(typeof(EntryPass));

        foreach(Purchasable entryPass in passes){
            ownedEntryPasses.Add(entryPass.name);
        }

        foreach(Purchasable car in cars){
            ownedCars.Add(car.name);
        }

        foreach(Region.ClickableRegion region in Enum.GetValues(typeof(Region.ClickableRegion))){
            regions.Add(region);
            SerializableList<EventSeries.SeriesTier> serTiers = new SerializableList<EventSeries.SeriesTier>();
            serTiers.SetList(profile.GetUnlockedTiers(region));
            tiers.Add(serTiers);

            // Now get all event series since we want them saved
            foreach(EventSeries eventSeries in Region.regions[region].series){
                series.Add(new SerializableEventSeries(eventSeries));
            }
        }
    }

    public Profile MakeProfile()
    {
        Debug.Log("Making and returning a loaded profile");

        if(unlockedCarDealers == null){
            unlockedCarDealers          = new List<string>();
        }
        if(unlockedEntryPassDealers == null){
            unlockedEntryPassDealers    = new List<string>();
        }
        if(ownedEntryPasses == null){
            ownedEntryPasses            = new List<string>();
        }
        if(ownedCars == null){
            ownedCars                   = new List<string>();
        }
        if(regions == null){
            regions                     = new List<Region.ClickableRegion>();
        }
        if(tiers == null){
            tiers                       = new List<SerializableList<EventSeries.SeriesTier>>();
        }

        Profile profile                 = new Profile(driverName);
        profile.SetValues(money, fame, renown, level);

        // Dealers
        foreach(string dealerName in unlockedCarDealers){
            Dealer dealer               = Dealers.GetDealer(dealerName, typeof(CarDealer));
            profile.UnlockDealer(dealer);
        }

        foreach(string dealerName in unlockedEntryPassDealers){
            Dealer dealer               = Dealers.GetDealer(dealerName, typeof(EntryPassDealer));
            profile.UnlockDealer(dealer);
        }

        // Products
        foreach(string productName in ownedCars){
            Car car         = Cars.GetCar(productName);
            profile.AddNewProduct(car);
        }

        foreach(string productName in ownedEntryPasses){
            EntryPass pass  = EntryPasses.GetEntryPass(productName);
            profile.AddNewProduct(pass);
        }

        // Region/Tiers
        for(int i = 0; i < regions.Count; i++){
            Region.ClickableRegion          region      = regions[i];
            List<EventSeries.SeriesTier>    tierList    = tiers[i].GetList();

            foreach(EventSeries.SeriesTier tier in tierList){
                profile.UnlockRegionTier(region, tier);
            }
        }

        // Series
        foreach(SerializableEventSeries serEventSeries in series){

            EventSeries  newSeries = new EventSeries(serEventSeries.name, serEventSeries.prefix, serEventSeries.suffix, serEventSeries.seriesTier, serEventSeries.partOfRegion, serEventSeries.seriesTheme);

            // Now add events to this series (simply instantiating them will do it automatically)
            foreach(SerializableEvent serEvent in serEventSeries.events){
                Event newEvent = new Event( serEvent.name, serEvent.prefix, serEvent.suffix, serEvent.eventType, serEvent.eventDuration,
                                            newSeries, serEvent.typeWhitelist.GetList(), serEvent.classWhitelist.GetList(), serEvent.brandWhitelist.GetList(),
                                            serEvent.nameWhitelist.GetList(), serEvent.topFameReward, serEvent.topMoneyReward);

                Dictionary<string, int> champDict       = new Dictionary<string, int>();
                for(int i = 0; i < serEvent.champhionshipNames.Count; i++){
                    champDict[serEvent.champhionshipNames[i]] = serEvent.championshipPoints[i];
                }
                newEvent.champhionshipPoints            = champDict;
                newEvent.completed                      = serEvent.completed;
                newEvent.gridSize                       = serEvent.gridSize;
                newEvent.finishPosition                 = serEvent.finishPosition;

                foreach(SerializableEventEntry serEventEntry in serEvent.eventEntries){
                    // Now add event entries to this event (simply instantiating them will do it automatically)
                    EventEntry newEventEntry            = new EventEntry(
                                                            Tracks.GetTrack(serEventEntry.trackName, serEventEntry.trackLayout, serEventEntry.trackCountry),
                                                            serEventEntry.gridSize, newEvent, minsParam:serEventEntry.mins, lapsParam:serEventEntry.laps,
                                                            startTimeParam:serEventEntry.startTime, timeProgressionParam:serEventEntry.timeProgression,
                                                            standingStartParam:serEventEntry.standingStart, weatherForecastParam:serEventEntry.weatherForecast.GetList(),
                                                            mandatoryPitStopParam:serEventEntry.mandatoryPitStop, pitStopMinTyresParam:serEventEntry.pitStopMinTyres,
                                                            fieldTypeParam:serEventEntry.fieldType
                                                        );
                    newEventEntry.attempted             = serEventEntry.attempted;
                    newEventEntry.nextUp                = serEventEntry.nextUp;
                    newEventEntry.totalDistanceTraveled = serEventEntry.totalDistanceTraveled;

                    List<ResultDriver> drivers          = new List<ResultDriver>();
                    foreach(SerializableResultDriver serDriver in serEventEntry.driverResults){
                        ResultDriver driver             = new ResultDriver(serDriver);
                        drivers.Add(driver);
                        if(driver.IsPlayer){
                            newEventEntry.playerResult = driver;
                        }
                    }
                    newEventEntry.driverResults         = drivers;

                    newEventEntry.playerCar             = serEventEntry.playerCarName == "" ? null : Cars.GetCar(serEventEntry.playerCarName);
                }
            }

        }

        return profile;
    }
}