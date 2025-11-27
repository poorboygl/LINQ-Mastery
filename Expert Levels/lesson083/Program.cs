public class Program
{
    static void Main()
    {
        var repo = new SkillCoverageRepository();
        var results = repo.GetMinimalSkillCoverage_Optimized();

        Console.WriteLine("=== Minimal Skill Coverage Summary ===\n");

        foreach (var item in results)
        {
            Console.WriteLine($"Skill: {item.SkillName} | Departments Covered: {item.DepartmentsCovered}");
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
    public required List<string> Skills { get; set; }
}

public class ProjectRequirement
{
    public int DepartmentId { get; set; }
    public required List<string> RequiredSkills { get; set; }
}

public class SkillCoverageSummary
{
    public required string SkillName { get; set; }
    public int DepartmentsCovered { get; set; }
}

public class SkillCoverageRepository
{
    public List<Department> Departments { get; set; } =
    [
        new Department { Id = 1, Name = "Engineering" },
        new Department { Id = 2, Name = "Marketing" },
        new Department { Id = 3, Name = "Sales" }
    ];

    public List<Employee> Employees { get; set; } =
    [
        new Employee { Id = 1, Name = "Alice", Skills = new List<string> { "C#", "SQL", "Data Analysis" } },
        new Employee { Id = 2, Name = "Bob", Skills = new List<string> { "C#", "Azure", "Project Management" } },
        new Employee { Id = 3, Name = "Charlie", Skills = new List<string> { "Marketing Strategy", "SEO", "Project Management" } },
        new Employee { Id = 4, Name = "Diana", Skills = new List<string> { "Salesforce", "Negotiation", "Data Analysis" } }
    ];

    public List<ProjectRequirement> ProjectRequirements { get; set; } =
    [
        new ProjectRequirement { DepartmentId = 1, RequiredSkills = new List<string> { "C#", "SQL" } },
        new ProjectRequirement { DepartmentId = 2, RequiredSkills = new List<string> { "Marketing Strategy", "Project Management" } },
        new ProjectRequirement { DepartmentId = 3, RequiredSkills = new List<string> { "Salesforce", "Negotiation" } }
    ];

    public List<SkillCoverageSummary> GetMinimalSkillCoverage()
    {
        var requiredSkills = ProjectRequirements
            .SelectMany(r => r.RequiredSkills)
            .Distinct()
            .ToList();

        var minimalSkillSet = new List<SkillCoverageSummary>();
        var uncoveredSkills = new HashSet<string>(requiredSkills);

        while (uncoveredSkills.Count > 0)
        {
            var bestSkill = Employees
                .SelectMany(e => e.Skills)
                .Distinct()
                .Where(skill => uncoveredSkills.Contains(skill))
                .Select(skill => new
                {
                    Skill = skill,
                    DepartmentsCovered = ProjectRequirements
                        .Count(req => req.RequiredSkills.Contains(skill))
                })
                .OrderByDescending(s => s.DepartmentsCovered)
                .FirstOrDefault();

            if (bestSkill == null)
                break;

            minimalSkillSet.Add(new SkillCoverageSummary
            {
                SkillName = bestSkill.Skill,
                DepartmentsCovered = bestSkill.DepartmentsCovered
            });

            uncoveredSkills.ExceptWith(ProjectRequirements
                .Where(req => req.RequiredSkills.Contains(bestSkill.Skill))
                .SelectMany(req => req.RequiredSkills)
                .Distinct());
        }

        return minimalSkillSet
            .OrderByDescending(summary => summary.DepartmentsCovered)
            .ToList();
    }

    public List<SkillCoverageSummary> GetMinimalSkillCoverage_Optimized()
    {
        // ---- PREPROCESS: skill -> departments ----
        var skillToDepartments = new Dictionary<string, HashSet<int>>();

        foreach (var req in ProjectRequirements)
        {
            foreach (var skill in req.RequiredSkills)
            {
                if (!skillToDepartments.ContainsKey(skill))
                    skillToDepartments[skill] = new HashSet<int>();

                skillToDepartments[skill].Add(req.DepartmentId);
            }
        }

        // ---- Set skills cần bao phủ ----
        var uncoveredSkills = new HashSet<string>(
            ProjectRequirements.SelectMany(r => r.RequiredSkills)
        );

        var result = new List<SkillCoverageSummary>();

        // ---- Lặp tới khi cover hết ----
        while (uncoveredSkills.Count > 0)
        {
            // Tìm skill nào cover nhiều Department nhất
            var bestSkill = uncoveredSkills
                .Where(skillToDepartments.ContainsKey)
                .Select(s => new
                {
                    Skill = s,
                    Coverage = skillToDepartments[s].Count
                })
                .OrderByDescending(s => s.Coverage)
                .FirstOrDefault();

            if (bestSkill == null)
                break;

            // Thêm vào kết quả
            result.Add(new SkillCoverageSummary
            {
                SkillName = bestSkill.Skill,
                DepartmentsCovered = bestSkill.Coverage
            });

            // Loại bỏ tất cả skill thuộc các department đã cover
            var coveredDeps = skillToDepartments[bestSkill.Skill];

            // Tìm tất cả required skills thuộc những department này
            var removeSkills = ProjectRequirements
                .Where(r => coveredDeps.Contains(r.DepartmentId))
                .SelectMany(r => r.RequiredSkills)
                .ToList();

            uncoveredSkills.ExceptWith(removeSkills);
        }

        return result.OrderByDescending(r => r.DepartmentsCovered).ToList();
    }
}

/*
 !=== Minimal Skill Coverage Summary ===

    Skill: C# | Departments Covered: 1
    Skill: Project Management | Departments Covered: 1
    Skill: Salesforce | Departments Covered: 1
 */

/*
!This exercise identifies the minimum skill set required to cover all project requirements across departments.

* 1.Identify Required Skills:

SelectMany(r => r.RequiredSkills).Distinct() gathers all unique skills required across departments.

* 2.Determine Minimal Skill Coverage:

In each iteration, the skill that covers the most uncovered requirements across departments is added to the minimal set, and uncoveredSkills is updated accordingly.

* 3.Return the Report:

The result is a list of SkillCoverageSummary objects, showing the minimal set of skills to cover all departmental project requirements.
 
 */