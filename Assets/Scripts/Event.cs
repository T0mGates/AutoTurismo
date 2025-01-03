using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using System.Linq;

public class EventSeries
{
    // Name and tier together should make unique series
    public EventSeriesManager.SeriesTier    seriesTier;
    public string                           name;
    public List<Event>                      events;
    public Tracks.ClickableRegion           partOfRegion;

    public EventSeries(string nameParam, EventSeriesManager.SeriesTier seriesTierParam, Tracks.ClickableRegion partOfRegionParam){
        name            = nameParam;
        seriesTier      = seriesTierParam;
        partOfRegion    = partOfRegionParam;
        events          = new List<Event>();

        EventSeriesManager.AddNewSeries(this);
    }

    // Override the equals method
    public override bool Equals(object obj)
    {
        if(obj == null || GetType() != obj.GetType()){
            return false;
        }

        EventSeries seriesToCompare = (EventSeries)obj;
        return seriesToCompare.name == name && seriesToCompare.seriesTier == seriesTier;
    }
    public override int GetHashCode()
    {
        return name.GetHashCode() + seriesTier.GetHashCode();
    }

}

public static class EventSeriesManager
{
    private static Dictionary<Tracks.ClickableRegion, List<EventSeries>> regionToSeries;

    static EventSeriesManager(){
        // Initialize our main dictionary
        regionToSeries = new Dictionary<Tracks.ClickableRegion, List<EventSeries>>();

        // Initialize the lists within
        foreach(Tracks.ClickableRegion region in Enum.GetValues(typeof(Tracks.ClickableRegion))){
            regionToSeries[region] = new List<EventSeries>();
        }
    }

    public static List<EventSeries> GetRegionSeries(Tracks.ClickableRegion region){
        return regionToSeries[region];
    }

    // Called to add a new series to a region
    // Called by EventSeries' constructor
    public static void AddNewSeries(EventSeries seriesToAdd){
        regionToSeries[seriesToAdd.partOfRegion].Add(seriesToAdd);
    }

    public enum SeriesTier
    {
        Rookie          = 0,
        Novice          = 1,
        Amateur         = 2,
        Pro             = 3,
        Elite           = 4,
        Master          = 5,
        Prodigy         = 6,
        Legend          = 7,
        WorldRenowned   = 8
    }

    public static Dictionary<SeriesTier, string> tierToString = new Dictionary<SeriesTier, string>
    {
        {SeriesTier.Rookie,             "Rookie"},
        {SeriesTier.Novice,             "Novice"},
        {SeriesTier.Amateur,            "Amateur"},
        {SeriesTier.Pro,                "Pro"},
        {SeriesTier.Elite,              "Elite"},
        {SeriesTier.Master,             "Master"},
        {SeriesTier.Prodigy,            "Prodigy"},
        {SeriesTier.Legend,             "Legend"},
        {SeriesTier.WorldRenowned,      "World-Renowned"}
    };

    // Used for race length calculations
    public static Dictionary<SeriesTier, int> tierToAvgKMPerMinuteSpeed = new Dictionary<SeriesTier, int>
    {
        {SeriesTier.Rookie,             100/60},
        {SeriesTier.Novice,             125/60},
        {SeriesTier.Amateur,            150/60},
        {SeriesTier.Pro,                175/60},
        {SeriesTier.Elite,              200/60},
        {SeriesTier.Master,             210/60},
        {SeriesTier.Prodigy,            220/60},
        {SeriesTier.Legend,             235/60},
        {SeriesTier.WorldRenowned,      250/60}
    };

