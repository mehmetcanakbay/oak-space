using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Burst;
using Unity.Jobs;
using UnityEngine.Jobs;

//Non threaded version
/*public class SpacetimeSpatialDistortions : MonoBehaviour
{
    public double distortionStrength = 50.0;
    private CelestialObject[] celestialObjects;
    private Transform[] celestialObjectsTransforms;
    
    private Transform transformCache;
    private Vector3[] spacetimePositionCache;
    private bool initialized = false;


    private void Start() {
        celestialObjects = Object.FindObjectsOfType<CelestialObject>();
        celestialObjectsTransforms = new Transform[celestialObjects.Length];


        for (int i = 0; i<celestialObjects.Length; i++) {
            celestialObjectsTransforms[i] = celestialObjects[i].transform;
        }
    }

    protected void DistortGrid() {
        World world = Helpers.GetWorld();
        if (!world) {return;}
        // else {Debug.LogError("World is null in object: " + this.gameObject.name);}

        ISpacetimeGrid spacetime = world.GetSpacetime();
        if (spacetime == null) {return;}
        // else {Debug.LogError("Spacetime is null in object: " + this.gameObject.name);}
        //null checks are done

        
        //generates 15KB of GC, TODO: refactor
        spacetimePositionCache = (Vector3[])spacetime.GetInitPositions().Clone(); //Remember, this is a reference, so clone.
        


        //down to 220B~ with this.
        Vector3[] initPosRef = spacetime.GetInitPositions();

        if (!initialized) {
            spacetimePositionCache = new Vector3[initPosRef.Length];
            initialized = true;
        }

        
        for (int k = 0; k<spacetimePositionCache.Length; k++) {
            spacetimePositionCache[k] = initPosRef[k];
        }
            
        for (int i = 0; i<initPosRef.Length; i++) {
            // float maxStrength = 0.0f;
            // Vector3 maxDir = Vector3.one;
            for (int j = 0; j<celestialObjects.Length; j++) {
                double mass = celestialObjects[j].GetMass();
                transformCache = celestialObjectsTransforms[j];

                float dist = Vector3.Distance(transformCache.position, initPosRef[i]); //get the distance for the eq (r^2 part)

                Vector3 dir = (transformCache.position-initPosRef[i]).normalized; //get the direction

                //delete the 1- part, no need to know "how much it is similar to a normal spacetime". besides, it breaks it.
                double einsteinForce = math.pow(((2*Universe.G*mass) / (dist*Universe.c*Universe.c)), 1.0); //change from newton to schwarzschild spatial distortion
                einsteinForce = einsteinForce <= 0 ? 0.0000001 : einsteinForce; 
                einsteinForce = math.sqrt(einsteinForce);
                // Debug.Log(einsteinForce);

                float remappedStrength = (float)math.remap(0.0, 1.0, 
                0.0, distortionStrength, 
                (float)einsteinForce);
                
                remappedStrength = Mathf.Clamp(remappedStrength, 0.0f, dist-0.1f);
                spacetimePositionCache[i] = spacetimePositionCache[i] + dir*((float)remappedStrength); //move particle towards dir

                // maxStrength = math.max(maxStrength, remappedStrength);
                // if (remappedStrength == maxStrength) maxDir = dir;
            }

            // spacetimePositionCache[i] = spacetimePositionCache[i] + maxDir*((float)maxStrength); //move particle towards dir
        }

        spacetime.SetPositions(spacetimePositionCache);
    }

    private void Update() {
        DistortGrid();
    }
}
*/


public class SpacetimeSpatialDistortions : MonoBehaviour
{
    public double distortionStrength = 50.0;
    private int celestialObjectCount;
    private Transform[] celestialObjectsTransforms;
    private Transform transformCache;
    private bool initialized = false;

    private NativeArray<Vector3> spacetimePositionCache;
    private NativeArray<Vector3> initPositions;
    private NativeArray<double> celestialObjectMasses;
    private NativeArray<Vector3> celestialObjectPositions;
    
    private ISpacetimeGrid spacetime;

    private GridPositionJob gridPositionJob;

    private void Start() {
        CelestialObject[] celestialObjects = Object.FindObjectsOfType<CelestialObject>();
        celestialObjectCount = celestialObjects.Length;
        celestialObjectsTransforms = new Transform[celestialObjectCount];

        celestialObjectMasses = new NativeArray<double>(celestialObjectCount, Allocator.Persistent);
        for (int i = 0; i<celestialObjectCount; i++) {
            celestialObjectsTransforms[i] = celestialObjects[i].transform;
            celestialObjectMasses[i] = celestialObjects[i].GetMass();
        }

        // if (!world ) return;
        // if (spacetime == null) return;
        World world = Helpers.GetWorld();
        spacetime = world.GetSpacetime();
        if (spacetime == null) {
            Debug.LogError("Spacetime is null!!!!");
        }

        Vector3[] initPosRef = spacetime.GetInitPositions();
        initPositions = new NativeArray<Vector3>(initPosRef.Length, Allocator.Persistent);
        spacetimePositionCache = new NativeArray<Vector3>(initPosRef.Length, Allocator.Persistent);
        celestialObjectPositions = new NativeArray<Vector3>(celestialObjectCount, Allocator.Persistent);

        for (int i = 0; i<initPosRef.Length;i++) {
            initPositions[i] = initPosRef[i];
        }
    }

