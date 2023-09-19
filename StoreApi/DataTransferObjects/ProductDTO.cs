namespace StoreApi.DataTransferObjects
{
    public class ProductDTO
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;

        public string? Brand { get; set; }

        public decimal Price { get; set; }

        public string Category { get; set; } = null!;

        public string? SubCategory { get; set; }

        public string? Info { get; set; }

        public int? ImageId { get; set; }

        public string Image { get; set; } = null!;
    }
}
