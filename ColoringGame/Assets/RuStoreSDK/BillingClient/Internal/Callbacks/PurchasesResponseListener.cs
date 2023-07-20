using RuStore.Internal;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RuStore.BillingClient.Internal {

    public class PurchasesResponseListener : ResponseListener<PurchasesResponse> {

        private const string javaClassName = "ru.rustore.unitysdk.billingclient.callbacks.PurchasesResponseListener";

        public PurchasesResponseListener(Action<RuStoreError> onFailure, Action<PurchasesResponse> onSuccess) : base(javaClassName, onFailure, onSuccess) {
        }

        protected override PurchasesResponse ConvertResponse(AndroidJavaObject responseObject)  {
            var response = new PurchasesResponse() {
                purchases = new List<Purchase>()
            };

            DataConverter.InitResponseWithCode(responseObject, response);

            using (var purchases = responseObject.Get<AndroidJavaObject>("purchases")) {
                if (purchases != null) {
                    var size = purchases.Call<int>("size");
                    for (var i = 0; i < size; i++) {
                        using (var p = purchases.Call<AndroidJavaObject>("get", i)) {
                            if (p != null) {
                                response.purchases.Add(DataConverter.ConvertPurchase(p));
                            }
                        }
                    }
                }
            }

            return response;
        }
    }
}
