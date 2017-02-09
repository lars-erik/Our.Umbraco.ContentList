namespace Our.Umbraco.ContentList.DataSources
{
    public class PagingParameter
    {
        public PagingParameter()
            : this(1000)
        {
        }

        public PagingParameter(int take)
        {
            Take = take;
        }

        public PagingParameter(int skip, int take)
        {
            Skip = skip;
            Take = take;
        }

        public PagingParameter(int skip, int take, int preSkip)
        {
            Skip = skip;
            Take = take;
            PreSkip = preSkip;
        }

        public int Skip { get; set; }

        public int Take { get; set; }

        public int PreSkip { get; set; }
    }
}