    // Set restrictions on what grade tracks each tier drivers can race on
    public static Dictionary<SeriesTier, List<Tracks.Grade>> tierAllowedOnGrade = new Dictionary<SeriesTier, List<Tracks.Grade>>
    {
        {SeriesTier.Rookie,             new List<Tracks.Grade>() { {Tracks.Grade.Four}, {Tracks.Grade.Three}, {Tracks.Grade.Historic} } },
        {SeriesTier.Novice,             new List<Tracks.Grade>() { {Tracks.Grade.Four}, {Tracks.Grade.Three}, {Tracks.Grade.Historic} } },
        {SeriesTier.Amateur,            new List<Tracks.Grade>() { {Tracks.Grade.Four}, {Tracks.Grade.Three}, {Tracks.Grade.Historic}, {Tracks.Grade.Temporary} } },
        {SeriesTier.Pro,                new List<Tracks.Grade>() { {Tracks.Grade.Four}, {Tracks.Grade.Three}, {Tracks.Grade.Historic}, {Tracks.Grade.Temporary}, {Tracks.Grade.Two} } },
        {SeriesTier.Elite,              new List<Tracks.Grade>() { {Tracks.Grade.Three}, {Tracks.Grade.Two}, {Tracks.Grade.Historic} } },
        {SeriesTier.Master,             new List<Tracks.Grade>() { {Tracks.Grade.Three}, {Tracks.Grade.One}, {Tracks.Grade.Historic}, {Tracks.Grade.Two} } },
        {SeriesTier.Prodigy,            new List<Tracks.Grade>() { {Tracks.Grade.One}, {Tracks.Grade.Historic}, {Tracks.Grade.Two} } },
        {SeriesTier.Legend,             new List<Tracks.Grade>() { {Tracks.Grade.One}, {Tracks.Grade.Historic}, {Tracks.Grade.Two} } },
        {SeriesTier.WorldRenowned,      new List<Tracks.Grade>() { {Tracks.Grade.One}, {Tracks.Grade.Historic}}}
    };

    // Set base fame pay for each tier
    public static Dictionary<SeriesTier, int> tierFameReward = new Dictionary<SeriesTier, int>
    {
        {SeriesTier.Rookie,             10},
        {SeriesTier.Novice,             15},
        {SeriesTier.Amateur,            20},
        {SeriesTier.Pro,                30},
        {SeriesTier.Elite,              35},
        {SeriesTier.Master,             40},
        {SeriesTier.Prodigy,            50},
        {SeriesTier.Legend,             60},
        {SeriesTier.WorldRenowned,      80},
    };
    // Set base money pay for each tier
    public static Dictionary<SeriesTier, int> tierMoneyReward = new Dictionary<SeriesTier, int>
    {
        {SeriesTier.Rookie,             500},
        {SeriesTier.Novice,             700},
        {SeriesTier.Amateur,            1000},
        {SeriesTier.Pro,                1250},
        {SeriesTier.Elite,              1500},
        {SeriesTier.Master,             1700},
        {SeriesTier.Prodigy,            1850},
        {SeriesTier.Legend,             2150},
        {SeriesTier.WorldRenowned,      2400},
    };
}

public class Event
{
    public string               name;
    public EventType            eventType;
    public EventDuration        eventDuration;
    public EventSeries          parentEventSeries;

    public List<Cars.CarType>   typeWhitelist;
    public List<Cars.CarClass>  classWhitelist;
    public List<Cars.CarBrand>  brandWhitelist;
    public List<string>         nameWhitelist;

    private int                 topFameReward;
    private int                 topMoneyReward;
    private int                 finishPosition;
    private int                 gridSize;
    private bool                completed;

    private List<EventEntry>    eventEntries;

