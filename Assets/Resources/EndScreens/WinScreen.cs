using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinScreen : MonoBehaviour
{
    public static WinScreen I;
    public GameObject WinPanel;
    public TMPro.TMP_Text WinText;
    public bool winTriggered;
    public float winTime = 0;
    float winHold = 5;

    private void Start()
    {

        if (I)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        I = this;
    }
    private void Update()
    {
        if (winTriggered) if (Time.time - winTime > winHold) { Application.Quit(); winTriggered = false; WinText.text = "Why must this cruel fate persist?";}

    }

    public void ShowWin(string Winner)
    {
        WinPanel.SetActive(true);
        WinText.text = Winner + " has won!";
        winTime = Time.time;
        winTriggered = true;
    }
}
