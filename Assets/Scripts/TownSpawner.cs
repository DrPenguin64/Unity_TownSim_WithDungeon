using UnityEngine;
using static LoadSaveTownData;

public class TownSpawner
{

    //Tiles
    private int[,] towndata_groundLayer;
    private int[,] towndata_objectLayer;
    private int[] towndata_otherdata;

    private Transform town_originCoordinate;

    public void Init()
    {
        // Load town data
        if (TryLoadTownData(ref towndata_groundLayer, ref towndata_objectLayer, ref towndata_otherdata)) // try load from disk
        { }
        else // Generate it
        {
            TownGeneration.RandGen_TownData(ref towndata_groundLayer, ref towndata_objectLayer, ref towndata_otherdata); // saves to vars, but not to disk
        }

        SpawnTown_FromTileData(towndata_groundLayer, towndata_objectLayer, towndata_otherdata, town_originCoordinate.gameObject); // spawn from origin
    }

    private void SpawnTown_FromTileData(int[][] towndata_groundLayer, int[][] towndata_objectLayer, int[] towndata_otherdata, GameObject root)
    {
        for (int i = 0; i < towndata_groundLayer.Length; i++)
        {
            for (int j = 0; j < towndata_groundLayer[0].Length; i++)
            {
                Vector3 _position = root.transform.position + new Vector3(j * TILE_SIZE, 0, i * TILE_SIZE);
                GameObject _theprefab = dataTable_groundBlocks[ towndata_groundLayer[i][j] ];
                GameObject _copy = GameObject.Instantiate(_theprefab, _position, Quaternion.Euler(0,0,0), root.transform);

                // Spawn object layer
                GameObject _theprefab_obj = dataTable_groundBlocks[towndata_objectLayer[i][j]];
                Vector3 _position_obj = root.transform.position + new Vector3(j * TILE_SIZE, OBJ_OFFSET, i * TILE_SIZE);
                if (_theprefab_obj == null) continue; // if its not something with a prefab to spawn, run any custom code
                else { GameObject _copy_obj = GameObject.Instantiate(_theprefab_obj, _position_obj, Quaternion.Euler(0, 0, 0), root.transform); }

                // do whatever with other data layer
            }
        }
    }



    void Start()
    {
        Init();
    }
}
