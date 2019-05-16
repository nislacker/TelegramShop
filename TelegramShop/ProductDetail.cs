namespace TelegramShop
{
    class ProductDetail
    {
        public int Count { get; set; }

        public Product Product { get; set; }

        public ProductDetail()
        {
               
        }
        public ProductDetail(Product product, int count)
        {
            Product = product;
            Count = count;
        }
    }
}