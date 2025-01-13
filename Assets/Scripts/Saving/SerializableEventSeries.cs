using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableEventSeries
{
    public EventSeries.SeriesTier               seriesTier;
    public string                               name;
    public SerializableList<SerializableEvent>  events;
    public Region.ClickableRegion               partOfRegion;

    public SerializableEventSeries(EventSeries eventSeries){
        seriesTier      = eventSeries.seriesTier;
        name            = eventSeries.name;

        events          = new SerializableList<SerializableEvent>();
        foreach(Event eventObj in eventSeries.events){
            events.Add(new SerializableEvent(eventObj, this));
        }

        partOfRegion    = eventSeries.partOfRegion;
    }
}