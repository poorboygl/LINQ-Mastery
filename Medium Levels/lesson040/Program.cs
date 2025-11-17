class Program
{
    static void Main()
    {
        var repo = new SalesRepository();
        var reports = repo.GetSalesReport();

        Console.WriteLine("=== SALES REPORT ===");

        foreach (var report in reports)
        {
            Console.WriteLine($"Product: {report.ProductName}");
            Console.WriteLine($"  Total Quantity Sold: {report.TotalQuantitySold}");
            Console.WriteLine($"  Total Sales Amount: {report.TotalSalesAmount:C}");
            Console.WriteLine();
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

public class SalesRecord
{
    public int ProductId { get; set; }
    public int QuantitySold { get; set; }
}

public class SalesReport
{
    public required string ProductName { get; set; }
    public int TotalQuantitySold { get; set; }
    public decimal TotalSalesAmount { get; set; }
}

public class SalesRepository
{
    public List<Product> Products { get; set; } =
    [
        new Product { Id = 1, Name = "Laptop", Price = 1000.00m },
        new Product { Id = 2, Name = "Mouse", Price = 25.00m },
        new Product { Id = 3, Name = "Keyboard", Price = 45.00m }
    ];

    public List<SalesRecord> SalesRecords { get; set; } =
    [
        new SalesRecord { ProductId = 1, QuantitySold = 5 },
        new SalesRecord { ProductId = 1, QuantitySold = 3 },
        new SalesRecord { ProductId = 2, QuantitySold = 10 },
        new SalesRecord { ProductId = 3, QuantitySold = 4 }
    ];

    public List<SalesReport> GetSalesReport()
    {
        return [.. SalesRecords
            .Join(Products, sale => sale.ProductId, product => product.Id, (sale, product) => new
            {
                product.Name,
                product.Price,
                sale.QuantitySold,
                TotalSaleAmount = sale.QuantitySold * product.Price
            })
            .GroupBy(item => item.Name)
            .Select(g => new SalesReport
            {
                ProductName = g.Key, // key chính là name. do groupby trả ra IGrouping<string, TElement>
                TotalQuantitySold = g.Sum(x => x.QuantitySold),
                TotalSalesAmount = g.Sum(x => x.TotalSaleAmount)
            })];
    }

}

/*
 === SALES REPORT ===
Product: Laptop
  Total Quantity Sold: 8
  Total Sales Amount: $8,000.00

Product: Mouse
  Total Quantity Sold: 10
  Total Sales Amount: $250.00

Product: Keyboard
  Total Quantity Sold: 4
  Total Sales Amount: $180.00
*/

/*
Joining SalesRecords and Products:

* 1.Join(Products, sale => sale.ProductId, product => product.Id, ...) connects each sale to the corresponding product, creating an intermediate object with Name, Price, QuantitySold, and TotalSaleAmount (calculated as QuantitySold * Price).

* 2.Grouping by Product Name:

GroupBy(item => item.Name) consolidates the sales data by product name, making it easy to aggregate totals for each product.

* 3.Calculating Total Quantity and Sales Amount:

Sum(x => x.QuantitySold) and Sum(x => x.TotalSaleAmount) within each group compute the total quantity sold and total sales amount, respectively, for each product.

* 4.Returning the Result:

The final result is a list of anonymous objects, each containing ProductName, TotalQuantitySold, and TotalSalesAmount.
 
 
 */
