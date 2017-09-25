using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class levelMarker : MonoBehaviour, ILevelObject {

	Renderer childRenderer;

	private int gridID;

	public void Awake() {
		childRenderer = GetComponentInChildren<Renderer>();

		GameManager.Instance.onExitGrid += exitGrid;
	}
	
	void Update () {

		Collider[] hitColliders = Physics.OverlapSphere(transform.position, 1.0f);
		bool hitPlayer = false;

		foreach (Collider col in hitColliders) {
			if (col.gameObject.tag == "Player"){

				hitPlayer = true;

				if (Input.GetKeyDown("space")){
					GameManager.Instance.enterGrid(gridID, transform.position);
					childRenderer.enabled = false;
				}
			}
		}

		if (hitPlayer) {				
			Color color = childRenderer.materials[0].color;
			childRenderer.materials[0].color = new Color (color.r, color.g, color.b, 0.75f);
		} else {
			Color color = childRenderer.materials[0].color;
			childRenderer.materials[0].color = new Color (color.r, color.g, color.b, 0.25f);
		}
	}

	public void addToGrid(int id) {
		gridID = id;
	}

	public void exitGrid(int id) {
		if (id == gridID && !GameManager.Instance.currentLevelObj.finished) {
			childRenderer.enabled = true;
		}
	}
}
