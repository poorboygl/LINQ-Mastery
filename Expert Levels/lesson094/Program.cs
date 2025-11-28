using System.Collections.Concurrent;

public class Program
{
    static void Main()
    {
        var repo = new OrderRepository();
        var topProductsByMonth = repo.GetTopProductsByMonth_PLINQ();

        Console.WriteLine("=== Monthly Top Products ===");
        foreach (var summary in topProductsByMonth)
        {
            Console.WriteLine($"{summary.Month}: {summary.ProductName} (Total Sold: {summary.TotalQuantitySold})");
        }

        Console.ReadKey();
    }
}


public class Product
{
    public int Id { get; set; }
    public required string Name { get; set; }
}

public class Order
{
    public int ProductId { get; set; }
    public DateTime OrderDate { get; set; }
    public int Quantity { get; set; }
}

public class MonthlyTopProductSummary
{
    public required string Month { get; set; }
    public required string ProductName { get; set; }
    public int TotalQuantitySold { get; set; }
}

public class OrderRepository
{
    public List<Product> Products { get; set; } =
    [
        new Product { Id = 1, Name = "Laptop" },
        new Product { Id = 2, Name = "Mouse" },
        new Product { Id = 3, Name = "Keyboard" }
    ];

    public List<Order> Orders { get; set; } =
    [
        new Order { ProductId = 1, OrderDate = new DateTime(2024, 1, 10), Quantity = 15 },
        new Order { ProductId = 2, OrderDate = new DateTime(2024, 1, 15), Quantity = 10 },
        new Order { ProductId = 1, OrderDate = new DateTime(2024, 2, 20), Quantity = 20 },
        new Order { ProductId = 3, OrderDate = new DateTime(2024, 2, 25), Quantity = 30 }
    ];

    public List<MonthlyTopProductSummary> GetTopProductsByMonth()
    {
        var topProducts = Orders
            .GroupBy(order => new { order.OrderDate.Year, order.OrderDate.Month, order.ProductId })
            .Select(g => new
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                ProductId = g.Key.ProductId,
                TotalQuantitySold = g.Sum(order => order.Quantity)
            })
            .GroupBy(g => new { g.Year, g.Month })
            .Select(monthGroup => monthGroup
                .OrderByDescending(g => g.TotalQuantitySold)
                .First())
            .Join(Products,
                  monthProduct => monthProduct.ProductId,
                  product => product.Id,
                  (monthProduct, product) => new MonthlyTopProductSummary
                  {
                      Month = new DateTime(monthProduct.Year, monthProduct.Month, 1).ToString("MMMM yyyy"),
                      ProductName = product.Name,
                      TotalQuantitySold = monthProduct.TotalQuantitySold
                  })
            .OrderBy(summary => summary.Month)
            .ToList();

