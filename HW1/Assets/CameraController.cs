using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float xSpeed = 4f;
    [SerializeField] private float ySpeed = 3f;
    [SerializeField] private float zoomMin = 1f;
    [SerializeField] private float zoomMax = 10f;
    [Range(1f, 10f)] public float zoom = 3f;


    private float zoomIncrement = 1;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            transform.position = new Vector3(transform.position.x - xSpeed * Time.deltaTime, transform.position.y, transform.position.z);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position = new Vector3(transform.position.x + xSpeed * Time.deltaTime, transform.position.y, transform.position.z);
        }


        if (Input.GetKey(KeyCode.W))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + ySpeed * Time.deltaTime, transform.position.z);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - ySpeed * Time.deltaTime, transform.position.z);
        }

        if (Mathf.Abs(Input.mouseScrollDelta.y) > 0)
        {
            zoom -= Input.mouseScrollDelta.y * zoomIncrement;
            zoom = Mathf.Clamp(zoom, zoomMin, zoomMax);

            Camera.main.orthographicSize = zoom;
        }
    }
}
