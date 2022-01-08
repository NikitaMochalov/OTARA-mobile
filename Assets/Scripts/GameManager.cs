using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
	public int TotalScores;

	public int Scores { get; private set; } = 0;
	public int TotalSheepsCount { get; private set; } = 0;
	public int CaughtSheepsCount = 0;

	//events
	public System.Action OnLeveleDone;

    public void AddScores(int addedScores)
	{
		Scores += addedScores;
	}

	public void SetSheepsCount(int count)
    {
		TotalSheepsCount = count;
	}

	public void SummingUpResults()
	{
		TotalScores += Scores;
		Scores = 0;
		TotalSheepsCount = 0;
		CaughtSheepsCount = 0;
    }
}