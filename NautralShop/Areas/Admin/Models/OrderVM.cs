namespace NautralShop.Areas.Admin.Models
{
    public class OrderVM
    {
        public string OrderId { get; set; } = null!;

        public DateTime OrderDate { get; set; }

        public int OrderTotalQuantity { get; set; }

        public int OrderTotalPrice { get; set; }

        public string OrderCustomerName { get; set; } = null!;

        public string OrderCustomerPhone { get; set; } = null!;

        public string OrderCustomerAddress { get; set; } = null!;

        public string? CustomerId { get; set; }

        public string? EmployeeId { get; set; }

        public int? PaymentMethodId { get; set; }

        public int? StatusOrderId { get; set; }
    }
}
