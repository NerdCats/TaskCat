using System.Globalization;
using TaskCat.Data.Model.Inventory;

namespace TaskCat.Lib.Email.Model
{
    public class EmailInvoiceItem 
    {
        public string Name { get; set; }
        public string Total { get; set; }
        public string PicUrl { get; set; }
        public int Quantity { get; set; }

        public EmailInvoiceItem()
        {

        }

        public EmailInvoiceItem(ItemDetails itemDetails, string cultureCode)
        {
            this.Name = itemDetails.Item;
            this.PicUrl = itemDetails.PicUrl;
            this.Quantity = itemDetails.Quantity;
            this.Total = itemDetails.TotalPlusVAT.ToString("C", new CultureInfo(cultureCode));
        }
    }
}