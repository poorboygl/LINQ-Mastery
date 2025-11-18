class Program
{
    static void Main()
    {
        var storeRepository = new StoreRepository();

        var topSellingByCategory = storeRepository.GetTopSellingProductsByCategory();

        Console.WriteLine("=== Top Selling Products By Category ===");

        foreach (var categorySummary in topSellingByCategory)
        {
            Console.WriteLine($"\nCategory: {categorySummary.CategoryName}");
            foreach (var product in categorySummary.TopProducts)
            {
                Console.WriteLine($"Product: {product.ProductName}");
                Console.WriteLine($"  Total Quantity Sold: {product.TotalQuantitySold}");
                Console.WriteLine($"  Total Sales Amount: {product.TotalSales:C}");
            }
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
    public int Id { get; set; }
    public required string Name { get; set; }
    public int CategoryId { get; set; }
    public decimal Price { get; set; }
}

public class Sale
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public DateTime SaleDate { get; set; }
}

public class ProductSalesSummary
{
    public required string ProductName { get; set; }
    public decimal TotalSales { get; set; }
    public int TotalQuantitySold { get; set; }
}

public class CategoryTopProductsSummary
{
    public required string CategoryName { get; set; }
    public List<ProductSalesSummary> TopProducts { get; set; } = [];
}

public class StoreRepository
{
    public List<Category> Categories { get; set; } =
    [
        new Category { Id = 1, Name = "Electronics" },
        new Category { Id = 2, Name = "Furniture" }
    ];

    public List<Product> Products { get; set; } =
    [
        new Product { Id = 1, Name = "Laptop", CategoryId = 1, Price = 1000m },
        new Product { Id = 2, Name = "Smartphone", CategoryId = 1, Price = 800m },
        new Product { Id = 3, Name = "Desk", CategoryId = 2, Price = 200m },
        new Product { Id = 4, Name = "Chair", CategoryId = 2, Price = 150m }
    ];

    public List<Sale> Sales { get; set; } =
    [
        new Sale { ProductId = 1, Quantity = 5, SaleDate = new DateTime(2023, 10, 1) },
        new Sale { ProductId = 1, Quantity = 3, SaleDate = new DateTime(2023, 10, 5) },
        new Sale { ProductId = 2, Quantity = 10, SaleDate = new DateTime(2023, 10, 10) },
        new Sale { ProductId = 3, Quantity = 7, SaleDate = new DateTime(2023, 10, 12) },
        new Sale { ProductId = 4, Quantity = 12, SaleDate = new DateTime(2023, 10, 15) }
    ];

    public List<CategoryTopProductsSummary> GetTopSellingProductsByCategory()
    {
        return [.. Categories
            .GroupJoin(Products,
                category => category.Id,
                product => product.CategoryId,
                (category, categoryProducts) => new
                {
                    CategoryName = category.Name,
                    TopProducts = categoryProducts
                        .Select(product => new ProductSalesSummary
                        {
                            ProductName = product.Name,
                            TotalSales = Sales
                                .Where(s => s.ProductId == product.Id)
                                .Sum(s => s.Quantity * product.Price),
                            TotalQuantitySold = Sales
                                .Where(s => s.ProductId == product.Id)
                                .Sum(s => s.Quantity)
                        })
                        .OrderByDescending(p => p.TotalSales)
                        .Take(3)
                        .ToList()
                })
            .Select(summary => new CategoryTopProductsSummary
            {
                CategoryName = summary.CategoryName,
                TopProducts = summary.TopProducts
            })];
    }
}


/*
! === Top Selling Products By Category ===

    Category: Electronics
    Product: Laptop
      Total Quantity Sold: 8
      Total Sales Amount: $8,000.00
    Product: Smartphone
      Total Quantity Sold: 10
      Total Sales Amount: $8,000.00

    Category: Furniture
    Product: Chair
      Total Quantity Sold: 12
      Total Sales Amount: $1,800.00
    Product: Desk
      Total Quantity Sold: 7
      Total Sales Amount: $1,400.00
 */

/*
! This exercise generates a sales report of top-selling products by category by combining data across categories, products, and sales.

* 1.Grouping Products by Category:

GroupJoin(Products, category => category.Id, product => product.CategoryId, ...) groups products under their respective categories.

* 2.Calculating Sales Metrics:

TotalSales: Sales.Where(s => s.ProductId == product.Id).Sum(s => s.Quantity * product.Price) calculates the total sales revenue for each product.

TotalQuantitySold: Sales.Where(s => s.ProductId == product.Id).Sum(s => s.Quantity) calculates the total quantity sold for each product.

* 3.Identifying Top-Selling Products:

OrderByDescending(p => p.TotalSales).Take(3) selects the top 3 products by total sales within each category.

* 4.Returning the Report:

The result is a list of CategoryTopProductsSummary objects, each containing the top 3 products by sales in each category.
 
 */