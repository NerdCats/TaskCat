namespace TaskCat.Lib.Email
{
    using Model;

    public interface IEmailTemplateParser
    {
        string PopulateEmail<T>(T payload) where T : IEmailPayload;
        string PopulateTemplate(string template, string templateKey, object payload);
    }
}