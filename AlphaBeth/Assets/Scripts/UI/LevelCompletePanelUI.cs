using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelCompletePanelUI : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup m_CanvasGroup;

    [SerializeField]
    private Button m_ResetButton;

    private void Start()
    {
        if (LevelDirector.Instance != null)
        {
            LevelDirector.Instance.LevelStartEvent += OnLevelStart;
            LevelDirector.Instance.LevelCompleteEvent += OnLevelComplete;
        }

        if (m_CanvasGroup != null)
            m_CanvasGroup.Hide();
    }

    private void OnDestroy()
    {
        if (LevelDirector.Instance != null)
        {
            LevelDirector.Instance.LevelStartEvent -= OnLevelStart;
            LevelDirector.Instance.LevelCompleteEvent -= OnLevelComplete;
        }
    }

    public void ResetLevel()
    {
        if (LevelDirector.Instance != null)
            LevelDirector.Instance.StartLevel();
    }

    private IEnumerator SelectButtonRoutine()
    {
        //Wtf event system?
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(m_ResetButton.gameObject);
    }

    //Callbacks
    private void OnLevelStart()
    {
        if (m_CanvasGroup != null)
            m_CanvasGroup.Hide();

        if (LevelDirector.Instance != null)
            LevelDirector.Instance.RemoveInputBlocker();
    }

    private void OnLevelComplete()
    {
        if (m_CanvasGroup != null)
            m_CanvasGroup.Show();

        if (m_ResetButton != null)
            StartCoroutine(SelectButtonRoutine());

        if (LevelDirector.Instance != null)
            LevelDirector.Instance.AddInputBlocker();
    }
}
