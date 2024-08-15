using UnityEngine;

namespace BerserkPixel.Readme
{
    public class Contact
    {
        private Shortcuts _shortcuts;

        public Contact(Readme readme)
        {
            _shortcuts = new Shortcuts(readme.AssetName, readme.urlsConfig);
        }

        public void Update(Readme readme)
        {
            _shortcuts = new Shortcuts(readme.AssetName, readme.urlsConfig);
        }

        public void DrawContact(string message)
        {
            Utils.CreateTitle("About");

            if (GUILayout.Button("Leave a Review")) _shortcuts.OpenReviewsPage();

            if (GUILayout.Button("Email Support")) _shortcuts.ShowSupportEmailEditor(message);

            if (GUILayout.Button("Get in touch")) _shortcuts.ShowBusinessEmailEditor();

            if (GUILayout.Button("Check more assets!")) _shortcuts.OpenBerserkStorePage();
        }
    }
}