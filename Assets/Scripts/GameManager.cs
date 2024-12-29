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
        ResultDriver driver = result.Drivers[j];
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

    public void BuyProduct(Purchasable product, Dealer dealer){
      // Check if we have enough
      if(CanAfford(product, dealer)){
        curProfile.LoseMoney(product.price);
        // Add the car to our owned cars and update the UI
        curProfile.AddNewProduct(product);
        menuManager.UpdateMainUI();
        // Refresh the dealer page
        menuManager.Dealer(dealer);

        // Popup a notification depending on the type of the newly acquired product
        if(product is Car car){
          menuManager.Notification("New Car Obtained!", "You've bought a brand new car!\n\n" + car.GetInfoBlurb(), car.GetSprite());
        }
        else if(product is EntryPass entryPass){
          menuManager.Notification("New Entry Pass Obtained!", "You've bought an entry pass!\n\n" + entryPass.GetInfoBlurb(), entryPass.GetSprite());
        }
      }
    }

    public void SellProduct(Purchasable product, int sellPrice){
      curProfile.GainMoney(sellPrice);
      // Remove the car from our owned cars and update the UI
      curProfile.RemoveOwnedProduct(product);
      menuManager.UpdateMainUI();
      // Refresh the garage/entrypass page, depending on the product type
      if(product is Car){
        menuManager.Garage();
      }
      else if(product is EntryPass){
        menuManager.EntryPasses();
      }
    }

    // Can you afford this car from this dealer (are there specials on, etc)
    public bool CanAfford(Purchasable product, Dealer dealer){
      int curMoney  = curProfile.GetMoney();

      if(curMoney >= product.price){
        return true;
      }
      return false;
    }
}
