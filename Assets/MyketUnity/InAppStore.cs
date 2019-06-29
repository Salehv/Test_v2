using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(StoreHandler))]
public class InAppStore : MonoBehaviour, IBillingListener
{
    public Product[] products;

    void Awake()
    {
        GetComponent<StoreHandler>().SetUpBillingService(this);
    }

    void Start()
    {
    }


    public void OnBillingServiceSetupFinished()
    {
        GetComponent<StoreHandler>().UpdatePurchasesAndDetails(products);
        print("[IABHandler] Setup Finished");
    }


    public void OnPurchasesUpdated(List<Purchase> purchases)
    {
        purchases.ForEach(OnPurchaseFinished);
    }

    public void OnPurchasesAndDetailsUpdated(List<Purchase> purchases, List<ProductDetail> productDetails)
    {
        purchases.ForEach(OnPurchaseFinished);
    }

    public void OnUserCancelPurchase(string errorMessage)
    {
    }

    public void OnPurchaseFinished(Purchase purchase)
    {
        GetComponent<StoreHandler>().ConsumePurchase(purchase.productId, purchase.purchaseToken);
    }

    public void OnConsumeFinished(String productId, String purchaseToken)
    {
        IABHandler.instance.OnConsumeFinished(productId, purchaseToken);
    }

    public void OnError(Int32 errorCode, String errorMessage)
    {
        print("[RRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRR]");
        switch (errorCode)
        {
            case 1: // error illegal State
                break;

            case 2: // error billing setup fail
                break;

            case 3: // error empty public key
                break;

            case 4: // error update purchases

                break;
            case 5: // error consume

                break;
            case 6: // error start activity

                break;
            case 7: // error purchase

                break;
        }
    }


    public void purchaseProduct(int productIndex)
    {
        Product product = products[productIndex];
        if (product.type == Product.ProductType.Subscription)
        {
            GetComponent<StoreHandler>().BuyProduct(product.productId, "subs");
        }
        else if (product.type == Product.ProductType.InApp)
        {
            GetComponent<StoreHandler>().BuyProduct(product.productId, "inapp");
        }
    }
}