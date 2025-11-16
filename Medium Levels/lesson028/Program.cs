class Program
{
    static void Main()
    {
        var repo = new ProductRepository();

        var comparisons = repo.GetProductComparisons();

        Console.WriteLine("== Product Comparisons ==");
        foreach (var cmp in comparisons)
        {
            Console.WriteLine(
                $"{cmp.ProductAName} vs {cmp.ProductBName} => Price Equal: {cmp.IsPriceEqual}"
            );
        }

        Console.ReadLine();
    }
}


public class Product
{
    public required string Name { get; set; }
    public decimal Price { get; set; }
}

public class ProductComparison
{
    public required string ProductAName { get; set; }
    public required string ProductBName { get; set; }
    public bool IsPriceEqual { get; set; }
}

public class ProductRepository
{
    public List<Product> StoreAProducts { get; set; } =
    [
        new Product { Name = "Laptop", Price = 1200.00m },
        new Product { Name = "Mouse", Price = 25.00m },
        new Product { Name = "Keyboard", Price = 45.00m }
    ];

    public List<Product> StoreBProducts { get; set; } =
    [
        new Product { Name = "Laptop", Price = 1150.00m },
        new Product { Name = "Keyboard", Price = 45.00m },
        new Product { Name = "Monitor", Price = 200.00m }
    ];

    // Complete this method
    public List<ProductComparison> GetProductComparisons()
    {
        return [.. StoreAProducts
           .Zip(StoreBProducts, (productA, productB) => new ProductComparison
           {
               ProductAName = productA.Name,
               ProductBName = productB.Name,
               IsPriceEqual = productA.Price == productB.Price
           })];
    }
}


/*
 == Product Comparisons ==
Laptop vs Laptop => Price Equal: False
Mouse vs Keyboard => Price Equal: False
Keyboard vs Monitor => Price Equal: False
 */


/*
In this exercise, you are asked to complete the GetProductComparisons method by using Zip. Here’s a breakdown of the solution:

* 1.Using Zip to Combine Two Collections:

Zip(StoreBProducts, (productA, productB) => ...) pairs each item in StoreAProducts with the corresponding item in StoreBProducts based on their positions in the list.

* 2.Creating ProductComparison Objects for Each Pair:

Each ProductComparison object stores the names of the paired products and a boolean indicating if their prices are equal.

* 3.Example Execution:

Calling GetProductComparisons() will return a list of ProductComparison objects, each showing the names and price comparison of paired products from the two stores.
 
 */