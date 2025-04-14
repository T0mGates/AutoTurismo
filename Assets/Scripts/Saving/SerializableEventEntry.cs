using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableEventEntry
{
    // Track obj
    public string                                       trackName;
    public string                                       trackLayout;
    public Tracks.Country                               trackCountry;

    public int                                          mins;
    public int                                          laps;
    public bool                                         attempted;
    public bool                                         nextUp;
    public SerializableEvent                            parentEvent;

    public int                                          gridSize;

    public string                                       startTime;

    public int                                          timeProgression;

    public bool                                         standingStart;

    public int                                          weatherSlots;

    public SerializableList<string>                     weatherForecast;

    public bool                                         mandatoryPitStop;

    public int                                          pitStopMinTyres;

    public EventEntry.OpponentFieldType                 fieldType;

    public SerializableList<SerializableResultDriver>   driverResults;
    public SerializableResultDriver                     playerResult;

    // Car object
    public string                                       playerCarName;

    public float                                        totalDistanceTraveled;

    public SerializableEventEntry(EventEntry entry, SerializableEvent serEvent){
        trackName                               = entry.track.name;
        trackLayout                             = entry.track.layout;
        trackCountry                            = entry.track.country;

        mins                                    = entry.mins;
        laps                                    = entry.laps;
        attempted                               = entry.attempted;
        nextUp                                  = entry.nextUp;
        parentEvent                             = serEvent;
        gridSize                                = entry.gridSize;

        startTime                               = entry.startTime;

        timeProgression                         = entry.timeProgression;

        standingStart                           = entry.standingStart;

        weatherSlots                            = entry.weatherSlots;

        weatherForecast                         = new SerializableList<string>();
        weatherForecast.SetList(entry.weatherForecast);

        mandatoryPitStop                        = entry.mandatoryPitStop;

        pitStopMinTyres                         = entry.pitStopMinTyres;

        fieldType                               = entry.fieldType;

        totalDistanceTraveled                   = entry.totalDistanceTraveled;

        driverResults                           = new SerializableList<SerializableResultDriver>();
        foreach(ResultDriver driver in entry.driverResults){
            SerializableResultDriver serDriver  = new SerializableResultDriver(driver);
            driverResults.Add(serDriver);
            if(driver.IsPlayer){
                playerResult                    = serDriver;
            }
        }

        playerCarName                           = entry.playerCar == null ? "" : entry.playerCar.name;
    }
}