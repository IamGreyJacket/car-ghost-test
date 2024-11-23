using System;
using UnityEngine;

[Serializable]
public struct InputRecord //contains data of car inputs as one record.
{
    public InputRecord(float accelerationInput, float steerInput, float brakeInput)
    {
        AccelerationInput = accelerationInput;
        SteerInput = steerInput;
        BrakeInput = brakeInput;
    }
    public float AccelerationInput;
    public float SteerInput;
    public float BrakeInput;
    
}
