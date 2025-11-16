
class Program
{
    static void Main()
    {
        // Khởi tạo repository
        var productRepository = new ProductRepository();

        var Products = productRepository.GetSortedProducts();

        Console.WriteLine("GetSortedProducts");
        foreach (var product in Products)
        {
            Console.WriteLine($"Id: {product.Id}, Name: {product.Name}, Price: {product.Price}");
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
        new Product { Id = 4, Name = "Laptop", Price = 1000.00m }
    ];

    public List<Product> GetSortedProducts()
    {
        return [.. Products.OrderBy(p => p.Name).ThenByDescending(p => p.Price)];
    }


    //public List<Product> GetSortedProducts()
    //{
    //    return Products.OrderBy(p => p.Name).ThenByDescending(p => p.Price).ToList();
    //}
}

/*
In this exercise, you are asked to complete the GetSortedProducts method in the ProductRepository class. Here’s a breakdown of the solution:

Using OrderBy and ThenByDescending:

The OrderBy method sorts the list by the Name property in ascending order.

The ThenByDescending method is chained to handle cases where multiple products have the same Name. It sorts these products by Price in descending order.

Converting to List with ToList():

The result of OrderBy and ThenByDescending is an IEnumerable<Product>.

Calling ToList() converts this to a List<Product>, which matches the return type of the method.

Example Execution:

With the provided sample data, calling GetSortedProducts() would return the list sorted with "Keyboard" first, then "Laptop" (highest price first), and so on.
 
 */