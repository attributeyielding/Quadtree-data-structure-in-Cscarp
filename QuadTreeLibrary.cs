
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


using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Represents a 2D point with X and Y coordinates.
/// </summary>
public class Point
{
    /// <summary>
    /// Gets the X coordinate of the point.
    /// </summary>
    public float X { get; }

    /// <summary>
    /// Gets the Y coordinate of the point.
    /// </summary>
    public float Y { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Point"/> class.
    /// </summary>
    /// <param name="x">The X coordinate.</param>
    /// <param name="y">The Y coordinate.</param>
    public Point(float x, float y)
    {
        X = x;
        Y = y;
    }
}

/// <summary>
/// Represents a 2D rectangle with X, Y, Width, and Height.
/// </summary>
public class Rectangle
{
    /// <summary>
    /// Gets the X coordinate of the rectangle's top-left corner.
    /// </summary>
    public float X { get; }

    /// <summary>
    /// Gets the Y coordinate of the rectangle's top-left corner.
    /// </summary>
    public float Y { get; }

    /// <summary>
    /// Gets the width of the rectangle.
    /// </summary>
    public float Width { get; }

    /// <summary>
    /// Gets the height of the rectangle.
    /// </summary>
    public float Height { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Rectangle"/> class.
    /// </summary>
    /// <param name="x">The X coordinate of the top-left corner.</param>
    /// <param name="y">The Y coordinate of the top-left corner.</param>
    /// <param name="width">The width of the rectangle.</param>
    /// <param name="height">The height of the rectangle.</param>
    /// <exception cref="ArgumentException">Thrown if width or height is not positive.</exception>
    public Rectangle(float x, float y, float width, float height)
    {
        // Validate width and height
        if (width <= 0 || height <= 0)
        {
            throw new ArgumentException("Width and height must be positive.");
        }

        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    /// <summary>
    /// Checks if the rectangle contains a given point.
    /// </summary>
    /// <param name="point">The point to check.</param>
    /// <returns>True if the point is inside the rectangle, otherwise false.</returns>
    public bool Contains(Point point)
    {
        return point.X >= X && point.X <= X + Width &&
               point.Y >= Y && point.Y <= Y + Height;
    }

    /// <summary>
    /// Checks if the rectangle intersects with another rectangle.
    /// </summary>
    /// <param name="range">The rectangle to check for intersection.</param>
    /// <returns>True if the rectangles intersect, otherwise false.</returns>
    public bool Intersects(Rectangle range)
    {
        return !(range.X > X + Width ||
                 range.X + range.Width < X ||
                 range.Y > Y + Height ||
                 range.Y + range.Height < Y);
    }

    /// <summary>
    /// Checks if the rectangle fully contains another rectangle.
    /// </summary>
    /// <param name="range">The rectangle to check.</param>
    /// <returns>True if the rectangle is fully contained, otherwise false.</returns>
    public bool Contains(Rectangle range)
    {
        return range.X >= X && range.X + range.Width <= X + Width &&
               range.Y >= Y && range.Y + range.Height <= Y + Height;
    }
}

/// <summary>
/// Represents an object stored in the Quadtree.
/// </summary>
public class QuadtreeObject
{
    /// <summary>
    /// Gets the position of the object.
    /// </summary>
    public Point Position { get; }

    /// <summary>
    /// Gets the bounds of the object.
    /// </summary>
    public Rectangle Bounds { get; }

    /// <summary>
    /// Gets the data associated with the object.
    /// </summary>
    public object Data { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="QuadtreeObject"/> class.
    /// </summary>
    /// <param name="position">The position of the object.</param>
    /// <param name="bounds">The bounds of the object.</param>
    /// <param name="data">The data associated with the object.</param>
    /// <exception cref="ArgumentNullException">Thrown if position or bounds is null.</exception>
    public QuadtreeObject(Point position, Rectangle bounds, object data = null)
    {
        Position = position ?? throw new ArgumentNullException(nameof(position));
        Bounds = bounds ?? throw new ArgumentNullException(nameof(bounds));
        Data = data;
    }
}

/// <summary>
/// Represents a Quadtree data structure for spatial partitioning.
/// </summary>
public class Quadtree
{
    private readonly int capacity; // Maximum number of objects per node
    private readonly int maxDepth; // Maximum depth of the tree

    private readonly Rectangle boundary; // Boundary of the current node
    private readonly List<QuadtreeObject> objects = new List<QuadtreeObject>(); // Objects in the current node
    private bool divided; // Whether the node has been subdivided
    private readonly int depth; // Current depth of the node

    private Quadtree northeast; // Northeast child node
    private Quadtree northwest; // Northwest child node
    private Quadtree southeast; // Southeast child node
    private Quadtree southwest; // Southwest child node

    /// <summary>
    /// Initializes a new instance of the <see cref="Quadtree"/> class.
    /// </summary>
    /// <param name="boundary">The boundary of the Quadtree node.</param>
    /// <param name="capacity">The maximum number of objects per node.</param>
    /// <param name="maxDepth">The maximum depth of the tree.</param>
    /// <param name="depth">The current depth of the node.</param>
    /// <exception cref="ArgumentNullException">Thrown if boundary is null.</exception>
    /// <exception cref="ArgumentException">Thrown if capacity or maxDepth is not positive.</exception>
    public Quadtree(Rectangle boundary, int capacity = 4, int maxDepth = 8, int depth = 0)
    {
        if (boundary == null)
        {
            throw new ArgumentNullException(nameof(boundary));
        }

        if (capacity <= 0)
        {
            throw new ArgumentException("Capacity must be positive.");
        }

        if (maxDepth <= 0)
        {
            throw new ArgumentException("MaxDepth must be positive.");
        }

        this.boundary = boundary;
        this.capacity = capacity;
        this.maxDepth = maxDepth;
        this.depth = depth;
    }

    /// <summary>
    /// Inserts an object into the Quadtree.
    /// </summary>
    /// <param name="obj">The object to insert.</param>
    /// <returns>The updated Quadtree.</returns>
    /// <exception cref="ArgumentNullException">Thrown if obj is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if obj is outside the Quadtree boundary.</exception>
    public Quadtree Insert(QuadtreeObject obj)
    {
        if (obj == null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        // Check if the object is within the boundary of this node
        if (!boundary.Contains(obj.Position))
        {
            throw new ArgumentOutOfRangeException(nameof(obj), "Object is outside the Quadtree boundary.");
        }

        // If the node is not full or max depth is reached, add the object to this node
        if (objects.Count < capacity || depth >= maxDepth)
        {
            objects.Add(obj);
            return this;
        }

        // If the node is not subdivided, subdivide it
        if (!divided)
        {
            Subdivide();
        }

        // Insert the object into the appropriate child node
        northeast.Insert(obj);
        northwest.Insert(obj);
        southeast.Insert(obj);
        southwest.Insert(obj);

        return this;
    }

    /// <summary>
    /// Inserts an object into the Quadtree asynchronously.
    /// </summary>
    /// <param name="obj">The object to insert.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task<Quadtree> InsertAsync(QuadtreeObject obj)
    {
        return await Task.Run(() => Insert(obj));
    }

    /// <summary>
    /// Queries the Quadtree for objects within a specified range.
    /// </summary>
    /// <param name="range">The range to query.</param>
    /// <returns>A list of objects within the range.</returns>
    /// <exception cref="ArgumentNullException">Thrown if range is null.</exception>
    public List<QuadtreeObject> Query(Rectangle range)
    {
        if (range == null)
        {
            throw new ArgumentNullException(nameof(range));
        }

        var found = new List<QuadtreeObject>();

        // If the range does not intersect with this node's boundary, return an empty list
        if (!boundary.Intersects(range))
        {
            return found;
        }

        // If the range fully contains this node's boundary, return all objects in this node
        if (range.Contains(boundary))
        {
            found.AddRange(objects);
            return found;
        }

        // Check each object in this node to see if it intersects with the range
        foreach (var obj in objects)
        {
            if (range.Intersects(obj.Bounds))
            {
                found.Add(obj);
            }
        }

        // If the node is subdivided, query the child nodes
        if (divided)
        {
            found.AddRange(northeast.Query(range));
            found.AddRange(northwest.Query(range));
            found.AddRange(southeast.Query(range));
            found.AddRange(southwest.Query(range));
        }

        return found;
    }

    /// <summary>
    /// Queries the Quadtree for objects within a specified range asynchronously.
    /// </summary>
    /// <param name="range">The range to query.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task<List<QuadtreeObject>> QueryAsync(Rectangle range)
    {
        return await Task.Run(() => Query(range));
    }

    /// <summary>
    /// Removes an object from the Quadtree.
    /// </summary>
    /// <param name="obj">The object to remove.</param>
    /// <returns>The updated Quadtree.</returns>
    /// <exception cref="ArgumentNullException">Thrown if obj is null.</exception>
    public Quadtree Remove(QuadtreeObject obj)
    {
        if (obj == null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        // If the object is not within this node's boundary, return the current node
        if (!boundary.Contains(obj.Position))
        {
            return this;
        }

        // Remove the object from this node
        objects.Remove(obj);

        // If the node is subdivided, remove the object from the child nodes
        if (divided)
        {
            northeast.Remove(obj);
            northwest.Remove(obj);
            southeast.Remove(obj);
            southwest.Remove(obj);
        }

        return this;
    }

    /// <summary>
    /// Removes an object from the Quadtree asynchronously.
    /// </summary>
    /// <param name="obj">The object to remove.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task<Quadtree> RemoveAsync(QuadtreeObject obj)
    {
        return await Task.Run(() => Remove(obj));
    }

    /// <summary>
    /// Updates the position of an object in the Quadtree.
    /// </summary>
    /// <param name="obj">The object to update.</param>
    /// <param name="newPosition">The new position of the object.</param>
    /// <returns>The updated Quadtree.</returns>
    /// <exception cref="ArgumentNullException">Thrown if obj or newPosition is null.</exception>
    public Quadtree Update(QuadtreeObject obj, Point newPosition)
    {
        if (obj == null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        if (newPosition == null)
        {
            throw new ArgumentNullException(nameof(newPosition));
        }

        // Remove the object from its current position
        var updatedTree = Remove(obj);

        // Create a new object with the updated position
        var updatedObj = new QuadtreeObject(newPosition, obj.Bounds, obj.Data);

        // Insert the updated object into the Quadtree
        return updatedTree.Insert(updatedObj);
    }

    /// <summary>
    /// Updates the position of an object in the Quadtree asynchronously.
    /// </summary>
    /// <param name="obj">The object to update.</param>
    /// <param name="newPosition">The new position of the object.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task<Quadtree> UpdateAsync(QuadtreeObject obj, Point newPosition)
    {
        return await Task.Run(() => Update(obj, newPosition));
    }

    /// <summary>
    /// Clears all objects and subdivisions from the Quadtree.
    /// </summary>
    /// <returns>The cleared Quadtree.</returns>
    public Quadtree Clear()
    {
        objects.Clear();
        divided = false;
        northeast = northwest = southeast = southwest = null;
        return this;
    }

    /// <summary>
    /// Subdivides the current node into four child nodes.
    /// </summary>
    private void Subdivide()
    {
        float x = boundary.X;
        float y = boundary.Y;
        float w = boundary.Width / 2;
        float h = boundary.Height / 2;

        // Create boundaries for the four child nodes
        var northeastBoundary = new Rectangle(x + w, y, w, h);
        var northwestBoundary = new Rectangle(x, y, w, h);
        var southeastBoundary = new Rectangle(x + w, y + h, w, h);
        var southwestBoundary = new Rectangle(x, y + h, w, h);

        // Create the four child nodes
        northeast = new Quadtree(northeastBoundary, capacity, maxDepth, depth + 1);
        northwest = new Quadtree(northwestBoundary, capacity, maxDepth, depth + 1);
        southeast = new Quadtree(southeastBoundary, capacity, maxDepth, depth + 1);
        southwest = new Quadtree(southwestBoundary, capacity, maxDepth, depth + 1);

        divided = true;
    }

    /// <summary>
    /// Visualizes the Quadtree structure.
    /// </summary>
    /// <param name="sb">The StringBuilder to store the visualization.</param>
    /// <param name="indent">The indentation level for formatting.</param>
    public void Visualize(StringBuilder sb = null, int indent = 0)
    {
        if (sb == null)
        {
            sb = new StringBuilder();
        }

        // Add indentation for formatting
        sb.Append(' ', indent * 2);

        // Append the current node's boundary and object count
        sb.AppendLine($"Node: Boundary={boundary.X}, {boundary.Y}, {boundary.Width}, {boundary.Height}, Objects={objects.Count}");

        // If the node is subdivided, visualize the child nodes
        if (divided)
        {
            northeast.Visualize(sb, indent + 1);
            northwest.Visualize(sb, indent + 1);
            southeast.Visualize(sb, indent + 1);
            southwest.Visualize(sb, indent + 1);
        }

        // If this is the root node, print the visualization
        if (indent == 0)
        {
            Console.WriteLine(sb.ToString());
        }
    }
}