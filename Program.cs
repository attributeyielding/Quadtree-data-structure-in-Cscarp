/// <summary>
/// A Quadtree data structure in C# is used for spatial partitioning, which is useful for efficiently querying objects in a 2D space.
/// </summary>
/// <remarks>
/// Author: Bilel Mnasser
/// Contact: personalhiddenmail@duck.com
/// GitHub: https://github.com/attributeyielding
/// website: https://personal-website-resume.netlify.app/#contact
/// Date: January 2025
/// Version: 1.0
/// </remarks>

/// <summary>
/// Entry point for the Quadtree demonstration.
/// </summary>
class Program
{
    static async Task Main(string[] args)
    {
        // Define the boundary of the Quadtree
        Rectangle boundary = new Rectangle(0, 0, 100, 100);

        // Create a new Quadtree with the specified boundary
        Quadtree quadtree = new Quadtree(boundary, capacity: 4, maxDepth: 8);

        // Create two objects to insert into the Quadtree
        var obj1 = new QuadtreeObject(new Point(10, 10), new Rectangle(10, 10, 5, 5), "Object 1");
        var obj2 = new QuadtreeObject(new Point(20, 20), new Rectangle(20, 20, 5, 5), "Object 2");

        // Insert the objects into the Quadtree asynchronously
        quadtree = await quadtree.InsertAsync(obj1);
        quadtree = await quadtree.InsertAsync(obj2);

        // Define a query range
        Rectangle queryRange = new Rectangle(0, 0, 30, 30);

        // Query the Quadtree for objects within the range
        var objectsInRange = await quadtree.QueryAsync(queryRange);

        // Print the objects found in the range
        Console.WriteLine("Objects in range:");
        foreach (var obj in objectsInRange)
        {
            Console.WriteLine($"({obj.Position.X}, {obj.Position.Y}) - {obj.Data}");
        }

        // Remove one object from the Quadtree
        quadtree = await quadtree.RemoveAsync(obj2);

        // Query the Quadtree again after removal
        Console.WriteLine("\nAfter removing Object 2:");
        objectsInRange = await quadtree.QueryAsync(queryRange);
        foreach (var obj in objectsInRange)
        {
            Console.WriteLine($"({obj.Position.X}, {obj.Position.Y}) - {obj.Data}");
        }

        // Visualize the Quadtree structure
        quadtree.Visualize();
    }
}