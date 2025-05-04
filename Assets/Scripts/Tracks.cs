using UnityEngine;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;

public class Track
{
    public string               name;
    public string               layout;
    public Tracks.Country       country;
    public Tracks.Grade         grade;
    public int                  maxGridSize;
    public float                kmLength;

    public Track(string nameParam, string layoutParam, int maxGridSizeParam, float kmLengthParam, Tracks.Grade gradeParam, Tracks.Country countryParam)
    {
        name        = nameParam;
        layout      = layoutParam;
        maxGridSize = maxGridSizeParam;
        kmLength    = kmLengthParam;
        grade       = gradeParam;
        country     = countryParam;
    }

    public string GetPrintName()
    {
        return name + " " + layout;
    }

    public Sprite GetTrackMapSprite()
    {
        string imageName = name.Replace(" ", "").Replace("-", "").Replace(".", "");
        if("" != layout){
            imageName += "_" + layout.Replace(" ", "").Replace("-", "").Replace(".", "");
        }
        return Resources.Load<Sprite>("Images/TrackMaps/" + imageName);
    }

    public Sprite GetTrackImageSprite()
    {
        string imageName = name.Replace(" ", "").Replace("-", "").Replace(".", "");

        return Resources.Load<Sprite>("Images/TrackImages/" + imageName);
    }

    public Sprite GetCountryFlagSprite()
    {
        string imageName = Tracks.countryToString[country].Replace(" ", "").Replace("-", "").Replace(".", "");

        return Resources.Load<Sprite>("Images/CountryImages/" + imageName);
    }

    public override bool Equals(object obj)
    {
        if(obj == null || GetType() != obj.GetType()){
            return false;
        }

        Track trackToCompare = (Track)obj;
        return trackToCompare.name == name && trackToCompare.layout == layout;
    }

    public override int GetHashCode()
    {
        return (name + layout).GetHashCode();
    }
}

public static class Tracks
{
    private static Dictionary<Country, List<Track>>                     countryTracks;
    private static Dictionary<Region.ClickableRegion, List<Country>>    regionToCountries;

    static Tracks()
    {
        // Initialize our Tracks DB
        countryTracks       = new Dictionary<Country, List<Track>>();
        regionToCountries   = new Dictionary<Region.ClickableRegion, List<Country>>();

        foreach(Country country in Enum.GetValues(typeof(Country)))
        {
            countryTracks[country]      = new List<Track>();
        }
        foreach(Region.ClickableRegion region in Enum.GetValues(typeof(Region.ClickableRegion)))
        {
            regionToCountries[region]   = new List<Country>();
        }

        // Make our track instances and add them to our Tracks
        countryTracks[Country.Argentina]                            = GetArgentinaTracks();
        countryTracks[Country.Australia]                            = GetAustraliaTracks();
        countryTracks[Country.Austria]                              = GetAustriaTracks();
        countryTracks[Country.Brazil]                               = GetBrazilTracks();
        countryTracks[Country.Canada]                               = GetCanadaTracks();
        countryTracks[Country.Ecuador]                              = GetEcuadorTracks();
        countryTracks[Country.England]                              = GetEnglandTracks();
        countryTracks[Country.Italy]                                = GetItalyTracks();
        countryTracks[Country.Japan]                                = GetJapanTracks();
        countryTracks[Country.Monaco]                               = GetMonacoTracks();
        countryTracks[Country.Norway]                               = GetNorwayTracks();
        countryTracks[Country.Portugal]                             = GetPortugalTracks();
        countryTracks[Country.SouthAfrica]                          = GetSouthAfricaTracks();
        countryTracks[Country.Spain]                                = GetSpainTracks();
        countryTracks[Country.USA]                                  = GetUSATracks();

        // (Clickable) Regions setup
        foreach(Region.ClickableRegion region in Enum.GetValues(typeof(Region.ClickableRegion)))
        {
            regionToCountries[region]                               = GetCountries(region);
        }
    }

