using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopupText : GameEntity
{
    [SerializeField] private float _popupSpeed;
    [SerializeField] private float _appearDuration;

    private TextMeshPro _textMeshPro;
    private float _appearTimer;

    private void Awake()
    {
        _textMeshPro = GetComponent<TextMeshPro>();
    }
    private void OnEnable()
    {
        _appearTimer = 0;
    }
    private void Update()
    {
        transform.position += Vector3.up * _popupSpeed * Time.deltaTime;
        _appearTimer += Time.deltaTime;
        if (_appearTimer >= _appearDuration)
        {
            MasterPool.Push(this);
        }
    }
    public void SetText(string text, Color color)
    {
        _textMeshPro.text = text;
        _textMeshPro.color = color;
    }
}
