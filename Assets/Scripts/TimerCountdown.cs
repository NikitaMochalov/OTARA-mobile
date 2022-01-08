using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerCountdown : MonoBehaviour
{
    public TextMeshProUGUI textDisplay;
    public int alertTime = 30;
    public Color alertTextColor = Color.red;
    public int secondsLeft = 90;

    bool takingAway = false;

    private void Start()
    {
        if (secondsLeft % 60 < 10)
        {
            textDisplay.text = "0" + secondsLeft / 60 + ":0" + secondsLeft % 60;
        }
        else
        {
            textDisplay.text = "0" + secondsLeft / 60 + ":" + secondsLeft % 60;
        }
    }

    private void Update()
    {
        if (takingAway == false && secondsLeft > 0)
        {
            StartCoroutine(TimerTake());
        }

        if (secondsLeft <= alertTime)
        {
            textDisplay.rectTransform.localScale = Vector3.one * 2;
            textDisplay.color = alertTextColor;
        }

        if (secondsLeft == 0)
        {
            GameManager.Instance.OnLeveleDone.Invoke();
        }
    }

    IEnumerator TimerTake()
    {
        takingAway = true;
        yield return new WaitForSeconds(1);
        secondsLeft -= 1;
        if (secondsLeft % 60 < 10)
        {
            textDisplay.text = "0" + secondsLeft / 60 + ":0" + secondsLeft % 60;
        }
        else
        {
            textDisplay.text = "0" + secondsLeft / 60 + ":" + secondsLeft % 60;
        }
        takingAway = false;
    }
}