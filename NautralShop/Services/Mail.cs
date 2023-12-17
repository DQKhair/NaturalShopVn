using MailKit.Net.Smtp;
using MimeKit;

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
	}
}
