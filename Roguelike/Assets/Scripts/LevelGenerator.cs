using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LevelGenerator : MonoBehaviour
{
    public static float LVL = 1;
    public enum gridSpace { empty, floor, wall, l, u, r, d, luc, ldc, ruc, rdc, rd, ru, ld, lu };//валк - просто стена, остально перечисление
    //стороны стены которая касается с полом, если перед стоит n то это значит стенка, которая не касается пола в этой позиции
    public gridSpace[,] grid;
    int roomHeight, roomWidth;
    Vector2 roomSizeWorldUnits;
    float worldUnitsInOneGridCell = 1;
        
    enum Mobs { mob1, boss1, boss2, boss3 };
    public GameObject portal, mob1, boss1, boss2, boss3;
    private GameObject player;

    public float distance;
    public int MobCountOnLvl;
    int floorCount;
    int mobCount;
    int floorNumber;
    public int[,] floorForSpawnMob;
    //для теста сколько полов
    int floorC = 0;
    struct walker
    {
        public Vector2 dir;
        public Vector2 pos;
    }
    List<walker> walkers;
    float chanceWalkerChangeDir = 0.5f, chanceWalkerSpawn = 0.05f;
    float chanceWalkerDestoy = 0.05f;
    int maxWalkers = 10;
    float percentToFill = 0.2f;
    public GameObject floor1, floor2, floor3, wall, l, u, r, d, luc, ldc, ruc, rdc, rd, ru, ld, lu, menorahL, menorahU, menorahR, menorahD;

    NormalGenerationCheck normalGenerationCheck;
    //вспомогательные массивы(словари)
    public int[,] LwallForMenorah;
    public int[,] UwallForMenorah;
    public int[,] RwallForMenorah;
    public int[,] DwallForMenorah;
    public float MenorahSpawnRange;
    public LayerMask whatIsMenorah;

    public AudioClip[] clips;
    AudioSource audioSource;
    public void Start()
    {

        Setup();
        CreateFloors();
        CreateWalls();
        SpawnLevel();
        
        normalGenerationCheck = GameObject.FindGameObjectWithTag("levelGenerator").GetComponent<NormalGenerationCheck>();
        if(!NormalGeneration()){
            Restarter r = new Restarter();
            r.Restart();
        }
        SpawnMob();
        SpawnMenorah();
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = clips[0];
        print(audioSource.clip);
        audioSource.Play();
    }
    void Setup()
    {
        floorC = 0;
        float hw = 13;
        if (LVL % 3 != 0)
        {
            hw = 12 + LVL * 2;
            if (hw > 20)
                hw = 20;
        }
        else
        {
            hw = 15;
        }
        roomSizeWorldUnits = new Vector2(hw, hw);
        //find grid size
        roomHeight = Mathf.RoundToInt(roomSizeWorldUnits.x / worldUnitsInOneGridCell);
        roomWidth = Mathf.RoundToInt(roomSizeWorldUnits.y / worldUnitsInOneGridCell);
        //create grid
        grid = new gridSpace[roomWidth, roomHeight];
        //set grid's default state
        for (int x = 0; x < roomWidth - 1; x++)
        {
            for (int y = 0; y < roomHeight - 1; y++)
            {
                //make every cell "empty"
                grid[x, y] = gridSpace.empty;
            }
        }
        //set first walker
        //init list
        walkers = new List<walker>();
        //create a walker 
        walker newWalker = new walker();
        newWalker.dir = RandomDirection();
        //find center of grid
        Vector2 spawnPos = new Vector2(Mathf.RoundToInt(roomWidth / 2.0f),
                                        Mathf.RoundToInt(roomHeight / 2.0f));
        newWalker.pos = spawnPos;
        //add walker to list
        walkers.Add(newWalker);
    }
    void CreateFloors()
    {
        if (LVL % 3 != 0)
        {
            int iterations = 0;//loop will not run forever
            do
            {
                //create floor at position of every walker
                foreach (walker myWalker in walkers)
                {
                    grid[(int)myWalker.pos.x, (int)myWalker.pos.y] = gridSpace.floor;
                }
                //chance: destroy walker
                int numberChecks = walkers.Count; //might modify count while in this loop
                for (int i = 0; i < numberChecks; i++)
                {
                    //only if its not the only one, and at a low chance
                    if (Random.value < chanceWalkerDestoy && walkers.Count > 1)
                    {
                        walkers.RemoveAt(i);
                        break; //only destroy one per iteration
                    }
                }
                //chance: walker pick new direction
                for (int i = 0; i < walkers.Count; i++)
                {
                    if (Random.value < chanceWalkerChangeDir)
                    {
                        walker thisWalker = walkers[i];
                        thisWalker.dir = RandomDirection();
                        walkers[i] = thisWalker;
                    }
                }
                //chance: spawn new walker
                numberChecks = walkers.Count; //might modify count while in this loop
                for (int i = 0; i < numberChecks; i++)
                {
                    //only if # of walkers < max, and at a low chance
                    if (Random.value < chanceWalkerSpawn && walkers.Count < maxWalkers)
                    {
                        //create a walker 
                        walker newWalker = new walker();
                        newWalker.dir = RandomDirection();
                        newWalker.pos = walkers[i].pos;
                        walkers.Add(newWalker);
                    }
                }
                //move walkers
                for (int i = 0; i < walkers.Count; i++)
                {
                    walker thisWalker = walkers[i];
                    thisWalker.pos += thisWalker.dir;
                    walkers[i] = thisWalker;
                }
                //avoid boarder of grid
                for (int i = 0; i < walkers.Count; i++)
                {
                    walker thisWalker = walkers[i];
                    //clamp x,y to leave a 1 space boarder: leave room for walls
                    thisWalker.pos.x = Mathf.Clamp(thisWalker.pos.x, 1, roomWidth - 2);
                    thisWalker.pos.y = Mathf.Clamp(thisWalker.pos.y, 1, roomHeight - 2);
                    walkers[i] = thisWalker;
                }
                //check to exit loop
                if ((float)NumberOfFloors() / (float)grid.Length > percentToFill)
                {
                    break;
                }
                iterations++;
            } while (iterations < 100000);
        }
        else
        {
            for (int x = 1; x < roomWidth - 1; x++)
            {
                for (int y = 1; y < roomHeight - 1; y++)
                {
                    grid[x, y] = gridSpace.floor;
                }
            }
            grid[2, 12] = gridSpace.empty;
            grid[12, 12] = gridSpace.empty;
            grid[12, 2] = gridSpace.empty;
            grid[2, 2] = gridSpace.empty;
        }
    }
    void CreateWalls()
    {
        //левая стенка
        for (int x = 0, y = 0; y < roomWidth; y++)
        {
            grid[x, y] = gridSpace.empty;
        }
        for (int x = 1, y = 0; y < roomWidth; y++)
        {
            grid[x, y] = gridSpace.empty;
        }
        //нижнее
        for (int x = 0, y = 0; x < roomWidth; x++)
        {
            grid[x, y] = gridSpace.empty;
        }
        for (int x = 0, y = 1; x < roomWidth; x++)
        {
            grid[x, y] = gridSpace.empty;
        }
        //верх
        for (int x = 0, y = roomHeight - 1; x < roomWidth; x++)
        {
            grid[x, y] = gridSpace.empty;
        }
        for (int x = 0, y = roomHeight - 2; x < roomWidth; x++)
        {
            grid[x, y] = gridSpace.empty;
        }
        //низ
        for (int x = roomWidth - 1, y = 0; y < roomHeight; y++)
        {
            grid[x, y] = gridSpace.empty;
        }
        for (int x = roomWidth - 2, y = 0; y < roomHeight; y++)
        {
            grid[x, y] = gridSpace.empty;
        }
        //for (int x = 0, y = 0; x < roomWidth - 1; x++)
        //{
        //    grid[x, y + 1] = gridSpace.empty;
        //}
        //for (int x = 0, y = 0; x < roomWidth - 1; x++)
        //{
        //    grid[x, y + 1] = gridSpace.empty;
        //}

        //loop though every grid space
        for (int x = 1; x < roomWidth - 1; x++)
        {
            for (int y = 1; y < roomHeight - 1; y++)
            {
                //if theres a floor, check the spaces around it
                if (grid[x, y] == gridSpace.floor)
                {
                    //стены лево\право, верх\низ
                    if (grid[x, y + 1] == gridSpace.empty)
                    {
                        grid[x, y + 1] = gridSpace.wall;
                    }
                    if (grid[x, y - 1] == gridSpace.empty)
                    {
                        grid[x, y - 1] = gridSpace.wall;
                    }
                    if (grid[x + 1, y] == gridSpace.empty)
                    {
                        grid[x + 1, y] = gridSpace.wall;
                    }
                    if (grid[x - 1, y] == gridSpace.empty)
                    {
                        grid[x - 1, y] = gridSpace.wall;
                    }
                }
            }

        }

        for (int x = 1; x < roomWidth - 1; x++)
        {
            for (int y = 1; y < roomHeight - 1; y++)
            {
                //if theres a floor, check the spaces around it
                if (grid[x, y] == gridSpace.floor)
                {

                    // углы
                    if (grid[x - 1, y + 1] == gridSpace.empty)
                    {
                        grid[x - 1, y + 1] = gridSpace.wall;
                    }
                    if (grid[x - 1, y - 1] == gridSpace.empty)
                    {
                        grid[x - 1, y - 1] = gridSpace.wall;
                    }
                    if (grid[x + 1, y + 1] == gridSpace.empty)
                    {
                        grid[x + 1, y + 1] = gridSpace.wall;
                    }
                    if (grid[x + 1, y - 1] == gridSpace.empty)
                    {
                        grid[x + 1, y - 1] = gridSpace.wall;
                    }
                }
            }
        }
        //убираем внутренние стенки
        for (int x = 1; x < roomWidth - 1; x++)
        {
            for (int y = 1; y < roomHeight - 1; y++)
            {
                //if theres a floor, check the spaces around it
                if (grid[x, y] == gridSpace.wall)
                {
                    if (grid[x - 1, y + 1] == gridSpace.empty || grid[x, y + 1] == gridSpace.empty ||
                        grid[x + 1, y + 1] == gridSpace.empty || grid[x + 1, y] == gridSpace.empty ||
                        grid[x + 1, y - 1] == gridSpace.empty || grid[x, y - 1] == gridSpace.empty ||
                        grid[x - 1, y - 1] == gridSpace.empty || grid[x - 1, y] == gridSpace.empty)
                    {

                    }
                    else
                    {
                        grid[x, y] = gridSpace.floor;
                    }
                }
            }
        }

                    for (int x = 1; x < roomWidth - 1; x++)
        {
            for (int y = 1; y < roomHeight - 1; y++)
            {
                //if theres a floor, check the spaces around it
                if (grid[x, y] == gridSpace.wall)
                {

                    // стороны буквы там де полы
                    if (grid[x - 1, y] != gridSpace.floor && grid[x + 1, y] != gridSpace.empty)
                    {
                        grid[x, y] = gridSpace.l;
                    }
                    //кастыль убрать
                    if (grid[x - 1, y] != gridSpace.empty && grid[x + 1, y] != gridSpace.floor)
                    {
                        grid[x, y] = gridSpace.r;
                    }
                    if (grid[x, y + 1] == gridSpace.floor && grid[x, y - 1] != gridSpace.floor)
                    {
                        grid[x, y] = gridSpace.d;
                    }
                    if (grid[x, y - 1] == gridSpace.floor && grid[x, y + 1] != gridSpace.floor)
                    {
                        grid[x, y] = gridSpace.u;
                    }
                    //наружные углы буквы - где пустота
                    if (grid[x + 1, y] != gridSpace.floor && grid[x + 1, y] != gridSpace.empty && grid[x - 1, y] == gridSpace.empty && grid[x, y + 1] == gridSpace.empty && grid[x, y - 1] != gridSpace.floor && grid[x, y - 1] != gridSpace.empty && grid[x + 1, y - 1] == gridSpace.floor)
                    {
                        grid[x, y] = gridSpace.luc;
                    }
                    if (grid[x + 1, y] == gridSpace.empty && grid[x, y + 1] == gridSpace.empty && grid[x - 1, y - 1] == gridSpace.floor && grid[x, y - 1] != gridSpace.empty && grid[x, y - 1] != gridSpace.floor && grid[x - 1, y] != gridSpace.empty && grid[x - 1, y] != gridSpace.floor)
                    {
                        grid[x, y] = gridSpace.ruc;
                    }
                    if (grid[x - 1, y] != gridSpace.floor && grid[x, y - 1] != gridSpace.floor && grid[x + 1, y + 1] == gridSpace.floor && grid[x, y + 1] != gridSpace.empty && grid[x, y + 1] != gridSpace.floor && grid[x + 1, y] != gridSpace.empty && grid[x + 1, y] != gridSpace.floor)
                    {
                        grid[x, y] = gridSpace.ldc;
                    }
                    //if (grid[x + 1, y] == gridSpace.empty && grid[x, y - 1] == gridSpace.empty && grid[x - 1, y + 1] == gridSpace.floor && grid[x, y + 1] != gridSpace.empty && grid[x, y + 1] != gridSpace.floor && grid[x - 1, y] != gridSpace.empty && grid[x - 1, y] != gridSpace.floor)
                    if (grid[x + 1, y] != gridSpace.floor && grid[x, y - 1] != gridSpace.floor && grid[x - 1, y + 1] == gridSpace.floor && grid[x, y + 1] != gridSpace.empty && grid[x, y + 1] != gridSpace.floor && grid[x - 1, y] != gridSpace.empty && grid[x - 1, y] != gridSpace.floor)
                    {
                        grid[x, y] = gridSpace.rdc;
                    }
                    ////лево\право верх\низ буквы там де полы
                    //if (grid[x - 1, y] == gridSpace.floor && grid[x + 1, y] == gridSpace.floor && grid[x, y + 1] != gridSpace.floor && grid[x, y + 1] != gridSpace.empty && grid[x, y - 1] != gridSpace.floor && grid[x, y - 1] != gridSpace.empty)
                    //{
                    //    grid[x, y] = gridSpace.lr;
                    //}
                    //if (grid[x, y + 1] == gridSpace.floor && grid[x, y - 1] == gridSpace.floor && grid[x - 1, y] != gridSpace.floor && grid[x - 1, y] != gridSpace.empty && grid[x + 1, y] != gridSpace.floor && grid[x + 1, y] != gridSpace.empty)
                    //{
                    //    grid[x, y] = gridSpace.ud;
                    //}
                    ////стенки, окруженные полами с 3 сторон буквы с тех сторон где пол
                    //if (grid[x + 1, y] == gridSpace.floor && grid[x - 1, y] == gridSpace.floor && grid[x, y - 1] == gridSpace.floor && grid[x, y + 1] != gridSpace.floor && grid[x, y + 1] != gridSpace.empty)
                    //{
                    //    grid[x, y] = gridSpace.ldr;
                    //}
                    //if (grid[x + 1, y] == gridSpace.floor && grid[x - 1, y] == gridSpace.floor && grid[x, y + 1] == gridSpace.floor && grid[x, y - 1] != gridSpace.floor && grid[x, y - 1] != gridSpace.empty)
                    //{
                    //    grid[x, y] = gridSpace.lur;
                    //}
                    //if (grid[x - 1, y] == gridSpace.floor && grid[x, y + 1] == gridSpace.floor && grid[x, y - 1] == gridSpace.floor && grid[x + 1, y] != gridSpace.floor && grid[x + 1, y] != gridSpace.empty)
                    //{
                    //    grid[x, y] = gridSpace.uld;
                    //}
                    //if (grid[x + 1, y] == gridSpace.floor && grid[x, y + 1] == gridSpace.floor && grid[x, y - 1] == gridSpace.floor && grid[x - 1, y] != gridSpace.floor && grid[x - 1, y] != gridSpace.empty)
                    //{
                    //    grid[x, y] = gridSpace.urd;
                    //}
                    //внутренние углы буквы - где полы
                    if (grid[x - 1, y] == gridSpace.floor && grid[x, y - 1] == gridSpace.floor && grid[x + 1, y] != gridSpace.empty && grid[x + 1, y] != gridSpace.floor && grid[x, y + 1] != gridSpace.empty && grid[x, y + 1] != gridSpace.floor)
                    {
                        grid[x, y] = gridSpace.ld;
                    }
                    if (grid[x + 1, y] == gridSpace.floor && grid[x, y - 1] == gridSpace.floor && grid[x - 1, y] != gridSpace.empty && grid[x - 1, y] != gridSpace.floor && grid[x, y + 1] != gridSpace.empty && grid[x, y + 1] != gridSpace.floor)
                    {
                        grid[x, y] = gridSpace.rd;
                    }
                    if (grid[x + 1, y] == gridSpace.floor && grid[x, y + 1] == gridSpace.floor && grid[x - 1, y] != gridSpace.empty && grid[x - 1, y] != gridSpace.floor && grid[x, y - 1] != gridSpace.empty && grid[x, y - 1] != gridSpace.floor)
                    {
                        grid[x, y] = gridSpace.ru;
                    }
                    if (grid[x, y + 1] == gridSpace.floor && grid[x - 1, y] == gridSpace.floor && grid[x + 1, y] != gridSpace.empty && grid[x + 1, y] != gridSpace.floor && grid[x, y - 1] != gridSpace.empty && grid[x, y - 1] != gridSpace.floor)
                    {
                        grid[x, y] = gridSpace.lu;
                    }
                    //// Т - образные стены n - не ,буква там, где стены
                    //if ((grid[x, y + 1] == gridSpace.floor || grid[x, y + 1] == gridSpace.empty) && grid[x - 1, y] != gridSpace.floor && grid[x - 1, y] != gridSpace.empty && grid[x, y - 1] != gridSpace.floor && grid[x, y - 1] != gridSpace.empty && grid[x + 1, y] != gridSpace.floor && grid[x + 1, y] != gridSpace.empty)
                    //{
                    //    grid[x, y] = gridSpace.nldr;
                    //}
                    //if ((grid[x, y - 1] == gridSpace.floor || grid[x, y - 1] == gridSpace.empty) && grid[x - 1, y] != gridSpace.floor && grid[x - 1, y] != gridSpace.empty && grid[x, y + 1] != gridSpace.floor && grid[x, y + 1] != gridSpace.empty && grid[x + 1, y] != gridSpace.floor && grid[x + 1, y] != gridSpace.empty)
                    //{
                    //    grid[x, y] = gridSpace.nlur;
                    //}
                    //if ((grid[x + 1, y] == gridSpace.floor || grid[x + 1, y] == gridSpace.empty) && grid[x - 1, y] != gridSpace.floor && grid[x - 1, y] != gridSpace.empty && grid[x, y - 1] != gridSpace.floor && grid[x, y - 1] != gridSpace.empty && grid[x, y + 1] != gridSpace.floor && grid[x, y + 1] != gridSpace.empty)
                    //{
                    //    grid[x, y] = gridSpace.nuld;
                    //}
                    //if ((grid[x - 1, y] == gridSpace.floor || grid[x - 1, y] == gridSpace.empty) && grid[x + 1, y] != gridSpace.floor && grid[x + 1, y] != gridSpace.empty && grid[x, y - 1] != gridSpace.floor && grid[x, y - 1] != gridSpace.empty && grid[x, y + 1] != gridSpace.floor && grid[x, y + 1] != gridSpace.empty)
                    //{
                    //    grid[x, y] = gridSpace.nurd;
                    //}

                }
            }
        }
    }

    //void RemoveSingleWalls()
    //{
    //    //loop though every grid space
    //    for (int x = 0; x < roomWidth - 1; x++)
    //    {
    //        for (int y = 0; y < roomHeight - 1; y++)
    //        {
    //            //if theres a wall, check the spaces around it
    //            if (grid[x, y] == gridSpace.wall)
    //            {
    //                //assume all space around wall are floors
    //                bool allFloors = true;
    //                //check each side to see if they are all floors
    //                for (int checkX = -1; checkX <= 1; checkX++)
    //                {
    //                    for (int checkY = -1; checkY <= 1; checkY++)
    //                    {
    //                        if (x + checkX < 0 || x + checkX > roomWidth - 1 ||
    //                            y + checkY < 0 || y + checkY > roomHeight - 1)
    //                        {
    //                            //skip checks that are out of range
    //                            continue;
    //                        }
    //                        if ((checkX != 0 && checkY != 0) || (checkX == 0 && checkY == 0))
    //                        {
    //                            //skip corners and center
    //                            continue;
    //                        }
    //                        if (grid[x + checkX, y + checkY] != gridSpace.floor)
    //                        {
    //                            allFloors = false;
    //                        }
    //                    }
    //                }
    //                if (allFloors)
    //                {
    //                    grid[x, y] = gridSpace.floor;
    //                }
    //            }
    //        }
    //    }
    //}
    void SpawnLevel()
    {
        int t = 0;
        int L = 0;
        int U = 0;
        int R = 0;
        int D = 0;
        int tLength = 0;
        int LLength = 0;
        int ULength = 0;
        int RLength = 0;
        int DLength = 0;
        for (int x = 0; x < roomWidth; x++)
        {
            for (int y = 0; y < roomHeight; y++)
            {
                switch (grid[x, y])
                {
                    case gridSpace.floor:
                        tLength++;
                        break;
                    case gridSpace.l:
                        LLength++;
                        break;
                    case gridSpace.u:
                        ULength++;
                        break;
                    case gridSpace.r:
                        RLength++;
                        break;
                    case gridSpace.d:
                        DLength++;
                        break;
                    case gridSpace.ld:
                        ULength++;
                        break;
                    case gridSpace.rd:
                        ULength++;
                        break;
                }
            }
        }
        floorForSpawnMob = new int[tLength, 2];
        LwallForMenorah = new int[LLength, 2];
        UwallForMenorah = new int[ULength, 2];
        RwallForMenorah = new int[RLength, 2];
        DwallForMenorah = new int[DLength, 2];

        for (int x = 0; x < roomWidth; x++)
        {
            for (int y = 0; y < roomHeight; y++)
            {
                switch (grid[x, y])
                {
                    //floor, wall, l, u, r, d, luc, ldc, ruc, rdc, lr, ud, ldr, lur, uld, urd, rd, ru, ld, lu, nldr, nlur, nuad, nurd, nlurd;
                    case gridSpace.empty:
                        break;
                    case gridSpace.floor:
                        float f = Random.Range(0f, 3f);
                        if (f < 1)
                        {
                            Spawn(x, y, floor1);
                        }
                        else if(f < 2)
                        {
                            Spawn(x, y, floor2);
                        }
                        else
                        {
                            Spawn(x, y, floor3);
                        }                      
                        floorForSpawnMob[t,0] = x;
                        floorForSpawnMob[t,1] = y;
                        floorC++;
                        t++;
                        break;

                    case gridSpace.wall:
                        Spawn(x, y, wall);
                        break;
                    case gridSpace.l:
                        LwallForMenorah[L, 0] = x;
                        LwallForMenorah[L, 1] = y;
                        L++;
                        Spawn(x, y, l);
                        break;
                    case gridSpace.u:
                        UwallForMenorah[U, 0] = x;
                        UwallForMenorah[U, 1] = y;
                        U++;
                        Spawn(x, y, u);
                        break;
                    case gridSpace.r:
                        RwallForMenorah[R, 0] = x;
                        RwallForMenorah[R, 1] = y;
                        R++;
                        Spawn(x, y, r);
                        break;
                    case gridSpace.d:
                        DwallForMenorah[D, 0] = x;
                        DwallForMenorah[D, 1] = y;
                        D++;
                        Spawn(x, y, d);
                        break;
                    case gridSpace.luc:
                        Spawn(x, y, luc);
                        break;
                    case gridSpace.ldc:
                        Spawn(x, y, ldc);
                        break;
                    case gridSpace.ruc:
                        Spawn(x, y, ruc);
                        break;
                    case gridSpace.rdc:
                        Spawn(x, y, rdc);
                        break;
                    case gridSpace.ld:
                        UwallForMenorah[U, 0] = x;
                        UwallForMenorah[U, 1] = y;
                        U++;
                        Spawn(x, y, ld);
                        break;
                    case gridSpace.rd:
                        UwallForMenorah[U, 0] = x;
                        UwallForMenorah[U, 1] = y;
                        U++;
                        Spawn(x, y, rd);
                        break;
                    case gridSpace.ru:
                        Spawn(x, y, ru);
                        break;
                    case gridSpace.lu:
                        Spawn(x, y, lu);
                        break;
                }
            }
        }
    }
    Vector2 RandomDirection()
    {
        //pick random int between 0 and 3
        int choice = Mathf.FloorToInt(Random.value * 3.99f);
        //use that int to chose a direction
        switch (choice)
        {
            case 0:
                return Vector2.down;
            case 1:
                return Vector2.left;
            case 2:
                return Vector2.up;
            default:
                return Vector2.right;
        }
    }
    int NumberOfFloors()
    {
        int count = 0;
        foreach (gridSpace space in grid)
        {
            if (space == gridSpace.floor)
            {
                count++;
            }
        }
        return count;
    }
    void Spawn(float x, float y, GameObject toSpawn)
    {
        //find the position to spawn
        //Vector2 offset = roomSizeWorldUnits / 2.0f;
        //Vector2 offset = roomSizeWorldUnits / 2.0f;
        Vector2 spawnPos = new Vector2(x, y) * worldUnitsInOneGridCell;
        //spawn object
        Instantiate(toSpawn, spawnPos, Quaternion.identity);
    }

    bool NormalGeneration()
    {
        if (LVL % 3 != 0)
        {
            int achievable = 0;
            for (int i = 0; i < floorC; i++)
            {
                achievable += normalGenerationCheck.GetAvailable(new Vector2(floorForSpawnMob[0, 0], floorForSpawnMob[0, 1]), new Vector2(floorForSpawnMob[i, 0], floorForSpawnMob[i, 1]));
            }
            if (achievable == floorC)
            {
                return true;
            }
            else return false;
        }
        return true;
    }

    public void SpawnMob()
    {
        MobCountOnLvl = 0;
        if (LVL % 3 != 0)
        {
            player = GameObject.FindGameObjectWithTag("Player");

            mobCount = (int)Random.Range(LevelGenerator.LVL, LevelGenerator.LVL + 4f);

            floorCount = floorForSpawnMob.GetLength(0);
            floorNumber = (int)Random.Range(0f, floorCount);

            floorNumber = (int)Random.Range(0f, floorCount);

            Vector2 floorPos = new Vector2(floorForSpawnMob[floorNumber, 0], floorForSpawnMob[floorNumber, 1]);

            player.transform.position = new Vector3(floorPos.x, floorPos.y, -90);

            for (int i = 0; i < mobCount; i++)
            {
                for (int j = 0; j < floorCount; j++)
                {
                    floorNumber = (int)Random.Range(0f, floorCount);
                    floorPos = new Vector2(floorForSpawnMob[floorNumber, 0], floorForSpawnMob[floorNumber, 1]);
                    if (Vector2.Distance(floorPos, player.transform.position) >= distance)
                    {
                        Instantiate(mob1, new Vector3(floorPos.x, floorPos.y, -90), Quaternion.identity);
                        MobCountOnLvl++;
                        break;
                    }
                }
            }
            //print(MobCountOnLvl);
        }
        else
        {
            player = GameObject.FindGameObjectWithTag("Player");

            player.transform.position = new Vector3(3, 3, -90);

            if (LVL == 3)
            {
                Instantiate(boss2, new Vector3(7, 7, -89), Quaternion.identity);
                MobCountOnLvl++;
            }
            else if (LVL == 6)
            {
                Instantiate(boss1, new Vector3(10, 10, -89), Quaternion.identity);
                MobCountOnLvl++;
            }
            else if (LVL == 9)
            {
                Instantiate(boss3, new Vector3(10, 10, -89), Quaternion.identity);
                MobCountOnLvl++;
            }
            else
            {
                float r = Random.Range(0f, 2f);

                if (r < 1)
                {
                    Instantiate(boss1, new Vector3(10, 10, -89), Quaternion.identity);
                    MobCountOnLvl++;
                }

                else if (r < 2)
                {
                    Instantiate(boss2, new Vector3(7, 7, -89), Quaternion.identity);
                    MobCountOnLvl++;
                }
            }
            
        }       
    }
        
    
    public void DecreaseMobCountOnLvl()
    {
        MobCountOnLvl--;
        if (MobCountOnLvl == 0)
        {
            GameObject Enemy = GameObject.FindGameObjectWithTag("Enemy");
            Instantiate(portal, new Vector3(Enemy.transform.position.x, Enemy.transform.position.y + 0.3f, -80), Quaternion.identity);
        }
    }

    public float[] GetPos1Pos2()
    {
        int r1 = Random.Range(0, floorForSpawnMob.GetLength(0));
        float[] f = new float[2];
        f[0] = floorForSpawnMob[r1, 0];
        f[1] = floorForSpawnMob[r1, 1];
        return f;
    }
    //
    private void SpawnMenorah()
    {

        for (int i= 0; i< UwallForMenorah.GetLength(0); i++)
        {
        Collider2D[] menorah = Physics2D.OverlapCircleAll(new Vector2(UwallForMenorah[i, 0], UwallForMenorah[i, 1]), MenorahSpawnRange, whatIsMenorah);
            if (menorah.Length <= 0)
                Instantiate(menorahU, new Vector3(UwallForMenorah[i, 0], UwallForMenorah[i, 1], -10), Quaternion.identity);
        }


        for (int i = 0; i < LwallForMenorah.GetLength(0); i++)
        {
            Collider2D[] menorah = Physics2D.OverlapCircleAll(new Vector2(LwallForMenorah[i, 0], LwallForMenorah[i, 1]), MenorahSpawnRange, whatIsMenorah);
            if (menorah.Length <= 0)
                Instantiate(menorahL, new Vector3(LwallForMenorah[i, 0], LwallForMenorah[i, 1], -10), Quaternion.identity);
        }
        

        for (int i = 0; i < RwallForMenorah.GetLength(0); i++)
        {
            Collider2D[] menorah = Physics2D.OverlapCircleAll(new Vector2(RwallForMenorah[i, 0], RwallForMenorah[i, 1]), MenorahSpawnRange, whatIsMenorah);
            if (menorah.Length <= 0)
                Instantiate(menorahR, new Vector3(RwallForMenorah[i, 0], RwallForMenorah[i, 1], -10), Quaternion.identity);
        }


        for (int i = 0; i < DwallForMenorah.GetLength(0); i++)
        {
            Collider2D[] menorah = Physics2D.OverlapCircleAll(new Vector2(DwallForMenorah[i, 0], DwallForMenorah[i, 1]), MenorahSpawnRange, whatIsMenorah);
            if (menorah.Length <= 0)
                Instantiate(menorahD, new Vector3(DwallForMenorah[i, 0], DwallForMenorah[i, 1], -10), Quaternion.identity);
        }

    }
}
