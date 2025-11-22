using System.Text.RegularExpressions;

class Program
{
    static void Main()
    {
        var repo = new ReviewRepository();

        var popularProducts = repo.GetPopularProductsAverageRating();

        Console.WriteLine("=== POPULAR PRODUCTS (>= 3 reviews) WITH AVERAGE RATING ===\n");

        if (popularProducts.Count == 0)
        {
            Console.WriteLine("No popular product found.");
        }
        else
        {
            foreach (var product in popularProducts)
            {
                Console.WriteLine($"Product: {product.ProductName}");
                Console.WriteLine($"Average Rating: {product.AverageRating:F2}\n");
            }
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

public class Review
{
    public int ProductId { get; set; }
    public int CustomerId { get; set; }
    public int Rating { get; set; } // Rating from 1 to 5
}

public class ProductRatingSummary
{
    public required string ProductName { get; set; }
    public double AverageRating { get; set; }
}

public class ReviewRepository
{
    private static readonly List<Review> reviews =
    [
        new Review { ProductId = 1, CustomerId = 1, Rating = 5 },
        new Review { ProductId = 1, CustomerId = 2, Rating = 4 },
        new Review { ProductId = 1, CustomerId = 3, Rating = 5 },
        new Review { ProductId = 2, CustomerId = 1, Rating = 3 },
        new Review { ProductId = 2, CustomerId = 2, Rating = 4 },
        new Review { ProductId = 3, CustomerId = 1, Rating = 4 }
    ];

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

    public List<Review> Reviews { get; set; } =
    [
        new Review { ProductId = 1, CustomerId = 1, Rating = 5 },
        new Review { ProductId = 1, CustomerId = 2, Rating = 4 },
        new Review { ProductId = 1, CustomerId = 3, Rating = 5 },
        new Review { ProductId = 2, CustomerId = 1, Rating = 3 },
        new Review { ProductId = 2, CustomerId = 2, Rating = 4 },
        new Review { ProductId = 3, CustomerId = 1, Rating = 4 }
    ];

    public List<ProductRatingSummary> GetPopularProductsAverageRating()
    {
        /*
             !=== POPULAR PRODUCTS (>= 3 reviews) WITH AVERAGE RATING ===

            Product: Laptop
            Average Rating: 4.67
        */
        var result = Reviews
                     .GroupBy(r => r.ProductId)
                     .Where(group => group.Count() >= 3)
                     .Select(group => new ProductRatingSummary 
                     { 
                        ProductName = Products.First( p => p.Id == group.Key ).Name,
                        AverageRating = group.Average( r => r.Rating)
                     })
                     .OrderByDescending( r => r.AverageRating )
                     .ToList();

        return result;
    }

    public List<ProductRatingSummary> GetPopularProductsAverageRating_AuthorWritting()
    {
        var popularProducts = Reviews
            .GroupBy(review => review.ProductId)
            .Where(group => group.Count() >= 3)
            .Select(group => new
            {
                ProductId = group.Key,
                AverageRating = group.Average(review => review.Rating)
            })
            .OrderByDescending(p => p.AverageRating)
            .Join(Products,
                  p => p.ProductId,
                  product => product.Id,
                  (p, product) => new ProductRatingSummary
                  {
                      ProductName = product.Name,
                      AverageRating = p.AverageRating
                  })
            .ToList();

        return popularProducts;
    }
}

/*
!This exercise identifies products with sufficient reviews and calculates their average rating.

* 1.Grouping by Product:

GroupBy(review => review.ProductId) groups reviews by each product.

* 2.Filtering Popular Products:

Where(group => group.Count() >= 3) ensures only products with 3 or more reviews are included.

* 3.Calculating Average Rating:

Average(review => review.Rating) calculates each product’s average rating.

* 4.Returning the Summary:

The result is a list of ProductRatingSummary objects, sorted by AverageRating in descending order.
*/