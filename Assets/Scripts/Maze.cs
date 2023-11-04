using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze : MonoBehaviour
{
    public MazeCell cellPrefab;
    private MazeCell[,] cells;

    public float generationStepDelay;
    public IntVector2 coordinates;
    public IntVector2 size;

    public MazePassage passagePrefab;
    public MazeWall wallPrefab;

    private bool mazeVisible = true;
    private GameObject[] ghosts, walls;
    private GameObject floor, mazeInstance;

    public void ToggleMaze()
    {
        mazeVisible = mazeVisible == true ? false : true;
        float scale = mazeVisible == true ? 1f : 0f;

        mazeInstance = GameObject.Find("mazeInstance");
        walls = GameObject.FindGameObjectsWithTag("ExtWall");
        floor = GameObject.Find("FloorNav");
        ghosts = GameObject.FindGameObjectsWithTag("ghost");

        //mazeInstance.transform.localScale = new Vector3(scale, scale, scale);   //this makes pacman eat the entire maze when it shrinks

        foreach (GameObject ghost in ghosts) ghost.gameObject.transform.localScale = new Vector3(scale, scale, scale);

        switch (mazeVisible)
        {
            case false:
                foreach (GameObject wall in walls) wall.gameObject.transform.localScale = new Vector3(scale, scale, scale);
                floor.GetComponent<MeshRenderer>().enabled = false;
                mazeInstance.transform.position = new Vector3(0f, 1000f, 0f);   //hide the maze in the sky instead
                break;
            case true:
            default:
                foreach (GameObject wall in walls) wall.gameObject.transform.localScale = new Vector3(.5f, 5f, 41f);
                floor.GetComponent<MeshRenderer>().enabled = true;
                mazeInstance.transform.position = new Vector3(0f, 0f, 0f);
                break;
        }
    }

    public IntVector2 RandomCoordinates
    {
        get
        {
            return new IntVector2(Random.Range(0, size.x), Random.Range(0, size.z));
        }
    }

    public bool ContainsCoordinates(IntVector2 coordinate)
    {
        return coordinate.x >= 0 && coordinate.x < size.x && coordinate.z >= 0 && coordinate.z < size.z;
    }

    public MazeCell GetCell(IntVector2 coordinates)
    {
        return cells[coordinates.x, coordinates.z];
    }

    public void Generate()
    {
        cells = new MazeCell[size.x, size.z];
        List<MazeCell> activeCells = new List<MazeCell>();
        DoFirstGenerationStep(activeCells);
        while (activeCells.Count > 0)
        {
            DoNextGenerationStep(activeCells);
        }
        Debug.Log("Maze Complete, resuming time");
    }

    private void DoFirstGenerationStep(List<MazeCell> activeCells)
    {
        activeCells.Add(CreateCell(RandomCoordinates));
    }

    private void DoNextGenerationStep(List<MazeCell> activeCells)
    {
        int currentIndex = activeCells.Count - 1;
        MazeCell currentCell = activeCells[currentIndex];
        if (currentCell.IsFullyInitialized)
        {
            activeCells.RemoveAt(currentIndex);
            return;
        }
        MazeDirection direction = currentCell.RandomUninitializedDirection;
        IntVector2 coordinates = currentCell.coordinates + direction.ToIntVector2();
        if (ContainsCoordinates(coordinates))
        {
            MazeCell neighbor = GetCell(coordinates);
            if (neighbor == null)
            {
                neighbor = CreateCell(coordinates);
                CreatePassage(currentCell, neighbor, direction);
                activeCells.Add(neighbor);
            }
            else
            {
                CreateWall(currentCell, neighbor, direction);
            }
        }
        else
        {
            CreateWall(currentCell, null, direction);
        }
    }

    private void CreatePassage(MazeCell cell, MazeCell otherCell, MazeDirection direction)
    {
        MazePassage passage = Instantiate(passagePrefab) as MazePassage;
        passage.Initialize(cell, otherCell, direction);
        passage = Instantiate(passagePrefab) as MazePassage;
        passage.Initialize(otherCell, cell, direction.GetOpposite());
    }

    private void CreateWall(MazeCell cell, MazeCell otherCell, MazeDirection direction)
    {
        MazeWall wall = Instantiate(wallPrefab) as MazeWall;
        wall.Initialize(cell, otherCell, direction);
        if (otherCell != null)
        {
            wall = Instantiate(wallPrefab) as MazeWall;
            wall.Initialize(otherCell, cell, direction.GetOpposite());
        }
    }

    private MazeCell CreateCell(IntVector2 coordinates)
    {
        MazeCell newCell = Instantiate(cellPrefab) as MazeCell;
        cells[coordinates.x, coordinates.z] = newCell;
        newCell.coordinates = coordinates;
        newCell.name = "Maze Cell " + coordinates.x + ", " + coordinates.z;
        newCell.transform.parent = transform;
        newCell.transform.localPosition = new Vector3(coordinates.x - size.x * 0.5f + 0.5f, -.5f, coordinates.z - size.z * 0.5f + 0.5f);
        return newCell;
    }
}
