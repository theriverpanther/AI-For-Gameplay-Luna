using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEditor.PlayerSettings;

public class Movement : MonoBehaviour
{
    protected GridManager gridManager;

    public Rigidbody2D rb;

    [Header("Balancing Values")]
    public float maxSpeed = 10f;
    public float maxAcceleration;
    public float targetRadius = 0.2f;
    public float slowRadius;
    public float dt = 0.1f;

    private Vector2 currentVelocity;

    protected bool move = true;

    [SerializeField] protected Stack<Vector2> path = new Stack<Vector2>();
    [SerializeField] protected List<Vector2> pathList = new List<Vector2>();

    [Header("Destination")]
    [SerializeField] protected Vector3 seekPoint = Vector3.zero;
    [SerializeField] protected Vector3 lastSeekPoint = Vector3.zero;

    [Header("Pathfinding")]
    [SerializeField] private List<Tile> closedList = new List<Tile>();
    [SerializeField] private List<Tile> openList = new List<Tile>();

    protected const int STRAIGHT_COST = 10;
    protected const int DIAGONAL_COST = 14;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        currentVelocity = default;
        rb = GetComponent<Rigidbody2D>();
        gridManager = GridManager.Instance;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if(lastSeekPoint != seekPoint)
        {
            AStar();
            lastSeekPoint = seekPoint;
        }
        SteerToTarget();
    }

    public void SteerToTarget()
    {
        if(path.Count > 0) 
        {
            Vector2 direction = path.Peek() - (Vector2)transform.position;
            float distance = direction.magnitude;

            move = distance > targetRadius;

            if (move)
            {
                float targetSpeed = 0;
                Vector2 targetVelocity = default;

                if (distance > slowRadius)
                {
                    targetSpeed = maxSpeed;
                }
                else
                {
                    targetSpeed = maxSpeed * (distance / slowRadius);
                }

                targetVelocity = direction.normalized * targetSpeed;

                Vector2 acceleration = targetVelocity - rb.velocity;
                acceleration /= dt;

                if (acceleration.magnitude > maxAcceleration)
                {
                    acceleration = acceleration.normalized * maxAcceleration;
                }

                currentVelocity = new Vector2(
                    currentVelocity.x + acceleration.x * Time.deltaTime,
                    currentVelocity.y + acceleration.y * Time.deltaTime);

                FaceTarget(currentVelocity);

                rb.velocity = currentVelocity;
                //transform.position = new Vector3(transform.position.x + velocity.x * Time.deltaTime, transform.position.y + velocity.y * Time.deltaTime, transform.position.z);
            }
            else
            {
                path.Pop();
            }
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    protected void FaceTarget(Vector3 velocity)
    {
        if(velocity.magnitude > 0)
        {
            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
    }

    /// <summary>
    /// Handles the A* pathfinding whenever called
    /// Utilizes heuristics to navigate to the desired endpoint
    /// </summary>
    public virtual void AStar()
    {
        // Clean old logic
        openList.Clear();
        closedList.Clear();
        path.Clear();
        pathList.Clear();

        // Add starting node
        Tile startTile = gridManager.GetTileAtPoint(transform.position.x, transform.position.y);
        Tile endTile = gridManager.GetTileAtPoint(seekPoint.x, seekPoint.y);

        openList.Add(startTile);

        // Allocate to memory once to optimize
        Tile currentTile;
        for(int x = 0; x < gridManager.Width; x++) 
        { 
            for(int y = 0; y < gridManager.Height; y++) 
            {
                currentTile = gridManager.GetTileAtPoint(x,y);
                currentTile.gCost = int.MaxValue;
                currentTile.CalculateFCost();
                currentTile.prevTile = null;
            }
        }

        // Set up initial costs
        startTile.gCost = 0;
        startTile.hCost = CalculateDistanceCost(startTile, endTile);
        startTile.CalculateFCost();

        // Core loop logic, try to solve for a path through the open tiles
        while(openList.Count > 0)
        {
            currentTile = GetMinFCost(openList);
            if(currentTile.Equals(endTile))
            {
                // If current = destination, we have the path.
                path = CalculatePath(endTile);
                CleanPath();
            }

            openList.Remove(currentTile);
            closedList.Add(currentTile);
            List<Tile> neighbors = GetNeighbors(currentTile);

            foreach(Tile t in neighbors) 
            {
                if (closedList.Contains(t)) continue;
                if(!t.Walkable)
                {
                    closedList.Add(t);
                    continue;
                }

                int tempGCost = currentTile.gCost + CalculateDistanceCost(currentTile, t);
                if(tempGCost < t.gCost)
                {
                    t.prevTile = currentTile;
                    t.gCost = tempGCost;
                    t.hCost = CalculateDistanceCost(t, endTile);
                    t.CalculateFCost();

                    if(!openList.Contains(t)) openList.Add(t);
                }
            }
        }

        // Out of nodes
    }
    
    /// <summary>
    /// Finds valid neighbors of a given tile
    /// </summary>
    /// <param name="tile">Current tile that needs its neighbors known</param>
    /// <returns>List of all of the valid neighbors</returns>
    private List<Tile> GetNeighbors(Tile tile)
    { 
        List<Tile> neighbors = new List<Tile>();
        // Left
        if(tile.Position.x - 1 >= 0)
        {
            neighbors.Add(gridManager.GetTileAtPoint(tile.Position.x - 1, tile.Position.y));
            // Lower Left
            if (tile.Position.y - 1 >= 0)
            {
                neighbors.Add(gridManager.GetTileAtPoint(tile.Position.x - 1, tile.Position.y - 1));
            }
            // Upper Left
            if (tile.Position.y + 1 < gridManager.Height)
            {
                neighbors.Add(gridManager.GetTileAtPoint(tile.Position.x - 1, tile.Position.y + 1));
            }
        }
        // Check to the right
        if (tile.Position.x + 1 < gridManager.Width)
        {
            neighbors.Add(gridManager.GetTileAtPoint(tile.Position.x + 1, tile.Position.y));
            // Lower Right
            if (tile.Position.y - 1 >= 0)
            {
                neighbors.Add(gridManager.GetTileAtPoint(tile.Position.x + 1, tile.Position.y - 1));
            }
            // Upper Right
            if (tile.Position.y + 1 < gridManager.Height)
            {
                neighbors.Add(gridManager.GetTileAtPoint(tile.Position.x + 1, tile.Position.y + 1));
            }
        }
        // Down
        if (tile.Position.y - 1 >= 0)
        {
            neighbors.Add(gridManager.GetTileAtPoint(tile.Position.x, tile.Position.y - 1));
        }
        // Up
        if (tile.Position.y + 1 < gridManager.Height)
        {
            neighbors.Add(gridManager.GetTileAtPoint(tile.Position.x, tile.Position.y + 1));
        }

        return neighbors;
    }

    /// <summary>
    /// Pursues the line of previous tiles to create the path stack for navigation
    /// </summary>
    /// <param name="endTile">The destination tile</param>
    /// <returns>A stack of positions for the agent to travel to</returns>
    private Stack<Vector2> CalculatePath(Tile endTile)
    {
        Stack<Vector2> path = new Stack<Vector2>();
        path.Push(endTile.Position);
        Tile currentTile = endTile;
        while(currentTile.prevTile != null)
        {
            //path.Push(currentTile.prevTile.Position);
            pathList.Add(currentTile.prevTile.Position);
            currentTile = currentTile.prevTile;
        }
        //path.Pop();
        return path;
    }

    private void CleanPath()
    {
        Vector2 lastDir = Vector2.zero;
        Vector2 dir = Vector2.zero;
        lastDir = (pathList[0] - path.Peek()).normalized;

        for (int i = 0; i < pathList.Count - 1; i++)
        {
            dir = (pathList[i + 1] - pathList[i]).normalized;
            if (dir != lastDir)
            {
                path.Push(pathList[i]);
                lastDir = dir;
            }
        }
    }

    /// <summary>
    /// Calculates the distance cost between two given tiles
    /// </summary>
    /// <param name="a">First tile to compare</param>
    /// <param name="b">Second tile to compare</param>
    /// <returns>The total cost it would take for the agent to navigate between the two tiles</returns>
    protected virtual int CalculateDistanceCost(Tile a, Tile b)
    {
        if(a == null || b == null) return int.MaxValue;
        if(a.Position == null || b.Position == null) return int.MaxValue;

        int xDistance = (int)Mathf.Abs(a.Position.x - b.Position.x);
        int yDistance = (int)Mathf.Abs(a.Position.y - b.Position.y);
        int remaining = Mathf.Abs(xDistance - yDistance);
        return DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + STRAIGHT_COST * remaining;
    }

    /// <summary>
    /// Searches through the tiles to return the tile with the lowest f cost
    /// </summary>
    /// <param name="tiles">List of tiles to search through</param>
    /// <returns>The tile with the lowest F cost</returns>
    private Tile GetMinFCost(List<Tile> tiles)
    {
        Tile lowest = tiles[0];
        for(int i = 1; i < tiles.Count; i++) 
        {
            if (tiles[i].fCost < lowest.fCost) 
            { 
                lowest = tiles[i];
            }
        }
        return lowest;
    }

    private void OnDrawGizmos()
    {
        foreach(Vector2 pos in path)
        {
            if(pos != gridManager.GetTileIndex(transform.position.x, transform.position.y))
            {
                Gizmos.color = transform.GetComponent<SpriteRenderer>().color;
                Gizmos.DrawSphere(pos, 0.25f);
            }    
        }
    }
}
