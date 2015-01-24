﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum CurrentLevel {
	BOSS_BATTLE,
	GRAVITY,
	BOMB_DISARM
}


public class GameManager : MonoBehaviour {
	private static GameManager instance = null;
	public static GameManager Instance {
		get { return instance; }
	}

	private int level = 0;

	private CurrentLevel[] levelList = new CurrentLevel [3] {
		CurrentLevel.BOSS_BATTLE,
		CurrentLevel.GRAVITY,
		CurrentLevel.BOMB_DISARM
	};


	void Awake (){
		if (instance != null) {
			GameObject.Destroy (this.gameObject);
		} else {
			instance = this;
			GameObject.DontDestroyOnLoad (this.gameObject);
		}
	}


	public void GoToNextLevel (){
		if (instance.level < this.levelList.Length) {
			instance.level++;
			Application.LoadLevel (instance.levelList [instance.level].ToString ());
		}
	}


	public void GoToLastLevel (){
		if (instance.level > 0) {
			instance.level--;
			Application.LoadLevel (instance.levelList [instance.level].ToString ());
		}
	}
}
