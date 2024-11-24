using System;
using System.Collections.Generic;

[Serializable]
public class Replay
{
    //List contains input records, which contains inputs for every FixedUpdate call
    public List<InputRecord> ReplayInputs = new List<InputRecord>();
    //List contains more information for rigidbody records
    //for moments such as collisions, that may RUIN the replay.
    public List<RigidbodyRecord> ReplayRigidbodyRecords = new List<RigidbodyRecord>();
    //List contains WHEN rigidbody record needs to be played.
    public List<int> RigidbodyRecordKeys = new List<int>();
}
