using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using static Registry_Buildings;
using static Registry_GroundTile;
using static Registry_OBJTile;
using static TownData_Registry;

public class RiverSegment
{
    Vector2 startNode;
    Vector2 endNode;


}

public static class TownGeneration
{
    public static int TILE_SIZE = 1;
    public static int OBJ_OFFSET = 1; // How many units above tiles to spawn objects

    private static int oceanThickness = 25;
    private static int trainLineThickness = 10;

    private static int minRiverThickness = 3;
    private static int maxRiverThickness = 6;
    public static float river_middleNode_MAX_DISTORTION = maxRiverThickness * 5;
    public static int min_river_bendnodes = 0;
    public static int max_river_bendnodes = 3;

    private enum RIVER_TYPE {straightLine, oneMajor_BendNode, twoMajor_BendNodes};

    private static int _mainland_townWidth = 100;
    private static int _mainland_townHeight = 100;

    private static int minNumRivers = 1;
    private static int maxNumRivers = 3;
    private static int minNumPonds = 0;
    private static int maxNumPonds = 3;

    private static int numHousesAtStart = 4;

    private enum TOWN_SIDES { RIGHT, TOP, LEFT, BOTTOM };

    // Generated setting
    public static int townWidth = _mainland_townWidth + oceanThickness;
    public static int townHeight = _mainland_townHeight + oceanThickness;

