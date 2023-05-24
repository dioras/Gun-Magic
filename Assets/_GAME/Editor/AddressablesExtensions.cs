using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

public class AddressablesExtensions : MonoBehaviour
{

    [MenuItem("Tools/Refresh Addressables")]
    public static void RefreshAddressables()
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        var group = settings.DefaultGroup;
        var guids = AssetDatabase.FindAssets("t:GameObject t:Material t:Texture2D t:Mesh", new[] { "Assets/_GAME/Portal/Materials" });

        var entriesAdded = new List<AddressableAssetEntry>();

        for (int i = 0; i < guids.Length; i++)
        {
            var entry = settings.CreateOrMoveEntry(guids[i], group, readOnly: false, postEvent: false);

            var path = AssetDatabase.GUIDToAssetPath(guids[i]);
            entry.address = Path.GetFileNameWithoutExtension(path);
            //entry.labels.Add("Level");

            entriesAdded.Add(entry);
        }

        settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entriesAdded, true);
    }
}
