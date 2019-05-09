using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using System.IO;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace SMG.EzRenamer
{
    public class EzRR_Sort : ScriptableObject
    {
        public enum SortOptions
        {
            nameA_Z,
            nameZ_A
        }
        public SortOptions sortOption = SortOptions.nameA_Z;
        private bool showOption = false;
        private static float lastMenuCallTimestamp;


        public void Draw()
        {
            EzRR_Style.DrawUILine(EzRR_Style.uiLineColor);
            EditorGUILayout.BeginHorizontal();
            showOption = EzRR_Style.DrawFoldoutHeader("Sort", showOption);
            EzRR_Style.DrawHelpButton("https://solomidgames.com/guides/ez-rename/sort.html");
            EditorGUILayout.EndHorizontal();
            if (showOption)
            {
                EditorGUI.indentLevel = 1;
                DrawSort();
                EditorGUILayout.Space();
                EditorGUI.indentLevel = 0;
                DrawButtons();
            }
        }

        private void DrawSort()
        {
            sortOption = (SortOptions)EditorGUILayout.EnumPopup("Sort Options", sortOption);
        }

        private void DrawButtons()
        {
            EzRR_Style.DrawHeader("Do Sort On:");
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Hierarchy", "ButtonLeft", GUILayout.Height(EzRR_Style.mediumBtnHeight), GUILayout.MinWidth(100)))
            {
                SortSelection();
            }
            if (GUILayout.Button("Project Folder", "ButtonRight", GUILayout.Height(EzRR_Style.mediumBtnHeight), GUILayout.MinWidth(100)))
            {
                SortChildren();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void SortSelection()
        {
            DoSort(sortOption, false);
        }

        private void SortChildren()
        {
            DoSort(sortOption, true);
        }

        /// <summary>
        /// This method is important because when sorting from shortcut the sort action will be done several times
        /// </summary>
        public static void ShortcutSortConfig(SortOptions sortOption, bool sortChildren)
        {
            if (Time.unscaledTime.Equals(lastMenuCallTimestamp))
                return;
            DoSort(sortOption, sortChildren);
        }

        private static void DoSort(SortOptions sortOption, bool sortChildren)
        {
            // Keep the selected gameobjects
            List<GameObject> _tempGameobjectsSelected = new List<GameObject>();
            // Keep the sibling index of each gameobject
            List<int> _gameObjectsSiblingIndex = new List<int>();

            if (!sortChildren)
            {
                foreach (GameObject gameObj in Selection.gameObjects)
                    _tempGameobjectsSelected.Add(gameObj);
            }
            else
            {
                if (Selection.gameObjects.Length > 1)
                {
                    EditorUtility.DisplayDialog(FEEDBACKS.Title._00, FEEDBACKS.Sort._01, FEEDBACKS.Button._00);
                    return;
                }

                for (int i = 0; i < Selection.gameObjects[0].transform.childCount; i++)
                    _tempGameobjectsSelected.Add(Selection.gameObjects[0].transform.GetChild(i).gameObject);
            }

            GameObject[] _gameobjectsSelected = _tempGameobjectsSelected.ToArray();

            if (!SortCheckErrorsToContinue(_gameobjectsSelected))
            {
                lastMenuCallTimestamp = 0;
                return;
            }
            // Get the sibling index of each game object. 
            // I'm using "for" because when using "foreach" the siblins are storage out of order.
            for (int i = 0; i < _gameobjectsSelected.Length; i++)
            {
                _gameObjectsSiblingIndex.Add(_gameobjectsSelected[i].transform.GetSiblingIndex());
            }
            // Sort the list to organize the values from the lowest to biggest
            _gameObjectsSiblingIndex.Sort();

            // Sort the array using the natural compare
            System.Array.Sort(_gameobjectsSelected, delegate (GameObject tempObjTrans0, GameObject tempObjTrans1)
            {
                return EditorUtility.NaturalCompare(tempObjTrans0.name, tempObjTrans1.name);
            });

            // Calculate the amount that each file will increase in the progress bar
            float _result = (float)_gameobjectsSelected.Length / 100f;

            // Set the new sibling index
            if (sortOption == SortOptions.nameA_Z)
            {
                for (int i = 0; i < _gameobjectsSelected.Length; i++)
                {
                    EditorUtility.DisplayProgressBar(FEEDBACKS.Title._06, FEEDBACKS.Sort._00, _result * i);
                    Undo.SetTransformParent(_gameobjectsSelected[i].transform, _gameobjectsSelected[i].transform.parent, "sort");
                    _gameobjectsSelected[i].transform.SetSiblingIndex(_gameObjectsSiblingIndex[i]);
                }
            }
            else
            {
                for (int i = 0; i < _gameobjectsSelected.Length; i++)
                {
                    EditorUtility.DisplayProgressBar(FEEDBACKS.Title._06, FEEDBACKS.Sort._00, _result * i);
                    int _index = _gameObjectsSiblingIndex[(_gameObjectsSiblingIndex.Count() - 1) - i];
                    Undo.SetTransformParent(_gameobjectsSelected[i].transform, _gameobjectsSelected[i].transform.parent, "sort");
                    _gameobjectsSelected[i].transform.SetSiblingIndex(_index);
                }
            }

            lastMenuCallTimestamp = Time.unscaledTime;
            EditorUtility.ClearProgressBar();
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }

        private static bool SortCheckErrorsToContinue(GameObject[] gameobjectsSelected)
        {
            if (gameobjectsSelected.Length <= 0)
            {
                EditorUtility.DisplayDialog(FEEDBACKS.Title._00, FEEDBACKS.General._00, FEEDBACKS.Button._00);
                return false;
            }

            // Verify if all selected gameobjects has the same parent
            List<Transform> _parents = new List<Transform>();
            for (int i = 0; i < gameobjectsSelected.Length; i++)
            {
                if (gameobjectsSelected[i].transform.parent != null && !_parents.Contains(gameobjectsSelected[i].transform.parent.transform))
                    _parents.Add(gameobjectsSelected[i].transform.parent.transform);

                if (_parents.Count > 1)
                {
                    EditorUtility.DisplayDialog(FEEDBACKS.Title._00, FEEDBACKS.Sort._02, FEEDBACKS.Button._00);
                    return false;
                }
            }

            return true;
        }
    }
}