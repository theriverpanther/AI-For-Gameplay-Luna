using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.SceneManagement;

public class Thief : Movement
{
    public bool sensed = false;

    public bool hasTarget = false;

    public float newPointTime = 4f;
    public float timer = 4f;

    public int fearVal = 1;

    private GameObject guard;
    private Vector2 guardPos;
    private Vector2 lastGuardPos;

    private Vector2 targetPos;
    private Vector2 exitPoint;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        targetPos = GameObject.Find("Target").transform.position;
        seekPoint = targetPos;
        exitPoint = GameObject.Find("EscapePoint").transform.position;
        guard = GameObject.Find("Guard");
        //guardPos = gridManager.GetTileAtPoint(guard.transform.position.x, guard.transform.position.y).Position;
        //lastGuardPos = guardPos;
    }

    // Update is called once per frame
    protected override void Update()
    {
        guardPos = gridManager.GetTileAtPoint(guard.transform.position.x, guard.transform.position.y).Position;

        if (Vector2.Distance(guardPos, transform.position) <= 0.125) Lose();
        if (Vector2.Distance(targetPos, transform.position) <= targetRadius) Escape();
        if (hasTarget && Vector2.Distance(exitPoint, transform.position) <= targetRadius) Win();


        if (sensed)
        {
            bool negative = Random.Range(0, 1) == 0;
            Vector2 dirToGuard = (guardPos - (Vector2)transform.position).normalized;
            Vector2 newDir = new Vector2(dirToGuard.y, dirToGuard.x * (negative ? -1 : 1));

            RaycastHit2D ray = Physics2D.Raycast(transform.position, newDir);

            if (Vector2.Distance(ray.collider.transform.position, transform.position) > 4)
                seekPoint = ray.collider.transform.position;

            else seekPoint = GameObject.Find("EscapePoint").transform.position;
        }
        else
        {
            if (!hasTarget) seekPoint = targetPos;
            else seekPoint = exitPoint;
        }
        
        if (lastSeekPoint != seekPoint)
        {
            if (Vector2.Distance(guardPos, lastGuardPos) > 3 || Vector2.Distance(lastSeekPoint, seekPoint) > 2) AStar();
            lastSeekPoint = seekPoint;
        }
        SteerToTarget();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Guard")
        {
            Lose();
        }
        else if(collision.gameObject.name == "Target")
        {
            Escape();
        }
    }
    private void Win()
    {
        GameObject.Find("Canvas").SetActive(true);
    }

    private void Lose()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void Escape()
    {
        seekPoint = exitPoint;
        hasTarget = true;
    }

    protected override int CalculateDistanceCost(Tile a, Tile b)
    {
        if (a == null || b == null) return int.MaxValue;
        if (a.Position == null || b.Position == null) return int.MaxValue;

        int xDistance = (int)Mathf.Abs(a.Position.x - b.Position.x);
        int yDistance = (int)Mathf.Abs(a.Position.y - b.Position.y);
        int remaining = Mathf.Abs(xDistance - yDistance);
        return DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + STRAIGHT_COST * remaining + fearVal * Mathf.RoundToInt(Vector2.Distance(transform.position, guardPos));
    }
}
