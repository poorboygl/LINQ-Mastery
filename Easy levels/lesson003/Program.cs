class Program
{
    static void Main()
    {
        var productRepository = new ProductRepository();

        var Products = productRepository.GetProductNamesAndPrices();

        Console.WriteLine("GetProductNamesAndPrices");
        foreach (var product in Products)
        {
            Console.WriteLine($"Name: {product.Name}, Price: {product.Price}");
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

    public List<(string Name, decimal Price)> GetProductNamesAndPrices()
    {
        return [.. Products.Select(p => (p.Name, p.Price))];
    }

    //public List<(string Name, decimal Price)> GetProductNamesAndPrices()
    //{
    //    return Products.Select(p => (p.Name, p.Price)).ToList();
    //}
}


/*
In this exercise, you are asked to complete the GetProductNamesAndPrices method in the ProductRepository class. Here’s a breakdown of the solution:

1- Using Select for Projection with Tuples:

The Select method allows us to project each Product object into a tuple that only includes Name and Price.

The syntax Products.Select(p => (p.Name, p.Price)) creates a list of tuples where each tuple has only the Name and Price values.

2- Converting to List with ToList():

The Select method returns an IEnumerable collection, so we call ToList() to convert it to a List<(string Name, decimal Price)>, which matches the return type of the method.

3- Example Execution:

With the sample data, calling GetProductNamesAndPrices() will return a list of tuples with only the Name and Price fields, like [ ("Laptop", 1200.00), ("Mouse", 25.00) ].
 
 */