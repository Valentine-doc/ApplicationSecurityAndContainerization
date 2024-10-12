namespace AppSecuriAndContainer.Models
{
    public class User
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public byte[] Password { get; set; }

        public byte[] Salt { get; set; }

        public RoleId RoleId  { get; set; }

        public virtual Role? Role { get; set; }
    }
}
