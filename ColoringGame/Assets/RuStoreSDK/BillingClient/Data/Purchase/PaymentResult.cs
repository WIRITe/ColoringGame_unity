namespace RuStore.BillingClient {

    public class PaymentResult {
        public enum PaymentFinishCode {

            SUCCESSFUL_PAYMENT,
            CLOSED_BY_USER,
            UNHANDLED_FORM_ERROR,
            PAYMENT_TIMEOUT,
            DECLINED_BY_SERVER,
            RESULT_UNKNOWN
        }
    }

    public class InvoiceResult : PaymentResult {

        public string invoiceId;
        public PaymentFinishCode finishCode;
    }

    public class InvalidInvoice : PaymentResult {

        public string invoiceId;
    }

    public class PurchaseResult : PaymentResult {

        public PaymentFinishCode finishCode;
        public string orderId;
        public string purchaseId;
        public string productId;
        public string invoiceId;
        public string subscriptionToken;
    }

    public class InvalidPurchase : PaymentResult {

        public string purchaseId;
        public string invoiceId;
        public string orderId;
        public int quantity;
        public string productId;
        public int errorCode;
    }

    public class InvalidPaymentState : PaymentResult {
    }
}
