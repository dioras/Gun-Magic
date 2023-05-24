using UnityEditor;

namespace _GAME.Keystore.Editor
{
    [InitializeOnLoad]
    public class PreloadSigningAlias
    {
        static PreloadSigningAlias()
        {
            PlayerSettings.Android.keystorePass = "gunmagic_132470";
            PlayerSettings.Android.keyaliasName = "gunmagickeystore";
            PlayerSettings.Android.keyaliasPass = "totally unexpected password 123";
        }
    }
}