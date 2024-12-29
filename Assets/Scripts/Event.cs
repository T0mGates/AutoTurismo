using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEditor.PackageManager;

public class EventSeries
{
    // Name / tier together should make unique series
    public EventSeriesManager.SeriesTier    seriesTier;
    public string                           name;
    public List<Event>                      events;

    public EventSeries(string nameParam, EventSeriesManager.SeriesTier seriesTierParam, List<Event> eventsParam){
        name        = nameParam;
        seriesTier  = seriesTierParam;
        events      = eventsParam;
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
    public static void AddNewSeries(Tracks.Country country, EventSeries seriesToAdd){
        countryToSeries[country].Add(seriesToAdd);
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
    public static Dictionary<SeriesTier, int> tierToAvgKMSpeed = new Dictionary<SeriesTier, int>
    {
        {SeriesTier.Rookie,             100},
        {SeriesTier.Novice,             125},
        {SeriesTier.Amateur,            150},
        {SeriesTier.Pro,                175},
        {SeriesTier.Elite,              200},
        {SeriesTier.Master,             210},
        {SeriesTier.Prodigy,            220},
        {SeriesTier.Legend,             235},
        {SeriesTier.WorldRenowned,      250}
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

    public List<EventEntry>     eventEntries;
    public int                  bonusExp;
    public int                  bonusMoney;

    public List<Cars.CarType>   typeWhitelist;
    public List<Cars.CarClass>  classWhitelist;
    public List<Cars.CarBrand>  brandWhitelist;
    public List<string>         nameWhitelist;

    public Event(
                string nameParam, EventType eventTypeParam, List<EventEntry> eventEntriesParam,
                List<Cars.CarType> allowedTypes, List<Cars.CarClass> allowedClasses, List<Cars.CarBrand> allowedBrands, List<string> allowedNames,
                int bonusExpParam, int bonusMoneyParam
                ){
        name            = nameParam;
        eventType       = eventTypeParam;

        eventEntries    = eventEntriesParam;
        bonusExp        = bonusExpParam;
        bonusMoney      = bonusMoneyParam;

        typeWhitelist   = allowedTypes;
        classWhitelist  = allowedClasses;
        brandWhitelist  = allowedBrands;
        nameWhitelist   = allowedNames;
    }

    public static Event GenerateNewRaceEvent(
                string eventName, Tracks.Country country, EventSeriesManager.SeriesTier tier, EventDuration duration,
                List<Cars.CarType> allowedTypes, List<Cars.CarClass> allowedClasses, List<Cars.CarBrand> allowedBrands, List<string> allowedNames,
                int bonusExp, int bonusMoney, bool useLaps = true
                ){
        // First pick the track depending on the tier and country
        List<Tracks.Grade> allowedGrades    = EventSeriesManager.tierAllowedOnGrade[tier];

        // Pick a random grade from that list
        Tracks.Grade        gradeToUse      = allowedGrades[UnityEngine.Random.Range(0, allowedGrades.Count)];

        // TODO: this might be an empty list depending on the country
        List<Track>         validTracks     = Tracks.GetTracks(country, gradeToUse);

        // Pick a random track from that list
        Track               trackToUse      = validTracks[UnityEngine.Random.Range(0, validTracks.Count)];

        List<EventEntry> eventEntries;

        if(useLaps){
            float           lapTime         = (float)trackToUse.kmLength / (float)EventSeriesManager.tierToAvgKMSpeed[tier];
            int             numLaps         = (int)(eventDurationToExpectedMins[duration] / lapTime);

            eventEntries                    = new List<EventEntry>() { {new EventEntry(trackToUse, -1, numLaps)} };
        }
        else{
            // In minutes
            int             numMins         = eventDurationToExpectedMins[duration];
            eventEntries                    = new List<EventEntry>() { {new EventEntry(trackToUse, numMins, -1)} };
        }

        return new Event(eventName, EventType.Race, eventEntries, allowedTypes, allowedClasses, allowedBrands, allowedNames, bonusExp, bonusMoney);
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
        {EventDuration.Average,         "Average"},
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
}

public class EventEntry
{
    public Track    track;
    public int      mins;
    public int      laps;

    // Probably want AI levels and whatnot

    public EventEntry(Track trackParam, int minsParam = -1, int lapsParam = -1){
        track   = trackParam;
        mins    = minsParam;
        laps    = lapsParam;
    }
}