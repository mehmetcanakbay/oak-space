using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct LuminosityInfo {
    public LuminosityInfo(double lum, double rate) {
        Luminosity = lum;
        EvaporationRatePerSecond = rate;
    }
    public double Luminosity;
    public double EvaporationRatePerSecond;
}

public class BlackHole : CelestialObject
{
    public double unitChange = 86400; //from seconds to days.
    /// <summary>
    /// Events user can subscribe to for various reasons (in this case UI)
    /// </summary>

    private double initMass;
    public float blackHoleSize = 3.0f;

    private HawkingRadiationInfoEvent evt;

    private void Start() {
        initMass = mass;
        evt = new HawkingRadiationInfoEvent();

    }

    public double Luminosity(double mass) {
        double upper = Universe.reducedPlanck * (Universe.c*Universe.c*Universe.c) * (Universe.c*Universe.c*Universe.c);
        double lower = 15360.0 * (double)Mathf.PI * Universe.G*Universe.G *mass*mass;
        return upper/lower;
    }

    public LuminosityInfo EvaporationRatePerSec(double mass) {
        //E=mc^2
        //m = E/c^2
        double energy = Luminosity(mass);
        double evapRate = energy/(Universe.c*Universe.c)*Simulation.timeScale*unitChange;
        return new LuminosityInfo(
            energy, evapRate
        );
    }


    public double Temperature(double mass) {
        double upper = (Universe.reducedPlanck * (Universe.c*Universe.c*Universe.c));
        double lower = (8.0*(double)Mathf.PI * Universe.G * mass * Universe.boltzmann);
        return upper/lower;
    } 

    public double DissapationTime(double mass) {
        double upper = 5120.0 * (double)Mathf.PI * (Universe.G*Universe.G) * (mass*mass*mass);
        double lower = Universe.reducedPlanck * (Universe.c*Universe.c*Universe.c*Universe.c); 
        return upper/lower;
    }

    double SecondsToYears(double val) {
        return val/Universe.secondsToYears;
    }

    private void Update() {
        transformCache.localScale = (float)(mass/initMass) * blackHoleSize * Vector3.one;
        LuminosityInfo luminfo = EvaporationRatePerSec(mass);
        if (mass>0.0) {
            double val = Math.Clamp(luminfo.EvaporationRatePerSecond, 0.0, mass);
            mass-=val;
        } 
        
        else {
            return;
        }

        // HawkingRadiationInfoEvent evt = new HawkingRadiationInfoEvent(
        //     luminfo.Luminosity,
        //     luminfo.EvaporationRatePerSecond,
        //     Temperature(mass),
        //     mass
        // );

        //this should be more optimized
        evt.Luminosity = luminfo.Luminosity;    
        evt.EvapRate = luminfo.EvaporationRatePerSecond;    
        evt.Temp = Temperature(mass);
        evt.Mass = mass;    

        EventManager.Broadcast(evt);


        // LuminosityEvent?.Invoke(luminfo.Luminosity);
        // EvapRateEvent?.Invoke(luminfo.EvaporationRatePerSecond);
        // MassEvent?.Invoke(mass);
        // TempEvent?.Invoke(Temperature(mass));
    }

    
}
