using UnityEngine;
using System.Collections.Generic;
using System;

public class Dealer
{
    // Name is unique
    public string                   name;
    public List<Cars.CarClass>      carClasses;
    public List<Cars.CarBrand>      carBrands;
    public List<Car>                cars;

    public Dealer(string nameParam, List<Cars.CarClass> carClassesParam, List<Cars.CarBrand> carBrandsParam){
        name            = nameParam;
        carClasses      = carClassesParam;
        carBrands       = carBrandsParam;

        // Add in all the cars that match the classes/brands of this dealer
        cars            = new List<Car>();
        foreach(Cars.CarClass carClass in carClasses){
            // For each class this dealer sells, add cars that are this class
            List<Car> carsToAdd  = carClass == Cars.CarClass.None ? new List<Car>() : Cars.GetCarsWithClass(carClass);
            foreach(Car car in carsToAdd){
                if(!cars.Contains(car)){
                    cars.Add(car);
                }
            }
        }

        foreach(Cars.CarBrand carBrand in carBrands){
            // For each brand this dealer sells, add cars that are this brand
            List<Car> carsToAdd  = carBrand == Cars.CarBrand.None ? new List<Car>() : Cars.GetCarsWithBrand(carBrand);
            foreach(Car car in carsToAdd){
                if(!cars.Contains(car)){
                    cars.Add(car);
                }
            }
        }
    }

    public Sprite GetSprite(){
        string imageName = name.Replace(" ", "") + "_Dealer";
        return Resources.Load<Sprite>("Images/" + imageName);
    }
}

public static class Dealers
{
    public const string                                         VEE_NAME            = "Formula Vee";
    public const string                                         COPA_CLASSIC_B_NAME = "Copa Classic - B";
    public const string                                         FORD_NAME           = "Ford";
    public const string                                         VOLKSWAGEN_NAME     = "Volkswagen";
    public const string                                         CHEVROLET_NAME      = "Chevrolet";
    public const string                                         PUMA_NAME           = "Puma";
    public const string                                         FIAT_NAME           = "Fiat";
    public const string                                         MINI_NAME           = "Mini";

    // Holds ALL dealers in the database
    private static List<Dealer>                                 dealers {get; set;}
    // Will match unique name to dealer
    private static Dictionary<string, Dealer>                   nameToDealer {get; set;}
    // Will match class to list of dealers who sell that class
    private static Dictionary<Cars.CarClass, List<Dealer>>      classToDealers {get; set;}
    // Will match brand to list of dealers who sell that brand
    private static Dictionary<Cars.CarBrand, List<Dealer>>      brandToDealers {get; set;}

    static Dealers(){
        dealers         = new List<Dealer>();
        nameToDealer    = new Dictionary<string, Dealer>();
        classToDealers  = new Dictionary<Cars.CarClass, List<Dealer>>();
        brandToDealers  = new Dictionary<Cars.CarBrand, List<Dealer>>();

        // Init our dicts with empty lists
        foreach(Cars.CarClass carClass in Enum.GetValues(typeof(Cars.CarClass))){
            classToDealers[carClass] = new List<Dealer>();
        }
        foreach(Cars.CarBrand carBrand in Enum.GetValues(typeof(Cars.CarBrand))){
            brandToDealers[carBrand] = new List<Dealer>();
        }

        // Init, add ALL of the game's dealers here
        // Dealers may sell brands of cars, classes of cars, or specific classes of brands of cars
        AddNewDealer(new Dealer(VEE_NAME,               new List<Cars.CarClass> {Cars.CarClass.FormulaVeeBrasil},   new List<Cars.CarBrand> {Cars.CarBrand.None}));
        AddNewDealer(new Dealer(COPA_CLASSIC_B_NAME,    new List<Cars.CarClass> {Cars.CarClass.CopaClassicB},       new List<Cars.CarBrand> {Cars.CarBrand.None}));
        AddNewDealer(new Dealer(FORD_NAME,              new List<Cars.CarClass> {Cars.CarClass.None},               new List<Cars.CarBrand> {Cars.CarBrand.Ford}));
        AddNewDealer(new Dealer(VOLKSWAGEN_NAME,        new List<Cars.CarClass> {Cars.CarClass.None},               new List<Cars.CarBrand> {Cars.CarBrand.Volkswagen}));
        AddNewDealer(new Dealer(CHEVROLET_NAME,         new List<Cars.CarClass> {Cars.CarClass.None},               new List<Cars.CarBrand> {Cars.CarBrand.Chevrolet}));
        AddNewDealer(new Dealer(PUMA_NAME,              new List<Cars.CarClass> {Cars.CarClass.None},               new List<Cars.CarBrand> {Cars.CarBrand.Puma}));
        AddNewDealer(new Dealer(FIAT_NAME,              new List<Cars.CarClass> {Cars.CarClass.None},               new List<Cars.CarBrand> {Cars.CarBrand.Fiat}));
        AddNewDealer(new Dealer(MINI_NAME,              new List<Cars.CarClass> {Cars.CarClass.None},               new List<Cars.CarBrand> {Cars.CarBrand.Mini}));
    }

    // Adds a new dealer to the 'database'
    public static void AddNewDealer(Dealer dealerToAdd){
        List<Cars.CarClass> dealerClasses                       = dealerToAdd.carClasses;
        List<Cars.CarBrand> dealerBrands                        = dealerToAdd.carBrands;
        string              dealerName                          = dealerToAdd.name;
        dealers.Add(dealerToAdd);
        nameToDealer[dealerName]                                = dealerToAdd;

        // For each car class
        foreach(Cars.CarClass carClass in dealerClasses){
            // Add the dealer to the list
            classToDealers[carClass].Add(dealerToAdd);
        }

        // For each car brand
        foreach(Cars.CarBrand carBrand in dealerBrands){
            // Add the dealer to the list
            brandToDealers[carBrand].Add(dealerToAdd);
        }
    }

    public static Dealer GetDealer(string name){
        return nameToDealer[name];
    }

    public static List<Dealer> GetDealers(Cars.CarClass dealerClass){
        return classToDealers[dealerClass];
    }
}