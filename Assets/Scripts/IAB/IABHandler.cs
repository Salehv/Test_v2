using System.Collections.Generic;
using TheGame;
using UnityEngine;

public class IABHandler : MonoBehaviour, IBillingListener
{
    [SerializeField] [Header("Shop items")]
    public bool debugMode;

    public IABItem[] items;

    [Header("UI Objects")] public GameObject shopLock;

    public StoreHandler storeHandler;

    public InAppStore inAppStore;
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

        InitMyket();
    }

    private void InitMyket()
    {
        storeHandler.SetUpBillingService(this);
        storeHandler.UpdatePurchases();
    }


    public void PurchaseItem(string id)
    {
        if (debugMode)
            print($"[IABHandler] Purchase item {id}");

        inAppStore.purchaseProduct(int.Parse(id.Split('_')[1]));
    }

    public void OnBillingServiceSetupFinished()
    {
        print("[IABHandler] Setup Finished");
    }

    public void OnPurchasesUpdated(List<Purchase> purchases)
    {
        purchases.ForEach(OnPurchaseFinished);
    }

    public void OnPurchasesAndDetailsUpdated(List<Purchase> purchases, List<ProductDetail> products)
    {
    }

    public void OnUserCancelPurchase(string errorMessage)
    {
    }

    public void OnPurchaseFinished(Purchase purchase)
    {
        storeHandler.ConsumePurchase(purchase.productId, purchase.purchaseToken);
    }

    public void OnConsumeFinished(string productId, string purchaseToken)
    {
        PopupHandler.ShowDebug("خرید موفقیت آمیز بود");
        GameManager.instance.AddCoins(itemMap[productId].amount);
        AnalyticsHandler.ItemBought(itemMap[productId].type, itemMap[productId].price,
            itemMap[productId].amount);
    }

    public void OnError(int errorCode, string errorMessage)
    {
        Debug.LogError("IAB ERROR");
    }
}