class Program
{
    static void Main()
    {
        var repo = new PurchaseRepository();
        var multiCategoryCustomers = repo.GetMultiCategoryCustomers();

        // Print description in English
        Console.WriteLine("List of customers who purchased products from >=2 categories:");
        Console.WriteLine("===============================================================");

        foreach (var customer in multiCategoryCustomers)
        {
            Console.WriteLine($"CustomerId: {customer.CustomerId}, Name: {customer.CustomerName}, Categories Bought: {customer.CategoryCount}");
        }

        Console.WriteLine("\nPress any key to exit...");
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
}

public class Purchase
{
    public int CustomerId { get; set; }
    public int ProductId { get; set; }
}

public class Customer
{
    public int Id { get; set; }
    public required string Name { get; set; }
}

public class MultiCategoryCustomerSummary
{
    public int CustomerId { get; set; }
    public required string CustomerName { get; set; }
    public int CategoryCount { get; set; }
}

public class PurchaseRepository
{
    public List<Category> Categories { get; set; } = 
    [
        new Category { Id = 1, Name = "Electronics" },
        new Category { Id = 2, Name = "Clothing" },
        new Category { Id = 3, Name = "Books" }
    ];

    public List<Product> Products { get; set; } = 
    [
        new Product { Id = 1, Name = "Laptop", CategoryId = 1 },
        new Product { Id = 2, Name = "Smartphone", CategoryId = 1 },
        new Product { Id = 3, Name = "T-shirt", CategoryId = 2 },
        new Product { Id = 4, Name = "Novel", CategoryId = 3 }
    ];

    public List<Purchase> Purchases { get; set; } = 
    [
        new Purchase { CustomerId = 1, ProductId = 1 },
        new Purchase { CustomerId = 1, ProductId = 3 },
        new Purchase { CustomerId = 2, ProductId = 2 },
        new Purchase { CustomerId = 2, ProductId = 4 },
        new Purchase { CustomerId = 3, ProductId = 4 }
    ];

    public List<Customer> Customers { get; set; } = 
    [
        new Customer { Id = 1, Name = "Alice" },
        new Customer { Id = 2, Name = "Bob" },
        new Customer { Id = 3, Name = "Charlie" }
    ];


    public List<MultiCategoryCustomerSummary> GetMultiCategoryCustomers()
    {
        /*
             List of customers who purchased products from >=2 categories:
            ===============================================================
            CustomerId: 1, Name: Alice, Categories Bought: 2
            CustomerId: 2, Name: Bob, Categories Bought: 2
         */
        // Tạo dictionary để lookup Customer nhanh hơn
        var customerLookup = Customers.ToDictionary(c => c.Id);

        var multiCategoryCustomers = Purchases
            .Join(Products,
                  purchase => purchase.ProductId,
                  product => product.Id,
                  (purchase, product) => new { purchase.CustomerId, product.CategoryId })
            .GroupBy(p => p.CustomerId)
            .Where(g => g.Select(p => p.CategoryId).Distinct().Count() >= 2)
            .Select(g =>
            {
                var customer = customerLookup[g.Key]; // lookup O(1)
                return new MultiCategoryCustomerSummary
                {
                    CustomerId = customer.Id,
                    CustomerName = customer.Name,
                    CategoryCount = g.Select(p => p.CategoryId).Distinct().Count()
                };
            })
            .OrderBy(c => c.CustomerName)
            .ToList();

        return multiCategoryCustomers;
    }

    public List<MultiCategoryCustomerSummary> GetMultiCategoryCustomers_AuthorWriting()
    {
        /*
            !List of customers who purchased products from >=2 categories:
            ===============================================================
            CustomerId: 1, Name: Alice, Categories Bought: 2
            CustomerId: 2, Name: Bob, Categories Bought: 2
         */
        var multiCategoryCustomers = Purchases
            .Join(Products,
                  purchase => purchase.ProductId,
                  product => product.Id,
                  (purchase, product) => new { purchase.CustomerId, product.CategoryId })
            .GroupBy(p => p.CustomerId)
            .Where(g => g.Select(p => p.CategoryId).Distinct().Count() >= 2)
            .Select(g =>
            {
                var customer = Customers.First(c => c.Id == g.Key);
                return new MultiCategoryCustomerSummary
                {
                    CustomerId = customer.Id,
                    CustomerName = customer.Name,
                    CategoryCount = g.Select(p => p.CategoryId).Distinct().Count()
                };
            })
            .OrderBy(c => c.CustomerName)
            .ToList();

        return multiCategoryCustomers;
    }

    public List<MultiCategoryCustomerSummary> GetMultiCategoryCustomers_2()
    {
        /*
             List of customers who purchased products from >=2 categories:
            ===============================================================
            CustomerId: 1, Name: Alice, Categories Bought: 2
            CustomerId: 2, Name: Bob, Categories Bought: 2
         */
        var customerLookup = Customers.ToDictionary(c => c.Id);

        var multiCategoryCustomers = Purchases
            .Join(Products,
                  purchase => purchase.ProductId,
                  product => product.Id,
                  (purchase, product) => new { purchase.CustomerId, product.CategoryId })
            .GroupBy(p => p.CustomerId)
            .Select(g =>
            {
                // Tạo biến tạm: tập category distinct của khách
                var distinctCategories = g.Select(p => p.CategoryId).Distinct().ToList();

                // Chỉ chọn khách hàng mua >=2 category
                if (distinctCategories.Count < 2)
                    return null;

                var customer = customerLookup[g.Key]; // dictionary lookup O(1)

                return new MultiCategoryCustomerSummary
                {
                    CustomerId = customer.Id,
                    CustomerName = customer.Name,
                    CategoryCount = distinctCategories.Count
                };
            })
            .Where(c => c != null)
            .OrderBy(c => c.CustomerName)
            .ToList();

        return multiCategoryCustomers;
    }

}

/*
 !This exercise identifies customers who have purchased products across multiple categories.

* 1.Joining Purchases with Products:

Join with Products to retrieve the category for each purchased product.

* 2.Grouping by Customer:

GroupBy(p => p.CustomerId) groups purchases by each customer.

* 3.Filtering Multi-Category Customers:

Select(p => p.CategoryId).Distinct().Count() >= 2 filters for customers who have purchased from 2 or more categories.

* 4.Returning the Summary:

The result is a list of MultiCategoryCustomerSummary objects, sorted by CustomerName alphabetically.
 
 */