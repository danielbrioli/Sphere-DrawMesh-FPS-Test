using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attach to a game object in editor to generate a neat-o sphere procedurally, and to test the FPS change when going from individual GameObjects per triangle mesh to a Graphics.DrawMesh() implementation in real time.
/// 
/// It's worth noting that non-ambient lighting doesn't affect the Graphics.DrawMesh() implementation. I think you can hack together something with a shader to fake it, but I haven't figured out how to do that yet. Might 
/// be able to update this in the future if I stumble on a solution.
/// </summary>
public class SphereGenerator : MonoBehaviour
{

    public bool useGraphicsDrawMesh = false;

    [Range(1,5)]
    public int resolution = 2;

    public Material sphereMaterial;
    private Triangle[] sphereTriangles;
    private Mesh sphereMesh;

    private GameObject[] sphereTriangleObjects;

    private bool reactivateSphereTriangleObjects = false;
    private bool objectsAreActive = true;

    private void Start()
    {

        CreateSphere();

    }

    private void Update()
    {

        if (useGraphicsDrawMesh)
        {

            if (objectsAreActive)
            {

                DeactivateSphereObjects();

            }

            RenderMeshSphere();

            reactivateSphereTriangleObjects = true;

        }
        else if (reactivateSphereTriangleObjects)
        {

            ActivateSphereObjects();

            reactivateSphereTriangleObjects = false;

        }

    }

    /// <summary>
    /// Holds the various methods used to initially generate GameObjects, triangles, meshes, etc.
    /// </summary>
    private void CreateSphere()
    {
        
        CreateInitialTriangles();
        SubdivideTriangles();
        CreateTriangleObjects();
        CreateMeshSphere();

    }

    /// <summary>
    /// Creates the initial triangles to subdivide.
    /// </summary>
    private void CreateInitialTriangles()
    {

        sphereTriangles = new Triangle[8];

        sphereTriangles[0] = new Triangle(Vector3.up, Vector3.forward, Vector3.right);
        sphereTriangles[1] = new Triangle(Vector3.up, Vector3.right, Vector3.back);
        sphereTriangles[2] = new Triangle(Vector3.up, Vector3.back, Vector3.left);
        sphereTriangles[3] = new Triangle(Vector3.up, Vector3.left, Vector3.forward);
        sphereTriangles[4] = new Triangle(Vector3.down, Vector3.forward, Vector3.left);
        sphereTriangles[5] = new Triangle(Vector3.down, Vector3.left, Vector3.back);
        sphereTriangles[6] = new Triangle(Vector3.down, Vector3.back, Vector3.right);
        sphereTriangles[7] = new Triangle(Vector3.down, Vector3.right, Vector3.forward);

    }

    /// <summary>
    /// Creates a normalized vector halfway between two other vectors.
    /// </summary>
    /// <param name="v0">The first Vector3.</param>
    /// <param name="v1">The second Vector3.</param>
    /// <returns>A Vector3, normalized, halfway between the Vector3 parameters.</returns>
    private Vector3 GetNormalMidpoint(Vector3 v0, Vector3 v1)
    {

        return (v0 + (0.5f * (v1 - v0))).normalized;

    }

    /// <summary>
    /// Subdivides a triangle into 4 smaller and equal triangles.
    /// </summary>
    /// <param name="tri">The Triangle to split.</param>
    /// <returns>An array holding the 4 smaller triangles.</returns>
    private Triangle[] SubdivideTriangle(Triangle tri)
    {

        Triangle[] newTris = new Triangle[4];

        newTris[0] = new Triangle(tri.vectors[0],
                                  GetNormalMidpoint(tri.vectors[0], tri.vectors[1]),
                                  GetNormalMidpoint(tri.vectors[0], tri.vectors[2]));

        newTris[1] = new Triangle(tri.vectors[1], 
                                  GetNormalMidpoint(tri.vectors[1], tri.vectors[2]), 
                                  GetNormalMidpoint(tri.vectors[1], tri.vectors[0]));

        newTris[2] = new Triangle(tri.vectors[2], 
                                  GetNormalMidpoint(tri.vectors[2], tri.vectors[0]), 
                                  GetNormalMidpoint(tri.vectors[2], tri.vectors[1]));

        newTris[3] = new Triangle(GetNormalMidpoint(tri.vectors[0], tri.vectors[1]), 
                                  GetNormalMidpoint(tri.vectors[1], tri.vectors[2]), 
                                  GetNormalMidpoint(tri.vectors[2], tri.vectors[0]));

        return newTris;

    }

