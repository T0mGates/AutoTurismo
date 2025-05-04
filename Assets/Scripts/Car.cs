using UnityEngine;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;

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
        return Resources.Load<Sprite>("Images/Cars/" + imageName);
    }

    public override string GetPrintName(){
        return brand.ToString() == "" ?  name : brand.ToString() + " " + name;
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

    public const string                     COPA_UNO                                = "Uno";

    // COPA CLASSIC B
    public const string                     COPA_B_CHEVETTE_NAME                    = "Chevette CCB";
    public const string                     COPA_B_GOL_NAME                         = "Gol CCB";
    public const string                     COPA_B_PASSAT_NAME                      = "Passat CCB";
    public const string                     COPA_B_GTE_NAME                         = "GTE CCB";
    public const string                     COPA_B_UNO_NAME                         = "Uno CCB";
    public const string                     COPA_B_COOPER_NAME                      = "Cooper S 1965 B";

    // COPA CLASSIC FL
    public const string                     COPA_FL_FUSCA_NAME                      = "Fusca CCFL";
    public const string                     COPA_FL_GOL_NAME                        = "Gol CCFL";
    public const string                     COPA_FL_PASSAT_NAME                     = "Passat CCFL";
    public const string                     COPA_FL_GTB_NAME                        = "GTB CCFL";

    // COPA TRUCK
    public const string                     COPA_TRUCK_STRALIS_NAME                 = "Stralis";
    public const string                     COPA_TRUCK_TGX_NAME                     = "TGX";
    public const string                     COPA_TRUCK_CONSTELLATION_NAME           = "Constellation";
    public const string                     COPA_TRUCK_ACTROS_NAME                  = "Benz Actros-2651";
    public const string                     COPA_TRUCK_TRUCK                        = "Truck";

    // Street: TSI
    public const string                     TSI_POLO_NAME                           = "Polo";
    public const string                     TSI_POLO_GTS_NAME                       = "Polo GTS";
    public const string                     TSI_VIRTUS_NAME                         = "Virtus";
    public const string                     TSI_VIRTUS_GTS_NAME                     = "Virtus GTS";

    // Street
    public const string                     STREET_CAMARO_NAME                      = "Camaro SS";

    public const string                     STREET_LANCER_R_NAME                    = "Lancer R";
    public const string                     STREET_LANCER_RS_NAME                   = "Lancer RS";

    // Street: Super Cars
    public const string                     SUPER_F1_NAME                           = "F1 LM";
    public const string                     SUPER_GTR_NAME                          = "GTR";

    // GT5
    public const string                     GTO_G55_NAME                            = "G55 GT3";
    // GT5
    public const string                     GTO_GTR_NAME                            = "GTR Race";

    // GT5
    public const string                     GT5_G40_NAME                            = "G40";
    public const string                     GT5_P052_NAME                           = "P052";

    // GT4
    public const string                     GT4_M4_NAME                             = "M4 GT4";
    public const string                     GT4_CAMARO_NAME                         = "Camaro GT4.R";
    public const string                     GT4_G55_NAME                            = "G55 GT4";
    public const string                     GT4_570S_NAME                           = "570S";
    public const string                     GT4_AMG_NAME                            = "AMG GT4";
    public const string                     GT4_CAYMAN_NAME                         = "Cayman GT4 CS MR";
    public const string                     GT4_R8_NAME                             = "R8 LMS GT4";

    // GT3
    public const string                     GT3_M6_NAME                             = "M6 GT3";
    public const string                     GT3_720S_NAME                           = "720S";
    public const string                     GT3_AMG_NAME                            = "AMG GT3";
    public const string                     GT3_GTR_NAME                            = "GT-R Nismo GT3";
    public const string                     GT3_911_NAME                            = "911 GT3 R";
    public const string                     GT3_R8_NAME                             = "R8 LMS GT3";

    // TODO: havent added these in yet
    public const string                     GT3_992_NAME                            = "992 GT3 R";
    public const string                     GT3_AMG_EVO_NAME                        = "AMG GT3 Evo";
    public const string                     GT3_720S_EVO_NAME                       = "720S EVO";
    public const string                     GT3_M4_NAME                             = "M4";

    // Carrera (similar to GT3)
    public const string                     CARRERA_911_38_NAME                     = "911 GT3 Cup 3.8";
    public const string                     CARRERA_911_40_NAME                     = "911 GT3 Cup 4.0";

    // GT1
    public const string                     GT1_GTR_NAME                            = "F1 GTR Longtail";
    public const string                     GT1_CLK_NAME                            = "Benz CLK LM";
    public const string                     GT1_R390_NAME                           = "R390 GT1";
    public const string                     GT1_911_NAME                            = "911 GT1";


    // Karts
    public const string                     KART_125CC_NAME                         = "125cc 2STROKE";
    public const string                     KART_GX390_NAME                         = "GX390 Race";
    public const string                     KART_RENTAL_NAME                        = "GX390 Rental";
    public const string                     KART_SHIFTER_NAME                       = "125cc Shifter";
    public const string                     KART_SUPER_NAME                         = "Super 125cc";

    // FORMULA VEE
    public const string                     VEE_2011_NAME                           = "Formula Vee 2011";
    public const string                     VEE_FIN_NAME                            = "Formula Vee Fin";

    // Formula Trainer
    public const string                     F_TRAINER_NAME                          = "Trainer";

    // Formula Trainer Advanced
    public const string                     F_TRAINER_ADVANCED_NAME                 = "Trainer Advanced";

    // Formula Inter
    public const string                     F_INTER_NAME                            = "Inter MG-15";

    // F3
    public const string                     F3_F301_NAME                            = "F301";
    public const string                     F3_F309_NAME                            = "F309";

    // Formula Reiza
    public const string                     F_REIZA_NAME                            = "Reiza";

    // Formula Ultimate Gen 2
    public const string                     F_GEN2_NAME                             = "Ultimate Gen 2";


    // Prototypes

    // Group C
    public const string                     GROUP_C_CORVETTE_NAME                   = "Corvette GTP";
    public const string                     GROUP_C_R89C_NAME                       = "R89C";
    public const string                     GROUP_C_962C_NAME                       = "962C";
    public const string                     GROUP_C_C9_NAME                         = "C9";

    // P4
    public const string                     P4_S2000_NAME                           = "S2000";
    public const string                     P4_MRX_DURATEC_NAME                     = "MRX Duratec P4";

    // P3
    public const string                     P3_MRX_HONDA_NAME                       = "MRX Honda P3";
    public const string                     P3_MRX_DURATEC_NAME                     = "MRX Duratec Turbo P3";
    public const string                     P3_ROCO_NAME                            = "001";

    // P2
    public const string                     P2_MRX_DURATEC_NAME                     = "MRX Duratec Turbo F2";
    public const string                     P2_SIGMA_NAME                           = "P1";

    // P1 Gen 2
    public const string                     P1_G58_NAME                             = "G58 Gen2";
    public const string                     P1_JS_NAME                              = "JS P320";
    public const string                     P1_AJR_CHEVROLET_NAME                   = "AJR Gen2 Chevrolet";
    public const string                     P1_AJR_HONDA_NAME                       = "AJR Gen2 Honda";
    public const string                     P1_AJR_NISSAN_NAME                      = "AJR Gen2 Nissan";
    public const string                     P1_G5_NAME                              = "P1 G5";

    // DLC
    // ==============================================================================================================

    // Supercars 1
    public const string                     SUPER_CORVETTE_C8_NAME                  = "Corvette C8 Z06";
    public const string                     HYPER_BT62_NAME                         = "BT62";
    public const string                     HYPER_SENNA_NAME                        = "Senna";

    // Racin' USA 1
    public const string                     GTE_C8R_NAME                            = "Corvette C8.R";
    public const string                     GTE_M8_NAME                             = "M8 GTE";
    public const string                     GTE_911_NAME                            = "911 RSR-19";
    public const string                     DPI_CADILLAC                            = "DPi V.R";


    // Holds ALL cars in the database
    private static  List<Car>                                               cars {get; set;}
    // Will match unique name to car
    private static  Dictionary<string, Car>                                 nameToCar {get; set;}
    // Will match type to list of cars who are that type
    private static  Dictionary<CarType, List<Car>>                          typeToCars {get; set;}
    // Will match class to list of cars who are that class
    private static  Dictionary<CarClass, List<Car>>                         classToCars {get; set;}
    // Will match brand to list of cars who are that brand
    private static  Dictionary<CarBrand, List<Car>>                         brandToCars {get; set;}

    // Holds tiers to classes
    private static  Dictionary<EventSeries.SeriesTier, List<CarClass>>      tierToClasses;
    public static   Dictionary<SettingsMenu.DLC, List<Car>>                 DLCCars;

    private static void InitializeCars()
    {
        // Init, add ALL of the game's cars here

        // Karts
        AddNewCar(new Car(KART_RENTAL_NAME,                 CarType.OpenWheeler,    new List<CarClass> {CarClass.KartRental},           CarBrand.Kart,              4000));
        AddNewCar(new Car(KART_GX390_NAME,                  CarType.OpenWheeler,    new List<CarClass> {CarClass.KartGX390},            CarBrand.Kart,              5000));
        AddNewCar(new Car(KART_125CC_NAME,                  CarType.OpenWheeler,    new List<CarClass> {CarClass.Kart125cc},            CarBrand.Kart,              5500));
        AddNewCar(new Car(KART_SHIFTER_NAME,                CarType.OpenWheeler,    new List<CarClass> {CarClass.KartShifter},          CarBrand.Kart,              6000));
        AddNewCar(new Car(KART_SUPER_NAME,                  CarType.OpenWheeler,    new List<CarClass> {CarClass.KartSuper},            CarBrand.Kart,              8000));

        // FORMULA VEE
        AddNewCar(new Car(VEE_2011_NAME,                    CarType.OpenWheeler,    new List<CarClass> {CarClass.FormulaVee},           CarBrand.Volkswagen,        6000));
        AddNewCar(new Car(VEE_FIN_NAME,                     CarType.OpenWheeler,    new List<CarClass> {CarClass.FormulaVee},           CarBrand.Volkswagen,        7500));

        AddNewCar(new Car(COPA_UNO,                         CarType.Touring,        new List<CarClass> {CarClass.CopaUno},              CarBrand.Fiat,              7000));

        // COPA CLASSIC B
        AddNewCar(new Car(COPA_B_CHEVETTE_NAME,             CarType.Touring,        new List<CarClass> {CarClass.CopaClassicB},         CarBrand.Chevrolet,         6500));
        AddNewCar(new Car(COPA_B_COOPER_NAME,               CarType.Touring,        new List<CarClass> {CarClass.CopaClassicB},         CarBrand.Mini,              7000));
        AddNewCar(new Car(COPA_B_GOL_NAME,                  CarType.Touring,        new List<CarClass> {CarClass.CopaClassicB},         CarBrand.Volkswagen,        7500));
        AddNewCar(new Car(COPA_B_GTE_NAME,                  CarType.Touring,        new List<CarClass> {CarClass.CopaClassicB},         CarBrand.Puma,              8000));
        AddNewCar(new Car(COPA_B_PASSAT_NAME,               CarType.Touring,        new List<CarClass> {CarClass.CopaClassicB},         CarBrand.Volkswagen,        8500));
        AddNewCar(new Car(COPA_B_UNO_NAME,                  CarType.Touring,        new List<CarClass> {CarClass.CopaClassicB},         CarBrand.Fiat,              7000));

        // COPA CLASSIC FL
        AddNewCar(new Car(COPA_FL_FUSCA_NAME,               CarType.Touring,        new List<CarClass> {CarClass.CopaClassicFL},        CarBrand.Volkswagen,        9000));
        AddNewCar(new Car(COPA_FL_GOL_NAME,                 CarType.Touring,        new List<CarClass> {CarClass.CopaClassicFL},        CarBrand.Volkswagen,        9500));
        AddNewCar(new Car(COPA_FL_PASSAT_NAME,              CarType.Touring,        new List<CarClass> {CarClass.CopaClassicFL},        CarBrand.Volkswagen,        10000));
        AddNewCar(new Car(COPA_FL_GTB_NAME,                 CarType.Touring,        new List<CarClass> {CarClass.CopaClassicFL},        CarBrand.Puma,              10500));

        // COPA TRUCK
        AddNewCar(new Car(COPA_TRUCK_STRALIS_NAME,          CarType.Truck,          new List<CarClass> {CarClass.CopaTruck},            CarBrand.Iveco,             7000));
        AddNewCar(new Car(COPA_TRUCK_TGX_NAME,              CarType.Truck,          new List<CarClass> {CarClass.CopaTruck},            CarBrand.MAN,               7500));
        AddNewCar(new Car(COPA_TRUCK_ACTROS_NAME,           CarType.Truck,          new List<CarClass> {CarClass.CopaTruck},            CarBrand.Mercedes,          8000));
        AddNewCar(new Car(COPA_TRUCK_CONSTELLATION_NAME,    CarType.Truck,          new List<CarClass> {CarClass.CopaTruck},            CarBrand.Volkswagen,        8500));
        AddNewCar(new Car(COPA_TRUCK_TRUCK,                 CarType.Truck,          new List<CarClass> {CarClass.CopaTruck},            CarBrand.Vulkan,            9000));

        // TSI
        AddNewCar(new Car(TSI_POLO_NAME,                    CarType.Road,           new List<CarClass> {CarClass.TSI},                  CarBrand.Volkswagen,        7500));
        AddNewCar(new Car(TSI_POLO_GTS_NAME,                CarType.Road,           new List<CarClass> {CarClass.TSI},                  CarBrand.Volkswagen,        8000));
        AddNewCar(new Car(TSI_VIRTUS_NAME,                  CarType.Road,           new List<CarClass> {CarClass.TSI},                  CarBrand.Volkswagen,        8500));
        AddNewCar(new Car(TSI_VIRTUS_GTS_NAME,              CarType.Road,           new List<CarClass> {CarClass.TSI},                  CarBrand.Volkswagen,        9000));

        // Street
        AddNewCar(new Car(STREET_CAMARO_NAME,               CarType.Road,           new List<CarClass> {CarClass.StreetCars},           CarBrand.Chevrolet,         12000));
        AddNewCar(new Car(STREET_LANCER_R_NAME,             CarType.Road,           new List<CarClass> {CarClass.Lancer},               CarBrand.Mitsubishi,        13000));
        AddNewCar(new Car(STREET_LANCER_RS_NAME,            CarType.Road,           new List<CarClass> {CarClass.Lancer},               CarBrand.Mitsubishi,        14000));

        // Street: Super cars
        AddNewCar(new Car(SUPER_F1_NAME,                    CarType.Road,           new List<CarClass> {CarClass.SuperCars},            CarBrand.McLaren,           16500));
        AddNewCar(new Car(SUPER_GTR_NAME,                   CarType.Road,           new List<CarClass> {CarClass.SuperCars},            CarBrand.Ultima,            17000));

        // GT Open
        AddNewCar(new Car(GTO_G55_NAME,                     CarType.GT,             new List<CarClass> {CarClass.GTOpen},               CarBrand.Ginetta,           16000));
        AddNewCar(new Car(GTO_GTR_NAME,                     CarType.GT,             new List<CarClass> {CarClass.GTOpen},               CarBrand.Ultima,            20000));

        // GT5
        AddNewCar(new Car(GT5_G40_NAME,                     CarType.GT,             new List<CarClass> {CarClass.GT5},                  CarBrand.Ginetta,           8500));
        AddNewCar(new Car(GT5_P052_NAME,                    CarType.GT,             new List<CarClass> {CarClass.GT5},                  CarBrand.Puma,              9000));

        // GT4
        AddNewCar(new Car(GT4_M4_NAME,                      CarType.GT,             new List<CarClass> {CarClass.GT4},                  CarBrand.BMW,               12000));
        AddNewCar(new Car(GT4_CAMARO_NAME,                  CarType.GT,             new List<CarClass> {CarClass.GT4},                  CarBrand.Chevrolet,         12000));
        AddNewCar(new Car(GT4_G55_NAME,                     CarType.GT,             new List<CarClass> {CarClass.GT4},                  CarBrand.Ginetta,           12500));
        AddNewCar(new Car(GT4_570S_NAME,                    CarType.GT,             new List<CarClass> {CarClass.GT4},                  CarBrand.McLaren,           12500));
        AddNewCar(new Car(GT4_AMG_NAME,                     CarType.GT,             new List<CarClass> {CarClass.GT4},                  CarBrand.Mercedes,          13000));
        AddNewCar(new Car(GT4_CAYMAN_NAME,                  CarType.GT,             new List<CarClass> {CarClass.GT4},                  CarBrand.Porsche,           13000));
        AddNewCar(new Car(GT4_R8_NAME,                      CarType.GT,             new List<CarClass> {CarClass.GT4},                  CarBrand.Audi,              13000));

        // GT3
        AddNewCar(new Car(GT3_M6_NAME,                      CarType.GT,             new List<CarClass> {CarClass.GT3},                  CarBrand.BMW,               16000));
        AddNewCar(new Car(GT3_R8_NAME,                      CarType.GT,             new List<CarClass> {CarClass.GT3},                  CarBrand.Audi,              16000));
        AddNewCar(new Car(GT3_720S_NAME,                    CarType.GT,             new List<CarClass> {CarClass.GT3},                  CarBrand.McLaren,           16500));
        AddNewCar(new Car(GT3_AMG_NAME,                     CarType.GT,             new List<CarClass> {CarClass.GT3},                  CarBrand.Mercedes,          16500));
        AddNewCar(new Car(GT3_GTR_NAME,                     CarType.GT,             new List<CarClass> {CarClass.GT3},                  CarBrand.Nissan,            17000));
        AddNewCar(new Car(GT3_911_NAME,                     CarType.GT,             new List<CarClass> {CarClass.GT3},                  CarBrand.Porsche,           17000));

        // CARRERA
        AddNewCar(new Car(CARRERA_911_38_NAME,              CarType.Sportscar,      new List<CarClass> {CarClass.Carrera},              CarBrand.Porsche,           16000));
        AddNewCar(new Car(CARRERA_911_40_NAME,              CarType.Sportscar,      new List<CarClass> {CarClass.Carrera},              CarBrand.Porsche,           18000));

        // GT1
        AddNewCar(new Car(GT1_GTR_NAME,                     CarType.GT,             new List<CarClass> {CarClass.GT1},                  CarBrand.McLaren,           22500));
        AddNewCar(new Car(GT1_CLK_NAME,                     CarType.GT,             new List<CarClass> {CarClass.GT1},                  CarBrand.Mercedes,          23000));
        AddNewCar(new Car(GT1_R390_NAME,                    CarType.GT,             new List<CarClass> {CarClass.GT1},                  CarBrand.Nissan,            23500));
        AddNewCar(new Car(GT1_911_NAME,                     CarType.GT,             new List<CarClass> {CarClass.GT1},                  CarBrand.Porsche,           24000));


        // Formula Trainer
        AddNewCar(new Car(F_TRAINER_NAME,                   CarType.OpenWheeler,    new List<CarClass> {CarClass.FormulaTrainer},       CarBrand.Formula,           8000));

        // Formula Trainer Advanced
        AddNewCar(new Car(F_TRAINER_ADVANCED_NAME,          CarType.OpenWheeler,    new List<CarClass> {CarClass.FormulaTrainerA},      CarBrand.Formula,           9500));

        // Formula Inter
        AddNewCar(new Car(F_INTER_NAME,                     CarType.OpenWheeler,    new List<CarClass> {CarClass.FormulaInter},         CarBrand.Formula,           11500));

        // F3
        AddNewCar(new Car(F3_F301_NAME,                     CarType.OpenWheeler,    new List<CarClass> {CarClass.F3},                   CarBrand.Dallara,           13500));
        AddNewCar(new Car(F3_F309_NAME,                     CarType.OpenWheeler,    new List<CarClass> {CarClass.F3},                   CarBrand.Dallara,           14000));

        // Formula Reiza
        AddNewCar(new Car(F_REIZA_NAME,                     CarType.OpenWheeler,    new List<CarClass> {CarClass.FormulaReiza},         CarBrand.Formula,           17000));

        // Formula Ultimate Gen 2
        AddNewCar(new Car(F_GEN2_NAME,                      CarType.OpenWheeler,    new List<CarClass> {CarClass.FormulaUltimateGen2},  CarBrand.Formula,           23000));


        // Prototype

        // Group C
        AddNewCar(new Car(GROUP_C_CORVETTE_NAME,            CarType.Prototype,      new List<CarClass> {CarClass.GroupC},               CarBrand.Chevrolet,         18000));
        AddNewCar(new Car(GROUP_C_R89C_NAME,                CarType.Prototype,      new List<CarClass> {CarClass.GroupC},               CarBrand.Nissan,            18000));
        AddNewCar(new Car(GROUP_C_962C_NAME,                CarType.Prototype,      new List<CarClass> {CarClass.GroupC},               CarBrand.Porsche,           18500));
        AddNewCar(new Car(GROUP_C_C9_NAME,                  CarType.Prototype,      new List<CarClass> {CarClass.GroupC},               CarBrand.Mercedes,          18500));

        // P4
        AddNewCar(new Car(P4_S2000_NAME,                    CarType.Prototype,      new List<CarClass> {CarClass.P4},                   CarBrand.MCR,               20500));
        AddNewCar(new Car(P4_MRX_DURATEC_NAME,              CarType.Prototype,      new List<CarClass> {CarClass.P4},                   CarBrand.MetalMoro,         20500));

        // P3
        AddNewCar(new Car(P3_MRX_HONDA_NAME,                CarType.Prototype,      new List<CarClass> {CarClass.P3},                   CarBrand.MetalMoro,         23000));
        AddNewCar(new Car(P3_MRX_DURATEC_NAME,              CarType.Prototype,      new List<CarClass> {CarClass.P3},                   CarBrand.MetalMoro,         23000));
        AddNewCar(new Car(P3_ROCO_NAME,                     CarType.Prototype,      new List<CarClass> {CarClass.P3},                   CarBrand.Roco,              23500));

        // P2
        AddNewCar(new Car(P2_MRX_DURATEC_NAME,              CarType.Prototype,      new List<CarClass> {CarClass.P2},                   CarBrand.MetalMoro,         26000));
        AddNewCar(new Car(P2_SIGMA_NAME,                    CarType.Prototype,      new List<CarClass> {CarClass.P2},                   CarBrand.Sigma,             26500));

        // P1 Gen 2
        AddNewCar(new Car(P1_G58_NAME,                      CarType.Prototype,      new List<CarClass> {CarClass.P1Gen2},               CarBrand.Ginetta,           30000));
        AddNewCar(new Car(P1_JS_NAME,                       CarType.Prototype,      new List<CarClass> {CarClass.P1Gen2},               CarBrand.Ligier,            30000));
        AddNewCar(new Car(P1_AJR_CHEVROLET_NAME,            CarType.Prototype,      new List<CarClass> {CarClass.P1Gen2},               CarBrand.MetalMoro,         31000));
        AddNewCar(new Car(P1_AJR_HONDA_NAME,                CarType.Prototype,      new List<CarClass> {CarClass.P1Gen2},               CarBrand.MetalMoro,         31000));
        AddNewCar(new Car(P1_AJR_NISSAN_NAME,               CarType.Prototype,      new List<CarClass> {CarClass.P1Gen2},               CarBrand.MetalMoro,         31000));
        AddNewCar(new Car(P1_G5_NAME,                       CarType.Prototype,      new List<CarClass> {CarClass.P1Gen2},               CarBrand.Sigma,             32000));

        // DLC
        // ============================================================================================================

        // Supercars 1
        AddNewCar(new Car(HYPER_BT62_NAME,                  CarType.Road,           new List<CarClass> {CarClass.HyperCars},            CarBrand.Brabham,           50000), SettingsMenu.DLC.SupercarsOne);
        AddNewCar(new Car(HYPER_SENNA_NAME,                 CarType.Road,           new List<CarClass> {CarClass.HyperCars},            CarBrand.McLaren,           55000), SettingsMenu.DLC.SupercarsOne);
        AddNewCar(new Car(SUPER_CORVETTE_C8_NAME,           CarType.Road,           new List<CarClass> {CarClass.SuperCars},            CarBrand.Chevrolet,         17250), SettingsMenu.DLC.SupercarsOne);

        // Racin' USA 1
        // GTE
        AddNewCar(new Car(GTE_C8R_NAME,                     CarType.GT,             new List<CarClass> {CarClass.GTE},                  CarBrand.Chevrolet,         15000), SettingsMenu.DLC.RacinUSAOne);
        AddNewCar(new Car(GTE_911_NAME,                     CarType.GT,             new List<CarClass> {CarClass.GTE},                  CarBrand.Porsche,           15500), SettingsMenu.DLC.RacinUSAOne);
        AddNewCar(new Car(GTE_M8_NAME,                      CarType.GT,             new List<CarClass> {CarClass.GTE},                  CarBrand.BMW,               16000), SettingsMenu.DLC.RacinUSAOne);
    }

    static Cars(){
        cars            = new List<Car>();
        nameToCar       = new Dictionary<string, Car>();
        typeToCars      = new Dictionary<CarType, List<Car>>();
        classToCars     = new Dictionary<CarClass, List<Car>>();
        brandToCars     = new Dictionary<CarBrand, List<Car>>();
        tierToClasses   = new Dictionary<EventSeries.SeriesTier, List<CarClass>>();
        DLCCars         = new Dictionary<SettingsMenu.DLC, List<Car>>();

        // Init our dicts with empty lists
        foreach(CarType carType in Enum.GetValues(typeof(CarType))){
            typeToCars[carType]     = new List<Car>();
        }
        foreach(CarClass carClass in Enum.GetValues(typeof(CarClass))){
            classToCars[carClass]   = new List<Car>();
        }
        foreach(CarBrand carBrand in Enum.GetValues(typeof(CarBrand))){
            brandToCars[carBrand]   = new List<Car>();
        }
        foreach(SettingsMenu.DLC dlc in Enum.GetValues(typeof(SettingsMenu.DLC))){
            DLCCars[dlc]            = new List<Car>();
        }

        InitializeCars();
        InitializeTierToClasses();
    }

    public static List<CarClass> GetClassesForTier(EventSeries.SeriesTier tier){
        return new List<CarClass> (tierToClasses[tier]);
    }

    private static void AddNewCar(Car carToAdd, SettingsMenu.DLC partOfDLC = 0)
    {
        CarType         carType                         = carToAdd.type;
        List<CarClass>  carClasses                      = carToAdd.classes;
        CarBrand        carBrand                        = carToAdd.brand;
        string          carName                         = carToAdd.name;
        cars.Add(carToAdd);
        nameToCar[carName]                              = carToAdd;

        // If not part of a DLC
        if(0 == partOfDLC)
        {
            // For each car class
            foreach(CarClass carClass in carClasses)
            {
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
        else
        {
            // If part of a DLC, add to the DLC cars dict
            DLCCars[partOfDLC].Add(carToAdd);
        }
    }

    public static void SetDLCState(SettingsMenu.DLC dlc, bool state)
    {
        foreach(Car car in DLCCars[dlc])
        {
            SetDLCCarState(car, state);
        }
    }

    // Adds or removes the car from important lists depending on the given state
    public static void SetDLCCarState(Car car, bool state)
    {
        CarType         carType                         = car.type;
        List<CarClass>  carClasses                      = car.classes;
        CarBrand        carBrand                        = car.brand;

        foreach(CarClass carClass in carClasses)
        {
            // Add/Remove the car from the list
            if(state)
            {
                classToCars[carClass].Add(car);
            }
            else if(classToCars[carClass].Contains(car))
            {
                classToCars[carClass].Remove(car);
            }
        }

        // For the car type
        // Add the car to the list
        if(state)
        {
            typeToCars[carType].Add(car);
            brandToCars[carBrand].Add(car);

            // Add the car to necessary dealers
        }
        // For the car brand
        // Add the car to the list
        else
        {
            if (typeToCars[carType].Contains(car))
            {
                typeToCars[carType].Remove(car);
            }

            if (brandToCars[carBrand].Contains(car))
            {
                brandToCars[carBrand].Remove(car);
            }
        }

        Dealers.SetDLCCarState(car, state);
    }

    public static List<Car> FilterCars(List<Car> toFilter, List<string> carNames, List<CarType> carTypes, List<CarClass> carClasses, List<CarBrand> carBrands){
        // Returns a list of cars that are in 'toFilter' and who's name/type/class/brand matches one of the given params
        List<Car> filteredCars  = new List<Car>();
        foreach(Car car in toFilter){
            // Needs to work with all 4 'categories' (type, class, brand, name), and it is an 'or' inside categories (ie, needs to match any of the classes within carClasses)
            // If list is empty, set to True
            bool validName          = 0 == carNames.Count;
            bool validType          = 0 == carTypes.Count;
            bool validClass         = 0 == carClasses.Count;
            bool validBrand         = 0 == carBrands.Count;

            foreach(string carName in carNames){
                if(car.name == carName){
                    filteredCars.Add(car);
                    validName      = true;
                    break;
                }
            }

            foreach(CarType carType in carTypes){
                if(car.type == carType){
                    validType  = true;
                    break;
                }
            }

            foreach(CarClass carClass in carClasses){
                if(car.classes.Contains(carClass)){
                    validClass  = true;
                    break;
                }
            }

            foreach(CarBrand carBrand in carBrands){
                if(car.brand == carBrand){
                    validBrand  = true;
                    break;
                }
            }

            if(validName && validType && validClass && validBrand){
                filteredCars.Add(car);
            }
        }

        return filteredCars;
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
        GT,
        Prototype,
        Club,
        Truck,
        Road,
        None
    }

    public static Dictionary<CarType, string> typeToString = new Dictionary<CarType, string>
    {
        {CarType.None,                      ""},
        {CarType.OpenWheeler,               "Open Wheeler"},
        {CarType.Touring,                   "Touring"},
        {CarType.Sportscar,                 "Sportscar"},
        {CarType.GT,                        "GT"},
        {CarType.Prototype,                 "Prototype"},
        {CarType.Club,                      "Club"},
        {CarType.Truck,                     "Truck"},
        {CarType.Road,                      "Road"}
    };

    // Adding an entry here requires you to add to:
    // classToString, classToTiers
    // and a SeriesBackgrounds image
    [System.Serializable]
    public enum CarClass
    {
        None,
        Kart125cc,
        KartGX390,
        KartRental,
        KartShifter,
        KartSuper,
        FormulaVee,
        CopaUno,
        CopaClassicB,
        CopaClassicFL,
        CopaTruck,
        TSI,
        StreetCars,
        SuperCars,
        HyperCars,
        GTOpen,
        GTE,
        GT5,
        GT4,
        GT3,
        GT1,
        Carrera,
        Lancer,
        FormulaTrainer,
        FormulaTrainerA,
        FormulaInter,
        F3,
        FormulaReiza,
        FormulaUltimateGen2,
        GroupC,
        P4,
        P3,
        P2,
        P1Gen2,
        DPi
    }

    public static Dictionary<CarClass, string> classToString = new Dictionary<CarClass, string>
    {
        {CarClass.None,                 ""},
        {CarClass.Kart125cc,            "Kart 125cc"},
        {CarClass.KartGX390,            "Kart GX 390"},
        {CarClass.KartRental,           "Kart Rental"},
        {CarClass.KartShifter,          "Kart Shifter"},
        {CarClass.KartSuper,            "Super Kart"},
        {CarClass.FormulaVee,           "Formula Vee"},
        {CarClass.CopaUno,              "Copa Uno"},
        {CarClass.CopaClassicB,         "Copa Classic - B"},
        {CarClass.CopaClassicFL,        "Copa Classic - FL"},
        {CarClass.CopaTruck,            "Copa Truck"},
        {CarClass.TSI,                  "TSI"},
        {CarClass.StreetCars,           "Street Cars"},
        {CarClass.SuperCars,            "Super Cars"},
        {CarClass.HyperCars,            "Hyper Cars"},
        {CarClass.GTE,                  "GT Endurance"},
        {CarClass.GTOpen,               "GT Open"},
        {CarClass.GT5,                  "GT5"},
        {CarClass.GT4,                  "GT4"},
        {CarClass.GT3,                  "GT3"},
        {CarClass.GT1,                  "GT1"},
        {CarClass.Carrera,              "Carrera"},
        {CarClass.Lancer,               "Lancer"},
        {CarClass.FormulaTrainer,       "Formula Trainer"},
        {CarClass.FormulaTrainerA,      "Formula Trainer Advanced"},
        {CarClass.FormulaInter,         "Formula Inter"},
        {CarClass.F3,                   "F3"},
        {CarClass.FormulaReiza,         "Formula Reiza"},
        {CarClass.FormulaUltimateGen2,  "Formula Ultimate Gen 2"},
        {CarClass.GroupC,               "Group C"},
        {CarClass.P4,                   "P4"},
        {CarClass.P3,                   "P3"},
        {CarClass.P2,                   "P2"},
        {CarClass.P1Gen2,               "P1 Generation 2"},
        {CarClass.DPi,                  "DPi"}
    };

    public static Dictionary<CarClass, List<EventSeries.SeriesTier>> classToTiers = new Dictionary<CarClass, List<EventSeries.SeriesTier>>
    {
        {CarClass.None,                 new List<EventSeries.SeriesTier> ()
        },
        {CarClass.Kart125cc,            new List<EventSeries.SeriesTier> {
            {EventSeries.SeriesTier.Rookie}, {EventSeries.SeriesTier.Novice}
        }},
        {CarClass.KartGX390,            new List<EventSeries.SeriesTier> {
            {EventSeries.SeriesTier.Rookie}, {EventSeries.SeriesTier.Novice}
        }},
        {CarClass.KartRental,           new List<EventSeries.SeriesTier> {
            {EventSeries.SeriesTier.Rookie}
        }},
        {CarClass.KartShifter,          new List<EventSeries.SeriesTier> {
            {EventSeries.SeriesTier.Rookie}, {EventSeries.SeriesTier.Novice}
        }},
        {CarClass.KartSuper,            new List<EventSeries.SeriesTier> {
            {EventSeries.SeriesTier.Rookie}, {EventSeries.SeriesTier.Novice}, {EventSeries.SeriesTier.Prodigy}, {EventSeries.SeriesTier.WorldRenowned}
        }},
        {CarClass.FormulaVee,           new List<EventSeries.SeriesTier> {
            {EventSeries.SeriesTier.Rookie}, {EventSeries.SeriesTier.Novice}, {EventSeries.SeriesTier.Master}
        }},
        {CarClass.CopaUno,              new List<EventSeries.SeriesTier> {
            {EventSeries.SeriesTier.Rookie}, {EventSeries.SeriesTier.Novice}, {EventSeries.SeriesTier.Master}
        }},
        {CarClass.CopaClassicB,         new List<EventSeries.SeriesTier> {
            {EventSeries.SeriesTier.Rookie}, {EventSeries.SeriesTier.Novice}, {EventSeries.SeriesTier.Prodigy}
        }},
        {CarClass.CopaClassicFL,        new List<EventSeries.SeriesTier> {
            {EventSeries.SeriesTier.Novice}, {EventSeries.SeriesTier.Amateur}, {EventSeries.SeriesTier.Legend}
        }},
        {CarClass.CopaTruck,            new List<EventSeries.SeriesTier> {
            {EventSeries.SeriesTier.Rookie}, {EventSeries.SeriesTier.Amateur}, {EventSeries.SeriesTier.Elite}
        }},
        {CarClass.TSI,                  new List<EventSeries.SeriesTier> {
            {EventSeries.SeriesTier.Novice}, {EventSeries.SeriesTier.Amateur}, {EventSeries.SeriesTier.Professional}
        }},
        {CarClass.Carrera,              new List<EventSeries.SeriesTier> {
            {EventSeries.SeriesTier.Novice}, {EventSeries.SeriesTier.Amateur}, {EventSeries.SeriesTier.Elite}
        }},
        {CarClass.StreetCars,           new List<EventSeries.SeriesTier> {
            {EventSeries.SeriesTier.Novice}, {EventSeries.SeriesTier.Amateur}, {EventSeries.SeriesTier.Professional}
        }},
        {CarClass.Lancer,               new List<EventSeries.SeriesTier> {
            {EventSeries.SeriesTier.Novice}, {EventSeries.SeriesTier.Amateur}, {EventSeries.SeriesTier.Professional}
        }},
        {CarClass.SuperCars,            new List<EventSeries.SeriesTier> {
            {EventSeries.SeriesTier.Amateur}, {EventSeries.SeriesTier.Professional}, {EventSeries.SeriesTier.Master}
        }},
        {CarClass.HyperCars,            new List<EventSeries.SeriesTier> {
            {EventSeries.SeriesTier.Elite}, {EventSeries.SeriesTier.Master}, {EventSeries.SeriesTier.Prodigy}
        }},
        {CarClass.GTE,                  new List<EventSeries.SeriesTier> {
            {EventSeries.SeriesTier.Elite}, {EventSeries.SeriesTier.Master}, {EventSeries.SeriesTier.Prodigy}
        }},
        {CarClass.GTOpen,               new List<EventSeries.SeriesTier> {
            {EventSeries.SeriesTier.Professional}, {EventSeries.SeriesTier.Master}, {EventSeries.SeriesTier.Legend}
        }},
        {CarClass.GT5,                  new List<EventSeries.SeriesTier> {
            {EventSeries.SeriesTier.Novice}, {EventSeries.SeriesTier.Amateur}
        }},
        {CarClass.GT4,                  new List<EventSeries.SeriesTier> {
            {EventSeries.SeriesTier.Professional}, {EventSeries.SeriesTier.Elite}
        }},
        {CarClass.GT3,                  new List<EventSeries.SeriesTier> {
            {EventSeries.SeriesTier.Elite}, {EventSeries.SeriesTier.Master}
        }},
        {CarClass.GT1,                  new List<EventSeries.SeriesTier> {
            {EventSeries.SeriesTier.Master}, {EventSeries.SeriesTier.Prodigy}, {EventSeries.SeriesTier.Legend}, {EventSeries.SeriesTier.WorldRenowned}
        }},
        {CarClass.FormulaTrainer,       new List<EventSeries.SeriesTier> {
            {EventSeries.SeriesTier.Rookie}, {EventSeries.SeriesTier.Novice}, {EventSeries.SeriesTier.Amateur}
        }},
        {CarClass.FormulaTrainerA,      new List<EventSeries.SeriesTier> {
            {EventSeries.SeriesTier.Novice}, {EventSeries.SeriesTier.Amateur}, {EventSeries.SeriesTier.Professional}
        }},
        {CarClass.FormulaInter,         new List<EventSeries.SeriesTier> {
            {EventSeries.SeriesTier.Amateur}, {EventSeries.SeriesTier.Professional}, {EventSeries.SeriesTier.Elite}
        }},
        {CarClass.F3,                   new List<EventSeries.SeriesTier> {
            {EventSeries.SeriesTier.Professional}, {EventSeries.SeriesTier.Elite}, {EventSeries.SeriesTier.Master}
        }},
        {CarClass.FormulaReiza,         new List<EventSeries.SeriesTier> {
            {EventSeries.SeriesTier.Elite}, {EventSeries.SeriesTier.Master}, {EventSeries.SeriesTier.Prodigy}
        }},
        {CarClass.FormulaUltimateGen2,  new List<EventSeries.SeriesTier> {
            {EventSeries.SeriesTier.Prodigy}, {EventSeries.SeriesTier.Legend}, {EventSeries.SeriesTier.WorldRenowned}
        }},
        {CarClass.GroupC,               new List<EventSeries.SeriesTier> {
            {EventSeries.SeriesTier.Prodigy}, {EventSeries.SeriesTier.Legend}, {EventSeries.SeriesTier.WorldRenowned}
        }},
        {CarClass.P4,                   new List<EventSeries.SeriesTier> {
            {EventSeries.SeriesTier.Professional}, {EventSeries.SeriesTier.Elite}
        }},
        {CarClass.P3,                   new List<EventSeries.SeriesTier> {
            {EventSeries.SeriesTier.Elite}, {EventSeries.SeriesTier.Master}, {EventSeries.SeriesTier.Prodigy}
        }},
        {CarClass.P2,                   new List<EventSeries.SeriesTier> {
            {EventSeries.SeriesTier.Master}, {EventSeries.SeriesTier.Prodigy}, {EventSeries.SeriesTier.Legend}
        }},
        {CarClass.P1Gen2,               new List<EventSeries.SeriesTier> {
            {EventSeries.SeriesTier.Prodigy}, {EventSeries.SeriesTier.Legend}, {EventSeries.SeriesTier.WorldRenowned}
        }},
        {CarClass.DPi,                  new List<EventSeries.SeriesTier> {
            {EventSeries.SeriesTier.Prodigy}, {EventSeries.SeriesTier.Legend}, {EventSeries.SeriesTier.WorldRenowned}
        }}
    };

    public enum CarBrand
    {
        None,
        Kart,
        Ford,
        Volkswagen,
        Puma,
        Fiat,
        Mini,
        Chevrolet,
        Iveco,
        MAN,
        Mercedes,
        Vulkan,
        Ginetta,
        Porsche,
        BMW,
        McLaren,
        Nissan,
        Dallara,
        Audi,
        Ultima,
        Formula,
        MCR,
        MetalMoro,
        Roco,
        Sigma,
        Ligier,
        Mitsubishi,
        Brabham
    }

    private static void InitializeTierToClasses(){
        CarClass carClass;
        List<EventSeries.SeriesTier> tierList;

        // For each entry in our classToTiers dict
        foreach(KeyValuePair<CarClass, List<EventSeries.SeriesTier>> entry in classToTiers){
            carClass = entry.Key;
            tierList = entry.Value;

            // Extract every tier from every class and organize a tierToClasses dict with the info gathered
            foreach(EventSeries.SeriesTier tier in tierList){
                if(!tierToClasses.ContainsKey(tier)){
                    tierToClasses[tier] = new List<CarClass>();
                }
                tierToClasses[tier].Add(carClass);
            }
        }
    }
}