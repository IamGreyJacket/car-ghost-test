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
    public bool IsWithGhost = false;
    private List<RaceParticipant> _participants = new List<RaceParticipant>();

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
        if (FindParticipants())
        {
            foreach (var participant in _participants)
            {
                participant.ForbidToDrive();
                participant.Rigidbody.velocity = Vector3.zero;
                participant.Rigidbody.Move(_startPosition.position, _startPosition.rotation);

                RaceStarted += participant.AllowToDrive;
                RaceCompleted += participant.ForbidToDrive;
            }
        }
        if (IsWithGhost) CreateGhostCar();
        else DestroyGhostCar();
    }

    /// <summary>
    /// Finds all RaceParticipant. If found at least one - returns true, otherwise returns false.
    /// </summary>
    /// <returns></returns>
    public bool FindParticipants()
    {
        //if there were any participants in out List, we unsubscribe them from RaceManager events just in case.
        if(_participants.Count > 0)
        {
            foreach(var participant in _participants)
            {
                RaceStarted -= participant.AllowToDrive;
                RaceCompleted -= participant.ForbidToDrive;
            }
        }
        var participants = new List<RaceParticipant>(FindObjectsOfType<RaceParticipant>());
        if (participants != null && participants.Count > 0)
        {
            _participants = participants;
            return true;
        }
        return false;
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
