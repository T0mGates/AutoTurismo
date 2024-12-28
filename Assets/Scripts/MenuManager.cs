using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Globalization;
using System;


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

    public void Dealership(){
        TurnAllOff();
        navigationMenu.SetActive(true);
        dealershipMenu.SetActive(true);
        PopulateDealership();
    }

    public void Dealer(string dealerName){
        TurnAllOff();
        navigationMenu.SetActive(true);
        dealerMenu.SetActive(true);
        PopulateDealer(dealerName);
    }

    public void Garage(){
        TurnAllOff();
        navigationMenu.SetActive(true);
        garageMenu.SetActive(true);
        PopulateGarage();
    }

    public void GotNewCar(Car car){

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

    private void PopulateDealership(){
        const string BG_IMAGE_NAME      = "DealerBtn/BG";
        const string DEALER_TEXT_NAME   = "DealerTxt";
        const string DEALER_BTN_NAME    = "DealerBtn";

        GameObject      newObj;
        List<Dealer>    dealers =  gameManager.curProfile.GetUnlockedDealers();

        for(int i = 0; i < dealers.Count; i++){
            Dealer dealer   = dealers[i];
            newObj          = (GameObject)Instantiate(dealerObjPrefab, dealershipContentTransform);
            newObj.transform.Find(BG_IMAGE_NAME).GetComponent<Image>().sprite                   = dealer.GetSprite();
            newObj.transform.Find(DEALER_TEXT_NAME).GetComponent<TextMeshProUGUI>().text        = dealer.name;

            newObj.transform.Find(DEALER_BTN_NAME).GetComponent<Button>().onClick.RemoveAllListeners();
            newObj.transform.Find(DEALER_BTN_NAME).GetComponent<Button>().onClick.AddListener(() => { Dealer(dealer.name); });
        }
    }

    private void PopulateDealer(string dealerName){
        const string CAR_IMAGE_NAME     = "CarImg";
        const string CAR_TEXT_NAME      = "CarNameTxt";
        const string CAR_CLASSES_NAME   = "CarClassesTxt";
        const string CAR_TYPE_NAME      = "CarTypeTxt";
        const string BUY_BTN_NAME       = "PriceBuy/BuyBtn";
        const string BUY_BTN_TEXT_NAME  = "PriceBuy/BuyBtn/BuyTxt";
        const string PRICE_TEXT_NAME    = "PriceBuy/PriceTxt";

        const string DEALER_TEXT_NAME   = "DealerTxt";

        GameObject  newObj;
        Dealer      dealer              = Dealers.GetDealer(dealerName);
        List<Car>   cars                = dealer.cars;

        // Set dealer title
        dealerMenu.transform.Find(DEALER_TEXT_NAME).GetComponent<TextMeshProUGUI>().text        = dealer.name;

        for(int i = 0; i < cars.Count; i++){
            Car car         = cars[i];
            newObj                                                                              = (GameObject)Instantiate(dealerCarPrefab, dealerContentTransform);

            // Set BG to green for now
            newObj.GetComponent<Image>().color                                                  = transparentGreen;

            // Change the image
            newObj.transform.Find(CAR_IMAGE_NAME).GetComponent<Image>().sprite                  = car.GetSprite();

            // Change the car name
            newObj.transform.Find(CAR_TEXT_NAME).GetComponent<TextMeshProUGUI>().text           = car.GetPrintName();

            // Change the car class(es)

            newObj.transform.Find(CAR_CLASSES_NAME).GetComponent<TextMeshProUGUI>().text        = car.GetPrintClasses();

            // Change the car type
            newObj.transform.Find(CAR_TYPE_NAME).GetComponent<TextMeshProUGUI>().text           = car.GetPrintType();

            // Change the buy button onclick action
            // If we own the car, disable the button
            Button buyBtn = newObj.transform.Find(BUY_BTN_NAME).GetComponent<Button>();
            if(gameManager.curProfile.OwnsCar(car)){
                buyBtn.interactable = false;
                newObj.transform.Find(BUY_BTN_TEXT_NAME).GetComponent<TextMeshProUGUI>().text   = "Owned";
                newObj.GetComponent<Image>().color                                              = transparentBlue;
            }
            // If we don't own the car and can't afford it, change BG to red
            else if(!gameManager.CanAfford(car, dealer)){
                newObj.GetComponent<Image>().color                                              = transparentRed;
            }

            buyBtn.onClick.RemoveAllListeners();
            buyBtn.onClick.AddListener(() => { ConfirmPurchasablePopup(car, dealer); });

            // Change the price (text)
            newObj.transform.Find(PRICE_TEXT_NAME).GetComponent<TextMeshProUGUI>().text         = car.GetPrintPrice();
        }
    }

    // Dealer is only needed for a 'buy', so if it is null, this will be treated as a 'selling' transaction
    private void ConfirmPurchasablePopup(Purchasable purchasable, Dealer dealer){
        // Either buying or selling
        bool isBuying               = dealer != null;

        const string BUY_BTN_NAME   = "PurchaseConfirmation/YesBtn";
        string bodyTextAddon        = "";

        BlockButtons();

        // Set the onclick event of the buy button on the notification screen
        if(purchasable is Car){
            Car car = (Car) purchasable;

            // Buy/sell the car and clear the notification
            notificationMenu.transform.Find(BUY_BTN_NAME).GetComponent<Button>().onClick.RemoveAllListeners();
            if(isBuying){
                notificationMenu.transform.Find(BUY_BTN_NAME).GetComponent<Button>().onClick.AddListener(() => { gameManager.BuyCar(car, dealer); });
            }
            else{
                // Selling
                notificationMenu.transform.Find(BUY_BTN_NAME).GetComponent<Button>().onClick.AddListener(() => { gameManager.SellCar(car, car.sellPrice); });
            }

            bodyTextAddon += "\n\nMake/Model: " + car.GetPrintName() + "\nType: " + car.GetPrintType() + "\nClass: " + car.GetPrintClasses();
        }

        string bodyText             = "";
        string remainingBalance     = "";

        if(isBuying){
            remainingBalance        = "$" + (gameManager.curProfile.GetMoney() - purchasable.price).ToString("n0");
            bodyText                = "Are you sure you want to buy a " + purchasable.name + " for " + purchasable.GetPrintPrice() + "?\n\nBalance after transaction: " + remainingBalance + bodyTextAddon;
        }
        else{
            // Selling
            remainingBalance        = "$" + (gameManager.curProfile.GetMoney() + purchasable.sellPrice).ToString("n0");
            bodyText = "Are you sure you want to sell a " + purchasable.name + " for " + purchasable.GetSellPrice() + "?\n\nBalance after transaction: " + remainingBalance + bodyTextAddon;
        }

        Notification("Confirm Transaction", bodyText, purchasable.GetSprite(), true);
    }

    private void PopulateGarage(){
        const string CAR_IMAGE_NAME     = "CarImg";
        const string CAR_TEXT_NAME      = "CarNameTxt";
        const string CAR_CLASSES_NAME   = "CarClassesTxt";
        const string CAR_TYPE_NAME      = "CarTypeTxt";
        const string SELL_BTN_NAME      = "PriceSell/SellBtn";
        const string PRICE_TEXT_NAME    = "PriceSell/PriceTxt";

        GameObject  newObj;
        List<Car>   cars                = gameManager.curProfile.GetOwnedCars();

        for(int i = 0; i < cars.Count; i++){
            Car car         = cars[i];

            newObj                                                                              = (GameObject)Instantiate(garageCarPrefab, garageContentTransform);

            // Set BG to blue (owned)
            newObj.GetComponent<Image>().color                                                  = transparentBlue;

            // Change the image
            newObj.transform.Find(CAR_IMAGE_NAME).GetComponent<Image>().sprite                  = car.GetSprite();

            // Change the car name
            newObj.transform.Find(CAR_TEXT_NAME).GetComponent<TextMeshProUGUI>().text           = car.GetPrintName();

            // Change the car class(es)
            newObj.transform.Find(CAR_CLASSES_NAME).GetComponent<TextMeshProUGUI>().text        = car.GetPrintClasses();

            // Change the car type
            newObj.transform.Find(CAR_TYPE_NAME).GetComponent<TextMeshProUGUI>().text           = car.GetPrintType();

            // Change the sell button onclick action
            // If we only own one car, disable the button
            Button sellBtn = newObj.transform.Find(SELL_BTN_NAME).GetComponent<Button>();
            if(1 == cars.Count){
                sellBtn.interactable = false;
                newObj.GetComponent<Image>().color                                              = transparentRed;
            }

            sellBtn.onClick.RemoveAllListeners();
            sellBtn.onClick.AddListener(() => { ConfirmPurchasablePopup(car, null); });

            // Change the price (text)
            newObj.transform.Find(PRICE_TEXT_NAME).GetComponent<TextMeshProUGUI>().text         = car.GetSellPrice();
        }
    }
}