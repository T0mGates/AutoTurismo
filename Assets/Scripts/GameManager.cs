using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    private MenuManager   menuManager;
    private const string  PATH_TO_JSONS  = @"C:\Users\Vitaly\Documents\SecondMonitor\Reports";
    public Profile        curProfile;

    // Start is called before the first frame update
    void Start()
    {
      menuManager = GameObject.FindWithTag("MenuManager").GetComponent<MenuManager>();

      IOManager.SetJsonDir(PATH_TO_JSONS);
      RaceResult result = IOManager.ReadInNewResult();
      for(int j = 0; j < result.Drivers.Count; j++){
        Driver driver = result.Drivers[j];
        if(driver.IsPlayer){
            // Found player
            Debug.Log("Found player: " + driver.DriverLongName + ", driving a " + driver.CarName + " to P" + driver.FinishingPosition.ToString());
        }
      }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetProfile(string profileName){
      // Check if profile exists, if it does, load it, else create a new profile
      curProfile = new Profile(profileName);
    }

    public void BuyCar(Car car, Dealer dealer){
      // Check if we have enough
      if(CanAfford(car, dealer)){
        curProfile.LoseMoney(car.price);
        // Add the car to our owned cars and update the UI
        curProfile.AddNewCar(car);
        menuManager.UpdateMainUI();
        // Refresh the dealer page
        menuManager.Dealer(dealer.name);

        // Pop up notification of the new acquisition
        menuManager.Notification("New Car Obtained!", "Congratulations!\nYou've bought a brand new car!\n\n\nMake/Model: " + car.GetPrintName() + "\nType: " + car.GetPrintType() + "\nClass: " + car.GetPrintClasses(), car.GetSprite());
      }
    }

    public void SellCar(Car car, int sellPrice){
      curProfile.GainMoney(sellPrice);
      // Remove the car from our owned cars and update the UI
      curProfile.RemoveOwnedCar(car);
      menuManager.UpdateMainUI();
      // Refresh the garage page
      menuManager.Garage();
    }

    // Can you afford this car from this dealer (are there specials on, etc)
    public bool CanAfford(Car car, Dealer dealer){
      int curMoney  = curProfile.GetMoney();

      if(curMoney >= car.price){
        return true;
      }
      return false;
    }
}
