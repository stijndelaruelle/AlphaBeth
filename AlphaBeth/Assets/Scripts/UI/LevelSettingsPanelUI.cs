using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelSettingsPanelUI : MonoBehaviour
{
    [Header("Input Fields")]
    [SerializeField]
    private InputField m_WidthInputField;

    [SerializeField]
    private InputField m_HeightInputField;

    [SerializeField]
    private InputField m_TextCharactersInputField;

    [SerializeField]
    private InputField m_SeedInputField;

    [SerializeField]
    private Button m_GenerateButton;

    [Header("Other references")]
    [SerializeField]
    private LevelGenerator m_LevelGenerator;

    [SerializeField]
    private CanvasGroup m_CanvasGroup;

    private void Start()
    {
        if (m_CanvasGroup != null)
            m_CanvasGroup.Hide();

        SaveGameManager.DeletedEvent += OnDeleteSaveGame;
    }

    private void OnDestroy()
    {
        if (m_LevelGenerator != null)
            m_LevelGenerator.LevelGeneratedEvent -= OnLevelGeneratedTroughUs;

        SaveGameManager.DeletedEvent -= OnDeleteSaveGame;
    }


    public void Show()
    {
        if (m_CanvasGroup != null)
            m_CanvasGroup.Show();

        if (LevelDirector.Instance != null)
            LevelDirector.Instance.AddInputBlocker();

        if (m_GenerateButton != null)
            StartCoroutine(SelectButtonRoutine());

        //Load all the save game data
        m_WidthInputField.text = SaveGameManager.GetInt(SaveGameManager.SAVE_LEVEL_WIDTH, 5).ToString();
        m_HeightInputField.text = SaveGameManager.GetInt(SaveGameManager.SAVE_LEVEL_HEIGHT, 5).ToString();
        m_TextCharactersInputField.text = SaveGameManager.GetString(SaveGameManager.SAVE_LEVEL_TEXTCHARACTERS, "sdfghjkl");
        m_SeedInputField.text = SaveGameManager.GetInt(SaveGameManager.SAVE_LEVEL_SEED, -1).ToString();
    }

    public void Hide()
    {
        if (m_CanvasGroup != null)
            m_CanvasGroup.Hide();

        if (LevelDirector.Instance != null)
            LevelDirector.Instance.RemoveInputBlocker();
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

        bool seedSuccess = true;
        if (int.TryParse(m_SeedInputField.text, out seed) == false)
        {
            Debug.Log("Seed did not contain a valid number, will be random!");
            seedSuccess = false;
        }

        //Save the settings
        SaveGameManager.SetInt(SaveGameManager.SAVE_LEVEL_WIDTH, width);
        SaveGameManager.SetInt(SaveGameManager.SAVE_LEVEL_HEIGHT, height);
        SaveGameManager.SetString(SaveGameManager.SAVE_LEVEL_TEXTCHARACTERS, m_TextCharactersInputField.text);

        if (seedSuccess)
            SaveGameManager.SetInt(SaveGameManager.SAVE_LEVEL_SEED, seed);

        m_LevelGenerator.LevelGeneratedEvent += OnLevelGeneratedTroughUs;
        m_LevelGenerator.GenerateGridLevel();
    }

    private IEnumerator SelectButtonRoutine()
    {
        //Wtf event system?
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(m_GenerateButton.gameObject);
    }

    private void OnLevelGeneratedTroughUs()
    {
        m_LevelGenerator.LevelGeneratedEvent -= OnLevelGeneratedTroughUs;

        //We've one our job!
        if (LevelDirector.Instance != null)
            LevelDirector.Instance.StartLevel();

        Hide();
    }

    private void OnDeleteSaveGame()
    {
        //Reset data
        m_WidthInputField.text = SaveGameManager.GetInt(SaveGameManager.SAVE_LEVEL_WIDTH, 5).ToString();
        m_HeightInputField.text = SaveGameManager.GetInt(SaveGameManager.SAVE_LEVEL_HEIGHT, 5).ToString();
        m_TextCharactersInputField.text = SaveGameManager.GetString(SaveGameManager.SAVE_LEVEL_TEXTCHARACTERS, "sdfghjkl");
        m_SeedInputField.text = SaveGameManager.GetInt(SaveGameManager.SAVE_LEVEL_SEED, -1).ToString();
    }
}
