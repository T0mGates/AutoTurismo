using UnityEngine;
using System;
using System.Collections.Generic;

public abstract class Purchasable{
    // DEVS: change the contents of this list to add new types of purchasable items
    public static   List<Type>        productTypes   = new List<Type>() { typeof(Car), typeof(EntryPass) } ;

    public int      price;
    public int      sellPrice;
    public string   name;

    public Purchasable(string nameParam, int priceParam, int sellPriceParam = -1){
        price       = priceParam;
        sellPrice   = sellPriceParam;
        name        = nameParam;

        if(-1 == sellPrice){
            // Default is price divided by 2
            // Maybe will want this to be tweaked later - maybe have some sort of coupons and whatnot as level up rewards
            sellPrice = price/2;
        }
    }

    public abstract Sprite GetSprite();

    public abstract string GetPrintName();
    public abstract string GetInfoBlurb();

    public string GetPrintPrice(){
        return "$" + price.ToString("n0");
    }

    public string GetSellPrice(){
        return "$" + sellPrice.ToString("n0");
    }
}