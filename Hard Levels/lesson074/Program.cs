class Program
{
    static void Main()
    {
        var repo = new OrderRepository();

        var results = repo.GetFavoriteProductByCustomer_SelectMany();

        Console.WriteLine("=== FAVORITE PRODUCT FOR EACH CUSTOMER ===\n");

        foreach (var item in results)
        {
            Console.WriteLine($"Customer: {item.CustomerName}");
            Console.WriteLine($"Favorite Product: {item.ProductName}");
            Console.WriteLine($"Order Count: {item.OrderCount}\n");
        }

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}

public class Product
{
    public int Id { get; set; }
    public required string Name { get; set; }
}

public class Customer
{
    public int Id { get; set; }
    public required string Name { get; set; }
}

public class Order
{
    public int CustomerId { get; set; }
    public int ProductId { get; set; }
}

public class FavoriteProductSummary
{
    public required string CustomerName { get; set; }
    public required string ProductName { get; set; }
    public int OrderCount { get; set; }
}

public class OrderRepository
{
    public List<Product> Products { get; set; } =
    [
        new Product { Id = 1, Name = "Laptop" },
        new Product { Id = 2, Name = "Smartphone" },
        new Product { Id = 3, Name = "Headphones" }
    ];

    public List<Customer> Customers { get; set; } =
    [
        new Customer { Id = 1, Name = "Alice" },
        new Customer { Id = 2, Name = "Bob" },
        new Customer { Id = 3, Name = "Charlie" }
    ];

    public List<Order> Orders { get; set; } =
    [
        new Order { CustomerId = 1, ProductId = 1 },
        new Order { CustomerId = 1, ProductId = 1 },
        new Order { CustomerId = 1, ProductId = 2 },
        new Order { CustomerId = 2, ProductId = 2 },
        new Order { CustomerId = 2, ProductId = 2 },
        new Order { CustomerId = 2, ProductId = 3 },
        new Order { CustomerId = 3, ProductId = 3 },
        new Order { CustomerId = 3, ProductId = 3 },
        new Order { CustomerId = 3, ProductId = 1 }
    ];

    public List<FavoriteProductSummary> GetFavoriteProductByCustomer()
    {
        /*
             !=== FAVORITE PRODUCT FOR EACH CUSTOMER ===

            Customer: Alice
            Favorite Product: Laptop
            Order Count: 2

            Customer: Bob
            Favorite Product: Smartphone
            Order Count: 2

            Customer: Charlie
            Favorite Product: Headphones
            Order Count: 2
        
         */

        var result = Orders
                     .GroupBy(o => o.CustomerId)
                     .Select(group =>
                     {
                         var customerName = Customers.First(c => c.Id == group.Key).Name;
                         var TopProduct = group
                                        .Where(o => o.CustomerId == group.Key)
                                        .GroupBy(o => o.ProductId)
                                        .Select(g => new
                                        {
                                            productName = Products.First(p => p.Id == g.Key).Name,
                                            TotalOrder = g.Count()
                                        })
                                        .OrderByDescending(o => o.TotalOrder)
                                        .First();
                         return new FavoriteProductSummary
                         {
                             CustomerName = customerName,
                             ProductName = TopProduct.productName,
                             OrderCount = TopProduct.TotalOrder

                         };
                     })
                     .OrderBy(summary => summary.CustomerName)
                     .ToList();

        return result;
    }

    public List<FavoriteProductSummary> GetFavoriteProductByCustomer_AuthorWriting()
    {
        var favoriteProducts = Orders
            .GroupBy(order => order.CustomerId)
            .Select(group =>
            {
                var favoriteProduct = group
                    .GroupBy(order => order.ProductId)
                    .Select(productGroup => new
                    {
                        ProductId = productGroup.Key,
                        OrderCount = productGroup.Count()
                    })
                    .OrderByDescending(p => p.OrderCount)
                    .First();

                var customerName = Customers.First(c => c.Id == group.Key).Name;
                var productName = Products.First(p => p.Id == favoriteProduct.ProductId).Name;

                return new FavoriteProductSummary
                {
                    CustomerName = customerName,
                    ProductName = productName,
                    OrderCount = favoriteProduct.OrderCount
                };
            })
            .OrderBy(summary => summary.CustomerName)
            .ToList();

        return favoriteProducts;
    }

    public List<FavoriteProductSummary> GetFavoriteProductByCustomer_SelectMany()
    {
        /*
            ChatGPT said:

            Có — hoàn toàn có thể viết lại bằng SelectMany, nhưng phải hiểu đúng mục tiêu:

            Mỗi khách hàng → chọn ra product mà họ đặt nhiều nhất

            Kết quả cuối vẫn là: 1 dòng cho mỗi Customer

            SelectMany thường dùng để làm phẳng dữ liệu sau khi join hoặc sau khi group-nested.
            Trong bài này, ta có thể dùng nó theo dạng:

            GroupBy(customer)
            → SelectMany để flatten các product của từng customer
            → Sau đó lại GroupBy để lấy top product.

            Dưới đây là phiên bản LINQ viết bằng SelectMany, chạy đúng kết quả:
         */

        var result = Orders
                // Step 1: Group orders theo CustomerId
                // Customer 1 → [(1,1), (1,1), (1,2)]
                // Customer 2 → [(2,2), (2,2), (2,3)]
                // Customer 3 → [(3,3), (3,3), (3,1)]
                .GroupBy(o => o.CustomerId)

                // Step 2: Flatten nested groups
                .SelectMany(group =>
                    group
                        // Step 2a: Trong mỗi group (customer), group theo ProductId
                        // Customer 1 → [(1,1), (1,1)], [(1,2)]
                        .GroupBy(o => o.ProductId)

                        // Step 2b: Chuyển mỗi nested group thành object tạm
                        .Select(g => new
                        {
                            CustomerId = group.Key, // customer Id
                            ProductId = g.Key,      // product Id
                            Count = g.Count()       // số lần đặt sản phẩm
                        })
                        // Customer 1 → [(1,1,2)], [(1,2,1)]

                        // Step 2c: Chọn sản phẩm mua nhiều nhất (favorite)
                        .OrderByDescending(x => x.Count)
                        .Take(1)   // Customer 1 → [[ (1,1,2) ]]
                )
                // Sau SelectMany: [[(1,1,2)], [(2,2,2)], [(3,3,2)]] ==> [(1,1,2), (2,2,2), (3,3,2)]
                // Step 3: Map sang DTO chuẩn
                .Select(x => new FavoriteProductSummary
                {
                    CustomerName = Customers.First(c => c.Id == x.CustomerId).Name,
                    ProductName = Products.First(p => p.Id == x.ProductId).Name,
                    OrderCount = x.Count
                })

                // Step 4: Sắp xếp theo tên customer
                .OrderBy(x => x.CustomerName)
                .ToList();

        return result;
    }
}

/*
 !This exercise identifies the product each customer has ordered the most.

* 1.Grouping Orders by Customer:

GroupBy(order => order.CustomerId) groups orders by each customer.

* 2.Finding the Most Ordered Product per Customer:

For each customer, GroupBy(order => order.ProductId) groups orders by product, and Count() calculates the number of orders for each product.

OrderByDescending(p => p.OrderCount).First() selects the product with the highest order count.

* 3.Returning the Summary:

The result is a list of FavoriteProductSummary objects, sorted by CustomerName alphabetically.
 
*/