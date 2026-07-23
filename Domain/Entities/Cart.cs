namespace Xenon.Domain.Models
{
    public class Cart
    {
        public List<CartLine> Lines { get; set; } = new List<CartLine>();

        public virtual void AddItem(Product product, int quantity)
        {
            CartLine? line = Lines
                .Where(p => p.Product.ProductID == product.ProductID)
                .FirstOrDefault();
            if (line == null)
            {
                Lines.Add(new CartLine
                {
                    Product = product,
                    Quantity = quantity
                });
            }
            else
            {
                line.Quantity += quantity;
            }
        }
        public virtual void RemoveLine(Product product) =>
            Lines.RemoveAll(l => l.Product.ProductID == product.ProductID);
        public virtual void DecreaseItem(Product product)
        {
            CartLine? line = Lines.FirstOrDefault(l => l.Product.ProductID == product.ProductID);
            if (line != null)
            {
                if (line.Quantity > 1)
                    line.Quantity--;
                else
                    Lines.Remove(line);
            }
        }
        public virtual void DecreaseItem(long productID)
        {
            CartLine? line = Lines.FirstOrDefault(l => l.Product.ProductID == productID);
            if (line != null)
            {
                if (line.Quantity > 1)
                    line.Quantity--;
                else
                    Lines.Remove(line);
            }
        }

        public decimal ComputeTotalValue() =>
            Lines.Sum(e => e.Product.Price * e.Quantity);
        public virtual void Clear() => Lines.Clear();

        
    }
    public class CartLine
    {
        public int CartLineID { get; set; }
        public Product Product { get; set; } = new();
        public int Quantity { get; set; }
    }
}