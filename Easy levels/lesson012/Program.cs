class Program
{
    static void Main()
    {
        var productRepo = new ProductRepository();

        int pageNumber = 2;
        int pageSize = 2;

        var page = productRepo.GetProductsPage(pageNumber, pageSize);

        Console.WriteLine($"Page {pageNumber} (PageSize {pageSize}):");
        foreach (var p in page)
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
        new Product { Id = 6, Name = "Chair", Price = 85.00m }
    ];

    public List<Product> GetProductsPage(int pageNumber, int pageSize)
    {
        return [.. Products
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)];
    }

    //public List<Product> GetProductsPage(int pageNumber, int pageSize)
    //{
    //    return Products
    //        .Skip((pageNumber - 1) * pageSize)
    //        .Take(pageSize)
    //        .ToList();
    //}
}


/*
 Page 2 (PageSize 2):
Id: 3, Name: Keyboard, Price: 45.00
Id: 4, Name: Monitor, Price: 200.00
 
 */

/*
 In this exercise, you are asked to complete the GetProductsPage method in the ProductRepository class by using Skip and Take. Here’s a breakdown of the solution:

1.Calculating Items to Skip:

Skip((pageNumber - 1) * pageSize) calculates the number of items to skip by multiplying pageSize with (pageNumber - 1). This skips all items from previous pages.

2.Using Take to Limit Results:

Take(pageSize) then limits the results to pageSize, retrieving only the items for the requested page.

3.Converting to a List of Products:

The result of Skip and Take is converted to List<Product> using ToList() to match the return type.

4.Example Execution:

Calling GetProductsPage(1, 2) will return the first two products.

Calling GetProductsPage(2, 2) will return the next two products.
 
 */