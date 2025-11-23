using System.Linq;

class Program
{
    static void Main()
    {
        var repo = new ProductRepository();

        var popularProducts = repo.GetPopularProducts();

        // In tiêu đề với điều kiện lọc
        Console.WriteLine("!=== POPULAR PRODUCTS ===");
        Console.WriteLine("(Condition: PurchaseCount >= 5, AverageRating >= 4)\n");

        foreach (var product in popularProducts)
        {
            Console.WriteLine($"Product: {product.ProductName}");
            Console.WriteLine($"Purchase Count: {product.PurchaseCount}");
            Console.WriteLine($"Average Rating: {product.AverageRating:F2}\n");
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

public class Purchase
{
    public int ProductId { get; set; }
    public int CustomerId { get; set; }
}

public class Rating
{
    public int ProductId { get; set; }
    public int CustomerId { get; set; }
    public int Score { get; set; } // Renamed from Rating to Score
}

public class PopularProductSummary
{
    public required string ProductName { get; set; }
    public int PurchaseCount { get; set; }
    public double AverageRating { get; set; }
}

public class ProductRepository
{
    public List<Product> Products { get; set; } =
    [
        new Product { Id = 1, Name = "Laptop" },
        new Product { Id = 2, Name = "Smartphone" },
        new Product { Id = 3, Name = "Headphones" }
    ];

    public List<Purchase> Purchases { get; set; } =
    [
        new Purchase { ProductId = 1, CustomerId = 1 },
        new Purchase { ProductId = 1, CustomerId = 2 },
        new Purchase { ProductId = 1, CustomerId = 3 },
        new Purchase { ProductId = 1, CustomerId = 4 },
        new Purchase { ProductId = 1, CustomerId = 5 },
        new Purchase { ProductId = 2, CustomerId = 1 },
        new Purchase { ProductId = 2, CustomerId = 2 },
        new Purchase { ProductId = 3, CustomerId = 1 }
    ];

    public List<Rating> Ratings { get; set; } =
    [
        new Rating { ProductId = 1, CustomerId = 1, Score = 5 },
        new Rating { ProductId = 1, CustomerId = 2, Score = 4 },
        new Rating { ProductId = 1, CustomerId = 3, Score = 4 },
        new Rating { ProductId = 1, CustomerId = 4, Score = 5 },
        new Rating { ProductId = 1, CustomerId = 5, Score = 4 },
        new Rating { ProductId = 2, CustomerId = 1, Score = 3 },
        new Rating { ProductId = 2, CustomerId = 2, Score = 4 },
        new Rating { ProductId = 3, CustomerId = 1, Score = 5 }
    ];

    public List<PopularProductSummary> GetPopularProducts()
    {


        /*
            !=== POPULAR PRODUCTS ===
            (Condition: PurchaseCount >= 5, AverageRating >= 4)

            Product: Laptop
            Purchase Count: 5
            Average Rating: 4.40        
         */


        // Tạo lookup cho Ratings theo ProductId
        var ratingLookup = Ratings
            .GroupBy(r => r.ProductId)
            .ToDictionary(g => g.Key, g => g.Average(r => r.Score));

        // Tạo lookup cho Purchases theo ProductId
        var purchaseLookup = Purchases
            .GroupBy(p => p.ProductId)
            .ToDictionary(g => g.Key, g => g.Count());

        var result = Products
            .Select(p => new
            {
                Product = p,
                PurchaseCount = purchaseLookup.ContainsKey(p.Id) ? purchaseLookup[p.Id] : 0,
                AverageScore = ratingLookup.ContainsKey(p.Id) ? ratingLookup[p.Id] : 0
            })
            .Where(p => p.PurchaseCount >= 5 && p.AverageScore >= 4)
            .OrderByDescending(p => p.AverageScore)
            .Select(p => new PopularProductSummary
            {
                ProductName = p.Product.Name,
                PurchaseCount = p.PurchaseCount,
                AverageRating = p.AverageScore
            })
            .ToList();

        return result;
    }

    public List<PopularProductSummary> GetPopularProducts_AuthorWriting()
    {
        var popularProducts = Products
                            .GroupJoin(Purchases,
                                    product => product.Id,
                                    purchase => purchase.ProductId,
                                    (product, purchases) => new
                                    {
                                        Product = product,
                                        PurchaseCount = purchases.Count()
                                    })
                            .Where(p => p.PurchaseCount >= 5)
                            .Join(Ratings.GroupBy(r => r.ProductId)
                                        .Select(group => new
                                        {
                                            ProductId = group.Key,
                                            AverageRating = group.Average(r => r.Score)
                                        }),
                                p => p.Product.Id,
                                r => r.ProductId,
                                (p, r) => new
                                {
                                    ProductName = p.Product.Name,
                                    PurchaseCount = p.PurchaseCount,
                                    AverageRating = r.AverageRating
                                })
                            .Where(p => p.AverageRating >= 4)
                            .OrderByDescending(p => p.AverageRating)
                            .Select(p => new PopularProductSummary
                            {
                                ProductName = p.ProductName,
                                PurchaseCount = p.PurchaseCount,
                                AverageRating = p.AverageRating
                            })
                            .ToList();

        return popularProducts;
    }
}


/*
    !This exercise identifies products with high ratings and frequent purchases.

    * 1.Counting Purchases:

    GroupJoin with Purchases counts the number of purchases for each product.

    * 2.Calculating Average Rating:

    GroupBy(r => r.ProductId).Average(r => r.Rating) calculates the average rating for each product.

    * 3.Filtering Popular Products:

    Only products with at least 5 purchases and an average rating of 4 or above are included.

    * 4.Returning the Summary:

    The result is a list of PopularProductSummary objects, sorted by AverageRating in descending order.      
*/