using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TheGame;
using UnityEngine;
using UnityEngine.UI;

public class IABHandler : MonoBehaviour
{
    [SerializeField] [Header("Shop items")]
    public bool debugMode;

    public IABItem[] items;

    [Header("UI Objects")] public GameObject shopLock;
    // public GameObject shopLoading;

    // public Text debug;

    private Dictionary<string, IABItem> itemMap;

    private void Start()
    {
        itemMap = new Dictionary<string, IABItem>();

        foreach (var item in items)
        {
            itemMap.Add(item.productId, item);
        }

        InitCafeBazaar();
    }


    public void InitCafeBazaar()
    {
        InitEvents();

        /*BazaarIAB.init(
            "MIHNMA0GCSqGSIb3DQEBAQUAA4G7ADCBtwKBrwCooaFh1ZJhgj1fHE6inFBGTKXIzeWYC7d6gdonZ1AxdAWZykxjbhIN1YbNpEc22hwz/A" +
            "bQdxhMXO8jLRnUdbuCp/9k0BYQuPVkmCa0wo3jdthokV2jClTMrDhAj3rNBFQ+s49K0h2SN/iH7+AcTszoqSMGtsG5Hhm0eZYyWBHf6IUOU5E0Tex+ku" +
            "9BaSf5LdEYh/QTXznV0GOhJ0MEzaLWRCPnZmzyaCvU7qeHk8ECAwEAAQ==");*/
    }

    private void InitEvents()
    {
        /*IABEventManager.billingSupportedEvent += BillingSupported;
        IABEventManager.billingNotSupportedEvent += BillingNotSupported;
        IABEventManager.querySkuDetailsSucceededEvent += SkuDetailsQuerySucceed;
        IABEventManager.querySkuDetailsFailedEvent += SkuDetailQueryFailed;
        IABEventManager.purchaseSucceededEvent += PurchaseSucceed;
        IABEventManager.consumePurchaseSucceededEvent += consumeSucceed;*/
    }


    private void BillingSupported()
    {
        //shopLoading.SetActive(true);
        Debug.Log("IAB Supported");
    }

    private void BillingNotSupported(string error)
    {
        Debug.LogError(error);
        shopLock.SetActive(true);
    }

    private void UpdateShopItems()
    {
        /*string[] details = itemMap.Keys.ToArray();
        BazaarIAB.querySkuDetails(details);*/
    }


    /*private void SkuDetailsQuerySucceed(List<BazaarSkuInfo> skus)
    {
        foreach (BazaarSkuInfo sku in skus)
        {
            itemMap[sku.ProductId].priceText.text = sku.Price;
        }
    }*/

    private void SkuDetailQueryFailed(string error)
    {
        PopupHandler.ShowInfo("فکر کنم اینترنتت وصل نیست . . .", RetryConnection);
    }

    public void RetryConnection()
    {
        UpdateShopItems();
    }


/*
    private void PurchaseSucceed(BazaarPurchase purchase)
    {
        BazaarIAB.consumeProduct(purchase.ProductId);
    }
*/

/*
    private void consumeSucceed(BazaarPurchase purchase)
    {
        PopupHandler.ShowDebug("خرید موفقیت آمیز بود");
        GameManager.instance.AddCoins(itemMap[purchase.ProductId].amount);
        AnalyticsHandler.ItemBought(itemMap[purchase.ProductId].type, itemMap[purchase.ProductId].price,
            itemMap[purchase.ProductId].amount);
    }
*/


    public void PurchaseItem(string id)
    {
        if (debugMode)
            print($"[IABHandler] Purchase item {id}");
/*
        BazaarIAB.purchaseProduct(id);
*/
    }
}


enum IABState
{
    q
}