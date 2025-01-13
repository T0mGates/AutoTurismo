using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using UnityEngine;

[System.Serializable]
public class SerializableEvent
{
    public string                                   name;
    public Event.EventType                          eventType;
    public Event.EventDuration                      eventDuration;
    public SerializableEventSeries                  parentEventSeries;

    public SerializableList<Cars.CarType>           typeWhitelist;
    public SerializableList<Cars.CarClass>          classWhitelist;
    public SerializableList<Cars.CarBrand>          brandWhitelist;
    public SerializableList<string>                 nameWhitelist;

    public int                                      topFameReward;
    public int                                      topMoneyReward;
    public int                                      finishPosition;
    public int                                      gridSize;
    public bool                                     completed;

    public SerializableList<SerializableEventEntry> eventEntries;

    // Championship points dict
    public SerializableList<string>                 champhionshipNames;
    public SerializableList<int>                    championshipPoints;

    public SerializableEvent(Event eventObj, SerializableEventSeries serEventSeries){
        name                    = eventObj.name;
        eventType               = eventObj.eventType;
        eventDuration           = eventObj.eventDuration;
        parentEventSeries       = serEventSeries;

        typeWhitelist           = new SerializableList<Cars.CarType>();
        typeWhitelist.SetList(eventObj.typeWhitelist);

        classWhitelist          = new SerializableList<Cars.CarClass>();
        classWhitelist.SetList(eventObj.classWhitelist);

        brandWhitelist          = new SerializableList<Cars.CarBrand>();
        brandWhitelist.SetList(eventObj.brandWhitelist);

        nameWhitelist           = new SerializableList<string>();
        nameWhitelist.SetList(eventObj.nameWhitelist);

        topFameReward           = eventObj.topFameReward;
        topMoneyReward          = eventObj.topMoneyReward;
        finishPosition          = eventObj.finishPosition;
        gridSize                = eventObj.gridSize;
        completed               = eventObj.completed;

        eventEntries            = new SerializableList<SerializableEventEntry>();
        foreach(EventEntry entry in eventObj.eventEntries){
            SerializableEventEntry serEntry = new SerializableEventEntry(entry, this);
            eventEntries.Add(serEntry);
        }

        champhionshipNames      = new SerializableList<string>();
        championshipPoints      = new SerializableList<int>();
        foreach(KeyValuePair<string, int> pair in eventObj.champhionshipPoints){
            champhionshipNames.Add(pair.Key);
            championshipPoints.Add(pair.Value);
        }
    }
}