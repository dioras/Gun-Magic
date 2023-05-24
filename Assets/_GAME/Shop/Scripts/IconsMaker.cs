using System.IO;
using UnityEngine;

public class IconsMaker : MonoBehaviour
{
    public Camera Camera;

    public string Path = "Assets/_Game/Portal/Icons/";

    public string FileName = "Portal";

    public int CurrentIndex = 0;


    [ContextMenu("Make Icon")]
    private void MakeIcon()
    {
        var icon = GetTexture(Camera);

        File.WriteAllBytes(Path + FileName  + $"_{CurrentIndex}" + ".png", icon.EncodeToPNG());

        CurrentIndex++;
    }


    // Take a "screenshot" of a camera's Render Texture.
    private Texture2D GetTexture(Camera camera)
    {
        // The Render Texture in RenderTexture.active is the one
        // that will be read by ReadPixels.
        var currentRT = RenderTexture.active;
        RenderTexture.active = camera.targetTexture;

        // Render the camera's view.
        camera.Render();

        // Make a new texture and read the active Render Texture into it.
        Texture2D image = new Texture2D(camera.targetTexture.width, camera.targetTexture.height);
        image.ReadPixels(new Rect(0, 0, camera.targetTexture.width, camera.targetTexture.height), 0, 0);
        image.Apply();

        // Replace the original active Render Texture.
        RenderTexture.active = currentRT;
        return image;
    }
}
