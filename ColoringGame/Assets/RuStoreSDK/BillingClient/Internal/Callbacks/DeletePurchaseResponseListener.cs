using RuStore.Internal;
using System;
using UnityEngine;

namespace RuStore.BillingClient.Internal {

    public class DeletePurchaseResponseListener : ResponseListener<DeletePurchaseResponse> {

        private const string javaClassName = "ru.rustore.unitysdk.billingclient.callbacks.DeletePurchaseListener";

        public DeletePurchaseResponseListener(Action<RuStoreError> onFailure, Action<DeletePurchaseResponse> onSuccess) : base(javaClassName, onFailure, onSuccess) {
        }

        protected override DeletePurchaseResponse ConvertResponse(AndroidJavaObject responseObject) {
            var response = new DeletePurchaseResponse();
            DataConverter.InitResponseWithCode(responseObject, response);

            return response;
        }
    }
}
