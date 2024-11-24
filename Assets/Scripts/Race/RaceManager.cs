using Ashsvp;
using Cinemachine;
using System;
using System.Collections.Generic;
using UnityEngine;

public class RaceManager : MonoBehaviour
{
    public event Action RaceStarted;
    public event Action RaceCompleted;

    [SerializeField]
    private ReplayCar _ghostCarPrefab;
    private ReplayCar _currentGhostCar;
    [Space]
    private SimcadeVehicleController _playerCar;
    private bool _playerSubscribed = false;
    [TagField, SerializeField]
    private string _playerTag = "Player";
    public bool IsWithGhost = false;

    [Space, SerializeField]
    private Transform _startPosition;

    [SerializeField]
    private FinishLine _finishLine;

    private void Awake()
    {
        _finishLine.RaceCompleted += FinishRace;
    }

    private void Start()
    {
        PrepareForRace();
    }

    //Teleports participants (cars that will participate in a race) to a start point of the race
    public void PrepareForRace()
    {
        if (FindPlayer())
        {
            /*
            foreach (var participant in _participants)
            {
                participant.ForbidToDrive();
                participant.Rigidbody.velocity = Vector3.zero;
                participant.Rigidbody.Move(_startPosition.position, _startPosition.rotation);

                RaceStarted += participant.AllowToDrive;
                RaceCompleted += participant.ForbidToDrive;
            }
            */
            ForbidPlayerToDrive();
            var playerRigidbody = _playerCar.GetComponent<Rigidbody>();
            playerRigidbody.velocity = Vector3.zero;
            playerRigidbody.angularVelocity = Vector3.zero;
            playerRigidbody.Move(_startPosition.position, _startPosition.rotation);

        }
        if (IsWithGhost) CreateGhostCar();
        else DestroyGhostCar();
    }

    /// <summary>
    /// Finds player car
    /// </summary>
    /// <returns></returns>
    public bool FindPlayer()
    {
        var cars = new List<SimcadeVehicleController>(FindObjectsOfType<SimcadeVehicleController>());
        _playerCar = cars.Find(c => c.CompareTag(_playerTag));
        if (_playerCar != null)
        {
            if (_playerSubscribed == false)
            {
                RaceStarted += AllowPlayerToDrive;
                RaceCompleted += ForbidPlayerToDrive;
                _playerSubscribed = true;
            }
            return true;
        }
        return false;
    }

    public void AllowPlayerToDrive()
    {
        _playerCar.CanDrive = true;
        _playerCar.CanAccelerate = true;
    }
    public void ForbidPlayerToDrive()
    {
        _playerCar.CanDrive = false;
        _playerCar.CanAccelerate = false;
        _playerCar.accelerationInput = 0;
        _playerCar.steerInput = 0;
        _playerCar.brakeInput = 1f;
    }

    public void SetIsWithGhost(bool isWithGhost)
    {
        IsWithGhost = isWithGhost;
    }

    private void CreateGhostCar()
    {
        DestroyGhostCar();
        _currentGhostCar = Instantiate(_ghostCarPrefab, _startPosition.position, _startPosition.rotation);
        Debug.Log($"Ghost car created at {_currentGhostCar.transform.position}");
        _currentGhostCar.Init();
        RaceStarted += _currentGhostCar.StartReplay;
    }

    private void DestroyGhostCar()
    {
        if (_currentGhostCar != null)
        {
            RaceStarted -= _currentGhostCar.StartReplay;
            Destroy(_currentGhostCar.gameObject);
            _currentGhostCar = null;
        }
    }

    public void StartRace()
    {
        RaceStarted.Invoke();
    }
    public void FinishRace()
    {
        RaceCompleted.Invoke();
    }

    private void OnDestroy()
    {
        _finishLine.RaceCompleted -= FinishRace;
    }
}
