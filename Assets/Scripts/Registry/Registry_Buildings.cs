using UnityEngine;

public static class Registry_Buildings
{
    // Registry array
    public static GameObject[] BUILDINGS; // stores prefabs
    public static int BUILDINGS_COUNT = 64;

    // Starter shops
    public static int BLD_NookShop_ID = 0;
    public static int BLD_Tailor_ID = 1;
    public static int BLD_Museum_ID = 2;
    public static int BLD_PostOffice_ID = 3;
    public static int BLD_TownHall_ID = 4;

    public static int BLD_PlayerHouse_ID = 16;
    public static int BLD_VillagerHouse_ID = 17;

    public static void CreateRegistry()
    {
        BUILDINGS = new GameObject[BUILDINGS_COUNT]; // null

        // Create registry
        BUILDINGS[BLD_NookShop_ID] = Resources.Load<GameObject>("Prefabs/BuildingsOutdoors/Nookshop_BASE");
        BUILDINGS[BLD_Tailor_ID] = Resources.Load<GameObject>("Prefabs/BuildingsOutdoors/Tailor");
        BUILDINGS[BLD_Museum_ID] = Resources.Load<GameObject>("Prefabs/BuildingsOutdoors/Museum");
        BUILDINGS[BLD_PostOffice_ID] = Resources.Load<GameObject>("Prefabs/BuildingsOutdoors/PostOffice");
        BUILDINGS[BLD_TownHall_ID] = Resources.Load<GameObject>("Prefabs/BuildingsOutdoors/TownHall");
        BUILDINGS[BLD_PlayerHouse_ID] = Resources.Load<GameObject>("Prefabs/BuildingsOutdoors/PlayerHouse_BASE");
        BUILDINGS[BLD_VillagerHouse_ID] = Resources.Load<GameObject>("Prefabs/BuildingsOutdoors/VillagerHouse_BASE");
    }


}
