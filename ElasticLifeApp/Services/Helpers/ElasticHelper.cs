using ElasticLifeApp.Models;
using Nest;

namespace ElasticLifeApp.Services.Helpers
{
    public static class ElasticHelper
    {
        public static void TranslateNESTToElasticQuery(IElasticClient elasticClient, SearchDescriptor<Product> searchDescriptor)
        {
            var stream = new System.IO.MemoryStream();
            elasticClient.RequestResponseSerializer.Serialize(searchDescriptor, stream);
            var jsonQuery = System.Text.Encoding.UTF8.GetString(stream.ToArray());
            File.WriteAllText(@"SearchQuery.json", jsonQuery);
        }
    }
}
