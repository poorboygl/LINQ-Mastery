class Program
{
    static void Main()
    {
        var repo = new ProductRepository();

        int pageNumber = 2;
        int pageSize = 3;

        var productsOnPage = repo.GetProductsByPage(pageNumber, pageSize);

        Console.WriteLine($"Page {pageNumber} (PageSize = {pageSize}):");

        foreach (var p in productsOnPage)
        {
            Console.WriteLine($"Id: {p.Id}, Name: {p.Name}, Price: {p.Price}");
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
        new Product { Id = 6, Name = "Chair", Price = 85.00m },
        new Product { Id = 7, Name = "Lamp", Price = 20.00m },
        new Product { Id = 8, Name = "Pen", Price = 5.00m }
    ];

    // Complete this method
    public List<Product> GetProductsByPage(int pageNumber, int pageSize)
    {
        if (pageNumber < 1 || pageSize < 1)
            return [];

        // Use Skip and Take for pagination
        return [.. Products
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)];
    }
}


/*
Page 2 (PageSize = 3):
Id: 4, Name: Monitor, Price: 200.00
Id: 5, Name: Desk, Price: 300.00
Id: 6, Name: Chair, Price: 85.00
 */

/*
In this exercise, you are asked to complete the GetProductsByPage method by using Skip and Take. Here’s a breakdown of the solution:

* 1.Using Skip to Ignore Previous Pages:

Skip((pageNumber - 1) * pageSize) skips over items from previous pages based on pageNumber and pageSize.

* 2.Using Take to Limit the Page Size:

Take(pageSize) then limits the result to only pageSize items for the current page.

* 3.Handling Invalid Input:

If pageNumber or pageSize is less than 1, an empty list is returned.

* 4.Example Execution:

Calling GetProductsByPage(2, 3) returns products 4, 5, and 6. 
 
 */