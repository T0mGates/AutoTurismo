using System;
using System.Collections.Generic;

using UnityEngine;

public class Profile
{
    private string                              driverName;
    private int                                 money;
    private int                                 experience;
    private int                                 maxExperience;
    private int                                 level;
    private Dictionary<Type, List<Dealer>>      unlockedDealersDict;
    private Dictionary<Type, List<Purchasable>> ownedProductsDict;

    public Profile(string driverNameParam){
        // Initialize our dealers dict
        unlockedDealersDict = new Dictionary<Type, List<Dealer>>();
        foreach(Type dealerType in Dealers.dealerTypes){
            unlockedDealersDict[dealerType] = new List<Dealer>();
        }

        // Initialize our owned products dict
        ownedProductsDict = new Dictionary<Type, List<Purchasable>>();
        foreach(Type productType in Purchasable.productTypes){
            ownedProductsDict[productType] = new List<Purchasable>();
        }

        // Base values
        driverName          = driverNameParam;

        SetBaseValues();
    }

    private void SetBaseValues(){
        money               = 20000;
        experience          = 0;
        level               = 1;
        SetMaxExperienceBasedOnLevel();

        // Base unlocks
        UnlockDealer(Dealers.GetDealer(Dealers.VEE_NAME,                    typeof(CarDealer)));
        UnlockDealer(Dealers.GetDealer(Dealers.VOLKSWAGEN_NAME,             typeof(CarDealer)));
        UnlockDealer(Dealers.GetDealer(Dealers.COPA_CLASSIC_B_NAME,         typeof(CarDealer)));
        UnlockDealer(Dealers.GetDealer(Dealers.VEE_NAME,                    typeof(EntryPassDealer)));
        UnlockDealer(Dealers.GetDealer(Dealers.VOLKSWAGEN_NAME,             typeof(EntryPassDealer)));
        UnlockDealer(Dealers.GetDealer(Dealers.COPA_CLASSIC_B_NAME,         typeof(EntryPassDealer)));

        Event raceEvent = Event.GenerateNewRaceEvent(
            "Sunday Cup - Copa Classic B",
            Tracks.Country.Brazil,
            EventSeriesManager.SeriesTier.Rookie,
            Event.EventDuration.Mini,
            new List<Cars.CarType>(),
            new List<Cars.CarClass>() { {Cars.CarClass.CopaClassicB} },
            new List<Cars.CarBrand>(),
            new List<string>(),
            25,
            1500,
            true
        );
        Event raceEventTwo = Event.GenerateNewRaceEvent(
            "Saturday Cup - Copa Classic B",
            Tracks.Country.Brazil,
            EventSeriesManager.SeriesTier.Amateur,
            Event.EventDuration.Long,
            new List<Cars.CarType>(),
            new List<Cars.CarClass>() { {Cars.CarClass.CopaClassicB} },
            new List<Cars.CarBrand>(),
            new List<string>(),
            30,
            1750,
            false
        );

        Event raceEventVee = Event.GenerateNewRaceEvent(
            "Sunday Cup - Formula Vee",
            Tracks.Country.Brazil,
            EventSeriesManager.SeriesTier.Novice,
            Event.EventDuration.Mini,
            new List<Cars.CarType>(),
            new List<Cars.CarClass>() { {Cars.CarClass.FormulaVeeBrasil} },
            new List<Cars.CarBrand>(),
            new List<string>(),
            25,
            1500,
            true
        );
        Event raceEventVeeTwo = Event.GenerateNewRaceEvent(
            "Saturday Cup - Formula Vee",
            Tracks.Country.Brazil,
            EventSeriesManager.SeriesTier.Amateur,
            Event.EventDuration.Long,
            new List<Cars.CarType>(),
            new List<Cars.CarClass>() { {Cars.CarClass.FormulaVeeBrasil} },
            new List<Cars.CarBrand>(),
            new List<string>(),
            30,
            1750,
            false
        );

        EventSeries newSeries = new EventSeries("Copa Classic B for Dummies", EventSeriesManager.SeriesTier.Rookie, new List<Event>() { {raceEvent}, {raceEventTwo} });
        EventSeries newSeriesTwo = new EventSeries("Formula Vee for Dummies", EventSeriesManager.SeriesTier.Novice, new List<Event>() { {raceEventVee}, {raceEventVeeTwo} });

        EventSeriesManager.AddNewSeries(Tracks.Country.Brazil, newSeries);
        EventSeriesManager.AddNewSeries(Tracks.Country.Brazil, newSeriesTwo);
        EventSeriesManager.AddNewSeries(Tracks.Country.Brazil, newSeriesTwo);
        EventSeriesManager.AddNewSeries(Tracks.Country.Brazil, newSeries);
    }

    public List<Purchasable> GetOwnedProducts(Type productType){
        return ownedProductsDict[productType];
    }

    public void UnlockDealer(Dealer dealerToAdd){
        foreach(Type dealerType in Dealers.dealerTypes){
            if(dealerType == dealerToAdd.GetType()){
                unlockedDealersDict[dealerType].Add(dealerToAdd);
            }
        }
    }

    public void AddNewProduct(Purchasable toAdd){
        ownedProductsDict[toAdd.GetType()].Add(toAdd);
    }

    public void RemoveOwnedProduct(Purchasable toRemove){
        ownedProductsDict[toRemove.GetType()].Remove(toRemove);
    }

    public bool OwnsProduct(Purchasable toCheck){
        foreach(Purchasable product in ownedProductsDict[toCheck.GetType()]){
            if(product.Equals(toCheck)){
                return true;
            }
        }
        return false;
    }

    // Returns bool of whether we leveled up or not
    public bool GainExperience(int gainedExperience){
        // Gain exp
        experience += gainedExperience;
        // If we have enough exp to level up, level up
        if(experience >= maxExperience){
            LevelUp();
            return true;
        }
        return false;
    }

    public List<Dealer> GetUnlockedDealers(Type dealerType){
        return unlockedDealersDict[dealerType];
    }

    public int GetCurrentExperience(){
        return experience;
    }

    public int GetMaxExperience(){
        return maxExperience;
    }

    public void LoseMoney(int toLose){
        money -= toLose;
        if(money < 0){
            money = 0;
        }
    }

    public void GainMoney(int toGain){
        money += toGain;
    }

    public int GetMoney(){
        return money;
    }

    public int GetLevel(){
        return level;
    }

    private void SetMaxExperienceBasedOnLevel(){
        maxExperience = 100 * level;
    }

    private void LevelUp(){
        experience -= maxExperience;
        ++level;
        SetMaxExperienceBasedOnLevel();
    }
}