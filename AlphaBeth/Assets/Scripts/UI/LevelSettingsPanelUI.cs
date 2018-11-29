using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSettingsPanelUI : MonoBehaviour
{
    [SerializeField]
    private LevelGenerator m_LevelGenerator;

    [SerializeField]
    private InputField m_WidthInputField;

    [SerializeField]
    private InputField m_HeightInputField;

    [SerializeField]
    private InputField m_AvailableCharactersInputField;

    [SerializeField]
    private InputField m_SeedInputField;

    private void Start()
    {
        //Hide ourselves
        Hide();
    }

    public void GenerateLevel()
    {
        if (m_LevelGenerator == null)
            return;

        //Parse all the int values
        int width = 0;
        int height = 0;
        int seed = 0;

        if (int.TryParse(m_WidthInputField.text, out width) == false)
        {
            Debug.Log("Width did not contain a valid number!");
            return;
        }

        if (int.TryParse(m_HeightInputField.text, out height) == false)
        {
            Debug.Log("Height did not contain a valid number!");
            return;
        }

        if (int.TryParse(m_SeedInputField.text, out seed) == false)
        {
            Debug.Log("Seed did not contain a valid number!");
            return;
        }

        m_LevelGenerator.GenerateGridLevel(width, height, m_AvailableCharactersInputField.text, seed);

        //We've one our job!
        Hide();
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
