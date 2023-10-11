using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMove : Movement
{
    // Basic functionality to add variety to scene
    private float idleDuration = 2f;
    private float idleTimer;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 tilePoint = GridManager.Instance.GetTileIndex(mousePos.x, mousePos.y);

            seekPoint = tilePoint != Vector3.negativeInfinity ? tilePoint : seekPoint;
        }
        base.Update();
        if (!move)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer >= idleDuration)
            {
                seekPoint = new Vector2(Random.Range(0, 12), Random.Range(0, 11));
                idleTimer = 0f;
            }

        }
        else
        {
            idleTimer = 0f;
        }
    }
}
