using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffGridMarker : MarkerObject {

    void Start()
    {
        GameManager.Instance.onEnterGrid += enable;
        GameManager.Instance.onExitGrid += disable;

        if (!GameManager.Instance.onGrid)
        {
            col.enabled = false;
        }
    }

    protected override bool clear()
    {
        return !GameManager.Instance.gridManager.exists(GridTools.worldPositionToGrid(transform.position));
    }
}
