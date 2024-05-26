using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.PlayerLoop;

//It's ~10FPS improvement frm the particlespactime.
//I'll keep this because this can cull. Particle one doesnt cull, it renders all the spheres.
public class SpacetimeGrid : MonoBehaviour, ISpacetimeGrid
{
    [SerializeField] private int xSize = 20;
    [SerializeField] private int ySize = 20;
    [SerializeField] private int zSize = 20;
    [SerializeField] private float offset = 0.0f;
    [SerializeField] private float nodeSizes = 0.1f;
    [SerializeField] private Mesh mesh;
    [SerializeField] private Material material;

    private Vector3[] initPositions;
    private Vector3[] currPositions;
    private Vector3 spaceTimeOffset;
    private Matrix4x4[] matricesTRS;
    private RenderParams renderParams;

    private const int MaxInstances = 1023; 
    private Matrix4x4[] batchMatrices; // Preallocate a batch array to reuse

    void Start() {
        World.Instance.spacetime = this;
        spaceTimeOffset = transform.position;
        renderParams = new RenderParams(material);

        batchMatrices = new Matrix4x4[MaxInstances];

        CreateShape();
    }

    void CreateShape() {
        currPositions = new Vector3[(xSize) * (ySize) * (zSize)];
        initPositions = new Vector3[(xSize) * (ySize) * (zSize)];
        matricesTRS = new Matrix4x4[(xSize) * (ySize) * (zSize)];

        int i = 0;
        for (int z = 0; z<zSize; z++) {
            for (int y = 0; y<ySize; y++) {
                for (int x=0; x<xSize; x++) {
                    currPositions[i] = new Vector3((x-xSize/2)*offset,(y-ySize/2)*offset,(z-zSize/2)*offset) + spaceTimeOffset;
                    matricesTRS[i].SetTRS(currPositions[i], Quaternion.identity, Vector3.one*nodeSizes);
                    i += 1;
                }   
            }
        }

        initPositions = (Vector3[])currPositions.Clone();
    }

    void Update() {
        // RenderShapes();
    }

    void RenderShapes() {
        //why do I have to do this? I dont understand.
        //In this video: https://www.youtube.com/watch?v=6mNj3M1il_c
        //He calls RenderMeshInstanced one time, for 90k cubes and it just works.
        //Mine doesnt. Everywhere on Google says that there is a 1023 limit. I'm confused.
        int totalInstances = currPositions.Length;
        int batchCount = Mathf.CeilToInt((float)totalInstances / MaxInstances);

        for (int i = 0; i < batchCount; i++) {
            int startIndex = i * MaxInstances;
            int count = Mathf.Min(MaxInstances, totalInstances - startIndex);

            for (int k = 0; k < count; k++) {
                batchMatrices[k] = matricesTRS[startIndex + k];
            }

            Graphics.RenderMeshInstanced(renderParams, mesh, 0, batchMatrices, count);
        }
    }

    public Vector3[] GetInitPositions() {
        return initPositions;
    }

    public void SetPositions(NativeArray<Vector3> newPos) {
        for (int k = 0; k<currPositions.Length; k++) {
            matricesTRS[k].SetTRS(newPos[k], Quaternion.identity, Vector3.one*nodeSizes);
        }

        RenderShapes();
    }

    public int GetGridLength() {
        return (xSize) * (ySize) * (zSize);
    }

}
