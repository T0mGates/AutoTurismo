using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections;
using System.Drawing.Text;
using UnityEngine.AI;


public class MenuManager : MonoBehaviour
{
    [Header("Menus")]
    public GameObject   introMenu;
    public GameObject   mainMenu;
    public GameObject   navigationMenu;
    public GameObject   newPlayerMenu;
    public GameObject   dealershipMenu;
    public GameObject   dealerMenu;
    public GameObject   garageMenu;
    public GameObject   notificationMenu;
    public GameObject   shopMenu;
    public GameObject   inventoryMenu;
    public GameObject   racingMenu;
    public GameObject   regionSeriesMenu;
    public GameObject   seriesMenu;
    public GameObject   eventEntryMenu;
    public GameObject   chooseCarMenu;
    public GameObject   eventEntryRaceMenu;
    public GameObject   eventEntryCompleteMenu;
    public GameObject   earnRewardMenu;

    [Header("Player Info")]
    public Slider       fameSlider;
    public TMP_Text     moneyText;
    public TMP_Text     fameText;
    public TMP_Text     levelText;

    [Header("Dealership")]
    public GameObject   dealerObjPrefab;
    public Transform    dealershipContentTransform;

    public GameObject   dealerCarPrefab;
    public Transform    dealerContentTransform;

    [Header("Garage")]
    public GameObject   garageCarPrefab;
    public Transform    garageContentTransform;

    private GameManager gameManager;
    private Color transparentGreen          = new Color(0.75f, 1.0f, 0.75f, 0.85f);
    private Color transparentRed            = new Color(1.0f, 0.75f, 0.75f, 0.85f);
    private Color transparentBlue           = new Color(0.75f, 0.75f, 1.0f, 0.85f);

    [Header("Region")]
    public GameObject   regionSeriesPrefab;
    public Transform    regionSeriesContentTransform;

    [Header("Series")]
    public GameObject       eventPrefab;
    public GameObject       eventEntryPrefab;
    public Transform        seriesContentTransform;

    // We will have multiple of these dynamically added to the seriesContentTransform
    private List<Transform> eventContentTransforms;

    [Header("Choose Car")]
    public GameObject       chooseCarPrefab;
    public Transform        chooseCarContentTransform;

    void Start(){
        eventContentTransforms  = new List<Transform>();

        gameManager             = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();

        TurnAllOff();

        introMenu.SetActive(true);
    }

    public void TurnAllOff(){
        mainMenu.SetActive(false);
        navigationMenu.SetActive(false);
        newPlayerMenu.SetActive(false);
        introMenu.SetActive(false);
        dealershipMenu.SetActive(false);
        dealerMenu.SetActive(false);
        garageMenu.SetActive(false);
        notificationMenu.SetActive(false);
        shopMenu.SetActive(false);
        inventoryMenu.SetActive(false);
        racingMenu.SetActive(false);
        regionSeriesMenu.SetActive(false);
        seriesMenu.SetActive(false);
        eventEntryMenu.SetActive(false);
        chooseCarMenu.SetActive(false);
        eventEntryRaceMenu.SetActive(false);
        eventEntryCompleteMenu.SetActive(false);
        earnRewardMenu.SetActive(false);

        // Destroy objects
        foreach(Transform child in dealershipContentTransform)
        {
            Destroy(child.gameObject);
        }
        foreach(Transform child in dealerContentTransform)
        {
            Destroy(child.gameObject);
        }
        foreach(Transform child in garageContentTransform)
        {
            Destroy(child.gameObject);
        }
        foreach(Transform child in regionSeriesContentTransform)
        {
            Destroy(child.gameObject);
        }
        foreach(Transform child in seriesContentTransform)
        {
            Destroy(child.gameObject);
        }
        foreach(Transform eventContentTransform in eventContentTransforms){
            foreach(Transform child in eventContentTransform){

                Destroy(child.gameObject);
            }
        }
        eventContentTransforms.Clear();
        foreach(Transform child in chooseCarContentTransform)
        {
            Destroy(child.gameObject);
        }
    }

    public void BlockNavButtons(){
        // Called to disable buttons in the nav bar
        foreach(Button btn in navigationMenu.GetComponentsInChildren<Button>()){
            btn.interactable = false;
        }
    }

    public void ActivateNavButtons(){
        // Reactivate the buttons
        foreach(Button btn in navigationMenu.GetComponentsInChildren<Button>()){
            btn.interactable = true;
        }
    }

    public void Home(bool newPlayer = false){
        TurnAllOff();
        mainMenu.SetActive(true);
        navigationMenu.SetActive(true);
        newPlayerMenu.SetActive(newPlayer);

        if(newPlayer){
            BlockNavButtons();
        }
        else{
            // Re-activate buttons
            ActivateNavButtons();
            UpdateMainUI();
        }
    }

    public void SubmitName(GameObject inputField){
        string text = inputField.GetComponent<TMP_InputField>().text;

        // Validate name
        if(text.Length > 0){
            gameManager.SetProfile(text);
            Home(false);
        }
        else{
            // Error
        }
    }

    public void UpdateMainUI(){
        fameSlider.value    = 0 == gameManager.curProfile.GetCurrentFame() ? 0 : (float)gameManager.curProfile.GetCurrentFame() / (float)gameManager.curProfile.GetMaxFame();
        // Format as a number with 0 decimals
        moneyText.text      = gameManager.curProfile.GetPrintMoney();
        fameText.text       = gameManager.curProfile.GetCurrentFame().ToString("n0") + " / " + gameManager.curProfile.GetMaxFame().ToString("n0");
        levelText.text      = "Level: " + gameManager.curProfile.GetLevel().ToString("n0");
    }

