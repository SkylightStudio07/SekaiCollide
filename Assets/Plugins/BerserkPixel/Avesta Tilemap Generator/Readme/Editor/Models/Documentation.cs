using UnityEngine;

namespace BerserkPixel.Readme
{
    public class Documentation
    {
        private Shortcuts _shortcuts;

        public Documentation(Readme readme)
        {
            _shortcuts = new Shortcuts(readme.AssetName, readme.urlsConfig);
        }

        public void Update(Readme readme)
        {
            _shortcuts = new Shortcuts(readme.AssetName, readme.urlsConfig);
        }

        public void DrawDocumentation()
        {
            Utils.CreateTitle("Learning Resources");

            if (GUILayout.Button("Online Documentation")) _shortcuts.ShowOnlineManual();

            if (GUILayout.Button("Youtube videos")) _shortcuts.ShowYoutube();
        }
    }
}