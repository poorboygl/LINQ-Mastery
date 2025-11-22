using System.Linq;

class Program
{
    static void Main()
    {
        var repo = new SalesAnalysisRepository();
        var summary = repo.GetMonthlyTopProduct_WithDictionary();

        Console.WriteLine("=== HIGHEST SALES MONTH SUMMARY ===\n");

        if (summary != null)
        {
            Console.WriteLine($"Month: {summary.Month}");
            Console.WriteLine($"Total Sales: {summary.TotalSales:C}");
            Console.WriteLine($"Top Product: {summary.TopProduct}");
        }
        else
        {
            Console.WriteLine("No sales data found.");
        }

        Console.ReadKey();
    }
}


public class Product
{
    public int Id { get; set; }
    public required string Name { get; set; }
}

public class SalesRecord
{
    public int ProductId { get; set; }
    public decimal SaleAmount { get; set; }
    public DateTime SaleDate { get; set; }
}

public class MonthlySalesSummary
{
    public required string Month { get; set; }
    public decimal TotalSales { get; set; }
    public required string TopProduct { get; set; }
}

public class SalesAnalysisRepository
{
    public List<Product> Products { get; set; } =
    [
        new Product { Id = 1, Name = "Laptop" },
        new Product { Id = 2, Name = "Smartphone" },
        new Product { Id = 3, Name = "Headphones" }
    ];

    public List<SalesRecord> SalesRecords { get; set; } =
    [
        new SalesRecord { ProductId = 1, SaleAmount = 1500m, SaleDate = new DateTime(2024, 1, 15) },
        new SalesRecord { ProductId = 2, SaleAmount = 800m, SaleDate = new DateTime(2024, 1, 25) },
        new SalesRecord { ProductId = 1, SaleAmount = 2000m, SaleDate = new DateTime(2024, 2, 5) },
        new SalesRecord { ProductId = 3, SaleAmount = 600m, SaleDate = new DateTime(2024, 2, 12) },
        new SalesRecord { ProductId = 2, SaleAmount = 700m, SaleDate = new DateTime(2024, 3, 3) }
    ];

    public MonthlySalesSummary GetHighestSalesMonth()
    {
        /*
          !=== HIGHEST SALES MONTH SUMMARY ===
            Month: February 2024
            Total Sales: $2,600.00
            Top Product: Laptop
         */

        var result = SalesRecords
                      .GroupBy(record => new { record.ProductId, Month = new DateTime(record.SaleDate.Year, record.SaleDate.Month, 1) })
                      .Select(group =>
                      {
                          var month = new DateTime(group.Key.Month.Year, group.Key.Month.Month, 1).ToString("MMMM yyyy");
                          var TopProduct = group.GroupBy(s => s.ProductId)
                                         .Select(records => new
                                         {
                                             NameProduct = Products.First(p => p.Id == records.Key).Name,
                                             TotalAmount = records.Sum(record => record.SaleAmount)
                                         })
                                         .OrderByDescending(r => r.TotalAmount)
                                         .FirstOrDefault();

                          return new MonthlySalesSummary
                          {
                              Month = month,
                              TopProduct = TopProduct?.NameProduct ?? "No products in Month",
                              TotalSales = TopProduct?.TotalAmount ?? 0,
                          };

                      })
                      .OrderByDescending(r => r.TotalSales)
                      .FirstOrDefault();
        return result;
    }

    public MonthlySalesSummary GetMonthlyTopProduct_WithDictionary()
    {

        /*
          !=== HIGHEST SALES MONTH SUMMARY ===
            Month: February 2024
            Total Sales: $2,600.00
            Top Product: Laptop
        */

        //Tạo dictionary để lookup Product Name nhanh (O(1))
        var productLookup = Products.ToDictionary(p => p.Id, p => p.Name);

        //Group sales theo tháng
        var monthlySummary = SalesRecords
            .GroupBy(r => new DateTime(r.SaleDate.Year, r.SaleDate.Month, 1))
            .Select(monthGroup =>
            {
                var month = monthGroup.Key.ToString("MMMM yyyy");

                //Tính tổng doanh thu cả tháng
                var totalSales = monthGroup.Sum(r => r.SaleAmount);

                //Tìm sản phẩm bán chạy nhất trong tháng
                var topProduct = monthGroup
                    .GroupBy(r => r.ProductId)
                    .Select(pg => new
                    {
                        ProductId = pg.Key,
                        TotalAmount = pg.Sum(r => r.SaleAmount)
                    })
                    .OrderByDescending(x => x.TotalAmount)
                    .FirstOrDefault();

                //Lấy tên product từ dictionary (an toàn)
                var productName = topProduct != null && productLookup.ContainsKey(topProduct.ProductId)
                    ? productLookup[topProduct.ProductId]
                    : "(No products)";

                return new MonthlySalesSummary
                {
                    Month = month,
                    TopProduct = productName,
                    TotalSales = totalSales
                };
            })
            .OrderByDescending(m => m.TotalSales) // chọn tháng có tổng doanh thu cao nhất
            .FirstOrDefault();

        return monthlySummary;
    }


    public MonthlySalesSummary GetHighestSalesMonth_AuthorWriting()
    {

        /*
             !=== HIGHEST SALES MONTH SUMMARY ===

            Month: February 2024
            Total Sales: $2,600.00
            Top Product: Laptop

        */
        var monthlySales = SalesRecords
            .GroupBy(record => new { record.SaleDate.Year, record.SaleDate.Month })
            .Select(group => new
            {
                Month = new DateTime(group.Key.Year, group.Key.Month, 1).ToString("MMMM yyyy"),
                TotalSales = group.Sum(record => record.SaleAmount),
                TopProduct = group
                    .GroupBy(record => record.ProductId)
                    .Select(productGroup => new
                    {
                        ProductId = productGroup.Key,
                        ProductSales = productGroup.Sum(record => record.SaleAmount)
                    })
                    .OrderByDescending(product => product.ProductSales)
                    .FirstOrDefault()
            })
            .OrderByDescending(month => month.TotalSales)
            .FirstOrDefault();

        if (monthlySales == null) return null;

        var topProductName = Products.First(p => p.Id == monthlySales.TopProduct.ProductId).Name;

        return new MonthlySalesSummary
        {
            Month = monthlySales.Month,
            TotalSales = monthlySales.TotalSales,
            TopProduct = topProductName
        };
    }
}


/*
 !This exercise identifies the month with the highest total sales and the product that contributed the most to that month’s sales.

* 1.Grouping by Month:

GroupBy(record => new { record.SaleDate.Year, record.SaleDate.Month }) groups sales by month.

* 2.Calculating Monthly Total Sales and Top Product:

For each month, Sum calculates total sales. GroupBy(record => record.ProductId) and OrderByDescending determine the top product.

* 3.Selecting the Month with Highest Sales:

The highest sales month is identified with OrderByDescending(month => month.TotalSales).FirstOrDefault().

* 4.Returning the Summary:

A MonthlySalesSummary object is created with the month’s name, total sales, and the top product’s name.
 
*/