    public void Dealership(Type dealerType){
        TurnAllOff();
        navigationMenu.SetActive(true);
        dealershipMenu.SetActive(true);
        PopulateDealership(dealerType);
    }

    public void CarDealership(){
        Dealership(typeof(CarDealer));
    }

    public void EntryPassDealership(){
        Dealership(typeof(EntryPassDealer));
    }

    public void Dealer(Dealer dealer){
        TurnAllOff();
        navigationMenu.SetActive(true);
        dealerMenu.SetActive(true);
        PopulateDealer(dealer);
    }

    public void Garage(){
        TurnAllOff();
        navigationMenu.SetActive(true);
        garageMenu.SetActive(true);
        PopulateGarage(typeof(Car));
    }

    public void EntryPasses(){
        TurnAllOff();
        navigationMenu.SetActive(true);
        garageMenu.SetActive(true);
        PopulateGarage(typeof(EntryPass));
    }

    public void Shop(){
        TurnAllOff();
        navigationMenu.SetActive(true);
        shopMenu.SetActive(true);
    }

    public void Inventory(){
        TurnAllOff();
        navigationMenu.SetActive(true);
        inventoryMenu.SetActive(true);
    }

    public void Racing(){
        TurnAllOff();
        navigationMenu.SetActive(true);
        racingMenu.SetActive(true);
    }

    public void RegionSeries(string clickableRegionName){
        foreach(Tracks.ClickableRegion region in Enum.GetValues(typeof(Tracks.ClickableRegion))){
            // Look if the string representation of the enum is the given region name
            if(region.ToString() == clickableRegionName.Replace(" ", "")){
                TurnAllOff();
                navigationMenu.SetActive(true);
                regionSeriesMenu.SetActive(true);
                PopulateRegionSeries(region);
            }
        }
    }

    // Series screen, show all event series for a country's series
    public void Series(EventSeries series){
        TurnAllOff();
        navigationMenu.SetActive(true);
        seriesMenu.SetActive(true);
        PopulateSeries(series);
    }

    // Event entry screen, show all info for an event entry
    public void EventEntry(EventEntry eventEntry){
        TurnAllOff();
        navigationMenu.SetActive(true);
        eventEntryMenu.SetActive(true);
        PopulateEventEntry(eventEntry);
    }

    // Event entry race screen, show all info for an IN SESSION event entry
    public void EventEntryRace(EventEntry eventEntry, Car car){
        TurnAllOff();
        navigationMenu.SetActive(true);
        eventEntryRaceMenu.SetActive(true);
        PopulateEventEntryRace(eventEntry, car);
    }

    // Event entry screen, show all info for an event entry
    public void ChooseCar(EventEntry eventEntry){
        TurnAllOff();
        navigationMenu.SetActive(true);
        chooseCarMenu.SetActive(true);
        PopulateChooseCar(eventEntry);
    }

    public void CompleteEventEntry(EventEntry eventEntry){
        const string TITLE_TEXT_NAME        = "TitleTxt";
        const string BODY_TEXT_NAME         = "BodyTxt";
        const string REWARD_IMG_NAME        = "RewardImg";

        const string NEXT_BTN_NAME          = "NextBtn";

        TurnAllOff();
        navigationMenu.SetActive(true);
        // Block nav buttons
        BlockNavButtons();
        // Cover the background with the menu
        mainMenu.SetActive(true);


        eventEntryCompleteMenu.transform.Find(TITLE_TEXT_NAME).GetComponent<TextMeshProUGUI>().text     = "Event Entry Complete";
        eventEntryCompleteMenu.transform.Find(BODY_TEXT_NAME).GetComponent<TextMeshProUGUI>().text      = eventEntry.GetResults();

        eventEntryCompleteMenu.transform.Find(REWARD_IMG_NAME).GetComponent<Image>().sprite             = eventEntry.playerCar.GetSprite();

        List<dynamic[]> queuedRewards                                                                    = new List<dynamic[]>();
        // If the whole event is finished, queue up the event rewards after the event entry rewards
        if(eventEntry.parentEvent.GetCompletedStatus()){
            dynamic[] eventParams = { "Event Rewards", eventEntry.parentEvent.GetRewardInfo(), eventEntry.parentEvent.GetFameReward(), eventEntry.parentEvent.GetMoneyReward() };
            queuedRewards.Add(eventParams);
        }


        // Set next btn on click action
        Button nextBtn                                                                                  = eventEntryCompleteMenu.transform.Find(NEXT_BTN_NAME).GetComponent<Button>();
        nextBtn.onClick.RemoveAllListeners();
        nextBtn.onClick.AddListener(() => {
            // Earn reward notification
            EarnReward("Event Entry Rewards", eventEntry.GetRewardInfo(), eventEntry.GetFameReward(), eventEntry.GetMoneyReward(), queuedRewards);
        });

        eventEntryCompleteMenu.SetActive(true);
    }

