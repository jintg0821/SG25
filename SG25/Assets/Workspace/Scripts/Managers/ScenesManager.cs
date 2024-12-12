using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScenesManager : Singleton<ScenesManager>
{
    public Button LoadSceneButton;
    public Button QuitButton;

    void Start()
    {
        Screen.SetResolution(1920, 1080, true);

        SetupButtons();
    }

    void SetupButtons()
    {
        LoadSceneButton = GameObject.Find("LoadSceneButton").gameObject.GetComponent<Button>();
        QuitButton = GameObject.Find("QuitButton").gameObject.GetComponent<Button>();
        QuitButton.onClick.AddListener(Application.Quit);

        string currentSceneName = SceneManager.GetActiveScene().name;

        if (currentSceneName == "StartScene")
        {
            LoadSceneButton.onClick.AddListener(() => SceneManager.LoadScene("GameScene"));
        }
        else if (currentSceneName == "GameScene")
        {
            UIManager UIManager = FindObjectOfType<UIManager>();
            UIManager.menuPanel.SetActive(false);
            LoadSceneButton.onClick.AddListener(() => SceneManager.LoadScene("StartScene"));
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetupButtons();
    }
}
