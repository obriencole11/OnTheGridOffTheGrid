using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (!GameManager.Instance.gridActive) {
			Vector3 lerpPos = Vector3.Lerp(transform.position, GameManager.Instance.player.transform.position, Time.deltaTime * 3.0f);
			transform.position = new Vector3(lerpPos.x, transform.position.y, lerpPos.z);
		} else {
			GameObject target = GameManager.Instance.levels[GameManager.Instance.currentLevel];
			Vector3 lerpPos = Vector3.Lerp(transform.position, target.transform.position, Time.deltaTime * 3.0f);
			transform.position = new Vector3(lerpPos.x, transform.position.y, lerpPos.z);
		}
	}
}