    public static Track GetTrack(string trackName, string trackLayout, Country trackCountry)
    {
        List<Track> tracks = GetTracks(trackCountry);

        foreach(Track track in tracks)
        {
            if(track.name == trackName && track.layout == trackLayout)
            {
                return track;
            }
        }

        Debug.LogWarning("Couldn't find track: " + trackName + " - " + trackLayout + ", looking into DLC tracks that aren't activated.");

        foreach(SettingsMenu.DLC dlc in Enum.GetValues(typeof(SettingsMenu.DLC)))
        {
            foreach(Track track in GetDLCTracks(dlc))
            {
                if(track.name == trackName && track.layout == trackLayout)
                {
                    return track;
                }
            }
        }

        Debug.LogError("Still couldn't find track.");
        return null;
    }

    public static List<Track>   GetTracks(Country country)
    {
        return countryTracks[country];
    }

    public static List<Track>   GetTracks(Country country, Grade grade)
    {
        List<Track> tracks = new List<Track>();

        foreach(Track track in GetTracks(country))
        {
            // If the track is the wanted grade, add it to our return list
            if(track.grade.Equals(grade))
            {
                tracks.Add(track);
            }
        }

        return tracks;
    }

    public static List<Track> GetTracks(Region.ClickableRegion region)
    {
        List<Track> tracks              = new List<Track>();

        foreach(Country country in regionToCountries[region]){
            tracks.AddRange(GetTracks(country));
        }

        return tracks;
    }

    public static List<Track> GetTracks(Region.ClickableRegion region, Grade grade)
    {
        List<Track> tracks              = new List<Track>();

        foreach(Country country in regionToCountries[region]){
            tracks.AddRange(GetTracks(country, grade));
        }

        return tracks;
    }

    public static void SetDLCState(SettingsMenu.DLC dlc, bool state)
    {
        List<Track> tracks    = GetDLCTracks(dlc);
        foreach(Track track in tracks)
        {
            if(state)
            {
                countryTracks[track.country].Add(track);
            }
            else if(countryTracks[track.country].Contains(track))
            {
                countryTracks[track.country].Remove(track);
            }
        }
    }

    private static List<Track> GetDLCTracks(SettingsMenu.DLC dlc)
    {
        List<Track> trackList = new List<Track>();
        switch(dlc)
        {
            case SettingsMenu.DLC.RacinUSAOne:
                trackList.Add(new Track(
                        "Daytona",
                        "Sports Car Course",
                        32,
                        5.729f,
                        Grade.Three,
                        Country.USA
                    ));
                trackList.Add(new Track(
                        "Daytona",
                        "NASCAR Road Course",
                        32,
                        5.729f,
                        Grade.Three,
                        Country.USA
                    ));
                trackList.Add(new Track(
                        "Laguna Seca",
                        "2020",
                        30,
                        3.602f,
                        Grade.Two,
                        Country.USA
                    ));
                trackList.Add(new Track(
                        "Long Beach",
                        "",
                        32,
                        3.167f,
                        Grade.Three,
                        Country.USA
                    ));
                break;

            case SettingsMenu.DLC.SupercarsOne:
                break;

            default:
                break;
        }

        return trackList;
    }

    [System.Serializable]
    public enum Country
    {
        Argentina,
        Australia,
        Austria,
        Brazil,
        Canada,
        Ecuador,
        England,
        Italy,
        Japan,
        Monaco,
        Norway,
        Portugal,
        SouthAfrica,
        Spain,
        USA
    }

    public static Dictionary<Country, string> countryToString = new Dictionary<Country, string>
    {
        {Country.Argentina,             "Argentina"},
        {Country.Australia,             "Australia"},
        {Country.Austria,               "Austria"},
        {Country.Brazil,                "Brazil"},
        {Country.Canada,                "Canada"},
        {Country.Ecuador,               "Ecuador"},
        {Country.England,               "England"},
        {Country.Italy,                 "Italy"},
        {Country.Japan,                 "Japan"},
        {Country.Monaco,                "Monaco"},
        {Country.Norway,                "Norway"},
        {Country.Portugal,              "Portugal"},
        {Country.SouthAfrica,           "South Africa"},
        {Country.Spain,                 "Spain"},
        {Country.USA,                   "USA"}
    };