    public void EarnReward(string title, string bodyText, int fameReward, int moneyReward, List<dynamic[]> queuedRewards = null){
        // queuedRewards is a list of arrays of 2 strings, 2 ints each (matching this function's parameters). This will queue up those rewards consecutively one after another

        const string TITLE_TEXT_NAME        = "TitleTxt";
        const string BODY_TEXT_NAME         = "BodyTxt";
        const string REWARD_IMG_NAME        = "RewardImg";

        const string CLAIM_BTN_NAME         = "ClaimRewardsBtn";
        const string CLAIM_BTN_TEXT_NAME    = "ClaimRewardsBtn/ClaimRewardsTxt";

        Button nextBtn                      = earnRewardMenu.transform.Find(CLAIM_BTN_NAME).GetComponent<Button>();
        TextMeshProUGUI nextBtnTxt          = earnRewardMenu.transform.Find(CLAIM_BTN_TEXT_NAME).GetComponent<TextMeshProUGUI>();

            // Inner animation function
            IEnumerator EarnRewardAnimation(string title, string bodyText, int fameReward, int moneyReward){
                nextBtn.onClick.RemoveAllListeners();
                // If we have any queued rewards, simply call this function again with 1 less queued reward (so, go through the list one by one)
                if(queuedRewards.Count > 0){
                    dynamic[] rewardParams = queuedRewards[0];
                    // If it is not a 4 string array, just ignore it all and go back to home, since it's ... a bug... not supposed to happen
                    if(rewardParams.Length != 4){
                        Debug.LogError("Queued reward was found to have " + rewardParams.Length.ToString() + ", but 4 params are expected. Skipping the reward.");
                        nextBtn.onClick.AddListener(()  => {
                            // On click, go back to home menu
                            Home();
                        });
                    }
                    else{
                        queuedRewards.RemoveAt(0);
                        // Now can safely call this function again
                        nextBtn.onClick.AddListener(()  => {
                            // On click, earn reward notification
                            EarnReward(rewardParams[0], rewardParams[1], rewardParams[2], rewardParams[3], queuedRewards);
                        });
                    }
                }
                else{
                    nextBtn.onClick.AddListener(()  => {
                        // On click, go back to home menu
                        Home();
                    });
                }

                // Hide the button until animations are done playing, change text to 'Clear'
                nextBtn.gameObject.SetActive(false);
                nextBtnTxt.text             = "Clear";

                // Start the 'animation'
                int oldLevel    = gameManager.curProfile.GetLevel();
                int gained      = 0;
                float curDelay  = 0.025f;
                bool reachedMax = false;
                bool doneFame   = false;

                // Can tweak these
                float decrease  = 0.0005f;
                float minimum   = 0.0005f;
                int curIncrease = 1;
                int maxIncrease = 6;

                while(gained < Mathf.Max(fameReward, moneyReward)){
                    // Fame
                    if(!doneFame){
                        if(gained < fameReward ){
                            gameManager.curProfile.GainFame(1);
                        }
                        else{
                            doneFame = true;
                            curIncrease++;
                        }
                    }

                    // Money
                    if(gained < moneyReward){
                        if(gained + curIncrease >= moneyReward){
                            curIncrease = moneyReward - gained;
                        }
                        gameManager.curProfile.GainMoney(curIncrease);
                    }

                    gained += curIncrease;
                    UpdateMainUI();
                    yield return new WaitForSeconds(curDelay);
                    // Gets faster and faster, but down to a limit
                    if(!reachedMax){
                        curDelay -= decrease;
                    }
                    else{
                        // Increase money gain rate if we have reached max speed
                        if(doneFame){
                            curIncrease = maxIncrease;
                        }
                    }

                    if(curDelay <= minimum && !reachedMax){
                        Debug.Log("reached MAX");
                        reachedMax = true;
                    }
                }

                int newLevel    = gameManager.curProfile.GetLevel();

                if(newLevel > oldLevel){
                    // Level up notification
                }

                nextBtn.gameObject.SetActive(true);

            }

        TurnAllOff();
        navigationMenu.SetActive(true);
        // Block nav buttons
        BlockNavButtons();
        // Cover the background with the menu
        mainMenu.SetActive(true);

        // Set text to claim, since it was set to something else beforehand
        nextBtnTxt.text                                                                         = "Claim";

        earnRewardMenu.transform.Find(TITLE_TEXT_NAME).GetComponent<TextMeshProUGUI>().text     = title;
        earnRewardMenu.transform.Find(BODY_TEXT_NAME).GetComponent<TextMeshProUGUI>().text      = bodyText;

        // eventEntryCompleteMenu.transform.Find(REWARD_IMG_NAME).GetComponent<Image>().sprite             = eventEntry.playerCar.GetSprite();

        // Set next btn on click action
        nextBtn.onClick.RemoveAllListeners();
        nextBtn.onClick.AddListener(() => {
            // On click, start the animation
            StartCoroutine(EarnRewardAnimation(title, bodyText, fameReward, moneyReward));
            });

        earnRewardMenu.SetActive(true);
    }

    public enum NotificationType
    {
        Normal,
        PurchaseConfirmation
    }

    public void Notification(string title, string bodyText, Sprite rewardSprite = null, NotificationType notificationType = NotificationType.Normal){
        const string TITLE_TEXT_NAME        = "TitleTxt";
        const string BODY_TEXT_NAME         = "NotificationTxt";
        const string REWARD_IMG_NAME        = "RewardImg";

        const string NORMAL_NOTI_NAME       = "NormalNotification";
        const string CONFIRMATION_NAME      = "PurchaseConfirmation";

        // Cover the background with the menu
        mainMenu.SetActive(true);

        // Block navigation buttons
        BlockNavButtons();

        notificationMenu.transform.Find(TITLE_TEXT_NAME).GetComponent<TextMeshProUGUI>().text   = title;
        notificationMenu.transform.Find(BODY_TEXT_NAME).GetComponent<TextMeshProUGUI>().text    = bodyText;

        if(rewardSprite != null){
            notificationMenu.transform.Find(REWARD_IMG_NAME).GetComponent<Image>().sprite       = rewardSprite;
            notificationMenu.transform.Find(REWARD_IMG_NAME).gameObject.SetActive(true);
        }
        else{
            notificationMenu.transform.Find(REWARD_IMG_NAME).gameObject.SetActive(false);
        }

        notificationMenu.transform.Find(NORMAL_NOTI_NAME).gameObject.SetActive(notificationType == NotificationType.Normal);
        notificationMenu.transform.Find(CONFIRMATION_NAME).gameObject.SetActive(notificationType == NotificationType.PurchaseConfirmation);

        notificationMenu.SetActive(true);
    }

