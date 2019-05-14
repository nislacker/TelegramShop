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

        public double GetCartTotalPrice()
        {
            double totalPrice = 0;

            for (int i = 0; i < ProductDetails.Count; ++i)
            {
                totalPrice += ProductDetails[i].Product.price * ProductDetails[i].Count;
            }

            return totalPrice;
        }

        public int GetProductsCount()
        {
            int count = 0;

            for (int i = 0; i < ProductDetails.Count; ++i)
            {
                count += ProductDetails[i].Count;
            }

            return count;
        }

        public void SetProductCount(ProductDetail productDetail, int count)
        {
            if (count <= 0) return;

            int i = ProductIndexInCart(productDetail);
            ProductDetails[i].Count = count;
        }

        public void IncrementProductCount(ProductDetail productDetail)
        {
            SetProductCount(productDetail, productDetail.Count + 1);
        }

        public void DecrementProductCount(ProductDetail productDetail)
        {
            SetProductCount(productDetail, productDetail.Count - 1);
        }

        public bool DeleteProductDetailByProductDetail(ProductDetail productDetail)
        {
            for (int i = 0; i < ProductDetails.Count; ++i)
            {
                if (ProductDetails[i].Product.id == productDetail.Product.id)
                {
                    ProductDetails.Remove(productDetail);
                    return true;
                }
            }

            return false;
        }

        public int ProductIndexInCart(ProductDetail productDetail)
        {
            for (int i = 0; i < ProductDetails.Count; ++i)
            {
                if (ProductDetails[i].Product.id == productDetail.Product.id)
                    return i;
            }

            return -1;
        }

        public int ProductIndexInCart(Product product)
        {
            for (int i = 0; i < ProductDetails.Count; ++i)
            {
                if (ProductDetails[i].Product.id == product.id)
                    return i;
            }

            return -1;
        }

        public void Add(ProductDetail productDetail)
        {
            int index = ProductIndexInCart(productDetail);

            if (index == -1)
                ProductDetails.Add(productDetail);
            else
                ProductDetails[index].Count++;
        }
    }
}
