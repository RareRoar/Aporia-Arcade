using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class Loader : MonoBehaviour
{
    [SerializeField]
    private int height = 30;
    [SerializeField]
    private int width = 30;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        InitGame();
    }

    public GameObject player;
    public GameObject pickUpSeed;
    public GameObject glitchTile;
    public GameObject wallTile;
    public static Loader instance = null;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static public void CallbackInitialization()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    static private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (instance != null)
        {
            instance.InitGame();
        }
    }
    void InitGame()
    {
        LevelGenerator matrix = new LevelGenerator(height, width);
        LayoutWalls(matrix);
        LayoutSeeds(matrix);
        MetaSceneInformation.CollectedSeeds = 0;
    }

    void LayoutWalls(LevelGenerator matrix)
    {
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (matrix.IsWall(i, j))
                {
                    int randNum = Random.Range(1, 101);
                    if (randNum <= 10)
                    {
                        Instantiate(glitchTile, new Vector3(i, 0, j), Quaternion.identity);
                    }
                    else
                    {
                        Instantiate(wallTile, new Vector3(i, 0, j), Quaternion.identity);
                    }
                }
            }
        }
    }

    void LayoutSeeds(LevelGenerator matrix)
    {
        foreach (var tile in matrix.EmptyTiles)
        {
            int randNum = Random.Range(1, 101);
            if (randNum <= 20)
            {
                Instantiate(pickUpSeed, new Vector3(tile.Position.X, 0, tile.Position.Y), Quaternion.identity);
                MetaSceneInformation.SeedCount++;
            }
        }
    }
}
