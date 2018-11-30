using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class ToggleOptionUI : MonoBehaviour
{
    private Toggle m_Toggle;

    [SerializeField]
    private string m_VariableName;

    [SerializeField]
    private bool m_DefaultValue;

    private void Start()
    {
        //Thanks to RequireComponent
        m_Toggle = GetComponent<Toggle>();

        SaveGameManager.DeletedEvent += OnDeleteSaveGame;

        //Init state
        bool isOn = SaveGameManager.GetBool(m_VariableName, m_DefaultValue);

        if (isOn != m_Toggle.isOn)
            m_Toggle.isOn = isOn;
    }

    private void OnDestroy()
    {
        SaveGameManager.DeletedEvent -= OnDeleteSaveGame;
    }

    public void TogglePressed(bool value)
    {
        SaveGameManager.SetBool(m_VariableName, value);
    }

    private void OnDeleteSaveGame()
    {
        m_Toggle.isOn = m_DefaultValue;
    }
}
