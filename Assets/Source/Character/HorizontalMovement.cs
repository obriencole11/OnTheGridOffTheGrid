using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectColor
{
    WHITE,
    BLACK
}

public class HorizontalMovement : GridMovement {

    public bool facingRight = true;

    public ObjectColor color = ObjectColor.WHITE;

    void Start()
    {
        // Grab a reference to the GridObject
        gridObject = gameObject.GetComponent<GridObject>();

        gridObject.onNewPosition += checkFacing;

        gridObject.turnNumber = (int)color + 1;

        checkFacing();
    }

    void checkFacing()
    {
        
        if (facingRight)
        {
            if (!GameManager.Instance.gridManager.exists(GridTools.directionPosition(gridObject, Direction.RIGHT)))
            {
                facingRight = false;
            }
        }
        else {
            if (!GameManager.Instance.gridManager.exists(GridTools.directionPosition(gridObject, Direction.LEFT)))
            {
                facingRight = true;
            }
        }


        if (facingRight)
        {
            gridObject.setNextDirection(Direction.RIGHT);
        } else {
            gridObject.setNextDirection(Direction.LEFT);
        }
    }

}
