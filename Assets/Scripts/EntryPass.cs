using UnityEngine;
using System.Collections.Generic;
using System;

public class EntryPass : Purchasable
{
    public Cars.CarType                     carType;
    public Cars.CarClass                    carClass;
    public Cars.CarBrand                    carBrand;
    public EventSeriesManager.SeriesTier    tier;

    public EntryPass(string nameParam, Cars.CarType typeParam, Cars.CarClass classParam, Cars.CarBrand brandParam, EventSeriesManager.SeriesTier tierParam, int priceParam, int sellPriceParam = -1) : base(nameParam, priceParam, sellPriceParam)
    {
        carType         = typeParam;
        carClass        = classParam;
        carBrand        = brandParam;
        tier            = tierParam;
    }

    public bool WorksWithCar(Car car){
        if(carType == car.type){
            return true;
        }

        if(carBrand == car.brand){
            return true;
        }

        foreach(Cars.CarClass carsClass in car.classes){
            if(carsClass == carClass){
                return true;
            }
        }

        return false;
    }

    public override Sprite GetSprite(){
        //string imageName = name.Replace(" ", "") + "_EntryPass";
        string imageName = Cars.typeToString[carType] + Cars.classToString[carClass] + carBrand.ToString() + "_EntryPass";
        Debug.Log("Looking for image: " + imageName);
        return Resources.Load<Sprite>("Images/" + imageName);
    }

    public override string GetPrintName(){
        return Cars.typeToString[carType] + Cars.classToString[carClass] + carBrand.ToString() + " - " + EventSeriesManager.tierToString[tier] + " Entry Pass";
    }

    public override string GetInfoBlurb()
    {
        return "Name: " + GetPrintName() + "\nType: " + GetPrintType() + "\nClass: " + GetPrintClasses();
    }

    public string GetPrintClasses(){
        return Cars.classToString[carClass];
    }

    public string GetPrintType(){
        return Cars.typeToString[carType];
    }

    // Override the equals method
    public override bool Equals(object obj)
    {
        if(obj == null || GetType() != obj.GetType()){
            return false;
        }

        EntryPass passToCompare = (EntryPass)obj;
        return passToCompare.name == name && passToCompare.tier == tier;
    }
    public override int GetHashCode()
    {
        return name.GetHashCode() + tier.GetHashCode();
    }
}

public static class EntryPasses
{
    public const int                                                CAR_BASE_COST       = 1500;
    public const int                                                CAR_COST_PER_TIER   = 2500;

    public const int                                                TYPE_BASE_COST      = 10000;
    public const int                                                TYPE_COST_PER_TIER  = 13500;

    public const int                                                CLASS_BASE_COST     = 5000;
    public const int                                                CLASS_COST_PER_TIER = 8500;

    public const int                                                BRAND_BASE_COST     = 8500;
    public const int                                                BRAND_COST_PER_TIER = 11750;

    // Holds ALL entry passes in the database
    private static List<EntryPass>                                  entryPasses {get; set;}
    // Will match unique name to car
    private static Dictionary<string, EntryPass>                    nameToEntryPass {get; set;}
    // Will match class to list of entry passes who are for that class
    private static Dictionary<Cars.CarClass, List<EntryPass>>       classToEntryPasses {get; set;}
    // Will match brand to list of passes who are for that brand
    private static Dictionary<Cars.CarBrand, List<EntryPass>>       brandToEntryPasses {get; set;}
    // Will match car type to list of passes who are for that type
    private static Dictionary<Cars.CarType, List<EntryPass>>        typeToEntryPasses {get; set;}

