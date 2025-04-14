using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;
using System.Collections;

public class EventSeries
{
    // Name and tier together should make unique series
    public SeriesTier                       seriesTier;
    public string                           name;
    public string                           prefix;
    public string                           suffix;
    public List<Event>                      events;
    public Region.ClickableRegion           partOfRegion;
    public SeriesTheme                      seriesTheme;


    public void ConstructorCommonFunction
                        (
                            string nameParam, string prefixParam, string suffixParam, SeriesTier seriesTierParam, Region.ClickableRegion partOfRegionParam, SeriesTheme seriesThemeParam
                        )
    {
        name            = nameParam;
        prefix          = prefixParam;
        suffix          = suffixParam;

        seriesTier      = seriesTierParam;
        partOfRegion    = partOfRegionParam;
        events          = new List<Event>();
        seriesTheme     = seriesThemeParam;

        Region.regions[partOfRegion].AddNewSeries(this);
    }

    public EventSeries  (
                            string nameParam, SeriesTier seriesTierParam, Region.ClickableRegion partOfRegionParam, SeriesTheme seriesThemeParam
                        )
    {

        // For the name
        List<string> possiblePrefixes   = themeToSeriesPrefixes[seriesThemeParam];
        List<string> possibleSuffixes   = themeToSeriesSuffixes[seriesThemeParam];

        string prefix   = "";
        string suffix   = "";

        int prefixIndex = UnityEngine.Random.Range(0, possiblePrefixes.Count);
        int suffixIndex = UnityEngine.Random.Range(0, possibleSuffixes.Count);

        // For now, either have a suffix or a prefix
        if(prefixIndex < possiblePrefixes.Count && suffixIndex < possibleSuffixes.Count){
            // Both are valid indices, now make sure they are both 'non empty strings' so that we only choose one or the other when they both have a value
            if("" != possiblePrefixes[prefixIndex] && "" != possibleSuffixes[suffixIndex]){
                if(0 == UnityEngine.Random.Range(0, 2)){
                    prefixIndex = -1;
                }
                else{
                    suffixIndex = -1;
                }
            }
        }

        if(prefixIndex < possiblePrefixes.Count && prefixIndex >= 0)
        {
            prefix = possiblePrefixes[prefixIndex];
        }
        if(suffixIndex < possibleSuffixes.Count && suffixIndex >= 0)
        {
            suffix = possibleSuffixes[suffixIndex];
        }

        ConstructorCommonFunction(nameParam, prefix, suffix, seriesTierParam, partOfRegionParam, seriesThemeParam);
    }

