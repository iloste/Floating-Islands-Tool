using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class Tile : ScriptableObject
{
    public enum SpecialConnection
    {
        None,
        Flipped,
        Symmetrical,
    }
    

    public Connection[] sockets;
    public int rotationIndex;
    public GameObject prefab;

    [HideInInspector] public List<List<Tile>> possibleConnectors;
    [HideInInspector] public List<List<Tile>> immpossibleConnectors;



    public Tile()
    {
        sockets = new Connection[4];
        rotationIndex = 0;
        possibleConnectors = new List<List<Tile>>();

        for (int i = 0; i < 4; i++)
        {
            possibleConnectors.Add(new List<Tile>()); 
        }
    }

}
