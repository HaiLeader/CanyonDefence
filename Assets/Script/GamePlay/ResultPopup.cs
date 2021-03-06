﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public enum TypeResultPopup{
	WIN,
	LOSE
}

public class ResultPopup : MonoBehaviour {

	public Text defficulty;
	public Text waves;
	public Text score;
	public TypeResultPopup typeResult;


	public void Open(){
		LoadInfo ();
		gameObject.SetActive (true);
	}

	public void Close(){
		gameObject.SetActive (false);
	}

	public void Retry(){
		//TODO Load application
		LoadScene.Load (Application.loadedLevelName);
	}

	public void GoHome(){
		//TODO Set gamestate = menu
		//TODO Goto home menu
		if (GameManager.instance != null) {
			GameManager.instance.SetGameState(GAMESTATE.MENU);
		}
		LoadScene.Load (Strings.SCEN_MENU);
	}

	private void LoadInfo(){
		defficulty.text = LoadDefficulty ();
		waves.text = LoadWavesInfo ();
		score.text = LoadScoreInfo ();

		switch (typeResult) {
			case TypeResultPopup.LOSE:
				//TODO
				break;
			case TypeResultPopup.WIN:
				//TODO: Kiem tra highscore
				if(CheckHighScore()){
					//TODO: Kich hoat popup highscore
					GameObject textHighscore = 
						gameObject.transform.FindChild("Bg content").gameObject.transform.FindChild("HighScoreImg").gameObject;
					if(textHighscore != null){
						textHighscore.SetActive(true);
					}
				}else{
					gameObject.transform.FindChild("Bg content").gameObject.transform.FindChild("HighScoreImg").gameObject.SetActive(false);
				}
				break;
		}
	}

	private string LoadDefficulty(){
		string text = "";
		switch (Attributes.difficulty) {
			case 1:
				text = "Difficulty :  Easy";
				break;
			case 2: 
				text = "Difficulty :  Normal";
				break;
			case 3:
				text = "Difficulty :  Hard";
				break;
			default:
				text = "Difficulty :  Easy";
				break;
		}
		return text;
	}

	private string LoadWavesInfo(){
		string text = "Waves: ";
		if (SpawnEnemy.instance != null) {
			int maxWave = SpawnEnemy.instance.getMaxWave ();
			int currentWave = SpawnEnemy.instance.getCurrentWave ();
			text += currentWave + 1 + "/" + maxWave;
		}
		return text;
	}

	private string LoadScoreInfo(){
		string text = "Scores: ";
		if (GameManager.instance != null) {
			text += GetScore();
		}
		return text;
	}

	private bool CheckHighScore(){
		int score = 0;
		bool ok = true;
		score = GetScore ();
		List<int> listScore = Attributes.getListScore ();
		foreach (int s in listScore) {
			if(score < s){
				ok = false;
			}
		}
		//Save score
		Attributes.addScoreToList (score);
		//Check highscore
		return ok;
	}

	private int GetScore(){
		float k = 0;
		switch (Attributes.difficulty) {
			case 1:
				k = 0.0f;
				break;
			case 2:
				k = 0.4f;
				break;
			case 3: 
				k = 0.6f;
				break;
			default:
				k = 0;
				break;
		}
		int score = GameManager.instance.Score + (int)(GameManager.instance.Score * k);
		return score;
	}
}
