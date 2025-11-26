public class Program
{
    static void Main()
    {
        var repo = new CompanyRepository();

        Console.WriteLine("=== Skill Comparison Across Departments ===\n");

        var results = repo.GetSkillAnalysisByDepartment_optimaze();

        foreach (var result in results)
        {
            Console.WriteLine($"Department: {result.DepartmentName}");
            Console.WriteLine($"  Unique Skills:");
            foreach (var skill in result.UniqueSkills)
            {
                Console.WriteLine($"    - {skill}");
            }

            Console.WriteLine($"  Shared Skills Across Departments:");
            foreach (var skill in result.SharedSkillsAcrossDepartments)
            {
                Console.WriteLine($"    - {skill}");
            }

            Console.WriteLine(new string('-', 50));
        }

        Console.ReadKey();
    }
}
public class Department
{
    public int Id { get; set; }
    public required string Name { get; set; }
}

public class Employee
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int DepartmentId { get; set; }
    public required List<string> Skills { get; set; }
}

public class Skill
{
    public required string Name { get; set; }
}

public class DepartmentSkillAnalysis
{
    public required string DepartmentName { get; set; }
    public required List<string> UniqueSkills { get; set; }
    public required List<string> SharedSkillsAcrossDepartments { get; set; }
}

public class CompanyRepository
{
    public List<Department> Departments { get; set; } =
    [
        new Department { Id = 1, Name = "Engineering" },
        new Department { Id = 2, Name = "Marketing" },
        new Department { Id = 3, Name = "Sales" }
    ];

    public List<Employee> Employees { get; set; } =
    [
        new Employee { Id = 1, Name = "Alice", DepartmentId = 1, Skills = ["C#", "SQL", "Machine Learning"] },
        new Employee { Id = 2, Name = "Bob", DepartmentId = 1, Skills = ["C#", "Azure", "SQL"] },
        new Employee { Id = 3, Name = "Charlie", DepartmentId = 2, Skills = ["Marketing Strategy", "Project Management", "SEO"] },
        new Employee { Id = 4, Name = "Diana", DepartmentId = 3, Skills = ["Salesforce", "Negotiation", "Project Management"] }
    ];

    public List<Skill> Skills { get; set; } =
    [
        new Skill { Name = "C#" },
        new Skill { Name = "SQL" },
        new Skill { Name = "Machine Learning" },
        new Skill { Name = "Azure" },
        new Skill { Name = "Marketing Strategy" },
        new Skill { Name = "Project Management" },
        new Skill { Name = "SEO" },
        new Skill { Name = "Salesforce" },
        new Skill { Name = "Negotiation" }
    ];

   public List<DepartmentSkillAnalysis> GetSkillAnalysisByDepartment()
    {
        /*
            !=== Skill Comparison Across Departments ===

            Department: Engineering
              Unique Skills:
                - C#
                - SQL
                - Machine Learning
                - Azure
              Shared Skills Across Departments:
            --------------------------------------------------
            Department: Marketing
              Unique Skills:
                - Marketing Strategy
                - SEO
              Shared Skills Across Departments:
                - Project Management
            --------------------------------------------------
            Department: Sales
              Unique Skills:
                - Salesforce
                - Negotiation
              Shared Skills Across Departments:
                - Project Management
            --------------------------------------------------
         */

        var departmentSkills = Departments
            .Select(department =>
            {
                var allDepartmentSkills = Employees
                    .Where(e => e.DepartmentId == department.Id)
                    .SelectMany(e => e.Skills)
                    .ToHashSet();

                var uniqueSkills = allDepartmentSkills
                    .Except(Employees
                        .Where(e => e.DepartmentId != department.Id)
                        .SelectMany(e => e.Skills)
                        .Distinct())
                    .ToList();

                var sharedSkillsAcrossDepartments = allDepartmentSkills
                    .Intersect(Employees
                        .Where(e => e.DepartmentId != department.Id)
                        .SelectMany(e => e.Skills)
                        .Distinct())
                    .ToList();

                return new DepartmentSkillAnalysis
                {
                    DepartmentName = department.Name,
                    UniqueSkills = uniqueSkills,
                    SharedSkillsAcrossDepartments = sharedSkillsAcrossDepartments
                };
            })
            .ToList();

        return departmentSkills;   
    }

