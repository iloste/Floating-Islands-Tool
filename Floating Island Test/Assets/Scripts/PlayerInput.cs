using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] bool marchingCubes;

    [SerializeField] GameObject target;
    private Vector3 worldPosRaw;
    private Vector3Int worldPosRefined;


    private void Update()
    {
        //GetInputWaveCollapse();
        GetWorldPositions();
        GetInputMarchingSquares();
        target.transform.position = worldPosRefined - new Vector3(0.5f, 0, 0.5f);
    }

    private void GetInputMarchingSquares()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            // GetWorldPositions();
            if (marchingCubes)
            {
                MCWFC.instance.FillVertex(new Vector3Int(worldPosRefined.x, worldPosRefined.y, worldPosRefined.z));
               // MarchingCubes.instance.FillVertex(new Vector3Int(worldPosRefined.x, worldPosRefined.y, worldPosRefined.z));
                //MarchingCubes.instance.FillVertex(new Vector3Int(worldPosRefined.x, worldPosRefined.y + 1, worldPosRefined.z));
                //MarchingCubesTest.instance.FillVertex(new Vector3Int(worldPosRefined.x, worldPosRefined.z, worldPosRefined.y + 2));
            }
            else
            {
                MarchingSquaresTest.instance.FillVertex(new Vector2Int(worldPosRefined.x, worldPosRefined.z));
            }
        }
        else if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            // GetWorldPositions();

            if (marchingCubes)
            {
                Transform tileTransform = TileInUse();
                if (tileTransform != null)
                {
                    Vector3Int position = new Vector3Int(Mathf.RoundToInt(tileTransform.position.x), Mathf.RoundToInt(tileTransform.position.y), Mathf.RoundToInt(tileTransform.position.z));
                    MCWFC.instance.ClearVertex(position);
                    //MarchingCubes.instance.ClearVertex(position);
                }
                else
                {
                    MCWFC.instance.ClearVertex(new Vector3Int(worldPosRefined.x, worldPosRefined.y, worldPosRefined.z));
                }
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
           
            WaveCollapse2D.instance.RemovePiece(new Vector2Int(worldPosRefined.x, worldPosRefined.z));

        }
    }

    private Transform TileInUse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity, LayerMask.GetMask("Tile")))
        {
            return hit.transform;
        }
        else
        {
            return null;
        }
    }


    private void GetWorldPositions()
    {
        worldPosRaw = new Vector3();
        // -1 so the plane starts at y == 1 (positive 1 would make y == -1)
        Plane plane = new Plane(Vector3.up, -1);
        
        float distance;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity, LayerMask.GetMask("Tile")))
        {
            worldPosRaw = hit.transform.position;
            worldPosRaw += hit.normal / 2;
           // worldPosRaw += Vector3.up/3;
            //worldPosRaw.y = Mathf.RoundToInt(worldPosRaw.y);
           // Debug.Log("Hit: " + hit.transform.name + "| Position: " + worldPosRaw);
        }
        else
        {
            if (plane.Raycast(ray, out distance))
            {
                worldPosRaw = ray.GetPoint(distance);
               
                //Debug.Log("plane position: " + worldPosRaw);
            }
        }



        worldPosRefined = new Vector3Int(Mathf.RoundToInt(worldPosRaw.x), Mathf.RoundToInt(worldPosRaw.y), Mathf.RoundToInt(worldPosRaw.z));
        //Debug.Log("Pos raw: " + worldPosRaw);
        //Debug.Log("Pos refined: " + worldPosRefined);

    }
}
