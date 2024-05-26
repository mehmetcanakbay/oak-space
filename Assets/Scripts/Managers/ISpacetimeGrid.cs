using Unity.Collections;
using UnityEngine;
using Unity.Mathematics;

public interface ISpacetimeGrid {
    int GetGridLength();
    // void SetPositions(Vector3[] newPos);
    void SetPositions(ref NativeArray<float3> newPos);
    Vector3[] GetInitPositions();
}