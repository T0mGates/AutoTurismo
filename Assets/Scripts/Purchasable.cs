using UnityEngine;
using System;
using System.Collections.Generic;

[System.Serializable]
public abstract class Purchasable : UnityEngine.Object
{
    // DEVS: change the contents of this list to add new types of purchasable items
    public static   SerializableList<SerializableType>  productTypes                = new SerializableList<SerializableType>() { new SerializableType(typeof(Car)), new SerializableType(typeof(EntryPass)) } ;

    public int                                          price;
    public int                                          sellPrice;
    public string                                       productName;

    public Purchasable(string nameParam, int priceParam, int sellPriceParam = -1){
        price       = priceParam;
        sellPrice   = sellPriceParam;
        productName = nameParam;

        if(-1 == sellPrice){
            // Default is price divided by 2
            // Maybe will want this to be tweaked later - maybe have some sort of coupons and whatnot as level up rewards
            sellPrice = price/2;
        }
    }

    // Needed to get around serialization
    public static Dictionary<Type, SerializableType> GetTypeToSerTypeDict(){
        return new Dictionary<Type, SerializableType>() { {typeof(Car), productTypes[0]}, {typeof(EntryPass), productTypes[1]} };
    }

    public abstract Sprite GetSprite();

    public abstract string GetPrintName();
    public abstract string GetInfoBlurb();

    public virtual Sprite GetBGSprite(){
        return null;
    }

    public string GetPrintPrice(){
        return "$" + price.ToString("n0");
    }

    public string GetSellPrice(){
        return "$" + sellPrice.ToString("n0");
    }
}