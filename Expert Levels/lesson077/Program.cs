class Program
{
    static void Main()
    {
        var repo = new SalesRepository();

        var result = repo.GetTopProductByRegionAndMonth();

        Console.WriteLine("=== REGION MONTHLY TOP PRODUCTS ===");
        foreach (var item in result)
        {
            Console.WriteLine(
                $"{item.RegionName} | {item.Month} | Top Product: {item.ProductName} | Total Sold: {item.TotalQuantitySold}");
        }

        Console.ReadKey();
    }
}
public class Region
{
    public int Id { get; set; }
    public required string Name { get; set; }
}

public class Product
{
    public int Id { get; set; }
    public required string Name { get; set; }
}

public class SalesRecord
{
    public int ProductId { get; set; }
    public int RegionId { get; set; }
    public DateTime SaleDate { get; set; }
    public int QuantitySold { get; set; }
}

public class RegionMonthlyTopProductSummary
{
    public required string RegionName { get; set; }
    public required string Month { get; set; }
    public required string ProductName { get; set; }
    public int TotalQuantitySold { get; set; }
}

public class SalesRepository
{
    public List<Region> Regions { get; set; } =
    [
        new Region { Id = 1, Name = "West" },
        new Region { Id = 2, Name = "East" }
    ];

    public List<Product> Products { get; set; } =
    [
        new Product { Id = 1, Name = "Laptop" },
        new Product { Id = 2, Name = "Tablet" }
    ];

    public List<SalesRecord> SalesRecords { get; set; } =
    [
        new SalesRecord { ProductId = 1, RegionId = 1, SaleDate = new DateTime(2023, 1, 10), QuantitySold = 300 },
        new SalesRecord { ProductId = 1, RegionId = 1, SaleDate = new DateTime(2023, 1, 20), QuantitySold = 200 },
        new SalesRecord { ProductId = 2, RegionId = 1, SaleDate = new DateTime(2023, 1, 15), QuantitySold = 250 },
        new SalesRecord { ProductId = 1, RegionId = 2, SaleDate = new DateTime(2023, 1, 25), QuantitySold = 400 },
        new SalesRecord { ProductId = 2, RegionId = 2, SaleDate = new DateTime(2023, 2, 5), QuantitySold = 300 }
    ];

    public List<RegionMonthlyTopProductSummary> GetTopProductByRegionAndMonth()
    {
        /*
             !=== REGION MONTHLY TOP PRODUCTS ===
            East | January 2023 | Top Product: Laptop | Total Sold: 400
            East | February 2023 | Top Product: Tablet | Total Sold: 300
            West | January 2023 | Top Product: Laptop | Total Sold: 500
         */
        var RegionLookUp = Regions.ToDictionary(r => r.Id);
        var productLookUp = Products.ToDictionary(p => p.Id);

        var result = SalesRecords
                    .GroupBy(r => new { r.RegionId, Month = new DateTime(r.SaleDate.Year, r.SaleDate.Month, 1) })
                    .Select(group =>
                    {
                        var regionName = RegionLookUp[group.Key.RegionId].Name;
                        var month = group.Key.Month;
                        var record = group.GroupBy(r => r.ProductId)
                                            .Select(g => new
                                            {
                                                productName = productLookUp[g.Key].Name,
                                                TotalQuantity = g.Sum(r => r.QuantitySold)
                                            })
                                            .OrderByDescending(l => l.TotalQuantity)
                                            .FirstOrDefault();
                        return new RegionMonthlyTopProductSummary
                        {
                            RegionName = regionName,
                            Month = month.ToString("MMMM yyyy"),
                            ProductName = record!.productName,
                            TotalQuantitySold = record!.TotalQuantity,
                        };
                    })
                    .OrderBy(summary => summary.RegionName)
                    .ThenBy(summary => DateTime.ParseExact(summary.Month, "MMMM yyyy", null).Year)
                    .ThenBy(summary => DateTime.ParseExact(summary.Month, "MMMM yyyy", null).Month)
                    .ToList();
        return result;
    }

    public List<RegionMonthlyTopProductSummary> GetTopProductByRegionAndMonth_AuthorWriting()
    {
        return SalesRecords
            .GroupBy(record => new { record.RegionId, Month = new DateTime(record.SaleDate.Year, record.SaleDate.Month, 1) })
            .Select(g => new
            {
                RegionId = g.Key.RegionId,
                Month = g.Key.Month,
                TopProduct = g
                    .GroupBy(record => record.ProductId)
                    .Select(productGroup => new
                    {
                        ProductId = productGroup.Key,
                        TotalQuantitySold = productGroup.Sum(r => r.QuantitySold)
                    })
                    .OrderByDescending(p => p.TotalQuantitySold)
                    .First()
            })
            .Select(summary => new RegionMonthlyTopProductSummary
            {
                RegionName = Regions.First(r => r.Id == summary.RegionId).Name,
                Month = summary.Month.ToString("MMMM yyyy"),
                ProductName = Products.First(p => p.Id == summary.TopProduct.ProductId).Name,
                TotalQuantitySold = summary.TopProduct.TotalQuantitySold
            })
            .OrderBy(summary => summary.RegionName)
            .ThenBy(summary => DateTime.ParseExact(summary.Month, "MMMM yyyy", null))
            .ToList();
    }
}

/*
 !This exercise generates a report showing the best-selling product for each region and month.

    * 1.Grouping Sales by Region and Month:

    GroupBy(record => new { record.RegionId, Month = new DateTime(record.SaleDate.Year, record.SaleDate.Month, 1) }) groups sales data by region and month.

    * 2.Calculating Total Quantity Sold by Product:

    TotalQuantitySold: A nested GroupBy by ProductId within each region and month group, followed by Sum(r => r.QuantitySold), calculates the total quantity sold for each product.

    * 3.Identifying Top Product by Quantity Sold:

    OrderByDescending(p => p.TotalQuantitySold).First() selects the product with the highest total quantity sold for each region and month.

    * 4.Returning the Report:

    The result is a list of RegionMonthlyTopProductSummary objects, showing the top-selling product for each region and month.
 */