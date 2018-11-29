using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class MistakeCounterUI : MonoBehaviour
{
    [SerializeField]
    private Player m_Player;

    private Text m_Text;
    private int m_MistakeCount;

    private void Start()
    {
        //Thanks to require component
        m_Text = GetComponent<Text>();
        UpdateText();

        if (m_Player != null)
            m_Player.InputMistakeEvent += OnInputMistake;
    }

    private void OnDestroy()
    {
        if (m_Player != null)
            m_Player.InputMistakeEvent -= OnInputMistake;
    }

    private void UpdateText()
    {
        string mistakeText = m_MistakeCount + " mistake";
        if (m_MistakeCount != 1) { mistakeText += "s"; }

        m_Text.text = mistakeText;
    }

    //Event callback
    private void OnInputMistake()
    {
        m_MistakeCount += 1;
        UpdateText();
    }
}
