using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteSaveButtonUI : MonoBehaviour
{
    public void DeleteSave()
    {
        SaveGameManager.DeleteSavGame();
    }
}
