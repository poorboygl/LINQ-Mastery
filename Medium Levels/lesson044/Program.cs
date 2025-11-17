class Program
{
    static void Main()
    {
        var repository = new SalesRepository();
        var summaries = repository.GetCategorySalesSummary();

        Console.WriteLine("Category Sales Summary:");
        Console.WriteLine("------------------------------------------------------------");
        Console.WriteLine("{0,-15} {1,15} {2,15} {3,20}", "Category", "Total Revenue", "Units Sold", "Average Price");

        foreach (var summary in summaries)
        {
            Console.WriteLine("{0,-15} {1,15:C} {2,15} {3,20:C}",
                summary.CategoryName,
                summary.TotalRevenue,
                summary.TotalUnitsSold,
                summary.AverageProductPrice);
        }

        Console.ReadKey();
    }
}

public class Category
{
    public int Id { get; set; }
    public required string Name { get; set; }
}

public class Product
{
    public int CategoryId { get; set; }
    public required string Name { get; set; }
    public decimal Price { get; set; }
    public int QuantitySold { get; set; }
}

public class CategorySalesSummary
{
    public required string CategoryName { get; set; }
    public decimal TotalRevenue { get; set; }
    public int TotalUnitsSold { get; set; }
    public decimal AverageProductPrice { get; set; }
}

public class SalesRepository
{
    public List<Category> Categories { get; set; } =
    [
        new Category { Id = 1, Name = "Electronics" },
        new Category { Id = 2, Name = "Furniture" },
        new Category { Id = 3, Name = "Decor" },
        new Category { Id = 4, Name = "Appliances" }
    ];

    public List<Product> Products { get; set; } =
    [
        new Product { CategoryId = 1, Name = "Laptop", Price = 1200.00m, QuantitySold = 5 },
        new Product { CategoryId = 1, Name = "Mouse", Price = 25.00m, QuantitySold = 50 },
        new Product { CategoryId = 2, Name = "Desk", Price = 300.00m, QuantitySold = 10 },
        new Product { CategoryId = 2, Name = "Chair", Price = 150.00m, QuantitySold = 5 }
    ];

    //? with GroupJoin
    public List<CategorySalesSummary> GetCategorySalesSummary()
    {
        return [.. Categories
            .GroupJoin(Products,
                category => category.Id,
                product => product.CategoryId,
                (category, categoryProducts) => new CategorySalesSummary
                {
                    CategoryName = category.Name,
                    TotalRevenue = categoryProducts.Sum(p => p.Price * p.QuantitySold),
                    TotalUnitsSold = categoryProducts.Sum(p => p.QuantitySold),
                    AverageProductPrice = categoryProducts.Any() ? categoryProducts.Average(p => p.Price) : 0
                })];
    }

    //? with Join
    //public List<CategorySalesSummary> GetCategorySalesSummary()
    //{
    //    return [.. Categories
    //        .Join(Products,
    //            category => category.Id,
    //            product => product.CategoryId,
    //            (category, product) => new { CategoryName = category.Name, ProductName = product.Name, product.Price, product.QuantitySold})
    //            .GroupBy( c => c.CategoryName)
    //            .Select( c => new CategorySalesSummary
    //            {
    //                CategoryName = c.Key,
    //                TotalRevenue = c.Sum(p => p.Price * p.QuantitySold),
    //                TotalUnitsSold = c.Sum(p => p.QuantitySold),
    //                AverageProductPrice = c.Any() ? c.Average(p => p.Price) : 0
    //            })];
    //}
}

/*
  ? with Join
     Category Sales Summary:
    ------------------------------------------------------------
    Category          Total Revenue      Units Sold        Average Price
    Electronics           $7,250.00              55              $612.50
    Furniture             $3,750.00              15              $225.00

? with GroupJoin
    Category Sales Summary:
    ------------------------------------------------------------
    Category          Total Revenue      Units Sold        Average Price
    Electronics           $7,250.00              55              $612.50
    Furniture             $3,750.00              15              $225.00
 !  Decor                     $0.00               0                $0.00
 !  Appliances                $0.00               0                $0.00
 
 */


/*
This exercise requires grouping and aggregating data across multiple collections to produce a summary for each category.

* 1.Grouping Products by Category with GroupJoin:

GroupJoin(Products, category => category.Id, product => product.CategoryId, ...) groups products under their respective categories.

* 2.Calculating Aggregates within Each Category:

TotalRevenue: Sums up the Price * QuantitySold for all products in each category.

TotalUnitsSold: Counts the total units sold by summing QuantitySold.

AverageProductPrice: Averages the product prices in each category.

* 3.Returning the Report:

The result is a list of CategorySalesSummary objects, with each item containing the aggregated sales data for a category.
 
 */