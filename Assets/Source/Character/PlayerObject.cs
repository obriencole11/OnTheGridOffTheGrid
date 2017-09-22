using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerObject : GridObject {

    Rigidbody rb;

    public new void Awake()
    {
        base.Awake();
        rb = gameObject.GetComponent<Rigidbody>();
    }

    public void Update()
    {

        float h = Input.GetAxis("Horizontal") * GameManager.Instance.settings.offGridSpeed;
        float v = Input.GetAxis("Vertical") * GameManager.Instance.settings.offGridSpeed;

        if (!GameManager.Instance.roundActive)
        {
            if (Input.GetKeyDown("right"))
            {
                Vector2 square = GridTools.directionPosition(state.gridPosition, Direction.RIGHT);
                if (GameManager.Instance.CouldPlayerMoveHere(square, Direction.RIGHT))
                {
                    turn(Direction.RIGHT);
                }
            }

            if (Input.GetKeyDown("left"))
            {
                Vector2 square = GridTools.directionPosition(state.gridPosition, Direction.LEFT);
                if (GameManager.Instance.CouldPlayerMoveHere(square, Direction.LEFT))
                {
                    turn(Direction.LEFT);
                }
            }

            if (Input.GetKeyDown("up"))
            {
                Vector2 square = GridTools.directionPosition(state.gridPosition, Direction.UP);
                if (GameManager.Instance.CouldPlayerMoveHere(square, Direction.UP))
                {
                    turn(Direction.UP);
                }
            }

            if (Input.GetKeyDown("down"))
            {
                Vector2 square = GridTools.directionPosition(state.gridPosition, Direction.DOWN);
                if (GameManager.Instance.CouldPlayerMoveHere(square, Direction.DOWN))
                {
                    turn(Direction.DOWN);
                }
            }

        }
    }

    public override void turn(Direction direction)
    {
        base.turn(direction);
        GameManager.Instance.advanceTurn();
    }

    public override void declareAction()
    {
        nextState.gridPosition = GridTools.directionPosition(state.gridPosition, nextState.direction);
    }

    protected override void register()
    {

        // Calculate the gridPosition and direction
        Vector2 gridPosition = GridTools.worldPositionToGrid(transform.position);
        var localDirection = transform.InverseTransformDirection(Vector3.forward);
        Direction currentDirection = GridTools.VectorToDirection(localDirection);

        state = new GridState(true, gridPosition, currentDirection);

        GameManager.Instance.registerPlayer(this);
    }

}
