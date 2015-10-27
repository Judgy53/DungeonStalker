using UnityEngine;
using System.Collections;

public interface IBlockable
{
    float MinBlockValue { get; set; }
    float MaxBlockValue { get; set; }
}
