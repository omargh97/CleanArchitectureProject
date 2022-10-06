using Demo.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Demo.Entities
{
    public partial class Users : Base
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z''-'\s]{1,40}$", ErrorMessage = "Characters are not allowed.")]
        public string LastName { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z''-'\s]{1,40}$", ErrorMessage = "Characters are not allowed.")]
        public string FirstName { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z''-'\s]{1,40}$", ErrorMessage = "Characters are not allowed.")]
        public string UserName { get; set; }

        [JsonIgnore]
        [Required]
        //[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$", ErrorMessage = "Characters are not allowed.")] //Exemple Test@1234
        [DataType(DataType.Password)]
        public byte[] PasswordHash { get; set; }

        [JsonIgnore]
        [Required]
        public byte[] PasswordSalt { get; set; }


        public string Token { get; set; }

        public Roles Role { get; set; }
    }
}
