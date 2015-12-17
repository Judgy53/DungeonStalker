using UnityEngine;
using System.Collections;

public class AnimationEventsReceiver : MonoBehaviour
{
    [SerializeField]
    private WeaponManager manager;

    public void PlayPrimarySound()
    {
        manager.PlayPrimarySound();
    }

    public void PlayPrimarySoundOH()
    {
        manager.PlayPrimarySoundOffHand();
    }
}