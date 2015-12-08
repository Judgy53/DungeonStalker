using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class UICurrentSave : MonoBehaviour
{
    private string SaveId = null;

    [SerializeField]
    private Text playerInfos;

    [SerializeField]
    private Text timePlayed;

    [SerializeField]
    private Text creationDate;

    public void SetSave(string save)
    {
        SaveId = save;

        FillInfos();
    }

    private void FillInfos()
    {
        string fullSaveName = "save" + SaveId;

        if(SaveManager.Instance.Saves.ContainsKey(fullSaveName))
        {
            Save save = SaveManager.Instance.Saves["save" + SaveId];

            if (save != null)
            {
                playerInfos.text = save.PlayerName + ", Level " + save.PlayerLevel;
                timePlayed.text = TimeToString(save.TimePlayed);
                creationDate.text = save.CreationDate.ToString("dd/MM/yyyy, HH:mm");

                return;
            }
        }

        playerInfos.text = "";
        timePlayed.text = "";
        creationDate.text = "";
    }

    private string TimeToString(long time)
    {
        long hours = time / 3600L;
        long minute = (time % 3600L) / 60L;
        long seconds = time % 60;

        return hours.ToString("000") + "h" + minute.ToString("00") + "m" + seconds.ToString("00");
    }
}