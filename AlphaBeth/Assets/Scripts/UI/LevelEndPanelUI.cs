using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelEndPanelUI : MonoBehaviour
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
            LevelDirector.Instance.LevelEndEvent += OnLevelEnd;
        }

        if (m_CanvasGroup != null)
            m_CanvasGroup.Hide();
    }

    private void OnDestroy()
    {
        if (LevelDirector.Instance != null)
        {
            LevelDirector.Instance.LevelStartEvent -= OnLevelStart;
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
    }

    private void OnLevelEnd()
    {
        if (m_CanvasGroup != null)
            m_CanvasGroup.Show();

        if (m_ResetButton != null)
            StartCoroutine(SelectButtonRoutine());
    }
}
