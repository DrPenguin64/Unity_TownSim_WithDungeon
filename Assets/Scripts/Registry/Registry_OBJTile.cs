using UnityEngine;
using static TownData_Registry;

public static class Registry_OBJTile
{
    // Object tiles
    public static int OBJ_EMPTY_ID = 0;
    public static int OBJ_fruitTree_ID = 1;
    public static int OBJ_pineTree_ID = 2;
    public static int OBJ_buriedSpot_ID = 3;
    public static int OBJ_shinySpot_ID = 4;
    public static int OBJ_moneyTree_ID = 5;
    public static int OBJ_normTree_ID = 6;
    public static int OBJ_sign_ID = 7; // additional data stores its msg
    public static int OBJ_bush_ID = 8;

    public static int OBJ_nookshop_reserved = 9;
    public static int OBJ_tailor_reserved = 10;
    public static int OBJ_villagerHouse_reserved = 11;
    public static int OBJ_playerHouse_reserved = 12;
    public static int OBJ_museum_reserved = 13;
    public static int OBJ_postoffice_reserved = 14;
    public static int OBJ_townhall_reserved = 15;

    public static int OBJ_traintracks_reserved = 32;

    public static int OBJ_rock_ID = 64; // additional data can be useds to store this



    public static GameObject OBJ_normTree_prefab;
    public static GameObject OBJ_fruitTree_prefab;
    public static GameObject OBJ_pineTree_prefab;
    public static GameObject OBJ_buriedSpot_prefab;
    public static GameObject OBJ_shinySpot_prefab;
    public static GameObject OBJ_rock_prefab;
    public static GameObject OBJ_sign_prefab;
    public static GameObject OBJ_bush_prefab;

    private static void LoadPrefabs()
    { // should give error message if fails to load
        OBJ_normTree_prefab = Resources.Load<GameObject>("Prefabs/OBJ_Tile/fruitless_tree");
        OBJ_fruitTree_prefab = Resources.Load<GameObject>("Prefabs/OBJ_Tile/fruit_tree");
        OBJ_pineTree_prefab = Resources.Load<GameObject>("Prefabs/OBJ_Tile/pine_tree");
        OBJ_buriedSpot_prefab = Resources.Load<GameObject>("Prefabs/OBJ_Tile/buriedSpot");
        OBJ_shinySpot_prefab = Resources.Load<GameObject>("Prefabs/OBJ_Tile/shinySpot");
        OBJ_rock_prefab = Resources.Load<GameObject>("Prefabs/OBJ_Tile/obj_rock");
        OBJ_sign_prefab = Resources.Load<GameObject>("Prefabs/OBJ_Tile/outdoor_sign");
        OBJ_bush_prefab = Resources.Load<GameObject>("Prefabs/OBJ_Tile/bush");
    }

    public static void CreateRegistry()
    {
        LoadPrefabs();
        dataTable_objectBlocks[OBJ_normTree_ID] = OBJ_normTree_prefab;
        dataTable_objectBlocks[OBJ_fruitTree_ID] = OBJ_fruitTree_prefab;
        dataTable_objectBlocks[OBJ_pineTree_ID] = OBJ_pineTree_prefab;
        dataTable_objectBlocks[OBJ_buriedSpot_ID] = OBJ_buriedSpot_prefab;
        dataTable_objectBlocks[OBJ_shinySpot_ID] = OBJ_shinySpot_prefab;
        dataTable_objectBlocks[OBJ_rock_ID] = OBJ_rock_prefab;
        dataTable_objectBlocks[OBJ_sign_ID] = OBJ_sign_prefab;
        dataTable_objectBlocks[OBJ_bush_ID] = OBJ_bush_prefab;
    }
}