    [System.Serializable]
    public enum Grade
    {
        Kart = 0,
        Temporary = 1,
        Historic = 2,
        Four = 3,
        Three = 4,
        Two = 5,
        One = 6,
        Oval = 7

    }

    public static Dictionary<Grade, string> gradeToString = new Dictionary<Grade, string>
    {
        {Grade.One,                 "1"},
        {Grade.Two,                 "2"},
        {Grade.Three,               "3"},
        {Grade.Four,                "4"},
        {Grade.Historic,            "Historic"},
        {Grade.Oval,                "Oval"},
        {Grade.Temporary,           "Temporary"},
        {Grade.Kart,                "Kart"}
    };

    public static List<Country> GetCountries(Region.ClickableRegion region)
    {
        List<Country> countries;

        switch(region)
        {
            case Region.ClickableRegion.NorthAmerica:
                return new List<Country>() { {Country.USA}, {Country.Canada} };
            case Region.ClickableRegion.SouthAmerica:
                return new List<Country>() { {Country.Ecuador}, {Country.Brazil}, {Country.Argentina} };
            case Region.ClickableRegion.Europe:
                return new List<Country>() { {Country.Norway}, {Country.Austria}, {Country.England}, {Country.Monaco}, {Country.Italy}, {Country.Spain}, {Country.Portugal} };
            case Region.ClickableRegion.Africa:
                return new List<Country>() { {Country.SouthAfrica} };
            case Region.ClickableRegion.Asia:
                return new List<Country>() { {Country.Japan} };
            case Region.ClickableRegion.Australia:
                return new List<Country>() { {Country.Australia} };

            case Region.ClickableRegion.NorthAmericaSouthAmerica:
                countries = GetCountries(Region.ClickableRegion.NorthAmerica);
                countries.AddRange(GetCountries(Region.ClickableRegion.SouthAmerica));
                return countries;

            case Region.ClickableRegion.SouthAmericaAfrica:
                countries = GetCountries(Region.ClickableRegion.SouthAmerica);
                countries.AddRange(GetCountries(Region.ClickableRegion.Africa));
                return countries;

            case Region.ClickableRegion.AfricaAustralia:
                countries = GetCountries(Region.ClickableRegion.Africa);
                countries.AddRange(GetCountries(Region.ClickableRegion.Australia));
                return countries;

            case Region.ClickableRegion.AustraliaAsia:
                countries = GetCountries(Region.ClickableRegion.Australia);
                countries.AddRange(GetCountries(Region.ClickableRegion.Asia));
                return countries;

            case Region.ClickableRegion.AsiaEurope:
                countries = GetCountries(Region.ClickableRegion.Asia);
                countries.AddRange(GetCountries(Region.ClickableRegion.Europe));
                return countries;

            case Region.ClickableRegion.EuropeNorthAmerica:
                countries = GetCountries(Region.ClickableRegion.Europe);
                countries.AddRange(GetCountries(Region.ClickableRegion.NorthAmerica));
                return countries;

            case Region.ClickableRegion.EuropeAfrica:
                countries = GetCountries(Region.ClickableRegion.Europe);
                countries.AddRange(GetCountries(Region.ClickableRegion.Africa));
                return countries;

            case Region.ClickableRegion.International:
                countries = GetCountries(Region.ClickableRegion.Europe);
                countries.AddRange(GetCountries(Region.ClickableRegion.NorthAmerica));
                countries.AddRange(GetCountries(Region.ClickableRegion.SouthAmerica));
                countries.AddRange(GetCountries(Region.ClickableRegion.Africa));
                countries.AddRange(GetCountries(Region.ClickableRegion.Australia));
                countries.AddRange(GetCountries(Region.ClickableRegion.Asia));
                return countries;

            default:
                Debug.LogError("Region: " + region.ToString() + " is not supported for 'GetCountries'.");
                return new List<Country>();
        }
    }

