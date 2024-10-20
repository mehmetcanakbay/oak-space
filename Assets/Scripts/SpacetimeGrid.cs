using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Unity.Mathematics;
using System.Runtime.InteropServices.WindowsRuntime;

//It's ~10FPS improvement frm the particlespactime.
//I'll keep this because this can cull. Particle one doesnt cull, it renders all the spheres.
public class SpacetimeGrid : MonoBehaviour, ISpacetimeGrid
{
    [SerializeField] public int xSize = 20;
    [SerializeField] public int ySize = 20;
    [SerializeField] public int zSize = 20;
    [SerializeField] public float offset = 0.0f;
    [SerializeField] public float nodeSizes = 0.1f;
    [SerializeField] private Mesh mesh;
    [SerializeField] private Material material;

    private Vector3[] initPositions;
    private Vector3[] currPositions;
    private Vector3 spaceTimeOffset;
    private Matrix4x4[] matricesTRS;
    private RenderParams renderParams;
    private MaterialPropertyBlock materialPropertyBlock;

    private const int MaxInstances = 2048; 
    private Matrix4x4[] batchMatrices; // Preallocate a batch array to reuse

    public void Initialize() {
        World.Instance.spacetime = this as ISpacetimeGrid;
        spaceTimeOffset = transform.position;
        renderParams = new RenderParams(material);
        materialPropertyBlock = new MaterialPropertyBlock();
        batchMatrices = new Matrix4x4[MaxInstances];

        CreateShape();
    }

    void Start() {

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


    void RenderShapes() {
        //why do I have to do this? I dont understand.
        //In this video: https://www.youtube.com/watch?v=6mNj3M1il_c
        //He calls RenderMeshInstanced one time, for 90k cubes and it just works.
        //Mine doesnt. Everywhere on Google says that there is a 1023 limit. I'm confused.
        int totalInstances = currPositions.Length;
        int batchCount = Mathf.CeilToInt((float)totalInstances / MaxInstances);
        //well it works in ver. 2024? big perf. improvement as well. 120 fps -> 160fps
        Graphics.RenderMeshInstanced(renderParams, mesh, 0, matricesTRS);

        //so this becomes useless
        /******************************************DEPRECATED
        for (int i = 0; i < batchCount; i++) {
            int startIndex = i * MaxInstances;
            int count = Mathf.Min(MaxInstances, totalInstances - startIndex);

            for (int k = 0; k < count; k++) {
                batchMatrices[k] = matricesTRS[startIndex + k];
            }

            Graphics.DrawMeshInstanced(mesh, 0, material, batchMatrices, count, materialPropertyBlock, UnityEngine.Rendering.ShadowCastingMode.Off, false); //this doesnt generate garbage but it makes FPS slower cus no culling
            // Graphics.RenderMeshInstanced(renderParams, mesh, 0, batchMatrices, count);
        }
        **************************************************/
    }

    public Vector3[] GetInitPositions() {
        return initPositions;
    }

    public void SetPositions(ref NativeArray<float3> newPos) {
        for (int k = 0; k<currPositions.Length; k++) {
            matricesTRS[k].SetTRS(newPos[k], Quaternion.identity, Vector3.one*nodeSizes);
        }

        RenderShapes();
    }

    public int GetGridLength() {
        return (xSize) * (ySize) * (zSize);
    }

    public Matrix4x4[] GetMatrixTRS() {
        return matricesTRS;
    }

}
