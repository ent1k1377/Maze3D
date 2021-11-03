using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MazeGenerator : MonoBehaviour
{
    public MazeCell[,] Maze
    {
        get
        {
            return _maze;
        }
    }

    public List<Chunk> Chunks
    {
        get
        {
            return _chunks;
        }
    }

    public float CellSize;

    public delegate void CreatingMaze();
    public event CreatingMaze OnCreatingMaze;

    [SerializeField] private Transform _cellsContainer;

    [SerializeField] private MazeCell _cellPrefab;

    [SerializeField] private NavMeshSurface _surface;

    private int _zSize => MazeOptions.MazeHeight;
    private int _xSize => MazeOptions.MazeWidth;
    private int _countOfWallsInChunkToDestroy => MazeOptions.AdditionalWallsToDestroy;
    private int _chunkSpawnAreaRadius => MazeOptions.RadiusOfSpawnAreaInChunk;

    private MazeCell[,] _maze;

    private List<Chunk> _chunks = new List<Chunk>();



    private void OnDrawGizmos()
    {
        if (_chunks.Count > 0)
        {
            foreach (Chunk chunk in _chunks)
            {
                foreach (Vector3 point in chunk.SpawnPositions)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(new Vector3(point.x, point.y + 0.4f, point.z), 0.1f);
                }
            }
        }
    }

    private void Start()
    {
        CreateMap();
        BakeNavMeshes();
    }

    private void BakeNavMeshes()
    {
        _surface.BuildNavMesh();
    }

    private void CreateMap()
    {
        _maze = new MazeCell[_zSize, _xSize];
        MazeCellInfo[,] cells = GenerateMazePaths();
        
        for (int z = 0; z < cells.GetLength(0); z++)
        {
            for (int x = 0; x < cells.GetLength(1); x++)
            {
                Vector3 position = new Vector3(cells[z, x].Xpos, 0, cells[z, x].Zpos);
                MazeCell mazeCell = Instantiate(_cellPrefab, position, Quaternion.identity, _cellsContainer);
                mazeCell.CellInfo = cells[z, x];
                mazeCell.CreateWalls();
                mazeCell.ChangeScalesToAvoidGlitches();
                _maze[z, x] = mazeCell;

                if (z == 0)
                    mazeCell.DownWall.OnTheEdge = true;
                else if (z == cells.GetLength(0) - 1)
                    mazeCell.TopWall.OnTheEdge = true;
                if (x == 0)
                    mazeCell.LeftWall.OnTheEdge = true;
                else if (x == cells.GetLength(1) - 1)
                    mazeCell.RightWall.OnTheEdge = true;
            }
        }

        _chunks = CreateMazeChunks(_maze);
        if (_countOfWallsInChunkToDestroy > 0)
        {
            foreach (Chunk chunk in _chunks)
            {
                DestroyRandomWallsInChunk(chunk);
            }
        }

        OnCreatingMaze?.Invoke();
    }

    private MazeCellInfo[,] GenerateMazePaths()
    {
        Stack<MazeCellInfo> cellsStack = new Stack<MazeCellInfo>();
        MazeCellInfo[,] maze = InitializeMazeCells();
        MazeCellInfo currentCell = maze[0, 0];

        currentCell.Visited = true;

        do
        {
            List<MazeCellInfo> accessibleNeighbours = new List<MazeCellInfo>();

            int Z = currentCell.Zpos;
            int X = currentCell.Xpos;

            if (Z > 0 && !maze[Z - 1, X].Visited) accessibleNeighbours.Add(maze[Z - 1, X]); // вниз
            if (X > 0 && !maze[Z, X - 1].Visited) accessibleNeighbours.Add(maze[Z, X - 1]); // влево
            if (Z < _zSize - 1 && !maze[Z + 1, X].Visited) accessibleNeighbours.Add(maze[Z + 1, X]); // вверх
            if (X < _xSize - 1 && !maze[Z, X + 1].Visited) accessibleNeighbours.Add(maze[Z, X + 1]); // вправо

            if (accessibleNeighbours.Count > 0)
            {
                MazeCellInfo chosen = accessibleNeighbours[Random.Range(0, accessibleNeighbours.Count)];
                RemoveWall(currentCell, chosen);
                chosen.Visited = true;
                cellsStack.Push(chosen);
                currentCell = chosen;
            }
            else
                currentCell = cellsStack.Pop();
        }
        while (cellsStack.Count > 0);

        return maze;

        void RemoveWall(MazeCellInfo current, MazeCellInfo chosen)
        {
            if (current.Xpos == chosen.Xpos)
            {
                if (current.Zpos > chosen.Zpos) current.HasDownWall = false;
                else chosen.HasDownWall = false;
            }
            else
            {
                if (current.Xpos > chosen.Xpos) current.HasLeftWall = false;
                else chosen.HasLeftWall = false;
            }
        }
    }

    private MazeCellInfo[,] InitializeMazeCells()
    {
        MazeCellInfo[,] cells = new MazeCellInfo[_zSize, _xSize];

        for (int z = 0; z < _zSize; z++)
        {
            for (int x = 0; x < _xSize; x++)
            {
                MazeCellInfo cellInfo = new MazeCellInfo(z, x);

                if (z == _zSize - 1) cellInfo.HasTopWall = true;
                if (x == _xSize - 1) cellInfo.HasRightWall = true;

                cells[z, x] = cellInfo;
            }
        }

        return cells;
    }

    public void DestoyWallsInRange(Vector3 originPosition, float range)
    {
        Collider[] colliders = Physics.OverlapSphere(originPosition, range);
        foreach (Collider collider in colliders)
        {
            if (collider.GetComponent<MazeWall>())
            {
                MazeWall wall = collider.GetComponent<MazeWall>();
                if (wall.gameObject.activeSelf && !wall.OnTheEdge)
                {
                    wall.gameObject.SetActive(false);
                }
            } 
        }

        foreach (Collider collider in colliders)
        {
            if (collider.GetComponent<MazeCell>())
            {
                MazeCell cell = collider.GetComponent<MazeCell>();
                cell.UpdateWallStates();
            }
        }
    }

    private void DestroyRandomWallsInChunk(Chunk chunk)
    {
        List<MazeWall> walls = new List<MazeWall>();
        foreach (MazeCell cell in chunk.MazeCells)
        {
            AddWall(cell.LeftWall);
            AddWall(cell.RightWall);
            AddWall(cell.TopWall);
            AddWall(cell.DownWall);
        }

        for (int i = 0; i < _countOfWallsInChunkToDestroy; i++)
        {
            walls[Random.Range(0, walls.Count)].gameObject.SetActive(false);
        }

        foreach (MazeCell cell in chunk.MazeCells)
        {
            cell.UpdateWallStates();
        }


        void AddWall(MazeWall wall)
        {
            if (wall.gameObject.activeSelf && !wall.OnTheEdge) 
                walls.Add(wall);
        }
    }

    private List<Chunk> CreateMazeChunks(MazeCell[,] maze)
    {
        List<Chunk> chunks = new List<Chunk>();
        int chunksCount = 4;
        int booksCount = 3;
        int chunkWidth;
        int chunkHeight;

        if (maze.GetLength(0) % 2 == 0) chunkHeight = maze.GetLength(0) / 2;
        else chunkHeight = (maze.GetLength(0) - 1) / 2;
        if (maze.GetLength(1) % 2 == 0) chunkWidth = maze.GetLength(1) / 2;
        else chunkWidth = (maze.GetLength(1) - 1) / 2;

        int bookNumber = 0;

        for (int h = 0; h < chunksCount / 2; h++)
        {
            for (int w = 0; w < chunksCount / 2; w++)
            {
                int currentStepInHeight = 0;
                int currentStepInWidth = 0;

                if (maze.GetLength(0) % 2 == 0) currentStepInHeight = chunkHeight * h;
                //else if (h > 0) currentStepInHeight = chunkHeight * h + 1;  ДЛЯ НЕЧЕТНОГО ЛАБИРИНТА
                //else if (w == 0) currentStepInHeight = chunkHeight * h;
                if (maze.GetLength(1) % 2 == 0) currentStepInWidth = chunkWidth * w;
                //else if (w > 0) currentStepInWidth = chunkWidth * w + 1;
                //else if (h == 0) currentStepInWidth = chunkWidth * w;

                Chunk chunk = CreateChunk(currentStepInHeight, currentStepInWidth);
            
                if (booksCount >= bookNumber && bookNumber != 0)
                {
                    chunk.BookSpawn = true;
                    bookNumber++;
                }
                else if (bookNumber == 0)
                    bookNumber++;

                chunks.Add(chunk);
            }
        }

        return chunks;


        Chunk CreateChunk(int startPosInColumn, int startPosInRow)
        {
            Chunk chunk = new Chunk();
            chunk.MazeCells = new MazeCell[chunkHeight, chunkWidth];

            int cornerPosInColumn = 0; //Номер "углового" элемента в массиве. Это должен быть угол и чанка, и всего лабиринта одновременно
            int cornerPosInRow = 0;

            for (int z = 0; z < chunkHeight; z++)
            {
                for (int x = 0; x < chunkWidth; x++)
                {
                    chunk.MazeCells[z, x] = maze[startPosInColumn + z, startPosInRow + x];
                    //Debug.Log($"zPos: {startPosInColumn + z}  xPos: {startPosInRow + x}");
                    //Ищем угол чанка и направление итераций
                    if (startPosInColumn + z == 0)
                    {
                        cornerPosInColumn = startPosInColumn + z;
                        chunk.VerticalLocation = VerticalLocations.bottom;
                    }
                    else if (startPosInColumn + z == maze.GetLength(0) - 1) 
                    {
                        cornerPosInColumn = startPosInColumn + z;
                        chunk.VerticalLocation = VerticalLocations.top;
                    }

                    if (startPosInRow + x == 0)
                    {
                        cornerPosInRow = startPosInRow + x;
                        chunk.HorizontalLocation = HorizontalLocations.left;
                    }
                    else if (startPosInRow + x == maze.GetLength(1) - 1)
                    {
                        cornerPosInRow = startPosInRow + x;
                        chunk.HorizontalLocation = HorizontalLocations.right;
                    }  
                }
            }

            //Debug.Log(chunk.VerticalLocation + "  " + chunk.HorizontalLocation);
            //Debug.Log(123);
        
            chunk.SpawnPositions = new Vector3[_chunkSpawnAreaRadius, _chunkSpawnAreaRadius];
            
            int originPosZ = 0;
            int originPosX = 0;
            if (cornerPosInColumn > 0)
                originPosZ = cornerPosInColumn - _chunkSpawnAreaRadius + 1;
            if (cornerPosInRow > 0)
                originPosX = cornerPosInRow - _chunkSpawnAreaRadius + 1;

            for (int z = 0; z < _chunkSpawnAreaRadius; z++)
            {
                for (int x = 0; x < _chunkSpawnAreaRadius; x++)
                {
                    float Zpos = maze[originPosZ + z, originPosX + x].transform.position.z;
                    float Xpos = maze[originPosZ + z, originPosX + x].transform.position.x;
                    chunk.SpawnPositions[z, x] = new Vector3(Xpos, 0, Zpos);
                }
            }

            return chunk;
        }
    }
}

public class Chunk
{
    public bool BookSpawn;

    public Book ContainedBook;

    public Vector3[,] SpawnPositions;
    public MazeCell[,] MazeCells;

    public HorizontalLocations HorizontalLocation;
    public VerticalLocations VerticalLocation;
}

public enum HorizontalLocations
{
    undefined,
    left,
    right
}

public enum VerticalLocations
{
    undefined,
    top,
    bottom
}