using RuStore.Internal;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RuStore.BillingClient.Internal {

    public class PurchaseInfoResponseListener : ResponseListener<PurchaseInfoResponse> {

        private const string javaClassName = "ru.rustore.unitysdk.billingclient.callbacks.PurchaseInfoResponseListener";

        public PurchaseInfoResponseListener(Action<RuStoreError> onFailure, Action<PurchaseInfoResponse> onSuccess) : base(javaClassName, onFailure, onSuccess) {
        }

        protected override PurchaseInfoResponse ConvertResponse(AndroidJavaObject responseObject)  {
            var response = new PurchaseInfoResponse();

            DataConverter.InitResponseWithCode(responseObject, response);

            using (var purchase = responseObject.Get<AndroidJavaObject>("purchase")) {
                if (purchase != null) {
                    response.purchase = DataConverter.ConvertPurchase(purchase);
                }
            }

            return response;
        }
    }
}
