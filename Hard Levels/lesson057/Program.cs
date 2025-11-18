using System.Linq;

class Program
{
    static void Main()
    {
        var repo = new WarehouseRepository();

        var summaries = repo.GetTopSuppliersByShipmentVolume();

        Console.WriteLine("=== TOP SUPPLIERS BY PRODUCT SHIPMENT VOLUME ===\n");

        foreach (var summary in summaries)
        {
            Console.WriteLine($"Product: {summary.ProductName}");

            foreach (var supplier in summary.TopSuppliers)
            {
                Console.WriteLine($"   Supplier: {supplier.SupplierName}, Quantity: {supplier.TotalQuantityShipped}");
            }

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

public class Supplier
{
    public int Id { get; set; }
    public required string Name { get; set; }
}

public class Shipment
{
    public int ProductId { get; set; }
    public int SupplierId { get; set; }
    public int Quantity { get; set; }
    public DateTime ShipmentDate { get; set; }
}

public class SupplierShipmentSummary
{
    public required string SupplierName { get; set; }
    public int TotalQuantityShipped { get; set; }
}

public class ProductTopSuppliersSummary
{
    public required string ProductName { get; set; }
    public List<SupplierShipmentSummary> TopSuppliers { get; set; } = [];
}

public class WarehouseRepository
{
    public List<Product> Products { get; set; } =
    [
        new Product { Id = 1, Name = "Steel" },
        new Product { Id = 2, Name = "Copper" }
    ];

    public List<Supplier> Suppliers { get; set; } =
    [
        new Supplier { Id = 1, Name = "Supplier A" },
        new Supplier { Id = 2, Name = "Supplier B" },
        new Supplier { Id = 3, Name = "Supplier C" }
    ];

    public List<Shipment> Shipments { get; set; } =
    [
        new Shipment { ProductId = 1, SupplierId = 1, Quantity = 500, ShipmentDate = new DateTime(2023, 10, 1) },
        new Shipment { ProductId = 1, SupplierId = 2, Quantity = 300, ShipmentDate = new DateTime(2023, 10, 5) },
        new Shipment { ProductId = 1, SupplierId = 1, Quantity = 200, ShipmentDate = new DateTime(2023, 10, 10) },
        new Shipment { ProductId = 2, SupplierId = 3, Quantity = 400, ShipmentDate = new DateTime(2023, 10, 12) },
        new Shipment { ProductId = 2, SupplierId = 2, Quantity = 150, ShipmentDate = new DateTime(2023, 10, 15) }
    ];

    public List<ProductTopSuppliersSummary> GetTopSuppliersByShipmentVolume()
    {
        //var result = Products
        //            .GroupJoin(Shipments,
        //                product => product.Id,
        //                shipment => shipment.ProductId,
        //                (product, productShipments) => new
        //                {
        //                    ProductName = product.Name,
        //                    TopSuppliers = productShipments
        //                        .GroupBy(shipment => shipment.SupplierId)
        //                        .Select(supplierGroup => new SupplierShipmentSummary
        //                        {
        //                            SupplierName = Suppliers.First(s => s.Id == supplierGroup.Key).Name,
        //                            TotalQuantityShipped = supplierGroup.Sum(s => s.Quantity)
        //                        })
        //                        .OrderByDescending(s => s.TotalQuantityShipped)
        //                        .Take(2)
        //                        .ToList()
        //                })
        //            .Select(summary => new ProductTopSuppliersSummary
        //            {
        //                ProductName = summary.ProductName,
        //                TopSuppliers = summary.TopSuppliers
        //            })
        //            .ToList();

        // Suppliers.First(s => s.Id == supplierGroup.Key):

        //✔ JOIN manual giữa bảng Supplier và nhóm Shipment(vì không dùng Join trong LINQ)
        //✔ Lấy đúng SupplierName cho mỗi SupplierId
        //✔ Giúp xây dựng SupplierShipmentSummary đầy đủ thông tin


        var result = Products.GroupJoin(Shipments,
                        product => product.Id,
                        shipment => shipment.ProductId,
                        (product, productShipments) => new ProductTopSuppliersSummary
                        {
                            ProductName = product.Name,
                            TopSuppliers = productShipments
                                .GroupBy(s => s.SupplierId)
                                .Select(g => new
                                {
                                    SupplierId = g.Key,
                                    TotalQuantity = g.Sum(x => x.Quantity)
                                })
                                .Join(Suppliers,
                                    g => g.SupplierId,
                                    supplier => supplier.Id,
                                    (g, supplier) => new SupplierShipmentSummary
                                    {
                                        SupplierName = supplier.Name,
                                        TotalQuantityShipped = g.TotalQuantity
                                    }
                                )
                                .OrderByDescending(x => x.TotalQuantityShipped)
                                .Take(2)
                                .ToList()
                        })
                        .ToList();

        return result;
    }
}

/*
 ! === TOP SUPPLIERS BY PRODUCT SHIPMENT VOLUME ===

Product: Steel
   Supplier: Supplier A, Quantity: 700
   Supplier: Supplier B, Quantity: 300
----------------------------------------
Product: Copper
   Supplier: Supplier C, Quantity: 400
   Supplier: Supplier B, Quantity: 150
----------------------------------------
*/


/*
! This exercise generates a report on top suppliers by shipment volume per product by combining data across products, suppliers, and shipments.

* 1.Grouping Shipments by Product:

GroupJoin(Shipments, product => product.Id, shipment => shipment.ProductId, ...) groups shipments by each product.

* 2.Calculating Shipment Metrics:

TotalQuantityShipped: supplierGroup.Sum(s => s.Quantity) calculates the total quantity shipped for each supplier.

* 3.Identifying Top Suppliers by Shipment Volume:

OrderByDescending(s => s.TotalQuantityShipped).Take(2) selects the top 2 suppliers by total shipment volume within each product.

* 4.Returning the Report:

The result is a list of ProductTopSuppliersSummary objects, each containing the top 2 suppliers by shipment volume for each product.
 
 */