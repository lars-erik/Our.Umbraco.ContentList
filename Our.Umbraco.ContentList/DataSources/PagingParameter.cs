namespace Our.Umbraco.ContentList.DataSources
{
    public class PagingParameter
    {
        public PagingParameter()
            : this(1000)
        {
        }

        public PagingParameter(long take)
        {
            Take = take;
        }

        public PagingParameter(long skip, long take)
        {
            Skip = skip;
            Take = take;
        }

        public PagingParameter(long skip, long take, long preSkip)
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