    public void ClearNotification(){
        notificationMenu.SetActive(false);
        mainMenu.SetActive(false);

        ActivateNavButtons();
    }

    private void PopulateDealership(Type dealerType){
        const string TITLE_TEXT_NAME    = "DealershipTxt";
        const string BG_IMAGE_NAME      = "DealerBtn/BG";
        const string DEALER_TEXT_NAME   = "DealerTxt";
        const string DEALER_BTN_NAME    = "DealerBtn";

        dealershipMenu.gameObject.transform.Find(TITLE_TEXT_NAME).GetComponent<TextMeshProUGUI>().text         = typeof(CarDealer).IsAssignableFrom(dealerType) ? "Choose a Car Dealer" : typeof(EntryPassDealer).IsAssignableFrom(dealerType) ? "Choose an Entry Pass Dealer" : "Unknown Dealership";

        GameObject      newObj;
        List<Dealer>    dealers =  gameManager.curProfile.GetUnlockedDealers(dealerType);

        // For every dealer, make a small interactable button
        for(int i = 0; i < dealers.Count; i++){
            Dealer dealer   = dealers[i];
            newObj          = (GameObject)Instantiate(dealerObjPrefab, dealershipContentTransform);
            newObj.transform.Find(BG_IMAGE_NAME).GetComponent<Image>().sprite                   = dealer.GetSprite();
            newObj.transform.Find(DEALER_TEXT_NAME).GetComponent<TextMeshProUGUI>().text        = dealer.name;

            newObj.transform.Find(DEALER_BTN_NAME).GetComponent<Button>().onClick.RemoveAllListeners();
            newObj.transform.Find(DEALER_BTN_NAME).GetComponent<Button>().onClick.AddListener(() => { Dealer(dealer); });
        }
    }

    private void PopulateDealer(Dealer dealer){
        const string PRODUCT_IMAGE_NAME = "ProductImg";
        const string NAME_TEXT_NAME     = "ProductNameTxt";
        const string CLASSES_TEXT_NAME  = "ProductClassesTxt";
        const string TYPE_TEXT_NAME     = "ProductTypeTxt";
        const string BUY_BTN_NAME       = "PriceBuy/BuyBtn";
        const string BUY_BTN_TEXT_NAME  = "PriceBuy/BuyBtn/BuyTxt";
        const string PRICE_TEXT_NAME    = "PriceBuy/PriceTxt";

        const string DEALER_TEXT_NAME   = "DealerTxt";
        const string BACK_BTN_NAME      = "BackBtn";

        GameObject          newObj;
        List<Purchasable>   products            = dealer.products;

        // Set dealer title
        dealerMenu.transform.Find(DEALER_TEXT_NAME).GetComponent<TextMeshProUGUI>().text        = dealer.name;
        // Set onclick for the back button, depending on what kind of dealer this is
        if(dealer       is CarDealer){
            dealerMenu.transform.Find(BACK_BTN_NAME).GetComponent<Button>().onClick.RemoveAllListeners();
            dealerMenu.transform.Find(BACK_BTN_NAME).GetComponent<Button>().onClick.AddListener(() => { CarDealership(); });
        }
        else if(dealer  is EntryPassDealer){
            dealerMenu.transform.Find(BACK_BTN_NAME).GetComponent<Button>().onClick.RemoveAllListeners();
            dealerMenu.transform.Find(BACK_BTN_NAME).GetComponent<Button>().onClick.AddListener(() => { EntryPassDealership(); });
        }

        foreach(Purchasable product in products){
            bool owned                                                                          = false;
            newObj                                                                              = (GameObject)Instantiate(dealerCarPrefab, dealerContentTransform);

            // Set BG to green for now
            newObj.GetComponent<Image>().color                                                  = transparentGreen;

            // Change the image
            newObj.transform.Find(PRODUCT_IMAGE_NAME).GetComponent<Image>().sprite              = product.GetSprite();

            // Change the product name
            newObj.transform.Find(NAME_TEXT_NAME).GetComponent<TextMeshProUGUI>().text          = product.GetPrintName();

            if(product is Car car){
                // Change the class(es) if it exists
                newObj.transform.Find(CLASSES_TEXT_NAME).GetComponent<TextMeshProUGUI>().text   = car.GetPrintClasses();

                // Change the type if it exists
                newObj.transform.Find(TYPE_TEXT_NAME).GetComponent<TextMeshProUGUI>().text      = car.GetPrintType();

                owned                                                                           = gameManager.curProfile.OwnsProduct(car);
            }
            else if(product is EntryPass entryPass){
                // Change the class(es) if it exists
                newObj.transform.Find(CLASSES_TEXT_NAME).GetComponent<TextMeshProUGUI>().text   = entryPass.GetPrintClasses();

                // Change the type if it exists
                newObj.transform.Find(TYPE_TEXT_NAME).GetComponent<TextMeshProUGUI>().text      = entryPass.GetPrintType();

                owned                                                                           = gameManager.curProfile.OwnsProduct(entryPass);
            }
            else{
                // Set classes to null since this item doesn't implement it
                newObj.transform.Find(CLASSES_TEXT_NAME).GetComponent<TextMeshProUGUI>().text   = "";

                // Set type to null since this item doesn't implement it
                newObj.transform.Find(TYPE_TEXT_NAME).GetComponent<TextMeshProUGUI>().text      = "";
            }

            // Change the buy button onclick action
            // If we own the product, disable the button
            Button buyBtn = newObj.transform.Find(BUY_BTN_NAME).GetComponent<Button>();
            if(owned){
                buyBtn.interactable = false;
                newObj.transform.Find(BUY_BTN_TEXT_NAME).GetComponent<TextMeshProUGUI>().text   = "Owned";
                newObj.GetComponent<Image>().color                                              = transparentBlue;
            }
            // If we don't own the product and can't afford it, change BG to red
            else if(!gameManager.CanAfford(product, dealer)){
                newObj.GetComponent<Image>().color                                              = transparentRed;
            }

            buyBtn.onClick.RemoveAllListeners();
            buyBtn.onClick.AddListener(() => { ConfirmPurchasablePopup(product, dealer); });

            // Change the price (text)
            newObj.transform.Find(PRICE_TEXT_NAME).GetComponent<TextMeshProUGUI>().text         = product.GetPrintPrice();
        }
    }