    public Event(
                string nameParam, EventType eventTypeParam, EventDuration eventDurationParam, EventSeries parentEventSeriesParam,
                List<Cars.CarType> allowedTypes, List<Cars.CarClass> allowedClasses, List<Cars.CarBrand> allowedBrands, List<string> allowedNames,
                int topFameRewardParam, int topMoneyRewardParam
                ){
        name                = nameParam;
        eventType           = eventTypeParam;
        eventDuration       = eventDurationParam;
        parentEventSeries   = parentEventSeriesParam;

        topFameReward       = topFameRewardParam;
        topMoneyReward      = topMoneyRewardParam;

        typeWhitelist       = allowedTypes;
        classWhitelist      = allowedClasses;
        brandWhitelist      = allowedBrands;
        nameWhitelist       = allowedNames;
        completed           = false;
        gridSize            = -1;

        // Will be filled in by newly instantiated event entries
        eventEntries        = new List<EventEntry>();

        parentEventSeries.events.Add(this);
    }

    public void EventEntryCompleted(EventEntry eventEntryParam){
        int nextUpIndex = eventEntries.IndexOf(eventEntryParam) + 1;

        if(nextUpIndex >= eventEntries.Count){
            Debug.Log("Event: " + name + " is complete!");
            // Depending on the event type, calculate final position
            switch(eventType){
                case EventType.Race:
                    finishPosition = eventEntryParam.playerResult.FinishingPositionInClass;
                    break;
                case EventType.Championship:
                    // TODO championship points and whatnot
                    break;

                default:
                    Debug.LogError("Event type: " + eventTypeToString[eventType] + " has not been implemented yet!");
                    break;
            }
            completed = true;
        }
        else{
            eventEntries[nextUpIndex].nextUp = true;
        }
    }

    public void AddNewEventEntry(EventEntry entryToAdd){
        // If this is the first event, make it the next up to race
        if(0 == eventEntries.Count){
            entryToAdd.nextUp = true;
        }

        // Need to figure out gridSize of the whole 'event'
        // If it is still the default (first event entry to be added)
        if(-1 == gridSize){
            gridSize = entryToAdd.gridSize;
        }
        else{
            // If this is a championship, make the lowest grid sized eventEntry the total gridSize for all entries (and for this event in general)
            switch(eventType){
                case EventType.Championship:
                    // Assume current gridSize is the lowest, so just check this one
                    if(entryToAdd.gridSize < gridSize){
                        // If it is a new low, edit the other event entries within this event to match this one (since a championship needs the same amount of drivers in every race)
                        gridSize = entryToAdd.gridSize;
                        foreach(EventEntry entry in eventEntries){
                            entry.gridSize = gridSize;
                        }
                    }
                    break;

                default:
                    Debug.LogError("Event type: " + eventTypeToString[eventType] + " has not been implemented yet!");
                    break;
            }
        }

        eventEntries.Add(entryToAdd);
    }

    public bool GetCompletedStatus(){
        return completed;
    }

    public int GetEventEntryPosition(EventEntry eventEntryToCheck){
        // Returns the 'position number' for a given event entry
        return eventEntries.IndexOf(eventEntryToCheck) + 1;
    }

