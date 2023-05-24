using Doozy.Engine.UI;
using UnityEngine;
using UnityEngine.UI;

namespace _GAME.LevelUIView
{
    public class LoadingScreenRefs : MonoBehaviour
    {
        public UIView UIView;
        //public Slider ProgressBar;
        public Image ProgressBarFillImage;

        public Image ProgreessBarBGImage;
        //public Image progressBarFillImage;

        public Image MainBGImage;

        public Color ProgressBarBGColor;
        public Color ProgressBarFillColor;

        public GameObject Logo;
        public Color DefaultColor;

        public bool IsFirstLoad = true;
    }
}
