namespace TaskCat.Lib.Email.Model
{
    using App.Settings;
    public interface IEmailPayload
    {
        ProprietorSettings Proprietor { get; set; }
    }
}
