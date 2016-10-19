namespace TaskCat.Lib.Email.SendWithUs
{
    using System.Collections.Generic;

    public class SendWithUsSettings
    {
        public string ApiKey { get; set; }
        public string ProviderId { get; set; }
        public Dictionary<string,string> Templates { get; set; }
    }
}