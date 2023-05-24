using UnityEditor;
using UnityEditor.AddressableAssets.Settings;

// ReSharper disable once CheckNamespace
public class PreBuildPlayerContent
{
    [InitializeOnLoadMethod]
    private static void Initialize()
    {
        BuildPlayerWindow.RegisterBuildPlayerHandler(BuildPlayerHandler);
    }

    private static void BuildPlayerHandler(BuildPlayerOptions buildPlayerOptions)
    {
        ConfigureOnBuild.ConfigureShop();
        ConfigureOnBuild.ClearDependencies();

        AddressableAssetSettings.CleanPlayerContent();
        AddressableAssetSettings.BuildPlayerContent();
        BuildPlayerWindow.DefaultBuildMethods.BuildPlayer(buildPlayerOptions);
    }
}