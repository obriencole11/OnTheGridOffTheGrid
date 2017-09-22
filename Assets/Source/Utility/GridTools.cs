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

    public static Vector2 rightPosition(Vector2 gridPosition)
    {
        return new Vector2(gridPosition.x + 1, gridPosition.y);
    }

    public static Vector2 leftPosition(Vector2 gridPosition)
    {
        return new Vector2(gridPosition.x - 1, gridPosition.y);
    }

    public static Vector2 upPosition(Vector2 gridPosition)
    {
        return new Vector2(gridPosition.x, gridPosition.y + 1);
    }

    public static Vector2 downPosition(Vector2 gridPosition)
    {
        return new Vector2(gridPosition.x, gridPosition.y - 1);
    }

    public static Vector2 directionPosition(Vector2 gridPosition, Direction direction)
    {
        if (direction == Direction.RIGHT)
        {
            return rightPosition(gridPosition);
        }
        else if (direction == Direction.LEFT)
        {
            return leftPosition(gridPosition);
        }
        else if (direction == Direction.UP)
        {
            return upPosition(gridPosition);
        }
        else if (direction == Direction.DOWN)
        {
            return downPosition(gridPosition);
        }
        else
        {
            throw new System.ArgumentException("Direction is not valid", "direction");
        }
    }

    public static Direction VectorToDirection(Vector3 forward)
    {

        if (forward == Vector3.forward)
        {
            return Direction.UP;
        }
        else if (forward == Vector3.back)
        {
            return Direction.DOWN;
        }
        else if (forward == Vector3.right)
        {
            return Direction.RIGHT;
        }
        else if (forward == Vector3.left)
        {
            return Direction.LEFT;
        }
        else
        {
            throw new System.ArgumentException("Direction is not valid", "direction");
        }
    }

    public static Vector3 DirectionToVector(Direction direction)
    {
        switch (direction)
        {
            case Direction.UP:
                return Vector3.forward;
            case Direction.DOWN:
                return Vector3.back;
            case Direction.RIGHT:
                return Vector3.right;
            case Direction.LEFT:
                return Vector3.left;
            default:
                throw new System.ArgumentException("Direction is not valid", "direction");
        }
    }

    public static Direction oppositeDirection(Direction direction)
    {
        Vector3 vector = DirectionToVector(direction);
        vector *= -1;

        return VectorToDirection(vector);
    }

    public static Quaternion DirectionToQuaternion(Direction direction)
    {
        return Quaternion.LookRotation(DirectionToVector(direction));
    }
}
