// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class SpacetimeLineDrawer : MonoBehaviour
// {
//     public int xSize = 10; // Set your grid size here
//     public int ySize = 10;
//     public int zSize = 10;
//     public float offset = 1.0f; // Distance between points
//     public Vector3 spaceTimeOffset; // Offset for the grid
//     public LineRenderer lineRenderer;

//     private void Start()
//     {
//         CreateGridLines();
//     }

//     // private void CreateGridLines()
//     // {
//     //     int lrIndex = 0;
//     //     int lrOffset = 0;
//     //     lineRenderer.positionCount = 5*5*5 + 5;

//     //     for (int i = 0; i < 5; i++)
//     //     {
//     //         for (int j = 0; j < 5; j++)
//     //         {
//     //             lineRenderer.SetPosition(lrIndex * 5 + 0 + lrOffset, new Vector3(i, j));
//     //             lineRenderer.SetPosition(lrIndex * 5 + 1 + lrOffset, new Vector3(i, j + 1));
//     //             lineRenderer.SetPosition(lrIndex * 5 + 2 + lrOffset, new Vector3(i + 1, j + 1));
//     //             lineRenderer.SetPosition(lrIndex * 5 + 3 + lrOffset, new Vector3(i + 1, j));
//     //             lineRenderer.SetPosition(lrIndex * 5 + 4 + lrOffset, new Vector3(i, j));
//     //             if (j == 5 - 1)
//     //             {
//     //                 lineRenderer.SetPosition(lrIndex * 5 + 5 + lrOffset, new Vector3(i + 1, j));
//     //                 lrOffset++;
//     //             }
//     //             lrIndex++;
//     //         }
//     //     }
//     // }
//     private void CreateGridLines()
// {
//     int lrIndex = 0;
//     int lrOffset = 0;
//     int gridSize = 5; // Size of the grid in each dimension
//     lineRenderer.positionCount = gridSize * gridSize * gridSize * 5 + gridSize * gridSize; // Adjust for lines and gaps

//     for (int z = 0; z < gridSize; z++)
//     {
//         for (int i = 0; i < gridSize; i++)
//         {
//             for (int j = 0; j < gridSize; j++)
//             {
//                 // Set positions for the square in the x-y plane at fixed z
//                 lineRenderer.SetPosition(lrIndex * 5 + 0 + lrOffset, new Vector3(i, j, z));
//                 lineRenderer.SetPosition(lrIndex * 5 + 1 + lrOffset, new Vector3(i, j + 1, z));
//                 lineRenderer.SetPosition(lrIndex * 5 + 2 + lrOffset, new Vector3(i + 1, j + 1, z));
//                 lineRenderer.SetPosition(lrIndex * 5 + 3 + lrOffset, new Vector3(i + 1, j, z));
//                 lineRenderer.SetPosition(lrIndex * 5 + 4 + lrOffset, new Vector3(i, j, z));
                
//                 // Draw vertical lines to connect to the next z level
//                 if (j == gridSize - 1)
//                 {
//                     lineRenderer.SetPosition(lrIndex * 5 + 5 + lrOffset, new Vector3(i, j, z + 1));
//                     lrOffset++;
//                 }

//                 lrIndex++;
//             }
//         }
//     }
// }
// }

using UnityEngine;

public class SpacetimeLineDrawer : MonoBehaviour
{
    public int width = 10;  // Number of lines in the x-direction
    public int depth = 10;  // Number of lines in the z-direction
    public int height = 10; // Number of lines in the y-direction
    public float spacing = 1.0f; // Space between lines
    public Material lineMaterial; // Material for the line

    private void Start()
    {
        CreateGrid();
    }

    private void CreateGrid()
    {
        // Create lines in the XZ plane (horizontal)
        for (int i = 0; i <= width; i++)
        {
            CreateLine(new Vector3(i * spacing, 0, 0), new Vector3(i * spacing, 0, depth * spacing)); // XZ
        }

        for (int j = 0; j <= depth; j++)
        {
            CreateLine(new Vector3(0, 0, j * spacing), new Vector3(width * spacing, 0, j * spacing)); // XZ
        }

        // Create lines in the YZ plane (vertical)
        for (int k = 0; k <= height; k++)
        {
            CreateLine(new Vector3(0, k * spacing, 0), new Vector3(0, k * spacing, depth * spacing)); // YZ
        }

        // Create lines in the XY plane (vertical)
        for (int m = 0; m <= width; m++)
        {
            CreateLine(new Vector3(m * spacing, 0, 0), new Vector3(m * spacing, height * spacing, 0)); // XY
        }

        for (int n = 0; n <= depth; n++)
        {
            CreateLine(new Vector3(0, 0, n * spacing), new Vector3(width * spacing, 0, n * spacing)); // XY
        }
    }

    private void CreateLine(Vector3 start, Vector3 end)
    {
        GameObject lineObject = new GameObject("Line");
        LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();
        lineRenderer.material = lineMaterial;
        lineRenderer.startWidth = 0.05f; // Width of the line
        lineRenderer.endWidth = 0.05f; // Width of the line
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
    }
}