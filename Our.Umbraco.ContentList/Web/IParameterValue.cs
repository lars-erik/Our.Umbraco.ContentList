namespace Our.Umbraco.ContentList.Web
{
    public interface IParameterValue
    {
        string Key { get; set; }
        object Value { get; set; }
    }
}