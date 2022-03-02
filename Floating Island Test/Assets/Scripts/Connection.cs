using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Connection
{

    public int number;
    public Tile.SpecialConnection spConnection;


    /// <summary>
    /// Returns true if the given connection can "fit into" this connection
    /// </summary>
    public bool FitsInto(Connection connection)
    {
        if (connection.number == this.number)
        {
            if (connection.spConnection == Tile.SpecialConnection.Symmetrical && this.spConnection == Tile.SpecialConnection.Symmetrical)
            {
                return true;
            }
            else if (connection.spConnection == Tile.SpecialConnection.Flipped && this.spConnection == Tile.SpecialConnection.None ||
                connection.spConnection == Tile.SpecialConnection.None && this.spConnection == Tile.SpecialConnection.Flipped)
            {
                return true;
            }
        }

        return false;
    }


    // To do: Make sure you understand what is happening here.
    #region Operators
    public static bool operator ==(Connection a, Connection b)
    {
        if (a.number == b.number && a.spConnection == b.spConnection)
        {
            return true;
        }

        return false;
    }


    public static bool operator != (Connection a, Connection b)
    {
        if (a.number != b.number || a.spConnection != b.spConnection)
        {
            return true;
        }

        return false;
    }

 


    public override bool Equals(object obj) => this.Equals((Connection)obj);

    public bool Equals(Connection p)
    {
        if (p == null)
        {
            return false;
        }

        // Optimization for a common success case.
        if (Object.ReferenceEquals(this, p))
        {
            return true;
        }

        // If run-time types are not exactly the same, return false.
        if (this.GetType() != p.GetType())
        {
            return false;
        }

        // Return true if the fields match.
        // Note that the base class is not invoked because it is
        // System.Object, which defines Equals as reference equality.
        return (number == p.number) && (spConnection == p.spConnection);
    }

    public override int GetHashCode() => (number, spConnection).GetHashCode();

    #endregion

}
