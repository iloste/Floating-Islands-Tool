using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class VertexList : ScriptableObject
{
    public Vector3[] vertexPositions;
    public Vector3Int[] triplets;
    public Vector2[] uvs;
    Dictionary<Vector3, int> vertexCount;




    public int AddVertex(Vector3 newPos)
    {
        for (int i = 0; i < vertexPositions.Length; i++)
        {
            if (vertexPositions[i] == newPos)
            {
                vertexCount[newPos]++;
                return i;
            }
        }

        Vector3[] temp = vertexPositions;
        vertexPositions = new Vector3[vertexPositions.Length + 1];

        for (int i = 0; i < temp.Length; i++)
        {
            vertexPositions[i] = temp[i];
        }

        vertexPositions[vertexPositions.Length - 1] = newPos;
        vertexCount.Add(newPos, 1);

        return vertexPositions.Length - 1;
    }


    public void RemoveVertex(Vector3 position)
    {
        if (vertexCount[position] > 1)
        {
            vertexCount[position]--;
            return;
        }

        Vector3[] temp = new Vector3[vertexPositions.Length - 1];
        int extra = 0;

        for (int i = 0; i < vertexPositions.Length; i++)
        {
            if (vertexPositions[i] != position)
            {
                if (i == vertexPositions.Length - 1 && extra == 0)
                {
                    return;
                }

                temp[i - extra] = vertexPositions[i];
            }
            else
            {
                extra = 1;
            }
        }

        vertexPositions = temp;
        vertexCount.Remove(position);
    }



    public void AddTriplet(Vector3Int newPos)
    {
        Vector3Int[] temp = triplets;
        triplets = new Vector3Int[triplets.Length + 1];

        for (int i = 0; i < temp.Length; i++)
        {
            triplets[i] = temp[i];
        }

        triplets[triplets.Length - 1] = newPos;
    }


    public void RemoveTriplet(Vector3Int triplet)
    {
        Vector3Int[] temp = new Vector3Int[triplets.Length - 1];
        int extra = 0;

        for (int i = 0; i < triplets.Length; i++)
        {
            if (triplets[i] != triplet)
            {
                if (i == triplets.Length - 1 && extra == 0)
                {
                    return;
                }

                temp[i - extra] = triplets[i];
            }
            else
            {
                extra = 1;
            }
        }

        triplets = temp;
        RemoveVertex(vertexPositions[triplet.x]);
        RemoveVertex(vertexPositions[triplet.y]);
        RemoveVertex(vertexPositions[triplet.z]);
    }



    public void Initialise()
    {
        triplets = new Vector3Int[0];
        vertexPositions = new Vector3[0];
        vertexCount = new Dictionary<Vector3, int>();
    }

}
