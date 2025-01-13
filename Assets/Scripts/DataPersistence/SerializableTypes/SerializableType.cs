using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableType
{
    [SerializeField] private string typeName;

    public SerializableType(Type type)
    {
        typeName = type.AssemblyQualifiedName;
    }

    public Type GetTypeFromWrapper()
    {
        return Type.GetType(typeName);
    }

    public string GetAssemblyQualifiedName(){
        return typeName;
    }

    public static SerializableType GetSerializableTypeFromDealerType(Type type){
        return Dealers.dealerTypeToSerialized[type];
    }

    public static SerializableType GetSerializableTypeFromPurchasableType(Type type){
        return Purchasable.GetTypeToSerTypeDict()[type];
    }

    public override int GetHashCode()
    {
        return typeName.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        return GetHashCode() == obj.GetHashCode();
    }
}
