using Ashsvp;
using System.Collections;
using System.IO;
using UnityEngine;

public class ReplayRecorder : MonoBehaviour
{
    private readonly string REPLAYS_PATH = Path.Combine(Application.dataPath, "Replays");

    [SerializeField]
    private RaceManager _raceManager;

    [Space, SerializeField]
    private SimcadeVehicleController _carController;
    [SerializeField]
    private Rigidbody _rigidbody;

    [SerializeField, Min(0)]
    private float _rigidbodyRecordDelay = 0.2f; //seconds.
    private Replay _currentReplay;
    private bool _isRecord = false;
    private bool _isRigidbodyRecord = false;
    
    //public bool IntervalIsFixedDeltaTime = false;
    private void Awake()
    {
        _raceManager.RaceStarted += StartRecord;
        _raceManager.RaceCompleted += StopRecord;
    }

    private void OnDestroy()
    {
        _raceManager.RaceStarted -= StartRecord;
        _raceManager.RaceCompleted -= StopRecord;
    }

    //since collisions may RUIN the replay, we mark collision event as "Unique".
    //UniqueRecord will provide more information for a more precise replay.
    private void OnCollisionEnter(Collision collision)
    {
        _isRigidbodyRecord = true;
        Debug.Log("collided with something");
    }
    private void OnCollisionStay(Collision collision)
    {
        _isRigidbodyRecord = true;
    }
    private void OnCollisionExit(Collision collision)
    {
        _isRigidbodyRecord = true;
        Debug.Log("exits collider");
    }

    public void StartRecord()
    {
        //if the race is with ghost, we don't need to record our ghost
        if (_raceManager.IsWithGhost == false)
        {
            StartCoroutine(Recording());
            StartCoroutine(RigidbodyRecordDelay());
        }
    }

    public void StopRecord()
    {
        _isRecord = false;
    }

    //Records input every FixedUpdate and sometimes Rigidbody information
    //(WaitForFixedUpdate used for precision of a replay)
    private IEnumerator Recording()
    {
        _currentReplay = new Replay();
        _isRecord = true;
        while (_isRecord)
        {
            Record(_carController);
            yield return new WaitForFixedUpdate();
        }
        SaveReplay("lastReplay");
    }

    //Delays the moment when more information for a replay will be saved
    //made to make a replay more precise, but also to save memory, taken by replay, at the cost of precision
    private IEnumerator RigidbodyRecordDelay()
    {
        while (_isRecord)
        {
            yield return new WaitForSeconds(_rigidbodyRecordDelay);
            _isRigidbodyRecord = true;
            Debug.Log("Unique moment delay passed, NOW is unique moment");
        }
        yield return null;
    }

    //Adds current input to the Replay record
    public void Record(SimcadeVehicleController carController)
    {
        //saves more information if collision happened.
        if (_isRigidbodyRecord)
        {
            var key = _currentReplay.ReplayInputs.Count;
            var rigidbodyRecord = new RigidbodyRecord(key, _rigidbody.position,
                _rigidbody.rotation, _rigidbody.velocity, _rigidbody.angularVelocity);

            _currentReplay.ReplayRigidbodyRecords.Add(rigidbodyRecord);
            _currentReplay.RigidbodyRecordKeys.Add(key);

            _isRigidbodyRecord = false;
        }
        //saves inputs
        var inputRecord = new InputRecord(carController.accelerationInput, carController.steerInput, carController.brakeInput);
        _currentReplay.ReplayInputs.Add(inputRecord);

    }

    //Saves replay into .json file
    private void SaveReplay(string replayName)
    {
        var jsonReplay = JsonUtility.ToJson(_currentReplay);
        if (!Directory.Exists(REPLAYS_PATH)) Directory.CreateDirectory(REPLAYS_PATH);
        File.WriteAllText(Path.Combine(REPLAYS_PATH, $"{replayName}.json"), jsonReplay);
    }
}
