using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CanvasLevelSelect : UICanvas
{
    [Header("Component")]
    [SerializeField] private Transform _buttonsParent;

    private void OnEnable()
    {
        SetLevelButton();
    }
    private void SetLevelButton()
    {
        for (int i = 0; i < _buttonsParent.childCount; i++)
        {
            int index = i + 1;
            Button button = _buttonsParent.GetChild(i).GetComponent<Button>();
            button.onClick.AddListener(() => Level(index));
        }
    }
    private void Level(int level)
    {
        SceneManager.LoadScene($"{level}");
    }
}
