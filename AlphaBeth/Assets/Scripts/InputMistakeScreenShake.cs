using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputMistakeScreenShake : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    private float m_Amplitude;

    [SerializeField]
    private float m_Frequency;

    [SerializeField]
    private float m_Duration;

    [Header("Required References")]
    [SerializeField]
    private Player m_Player;

    [SerializeField]
    private CinemachineVirtualCamera m_VirtualCamera;
    private CinemachineBasicMultiChannelPerlin m_Noise;

    private Coroutine m_ShakeRoutine;

    private void Start()
    {
        if (m_Player != null)
            m_Player.InputMistakeEvent += OnInputMistake;

        if (m_VirtualCamera != null)
        {
            m_Noise = m_VirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            if (m_Noise != null)
            {
                m_Noise.m_AmplitudeGain = 0.0f;
                m_Noise.m_FrequencyGain = 0.0f;
            }
        }
    }

    private void OnDestroy()
    {
        if (m_Player != null)
            m_Player.InputMistakeEvent -= OnInputMistake;
    }

    private IEnumerator ShakeRoutine()
    {
        if (m_Noise == null)
            yield return null;

        m_Noise.m_AmplitudeGain = m_Amplitude;
        m_Noise.m_FrequencyGain = m_Frequency;

        yield return new WaitForSeconds(m_Duration);

        m_Noise.m_AmplitudeGain = 0.0f;
        m_Noise.m_FrequencyGain = 0.0f;

        m_ShakeRoutine = null;
        yield return null;
    }

    private void OnInputMistake()
    {
        if (m_ShakeRoutine != null)
            StopCoroutine(m_ShakeRoutine);

        m_ShakeRoutine = StartCoroutine(ShakeRoutine());
    }
}
