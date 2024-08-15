using UnityEditor;
using UnityEngine;

namespace BerserkPixel.Readme
{
    public class SpecialThanks
    {
        // amazing people that has reached out with feedback! Keep it coming!
        // To appear here you can contribute at support@berserkpixel.studio 
        // or add me on Discord "Bet0" (#1302)
        private readonly string[] _heroes = {"AlCh3mi"};

        public void Draw()
        {
            Utils.DrawUILine(Color.gray, 1, 20);

            EditorGUILayout.LabelField("Special Thanks to:", EditorStyles.miniBoldLabel);

            foreach (var hero in _heroes) EditorGUILayout.LabelField($"{hero}", EditorStyles.miniLabel);

            EditorGUILayout.Separator();
        }
    }
}