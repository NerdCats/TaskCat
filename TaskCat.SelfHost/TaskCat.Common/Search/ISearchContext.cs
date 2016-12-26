namespace TaskCat.Common.Search
{
    using Nest;
    public interface ISearchContext
    {
        ElasticClient Client { get; }
    }
}