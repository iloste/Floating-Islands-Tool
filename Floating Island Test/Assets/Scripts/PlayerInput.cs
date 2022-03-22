using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] bool marchingCubes;

    private Vector3 worldPosRaw;
    private Vector3Int worldPosRefined;


    private void Update()
    {
        //GetInputWaveCollapse();
        GetInputMarchingSquares();
    }

    private void GetInputMarchingSquares()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            GetWorldPositions();
            if (!marchingCubes)
            {
                MarchingCubes.instance.FillVertex(new Vector3Int(worldPosRefined.x, worldPosRefined.z, worldPosRefined.y + 1));
                //MarchingCubesTest.instance.FillVertex(new Vector3Int(worldPosRefined.x, worldPosRefined.z, worldPosRefined.y + 2));
            }
            else
            {
                MarchingSquaresTest.instance.FillVertex(new Vector2Int(worldPosRefined.x, worldPosRefined.z));
            }
        }
        else if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            GetWorldPositions();
            if (!marchingCubes)
            {
                MarchingCubes.instance.ClearVertex(new Vector3Int(worldPosRefined.x, worldPosRefined.z, worldPosRefined.y + 1));
            }
        }
    }

    private void GetInputWaveCollapse()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            GetWorldPositions();
            WaveCollapse2D.instance.AddPiece(new Vector2Int(worldPosRefined.x, worldPosRefined.z));
        }
        else if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            GetWorldPositions();
            WaveCollapse2D.instance.RemovePiece(new Vector2Int(worldPosRefined.x, worldPosRefined.z));
        }
    }


    private void GetWorldPositions()
    {
        worldPosRaw = new Vector3();
        Plane plane = new Plane(Vector3.up, 0);
        float distance;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity, LayerMask.GetMask("Default")))
        {
            Debug.Log("Hit: " + hit.transform.name);
            worldPosRaw = hit.point;
        }
        else
        {
            if (plane.Raycast(ray, out distance))
            {
                worldPosRaw = ray.GetPoint(distance);
            }
        }

      

        worldPosRefined = new Vector3Int((int)worldPosRaw.x, 0, (int)worldPosRaw.z);

        float x = (int)worldPosRaw.x;
        float y = (int)worldPosRaw.y;
        float z = (int)worldPosRaw.z;

        if (worldPosRaw.x - x > 0.5)
        {
            x += 0.5f;
        }
        if (worldPosRaw.x - y > 0.5)
        {
            y += 0.5f;
        }
        if (worldPosRaw.x - z > 0.5)
        {
            z += 0.5f;
        }
    }
}
