namespace TaskCat.Common.Email
{
    using System.Net;

    public class SendEmailResponse
    {
        public SendEmailResponse(HttpStatusCode statusCode)
        {
            this.StatusCode = statusCode;
            if ((int)StatusCode >= 200 && (int)StatusCode <= 300)
            {
                this.Success = true;
            }
        }

        public SendEmailResponse(HttpStatusCode statusCode, string error) : this(statusCode)
        {
            this.Error = error;
        }
        public bool Success { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string Error { get; set; }
    }
}