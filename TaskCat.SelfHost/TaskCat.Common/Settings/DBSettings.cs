namespace TaskCat.Common.Settings
{
    using System.ComponentModel.DataAnnotations;
    public class DBSettings
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "DB ConnectionString is Missing")]
        public string ConnectionString { get; set; }
    }
}