    public List<DepartmentSkillAnalysis> GetSkillAnalysisByDepartment_2()
    {

        // 1) Build map department -> skills set (done once)
        var skillsByDept = Employees
            .GroupBy(e => e.DepartmentId)
            .ToDictionary(
                g => g.Key,
                g => g.SelectMany(e => e.Skills).ToHashSet()
            );

        // 2) For each department compute unique/shared
        var departmentSkills = Departments.Select(dept =>
        {
            var thisSet = skillsByDept.GetValueOrDefault(dept.Id, new HashSet<string>());

            // skills in other depts (union)
            var otherUnion = skillsByDept
                .Where(kv => kv.Key != dept.Id)
                .SelectMany(kv => kv.Value)
                .ToHashSet();

            var unique = thisSet.Except(otherUnion).ToList();
            var shared = thisSet.Intersect(otherUnion).ToList();

            return new DepartmentSkillAnalysis
            {
                DepartmentName = dept.Name,
                UniqueSkills = unique,
                SharedSkillsAcrossDepartments = shared
            };
        }).ToList();

        return departmentSkills;
    }

    public List<DepartmentSkillAnalysis> GetSkillAnalysisByDepartment_optimaze()
    {
        /*
           !=== Skill Comparison Across Departments ===

            Department: Engineering
              Unique Skills:
                - C#
                - SQL
                - Machine Learning
                - Azure
              Shared Skills Across Departments:
            --------------------------------------------------
            Department: Marketing
              Unique Skills:
                - Marketing Strategy
                - SEO
              Shared Skills Across Departments:
                - Project Management
            --------------------------------------------------
            Department: Sales
              Unique Skills:
                - Salesforce
                - Negotiation
              Shared Skills Across Departments:
                - Project Management
            --------------------------------------------------
         */

        // 1) Pre-calc skills per department using HashSet
        var deptSkills = Departments.ToDictionary(
            d => d.Id,
            d => Employees
                    .Where(e => e.DepartmentId == d.Id)
                    .SelectMany(e => e.Skills)
                    .ToHashSet()
        );

        var result = new List<DepartmentSkillAnalysis>(Departments.Count);

        foreach (var d in Departments)
        {
            var skills = deptSkills[d.Id];

            // otherSkills = union of skills of all other departments
            var otherSkills = new HashSet<string>();
            foreach (var kv in deptSkills)
            {
                if (kv.Key == d.Id) continue;
                otherSkills.UnionWith(kv.Value);
            }

            // unique = skills - otherSkills
            var uniqueSkills = new HashSet<string>(skills);
            uniqueSkills.ExceptWith(otherSkills);

            // shared = skills ∩ otherSkills
            var sharedSkills = new HashSet<string>(skills);
            sharedSkills.IntersectWith(otherSkills);

            result.Add(new DepartmentSkillAnalysis
            {
                DepartmentName = d.Name,
                UniqueSkills = uniqueSkills.ToList(),
                SharedSkillsAcrossDepartments = sharedSkills.ToList()
            });
        }

        return result;
    }
}

/*

!This exercise generates a report showing unique and shared skills for each department.

* 1.Flattening Skills per Department:

SelectMany(e => e.Skills).Distinct() flattens the list of skills for each employee within a department.

* 2.Calculating Unique and Shared Skills:

UniqueSkills: Except finds skills that are unique to the department.

SharedSkillsAcrossDepartments: Intersect identifies skills shared across departments.

* 3.Returning the Report:

The result is a list of DepartmentSkillAnalysis objects showing each department’s unique and shared skills.
 
*/