    private void PopulateGarage(Type productType){
        const string        PRODUCT_IMAGE_NAME = "ProductImg";
        const string        NAME_TEXT_NAME     = "ProductNameTxt";
        const string        CLASSES_TEXT_NAME  = "ProductClassesTxt";
        const string        TYPE_TEXT_NAME     = "ProductTypeTxt";
        const string        SELL_BTN_NAME      = "PriceSell/SellBtn";
        const string        PRICE_TEXT_NAME    = "PriceSell/PriceTxt";

        const string        TITLE_TEXT_NAME    = "TitleTxt";

        GameObject          newObj;
        List<Purchasable>   products           = gameManager.curProfile.GetOwnedProducts(productType);

        // Change the title depending on what the product type is
        if(typeof(EntryPass).IsAssignableFrom(productType)){
            garageMenu.transform.Find(TITLE_TEXT_NAME).GetComponent<TextMeshProUGUI>().text     = "Owned Entry Passes";
        }
        else if(typeof(Car).IsAssignableFrom(productType)){
            garageMenu.transform.Find(TITLE_TEXT_NAME).GetComponent<TextMeshProUGUI>().text     = "Owned Cars";
        }
        else{
            garageMenu.transform.Find(TITLE_TEXT_NAME).GetComponent<TextMeshProUGUI>().text     = "Unknown";
        }

        foreach(Purchasable product in products){

            newObj                                                                              = (GameObject)Instantiate(garageCarPrefab, garageContentTransform);

            // Set BG to blue (owned)
            newObj.GetComponent<Image>().color                                                  = transparentBlue;

            // Change the image
            newObj.transform.Find(PRODUCT_IMAGE_NAME).GetComponent<Image>().sprite              = product.GetSprite();

            // Change the car name
            newObj.transform.Find(NAME_TEXT_NAME).GetComponent<TextMeshProUGUI>().text          = product.GetPrintName();

            if(product is Car car){
                // Change the car class(es)
                newObj.transform.Find(CLASSES_TEXT_NAME).GetComponent<TextMeshProUGUI>().text   = car.GetPrintClasses();
                // Change the car type
                newObj.transform.Find(TYPE_TEXT_NAME).GetComponent<TextMeshProUGUI>().text      = car.GetPrintType();
            }
            else if(product is EntryPass entryPass){
                // Change the pass class(es)
                newObj.transform.Find(CLASSES_TEXT_NAME).GetComponent<TextMeshProUGUI>().text   = entryPass.GetPrintClasses();
                // Change the pass type
                newObj.transform.Find(TYPE_TEXT_NAME).GetComponent<TextMeshProUGUI>().text      = entryPass.GetPrintType();
            }

            // Change the sell button onclick action
            // If we only own one car, disable the button
            Button sellBtn = newObj.transform.Find(SELL_BTN_NAME).GetComponent<Button>();
            if(1 == products.Count){
                sellBtn.interactable = false;
                newObj.GetComponent<Image>().color                                              = transparentRed;
            }

            sellBtn.onClick.RemoveAllListeners();
            sellBtn.onClick.AddListener(() => { ConfirmPurchasablePopup(product, null); });

            // Change the price (text)
            newObj.transform.Find(PRICE_TEXT_NAME).GetComponent<TextMeshProUGUI>().text         = product.GetSellPrice();
        }
    }

    private void PopulateRegionSeries(Tracks.ClickableRegion region){
        const string        REGION_TEXT_NAME    = "RegionSeriesTxt";

        // These are for the prefab
        const string        SERIES_TITLE_NAME   = "SeriesNameTxt";
        const string        SERIES_TIER_NAME    = "SeriesTierTxt";
        const string        VIEW_BTN_NAME       = "ViewBtn";

        // These will be put before and after the series tier text
        const string        SERIES_TIER_PREFIX  = "Only for ";
        const string        SERIES_TIER_SUFFIX  = " Drivers";
        // Don't forget image later on

        GameObject          newObj;
        List<EventSeries>   seriesList          = EventSeriesManager.GetRegionSeries(region);

        // Change the title depending on what the country type is
        regionSeriesMenu.transform.Find(REGION_TEXT_NAME).GetComponent<TextMeshProUGUI>().text = Tracks.regionToString[region];

        foreach(EventSeries series in seriesList){

            newObj                                                                              = (GameObject)Instantiate(regionSeriesPrefab, regionSeriesContentTransform);

            //TODO: Change the image
            //newObj.transform.Find(PRODUCT_IMAGE_NAME).GetComponent<Image>().sprite              = product.GetSprite();

            // Change the series name
            newObj.transform.Find(SERIES_TITLE_NAME).GetComponent<TextMeshProUGUI>().text       = series.name;

            // Change the tier text
            newObj.transform.Find(SERIES_TIER_NAME).GetComponent<TextMeshProUGUI>().text        = SERIES_TIER_PREFIX + EventSeriesManager.tierToString[series.seriesTier] + SERIES_TIER_SUFFIX;

            // Change the view button onclick action
            Button viewBtn = newObj.transform.Find(VIEW_BTN_NAME).GetComponent<Button>();

            viewBtn.onClick.RemoveAllListeners();
            viewBtn.onClick.AddListener(() => { Series(series); });
        }
    }

