using System.Collections.Generic;
using UnityEngine;
using System;

public class Region
{
    public static Dictionary<ClickableRegion, Region>               regions;

    public List<EventSeries>                                        series;
    public Dictionary<EventSeries.SeriesTier, List<EventSeries>>    tierToSeries;
    public List<EventSeries.SeriesTier>                             unlockedTiers;

    static Region(){
        regions             = new Dictionary<ClickableRegion, Region>();

        foreach(ClickableRegion region in Enum.GetValues(typeof(ClickableRegion))){
            regions[region] = new Region();
        }
    }

    public Region(){
        // Initializations
        series          = new List<EventSeries>();
        tierToSeries    = new Dictionary<EventSeries.SeriesTier, List<EventSeries>>();
        unlockedTiers   = new List<EventSeries.SeriesTier>();

        // Initialize the lists within
        foreach(EventSeries.SeriesTier tier in Enum.GetValues(typeof(EventSeries.SeriesTier))){
            tierToSeries[tier]          = new List<EventSeries>();
        }
    }

    public void UnlockTier(EventSeries.SeriesTier tier){
        if(!unlockedTiers.Contains(tier)){
            unlockedTiers.Add(tier);
        }
    }

    // Called by EventSeries' constructor
    public void AddNewSeries(EventSeries seriesToAdd){
        series.Add(seriesToAdd);
        tierToSeries[seriesToAdd.seriesTier].Add(seriesToAdd);
    }

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