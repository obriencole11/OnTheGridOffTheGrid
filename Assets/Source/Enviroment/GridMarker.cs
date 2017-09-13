using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMarker : MarkerObject {

	// Use this for initialization
	void Start () {
        GameManager.Instance.onEnterGrid += disable;
        GameManager.Instance.onExitGrid += enable;

        if (GameManager.Instance.onGrid)
        {
            col.enabled = false;

        }
    }

    protected override bool clear()
    {
        return true;
    }
}
