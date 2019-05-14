using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramShop
{
    class Cart
    {
        List<ProductDetail> ProductDetails = new List<ProductDetail>();

        public List<ProductDetail> GetProductDetails() { return ProductDetails; }

        public int IsCartContainProduct(ProductDetail productDetail)
        {
            for(int i = 0; i< ProductDetails.Count; ++i)
            {
                if (ProductDetails[i].Product.id == productDetail.Product.id)
                    return i;
            }

            return -1;
        }

        public void Add(ProductDetail productDetail)
        {
            int index = IsCartContainProduct(productDetail);

            if (index == -1)
                ProductDetails.Add(productDetail);
            else
                ProductDetails[index].Count++;
        }
    }
}
