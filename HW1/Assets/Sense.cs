using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sense : MonoBehaviour
{
    public bool collidedThief = false;
    private GameObject thief;

    private void Start()
    {
        thief = GameObject.Find("Thief");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Thief")
        {
            //EnemySensed(collision);
            RaycastHit2D results = Physics2D.Raycast(transform.parent.position, (collision.transform.position - transform.parent.position).normalized, 10f);
            //Debug.DrawLine(transform.position, results.transform.position, Color.red, 2f);
            //Debug.Log($"Pos:{transform.position}, Collider: {collision.transform.position}");

            if (results.collider != null && results.collider.gameObject.name == "Thief")
            {
                Debug.DrawLine(transform.parent.position, collision.transform.position, Color.red, 2f);
                EnemySensed(collision);
            }
            else
            {
                thief = collision.gameObject;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.name == "Thief")
        {
            EnemyGone();
        }
    }

    private void EnemyGone()
    {
        if(thief != null)
        {
            collidedThief = false;
            thief.GetComponent<Thief>().sensed = false;
            Debug.DrawLine(transform.parent.position, thief.transform.position, Color.blue, 2f);
            thief = null;
            transform.parent.GetComponent<Guard>().thiefSeen = false;
        }
    }

    private void EnemySensed(Collider2D collision)
    {
        EnemySensed(collision.gameObject);
    }

    private void EnemySensed(GameObject obj)
    {
        thief = obj;
        collidedThief = true;
        transform.parent.GetComponent<Guard>().thiefPos = thief.transform.position;
        transform.parent.GetComponent<Guard>().thiefSeen = true;
        thief.GetComponent<Thief>().sensed = true;
    }

    private void Update()
    {
        if (thief != null)
        {
            RaycastHit2D results;
            results = Physics2D.Raycast(transform.position, (thief.transform.position - transform.position).normalized, 10f);
            //Debug.DrawLine(transform.position, results.point, Color.red, 2f);
            //DebugDisplay.Instance.PlaceDot("Raycast", results.point);
            if (results.collider != null && results.collider.gameObject.name == "Thief")
            {
                //Debug.Log($"Pos:{transform.position}, Collider: {player.transform.position}");

                //Debug.DrawLine(transform.position, (thief.transform.position - transform.position).normalized, Color.red, 2f);
                EnemySensed(thief);
            }
            else
            {
                EnemyGone();
            }
        }
    }
}
