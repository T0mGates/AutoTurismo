using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using System.Reflection;
using UnityEngine.UIElements;
using Unity.Collections.LowLevel.Unsafe;

public class Profile
{
    public string                                                                                       driverName;
    public int                                                                                          money;
    public int                                                                                          fame;
    public int                                                                                          maxFame;
    public int                                                                                          level;
    public int                                                                                          renown;
    [field:SerializeField]
    public SerializableDictionary<SerializableType, SerializableList<Dealer>>                           unlockedDealersDict;
    [field:SerializeField]
    public SerializableDictionary<SerializableType, SerializableList<Purchasable>>                      ownedProductsDict;
    [field:SerializeField]
    public SerializableDictionary<Region.ClickableRegion, SerializableList<EventSeries.SeriesTier>>     unlockedRegionTiers;

    public Profile( string driverNameParam, int moneyParam, int fameParam, int maxFameParam, int levelParam, int renownParam,
                    SerializableDictionary<SerializableType, SerializableList<Dealer>> unlockedDealersDictParam, SerializableDictionary<SerializableType,
                    SerializableList<Purchasable>> ownedProductsDictParam, SerializableDictionary<Region.ClickableRegion,
                    SerializableList<EventSeries.SeriesTier>> unlockedRegionTiersParam ){
        driverName          = driverNameParam;
        money               = moneyParam;
        fame                = fameParam;
        maxFame             = maxFameParam;
        level               = levelParam;
        renown              = renownParam;
        unlockedDealersDict = unlockedDealersDictParam;
        ownedProductsDict   = ownedProductsDictParam;
        unlockedRegionTiers = unlockedRegionTiersParam;
    }

    public List<Purchasable> GetOwnedProducts(Type productType){
        SerializableType serType = SerializableType.GetSerializableTypeFromPurchasableType(productType);
        return ownedProductsDict[serType].GetList();
    }

    public void UnlockDealer(Dealer dealerToAdd){
        foreach(SerializableType dealerType in Dealers.dealerTypes){
            if(dealerType.GetAssemblyQualifiedName() == dealerToAdd.GetType().AssemblyQualifiedName){
                unlockedDealersDict[dealerType].Add(dealerToAdd);
            }
        }
    }

    public void UnlockRegionTier(Region.ClickableRegion region, EventSeries.SeriesTier tier){
        if(!unlockedRegionTiers[region].Contains(tier)){
            unlockedRegionTiers[region].Add(tier);
        }
    }

    public void AddNewProduct(Purchasable toAdd){
        ownedProductsDict[SerializableType.GetSerializableTypeFromPurchasableType(toAdd.GetType())].Add(toAdd);
    }

    public void RemoveOwnedProduct(Purchasable toRemove){
        ownedProductsDict[SerializableType.GetSerializableTypeFromPurchasableType(toRemove.GetType())].Remove(toRemove);
    }

    public bool OwnsProduct(Purchasable toCheck){
        foreach(Purchasable product in ownedProductsDict[SerializableType.GetSerializableTypeFromPurchasableType(toCheck.GetType())]){
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
        SerializableType serType            = Dealers.dealerTypeToSerialized[dealerType];
        SerializableList<Dealer> dealerss   = unlockedDealersDict[serType];

        return dealerss.GetList();
    }

    public List<EventSeries.SeriesTier> GetUnlockedTiers(Region.ClickableRegion region){
        return unlockedRegionTiers[region].GetList();
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
        foreach(KeyValuePair<Region.ClickableRegion, SerializableList<EventSeries.SeriesTier>> entry in unlockedRegionTiers){
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