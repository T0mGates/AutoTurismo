using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;


public class GameManager : MonoBehaviour, IDataPersistence
{
    private MenuManager   menuManager;
    private string        pathToJsons  = @"C:\Users\Vitaly\Documents\SecondMonitor\Reports";
    public Profile        curProfile;

    // Start is called before the first frame update
    void Start()
    {
      menuManager = GameObject.FindWithTag("MenuManager").GetComponent<MenuManager>();

      IOManager.SetJsonDir(pathToJsons);

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
          "Could not find an AMS 2 race session result that had a grid size (including the player) of " + eventEntry.gridSize.ToString() + " in directory: " + pathToJsons +
          ".\nMake sure Second Monitor is running before, during and after the race, and make sure the race settings shown in AutoTurismo match the in-game race settings!");
        return;
      }

      // At this point, we have a valid result
      eventEntry.CompleteEventEntry(result.Drivers, car);
      checkResultBtn.interactable   = true;

      menuManager.CompleteEventEntry(eventEntry);

      // TODO: check if event is done. If so, figure out a system to delete it from the series and add in a new one to replace it or something
      // maybe completing a rookie formula vee event adds another rookie formula vee event and also adds a novice formula vee event, if there isn't already one
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
          menuManager.Notification("New Entry Pass Obtained!", "You've bought an entry pass!\n\n" + entryPass.GetInfoBlurb(), entryPass.GetSprite(), entryPass.GetBGSprite());
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

    public void UnlockRegionTier(Region.ClickableRegion region, EventSeries.SeriesTier tier){
      // Check if we have enough
      int cost    = Region.GetRenownCostForRegionTier(region, tier);

      if(curProfile.GetRenown() >= cost){
        // Make sure we own the prerequisites
        List<Region.ClickableRegion> prereqs = Region.GetRegionPrereqs(region);
        foreach(Region.ClickableRegion regionPrereq in prereqs){
          if(!curProfile.GetUnlockedTiers(regionPrereq).Contains(tier)){
            // Can't unlock, alert the user
            menuManager.Notification("Alert",
              "You don't own the prerequisites to unlock the " + EventSeries.tierToString[tier] + " series for " + Region.regionToString[region] +
              ".\n\n You haven't unlocked the " + EventSeries.tierToString[tier] + " series for " + Region.regionToString[regionPrereq] + ", which is a prerequisite.");
              return;
          }
        }

        curProfile.LoseRenown(cost);
        // Add the region/tier to our unlocked region/tier and generate a new series for that region/tier
        if(!curProfile.HasUnlockedARegionTier()){
          curProfile.UnlockRegionTier(region, tier);

          Cars.CarClass carClass;
          // Unlock 3 series total if it's the player's first unlock
          // Unlock the dealers for those first 3 event series
          for(int i = 0; i < 3; i++){
            carClass = Region.regions[region].GenerateNewEventSeries(tier);
            curProfile.UnlockDealer(Dealers.GetDealer(Cars.classToString[carClass],   typeof(CarDealer)));
            curProfile.UnlockDealer(Dealers.GetDealer(Cars.classToString[carClass],   typeof(EntryPassDealer)));
          }
        }
        else{
          Region.regions[region].GenerateNewEventSeries(tier);
        }

        menuManager.UpdateMainUI();

        // Refresh the region's page
        menuManager.RegionSelect(region.ToString());

        // Popup a success notification
        menuManager.Notification("New Region Series Tier Unlocked!", "You've unlocked the " + EventSeries.tierToString[tier] + " series for " + Region.regionToString[region] + "!\n");
      }
      else{
        // Can't afford, alert the user
        menuManager.Notification("Alert",
          "You don't have enough renown to unlock the " + EventSeries.tierToString[tier] + " series for " + Region.regionToString[region] + ".\n\nCurrent Renown: " + curProfile.GetRenown().ToString() + "\nRenown Cost: " + cost.ToString("n0"));
      }
    }

    public void LoadData(GameData data){
        curProfile                = new Profile(data.driverName, data.money, data.fame, data.maxFame, data.level, data.renown, data.unlockedDealersDict, data.ownedProductsDict, data.unlockedRegionTiers);
        pathToJsons               = data.pathToJsons;
    }

    public void SaveData(GameData data){
        data.driverName           = curProfile.driverName;
        data.money                = curProfile.money;
        data.fame                 = curProfile.fame;
        data.maxFame              = curProfile.maxFame;
        data.level                = curProfile.level;
        data.renown               = curProfile.renown;
        data.unlockedDealersDict  = curProfile.unlockedDealersDict;
        data.ownedProductsDict    = curProfile.ownedProductsDict;
        data.unlockedRegionTiers  = curProfile.unlockedRegionTiers;

        data.pathToJsons          = pathToJsons;
    }
}
