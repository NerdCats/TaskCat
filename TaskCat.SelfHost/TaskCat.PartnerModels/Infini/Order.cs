using System.Collections.Generic;

namespace TaskCat.PartnerModels.Infini
{
    public class Order
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public int otype { get; set; }
        public int order_amt { get; set; }
        public int pay_amt { get; set; }
        public int cod_charges { get; set; }
        public int gifting_charges { get; set; }
        public int payment_method { get; set; }
        public int payment_status { get; set; }
        public int payment_id { get; set; }
        public string transaction_id { get; set; }
        public string transaction_status { get; set; }
        public string description { get; set; }
        public Dictionary<string, CartItem> cart { get; set; }
        public int cashback_used { get; set; }
        public int cashback_earned { get; set; }
        public int cashback_credited { get; set; }
        public int voucher_used { get; set; }
        public int coupon_used { get; set; }
        public int discount_type { get; set; }
        public string discount_amt { get; set; }
        public int shipping_amt { get; set; }
        public int voucher_amt_used { get; set; }
        public int coupon_amt_used { get; set; }
        public string shiplabel_tracking_id { get; set; }
        public string referal_code_used { get; set; }
        public int referal_code_amt { get; set; }
        public int user_ref_points { get; set; }
        public int ref_flag { get; set; }
        public OrderStatusCode order_status { get; set; }
        public int forward_id { get; set; }
        public string ship_date { get; set; }
        public string order_comment { get; set; }
        public int currency_id { get; set; }
        public int currency_value { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string address3 { get; set; }
        public string phone_no { get; set; }
        public string country_id { get; set; }
        public string zone_id { get; set; }
        public string postal_code { get; set; }
        public string city { get; set; }
        public int print_invoice { get; set; }
        public int cart_rec_notify1 { get; set; }
        public int cart_rec_notify2 { get; set; }
        public int incomplete_order { get; set; }
        public int flag_id { get; set; }
        public string flag_remark { get; set; }
        public string loyalty_cron_status { get; set; }
        public string remark { get; set; }
        public int credit_interest_rate { get; set; }
        public int credit_no_of_days { get; set; }
        public int credit_interest_val { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
    }
}
