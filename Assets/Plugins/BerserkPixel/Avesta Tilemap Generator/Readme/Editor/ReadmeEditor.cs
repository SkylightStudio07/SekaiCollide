using UnityEditor;
using UnityEngine;

namespace BerserkPixel.Readme
{
    [CustomEditor(typeof(Readme))]
    [InitializeOnLoad]
    public class ReadmeEditor : Editor
    {
        private static readonly string kShowedReadmeSessionStateName = "ReadmeEditor.showedReadme";

        private static string _fileType;
        private Contact _contact;
        private DebugInfo _debugInfo;
        private Documentation _documentation;

        private Readme _readme;
        private SpecialThanks _specialThanks;

        private UpdateCheck _updateCheck;

        static ReadmeEditor()
        {
            EditorApplication.delayCall += SelectReadmeAutomatically;
        }

        private void OnEnable()
        {
            _readme = serializedObject.targetObject as Readme;
            if (_readme == null)
            {
                Debug.LogError($"[{_readme.AssetName}] Readme error.");
                return;
            }

            _fileType = _readme.FileType;

            Setup();
        }

        private static void SelectReadmeAutomatically()
        {
            if (!EditorPrefs.GetBool(kShowedReadmeSessionStateName, false))
            {
                SelectReadme();
                EditorPrefs.SetBool(kShowedReadmeSessionStateName, true);
            }
        }

        private void Setup()
        {
            if (_readme.checkUpdate) _updateCheck = new UpdateCheck(_readme);

            if (_readme.documentation) _documentation = new Documentation(_readme);

            if (_readme.debug) _debugInfo = new DebugInfo(_readme);

            if (_readme.contact) _contact = new Contact(_readme);

            if (_readme.specialThanks) _specialThanks = new SpecialThanks();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (_readme == null)
            {
                Debug.LogError("Readme asset is null");
                return;
            }

            if (_readme.urlsConfig == null)
            {
                Debug.LogError("The urls provided don't exist");
                return;
            }

            Utils.CreateTitle(_readme.AssetName);

            EditorGUILayout.LabelField($"Version v{_readme.AssetVersion}", EditorStyles.miniLabel);
            EditorGUILayout.Separator();

            if (_readme.checkUpdate)
            {
                _updateCheck.Update(_readme);
                _updateCheck.DrawUpdateCheck();
            }

            if (_readme.documentation)
            {
                _documentation.Update(_readme);
                _documentation.DrawDocumentation();
            }

            if (_readme.contact)
            {
                _contact.Update(_readme);

                string debugInfo = null;
                if (_debugInfo != null) debugInfo = _debugInfo.GetDebugInfoString();
                _contact.DrawContact(debugInfo);
            }

            if (_readme.debug)
                _debugInfo.DrawDebug();

            if (_readme.specialThanks)
                _specialThanks.Draw();
        }

        private static void SelectReadme()
        {
            var ids = AssetDatabase.FindAssets($"Readme t:{_fileType}");
            if (ids.Length == 1)
            {
                var readmeObject = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(ids[0]));

                Selection.objects = new[] {readmeObject};
            }
            else
            {
                Debug.Log("Couldn't find a readme");
            }
        }
    }
}