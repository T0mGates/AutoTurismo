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
    public Tracks.Country                   partOfCountry;

    public EventSeries(string nameParam, EventSeriesManager.SeriesTier seriesTierParam, Tracks.Country partOfCountryParam){
        name            = nameParam;
        seriesTier      = seriesTierParam;
        partOfCountry   = partOfCountryParam;
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
    private static Dictionary<Tracks.Country, List<EventSeries>> countryToSeries;

    static EventSeriesManager(){
        // Initialize our main dictionary
        countryToSeries = new Dictionary<Tracks.Country, List<EventSeries>>();

        // Initialize the lists within
        foreach(Tracks.Country country in Enum.GetValues(typeof(Tracks.Country))){
            countryToSeries[country] = new List<EventSeries>();
        }
    }

    public static List<EventSeries> GetCountrySeries(Tracks.Country country){
        return countryToSeries[country];
    }

    // Called to add a new series to a country
    // Called by EventSeries' constructor
    public static void AddNewSeries(EventSeries seriesToAdd){
        countryToSeries[seriesToAdd.partOfCountry].Add(seriesToAdd);
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
        {SeriesTier.WorldRenowned,      "WorldRenowned"}
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
        {SeriesTier.Rookie,             new List<Tracks.Grade>() { {Tracks.Grade.Kart}, {Tracks.Grade.Four}, {Tracks.Grade.Temporary} } },
        {SeriesTier.Novice,             new List<Tracks.Grade>() { {Tracks.Grade.Kart}, {Tracks.Grade.Four}, {Tracks.Grade.Temporary}, {Tracks.Grade.Three}}},
        {SeriesTier.Amateur,            new List<Tracks.Grade>() { {Tracks.Grade.Kart}, {Tracks.Grade.Four}, {Tracks.Grade.Temporary}, {Tracks.Grade.Three}, {Tracks.Grade.Historic}}},
        {SeriesTier.Pro,                new List<Tracks.Grade>() { {Tracks.Grade.Temporary}, {Tracks.Grade.Three}, {Tracks.Grade.Historic}, {Tracks.Grade.Two}}},
        {SeriesTier.Elite,              new List<Tracks.Grade>() { {Tracks.Grade.Three}, {Tracks.Grade.Historic}, {Tracks.Grade.Two}}},
        {SeriesTier.Master,             new List<Tracks.Grade>() { {Tracks.Grade.One}, {Tracks.Grade.Historic}, {Tracks.Grade.Two}}},
        {SeriesTier.Prodigy,            new List<Tracks.Grade>() { {Tracks.Grade.One}, {Tracks.Grade.Historic}, {Tracks.Grade.Two}}},
        {SeriesTier.Legend,             new List<Tracks.Grade>() { {Tracks.Grade.One}, {Tracks.Grade.Historic}, {Tracks.Grade.Two}}},
        {SeriesTier.WorldRenowned,      new List<Tracks.Grade>() { {Tracks.Grade.One}, {Tracks.Grade.Historic}}}
    };
}

public class Event
{
    public string               name;
    public EventType            eventType;
    public EventDuration        eventDuration;
    public EventSeries          parentEventSeries;

    public List<EventEntry>     eventEntries;
    public int                  bonusFame;
    public int                  bonusMoney;

    public List<Cars.CarType>   typeWhitelist;
    public List<Cars.CarClass>  classWhitelist;
    public List<Cars.CarBrand>  brandWhitelist;
    public List<string>         nameWhitelist;

    public Event(
                string nameParam, EventType eventTypeParam, EventDuration eventDurationParam, EventSeries parentEventSeriesParam,
                List<Cars.CarType> allowedTypes, List<Cars.CarClass> allowedClasses, List<Cars.CarBrand> allowedBrands, List<string> allowedNames,
                int bonusFameParam, int bonusMoneyParam
                ){
        name                = nameParam;
        eventType           = eventTypeParam;
        eventDuration       = eventDurationParam;
        parentEventSeries   = parentEventSeriesParam;

        bonusFame           = bonusFameParam;
        bonusMoney          = bonusMoneyParam;

        typeWhitelist       = allowedTypes;
        classWhitelist      = allowedClasses;
        brandWhitelist      = allowedBrands;
        nameWhitelist       = allowedNames;

        // Will be filled in by newly instantiated event entries
        eventEntries        = new List<EventEntry>();

        parentEventSeries.events.Add(this);
    }

