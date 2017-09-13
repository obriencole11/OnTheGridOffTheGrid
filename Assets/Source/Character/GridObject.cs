using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject : MonoBehaviour {

    // The 2D position on the grid
    public Vector2 gridPosition { get; private set; }

    // The next position to move to
    public Direction nextDirection { get; private set; }

    public bool canKill = true;

    // Whether the object is active or not
    private bool _destroyed = false;
    public bool destroyed
    {
        get
        {
            return _destroyed;
        }
        set
        {
            _destroyed = value;

            // Disable all child meshes
            Renderer mesh = GetComponentInChildren<Renderer>();
            mesh.enabled = !destroyed;

            // Change the tile state
            if (destroyed)
            {
                GameManager.Instance.gridManager.updateTile(gridPosition, false);
                GameManager.Instance.removeGridObject(this);
            }
            else
            {
                GameManager.Instance.gridManager.updateTile(gridPosition, true, this);
                GameManager.Instance.addGridObject(this);
            }
        }
    }

    // The relative turn this object will move during
    [HideInInspector]
    public int turnNumber = 0;

    // An event called when the character is moved
    public delegate void OnNewPosition();
    public OnNewPosition onNewPosition;

    public void Awake()
    {
        // Register this grid object
        GameManager.Instance.addGridObject(this);
    }

    public void Start()
    {
        

        GameManager.Instance.onEnterGrid += AddObject;
    }

    public void AddObject()
    {
        gridPosition = GridTools.worldPositionToGrid(transform.position);
        setPosition(gridPosition);
        GameManager.Instance.gridManager.updateTile(gridPosition, true, this);
    }

    // Blend the position of the object, this is for animating the object rather than actually moving it in grid space
    public void blendPosition(float blend, Vector2 position)
    {
        Vector2 newPosition = new Vector2(Mathf.Lerp(gridPosition.x, position.x, blend), Mathf.Lerp(gridPosition.y, position.y, blend));
        transform.position = GridTools.gridPositionToWorld(newPosition);
    }

    // Set the new grid position and move the transform to it
    public void setPosition(Vector2 newPosition)
    {
        gridPosition = newPosition;
        transform.position = GridTools.gridPositionToWorld(newPosition);

        if (onNewPosition != null) onNewPosition();
    }

    // Declare the next position for this object
    public void setNextDirection(Direction direction)
    {
        nextDirection = direction;
    }

    public void destroy()
    {
        gameObject.SetActive(false);
    }


}