    private static List<Track> GetArgentinaTracks()
    {
        return new List<Track>()
        {
            {
                new Track(
                    "Buenos Aires Circuito",
                    "No. 12",
                    32,
                    5.65f,
                    Grade.Two,
                    Country.Argentina
                )
            },
            {
                new Track(
                    "Buenos Aires Circuito",
                    "No. 15",
                    32,
                    5.94f,
                    Grade.Two,
                    Country.Argentina
                )
            },
            {
                new Track(
                    "Buenos Aires Circuito",
                    "No. 6",
                    32,
                    4.25f,
                    Grade.Two,
                    Country.Argentina
                )
            },
            {
                new Track(
                    "Buenos Aires Circuito",
                    "No. 6 S",
                    32,
                    2.60f,
                    Grade.Two,
                    Country.Argentina
                )
            },
            {
                new Track(
                    "Buenos Aires Circuito",
                    "No. 7",
                    32,
                    3.32f,
                    Grade.Two,
                    Country.Argentina
                )
            },
            {
                new Track(
                    "Buenos Aires Circuito",
                    "No. 8",
                    32,
                    3.33f,
                    Grade.Two,
                    Country.Argentina
                )
            },
            {
                new Track(
                    "Buenos Aires Circuito",
                    "No. 9",
                    32,
                    2.06f,
                    Grade.Two,
                    Country.Argentina
                )
            },
            {
                new Track(
                    "Córdoba",
                    "No. 2",
                    26,
                    2.06f,
                    Grade.Three,
                    Country.Argentina
                )
            },
            {
                new Track(
                    "Córdoba",
                    "No. 4",
                    26,
                    4.06f,
                    Grade.Three,
                    Country.Argentina
                )
            },
            {
                new Track(
                    "Córdoba",
                    "TC",
                    26,
                    3.65f,
                    Grade.Three,
                    Country.Argentina
                )
            },
            {
                new Track(
                    "Termas de Río Hondo",
                    "",
                    48,
                    4.80f,
                    Grade.Two,
                    Country.Argentina
                )
            }
        };
    }

    private static List<Track> GetAustraliaTracks()
    {
        return new List<Track>()
        {
            {
                new Track(
                    "Adelaide",
                    "",
                    39,
                    3.21f,
                    Grade.Three,
                    Country.Australia
                )
            },
            {
                new Track(
                    "Adelaide",
                    "Historic 1988",
                    26,
                    3.78f,
                    Grade.Historic,
                    Country.Australia
                )
            },
            {
                new Track(
                    "Bathurst",
                    "2020",
                    48,
                    6.21f,
                    Grade.Three,
                    Country.Australia
                )
            },
        };
    }

    private static List<Track> GetAustriaTracks()
    {
        return new List<Track>()
        {
            {
                new Track(
                    "Spielberg",
                    "",
                    32,
                    4.31f,
                    Grade.One,
                    Country.Austria
                )
            },
            {
                new Track(
                    "Spielberg",
                    "Historic 1974",
                    26,
                    5.89f,
                    Grade.Historic,
                    Country.Austria
                )
            },
            {
                new Track(
                    "Spielberg",
                    "Historic 1977",
                    26,
                    5.91f,
                    Grade.Historic,
                    Country.Austria
                )
            },
            {
                new Track(
                    "Spielberg",
                    "Short",
                    32,
                    2.33f,
                    Grade.Three,
                    Country.Austria
                )
            }
        };
    }

