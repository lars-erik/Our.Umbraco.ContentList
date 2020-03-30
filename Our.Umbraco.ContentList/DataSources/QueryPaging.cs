namespace Our.Umbraco.ContentList.DataSources
{
    public class QueryPaging
    {
        public QueryPaging()
            : this(1000)
        {
        }

        public QueryPaging(long take)
        {
            Take = take;
        }

        public QueryPaging(long skip, long take)
        {
            Skip = skip;
            Take = take;
        }

        public QueryPaging(long skip, long take, long preSkip)
        {
            Skip = skip;
            Take = take;
            PreSkip = preSkip;
        }

        public long Skip { get; set; }

        public long Take { get; set; }

        public long PreSkip { get; set; }
    }
}