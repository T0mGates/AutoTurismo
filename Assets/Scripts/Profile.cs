using System.Collections.Generic;

using UnityEngine;

public class Profile
{
    private string          driverName;
    private int             money;
    private int             experience;
    private int             maxExperience;
    private int             level;
    private List<Dealer>    unlockedDealers;
    private List<Car>       ownedCars;

    public Profile(string driverNameParam){
        unlockedDealers = new List<Dealer>();
        ownedCars       = new List<Car>();

        // Base values
        driverName      = driverNameParam;
        money           = 20000;
        experience      = 0;
        level           = 1;
        SetMaxExperienceBasedOnLevel();

        // Base unlocks
        UnlockDealer(Dealers.GetDealer(Dealers.VEE_NAME));
        UnlockDealer(Dealers.GetDealer(Dealers.VOLKSWAGEN_NAME));
        UnlockDealer(Dealers.GetDealer(Dealers.COPA_CLASSIC_B_NAME));
    }

    public List<Car> GetOwnedCars(){
        return ownedCars;
    }

    public void UnlockDealer(Dealer dealerToAdd){
        unlockedDealers.Add(dealerToAdd);
    }

    public void AddNewCar(Car carToAdd){
        ownedCars.Add(carToAdd);
    }

    public void RemoveOwnedCar(Car carToRemove){
        ownedCars.Remove(carToRemove);
    }

    public bool OwnsCar(Car carToCheck){
        foreach(Car car in ownedCars){
            if(car.Equals(carToCheck)){
                return true;
            }
        }
        return false;
    }

    // Returns bool of whether we leveled up or not
    public bool GainExperience(int gainedExperience){
        // Gain exp
        experience += gainedExperience;
        // If we have enough exp to level up, level up
        if(experience >= maxExperience){
            LevelUp();
            return true;
        }
        return false;
    }

    public List<Dealer> GetUnlockedDealers(){
        return unlockedDealers;
    }

    public int GetCurrentExperience(){
        return experience;
    }

    public int GetMaxExperience(){
        return maxExperience;
    }

    public void LoseMoney(int toLose){
        money -= toLose;
        if(money < 0){
            money = 0;
        }
    }

    public void GainMoney(int toGain){
        money += toGain;
    }

    public int GetMoney(){
        return money;
    }

    public int GetLevel(){
        return level;
    }

    private void SetMaxExperienceBasedOnLevel(){
        maxExperience = 100 * level;
    }

    private void LevelUp(){
        experience -= maxExperience;
        ++level;
        SetMaxExperienceBasedOnLevel();
    }
}