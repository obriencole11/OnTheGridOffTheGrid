using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class level : MonoBehaviour {

	private int levelID;

	public bool finished = false;

	// Use this for initialization
	void Awake () {
		levelID = GameManager.Instance.getLevelID(gameObject);

		ILevelObject[] levelObjects = GetComponentsInChildren<ILevelObject>();

		foreach (ILevelObject levelObject in levelObjects) {
			levelObject.addToGrid(levelID);
		} 
	}

}
