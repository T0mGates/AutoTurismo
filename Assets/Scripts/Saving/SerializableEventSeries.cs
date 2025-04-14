using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableEventSeries
{
    public EventSeries.SeriesTier               seriesTier;
    public string                               name;
    public string                               prefix;
    public string                               suffix;
    public SerializableList<SerializableEvent>  events;
    public Region.ClickableRegion               partOfRegion;
    public EventSeries.SeriesTheme              seriesTheme;

    public SerializableEventSeries(EventSeries eventSeries){
        seriesTier      = eventSeries.seriesTier;
        name            = eventSeries.name;
        prefix          = eventSeries.prefix;
        suffix          = eventSeries.suffix;

        events          = new SerializableList<SerializableEvent>();
        foreach(Event eventObj in eventSeries.events){
            events.Add(new SerializableEvent(eventObj, this));
        }

        partOfRegion    = eventSeries.partOfRegion;
        seriesTheme     = eventSeries.seriesTheme;
    }
}