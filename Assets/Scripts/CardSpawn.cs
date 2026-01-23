using System;
using UnityEngine;
using UnityEngine.UI;

public class CardSpawn : MonoBehaviour
{
    public bool isClick;
    public float countdown;
    public Button btn;
    public Image fill;
    public Action onClick;
    public void Start()
    {
        isClick = false;
        btn.onClick.AddListener(OnClick);
    }
    public void OnClick()
    {
        onClick?.Invoke();
    }
    public void AddListener(Action action)
    {
        onClick = action;
    }
}