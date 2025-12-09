#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace EJETAGame.Editor
{
    /// <summary>
    /// Editor helper to prevent Unity Inspector errors when objects are destroyed.
    /// This script hooks into Unity's editor callbacks to deselect objects before they're destroyed.
    /// </summary>
    [InitializeOnLoad]
    public static class InspectorErrorPrevention
    {
        private static Object lastSelectedObject = null;

        static InspectorErrorPrevention()
        {
            //Subscribe to editor callbacks
            EditorApplication.update += CheckForDestroyedSelection;
            Selection.selectionChanged += OnSelectionChanged;
        }

        private static void OnSelectionChanged()
        {
            //Track the currently selected object
            lastSelectedObject = Selection.activeObject;
        }

        private static void CheckForDestroyedSelection()
        {
            //Check if currently selected object is null (was destroyed)
            if (Selection.activeObject != null)
            {
                //Verify the object still exists
                if (Selection.activeObject == null || 
                    (Selection.activeGameObject != null && Selection.activeGameObject == null))
                {
                    //Object was destroyed, clear selection to prevent inspector errors
                    Selection.activeObject = null;
                }
            }

            //Also check if last selected object was destroyed
            if (lastSelectedObject != null)
            {
                try
                {
                    //Try to access the object - if it throws, it's destroyed
                    string name = lastSelectedObject.name;
                }
                catch
                {
                    //Object was destroyed, clear if it's still selected
                    if (Selection.activeObject == lastSelectedObject)
                    {
                        Selection.activeObject = null;
                    }
                    lastSelectedObject = null;
                }
            }
        }
    }

    /// <summary>
    /// Helper class with static methods to safely destroy objects in editor
    /// </summary>
    public static class SafeDestroyHelper
    {
        /// <summary>
        /// Safely destroy a component, deselecting it first if selected
        /// </summary>
        public static void SafeDestroyComponent(Component component)
        {
            if (component == null) return;

            GameObject obj = component.gameObject;

            //Deselect if selected
            if (Selection.activeGameObject == obj || Selection.activeObject == component)
            {
                Selection.activeObject = null;
            }

            if (Application.isPlaying)
            {
                Object.Destroy(component);
            }
            else
            {
                Object.DestroyImmediate(component);
            }
        }

        /// <summary>
        /// Safely destroy a GameObject, deselecting it first if selected
        /// </summary>
        public static void SafeDestroyGameObject(GameObject obj)
        {
            if (obj == null) return;

            //Deselect if selected
            if (Selection.activeGameObject == obj || Selection.activeObject == obj)
            {
                Selection.activeObject = null;
            }

            if (Application.isPlaying)
            {
                Object.Destroy(obj);
            }
            else
            {
                Object.DestroyImmediate(obj);
            }
        }
    }
}
#endif
