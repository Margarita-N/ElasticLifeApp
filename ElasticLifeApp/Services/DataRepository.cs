using ElasticLifeApp.Models;
using Newtonsoft.Json;

namespace ElasticLifeApp.Services
{
    public class DataRepository
    {
        private const string DATA_REPO_DIR = @"C:\Users\Margarita\Gjirafa\Resources\MallProducts";
        private const int PAGE_SIZE = 10000;

        private List<Product> _data;

        public DataRepository()
        {
            LoadData();
        }

        private void LoadData()
        {
            var dataFiles = Directory.GetFiles(DATA_REPO_DIR);
            _data = new List<Product>();

            foreach (var dataFile in dataFiles)
            {
                using (var reader = new StreamReader(dataFile))
                {
                    var stringData = reader.ReadToEnd();
                    var products = JsonConvert.DeserializeObject<List<Product>>(stringData);

                    _data.AddRange(products);
                }
            }
        }

        public int GetDataCount()
        {
            return _data.Count;
        }

        public IEnumerable<Product> GetPagedProducts(int page)
        {
            return _data.Skip(page * PAGE_SIZE).Take(PAGE_SIZE);
        }
    }
}
