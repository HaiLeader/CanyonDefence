﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum MoveDirection {
	TOP_LEFT,
	TOP_RIGHT,
	BOTTOM_LEFT,
	BOTTOM_RIGHT,
	TOP,
	RIGHT,
	LEFT,
	BOTTOM
}

public enum EnemyState {
	START_RUN,
	DESTROY
}

public enum Type {
	ONGROUND,
	FLY
}

public class Enemy : MonoBehaviour {

	public bool isMoving ;

	Vector3 direction, destination;

	MoveDirection moveDirection;

	public GameObject[] waypoints;

	int currentWaypoint = 0;

	private float lastWaypointSwitchTime;

	public float speed = 1.0f;

	public int gold;

	private HealthBar healthBar;

	EnemyState enemyState;

	public Type type;

	[HideInInspector]
	public System.Action actionAfterDestroy;

	public List<EnemyObserver> observers;

	void Awake() {
		observers = new List<EnemyObserver>();
		switch (Attributes.difficulty) {
		case 1:
			speed *= 0.75f;
			break;
		case 2:
			speed *= 1.25f;
			break;
		case 3:
			speed *= 1.75f;
			break;
		}
	}
	// Use this for initialization
	void Start () {

		lastWaypointSwitchTime = Time.time;
		healthBar = transform.GetChild (2).GetComponent<HealthBar> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (isMoving && GameManager.instance.gamestate == GAMESTATE.GAMEPLAYING) {
			Vector3 startPosition = waypoints [currentWaypoint].transform.position;
			Vector3 endPosition = waypoints [currentWaypoint + 1].transform.position;

			float pathLength = Vector3.Distance (startPosition, endPosition);
			float totalTimeForPath = pathLength / speed;
			float currentTimeOnPath = Time.time - lastWaypointSwitchTime;
			gameObject.transform.position = Vector3.Lerp (startPosition, endPosition, currentTimeOnPath / totalTimeForPath);

			if (gameObject.transform.position.Equals(endPosition)) {
				if (currentWaypoint < waypoints.Length - 2) {
					// 3.a 
					currentWaypoint++;
					lastWaypointSwitchTime = Time.time;
					CalculateRotation(startPosition, endPosition);
				} else {
					GameManager.instance.DecreeHealth();
					SetEnemyState(EnemyState.DESTROY);
				}
			}
		}
	}

	public void SetEnemyState(EnemyState state) {

		enemyState = state;
		switch (state) {
		case EnemyState.DESTROY:
			Debug.Log ("Destroy game object");
			//Am thanh
			if(SoundManager.instance != null){
				SoundManager.instance.PlaySFX(SFX.ENEMY_DIE);
			}

			if (actionAfterDestroy != null)
				actionAfterDestroy();
			foreach(EnemyObserver observer in observers) {
				observer.Notify(this);
			}
			Destroy(gameObject);
			break;
		case EnemyState.START_RUN:
			Move();
			break;
		}
	}

	public void Move() {
		isMoving = true;
		currentWaypoint = 0;
		gameObject.transform.position = waypoints [currentWaypoint].transform.position;
	}

	void CalculateRotation(Vector3 preDirection, Vector3 destination) {
		Vector3 newStartPosition = waypoints [currentWaypoint].transform.position;
		Vector3 newEndPosition = waypoints [currentWaypoint + 1].transform.position;
		Vector3 newDirection = (newEndPosition - newStartPosition);
		//2
		float x = newDirection.x;
		float y = newDirection.y;
		float rotationAngle = Mathf.Atan2 (y, x) * 180 / Mathf.PI;
		//3
		GameObject sprite = (GameObject)
			gameObject.transform.FindChild("Sprite").gameObject;
		sprite.transform.rotation = 
			Quaternion.AngleAxis(rotationAngle, Vector3.forward);

	}

	public int GetScore(){
		int heso = Random.Range (1, 3);
		return heso * gold;
	}

	public int GetGold(){
		return gold;
	}
	
}
