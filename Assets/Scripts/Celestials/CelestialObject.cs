using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public abstract class CelestialObject : MonoBehaviour
{
    public double mass = 2E+30;
    protected Transform transformCache;

    // private Vector3[] spacetimePositionCache;
    // private bool initialized = false;

    void Awake() {
        transformCache = transform;
    }

    private double NewtonGravitationForce(double otherMass, double dist) {
        double upper = Universe.G * otherMass * mass;
        return upper/dist;
    }

    /*
    protected void DistortGrid() {
        Vector3[] pos = gridToEffect.GetGridPositions();
        for (int i = 0; i<pos.Length; i++) {
            float dist = Vector3.Distance(transformCache.position, pos[i]);
            Vector3 dir = (transformCache.position-pos[i]).normalized;
            pos[i] = pos[i] + dir*(float)NewtonGravitationForce(distortionStrength, (double)dist);
        }

        gridToEffect.SetGridPositions(pos);
    }*/

    // protected void DistortGrid() {
    //     World world = Helpers.GetWorld();
    //     if (!world) {return;}
    //     // else {Debug.LogError("World is null in object: " + this.gameObject.name);}

    //     ISpacetimeGrid spacetime = world.GetSpacetime();
    //     if (spacetime == null) {return;}
    //     // else {Debug.LogError("Spacetime is null in object: " + this.gameObject.name);}
    //     //null checks are done

    //     /*
    //     //generates 15KB of GC, TODO: refactor
    //     spacetimePositionCache = (Vector3[])spacetime.GetInitPositions().Clone(); //Remember, this is a reference, so clone.
    //     */


    //     //down to 220B~ with this.
    //     Vector3[] initPosRef = spacetime.GetInitPositions();

    //     if (!initialized) {
    //         spacetimePositionCache = new Vector3[initPosRef.Length];
    //         initialized = true;
    //     }

    //     for (int k = 0; k<spacetimePositionCache.Length; k++) {
    //         spacetimePositionCache[k] = initPosRef[k];
    //     }

            
    //     for (int i = 0; i<initPosRef.Length; i++) {
    //         float dist = Vector3.Distance(transformCache.position, initPosRef[i]); //get the distance for the eq (r^2 part)
    //         //this is useless. why?
    //         // if (dist <= 0.1f) {
    //             // continue;
    //         // }

    //         Vector3 dir = (transformCache.position-initPosRef[i]).normalized; //get the direction

    //         // double computedForce = NewtonGravitationForce(distortionStrength, (double)dist);
    //         // float remappedStrength = (float)math.remap(0.0, Universe.G*mass*distortionStrength, 0.0, distortionMaxStrength, (float)einsteinForce);

    //         //delete the 1- part, no need to know "how much it is similar to a normal spacetime". besides, it breaks it.
    //         double einsteinForce = math.pow(((2*Universe.G*mass) / (dist*Universe.c*Universe.c)), 1.0); //change from newton to schwarzschild spatial distortion
    //         einsteinForce = einsteinForce <= 0 ? 0.0000001 : einsteinForce; 
    //         einsteinForce = math.sqrt(einsteinForce);
    //         // Debug.Log(einsteinForce);

    //         float remappedStrength = (float)math.remap(0.0, 1.0, 
    //         0.0, distortionStrength, 
    //         (float)einsteinForce);
            
    //         remappedStrength = Mathf.Clamp(remappedStrength, 0.0f, dist-0.1f);
    //         spacetimePositionCache[i] = initPosRef[i] + dir*((float)remappedStrength); //move particle towards dir
            
    //         //Debug.DrawRay(spacetimePositionCache[i], dir*((float)remappedStrength), Color.red, 0.1f);
    //         // Debug.Log((float)NewtonGravitationForce(distortionStrength, (double)dist));
    //         // spacetimePositionCache[i] = spacetimePositionCache[i] + dir*(float)NewtonGravitationForce(distortionStrength, (double)dist); //move particle towards dir
    //     }

    //     spacetime.SetPositions(spacetimePositionCache);
    // }

    public double GetMass() {
        return mass;
    }
}
