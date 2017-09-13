using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public bool filled;
    public GridObject contents;

    public Tile(bool _filled, GridObject _contents)
    {
        filled = _filled;
        contents = _contents;
    }
}

public class GridManager {

    private IDictionary<Vector2, Tile> Grid = new Dictionary<Vector2, Tile>();
    
    public void addTile(Vector2 square)
    {
        Grid.Add(square, new Tile(false, null));
    }

    public void updateTile(Vector2 square, bool filled, GridObject gridObject = null)
    {
        Grid[square].filled = filled;
        Grid[square].contents = gridObject;
    }

    // Returns the contents of a specified square
    public GridObject getGridContents(Vector2 square)
    {
        return Grid[square].contents;
    }

    // Checks if a specified square is filled or not
    public bool isFilled(Vector2 square)
    {
        if (exists(square))
        {
            return Grid[square].filled;
        } else
        {
            return false;
        }
    }

    // Checks if a specified square exists
    public bool exists(Vector2 square)
    {
        return Grid.ContainsKey(square);
    }

}
