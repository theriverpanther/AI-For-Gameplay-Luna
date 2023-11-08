using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : Movement
{
    [SerializeField]
    private GameObject target;
    // Start is called before the first frame update
    protected override void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player");
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        Vector2 playerGridPos = gridManager.GetTileIndex(target.transform.position);
        if(!seekPoint.Equals(playerGridPos))
        {
            seekPoint = playerGridPos;
        }
        base.Update();
    }
}
