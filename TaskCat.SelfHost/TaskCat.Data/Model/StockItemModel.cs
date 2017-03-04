namespace TaskCat.Data.Model
{
    using System.ComponentModel.DataAnnotations;

    public class StockItemModel
    {
        public string RefId { get; set; }
        public string RefEntityType { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Item name not provided")]
        public string Item { get; set; }
        public string PicUrl { get; set; }
        public int Quantity { get; set; }
        public string Location { get; set; }
    }
}
