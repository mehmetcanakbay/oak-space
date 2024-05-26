using UnityEngine;

public class HawkingRadiationInfoEvent : GameEvent {
    public HawkingRadiationInfoEvent(
        double Luminosity,
        double EvapRate,
        double Temp,
        double Mass
    ) {
        this.Luminosity = Luminosity;
        this.EvapRate = EvapRate;
        this.Temp = Temp;
        this.Mass = Mass;
    }

    public HawkingRadiationInfoEvent() {

    }
    
    public double Luminosity;
    public double EvapRate;
    public double Temp;
    public double Mass;
}

public static class Events {
    // public static HawkingRadiationInfoEvent HawkingRadiationInfo = new HawkingRadiationInfoEvent();
}