using System.Collections.Generic;
using UnityEngine;

public class RaceResult
{
    public string SessionRunTime { get; set; }
    public string SessionType { get; set; }
    public bool WasGreen { get; set; }
    public string Simulator { get; set; }
    public string SessionLengthType { get; set; }
    public int TotalNumberOfLaps { get; set; }
    public string SessionRunDuration { get; set; }
    public ResultTrack TrackInfo { get; set; }
    public List<ResultDriver> Drivers { get; set; }
    public string FuelConsumptionInformation { get; set; }
}

public class ResultTrack
{
    public string TrackName { get; set; }
    public string TrackLayoutName { get; set; }
    public ResultLayoutLength LayoutLength { get; set; }
}

public class ResultLayoutLength
{
    public float InMeters { get; set; }
}

public class ResultDriver
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
    public ResultLap[] Laps { get; set; }
    public bool IsPlayer { get; set; }
    public bool Finished { get; set; }
    public string FinishStatus { get; set; }
    public float Rating { get; set; }
    public float RatingRelativeToPlayer { get; set; }
    public string GapToPlayerByTiming { get; set; }

    public ResultDriver(){}

    public ResultDriver(SerializableResultDriver serDriver){
        ClassName                   = serDriver.ClassName;
        ClassId                     = serDriver.ClassId;
        DriverId                    = serDriver.DriverId;
        DriverLongName              = serDriver.DriverLongName;
        FinishingPosition           = serDriver.FinishingPosition;
        FinishingPositionInClass    = serDriver.FinishingPositionInClass;
        AveragePosition             = serDriver.AveragePosition;
        InitialPosition             = serDriver.InitialPosition;
        InitialPositionInClass      = serDriver.InitialPositionInClass;
        CarName                     = serDriver.CarName;
        TotalLaps                   = serDriver.TotalLaps;
        TotalDistance               = serDriver.TotalDistance;
        GapToPlayerRelative         = serDriver.GapToPlayerRelative;
        LapsDifferenceToPlayer      = serDriver.LapsDifferenceToPlayer;
        TopSpeed                    = serDriver.TopSpeed;

        Laps                        = serDriver.Laps.GetList().ToArray();

        IsPlayer                    = serDriver.IsPlayer;
        Finished                    = serDriver.Finished;
        FinishStatus                = serDriver.FinishStatus;
        Rating                      = serDriver.Rating;
        RatingRelativeToPlayer      = serDriver.RatingRelativeToPlayer;
        GapToPlayerByTiming         = serDriver.GapToPlayerByTiming;
    }
}

[System.Serializable]
public class ResultTopSpeed
{
    public float InMs { get; set; }
}

[System.Serializable]
public class ResultLap
{
    public bool IsPitLap { get; set; }
    public int LapNumber { get; set; }
    public bool IsValid { get; set; }
    public int LapStartPosition { get; set; }
    public int LapStartPositionClass { get; set; }
    public string LapTime { get; set; }
    public string Sector1 { get; set; }
    public string Sector2 { get; set; }
    public string Sector3 { get; set; }
}

public enum FinishStatus
    {
        Finished,
        Dnf
    }