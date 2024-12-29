using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Globalization;
using System;
using Unity.VisualScripting;
using UnityEngine.TextCore.Text;


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
    public GameObject   countrySeriesMenu;

    [Header("Player Info")]
    public Slider       expSlider;
    public TMP_Text     moneyText;
    public TMP_Text     expText;
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

    [Header("Country")]
    public GameObject   countrySeriesPrefab;
    public Transform    countrySeriesContentTransform;

    void Start(){
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();

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
        countrySeriesMenu.SetActive(false);

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
        foreach(Transform child in countrySeriesContentTransform)
        {
            Destroy(child.gameObject);
        }
    }

    public void BlockButtons(){
        // Called to disable buttons in the nav bar
        foreach(Button btn in navigationMenu.GetComponentsInChildren<Button>()){
            btn.interactable = false;
        }
    }

    public void Home(bool newPlayer = false){
        introMenu.SetActive(false);
        mainMenu.SetActive(true);
        navigationMenu.SetActive(true);
        newPlayerMenu.SetActive(newPlayer);

        if(newPlayer){
            BlockButtons();
        }
        else{
            // Re-activate buttons
            ClearNotification();
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
        expSlider.value = 0 == gameManager.curProfile.GetCurrentExperience() ? 0 : (float)gameManager.curProfile.GetCurrentExperience() / (float)gameManager.curProfile.GetMaxExperience();
        // Format as a number with 0 decimals
        moneyText.text  = gameManager.curProfile.GetMoney().ToString("n0");
        expText.text    = gameManager.curProfile.GetCurrentExperience().ToString() + " / " + gameManager.curProfile.GetMaxExperience().ToString();
        levelText.text  = "Level: " + gameManager.curProfile.GetLevel().ToString();
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

    public void CountrySeries(string countryName){
        foreach(Tracks.Country country in Enum.GetValues(typeof(Tracks.Country))){
            // Look if the string representation of the enum is the given country name
            if(country.ToString() == countryName.Replace(" ", "")){
                TurnAllOff();
                navigationMenu.SetActive(true);
                countrySeriesMenu.SetActive(true);
                PopulateCountrySeries(country);
            }
        }
    }

    public void Series(EventSeries series){

    }

    public void Notification(string title, string bodyText, Sprite rewardSprite, bool isAConfirmation = false){
        const string TITLE_TEXT_NAME        = "TitleTxt";
        const string BODY_TEXT_NAME         = "NotificationTxt";
        const string REWARD_IMG_NAME        = "RewardImg";

        const string NORMAL_NOTI_NAME       = "NormalNotification";
        const string CONFIRMATION_NAME      = "PurchaseConfirmation";

        // Cover the background with the menu
        mainMenu.SetActive(true);

        // Block navigation buttons
        BlockButtons();

        notificationMenu.transform.Find(TITLE_TEXT_NAME).GetComponent<TextMeshProUGUI>().text   = title;
        notificationMenu.transform.Find(BODY_TEXT_NAME).GetComponent<TextMeshProUGUI>().text    = bodyText;
        notificationMenu.transform.Find(REWARD_IMG_NAME).GetComponent<Image>().sprite           = rewardSprite;

        notificationMenu.transform.Find(NORMAL_NOTI_NAME).gameObject.SetActive(!isAConfirmation);
        notificationMenu.transform.Find(CONFIRMATION_NAME).gameObject.SetActive(isAConfirmation);

        notificationMenu.SetActive(true);
    }

    public void ClearNotification(){
        notificationMenu.SetActive(false);
        mainMenu.SetActive(false);

        // Reactivate the buttons
        foreach(Button btn in navigationMenu.GetComponentsInChildren<Button>()){
            btn.interactable = true;
        }
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
        //const string        BACK_BTN_NAME      = "BackBtn";

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

    private void PopulateCountrySeries(Tracks.Country country){
        const string        COUNTRY_TEXT_NAME   = "CountrySeriesTxt";

        // These are for the prefab
        const string        SERIES_TITLE_NAME   = "SeriesNameTxt";
        const string        SERIES_TIER_NAME    = "SeriesTierTxt";
        const string        VIEW_BTN_NAME       = "ViewBtn";

        // These will be put before and after the series tier text
        const string        SERIES_TIER_PREFIX  = "Only for ";
        const string        SERIES_TIER_SUFFIX  = " Drivers";
        // Don't forget image later on

        GameObject          newObj;
        List<EventSeries>   seriesList          = EventSeriesManager.GetCountrySeries(country);

        // Change the title depending on what the country type is
        countrySeriesMenu.transform.Find(COUNTRY_TEXT_NAME).GetComponent<TextMeshProUGUI>().text     = Tracks.countryToString[country];

        foreach(EventSeries series in seriesList){

            newObj                                                                              = (GameObject)Instantiate(countrySeriesPrefab, countrySeriesContentTransform);

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

    // Dealer is only needed for a 'buy', so if it is null, this will be treated as a 'selling' transaction
    private void ConfirmPurchasablePopup(Purchasable product, Dealer dealer){
        // Either buying or selling
        bool isBuying               = dealer != null;

        const string BUY_BTN_NAME   = "PurchaseConfirmation/YesBtn";

        BlockButtons();

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

        Notification("Confirm Transaction", bodyText, product.GetSprite(), true);
    }
}