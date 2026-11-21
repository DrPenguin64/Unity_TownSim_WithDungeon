using UnityEngine;
using static TownData_Registry;

public static class Registry_GroundTile
{
    // Ground tiles
    public static GameObject grassblock_prefab;
    public static GameObject dirtblock_prefab;
    public static GameObject waterblock_prefab;
    public static GameObject cobblestone_block_Prefab;

    // Diggable/plantable
    public static int GROUND_grass_ID = 0;
    public static int GROUND_DIRT_ID = 1;
    public static int GROUND_BEACH_ID = 2;

    // water
    public static int GROUND_oceanWater_ID = 6;
    public static int GROUND_riverWater_ID = 7;
    public static int GROUND_pondWater_ID = 8;

    // Can't dig on/manmade/can't plant
    public static int GROUND_CBSTONE_ID = 16;


    private static void LoadPrefabs()
    {
        grassblock_prefab = Resources.Load<GameObject>("Prefabs/GroundTile/grassblock");
        dirtblock_prefab = Resources.Load<GameObject>("Prefabs/GroundTile/dirtblock");
        waterblock_prefab = Resources.Load<GameObject>("Prefabs/GroundTile/waterblock");
        cobblestone_block_Prefab = Resources.Load<GameObject>("Prefabs/GroundTile/cobblestoneblock");
    }

    public static void CreateRegistry()
    {
        LoadPrefabs();
        dataTable_groundBlocks[0] = grassblock_prefab;
        dataTable_groundBlocks[1] = dirtblock_prefab;
        dataTable_groundBlocks[2] = waterblock_prefab;
        dataTable_groundBlocks[3] = cobblestone_block_Prefab;
    }
}
