using System;
using System.ComponentModel.DataAnnotations;
using iread_identity_ms.DataAccess.Data.Type;

namespace iread_identity_ms.Web.Dto
{

    public class RegisterStudentDto
    {

        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = true)]
        public string Password { get; set; }

        [Required(AllowEmptyStrings = false)]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        public Nullable<int> AvatarId { get; set; }

    }
}