using UnityEngine;
using System.Collections.Generic;
using System;

public class Car : Purchasable
{
    public Cars.CarType         type;
    public List<Cars.CarClass>  classes;
    public Cars.CarBrand        brand;

    public Car(string nameParam, Cars.CarType typeParam, List<Cars.CarClass> classesParam, Cars.CarBrand brandParam, int priceParam, int sellPriceParam = -1) : base(nameParam, priceParam, sellPriceParam)
    {
        type        = typeParam;
        classes     = classesParam;
        brand       = brandParam;
    }

    public override Sprite GetSprite(){
        string imageName = name.Replace(" ", "") + "_Car";
        return Resources.Load<Sprite>("Images/" + imageName);
    }

    public override string GetPrintName(){
        return Cars.brandToString[brand] == "" ?  name : Cars.brandToString[brand] + " " + name;
    }

    public override string GetInfoBlurb()
    {
        return "Make/Model: " + GetPrintName() + "\nType: " + GetPrintType() + "\nClass: " + GetPrintClasses();
    }

    public string GetPrintClasses(){
        string classesString = "";

        foreach(Cars.CarClass carClass in classes){
            classesString += Cars.classToString[carClass] + ", ";
        }
        if(classesString.Length > 0){
            // Strip the last two characters
            classesString = classesString.Substring(0, classesString.Length - 2);
        }

        return classesString;
    }

    public string GetPrintType(){
        return Cars.typeToString[type];
    }

    // Override the equals method
    public override bool Equals(object obj)
    {
        if(obj == null || GetType() != obj.GetType()){
            return false;
        }

        Car carToCompare = (Car)obj;
        return carToCompare.name == name;
    }
    public override int GetHashCode()
    {
        return name.GetHashCode();
    }
}

public static class Cars
{
    // Names are unique
    public const string                     VEE_2011_NAME                   = "Formula Vee 2011";
    public const string                     VEE_FIN_NAME                    = "Formula Vee Fin";
    public const string                     COPA_B_CHEVETTE_NAME            = "Chevette CCB";
    public const string                     COPA_B_GOL_NAME                 = "Gol CCB";
    public const string                     COPA_B_PASSAT_NAME              = "Passat CCB";
    public const string                     COPA_B_GTE_NAME                 = "GTE CCB";
    public const string                     COPA_B_UNO_NAME                 = "Uno CCB";
    public const string                     COPA_B_COOPER_NAME              = "Cooper S 1965 B";

    // Holds ALL cars in the database
    private static List<Car>                                cars {get; set;}
    // Will match unique name to car
    private static Dictionary<string, Car>                  nameToCar {get; set;}
    // Will match type to list of cars who are that type
    private static Dictionary<CarType, List<Car>>           typeToCars {get; set;}
    // Will match class to list of cars who are that class
    private static Dictionary<CarClass, List<Car>>          classToCars {get; set;}
    // Will match brand to list of cars who are that brand
    private static Dictionary<CarBrand, List<Car>>          brandToCars {get; set;}

