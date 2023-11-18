using System.ComponentModel.DataAnnotations;

namespace NautralShop.Models
{
	public class UserLogin
	{
		[Required(ErrorMessage = "Tên tài khoản không được để trống")]
		public string? Username {  get; set; }
		[Required(ErrorMessage = "Mật khẩu không được để trống")]
		public string? Password { get; set; }
		public bool RememberMe { get; set; }
	}
}
