namespace TaskCat.Common.Utility.Odata
{
    public class OdataRequestModel
    {
        public string OdataQueryString { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public bool Envelope { get; set; }
        public bool CountOnly { get; set; }
        public bool ContainsSelect { get; set; }
    }
}
