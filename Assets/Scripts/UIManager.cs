using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public GameObject levelEndScreen;
    public GameObject HUD;
    public TextMeshProUGUI totalScores;
    public TextMeshProUGUI lvlScores;

    void Start()
    {
        GameManager.Instance.OnLeveleDone += LeveleDone;
    }

    public void LeveleDone()
    {
        levelEndScreen.SetActive(true);
        HUD.SetActive(false);
        lvlScores.text = "Level scores: " + GameManager.Instance.Scores;
        GameManager.Instance.SummingUpResults();
        totalScores.text = "Total scores: " + GameManager.Instance.TotalScores;
        GameManager.Instance.OnLeveleDone -= LeveleDone;
    }
}
