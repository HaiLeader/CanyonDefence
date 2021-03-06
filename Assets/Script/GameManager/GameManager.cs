﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	public static GameManager instance;
	public ShakeCamera shakeCamera;
	public GAMESTATE gamestate;
	public GAMESTATE preGameState;

	public ResultControl resultControl;

    public Text goldText;
    public int gold;
    public int Gold {
        get { return gold; }
        set {
            gold = value;
            goldText.text = gold.ToString();
        }
    }

	public Text scoreText;
	public int score;
	public int Score{
		get { return score;}
		set{
			score = value;
			scoreText.text = score.ToString();
		}
	}

    public Text waveText;

	public Text livesText;

	public int lives;
	
	public GameObject startGameObject;
	public HelpGamePlayController helpPopup;

    public bool gameOver = false;

	private float timescale = 1.0f;
	private bool paused = false;
    private int wave;
    public int Wave {
        get { return wave; }
        set {
            wave = value;
            if (!gameOver) {
            }
			waveText.text = GetCurrentWaveString();
        }
    }


    void Awake(){
		wave = 0;
		instance = this;
		timescale = 1.0f;
	}

    void Start() {
        Gold = 1000;
		Score = 0;
		lives = 11;	
		if (SoundManager.instance != null) {
			SoundManager.instance.PlayBGM(BGM.GAMEPLAY);
		}
		waveText.text = GetCurrentWaveString();
		goldText.text = Gold.ToString ();
		livesText.text = lives.ToString ();
		scoreText.text = Score.ToString ();
		CheckGameState ();
    }

	public void DecreeHealth() {
		if (lives > 0) {
			--lives;
			livesText.text = lives + "";
			//Am thanh khi mat mang
			if(SoundManager.instance != null){
				SoundManager.instance.PlaySFX(SFX.PLAYER_DIE);
			}
			//Rung man hinh
			shakeCamera.DoShake();
			//Rung dien thoai
			if (Attributes.isVibrationOn ()) {
				Handheld.Vibrate();
			}
			if (lives == 0) {
				Debug.Log ("Game Over: Lose");
				SetGameState(GAMESTATE.LOSEGAME);
			}
		}

	}

	public void SetGameState(GAMESTATE state){
		preGameState = gamestate;
		gamestate = state;
		switch (gamestate) {
			case GAMESTATE.GAMEPLAYING:
				paused = false;
				Time.timeScale = timescale;
				break;
			case GAMESTATE.GAMEPAUSE:
				Time.timeScale = 0.0f;
				break;
			case GAMESTATE.GAMESTART:
				if(!startGameObject.activeSelf)
					startGameObject.SetActive(true);
				Time.timeScale = timescale;
				break;
			case GAMESTATE.LOSEGAME:
				timescale = 1.0f;
				Time.timeScale = timescale;
				//Show popup result
				resultControl.YouLose();
				break;
			case GAMESTATE.WINGAME:
				timescale = 1.0f;
				Time.timeScale = timescale;
				//: Save game vao score
				
				//Show popup result
				resultControl.YouWin();
				break;
			case GAMESTATE.MENU:
				timescale = 1.0f;
				Time.timeScale = timescale;
				break;
			case GAMESTATE.SETTING:
				Time.timeScale = 0.0f;
				break;
			case GAMESTATE.HELP:
				helpPopup.Open();
				break;
		}
	}

	void Update(){
		if (Input.GetKeyDown (KeyCode.T)) {
			SetTimeScale(3.0f);
		}
		if (paused && gamestate != GAMESTATE.HELP) {
			SetGameState(GAMESTATE.GAMEPAUSE);
		}
		Debug.Log (gamestate);
		if (Input.GetKeyDown (KeyCode.H)) {
			SetGameState(GAMESTATE.HELP);
		}

	}

	void OnApplicationPause(bool pauseStatus) {
		paused = pauseStatus;
	}


	public void SetTimeScale(float time){
		timescale = time;
		Time.timeScale = timescale;
	}

	private string GetCurrentWaveString(){
		string text = "";
		if (SpawnEnemy.instance != null) {
			if(wave <= SpawnEnemy.instance.getMaxWave())
				text += wave + 1 + " OF " + SpawnEnemy.instance.getMaxWave();
		}
		return text;
	}

	private void CheckGameState(){
		Debug.Log ("LOL" + Attributes.GetWatchHelp());
		if (Attributes.GetWatchHelp () != Attributes.IS_ON) {
			SetGameState (GAMESTATE.HELP);
		} else {
			SetGameState(GAMESTATE.GAMESTART);
		}
	}
}

public enum GAMESTATE{
	GAMEPLAYING,
	GAMEPAUSE,
	GAMESTART,
	LOSEGAME,
	WINGAME,
	MENU,
	SETTING,
	NEXTWAVE,
	HELP
}
