using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct UniqueRecord 
{
    //contains more information for a Replay. Usually used for collision moments
    public UniqueRecord(int key, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angularVelocity)
    {
        Key = key;
        CarPosition = position;
        RigidbodyRotation = rotation;
        RigidbodyVelocity = velocity;
        AngularVelocity = angularVelocity;
    }
    public int Key;
    public Vector3 CarPosition;
    public Quaternion RigidbodyRotation;
    public Vector3 RigidbodyVelocity;
    public Vector3 AngularVelocity;
    
}