    // Returns a string that includes details on all the whitelists for this event
    public string GetPrintWhitelist(){
        const string OR_SEPARATOR   = " OR";
        const string AND_SEPARATOR  = "\nAND\n";

        string toReturn             = "";

        // Car Names
        if(nameWhitelist.Count > 0){
            toReturn += "Car Names:";
        }
        foreach(string carName in nameWhitelist){
            toReturn += " " + carName + OR_SEPARATOR;
        }
        // Remove the trailing comma, add new line
        if(nameWhitelist.Count > 0){
            toReturn = toReturn.Substring(0, toReturn.Length - OR_SEPARATOR.Length) + AND_SEPARATOR;
        }

        // Car Types
        if(typeWhitelist.Count > 0){
            toReturn += "Car Types:";
        }
        foreach(Cars.CarType carType in typeWhitelist){
            toReturn += " " + Cars.typeToString[carType] + OR_SEPARATOR;
        }
        // Remove the trailing comma, add new line
        if(typeWhitelist.Count > 0){
            toReturn = toReturn.Substring(0, toReturn.Length - OR_SEPARATOR.Length) + AND_SEPARATOR;
        }

        // Car Classes
        if(classWhitelist.Count > 0){
            toReturn += "Car Classes:";
        }
        foreach(Cars.CarClass carClass in classWhitelist){
            toReturn += " " + Cars.classToString[carClass] + OR_SEPARATOR;
        }
        // Remove the trailing comma, add new line
        if(classWhitelist.Count > 0){
            toReturn = toReturn.Substring(0, toReturn.Length - OR_SEPARATOR.Length) + AND_SEPARATOR;
        }

        // Car Brands
        if(brandWhitelist.Count > 0){
            toReturn += "Car Brands:";
        }
        foreach(Cars.CarBrand carBrand in brandWhitelist){
            toReturn += " " + carBrand.ToString() + OR_SEPARATOR;
        }
        // Remove the trailing comma, add new line
        if(brandWhitelist.Count > 0){
            toReturn = toReturn.Substring(0, toReturn.Length - OR_SEPARATOR.Length) + AND_SEPARATOR;
        }

        // If our string is not empty, remove the trailing 'AND_SEPARATOR'
        if(toReturn.Length >= AND_SEPARATOR.Length){
             toReturn = toReturn.Substring(0, toReturn.Length - AND_SEPARATOR.Length);
        }
        return toReturn;
    }

    public string GetRewardInfo(){
        return
            "You have finished the "    + GetPrintEventType()       + " event: " + name + "!" +
            "\n\nSeries Tier: "         + EventSeriesManager.tierToString[parentEventSeries.seriesTier] +
            "\nTop Reward: "            + GetPrintTopMoneyReward()  + " and " + GetPrintTopFameReward() +
            "\n\nGrid Size: "           + gridSize.ToString()       +
            "\nYour Result: P"          + finishPosition.ToString() + " = " + ((int)(FinishPositionMultiplier()*100)).ToString() + "% of the Top Reward" +
            "\n\nTotal Rewards = "      + GetPrintMoneyReward()     + " and " + GetPrintFameReward();
    }

    public float FinishPositionMultiplier(){
        if(finishPosition == 0){
            return 0;
        }
        if(finishPosition > gridSize){
            Debug.LogError("Detected that a finish position was greater than the given grid size.");
            return 0;
        }
        return Mathf.Round(((float)(gridSize-finishPosition + 1))/(float)gridSize * 100f) / 100f;
    }

    public int GetMoneyReward(){
        return Mathf.CeilToInt(topMoneyReward * FinishPositionMultiplier());
    }
    public string GetPrintTopMoneyReward(){
        return "$" + topMoneyReward.ToString("n0");
    }
    public string GetPrintMoneyReward(){
        return "$" + GetMoneyReward().ToString("n0");
    }

    public string GetPrintTopFameReward(){
        return topFameReward.ToString("n0") + " Fame";
    }
    public string GetPrintFameReward(){
        return GetFameReward().ToString("n0") + " Fame";
    }
    public int GetFameReward(){
        return Mathf.CeilToInt(topFameReward * FinishPositionMultiplier());
    }

    public string GetPrintEventType(){
        return eventDurationToString[eventDuration] + " " + eventTypeToString[eventType];
    }

    public List<EventEntry> GetEventEntries(){
        return eventEntries;
    }

