using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _rotate_over_time : MonoBehaviour
{
    public float f_rotation_speed = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * (f_rotation_speed * Time.deltaTime));
    }
}
