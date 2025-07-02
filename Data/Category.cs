namespace DoAn.Data
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; //name cate gym & spa , party event , ...

        public ICollection<Product> Products { get; set; } = new List<Product>(); // cate may have many product or service
    }
}
