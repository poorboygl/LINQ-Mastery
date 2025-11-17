class Program
{
    static void Main()
    {
        var repo = new ProductRepository();

        var firstAbove100 = repo.GetFirstProductAbovePrice(100);
        Console.WriteLine(firstAbove100 != null
            ? $"First product above $100: {firstAbove100.Name} (${firstAbove100.Price})"
            : "No product found above $100");

        var lastBelow100 = repo.GetLastProductBelowPrice(100);
        Console.WriteLine(lastBelow100 != null
            ? $"Last product below $100: {lastBelow100.Name} (${lastBelow100.Price})"
            : "No product found below $100");

        var singleKeyboard = repo.GetSingleProductByName("Keyboard");
        Console.WriteLine(singleKeyboard != null
            ? $"Single product named 'Keyboard': {singleKeyboard.Name} (${singleKeyboard.Price})"
            : "No product named 'Keyboard' found");

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
    public Product? GetFirstProductAbovePrice(decimal priceThreshold)
    {
        // Use FirstOrDefault to find the first product with Price > priceThreshold
        return Products.FirstOrDefault(p => p.Price > priceThreshold); // Add your code here
    }

    // Complete this method
    public Product? GetLastProductBelowPrice(decimal priceThreshold)
    {
        // Use LastOrDefault to find the last product with Price < priceThreshold
        return Products.LastOrDefault(p => p.Price < priceThreshold); // Add your code here
    }

    // Complete this method
    public Product? GetSingleProductByName(string name)
    {
        // Use SingleOrDefault to find the product with the specified Name
        return Products.SingleOrDefault(p => p.Name == name);
    }
}


/*
First product above $100: Laptop ($1200.00)
Last product below $100: Chair ($85.00)
Single product named 'Keyboard': Keyboard ($45.00)
 */


/*
In this exercise, you are asked to complete three methods using FirstOrDefault, LastOrDefault, and SingleOrDefault. Here’s a breakdown of the solution:

* 1.Using FirstOrDefault to Find the First Product Above a Price:

FirstOrDefault(p => p.Price > priceThreshold) retrieves the first product with Price above priceThreshold or returns null if no such product exists.

* 2.Using LastOrDefault to Find the Last Product Below a Price:

LastOrDefault(p => p.Price < priceThreshold) retrieves the last product with Price below priceThreshold or returns null if no such product exists.

* 3.Using SingleOrDefault to Find a Unique Product by Name:

SingleOrDefault(p => p.Name == name) retrieves a unique product with the specified Name or returns null if no match or multiple matches exist.

* 4.Example Execution:

Calling GetFirstProductAbovePrice(100) returns the first product priced above $100.

Calling GetSingleProductByName("Laptop") returns the product named "Laptop" if it exists uniquely.
 
 */
