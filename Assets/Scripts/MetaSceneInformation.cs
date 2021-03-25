using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetaSceneInformation : MonoBehaviour
{
    public static List<string> names;
    public static List<int> levels;
    public static string PlayerName { get; set; }
    public static int Level { get; set; }
    public static float time = 0.0f;
    public static int SeedCount { get; set; }
    public static int CollectedSeeds { get; set; }
    private static bool collected_ = false;
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    
    public static void SetCollected()
    {
        collected_ = true;
    }
    public static void ResetCollected()
    {
        collected_ = true;
    }
}
