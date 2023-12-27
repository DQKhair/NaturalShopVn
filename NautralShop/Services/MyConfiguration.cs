namespace FlowerStore.MyConfiguration
{
    public class MyConfiguration
    {
        private string vnp_Url = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
        private string vnp_Api = "https://sandbox.vnpayment.vn/merchant_webapi/api/transaction";
        private string vnp_TmnCode = "QFAQZF15";
        private string vnp_HashSecret = "TXZIBSRJGQJWVVHIIIEACXHJJNLDYJBR";
        private string vnp_Returnurl = "https://localhost:7234/Checkout/CheckoutOrderSuccess";
        public MyConfiguration() { }

        public string Vnp_Url { get => vnp_Url; set => vnp_Url = value; }
        public string Vnp_Api { get => vnp_Api; set => vnp_Api = value; }
        public string Vnp_TmnCode { get => vnp_TmnCode; set => vnp_TmnCode = value; }
        public string Vnp_HashSecret { get => vnp_HashSecret; set => vnp_HashSecret = value; }
        public string Vnp_Returnurl { get => vnp_Returnurl; set => vnp_Returnurl = value; }
    }
}