    private static List<Track> GetBrazilTracks()
    {
        return new List<Track>()
        {
            {
                new Track(
                    "Brasília",
                    "Full",
                    30,
                    5.47f,
                    Grade.Three,
                    Country.Brazil
                )
            },
            {
                new Track(
                    "Brasília",
                    "Outer",
                    30,
                    2.91f,
                    Grade.Three,
                    Country.Brazil
                )
            },
            {
                new Track(
                    "Campo Grande",
                    "",
                    48,
                    3.43f,
                    Grade.Three,
                    Country.Brazil
                )
            },
            {
                new Track(
                    "Cascavel",
                    "",
                    48,
                    3.30f,
                    Grade.Three,
                    Country.Brazil
                )
            },
            {
                new Track(
                    "Copa São Paulo",
                    "Kart Stage 2",
                    28,
                    0.98f,
                    Grade.Kart,
                    Country.Brazil
                )
            },
            {
                new Track(
                    "Curitiba",
                    "",
                    30,
                    3.69f,
                    Grade.Three,
                    Country.Brazil
                )
            },
            {
                new Track(
                    "Curitiba",
                    "Outer",
                    30,
                    2.60f,
                    Grade.Three,
                    Country.Brazil
                )
            },
            {
                new Track(
                    "Curvelo",
                    "Long",
                    48,
                    4.42f,
                    Grade.Two,
                    Country.Brazil
                )
            },
            {
                new Track(
                    "Curvelo",
                    "Short",
                    48,
                    3.33f,
                    Grade.Three,
                    Country.Brazil
                )
            },
            {
                new Track(
                    "Galeao Airport",
                    "",
                    32,
                    3.20f,
                    Grade.Temporary,
                    Country.Brazil
                )
            },
            {
                new Track(
                    "Goiânia",
                    "",
                    48,
                    3.82f,
                    Grade.Four,
                    Country.Brazil
                )
            },
            {
                new Track(
                    "Goiânia",
                    "External",
                    48,
                    2.59f,
                    Grade.Three,
                    Country.Brazil
                )
            },
            {
                new Track(
                    "Goiânia",
                    "Short",
                    36,
                    1.91f,
                    Grade.Three,
                    Country.Brazil
                )
            },
            {
                new Track(
                    "Granja Viana",
                    "Kart 101",
                    28,
                    1.01f,
                    Grade.Kart,
                    Country.Brazil
                )
            },
            {
                new Track(
                    "Granja Viana",
                    "Kart 102",
                    28,
                    0.99f,
                    Grade.Kart,
                    Country.Brazil
                )
            },
            {
                new Track(
                    "Granja Viana",
                    "Kart 121",
                    28,
                    0.80f,
                    Grade.Kart,
                    Country.Brazil
                )
            },
            {
                new Track(
                    "Guaporé",
                    "",
                    40,
                    3.08f,
                    Grade.Three,
                    Country.Brazil
                )
            },
            {
                new Track(
                    "Interlagos",
                    "GP",
                    48,
                    4.29f,
                    Grade.One,
                    Country.Brazil
                )
            },
            {
                new Track(
                    "Interlagos",
                    "Historic 1976",
                    32,
                    7.92f,
                    Grade.Historic,
                    Country.Brazil
                )
            },
            {
                new Track(
                    "Interlagos",
                    "Historic 1978 Outer",
                    32,
                    3.23f,
                    Grade.Historic,
                    Country.Brazil
                )
            },
            {
                new Track(
                    "Interlagos",
                    "Kart One",
                    38,
                    1.12f,
                    Grade.Kart,
                    Country.Brazil
                )
            },
            {
                new Track(
                    "Interlagos",
                    "Kart Three",
                    38,
                    0.69f,
                    Grade.Kart,
                    Country.Brazil
                )
            },
            {
                new Track(
                    "Interlagos",
                    "Kart Two",
                    38,
                    1.12f,
                    Grade.Kart,
                    Country.Brazil
                )
            },
            {
                new Track(
                    "Interlagos",
                    "Stock Car Brasil",
                    48,
                    4.29f,
                    Grade.Two,
                    Country.Brazil
                )
            },
            {
                new Track(
                    "Jacarepaguá",
                    "Historic 1988",
                    40,
                    5.00f,
                    Grade.Historic,
                    Country.Brazil
                )
            },
            {
                new Track(
                    "Jacarepaguá",
                    "Historic 2005",
                    40,
                    4.90f,
                    Grade.Historic,
                    Country.Brazil
                )
            },
            {
                new Track(
                    "Jacarepaguá",
                    "Historic 2005 Oval",
                    32,
                    3.00f,
                    Grade.Historic,
                    Country.Brazil
                )
            },
            {
                new Track(
                    "Jacarepaguá",
                    "Historic 2012 SCB",
                    40,
                    3.30f,
                    Grade.Historic,
                    Country.Brazil
                )
            },
            {
                new Track(
                    "Jacarepaguá",
                    "Historic 2012 Short",
                    40,
                    3.04f,
                    Grade.Historic,
                    Country.Brazil
                )
            },
            {
                new Track(
                    "Londrina",
                    "Kart One",
                    28,
                    1.00f,
                    Grade.Kart,
                    Country.Brazil
                )
            },
            {
                new Track(
                    "Londrina",
                    "Kart Two",
                    28,
                    0.86f,
                    Grade.Kart,
                    Country.Brazil
                )
            },
            {
                new Track(
                    "Londrina",
                    "Long",
                    48,
                    3.14f,
                    Grade.Three,
                    Country.Brazil
                )
            },
            {
                new Track(
                    "Londrina",
                    "Short",
                    48,
                    3.02f,
                    Grade.Three,
                    Country.Brazil
                )
            },
            {
                new Track(
                    "Salvador",
                    "Street Circuit",
                    34,
                    2.72f,
                    Grade.Temporary,
                    Country.Brazil
                )
            },
            {
                new Track(
                    "Santa Cruz do Sul",
                    "",
                    48,
                    3.32f,
                    Grade.Four,
                    Country.Brazil
                )
            },
            {
                new Track(
                    "Speedland",
                    "Kart 1",
                    14,
                    0.96f,
                    Grade.Kart,
                    Country.Brazil
                )
            },
            {
                new Track(
                    "Speedland",
                    "Kart 2",
                    14,
                    0.98f,
                    Grade.Kart,
                    Country.Brazil
                )
            },
            {
                new Track(
                    "Speedland",
                    "Kart 3",
                    14,
                    0.43f,
                    Grade.Kart,
                    Country.Brazil
                )
            },
            {
                new Track(
                    "Speedland",
                    "Kart 4",
                    14,
                    0.58f,
                    Grade.Kart,
                    Country.Brazil
                )
            },
            {
                new Track(
                    "Tarumã",
                    "Chicane",
                    32,
                    3.07f,
                    Grade.Three,
                    Country.Brazil
                )
            },
            {
                new Track(
                    "Tarumã",
                    "Internacional",
                    32,
                    3.01f,
                    Grade.Four,
                    Country.Brazil
                )
            },
            {
                new Track(
                    "Velo Città",
                    "",
                    34,
                    3.32f,
                    Grade.Three,
                    Country.Brazil
                )
            },
            {
                new Track(
                    "Velo Città",
                    "Club Day",
                    34,
                    1.72f,
                    Grade.Three,
                    Country.Brazil
                )
            },
            {
                new Track(
                    "Velo Città",
                    "Track Day",
                    34,
                    3.36f,
                    Grade.Three,
                    Country.Brazil
                )
            },
            {
                new Track(
                    "Velopark",
                    "2010",
                    34,
                    2.15f,
                    Grade.Three,
                    Country.Brazil
                )
            },
            {
                new Track(
                    "Velopark",
                    "2017",
                    34,
                    2.27f,
                    Grade.Three,
                    Country.Brazil
                )
            }
        };
    }