    static Cars(){
        cars         = new List<Car>();
        nameToCar    = new Dictionary<string, Car>();
        typeToCars   = new Dictionary<CarType, List<Car>>();
        classToCars  = new Dictionary<CarClass, List<Car>>();
        brandToCars  = new Dictionary<CarBrand, List<Car>>();

        // Init our dicts with empty lists
        foreach(CarType carType in Enum.GetValues(typeof(CarType))){
            typeToCars[carType] = new List<Car>();
        }
        foreach(CarClass carClass in Enum.GetValues(typeof(CarClass))){
            classToCars[carClass] = new List<Car>();
        }
        foreach(CarBrand carBrand in Enum.GetValues(typeof(CarBrand))){
            brandToCars[carBrand] = new List<Car>();
        }

        // Init, add ALL of the game's cars here
        AddNewCar(new Car(VEE_2011_NAME,            CarType.OpenWheeler,    new List<CarClass> {CarClass.FormulaVeeBrasil},     CarBrand.Volkswagen,        6000));
        AddNewCar(new Car(VEE_FIN_NAME,             CarType.OpenWheeler,    new List<CarClass> {CarClass.FormulaVeeBrasil},     CarBrand.Volkswagen,        7500));
        AddNewCar(new Car(COPA_B_CHEVETTE_NAME,     CarType.Touring,        new List<CarClass> {CarClass.CopaClassicB},         CarBrand.Chevrolet,         6500));
        AddNewCar(new Car(COPA_B_COOPER_NAME,       CarType.Touring,        new List<CarClass> {CarClass.CopaClassicB},         CarBrand.Mini,              7000));
        AddNewCar(new Car(COPA_B_GOL_NAME,          CarType.Touring,        new List<CarClass> {CarClass.CopaClassicB},         CarBrand.Volkswagen,        7500));
        AddNewCar(new Car(COPA_B_GTE_NAME,          CarType.Touring,        new List<CarClass> {CarClass.CopaClassicB},         CarBrand.Puma,              8000));
        AddNewCar(new Car(COPA_B_PASSAT_NAME,       CarType.Touring,        new List<CarClass> {CarClass.CopaClassicB},         CarBrand.Volkswagen,        8500));
        AddNewCar(new Car(COPA_B_UNO_NAME,          CarType.Touring,        new List<CarClass> {CarClass.CopaClassicB},         CarBrand.Fiat,              9000));
    }

    public static void AddNewCar(Car carToAdd)
    {
        CarType         carType                         = carToAdd.type;
        List<CarClass>  carClasses                      = carToAdd.classes;
        CarBrand        carBrand                        = carToAdd.brand;
        string          carName                         = carToAdd.name;
        cars.Add(carToAdd);
        nameToCar[carName]                              = carToAdd;

        // For each car class
        foreach(CarClass carClass in carClasses){
            // Add the car to the list
            classToCars[carClass].Add(carToAdd);
        }

        // For the car type
        // Add the car to the list
        typeToCars[carType].Add(carToAdd);

        // For the car brand
        // Add the car to the list
        brandToCars[carBrand].Add(carToAdd);
    }

    public static Car GetCar(string carName){
        return nameToCar[carName];
    }

    public static List<Car> GetCarsWithType(CarType carType){
        return typeToCars[carType];
    }

    public static List<Car> GetCarsWithBrand(CarBrand carBrand){
        return brandToCars[carBrand];
    }

    public static List<Car> GetCarsWithClass(CarClass carClass){
        return classToCars[carClass];
    }

    public enum CarType
    {
        OpenWheeler,
        Touring,
        Sportscar,
        GranTurismo,
        Prototype,
        Club,
        Truck,
        None
    }

    public static Dictionary<CarType, string> typeToString = new Dictionary<CarType, string>
    {
        {CarType.None,                      ""},
        {CarType.OpenWheeler,               "Open Wheeler"},
        {CarType.Touring,                   "Touring"},
        {CarType.Sportscar,                 "Sportscar"},
        {CarType.GranTurismo,               "Gran Turismo"},
        {CarType.Prototype,                 "Prototype"},
        {CarType.Club,                      "Club"},
        {CarType.Truck,                     "Truck"}
    };

    public enum CarClass
    {
        None,
        FormulaVeeBrasil,
        CopaClassicB
    }

    public static Dictionary<CarClass, string> classToString = new Dictionary<CarClass, string>
    {
        {CarClass.None,             ""},
        {CarClass.FormulaVeeBrasil, "Formula Vee - Brasil"},
        {CarClass.CopaClassicB,     "Copa Classic - B"}
    };

    public enum CarBrand
    {
        None,
        Ford,
        Volkswagen,
        Puma,
        Fiat,
        Mini,
        Chevrolet
    }

    public static Dictionary<CarBrand, string> brandToString = new Dictionary<CarBrand, string>
    {
        {CarBrand.None,             ""},
        {CarBrand.Chevrolet,        "Chevrolet"},
        {CarBrand.Fiat,             "Fiat"},
        {CarBrand.Ford,             "Ford"},
        {CarBrand.Mini,             "Mini"},
        {CarBrand.Puma,             "Puma"},
        {CarBrand.Volkswagen,       "Volkswagen"},
    };
}