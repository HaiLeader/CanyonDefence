﻿using UnityEngine;
using System.Collections;

public class BulletController : MonoBehaviour {

    public float speed = 10;
    public int damage;
    public GameObject target;
    public Vector3 startPosition;
    public Vector3 targetPosition;

    private float distance;
    private float startTime;


    void Start() {
        startTime = Time.time;
        distance = Vector3.Distance(startPosition, targetPosition);
    }

    void Update() {
		if (GameManager.instance.gamestate == GAMESTATE.GAMEPLAYING) {
			float timeInterval = Time.time - startTime;
			gameObject.transform.position = Vector3.Lerp(startPosition, targetPosition, timeInterval * speed / distance);
			
			if (gameObject.transform.position.Equals(targetPosition)) {
				if (target != null) {
					Transform healthBarTransform = target.transform.FindChild("HealthBar");
					HealthBar healthBar = healthBarTransform.gameObject.GetComponent<HealthBar>();
					healthBar.Damage(Mathf.Max(damage, 0));
					
					/*
                if (healthBar.currentHealth <= 0) {
                    Destroy(target);
                    //TODO: Them hieu ung chet
                    //TODO: Them audio source

                    //TODO: Update gold
                    GameManager.instance.Gold += 50;
                }
                */
				}
				Destroy(gameObject);
			}
		}
    }
}
