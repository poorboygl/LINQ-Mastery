class Program
{
    static void Main()
    {
        var repo = new SalesRepository();
        var topProducts = repo.GetTopSellingProductsByCategory_2();

        Console.WriteLine("=== TOP SELLING PRODUCTS BY CATEGORY ===\n");

        foreach (var summary in topProducts)
        {
            Console.WriteLine($"Category: {summary.CategoryName}");
            Console.WriteLine($"  Top Product: {summary.ProductName}");
            Console.WriteLine($"  Total Earnings: {summary.TotalEarnings:C}"); // C = định dạng tiền tệ
            Console.WriteLine();
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
}

public class Sale
{
    public int ProductId { get; set; }
    public decimal SaleAmount { get; set; }
}

public class TopProductSummary
{
    public required string CategoryName { get; set; }
    public required string ProductName { get; set; }
    public decimal TotalEarnings { get; set; }
}

public class SalesRepository
{
    public List<Category> Categories { get; set; } =
    [
        new Category { Id = 1, Name = "Electronics" },
        new Category { Id = 2, Name = "Books" },
        new Category { Id = 3, Name = "Clothing" }
    ];

    public List<Product> Products { get; set; } =
    [
        new Product { Id = 1, Name = "Laptop", CategoryId = 1 },
        new Product { Id = 2, Name = "Smartphone", CategoryId = 1 },
        new Product { Id = 3, Name = "Novel", CategoryId = 2 },
        new Product { Id = 4, Name = "Jeans", CategoryId = 3 }
    ];

    public List<Sale> Sales { get; set; } =
    [
        new Sale { ProductId = 1, SaleAmount = 1500m },
        new Sale { ProductId = 1, SaleAmount = 2000m },
        new Sale { ProductId = 2, SaleAmount = 500m },
        new Sale { ProductId = 3, SaleAmount = 300m },
        new Sale { ProductId = 3, SaleAmount = 700m },
        new Sale { ProductId = 4, SaleAmount = 1200m }
    ];

    public List<TopProductSummary> GetTopSellingProductsByCategory()
    {
        /*
         !=== TOP SELLING PRODUCTS BY CATEGORY ===

             Category: Books
               Top Product: Novel
               Total Earnings: $1,000.00

             Category: Clothing
               Top Product: Jeans
               Total Earnings: $1,200.00

             Category: Electronics
               Top Product: Laptop
               Total Earnings: $3,500.00
         */
        var result = Categories
                    .GroupJoin(Products,
                    c => c.Id,
                    p => p.CategoryId,
                    (category, categoryProducts) =>
                    {
                        var categoryName = category.Name;

                        // Lấy danh sách product Id thuộc category hiện tại
                        var productIds = categoryProducts.Select(p => p.Id).ToList();

                        var TopProduct = Sales
                                        .Where(s => productIds.Contains(s.ProductId))
                                        .GroupBy(s => s.ProductId)
                                        .Select(group => new
                                        {
                                            nameProduct = Products.First(p => p.Id == group.Key).Name,
                                            totalEarnings = group.Sum(s => s.SaleAmount)
                                        })
                                        .OrderByDescending(l => l.totalEarnings)
                                        .First();

                        return new TopProductSummary
                        {
                            CategoryName = categoryName,
                            ProductName = TopProduct.nameProduct,
                            TotalEarnings = TopProduct.totalEarnings,
                        };

                    })
                    .OrderBy(summary => summary.CategoryName)
                    .ToList();
        return result;

    }

    public List<TopProductSummary> GetTopSellingProductsByCategory_2()
    {
        /*
        !=== TOP SELLING PRODUCTS BY CATEGORY ===

            Category: Books
              Top Product: Novel
              Total Earnings: $1,000.00

            Category: Clothing
              Top Product: Jeans
              Total Earnings: $1,200.00

            Category: Electronics
              Top Product: Laptop
              Total Earnings: $3,500.00
        */
        var topProducts = Products
            .GroupBy(product => product.CategoryId)
            .Select(group =>
            {
                var topProduct = group
                    .Join(Sales,
                          product => product.Id,
                          sale => sale.ProductId,
                          (product, sale) => new
                          {
                              product.Name,
                              CategoryId = product.CategoryId,
                              SaleAmount = sale.SaleAmount
                          })
                    .GroupBy(p => p.Name)
                    .Select(p => new
                    {
                        ProductName = p.Key,
                        TotalEarnings = p.Sum(x => x.SaleAmount)
                    })
                    .OrderByDescending(p => p.TotalEarnings)
                    .FirstOrDefault();

                var categoryName = Categories.First(c => c.Id == group.Key).Name;

                return new TopProductSummary
                {
                    CategoryName = categoryName,
                    ProductName = topProduct.ProductName,
                    TotalEarnings = topProduct.TotalEarnings
                };
            })
            .OrderBy(summary => summary.CategoryName)
            .ToList();

        return topProducts;
    }
}


/*
!This exercise identifies the highest-earning product in each category based on total sales.

* 1.Grouping by Category:

GroupBy(product => product.CategoryId) groups products by their category.

* 2.Calculating Total Earnings per Product:

For each product, Join with Sales and GroupBy to calculate total earnings by summing SaleAmount.

* 3.Selecting the Top Product per Category:

OrderByDescending(p => p.TotalEarnings).FirstOrDefault() selects the product with the highest earnings for each category.

* 4.Returning the Summary:

The result is a list of TopProductSummary objects, sorted alphabetically by CategoryName.
 
 */