    public static Event GenerateNewEvent(
                string eventName, EventType eventType, EventDuration duration, EventSeries parentEventSeriesParam, List<Tracks.Country> allowedCountries,
                List<Cars.CarType> allowedTypes, List<Cars.CarClass> allowedClasses, List<Cars.CarBrand> allowedBrands, List<string> allowedNames,
                bool useLaps = true, List<Track> blacklistedTracks = null
                ){
        // First pick the track depending on the tier and country
        EventSeriesManager.SeriesTier   tier            = parentEventSeriesParam.seriesTier;
        Track                           trackToUse      = null;
        List<Tracks.Grade>              allowedGrades   = EventSeriesManager.tierAllowedOnGrade[tier];

        // If any kart is in the event, only kart tracks can be raced
        if(allowedClasses.Contains(Cars.CarClass.Kart125cc) || allowedClasses.Contains(Cars.CarClass.KartGX390) || allowedClasses.Contains(Cars.CarClass.KartRental) || allowedClasses.Contains(Cars.CarClass.KartShifter) || allowedClasses.Contains(Cars.CarClass.KartSuper) || allowedBrands.Contains(Cars.CarBrand.Kart)){
            allowedGrades = new List<Tracks.Grade> { {Tracks.Grade.Kart} };
        }

        // Keep track of which grades we have tried
        List<Tracks.Grade>              triedGrades     = new List<Tracks.Grade>();
        // Keep track of which countries we have tried
        List<Tracks.Country>            triedCountries  = new List<Tracks.Country>();

        Tracks.Grade                    gradeToUse;
        Tracks.Country                  countryToUse;
        List<Track>                     validTracks     = new List<Track>();

        List<EventEntry>                eventEntries    = new List<EventEntry>();
        // Get the number of actual entries ('races') we want within this event
        int                             numEntries      = GetNumEventEntries(eventType, duration);
        List<Track>                     tracksToUse     = new List<Track>();

        int topFameReward                               = GenerateTopFameReward(duration, parentEventSeriesParam.seriesTier) * numEntries;
        int topMoneyReward                              = GenerateTopMoneyReward(duration, parentEventSeriesParam.seriesTier) * numEntries;

        // Holds which countries have currently been used
        List<Tracks.Country> usedCountries              = new List<Tracks.Country>();


        bool triedAllGrades                             = true;
        bool triedAllCountries                          = true;

        // While we haven't filled out our tracksToUse
        while(tracksToUse.Count < numEntries){

            // If we already checked for a track for every country, just reset and have duplicate countries
            if(usedCountries.Count >= allowedCountries.Count){
                usedCountries.Clear();
            }

            triedGrades.Clear();
            trackToUse = null;
            validTracks.Clear();
            triedAllGrades = false;

            // While we haven't tried every grade and we haven't honed in on a track for this iteration
            while(triedGrades.Count < allowedGrades.Count && null == trackToUse){
                // Start with the track grade
                // Pick a random grade from the list of valid track grades (so long we haven't tried that grade yet)
                while(true){
                    gradeToUse  = allowedGrades[UnityEngine.Random.Range(0, allowedGrades.Count)];
                    // If we haven't tried this grade, break
                    if(!triedGrades.Contains(gradeToUse)){
                        break;
                    }
                    // If we have tried every grade, break out completely
                    if(triedGrades.Count == allowedGrades.Count){
                        triedAllGrades = true;
                        break;
                    }
                }

                if(triedAllGrades){
                    break;
                }

                triedGrades.Add(gradeToUse);

                triedCountries.Clear();
                triedAllCountries = false;

                // While we haven't tried every country and we haven't found a single valid track
                while(triedCountries.Count < allowedCountries.Count && 0 == validTracks.Count){
                    // Pick a random country from the list of allowed countries (so long we haven't tried that country yet)
                    while(true){
                        countryToUse  = allowedCountries[UnityEngine.Random.Range(0, allowedCountries.Count)];
                        // If we haven't tried this country, go on (break)
                        if(!triedCountries.Contains(countryToUse)){
                            if(!usedCountries.Contains(countryToUse)){
                                break;
                            }
                            triedCountries.Add(countryToUse);
                        }
                        // If we have tried every country, break out completely
                        if(triedCountries.Count == allowedCountries.Count){
                            triedAllCountries = true;
                            break;
                        }
                    }

                    if(triedAllCountries){
                        break;
                    }

                    triedCountries.Add(countryToUse);

                    // Get the tracks for one of the allowed countries
                    // This might be an empty list depending on the country
                    validTracks                     = Tracks.GetTracks(countryToUse, gradeToUse);
                    // If we had some sort of track blacklist, it would be used here to remove from 'trackToUse'
                    if(null != blacklistedTracks){
                        foreach(Track track in blacklistedTracks){
                            if(validTracks.Contains(track)){
                                validTracks.Remove(track);
                            }
                        }
                    }
                    // Don't add a duplicate (except if we have no other option)
                    //if(validTracks.Count > tracksToUse.Count){
                    foreach(Track track in tracksToUse){
                        if(validTracks.Contains(track)){
                            validTracks.Remove(track);
                        }
                    }
                    //}
                // tried every country
                }

                // If validTracks is still empty, means we have no valid tracks so try again
                // If it is not empty, break out of the loop after assigning our track
                if(validTracks.Count > 0){
                    // Pick a random track from that list
                    trackToUse                      = validTracks[UnityEngine.Random.Range(0, validTracks.Count)];
                }
            // tried every grade
            }

            // If trackToUse is still null at this point, it means we couldn't generate a new unique track for an event entry
            if(trackToUse == null){
                // If tracksToUse also has 0 entries, means no track was valid so return null
                // Else, it means that some tracks were legal, but due to dupes we couldn't entirely fill it out, so simply fill it out with tracks it already has
                if(0 == tracksToUse.Count){
                    return null;
                }

                int maxTrackIndex       = tracksToUse.Count - 1;
                List<int> usedIndices   = new List<int>();
                int indexToUse          = -1;
                while(tracksToUse.Count < numEntries){
                    while(usedIndices.Contains(indexToUse) || -1 == indexToUse){
                        indexToUse = UnityEngine.Random.Range(0, maxTrackIndex + 1);
                    }
                    tracksToUse.Add(tracksToUse[indexToUse]);
                    usedIndices.Add(indexToUse);

                    // If we've used all the possible indices, restart the indices
                    if(usedIndices.Count == maxTrackIndex + 1){
                        usedIndices.Clear();
                    }
                }
            }
            else{
                usedCountries.Add(trackToUse.country);
                tracksToUse.Add(trackToUse);
            }
        // filled up our tracksToUse
        }

        Event newEvent = new Event(eventName, eventType, duration, parentEventSeriesParam, allowedTypes, allowedClasses, allowedBrands, allowedNames, topFameReward, topMoneyReward);

        // For each track, make an event entry
        foreach(Track track in tracksToUse){
            eventEntries.Add(EventEntry.GenerateNewEventEntry(track, duration, tier, newEvent, useLaps));
        }

        return newEvent;
    }

