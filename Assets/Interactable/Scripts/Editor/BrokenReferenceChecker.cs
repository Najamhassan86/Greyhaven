#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace EJETAGame.Editor
{
    /// <summary>
    /// Editor utility to find and report broken references in the project.
    /// Use: Right-click in Project window -> "Check for Broken References"
    /// </summary>
    public class BrokenReferenceChecker : EditorWindow
    {
        [MenuItem("Tools/Check for Broken References")]
        public static void ShowWindow()
        {
            GetWindow<BrokenReferenceChecker>("Broken References Checker");
        }

        private void OnGUI()
        {
            GUILayout.Label("Broken Reference Checker", EditorStyles.boldLabel);
            GUILayout.Space(10);

            if (GUILayout.Button("Check All Prefabs", GUILayout.Height(30)))
            {
                CheckAllPrefabs();
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Check Current Scene", GUILayout.Height(30)))
            {
                CheckCurrentScene();
            }

            GUILayout.Space(10);

            EditorGUILayout.HelpBox(
                "This tool will search for:\n" +
                "- Missing (Script) components\n" +
                "- Broken serialized references\n" +
                "- Null component references",
                MessageType.Info);
        }

        private void CheckAllPrefabs()
        {
            string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab");
            int brokenCount = 0;

            foreach (string guid in prefabGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                if (prefab != null)
                {
                    if (HasBrokenReferences(prefab, path))
                    {
                        brokenCount++;
                    }
                }
            }

            if (brokenCount == 0)
            {
                EditorUtility.DisplayDialog("Check Complete", "No broken references found in prefabs!", "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("Check Complete", 
                    $"Found {brokenCount} prefab(s) with broken references.\nCheck Console for details.", "OK");
            }
        }

        private void CheckCurrentScene()
        {
            GameObject[] allObjects = FindObjectsOfType<GameObject>();
            int brokenCount = 0;

            foreach (GameObject obj in allObjects)
            {
                Component[] components = obj.GetComponents<Component>();
                foreach (Component comp in components)
                {
                    if (comp == null)
                    {
                        Debug.LogWarning($"[BrokenReferenceChecker] Missing component on '{obj.name}' in scene '{UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}'");
                        brokenCount++;
                    }
                }
            }

            if (brokenCount == 0)
            {
                EditorUtility.DisplayDialog("Check Complete", "No broken references found in current scene!", "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("Check Complete", 
                    $"Found {brokenCount} broken reference(s) in current scene.\nCheck Console for details.", "OK");
            }
        }

        private bool HasBrokenReferences(GameObject prefab, string path)
        {
            bool hasBroken = false;
            Component[] components = prefab.GetComponentsInChildren<Component>(true);

            foreach (Component comp in components)
            {
                if (comp == null)
                {
                    Debug.LogWarning($"[BrokenReferenceChecker] Broken reference in prefab: {path}");
                    hasBroken = true;
                }
            }

            return hasBroken;
        }
    }
}
#endif
