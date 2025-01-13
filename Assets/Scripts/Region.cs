using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting.Antlr3.Runtime;

public class Region
{
    public static Dictionary<ClickableRegion, Region>               regions;
    public static int                                               MAX_EVENTS_PER_REGION = 5;

    public List<EventSeries>                                        series;
    public Dictionary<EventSeries.SeriesTier, List<EventSeries>>    tierToSeries;
    public ClickableRegion                                          region;

    static Region(){
        regions             = new Dictionary<ClickableRegion, Region>();

        foreach(ClickableRegion region in Enum.GetValues(typeof(ClickableRegion))){
            regions[region] = new Region(region);
        }
    }

    public Region(ClickableRegion regionParam){
        // Initializations
        series          = new List<EventSeries>();
        tierToSeries    = new Dictionary<EventSeries.SeriesTier, List<EventSeries>>();
        region          = regionParam;

        // Initialize the lists within
        foreach(EventSeries.SeriesTier tier in Enum.GetValues(typeof(EventSeries.SeriesTier))){
            tierToSeries[tier]          = new List<EventSeries>();
        }
    }

    // Returns the class the event series was created for
    public Cars.CarClass GenerateNewEventSeries(EventSeries.SeriesTier tier){
        // First, choose a class
        // Make sure we aren't making dupes
        List<Cars.CarClass> possibleClasses = Cars.GetClassesForTier(tier);
        FilterClassesByRegion(region, tier, possibleClasses);

        Cars.CarClass   classToUse          = Cars.CarClass.None;
        bool            duplicate           = false;

        // Randomly select a class until we get a non-dupe
        while(classToUse == Cars.CarClass.None){
            classToUse                      = possibleClasses[UnityEngine.Random.Range(0, possibleClasses.Count)];
            duplicate                       = false;
            foreach(EventSeries eventSeries in tierToSeries[tier]){
                if(eventSeries.events.Count > 0){
                    foreach(Cars.CarClass carClass in eventSeries.events[0].classWhitelist){
                        if(carClass == classToUse){
                            duplicate       = true;
                            break;
                        }
                    }
                }
                if(duplicate){
                    possibleClasses.Remove(classToUse);
                    classToUse              = Cars.CarClass.None;
                    break;
                }
            }

            if(possibleClasses.Count == 0){
                break;
            }
        }

        if(classToUse == Cars.CarClass.None){
            Debug.Log("No valid car classes were found, perhaps due to already exhasuting all duplicates.");
            return classToUse;
        }

        EventSeries  newSeries              = new EventSeries(Cars.classToString[classToUse] + " Showdown", tier, region);

        // First event will always be a mini race
        Event.GenerateNewEvent(
            "Weekend Race - " + Cars.classToString[classToUse],
            Event.EventType.Race,
            Event.EventDuration.Mini,
            newSeries,
            Tracks.GetCountries(region),
            new List<Cars.CarType>(),
            new List<Cars.CarClass>() { {classToUse} },
            new List<Cars.CarBrand>(),
            new List<string>(),
            useLaps:true
        );

        return classToUse;
    }

    // Called by EventSeries' constructor
    public void AddNewSeries(EventSeries seriesToAdd){
        series.Add(seriesToAdd);
        tierToSeries[seriesToAdd.seriesTier].Add(seriesToAdd);
    }

    public static int GetRenownCostForRegionTier(ClickableRegion region, EventSeries.SeriesTier tier){
        switch(region){
            case ClickableRegion.NorthAmerica:
            case ClickableRegion.SouthAmerica:
            case ClickableRegion.Europe:
            case ClickableRegion.Africa:
            case ClickableRegion.Asia:
            case ClickableRegion.Australia:
                return (int)tier + 1;

            case ClickableRegion.NorthAmericaSouthAmerica:
            case ClickableRegion.SouthAmericaAfrica:
            case ClickableRegion.AfricaAustralia:
            case ClickableRegion.AustraliaAsia:
            case ClickableRegion.AsiaEurope:
            case ClickableRegion.EuropeNorthAmerica:
            case ClickableRegion.EuropeAfrica:
                return 2 * ((int)tier + 1);

            case ClickableRegion.International:
                return 5 * ((int)tier + 1);
        }

        return 0;
    }

