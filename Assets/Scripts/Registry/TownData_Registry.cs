using UnityEngine;
using static Registry_GroundTile;
using static Registry_OBJTile;

public static class TownData_Registry
{
    // Data Tables
    public static GameObject[] dataTable_groundBlocks = new GameObject[32];
    public static GameObject[] dataTable_objectBlocks = new GameObject[32];

    public static void CreateRegistry()
    {
        Register_Tiles();
    }

    public static void Register_Tiles()
    {
        Registry_GroundTile.CreateRegistry();
        Registry_OBJTile.CreateRegistry();
    }


}
