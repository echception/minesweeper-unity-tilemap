using UnityEditor;
using UnityEngine;

namespace Minesweeper
{
    /// <summary>
    /// Editor for the RevealedSquare tile.  Provides a better interface than the array of sprites we would get otherwise
    /// </summary>
    [CustomEditor(typeof(RevealedSquare))]
    public class RevealedSquareEditor : Editor
    {
        private RevealedSquare tile { get { return (target as RevealedSquare); } }

        public void OnEnable()
        {
            if (tile.NumberedSprites == null || tile.NumberedSprites.Length != 9)
            {
                tile.NumberedSprites = new Sprite[9];
                EditorUtility.SetDirty(tile);
            }
        }

        public override void OnInspectorGUI()
        {
            float oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 210;

            EditorGUI.BeginChangeCheck();
            tile.NumberedSprites[0] = (Sprite)EditorGUILayout.ObjectField("0", tile.NumberedSprites[0], typeof(Sprite), false, null);
            tile.NumberedSprites[1] = (Sprite)EditorGUILayout.ObjectField("1", tile.NumberedSprites[1], typeof(Sprite), false, null);
            tile.NumberedSprites[2] = (Sprite)EditorGUILayout.ObjectField("2", tile.NumberedSprites[2], typeof(Sprite), false, null);
            tile.NumberedSprites[3] = (Sprite)EditorGUILayout.ObjectField("3", tile.NumberedSprites[3], typeof(Sprite), false, null);
            tile.NumberedSprites[4] = (Sprite)EditorGUILayout.ObjectField("4", tile.NumberedSprites[4], typeof(Sprite), false, null);
            tile.NumberedSprites[5] = (Sprite)EditorGUILayout.ObjectField("5", tile.NumberedSprites[5], typeof(Sprite), false, null);
            tile.NumberedSprites[6] = (Sprite)EditorGUILayout.ObjectField("6", tile.NumberedSprites[6], typeof(Sprite), false, null);
            tile.NumberedSprites[7] = (Sprite)EditorGUILayout.ObjectField("7", tile.NumberedSprites[7], typeof(Sprite), false, null);
            tile.NumberedSprites[8] = (Sprite)EditorGUILayout.ObjectField("8", tile.NumberedSprites[8], typeof(Sprite), false, null);
            tile.BombSprite = (Sprite)EditorGUILayout.ObjectField("Bomb", tile.BombSprite, typeof(Sprite), false, null);
            if (EditorGUI.EndChangeCheck())
                EditorUtility.SetDirty(tile);

            EditorGUIUtility.labelWidth = oldLabelWidth;
        }
    }
}
