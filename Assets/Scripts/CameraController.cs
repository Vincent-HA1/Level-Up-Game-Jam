using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera Camera; 
    // Start is called before the first frame update
    void Start()
    {
        Camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        Camera.transform.position = new Vector3(transform.position.x, transform.position.y, Camera.transform.position.z);
    }
}
