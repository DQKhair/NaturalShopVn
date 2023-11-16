using System.ComponentModel.DataAnnotations;

namespace NautralShop.Areas.Admin.Models
{
    public class EmployeeVM
    {
        public string EmployeeId { get; set; } = null!;
        [Required(ErrorMessage = "Vui lòng nhập họ")]
        public string EmployeeFName { get; set; } = null!;
        [Required(ErrorMessage = "Vui lòng nhập tên")]
        public string EmployeeLName { get; set; } = null!;

        public string? EmployeeAddress { get; set; }

        public string? EmployeeEmail { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Số điện thoại chỉ chấp nhận các ký tự số")]
        public string? EmployeePhone { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên tài khoản")]
        [MaxLength(50)]
        [MinLength(6,ErrorMessage ="Ít nhất 6 ký tự")]
        public string? EmployeeUsername { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [MaxLength(50)]
        public string? EmployeePassword { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập lại mật khẩu")]
        [Compare("EmployeePassword", ErrorMessage = "Mật khẩu và mật khẩu nhập lại không khớp")]
        public string? EmployeeRePassword { get; set; }

    }
}
