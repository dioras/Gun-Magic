using _GAME;
using _GAME.Env;
using _GAME.Level;
using _GAME.Player;
using _GAME.Shop;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.SceneManagement;
using UnityEngine;

public class ConfigureOnBuild : MonoBehaviour
{
    [MenuItem("Tools/Configure Shop")]
    public static void ConfigureShop()
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        var group = settings.DefaultGroup;
        var guids = AssetDatabase.FindAssets("t:ScriptableObject", new[] { "Assets/_GAME/Shop/Data" });

        var entriesAdded = new List<AddressableAssetEntry>();

        for (int i = 0; i < guids.Length; i++)
        {
            var path = AssetDatabase.GUIDToAssetPath(guids[i]);
            var asset = AssetDatabase.LoadAssetAtPath<ShopElements>(path);

            int counter = 0;
            foreach (var prod in asset.Products)
            {
                if (counter != 0) prod.Locked = true;
                counter++;
            }

            EditorUtility.SetDirty(asset);
        }
    }

    public static void ConfigureEnemySetting()
    {
        var guids = AssetDatabase.FindAssets("EnemySettings t:ScriptableObject", new[] { "Assets/_GAME/Enemy/Settings" });

        for (int i = 0; i < guids.Length; i++)
        {
            var path = AssetDatabase.GUIDToAssetPath(guids[i]);
            var asset = AssetDatabase.LoadAssetAtPath<_GAME.Enemy.EnemySettings>(path);

            asset.EnemyEffectsPreset = null;
            asset.EnemyPrefabsPreset = null;

            EditorUtility.SetDirty(asset);
        }
    }

    public static void ConfigureEnvFeature()
    {
        var env = FindObjectOfType<EnvFeature>();
        env.ExplosionEffect = null;
    }

    public static void ConfigurePlayerFeature()
    {
        var player = FindObjectOfType<PlayerFeature>();
    }

    public static void ConfigureGameFeature()
    {
        var game = FindObjectOfType<GameFeature>();
        game.IsTestMode = false;
    }

    public static void ConfigureLevelFeature()
    {
        var level = FindObjectOfType<LevelFeature>();
        level.GenerateLevelFromJSON = true;
    }

    [MenuItem("Tools/ClearDependencies")]
    public static void ClearDependencies()
    {
        ConfigureGameFeature();
        ConfigureLevelFeature();
        ConfigureEnemySetting();
        ConfigureEnvFeature();
        ConfigurePlayerFeature();
        string[] path = EditorSceneManager.GetActiveScene().path.Split(char.Parse("/"));
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        bool saved = EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        Debug.Log("Scene saved = " + saved);
    }
}