    public EventSeries  (
                            string nameParam, string prefixParam, string suffixParam, SeriesTier seriesTierParam, Region.ClickableRegion partOfRegionParam,
                            SeriesTheme seriesThemeParam
                        )
    {
        ConstructorCommonFunction(nameParam, prefixParam, suffixParam, seriesTierParam, partOfRegionParam, seriesThemeParam);
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

    public string GetFullName()
    {
        return prefix + " " + name + " " + suffix;
    }

    public Sprite GetSprite()
    {
        string imageName = name.Replace(" ", "").Replace("-", "");
        Debug.Log("Looking for Series Background: " + imageName);
        return Resources.Load<Sprite>("Images/SeriesBackgrounds/" + imageName);
    }

    [System.Serializable]
    public enum SeriesTier
    {
        Rookie          = 0,
        Novice          = 1,
        Amateur         = 2,
        Professional    = 3,
        Elite           = 4,
        Master          = 5,
        Prodigy         = 6,
        Legend          = 7,
        WorldRenowned   = 8
    };

    public static Dictionary<SeriesTier, string> tierToString                               = new Dictionary<SeriesTier, string>
    {
        {SeriesTier.Rookie,             "Rookie"},
        {SeriesTier.Novice,             "Novice"},
        {SeriesTier.Amateur,            "Amateur"},
        {SeriesTier.Professional,       "Professional"},
        {SeriesTier.Elite,              "Elite"},
        {SeriesTier.Master,             "Master"},
        {SeriesTier.Prodigy,            "Prodigy"},
        {SeriesTier.Legend,             "Legend"},
        {SeriesTier.WorldRenowned,      "World-Renowned"}
    };

    // Holds the default duration of an event for the given tier
    public static Dictionary<SeriesTier, Event.EventDuration>    tierDefaultDuration        = new Dictionary<SeriesTier, Event.EventDuration>
    {
        {SeriesTier.Rookie,             Event.EventDuration.Mini},
        {SeriesTier.Novice,             Event.EventDuration.Mini},
        {SeriesTier.Amateur,            Event.EventDuration.Short},
        {SeriesTier.Professional,       Event.EventDuration.Short},
        {SeriesTier.Elite,              Event.EventDuration.Average},
        {SeriesTier.Master,             Event.EventDuration.Average},
        {SeriesTier.Prodigy,            Event.EventDuration.FairlyLong},
        {SeriesTier.Legend,             Event.EventDuration.FairlyLong},
        {SeriesTier.WorldRenowned,      Event.EventDuration.Long}
    };

    public static Dictionary<SeriesTier, List<Event.EventDuration>> tierDurationWhitelist   = new Dictionary<SeriesTier, List<Event.EventDuration>>
    {
        {SeriesTier.Rookie,             new List<Event.EventDuration>() { {Event.EventDuration.Mini}, {Event.EventDuration.Short}, {Event.EventDuration.Average} } },
        {SeriesTier.Novice,             new List<Event.EventDuration>() { {Event.EventDuration.Mini}, {Event.EventDuration.Short}, {Event.EventDuration.Average} } },
        {SeriesTier.Amateur,            new List<Event.EventDuration>() { {Event.EventDuration.Mini}, {Event.EventDuration.Short}, {Event.EventDuration.Average} } },
        {SeriesTier.Professional,       new List<Event.EventDuration>() { {Event.EventDuration.Mini}, {Event.EventDuration.Short}, {Event.EventDuration.Average}, {Event.EventDuration.FairlyLong} } },
        {SeriesTier.Elite,              new List<Event.EventDuration>() { {Event.EventDuration.Mini}, {Event.EventDuration.Short}, {Event.EventDuration.Average}, {Event.EventDuration.FairlyLong} } },
        {SeriesTier.Master,             new List<Event.EventDuration>() { {Event.EventDuration.Mini}, {Event.EventDuration.Short}, {Event.EventDuration.Average}, {Event.EventDuration.FairlyLong}, {Event.EventDuration.Long} } },
        {SeriesTier.Prodigy,            new List<Event.EventDuration>() { {Event.EventDuration.Mini}, {Event.EventDuration.Short}, {Event.EventDuration.Average}, {Event.EventDuration.FairlyLong}, {Event.EventDuration.Long} } },
        {SeriesTier.Legend,             new List<Event.EventDuration>() { {Event.EventDuration.Mini}, {Event.EventDuration.Short}, {Event.EventDuration.Average}, {Event.EventDuration.FairlyLong}, {Event.EventDuration.Long}, {Event.EventDuration.Endurance} } },
        {SeriesTier.WorldRenowned,      new List<Event.EventDuration>() { {Event.EventDuration.Mini}, {Event.EventDuration.Short}, {Event.EventDuration.Average}, {Event.EventDuration.FairlyLong}, {Event.EventDuration.Long}, {Event.EventDuration.Endurance} } }
    };

    // Used for race length calculations
    public static Dictionary<SeriesTier, int> tierToAvgKMPerMinuteSpeed = new Dictionary<SeriesTier, int>
    {
        {SeriesTier.Rookie,             100/60},
        {SeriesTier.Novice,             105/60},
        {SeriesTier.Amateur,            110/60},
        {SeriesTier.Professional,       115/60},
        {SeriesTier.Elite,              120/60},
        {SeriesTier.Master,             125/60},
        {SeriesTier.Prodigy,            130/60},
        {SeriesTier.Legend,             135/60},
        {SeriesTier.WorldRenowned,      140/60}
    };

    // Set restrictions on what grade tracks each tier drivers can race on
    public static Dictionary<SeriesTier, List<Tracks.Grade>> tierAllowedOnGrade = new Dictionary<SeriesTier, List<Tracks.Grade>>
    {
        {SeriesTier.Rookie,             new List<Tracks.Grade>() { {Tracks.Grade.Four}, {Tracks.Grade.Three}, {Tracks.Grade.Historic} } },
        {SeriesTier.Novice,             new List<Tracks.Grade>() { {Tracks.Grade.Four}, {Tracks.Grade.Three}, {Tracks.Grade.Historic} } },
        {SeriesTier.Amateur,            new List<Tracks.Grade>() { {Tracks.Grade.Four}, {Tracks.Grade.Three}, {Tracks.Grade.Historic}, {Tracks.Grade.Temporary} } },
        {SeriesTier.Professional,       new List<Tracks.Grade>() { {Tracks.Grade.Four}, {Tracks.Grade.Three}, {Tracks.Grade.Historic}, {Tracks.Grade.Temporary}, {Tracks.Grade.Two} } },
        {SeriesTier.Elite,              new List<Tracks.Grade>() { {Tracks.Grade.Three}, {Tracks.Grade.Two}, {Tracks.Grade.Historic} } },
        {SeriesTier.Master,             new List<Tracks.Grade>() { {Tracks.Grade.Three}, {Tracks.Grade.One}, {Tracks.Grade.Historic}, {Tracks.Grade.Two} } },
        {SeriesTier.Prodigy,            new List<Tracks.Grade>() { {Tracks.Grade.One}, {Tracks.Grade.Historic}, {Tracks.Grade.Two} } },
        {SeriesTier.Legend,             new List<Tracks.Grade>() { {Tracks.Grade.One}, {Tracks.Grade.Historic}, {Tracks.Grade.Two} } },
        {SeriesTier.WorldRenowned,      new List<Tracks.Grade>() { {Tracks.Grade.One}, {Tracks.Grade.Historic}}}
    };

    // Set base fame pay for each tier (assume a single race event)
    public static Dictionary<SeriesTier, int> tierFameReward = new Dictionary<SeriesTier, int>
    {
        {SeriesTier.Rookie,             18},
        {SeriesTier.Novice,             23},
        {SeriesTier.Amateur,            31},
        {SeriesTier.Professional,       41},
        {SeriesTier.Elite,              50},
        {SeriesTier.Master,             58},
        {SeriesTier.Prodigy,            66},
        {SeriesTier.Legend,             75},
        {SeriesTier.WorldRenowned,      90},
    };
    // Set base money pay for each tier (assume a single race event)
    public static Dictionary<SeriesTier, int> tierMoneyReward = new Dictionary<SeriesTier, int>
    {
        {SeriesTier.Rookie,             1150},
        {SeriesTier.Novice,             1400},
        {SeriesTier.Amateur,            1620},
        {SeriesTier.Professional,       1975},
        {SeriesTier.Elite,              2150},
        {SeriesTier.Master,             2400},
        {SeriesTier.Prodigy,            2725},
        {SeriesTier.Legend,             3100},
        {SeriesTier.WorldRenowned,      3550},
    };

    // When you add a theme here, make sure to edit logic in GenerateNewEventEntry to add in logic for this theme (what kind of entries it produces)
    // Also update the below dictionaries
    [System.Serializable]
    public enum SeriesTheme
    {
        Normal      = 0,
        Rainy       = 1,
        Foggy       = 2,
        Stormy      = 3,
        Night       = 4
    };

    // Prefix lists for each theme (for series)
    public static Dictionary<SeriesTheme, List<string>> themeToSeriesPrefixes   = new Dictionary<SeriesTheme, List<string>>
    {
        {SeriesTheme.Normal,        new List<string> { "" } },
        {SeriesTheme.Rainy,         new List<string> { "In the Rain with", "Rainy", "Drizzled", "Soaked Streets" } },
        {SeriesTheme.Foggy,         new List<string> { "Foggy", "Shrouded" } },
        {SeriesTheme.Stormy,        new List<string> { "Thunderous" } },
        {SeriesTheme.Night,         new List<string> { "Pitch Black", "Under the Moon" } }
    };

    // Suffix lists for each theme (for series)
    public static Dictionary<SeriesTheme, List<string>> themeToSeriesSuffixes   = new Dictionary<SeriesTheme, List<string>>
    {
        {SeriesTheme.Normal,        new List<string> { "Showdown", "Madness", "Face-Off", "Wrangle", "Warfare", "Contest" } },
        {SeriesTheme.Rainy,         new List<string> { "Rainfest", "Rainmasters" } },
        {SeriesTheme.Foggy,         new List<string> { "in the Fog", "Through Fog" } },
        {SeriesTheme.Stormy,        new List<string> { "Stormfest", "Maelstrom" } },
        {SeriesTheme.Night,         new List<string> { "at Night", "In the Dark", "After Hours" } }
    };

    // Prefix lists for each theme (for events)
    public static Dictionary<SeriesTheme, Dictionary<Event.EventType, List<string>>> themeToEventPrefixes   = new Dictionary<SeriesTheme, Dictionary<Event.EventType, List<string>>>
    {
        {SeriesTheme.Normal,        new Dictionary<Event.EventType, List<string>>
            {
                { Event.EventType.Race,             new List<string> { "Weekend Race -", "Trackday Competition -", "Sprint Clash -", "Trackday Throwdown -", "Weekend Burnout -" } },
                { Event.EventType.Championship,     new List<string> { "Weekend Championship -", "Contender's Circuit -", "Apex Cup -", "Driving Series -", "Burning Rubber League -", "Full Throttle Championship -" } }
            }
        },

        {SeriesTheme.Foggy,         new Dictionary<Event.EventType, List<string>>
            {
                { Event.EventType.Race,             new List<string> { "Foggy Race -", "Race in the Fog -" } },
                { Event.EventType.Championship,     new List<string> { "Foggy Championship -", "Championship in the Fog -", "Masters of the Mist -", "Haze Championship -" } }
            }
        },

        {SeriesTheme.Night,         new Dictionary<Event.EventType, List<string>>
            {
                { Event.EventType.Race,             new List<string> { "Race at Night -", "Midnight Sprint -" } },
                { Event.EventType.Championship,     new List<string> { "Dark Championship -", "Obscure Championship -", "Championship at Night -", "Nightfall Series -", "Lunar League -" } }
            }
        },

        {SeriesTheme.Rainy,         new Dictionary<Event.EventType, List<string>>
            {
                { Event.EventType.Race,             new List<string> { "Slippery Race -", "Rainy Race -", "Wet Road Challenge -" } },
                { Event.EventType.Championship,     new List<string> { "In the Rain Championship -", "Championship in the Wet -", "Rainmasters Series -", "Cascade Cup -" } }
            }
        },

        {SeriesTheme.Stormy,        new Dictionary<Event.EventType, List<string>>
            {
                { Event.EventType.Race,             new List<string> { "Stormy Race -", "Race in the Storms -", "Lightning Clash -", "Electric Sprint -" } },
                { Event.EventType.Championship,     new List<string> { "Stormy Championship -", "Championship in the Storms -", "Shockwave Series -" } }
            }
        },
    };

    // Suffix lists for each theme (for events)
    public static Dictionary<SeriesTheme, Dictionary<Event.EventType, List<string>>> themeToEventSuffixes   = new Dictionary<SeriesTheme, Dictionary<Event.EventType, List<string>>>
    {
        {SeriesTheme.Normal,        new Dictionary<Event.EventType, List<string>>
            {
                { Event.EventType.Race,             new List<string> { "" } },
                { Event.EventType.Championship,     new List<string> { "" } }
            }
        },

        {SeriesTheme.Foggy,         new Dictionary<Event.EventType, List<string>>
            {
                { Event.EventType.Race,             new List<string> { "Through the Drizzle" } },
                { Event.EventType.Championship,     new List<string> { "" } }
            }
        },

        {SeriesTheme.Night,         new Dictionary<Event.EventType, List<string>>
            {
                { Event.EventType.Race,             new List<string> { "Under the Moonlight" } },
                { Event.EventType.Championship,     new List<string> { "Night Championship" } }
            }
        },

        {SeriesTheme.Rainy,         new Dictionary<Event.EventType, List<string>>
            {
                { Event.EventType.Race,             new List<string> { "" } },
                { Event.EventType.Championship,     new List<string> { "" } }
            }
        },

        {SeriesTheme.Stormy,        new Dictionary<Event.EventType, List<string>>
            {
                { Event.EventType.Race,             new List<string> { "" } },
                { Event.EventType.Championship,     new List<string> { "" } }
            }
        },
    };
}

public class Event
{
    public string                   name;
    public string                   prefix;
    public string                   suffix;
    public EventType                eventType;
    public EventDuration            eventDuration;
    public EventSeries              parentEventSeries;

