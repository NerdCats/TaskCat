namespace TaskCat.Model.Response
{
    public class ErrorResponse
    {
        public string Message { get; set; }
        public object Data { get; set; }

        public bool ShouldSerializeData()
        {
            return Data != null;
        }
    }
}