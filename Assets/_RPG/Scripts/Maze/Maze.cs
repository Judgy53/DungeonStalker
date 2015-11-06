using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Maze : MonoBehaviour
{
    public Vector2i size = new Vector2i(20, 20);

    public MazeCell cellPrefab = null;
    public MazePassage passagePrefab = null;
    public MazeWall[] wallPrefabs = null;

    public MazeDoor doorPrefab = null;

    [Range(0.0f, 1.0f)]
    public float doorProbablity = 0.1f;

    public GameObject playerStartPrefab = null;

    public GameObject[] enemiesPrefabs = null;

    public MazeRoomSettings[] roomSettings = new MazeRoomSettings[0];

    private MazeCell[,] cells = new MazeCell[0, 0];
    private List<MazeRoom> rooms = new List<MazeRoom>();

    private List<GameObject> enemies = new List<GameObject>();

    private bool playerStartCreated = false;

    public Vector2i RandomCoordinates
    {
        get
        {
            return new Vector2i(Random.Range(0, size.x), Random.Range(0, size.z));
        }
    }

    public void Regenerate()
    {
        StopAllCoroutines();

        foreach (var cell in cells)
        {
            if (cell != null)
                GameObject.Destroy(cell.gameObject);
        }

        foreach (var e in enemies)
        {
            if (e != null)
                GameObject.Destroy(e);
        }

        cells = new MazeCell[0, 0];

        StartCoroutine(Generate());
    }

    public IEnumerator Generate()
    {
        cells = new MazeCell[size.x, size.z];
        List<MazeCell> activeCells = new List<MazeCell>();
        DoFirstGenerationStep(activeCells);
        while (activeCells.Count > 0)
        {
            if (activeCells.Count % 100 == 0)
                yield return false;
            DoNextGenerationStep(activeCells);
        }
    }

    public IEnumerator Populate(int enemiesNumber, int maxEnemiesPerRoom)
    {
        enemies = new List<GameObject>();
        while (enemies.Count < enemiesNumber)
        {
            MazeRoom randRoom = rooms[Random.Range(0, rooms.Count)];
            int randNumber = Random.Range(1, maxEnemiesPerRoom + 1);
            yield return StartCoroutine(GenerateEnemiesInRoom(randRoom, randNumber));
        }
    }

    private IEnumerator GenerateEnemiesInRoom(MazeRoom room, int number)
    {
        int enemiesGenerated = 0;
        List<MazeCell> activeCells = new List<MazeCell>(room.Cells);

        while (enemiesGenerated < number)
        {
            if (enemies.Count % 10 == 0)
                yield return false;

            if (activeCells.Count == 0)
                break;

            MazeCell randCell = activeCells[Random.Range(0, activeCells.Count)];
            activeCells.Remove(randCell);

            GameObject enemy = GameObject.Instantiate(enemiesPrefabs[Random.Range(0, enemiesPrefabs.Length)]) as GameObject;
            enemy.transform.parent = transform;
            enemy.transform.localPosition = new Vector3(randCell.coordinates.x - size.x * 0.5f + 0.5f, transform.position.y + 1.0f, randCell.coordinates.z - size.z * 0.5f + 0.5f);
            enemiesGenerated++;

            enemies.Add(enemy);
        }
    }

    private void DoFirstGenerationStep(List<MazeCell> activeCells)
    {
        MazeCell newCell = CreateCell(RandomCoordinates);
        newCell.Initialize(CreateRoom(-1));
        activeCells.Add(newCell);
        playerStartCreated = false;
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
        Vector2i coordinates = currentCell.coordinates + direction.ToVector2i();

        if (ContainsCoordinates(coordinates))
        {
            MazeCell neighbor = GetCell(coordinates);
            if (neighbor == null)
            {
                neighbor = CreateCell(coordinates);
                CreatePassage(currentCell, neighbor, direction);
                activeCells.Add(neighbor);
            }
            else if (currentCell.room.settingsIndex == neighbor.room.settingsIndex)
                CreatePassageInSameRoom(currentCell, neighbor, direction);
            else
                CreateWall(currentCell, neighbor, direction);
        }
        else
        {
            if (playerStartCreated)
                CreateWall(currentCell, null, direction);
            else
            {
                CreateWall(currentCell, null, direction);
                //CreateReloadDoor();
                GameObject start = GameObject.Instantiate(playerStartPrefab) as GameObject;
                start.transform.parent = transform;

                start.transform.localPosition = new Vector3(
                    coordinates.x - size.x * 0.5f + 0.5f - direction.ToVector2i().x,
                    transform.position.y + start.transform.localPosition.y, 
                    coordinates.z - size.z * 0.5f + 0.5f - direction.ToVector2i().z);

                playerStartCreated = true;
            }
        }
    }

    public bool ContainsCoordinates(Vector2i coordinate)
    {
        return coordinate.x >= 0 && coordinate.x < size.x && coordinate.z >= 0 && coordinate.z < size.z;
    }

    public MazeCell GetCell(Vector2i coordinates)
    {
        return cells[coordinates.x, coordinates.z];
    }

    private MazeCell CreateCell(Vector2i coordinates)
    {
        MazeCell newCell = GameObject.Instantiate(cellPrefab, transform.position, transform.rotation) as MazeCell;
        cells[coordinates.x, coordinates.z] = newCell;
        newCell.coordinates = coordinates;
        newCell.name = "Maze Cell " + coordinates.x + ", " + coordinates.z;
        newCell.transform.parent = transform;
        newCell.transform.localPosition = new Vector3(coordinates.x - size.x * 0.5f + 0.5f, 0.0f, coordinates.z - size.z * 0.5f + 0.5f);

        return newCell;
    }

    private void CreatePassageInSameRoom(MazeCell cell, MazeCell otherCell, MazeDirection direction)
    {
        MazePassage passage = GameObject.Instantiate(passagePrefab) as MazePassage;
        passage.Initialize(cell, otherCell, direction);
        passage = GameObject.Instantiate(passagePrefab) as MazePassage;
        passage.Initialize(otherCell, cell, direction.GetOpposite());

        if (cell.room != otherCell.room)
        {
            MazeRoom roomToAssimilate = otherCell.room;
            cell.room.Assimilate(roomToAssimilate);
            rooms.Remove(roomToAssimilate);
            Destroy(roomToAssimilate);
        }
    }

    private void CreatePassage(MazeCell cell, MazeCell otherCell, MazeDirection direction)
    {
        MazePassage prefab = Random.value < doorProbablity ? doorPrefab : passagePrefab;
        MazePassage passage = GameObject.Instantiate(prefab) as MazePassage;
        passage.Initialize(cell, otherCell, direction);
        passage = GameObject.Instantiate(prefab) as MazePassage;
        if (passage is MazeDoor)
            otherCell.Initialize(CreateRoom(cell.room.settingsIndex));
        else
            otherCell.Initialize(cell.room);
        passage.Initialize(otherCell, cell, direction.GetOpposite());
    }

    private void CreateWall(MazeCell cell, MazeCell otherCell, MazeDirection direction)
    {
        MazeWall wall = Instantiate(wallPrefabs[Random.Range(0, wallPrefabs.Length)]) as MazeWall;
        wall.Initialize(cell, otherCell, direction);
        if (otherCell != null)
        {
            wall = Instantiate(wallPrefabs[Random.Range(0, wallPrefabs.Length)]) as MazeWall;
            wall.Initialize(otherCell, cell, direction.GetOpposite());
        }
    }

    private MazeRoom CreateRoom(int indexToExclude)
    {
        MazeRoom newRoom = ScriptableObject.CreateInstance<MazeRoom>();
        
        newRoom.settingsIndex = Random.Range(0, roomSettings.Length);
        if (newRoom.settingsIndex == indexToExclude)
            newRoom.settingsIndex = (newRoom.settingsIndex + 1) % roomSettings.Length;
        newRoom.settings = roomSettings[newRoom.settingsIndex];
        
        rooms.Add(newRoom);

        return newRoom;
    }
}


public enum MazeDirection
{
    North,
    East,
    South,
    West
}

public static class MazeDirections
{
    public const int Count = 4;

    private static Vector2i[] vectors = 
    {
        new Vector2i(0, 1),
        new Vector2i(1, 0),
        new Vector2i(0, -1),
        new Vector2i(-1, 0)
    };

    private static MazeDirection[] opposites =
    {
        MazeDirection.South,
        MazeDirection.West,
        MazeDirection.North,
        MazeDirection.East
    };

    private static Quaternion[] rotations = 
    {
        Quaternion.identity,
        Quaternion.Euler(0.0f, 90.0f, 0.0f),
        Quaternion.Euler(0.0f, 180.0f, 0.0f),
        Quaternion.Euler(0.0f, 270.0f, 0.0f)
    };

    public static MazeDirection RandomValue
    {
        get
        {
            return (MazeDirection)Random.Range(0, Count);
        }
    }

    public static Vector2i ToVector2i(this MazeDirection dir)
    {
        return vectors[(int)dir];
    }

    public static MazeDirection GetOpposite(this MazeDirection direction)
    {
        return opposites[(int)direction];
    }

    public static Quaternion ToRotation(this MazeDirection direction)
    {
        return rotations[(int)direction];
    }
}
