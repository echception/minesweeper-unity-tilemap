using System.IO;
using UnityEditor;
using UnityEngine;

namespace Minesweeper
{
    /// <summary>
    /// Adds menu items to Unity for our tiles
    /// </summary>
    public static class MenuItems
    {
        [MenuItem("Assets/Create/Minesweeper/UnrevealedSquare")]
        public static void CreateUnrevealedTile()
        {
            CreateAssetAtLocation<UnrevealedSquare>("UnrevealedSquare.asset");
        }

        [MenuItem("Assets/Create/Minesweeper/RevealedSquare")]
        public static void CreateRevealedTile()
        {
            CreateAssetAtLocation<RevealedSquare>("RevealedSquare.asset");
        }

        private static void CreateAssetAtLocation<T>(string defaultName) where T : ScriptableObject
        {
            Object selectedObject = Selection.activeObject;
            string path = "Assets";

            if (selectedObject != null)
            {
                string assetPath = AssetDatabase.GetAssetPath(selectedObject.GetInstanceID());

                if (!string.IsNullOrEmpty(assetPath))
                {
                    if (Directory.Exists(assetPath))
                    {
                        path = assetPath;
                    }
                    else
                    {
                        path = Path.GetDirectoryName(assetPath);
                    }
                }
            }

            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<T>(), AssetDatabase.GenerateUniqueAssetPath(Path.Combine(path, defaultName)));
        }
    }
}