    public enum EventDuration
    {
        Mini = 0, // Around 5-7 minutes
        Short = 1, // Around 9-11 minutes
        Average = 2, // Around 15 minutes
        FairlyLong = 3, // Around 20-25 minutes
        Long = 4, // Around 30 minutes
        Endurance = 5 // 1 hour +
    }

    // Holds how long in minutes an event is supposed to take for each duration
    public static Dictionary<EventDuration, int>    eventDurationToExpectedMins = new Dictionary<EventDuration, int>
    {
        {EventDuration.Mini,            6},
        {EventDuration.Short,           11},
        {EventDuration.Average,         15},
        {EventDuration.FairlyLong,      23},
        {EventDuration.Long,            30},
        {EventDuration.Endurance,       60}
    };

    public static Dictionary<EventDuration, string> eventDurationToString       = new Dictionary<EventDuration, string>
    {
        {EventDuration.Mini,            "Mini"},
        {EventDuration.Short,           "Short"},
        {EventDuration.Average,         "Normal"},
        {EventDuration.FairlyLong,      "Fairly Long"},
        {EventDuration.Long,            "Long"},
        {EventDuration.Endurance,       "Endurance"}
    };

    public enum EventType
    {
        Race,
        Challenge,
        Special,
        Championship
    }

    public static Dictionary<EventType, string> eventTypeToString = new Dictionary<EventType, string>
    {
        {EventType.Race,            "Race"},
        {EventType.Challenge,       "Challenge"},
        {EventType.Special,         "Special"},
        {EventType.Championship,    "Championship"}
    };

