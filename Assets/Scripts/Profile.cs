using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using System.Reflection;
using UnityEngine.UIElements;

public class Profile
{
    private string                                                              driverName;
    private int                                                                 money;
    private int                                                                 fame;
    private int                                                                 maxFame;
    private int                                                                 level;
    private int                                                                 renown;
    private Dictionary<Type, List<Dealer>>                                      unlockedDealersDict;
    private Dictionary<Type, List<Purchasable>>                                 ownedProductsDict;
    private Dictionary<Region.ClickableRegion, List<EventSeries.SeriesTier>>    unlockedRegionTiers;

    public Profile(string driverNameParam){
        // Initialize our dealers dict
        unlockedDealersDict = new Dictionary<Type, List<Dealer>>();
        foreach(Type dealerType in Dealers.dealerTypes){
            unlockedDealersDict[dealerType] = new List<Dealer>();
        }

        // Initialize our owned products dict
        ownedProductsDict   = new Dictionary<Type, List<Purchasable>>();
        foreach(Type productType in Purchasable.productTypes){
            ownedProductsDict[productType]  = new List<Purchasable>();
        }

        // Initialize our unlocked tiers dict
        unlockedRegionTiers = new Dictionary<Region.ClickableRegion, List<EventSeries.SeriesTier>>();
        foreach(Region.ClickableRegion region in Enum.GetValues(typeof(Region.ClickableRegion))){
            unlockedRegionTiers[region]           = new List<EventSeries.SeriesTier>();
        }

        // Base values
        driverName          = driverNameParam;

        SetBaseValues();
    }

    public void SetValues(int moneyParam, int fameParam, int renownParam, int levelParam){
        money               = moneyParam;
        fame                = fameParam;
        renown              = renownParam;
        level               = levelParam;
        SetMaxFameBasedOnLevel();
    }

    private void SetBaseValues(){
        money               = 13000;
        fame                = 0;
        renown              = 1;
        level               = 1;
        SetMaxFameBasedOnLevel();
        BaseUnlocks();
    }

    public void BaseUnlocks(){
    }

    public string GetName(){
        return driverName;
    }

    public List<Purchasable> GetOwnedProducts(Type productType){
        return ownedProductsDict[productType];
    }

    public void UnlockDealer(Dealer dealerToAdd){
        foreach(Type dealerType in Dealers.dealerTypes){
            if(dealerType == dealerToAdd.GetType()){
                if(!unlockedDealersDict[dealerType].Contains(dealerToAdd)){
                    unlockedDealersDict[dealerType].Add(dealerToAdd);
                }
            }
        }
    }

    public void UnlockRegionTier(Region.ClickableRegion region, EventSeries.SeriesTier tier){
        if(!unlockedRegionTiers[region].Contains(tier)){
            unlockedRegionTiers[region].Add(tier);
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
    public bool GainFame(int gainedFame){
        // Gain fame
        fame += gainedFame;
        // If we have enough fame to level up, level up
        if(fame >= maxFame){
            LevelUp();
            return true;
        }
        return false;
    }

    public List<Dealer> GetUnlockedDealers(Type dealerType){
        return unlockedDealersDict[dealerType];
    }

    public List<EventSeries.SeriesTier> GetUnlockedTiers(Region.ClickableRegion region){
        return unlockedRegionTiers[region];
    }

    public int GetCurrentFame(){
        return fame;
    }

    public int GetMaxFame(){
        return maxFame;
    }

    public void LoseMoney(int toLose){
        money -= toLose;
        if(money < 0){
            money = 0;
        }
    }

    public void LoseRenown(int toLose){
        renown -= toLose;
        if(renown < 0){
            renown = 0;
        }
    }

    public void GainMoney(int toGain){
        money += toGain;
    }

    public string GetPrintMoney(){
        return "$" + GetMoney().ToString("n0");
    }

    public string GetPrintRenown(){
        return GetRenown().ToString("n0") + " Renown";
    }

    public int GetMoney(){
        return money;
    }

    public int GetLevel(){
        return level;
    }

    public int GetRenown(){
        return renown;
    }

    // Used to detect which cars are legal for an event
    public List<Car> GetOwnedCarsFiltered(
        List<string> carNames, List<Cars.CarType> carTypes, List<Cars.CarClass> carClasses, List<Cars.CarBrand> carBrands
    ){
        // Get our filtered cars
        return Cars.FilterCars(GetOwnedProducts(typeof(Car)).OfType<Car>().ToList(), carNames, carTypes, carClasses, carBrands);
    }

    // Used to detect which cars are both for an event and also if we have a valid entry pass for the car
    public List<Car> GetOwnedCarsThatCanRaceEvent(
        List<string> carNames, List<Cars.CarType> carTypes, List<Cars.CarClass> carClasses, List<Cars.CarBrand> carBrands, EventSeries.SeriesTier seriesTier
    ){
        // Get our filtered cars and entry passes
        List<Car>       filteredCars    = GetOwnedCarsFiltered(carNames, carTypes, carClasses, carBrands);
        List<EntryPass> filteredPasses  = EntryPasses.FilterEntryPasses(GetOwnedProducts(typeof(EntryPass)).OfType<EntryPass>().ToList(), seriesTier);

        List<Car>       toReturn        = new List<Car>();

        // Now check which cars we have valid entry passes for
        foreach(Car car in filteredCars){
            foreach(EntryPass entryPass in filteredPasses){
                if(entryPass.WorksWithCar(car)){
                    toReturn.Add(car);
                    break;
                }
            }
        }

        return toReturn;
    }

    public bool HasUnlockedARegionTier(){
        foreach(KeyValuePair<Region.ClickableRegion, List<EventSeries.SeriesTier>> entry in unlockedRegionTiers){
            if(entry.Value.Count > 0){
                return true;
            }
        }
        return false;
    }

    private void SetMaxFameBasedOnLevel(){
        maxFame = 100 +  (5 * (level-1));
    }

    private void LevelUp(){
        fame -= maxFame;
        ++level;
        ++renown;
        SetMaxFameBasedOnLevel();
    }
}