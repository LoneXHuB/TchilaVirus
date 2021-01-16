using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public GameObject HUD;
    public GameObject startMenu;
    public InputField userNameField;
    public Dropdown teamDropdown;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists ! ");
            Destroy(this);
        }
    }
    public void ConnectToServer()
    {
        startMenu.SetActive(false);
        HUD.SetActive(true);
        userNameField.interactable = false;
        teamDropdown.interactable = false;
        Client.instance.ConnectToServer();
    }
}
