namespace DapperExamples.Model
{
    public class OrderDetail
    {
        public int OrderId { get; set; } 
        public int ProductId { get; set; } 
        public decimal UnitPrice { get; set; } 
        public short Quantity { get; set; } 
        public float Discount { get; set; } 

                /// <summary>
        /// Parent Product pointed by [OrderDetails].([ProductId]) (FK_Order_Details_Products)
        /// </summary>
        public virtual Product Product { get; set; } // FK_Order_Details_Products

    }
}
