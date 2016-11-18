namespace TaskCat.Common.Email.Model
{
    using Settings;

    public interface IEmailPayload
    {
        ProprietorSettings Proprietor { get; set; }
    }
}
