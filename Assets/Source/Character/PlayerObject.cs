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

        GameManager.Instance.onExitGrid += exitGrid;

        // Register this grid object with the game manager
        register();

    }

    public void Update()
    {

        float h = Input.GetAxis("Horizontal") * GameManager.Instance.settings.offGridSpeed;
        float v = Input.GetAxis("Vertical") * GameManager.Instance.settings.offGridSpeed;

        if (GameManager.Instance.gridActive) {
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
        } else {
                
                // Otherwise if the grid is not active, accept smooth movemtent
                // force = acceleration
                // acceleration = desiredVelocity - currentVelocity
                rb.AddForce(new Vector3 (h - rb.velocity.x, 0, v - rb.velocity.z), ForceMode.Impulse);
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

    public override void enterGrid(int id) {
        rb.velocity *= 0.0f;
        register();
        // Apply the state
        applyState(state);
    }

    public void exitGrid(int id) {
        gameObject.GetComponent<Collider>().enabled = true;
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
