using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearRankData : ScriptableObject
{
    [System.Serializable]
    public class RankData
    {
        public int BestTime;
        public int BetterTime;
    }

    public List<RankData> Elements;
}