    /// <summary>
    /// Run subdivision across all triangles for each level of resolution.
    /// </summary>
    private void SubdivideTriangles()
    {

        for (int i = 0; i < resolution; i++)
        {

            int oldArrayLength = sphereTriangles.Length;
            int newArrayLength = oldArrayLength * 4;

            Triangle[] newSphereTris = new Triangle[newArrayLength];

            for (int j = 0; j < oldArrayLength; j++)
            {

                Triangle[] newTris = SubdivideTriangle(sphereTriangles[j]);

                newSphereTris[j * 4] = newTris[0];
                newSphereTris[(j * 4) + 1] = newTris[1];
                newSphereTris[(j * 4) + 2] = newTris[2];
                newSphereTris[(j * 4) + 3] = newTris[3];

            }

            sphereTriangles = newSphereTris;

        }

    }

    /// <summary>
    /// Create the mesh for the Graphics.DrawMesh() implementation.
    /// </summary>
    private void CreateMeshSphere()
    {

        int arrayLength = sphereTriangles.Length;
        int meshArrayLength = arrayLength * 3;

        int[] sphereTriangleIndex = new int[meshArrayLength];
        Vector3[] sphereTriangleVertices = new Vector3[meshArrayLength];

        sphereMesh = new Mesh();

        for (int i = 0; i < arrayLength; i++)
        {

            sphereTriangleVertices[i * 3] = sphereTriangles[i].vectors[0];
            sphereTriangleVertices[(i * 3) + 1] = sphereTriangles[i].vectors[1];
            sphereTriangleVertices[(i * 3) + 2] = sphereTriangles[i].vectors[2];

            sphereTriangleIndex[i * 3] = i * 3;
            sphereTriangleIndex[(i * 3) + 1] = (i * 3) + 1;
            sphereTriangleIndex[(i * 3) + 2] = (i * 3) + 2;

        }

        sphereMesh.vertices = sphereTriangleVertices;
        sphereMesh.triangles = sphereTriangleIndex;

    }

    /// <summary>
    /// Render the sphere with the Graphics.DrawMesh() implementation.
    /// </summary>
    private void RenderMeshSphere()
    {

        Graphics.DrawMesh(sphereMesh, Vector3.zero, Quaternion.identity, sphereMaterial, 0);

    }

    /// <summary>
    /// Creates a GameObject from a triangle.
    /// </summary>
    /// <param name="tri">The triangle to create the GameObject from.</param>
    /// <returns>Returns a GameObject representing a triangle.</returns>
    private GameObject CreateTriangleObject(Triangle tri)
    {

        GameObject triObject = new GameObject();
        triObject.transform.parent = this.transform;
        triObject.name = "TriangleObject";

        MeshFilter mf = triObject.AddComponent<MeshFilter>();
        mf.mesh.vertices = tri.vectors;
        mf.mesh.triangles = tri.tris();

        MeshRenderer mr = triObject.AddComponent<MeshRenderer>();
        mr.sharedMaterial = sphereMaterial;

        mf.mesh.RecalculateNormals();
        mf.mesh.UploadMeshData(false);

        return triObject;

    }

    /// <summary>
    /// Create all GameObjects to be used in generating the sphere.
    /// </summary>
    private void CreateTriangleObjects()
    {

        int arrayLength = sphereTriangles.Length;

        sphereTriangleObjects = new GameObject[arrayLength];

        for (int i = 0; i < arrayLength; i++)
        {

            sphereTriangleObjects[i] = CreateTriangleObject(sphereTriangles[i]);

        }

        StaticBatchingUtility.Combine(sphereTriangleObjects, this.gameObject); // not working... does the parent object need a mesh/mf/mr?

    }

    /// <summary>
    /// Activates all triangle GameObjects.
    /// </summary>
    private void ActivateSphereObjects()
    {

        objectsAreActive = true;

        int arrayLength = sphereTriangleObjects.Length;

        for (int i = 0; i < arrayLength; i++)
        {

            sphereTriangleObjects[i].SetActive(true);

        }

    }

    /// <summary>
    /// Deactivates all triangle GameObjects.
    /// </summary>
    private void DeactivateSphereObjects()
    {

        objectsAreActive = false;

        int arrayLength = sphereTriangleObjects.Length;

        for (int i = 0; i < arrayLength; i++)
        {

            sphereTriangleObjects[i].SetActive(false);

        }

    }

}
