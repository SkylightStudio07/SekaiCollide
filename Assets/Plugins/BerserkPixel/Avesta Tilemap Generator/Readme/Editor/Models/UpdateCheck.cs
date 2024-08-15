using System;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.UI;
using UnityEngine;

namespace BerserkPixel.Readme
{
    public class UpdateCheck
    {
        private string _assetName;
        private string _assetVersion;
        private string _packageName;
        private bool _showingVersionMessage;
        private BerserkURL _urls;
        private string _versionLatest;

        public UpdateCheck(Readme readme)
        {
            Update(readme);

            _showingVersionMessage = false;
            _versionLatest = null;
        }

        public void Update(Readme readme)
        {
            _assetName = readme.AssetName;
            _assetVersion = readme.AssetVersion;
            _packageName = readme.PackageName;
            _urls = readme.urlsConfig;
        }

        public void DrawUpdateCheck()
        {
            if (_showingVersionMessage)
            {
                EditorGUILayout.Space(20);

                if (_versionLatest == null)
                {
                    EditorGUILayout.HelpBox("Checking the latest version...", MessageType.None);
                }
                else
                {
                    var local = Version.Parse(_assetVersion);
                    var remote = Version.Parse(_versionLatest);
                    if (local >= remote)
                    {
                        EditorGUILayout.HelpBox($"You have the latest version! v{_assetVersion}.",
                            MessageType.Info);
                    }
                    else
                    {
                        EditorGUILayout
                            .HelpBox(
                                "Update needed. " +
                                $"The latest version is v{_versionLatest}, but you have v{_assetVersion}.",
                                MessageType.Warning);
                        EditorGUILayout.Space(4);
                        if (GUILayout.Button("Open PackageManager")) OpenPackageManager();
                        EditorGUILayout.Space(4);
                    }
                }
            }

            if (GUILayout.Button("Check for Updates"))
            {
                _showingVersionMessage = true;
                _versionLatest = null;
                CheckVersion(version => { _versionLatest = version; });
            }

            if (_showingVersionMessage) EditorGUILayout.Space(20);
        }

        private void CheckVersion(Action<string> callback)
        {
            NetworkManager.GetVersion(_assetName, _urls.URL_VERSION, callback);
        }

        private void OpenPackageManager()
        {
#if UNITY_2020_3_OR_NEWER
            Client.Resolve();
#endif
            Window.Open(_packageName);
        }
    }
}