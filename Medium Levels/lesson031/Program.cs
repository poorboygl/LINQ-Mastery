class Program
{
    static void Main()
    {
        var repo = new ProductRepository();

        var allComponents = repo.GetAllComponents();

        Console.WriteLine("== All Components ==");

        foreach (var component in allComponents)
        {
            Console.WriteLine($"Component {component.ComponentId}: {component.Name}");
        }

        Console.ReadLine();
    }
}

public class Component
{
    public int ComponentId { get; set; }
    public required string Name { get; set; }
}

public class Product
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public List<Component> Components { get; set; } = []; // Components of the product
}

public class ProductRepository
{
    public List<Product> Products { get; set; } =
    [
        new Product
        {
            Id = 1,
            Name = "Laptop",
            Components =
            [
                new Component { ComponentId = 1, Name = "Screen" },
                new Component { ComponentId = 2, Name = "Battery" }
            ]
        },
        new Product
        {
            Id = 2,
            Name = "Keyboard",
            Components =
            [
                new Component { ComponentId = 3, Name = "Keycap Set" },
                new Component { ComponentId = 4, Name = "Circuit Board" }
            ]
        }
    ];


    public List<Component> GetAllComponents()
    {
        return [.. Products.SelectMany(product => product.Components)]; // Add your code here
    }
}


/*
 == All Components ==
Component 1: Screen
Component 2: Battery
Component 3: Keycap Set
Component 4: Circuit Board
 */


/*
In this exercise, you are asked to complete the GetAllComponents method by using SelectMany. Here’s a breakdown of the solution:

* 1.Using SelectMany to Flatten Collections:

SelectMany(product => product.Components) flattens all Components lists across products into a single sequence of Component objects.

* 2.Converting to a List of Components:

The result of SelectMany is converted to List<Component> using ToList() to match the return type.

* 3.Example Execution:

Calling GetAllComponents() returns a single list of all components from all products, such as [Component(Name="Screen"), Component(Name="Battery"), ...].
 
 
 */