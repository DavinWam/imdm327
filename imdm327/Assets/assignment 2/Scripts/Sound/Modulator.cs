using UnityEngine;

[CreateAssetMenu(fileName = "NewModulator", menuName = "Sound/Modulator")]
public class Modulator : Operator
{
    public override float GetQuantizedFrequency()
    {
        // Add any modulator-specific functionality if needed,
        // or simply call the base method if the behavior is the same.
        return base.GetQuantizedFrequency();
    }
}