    public List<Cars.CarType>       typeWhitelist;
    public List<Cars.CarClass>      classWhitelist;
    public List<Cars.CarBrand>      brandWhitelist;
    public List<string>             nameWhitelist;

    public int                      topFameReward;
    public int                      topMoneyReward;
    public int                      finishPosition;
    public int                      gridSize;
    public bool                     completed;

    public List<EventEntry>         eventEntries;
    public Dictionary<string, int>  champhionshipPoints;

    private void ConstructorCommonFunction
                (
                    string nameParam, string prefixParam, string suffixParam, EventType eventTypeParam, EventDuration eventDurationParam, EventSeries parentEventSeriesParam,
                    List<Cars.CarType> allowedTypesParam, List<Cars.CarClass> allowedClassesParam, List<Cars.CarBrand> allowedBrandsParam, List<string> allowedNamesParam,
                    int topFameRewardParam, int topMoneyRewardParam
                )
    {
        champhionshipPoints = new Dictionary<string, int>();

        name                = nameParam;
        prefix              = prefixParam;
        suffix              = suffixParam;

        eventType           = eventTypeParam;
        eventDuration       = eventDurationParam;
        parentEventSeries   = parentEventSeriesParam;

        topFameReward       = topFameRewardParam;
        topMoneyReward      = topMoneyRewardParam;

        typeWhitelist       = allowedTypesParam;
        classWhitelist      = allowedClassesParam;
        brandWhitelist      = allowedBrandsParam;
        nameWhitelist       = allowedNamesParam;
        completed           = false;
        gridSize            = -1;

        // Will be filled in by newly instantiated event entries
        eventEntries        = new List<EventEntry>();

        parentEventSeries.events.Add(this);
    }

