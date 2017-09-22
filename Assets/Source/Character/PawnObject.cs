using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnObject : GridObject {


    public override void declareAction()
    {

        // Decide on a direction
        nextState.direction = state.direction;
        nextState.gridPosition = GridTools.directionPosition(state.gridPosition, nextState.direction);
        if (!GameManager.Instance.CouldObjectMoveHere(nextState.gridPosition, this) || !GameManager.Instance.SquareExists(nextState.gridPosition))
        {
            nextState.direction = GridTools.oppositeDirection(state.direction);
            nextState.gridPosition = GridTools.directionPosition(state.gridPosition, nextState.direction);
            
            if (!GameManager.Instance.CouldObjectMoveHere(nextState.gridPosition, this) || !GameManager.Instance.SquareExists(nextState.gridPosition))
            {
                nextState.gridPosition = state.gridPosition;
            }
        }

        //Update the orientation
        //transform.rotation = GridTools.DirectionToQuaternion(nextState.direction);

    }

    public override void animate(Vector2 nextPosition, float blend)
    {
        base.animate(nextPosition, blend);
        transform.rotation = Quaternion.Slerp(GridTools.DirectionToQuaternion(state.direction), GridTools.DirectionToQuaternion(nextState.direction), blend);
    }


}
