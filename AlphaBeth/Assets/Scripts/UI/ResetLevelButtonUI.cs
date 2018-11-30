using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetLevelButtonUI : MonoBehaviour
{
    public void ResetLevel()
    {
        if (LevelDirector.Instance != null)
            LevelDirector.Instance.StartLevel();
    }
}
