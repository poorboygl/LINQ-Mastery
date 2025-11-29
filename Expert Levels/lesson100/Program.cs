using System.Collections.Concurrent;

public class Program
{
    static void Main()
    {
        var repo = new RentalRepository();
        var topMovies = repo.GetTopMoviesByMonth_Optimized();

        Console.WriteLine("=== Monthly Top Movies ===");
        foreach (var summary in topMovies)
        {
            Console.WriteLine($"{summary.Month}: {summary.MovieTitle} (Rentals: {summary.RentalCount})");
        }

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}

public class Movie
{
    public int Id { get; set; }
    public required string Title { get; set; }
}

public class Rental
{
    public int MovieId { get; set; }
    public DateTime RentalDate { get; set; }
    public int CustomerId { get; set; }
}

public class MonthlyTopMovieSummary
{
    public required string Month { get; set; }
    public required string MovieTitle { get; set; }
    public int RentalCount { get; set; }
}

public class RentalRepository
{
    public List<Movie> Movies { get; set; } =
    [
        new Movie { Id = 1, Title = "Inception" },
        new Movie { Id = 2, Title = "The Matrix" },
        new Movie { Id = 3, Title = "Interstellar" }
    ];

    public List<Rental> Rentals { get; set; } =
    [
        new Rental { MovieId = 1, RentalDate = new DateTime(2024, 1, 10), CustomerId = 101 },
        new Rental { MovieId = 1, RentalDate = new DateTime(2024, 1, 20), CustomerId = 102 },
        new Rental { MovieId = 2, RentalDate = new DateTime(2024, 1, 15), CustomerId = 103 },
        new Rental { MovieId = 3, RentalDate = new DateTime(2024, 2, 5), CustomerId = 104 },
        new Rental { MovieId = 1, RentalDate = new DateTime(2024, 2, 10), CustomerId = 105 },
        new Rental { MovieId = 3, RentalDate = new DateTime(2024, 2, 25), CustomerId = 106 }
    ];

    public List<MonthlyTopMovieSummary> GetTopMoviesByMonth()
    {
        var topMovies = Rentals
            .GroupBy(rental => new { rental.RentalDate.Year, rental.RentalDate.Month, rental.MovieId })
            .Select(g => new
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                MovieId = g.Key.MovieId,
                RentalCount = g.Count()
            })
            .GroupBy(g => new { g.Year, g.Month })
            .Select(monthGroup => monthGroup
                .OrderByDescending(g => g.RentalCount)
                .First())
            .Join(Movies,
                  monthMovie => monthMovie.MovieId,
                  movie => movie.Id,
                  (monthMovie, movie) => new MonthlyTopMovieSummary
                  {
                      Month = new DateTime(monthMovie.Year, monthMovie.Month, 1).ToString("MMMM yyyy"),
                      MovieTitle = movie.Title,
                      RentalCount = monthMovie.RentalCount
                  })
            .OrderBy(summary => summary.Month)
            .ToList();

        return topMovies;
    }
    public List<MonthlyTopMovieSummary> GetTopMoviesByMonth_Optimized()
    {
        // Key: (Year, Month, MovieId), Value: rental count
        var rentalCounts = new Dictionary<(int Year, int Month, int MovieId), int>();

        foreach (var rental in Rentals)
        {
            var key = (rental.RentalDate.Year, rental.RentalDate.Month, rental.MovieId);
            if (!rentalCounts.ContainsKey(key))
                rentalCounts[key] = 0;
            rentalCounts[key]++;
        }

        // Key: (Year, Month), Value: (MovieId, RentalCount)
        var topByMonth = new Dictionary<(int Year, int Month), (int MovieId, int RentalCount)>();

        foreach (var kvp in rentalCounts)
        {
            var ym = (kvp.Key.Year, kvp.Key.Month);
            if (!topByMonth.ContainsKey(ym) || kvp.Value > topByMonth[ym].RentalCount)
            {
                topByMonth[ym] = (kvp.Key.MovieId, kvp.Value);
            }
        }

        // Map to MonthlyTopMovieSummary
        var result = topByMonth
            .Select(kvp =>
            {
                var movie = Movies.First(m => m.Id == kvp.Value.MovieId);
                return new MonthlyTopMovieSummary
                {
                    Month = new DateTime(kvp.Key.Year, kvp.Key.Month, 1).ToString("MMMM yyyy"),
                    MovieTitle = movie.Title,
                    RentalCount = kvp.Value.RentalCount
                };
            })
            .OrderBy(x => x.Month)
            .ToList();

        return result;
    }

}

/*
 !=== Monthly Top Movies ===
February 2024: Interstellar (Rentals: 2)
January 2024: Inception (Rentals: 2)
 */

/*
 !This exercise identifies the most popular rented movies each month based on rental frequency.

* 1.Grouping by Month and Movie:

GroupBy(rental => new { rental.RentalDate.Year, rental.RentalDate.Month, rental.MovieId }) groups rentals by month and movie.

* 2.Counting Rental Frequency:

Count() calculates the number of rentals for each movie within each month.

* 3.Selecting Most Rented Movie per Month:

OrderByDescending(g => g.RentalCount).First() selects the most rented movie for each month.

* 4.Returning the Summary:

The result is a list of MonthlyTopMovieSummary objects, sorted by Month in ascending order.
 */