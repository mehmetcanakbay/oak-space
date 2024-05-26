using Unity.Collections;
using UnityEngine;

public interface ISpacetimeGrid {
    int GetGridLength();
    // void SetPositions(Vector3[] newPos);
    void SetPositions(NativeArray<Vector3> newPos);
    Vector3[] GetInitPositions();
}