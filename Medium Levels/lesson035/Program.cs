class Program
{
    static void Main()
    {
        var repo = new ProductRepository();

        var reversedProducts = repo.GetProductsInReverseOrder();

        Console.WriteLine("Products in reverse order:");
        foreach (var product in reversedProducts)
        {
            Console.WriteLine($"Id: {product.Id}, Name: {product.Name}, Price: {product.Price:C}");
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
        new Product { Id = 4, Name = "Monitor", Price = 200.00m },
        new Product { Id = 5, Name = "Desk", Price = 300.00m },
        new Product { Id = 6, Name = "Chair", Price = 85.00m }
    ];

    // Complete this method
    public List<Product> GetProductsInReverseOrder()
    {
        // Use Reverse to get products in reverse order
        return [.. Products.AsEnumerable().Reverse()];
    }
}

/*
 Products in reverse order:
Id: 6, Name: Chair, Price: $85.00
Id: 5, Name: Desk, Price: $300.00
Id: 4, Name: Monitor, Price: $200.00
Id: 3, Name: Keyboard, Price: $45.00
Id: 2, Name: Mouse, Price: $25.00
Id: 1, Name: Laptop, Price: $1,200.00
 */


/*
In this exercise, you are asked to complete the GetProductsInReverseOrder method by using Reverse. Here’s a breakdown of the solution:

* 1.Using Reverse to Reverse the Order of Products:

Use AsEnumerable()to creates an IEnumerable<Product> wrapper

Products.Reverse() reverses the order of products in the list.

AsEnumerable() creates an IEnumerable<Product> wrapper, and Reverse() reverses this enumeration without modifying the original collection. ToList() then creates a new list with the reversed order.

* 2.Converting to a List:

The result of Reverse is converted to List<Product> using ToList() to match the return type.

* 3.Example Execution:

Calling GetProductsInReverseOrder() returns the list of products in the opposite order of their original arrangement.
 
 */