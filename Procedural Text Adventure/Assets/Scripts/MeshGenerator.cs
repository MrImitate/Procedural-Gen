﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshGenerator : TextureGenerator
{
    public float treeChance = 0.1f;
    public float houseChance = 0.4f;
    public float boatChance = 0.01f;

    float meshWidth, meshHeight;
    //List<Vector3> vertexList;
    Vector3[] vertices;
    List<GameObject> instantiatedObjects;

    public override void VisualizeGrid()
    {
        base.VisualizeGrid();

        // Destroy previously instantiated objects
        if (instantiatedObjects != null)
        {
            foreach (GameObject obj in instantiatedObjects)
            {
                Destroy(obj);
            }
        }
        instantiatedObjects = new List<GameObject>();

        // Make 3d objects and fill vertexlist
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Tile tile = grid[x, z];

                //float yValue = tile.y < possibleYValues[2] ? possibleYValues[2] * noiseIntensity : tile.y * noiseIntensity;
                //vertexList.Add(new Vector3(x * (meshWidth / width), yValue, z * (meshHeight / height)));

                float scale = meshWidth / width; 
                //Vector3 pos = vertexList[vertexList.Count - 1] + transform.position;
                Vector3 pos = vertices[x * width + z] + transform.position;

                // Determine object by tiletype and its corresponding chance
                if (tile.type == Tile.TileType.Grass && Random.value < treeChance)
                {
                    GameObject newTree = Instantiate(Resources.Load<GameObject>("Prefabs/TreePrefab"), pos, Quaternion.identity) as GameObject;
                    newTree.transform.parent = transform;
                    newTree.transform.localScale *= scale;
                    instantiatedObjects.Add(newTree);
                }
                else if (tile.type == Tile.TileType.Village && Random.value < houseChance)
                {
                    GameObject newHouse = Instantiate(Resources.Load<GameObject>("Prefabs/HousePrefab"), pos, Quaternion.identity) as GameObject;
                    newHouse.transform.parent = transform;
                    newHouse.transform.localScale += Vector3.up * (.2f * Random.value);
                    newHouse.transform.localScale *= scale;
                    instantiatedObjects.Add(newHouse);
                }
                else if (tile.type == Tile.TileType.VillageBorder)
                {
                    GameObject newBorder = Instantiate(Resources.Load<GameObject>("Prefabs/BorderPrefab"), pos, Quaternion.identity) as GameObject;
                    newBorder.transform.parent = transform;
                    newBorder.transform.localScale *= scale;
                    instantiatedObjects.Add(newBorder);
                }
                else if (tile.type == Tile.TileType.Water && Random.value < boatChance)
                {
                    GameObject newBoat = Instantiate(Resources.Load<GameObject>("Prefabs/BoatPrefab"), pos, Quaternion.identity) as GameObject;
                    newBoat.transform.parent = transform;
                    newBoat.transform.localScale *= scale;
                    instantiatedObjects.Add(newBoat);
                }
                else if (tile.type == Tile.TileType.Road)
                {
                    GameObject newBoat = Instantiate(Resources.Load<GameObject>("Prefabs/RoadPrefab"), pos, Quaternion.identity) as GameObject;
                    newBoat.transform.parent = transform;
                    newBoat.transform.localScale *= scale;
                    instantiatedObjects.Add(newBoat);
                }
            }
        }
    }

    public void CreateMesh(float _meshWidth, float _meshHeight)
    {
        meshWidth = _meshWidth;
        meshHeight = _meshHeight;

        vertices = new Vector3[width * height];
        
        //vertexList = new List<Vector3>();
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Tile tile = grid[x, z];

                float yValue = tile.y < possibleYValues[2] ? possibleYValues[2] * noiseIntensity : tile.y * noiseIntensity;
                Vector3 vertex = new Vector3(x * (meshWidth / width), yValue, z * (meshHeight / height));
                //vertexList.Add(vertex);
                vertices[x * width + z] = vertex;
            }
        }

        //  Vector3[] vertices = vertexList.ToArray();

        JitterVertices(ref vertices);

        int[] triangles = FindTriangles(vertices.Length, vertices);

        Vector2[] uvs = new Vector2[vertices.Length];
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x / meshWidth, vertices[i].z / meshHeight);
        }

        Mesh newMesh = new Mesh();
        newMesh.vertices = vertices;
        newMesh.triangles = triangles;
        newMesh.uv = uvs;
        newMesh.RecalculateNormals();
        newMesh.RecalculateBounds();

        GetComponent<MeshFilter>().mesh = newMesh;
        //GetComponent<MeshRenderer>().material.mainTexture = gridTexture;
    }

    int[] FindTriangles(int vertices, Vector3[] v)
    {
        int[] triangles = new int[(width - 1) * (height - 1) * 2 * 3];

        int index = 0;
        for (int i = 0; i < vertices - height; i++)
        {

            // Triangle 1
            if (i % height < height - 1)
            {
                triangles[index++] = i;
                triangles[index++] = i + 1;
                triangles[index++] = i + height;
            }

            // Triangle 2
            if (i % height > 0)
            {
                triangles[index++] = i;
                triangles[index++] = i + height;
                triangles[index++] = i + height - 1;
            }
        }

        return triangles;
    }

    void JitterVertices(ref Vector3[] vertices)
    {
        float w = (meshWidth / width) / 4;
        float h = (meshHeight / height) / 4;

        for (int i = 0; i < vertices.Length; i++)
        {
			
            if (!IsBoundary(i))
            {
                Vector3 v = vertices[i];
                v.x += (Random.value * 2 - 1) * w;
                v.z += (Random.value * 2 - 1) * h;
                vertices[i] = v;
            }
        }
    }
}
