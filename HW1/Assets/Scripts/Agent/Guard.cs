using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Guard : Movement
{
    public bool thiefSeen = false;
    public Vector2 thiefPos = Vector2.zero;

    private List<Vector2> routePoints = new List<Vector2>();
    private int routeIndex = -1;

    private List<Sense> senses = new List<Sense>();
    private bool scanRunning = false;


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        GameObject route = GameObject.Find("GuardRoute");
        float closestDist = float.MaxValue;
        float tempDist = 0;
        
        for(int i = 0; i < route.transform.childCount; i++)
        {
            routePoints.Add(route.transform.GetChild(i).position);
            tempDist = Vector2.Distance(transform.position, routePoints[i]);
            if (tempDist < closestDist)
            {
                routeIndex = i;
                closestDist = tempDist;
            }
        }

        senses.Add(transform.GetChild(1).gameObject.GetComponent<Sense>());
        senses.Add(transform.GetChild(2).gameObject.GetComponent<Sense>());

        seekPoint = routePoints[routeIndex];


        transform.position = new Vector3(Random.Range(1,gridManager.Width - 1), Random.Range(3,9), 0);
    }

    // Update is called once per frame
    protected override void Update()
    {
        rb.totalTorque = 0;
        foreach(Sense s in senses)
        {
            if(s.collidedThief)
            {
                thiefSeen = true;
            }
        }
        if(thiefSeen)
        {
            seekPoint = thiefPos;
        }
        else
        {
            if (Vector2.Distance(routePoints[routeIndex], transform.position) < targetRadius)
            {
                //if(!scanRunning) StartCoroutine(Scan());
                routeIndex++;
                if (routeIndex >= routePoints.Count) routeIndex = 0;
                seekPoint = routePoints[routeIndex];
            }
            seekPoint = routePoints[routeIndex];
        }
        
        
        if (lastSeekPoint != seekPoint)
        {
            Vector2 seekDir = (seekPoint - transform.position).normalized;
            Vector2 lastSeekDir = (lastSeekPoint - transform.position).normalized;
            if (Vector2.Distance(seekPoint,lastSeekPoint) > 2 || Mathf.Abs(Mathf.Atan2(lastSeekDir.y, lastSeekDir.x) - Mathf.Atan2(seekDir.y, seekDir.x)) > Mathf.PI / 4)
            {
                lastSeekPoint = seekPoint;
                AStar();
            }
        }
        SteerToTarget();
    }

    private IEnumerator Scan()
    {
        scanRunning = true;
        rb.rotation += 90;
        rb.totalTorque = 0;

        if (!thiefSeen) yield return new WaitForSeconds(2);
        else yield return null;

        rb.rotation += 180;
        rb.totalTorque = 0;

        if (!thiefSeen) yield return new WaitForSeconds(2);
        else yield return null;

        scanRunning = false;
        yield return null;
    }
}