    public Event(
                    string nameParam, string prefixParam, string suffixParam, EventType eventTypeParam, EventDuration eventDurationParam, EventSeries parentEventSeriesParam,
                    List<Cars.CarType> allowedTypesParam, List<Cars.CarClass> allowedClassesParam, List<Cars.CarBrand> allowedBrandsParam, List<string> allowedNamesParam,
                    int topFameRewardParam, int topMoneyRewardParam
                ){
        ConstructorCommonFunction   (   nameParam, prefixParam, suffixParam, eventTypeParam, eventDurationParam, parentEventSeriesParam,
                                        allowedTypesParam, allowedClassesParam, allowedBrandsParam, allowedNamesParam, topFameRewardParam, topMoneyRewardParam
                                    );
    }

    public Event(
                    string nameParam, EventType eventTypeParam, EventDuration eventDurationParam, EventSeries parentEventSeriesParam,
                    List<Cars.CarType> allowedTypesParam, List<Cars.CarClass> allowedClassesParam, List<Cars.CarBrand> allowedBrandsParam, List<string> allowedNamesParam,
                    int topFameRewardParam, int topMoneyRewardParam
                ){
        // For the name
        List<string> possiblePrefixes   = EventSeries.themeToEventPrefixes[parentEventSeriesParam.seriesTheme][eventTypeParam];
        List<string> possibleSuffixes   = EventSeries.themeToEventSuffixes[parentEventSeriesParam.seriesTheme][eventTypeParam];

        string prefix   = "";
        string suffix   = "";

        int prefixIndex = UnityEngine.Random.Range(0, possiblePrefixes.Count);
        int suffixIndex = UnityEngine.Random.Range(0, possibleSuffixes.Count);

        // For now, either have a suffix or a prefix
        if(prefixIndex < possiblePrefixes.Count && suffixIndex < possibleSuffixes.Count){
            // Both are valid indices, now make sure they are both 'non empty strings' so that we only choose one or the other when they both have a value
            if("" != possiblePrefixes[prefixIndex] && "" != possibleSuffixes[suffixIndex]){
                if(0 == UnityEngine.Random.Range(0, 2)){
                    prefixIndex = -1;
                }
                else{
                    suffixIndex = -1;
                }
            }
        }

        if(prefixIndex < possiblePrefixes.Count && prefixIndex >= 0)
        {
            prefix = possiblePrefixes[prefixIndex];
        }
        if(suffixIndex < possibleSuffixes.Count && suffixIndex >= 0)
        {
            suffix = possibleSuffixes[suffixIndex];
        }

        ConstructorCommonFunction   (   nameParam, prefix, suffix, eventTypeParam, eventDurationParam, parentEventSeriesParam,
                                        allowedTypesParam, allowedClassesParam, allowedBrandsParam, allowedNamesParam, topFameRewardParam, topMoneyRewardParam
                                    );
    }

    public string GetFullName()
    {
        return prefix + " " + name + " " + suffix;
    }

