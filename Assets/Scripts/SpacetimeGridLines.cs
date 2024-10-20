using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpacetimeGridLines : MonoBehaviour
{
    // Start is called before the first frame update
    SpacetimeGrid grid;
    public Material mat;
    Mesh gridMesh;
    int[] indices;
    Vector3[] vertexes;
    Matrix4x4[] matrixTRS;

    int xSize = 0; 
    int ySize = 0; 
    int zSize = 0; 

    int totalLength;

    Camera targetCamera;
    MaterialPropertyBlock materialPropertyBlock ;

    bool stopDrawing = false;
    void Start()
    {
        materialPropertyBlock = new MaterialPropertyBlock();
        if (mat == null)
			mat = new Material(Shader.Find("Unlit/Color"));

        targetCamera = GameObject.FindGameObjectWithTag("LineCamera") ? GameObject.FindGameObjectWithTag("LineCamera").GetComponent<Camera>() : null;

        TryGetComponent<SpacetimeGrid>(out grid);
        if (grid == null) {
            Debug.LogError("Grid is null!!");
        } else {
            xSize = grid.xSize;
            ySize = grid.ySize;
            zSize = grid.zSize;
            totalLength = grid.GetGridLength();
            matrixTRS = new Matrix4x4[xSize*ySize*zSize];
            vertexes = new Vector3[totalLength];
            CreateMesh();
        }
    }

    public void Reset() {
        stopDrawing = true;
        xSize = grid.xSize;
        ySize = grid.ySize;
        zSize = grid.zSize;
        totalLength = grid.GetGridLength();
        matrixTRS = new Matrix4x4[xSize*ySize*zSize];
        vertexes = new Vector3[totalLength];
        CreateMesh();
        stopDrawing = false;
    }

    void CreateMesh() {
        matrixTRS = grid.GetMatrixTRS();
        
        for (int i = 0; i<totalLength;i++) {
            vertexes[i] = matrixTRS[i].GetPosition();
        }

        gridMesh = new Mesh();

        //GOAT-GPT
        int linesCount = (xSize - 1) * zSize * ySize + (zSize - 1) * xSize * ySize + (ySize - 1) * xSize * zSize;
        indices = new int[linesCount*2];
        CalcIndices();
        gridMesh.vertices = vertexes;
        gridMesh.SetIndices(indices, MeshTopology.Lines, 0);

    }

    void UpdateMesh() {
        matrixTRS = grid.GetMatrixTRS();
        
        for (int i = 0; i<totalLength;i++) {
            vertexes[i] = matrixTRS[i].GetPosition();
        }

        CalcIndices();

        gridMesh.vertices = vertexes;
        gridMesh.SetIndices(indices, MeshTopology.Lines, 0);
    }

    void CalcIndices() {
        int index = 0;

        for (int z = 0; z < zSize; z++)
        {
            for (int y = 0; y < ySize; y++)
            {
                for (int x = 0; x < xSize - 1; x++)
                {
                    indices[index++] = (z * ySize + y) * xSize + x;       // Current point
                    indices[index++] = (z * ySize + y) * xSize + (x + 1); // Next point
                }
            }
        }

        // Vertical lines along the y-axis
        for (int x = 0; x < xSize; x++)
        {
            for (int z = 0; z < zSize; z++)
            {
                for (int y = 0; y < ySize - 1; y++)
                {
                    indices[index++] = (z * ySize + y) * xSize + x;       // Current point
                    indices[index++] = (z * ySize + (y + 1)) * xSize + x; // Point above
                }
            }
        }

        // Vertical lines along the z-axis
        for (int y = 0; y < ySize; y++)
        {
            for (int x = 0; x < xSize; x++)
            {
                for (int z = 0; z < zSize - 1; z++)
                {
                    indices[index++] = (z * ySize + y) * xSize + x;       // Current point
                    indices[index++] = ((z + 1) * ySize + y) * xSize + x; // Point in front
                }
            }
        }

    }

    // Update is called once per frame
    void OnRenderObject()
    {
        if (stopDrawing) return;
        UpdateMesh();
        mat.SetPass(0);
		// Graphics.DrawMesh(gridMesh, transform.localToWorldMatrix, mat, LayerMask.NameToLayer("SpacetimeLines"), targetCamera ? targetCamera: null, 0, materialPropertyBlock, false, false);
		Graphics.DrawMeshNow(gridMesh, transform.localToWorldMatrix, 0);
    }
}
