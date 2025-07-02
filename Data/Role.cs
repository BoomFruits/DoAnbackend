namespace DoAn.Data
{
    public class Role
    {
        public int Id { get; set; }
        public string Rolename { get; set; } = string.Empty;
        public ICollection<User> Users { get; set; } = new List<User>();

    }
}
