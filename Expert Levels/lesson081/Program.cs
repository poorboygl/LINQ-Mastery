public class Program
{
    static void Main()
    {
        var repo = new MentorshipRepository();
        var longestChain = repo.GetLongestMentorshipChain_Optimized();

        Console.WriteLine("=== Longest Mentorship Chain ===\n");

        foreach (var link in longestChain)
        {
            Console.WriteLine($"{link.MentorName} -> {link.MenteeName}");
        }

        Console.WriteLine($"\nTotal chain length: {longestChain.Count + 1} employees");

        Console.ReadKey();
    }
}


public class Employee
{
    public int Id { get; set; }
    public required string Name { get; set; }
}

public class Mentorship
{
    public int MentorId { get; set; }
    public int MenteeId { get; set; }
}

public class EmployeeMentorshipChain
{
    public required string MentorName { get; set; }
    public required string MenteeName { get; set; }
}

public class MentorshipRepository
{
    public List<Employee> Employees { get; set; } =
    [
        new Employee { Id = 1, Name = "Alice" },
        new Employee { Id = 2, Name = "Bob" },
        new Employee { Id = 3, Name = "Charlie" },
        new Employee { Id = 4, Name = "Diana" },
        new Employee { Id = 5, Name = "Edward" }
    ];

    public List<Mentorship> Mentorships { get; set; } =
    [
        new Mentorship { MentorId = 1, MenteeId = 2 },
        new Mentorship { MentorId = 2, MenteeId = 3 },
        new Mentorship { MentorId = 3, MenteeId = 4 },
        new Mentorship { MentorId = 4, MenteeId = 5 }
    ];

    public List<EmployeeMentorshipChain> GetLongestMentorshipChain()
    {
        var mentorshipChains = new List<List<int>>();

        foreach (var mentor in Mentorships.Select(m => m.MentorId).Distinct())
        {
            var chain = new List<int> { mentor };
            var currentMentee = mentor;

            while (Mentorships.Any(m => m.MentorId == currentMentee))
            {
                currentMentee = Mentorships.First(m => m.MentorId == currentMentee).MenteeId;
                chain.Add(currentMentee);
            }

            mentorshipChains.Add(chain);
        }

        var longestChain = mentorshipChains.OrderByDescending(chain => chain.Count).First();

        return longestChain
            .Zip(longestChain.Skip(1), (mentorId, menteeId) => new EmployeeMentorshipChain
            {
                MentorName = Employees.First(e => e.Id == mentorId).Name,
                MenteeName = Employees.First(e => e.Id == menteeId).Name
            })
            .ToList();
    }


    public List<EmployeeMentorshipChain> GetLongestMentorshipChain_Optimized()
    {
        // Build dictionary: MentorId -> MenteeId (O(N))
        var next = Mentorships.ToDictionary(m => m.MentorId, m => m.MenteeId);

        // Build hashset of all mentees
        var allMentees = Mentorships.Select(m => m.MenteeId).ToHashSet();

        // Start point = mentor who is NOT a mentee → chain head
        var head = next.Keys.First(mentorId => !allMentees.Contains(mentorId));

        // Walk chain from head (O(N))
        var chainIds = new List<int>();
        var current = head;

        chainIds.Add(current);

        while (next.TryGetValue(current, out var mentee))
        {
            chainIds.Add(mentee);
            current = mentee;
        }

        // Convert to output format
        return chainIds
            .Zip(chainIds.Skip(1), (mentorId, menteeId) => new EmployeeMentorshipChain
            {
                MentorName = Employees.First(e => e.Id == mentorId).Name,
                MenteeName = Employees.First(e => e.Id == menteeId).Name
            })
            .ToList();
    }

    public List<EmployeeMentorshipChain> GetLongestMentorshipChain_Optimized_2()
    {
        var next = Mentorships.ToDictionary(m => m.MentorId, m => m.MenteeId);
        var allMentees = Mentorships.Select(m => m.MenteeId).ToHashSet();

        // head = mentor không là mentee
        var head = next.Keys.First(k => !allMentees.Contains(k));

        // build chain
        var chainIds = new List<int>();
        var current = head;
        while (next.TryGetValue(current, out var mentee))
        {
            chainIds.Add(current);
            current = mentee;
        }
        chainIds.Add(current); // add last mentee

        // Dictionary lookup EmployeeId -> Name
        var employeeLookup = Employees.ToDictionary(e => e.Id, e => e.Name);

        // convert to output
        return chainIds.Zip(chainIds.Skip(1), (mentorId, menteeId) => new EmployeeMentorshipChain
        {
            MentorName = employeeLookup[mentorId],
            MenteeName = employeeLookup[menteeId]
        }).ToList();
    }

}

/*
 !=== Longest Mentorship Chain ===

Alice -> Bob
Bob -> Charlie
Charlie -> Diana
Diana -> Edward

Total chain length: 5 employees
 
 */

/*
 !This exercise generates the longest mentorship chain by following mentorship relationships across multiple levels.

* 1.Building Chains:

For each mentor, a chain is constructed by iteratively finding mentees linked to the mentor. The process continues until no further mentee can be found, forming a complete chain.

* 2.Identifying the Longest Chain:

OrderByDescending(chain => chain.Count).First() identifies the chain with the highest number of mentorship connections.

* 3.Returning the Longest Chain in Sequence:

The result is a list of EmployeeMentorshipChain objects, showing mentor-mentee pairs in sequential order.
 
 */