    // Generates a number of event entries to use given an event type and duration
    private static int GetNumEventEntries(EventType eventType, EventDuration duration){
        switch(eventType){
            case EventType.Race:
                return 1;

            case EventType.Championship:
                switch(duration){
                    case EventDuration.Mini:
                        return 3;
                    case EventDuration.Short:
                        return 4;
                    case EventDuration.Average:
                        return 5;
                    case EventDuration.FairlyLong:
                        return 6;
                    case EventDuration.Long:
                        return 7;
                    case EventDuration.Endurance:
                        return UnityEngine.Random.Range(8, 11);
                }
                break;

            default:
                Debug.Log(eventType.ToString() + " event type has not been implemented yet!");
                return 0;
        }

        return 0;
    }

    // Generates a top fame reward given an event duration and tier
    private static int GenerateTopFameReward(EventDuration duration, EventSeriesManager.SeriesTier seriesTier){
        int baseReward      = EventSeriesManager.tierFameReward[seriesTier];
        float multiplier    = 1.0f + ((float)duration/2.0f);

        int rangeNum        = baseReward/3;

        int realReward      = baseReward + UnityEngine.Random.Range(-1*rangeNum, rangeNum+1);

        return (int)((float)realReward * multiplier);
    }
    // Generates a top money reward given an event duration and tier
    private static int GenerateTopMoneyReward(EventDuration duration, EventSeriesManager.SeriesTier seriesTier){
        int baseReward      = EventSeriesManager.tierMoneyReward[seriesTier];
        float multiplier    = 1.0f + ((float)duration/2.0f);

        int rangeNum        = baseReward/3;

        int realReward      = baseReward + UnityEngine.Random.Range(-1*rangeNum, rangeNum+1);

        return (int)((float)realReward * multiplier);
    }
}

public class EventEntry
{
    public Track                track;
    public int                  mins;
    public int                  laps;
    public bool                 attempted;
    // Whether this entry is the 'next up' to race in the event
    public bool                 nextUp;
    public Event                parentEvent;

    // Probably want AI levels and whatnot
    public int                  gridSize;

    // Will be filled in once the event entry is done
    public List<ResultDriver>   driverResults;
    public ResultDriver         playerResult;
    public Car                  playerCar;

    public EventEntry(Track trackParam, int gridSizeParam, Event parentEventParam, int minsParam = -1, int lapsParam = -1){
        track           = trackParam;
        gridSize        = gridSizeParam;
        mins            = minsParam;
        laps            = lapsParam;
        attempted       = false;
        nextUp          = false;
        parentEvent     = parentEventParam;

        driverResults   = new List<ResultDriver>();

        parentEvent.AddNewEventEntry(this);
    }

    public void CompleteEventEntry(List<ResultDriver> driverResultsParam, Car playerCarParam){
        Debug.Log("EventEntry complete!");
        playerCar   = playerCarParam;
        attempted   = true;
        nextUp      = false;

        driverResults   = driverResultsParam;
        foreach(ResultDriver driver in driverResultsParam){
            if(driver.IsPlayer){
                // Found player
                Debug.Log("Found player: " + driver.DriverLongName + ", driving a " + driver.CarName + " to P" + driver.FinishingPositionInClass.ToString());
                playerResult = driver;
                break;
            }
        }

        // Notify the Event that this entry is done, return whether this finished up that event
        parentEvent.EventEntryCompleted(this);
    }

