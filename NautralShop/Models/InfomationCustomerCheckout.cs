namespace NaturalShop.Models
{
    public class InfomationCustomerCheckout
    {
        public string FName {  get; set; } = string.Empty;
        public string LName {  get; set; } = string.Empty;
        public string Phone {  get; set; } = string.Empty;
        public string Email {  get; set; } = string.Empty;
        public string AddressStreet { get; set; } = string.Empty;
        public string AddressWard { get; set; } = string.Empty;
        public string AddressDistrict {  get; set; } = string.Empty;
        public string AddressCity { get; set; } = string.Empty;
        public int PaymentMethod { get; set; }
        public string? CustomerId { get; set; }
    }
}
