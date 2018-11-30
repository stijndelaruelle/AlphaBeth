using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWarVisuals : MonoBehaviour
{
    [SerializeField]
    private GameObject m_Visuals;

    private void Start()
    {
        SaveGameManager.BoolVariableChangedEvent += OnSaveGameBoolVariableChanged;
        SaveGameManager.DeletedEvent += OnDeleteSaveGame;

        if (m_Visuals != null)
            m_Visuals.SetActive(SaveGameManager.GetBool(SaveGameManager.SAVE_OPTION_FOGOFWAR, true));
    }

    private void OnDestroy()
    {
        SaveGameManager.BoolVariableChangedEvent -= OnSaveGameBoolVariableChanged;
        SaveGameManager.DeletedEvent -= OnDeleteSaveGame;
    }

    private void OnSaveGameBoolVariableChanged(string key, bool value)
    {
        if (key != SaveGameManager.SAVE_OPTION_FOGOFWAR)
            return;

        m_Visuals.SetActive(value);
    }

    private void OnDeleteSaveGame()
    {
        m_Visuals.SetActive(true);
    }
}
