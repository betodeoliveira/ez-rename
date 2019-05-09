using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace SMG.EzRenamer
{
    public class EzRR_CaseChange : ScriptableObject
    {
        private bool showOption = false;

        // Case Change
        private string[] caseChangeOptions = new string[]
        {
            "aa",
            "Aa",
            "AA",
        };

        private int caseChangeOptionIndex = 0;

        private string caseChangeName;

        public void Draw()
        {
            EzRR_Style.DrawUILine(EzRR_Style.uiLineColor);
            EditorGUILayout.BeginHorizontal();
            showOption = EzRR_Style.DrawFoldoutHeader("Case Change", showOption);
            EzRR_Style.DrawHelpButton("https://solomidgames.com/guides/ez-rename/case-change.html");
            EditorGUILayout.EndHorizontal();
            if (showOption)
            {
                EditorGUI.indentLevel = 1;
                DrawCaseChange();
                EditorGUILayout.Space();
                EditorGUI.indentLevel = 0;
                DrawButtons();
            }
        }

        private void DrawCaseChange()
        {
            caseChangeOptionIndex = EditorGUILayout.Popup("New Casing", caseChangeOptionIndex, caseChangeOptions);
        }

        private void DrawButtons()
        {
            EzRR_Style.DrawHeader("Do Case Change On:");
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Hierarchy", "ButtonLeft", GUILayout.Height(EzRR_Style.mediumBtnHeight), GUILayout.MinWidth(100)))
            {
                DoCaseChangeHierarchy();
            }
            if (GUILayout.Button("Project Folder", "ButtonRight", GUILayout.Height(EzRR_Style.mediumBtnHeight), GUILayout.MinWidth(100)))
            {
                DoCaseChangeProjectFolder();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DoCaseChangeHierarchy()
        {
            GameObject[] _gameObjectsSelected = Selection.gameObjects;
            if (ErrorsOnHierarchy(_gameObjectsSelected))
                return;
            // Calculate the amount that each file will increase in the progress bar
            float _result = (float)_gameObjectsSelected.Length / 100f;
            Undo.RecordObjects(_gameObjectsSelected, "caseChange");
            for (int i = 0; i < _gameObjectsSelected.Length; i++)
            {
                EditorUtility.DisplayProgressBar(FEEDBACKS.Title._05, FEEDBACKS.CaseChange._00, _result * i);
                caseChangeName = _gameObjectsSelected[i].name;
                ConfigCaseChange();
                _gameObjectsSelected[i].name = caseChangeName;
            }

            EditorUtility.ClearProgressBar();
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }

        private bool ErrorsOnHierarchy(GameObject[] gameObjectsSelected)
        {
            // Verifies if there's at least one gameobject selected
            if (gameObjectsSelected.Length <= 0)
            {
                EditorUtility.DisplayDialog(FEEDBACKS.Title._00, FEEDBACKS.General._00, FEEDBACKS.Button._00);
                {
                    return true;
                }
            }
            else
                return false;
        }

        private void DoCaseChangeProjectFolder()
        {
            Object[] _objectsSelected = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
            if (ErrorsOnProjectFolder(_objectsSelected))
                return;
            // Keep the path of the current file
            string _path;
            // Calculate the amount that each file will increase in the progress bar
            float _result = (float)_objectsSelected.Length / 100f;
            // Rename the files
            for (int i = 0; i < _objectsSelected.Length; i++)
            {
                EditorUtility.DisplayProgressBar(FEEDBACKS.Title._05, FEEDBACKS.CaseChange._00, _result * i);
                _path = AssetDatabase.GetAssetPath(_objectsSelected[i]);
                caseChangeName = _objectsSelected[i].name;
                ConfigCaseChange();
                AssetDatabase.RenameAsset(_path, caseChangeName);
            }

            EditorUtility.ClearProgressBar();
        }

        private bool ErrorsOnProjectFolder(Object[] objectsSelected)
        {
            // Verifies if there's at least one object selected
            if (objectsSelected.Length <= 0)
            {
                EditorUtility.DisplayDialog(FEEDBACKS.Title._00, FEEDBACKS.General._01, FEEDBACKS.Button._00);
                return true;
            }
            else
                return false;
        }

        private void ConfigCaseChange()
        {
            switch (caseChangeOptionIndex)
            {
                case 0:
                    caseChangeName = caseChangeName.ToLower();
                    break;

                case 1:
                    caseChangeName = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(caseChangeName.ToLower());
                    break;

                case 2:
                    caseChangeName = caseChangeName.ToUpper();
                    break;
            }
        }
    }
}