        return topProducts;
    }

    public List<MonthlyTopProductSummary> GetTopProductsByMonth_Dictionary()
    {
        // 1) Cache product lookup O(1)
        var productDict = Products.ToDictionary(p => p.Id, p => p.Name);

        // 2) Gom orders theo YearMonth + ProductId
        var salesDict = new Dictionary<(int Year, int Month, int ProductId), int>();

        foreach (var order in Orders)
        {
            var key = (order.OrderDate.Year, order.OrderDate.Month, order.ProductId);
            if (!salesDict.ContainsKey(key))
                salesDict[key] = 0;
            salesDict[key] += order.Quantity;
        }

        // 3) Gom theo YearMonth → tìm product bán chạy nhất mỗi tháng
        var topProductsPerMonth = new Dictionary<(int Year, int Month), (int ProductId, int TotalQuantity)>();

        foreach (var kvp in salesDict)
        {
            var ym = (kvp.Key.Year, kvp.Key.Month);
            var quantity = kvp.Value;
            var productId = kvp.Key.ProductId;

            if (!topProductsPerMonth.ContainsKey(ym))
            {
                topProductsPerMonth[ym] = (productId, quantity);
            }
            else
            {
                // nếu bằng thì giữ ProductId nhỏ hơn (giống LINQ First())
                if (quantity > topProductsPerMonth[ym].TotalQuantity ||
                   (quantity == topProductsPerMonth[ym].TotalQuantity &&
                    productId < topProductsPerMonth[ym].ProductId))
                {
                    topProductsPerMonth[ym] = (productId, quantity);
                }
            }
        }

        // 4) Chuyển sang danh sách MonthlyTopProductSummary
        var result = new List<MonthlyTopProductSummary>();

        foreach (var kvp in topProductsPerMonth)
        {
            var monthDate = new DateTime(kvp.Key.Year, kvp.Key.Month, 1);
            result.Add(new MonthlyTopProductSummary
            {
                Month = monthDate.ToString("MMMM yyyy"),
                ProductName = productDict[kvp.Value.ProductId],
                TotalQuantitySold = kvp.Value.TotalQuantity
            });
        }

        return result;
    }

    public List<MonthlyTopProductSummary> GetTopProductsByMonth_Parallel()
    {
        // 1) Cache product lookup O(1)
        var productDict = Products.ToDictionary(p => p.Id, p => p.Name);

        // 2) Gom orders theo (Year, Month, ProductId) thread-safe
        var salesDict = new ConcurrentDictionary<(int Year, int Month, int ProductId), int>();

        Parallel.ForEach(Orders, order =>
        {
            var key = (order.OrderDate.Year, order.OrderDate.Month, order.ProductId);
            salesDict.AddOrUpdate(key, order.Quantity, (k, old) => old + order.Quantity);
        });

        // 3) Tìm top product mỗi tháng
        var topProductsPerMonth = new ConcurrentDictionary<(int Year, int Month), (int ProductId, int TotalQuantity)>();

        foreach (var kvp in salesDict)
        {
            var ym = (kvp.Key.Year, kvp.Key.Month);
            var productId = kvp.Key.ProductId;
            var quantity = kvp.Value;

            topProductsPerMonth.AddOrUpdate(
                ym,
                (productId, quantity),
                (k, existing) =>
                {
                    // nếu quantity lớn hơn → cập nhật
                    // nếu bằng → giữ ProductId nhỏ hơn (giống LINQ First())
                    if (quantity > existing.TotalQuantity ||
                       (quantity == existing.TotalQuantity && productId < existing.ProductId))
                        return (productId, quantity);
                    return existing;
                });
        }

        // 4) Chuyển sang danh sách MonthlyTopProductSummary
        var result = new List<MonthlyTopProductSummary>();
        foreach (var kvp in topProductsPerMonth)
        {
            var monthDate = new DateTime(kvp.Key.Year, kvp.Key.Month, 1);
            result.Add(new MonthlyTopProductSummary
            {
                Month = monthDate.ToString("MMMM yyyy"),
                ProductName = productDict[kvp.Value.ProductId],
                TotalQuantitySold = kvp.Value.TotalQuantity
            });
        }

        // Không OrderBy → giữ thứ tự giống code gốc
        return result;
    }

    public List<MonthlyTopProductSummary> GetTopProductsByMonth_PLINQ()
    {
        // 1) Cache product lookup O(1)
        var productDict = Products.ToDictionary(p => p.Id, p => p.Name);

        // 2) Gom orders song song theo (Year, Month, ProductId)
        var salesDict = Orders
            .AsParallel()
            .GroupBy(order => (order.OrderDate.Year, order.OrderDate.Month, order.ProductId))
            .Select(g => new
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                ProductId = g.Key.ProductId,
                TotalQuantity = g.Sum(o => o.Quantity)
            })
            .ToList();

        // 3) Tìm top product mỗi tháng
        var topProductsPerMonth = salesDict
            .GroupBy(x => (x.Year, x.Month))
            .Select(g =>
                g.OrderByDescending(x => x.TotalQuantity)
                 .ThenBy(x => x.ProductId) // nếu bằng số lượng → ProductId nhỏ hơn
                 .First()
            )
            .ToList();

        // 4) Chuyển sang danh sách MonthlyTopProductSummary
        var result = topProductsPerMonth
            .Select(x => new MonthlyTopProductSummary
            {
                Month = new DateTime(x.Year, x.Month, 1).ToString("MMMM yyyy"),
                ProductName = productDict[x.ProductId],
                TotalQuantitySold = x.TotalQuantity
            })
            .ToList();

        // Không OrderBy cuối cùng → giữ thứ tự giống code gốc
        return result;
    }
}

/*
 !=== Monthly Top Products ===
    February 2024: Keyboard (Total Sold: 30)
    January 2024: Laptop (Total Sold: 15)
 */

/*
!This exercise identifies the most popular product each month based on sales.

* 1.Grouping by Month and Product:

GroupBy(order => new { order.OrderDate.Year, order.OrderDate.Month, order.ProductId }) groups orders by month and product.

* 2.Calculating Total Quantity Sold:

Sum(order => order.Quantity) calculates the total quantity sold for each product within each month.

* 3.Selecting Top Product per Month:

OrderByDescending(g => g.TotalQuantitySold).First() selects the top product for each month.

* 4.Returning the Summary:

The result is a list of MonthlyTopProductSummary objects, sorted by Month in ascending order.
 
 */