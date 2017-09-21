using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Events;
using System;

public class IAPController : MonoBehaviour, IStoreListener {
	public delegate void OnPurchaseSuccess(PurchaseEventArgs arg, int index);
	public event OnPurchaseSuccess onPurchaseSuccess;
	public delegate void OnPurchaseFail(Product product, PurchaseFailureReason failureReason);
	public event OnPurchaseFail onPurchaseFail;
	public delegate void OnPurchasesInit();
	public event OnPurchasesInit onPurchasesInit;

	private IStoreController controller;
	private IExtensionProvider extensions;

	public string[] ProductIds;

	void Start() {
		Init();
	}

	public void Init() {
		if(IsInitialized()) {
			return;
		}

		var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
		foreach(string item in ProductIds) {
			builder.AddProduct(item, ProductType.Consumable);
		}

		UnityPurchasing.Initialize (this, builder);
	}

	void InitForce() {
		controller = null;
		extensions = null;
		Init();
	}

	private bool IsInitialized(){
		return controller != null && extensions != null;
	}

	public void OnInitialized (IStoreController controller, IExtensionProvider extensions) {
		this.controller = controller;
		this.extensions = extensions;

		if(onPurchasesInit != null) {
			onPurchasesInit();
		}
	}
		
	public void OnInitializeFailed (InitializationFailureReason error) {
		Debug.LogError("OnInitializeFailed InitializationFailureReason:" + error);
	}

	public void BuyProductID(int index) {
		Preconditions.Check(index < ProductIds.Length, "Invalid index {0} for product", index);
		BuyProductID(ProductIds[index]);
	}

	public bool BuyProductID(string productId) {
		// If Purchasing has been initialized ...
		if (IsInitialized()) {
			Product product = controller.products.WithID(productId);

			if (product != null && product.availableToPurchase){
				Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
				controller.InitiatePurchase(product);
				return true;
			} else {
				Debug.LogError("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
				OnError("Продукт не доступен");
			}
		} else {
			Debug.LogError("BuyProductID FAIL. Not initialized.");
			OnError(null);
		}

		return false;
	}


	// Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google. 
	// Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
	public void RestorePurchases()
	{
		// If Purchasing has not yet been set up ...
		if (!IsInitialized())
		{
			// ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
			Debug.Log("RestorePurchases FAIL. Not initialized.");
			return;
		}

		// If we are running on an Apple device ... 
		if (Application.platform == RuntimePlatform.IPhonePlayer || 
			Application.platform == RuntimePlatform.OSXPlayer)
		{
			// ... begin restoring purchases
			Debug.Log("RestorePurchases started ...");

			// Fetch the Apple store-specific subsystem.
			var apple = extensions.GetExtension<IAppleExtensions>();
			// Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
			// the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
			apple.RestoreTransactions((result) => {
				// The first phase of restoration. If no more responses are received on ProcessPurchase then 
				// no purchases are available to be restored.
				Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
			});
		}
		// Otherwise ...
		else
		{
			// We are not running on an Apple device. No work is necessary to restore purchases.
			Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
		}
	}

	void OnError(string error, UnityAction yesEvent = null) {
		if(yesEvent == null) {
			yesEvent = InitForce;
		}

		string text = string.Format("Магазин не доступен. {0} Проверьте соединение с сетью", ((error == null)? "" : error + "."));
		ModalPanels.Show(ModalPanelName.ErrorPanel, text, yesEvent);
	}

	void OnCancel() {
		ModalPanels.Show(ModalPanelName.ErrorPanel, "Покупка отменена");
	}

	public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args) {
		int index = Array.IndexOf(ProductIds, args.purchasedProduct.definition.id);
		if (index >= 0) {
			try {
				if(onPurchaseSuccess != null) {
					onPurchaseSuccess(args, index);
				}
			} catch(Exception e) {
				Debug.LogError(e);
				ModalPanels.Show(ModalPanelName.ErrorPanel, e.ToString());
			}
		} else {
			Debug.LogError(string.Format("ProcessPurchase: FAIL. Unrecognized product: '{0}'", args.purchasedProduct.definition.id));
		}

		return PurchaseProcessingResult.Complete;
	}


	public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason) {
		Debug.LogError(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));

		if(failureReason == PurchaseFailureReason.UserCancelled) {
			OnCancel();
		} else {
			OnError(failureReason.ToString());
		}

		if(onPurchaseFail != null) {
			onPurchaseFail(product, failureReason);
		}
	}

	public Product GetProduct(string id) {
		if(!IsInitialized()) {
			return null;
		}

		foreach (Product product in controller.products.all) {
			if(product.definition.id == id) {
				return product;
			}
		}

		return null;
	}
}