    private static List<Track> GetCanadaTracks()
    {
        return new List<Track>()
        {
            {
                new Track(
                    "Montreal",
                    "",
                    40,
                    4.36f,
                    Grade.One,
                    Country.Canada
                )
            },
            {
                new Track(
                    "Montreal",
                    "Historic 1988",
                    40,
                    4.43f,
                    Grade.Historic,
                    Country.Canada
                )
            }
        };
    }

    private static List<Track> GetEcuadorTracks()
    {
        return new List<Track>()
        {
            {
                new Track(
                    "Autódromo Yahuarcocha",
                    "",
                    30,
                    4.30f,
                    Grade.Four,
                    Country.Ecuador
                )
            },
            {
                new Track(
                    "Autódromo Yahuarcocha",
                    "Reverse",
                    30,
                    4.30f,
                    Grade.Four,
                    Country.Ecuador
                )
            }
        };
    }

    private static List<Track> GetEnglandTracks()
    {
        return new List<Track>()
        {
            {
                new Track(
                    "Brands Hatch",
                    "",
                    32,
                    3.91f,
                    Grade.Two,
                    Country.England
                )
            },
            {
                new Track(
                    "Brands Hatch",
                    "Indy",
                    32,
                    1.94f,
                    Grade.Three,
                    Country.England
                )
            },
            {
                new Track(
                    "Cadwell Park",
                    "",
                    26,
                    3.50f,
                    Grade.Three,
                    Country.England
                )
            },
            {
                new Track(
                    "Donington",
                    "GP",
                    38,
                    4.02f,
                    Grade.Two,
                    Country.England
                )
            },
            {
                new Track(
                    "Donington",
                    "National",
                    38,
                    3.18f,
                    Grade.Three,
                    Country.England
                )
            },
            {
                new Track(
                    "Oulton Park",
                    "Classic",
                    28,
                    4.29f,
                    Grade.Three,
                    Country.England
                )
            },
            {
                new Track(
                    "Oulton Park",
                    "Fosters",
                    28,
                    2.66f,
                    Grade.Four,
                    Country.England
                )
            },
            {
                new Track(
                    "Oulton Park",
                    "International",
                    28,
                    4.33f,
                    Grade.Three,
                    Country.England
                )
            },
            {
                new Track(
                    "Oulton Park",
                    "Island",
                    28,
                    3.63f,
                    Grade.Four,
                    Country.England
                )
            },
            {
                new Track(
                    "Snetterton",
                    "100",
                    26,
                    1.58f,
                    Grade.Four,
                    Country.England
                )
            },
            {
                new Track(
                    "Snetterton",
                    "200",
                    26,
                    3.22f,
                    Grade.Two,
                    Country.England
                )
            },
            {
                new Track(
                    "Snetterton",
                    "300",
                    26,
                    4.78f,
                    Grade.Two,
                    Country.England
                )
            }
        };
    }