    protected void DistortGrid() {
        if (spacetime == null) {return;}
        // else {Debug.LogError("Spacetime is null in object: " + this.gameObject.name);}
        //null checks are done

        
        //generates 15KB of GC, TODO: refactor
        // spacetimePositionCache = (Vector3[])spacetime.GetInitPositions().Clone(); //Remember, this is a reference, so clone.
        


        //down to 220B~ with this.
        

        Vector3[] initPosRef = spacetime.GetInitPositions();
        
        for (int k = 0; k<spacetimePositionCache.Length; k++) {
            spacetimePositionCache[k] = initPosRef[k];
        }
            
        for (int i = 0; i<initPosRef.Length; i++) {
            // float maxStrength = 0.0f;
            // Vector3 maxDir = Vector3.one;
            for (int j = 0; j<celestialObjectCount; j++) {
                double mass = celestialObjectMasses[j];
                transformCache = celestialObjectsTransforms[j];

                float dist = Vector3.Distance(transformCache.position, initPosRef[i]); //get the distance for the eq (r^2 part)

                Vector3 dir = (transformCache.position-initPosRef[i]).normalized; //get the direction

                //delete the 1- part, no need to know "how much it is similar to a normal spacetime". besides, it breaks it.
                double einsteinForce = math.pow(((2*Universe.G*mass) / (dist*Universe.c*Universe.c)), 1.0); //change from newton to schwarzschild spatial distortion
                einsteinForce = einsteinForce <= 0 ? 0.0000001 : einsteinForce; 
                einsteinForce = math.sqrt(einsteinForce);
                // Debug.Log(einsteinForce);

                float remappedStrength = (float)math.remap(0.0, 1.0, 
                0.0, distortionStrength, 
                (float)einsteinForce);
                
                remappedStrength = Mathf.Clamp(remappedStrength, 0.0f, dist-0.1f);
                spacetimePositionCache[i] = spacetimePositionCache[i] + dir*((float)remappedStrength); //move particle towards dir
            }
        }

    }

    private void Update() {
        // DistortGrid();
        for (int i = 0; i<celestialObjectCount; i++) {
            celestialObjectPositions[i] = celestialObjectsTransforms[i].position;
        }
        gridPositionJob = new GridPositionJob();
        gridPositionJob.initPosRef = initPositions;
        gridPositionJob.spacetimePositionCache = spacetimePositionCache;
        gridPositionJob.celestialObjectMasses = celestialObjectMasses;
        gridPositionJob.celestialObjectPositions = celestialObjectPositions;
        gridPositionJob.distortionStrength = distortionStrength;
        gridPositionJob.celestialObjectCount = celestialObjectCount;
        gridPositionJob.Schedule(initPositions.Length, 64).Complete();

        spacetime.SetPositions(spacetimePositionCache);
    }

    private void OnDestroy() {
        spacetimePositionCache.Dispose();
        initPositions.Dispose();
        celestialObjectMasses.Dispose();
        celestialObjectPositions.Dispose();
    }
}

[BurstCompile]
internal struct GridPositionJob : IJobParallelFor {
    public NativeArray<Vector3> spacetimePositionCache;
    [ReadOnly] public NativeArray<Vector3> initPosRef;
    [ReadOnly] public NativeArray<double> celestialObjectMasses;
    [ReadOnly] public NativeArray<Vector3> celestialObjectPositions;
    public double distortionStrength;
    public int celestialObjectCount;

    public void Execute(int i) {
        spacetimePositionCache[i] = initPosRef[i];

        for (int j = 0; j<celestialObjectCount; j++) {
            double mass = celestialObjectMasses[j];

            float dist = Vector3.Distance(celestialObjectPositions[j], initPosRef[i]); //get the distance for the eq (r^2 part)

            Vector3 dir = (celestialObjectPositions[j]-initPosRef[i]).normalized; //get the direction

            //delete the 1- part, no need to know "how much it is similar to a normal spacetime". besides, it breaks it.
            double einsteinForce = math.pow(((2*Universe.G*mass) / (dist*Universe.c*Universe.c)), 1.0); //change from newton to schwarzschild spatial distortion
            einsteinForce = einsteinForce <= 0 ? 0.0000001 : einsteinForce; 
            einsteinForce = math.sqrt(einsteinForce);
            // Debug.Log(einsteinForce);

            float remappedStrength = (float)math.remap(0.0, 1.0, 
            0.0, distortionStrength, 
            (float)einsteinForce);
            
            remappedStrength = Mathf.Clamp(remappedStrength, 0.0f, dist-0.1f);
            spacetimePositionCache[i] = spacetimePositionCache[i] + dir*((float)remappedStrength); //move particle towards dir
        }
    }
}