    private void PopulateSeries(EventSeries series){
        // Country name is needed to know which menu the back btn will go to
        // Tier is needed for the text

        const string        SERIES_TITLE_NAME               = "SeriesTxt";
        const string        SERIES_TIER_NAME                = "SeriesTierTxt";
        const string        BACK_BTN_NAME                   = "BackBtn";

        // These are for the event prefab
        const string        EVENT_TITLE_NAME                = "EventNameTxt";
        const string        EVENT_TYPE_NAME                 = "EventTypeTxt";
        const string        MONEY_TEXT_NAME                 = "Reward/MoneyRewardTxt";
        const string        FAME_TEXT_NAME                  = "Reward/FameRewardTxt";
        const string        WHITELIST_TEXT_NAME             = "WhitelistTxt";
        const string        EVENT_CONTENT_TRANSFORM_NAME    = "EventEntriesScrollView/Viewport/EventEntriesContent";

        // Don't forget image later on

        GameObject          newObj;

        // Change the series title
        seriesMenu.transform.Find(SERIES_TITLE_NAME).GetComponent<TextMeshProUGUI>().text       = series.name;
        // Change the series tier
        seriesMenu.transform.Find(SERIES_TIER_NAME).GetComponent<TextMeshProUGUI>().text        = EventSeriesManager.tierToString[series.seriesTier] + " Drivers Only";

        // Set onclick for the back button, depending on what menu preceeded this one
        seriesMenu.transform.Find(BACK_BTN_NAME).GetComponent<Button>().onClick.RemoveAllListeners();
        seriesMenu.transform.Find(BACK_BTN_NAME).GetComponent<Button>().onClick.AddListener(()  => { RegionSeries(series.partOfRegion.ToString()); });

        foreach(Event seriesEvent in series.events){

            newObj                                                                              = (GameObject)Instantiate(eventPrefab, seriesContentTransform);

            //TODO: Change the image
            //newObj.transform.Find(PRODUCT_IMAGE_NAME).GetComponent<Image>().sprite              = product.GetSprite();

            // Change the event name
            newObj.transform.Find(EVENT_TITLE_NAME).GetComponent<TextMeshProUGUI>().text        = seriesEvent.name;

            // Change the event type text
            newObj.transform.Find(EVENT_TYPE_NAME).GetComponent<TextMeshProUGUI>().text         = seriesEvent.GetPrintEventType();

            // Change the money reward text
            newObj.transform.Find(MONEY_TEXT_NAME).GetComponent<TextMeshProUGUI>().text         = seriesEvent.GetPrintTopMoneyReward();

            // Change the fame reward text
            newObj.transform.Find(FAME_TEXT_NAME).GetComponent<TextMeshProUGUI>().text          = seriesEvent.GetPrintTopFameReward();

            // Change the whitelist text
            newObj.transform.Find(WHITELIST_TEXT_NAME).GetComponent<TextMeshProUGUI>().text     = seriesEvent.GetPrintWhitelist();

            // Now do the inner event entries for this event, after locating the eventContentTransform found in the newly made prefab
            Transform eventContentTransform                                                     = newObj.transform.Find(EVENT_CONTENT_TRANSFORM_NAME).GetComponent<Transform>();
            eventContentTransforms.Add(eventContentTransform);
            PopulateEvent(seriesEvent);
        }
    }

    private void PopulateEvent(Event seriesEvent){
        // These are for the event entry prefab
        const string        ENTRY_TITLE_NAME                                                = "EventEntryTxt";

        GameObject          newObj;

        int count                                                                           = 1;

        bool foundNextUp                                                                    = false;

        foreach(EventEntry eventEntry in seriesEvent.GetEventEntries()){
            // The eventContentTransform to use will be the most newly added transform to the eventContentTransforms list
            newObj                                                                          = (GameObject)Instantiate(eventEntryPrefab, eventContentTransforms[eventContentTransforms.Count-1]);

            // Change the event entry's text
            newObj.transform.Find(ENTRY_TITLE_NAME).GetComponent<TextMeshProUGUI>().text    = count.ToString();
            count++;

            if(foundNextUp){
                // If it is after the 'next up to race' eventEntry, reduce the alpha value to show it is not raceable yet
                Image entryImg                                                                  = newObj.transform.GetComponent<Image>();
                Color tmpColor                                                                  = entryImg.color;
                tmpColor.a                                                                      = 0.50f;
                entryImg.color                                                                  = tmpColor;
            }
            else{
                // Check if it is next up, if yes, next entries should be dimmed
                if(eventEntry.nextUp){
                    foundNextUp                                                                 = true;
                }
            }

            // Change the event entry button's onclick action
            Button eventEntryBtn                                                                = newObj.GetComponent<Button>();

            eventEntryBtn.onClick.RemoveAllListeners();
            eventEntryBtn.onClick.AddListener(()                                                => { EventEntry(eventEntry); });
        }
    }

    private void PopulateEventEntry(EventEntry eventEntry){
        const string        CHOOSE_CAR_BTN_NAME         = "ChooseCarBtn";

        PopulateEventEntryCommonFunction(eventEntry, eventEntryMenu);

        Button chooseCarBtn                             = eventEntryMenu.transform.Find(CHOOSE_CAR_BTN_NAME).GetComponent<Button>();

        chooseCarBtn.onClick.RemoveAllListeners();
        chooseCarBtn.onClick.AddListener(()             => { ChooseCar(eventEntry); });
    }

