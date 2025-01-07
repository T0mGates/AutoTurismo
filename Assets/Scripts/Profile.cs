using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using System.Reflection;
using UnityEngine.UIElements;

public class Profile
{
    private string                              driverName;
    private int                                 money;
    private int                                 fame;
    private int                                 maxFame;
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
        money               = 50000;
        fame                = 0;
        level               = 1;
        SetMaxFameBasedOnLevel();
        BaseUnlocks();
    }

    public void BaseUnlocks(){
        // Base unlocks (DEALERS)
        UnlockDealer(Dealers.GetDealer(Cars.classToString[Cars.CarClass.FormulaVee],                    typeof(CarDealer)));
        UnlockDealer(Dealers.GetDealer(Cars.CarBrand.Volkswagen.ToString(),             typeof(CarDealer)));
        UnlockDealer(Dealers.GetDealer(Cars.classToString[Cars.CarClass.CopaClassicB],         typeof(CarDealer)));
        UnlockDealer(Dealers.GetDealer(Cars.classToString[Cars.CarClass.FormulaVee],                    typeof(EntryPassDealer)));
        UnlockDealer(Dealers.GetDealer(Cars.CarBrand.Volkswagen.ToString(),             typeof(EntryPassDealer)));
        UnlockDealer(Dealers.GetDealer(Cars.classToString[Cars.CarClass.CopaClassicB],         typeof(EntryPassDealer)));

        // Base unlocks (EVENTS)

        foreach(Region.ClickableRegion region in Enum.GetValues(typeof(Region.ClickableRegion))){
            EventSeries newSeriesCopa   = new EventSeries("Copa Classic B for Dummies"  , EventSeries.SeriesTier.Rookie, region);
            EventSeries newSeriesVee    = new EventSeries("Formula Vee for Dummies"     , EventSeries.SeriesTier.Amateur, region);
            EventSeries newSeriesCopaFL   = new EventSeries("Copa Classic FL for Gods" , EventSeries.SeriesTier.Elite, region);

            Event.GenerateNewEvent(
                "Weekend Race - Copa Classic B",
                Event.EventType.Race,
                Event.EventDuration.Mini,
                newSeriesCopa,
                Tracks.GetCountries(region),
                new List<Cars.CarType>(),
                new List<Cars.CarClass>() { {Cars.CarClass.CopaClassicB} },
                new List<Cars.CarBrand>(),
                new List<string>(),
                useLaps:true
            );
            Event.GenerateNewEvent(
                "Weekend Race - Copa Classic B",
                Event.EventType.Race,
                Event.EventDuration.Long,
                newSeriesCopa,
                Tracks.GetCountries(region),
                new List<Cars.CarType>(),
                new List<Cars.CarClass>() { {Cars.CarClass.CopaClassicB} },
                new List<Cars.CarBrand>(),
                new List<string>(),
                false
            );
            Event.GenerateNewEvent(
                "Sunday Cup - Copa Classic B vs Formula Vee",
                Event.EventType.Championship,
                Event.EventDuration.Mini,
                newSeriesCopa,
                Tracks.GetCountries(region),
                new List<Cars.CarType>() { },
                new List<Cars.CarClass>() { {Cars.CarClass.CopaClassicB}, {Cars.CarClass.FormulaVee} },
                new List<Cars.CarBrand>() { {Cars.CarBrand.Chevrolet}, {Cars.CarBrand.Volkswagen} },
                new List<string>(),
                false
            );

            Event.GenerateNewEvent(
                "Weekend Race - Formula Vee",
                Event.EventType.Race,
                Event.EventDuration.Mini,
                newSeriesVee,
                Tracks.GetCountries(region),
                new List<Cars.CarType>(),
                new List<Cars.CarClass>() { {Cars.CarClass.FormulaVee} },
                new List<Cars.CarBrand>(),
                new List<string>(),
                true
            );
            Event.GenerateNewEvent(
                "Weekend Race - Formula Vee",
                Event.EventType.Race,
                Event.EventDuration.Long,
                newSeriesVee,
                Tracks.GetCountries(region),
                new List<Cars.CarType>(),
                new List<Cars.CarClass>() { {Cars.CarClass.FormulaVee} },
                new List<Cars.CarBrand>(),
                new List<string>(),
                false
            );
            Event.GenerateNewEvent(
                "Endurance Championship - Formula Vee",
                Event.EventType.Championship,
                Event.EventDuration.Endurance,
                newSeriesVee,
                Tracks.GetCountries(region),
                new List<Cars.CarType>(),
                new List<Cars.CarClass>() { {Cars.CarClass.FormulaVee} },
                new List<Cars.CarBrand>(),
                new List<string>(),
                false
            );
            Event.GenerateNewEvent(
                "Sunday Cup - Copa Classic FL",
                Event.EventType.Championship,
                Event.EventDuration.Average,
                newSeriesCopaFL,
                Tracks.GetCountries(region),
                new List<Cars.CarType>(),
                new List<Cars.CarClass>() { {Cars.CarClass.CopaClassicFL} },
                new List<Cars.CarBrand>(),
                new List<string>(),
                false
            );
        }
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

    public void GainMoney(int toGain){
        money += toGain;
    }

    public string GetPrintMoney(){
        return "$" + GetMoney().ToString("n0");
    }

    public int GetMoney(){
        return money;
    }

    public int GetLevel(){
        return level;
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

    private void SetMaxFameBasedOnLevel(){
        maxFame = 100 * level;
    }

    private void LevelUp(){
        fame -= maxFame;
        ++level;
        SetMaxFameBasedOnLevel();
    }
}