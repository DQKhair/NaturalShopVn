using MailKit.Net.Smtp;
using MimeKit;
using NaturalShop.Models;
using NautralShop.Models;
using System.Security.Cryptography;

namespace NaturalShop.Services
{
	public class Mail
	{
		public async Task SendEmailAsync(string toEmail, string subject, string message)
		{
			var email = new MimeMessage();
			email.From.Add(new MailboxAddress("Natural Shop", "khaidinh230702@gmail.com"));
			email.To.Add(new MailboxAddress("Reset passwork", toEmail));
			email.Subject = subject;

			email.Body = new TextPart("plain")
			{
				Text = message
			};

			using (var client = new SmtpClient())
			{
				await client.ConnectAsync("smtp.gmail.com", 587, false);
				await client.AuthenticateAsync("khaidinh230702@gmail.com", "kbiz xwfc mibl jepb");

				await client.SendAsync(email);

				await client.DisconnectAsync(true);
			}
		}

        public async Task SendEmailAsyncForProduct(string toEmail, string subject,int totalMoneyOrder, List<Cart> carts, InfomationCustomerCheckout dataInfoCus)
        {
            string paymentMethod = dataInfoCus.PaymentMethod == 1 ? "Thanh toán khi nhận hàng" : "Thanh toán online";
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("Natural Shop", "khaidinh230702@gmail.com"));
            email.To.Add(new MailboxAddress("Đơn hàng đã đặt", toEmail));
            email.Subject = subject;

		   var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = "<h1>Natural shop cám ơn quý khách đã đặt sản phẩm </h1>";
            bodyBuilder.HtmlBody += "<h3>Thông tin khách hàng</h1>";
            bodyBuilder.HtmlBody += $"<p>Tên khách hàng: {dataInfoCus.LName} {dataInfoCus.FName}</p>";
            bodyBuilder.HtmlBody += $"<p>Địa chỉ nhận hàng: {dataInfoCus.AddressStreet}, {dataInfoCus.AddressWard}, {dataInfoCus.AddressDistrict}, {dataInfoCus.AddressCity}</p>";
            bodyBuilder.HtmlBody += $"<p>Số điện thoại: {dataInfoCus.Phone}</p>";
            bodyBuilder.HtmlBody += $"<p>Tổng tiền (bao gồm phí vận chuyển): {totalMoneyOrder} VND</p>";
            bodyBuilder.HtmlBody += $"<p>Phương thức thanh toán: {paymentMethod} </p>";
            bodyBuilder.HtmlBody += "<h3>Thông tin sản phẩm</h1>";
            bodyBuilder.HtmlBody += "<table border='1'><tr><th>Tên sản phẩm</th><th>Số lượng</th><th>Giá</th></tr>";
            foreach (var item in carts)
            {
                bodyBuilder.HtmlBody += $"<tr><td> {item.Product?.ProductName} </td><td> {item.Quantity} </td><td>{(item.Product?.ProductPrice - item.Product?.ProductValuePromotion) * item.Quantity} VND </td></tr>";
            }

            bodyBuilder.HtmlBody += "</table>";

            email.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 587, false);
                await client.AuthenticateAsync("khaidinh230702@gmail.com", "kbiz xwfc mibl jepb");

                await client.SendAsync(email);

                await client.DisconnectAsync(true);
            }
        }

        public async Task SendEmailAsyncForEmployee(int totalMoneyOrder, List<Cart> carts, InfomationCustomerCheckout dataInfoCus)
        {
            string paymentMethod = dataInfoCus.PaymentMethod == 1 ? "Thanh toán khi nhận hàng" : "Thanh toán online";
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("Natural Shop", "khaidinh230702@gmail.com"));
            email.To.Add(new MailboxAddress("Có đơn hàng mới được đặt", "jonddemon2307@gmail.com"));
            email.Subject = "Có đơn hàng mới được đặt";

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = "<h1>Natural shop có đơn hàng mới </h1>";
            bodyBuilder.HtmlBody += "<h3>Thông tin khách hàng</h1>";
            bodyBuilder.HtmlBody += $"<p>Tên khách hàng: {dataInfoCus.LName} {dataInfoCus.FName}</p>";
            bodyBuilder.HtmlBody += $"<p>Địa chỉ nhận hàng: {dataInfoCus.AddressStreet}, {dataInfoCus.AddressWard}, {dataInfoCus.AddressDistrict}, {dataInfoCus.AddressCity}</p>";
            bodyBuilder.HtmlBody += $"<p>Số điện thoại: {dataInfoCus.Phone}</p>";
            bodyBuilder.HtmlBody += $"<p>Tổng tiền (bao gồm phí vận chuyển): {totalMoneyOrder} VND</p>";
            bodyBuilder.HtmlBody += $"<p>Phương thức thanh toán: {paymentMethod} </p>";
            bodyBuilder.HtmlBody += "<h3>Thông tin sản phẩm</h1>";
            bodyBuilder.HtmlBody += "<table border='1'><tr><th>Tên sản phẩm</th><th>Số lượng</th><th>Giá</th></tr>";
            foreach (var item in carts)
            {
                bodyBuilder.HtmlBody += $"<tr><td> {item.Product?.ProductName} </td><td> {item.Quantity} </td><td>{(item.Product?.ProductPrice - item.Product?.ProductValuePromotion) * item.Quantity} VND </td></tr>";
            }

            bodyBuilder.HtmlBody += "</table>";

            email.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 587, false);
                await client.AuthenticateAsync("khaidinh230702@gmail.com", "kbiz xwfc mibl jepb");

                await client.SendAsync(email);

                await client.DisconnectAsync(true);
            }
        }
    }
}
