using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace TORISOUP.SmartPrefabClone.Editor
{
    internal sealed class SmartPrefabClone
    {
        internal static void Clone(string originPath, string savePath)
        {
            // xxx.prefab の xxx 部分
            var originName = Path.GetFileNameWithoutExtension(savePath);

            // 保存先のフルパス
            var saveDir = Directory.GetParent(savePath).FullName;
            // Materialの保存先のフルパス
            var materialFullPath = Path.Combine(saveDir, string.Format("{0}_materials", originName));
            // Materialの保存先の相対パス
            var materialPath = materialFullPath.ToAssetsPath();
            
            // ディレクトリが無いなら作る
            if (!Directory.Exists(materialFullPath))
            {
                Directory.CreateDirectory(materialFullPath);
            }


            // Prefabのコピーを保存
            AssetDatabase.CopyAsset(originPath, savePath);

            // コピーしたやつを読み込む
            var copy = AssetDatabase.LoadAssetAtPath(savePath, typeof(GameObject)) as GameObject;
            if (copy == null) return;
            
            // コピー済みのMaterialキャッシュ
            var cache = new Dictionary<int, Material>();

            // PrefabのRendererをすべて取得
            foreach (var renderer in copy.GetComponentsInChildren<Renderer>())
            {
                var materials = renderer.sharedMaterials;

                // materialを全列挙してコピーを作る
                foreach (var t in materials)
                {
                    if (cache.ContainsKey(t.GetInstanceID())) continue;
                    var sh = t.shader;
                    var nm = new Material(sh);
                    nm.CopyPropertiesFromMaterial(t);

                    // 同じファイル名だったら数字をつける
                    var fileName = string.Format("{0}.mat", t.name);
                    var num = 1;
                    while (File.Exists(Path.Combine(materialFullPath, fileName)))
                    {
                        fileName = string.Format("{0}{1}.mat", t.name, num++);
                    }

                    // コピーしたマテリアルを保存
                    AssetDatabase.CreateAsset(nm, Path.Combine(materialPath, fileName));
                    cache[t.GetInstanceID()] = nm;
                }

                // コピーしたmaterialの方を参照させる
                renderer.materials = materials.Select(x => cache[x.GetInstanceID()]).ToArray();
            }

            EditorUtility.SetDirty(copy);
            AssetDatabase.SaveAssets();
        }
    }
}