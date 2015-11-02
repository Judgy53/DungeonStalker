using UnityEngine;
using System.Collections;
using System;

public interface IStatsDependable
{
    StatsManager StatsManager { get; }

    void OnStatsChange(object sender, EventArgs args);
}
