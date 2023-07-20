using RuStore.Internal;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RuStore.BillingClient.Internal {

    public class ProductsResponseListener : ResponseListener<ProductsResponse> {

        private const string javaClassName = "ru.rustore.unitysdk.billingclient.callbacks.ProductsResponseListener";

        public ProductsResponseListener(Action<RuStoreError> onFailure, Action<ProductsResponse> onSuccess) : base(javaClassName, onFailure, onSuccess) {
        }

        protected override ProductsResponse ConvertResponse(AndroidJavaObject responseObject) {
            var response = new ProductsResponse() {
                products = new List<Product>()
            };

            DataConverter.InitResponseWithCode(responseObject, response);

            using (var products = responseObject.Get<AndroidJavaObject>("products"))
            {
                if (products != null)
                {
                    var size = products.Call<int>("size");
                    for (var i = 0; i < size; i++)
                    {
                        using (var p = products.Call<AndroidJavaObject>("get", i))
                        {
                            if (p != null)
                            {
                                response.products.Add(DataConverter.ConvertProduct(p));
                            }
                        }
                    }
                }
            }

            return response;
        }
    }
}
