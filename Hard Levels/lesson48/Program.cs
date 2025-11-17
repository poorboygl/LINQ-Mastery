class Program
{
    static void Main()
    {
        var repo = new SalesRepository();

        var summaries = repo.GetTopSellingProductsByCategory();

        foreach (var summary in summaries)
        {
            Console.WriteLine($"Category: {summary.CategoryName}");
            Console.WriteLine("Top Products:");

            if (summary.TopProducts.Count == 0)
            {
                Console.WriteLine("  (No products)");
            }
            else
            {
                foreach (var product in summary.TopProducts)
                {
                    Console.WriteLine($"  - {product.Name}, Units Sold: {product.UnitsSold}");
                }
            }

            Console.WriteLine();
        }

        Console.ReadKey();
    }
}

public class Product
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int CategoryId { get; set; }
    public decimal Price { get; set; }
    public int UnitsSold { get; set; }
}

public class Category
{
    public int Id { get; set; }
    public required string Name { get; set; }
}

public class CategoryTopSellingProducts
{
    public required string CategoryName { get; set; }
    public List<Product> TopProducts { get; set; } = [];
}

public class SalesRepository
{
    public List<Product> Products { get; set; } =
    [
        new Product { Id = 1, Name = "Laptop", CategoryId = 1, Price = 1000m, UnitsSold = 500 },
        new Product { Id = 2, Name = "Smartphone", CategoryId = 1, Price = 800m, UnitsSold = 300 },
        new Product { Id = 3, Name = "Tablet", CategoryId = 1, Price = 600m, UnitsSold = 150 },
        new Product { Id = 4, Name = "Chair", CategoryId = 2, Price = 100m, UnitsSold = 200 },
        new Product { Id = 5, Name = "Desk", CategoryId = 2, Price = 300m, UnitsSold = 250 },
        new Product { Id = 6, Name = "Bookcase", CategoryId = 2, Price = 400m, UnitsSold = 120 }
    ];

    public List<Category> Categories { get; set; } =
    [
        new Category { Id = 1, Name = "Electronics" },
        new Category { Id = 2, Name = "Furniture" }
    ];

    public List<CategoryTopSellingProducts> GetTopSellingProductsByCategory()
    {
        return [.. Categories
            .GroupJoin(Products,
                category => category.Id,
                product => product.CategoryId,
                (category, categoryProducts) => new CategoryTopSellingProducts
                {
                    CategoryName = category.Name,
                    TopProducts = [.. categoryProducts
                        .OrderByDescending(p => p.UnitsSold)
                        .Take(2)]
                })];
    }
}

/*
 Category: Electronics
Top Products:
  - Laptop, Units Sold: 500
  - Smartphone, Units Sold: 300

Category: Furniture
Top Products:
  - Desk, Units Sold: 250
  - Chair, Units Sold: 200
 */


/*
 This exercise involves grouping and ranking products within each category based on sales data.

* 1.Grouping Products by Category:

GroupJoin(Products, category => category.Id, product => product.CategoryId, ...) groups products under their respective categories.

* 2.Identifying Top-Selling Products:

Sorting: OrderByDescending(p => p.UnitsSold) sorts the products in descending order by units sold.

Selecting Top 2: Take(2) limits the result to the top 2 products by sales in each category.

* 3.Returning the Report:

The result is a list of CategoryTopSellingProducts objects, each containing a list of the top-selling products within the category.
 
 */