using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeLevelScript : MonoBehaviour
{
    private void Start()
    {
        //Alter the settings to be super indie
        SaveGameManager.SetBool(SaveGameManager.SAVE_OPTION_NEWCHARSONMISTAKE, false);
        SaveGameManager.SetBool(SaveGameManager.SAVE_OPTION_FOGOFWAR, true);
        SaveGameManager.SetBool(SaveGameManager.SAVE_OPTION_NODESDISAPPEAR, false);
        SaveGameManager.SetBool(SaveGameManager.SAVE_OPTION_SCREENSHAKE, true);
    }
}
