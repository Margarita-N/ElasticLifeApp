namespace ElasticLifeApp.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Link { get; set; }
        public string Availability { get; set; }
        public string Condition { get; set; }
        public string ImageLink { get; set; }
        public decimal Price { get; set; }
        public string Gtin { get; set; }
        public string Product_Type { get; set; }
        public string Brand { get; set; }
        public string Gender { get; set; }
    }
}
