using Ashsvp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceParticipant : MonoBehaviour
{
    [SerializeField]
    private SimcadeVehicleController _carController;
    [SerializeField]
    private Rigidbody _rigidbody;
    public SimcadeVehicleController CarController => _carController;
    public Rigidbody Rigidbody => _rigidbody;

    public void AllowToDrive()
    {
        _carController.CanDrive = true;
        _carController.CanAccelerate = true;
    }
    public void ForbidToDrive()
    {
        _carController.CanDrive = false;
        _carController.CanAccelerate = false;
        _carController.accelerationInput = 0;
        _carController.steerInput = 0;
        _carController.brakeInput = 1f;
    }
}