    public void EventEntryCompleted(EventEntry eventEntryParam){
        int nextUpIndex = eventEntries.IndexOf(eventEntryParam) + 1;

        // Depending on event type, something happens after an event entry is completed
        switch(eventType){
            case EventType.Championship:
                UpdateChamphionshipPoints(eventEntryParam.driverResults);
                break;
            default:
                break;
        }

        // Completed the event
        if(nextUpIndex >= eventEntries.Count){
            Debug.Log("Event: " + name + " is complete!");
            // Depending on the event type, calculate final position
            switch(eventType){
                case EventType.Race:
                    finishPosition  = eventEntryParam.playerResult.FinishingPositionInClass;
                    break;
                case EventType.Championship:
                    // Calculate finish position
                    finishPosition  = GetChampionshipFinishPosition();
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
        // If it is the first event entry to be added, set it to that event entry's grid size
        if(eventEntries.Count == 0){
            gridSize = entryToAdd.gridSize;
        }
        else{
            // If this is a championship, make the lowest grid sized eventEntry the total gridSize for all entries (and for this event in general)
            switch(eventType){
                case EventType.Championship:
                    // Assume current gridSize is the highest, so just check this one
                    if(entryToAdd.gridSize > gridSize){
                        // If it is a new high, edit the other event entries within this event to match this one (since a championship needs the same amount of drivers in every race)
                        gridSize = entryToAdd.gridSize;

                        foreach(EventEntry entry in eventEntries){
                            entry.gridSize = gridSize;
                        }
                    }
                    else{
                        // If it's lower (or equal), set the gridSize to the event's gridSize
                        entryToAdd.gridSize = gridSize;
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

    // Will only return anything useful if at least one eventEntry has been completed from this event
    public string GetPrintStandings(){
        string toReturn     = "";

        if(!eventEntries[0].attempted){
            return toReturn;
        }

        string playerName   = eventEntries[0].playerResult.DriverLongName;

        // Holds finishing positions
        List<int> points                            = new List<int>();
        // Matches finish pos to driver long name
        Dictionary<int, List<string>> pointsToName  = new Dictionary<int, List<string>>();

        foreach(KeyValuePair<string, int> entry in champhionshipPoints){
            // First, need a sorted list of all points
            // No dupes in points else we will see duplicate names
            if(!points.Contains(entry.Value)){
                points.Add(entry.Value);
            }

            if(!pointsToName.ContainsKey(entry.Value)){
                pointsToName[entry.Value] = new List<string>();
            }
            pointsToName[entry.Value].Add(entry.Key);
        }
        // Sort in descending order
        points = points.OrderByDescending(i => i).ToList();

        int count = 1;

        foreach(int pointEntry in points){
            foreach(string driverName in pointsToName[pointEntry]){
                // If this is the player, add a (You) suffix
                string suffix       = driverName == playerName ? " (You)" : "";
                toReturn += count.ToString() + ". " + driverName + suffix + " - " + pointEntry.ToString() + " Points\n";
            }
            count += pointsToName[pointEntry].Count;
        }

        return toReturn;
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
            "\n\nSeries Tier: "         + EventSeries.tierToString[parentEventSeries.seriesTier] +
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

    public Sprite GetSprite(){
        string imageName = eventTypeToString[eventType] + "_Event";
        Debug.Log("Looking for Event Background: " + imageName);
        return Resources.Load<Sprite>("Images/EventBackgrounds/" + imageName);
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
        EventSeries.SeriesTier          tier            = parentEventSeriesParam.seriesTier;
        Track                           trackToUse      = null;
        List<Tracks.Grade>              allowedGrades   = EventSeries.tierAllowedOnGrade[tier];

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

        int topFameReward                               = GenerateTopFameReward(duration, parentEventSeriesParam.seriesTier, parentEventSeriesParam.partOfRegion) * numEntries;
        int topMoneyReward                              = GenerateTopMoneyReward(duration, parentEventSeriesParam.seriesTier, parentEventSeriesParam.partOfRegion) * numEntries;

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

        Event newEvent = new Event
                                (
                                    eventName, eventType, duration, parentEventSeriesParam, allowedTypes, allowedClasses,
                                    allowedBrands, allowedNames, topFameReward, topMoneyReward
                                );

        // For each track, make an event entry
        foreach(Track track in tracksToUse){
            eventEntries.Add(EventEntry.GenerateNewEventEntry(track, duration, tier, newEvent, useLaps, parentEventSeriesParam.seriesTheme));
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
    public static Dictionary<EventDuration, int>    eventDurationToExpectedMins         = new Dictionary<EventDuration, int>
    {
        {EventDuration.Mini,            6},
        {EventDuration.Short,           11},
        {EventDuration.Average,         15},
        {EventDuration.FairlyLong,      23},
        {EventDuration.Long,            30},
        {EventDuration.Endurance,       60}
    };

    public static Dictionary<EventDuration, string> eventDurationToString               = new Dictionary<EventDuration, string>
    {
        {EventDuration.Mini,            "Mini"},
        {EventDuration.Short,           "Short"},
        {EventDuration.Average,         "Normal"},
        {EventDuration.FairlyLong,      "Fairly Long"},
        {EventDuration.Long,            "Long"},
        {EventDuration.Endurance,       "Endurance"}
    };

    // Whenever you add a new type, edit the "themeToEvent..." dictionaries for suffixes and prefixes
    public enum EventType
    {
        Race,
        Challenge,
        Special,
        Championship
    };

    public static Dictionary<EventType, string> eventTypeToString = new Dictionary<EventType, string>
    {
        {EventType.Race,            "Race"},
        {EventType.Challenge,       "Challenge"},
        {EventType.Special,         "Special"},
        {EventType.Championship,    "Championship"}
    };

    public static Dictionary<int, int> pointsDict = new Dictionary<int, int>() {
        {1,     25},
        {2,     18},
        {3,     15},
        {4,     12},
        {5,     10},
        {6,     8},
        {7,     6},
        {8,     4},
        {9,     2},
        {10,    1},
        {0,     0}
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
    private static int GenerateTopFameReward(EventDuration duration, EventSeries.SeriesTier seriesTier, Region.ClickableRegion region){
        int baseReward      = EventSeries.tierFameReward[seriesTier];
        float multiplier    = 1.0f + ((float)duration/2.0f) + Region.GetAddedRewardMultiplierForRegion(region);

        int rangeNum        = baseReward/3;

        int realReward      = baseReward + UnityEngine.Random.Range(-1*rangeNum, rangeNum+1);

        return (int)((float)realReward * multiplier);
    }
    // Generates a top money reward given an event duration and tier
    private static int GenerateTopMoneyReward(EventDuration duration, EventSeries.SeriesTier seriesTier, Region.ClickableRegion region){
        int baseReward      = EventSeries.tierMoneyReward[seriesTier];
        float multiplier    = 1.0f + ((float)duration/2.0f) + Region.GetAddedRewardMultiplierForRegion(region);

        int rangeNum        = baseReward/3;

        int realReward      = baseReward + UnityEngine.Random.Range(-1*rangeNum, rangeNum+1);

        return (int)((float)realReward * multiplier);
    }

    private void UpdateChamphionshipPoints(List<ResultDriver> drivers){
        int toAdd = 0;
        // Update champhionship points
        foreach(ResultDriver driver in drivers){
            if(!champhionshipPoints.ContainsKey(driver.DriverLongName)){
                champhionshipPoints[driver.DriverLongName] = 0;
            }
            toAdd = driver.FinishingPositionInClass > 10 ? 0 : pointsDict[driver.FinishingPositionInClass];
            champhionshipPoints[driver.DriverLongName] = champhionshipPoints[driver.DriverLongName] + toAdd;
        }
    }

    private int GetChampionshipFinishPosition(){
        string playerName   = eventEntries[0].playerResult.DriverLongName;

        List<int> points    = new List<int>();
        int playerPoints    = -1;

        foreach(KeyValuePair<string, int> entry in champhionshipPoints){
            if(playerName == entry.Key){
                // Is player, so store the points
                playerPoints = entry.Value;
            }

            points.Add(entry.Value);
        }

        if(-1 == playerPoints){
            Debug.LogError("Could not find player's champhionship points!");
            return gridSize;
        }

        // Descending order
        points = points.OrderByDescending(i => i).ToList();
        return points.IndexOf(playerPoints) + 1;
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

    // More race settings
    public string               startTime;

    // Can be 1, 2, 5, 10, 15, 20, 25, 30, 40, 50, 60
    public int                  timeProgression;

    // False == rolling start
    public bool                 standingStart;

    // Weather slots == 0 means Real Weather
    public int                  weatherSlots;

    // Length of weatherForecast should match weatherSlots
    // Current weatherForecast options:
    //Clear, light cloud, medium cloud, heavy cloud, overcast
    //light rain, rain, storm, thunderstorm, fog with rain, heavy fog with rain
    //fog, heavy fog, hazy
    public List<string>         weatherForecast;

    // Whether a pit stop is mandatory
    public bool                 mandatoryPitStop;

    // 0 - 4
    public int                  pitStopMinTyres;

    public OpponentFieldType    fieldType;

    // Will be filled in once the event entry is done
    public List<ResultDriver>   driverResults;
    public ResultDriver         playerResult;
    public Car                  playerCar;

    // Will be used for rewards
    public float                totalDistanceTraveled;


    public EventEntry(  Track trackParam, int gridSizeParam, Event parentEventParam, string startTimeParam, int timeProgressionParam, bool standingStartParam,
                        List<string> weatherForecastParam, bool mandatoryPitStopParam, int pitStopMinTyresParam, OpponentFieldType fieldTypeParam,
                        int minsParam = -1, int lapsParam = -1 )
    {
        track                   = trackParam;
        gridSize                = gridSizeParam;
        mins                    = minsParam;
        laps                    = lapsParam;
        attempted               = false;
        nextUp                  = false;
        parentEvent             = parentEventParam;
        playerCar               = null;
        totalDistanceTraveled   = 0.0f;

        driverResults           = new List<ResultDriver>();

        startTime               = startTimeParam;
        timeProgression         = timeProgressionParam;
        standingStart           = standingStartParam;
        weatherForecast         = weatherForecastParam;
        weatherSlots            = weatherForecast.Count;
        mandatoryPitStop        = mandatoryPitStopParam;
        pitStopMinTyres         = pitStopMinTyresParam;
        fieldType               = fieldTypeParam;

        parentEvent.AddNewEventEntry(this);
    }

    public void CompleteEventEntry(List<RaceResult> resultsParam, Car playerCarParam){
        Debug.Log("EventEntry complete!");
        ResultDriver curDriver  = null;
        playerCar               = playerCarParam;

        int bestFinish          = -1;
        foreach(RaceResult result in resultsParam){

            // Look for the player within this result
            foreach(ResultDriver driver in result.Drivers){
                driverResults.Add(driver);

                if(driver.IsPlayer){
                    // Found player
                    Debug.Log("Found player: " + driver.DriverLongName + ", driving a " + driver.CarName + " to P" + driver.FinishingPositionInClass.ToString());
                    curDriver = driver;
                    // In meters (as it comes in second monitor)
                    totalDistanceTraveled += curDriver.TotalDistance;
                }
            }

            // Check if this should be the stored result
            if(curDriver.FinishingPositionInClass < bestFinish || -1 == bestFinish){
                playerResult = curDriver;
                bestFinish   = playerResult.FinishingPositionInClass;
            }
        }

        attempted       = true;
        nextUp          = false;

        // Notify the Event that this entry is done
        parentEvent.EventEntryCompleted(this);
    }

    public int GetFameReward(){
        // Get some sort of fame reward just for racing
        return Mathf.CeilToInt((GetDistanceFameBonus() + GetFinishStatusFameBonus()) * GetSeriesTierRewardMultiplier());
    }
    public int GetDistanceFameBonus(){
        return (int) (totalDistanceTraveled / 1250);
    }
    public int GetFinishStatusFameBonus(){
        // Finished the race
        if(playerResult.FinishStatus == FinishStatus.Finished.ToString()){
            return 50;
        }
        if(playerResult.FinishStatus == FinishStatus.Dnf.ToString()){
            return 0;
        }
        Debug.LogWarning("Detected unknown finish status: " + playerResult.FinishStatus);
        return 25;
    }

    public int GetMoneyReward(){
        // Get some sort of money reward just for racing
        return Mathf.CeilToInt((GetDistanceMoneyBonus() + GetFinishStatusMoneyBonus()) * GetSeriesTierRewardMultiplier());
    }
    public int GetDistanceMoneyBonus(){
        return (int) (totalDistanceTraveled / 60);
    }
    public int GetFinishStatusMoneyBonus(){
        // Finished the race
        if(playerResult.FinishStatus == FinishStatus.Finished.ToString()){
            return 500;
        }
        if(playerResult.FinishStatus == FinishStatus.Dnf.ToString()){
            return 0;
        }
        Debug.LogWarning("Detected unknown finish status: " + playerResult.FinishStatus);
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
            "\nDistance (KM) Traveled (including restarts): " + totalDistanceTraveled / 1000 + " = $" +
                GetDistanceMoneyBonus().ToString("n0") + " and " + GetDistanceFameBonus().ToString("n0") + " Fame"+
            "\n\nSeries Tier: " + EventSeries.tierToString[parentEvent.parentEventSeries.seriesTier] + " = " +
                GetSeriesTierRewardMultiplier().ToString("n2") + "x Multiplier" +
            "\n\nTotal Rewards = $" + GetMoneyReward().ToString("n0") + " and " + GetFameReward().ToString("n0") + " Fame";
    }

    public string GetResults(){
        return "You started: P"     + playerResult.InitialPositionInClass.ToString() +
                "\nYou finished: P" + playerResult.FinishingPositionInClass.ToString() +
                "\nOn this track: " + track.GetPrintName() +
                "\nDriving the: "   + playerCar.GetPrintName();
    }

    public string GetPrintResults(){
        switch(parentEvent.eventType){
            case Event.EventType.Championship:
                // Used as a replacement for '0' (dnf) in the ascending order list
                const int ZERO                          = 999;

                string toReturn                         = "";
                // Holds finishing positions
                List<int> finishingPositions            = new List<int>();
                // Matches finish pos to driver long name
                Dictionary<int, List<string>> posToName = new Dictionary<int, List<string>>();
                int posToAdd;

                foreach(ResultDriver driver in driverResults){
                    // First, need a sorted list of all finishing positions (need to account for dupes (multiclass))
                    posToAdd                            = driver.FinishingPositionInClass == 0 ? ZERO : driver.FinishingPositionInClass;

                    // Add finishing pos if not already in our list
                    if(!finishingPositions.Contains(posToAdd))
                    {
                        finishingPositions.Add(posToAdd);
                    }

                    if(!posToName.ContainsKey(posToAdd))
                    {
                        posToName[posToAdd] = new List<string>();
                    }
                    posToName[posToAdd].Add(driver.DriverLongName);
                }
                // Sort in ascending order
                finishingPositions  = finishingPositions.OrderBy(i => i).ToList();

                int driverNum       = 0;
                foreach(int finishPos in finishingPositions)
                {
                    foreach(string driverName in posToName[finishPos])
                    {
                        driverNum++;

                        // If this is the player, add a (You) suffix
                        string suffix       = driverName == playerResult.DriverLongName ? " (You)" : "";
                        int gainedPoints    = finishPos > 10 ? 0 : Event.pointsDict[finishPos];
                        toReturn            += "P" + finishPos.ToString() + ": " + driverName + suffix + " = " + gainedPoints.ToString() + " Points";

                        // To save space, alternate newline and separator if gridSize is too high
                        if((0 == driverNum % 2) && gridSize >= 30)
                        {
                            toReturn        += " | ";
                        }
                        else
                        {
                            toReturn        += "\n";
                        }
                    }
                }
                return toReturn;

            default:
                return "GetPrintResults for event type: " + parentEvent.eventType.ToString() + " is Not Implemented";
        }
    }

    public static EventEntry GenerateNewEventEntry(Track track, Event.EventDuration duration, EventSeries.SeriesTier tier, Event parentEventParam, bool useLaps, EventSeries.SeriesTheme theme){
        const int MIN_LAPS                  = 3;

        // Get the grid size we shall use, minimum of 12, except if the maxGridSize is lower
        int gridSize                        = Mathf.Max((int)(track.maxGridSize / 2.50f) + UnityEngine.Random.Range(1, 6), 12);
        if(gridSize > track.maxGridSize){
            gridSize = track.maxGridSize;
        }

        // Calculate the race parameters: startTime, timeProgression, standingStart, weatherSlots, weatherForecast, mandatoryPitStop, pitStopMinTyres, fieldType
        // Set some defaults
        string              startTime           = "10:00";
        int                 timeProgression     = 1;
        bool                standingStart       = true;
        int                 weatherSlots        = 1;
        List<string>        weatherForecast     = new List<string>();
        bool                mandatoryPitStop    = false;
        int                 pitStopMinTyres     = 0;

        // Special value to be used if we need to override weather (if a theme requires special weather and weatherSlots == 0)
        int                 specialWeatherSlots = weatherSlots;

        // For now, have same class opponents
        // TODO: With later updates when new events are in and multiclassing is in, will have to update how this is decided
        OpponentFieldType   fieldType           = OpponentFieldType.SameClass;


        int choice  = UnityEngine.Random.Range(1, 101);

        // startTime
        int intTime = 0;
        if(choice <= 80){
            // Normal
            intTime = UnityEngine.Random.Range(11, 17);
        }
        else if(choice <= 85){
            // Early morning
            intTime = UnityEngine.Random.Range(3, 7);
        }
        else if(choice <= 90){
            // Morning
            intTime = UnityEngine.Random.Range(6, 10);
        }
        else if(choice <= 95){
            // Evening
            intTime = UnityEngine.Random.Range(17, 21);
        }
        else{
            // Night
            intTime = UnityEngine.Random.Range(22, 27);
        }

        // timeProgression
        switch(duration){
            case Event.EventDuration.Mini:
            case Event.EventDuration.Short:
                // 75% chance for 'Real Weather', meaning set weatherSlots to 0
                choice                      = UnityEngine.Random.Range(1, 101);

                if(choice > 25){
                    weatherSlots            = 0;
                }
                break;

            case Event.EventDuration.Average:
                choice                      = UnityEngine.Random.Range(1, 101);

                if(choice > 90){
                    // 10% chance for a pit stop race
                    timeProgression         = 10;
                    weatherSlots            = 2;
                    mandatoryPitStop        = true;
                }

                // 15% chance for a rolling start
                choice                      = UnityEngine.Random.Range(1, 101);
                if(choice > 85){
                    standingStart           = false;
                }

                // 70% chance for 'Real Weather', meaning set weatherSlots to 0
                choice                      = UnityEngine.Random.Range(1, 101);

                if(choice > 30){
                    weatherSlots            = 0;
                }

                break;

            case Event.EventDuration.FairlyLong:
                choice                      = UnityEngine.Random.Range(1, 101);

                if(choice > 65){
                    // 35% chance for a pit stop race

                    choice                  = UnityEngine.Random.Range(1, 101);
                    timeProgression         = 10;

                    if(choice > 50){
                        timeProgression     = 5;
                    }

                    weatherSlots            = 2;
                    mandatoryPitStop        = true;
                }

                // 35% chance for a rolling start
                choice                      = UnityEngine.Random.Range(1, 101);
                if(choice > 65){
                    standingStart           = false;
                }

                // 65% chance for 'Real Weather', meaning set weatherSlots to 0
                choice                      = UnityEngine.Random.Range(1, 101);

                if(choice > 35){
                    weatherSlots            = 0;
                    specialWeatherSlots     = 2;
                }

                break;

            case Event.EventDuration.Long:
                choice                  = UnityEngine.Random.Range(1, 101);

                if(choice > 20){
                    // 80% chance for a pit stop race

                    choice                  = UnityEngine.Random.Range(1, 101);
                    timeProgression         = 10;

                    if(choice > 50){
                        timeProgression     = 15;
                    }

                    weatherSlots            = UnityEngine.Random.Range(2, 4);
                    mandatoryPitStop        = true;
                }

                // 80% chance for a rolling start
                choice                      = UnityEngine.Random.Range(1, 101);
                if(choice > 20){
                    standingStart           = false;
                }

                // 65% chance for 'Real Weather', meaning set weatherSlots to 0
                choice                      = UnityEngine.Random.Range(1, 101);

                if(choice > 35){
                    weatherSlots            = 0;
                    specialWeatherSlots     = UnityEngine.Random.Range(2, 4);
                }

                break;

            case Event.EventDuration.Endurance:
                choice                  = UnityEngine.Random.Range(1, 101);

                // 100% chance for a pit stop race
                choice                  = UnityEngine.Random.Range(1, 101);
                timeProgression         = 15;

                if(choice > 50){
                    timeProgression     = 20;
                }

                weatherSlots            = UnityEngine.Random.Range(3, 5);
                mandatoryPitStop        = true;

                // 100% chance for a rolling start
                standingStart           = false;

                // 60% chance for 'Real Weather', meaning set weatherSlots to 0
                choice                  = UnityEngine.Random.Range(1, 101);

                if(choice > 40){
                    weatherSlots        = 0;
                    specialWeatherSlots = UnityEngine.Random.Range(3, 5);
                }
                break;

            default:
                Debug.LogError("Duration: " + Event.eventDurationToString[duration] + " still needs an implementation.");
                break;
        }

        // Done setting values, now calculate weatherForecast and pitStopMinTyres

        // 50% chance for all tyres required to be changed on pit
        choice                          = UnityEngine.Random.Range(1, 3);
        if(1 == choice && mandatoryPitStop){
            pitStopMinTyres             = 4;
        }

        List<string> clearWeather       = new List<string> { "Clear", "Light Cloud", "Medium Cloud", "Heavy Cloud", "Overcast" };
        List<string> rainWeather        = new List<string> { "Light Rain", "Rain", "Fog with Rain", "Heavy Fog with Rain" };
        List<string> stormWeather       = new List<string> { "Storm", "Thunderstorm" };
        List<string> fogWeather         = new List<string> { "Fog", "Heavy Fog", "Hazy" };

        // Check the theme and set values accordingly
        switch(theme)
        {
            case EventSeries.SeriesTheme.Foggy:
                weatherSlots    = specialWeatherSlots;

                for(int i = 0; i < weatherSlots; i++)
                {
                    weatherForecast.Add(fogWeather[UnityEngine.Random.Range(0, fogWeather.Count)]);
                }
                break;

            case EventSeries.SeriesTheme.Night:
                intTime         = UnityEngine.Random.Range(22, 27);
                break;

            case EventSeries.SeriesTheme.Rainy:
                weatherSlots    = specialWeatherSlots;

                for(int i = 0; i < weatherSlots; i++)
                {
                    weatherForecast.Add(rainWeather[UnityEngine.Random.Range(0, rainWeather.Count)]);
                }
                break;

            case EventSeries.SeriesTheme.Stormy:
                weatherSlots    = specialWeatherSlots;

                for(int i = 0; i < weatherSlots; i++)
                {
                    weatherForecast.Add(stormWeather[UnityEngine.Random.Range(0, stormWeather.Count)]);
                }
                break;

            default:
                Debug.LogWarning("SeriesTheme: " + theme.ToString() + "'s special cases has not been implemented yet.");
                break;
        }

        if(0 == weatherForecast.Count && weatherSlots > 0)
        {
            for(int i = 0; i < weatherSlots; i++)
            {
                // Each slot has a 75% chance to be clear weather
                // Each slot has a 8% chance to be rainy weather
                // Each slot has a 6% chance to be rainy weather
                // Each slot has a 11% chance to be fog weather
                choice                      = UnityEngine.Random.Range(1, 101);
                if(choice <= 75){
                    weatherForecast.Add(clearWeather[UnityEngine.Random.Range(0, clearWeather.Count)]);
                }
                else if(choice <= 83){
                    weatherForecast.Add(rainWeather[UnityEngine.Random.Range(0, rainWeather.Count)]);
                }
                else if(choice <= 89){
                    weatherForecast.Add(stormWeather[UnityEngine.Random.Range(0, stormWeather.Count)]);
                }
                else{
                    weatherForecast.Add(fogWeather[UnityEngine.Random.Range(0, fogWeather.Count)]);
                }
            }
        }

        // Make sure intTime is in the 24 hour mark (so, if it's 27 it should be 3)
        intTime         %= 24;
        startTime       = intTime.ToString();
        // Make sure to add a '0' if it is a single digit
        if(1 == startTime.Length){
            startTime   = "0" + startTime;
        }
        startTime       += ":00";

        // Either use laps or mins, depending on if useLaps is true or not
        if(useLaps){
            float           lapTime         = (float)track.kmLength / (float)EventSeries.tierToAvgKMPerMinuteSpeed[tier];
            int             numLaps         = (int)(Event.eventDurationToExpectedMins[duration] / lapTime);

            if(numLaps < MIN_LAPS){
                numLaps = MIN_LAPS;
            }

            return new EventEntry(  track, gridSize, parentEventParam, minsParam:-1, lapsParam:numLaps,
                                    startTimeParam:startTime, timeProgressionParam:timeProgression, standingStartParam:standingStart, weatherForecastParam:weatherForecast,
                                    mandatoryPitStopParam:mandatoryPitStop, pitStopMinTyresParam:pitStopMinTyres, fieldTypeParam:fieldType );
        }
        else{
            // In minutes
            int             numMins         = Event.eventDurationToExpectedMins[duration];
            return new EventEntry(  track, gridSize, parentEventParam, minsParam:numMins, lapsParam:-1,
                                    startTimeParam:startTime, timeProgressionParam:timeProgression, standingStartParam:standingStart, weatherForecastParam:weatherForecast,
                                    mandatoryPitStopParam:mandatoryPitStop, pitStopMinTyresParam:pitStopMinTyres, fieldTypeParam:fieldType );
        }
    }

    [System.Serializable]
    public enum OpponentFieldType
    {
        Identical   = 0,
        SameClass   = 1,
        MultiClass  = 2
    };
    public static Dictionary<OpponentFieldType, string> opponentFieldTypeToString = new Dictionary<OpponentFieldType, string>
    {
        {OpponentFieldType.Identical,       "Identical"},
        {OpponentFieldType.SameClass,       "Same Class"},
        {OpponentFieldType.MultiClass,      "Multi-Class"}
    };
}