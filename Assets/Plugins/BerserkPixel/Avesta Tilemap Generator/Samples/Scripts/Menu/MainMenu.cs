using UnityEngine;
using UnityEngine.SceneManagement;

namespace Plugins.BerserkPixel.Avesta_Tilemap_Generator.Samples.Scripts.Menu
{
    public class MainMenu : MonoBehaviour
    {
        public void Click_GoToMenu()
        {
            SceneManager.LoadScene((int) SceneIndexes.Menu);
        }

        public void Click_GoToDemo()
        {
            SceneManager.LoadScene((int) SceneIndexes.Demo);
        }

        public void Click_GoToOcclusion()
        {
            SceneManager.LoadScene((int) SceneIndexes.Occlusion);
        }

        public void Click_GoToDestructible()
        {
            SceneManager.LoadScene((int) SceneIndexes.Destructible);
        }

        public void Click_GoToBomberman()
        {
            SceneManager.LoadScene((int) SceneIndexes.Bomberman);
        }

        public void Click_GoTo3D()
        {
            SceneManager.LoadScene((int) SceneIndexes.ThreeD);
        }
    }

    public enum SceneIndexes
    {
        Menu = 0,
        Demo = 1,
        Occlusion = 2,
        Destructible = 3,
        Bomberman = 4,
        ThreeD = 5,
    }
}