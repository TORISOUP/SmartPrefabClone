using System.Linq;
using UnityEditor;

namespace TORISOUP.SmartPrefabClone.Editor
{
    public sealed class SmartPrefabCloneWindow
    {
        private static string SelectAssetPath => AssetDatabase.GetAssetPath(Selection.objects[0]);

        [MenuItem("Assets/Clone Prefab with Material", priority = 0)]
        private static void Show()
        {
            // 選択中のGameObject取得
            var gameObject = Selection.gameObjects.FirstOrDefault();
            if (gameObject == null) return;

            // 保存先指定
            var filePath = EditorUtility.SaveFilePanelInProject(
                "保存先", $"{gameObject.name}_clone", "prefab", "");

            if (string.IsNullOrEmpty(filePath)) return;

            // Prefabのクローン
            SmartPrefabClone.Clone(SelectAssetPath, filePath);
        }


        /// <summary>
        /// 1つGameObjectを選択している場合のみ有効
        /// </summary>
        [MenuItem("Assets/Clone Prefab with Material", validate = true)]
        private static bool ShowValidation()
        {
            return Selection.gameObjects.Length == 1;
        }
    }
}