    // There are some classes that simply can't race in some regions, so blacklist them here
    // Will remove classes from the list
    public static void FilterClassesByRegion(ClickableRegion region, EventSeries.SeriesTier tier, List<Cars.CarClass> carClasses){
        List<Cars.CarClass> classesToRemove = new List<Cars.CarClass>();

        // Look for special cases
        switch(region){
            case ClickableRegion.NorthAmerica:
            case ClickableRegion.Africa:
            case ClickableRegion.Asia:
            case ClickableRegion.Australia:
            case ClickableRegion.AustraliaAsia:
            case ClickableRegion.AfricaAustralia:
                classesToRemove.Add(Cars.CarClass.Kart125cc);
                classesToRemove.Add(Cars.CarClass.KartGX390);
                classesToRemove.Add(Cars.CarClass.KartRental);
                classesToRemove.Add(Cars.CarClass.KartShifter);
                classesToRemove.Add(Cars.CarClass.KartSuper);
                break;

            default:
                break;
        }

        foreach(Cars.CarClass carClass in classesToRemove){
            if(carClasses.Contains(carClass)){
                carClasses.Remove(carClass);
            }
        }
    }

    // Returns a list of regions that are seen as 'prerequisites' for a region
    // So, if you want to unlock a tier for a region, that tier must be unlocked for the returned regions first
    public static List<ClickableRegion> GetRegionPrereqs(ClickableRegion region){
        List<ClickableRegion> prereqs = new List<ClickableRegion>();

        switch(region){
            case ClickableRegion.NorthAmerica:
            case ClickableRegion.SouthAmerica:
            case ClickableRegion.Europe:
            case ClickableRegion.Africa:
            case ClickableRegion.Asia:
            case ClickableRegion.Australia:
                return prereqs;

            case ClickableRegion.NorthAmericaSouthAmerica:
                prereqs.Add(ClickableRegion.NorthAmerica);
                prereqs.Add(ClickableRegion.SouthAmerica);
                return prereqs;

            case ClickableRegion.SouthAmericaAfrica:
                prereqs.Add(ClickableRegion.SouthAmerica);
                prereqs.Add(ClickableRegion.Africa);
                return prereqs;

            case ClickableRegion.AfricaAustralia:
                prereqs.Add(ClickableRegion.Africa);
                prereqs.Add(ClickableRegion.Australia);
                return prereqs;

            case ClickableRegion.AustraliaAsia:
                prereqs.Add(ClickableRegion.Australia);
                prereqs.Add(ClickableRegion.Asia);
                return prereqs;

            case ClickableRegion.AsiaEurope:
                prereqs.Add(ClickableRegion.Asia);
                prereqs.Add(ClickableRegion.Europe);
                return prereqs;

            case ClickableRegion.EuropeNorthAmerica:
                prereqs.Add(ClickableRegion.Europe);
                prereqs.Add(ClickableRegion.NorthAmerica);
                return prereqs;

            case ClickableRegion.EuropeAfrica:
                prereqs.Add(ClickableRegion.Europe);
                prereqs.Add(ClickableRegion.Africa);
                return prereqs;

            case ClickableRegion.International:
                prereqs.Add(ClickableRegion.NorthAmerica);
                prereqs.Add(ClickableRegion.SouthAmerica);
                prereqs.Add(ClickableRegion.Africa);
                prereqs.Add(ClickableRegion.Australia);
                prereqs.Add(ClickableRegion.Asia);
                prereqs.Add(ClickableRegion.Europe);
                return prereqs;
        }

        return prereqs;
    }

    // Make sure to add to GetRenownCostForRegionTier() and GetRegionPrereqs() if adding a new region
    public enum ClickableRegion
    {
        // Continents
        NorthAmerica,
        SouthAmerica,
        Europe,
        Africa,
        Asia,
        Australia,

        // Inter continent
        NorthAmericaSouthAmerica,
        SouthAmericaAfrica,
        AfricaAustralia,
        AustraliaAsia,
        AsiaEurope,
        EuropeNorthAmerica,
        EuropeAfrica,

        International
    }

    public static Dictionary<ClickableRegion, string> regionToString = new Dictionary<ClickableRegion, string>
    {
        {ClickableRegion.NorthAmerica,                  "North America"},
        {ClickableRegion.SouthAmerica,                  "South America"},
        {ClickableRegion.Europe,                        "Europe"},
        {ClickableRegion.Africa,                        "Africa"},
        {ClickableRegion.Asia,                          "Asia"},
        {ClickableRegion.Australia,                     "Australia"},
        {ClickableRegion.NorthAmericaSouthAmerica,      "Western Regionals"},
        {ClickableRegion.SouthAmericaAfrica,            "Southwestern Regionals"},
        {ClickableRegion.AfricaAustralia,               "Southeastern Regionals"},
        {ClickableRegion.AustraliaAsia,                 "Eastern Regionals"},
        {ClickableRegion.AsiaEurope,                    "Northeastern Regionals"},
        {ClickableRegion.EuropeNorthAmerica,            "Northwestern Regionals"},
        {ClickableRegion.EuropeAfrica,                  "Central Regionals"},
        {ClickableRegion.International,                 "International"}
    };
}