    static EntryPasses(){
        entryPasses         = new List<EntryPass>();
        nameToEntryPass     = new Dictionary<string, EntryPass>();
        typeToEntryPasses   = new Dictionary<Cars.CarType, List<EntryPass>>();
        classToEntryPasses  = new Dictionary<Cars.CarClass, List<EntryPass>>();
        brandToEntryPasses  = new Dictionary<Cars.CarBrand, List<EntryPass>>();

        // Init our dicts with empty lists
        foreach(Cars.CarType carType in Enum.GetValues(typeof(Cars.CarType))){
            typeToEntryPasses[carType] = new List<EntryPass>();
        }
        foreach(Cars.CarClass carClass in Enum.GetValues(typeof(Cars.CarClass))){
            classToEntryPasses[carClass] = new List<EntryPass>();
        }
        foreach(Cars.CarBrand carBrand in Enum.GetValues(typeof(Cars.CarBrand))){
            brandToEntryPasses[carBrand] = new List<EntryPass>();
        }

        InitializeEntryPasses();
    }

    public static List<EntryPass> FilterEntryPasses(List<EntryPass> entryPasses, EventSeriesManager.SeriesTier tier){

        List<EntryPass> filteredPasses = new List<EntryPass>();

        // Filter the given entry passes by their series tier
        foreach(EntryPass entryPass in entryPasses){
            if(entryPass.tier == tier){
                filteredPasses.Add(entryPass);
            }
        }

        return filteredPasses;
    }

    public static void AddNewEntryPass(EntryPass passToAdd)
    {
        Cars.CarClass   carClass        = passToAdd.carClass;
        Cars.CarBrand   carBrand        = passToAdd.carBrand;
        Cars.CarType    carType         = passToAdd.carType;
        string          passName        = passToAdd.name;

        entryPasses.Add(passToAdd);
        nameToEntryPass[passName]       = passToAdd;

        // For the car type
        // Add the entry pass to the list
        typeToEntryPasses[carType].Add(passToAdd);

        // For the car class
        // Add the entry pass to the list
        classToEntryPasses[carClass].Add(passToAdd);

        // For the car brand
        // Add the entry pass to the list
        brandToEntryPasses[carBrand].Add(passToAdd);
    }

    public static EntryPass GetEntryPass(string passName){
        return nameToEntryPass[passName];
    }

    public static List<EntryPass> GetPassesWithType(Cars.CarType carType){
        return typeToEntryPasses[carType];
    }

    public static List<EntryPass> GetPassesWithBrand(Cars.CarBrand carBrand){
        return brandToEntryPasses[carBrand];
    }

    public static List<EntryPass> GetPassesWithClass(Cars.CarClass carClass){
        return classToEntryPasses[carClass];
    }

    private static void InitializeEntryPasses(){
        // Init, add ALL of the game's entry passes here
        // For now, simply add passes for each tier, for each car brand, car class and car type
        foreach(EventSeriesManager.SeriesTier passTier in Enum.GetValues(typeof(EventSeriesManager.SeriesTier))){
            foreach(Cars.CarType carType in Enum.GetValues(typeof(Cars.CarType))){
                AddNewEntryPass(new EntryPass(
                    Cars.typeToString[carType] + EventSeriesManager.tierToString[passTier],
                    carType,
                    Cars.CarClass.None,
                    Cars.CarBrand.None,
                    passTier,
                    TYPE_BASE_COST + (TYPE_COST_PER_TIER * (int)passTier)));
            }

            foreach(Cars.CarClass carClass in Enum.GetValues(typeof(Cars.CarClass))){
                AddNewEntryPass(new EntryPass(
                    Cars.classToString[carClass] + EventSeriesManager.tierToString[passTier],
                    Cars.CarType.None,
                    carClass,
                    Cars.CarBrand.None,
                    passTier,
                    CLASS_BASE_COST + (CLASS_COST_PER_TIER * (int)passTier)));
            }

            foreach(Cars.CarBrand carBrand in Enum.GetValues(typeof(Cars.CarBrand))){
                AddNewEntryPass(new EntryPass(
                    carBrand.ToString() + EventSeriesManager.tierToString[passTier],
                    Cars.CarType.None,
                    Cars.CarClass.None,
                    carBrand,
                    passTier,
                    BRAND_BASE_COST + (BRAND_COST_PER_TIER * (int)passTier)));
            }
        }
    }
}