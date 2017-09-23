using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : Singleton<GameManager> {

    // The Settings Asset to use
    public GameSettings settings;

    // A list of every non-player grid object that is active
    private List<GridObject> gridObjects = new List<GridObject>();
    private GridObject player;

    // The history
    private List<IDictionary<GridObject, GridState>> stateHistory = new List<IDictionary<GridObject, GridState>>();

    // The player object
    private PlayerObject playerObject;

    // A grid manager class to manage tile state
    private GridManager _grid = new GridManager();
    public GridManager grid
    {
        get
        {
            return _grid;
        }
        private set
        {
            _grid = value;
        }
    }

    private IDictionary<GridObject, Vector2> currentActions;

    // A bool to determine whether input should be received
    public bool roundActive = false;

    private float turnTimer = 0.0f;

    private bool playerTurn = false;

    public void Awake()
    {
        //During this the gridobject list is populated and the grid tiles are added
    }

    public void Start()
    {
        createNewStateList();
        RefreshGrid();
    }

    public void Update()
    {

        if (roundActive)
        {
            turnTimer -= Time.deltaTime / settings.turnDuration;

            if (turnTimer > 0.0f)
            {
                updateObjects();
            } else {
                endTurn();
            }
        } else
        {
            if (Input.GetKeyDown("z"))
            {
                undo();
            } else if (Input.GetKeyDown("r"))
            {
                reset();
            }
        }
    }

    // Begins a complete round of turns
    public void advanceTurn()
    {
        startRound();
    }

    private void startRound()
    {
        createNewStateList();
        roundActive = true;
        startNewTurn();
    }

    private void startNewTurn()
    {
        // Reset per round variables
        playerTurn = !playerTurn;
        turnTimer = 1.0f;
        currentActions = new Dictionary<GridObject, Vector2>();

        // Have every gridObject declare their next action
        if (playerTurn)
        {
            // Add the player to this list
            playerObject.declareAction();
            currentActions.Add(playerObject, playerObject.nextState.gridPosition);

            // Add any objects the player is pushing
            if (SquareFilled(playerObject.nextState.gridPosition))
            {
                push(GridContents(playerObject.nextState.gridPosition), playerObject.nextState.direction);
            }

        } else
        {
            foreach (GridObject gridObject in gridObjects)
            {
                if (gridObject.state.exists)
                {
                    gridObject.declareAction();

                    if (gridObject.nextState.gridPosition != gridObject.state.gridPosition)
                    {
                        currentActions.Add(gridObject, gridObject.nextState.gridPosition);
                    }
                }

            }
        }

    }

    // This updates the position of all objects whos turn it is
    private void updateObjects()
    {
        foreach (GridObject gridObject in currentActions.Keys)
        {
            gridObject.animate(currentActions[gridObject], settings.basicMovement.Evaluate(1- turnTimer));
        }
    }

    
    private void endTurn()
    {
        // Create a dictionary to hold all to be created rooks
        IDictionary<Vector2, ColorType> upgradeList = new Dictionary<Vector2, ColorType>();

        // Create a dictionary to hold the final actions of every current gridObject
        IDictionary<GridObject, GridState> nextActions = new Dictionary<GridObject, GridState>();
        
        foreach (GridObject gridObject in currentActions.Keys)
        {
            
            Vector2 nextPosition = currentActions[gridObject];

            bool moving = false;
            bool colliding = false;
            bool destroy = false;
            bool upgrade = false;
            

            if (gridObject != playerObject)
            {
                
                // Check if the next space is filled
                if (SquareFilled(nextPosition) && GridContents(nextPosition) != gridObject)
                {
                    GridObject fillObject = GridContents(nextPosition);
                    print(fillObject.state.gridPosition);
                    //print(fillObject.nextState.gridPosition);
                    print(currentActions[fillObject]);

                    // Check if the object is moving
                    if (currentActions.Keys.Contains(fillObject))
                    {
                        if (fillObject.nextState.gridPosition != fillObject.state.gridPosition)
                        {
                            moving = true;
                        }
                    }

                    // if it is, check if its moving towards this gridObject
                    if (moving)
                    {
                        if (gridObject.state.gridPosition == currentActions[fillObject])
                        {
                            colliding = true;
                        }
                    }
                    else
                    {
                        colliding = true;
                    }

                    // If it is colliding, check what happens
                    if (colliding)
                    {
                        if (fillObject.color == gridObject.color && fillObject.powerLevel == gridObject.powerLevel)
                        {
                            destroy = true;
                            upgrade = true;
                        }
                        else
                        {
                            if (fillObject.powerLevel >= gridObject.powerLevel)
                            {
                                destroy = true;
                            }
                        }
                    }
                } else {
                    foreach (GridObject otherObject in currentActions.Keys)
                    {
                        if (otherObject != gridObject)
                        {
                            colliding = false;
                            if (nextPosition == currentActions[otherObject])
                            {
                                colliding = true;
                            }

                            if (colliding)
                            {
                                if (otherObject.color == gridObject.color && otherObject.powerLevel == gridObject.powerLevel)
                                {

                                    print(otherObject.color + " " + gridObject.color);
                                    destroy = true;
                                    upgrade = true;
                                }
                                else
                                {
                                    if (otherObject.powerLevel >= gridObject.powerLevel)
                                    {
                                        destroy = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            GridState newState = new GridState(!destroy, nextPosition, gridObject.nextState.direction);
            nextActions.Add(gridObject, newState);

            if (upgrade)
            {
                if (!upgradeList.ContainsKey(nextPosition))
                {
                    upgradeList.Add(nextPosition, gridObject.color);
                }
            }
        }

        foreach(GridObject gridObject in nextActions.Keys){
            gridObject.applyState(nextActions[gridObject]);
        }

        foreach (Vector2 square in upgradeList.Keys)
        {
            createRook(square, upgradeList[square]);
        }

        RefreshGrid();

        if (playerTurn)
        {
            // End the turn
            startNewTurn();
        } else
        {
            endRound();
        }
    }

    private void endRound()
    {
        roundActive = false;
        turnTimer = 0.0f;
        playerTurn = false;
    }

    private bool CanKill(GridObject object1, GridObject object2)
    {
        return object1.powerLevel >= object2.powerLevel;
    }

    private bool CanBePushed(GridObject gridObject, Direction direction)
    {
        Vector2 nextSquare = GridTools.directionPosition(gridObject.state.gridPosition, direction);

        if (SquareExists(nextSquare))
        {
            if (SquareFilled(nextSquare))
            {
                if (gridObject.color == GridContents(nextSquare).color && gridObject.powerLevel != GridContents(nextSquare).powerLevel)
                {

                    // If they're the same color, check if they can be pushed
                    return CanBePushed(GridContents(nextSquare), direction);
                }
                else
                {
                    // Otherwise someone is dying here
                    return true;
                }
            }
            else
            {
                return true;
            }
        } else
        {
            return false;
        }
        
    }

    private void push(GridObject gridObject, Direction direction)
    {
        Vector2 nextPosition = GridTools.directionPosition(gridObject.state.gridPosition, direction);

        currentActions.Add(gridObject, nextPosition);

        if (SquareFilled(nextPosition))
        {
            if (gridObject.color == GridContents(nextPosition).color && gridObject.powerLevel != GridContents(nextPosition).powerLevel)
            {
                push(GridContents(nextPosition), direction);
            } else
            {
                currentActions.Add(GridContents(nextPosition), nextPosition);
            }
        }
    }

    private void createRook(Vector2 square, ColorType color)
    {
        GameObject rook = Instantiate(settings.rookPrefab, GridTools.gridPositionToWorld(square), Quaternion.identity);
        rook.GetComponent<RookObject>().setColor(color);
    }

    #region GridObject commands

    // Adds a new grid object
    public void registerObject(GridObject obj)
    {
        gridObjects.Add(obj);
    }

    // Adds the object representing the player
    public void registerPlayer(PlayerObject obj)
    {
        playerObject = obj;
    }

    #endregion


    #region History commands

    // Returns the latest history list
    private IDictionary<GridObject, GridState> currentHistoryList
    {
        get
        {
            return stateHistory[stateHistory.Count - 1];
        }
    }

    private void undo()
    {
        if (stateHistory.Count > 0)
        {

            //Iterate through the stateHistory and set their corresponding gridobjects state
            foreach (GridObject gridObject in gridObjects)
            {
                if (currentHistoryList.ContainsKey(gridObject))
                {
                    gridObject.applyState(currentHistoryList[gridObject]);
                } else
                {
                    gridObject.state.exists = false;
                    gridObject.applyState(gridObject.state);
                }
            }
            playerObject.applyState(currentHistoryList[playerObject]);

            RefreshGrid();

            //Remove the latest history entry
            stateHistory.RemoveAt(stateHistory.Count - 1);
        }
        

    }

    private void reset()
    {
        createNewStateList();
        IDictionary<GridObject, GridState> firstHistoryList = stateHistory[0];
        foreach (GridObject gridObject in gridObjects)
        {
            if (firstHistoryList.ContainsKey(gridObject))
            {
                gridObject.applyState(firstHistoryList[gridObject]);
            }
            else
            {
                gridObject.state.exists = false;
                gridObject.applyState(gridObject.state);
            }
        }
        playerObject.applyState(firstHistoryList[playerObject]);

        RefreshGrid();

        createNewStateList();
    }

    private void createNewStateList()
    {
        //Add a new list
        IDictionary<GridObject, GridState> newStateList = new Dictionary<GridObject, GridState>();

        // For every gridobject in the scene, add their current state to the list
        foreach (GridObject gridObject in gridObjects)
        {
            newStateList.Add(gridObject, gridObject.state);
        }
        newStateList.Add(playerObject, playerObject.state);

        // Add the list to the history list
        stateHistory.Add(newStateList);
    }

    #endregion


    #region Grid commands

    public void AddTile(Vector2 square)
    {
        grid.addTile(square);
    }

    public bool SquareExists(Vector2 square)
    {
        return grid.exists(square);
    }

    public bool SquareFilled(Vector2 square)
    {
        return grid.isFilled(square);
    }

    private void RefreshGrid()
    {
        // Empty the grid
        grid.emptyGrid();

        // Fill the grid with each gridObject
        foreach (GridObject gridObject in gridObjects)
        {
            if (gridObject.state.exists)
            {
                grid.updateTile(gridObject.state.gridPosition, true, gridObject);
            }
        }

        // Add the player to the grid
        grid.updateTile(playerObject.state.gridPosition, true, playerObject);
    }

    public GridObject GridContents(Vector2 square)
    {
        return grid.getGridContents(square);
    }

    public bool CouldPlayerMoveHere(Vector2 square, Direction direction)
    {
        if (!SquareExists(square))
        {
            return false;
        }

        if (SquareFilled(square))
        {
            return CanBePushed(GridContents(square), direction);
        } else
        {
            return true;
        }
    }

    public bool CouldObjectMoveHere(Vector2 square, GridObject gridObject)
    {
        if (!SquareExists(square))
        {
            return false;
        }

        if (SquareFilled(square))
        {
            if (GridContents(square).color == gridObject.color || GridContents(square) == playerObject)
            {
                return false;
            } else
            {
                return true;
            }
        } else
        {
            return true;
        }
    }

    #endregion

}
