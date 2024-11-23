using Cinemachine;
using System;
using UnityEngine;
using UnityEngine.Events;

public class FinishLine : MonoBehaviour
{
    public event Action RaceCompleted;

    [TagField, SerializeField]//the object will need to have this tag to trigger FinishLine
    private string _targetTag;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(_targetTag))
        {
            FinishRace();
        }
    }

    private void FinishRace()
    {
        RaceCompleted.Invoke();
        OnRaceFinish.Invoke();
    }

    public UnityEvent OnRaceFinish;
}
