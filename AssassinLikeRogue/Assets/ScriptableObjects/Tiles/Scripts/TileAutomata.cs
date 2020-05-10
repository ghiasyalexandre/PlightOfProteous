using UnityEngine.Tilemaps;
using UnityEngine;
using UnityEditor;

public class TileAutomata : MonoBehaviour {

    [Range(0,100)]
    public int iniChance;
    [Range(1,8)]
    public int birthLimit;
    [Range(1,8)]
    public int deathLimit;
    [Range(1,8)]
    public int extraTileChance;
    [Range(1, 8)]
    public int testTileChance;

    [Range(1,10)]
    public int numR;
    private int count = 0;

    private int[,] terrainMap;
    public Vector3Int tmpSize;
    public Tilemap testMap;
    public Tilemap extraMap;
    public Tilemap topMap;
    public Tilemap botMap;
    public Tile testTile;
    public Tile extraTile;
    public Tile topTile;
    public Tile botTile;

    int width;
    int height;

    public void Awake()
    {
        doSim(numR);
    }

    public void doSim(int nu)
    {
        clearMap(false);
        width = tmpSize.x;
        height = tmpSize.y;

        if (terrainMap == null)
        {
            terrainMap = new int[width, height];
            initPos();
        }

        for (int i = 0; i < nu; i++)
        {
            terrainMap = genTilePos(terrainMap);
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (terrainMap[x, y] == 1)
                {
                    topMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), topTile);
                    if (Random.Range(0, extraTileChance) == 0)
                    {
                        extraMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), extraTile);
                    }
                    if (Random.Range(0, testTileChance) == 0)
                    {
                        testMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), testTile);
                    }
                }
                botMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), botTile);
            }
        }
    }

    public void initPos()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                terrainMap[x, y] = Random.Range(1, 101) < iniChance ? 1 : 0;
            }
        }
    }

    public int[,] genTilePos(int[,] oldMap)
    {
        int[,] newMap = new int[width,height];
        BoundsInt myB = new BoundsInt(-1, -1, 0, 3, 3, 1);
        int neighb;


        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                neighb = 0;
                foreach (var b in myB.allPositionsWithin)
                {
                    if (b.x == 0 && b.y == 0) continue;
                    if (x+b.x >= 0 && x+b.x < width && y+b.y >= 0 && y+b.y < height)
                    {
                        neighb += oldMap[x + b.x, y + b.y];
                    }
                    else
                    {
                        neighb++;
                    }
                }

                if (oldMap[x,y] == 1)
                {
                    if (neighb < deathLimit) newMap[x, y] = 0;
                    else
                    {
                        newMap[x, y] = 1;

                    }
                }

                if (oldMap[x,y] == 0)
                {
                    if (neighb > birthLimit) newMap[x, y] = 1;
                    else
                    {
                        newMap[x, y] = 0;
                    }
                }
            }
        }
        return newMap;
    }

	void LateUpdate ()
    {
        if (Input.GetButtonDown("Enable Debug Button 1"))
        {
            doSim(numR);
        }

        if (Input.GetButtonDown("Enable Debug Button 2"))
        {
            clearMap(true);
        }


        if (Input.GetKeyDown(KeyCode.Backslash))
        {
            SaveAssetMap();
            count++;
        }

    }

    public void clearMap(bool complete)
    {
        extraMap.ClearAllTiles();
        testMap.ClearAllTiles();
        topMap.ClearAllTiles();
        botMap.ClearAllTiles();
        if (complete)
        {
            terrainMap = null;
        }
    }
    
    public void SaveAssetMap()
    {
        string saveName = "tmapXY_" + count;
        var mf = GameObject.Find("Grid");

        if (mf)
        {
            var savePath = "Assets/" + saveName + ".prefab";
#pragma warning disable CS0618 // Type or member is obsolete
            if (PrefabUtility.CreatePrefab(savePath, mf))
#pragma warning restore CS0618 // Type or member is obsolete
            {
                EditorUtility.DisplayDialog("Tilemap saved", "Your Tilemap was saved under" + savePath, "Continue");
            }
            else
            {
                EditorUtility.DisplayDialog("Tilemap NOT saved", "An ERROR occured while trying to saveTilemap under" + savePath, "Continue");
            }
        }
    }
    
}
