using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Prototype.Editor
{
    [CustomEditor(typeof(SpriteDatabase))]
    public class SpriteDatabaseEditor : UnityEditor.Editor
    {
        private string searchFilter = "";
        private Vector2 scrollPos;

        public override void OnInspectorGUI()
        {
            // Display original array dropdown (optional)
            base.OnInspectorGUI();

            // Cast the target to your specific class
            var spriteDatabase = (SpriteDatabase) target;

            // Search bar
            EditorGUILayout.BeginHorizontal(GUI.skin.FindStyle("Toolbar"));
            searchFilter = EditorGUILayout.TextField(searchFilter, GUI.skin.FindStyle("ToolbarSearchTextField"));
            if (GUILayout.Button("", GUI.skin.FindStyle("ToolbarSearchCancelButton")))
            {
                // Remove focus if pressed
                searchFilter = "";
                GUI.FocusControl(null);
            }

            EditorGUILayout.EndHorizontal();

            // Start a scrolling view inside GUI, to handle lots of sprites
            EditorGUILayout.BeginVertical();
            int rowHeight = 64;
            int maxVisibleRows = 5; // or whatever fits your typical screen size
            var scrollViewHeight = Mathf.Min(1000, Mathf.Max(rowHeight * maxVisibleRows, spriteDatabase.sprites.Length * rowHeight));
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(scrollViewHeight));

            // Filtered or full list depending on searchFilter
            var filteredList = string.IsNullOrEmpty(searchFilter)
                ? spriteDatabase.sprites.Take(Math.Min(spriteDatabase.sprites.Length, 10)).ToArray()
                : spriteDatabase.sprites.Where(asset => asset.id.ToLower().Contains(searchFilter.ToLower()));

            foreach (var spriteAsset in filteredList)
            {
                EditorGUILayout.BeginHorizontal();
                // Display the name
                EditorGUILayout.LabelField(spriteAsset.id, GUILayout.Width(200));

                // Display the sprite with a thumbnail
                spriteAsset.sprite = (Sprite) EditorGUILayout.ObjectField(spriteAsset.sprite, typeof(Sprite), false,
                    GUILayout.Width(rowHeight), GUILayout.Height(rowHeight));
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();

            // Save changes to the SpriteDatabase
            if (GUI.changed) EditorUtility.SetDirty(spriteDatabase);
        }
    }
}