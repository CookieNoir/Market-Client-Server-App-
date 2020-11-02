using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentPlayer : PlayerInfo
{
    public static CurrentPlayer instance;
    public GameObject currentPlayerUiHandler;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            currentPlayerUiHandler.SetActive(false);
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying component");
            Destroy(this);
        }
    }
}
