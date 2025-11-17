class Program
{
    static void Main()
    {
        var repo = new InventoryRepository();

        var summaries = repo.GetOutOfStockProductsBySupplier();

        Console.WriteLine("Suppliers with Out-of-Stock Products:\n");

        foreach (var s in summaries)
        {
            Console.WriteLine($"Supplier: {s.SupplierName}");
            Console.WriteLine($"  Out-of-Stock Products: {s.TotalOutOfStockProducts}");
            Console.WriteLine($"  Potential Revenue Loss: {s.TotalPotentialRevenueLoss:C}");
            Console.WriteLine();
        }

        Console.ReadKey();
    }
}

public class Product
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int SupplierId { get; set; }
    public int StockQuantity { get; set; }
    public decimal Price { get; set; }
}

public class Supplier
{
    public int Id { get; set; }
    public required string Name { get; set; }
}

public class SupplierOutOfStockSummary
{
    public required string SupplierName { get; set; }
    public int TotalOutOfStockProducts { get; set; }
    public decimal TotalPotentialRevenueLoss { get; set; }
}

public class InventoryRepository
{
    public List<Product> Products { get; set; } =
    [
        new Product { Id = 1, Name = "Laptop", SupplierId = 1, StockQuantity = 0, Price = 1200m },
        new Product { Id = 2, Name = "Mouse", SupplierId = 1, StockQuantity = 10, Price = 25m },
        new Product { Id = 3, Name = "Keyboard", SupplierId = 2, StockQuantity = 0, Price = 75m },
        new Product { Id = 4, Name = "Monitor", SupplierId = 2, StockQuantity = 5, Price = 200m },
        new Product { Id = 5, Name = "Desk", SupplierId = 3, StockQuantity = 0, Price = 300m }
    ];

    public List<Supplier> Suppliers { get; set; } =
    [
        new Supplier { Id = 1, Name = "Tech Supplies Co." },
        new Supplier { Id = 2, Name = "Office Essentials Ltd." },
        new Supplier { Id = 3, Name = "Furniture Solutions" }
    ];

    public List<SupplierOutOfStockSummary> GetOutOfStockProductsBySupplier()
    {
        return [.. Suppliers
            .GroupJoin(Products.Where(p => p.StockQuantity == 0),
                supplier => supplier.Id,
                product => product.SupplierId,
                (supplier, outOfStockProducts) => new SupplierOutOfStockSummary
                {
                    SupplierName = supplier.Name,
                    TotalOutOfStockProducts = outOfStockProducts.Count(),
                    TotalPotentialRevenueLoss = outOfStockProducts.Sum(p => p.Price)
                })
            .Where(summary => summary.TotalOutOfStockProducts > 0)
            .OrderByDescending(summary => summary.TotalPotentialRevenueLoss)];
    }
}

/*
 Suppliers with Out-of-Stock Products:

Supplier: Tech Supplies Co.
  Out-of-Stock Products: 1
  Potential Revenue Loss: $1,200.00

Supplier: Furniture Solutions
  Out-of-Stock Products: 1
  Potential Revenue Loss: $300.00

Supplier: Office Essentials Ltd.
  Out-of-Stock Products: 1
  Potential Revenue Loss: $75.00
 
 */

/*
This exercise focuses on joining and aggregating data across suppliers and products to analyze out-of-stock items.

* 1.Filtering Out-of-Stock Products:

Products.Where(p => p.StockQuantity == 0) filters for products with a StockQuantity of 0.

* 2.Grouping Products by Supplier:

GroupJoin(..., supplier => supplier.Id, product => product.SupplierId, ...) groups out-of-stock products by supplier.

* 3.Calculating Out-of-Stock Metrics:

TotalOutOfStockProducts: Counts the out-of-stock products for each supplier.

TotalPotentialRevenueLoss: Sums the price of all out-of-stock products for each supplier.

* 4.Returning the Report:

The result is a list of SupplierOutOfStockSummary objects, sorted by TotalPotentialRevenueLoss in descending order.
 
 
 */