using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] MazeGenerator _mazeGenerator;

    [SerializeField] List<UnitInfo> _units = new List<UnitInfo>();
    [SerializeField] List<Book> _books = new List<Book>();

    [SerializeField] Coin _coinTemplate;
    [SerializeField] Transform _coinsContainer;
    [SerializeField] Transform _coinsEffectPool;

    [SerializeField] LayerMask _bookLayer;
    [SerializeField] LayerMask _obstacleLayer;

    int _minCoinsAmountInPack => MazeOptions.MinCoinsInPack;
    int _maxCoinsAmountInPack => MazeOptions.MaxCoinsInPack;


    void Awake()
    {
        _mazeGenerator.OnCreatingMaze += Spawn;
    }

    void Spawn()
    {
        SpawnBooks();
        SetRandomBooksToGhosts();
        SpawnUnits();
        SpawnCoins();
    }

    void SpawnCoins()
    {
        for (int p = 1; p < _mazeGenerator.Chunks.Count; p++)
        {
            Chunk chunk = _mazeGenerator.Chunks[p];
            MazeCell[,] cells = chunk.MazeCells;
            List<ElementIndexes> availableIndexes = new List<ElementIndexes>();

            int minZ = 0;
            int maxZ = cells.GetLength(0);
            int minX = 0;
            int maxX = cells.GetLength(1);

            if (chunk.VerticalLocation == VerticalLocations.top)
                minZ = 1;
            else if ((chunk.VerticalLocation == VerticalLocations.bottom))
                maxZ = cells.GetLength(0) - 1;
            if (chunk.HorizontalLocation == HorizontalLocations.right)
                minX = 1;
            else if (chunk.HorizontalLocation == HorizontalLocations.left)
                maxX = cells.GetLength(1) - 1;

            int coinsCount = Random.Range(_minCoinsAmountInPack, _maxCoinsAmountInPack + 1);
            do
            {
                availableIndexes.Clear();

                int Zpos = Random.Range(minZ, maxZ);
                int Xpos = Random.Range(minX, maxX);
                MazeCell cellToSpawnPack = cells[Zpos, Xpos];

                while (Physics.OverlapSphere(cellToSpawnPack.transform.position, 0.2f, _bookLayer).Length > 0 &&
                    cellToSpawnPack.Coin == null)
                {
                    Zpos = Random.Range(minZ, maxZ);
                    Xpos = Random.Range(minX, maxX);
                    cellToSpawnPack = cells[Zpos, Xpos];
                }

                ElementIndexes currentIndexes = new ElementIndexes(Zpos, Xpos);
                availableIndexes.Add(currentIndexes);
                MazeCell currentCell = cellToSpawnPack;            

                for (int c = 0; c < coinsCount - 1; c++)
                {
                    List<ElementIndexes> tempIndexes = new List<ElementIndexes>();

                    if (currentIndexes.Z + 1 < maxZ && cells[currentIndexes.Z + 1, currentIndexes.X].Coin == null && 
                        !availableIndexes.Contains(new ElementIndexes(currentIndexes.Z + 1, currentIndexes.X)) &&
                        !ObstacleOnTheWay(currentCell.transform.position, cells[currentIndexes.Z + 1, currentIndexes.X].transform.position))
                    {
                        tempIndexes.Add(new ElementIndexes(currentIndexes.Z + 1, currentIndexes.X));
                    }
                    if (currentIndexes.X + 1 < maxX && cells[currentIndexes.Z, currentIndexes.X + 1].Coin == null && 
                        !availableIndexes.Contains(new ElementIndexes(currentIndexes.Z, currentIndexes.X + 1)) &&
                        !ObstacleOnTheWay(currentCell.transform.position, cells[currentIndexes.Z, currentIndexes.X + 1].transform.position))
                    {
                        tempIndexes.Add(new ElementIndexes(currentIndexes.Z, currentIndexes.X + 1));
                    }
                    if (currentIndexes.Z - 1 >= minZ && cells[currentIndexes.Z - 1, currentIndexes.X].Coin == null && 
                        !availableIndexes.Contains(new ElementIndexes(currentIndexes.Z - 1, currentIndexes.X)) &&
                        !ObstacleOnTheWay(currentCell.transform.position, cells[currentIndexes.Z - 1, currentIndexes.X].transform.position))
                    {
                        tempIndexes.Add(new ElementIndexes(currentIndexes.Z - 1, currentIndexes.X));
                    }
                    if (currentIndexes.X - 1 >= minX && cells[currentIndexes.Z, currentIndexes.X - 1].Coin == null && 
                        !availableIndexes.Contains(new ElementIndexes(currentIndexes.Z, currentIndexes.X - 1)) &&
                        !ObstacleOnTheWay(currentCell.transform.position, cells[currentIndexes.Z, currentIndexes.X - 1].transform.position))
                    {
                        tempIndexes.Add(new ElementIndexes(currentIndexes.Z, currentIndexes.X - 1));
                    }

                    if (tempIndexes.Count == 0)
                        break;

                    currentIndexes = tempIndexes[Random.Range(0, tempIndexes.Count)];
                    availableIndexes.Add(currentIndexes);
                    currentCell = cells[currentIndexes.Z, currentIndexes.X];
                }
            }
            while (availableIndexes.Count < coinsCount);

            foreach (ElementIndexes indexes in availableIndexes)
            {
                MazeCell cell = cells[indexes.Z, indexes.X];

                if (cell.Coin == null)
                {
                    cell.Coin = Instantiate(_coinTemplate, 
                        cell.transform.position + Vector3.up / 2, 
                        GetRandomRotationInY(_coinTemplate.transform), 
                        _coinsContainer);

                    cell.Coin.EffectsPool = _coinsEffectPool;
                }
            }
        }

        Quaternion GetRandomRotationInY (Transform obj)
        {
            Quaternion rotation = Quaternion.Euler(obj.eulerAngles.x, Random.Range(0f, 360f), obj.eulerAngles.z);
            return rotation;
        }

        bool ObstacleOnTheWay(Vector3 startPos, Vector3 finishPos)
        {
            Vector3 start = new Vector3(startPos.x, startPos.y + 0.5f, startPos.z);
            Vector3 finish = new Vector3(finishPos.x, finishPos.y + 0.5f, finishPos.z);
            Vector3 direction = (finish - start).normalized;
            float distance = Vector3.Distance(start, finish);
            
            if (Physics.Raycast(start, direction, distance, _obstacleLayer))
                return true;
            else
                return false;
        }

        
    }

    void SpawnUnits()
    {
        if (_units.Count > 0)
        {
            List<UnitInfo> units = new List<UnitInfo>(_units);
            foreach (Chunk chunk in _mazeGenerator.Chunks)
            {
                if (units.Count > 0)
                {
                    List<UnitInfo> tempUnits = new List<UnitInfo>(units);
                    while (tempUnits.Count > 0)
                    {
                        UnitInfo randomUnit = tempUnits[Random.Range(0, tempUnits.Count)];
                        tempUnits.Remove(randomUnit);
    
                        if (!randomUnit.isPlayer && chunk.BookSpawn 
                            && chunk.ContainedBook.transform == randomUnit.GetComponent<EnemyAI>().BookToDefend)
                        {
                            int randomZ = Random.Range(0, chunk.SpawnPositions.GetLength(0));
                            int randomX = Random.Range(0, chunk.SpawnPositions.GetLength(1));
                            Vector3 randomPosition = chunk.SpawnPositions[randomZ, randomX];
                            randomUnit.transform.position = new Vector3(randomPosition.x, randomUnit.transform.position.y, randomPosition.z);
                            StartCoroutine(ActivateAfterSeconds(randomUnit.gameObject, 1f));
                            units.Remove(randomUnit);
                            break;
                        }
                        else if (randomUnit.isPlayer && !chunk.BookSpawn)
                        {
                            Vector3 position = chunk.MazeCells[0, 0].transform.position;
                            randomUnit.transform.position = new Vector3(position.x, randomUnit.transform.position.y, position.z);
                            units.Remove(randomUnit);
                            break;
                        } 
                    }
                }
            }
        }
    }

    void SpawnBooks()
    {
        if (_books.Count > 0)
        {
            List<Book> books = new List<Book>(_books);
            foreach (Chunk chunk in _mazeGenerator.Chunks)
            {
                if (chunk.BookSpawn)
                {
                    Book randomBook = books[Random.Range(0, books.Count)];
                    books.Remove(randomBook);

                    int randomZ = Random.Range(0, chunk.SpawnPositions.GetLength(0));
                    int randomX = Random.Range(0, chunk.SpawnPositions.GetLength(1));
                    Vector3 randomPosition = chunk.SpawnPositions[randomZ, randomX];

                    List<float> shiftsZ = new List<float>();
                    List<float> shiftsX = new List<float>();
                    float shiftZ = _mazeGenerator.CellSize / 2;
                    float shiftX = _mazeGenerator.CellSize / 2;
                    if (randomZ > 0) shiftsZ.Add(-shiftZ);
                    if (randomZ < chunk.SpawnPositions.GetLength(0) - 1) shiftsZ.Add(shiftZ);
                    if (randomX > 0) shiftsX.Add(-shiftX);
                    if (randomX < chunk.SpawnPositions.GetLength(1) - 1) shiftsX.Add(shiftX);
                    float randomShiftZ = shiftsZ[Random.Range(0, shiftsZ.Count)];
                    float randomShiftX = shiftsX[Random.Range(0, shiftsX.Count)];

                    randomBook.transform.position = new Vector3(randomPosition.x + randomShiftX, randomBook.transform.position.y, randomPosition.z + randomShiftZ);
                    chunk.ContainedBook = randomBook;
                    randomBook.OriginChunk = chunk;
                    _mazeGenerator.DestoyWallsInRange(randomBook.transform.position, 0.5f);
                }
            }
        }

        
    }

    void SetRandomBooksToGhosts()
    {
        List<Book> books = new List<Book>(_books);
        int defaultBooksCount = books.Count;
        List<EnemyAI> ghosts = new List<EnemyAI>();
        foreach (UnitInfo unit in _units)
        {
            if (!unit.isPlayer)
            {
                ghosts.Add(unit.GetComponent<EnemyAI>());
            }
        }

        foreach (EnemyAI ghost in ghosts)
        {
            UnitInfo ghostInfo = ghost.GetComponent<UnitInfo>();
            if (ghostInfo.haveBook)
            {
                if (books.Count > 0)
                {
                    List<Book> tempBooks = new List<Book>(books);
                    while (tempBooks.Count > 0)
                    {
                        Book randomBook = tempBooks[Random.Range(0, tempBooks.Count)];
                        tempBooks.Remove(randomBook);
                        if (randomBook != ghostInfo.Books[0] && randomBook.Keeper == null)
                        {
                            if (books.Count == defaultBooksCount || books.Count == 1)
                            {
                                ghost.BookToDefend = randomBook.transform;
                                randomBook.Keeper = ghostInfo;
                                books.Remove(randomBook);
                                break;
                            }
                            else if ((books.Count < defaultBooksCount && books.Count > 1) 
                                && (randomBook.Owner.GetComponent<EnemyAI>().BookToDefend == null 
                                || randomBook.Owner.GetComponent<EnemyAI>().BookToDefend != ghostInfo.Books[0].transform)) //проверка, что бы 2 призрака не обменялись книгами и не оставили 3го с его собственной
                            {
                                ghost.BookToDefend = randomBook.transform;
                                randomBook.Keeper = ghostInfo;
                                books.Remove(randomBook);
                                break;
                            }
                        }
                    }
                }
            }
        }
    }

    IEnumerator ActivateAfterSeconds(GameObject obj, float sec)
    {
        yield return new WaitForSeconds(sec);
        obj.SetActive(true);
    }
    

    struct ElementIndexes
    {
        public int Z;
        public int X;

        public ElementIndexes (int z, int x)
        {
            Z = z;
            X = x;
        }
    }
}
