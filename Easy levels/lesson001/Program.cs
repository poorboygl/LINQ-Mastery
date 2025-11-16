class Program
{
    static void Main()
    {
        // Khởi tạo repository
        var productRepository = new ProductRepository();

        // Lấy danh sách sản phẩm đắt hơn 50
        var expensiveProducts = productRepository.GetExpensiveProducts(50);

        // In ra
        Console.WriteLine("Expensive Products:");
        foreach (var product in expensiveProducts)
        {
            Console.WriteLine($"Id: {product.Id}, Name: {product.Name}, Price: {product.Price}");
        }

        // Dừng console
        Console.ReadLine();
    }
}


public class Product
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public decimal Price { get; set; }
}

public class ProductRepository
{
    public List<Product> Products { get; set; } =
    [
        new() { Id = 1, Name = "Laptop", Price = 1200.00m },
        new() { Id = 2, Name = "Mouse", Price = 25.00m },
        new() { Id = 3, Name = "Keyboard", Price = 45.00m },
        new() { Id = 4, Name = "Monitor", Price = 200.00m }
    ];

    //public List<Product> GetExpensiveProducts(decimal priceThreshold)
    //{
    //    return Products.Where(p => p.Price > priceThreshold).ToList();
    //}

    public List<Product> GetExpensiveProducts(decimal priceThreshold)
    {
        Func<Product, bool> isExpensive = p => p.Price > priceThreshold;

        return [.. Products.Where(isExpensive)];
    }

}


// where
//foreach(var p in Products)
//{
//    if(isExpensive(p)) // callback được gọi ở đây
//        yield return p;
//}


/* 
 
In this exercise, you are asked to complete the GetExpensiveProducts method in the ProductRepository class. Here’s a breakdown of the solution:

Using the Where Method:

The Where method in LINQ is used to filter collections based on a specified condition. In this case, we want to filter the Products list to only include products where the Price is greater than the priceThreshold parameter.

The syntax Products.Where(p => p.Price > priceThreshold) checks each product in the list to see if its Price is greater than priceThreshold.

Converting to List with ToList():

The Where method returns an IEnumerable<Product>, which is an interface for a collection of products that meet the filter condition.

To return this result as a List<Product>, we use the ToList() method. This ensures that the return type matches the method’s signature (List<Product>).

Putting It All Together:

The final solution is Products.Where(p => p.Price > priceThreshold).ToList();, which filters the products and converts them to a list in one line.

Example Execution

If you call GetExpensiveProducts(50), the method will:

Check each product in the Products list.

Return only the products with a Price greater than 50, such as "Laptop" and "Monitor".

This method provides an efficient way to retrieve products that meet specific price criteria.
 
 */
