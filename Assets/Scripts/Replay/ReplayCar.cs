using Ashsvp;
using System.Collections;
using System.IO;
using UnityEngine;

public class ReplayCar : MonoBehaviour 
{ 
    private readonly string REPLAYS_PATH = Path.Combine(Application.dataPath, "Replays");

    [SerializeField]
    private SimcadeVehicleController _ghostCarController;
    [SerializeField]
    private Rigidbody _rigidbody;
    private Replay _currentReplay;
    private int _currentRecord = 0;
    private bool _isPlaying = false;

    private void Start()
    {
        Init();
    }
     //Initializes ghost-car
    public void Init()
    {
        if (LoadReplay("lastReplay"))
        {
            _ghostCarController.CanDrive = false;
            Debug.Log("Ghost initialized");
        }
    }

    public void StartReplay()
    {
        _ghostCarController.CanDrive = true;
        StartCoroutine(PlayReplay());
    }

    //Stops ghost-car after replay is finished.
    public void StopReplay()
    {
        _isPlaying = false;
        _ghostCarController.CanDrive = false;
        _ghostCarController.CanAccelerate = false;
        _ghostCarController.accelerationInput = 0;
        _ghostCarController.steerInput = 0;
        _ghostCarController.brakeInput = 1f;
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
    }

    //Applies inputs in replay every FixedUpdate and sometimes Rigidbody parameters
    //(WaitForFixedUpdate is used for precision of a replay)
    private IEnumerator PlayReplay()
    {
        _isPlaying = true;
        while (_isPlaying)
        {
            ApplyInputRecord(_ghostCarController);
            yield return new WaitForFixedUpdate();
        }
    }

    private bool ApplyInputRecord(SimcadeVehicleController carController)
    {
        //checking if replay ended
        if (_currentRecord >= _currentReplay.ReplayInputs.Count)
        {
            StopReplay();
            return false;
        }
        var inputRecord = _currentReplay.ReplayInputs[_currentRecord];
        carController.accelerationInput = inputRecord.AccelerationInput;
        carController.steerInput = inputRecord.SteerInput;
        carController.brakeInput = inputRecord.BrakeInput;
        //checking if unique moment happened at this moment
        if(_currentReplay.RigidbodyRecordKeys.Contains(_currentRecord))
        {
            var rigidbodyRecord = _currentReplay.ReplayRigidbodyRecords.Find(r => r.Key == _currentRecord);
            _rigidbody.velocity = rigidbodyRecord.RigidbodyVelocity;
            _rigidbody.angularVelocity = rigidbodyRecord.AngularVelocity;
            _rigidbody.Move(rigidbodyRecord.CarPosition, rigidbodyRecord.RigidbodyRotation);
            Debug.Log("Unique Record Played");
        }
        _currentRecord++;
        return true;
    }

    /// <summary>
    /// Loads replay for a ghost-car. If replay load is successful returns true, otherwise returns false.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool LoadReplay(string replayName)
    {
        string jsonReplay = "";
        string replayPath = Path.Combine(REPLAYS_PATH, $"{replayName}.json");
        if (File.Exists(replayPath))
        {
            jsonReplay = File.ReadAllText(replayPath);
            _currentReplay = JsonUtility.FromJson<Replay>(jsonReplay);
            return true;
        }
        Debug.Log($"Save with \"{Path.GetFileName(replayPath)}\" name doesn't exist");
        return false;
    }
}
