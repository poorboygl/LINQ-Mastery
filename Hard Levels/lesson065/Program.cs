class Program
{
    static void Main()
    {
        var repo = new ProductRepository();

        var summaries = repo.GetAverageRatingByCategory();

        Console.WriteLine("=== CATEGORY AVERAGE RATING ===\n");

        foreach (var s in summaries)
        {
            Console.WriteLine($"Category: {s.CategoryName}");
            Console.WriteLine($"  Average Rating: {s.AverageRating:F2}\n");
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
    public int CategoryId { get; set; }
    public required string Name { get; set; }
}

public class Rating
{
    public int ProductId { get; set; }
    public int CustomerId { get; set; }
    public double Score { get; set; }
}

public class CategoryRatingSummary
{
    public required string CategoryName { get; set; }
    public double AverageRating { get; set; }
}

public class ProductRepository
{
    public List<Category> Categories { get; set; } =
    [
        new Category { Id = 1, Name = "Electronics" },
        new Category { Id = 2, Name = "Furniture" }
    ];

    public List<Product> Products { get; set; } =
    [
        new Product { Id = 1, CategoryId = 1, Name = "Laptop" },
        new Product { Id = 2, CategoryId = 1, Name = "Smartphone" },
        new Product { Id = 3, CategoryId = 2, Name = "Chair" }
    ];

    public List<Rating> Ratings { get; set; } =
    [
        new Rating { ProductId = 1, CustomerId = 1, Score = 4.5 },
        new Rating { ProductId = 1, CustomerId = 2, Score = 4.0 },
        new Rating { ProductId = 2, CustomerId = 1, Score = 5.0 },
        new Rating { ProductId = 3, CustomerId = 2, Score = 3.5 },
        new Rating { ProductId = 3, CustomerId = 3, Score = 4.0 }
    ];

    public List<CategoryRatingSummary> GetAverageRatingByCategory()
    {
        return Categories
            .Select(category => new CategoryRatingSummary
            {
                CategoryName = category.Name,
                AverageRating = Products
                    .Where(product => product.CategoryId == category.Id)
                    .Join(Ratings,
                          product => product.Id,
                          rating => rating.ProductId,
                          (product, rating) => rating.Score)
                    .Average()
            })
            .OrderByDescending(summary => summary.AverageRating)
            .ToList();
    }

    public List<CategoryRatingSummary> GetAverageRatingByCategory_2()
    {
        /*
         !=== CATEGORY AVERAGE RATING ===

            Category: Electronics
              Average Rating: 4.50

            Category: Furniture
              Average Rating: 3.75
         
        */

        var result = Categories
                     .GroupJoin(Products,
                     c => c.Id,
                     p => p.CategoryId,
                     (category, CategoryProducts) =>
                     {
                         var categoryName = category.Name;
                         var ratingScores = CategoryProducts
                                            .Join(Ratings,
                                                 p => p.Id,
                                                 r => r.ProductId,
                                                 (product, rating) => new
                                                 {
                                                     score = rating.Score
                                                 })
                                            .ToList();
                         return new CategoryRatingSummary
                         {
                             CategoryName = categoryName,
                             AverageRating = ratingScores.Average(r => r.score)
                         };
                     })
                     .OrderByDescending(summary => summary.AverageRating)
                     .ToList();
        return result;
    }
}


/*
 ! This exercise generates a report showing the average customer rating for each product category.

* 1.Filtering Products by Category:

Products.Where(product => product.CategoryId == category.Id) selects products within each category.

* 2.Calculating Average Rating:

AverageRating: Join(Ratings, product => product.Id, rating => rating.ProductId, (product, rating) => rating.Score).Average() calculates the average rating for all products within a category.

* 3.Returning the Report:

The result is a list of CategoryRatingSummary objects, showing each category’s average rating.
*/