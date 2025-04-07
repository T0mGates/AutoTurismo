using System;
using UnityEngine;

[System.Serializable]
public class SerializableResultDriver
{
    public string ClassName { get; set; }
    public string ClassId { get; set; }
    public string DriverId { get; set; }
    public string DriverLongName { get; set; }
    public int FinishingPosition { get; set; }
    public int FinishingPositionInClass { get; set; }
    public float AveragePosition { get; set; }
    public int InitialPosition { get; set; }
    public int InitialPositionInClass { get; set; }
    public string CarName { get; set; }
    public int TotalLaps { get; set; }
    public float TotalDistance { get; set; }
    public string GapToPlayerRelative { get; set; }
    public int LapsDifferenceToPlayer { get; set; }
    public ResultTopSpeed TopSpeed { get; set; }
    public SerializableList<ResultLap> Laps { get; set; }
    public bool IsPlayer { get; set; }
    public bool Finished { get; set; }
    public string FinishStatus { get; set; }
    public float Rating { get; set; }
    public float RatingRelativeToPlayer { get; set; }
    public int CarNumber { get; set; }
    public string TeamName { get; set; }
    public string GapToPlayerByTiming { get; set; }

    public SerializableResultDriver(ResultDriver driver){
        ClassName                   = driver.ClassName;
        ClassId                     = driver.ClassId;
        DriverId                    = driver.DriverId;
        DriverLongName              = driver.DriverLongName;
        FinishingPosition           = driver.FinishingPosition;
        FinishingPositionInClass    = driver.FinishingPositionInClass;
        AveragePosition             = driver.AveragePosition;
        InitialPosition             = driver.InitialPosition;
        InitialPositionInClass      = driver.InitialPositionInClass;
        CarName                     = driver.CarName;
        TotalLaps                   = driver.TotalLaps;
        TotalDistance               = driver.TotalDistance;
        GapToPlayerRelative         = driver.GapToPlayerRelative;
        LapsDifferenceToPlayer      = driver.LapsDifferenceToPlayer;
        TopSpeed                    = driver.TopSpeed;

        Laps                        = new SerializableList<ResultLap>();
        foreach(ResultLap lap in driver.Laps){
            Laps.Add(lap);
        }

        IsPlayer                    = driver.IsPlayer;
        Finished                    = driver.Finished;
        FinishStatus                = driver.FinishStatus;
        Rating                      = driver.Rating;
        RatingRelativeToPlayer      = driver.RatingRelativeToPlayer;
        CarNumber                   = driver.CarNumber;
        TeamName                    = driver.TeamName;
        GapToPlayerByTiming         = driver.GapToPlayerByTiming;
    }
}