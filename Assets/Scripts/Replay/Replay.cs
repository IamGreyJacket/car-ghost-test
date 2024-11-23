using System;
using System.Collections.Generic;

[Serializable]
public class Replay
{
    //List contains Record, which contains inputs for every FixedUpdate call
    public List<InputRecord> ReplayInputs = new List<InputRecord>();
    //List contains more information for unique moments, such as collisions, that may RUIN the replay.
    public List<UniqueRecord> ReplayUniqueRecords = new List<UniqueRecord>();
    //List contains WHEN unique moment happened and WHEN it needs to be played.
    public List<int> UniqueRecordKeys = new List<int>();
}