    private void PopulateChooseCar(EventEntry eventEntry){

        // Menu where you choose a car for an event
        const string                    PRODUCT_IMAGE_NAME      = "CarImg";
        const string                    NAME_TEXT_NAME          = "CarNameTxt";
        const string                    CLASSES_TEXT_NAME       = "CarClassesTxt";
        const string                    TYPE_TEXT_NAME          = "CarTypeTxt";
        const string                    NO_CAR_OBJ_NAME         = "NoCarObj";
        const string                    NO_CAR_TEXT_NAME        = "NoCarTxt";
        const string                    CHOOSE_BTN_NAME         = "ChooseBtn";

        const string                    BACK_BTN_NAME           = "BackBtn";

        GameObject          newObj;

        EventSeriesManager.SeriesTier   seriesTier              = eventEntry.parentEvent.parentEventSeries.seriesTier;
        List<Cars.CarType>              allowedCarTypes         = eventEntry.parentEvent.typeWhitelist;
        List<Cars.CarClass>             allowedCarClasses       = eventEntry.parentEvent.classWhitelist;
        List<Cars.CarBrand>             allowedCarBrands        = eventEntry.parentEvent.brandWhitelist;
        List<string>                    allowedCarNames         = eventEntry.parentEvent.nameWhitelist;

        // Change the back button onclick action
        Button backBtn = chooseCarMenu.transform.Find(BACK_BTN_NAME).GetComponent<Button>();

        backBtn.onClick.RemoveAllListeners();
        backBtn.onClick.AddListener(() => { EventEntry(eventEntry); });

        // Get your owned cars that you can race in this event entry
        // These are the cars you own that fit in this event (but you may or may not have the proper entry pass)
        List<Car>           legalOwnedCars                  = gameManager.curProfile.GetOwnedCarsFiltered(allowedCarNames, allowedCarTypes, allowedCarClasses, allowedCarBrands);
        // These are the cars that you own that fit in this event, that you do have an entry pass for
        List<Car>           legalRaceableOwnedCars          = gameManager.curProfile.GetOwnedCarsThatCanRaceEvent(allowedCarNames, allowedCarTypes, allowedCarClasses, allowedCarBrands, seriesTier);

        GameObject          noCarObj                        = chooseCarMenu.transform.Find(NO_CAR_OBJ_NAME).gameObject;
        // If we don't have any valid cars, alert the user in some way
        if(0 == legalOwnedCars.Count){
            noCarObj.gameObject.transform.Find(NO_CAR_TEXT_NAME).GetComponent<TextMeshProUGUI>().text = eventEntry.parentEvent.GetPrintWhitelist();
            noCarObj.SetActive(true);
        }
        else{
            noCarObj.SetActive(false);
        }

        foreach(Car car in legalOwnedCars){

            newObj                                                                              = (GameObject)Instantiate(chooseCarPrefab, chooseCarContentTransform);

            // Set BG to green if this car is raceable (have an entry pass that works with it and the event), else set BG to red
            newObj.GetComponent<Image>().color                                                  = legalRaceableOwnedCars.Contains(car) ? transparentGreen : transparentRed;

            // Change the image
            newObj.transform.Find(PRODUCT_IMAGE_NAME).GetComponent<Image>().sprite              = car.GetSprite();

            // Change the car name
            newObj.transform.Find(NAME_TEXT_NAME).GetComponent<TextMeshProUGUI>().text          = car.GetPrintName();

            // Change the car class(es)
            newObj.transform.Find(CLASSES_TEXT_NAME).GetComponent<TextMeshProUGUI>().text       = car.GetPrintClasses();
            // Change the car type
            newObj.transform.Find(TYPE_TEXT_NAME).GetComponent<TextMeshProUGUI>().text          = car.GetPrintType();

            // Change the 'choose' button onclick action
            Button chooseBtn                                                                    = newObj.transform.Find(CHOOSE_BTN_NAME).GetComponent<Button>();

            chooseBtn.onClick.RemoveAllListeners();
            // If you don't own an entry pass for this legal car, alert the user
            chooseBtn.onClick.AddListener(()                                                    => { ChooseCarClicked(eventEntry, car, legalRaceableOwnedCars.Contains(car)); });
        }
    }

    private void PopulateEventEntryRace(EventEntry eventEntry, Car car){
        const string        CHECK_RESULT_BTN_NAME       = "CheckResultBtn";
        const string        CAR_TEXT_NAME               = "Car/CarNameTxt";
        const string        CAR_INFO_TEXT_NAME          = "Car/CarInfoTxt";
        const string        CAR_IMAGE_NAME              = "Car/CarImg";

        PopulateEventEntryCommonFunction(eventEntry, eventEntryRaceMenu);

        // Change the image
        eventEntryRaceMenu.transform.Find(CAR_IMAGE_NAME).GetComponent<Image>().sprite              = car.GetSprite();

        // Change the car name
        eventEntryRaceMenu.transform.Find(CAR_TEXT_NAME).GetComponent<TextMeshProUGUI>().text       = car.GetPrintName();

        // Change the car type and class(es)
        eventEntryRaceMenu.transform.Find(CAR_INFO_TEXT_NAME).GetComponent<TextMeshProUGUI>().text  = "Type: " + car.GetPrintType() + "\nClasses: " + car.GetPrintClasses();

        Button checkResultBtn                                                                       = eventEntryRaceMenu.transform.Find(CHECK_RESULT_BTN_NAME).GetComponent<Button>();

        checkResultBtn.onClick.RemoveAllListeners();
        checkResultBtn.onClick.AddListener(()                                                       => { gameManager.CheckRaceCompletion(eventEntry, car, checkResultBtn); });
    }