    public static void RandGen_TownData(ref int[,] towndata_groundLayer, ref int[,] towndata_objectLayer, ref int[,] towndata_otherdata)
    {
        // First, init as all 0s
        towndata_groundLayer = new int[townHeight, townWidth]; // defaults to 0 = grass
        towndata_objectLayer = new int[townHeight, townWidth]; // defaults to = empty


        // Spawn ocean borders (bottom)
        for (int i = townHeight-1; i >= townHeight-1-oceanThickness; i--)
        {
            for (int j = 0; j < townWidth; j++)
            {
                towndata_groundLayer[i, j] = GROUND_oceanWater_ID;
            }
        }
        // Spawn ocean borders (left and right)
        for (int i = trainLineThickness; i < townHeight; i++)
        {
            for (int j = 0; j < oceanThickness; j++) // left border
            {
                towndata_groundLayer[i, j] = GROUND_oceanWater_ID;
            }
            for (int j = townWidth - 1; j >= townWidth - 1 -oceanThickness; j--) // right border
            {
                towndata_groundLayer[i, j] = GROUND_oceanWater_ID;
            }
        }

        // Spawn train track layer
        for (int i = 0; i < trainLineThickness; i++)
        {
            for (int j = 0; j < trainLineThickness; j++)
            {
                towndata_objectLayer[i, j] = OBJ_traintracks_reserved;
            }
        }



        // Next, spawn rivers, avoiding the reserved spaces for buildings, and reserved spaces for train tracks etc.
        int riverCount_Requested = Random.Range(minNumRivers, maxNumRivers);
        int pondCount_Requested = Random.Range(minNumPonds, maxNumPonds);
        SpawnRivers(riverCount_Requested, ref int[,] towndata_groundLayer, int[,] towndata_objectLayer);
        /* ^^ SpawnRivers is the preferred/new method/way of doing this
        for (int i = 0; i < riverCount_Requested; i++)
        {
            SpawnRiver();
        }*/

        // Spawn buildings after
        // Actually, because of fitting issues, maybe we should first register every river / building we want to spawn as a request which we store
        // and then in a separate step, we use the constraints of all of them to fit them all in?

        // Register requests to place buildings and other features
        List<RequestPlacement> _requestPlacement = new List<RequestPlacement>();
        _requestPlacement.Add(new RequestPlacement_Building(OBJ_nookshop_reserved, BLD_NookShop_width, BLD_NookShop_height); // Buildings contains data about siize of placement and placement constraints (which tiles are banned/allowed, etc).
        _requestPlacement.Add(new RequestPlacement_Building(OBJ_townhall_reserved, BLD_TownHall_width, BLD_TownHall_height);
        _requestPlacement.Add(new RequestPlacement_Building(OBJ_tailor_reserved, BLD_Tailor_width, BLD_Tailor_height);
        _requestPlacement.Add(new RequestPlacement_Building(OBJ_postoffice_reserved, BLD_PostOffice_width, BLD_PostOffice_height);
        _requestPlacement.Add(new RequestPlacement_Building(OBJ_museum_reserved, BLD_Museum_width, BLD_Museum_height);

        for (int npcHouseNum = 0; npcHouseNum < numHousesAtStart; npcHouseNum ++)
        {
            _requestPlacement.Add(new RequestPlacement_Building(OBJ_villagerHouse_reserved, BLD_VillagerHouse_width, BLD_VillagerHouse_height);
        }

        // Actually place all the buildings
        Resolve_RequestPlacements(_requestPlacement, ref int[,] towndata_groundLayer, int[,] towndata_objectLayer);

        // Spawn ponds last
        for (int i = 0; i < pondCount_Requested; i++)
        {
            SpawnPond(ref int[,] towndata_groundLayer, int[,] towndata_objectLayer);
        }
        // Possible problem: We may be running out of floor space for buildings (we will see)
    }

    /*
    private static void SpawnBuilding(int buildingID, ref int[,] towndata_groundLayer, ref int[,] towndata_objectLayer, ref int[,] towndata_otherdata)
    {
        // Get data from prefab
        int buildingWidth = BUILDINGS[buildingID].GetComponent<Building>().buildingWidth;
        int buildingHeight = BUILDINGS[buildingID].GetComponent<Building>().buildingHeight;

        // Avoid any reserved spots (i.e. value of towndata_objectLayer[i,j] > 0) and find a place to fit the building at
        // todo

    }*/

    private static List<Vector2> get_TownBorder_LeftRight() // returns [i,j] 
    {
        List<Vector2> townBorders_leftRight = new List<Vector2>();
        for (int i = trainLineThickness; i < townHeight; i++)
        {
            townBorders_leftRight.Add(new Vector2(i, oceanThickness));
            townBorders_leftRight.Add(new Vector2(i, townWidth - 1 - oceanThickness));
        }
        return townBorders_leftRight;
    }

    private static List<Vector2> get_townborder_left()
    {
        List<Vector2> border = new List<Vector2>();
        for (int i = trainLineThickness; i < townHeight; i++)
        {
            border.Add(new Vector2(i, oceanThickness));
        }
        return border;
    }

    private static List<Vector2> get_townborder_right()
    {
        List<Vector2> border = new List<Vector2>();
        for (int i = trainLineThickness; i < townHeight; i++)
        {
            border.Add(new Vector2(i, townWidth - 1 - oceanThickness));
        }
        return border;
    }

    private static List<Vector2> get_townborder_bottom()
    {
        List<Vector2> border = new List<Vector2>();
        for (int j = 0; j < townWidth; j++)
        {
            int i = townHeight - 1 - oceanThickness;
            border.Add(new Vector2(i, j));
        }
        return border;
    }

    private static List<Vector2> get_townborder_top()
    {
        List<Vector2> border = new List<Vector2>();
        for (int j = 0; j < townWidth; j++)
        {
            int i = 0 + trainLineThickness;
            border.Add(new Vector2(i, j));
        }
        return border;
    }

    private static void SpawnRivers(int riverCount, ref int[,] towndata_groundLayer, int[,] towndata_objectLayer)
    {
        int min_startendpoint_distance = 2 * maxRiverThickness; // force rivers to be a bit apart from one another

        List<Vector2>[] townBorders = { get_townborder_right(), get_townborder_top(), get_townborder_left(), get_townborder_bottom() };

        // Choose start /ends
        int[] lastRiver_startSide_endside = { -1, -1 };
        Vector2[] startPoints_chosen = new Vector2[riverCount]; // nice
        Vector2[] endPoints_chosen = new Vector2[riverCount]; //
        int[] middleNodeCounts_Chosen = new int[riverCount];
        //Vector2[] middleNodes_chosen = new Vector2[riverCount];
        List<Vector2>[] middlePoints_Chosen = new List<Vector2>[riverCount]; // each is a list of middle nodes
        
        for (int r = 0; r < riverCount; r++)
        {
            List<int> middleNodeCounts_allowed = new List<int>(){ 0, 1, 2, 3};
            if (r > 0) middleNodeCounts_allowed.Remove(middleNodeCounts_Chosen[r - 1]); // force different number than previous river

            middleNodeCounts_Chosen[r] = middleNodeCounts_allowed[Random.Range(0, middleNodeCounts_allowed.Count)];
            int middleNodeCount_Choice = middleNodeCounts_Chosen[r];
            // make start

            List<int> sides = new List<int> (){ 0, 1, 2, 3 };
            sides.Remove(lastRiver_startSide_endside[0]);
            sides.Remove(lastRiver_startSide_endside[1]);
            int _side_startpoint = sides[Random.Range(0, sides.Count)];// see B
            // Select any not this river startside
            sides = new List<int>() { 0, 1, 2, 3 };
            sides.Remove(_side_startpoint);    
            int _side_endpoint = sides[Random.Range(0, sides.Count)];
            // Update lastriverstartsideendside
            lastRiver_startSide_endside[0] = _side_startpoint;
            lastRiver_startSide_endside[1] = _side_endpoint;

            // Now pick an eligible location for start
            List<Vector2> _tmp_start_possiblePoints = townBorders[_side_startpoint];
            List<Vector2> FILTERED_start_possiblePoints = new List<Vector2>();
            foreach (Vector2 v in _tmp_start_possiblePoints)
            {
                bool exclude = false;
                foreach (Vector2 bannedPoint in startPoints_chosen)
                {
                    if (Vector2.Distance(bannedPoint, v) <= min_startendpoint_distance) exclude = true;
                }
                foreach (Vector2 bannedPoint in endPoints_chosen)
                {
                    if (Vector2.Distance(bannedPoint, v) <= min_startendpoint_distance) exclude = true;
                }
                // If !exclude, add to filtered list
                if (!exclude) FILTERED_start_possiblePoints.Add(v);
            }

            // Now pick eligible location for end (we can do without updating banned points list, since it is guarnatteed to be on a differnt side from startpoint)
            List<Vector2> _tmp_end_possiblePoints = townBorders[_side_endpoint];
            List<Vector2> FILTERED_end_possiblePoints = new List<Vector2>();
            foreach (Vector2 v in _tmp_end_possiblePoints)
            {
                bool exclude = false;
                foreach (Vector2 bannedPoint in startPoints_chosen)
                {
                    if (Vector2.Distance(bannedPoint, v) <= min_startendpoint_distance) exclude = true;
                }
                foreach (Vector2 bannedPoint in endPoints_chosen)
                {
                    if (Vector2.Distance(bannedPoint, v) <= min_startendpoint_distance) exclude = true;
                }
                // If !exclude, add to filtered list
                if (!exclude) FILTERED_end_possiblePoints.Add(v);
            }

            // Update arrays
            Vector2 startPoint_choice = FILTERED_start_possiblePoints[Random.Range(0, FILTERED_start_possiblePoints.Count)];
            Vector2 endPoint_choice = FILTERED_end_possiblePoints[Random.Range(0, FILTERED_end_possiblePoints.Count)];

            // Update arrays
            startPoints_chosen[r] = startPoint_choice;
            endPoints_chosen[r] = endPoint_choice;
            middlePoints_Chosen[r] = new List<Vector2>();

            // Forced random node
            for (int x = 0; x < middleNodeCount_Choice; x++)
            {
                Vector2 superRandomNode = Vector2.Lerp(startPoint_choice, endPoint_choice, Random.Range(0.25f, 0.75f));
                superRandomNode += Random.insideUnitCircle * river_middleNode_MAX_DISTORTION;
                middlePoints_Chosen[r].Add(superRandomNode);
            }

            // Place little middle nodes
            int littleNodeCount = 12; // evenly between line and nearest middlenode, with small noise

            // Build path
            List<Vector2> middleNodes_copy = middlePoints_Chosen[r].Copy();
            List<Vector2> path_start_middlenodes_end = new List<Vector2>();
            path_start_middlenodes_end.Add(startPoint_choice);
            while (true)
            {
                Vector2 lastNode = path_start_middlenodes_end[path_start_middlenodes_end.Count - 1];

                // Find shortest node to x
                float shortestDistance = float.MaxValue;
                Vector2 shortestNode = Vector2.negativeInfinity;
                if (middleNodes_copy.Count == 0)
                {
                    path_start_middlenodes_end.Add(endPoint_choice);
                    break; // end
                }
                // Find shortest node to last node
                foreach (Vector2 nextNode in middleNodes_copy)
                {
                    if (Vector2.Distance(nextNode, lastNode) < shortestDistance) 
                    {
                        shortestDistance = Vector2.Distance(nextNode, lastNode);
                        shortestNode = nextNode;
                    } 
                }
                path_start_middlenodes_end.Add(shortestNode);
                middleNodes_copy.Remove(shortestNode);
            }

            // Path is built to path_start_middlenodes_end
            int segmentIndex = 0;
            while (path_start_middlenodes_end[segmentIndex + 1] != endPoint_choice)
            {
                Vector2 currentSegment = path_start_middlenodes_end[segmentIndex] - path_start_middlenodes_end[segmentIndex + 1];
                Vector2 currentSegment_normal = new Vector2(-currentSegment.normalized.y, currentSegment.normalized.x);

            }


            // 2 Segments
            Vector2 start2Middle = middleNode_choice - startPoint_choice;
            Vector2 start2Middle_Normal = new Vector2(-start2Middle.normalized.y, start2Middle.normalized.x);
            Vector2 middle2End = endPoint_choice - middleNode_choice;
            Vector2 middle2End_Normal = new Vector2(-middle2End.normalized.y, middle2End.normalized.x);

            // Place "little deviation" points

            // Decide how many middle points
            // Divide segment from start to end into x middle points
            // 1 middle point is allowed to have a big deviation, the others should have moderate devations
            // 
            // Decide where each middle point is placed
        }
    }
}
