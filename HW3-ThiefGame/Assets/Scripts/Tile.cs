using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Color normalColor = Color.green;
    public Color offsetColor = Color.cyan;
    public Color impassableColor = Color.black;

    // Pathfinding logic
    [SerializeField] private Vector2 position;
    private bool walkable = true;

    public int gCost;
    public int hCost;
    public int fCost;
    public Tile prevTile;

    public Vector2 Position { get { return position; } }
    public bool Walkable { get { return walkable; } set { walkable = value; } }

    // Copy constructor, for the purpose of pathfinding
    // Definitely not the optimal solution but I think I dug myself into this
    //public Tile(Tile tile)
    //{
    //    this.position = tile.Position;
    //    this.walkable = tile.Walkable;
    //}

    // Start is called before the first frame update
    void Start()
    {
        // If there isnt a color value assigned, add one based off of the SpriteRenderer
        if(normalColor == null)
        {
            normalColor = GetComponent<SpriteRenderer>().color;
        }
        // Set the position value only if it needs to be changed
        position = transform.position;
        walkable = true;
    }

    public void ChangeColor(Color color)
    {
        this.GetComponent<SpriteRenderer>().color = color;
    }

    private void OnMouseEnter()
    {
        transform.GetChild(0).gameObject.SetActive(true);
    }
    private void OnMouseExit()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

    public void ToggleWalkable()
    {
        Debug.Log("test");
        walkable = !walkable;
        transform.GetChild(1).gameObject.SetActive(!walkable);
    }
}
