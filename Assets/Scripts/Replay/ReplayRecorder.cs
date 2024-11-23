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

    private Replay _currentReplay;
    private bool _isRecord = false;
    private bool _isCollisionInteract = false;
    
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
        _isCollisionInteract = true;
        Debug.Log("collided with something");
    }
    private void OnCollisionStay(Collision collision)
    {
        _isCollisionInteract = true;
    }
    private void OnCollisionExit(Collision collision)
    {
        _isCollisionInteract = true;
        Debug.Log("exits collider");
    }

    public void StartRecord()
    {
        //if the race is with ghost, we don't need to record our ghost
        if (_raceManager.IsWithGhost == false)
        {
            StartCoroutine(Recording());
        }
    }

    public void StopRecord()
    {
        if(_raceManager.IsWithGhost == false) _isRecord = false;
    }

    //Records input and sometimes Rigidbody every FixedUpdate (WaitForFixedUpdate used for precision of a replay)
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

    //Adds current input to the Replay record
    public void Record(SimcadeVehicleController carController)//todo
    {
        //saves more information if collision happened.
        if (_isCollisionInteract)
        {
            var key = _currentReplay.ReplayInputs.Count;
            var uniqueRecord = new UniqueRecord(key, _rigidbody.position,
                _rigidbody.rotation, _rigidbody.velocity, _rigidbody.angularVelocity);

            _currentReplay.ReplayUniqueRecords.Add(uniqueRecord);
            _currentReplay.UniqueRecordKeys.Add(key);

            _isCollisionInteract = false;
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