    private static List<Track> GetItalyTracks()
    {
        return new List<Track>()
        {
            {
                new Track(
                    "Imola",
                    "GP",
                    48,
                    4.90f,
                    Grade.One,
                    Country.Italy
                )
            },
            {
                new Track(
                    "Imola",
                    "Historic 1972",
                    26,
                    5.01f,
                    Grade.Historic,
                    Country.Italy
                )
            },
            {
                new Track(
                    "Imola",
                    "Historic 1988",
                    26,
                    5.04f,
                    Grade.Historic,
                    Country.Italy
                )
            },
            {
                new Track(
                    "Imola",
                    "Historic 2001",
                    26,
                    4.93f,
                    Grade.Historic,
                    Country.Italy
                )
            },
            {
                new Track(
                    "Ortona",
                    "Kart Four",
                    30,
                    0.96f,
                    Grade.Kart,
                    Country.Italy
                )
            },
            {
                new Track(
                    "Ortona",
                    "Kart One",
                    30,
                    1.50f,
                    Grade.Kart,
                    Country.Italy
                )
            },
            {
                new Track(
                    "Ortona",
                    "Kart Three",
                    30,
                    1.33f,
                    Grade.Kart,
                    Country.Italy
                )
            },
            {
                new Track(
                    "Ortona",
                    "Kart Two",
                    30,
                    1.55f,
                    Grade.Kart,
                    Country.Italy
                )
            }
        };
    }

