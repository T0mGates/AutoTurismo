using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    private MenuManager   menuManager;
    public Profile        curProfile;
    public int            curProfileSlot;

    // Start is called before the first frame update
    void Start()
    {
      menuManager = GameObject.FindWithTag("MenuManager").GetComponent<MenuManager>();
    }

    void OnApplicationQuit(){
      if(curProfile != null){
        SaveSystem.SaveProfile(curProfile, curProfileSlot);
      }
    }

    public void SetPathToJsons(string pathToJsons){
      IOManager.SetJsonDir(pathToJsons);
    }

    public void CheckRaceCompletion(EventEntry eventEntry, Car car, Button checkResultBtn){
      // First stop the user from being able to spam the button
      checkResultBtn.interactable = false;
      // Don't forget to re-enable the given checkResultBtn
      // checkResultBtn.interactable = true;

      // Holds all json files
      List<RaceResult> results = new List<RaceResult>();

      // Will hold one json file
      RaceResult result;
      while(true){
        result = IOManager.ReadInNewResult();
        if(result != null){
          // Check if this was an AMS 2 race session that has the same gridSize as our eventEntry, if yes, add it to our results
          if(result.Simulator == "AMS 2" && result.SessionType == "Race" && eventEntry.gridSize == result.Drivers.Count){
            results.Add(result);
          }
        }
        else{
          // If no json file was found, break out of the while loop
          break;
        }
      }

      // If results has no entries here, means we couldn't find a result
      if(0 == results.Count){
        checkResultBtn.interactable = true;
        menuManager.Notification("Alert",
          "Could not find an AMS 2 race session result that had a grid size (including the player) of " + eventEntry.gridSize.ToString() + " in directory: " + IOManager.GetJsonDir() +
          ".\nMake sure Second Monitor is running before, during and after the race, and make sure the race settings shown in AutoTurismo match the in-game race settings!");
        return;
      }

      // At this point, we have a valid result
      eventEntry.CompleteEventEntry(results, car);
      checkResultBtn.interactable   = true;

      menuManager.CompleteEventEntry(eventEntry);

      // If event is finished
      // No matter what, regenerate an event with a similar duration, same classes/brands/names and type
      if(eventEntry.parentEvent.GetCompletedStatus()){
        MakeNewEventForSeries(eventEntry.parentEvent, eventEntry.parentEvent.eventType);

        // Delete the event from the pool
        eventEntry.parentEvent.parentEventSeries.events.Remove(eventEntry.parentEvent);

        // Potentially add a new event (if this was a top 3) - with a max of 5
        if(eventEntry.parentEvent.finishPosition <= 3 && eventEntry.parentEvent.finishPosition > 0){
          if(eventEntry.parentEvent.parentEventSeries.events.Count < Region.MAX_EVENTS_PER_REGION){
            // For now, just flip-flop between race and champhionship
            Event.EventType eventType = eventEntry.parentEvent.eventType == Event.EventType.Race ? Event.EventType.Championship : Event.EventType.Race;
            MakeNewEventForSeries(eventEntry.parentEvent, eventType);
          }

          // Potentially add a whole new series to the region (if this was a top 3 in a championship)
          if(eventEntry.parentEvent.eventType == Event.EventType.Championship){
            GenerateNewEventSeries(eventEntry.parentEvent.parentEventSeries.partOfRegion, eventEntry.parentEvent.parentEventSeries.seriesTier);
          }
        }
      }
    }

    public void MakeNewEventForSeries(Event eventObj, Event.EventType eventType = Event.EventType.Race){
      Event.EventDuration       durationToUse;
      Event.EventDuration       oldDuration         = eventObj.eventDuration;

      List<Event.EventDuration> availableDurations  = EventSeries.tierDurationWhitelist[eventObj.parentEventSeries.seriesTier];

      // -1, 0, 1
      int                       modifier            = UnityEngine.Random.Range(-1, 2);

      int                       index               = availableDurations.IndexOf(oldDuration);
      if(-1 == index || modifier + index < 0 || modifier + index >= availableDurations.Count){
        durationToUse = oldDuration;
      }
      else{
        durationToUse = availableDurations[modifier + index];
      }

      // Generate an event with a similar duration, same classes/brands/names and type
      Event.GenerateNewEvent(
            eventObj.name,
            eventType,
            durationToUse,
            eventObj.parentEventSeries,
            Tracks.GetCountries(eventObj.parentEventSeries.partOfRegion),
            eventObj.typeWhitelist,
            eventObj.classWhitelist,
            eventObj.brandWhitelist,
            eventObj.nameWhitelist,
            useLaps:eventObj.eventEntries[0].laps == -1 ? false : true
      );
    }

    public void MakeNewProfile(string profileName){
      // Check if profile exists, if it does, load it, else create a new profile
      curProfile      = new Profile(profileName);
    }

    public void SetProfile(Profile profile, int newProfileSlot){
      curProfile      = profile;
      SetProfileSlot(newProfileSlot);
    }

    public void SetProfileSlot(int newProfileSlot){
      curProfileSlot  = newProfileSlot;
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

    public void GenerateNewEventSeries(Region.ClickableRegion region, EventSeries.SeriesTier tier){
      Cars.CarClass carClass = Region.regions[region].GenerateNewEventSeries(tier);
      curProfile.UnlockDealer(Dealers.GetDealer(Cars.classToString[carClass],   typeof(CarDealer)));
      curProfile.UnlockDealer(Dealers.GetDealer(Cars.classToString[carClass],   typeof(EntryPassDealer)));
    }

    public void BuyRegionTier(Region.ClickableRegion region, EventSeries.SeriesTier tier){
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

          // Unlock 3 series total if it's the player's first unlock
          // Unlock the dealers for those first 3 event series
          for(int i = 0; i < 3; i++){
            GenerateNewEventSeries(region, tier);
          }
        }

        else{
          curProfile.UnlockRegionTier(region, tier);
          // Decided it is best to unlock 3 no matter what for a new tier so player has more choice
          for(int i = 0; i < 3; i++){
            GenerateNewEventSeries(region, tier);
          }
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
}
