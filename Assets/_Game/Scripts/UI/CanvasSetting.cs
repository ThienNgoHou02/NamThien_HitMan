using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CanvasSetting : UICanvas
{
    [Header("Button")]
    [SerializeField] private Button _menuButton;
    [SerializeField] private Button _continueButton;

    private void Awake()
    {
        _menuButton.onClick.AddListener(BackToMenu);
        _continueButton.onClick.AddListener(Contiune);
    }
    private void Contiune()
    {
        Time.timeScale = 1f;
        Close(0);
        UIManager.Instance.OpenUI<CanvasGameplay>();
    }
    private void BackToMenu()
    {
        Time.timeScale = 1f;
        Close(0);
        SceneManager.LoadScene("StartScene");
    }
}
