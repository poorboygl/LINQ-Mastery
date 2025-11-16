class Program
{
    static void Main()
    {
        var productRepository = new ProductRepository();

        var random = new Random();
        int number = random.Next(1, 5); // tạo số từ 1 đến 4 (5 là exclusive)
        Console.WriteLine($"The number is: {number}");
        var product = productRepository.GetProductById(number);

        if (product == null) return;

        // Lấy sản phẩm đầu tiên có giá > product.Price
        var expensiveProduct = productRepository.GetFirstProductAbovePrice(product.Price);

        Console.WriteLine("GetFirstProductAbovePrice:");
        if (expensiveProduct != null)
        {
            Console.WriteLine($"ID: {expensiveProduct.Id}, Name: {expensiveProduct.Name}, Price: {expensiveProduct.Price}");
        }
        else
        {
            Console.WriteLine("No product found above the price threshold.");
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

public class ProductRepository
{
    public List<Product> Products { get; set; } =
    [
        new Product { Id = 1, Name = "Laptop", Price = 1200.00m },
        new Product { Id = 2, Name = "Mouse", Price = 25.00m },
        new Product { Id = 3, Name = "Keyboard", Price = 45.00m },
        new Product { Id = 4, Name = "Monitor", Price = 200.00m }
    ];

    // Trả về sản phẩm đầu tiên có giá > priceThreshold
    public Product? GetFirstProductAbovePrice(decimal priceThreshold)
    {
        // thêm ? sau Product vì return có thể ra null
        return Products.FirstOrDefault(p => p.Price > priceThreshold);
    }

    // Trả về sản phẩm theo Id
    public Product? GetProductById(int id)
    {
        return Products.SingleOrDefault(p => p.Id == id);
    }
}

/*
 In this exercise, you are asked to complete two methods in the ProductRepository class. Here’s a breakdown of each solution:

Using FirstOrDefault for GetFirstProductAbovePrice:

FirstOrDefault is used to return the first element in the list that matches a condition (or null if none match).

Products.FirstOrDefault(p => p.Price > priceThreshold) will return the first product with a Price greater than the specified priceThreshold.

Using SingleOrDefault for GetProductById:

SingleOrDefault is used to return a single element from the list that matches a condition.

Products.SingleOrDefault(p => p.Id == id) will return the product with the specified Id, or null if no such product exists.

SingleOrDefault  vs FirstOrDefault

FirstOrDefault returns the first element that matches the condition, or null if no match is found, and doesn't require the result to be unique.

SingleOrDefault expects exactly one match and throws an exception if there’s more than one, returning null only if no match is found.
 
 */