    public static Event GenerateNewEvent(
                string eventName, EventType eventType, EventDuration duration, EventSeries parentEventSeriesParam, List<Tracks.Country> allowedCountries,
                List<Cars.CarType> allowedTypes, List<Cars.CarClass> allowedClasses, List<Cars.CarBrand> allowedBrands, List<string> allowedNames,
                int bonusFame, int bonusMoney, bool useLaps = true, List<Track> blacklistedTracks = null
                ){
        // First pick the track depending on the tier and country
        EventSeriesManager.SeriesTier   tier            = parentEventSeriesParam.seriesTier;
        Track                           trackToUse      = null;
        List<Tracks.Grade>              allowedGrades   = EventSeriesManager.tierAllowedOnGrade[tier];

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

        // While we haven't filled out our tracksToUse
        while(tracksToUse.Count < numEntries){
            // While we haven't tried every grade
            while(triedGrades.Count < allowedGrades.Count){
                // Start with the track grade
                // Pick a random grade from the list of valid track grades (so long we haven't tried that grade yet)
                while(true){
                    gradeToUse  = allowedGrades[UnityEngine.Random.Range(0, allowedGrades.Count)];
                    // If we haven't tried this grade, break
                    if(!triedGrades.Contains(gradeToUse)){
                        break;
                    }
                }
                triedGrades.Add(gradeToUse);

                // While we haven't tried every country and we haven't found a single valid track
                while(triedCountries.Count < allowedCountries.Count && 0 == validTracks.Count){
                    // Pick a random country from the list of allowed countries (so long we haven't tried that country yet)
                    while(true){
                        countryToUse  = allowedCountries[UnityEngine.Random.Range(0, allowedCountries.Count)];
                        // If we haven't tried this grade, break
                        if(!triedCountries.Contains(countryToUse)){
                            break;
                        }
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
                        // Also don't add a duplicate
                        foreach(Track track in tracksToUse){
                            if(validTracks.Contains(track)){
                                validTracks.Remove(track);
                            }
                        }
                    }
                }
                // If validTracks is still empty, means we have no valid tracks so try again
                // If it is not empty, break out of the loop after assigning our track
                if(validTracks.Count > 0){
                    // Pick a random track from that list
                    trackToUse                      = validTracks[UnityEngine.Random.Range(0, validTracks.Count)];
                    break;
                }
            }

            // If trackToUse is still null at this point, it means we couldn't generate a new unique track for an event entry
            if(trackToUse == null){
                // If tracksToUse also has 0 entries, means no track was valid so return null
                // Else, it means that some tracks were legal, but due to dupes we couldn't entirely fill it out, so simply fill it out with tracks it already has
                if(0 == tracksToUse.Count){
                    return null;
                }

                int maxTrackIndex = tracksToUse.Count - 1;
                while(tracksToUse.Count < numEntries){
                    tracksToUse.Add(tracksToUse[UnityEngine.Random.Range(0, maxTrackIndex)]);
                }
            }
            else{
                tracksToUse.Add(trackToUse);
            }
        }

        Event newEvent = new Event(eventName, eventType, duration, parentEventSeriesParam, allowedTypes, allowedClasses, allowedBrands, allowedNames, bonusFame, bonusMoney);

        // For each track, make an event entry
        foreach(Track track in tracksToUse){
            eventEntries.Add(EventEntry.GenerateNewEventEntry(track, duration, tier, newEvent, useLaps));
        }

        return newEvent;
    }

    public int GetEventEntryPosition(EventEntry eventEntryToCheck){
        // Returns the 'position number' for a given event entry
        return eventEntries.IndexOf(eventEntryToCheck) + 1;
    }

    // Returns a string that includes details on all the whitelists for this event
    public string getPrintWhitelist(){
        string toReturn = "";

        // Car Names
        if(nameWhitelist.Count > 0){
            toReturn += "Car Names:";
        }
        foreach(string carName in nameWhitelist){
            toReturn += " " + carName + ",";
        }
        // Remove the trailing comma, add new line
        if(nameWhitelist.Count > 0){
            toReturn = toReturn.Substring(0, toReturn.Length - 1) + "\n";
        }

        // Car Types
        if(typeWhitelist.Count > 0){
            toReturn += "Car Types:";
        }
        foreach(Cars.CarType carType in typeWhitelist){
            toReturn += " " + Cars.typeToString[carType] + ",";
        }
        // Remove the trailing comma, add new line
        if(typeWhitelist.Count > 0){
            toReturn = toReturn.Substring(0, toReturn.Length - 1) + "\n";
        }

        // Car Classes
        if(classWhitelist.Count > 0){
            toReturn += "Car Classes:";
        }
        foreach(Cars.CarClass carClass in classWhitelist){
            toReturn += " " + Cars.classToString[carClass] + ",";
        }
        // Remove the trailing comma, add new line
        if(classWhitelist.Count > 0){
            toReturn = toReturn.Substring(0, toReturn.Length - 1) + "\n";
        }

        // Car Brands
        if(brandWhitelist.Count > 0){
            toReturn += "Car Brands:";
        }
        foreach(Cars.CarBrand carBrand in brandWhitelist){
            toReturn += " " + Cars.brandToString[carBrand] + ",";
        }
        // Remove the trailing comma, add new line
        if(brandWhitelist.Count > 0){
            toReturn = toReturn.Substring(0, toReturn.Length - 1) + "\n";
        }

        return toReturn;
    }

    public string getPrintMoneyReward(){
        return "$" + bonusMoney.ToString("n0");
    }

    public enum EventDuration
    {
        Mini, // Around 5-7 minutes
        Short, // Around 9-11 minutes
        Average, // Around 15 minutes
        FairlyLong, // Around 20-25 minutes
        Long, // Around 30 minutes
        Endurance // 1 hour +
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
}

public class EventEntry
{
    public Track    track;
    public int      mins;
    public int      laps;
    public bool     completed;
    public bool     attempted;
    public int      finishPosition;
    public Event    parentEvent;

    // Probably want AI levels and whatnot
    public int      gridSize;

    public EventEntry(Track trackParam, int gridSizeParam, Event parentEventParam, int minsParam = -1, int lapsParam = -1){
        track           = trackParam;
        gridSize        = gridSizeParam;
        mins            = minsParam;
        laps            = lapsParam;
        completed       = false;
        attempted       = false;
        finishPosition  = -1;
        parentEvent     = parentEventParam;

        parentEvent.eventEntries.Add(this);
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