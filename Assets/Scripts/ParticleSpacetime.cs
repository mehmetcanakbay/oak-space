using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Unity.Mathematics;

public class ParticleSpacetime : MonoBehaviour, ISpacetimeGrid
{

    public int xSize = 20;
    public int ySize = 20;
    public int zSize = 20;

    public float offset = 0.0f;
    public ParticleSystem partSystem;

    private Vector3[] initParticlePositions;
    private ParticleSystem.Particle[] particles;
    private Vector3 spaceTimeOffset;
    // Start is called before the first frame update
    void Start()
    {
        TryGetComponent<ParticleSystem>(out partSystem);
        World.Instance.spacetime = this;
        if (partSystem == null) return;
        spaceTimeOffset = transform.position;

        //spawn all the particles
        ParticleSystem.Burst info = partSystem.emission.GetBurst(0);
        info.count = xSize*ySize*zSize; 

        partSystem.emission.SetBurst(0, info);
        
        //emit all the particles needed
        ParticleSystem.EmitParams param = new ParticleSystem.EmitParams();
        param.startSize = 0.5f;
        param.startLifetime = float.MaxValue;
        partSystem.Emit(param, xSize*ySize*zSize);

        particles = new ParticleSystem.Particle[(xSize+1) * (ySize+1) * (zSize+1)];
        initParticlePositions = new Vector3[(xSize+1) * (ySize+1) * (zSize+1)];

        partSystem.GetParticles(particles);

        int i = 0;
        for (int z = 0; z<zSize; z++) {
            for (int y = 0; y<ySize; y++) {
                for (int x=0; x<xSize; x++) {
                    particles[i].position = new Vector3((x-xSize/2)*offset,(y-ySize/2)*offset,(z-zSize/2)*offset) + spaceTimeOffset;
                    i += 1;
                }   
            }
        }

        //cache the init positions, distortion will be based on init positions, not curr positions
        for (int k = 0; k<particles.Length; k++) {
            initParticlePositions[k] = particles[k].position;
        }

        UpdateParticlePositions();
    }   

    public Vector3[] GetInitPositions() {
        return initParticlePositions;
    }

    public ParticleSystem.Particle[] GetParticles() {
        return particles;
    }

    public void SetPositions(ref NativeArray<float3> newPos) {
        for (int k = 0; k<particles.Length; k++) {
            particles[k].position = newPos[k];
        }

        UpdateParticlePositions();
    }

    private void UpdateParticlePositions() {
        partSystem.SetParticles(particles, particles.Length);
    }

    
    public int GetGridLength() {
        return (xSize) * (ySize) * (zSize);
    }


}
