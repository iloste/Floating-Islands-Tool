using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DisplaySocketsInScene : MonoBehaviour
{
    [SerializeField] MCTile tile;

    bool displaySockets;

    private void OnDrawGizmos()
    {
        if (Selection.count == 1)
        {
            if (Selection.activeGameObject == this.gameObject)
            {
                displaySockets = true;
            }
            else
            {
                displaySockets = false;
            }
        }
        else if (Selection.count > 1)
        {
            for (int i = 0; i < Selection.gameObjects.Length; i++)
            {
                if (Selection.gameObjects[i] == this.gameObject)
                {
                    displaySockets = true;
                    break;
                }
            }
        }
        else
        {
            displaySockets = false;
        }


        if (displaySockets)
        {
            if (tile != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(transform.position, 0.1f);
                // Gizmos.DrawLine(transform.position, transform.position + transform.forward);
                DisplaySockets();
            }
        }
    }


    private void DisplaySockets()
    {
        Vector3 direction = Vector3.zero;

        if (tile.sockets[0].number >= 0)
        {
            direction = transform.position + transform.forward / 3;
            DrawConnection(direction, tile.sockets[0]);
            // DrawDirection(transform.position, direction * .75f, Color.red);
        }
        if (tile.sockets[1].number >= 0)
        {
            direction = transform.position + transform.right / 3;
            DrawConnection(direction, tile.sockets[1]);
            //  DrawDirection(transform.position, direction * .75f, Color.blue);
        }
        if (tile.sockets[2].number >= 0)
        {
            direction = transform.position - transform.forward / 3;
            DrawConnection(direction, tile.sockets[2]);
            // DrawDirection(transform.position, direction * .75f, Color.blue);
        }
        if (tile.sockets[3].number >= 0)
        {
            direction = transform.position - transform.right / 3;
            DrawConnection(direction, tile.sockets[3]);
            // DrawDirection(transform.position, direction * .75f, Color.blue);
        }
        if (tile.sockets[4].number >= 0)
        {
            direction = transform.position + transform.up / 3;
            DrawConnection(direction, tile.sockets[4]);
            // DrawDirection(transform.position, direction * .75f, Color.blue);
        }
        if (tile.sockets[5].number >= 0)
        {
            direction = transform.position - transform.up / 3;
            DrawConnection(direction, tile.sockets[5]);
            // DrawDirection(transform.position, direction * .75f, Color.blue);
        }
    }

    private void DrawConnection(Vector3 position, Connection connection)
    {
        Gizmos.color = Color.white;
        Handles.Label(position, connection.number + " " + connection.spConnection.ToString());
    }

    private void DrawDirection(Vector3 start, Vector3 end, Color color)
    {
        Handles.DrawBezier(start, end, start, end, color, null, 3);
    }
}
