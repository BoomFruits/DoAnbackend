namespace DoAn.DTO
{
    public class ProductFilterDTO
    {
        public string? Keyword { get; set; }
        public double? MinPrice { get; set; }
        public double? MaxPrice { get; set; }
        public int? CategoryId { get; set; }
        public bool? IsAvailable { get; set; }

        public int Page { get; set; } = 1;        
        public int PageSize { get; set; } = 10; 
    }
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalItems { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }

        //public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
    }
}
