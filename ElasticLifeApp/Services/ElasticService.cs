using ElasticLifeApp.Models;
using ElasticLifeApp.Services.Helpers;
using Nest;

namespace ElasticLifeApp.Services
{
    public class ElasticService
    {
        private readonly DataRepository _dataRepository;
        private readonly IElasticClient _client;

        private const string ELASTIC_INDEX_NAME = "test";
        public ElasticService(
            DataRepository dataRepository,
            IElasticClient elasticClient)
        {
            _dataRepository = dataRepository;
            _client = elasticClient;
        }

        public void CheckForData()
        {
            var indexExistsResponse = _client.Indices.Exists(ELASTIC_INDEX_NAME);

            if (!indexExistsResponse.Exists)
            {
                BulkIndex();
            }
        }

        public void BulkIndex()
        {
            var dataCount = _dataRepository.GetDataCount();
            var pages = Math.Ceiling(dataCount / 10000.0);

            for (var i = 0; i < pages; i++)
            {
                var data = _dataRepository.GetPagedProducts(i);

                var elasticResponse = _client.IndexMany(data, ELASTIC_INDEX_NAME);
            }
        }

        public IEnumerable<Product> Search(SearchRequestModel request)
        {
            var searchDescriptor = new SearchDescriptor<Product>();
            searchDescriptor.Index(ELASTIC_INDEX_NAME);

            //The boolean query is built by combining the filter and the title match
            var titleMatchQuery = BuildTitleMatchQuery(request.Query);

            QueryContainer filterQuery = null;
            if (request.FilterInStock)
            {
                filterQuery = FilterInStock();
            }

            var boolQueryDescriptor = new QueryContainerDescriptor<Product>();

            //The filter query can also take an array of QueryContainer if filtering on more than one field is required
            boolQueryDescriptor.Bool(b => b
                                    .Filter(filterQuery)
                                    .Must(titleMatchQuery));

            searchDescriptor.Query(x=>boolQueryDescriptor);

            //Sorting can be done by multiple fields
            //Sorting is applied in the same order they are given to the query
            if (request.SortByPrice)
            {
                searchDescriptor = SortByPrice(searchDescriptor);
            }

            ElasticHelper.TranslateNESTToElasticQuery(_client,searchDescriptor);

            var response = _client.Search<Product>(searchDescriptor);

            return response.Documents;
        }

        //When using filter on a text field, the term query should be used on the keyword field
        public QueryContainer FilterInStock()
        {
            QueryContainerDescriptor<Product> descriptor = new QueryContainerDescriptor<Product>();
            var queryFilterContainer = descriptor.Term(t => t.Field("availability.keyword").Value("in stock"));
            return queryFilterContainer;
        }

        public SearchDescriptor<Product> SortByPrice(SearchDescriptor<Product> searchDescriptor)
        {
            return searchDescriptor.Sort(s => s.Descending(x => x.Price));
        }

        public QueryContainer BuildTitleMatchQuery(string query)
        {
            QueryContainerDescriptor<Product> descriptor = new QueryContainerDescriptor<Product>();
            descriptor.Match(x => x.Field(f => f.Title).Query(query));

            return descriptor;
        }

        //The previous boosting query from the lecture
        public QueryContainer BuildBoostQuery(string query)
        {
            QueryContainerDescriptor<Product> descriptor = new QueryContainerDescriptor<Product>();
            descriptor.Boosting(b => b.Negative(n => n
                                          .Match(m => m.Field("title").Query("për")))
                                       .Positive(p => p
                                           .Bool(b => b
                                              .Must(m => m
                                                 .Match(match => match
                                                    .Field("title")
                                                    .Query(query)))))
                                            .NegativeBoost(0.5));

            return descriptor;
        }


    }
}
