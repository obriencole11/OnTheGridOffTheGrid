using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject : MonoBehaviour {

    // The current state of the GridObject, so whether it exists, its gridPosition, and its direction
    private GridState _state;
    public GridState state
    {
        get
        {
            return _state;
        }
        protected set
        {
            _state = value;
        }
    }

    private GridState _nextState;
    public GridState nextState
    {
        get
        {
            return _nextState;
        }
        protected set
        {
            _nextState = value;
        }
    }

    public ColorType color = ColorType.NONE;
    
    public int powerLevel { get; protected set; }

    public void Awake()
    {

        // Register this grid object with the game manager
        register();

        // Apply the state
        applyState(state);

        // Set the color
        setColor(color);
    }

    public void Start()
    {
        powerLevel = 1;
    }


    #region Public Methods

    public void move(Vector2 gridPosition)
    {
        nextState.gridPosition = gridPosition;
    }

    public virtual void turn(Direction direction)
    {
        nextState.direction = direction;
    }

    public virtual void animate(Vector2 nextPosition, float blend)
    {
        Vector2 blendedPosition = Vector2.Lerp(state.gridPosition, nextPosition, blend);
        transform.position = GridTools.gridPositionToWorld(blendedPosition);
    }

    public virtual void declareAction()
    {
        // The default declaration is to not move at all
        nextState = state;
    }

    public void setColor(ColorType newColor)
    {
        color = newColor;

        if (color == ColorType.WHITE)
        {
            gameObject.GetComponentInChildren<Renderer>().material = GameManager.Instance.settings.whiteMaterial;
        }
        else if (color == ColorType.BLACK)
        {
            gameObject.GetComponentInChildren<Renderer>().material = GameManager.Instance.settings.blackMaterial;
        }
    }

    public void applyState(GridState newState)
    {
        // This actually update the position, rotation and visibilty based on the current state

        state = newState;
        nextState = new GridState(state.exists, state.gridPosition, state.direction);

        // Update the visibility
        Renderer mesh = GetComponentInChildren<Renderer>();
        mesh.enabled = state.exists;

        // Update the position
        transform.position = GridTools.gridPositionToWorld(state.gridPosition);

        //Update the orientation
        transform.rotation = GridTools.DirectionToQuaternion(state.direction);
        

    }

    #endregion


    #region Private Methods

    protected virtual void register()
    {
        // Calculate the gridPosition and direction
        Vector2 gridPosition = GridTools.worldPositionToGrid(transform.position);
        var localDirection = transform.InverseTransformDirection(Vector3.forward);
        localDirection.x *= -1;
        Direction currentDirection = GridTools.VectorToDirection(localDirection);

        state = new GridState(true, gridPosition, currentDirection);

        GameManager.Instance.registerObject(this);
    }

    #endregion

}