    private void PopulateEventEntryCommonFunction(EventEntry eventEntry, GameObject menuToEdit){
        const string        ENTRY_TITLE_NAME            = "TitleTxt";
        const string        ENTRY_NUM_TEXT_NAME         = "EventEntryNumTxt";
        const string        TRACK_TEXT_NAME             = "TrackTxt";
        const string        COUNTRY_TEXT_NAME           = "CountryTxt";
        const string        GRADE_TEXT_NAME             = "GradeTxt";
        const string        GRID_SIZE_TEXT_NAME         = "GridSizeTxt";
        const string        LAPS_MINS_TEXT_NAME         = "LapsMinsTxt";
        const string        WHITELIST_TEXT_NAME         = "WhitelistTxt";
        const string        BG_IMAGE_NAME               = "BG";
        const string        BACK_BTN_NAME               = "BackBtn";

        // Change all the text
        menuToEdit.transform.Find(ENTRY_TITLE_NAME).GetComponent<TextMeshProUGUI>().text        = eventEntry.parentEvent.name;
        menuToEdit.transform.Find(ENTRY_NUM_TEXT_NAME).GetComponent<TextMeshProUGUI>().text     = "Race "                           + eventEntry.parentEvent.GetEventEntryPosition(eventEntry).ToString() + " of " + eventEntry.parentEvent.GetEventEntries().Count.ToString();
        menuToEdit.transform.Find(TRACK_TEXT_NAME).GetComponent<TextMeshProUGUI>().text         = "Name: "                          + eventEntry.track.GetPrintName();
        menuToEdit.transform.Find(COUNTRY_TEXT_NAME).GetComponent<TextMeshProUGUI>().text       = "Country: "                       + Tracks.countryToString[eventEntry.track.country];
        menuToEdit.transform.Find(GRADE_TEXT_NAME).GetComponent<TextMeshProUGUI>().text         = "Grade: "                         + Tracks.gradeToString[eventEntry.track.grade];
        menuToEdit.transform.Find(GRID_SIZE_TEXT_NAME).GetComponent<TextMeshProUGUI>().text     = "Grid Size (includes Player): "   + eventEntry.gridSize.ToString();
        menuToEdit.transform.Find(LAPS_MINS_TEXT_NAME).GetComponent<TextMeshProUGUI>().text     = eventEntry.laps != -1 ? "Laps: "  + eventEntry.laps.ToString() : "Mins: " + eventEntry.mins.ToString();
        menuToEdit.transform.Find(WHITELIST_TEXT_NAME).GetComponent<TextMeshProUGUI>().text     = "Entry Pass Tier: "               + eventEntry.parentEvent.parentEventSeries.seriesTier.ToString() + "\n\n" + eventEntry.parentEvent.GetPrintWhitelist();

        // Change the event entry back button's onclick action
        Button backBtn                                  = menuToEdit.transform.Find(BACK_BTN_NAME).GetComponent<Button>();

        backBtn.onClick.RemoveAllListeners();
        backBtn.onClick.AddListener(()                  => { Series(eventEntry.parentEvent.parentEventSeries); });
    }

    // Final click before actually 'racing'
    private void ChooseCarClicked(EventEntry eventEntry, Car car, bool ownEntryPassForCar){
        // First check if we can actually use our selected car for the event
        if(!ownEntryPassForCar){
            // Alert the user, kind of acts as our tutorial for entry passes lol
            Notification("Alert",
            "You do not own a '" + EventSeriesManager.tierToString[eventEntry.parentEvent.parentEventSeries.seriesTier] +
            "' entry pass for any of:\n\nCar type: " + car.GetPrintType() + "\nCar classes: " + car.GetPrintClasses() +
            "\nCar brand: " + car.brand.ToString());
            return;
        }

        // If we can use the car, make sure this event is the next one up
        if(!eventEntry.nextUp){
            // Alert the user
            Notification("Alert",
            "This event entry is not the next one up in the " + eventEntry.parentEvent.name + " event.\nMake sure to complete the event in sequential order!");
            return;
        }

        // At this point, safe to go race!
        // TODO: Clear the json directory!!!
        Debug.Log("Should clear JSON dir here");
        EventEntryRace(eventEntry, car);
    }

    // Dealer is only needed for a 'buy', so if it is null, this will be treated as a 'selling' transaction
    private void ConfirmPurchasablePopup(Purchasable product, Dealer dealer){
        // Either buying or selling
        bool isBuying               = dealer != null;

        const string BUY_BTN_NAME   = "PurchaseConfirmation/YesBtn";

        BlockNavButtons();

        // Set the onclick event of the buy button on the notification screen
        // Buy/sell the product and clear the notification
        notificationMenu.transform.Find(BUY_BTN_NAME).GetComponent<Button>().onClick.RemoveAllListeners();
        if(isBuying){
            notificationMenu.transform.Find(BUY_BTN_NAME).GetComponent<Button>().onClick.AddListener(() => { gameManager.BuyProduct(product, dealer); });
        }
        else{
            // Selling
            notificationMenu.transform.Find(BUY_BTN_NAME).GetComponent<Button>().onClick.AddListener(() => { gameManager.SellProduct(product, product.sellPrice); });
        }

        string bodyText             = "";
        string remainingBalance     = "";

        if(isBuying){
            remainingBalance        = "$" + (gameManager.curProfile.GetMoney() - product.price).ToString("n0");
            bodyText                = "Are you sure you want to buy a " + product.GetPrintName() + " for " + product.GetPrintPrice() + "?\nBalance after transaction: " + remainingBalance + "\n" + product.GetInfoBlurb();
        }
        else{
            // Selling
            remainingBalance        = "$" + (gameManager.curProfile.GetMoney() + product.sellPrice).ToString("n0");
            bodyText = "Are you sure you want to sell a " + product.GetPrintName() + " for " + product.GetSellPrice() + "?\nBalance after transaction: " + remainingBalance + "\n" + product.GetInfoBlurb();
        }

        Notification("Confirm Transaction", bodyText, product.GetSprite(), NotificationType.PurchaseConfirmation);
    }
}