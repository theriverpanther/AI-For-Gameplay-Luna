using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thief : Movement
{
    private bool isFollowed = false;

    public float newPointTime = 4f;
    public float timer = 4f;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();        
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (timer >= newPointTime)
        {
            seekPoint = new Vector3(Random.Range(1, gridManager.Width - 1), Random.Range(1, gridManager.Height - 1));
            timer = 0;
        }
        //else timer += Time.deltaTime;

        base.Update();
        SteerToTarget();
    }
}
