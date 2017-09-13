using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction {
    UP,
    DOWN,
    LEFT,
    RIGHT
}

public static class GridTools
{

    public static Vector3 gridPositionToWorld(Vector2 gridPosition)
    {
        return new Vector3(gridPosition.x, 0, gridPosition.y);
    }

    public static Vector2 worldPositionToGrid(Vector3 worldPosition)
    {
        return new Vector2(worldPosition.x, worldPosition.z);
    }

    public static Vector2 rightPosition(GridObject gridObject)
    {
        return new Vector2(gridObject.gridPosition.x + 1, gridObject.gridPosition.y);
    }

    public static Vector2 leftPosition(GridObject gridObject)
    {
        return new Vector2(gridObject.gridPosition.x - 1, gridObject.gridPosition.y);
    }

    public static Vector2 upPosition(GridObject gridObject)
    {
        return new Vector2(gridObject.gridPosition.x, gridObject.gridPosition.y + 1);
    }

    public static Vector2 downPosition(GridObject gridObject)
    {
        return new Vector2(gridObject.gridPosition.x, gridObject.gridPosition.y - 1);
    }

    public static Vector2 directionPosition(GridObject gridObject, Direction direction)
    {
        if (direction == Direction.RIGHT)
        {
            return rightPosition(gridObject);
        }
        else if (direction == Direction.LEFT)
        {
            return leftPosition(gridObject);
        }
        else if (direction == Direction.UP)
        {
            return upPosition(gridObject);
        }
        else if (direction == Direction.DOWN)
        {
            return downPosition(gridObject);
        }
        else
        {
            throw new System.ArgumentException("Direction is not valid", "direction");
        }
    }
}
