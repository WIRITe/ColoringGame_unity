using RuStore.Internal;
using System;
using UnityEngine;

namespace RuStore.BillingClient.Internal {

    public class PaymentResultListener : ResponseListener<PaymentResult> {

        private const string javaClassName = "ru.rustore.unitysdk.billingclient.callbacks.PaymentResultListener";

        public PaymentResultListener(Action<RuStoreError> onFailure, Action<PaymentResult> onSuccess) : base(javaClassName, onFailure, onSuccess) {
        }

        protected override PaymentResult ConvertResponse(AndroidJavaObject responseObject) {
            var resultType = "";
            if (responseObject != null) {
                using (var javaClass = responseObject.Call<AndroidJavaObject>("getClass")) {
                    var className = javaClass.Call<string>("getName").Split('$');
                    resultType = className[className.Length - 1];
                }
            }

            switch (resultType) {
                case "InvoiceResult":
                    return new InvoiceResult() {
                        invoiceId = responseObject.Get<string>("invoiceId"),
                        finishCode = (PaymentResult.PaymentFinishCode)Enum.Parse(typeof(PaymentResult.PaymentFinishCode), responseObject.Get<AndroidJavaObject>("finishCode").Call<string>("toString"), true)
                    };
                case "InvalidInvoice":
                    return new InvalidInvoice() {
                        invoiceId = responseObject.Get<string>("invoiceId")
                    };
                case "PurchaseResult":
                    return new PurchaseResult() {
                        finishCode = (PaymentResult.PaymentFinishCode)Enum.Parse(typeof(PaymentResult.PaymentFinishCode), responseObject.Get<AndroidJavaObject>("finishCode").Call<string>("toString"), true),
                        orderId = responseObject.Get<string>("orderId"),
                        purchaseId = responseObject.Get<string>("purchaseId"),
                        productId = responseObject.Get<string>("productId"),
                        invoiceId = responseObject.Get<string>("invoiceId"),
                        subscriptionToken = responseObject.Get<string>("subscriptionToken")
                    };
                case "InvalidPurchase":
                    return new InvalidPurchase() {
                        purchaseId = responseObject.Get<string>("purchaseId"),
                        invoiceId = responseObject.Get<string>("invoiceId"),
                        orderId = responseObject.Get<string>("orderId"),
                        quantity = responseObject.Get<AndroidJavaObject>("quantity")?.Call<int>("intValue") ?? 0,
                        productId = responseObject.Get<string>("productId"),
                        errorCode = responseObject.Get<AndroidJavaObject>("errorCode")?.Call<int>("intValue") ?? 0
                    };
                default:
                    return new InvalidPaymentState();
            }
        }
    }
}
