using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

// Basic Infrastructure from https://www.youtube.com/watch?v=kkAjpQAM-jE&list=TLPQMTExMDIwMjPvz9ayyim_cw&index=2
public class GridManager : MonoBehaviour
{
    #region Variables
    public static GridManager Instance;

    private int width = 15;
    private int height = 30;

    [SerializeField] GameObject tilePrefab;

    private Dictionary<Vector2, Tile> tiles;

    [SerializeField] private List<Movement> agents = new List<Movement>();
    #endregion

    #region Properties
    public int Width { get { return width; } }
    public int Height { get { return height; } }
    #endregion

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Use a dictionary for ease of access
        tiles = new Dictionary<Vector2, Tile>();
        // Generate the grid of tiles
        GenerateGrid();
        // Get a list of all of the agents in the scene
        // Only for the intent to refresh their A* when enviornment changes
        //List<GameObject> temp = GameObject.FindGameObjectsWithTag("Player").ToList();
        //temp.AddRange(GameObject.FindGameObjectsWithTag("Agent"));
        //foreach(GameObject obj in temp)
        //{
        //    agents.Add(obj.GetComponent<Movement>());
        //}
    }

    private void Update()
    {
        //if(Input.GetMouseButtonDown(1))
        //{
        //    Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //    Tile t = GetTileAtPoint(mousePos.x, mousePos.y);
        //    if (t != null)
        //    {
        //        t.ToggleWalkable();
        //        foreach(Movement m in agents)
        //        {
        //            // Curious if adding some vulnerability for agents would be worth to call this less
        //            m.AStar();
        //        }    
        //    }
        //}
    }

    void GenerateGrid() 
    {
        // Allocate to memory once to save performance
        GameObject spawnedTile;
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                spawnedTile = Instantiate(tilePrefab, new Vector3(x, y, 0), Quaternion.identity, transform);
                spawnedTile.name = $"Tile ({x},{y})";

                // Tell the user the coordinates when hovering over
                // Currently just used for the sake of debugging
                spawnedTile.transform.GetChild(0).GetChild(0).GetComponent<TextMeshPro>().text = $"({x},{y})";

                // Utilize offset colors for ease of visibility
                bool offsetTile = (x+y) % 2 == 0;
                Tile t = spawnedTile.GetComponent<Tile>();

                if (x == 0 || y == 0 || x == width - 1 || y == height - 1)
                {
                    t.ChangeColor(t.impassableColor);
                    t.Walkable = false;
                    t.gameObject.GetComponent<BoxCollider2D>().enabled = true;
                }

                else if(y == 24 && x < 10)
                {
                    t.ChangeColor(t.impassableColor);
                    t.Walkable = false;
                    t.gameObject.GetComponent<BoxCollider2D>().enabled = true;
                }

                else t.ChangeColor(offsetTile ? t.offsetColor : t.normalColor);

                // Add the tile to the dictionary
                tiles.Add(new Vector2(x, y), t);
            }
        }
        GameObject propParent = GameObject.Find("Props");
        int childCount = propParent.transform.childCount;
        Tile temp;
        for(int i = 0; i < childCount; i++)
        {
            tiles.TryGetValue(propParent.transform.GetChild(i).position, out temp);
            temp.ChangeColor(temp.impassableColor);
            temp.Walkable = false;
            temp.gameObject.GetComponent<BoxCollider2D>().enabled = true;
        }


        // If the camera needs to align with the grid, do so
        Camera.main.transform.position = new Vector3((float)width / 2.0f - 0.5f, (float)height / 2.0f - 0.5f, -10);
    }

    #region Helpers
    /// <summary>
    /// Checks the manager dictionary for a given tile
    /// </summary>
    /// <param name="x">X position of the desired tile</param>
    /// <param name="y">Y position of the desired tile</param>
    /// <returns>The tile contained at the desired position, null if none</returns>
    public Tile GetTileAtPoint(float x, float y)
    {
        Vector2 roundedCoord = new Vector2(Mathf.Round(x), Mathf.Round(y));
        try
        {
            if (tiles.TryGetValue(roundedCoord, out Tile t))
            {
                return t;
            }
            return null;
        }
        catch { return null; }
        
    }

    /// <summary>
    /// Checks the manager dictionary for a tile at a given index
    /// </summary>
    /// <param name="x">X position of the desired tile</param>
    /// <param name="y">y position of the desired tile</param>
    /// <returns>The Vector2 position of a tile if there is one, Vector2.negativeInfinity otherwise</returns>
    public Vector2 GetTileIndex(float x, float y)
    {
        Vector2 roundedCoord = new Vector2(Mathf.Round(x), Mathf.Round(y));
        try
        {
            if (tiles.TryGetValue(roundedCoord, out Tile t))
            {
                return t.Position;
            }
            return Vector2.negativeInfinity;
        }
        catch { return Vector2.negativeInfinity; }
    }

    /// <summary>
    /// Checks the manager dictionary for a tile at a given index
    /// </summary>
    /// <param name="pos">The Vector2 position of the desired tile</param>
    /// <returns>The Vector2 position of a tile if there is one, Vector2.negativeInfinity otherwise</returns>
    public Vector2 GetTileIndex(Vector2 pos)
    {
        Vector2 roundedCoord = new Vector2(Mathf.Round(pos.x), Mathf.Round(pos.y));
        try
        {
            if (tiles.TryGetValue(roundedCoord, out Tile t))
            {
                return t.Position;
            }
            return Vector2.negativeInfinity;
        }
        catch { return Vector2.negativeInfinity; }
    }
    #endregion
}
