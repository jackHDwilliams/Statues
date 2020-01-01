using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze_Generation : MonoBehaviour
{
    [System.Serializable]
    public class Cells
    {
        public bool visted;
        public GameObject north, south, east, west;
        public GameObject floor;
    }

    [System.Serializable]
    public abstract class Algorithm
    {
        protected Cells[,] mazeCells;
        protected int mazeX, mazeY;

        protected Algorithm(Cells[,] mazeCells) : base()
        {
            this.mazeCells = mazeCells;
            mazeX = mazeCells.GetLength(0);
            mazeY = mazeCells.GetLength(1);
        }
    }



    public int mazeX, mazeY;
    public GameObject floorPrefab;
    public List<GameObject> wallPrefab;
    public float wallSize;

    private Cells[,] mazeCells;
    private GameObject parentGameObject;

    private int currentRow = 0;
    private int currentColumn = 0;

    private bool courseCompleted = false;

    public GameObject player;



    void Start()
    {
        AccessMaze();
        CreateMaze();

        //Instantiate(player, new Vector3(Random.Range(0, mazeX), 0, Random.Range(0, mazeY)), Quaternion.identity);
    }

    void Update()
    {

    }

    public void AccessMaze()
    {
        parentGameObject = new GameObject();
        parentGameObject.name = "Maze";

        mazeCells = new Cells[mazeX, mazeY];

        for (int x = 0; x < mazeX; x++)
        {
            for (int y = 0; y < mazeY; y++)
            {
                mazeCells[x, y] = new Cells();

                //Floor
                mazeCells[x, y].floor = Instantiate(floorPrefab, new Vector3(x * wallSize, -(wallSize / 2f), y * wallSize), Quaternion.identity);
                mazeCells[x, y].floor.name = "Floor " + x + "," + y;
                mazeCells[x, y].floor.transform.parent = parentGameObject.transform;

                //West
                if (y == 0)
                {
                    mazeCells[x, y].west = Instantiate(wallPrefab[Random.Range(0, wallPrefab.Count)], new Vector3(x * wallSize, 0, (y * wallSize) - (wallSize / 2f)), Quaternion.identity) as GameObject;
                    mazeCells[x, y].west.name = "West Wall " + x + "," + y;
                    mazeCells[x, y].west.transform.parent = parentGameObject.transform;
                }

                //East
                mazeCells[x, y].east = Instantiate(wallPrefab[Random.Range(0, wallPrefab.Count)], new Vector3(x * wallSize, 0, (y * wallSize) + (wallSize / 2f)), Quaternion.identity) as GameObject;
                mazeCells[x, y].east.name = "East Wall " + x + "," + y;
                mazeCells[x, y].east.transform.parent = parentGameObject.transform;

                //North
                if (x == 0)
                {
                    mazeCells[x, y].north = Instantiate(wallPrefab[Random.Range(0, wallPrefab.Count)], new Vector3((x * wallSize) - (wallSize / 2f), 0, y * wallSize), Quaternion.identity) as GameObject;
                    mazeCells[x, y].north.name = "North Wall " + x + "," + y;
                    mazeCells[x, y].north.transform.Rotate(Vector3.up * 90f);
                    mazeCells[x, y].north.transform.parent = parentGameObject.transform;
                }

                //South
                mazeCells[x, y].south = Instantiate(wallPrefab[Random.Range(0, wallPrefab.Count)], new Vector3((x * wallSize) + (wallSize / 2f), 0, y * wallSize), Quaternion.identity) as GameObject;
                mazeCells[x, y].south.name = "South Wall " + x + "," + y;
                mazeCells[x, y].south.transform.Rotate(Vector3.up * 90f);
                mazeCells[x, y].south.transform.parent = parentGameObject.transform;
            }
        }
    }

    public void CreateMaze()
    {
        LocateAndDestory();
    }

    void LocateAndDestory()
    {
        mazeCells[currentRow, currentColumn].visted = true;

        while (!courseCompleted)
        {
            DestroyWallObject();
            LocateWallObject();
        }
    }

    void DestroyWallObject()
    {
        while (RouteAvailable(currentRow, currentColumn))
        {
            int direct = Random.Range(1, 5);

            if (direct == 1 && AvailableCell(currentRow - 1, currentColumn))
            {
                // North Walls
                DestroyWall(mazeCells[currentRow, currentColumn].north);
                DestroyWall(mazeCells[currentRow - 1, currentColumn].south);
                currentRow--;
            }
            else if (direct == 2 && AvailableCell(currentRow + 1, currentColumn))
            {
                // South Walls
                DestroyWall(mazeCells[currentRow, currentColumn].south);
                DestroyWall(mazeCells[currentRow + 1, currentColumn].north);
                currentRow++;
            }
            if (direct == 3 && AvailableCell(currentRow, currentColumn + 1))
            {
                // East Walls
                DestroyWall(mazeCells[currentRow, currentColumn].east);
                DestroyWall(mazeCells[currentRow, currentColumn + 1].west);
                currentColumn++;
            }
            if (direct == 4 && AvailableCell(currentRow, currentColumn - 1))
            {
                // West Walls
                DestroyWall(mazeCells[currentRow, currentColumn].west);
                DestroyWall(mazeCells[currentRow, currentColumn - 1].east);
                currentColumn--;
            }

            mazeCells[currentRow, currentColumn].visted = true;
        }
    }

    void LocateWallObject()
    {
        courseCompleted = true;

        for (int x = 0; x < mazeX; x++)
        {
            for (int y = 0; y < mazeY; y++)
            {
                if (!mazeCells[x, y].visted && AdjacentVistedCells(x, y))
                {
                    courseCompleted = false;
                    currentRow = x;
                    currentColumn = y;
                    DestoryAdjacentWall(currentRow, currentColumn);
                    mazeCells[currentRow, currentColumn].visted = true;
                    return;
                }
            }
        }
    }

    bool RouteAvailable(int row, int col)
    {
        int availableRoutes = 0;

        if (row > 0 && !mazeCells[row - 1, col].visted)
        {
            availableRoutes++;
        }
        if (row < mazeX - 1 && !mazeCells[row + 1, col].visted)
        {
            availableRoutes++;
        }
        if (col > 0 && !mazeCells[row, col - 1].visted)
        {
            availableRoutes++;
        }
        if (col < mazeY - 1 && !mazeCells[row, col + 1].visted)
        {
            availableRoutes++;
        }

        return availableRoutes > 0;
    }

    bool AvailableCell(int row, int col)
    {
        if (row >= 0 && row < mazeX && col >= 0 && col < mazeY && !mazeCells[row, col].visted)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void DestroyWall(GameObject wall)
    {
        if (wall != null)
        {
            GameObject.Destroy(wall);
        }
    }

    bool AdjacentVistedCells(int row, int col)
    {
        int vistedCells = 0;

        //North
        if (row > 0 && mazeCells[row - 1, col].visted)
        {
            vistedCells++;
        }
        //South
        if (row < (mazeX - 2) && mazeCells[row + 1, col].visted)
        {
            vistedCells++;
        }
        //West
        if (col > 0 && mazeCells[row, col - 1].visted)
        {
            vistedCells++;
        }
        //East
        if (col < (mazeY - 2) && mazeCells[row, col + 1].visted)
        {
            vistedCells++;
        }

        return vistedCells > 0;

    }

    void DestoryAdjacentWall(int row, int col)
    {
        bool wallDestroyed = false;

        while (!wallDestroyed)
        {
            int direct = Random.Range(1, 5);

            if (direct == 1 && row > 0 && mazeCells[row - 1, col].visted)
            {
                DestroyWall(mazeCells[row, col].north);
                DestroyWall(mazeCells[row - 1, col].south);
                wallDestroyed = true;
            }
            else if (direct == 2 && row < (mazeX - 2) && mazeCells[row + 1, col].visted)
            {
                DestroyWall(mazeCells[row, col].south);
                DestroyWall(mazeCells[row + 1, col].north);
                wallDestroyed = true;
            }
            else if (direct == 3 && col > 0 && mazeCells[row, col - 1].visted)
            {
                DestroyWall(mazeCells[row, col].west);
                DestroyWall(mazeCells[row, col - 1].east);
                wallDestroyed = true;
            }
            else if (direct == 4 && col < (mazeY - 2) && mazeCells[row, col + 1].visted)
            {
                DestroyWall(mazeCells[row, col].east);
                DestroyWall(mazeCells[row, col + 1].west);
                wallDestroyed = true;
            }
        }
    }
}
