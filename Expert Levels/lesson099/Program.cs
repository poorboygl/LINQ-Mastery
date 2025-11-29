using System.Collections.Concurrent;

public class Program
{
    static void Main()
    {
        var repo = new OrderRepository();
        var topCoPurchased = repo.GetFrequentCoPurchasedProducts_Parallel();

        Console.WriteLine("=== Top 5 Co-Purchased Product Pairs ===");
        foreach (var item in topCoPurchased)
        {
            Console.WriteLine($"Product {item.Product1Id} & Product {item.Product2Id} - CoPurchaseCount: {item.CoPurchaseCount}");
        }

        Console.ReadKey();
    }
}

public class Customer
{
    public int Id { get; set; }
    public required string Name { get; set; }
}

public class Order
{
    public int CustomerId { get; set; }
    public required List<int> ProductIds { get; set; }  // List of products purchased in the same order
}

public class CoPurchasedProductSummary
{
    public int Product1Id { get; set; }
    public int Product2Id { get; set; }
    public int CoPurchaseCount { get; set; }
}

public class OrderRepository
{
    public List<Customer> Customers { get; set; } =
    [
        new Customer { Id = 1, Name = "Alice" },
        new Customer { Id = 2, Name = "Bob" },
        new Customer { Id = 3, Name = "Charlie" }
    ];

    public List<Order> Orders { get; set; } =
    [
        new Order { CustomerId = 1, ProductIds = [1, 2, 3] },
        new Order { CustomerId = 1, ProductIds = [2, 3] },
        new Order { CustomerId = 2, ProductIds = [1, 3] },
        new Order { CustomerId = 3, ProductIds = [1, 2] },
        new Order { CustomerId = 3, ProductIds = [2, 3, 4] }
    ];

    public List<CoPurchasedProductSummary> GetFrequentCoPurchasedProducts()
    {
        var coPurchasedProducts = Orders
            .SelectMany(order => order.ProductIds
                .SelectMany((productId, index) =>
                    order.ProductIds.Skip(index + 1).Select(otherProductId =>
                        new { Product1Id = Math.Min(productId, otherProductId), Product2Id = Math.Max(productId, otherProductId) })
                )
            )
            .GroupBy(pair => new { pair.Product1Id, pair.Product2Id })
            .Select(g => new CoPurchasedProductSummary
            {
                Product1Id = g.Key.Product1Id,
                Product2Id = g.Key.Product2Id,
                CoPurchaseCount = g.Count()
            })
            .OrderByDescending(summary => summary.CoPurchaseCount)
            .Take(5)
            .ToList();

        return coPurchasedProducts;
    }

    public List<CoPurchasedProductSummary> GetFrequentCoPurchasedProducts_Dictionary()
    {
        var coPurchaseDict = new Dictionary<(int, int), int>();

        foreach (var order in Orders)
        {
            var products = order.ProductIds;

            for (int i = 0; i < products.Count; i++)
            {
                for (int j = i + 1; j < products.Count; j++)
                {
                    int p1 = Math.Min(products[i], products[j]);
                    int p2 = Math.Max(products[i], products[j]);
                    var key = (p1, p2);

                    if (coPurchaseDict.ContainsKey(key))
                        coPurchaseDict[key]++;
                    else
                        coPurchaseDict[key] = 1;
                }
            }
        }

        // Chọn top 5
        var topCoPurchased = coPurchaseDict
            .OrderByDescending(kvp => kvp.Value)
            .Take(5)
            .Select(kvp => new CoPurchasedProductSummary
            {
                Product1Id = kvp.Key.Item1,
                Product2Id = kvp.Key.Item2,
                CoPurchaseCount = kvp.Value
            })
            .ToList();

        return topCoPurchased;
    }

    public List<CoPurchasedProductSummary> GetFrequentCoPurchasedProducts_PLINQ()
    {
        var coPurchasedProducts = Orders
            .AsParallel()
            .SelectMany(order =>
                order.ProductIds
                     .SelectMany((productId, index) =>
                         order.ProductIds.Skip(index + 1)
                         .Select(otherProductId => new
                         {
                             Product1Id = Math.Min(productId, otherProductId),
                             Product2Id = Math.Max(productId, otherProductId)
                         })
                     )
            )
            .GroupBy(pair => new { pair.Product1Id, pair.Product2Id })
            .Select(g => new CoPurchasedProductSummary
            {
                Product1Id = g.Key.Product1Id,
                Product2Id = g.Key.Product2Id,
                CoPurchaseCount = g.Count()
            })
            .OrderByDescending(summary => summary.CoPurchaseCount)
            .Take(5)
            .ToList();

        return coPurchasedProducts;
    }

    public List<CoPurchasedProductSummary> GetFrequentCoPurchasedProducts_Parallel()
    {
        // Dùng ConcurrentDictionary để thread-safe
        var coPurchaseDict = new ConcurrentDictionary<(int, int), int>();

        Parallel.ForEach(Orders, order =>
        {
            var products = order.ProductIds;

            for (int i = 0; i < products.Count; i++)
            {
                for (int j = i + 1; j < products.Count; j++)
                {
                    int p1 = Math.Min(products[i], products[j]);
                    int p2 = Math.Max(products[i], products[j]);
                    var key = (p1, p2);

                    coPurchaseDict.AddOrUpdate(key, 1, (k, oldVal) => oldVal + 1);
                }
            }
        });

        // Chọn top 5
        var topCoPurchased = coPurchaseDict
            .OrderByDescending(kvp => kvp.Value)
            .Take(5)
            .Select(kvp => new CoPurchasedProductSummary
            {
                Product1Id = kvp.Key.Item1,
                Product2Id = kvp.Key.Item2,
                CoPurchaseCount = kvp.Value
            })
            .ToList();

        return topCoPurchased;
    }
}

/*
 !=== Top 5 Co-Purchased Product Pairs ===
Product 2 & Product 3 - CoPurchaseCount: 3
Product 1 & Product 2 - CoPurchaseCount: 2
Product 1 & Product 3 - CoPurchaseCount: 2
Product 2 & Product 4 - CoPurchaseCount: 1
Product 3 & Product 4 - CoPurchaseCount: 1
 
 */

/*
This exercise identifies frequently co-purchased product pairs based on customer order data.
* 1.Generating Product Pairs:

SelectMany is used to generate all unique pairs of products for each order.

* 2.Grouping by Product Pair:

GroupBy(pair => new { pair.Product1Id, pair.Product2Id }) groups co-purchased products by unique pairs.

* 3.Counting Co-Purchases:

Count() calculates how often each product pair is purchased together.

* 4.Selecting Top Co-Purchased Product Pairs:

OrderByDescending(summary => summary.CoPurchaseCount).Take(5) selects the top 5 most frequently co-purchased product pairs.

* 5.Returning the Summary:

The result is a list of CoPurchasedProductSummary objects, sorted by CoPurchaseCount in descending order.
 
 */