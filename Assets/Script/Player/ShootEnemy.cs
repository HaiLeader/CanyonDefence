﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShootEnemy : MonoBehaviour, EnemyObserver {

    public List<GameObject> enemiesInRange;

    private float lastShotTime;
    private PlayerData playerData;
    public GameObject target;

	public float range;

	private Vector2 preDirection ;

	public float defaultRotation = 90;

	private GameObject rangeImageObject;


	public bool isShowingRange = false;

	void Awake() {
		rangeImageObject = transform.FindChild ("RangeImage").gameObject;
		HideRange ();
	}
	

    void Start() {
        lastShotTime = Time.time;
        playerData = gameObject.GetComponentInChildren<PlayerData>();
		preDirection = new Vector2 (0, 1);
    }

    void Update() {
		if (GameManager.instance.gamestate == GAMESTATE.GAMEPLAYING) {
			float minimalEnemyDistance = float.MaxValue;
			//foreach (GameObject enemy in enemiesInRange) { }
			if (enemiesInRange.Count > 0) {
				target = enemiesInRange[0];
				if (Time.time - lastShotTime > playerData.CurrentLevel.fireRate) {
					Shoot(target.GetComponent<Collider2D>());
					lastShotTime = Time.time;
				}
				Vector2 desDirection = gameObject.transform.position - target.transform.position;
				CalculateRotation();
			}
		}
 
    }



	public void ShowRange() {
		isShowingRange = true;
		rangeImageObject.SetActive (true);
	}

	public void HideRange() {
		isShowingRange = false;
		rangeImageObject.SetActive (false);
	}

    void Shoot(Collider2D target) {
        GameObject bulletPrefab = playerData.CurrentLevel.bullet;

        Vector3 startPosition = gameObject.transform.position;
        Vector3 targetPosition = target.transform.position;
        startPosition.z = bulletPrefab.transform.position.z;
        targetPosition.z = bulletPrefab.transform.position.z;

        GameObject newBullet = (GameObject)Instantiate(bulletPrefab);
        newBullet.transform.position = startPosition;
        BulletController bulletControl = newBullet.GetComponent<BulletController>();
        bulletControl.target = target.gameObject;
        bulletControl.startPosition = startPosition;
        bulletControl.targetPosition = targetPosition;

//		if (Attributes.isSoundSFXOn ()) {
			AudioSource audio = gameObject.GetComponent<AudioSource>();
			audio.volume = Attributes.getSFXVolume();
			audio.Play();
//		}
    }

	public  float Distance(Vector2 v) {
		return Vector2.Distance (transform.position, v);
	}

	public void AddEnemyInRange(GameObject enemy) {
		if (!enemiesInRange.Contains (enemy) && enemy != null) {
			enemiesInRange.Add(enemy);
		}
	}

	public void DeleteEnemyInRange(GameObject enemy) {
		if (enemiesInRange.Contains (enemy)) {
			enemiesInRange.Remove(enemy);
		}
	}

	public void Notify(Enemy enemy) {
		DeleteEnemyInRange (enemy.gameObject);
	}

	void CalculateRotation() {
		Vector3 direction = target.transform.position-gameObject.transform.position;
		gameObject.transform.rotation = Quaternion.AngleAxis(
			Mathf.Atan2 (direction.y, direction.x) * 180 / Mathf.PI - 90,
			new Vector3 (0, 0, 1));

	}

  
}
