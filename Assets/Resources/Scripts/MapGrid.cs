using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGrid : MonoBehaviour
{
    public Vector3[,] Grid
    {
        get
        {
            return _grid;
        }
    }
    Vector3[,] _grid;

    [SerializeField] MazeGenerator _mazeGenerator;

    [SerializeField] LayerMask _floorLayer;
    [SerializeField] LayerMask _obstacleLayer;


    void Awake()
    {
        _mazeGenerator.OnCreatingMaze += CreateMapGrid;
    }

    void CreateMapGrid()
    {
        _grid = new Vector3[_mazeGenerator.Maze.GetLength(0), _mazeGenerator.Maze.GetLength(1)];
        for (int z = 0; z < _grid.GetLength(0); z++)
        {
            for (int x = 0; x < _grid.GetLength(1); x++)
            {
                _grid[z, x] = _mazeGenerator.Maze[z, x].transform.position;
            }
        }
    }

    public List<Vector3> GetAreaInDirection(Vector3 originPoint, int radius, Vector3 direction)
    {
        List<Vector3> area = new List<Vector3>();
        Vector3 point = new Vector3(Mathf.Round(originPoint.x), 0, Mathf.Round(originPoint.z));
        for(int z = 0; z < Grid.GetLength(0); z++)
        {
            for(int x = 0; x < Grid.GetLength(1); x++)
            {
                if (point == Grid[z, x])
                {
                    int zMult = 1;
                    int xMult = 1;
                    int zLimit = Grid.GetLength(0);
                    int xLimit = Grid.GetLength(1);
                    int zRadius = radius;
                    int xRadius = radius;

                    if (direction.z > 0.5f)
                    {
                        zRadius = radius * 2;
                    }
                    else if (direction.z < -0.5f)
                    {
                        zMult = -1;
                        zRadius = radius * 2;
                    }

                    if (direction.x < -0.5f)
                    {
                        xMult = -1;
                        xRadius = radius * 2;
                    }
                    else if (direction.x > 0.5f)
                    {
                        xRadius = radius * 2;
                    }

                    for (int z2 = 0; z2 <= zRadius; z2++)
                    {
                        for (int x2 = 0; x2 <= xRadius; x2++)
                        {
                            int zNumber = z + (z2 * zMult);
                            int zNumber2 = z - (z2 * zMult);
                            int xNumber = x + (x2 * xMult);
                            int xNumber2 = x - (x2 * xMult);
                            if (zRadius > xRadius)
                            {
                                CheckArrayElementAndAdd(zNumber, xNumber);
                                CheckArrayElementAndAdd(zNumber, xNumber2);
                            }
                            else if (zRadius < xRadius)
                            {
                                CheckArrayElementAndAdd(zNumber, xNumber);
                                CheckArrayElementAndAdd(zNumber2, xNumber);
                            }
                            else if (zRadius == xRadius)
                                CheckArrayElementAndAdd(zNumber, xNumber);
                        }
                    }

                    void CheckArrayElementAndAdd(int zNum, int xNum)
                    {
                        if (IsElementInRange(zMult, zNum, zLimit) == true && IsElementInRange(xMult, xNum, xLimit) == true
                            && Physics.OverlapSphere(Grid[zNum, xNum] + Vector3.up, 0.3f, _obstacleLayer).Length == 0)
                        {
                            area.Add(Grid[zNum, xNum]);
                        }
                    }

                    bool IsElementInRange(int multiplier, int value, int limit)
                    {
                        bool result = false;
                        if (multiplier < 0)
                        {
                            if (value > 0 && value < limit) result = true;
                            else result = false;
                        }
                        else
                        {
                            if (value > 0 && value < limit) result = true;
                            else result = false;
                        }
                        return result;
                    }
                    break;
                }
            }
        }
        return area;
    }

    public List<Vector3> GetAreaAtNearbyPoint(Vector3 originPoint, int radius)
    {
        List<Vector3> area = new List<Vector3>();
        Vector3 point = new Vector3(Mathf.Round(originPoint.x), 0, Mathf.Round(originPoint.z));
        for(int z = 0; z < Grid.GetLength(0); z++)
        {
            for(int x = 0; x < Grid.GetLength(1); x++)
            {
                if (point == Grid[z, x])
                {
                    for (int z2 = 0; z2 < radius ; z2++)
                    {
                        for (int x2 = 0; x2 < radius; x2++)
                        {
                            if (z - z2 >= 0 && x - x2 >= 0)
                                area.Add(Grid[z - z2, x - x2]);

                            if (z - z2 >= 0 && x + x2 < Grid.GetLength(1))
                                area.Add(Grid[z - z2, x + x2]);

                            if (z + z2 < Grid.GetLength(0) && x - x2 >= 0)
                                area.Add(Grid[z + z2, x - x2]);

                            if (z + z2 < Grid.GetLength(0) && x + x2 < Grid.GetLength(1))
                                area.Add(Grid[z + z2, x + x2]);
                        }
                    }   
                }
            }
        }
        return area;
    }

    void OnDestroy()
    {
        _mazeGenerator.OnCreatingMaze -= CreateMapGrid;
    }
}
