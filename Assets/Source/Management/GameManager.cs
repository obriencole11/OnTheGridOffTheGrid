using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : Singleton<GameManager> {

    // The Settings Asset to use
    public GameSettings settings;

    // A list of every grid object that is active
    private List<GridObject> gridObjects = new List<GridObject>();

    // A grid manager class to manage tile state
    public GridManager gridManager = new GridManager();

    // A bool to determine whether input should be received
    public bool gameActive = true;

    // A bool to determine whether the player is on the grid or not
    public bool onGrid = false;

    // A timer to count seconds between turns
    private float turnTimer = 0.0f;

    // The current turn number
    private int turnNumber = 0;

    // Event when the grid is entered
    public delegate void OnEnterGrid();
    public OnEnterGrid onEnterGrid;

    // Event when the grid is exited
    public delegate void OnExitGrid();
    public OnExitGrid onExitGrid;

    // Action Dictionaries
    private IDictionary<GridObject, Vector2> movementActions = new Dictionary<GridObject, Vector2>();
    private IDictionary<GridObject, Vector2> collisionActions = new Dictionary<GridObject, Vector2>();

    // Adds a new grid object
    public void addGridObject(GridObject obj)
    {
        gridObjects.Add(obj);
    }

    // Removes an existing grid object
    public void removeGridObject(GridObject obj)
    {
        gridObjects.Remove(obj);
    }

    // Begins a complete round of turns
    public void advanceTurn()
    {
        // Reset tracking variables
        gameActive = false;
        turnTimer = 0.0f;
        turnNumber = 0;
        generateActions();
    }

    public void generateActions()
    {

        // Clear the current action dictionaries
        movementActions.Clear();
        collisionActions.Clear();

        // Generate a list of currentGridObjects and a list of idle Grid Objects
        List<GridObject> currentObjects = new List<GridObject>();
        List<GridObject> idleObjects = new List<GridObject>();

        foreach (GridObject gridObject in gridObjects)
        {
            if (gridObject.turnNumber == turnNumber)
            {
                currentObjects.Add(gridObject);
            } else {
                idleObjects.Add(gridObject);
            }
        }

        // Now iterate throught the current objects, and determine their action for the turn
        foreach (GridObject gridObject in currentObjects)
        {

            // Calculate the next position for the object
            Vector2 nextPosition = GridTools.directionPosition(gridObject, gridObject.nextDirection);

            if (gridManager.exists(nextPosition))
            {
                // Check if the destination is filled
                if (gridManager.isFilled(nextPosition))
                {
                    // If so check if the filler is a current Object
                    GridObject fillObject = gridManager.getGridContents(nextPosition);

                    if (currentObjects.Contains(fillObject))
                    {
                        // If so, check if its gonna collide with the gridObject
                        Vector2 fillObjectPosition = GridTools.directionPosition(fillObject, fillObject.nextDirection);

                        if (gridObject.gridPosition == fillObjectPosition)
                        {
                            // If it is, add the gridObject to the collide group
                            collisionActions.Add(gridObject, nextPosition);
                        }
                        else
                        {
                            // Otherwise, check if it can move
                            if (!gridManager.exists(fillObjectPosition)) {

                                // If it can't move, try to push it
                                Vector2 fillObjectNextPosition = GridTools.directionPosition(fillObject, gridObject.nextDirection);

                                if (gridManager.isFilled(fillObjectNextPosition) || !gridManager.exists(fillObjectNextPosition))
                                {

                                    // If so add it to the collision actions
                                    collisionActions.Add(gridObject, nextPosition);
                                }
                                else
                                {
                                    // Otherwise, push the object
                                    movementActions.Add(gridObject, nextPosition);
                                    movementActions.Add(fillObject, fillObjectNextPosition);
                                }

                            }
                            else
                            {
                                // Otherwise add it to the movement group
                                movementActions.Add(gridObject, nextPosition);
                            }
                        }

                    }
                    else {

                        // If not, see if the fillObject is pushable
                        Vector2 fillObjectNextPosition = GridTools.directionPosition(fillObject, gridObject.nextDirection);

                        if (gridManager.isFilled(fillObjectNextPosition) || !gridManager.exists(fillObjectNextPosition))
                        {
                            if (gridObject.canKill)
                            {
                                fillObject.destroyed = true;
                            }

                            // If so add it to the collision actions
                            collisionActions.Add(gridObject, nextPosition);
                        }
                        else
                        {
                            // Otherwise, push the object
                            movementActions.Add(gridObject, nextPosition);
                            movementActions.Add(fillObject, fillObjectNextPosition);
                        }

                    }

                }
                else
                {
                    // If not check if its a destination of another current object
                    bool isShared = false;

                    foreach (GridObject otherObject in currentObjects)
                    {
                        Vector2 otherNextPosition = GridTools.directionPosition(otherObject, otherObject.nextDirection);

                        if (gridObject != otherObject && nextPosition == otherNextPosition)
                        {
                            isShared = true;
                            break;
                        }
                    }

                    if (isShared)
                    {
                        // If it is add it to the collision group
                        collisionActions.Add(gridObject, nextPosition);
                    }
                    else
                    {
                        // Otherwise add it to the movement group
                        movementActions.Add(gridObject, nextPosition);
                    }
                }
            }
            

        }
    }

    public void Start()
    {
    }

    public void Update()
    {

        // BUTTON EVENTS
        if (Input.GetKeyDown("r")){
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            gridObjects = new List<GridObject>();
            gridManager = new GridManager();
            gameActive = true;
            turnTimer = 0.0f;
            turnNumber = 0;
        }

        // TURN UPDATE
        // If the game is not active, begin the turn sequence
        if (!gameActive)
        {
            // Set the timer
            turnTimer += Time.deltaTime / settings.turnDuration;

            // If the timer has not finished...
            if (turnTimer < 1.0)
            {
                // Blend the positions for each action

                foreach (GridObject gridObject in movementActions.Keys)
                {
                    float blend = settings.basicMovement.Evaluate(turnTimer);
                    gridObject.blendPosition(blend, movementActions[gridObject]);
                }

                foreach (GridObject gridObject in collisionActions.Keys)
                {
                    float blend = settings.collision.Evaluate(turnTimer);
                    gridObject.blendPosition(blend, collisionActions[gridObject]);
                }

            }
            else
            {

                // Set the final position for each grid object whos turn it is
                // Additionally, update the gridmanager on the filled status
                foreach (GridObject gridObject in movementActions.Keys)
                {
                    gridManager.updateTile(gridObject.gridPosition, false);
                    
                }

                foreach (GridObject gridObject in movementActions.Keys)
                {
                    gridManager.updateTile(movementActions[gridObject], true, gridObject);
                    gridObject.setPosition(movementActions[gridObject]);
                }

                foreach (GridObject gridObject in collisionActions.Keys)
                {
                    gridObject.setPosition(gridObject.gridPosition);
                }

                turnNumber++;
                turnTimer = 0.0f;

                if (turnNumber == settings.turnCount)
                {
                    gameActive = true;
                }


                generateActions();

            }
            
        }
    }

    public void EnterGrid()
    {
        onGrid = true;
        onEnterGrid();
        
    }

    public void ExitGrid()
    {
        onGrid = false;
        onExitGrid();
    }
}   
