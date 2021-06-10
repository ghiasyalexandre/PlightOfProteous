using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomInstance : MonoBehaviour
{
    public Texture2D tex;
    [HideInInspector]
    public Vector2 gridPos;
    public int type; // 0: Normal, 1: Enter, 2: Boss
    public GameObject treasureChest;
    [SerializeField]
    bool doorTop, doorBot, doorLeft, doorRight;
    [SerializeField]
    GameObject doorU, doorD, doorL, doorR;
    [SerializeField]
    GameObject wallU, wallD, wallL, wallR, wall;
    [SerializeField]
    ColorToGameObject[] mappings;
    float tileSize = 0.4f;
    Vector2 roomSizeInTiles = new Vector2(9, 17);
    bool playerEntered;
    bool isClearing;
    bool isCleared;
    Transform _camera;
    CameraClamp cameraClamp;
    Vector3 moveJump = Vector2.zero;
    float minX, maxX, minY, maxY;
    float startTime;
    float checkTime = 1.5f;
    int numberOfEnemies;
    LayerMask enemyMask;
    private Collider2D[] colliders;
    public Collider2D[] GetColliders() { return colliders; }
    private List<DoorOpen> doors = new List<DoorOpen>();


    private void OnEnable()
    {
        _camera = GameObject.FindGameObjectWithTag("MainCamera").transform;
        cameraClamp = _camera.GetComponent<CameraClamp>();
        enemyMask = LayerMask.GetMask("Enemy");
    }

    private void Start()
    {
        SheetAssigner SA = FindObjectOfType<SheetAssigner>();
        Vector2 tempJump = SA.roomDimensions + SA.gutterSize;
        moveJump = new Vector3(tempJump.x, tempJump.y, 0); //distance b/w rooms to be used for movement
        minX = transform.position.x - SA.roomDimensions.x / 4 + 0.45f;
        maxX = transform.position.x + SA.roomDimensions.x / 4 - 0.45f;
        minY = transform.position.y - SA.roomDimensions.y / 4 + 0.3f;
        maxY = transform.position.y + SA.roomDimensions.y / 4 - 0.3f;
    }

    private void Update()
    {
        if (playerEntered)
        {
            //Debug.Log("Player Entered the room ");
            ActivateRoom();
        }

        if (isClearing)
        {
            if (startTime >= checkTime)
            {
                isCleared = CheckIfRoomCleared();
                startTime = 0f;
            }
            else
            {
                startTime += Time.deltaTime;
            }
        }
    }

    bool CheckIfRoomCleared()
    {
        int activeEnemies = 0;

        colliders = Physics2D.OverlapAreaAll(new Vector2(transform.position.x - transform.localScale.x * 0.45f, transform.position.y - transform.localScale.y * 0.45f),
                                             new Vector2(transform.position.x + transform.localScale.x * 0.45f, transform.position.y + transform.localScale.y * 0.45f), enemyMask);

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject.activeInHierarchy)
            {
                activeEnemies++;
            }
        }

        numberOfEnemies = activeEnemies;

        if (activeEnemies == 0)
        {
            isClearing = false;
            SpawnReward();
            LockRoom(false);
            return true;
        }
        return false;
    }

    void CameraChanging()
    {
        cameraClamp.SetClamp(minX, maxX, minY, maxY);
    }

    public void Setup(Texture2D _tex, Vector2 _gridPos, int _type, bool _doorTop, bool _doorBot, bool _doorLeft, bool _doorRight)
    {
        tex = _tex;
        gridPos = _gridPos;
        type = _type;
        doorTop = _doorTop;
        doorBot = _doorBot;
        doorLeft = _doorLeft;
        doorRight = _doorRight;
        PlaceDoors();
        foreach(DoorOpen door in doors)
        {
            door.transform.parent = transform.parent;
        }
        GenerateRoomTiles();
    }

    void PlaceDoors()
    {
        Vector3 spawnPos = transform.position + Vector3.up * (roomSizeInTiles.y / 4 * tileSize) - Vector3.up * (tileSize / 4);
        if (doorTop)
        {
            doors.Add(Instantiate(doorU, spawnPos, Quaternion.identity).GetComponent<DoorOpen>()); //.transform.parent = transform;
        }
        else
        {
            Instantiate(wallU, spawnPos, Quaternion.identity).transform.parent = transform;
        }

        spawnPos = transform.position + Vector3.down * (roomSizeInTiles.y / 4 * tileSize) - Vector3.down * (tileSize / 4);
        if (doorBot)
        {
            doors.Add(Instantiate(doorD, spawnPos, Quaternion.identity).GetComponent<DoorOpen>()); //.transform.parent = transform;
        }
        else
        {
            Instantiate(wallD, spawnPos, Quaternion.identity).transform.parent = transform;
        }

        spawnPos = transform.position + Vector3.right * (roomSizeInTiles.x * tileSize) - Vector3.right * (tileSize);
        if (doorRight)
        {
            doors.Add(Instantiate(doorR, spawnPos, Quaternion.identity).GetComponent<DoorOpen>()); //.transform.parent = transform;
        }
        else
        {
            Instantiate(wallR, spawnPos, Quaternion.identity).transform.parent = transform;
        }

        spawnPos = transform.position + Vector3.left * (roomSizeInTiles.x * tileSize) - Vector3.left * (tileSize);
        if (doorLeft)
        {
            doors.Add(Instantiate(doorL, spawnPos, Quaternion.identity).GetComponent<DoorOpen>()); //.transform.parent = transform;
        }
        else
        {
            Instantiate(wallL, spawnPos, Quaternion.identity).transform.parent = transform;
        }
    }

    void PlaceWall(Vector3 spawnPos, GameObject wallSpawn)
    {
        Instantiate(wallSpawn, spawnPos, Quaternion.identity).transform.parent = transform;
    }

    void GenerateRoomTiles()
    {
        var pixelColor = tex.GetPixels32();

        //loop through every pixel of the texture
        for (int x = 0; x < tex.width; x++)
        {
            for (int y = 0; y < tex.height; y++)
            {
                GenerateTile(x, y);
            }
        }
    }

     void GenerateTile(int x, int y)
     {
        Color pixelColor = tex.GetPixel(x,y);
        //skip clear spaces in texture
        if (pixelColor.a == 0)
        {
            return;
        }

        //find the color to match the pixel
        foreach (ColorToGameObject mapping in mappings)
        {
            if (mapping.color == pixelColor)
            {
                Vector3 spawnPos = positionFromTileGrid(x, y);
                Instantiate(mapping.prefab, spawnPos, Quaternion.identity).transform.parent = this.transform;
            }
        }
     }

     Vector3 positionFromTileGrid(int x, int y)
     {
        Vector3 ret;
        //find difference between the corner of the texture and the center of this object
        Vector3 offset = new Vector3((-roomSizeInTiles.x + 1) * tileSize, (roomSizeInTiles.y / 4) * tileSize - (tileSize / 4), 0);
        //find scaled up position at the offset
        ret = new Vector3(tileSize * (float)x, -tileSize * (float)y, 0) + offset + transform.position;
        return ret;
     }

     void SpawnReward()
     {
        Instantiate(treasureChest, transform.position, Quaternion.identity).transform.parent = transform;
     }

    void ActivateRoom()
    {
        LockRoom(true);

        int i;
        colliders = Physics2D.OverlapAreaAll(new Vector2(transform.position.x - transform.localScale.x * 0.45f, transform.position.y - transform.localScale.y * 0.45f),
                                             new Vector2(transform.position.x + transform.localScale.x * 0.45f, transform.position.y + transform.localScale.y * 0.45f), enemyMask);

        for (i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject.tag == "Enemy")
            {
                colliders[i].gameObject.SetActive(true);
                colliders[i].GetComponent<IEnemy>().Aggro = true;
                colliders[i].GetComponent<ITakeDamage>().Invulnerable = false;
            }
        }

        numberOfEnemies = i + 1;
        playerEntered = false;
        isClearing = true;
    }

    void LockRoom(bool toLock)
    {
        foreach(DoorOpen door in doors)
        {
            door.IsLocked = toLock;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            cameraClamp.SetClamp(minX, maxX, minY, maxY);
            if (isCleared == false)
                playerEntered = true;
        }
    }
}