    private static List<Track> GetJapanTracks()
    {
        return new List<Track>()
        {
            {
                new Track(
                    "Kansai",
                    "Classic",
                    48,
                    5.82f,
                    Grade.Two,
                    Country.Japan
                )
            },
            {
                new Track(
                    "Kansai",
                    "East",
                    48,
                    2.24f,
                    Grade.Three,
                    Country.Japan
                )
            },
            {
                new Track(
                    "Kansai",
                    "GP",
                    48,
                    5.81f,
                    Grade.One,
                    Country.Japan
                )
            },
            {
                new Track(
                    "Kansai",
                    "West",
                    26,
                    3.46f,
                    Grade.Three,
                    Country.Japan
                )
            }
        };
    }

    private static List<Track> GetMonacoTracks()
    {
        return new List<Track>()
        {
            {
                new Track(
                    "Azure",
                    "Circuit",
                    26,
                    3.33f,
                    Grade.One,
                    Country.Monaco
                )
            }
        };
    }

    private static List<Track> GetNorwayTracks()
    {
        return new List<Track>()
        {
            {
                new Track(
                    "Buskerud",
                    "Kart Long",
                    24,
                    1.53f,
                    Grade.Kart,
                    Country.Norway
                )
            },
            {
                new Track(
                    "Buskerud",
                    "Kart Short",
                    24,
                    0.95f,
                    Grade.Kart,
                    Country.Norway
                )
            }
        };
    }

    private static List<Track> GetPortugalTracks()
    {
        return new List<Track>()
        {
            {
                new Track(
                    "Cascais",
                    "",
                    48,
                    4.18f,
                    Grade.One,
                    Country.Portugal
                )
            },
            {
                new Track(
                    "Cascais",
                    "Alternate",
                    48,
                    4.15f,
                    Grade.Three,
                    Country.Portugal
                )
            }
        };
    }

    private static List<Track> GetSouthAfricaTracks()
    {
        return new List<Track>()
        {
            {
                new Track(
                    "Kyalami",
                    "",
                    48,
                    4.52f,
                    Grade.Two,
                    Country.SouthAfrica
                )
            },
            {
                new Track(
                    "Kyalami",
                    "Historic 1976",
                    26,
                    4.04f,
                    Grade.Historic,
                    Country.SouthAfrica
                )
            }
        };
    }

    private static List<Track> GetSpainTracks()
    {
        return new List<Track>()
        {
            {
                new Track(
                    "Jerez",
                    "Chicane",
                    42,
                    4.42f,
                    Grade.One,
                    Country.Spain
                )
            },
            {
                new Track(
                    "Jerez",
                    "Moto",
                    42,
                    4.42f,
                    Grade.Three,
                    Country.Spain
                )
            }
        };
    }

    private static List<Track> GetUSATracks()
    {
        return new List<Track>()
        {
            {
                new Track(
                    "VIRginia International Raceway",
                    "Full",
                    48,
                    5.26f,
                    Grade.Two,
                    Country.USA
                )
            },
            {
                new Track(
                    "VIRginia International Raceway",
                    "Grand",
                    28,
                    6.75f,
                    Grade.Three,
                    Country.USA
                )
            },
            {
                new Track(
                    "VIRginia International Raceway",
                    "North",
                    28,
                    3.62f,
                    Grade.Three,
                    Country.USA
                )
            },
            {
                new Track(
                    "VIRginia International Raceway",
                    "Patriot",
                    14,
                    1.77f,
                    Grade.Four,
                    Country.USA
                )
            },
            {
                new Track(
                    "VIRginia International Raceway",
                    "South",
                    26,
                    2.65f,
                    Grade.Three,
                    Country.USA
                )
            }
        };
    }
}