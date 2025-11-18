class Program
{
    static void Main()
    {
        var repo = new SalesRepository();

        var summaries = repo.GetMonthlySalesByCategory();


        //List<object> test = [];
        //repo.GetAllSaleRecordByCategory(out test);

        Console.WriteLine("== Category Monthly Sales Summary ==\n");

        foreach (var summary in summaries)
        {
            Console.WriteLine($"Category: {summary.CategoryName}");
            Console.WriteLine($"Month   : {summary.Month}");
            Console.WriteLine($"Total Sales Amount: {summary.TotalSalesAmount:C}");
            Console.WriteLine($"Total Units Sold : {summary.TotalUnitsSold}");
            Console.WriteLine(new string('-', 40));
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
    public decimal Price { get; set; }
}

public class SalesRecord
{
    public int ProductId { get; set; }
    public DateTime SaleDate { get; set; }
    public int Quantity { get; set; }
}

public class CategoryMonthlySalesSummary
{
    public required string CategoryName { get; set; }
    public required string Month { get; set; }
    public decimal TotalSalesAmount { get; set; }
    public int TotalUnitsSold { get; set; }
}

public class SalesRepository
{
    public List<Category> Categories { get; set; } =
    [
        new Category { Id = 1, Name = "Electronics" },
        new Category { Id = 2, Name = "Furniture" }
    ];

    public List<Product> Products { get; set; } =
    [
        new Product { Id = 1, Name = "Laptop", CategoryId = 1, Price = 1000m },
        new Product { Id = 2, Name = "Phone", CategoryId = 1, Price = 500m },
        new Product { Id = 3, Name = "Chair", CategoryId = 2, Price = 150m },
        new Product { Id = 4, Name = "Desk", CategoryId = 2, Price = 300m }
    ];

    public List<SalesRecord> SalesRecords { get; set; } =
    [
        new SalesRecord { ProductId = 1, SaleDate = new DateTime(2023, 1, 15), Quantity = 5 },
        new SalesRecord { ProductId = 2, SaleDate = new DateTime(2023, 1, 20), Quantity = 10 },
        new SalesRecord { ProductId = 3, SaleDate = new DateTime(2023, 2, 5), Quantity = 8 },
        new SalesRecord { ProductId = 4, SaleDate = new DateTime(2023, 2, 12), Quantity = 6 }
    ];

    public List<CategoryMonthlySalesSummary> GetMonthlySalesByCategory()
    {
        return [.. Categories
            .GroupJoin(Products,
                category => category.Id,
                product => product.CategoryId,
                (category, categoryProducts) => new  // Phân loại Product ra từng category
                {
                    CategoryName = category.Name,
                    Sales = categoryProducts        //Product join với SaleRecord chỉ lấy phần chung
                        .Join(SalesRecords,
                            product => product.Id,
                            sale => sale.ProductId,
                            (product, sale) => new
                            {
                                SaleMonth = sale.SaleDate.ToString("MMMM yyyy"),
                                TotalSaleAmount = product.Price * sale.Quantity,
                                Quantity = sale.Quantity
                            })
                })
                .SelectMany(group => group.Sales
                .GroupBy(sale => sale.SaleMonth)
                .Select(monthGroup => new CategoryMonthlySalesSummary
                {
                    CategoryName = group.CategoryName,
                    Month = monthGroup.Key,
                    TotalSalesAmount = monthGroup.Sum(s => s.TotalSaleAmount),
                    TotalUnitsSold = monthGroup.Sum(s => s.Quantity)
                }))
            .OrderBy(summary => summary.CategoryName)
            .ThenBy(summary => summary.Month)];
    }

    public void GetAllSaleRecordByCategory(out List<object> result)
    {
        result = [.. Categories
            .GroupJoin(Products,
                category => category.Id,
                product => product.CategoryId,
                (category, categoryProducts) => new  // Phân loại Product ra từng category
                {
                    CategoryName = category.Name,
                    Sales = categoryProducts        //Product join với SaleRecord chỉ lấy phần chung
                        .Join(SalesRecords,
                            product => product.Id,
                            sale => sale.ProductId,
                            (product, sale) => new
                            {
                                product.Id,
                                product.Name,
                                sale.Quantity,
                                sale.SaleDate
                            }).ToList()
                       
                })
             //.SelectMany(group => group.Sales)
            ];
    }
}


/*
     Category: Electronics
    Month   : January 2023
    Total Sales Amount: $10,000.00
    Total Units Sold : 15
    ----------------------------------------
    Category: Furniture
    Month   : February 2023
    Total Sales Amount: $3,000.00
    Total Units Sold : 14
 */

/*
 This exercise involves generating a monthly sales report for each category by combining data from categories, products, and sales records.

* 1.Grouping Sales by Category and Month:

GroupJoin links categories to products, and Join links products to their sales records.

GroupBy(sale => sale.SaleMonth) groups sales records by month.

* 2.Calculating Sales Metrics:

TotalSalesAmount: Sums up the total sales amount for each category in each month.

TotalUnitsSold: Sums up the total quantity of units sold for each category in each month.

* 3.Returning the Sorted Summary:

OrderBy(summary => summary.CategoryName).ThenBy(summary => summary.Month) sorts by CategoryName and Month.
 
 */