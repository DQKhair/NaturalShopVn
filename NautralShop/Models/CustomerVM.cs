using System.ComponentModel.DataAnnotations;

namespace NautralShop.Models
{
	public class CustomerVM
	{
		[Required(ErrorMessage = "Vui lòng nhập họ")]
		public string CustomerLastName { get; set; } = null!;

		[Required(ErrorMessage = "Vui lòng nhập tên")]
		public string CustomerFirstName { get; set; } = null!;

		public string? CustomerAddress { get; set; }

		public string? CustomerEmail { get; set; }

		[Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
		[RegularExpression(@"^[0-9]*$", ErrorMessage = "Số điện thoại chỉ chấp nhận các ký tự số")]
		public string? CustomerPhone { get; set; }

		[Required(ErrorMessage = "Vui lòng nhập tên tài khoản")]
		public string? CustomerUsername { get; set; }

		[Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
		public string? CustomerPassword { get; set; }

		[Required(ErrorMessage = "Vui lòng nhập lại mật khẩu")]
		[Compare("CustomerPassword", ErrorMessage = "Mật khẩu và mật khẩu nhập lại không khớp")]
		public string? CustomerRePassword { get; set; }

		public int? CustomerPoint { get; set; }

		public bool CustomerStatus { get; set; }

		public int? AccountTypeId { get; set; }

	}
}
