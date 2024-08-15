using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BerserkPixel.Readme
{
    public class DebugInfo
    {
        private readonly string _assetName;
        private readonly string _assetVersion;
        private readonly string _unityVersion;

        public DebugInfo(Readme readme)
        {
            _assetName = readme.AssetName;
            _assetVersion = readme.AssetVersion;
            _unityVersion = readme.UnityVersion;
        }

        public void DrawDebug()
        {
            Utils.DrawUILine(Color.gray, 1, 20);
            GUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Debug info", EditorStyles.miniBoldLabel);

            GUILayout.BeginVertical();
            if (GUILayout.Button("Copy", EditorStyles.miniButtonLeft)) CopyDebugInfoToClipboard();

            if (EditorGUIUtility.systemCopyBuffer == GetDebugInfoString())
                EditorGUILayout.LabelField("Copied!", EditorStyles.miniLabel);

            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            var debugInfo = GetDebugInfo();
            foreach (var s in debugInfo) EditorGUILayout.LabelField($"    {s}", EditorStyles.miniLabel);

            EditorGUILayout.Separator();
        }

        public void CopyDebugInfoToClipboard()
        {
            EditorGUIUtility.systemCopyBuffer = GetDebugInfoString();
        }

        public string GetDebugInfoString()
        {
            var info = GetDebugInfo();
            return string.Join("\n", info);
        }

        public string[] GetDebugInfo()
        {
            var renderPipeline = Shader.globalRenderPipeline;
            if (string.IsNullOrWhiteSpace(renderPipeline))
                renderPipeline = "Built-In";

            var info = new List<string>
            {
                $"{_assetName} version {_assetVersion}",
                $"Unity {_unityVersion}",
                $"Dev platform: {Application.platform}",
                $"Target platform: {EditorUserBuildSettings.activeBuildTarget}",
                $"Render pipeline: {renderPipeline}"
            };

            return info.ToArray();
        }
    }
}