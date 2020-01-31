using System.IO;
using System.Text.RegularExpressions;

namespace TORISOUP.SmartPrefabClone.Editor
{
    internal static class Ext
    {
        /// <summary>
        /// フルパスからAssets/以下の相対パスに変換する
        /// </summary>
        internal static string ToAssetsPath(this string path)
        {
            var delimit = Regex.Escape(Path.DirectorySeparatorChar.ToString());
            var split = Regex.Split(path, string.Format("{0}Assets{1}", delimit, delimit));
            return Path.Combine("Assets", split[1]);
        }
    }
}