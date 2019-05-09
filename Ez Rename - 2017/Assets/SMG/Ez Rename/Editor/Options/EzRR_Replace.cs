using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using System.IO;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

namespace SMG.EzRenamer
{
    public class EzRR_Replace : ScriptableObject
    {
        private bool showOption = false;
        private bool caseSensitive = true;

        private string stringToReplace;
        private string stringToReplaceWith;
        private string replaceNewName;

        public void Draw()
        {
            EzRR_Style.DrawUILine(EzRR_Style.uiLineColor);
            EditorGUILayout.BeginHorizontal();
            showOption = EzRR_Style.DrawFoldoutHeader("Replace", showOption);
            EzRR_Style.DrawHelpButton("https://solomidgames.com/guides/ez-rename/replace.html");
            EditorGUILayout.EndHorizontal();
            if (showOption)
            {
                EditorGUI.indentLevel = 1;
                DrawReplace();
                EditorGUILayout.Space();
                EditorGUI.indentLevel = 0;
                DrawButtons();
            }
        }

        private void DrawReplace()
        {
            stringToReplace = EditorGUILayout.TextField("Replace:", stringToReplace);
            stringToReplaceWith = EditorGUILayout.TextField("With:", stringToReplaceWith);
            caseSensitive = EditorGUILayout.Toggle("Case Sensitive", caseSensitive);
        }

        private void DrawButtons()
        {
            EzRR_Style.DrawHeader("Do Replace On:");
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Hierarchy", "ButtonLeft", GUILayout.Height(EzRR_Style.mediumBtnHeight), GUILayout.MinWidth(100)))
            {
                DoReplaceHierarchy();
            }
            if (GUILayout.Button("Project Folder", "ButtonRight", GUILayout.Height(EzRR_Style.mediumBtnHeight), GUILayout.MinWidth(100)))
            {
                DoReplaceProjectFolder();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DoReplaceHierarchy()
        {
            GameObject[] _gameObjectsSelected = Selection.gameObjects;
            // Check for errors before continuing
            if (ErrorsOnHierarchy(_gameObjectsSelected))
                return;
            // Calculate the amount that each file will increase in the progress bar
            float _result = (float)_gameObjectsSelected.Length / 100f;
            Undo.RecordObjects(_gameObjectsSelected, "replace");
            for (int i = 0; i < _gameObjectsSelected.Length; i++)
            {
                EditorUtility.DisplayProgressBar(FEEDBACKS.Title._02, FEEDBACKS.Replace._00, _result * i);
                replaceNewName = _gameObjectsSelected[i].name;
                if (caseSensitive)
                {
                    replaceNewName = replaceNewName.Replace(stringToReplace, stringToReplaceWith);
                }
                else
                {
                    replaceNewName = Regex.Replace(replaceNewName, stringToReplace, stringToReplaceWith, RegexOptions.IgnoreCase);
                }
                _gameObjectsSelected[i].name = replaceNewName;
            }

            EditorUtility.ClearProgressBar();
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }

        private bool ErrorsOnHierarchy(GameObject[] gameObjectsSelected)
        {
            // Verifies if the string to be replaced is empty or null
            if (EzRR_ErrorCheckers.CheckForStringNullOrEmpty(stringToReplace))
            {
                EditorUtility.DisplayDialog(FEEDBACKS.Title._00, FEEDBACKS.Replace._01, FEEDBACKS.Button._00);
                return true;
            }
            // Verifies if there's at least one gameobject selected
            else if (gameObjectsSelected.Length <= 0)
            {
                EditorUtility.DisplayDialog(FEEDBACKS.Title._00, FEEDBACKS.General._00, FEEDBACKS.Button._00);
                {
                    return true;
                }
            }
            else
                return false;
        }

        private void DoReplaceProjectFolder()
        {
            Object[] _objectsSelected = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
            // Check for errors before continuing
            if (ErrorsOnProjectFolder(_objectsSelected))
                return;
            // Keep the path of the current file
            string _path;
            // Calculate the amount that each file will increase in the progress bar
            float _result = (float)_objectsSelected.Length / 100f;
            for (int i = 0; i < _objectsSelected.Length; i++)
            {
                EditorUtility.DisplayProgressBar(FEEDBACKS.Title._02, FEEDBACKS.Replace._00, _result * i);
                _path = AssetDatabase.GetAssetPath(_objectsSelected[i]);
                replaceNewName = _objectsSelected[i].name;
                if (caseSensitive)
                {
                    replaceNewName = replaceNewName.Replace(stringToReplace, stringToReplaceWith);
                }
                else
                {
                    replaceNewName = Regex.Replace(replaceNewName, stringToReplace, stringToReplaceWith, RegexOptions.IgnoreCase);
                }
                if (EzRR_ErrorCheckers.CheckForInvalidNameEndings(replaceNewName))
                {
                    Debug.LogWarning("Skipping " + _objectsSelected[i].name + ":\n" + FEEDBACKS.General._03);
                }
                else
                {
                    // If something happens during the replacing get the message and show it on console
                    string _message = AssetDatabase.RenameAsset(_path, replaceNewName);
                    if (!string.IsNullOrEmpty(_message))
                        Debug.LogWarning(_message);
                }
            }

            EditorUtility.ClearProgressBar();
        }

        private bool ErrorsOnProjectFolder(Object[] objectsSelected)
        {
            // Verifies if the string to be replaced is empty or null
            if (EzRR_ErrorCheckers.CheckForStringNullOrEmpty(stringToReplace))
            {
                EditorUtility.DisplayDialog(FEEDBACKS.Title._00, FEEDBACKS.Replace._01, FEEDBACKS.Button._00);
                return true;
            }
            // Verifies if there's at least one object selected
            else if (objectsSelected.Length <= 0)
            {
                EditorUtility.DisplayDialog(FEEDBACKS.Title._00, FEEDBACKS.General._01, FEEDBACKS.Button._00);
                return true;
            }
            // Verifies invalid characters
            else if (EzRR_ErrorCheckers.CheckForInvalidCharacters(stringToReplaceWith))
            {
                EditorUtility.DisplayDialog(FEEDBACKS.Title._00, FEEDBACKS.General._02, FEEDBACKS.Button._00);
                return true;
            }
            else
                return false;
        }
    }
}