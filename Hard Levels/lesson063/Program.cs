class Program
{
    static void Main()
    {
        var repo = new SalesRepository();
        var topProducts = repo.GetTopSellingProductByMonth_2();

        Console.WriteLine("=== MONTHLY TOP SELLING PRODUCT REPORT ===\n");

        foreach (var summary in topProducts)
        {
            Console.WriteLine($"Month: {summary.Month}");
            Console.WriteLine($"  Top Product: {summary.ProductName}");
            Console.WriteLine($"  Total Quantity Sold: {summary.TotalQuantitySold}");
            Console.WriteLine(new string('-', 40));
        }

        Console.ReadKey();
    }
}

public class Product
{
    public int Id { get; set; }
    public required string Name { get; set; }
}

public class Sale
{
    public int ProductId { get; set; }
    public DateTime SaleDate { get; set; }
    public int QuantitySold { get; set; }
}

public class MonthlyTopProductSummary
{
    public required string Month { get; set; }
    public required string ProductName { get; set; }
    public int TotalQuantitySold { get; set; }
}

public class SalesRepository
{
    public List<Product> Products { get; set; } =
    [
        new Product { Id = 1, Name = "Laptop" },
        new Product { Id = 2, Name = "Tablet" },
        new Product { Id = 3, Name = "Smartphone" }
    ];

    public List<Sale> Sales { get; set; } =
    [
        new Sale { ProductId = 1, SaleDate = new DateTime(2023, 1, 15), QuantitySold = 200 },
        new Sale { ProductId = 2, SaleDate = new DateTime(2023, 1, 20), QuantitySold = 150 },
        new Sale { ProductId = 1, SaleDate = new DateTime(2023, 2, 10), QuantitySold = 300 },
        new Sale { ProductId = 3, SaleDate = new DateTime(2023, 2, 25), QuantitySold = 250 },
        new Sale { ProductId = 2, SaleDate = new DateTime(2023, 3, 5), QuantitySold = 400 }
    ];

    public List<MonthlyTopProductSummary> GetTopSellingProductByMonth()
    {
        /*
         !=== MONTHLY TOP SELLING PRODUCT REPORT ===

            Month: January 2023
              Top Product: Laptop
              Total Quantity Sold: 200
            ----------------------------------------
            Month: February 2023
              Top Product: Laptop
              Total Quantity Sold: 300
            ----------------------------------------
            Month: March 2023
              Top Product: Tablet
              Total Quantity Sold: 400
            ----------------------------------------         
         */

        var result = Sales
                    .GroupBy(s => new { s.SaleDate.Year, s.SaleDate.Month })
                    .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                    .Select(group =>
                    {
                        var month = new DateTime(group.Key.Year, group.Key.Month, 1).ToString("MMMM yyyy");
                        var productGroup = group
                                        .GroupBy(s => s.ProductId)
                                        .Select(productGroup => new
                                        {
                                            NameProduct = Products.First(product => product.Id == productGroup.Key).Name,
                                            TotalQuan = productGroup.Sum(s => s.QuantitySold)
                                        })
                                        .OrderByDescending(r => r.TotalQuan)
                                        .First();
                        var name = productGroup;
                        return new MonthlyTopProductSummary
                        {
                            Month = month,
                            ProductName = productGroup.NameProduct,
                            TotalQuantitySold = productGroup.TotalQuan
                        };
                    }).ToList();

        return result;
    }

    public List<MonthlyTopProductSummary> GetTopSellingProductByMonth_2()
    {
        /*
         !=== MONTHLY TOP SELLING PRODUCT REPORT ===

            Month: January 2023
              Top Product: Laptop
              Total Quantity Sold: 200
            ----------------------------------------
            Month: February 2023
              Top Product: Laptop
              Total Quantity Sold: 300
            ----------------------------------------
            Month: March 2023
              Top Product: Tablet
              Total Quantity Sold: 400
            ----------------------------------------         
         */

        return Sales
                .GroupBy(sale => new { sale.SaleDate.Year, sale.SaleDate.Month })
                .Select(group => new MonthlyTopProductSummary
                {
                    Month = new DateTime(group.Key.Year, group.Key.Month, 1).ToString("MMMM yyyy"),
                    ProductName = Products
                        .First(product => product.Id == group
                            .OrderByDescending(sale => sale.QuantitySold)
                            .First().ProductId)
                        .Name,
                    TotalQuantitySold = group.Max(sale => sale.QuantitySold)
                })
                .OrderBy(summary => DateTime.ParseExact(summary.Month, "MMMM yyyy", null))
                .ToList();
    }

    public Dictionary<string, MonthlyTopProductSummary> GetTopSellingProductByMonth_3()
    {
        var result = Sales
            .GroupBy(s => new { s.SaleDate.Year, s.SaleDate.Month })
            .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
            .ToDictionary(
                group => new DateTime(group.Key.Year, group.Key.Month, 1).ToString("MMMM yyyy"),
                group =>
                {
                    var topProduct = group
                        .GroupBy(s => s.ProductId)
                        .Select(pg => new
                        {
                            NameProduct = Products.First(p => p.Id == pg.Key).Name,
                            TotalQuan = pg.Sum(s => s.QuantitySold)
                        })
                        .OrderByDescending(p => p.TotalQuan)
                        .First();

                    return new MonthlyTopProductSummary
                    {
                        Month = new DateTime(group.Key.Year, group.Key.Month, 1).ToString("MMMM yyyy"),
                        ProductName = topProduct.NameProduct,
                        TotalQuantitySold = topProduct.TotalQuan
                    };
                });

        return result;
    }
}
/*
 !This exercise generates a report showing the best-selling product for each month.

* 1.Grouping Sales by Month:

Sales.GroupBy(sale => new { sale.SaleDate.Year, sale.SaleDate.Month }) groups sales data by month and year.

* 2.Calculating Top Product by Quantity Sold:

Max(sale => sale.QuantitySold) finds the maximum quantity sold for each product within a month.

OrderByDescending(sale => sale.QuantitySold).First() selects the product with the highest quantity sold.

* 3.Returning the Report:

The result is a list of MonthlyTopProductSummary objects, showing the top-selling product by quantity sold for each month.
 
 */
