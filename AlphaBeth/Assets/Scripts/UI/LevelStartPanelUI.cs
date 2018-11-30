using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelStartPanelUI : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup m_CanvasGroup;

    [SerializeField]
    private Button m_StartButton;

    private void Start()
    {
        if (LevelDirector.Instance != null)
        {
            LevelDirector.Instance.LevelReadyEvent += OnLevelReady;
            LevelDirector.Instance.LevelStartEvent += OnLevelStart;
        }

        if (m_StartButton != null)
        {
            m_StartButton.interactable = false; //Only allow pressing play after the level is ready!
            StartCoroutine(SelectButtonRoutine()); //Wtf event system?
        }

        if (m_CanvasGroup != null)
            m_CanvasGroup.Show();

        if (LevelDirector.Instance != null)
            LevelDirector.Instance.AddInputBlocker();
    }

    private void OnDestroy()
    {
        if (LevelDirector.Instance != null)
        {
            LevelDirector.Instance.LevelReadyEvent -= OnLevelReady;
            LevelDirector.Instance.LevelStartEvent -= OnLevelStart;
        }
    }

    public void StartLevel()
    {
        if (LevelDirector.Instance != null)
            LevelDirector.Instance.StartLevel();
    }

    private IEnumerator SelectButtonRoutine()
    {
        //Wtf event system?
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(m_StartButton.gameObject);
    }

    //Callbacks
    private void OnLevelReady()
    {
        if (m_StartButton != null)
            m_StartButton.interactable = true;
    }

    private void OnLevelStart()
    {
        if (m_CanvasGroup != null)
            m_CanvasGroup.Hide();

        if (LevelDirector.Instance != null)
            LevelDirector.Instance.RemoveInputBlocker();
    }
}
