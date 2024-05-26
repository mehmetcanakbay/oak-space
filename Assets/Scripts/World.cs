using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public static World Instance;
    public ISpacetimeGrid spacetime;

    void Awake() {
        Instance = this;
    }

    public ISpacetimeGrid GetSpacetime() {
        return spacetime;
    }

}
