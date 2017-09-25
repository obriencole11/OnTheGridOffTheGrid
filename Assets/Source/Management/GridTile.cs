using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTile : MonoBehaviour, ILevelObject {

    private int gridID;

    Vector2 gridPosition
    {
        get
        {
            return GridTools.worldPositionToGrid(transform.position);
        }
    }

	// Use this for initialization
	void Awake () {
        //GameManager.Instance.AddTile(gridPosition);
        GameManager.Instance.onNewGrid += enterGrid;
	}

    public void addToGrid(int id) {
        gridID = id;
    }

    void enterGrid(int id) {
        if (gridID == null || id == gridID) {
            GameManager.Instance.AddTile(gridPosition);
        }
    }

}
