using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;


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

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CheckRaceCompletion(EventEntry eventEntry, Car car, Button checkResultBtn){
      // First stop the user from being able to spam the button
      checkResultBtn.interactable = false;
      // Don't forget to re-enable the given checkResultBtn
      // checkResultBtn.interactable = true;

      RaceResult result;
      while(true){
        result = IOManager.ReadInNewResult();
        if(result != null){
          // Check if this was an AMS 2 race session that has the same gridSize as our eventEntry, if yes, break out of the loop
          if(result.Simulator == "AMS 2" && result.SessionType == "Race" && eventEntry.gridSize == result.Drivers.Count){
            break;
          }
          else{
            result = null;
          }
        }
        else{
          // If no json file was found, break out of the while loop
          break;
        }
      }

      // If result is null here, means we couldn't find a result
      if(null == result){
        checkResultBtn.interactable = true;
        menuManager.Notification("Alert",
          "Could not find an AMS 2 race session result that had a grid size (including the player) of " + eventEntry.gridSize.ToString() + " in directory: " + PATH_TO_JSONS +
          ".\nMake sure Second Monitor is running before, during and after the race, and make sure the race settings shown in AutoTurismo match the in-game race settings!");
        return;
      }

      eventEntry.CompleteEventEntry(result.Drivers, car);
      checkResultBtn.interactable   = true;

      menuManager.CompleteEventEntry(eventEntry);

      // TODO: check if event is done. If so, figure out a system to delete it from the series and add in a new one to replace it or something
      // maybe completing a rookie formula vee event adds another rookie formula vee event and also adds a novice formula vee event, if there isn't already one
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
      else{
        // Can't afford, alert the user
        menuManager.Notification("Alert",
          "You don't have enough money to buy: " + product.GetPrintName() + ".\n\nBalance: " + curProfile.GetPrintMoney() + "\nCost: " + product.GetPrintPrice());
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
