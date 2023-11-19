using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinner : MonoBehaviour
{
    // Start is called before the first frame update
    public float x , y = 1, z;

    public void setRotation(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(x, y, z);
    }
}
