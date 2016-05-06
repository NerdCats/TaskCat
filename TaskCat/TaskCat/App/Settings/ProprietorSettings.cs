namespace TaskCat.App.Settings
{
    using Data.Model.Geocoding;
    using System.ComponentModel.DataAnnotations;

    public class ProprietorSettings
    {
        public ProprietorSettings()
        {
                
        }

        [Required]
        public string Name { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public DefaultAddress Address { get; set; }
        [Required]
        public string CultureCode { get; set; }
        public SocialLinks Socials { get; set; }
        public string LogoUrl { get; set; }
        public string Website { get; set; }
    }

    public class SocialLinks
    {
        public string Facebook { get; set; }
        public string Twitter { get; set; }
    }
}