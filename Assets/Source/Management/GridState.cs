using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridState {

    public bool exists;
    public Vector2 gridPosition;
    public Direction direction;

    public GridState(bool _exists, Vector2 _gridPosition, Direction _direction)
    {
        exists = _exists;
        gridPosition = _gridPosition;
        direction = _direction;
    }

}
