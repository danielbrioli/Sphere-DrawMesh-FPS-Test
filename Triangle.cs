using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple struct that represents a Triangle via 3 Vector3s.
/// </summary>
public struct Triangle
{

    public Vector3[] vectors;

    /// <summary>
    /// Make a Triangle from a Vector3 array.
    /// </summary>
    /// <param name="vectors"></param>
    public Triangle(Vector3[] vectors)
    {

        this.vectors = vectors;

    }

    /// <summary>
    /// Make a Triangle from 3 Vector3s.
    /// </summary>
    /// <param name="v0">Vector3 #1.</param>
    /// <param name="v1">Vector3 #2.</param>
    /// <param name="v2">Vector3 #3.</param>
    public Triangle(Vector3 v0, Vector3 v1, Vector3 v2)
    {

        this.vectors = new Vector3[3];

        this.vectors[0] = v0;
        this.vectors[1] = v1;
        this.vectors[2] = v2;

    }

    /// <summary>
    /// Method that returns this triangle's uhh... triangle array. (Mumbles about Unity's bad naming convention getting in the way of his own bad naming convention.)
    /// </summary>
    /// <returns></returns>
    public int[] tris()
    {

        return new int[3] { 0, 1, 2 };

    }

}
