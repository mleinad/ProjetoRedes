using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class NetWorkManagerUI : MonoBehaviour
{
    public Relay relay;
    private void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        Button buttonServe = root.Q<Button>("createBtn");
        Button buttonClient = root.Q<Button>("joinBtn");
        TextField textField = root.Q<TextField>("TextField");
        VisualElement visualElement = root.Q<VisualElement>("uiElement");


        buttonServe.clicked += () =>
        {
            relay.CreateRelay();
            visualElement.SetEnabled(false);
        };
        buttonClient.clicked += () => 
        {
            
            relay.JoinRelay(textField.text);
            visualElement.SetEnabled(false);

        };


    }




        
        
}