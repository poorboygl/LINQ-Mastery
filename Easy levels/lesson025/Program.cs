using System.Net.NetworkInformation;
using System.Numerics;

class Program
{
    static void Main()
    {
        var repo = new ProductRepository();

        var discountedProducts = repo.GetDiscountedProducts(100, 0.1m);

        foreach (var p in discountedProducts)
        {
            Console.WriteLine($"{p.Name} - Discounted: {p.DiscountedPrice}");
        }

        Console.ReadLine();
    }  
}
public class Product
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public decimal Price { get; set; }
}

public class DiscountedProduct
{
    public required string Name { get; set; }
    public decimal DiscountedPrice { get; set; }
}

public class ProductRepository
{
    public List<Product> Products { get; set; } =
    [
        new Product { Id = 1, Name = "Laptop", Price = 1200.00m },
        new Product { Id = 2, Name = "Mouse", Price = 25.00m },
        new Product { Id = 3, Name = "Keyboard", Price = 45.00m },
        new Product { Id = 4, Name = "Monitor", Price = 200.00m },
        new Product { Id = 5, Name = "Desk", Price = 300.00m },
        new Product { Id = 6, Name = "Chair", Price = 85.00m }
    ];


    public List<DiscountedProduct> GetDiscountedProducts(decimal priceThreshold, decimal discountPercentage)
    {

        return [.. Products
            .Where(p => p.Price > priceThreshold)
            .Select(p => new DiscountedProduct
            {
                Name = p.Name,
                DiscountedPrice = p.Price * (1 - discountPercentage)
            })];
    }

    //public List<DiscountedProduct> GetDiscountedProducts(decimal priceThreshold, decimal discountPercentage)
    //{

    //    return Products
    //        .Where(p => p.Price > priceThreshold)
    //        .Select(p => new DiscountedProduct
    //        {
    //            Name = p.Name,
    //            DiscountedPrice = p.Price * (1 - discountPercentage)
    //        })
    //        .ToList();
    //}
}

/*
 ✔ Vì sao gọi là Projection?

Projection nghĩa là chiếu dữ liệu từ kiểu này sang kiểu khác — tức là tạo ra một object mới chỉ chứa những trường bạn muốn.

 ✔ Dấu hiệu nhận biết Projection

Projection xảy ra khi bạn:

Dùng select

Hoặc dùng lambda dạng x => new Something { ... }

Hoặc chọn ra một phần field, không dùng full object

 */
//p => new DiscountedProduct
//{
//    Name = p.Name,
//    DiscountedPrice = p.Price * (1 - discountPercentage)
//}


/*
Laptop - Discounted: 1080.000
Monitor - Discounted: 180.000
Desk - Discounted: 270.000
 */

/*
 In this exercise, you are asked to complete the GetDiscountedProducts method by combining Where and Select.Here’s a breakdown of the solution:

* 1.Using Where to Filter by Price Threshold:

Where(p => p.Price > priceThreshold) filters products based on the price condition.

* 2.Using Select to Project New Structure:

Select(p => new DiscountedProduct { ... }) projects each filtered product into a DiscountedProduct object with the Name and DiscountedPrice.

DiscountedPrice is calculated as p.Price * (1 - discountPercentage).

* 3.Example Execution:

Calling GetDiscountedProducts(100, 0.1m) will return a list of products with prices above $100, each with a 10% discount applied.
 
 */

