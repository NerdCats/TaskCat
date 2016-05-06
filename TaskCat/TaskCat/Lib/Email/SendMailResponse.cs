namespace TaskCat.Lib.Email
{
    using System.Net;

    public class SendMailResponse
    {
        public bool Success { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string Error { get; set; }
    }
}