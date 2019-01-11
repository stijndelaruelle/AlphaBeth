using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class TimeCounterUI : MonoBehaviour
{
    private Text m_Text;
    private Stopwatch m_StopWatch;

    private void Awake()
    {
        m_StopWatch = new Stopwatch();
    }

    private void Start()
    {
        //Thanks to require component
        m_Text = GetComponent<Text>();

        if (LevelDirector.Instance != null)
        {
            LevelDirector.Instance.LevelStartEvent += OnLevelStart;
            LevelDirector.Instance.LevelCompleteEvent += OnLevelEnd;
        }
    }

    private void OnDestroy()
    {
        if (LevelDirector.Instance != null)
        {
            LevelDirector.Instance.LevelStartEvent -= OnLevelStart;
            LevelDirector.Instance.LevelCompleteEvent -= OnLevelEnd;
        }
    }

    //Event callbacks
    private void OnLevelStart()
    {
        //Reset conter
        m_StopWatch.Reset();
        m_StopWatch.Start();
    }

    private void OnLevelEnd()
    {
        m_StopWatch.Stop();
        m_Text.text = (int)m_StopWatch.Elapsed.TotalSeconds + "s " + m_StopWatch.Elapsed.Milliseconds + "ms";
    }
}

