using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTile : MonoBehaviour {

    Vector2 gridPosition
    {
        get
        {
            return GridTools.worldPositionToGrid(transform.position);
        }
    }

	// Use this for initialization
	void Awake () {
        GameManager.Instance.AddTile(gridPosition);
	}

}
