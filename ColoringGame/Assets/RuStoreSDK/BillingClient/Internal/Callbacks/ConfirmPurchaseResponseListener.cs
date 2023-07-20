using RuStore.Internal;
using System;
using UnityEngine;

namespace RuStore.BillingClient.Internal {

    public class ConfirmPurchaseResponseListener : ResponseListener<ConfirmPurchaseResponse> {

        private const string javaClassName = "ru.rustore.unitysdk.billingclient.callbacks.ConfirmPurchaseListener";

        public ConfirmPurchaseResponseListener(Action<RuStoreError> onFailure, Action<ConfirmPurchaseResponse> onSuccess) : base(javaClassName, onFailure, onSuccess) {
        }

        protected override ConfirmPurchaseResponse ConvertResponse(AndroidJavaObject responseObject) {
            var response = new ConfirmPurchaseResponse();
            DataConverter.InitResponseWithCode(responseObject, response);

            return response;
        }
    }
}
