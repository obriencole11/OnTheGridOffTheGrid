using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalMovement : GridMovement {

    public bool facingUp = false;

    public ObjectColor color = ObjectColor.WHITE;

    void Start()
    {
        // Grab a reference to the GridObject
        gridObject = gameObject.GetComponent<GridObject>();

        gridObject.onNewPosition += checkFacing;
        

        gridObject.turnNumber = (int)color + 1;

    }

    void checkFacing()
    {

        if (facingUp)
        {
            if (!GameManager.Instance.gridManager.exists(GridTools.directionPosition(gridObject, Direction.UP)))
            {
                facingUp = false;
            }
        }
        else {
            if (!GameManager.Instance.gridManager.exists(GridTools.directionPosition(gridObject, Direction.DOWN)))
            {
                facingUp = true;
            }
        }


        if (facingUp)
        {
            gridObject.setNextDirection(Direction.UP);
        }
        else {
            gridObject.setNextDirection(Direction.DOWN);
        }
    }
}
