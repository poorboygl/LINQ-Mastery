using System.Collections.Concurrent;

public class Program
{
    static void Main()
    {
        var repo = new InventoryRepository();

        int stockThreshold = 10;

        var items = repo.GetItemsToReorder_Parallel(stockThreshold);

        Console.WriteLine("=== Items to Reorder ===");
        Console.WriteLine($"Stock Threshold: {stockThreshold}");
        Console.WriteLine();

        foreach (var item in items)
        {
            Console.WriteLine($"Item: {item.ItemName}");
            Console.WriteLine($"  Stock: {item.Stock}");
            Console.WriteLine($"  Total Sold: {item.TotalQuantitySold}");
            Console.WriteLine($"  Supplier: {item.SupplierName}");
            Console.WriteLine();
        }

        Console.ReadKey();
    }
}

public class InventoryItem
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int Stock { get; set; }
}

public class SalesRecord
{
    public int ItemId { get; set; }
    public int QuantitySold { get; set; }
}

public class Supplier
{
    public int ItemId { get; set; }
    public required string SupplierName { get; set; }
}

public class ReorderItemSummary
{
    public required string ItemName { get; set; }
    public int Stock { get; set; }
    public int TotalQuantitySold { get; set; }
    public required string SupplierName { get; set; }
}

public class InventoryRepository
{
    public List<InventoryItem> InventoryItems { get; set; } =
    [
        new InventoryItem { Id = 1, Name = "Laptop", Stock = 5 },
        new InventoryItem { Id = 2, Name = "Mouse", Stock = 15 },
        new InventoryItem { Id = 3, Name = "Keyboard", Stock = 8 },
        new InventoryItem { Id = 4, Name = "Monitor", Stock = 3 }
    ];

    public List<SalesRecord> SalesRecords { get; set; } =
    [
        new SalesRecord { ItemId = 1, QuantitySold = 100 },
        new SalesRecord { ItemId = 2, QuantitySold = 50 },
        new SalesRecord { ItemId = 3, QuantitySold = 80 },
        new SalesRecord { ItemId = 4, QuantitySold = 120 }
    ];

    public List<Supplier> Suppliers { get; set; } =
    [
        new Supplier { ItemId = 1, SupplierName = "TechSupplier Inc." },
        new Supplier { ItemId = 2, SupplierName = "AccessoryWorld" },
        new Supplier { ItemId = 3, SupplierName = "KeyboardsRUs" },
        new Supplier { ItemId = 4, SupplierName = "MonitorMart" }
    ];

    public List<ReorderItemSummary> GetItemsToReorder(int stockThreshold)
    {
        var reorderItems = InventoryItems
            .Where(item => item.Stock < stockThreshold)
            .Join(SalesRecords.GroupBy(sr => sr.ItemId)
                              .Select(group => new
                              {
                                  ItemId = group.Key,
                                  TotalQuantitySold = group.Sum(sr => sr.QuantitySold)
                              }),
                  item => item.Id,
                  sales => sales.ItemId,
                  (item, sales) => new
                  {
                      ItemName = item.Name,
                      Stock = item.Stock,
                      TotalQuantitySold = sales.TotalQuantitySold,
                      ItemId = item.Id
                  })
            .OrderByDescending(item => item.TotalQuantitySold)
            .Take(5)
            .Join(Suppliers,
                  item => item.ItemId,
                  supplier => supplier.ItemId,
                  (item, supplier) => new ReorderItemSummary
                  {
                      ItemName = item.ItemName,
                      Stock = item.Stock,
                      TotalQuantitySold = item.TotalQuantitySold,
                      SupplierName = supplier.SupplierName
                  })
            .ToList();

        return reorderItems;
    }

    public List<ReorderItemSummary> GetItemsToReorder_Optimized(int stockThreshold)
    {
        // Dictionary Items lookup
        var itemDict = InventoryItems.ToDictionary(i => i.Id);

        // Dictionary Suppliers lookup
        var supplierDict = Suppliers.ToDictionary(s => s.ItemId, s => s.SupplierName);

        // Dictionary để tính tổng quantity sold cho từng item
        var salesDict = new Dictionary<int, int>();

        foreach (var sale in SalesRecords)
        {
            if (!salesDict.ContainsKey(sale.ItemId))
                salesDict[sale.ItemId] = sale.QuantitySold;
            else
                salesDict[sale.ItemId] += sale.QuantitySold;
        }

        // Tạo danh sách item cần reorder (stock < threshold)
        var reorderList = new List<ReorderItemSummary>();

        foreach (var item in InventoryItems)
        {
            if (item.Stock >= stockThreshold)
                continue;

            int totalSold = salesDict.ContainsKey(item.Id)
                ? salesDict[item.Id]
                : 0;

            reorderList.Add(new ReorderItemSummary
            {
                ItemName = item.Name,
                Stock = item.Stock,
                TotalQuantitySold = totalSold,
                SupplierName = supplierDict[item.Id]
            });
        }

        return reorderList
            .OrderByDescending(x => x.TotalQuantitySold)
            .Take(5)
            .ToList();
    }

    public List<ReorderItemSummary> GetItemsToReorder_Parallel(int stockThreshold)
    {
        // 1) Tạo lookup Supplier
        var supplierDict = Suppliers.ToDictionary(s => s.ItemId, s => s.SupplierName);

        // 2) Tính tổng số lượng sold — dùng ConcurrentDictionary vì chạy song song
        var salesDict = new ConcurrentDictionary<int, int>();

        Parallel.ForEach(SalesRecords, sale =>
        {
            salesDict.AddOrUpdate(
                sale.ItemId,
                sale.QuantitySold,
                (id, oldValue) => oldValue + sale.QuantitySold
            );
        });

        // 3) Xử lý InventoryItems song song → lọc item cần reorder
        var reorderBag = new ConcurrentBag<ReorderItemSummary>();

        Parallel.ForEach(InventoryItems, item =>
        {
            if (item.Stock >= stockThreshold)
                return;

            int totalSold = salesDict.TryGetValue(item.Id, out int value) ? value : 0;

            reorderBag.Add(new ReorderItemSummary
            {
                ItemName = item.Name,
                Stock = item.Stock,
                TotalQuantitySold = totalSold,
                SupplierName = supplierDict[item.Id]
            });
        });

        // 4) Sắp xếp kết quả sau khi hoàn thành song song
        return reorderBag
            .OrderByDescending(x => x.TotalQuantitySold)
            .Take(5)
            .ToList();
    }


}

/*
 !=== Items to Reorder ===
    Stock Threshold: 10

    Item: Monitor
      Stock: 3
      Total Sold: 120
      Supplier: MonitorMart

    Item: Laptop
      Stock: 5
      Total Sold: 100
      Supplier: TechSupplier Inc.

    Item: Keyboard
      Stock: 8
      Total Sold: 80
      Supplier: KeyboardsRUs
 */

/*
!This exercise identifies items that need reordering based on low stock and high demand.

* 1.Filtering by Stock Level:

Where(item => item.Stock < stockThreshold) filters items with stock below the threshold.

* 2.Calculating Total Quantity Sold:

GroupBy(sr => sr.ItemId).Sum(sr => sr.QuantitySold) calculates total demand for each item.

* 3.Selecting Top High-Demand Items:

OrderByDescending(item => item.TotalQuantitySold).Take(5) selects the top 5 items by total demand.

* 4.Returning the Summary:

The result is a list of ReorderItemSummary objects, sorted by TotalQuantitySold in descending order
 
 */