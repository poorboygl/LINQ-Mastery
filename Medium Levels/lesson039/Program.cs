class Program
{
    static void Main()
    {
        var repo = new TransactionRepository();

        var summaries = repo.GetCustomerTransactionSummary();

        foreach (var summary in summaries)
        {
            Console.WriteLine($"Customer: {summary.CustomerName}");
            Console.WriteLine($"  Total Spent: {summary.TotalSpent}");
            Console.WriteLine($"  Average Transaction: {summary.AverageTransactionAmount}");
            Console.WriteLine($"  Transaction Count: {summary.TransactionCount}");
            Console.WriteLine();
        }

        Console.ReadLine();
    }
}
public record Customer
{
    public int Id { get; set; }
    public required string Name { get; set; }
}

public record Transaction
{
    public int CustomerId { get; set; }
    public decimal Amount { get; set; }
}

public record CustomerTransactionSummary
{
    public required string CustomerName { get; set; }
    public decimal TotalSpent { get; set; }
    public decimal AverageTransactionAmount { get; set; }
    public int TransactionCount { get; set; }
}

public record TransactionRepository
{
    public List<Customer> Customers { get; set; } =
    [
        new Customer { Id = 1, Name = "Alice" },
        new Customer { Id = 2, Name = "Bob" },
        new Customer { Id = 3, Name = "Charlie" }
    ];

    public List<Transaction> Transactions { get; set; } =
    [
        new Transaction { CustomerId = 1, Amount = 100.00m },
        new Transaction { CustomerId = 1, Amount = 200.00m },
        new Transaction { CustomerId = 2, Amount = 150.00m },
        new Transaction { CustomerId = 3, Amount = 75.00m },
        new Transaction { CustomerId = 3, Amount = 125.00m }
    ];

    public List<CustomerTransactionSummary> GetCustomerTransactionSummary()
    {
        return [.. Customers
            .GroupJoin(Transactions,
                customer => customer.Id,
                transaction => transaction.CustomerId,
                (customer, customerTransactions) => new CustomerTransactionSummary
                {
                    CustomerName = customer.Name,
                    TotalSpent = customerTransactions.Sum(t => t.Amount),
                    AverageTransactionAmount = customerTransactions.Any() ? customerTransactions.Average(t => t.Amount) : 0,
                    TransactionCount = customerTransactions.Count()
                })];
    }
}

/*
 Customer: Alice
  Total Spent: 300.00
  Average Transaction: 150.00
  Transaction Count: 2

Customer: Bob
  Total Spent: 150.00
  Average Transaction: 150.00
  Transaction Count: 1

Customer: Charlie
  Total Spent: 200.00
  Average Transaction: 100.00
  Transaction Count: 2
 */


/*
This exercise requires complex data analysis across two collections, utilizing GroupJoin to gather customer-specific transactions.

1.Using GroupJoin to Associate Transactions with Customers:

    GroupJoin(Transactions, customer => customer.Id, transaction => transaction.CustomerId, ...) associates each customer with their related transactions, creating a collection of customerTransactions for each customer.

2.Calculating Customer-Specific Aggregates:

    TotalSpent: Sums the Amount of all transactions within customerTransactions.

    AverageTransactionAmount: Uses .Any() to check if transactions exist, then calculates the average amount.

    TransactionCount: Counts the number of transactions for each customer.

3.Returning the Report:

    The final result is a list of CustomerTransactionSummary objects, where each entry contains the customer’s name, total spent, average transaction amount, and transaction count
 
 */