    public int GetFameReward(){
        // Get some sort of fame reward just for racing
        return Mathf.CeilToInt((GetDistanceFameBonus() + GetFinishStatusFameBonus()) * GetSeriesTierRewardMultiplier());
    }
    public int GetDistanceFameBonus(){
        return (int) (playerResult.TotalDistance / 500);
    }
    public int GetFinishStatusFameBonus(){
        // Finished the race
        if(playerResult.FinishStatus == FinishStatus.Finished.ToString()){
            return 50;
        }
        if(playerResult.FinishStatus == FinishStatus.Dnf.ToString()){
            return 0;
        }
        Debug.LogError("Detected unknown finish status: " + playerResult.FinishStatus);
        return 25;
    }

    public int GetMoneyReward(){
        // Get some sort of money reward just for racing
        return Mathf.CeilToInt((GetDistanceMoneyBonus() + GetFinishStatusMoneyBonus()) * GetSeriesTierRewardMultiplier());
    }
    public int GetDistanceMoneyBonus(){
        return (int) (playerResult.TotalDistance / 50);
    }
    public int GetFinishStatusMoneyBonus(){
        // Finished the race
        if(playerResult.FinishStatus == FinishStatus.Finished.ToString()){
            return 500;
        }
        if(playerResult.FinishStatus == FinishStatus.Dnf.ToString()){
            return 0;
        }
        Debug.LogError("Detected unknown finish status: " + playerResult.FinishStatus);
        return 250;

    }
    public float GetSeriesTierRewardMultiplier(){
        // 2 decimal places
        return Mathf.Round(((int)parentEvent.parentEventSeries.seriesTier) / 8.0f * 100f) / 100f + 1.0f;
    }

    public string GetRewardInfo(){
        return
            "Player's Finish Status: " + playerResult.FinishStatus + " = $" +
                GetFinishStatusMoneyBonus().ToString("n0") + " and " + GetFinishStatusFameBonus().ToString("n0") + " Fame" +
            "\nDistance (KM) Traveled: " + playerResult.TotalDistance / 1000 + " = $" +
                GetDistanceMoneyBonus().ToString("n0") + " and " + GetDistanceFameBonus().ToString("n0") + " Fame"+
            "\n\nSeries Tier: " + EventSeriesManager.tierToString[parentEvent.parentEventSeries.seriesTier] + " = " +
                GetSeriesTierRewardMultiplier().ToString("n2") + "x Multiplier" +
            "\n\nTotal Rewards = $" + GetMoneyReward().ToString("n0") + " and " + GetFameReward().ToString("n0") + " Fame";
    }

    public string GetResults(){
        return "You started: P"     + playerResult.InitialPositionInClass.ToString() +
                "\nYou finished: P" + playerResult.FinishingPositionInClass.ToString() +
                "\nOn this track: " + track.GetPrintName() +
                "\nDriving the: "   + playerCar.GetPrintName();
    }

    public static EventEntry GenerateNewEventEntry(Track track, Event.EventDuration duration, EventSeriesManager.SeriesTier tier, Event parentEventParam, bool useLaps){
        const int MIN_LAPS                  = 3;

        // Get the grid size we shall use
        int gridSize                        = (int)(track.maxGridSize / 2.50f) + UnityEngine.Random.Range(-4, 5);

        // Either use laps or mins, depending on if useLaps is true or not
        if(useLaps){
            float           lapTime         = (float)track.kmLength / (float)EventSeriesManager.tierToAvgKMPerMinuteSpeed[tier];
            int             numLaps         = (int)(Event.eventDurationToExpectedMins[duration] / lapTime);

            if(numLaps < MIN_LAPS){
                numLaps = MIN_LAPS;
            }

            return new EventEntry(track, gridSize, parentEventParam, minsParam:-1, lapsParam:numLaps);
        }
        else{
            // In minutes
            int             numMins         = Event.eventDurationToExpectedMins[duration];
            return new EventEntry(track, gridSize, parentEventParam, minsParam:numMins, lapsParam:-1);
        }
    }
}