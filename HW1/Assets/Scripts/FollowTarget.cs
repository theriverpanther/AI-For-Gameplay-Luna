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
        seekPoint = target.transform.position;
        base.Update();
    }
}
