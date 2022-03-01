using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        Vector2[] temp = GetComponent<MeshFilter>().mesh.uv;
    }


}
