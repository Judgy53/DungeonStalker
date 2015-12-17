using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Maze : MonoBehaviour
{
    public Vector2i size = new Vector2i(20, 20);

    public MazeCell cellPrefab = null;
    public GameObject roofPrefab = null;
    public MazePassage passagePrefab = null;
    public MazeWall[] wallPrefabs = null;
    public GameObject straightWallDecorPrefab = null;
    public GameObject closedTurnWallDecorPrefab = null;
    public GameObject openTurnWallDecorPrefab = null;

    public MazeDoor doorPrefab = null;

    [Range(0.0f, 1.0f)]
    public float doorProbablity = 0.1f;

    public bool generateRoof = true;

    public GameObject playerStartPrefab = null;

    public GameObject[] enemiesPrefabs = null;

    public Container chestPrefab = null;

    public MazeRoomSettings[] roomSettings = new MazeRoomSettings[0];

    private MazeCell[,] cells = new MazeCell[0, 0];
    public MazeCell[,] Cells { get { return cells; } }

    private List<MazeRoom> rooms = new List<MazeRoom>();
    public List<MazeRoom> Rooms { get { return rooms; } }

    private List<GameObject> enemies = new List<GameObject>();
    public List<GameObject> Enemies { get { return enemies; } }

    private List<Container> chests = new List<Container>();
    public List<Container> Chests { get { return chests; } }

    private bool playerStartCreated = false;

    public MazeCell playerStartCell = null;

    private int seed = -1;
    public int Seed { get { return seed; } set { seed = value; } }

    public DoorChangeStage exitDoorPrefab = null;

    public Vector2i RandomCoordinates
    {
        get
        {
            return new Vector2i(Random.Range(0, size.x), Random.Range(0, size.z));
        }
    }

    public void Clear()
    {
        foreach (var cell in cells)
        {
            if (cell != null)
                GameObject.Destroy(cell.gameObject);
        }

        foreach (var e in enemies)
        {
            if (e != null)
                Destroy(e);
        }

        enemies.Clear();

        cells = new MazeCell[0, 0];
    }

    public IEnumerator Generate()
    {
        if (seed == -1)
            seed = Random.Range(0, 1000000);

        Random.seed = seed;

        cells = new MazeCell[size.x, size.z];
        List<MazeCell> activeCells = new List<MazeCell>();
        DoFirstGenerationStep(activeCells);
        while (activeCells.Count > 0)
        {
            if (activeCells.Count % 100 == 0)
                yield return false;
            DoNextGenerationStep(activeCells);
        }

        yield return StartCoroutine(PlaceWallDecors());

        PlaceExitDoor();
    }

    public IEnumerator Populate(int enemiesNumber, int maxEnemiesPerRoom)
    {
        enemies = new List<GameObject>();
        List<MazeRoom> activeRooms = new List<MazeRoom>(rooms);

        if (playerStartCell != null)
            activeRooms.Remove(playerStartCell.room);

        while (enemies.Count < enemiesNumber)
        {
            if (activeRooms.Count == 0)
                break;

            MazeRoom randRoom = activeRooms[Random.Range(0, activeRooms.Count)];
            activeRooms.Remove(randRoom);

            int randNumber = Random.Range(1, maxEnemiesPerRoom + 1);
            yield return StartCoroutine(GenerateEnemiesInRoom(randRoom, randNumber));
        }

        foreach (var enemy in enemies)
        {
            StatsManager stm = enemy.GetComponent<StatsManager>();
            if (stm != null)
                stm.GenerateRandomStats(checked((uint)Random.Range(Mathf.Max(1u, GameManager.Stage - 1u), Mathf.Max(2u, GameManager.Stage + 1u))));
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
            if (randCell.IsFree)
            {
                randCell.RegisterEntity();

                GameObject enemy = GameObject.Instantiate(enemiesPrefabs[Random.Range(0, enemiesPrefabs.Length)]) as GameObject;
                enemy.transform.parent = transform;
                enemy.transform.localPosition = new Vector3(randCell.coordinates.x - size.x * 0.5f + 0.5f, transform.position.y + 1.0f, randCell.coordinates.z - size.z * 0.5f + 0.5f);
                enemiesGenerated++;

                enemies.Add(enemy);
            }
        }
    }

    public IEnumerator GenerateChests(int chestNumber, int maxChestNumberPerRoom)
    {
        chests = new List<Container>();
        List<MazeRoom> activeRooms = new List<MazeRoom>(rooms);

        while (chests.Count < chestNumber)
        {
            if (activeRooms.Count == 0)
                break;

            MazeRoom randRoom = activeRooms[Random.Range(0, activeRooms.Count)];
            activeRooms.Remove(randRoom);

            int randNumber = Random.Range(1, maxChestNumberPerRoom + 1);

            yield return StartCoroutine(GenerateChestsInRoom(randRoom, randNumber));
        }
    }

    private IEnumerator GenerateChestsInRoom(MazeRoom room, int number)
    {
        int chestsGenerated = 0;
        List<MazeCell> activeCells = new List<MazeCell>(room.Cells);

        while (chestsGenerated < number)
        {
            if (enemies.Count % 10 == 0)
                yield return false;

            if (activeCells.Count == 0)
                break;

            MazeCell randCell = activeCells[Random.Range(0, activeCells.Count)];
            activeCells.Remove(randCell);
            if (randCell.IsFree)
            {
                int passageCount = 0;
                for (int i = 0; i < 4; i++)
                {
                    MazeCellEdge edge = randCell.GetEdge(i);
                    if (edge is MazePassage)
                    {
                        passageCount++;
                        if (passageCount > 1)
                            break;
                    }
                }

                if (passageCount > 1)
                    continue;

                randCell.RegisterEntity();

                Container chest = GameObject.Instantiate(chestPrefab) as Container;
                chest.transform.parent = transform;

                MeshRenderer renderer = chest.GetComponent<MeshRenderer>();
                if (renderer != null)
                    chest.transform.localPosition = new Vector3(randCell.coordinates.x - size.x * 0.5f + 0.5f, 
                        transform.position.y + renderer.bounds.extents.y, 
                        randCell.coordinates.z - size.z * 0.5f + 0.5f);
                else
                    chest.transform.localPosition = new Vector3(randCell.coordinates.x - size.x * 0.5f + 0.5f, 
                        transform.position.y + 1.0f, 
                        randCell.coordinates.z - size.z * 0.5f + 0.5f);

                chestsGenerated++;

                chests.Add(chest);
            }
        }
    }

    private void DoFirstGenerationStep(List<MazeCell> activeCells)
    {
        MazeCell newCell = CreateCell(RandomCoordinates);
        newCell.Initialize(CreateRoom(-1));
        activeCells.Add(newCell);
        playerStartCreated = false;
        playerStartCell = null;
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
                //Todo : Create non usable door, Showing : 'I can't go back !'

                /*if (GameManager.Stage != 1)
                    CreateChangeStageDoor(currentCell, null, direction, 0);
                else*/
                CreateWall(currentCell, null, direction);

                GameObject start = GameObject.Instantiate(playerStartPrefab) as GameObject;
                start.transform.parent = transform;

                start.transform.localPosition = new Vector3(
                    coordinates.x - size.x * 0.5f + 0.5f - direction.ToVector2i().x,
                    transform.position.y + start.transform.localPosition.y, 
                    coordinates.z - size.z * 0.5f + 0.5f - direction.ToVector2i().z);

                playerStartCreated = true;
                playerStartCell = currentCell;
                playerStartCell.RegisterEntity();
            }
        }
    }

    private void PlaceExitDoor()
    {
        while (true)
        {
            MazeCell rcell = cells[Random.Range(0, size.x), Random.Range(0, size.z)];
            for (int i = 0; i < MazeDirections.Count; i++)
            {
                MazeDirection dir = MazeDirection.North + i;
                MazeCellEdge edge = rcell.GetEdge(dir);
                if (edge is MazeWall)
                {
                    rcell.DestroyEdge(dir);
                    Vector2i coordinates = rcell.coordinates + dir.ToVector2i();
                    if (ContainsCoordinates(coordinates))
                        CreateChangeStageDoor(rcell, GetCell(coordinates), dir, 1);
                    else
                        CreateChangeStageDoor(rcell, null, dir, 1);

                    return;
                }
            }
        }
    }

    private IEnumerator PlaceWallDecors()
    {
        int checkedCellCount = 0;
        foreach (MazeCell cell in cells)
        {
            if (checkedCellCount % 100 == 0)
                yield return null;

            for (MazeDirection dir = 0; (int)dir < MazeDirections.Count; dir++)
            {
                if (!(cell.GetEdge(dir) is MazeWall))
                    continue;

                CheckWallside(cell, dir, 1);
                CheckWallside(cell, dir, -1);
            }
            checkedCellCount++;
        }

        Debug.Log("Finished placing decor !");
    }

    private void CheckWallside(MazeCell cell, MazeDirection dir, int delta)
    {
        if (cell.GetEdge(dir + delta) is MazeWall)
        {
            if (closedTurnWallDecorPrefab != null && !cell.IsCornerInitialized(dir, delta))
                cell.InitializeCorner(closedTurnWallDecorPrefab, dir, delta);
            return;
        }

        if (ContainsCoordinates(cell.coordinates + (dir + delta).ToVector2i() + dir.ToVector2i()))
        {
            MazeCell otherCell = GetCell(cell.coordinates + (dir + delta).ToVector2i() + dir.ToVector2i());
            if (otherCell.GetEdge(dir - delta) is MazeWall)
            {
                if (openTurnWallDecorPrefab != null)
                {
                    if (!cell.IsCornerInitialized(dir, delta))
                        cell.InitializeCorner(openTurnWallDecorPrefab, dir, delta);

                    if (ContainsCoordinates(cell.coordinates + (dir + delta).ToVector2i()))
                    {
                        MazeCell sideCell = GetCell(cell.coordinates + (dir + delta).ToVector2i());
                        if (!sideCell.IsCornerInitialized(dir, -delta))
                            sideCell.InitializeCorner(openTurnWallDecorPrefab, dir, -delta);
                    }
                }
                return;
            }
        }

        if (ContainsCoordinates(cell.coordinates + (dir + delta).ToVector2i()))
        {
            MazeCell otherCell = GetCell(cell.coordinates + (dir + delta).ToVector2i());
            if (otherCell.GetEdge(dir) is MazeWall)
            {
                if (straightWallDecorPrefab != null && !cell.IsCornerInitialized(dir, delta))
                    cell.InitializeCorner(straightWallDecorPrefab, dir, delta);
                return;
            }
        }
    }

    public void CreateChangeStageDoor(MazeCell cell, MazeCell otherCell, MazeDirection direction, uint delta)
    {
        DoorChangeStage newDoor = GameObject.Instantiate(exitDoorPrefab as Behaviour, transform.position, transform.rotation) as DoorChangeStage;
        newDoor.Delta = delta;

        newDoor.Initialize(cell, otherCell, direction);
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
        //newCell.transform.parent = transform;
        newCell.transform.SetParent(transform, false);
        newCell.transform.localPosition = new Vector3(coordinates.x - size.x * 0.5f + 0.5f, 0.0f, coordinates.z - size.z * 0.5f + 0.5f);

        if (generateRoof)
        {
            GameObject roof = GameObject.Instantiate(roofPrefab, transform.position, transform.rotation) as GameObject;
            roof.transform.SetParent(newCell.transform, false);
        }

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

    public void Save(SaveData data)
    {
        int count = 0;
        data.Add("enemyCount", enemies.Count);

        foreach(GameObject enemy in enemies)
        {
            if (enemy == null)
                continue;

            ISavable[] savables = enemy.GetComponents<ISavable>();

            if (savables.Length == 0)
                continue;

            string prefix = "enemy_" + count++ + "_";
            data.Prefix = prefix;

            data.Add("prefab_path", ResourcesPathHelper.GetEnemyPath(enemy.name));

            foreach(ISavable sav in savables)
            {
                sav.Save(data);
                data.Prefix = prefix; //reset prefix after each save
            }
        }

        data.Prefix = "";
    }

    public IEnumerator Load(SaveData data)
    {
        foreach (var e in enemies)
        {
            if (e != null)
                Destroy(e);
        }
        enemies.Clear();

        int count = int.Parse(data.Get("enemyCount"));

        for (int i = 0; i < count; i++)
        {
            if(i % 10 == 0)
                yield return null;

            string prefix = "enemy_" + i + "_";
            data.Prefix = prefix;

            string prefab_path = data.Get("prefab_path");

            if (prefab_path == null)
                continue;

            GameObject prefab = Resources.Load<GameObject>(prefab_path);

            if (prefab == null)
            {
                Debug.LogError("Enemy Prefab not found (" + prefab_path + ")");
                continue;
            }

            GameObject enemy = Instantiate(prefab) as GameObject;

            ISavable[] savables = enemy.GetComponents<ISavable>();

            if (savables.Length == 0)
                continue;

            foreach (ISavable sav in savables)
            {
                sav.Load(data);
                data.Prefix = prefix; // reset prefix after each load
            }

            enemies.Add(enemy);
        }
    }
}


public struct MazeDirection
{
    private int InternalValue { get; set; }

    public static readonly int North = 0;
    public static readonly int East = 1;
    public static readonly int South = 2;
    public static readonly int West = 3;

    public static implicit operator MazeDirection(int other)
    {
        return new MazeDirection
        {
            InternalValue = other
        };
    }

    public static implicit operator int(MazeDirection other)
    {
        return other.InternalValue;
    }

    public static MazeDirection operator +(MazeDirection dir, int i)
    {
        if (i < 0)
            return dir - -i;

        if ((int)dir + i > MazeDirections.Count - 1)
            return (int)i + (int)dir - MazeDirections.Count;

        return (int)dir + i;
    }

    public static MazeDirection operator -(MazeDirection dir, int i)
    {
        if (i < 0)
            return dir + -i;

        if ((int)dir - i < 0)
            return (int)dir - i + MazeDirections.Count;

        return (int)dir - i;
    }

    public static MazeDirection operator ++(MazeDirection dir)
    {
        return (int)dir + 1;
    }

    public static MazeDirection operator --(MazeDirection dir)
    {
        return (int)dir - 1;
    }
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
