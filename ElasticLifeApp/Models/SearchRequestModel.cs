namespace ElasticLifeApp.Models
{
    public class SearchRequestModel
    {
        public string Query { get; set; }

        //default values
        public bool FilterInStock { get; set; } = true;
        public bool SortByPrice { get; set; } = true;
    }
}
