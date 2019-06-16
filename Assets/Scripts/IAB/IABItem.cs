using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[System.Serializable]
public class IABItem : MonoBehaviour
{
    public string productId;

    public int amount;

    public int price;

    public Text priceText;

    public IABItemType type;
}

public enum IABItemType
{
    PACK_OF_COIN,
    BAG_OF_COIN,
    BOX_OF_COIN,
    CHEST_OF_COIN
}