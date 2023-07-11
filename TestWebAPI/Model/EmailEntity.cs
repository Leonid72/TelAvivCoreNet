using System.ComponentModel.DataAnnotations;

namespace TestWebAPI.Model
{
    public class EmailEntity
    {
        [DataType(DataType.EmailAddress, ErrorMessage = "E-mail is not valid")]
        public string Emailaddress { get; set; }
        public DateTime DateReceive { get; set; }
    }
}
