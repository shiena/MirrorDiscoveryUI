using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConnectButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI buttonLabel;

    public void Initialize(string label, Action onClick)
    {
        buttonLabel.text = label;
        button.onClick.AddListener(() => onClick?.Invoke());
    }
}