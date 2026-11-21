using UnityEngine;

public static class G_Registry
{
    public static void CreateRegistry()
    {
        TownData_Registry.CreateRegistry(); // create tile registires etc, for things that spawn outside
        Registry_Buildings.CreateRegistry();
    }
}
