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


    public SerializableList<SerializableResultDriver>   driverResults;
    public SerializableResultDriver                     playerResult;

    // Car object
    public string                                       playerCarName;

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