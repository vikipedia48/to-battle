using System;
using System.Collections.Generic;
using System.Drawing;
using Priority_Queue;
using UnityEngine;

// A* needs only a WeightedGraph and a Point type L, and does *not*
// have to be a grid. However, in the example code I am using a grid.
public interface WeightedGraph<L>
{
    
    double Cost(Point a, Point b);
    IEnumerable<Point> Neighbors(Point id, Func<int, int, bool> IsPassable);
}

public class SquareGrid : WeightedGraph<Point>
{
    // Implementation notes: I made the fields public for convenience,
    // but in a real project you'll probably want to follow standard
    // style and make them private.
    
    public static readonly Point[] DIRS = new []
        {
            new Point(1, 0),
            new Point(0, -1),
            new Point(-1, 0),
            new Point(0, 1)
        };

    public int width, height;

    public SquareGrid(int width, int height)
    {
        this.width = width;
        this.height = height;
    }

    public bool InBounds(Point id)
    {
        return 0 <= id.X && id.X < width
            && 0 <= id.Y && id.Y < height;
    }

    public bool Passable(Point id, Func<int, int, bool> IsPassable)
    {
        return IsPassable(id.X,id.Y);
    }

    public double Cost(Point a, Point b)
    {
        return 1;
    }
    
    public IEnumerable<Point> Neighbors(Point id, Func<int, int, bool> IsPassable)
    {
        foreach (var dir in DIRS) {
            Point next = new Point(id.X + dir.X, id.Y + dir.Y);
            if (InBounds(next) && Passable(next, IsPassable)) {
                yield return next;
            }
        }
    }
}

public class AStarSearch
{
    public Dictionary<Point, Point> cameFrom
        = new Dictionary<Point, Point>();
    public Dictionary<Point, double> costSoFar
        = new Dictionary<Point, double>();

    private Point start, goal; 

    // Note: a generic version of A* would abstract over Point and
    // also Heuristic
    static public double Heuristic(Point a, Point b)
    {
        return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
    }

    public AStarSearch(WeightedGraph<Point> graph, Point start, Point goal, Func<int, int, bool> IsPassable)
    {
        this.start = start;
        this.goal = goal;
        var frontier = new SimplePriorityQueue<Point, double>();
        frontier.Enqueue(start, 0);

        cameFrom[start] = start;
        costSoFar[start] = 0;

        while (frontier.Count > 0)
        {
            var current = frontier.Dequeue();

            if (current.Equals(goal))
            {
                break;
            }

            foreach (var next in graph.Neighbors(current, IsPassable))
            {
                double newCost = costSoFar[current]
                    + graph.Cost(current, next);
                if (!costSoFar.ContainsKey(next)
                    || newCost < costSoFar[next])
                {
                    costSoFar[next] = newCost;
                    double priority = newCost + Heuristic(next, goal);
                    frontier.Enqueue(next, priority);
                    cameFrom[next] = current;
                }
            }
        }
    }

    public List<Point> GetPath()
    {
        var rv = new List<Point>();
        var current = this.goal;
        if (!cameFrom.ContainsKey(current)) return rv;
        rv.Add(this.goal);
        while (!current.Equals(start))
        {
            current = this.cameFrom[current];
            rv.Add(current);
        }
        rv.Reverse();
        return rv;
    }
}
