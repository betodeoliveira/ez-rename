using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using System.IO;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace SMG.EzRenamer
{
    public class EzRR_Rename : ScriptableObject
    {
        // Delimiter
        private enum DelimiterTypes
        {
            nothing,
            hyphen,
            dot,
            space,
            underline,
            custom
        }
        private DelimiterTypes prefixDelimiterTypes = DelimiterTypes.nothing;
        private DelimiterTypes suffixDelimiterTypes = DelimiterTypes.nothing;
        private DelimiterTypes enumerateDelimiterTypes = DelimiterTypes.nothing;
        private string delimiter;
        // Show
        private bool showOption = true;
        // Name
        private string newName;
        private string previewName;
        private string finalName;
        Texture2D cloneIcon;
        // Prefix
        private bool usePrefix;
        private string filePrefix;
        private string prefixCustomSeparator;
        // Suffix
        private bool addSuffix;
        private string fileSuffix;
        private string suffixCustomSeparator;
        // Enumerate
        private bool enumerate;
        private int enumerateInitNumber;
        private int enumerateIncrement;
        private int enumerateCounter;
        private string enumerateCustomSeparator;
        // Enumerate Options
        private enum EnumerateOptions
        {
            onBegin,
            onEnd,
            betweenPrefixAndName,
            betweenSuffixAndName
        }
        EnumerateOptions enumerateOptions = EnumerateOptions.onEnd;
        // Enumerate Formats
        private string[] enumerateFormats = new string[]
        {
            "0, 1, 2...",
            "00, 01, 02...",
            "000, 001, 002...,",
            "0000, 0001, 0002...",
            "00000, 00001, 00002..."
        };
        private int enumerateFormatIndex = 0;
        private string enumerateFormat;

        private void OnEnable()
        {
            cloneIcon = new Texture2D(16, 16);
            cloneIcon = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/SMG/Ez Rename/Editor/Editor Resources/Icons/clone-icon.png", typeof(Texture2D));
        }

        public void Draw()
        {
            EditorGUILayout.BeginHorizontal();
            showOption = EzRR_Style.DrawFoldoutHeader("Rename", showOption);
            EzRR_Style.DrawHelpButton("https://solomidgames.com/guides/ez-rename/rename.html");
            EditorGUILayout.EndHorizontal();
            if (showOption)
            {
                EditorGUI.indentLevel = 1;
                DrawNewName();
                DrawPrefixAndSufix();
                DrawEnumerate();
                EditorGUILayout.Space();
                DrawPreview();
                EditorGUILayout.Space();
                EditorGUI.indentLevel = 0;
                DrawButtons();
            }
        }

        private void DrawNewName()
        {
            EditorGUILayout.BeginHorizontal();
            newName = EditorGUILayout.TextField("New Name:", newName);
            if (GUILayout.Button(cloneIcon, GUILayout.Width(20), GUILayout.Height(13)))
            {
                CopySelectedObjectName();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawPrefixAndSufix()
        {
            // Prefix
            usePrefix = EzRR_Style.ToggleLeftBold("Prefix", usePrefix);
            EditorGUI.BeginDisabledGroup(usePrefix == false);
            EditorGUI.indentLevel = 3;
            filePrefix = EditorGUILayout.TextField("Prefix", filePrefix);
            prefixDelimiterTypes = (DelimiterTypes)EditorGUILayout.EnumPopup("Delimiter", prefixDelimiterTypes);
            if (prefixDelimiterTypes == DelimiterTypes.custom)
                prefixCustomSeparator = EditorGUILayout.TextField("Custom Delimiter", prefixCustomSeparator);
            else
                prefixCustomSeparator = "";
            EditorGUI.indentLevel = 1;
            EditorGUILayout.Space();
            EditorGUI.EndDisabledGroup();
            // Suffix
            addSuffix = EzRR_Style.ToggleLeftBold("Suffix", addSuffix);
            EditorGUI.BeginDisabledGroup(addSuffix == false);
            EditorGUI.indentLevel = 3;
            fileSuffix = EditorGUILayout.TextField("Suffix", fileSuffix);
            suffixDelimiterTypes = (DelimiterTypes)EditorGUILayout.EnumPopup("Delimiter", suffixDelimiterTypes);
            if (suffixDelimiterTypes == DelimiterTypes.custom)
                suffixCustomSeparator = EditorGUILayout.TextField("Custom Delimiter", suffixCustomSeparator);
            else
                suffixCustomSeparator = "";
            EditorGUI.indentLevel = 1;
            EditorGUILayout.Space();
            EditorGUI.EndDisabledGroup();
        }

        private void DrawEnumerate()
        {
            enumerate = EzRR_Style.ToggleLeftBold("Enumerate", enumerate);
            EditorGUI.BeginDisabledGroup(enumerate == false);
            EditorGUI.indentLevel = 3;
            enumerateOptions = (EnumerateOptions)EditorGUILayout.EnumPopup("Number Goes", enumerateOptions);
            enumerateInitNumber = EditorGUILayout.IntField("Initial Number", enumerateInitNumber);
            enumerateIncrement = EditorGUILayout.IntField("Increment", enumerateIncrement);
            enumerateFormatIndex = EditorGUILayout.Popup("Format", enumerateFormatIndex, enumerateFormats);
            if (enumerateIncrement <= 0)
                enumerateIncrement = 1;
            enumerateDelimiterTypes = (DelimiterTypes)EditorGUILayout.EnumPopup("Delimiter", enumerateDelimiterTypes);

            if (enumerateDelimiterTypes == DelimiterTypes.custom)
                enumerateCustomSeparator = EditorGUILayout.TextField("Custom Delimiter", enumerateCustomSeparator);
            else
                enumerateCustomSeparator = "";
            EditorGUI.indentLevel = 1;
            EditorGUILayout.Space();
            EditorGUI.EndDisabledGroup();
        }

        private void DrawPreview()
        {
            ConfigNamePreview();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Preview:");
            EditorGUI.BeginDisabledGroup(true);
            GUILayout.TextArea(previewName);
            EditorGUI.EndDisabledGroup();
            GUILayout.EndHorizontal();
        }

        private void DrawButtons()
        {
            EzRR_Style.DrawHeader("Do Rename On:");
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Hierarchy", "ButtonLeft", GUILayout.Height(EzRR_Style.mediumBtnHeight), GUILayout.MinWidth(100)))
            {
                DoRenameHierarchy();
            }
            if (GUILayout.Button("Project Folder", "ButtonRight", GUILayout.Height(EzRR_Style.mediumBtnHeight), GUILayout.MinWidth(100)))
            {
                DoRenameProjectFolder();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void CopySelectedObjectName()
        {
            GameObject[] _gameObjectsSelected = Selection.gameObjects;
            Object[] _objectsSelected = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
            // If multiple Objects are selected the name that will be placed is from the first Object.
            if (_gameObjectsSelected.Length > 0)
            {
                newName = _gameObjectsSelected[0].name;
            }
            else if (_objectsSelected.Length > 0)
            {
                newName = _objectsSelected[0].name;
            }
            else
            {
                EditorUtility.DisplayDialog(FEEDBACKS.Title._00, FEEDBACKS.Rename._01, FEEDBACKS.Button._00);
            }
        }

        private void DoRenameHierarchy()
        {
            GameObject[] _gameObjectsSelected = Selection.gameObjects;
            // Check for errors before continuing
            if (ErrorsOnHierarchy(_gameObjectsSelected))
                return;
            // Sort the gameobjects inside the array based on the siblin index
            System.Array.Sort(_gameObjectsSelected, delegate (GameObject tempSelection0, GameObject tempSelection1)
            {
                return EditorUtility.NaturalCompare(tempSelection0.transform.GetSiblingIndex().ToString(),
                    tempSelection1.transform.GetSiblingIndex().ToString());
            });

            string _filePrefix = GetPrefix();
            string _fileSuffix = GetSuffix();

            enumerateCounter = enumerateInitNumber;
            ConfigEnumerateFormat();
            // Calculate the amount that each file will increase in the progress bar
            float _result = (float)_gameObjectsSelected.Length / 100f;
            Undo.RecordObjects(_gameObjectsSelected, "rename");
            for (int i = 0; i < _gameObjectsSelected.Length; i++)
            {
                EditorUtility.DisplayProgressBar(FEEDBACKS.Title._02, FEEDBACKS.Rename._00, _result * i);

                finalName = _gameObjectsSelected[i].name;

                if (enumerate)
                {
                    switch (enumerateOptions)
                    {
                        case EnumerateOptions.onBegin:
                            finalName = enumerateCounter.ToString(enumerateFormat) + GetDelimiterType(enumerateDelimiterTypes) + _filePrefix + newName + _fileSuffix;
                            break;

                        case EnumerateOptions.onEnd:
                            finalName = _filePrefix + newName + _fileSuffix + GetDelimiterType(enumerateDelimiterTypes) + enumerateCounter.ToString(enumerateFormat);
                            break;

                        case EnumerateOptions.betweenPrefixAndName:
                            finalName = _filePrefix + enumerateCounter.ToString(enumerateFormat) + GetDelimiterType(enumerateDelimiterTypes) + newName + _fileSuffix;
                            break;

                        case EnumerateOptions.betweenSuffixAndName:
                            finalName = _filePrefix + newName + GetDelimiterType(enumerateDelimiterTypes) + enumerateCounter.ToString(enumerateFormat) + _fileSuffix;
                            break;
                    }

                    enumerateCounter += enumerateIncrement;
                }
                else
                {
                    finalName = _filePrefix + newName + _fileSuffix;
                }
                _gameObjectsSelected[i].name = finalName;
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
                return true;
            }
            // Verifies if all selected gameobjects has the same parent
            else if (EzRR_ErrorCheckers.CheckForDifferentParents(gameObjectsSelected))
            {
                if (EditorUtility.DisplayDialog(FEEDBACKS.Title._00, FEEDBACKS.Rename._02, FEEDBACKS.Button._02, FEEDBACKS.Button._01))
                    return false;
                else
                    return true;
            }
            else
                return false;
        }

        private void DoRenameProjectFolder()
        {
            Object[] _objectsSelected = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
            // Check for errors before continuing
            if (ErrorsOnProjectFolder(_objectsSelected))
                return;
            // Sort the gameobjects inside the array based on name
            System.Array.Sort(_objectsSelected, delegate (Object objectSelected0, Object objectSelected1)
            {
                return EditorUtility.NaturalCompare(objectSelected0.name, objectSelected1.name);
            });
            // Keep the path of the current file
            string _path;
            // The files on project folder can't has the same name
            if (!enumerate)
            {
                enumerate = true;
                EditorUtility.DisplayDialog(FEEDBACKS.Title._00, FEEDBACKS.Rename._05, FEEDBACKS.Button._02);
            }
            // Reset the obj name to prevent conflict
            for (int i = 0; i < _objectsSelected.Length; i++)
            {
                _path = AssetDatabase.GetAssetPath(_objectsSelected[i]);
                AssetDatabase.RenameAsset(_path, "renaming" + i.ToString());
            }
            // Configurate the options
            string _filePrefix = GetPrefix();
            string _fileSuffix = GetSuffix();
            enumerateCounter = enumerateInitNumber;
            ConfigEnumerateFormat();
            // Calculate the amount that each file will increase in the progress bar
            float _result = (float)_objectsSelected.Length / 100f;
            // Rename the files
            for (int i = 0; i < _objectsSelected.Length; i++)
            {
                EditorUtility.DisplayProgressBar(FEEDBACKS.Title._01, FEEDBACKS.Rename._00, _result * i);
                _path = AssetDatabase.GetAssetPath(_objectsSelected[i]);

                if (enumerate)
                {
                    switch (enumerateOptions)
                    {
                        case EnumerateOptions.onEnd:
                            finalName = _filePrefix + newName + _fileSuffix + GetDelimiterType(enumerateDelimiterTypes) + enumerateCounter.ToString(enumerateFormat);
                            break;

                        case EnumerateOptions.onBegin:
                            finalName = enumerateCounter.ToString(enumerateFormat + GetDelimiterType(enumerateDelimiterTypes) + _filePrefix + newName + _fileSuffix);
                            break;

                        case EnumerateOptions.betweenPrefixAndName:
                            finalName = _filePrefix + enumerateCounter.ToString(enumerateFormat + GetDelimiterType(enumerateDelimiterTypes) + newName + _fileSuffix);
                            break;

                        case EnumerateOptions.betweenSuffixAndName:
                            finalName = _filePrefix + newName + GetDelimiterType(enumerateDelimiterTypes) + enumerateCounter.ToString(enumerateFormat + _fileSuffix);
                            break;
                    }
                    enumerateCounter += enumerateIncrement;
                }
                else
                {
                    finalName = _filePrefix + newName + _fileSuffix;
                }
                // If something happens during the renaming get the message and show it on console
                string _message = AssetDatabase.RenameAsset(_path, finalName);
                if (!string.IsNullOrEmpty(_message))
                    Debug.LogWarning(_message);
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
            // Verifies if all the objects are the same type
            else if (EzRR_ErrorCheckers.CheckForDifferentTypes(objectsSelected))
            {
                EditorUtility.DisplayDialog(FEEDBACKS.Title._00, FEEDBACKS.Rename._03, FEEDBACKS.Button._00);
                return true;
            }
            // Verifies the objects extension
            else if (EzRR_ErrorCheckers.CheckForBlockedExtension(objectsSelected))
            {
                EditorUtility.DisplayDialog(FEEDBACKS.Title._00, FEEDBACKS.Rename._04, FEEDBACKS.Button._00);
                return true;
            }
            // Verifies invalid characters
            else if (EzRR_ErrorCheckers.CheckForInvalidCharacters(previewName))
            {
                EditorUtility.DisplayDialog(FEEDBACKS.Title._00, FEEDBACKS.General._02, FEEDBACKS.Button._00);
                return true;
            }
            // Verifies invalid name endings
            else if (EzRR_ErrorCheckers.CheckForInvalidNameEndings(previewName))
            {
                EditorUtility.DisplayDialog(FEEDBACKS.Title._00, FEEDBACKS.General._03, FEEDBACKS.Button._00);
                return true;
            }
            else
                return false;
        }

        private string GetPrefix()
        {
            return (usePrefix) ? filePrefix + GetDelimiterType(prefixDelimiterTypes, "prefix") : "";
        }

        private string GetSuffix()
        {
            return (addSuffix) ? GetDelimiterType(suffixDelimiterTypes, "suffix") + fileSuffix : "";
        }

        private string GetDelimiterType(DelimiterTypes delimiterType, string prefixOrSuffix = "")
        {
            switch (delimiterType)
            {
                case DelimiterTypes.nothing:
                    delimiter = "";
                    break;

                case DelimiterTypes.hyphen:
                    delimiter = "-";
                    break;

                case DelimiterTypes.dot:
                    delimiter = ".";
                    break;

                case DelimiterTypes.space:
                    delimiter = " ";
                    break;

                case DelimiterTypes.underline:
                    delimiter = "_";
                    break;

                case DelimiterTypes.custom:
                    if (string.Equals(prefixOrSuffix, "prefix"))
                        delimiter = prefixCustomSeparator;
                    else if (string.Equals(prefixOrSuffix, "suffix"))
                        delimiter = suffixCustomSeparator;
                    else
                        delimiter = enumerateCustomSeparator;
                    break;
            }

            return delimiter;
        }

        private void ConfigEnumerateFormat()
        {
            switch (enumerateFormatIndex)
            {
                case 0:
                    enumerateFormat = "0";
                    break;

                case 1:
                    enumerateFormat = "00";
                    break;

                case 2:
                    enumerateFormat = "000";
                    break;

                case 3:
                    enumerateFormat = "0000";
                    break;

                case 4:
                    enumerateFormat = "00000";
                    break;
            }
        }

        private void ConfigNamePreview()
        {
            string _filePrefix = GetPrefix();
            string _fileSuffix = GetSuffix();

            enumerateCounter = enumerateInitNumber;
            ConfigEnumerateFormat();

            if (enumerate)
            {
                switch (enumerateOptions)
                {
                    case EnumerateOptions.onBegin:
                        previewName = enumerateCounter.ToString(enumerateFormat) + GetDelimiterType(enumerateDelimiterTypes) + _filePrefix + newName + _fileSuffix;
                        break;

                    case EnumerateOptions.onEnd:
                        previewName = _filePrefix + newName + _fileSuffix + GetDelimiterType(enumerateDelimiterTypes) + enumerateCounter.ToString(enumerateFormat);
                        break;

                    case EnumerateOptions.betweenPrefixAndName:
                        previewName = _filePrefix + enumerateCounter.ToString(enumerateFormat) + GetDelimiterType(enumerateDelimiterTypes) + newName + _fileSuffix;
                        break;

                    case EnumerateOptions.betweenSuffixAndName:
                        previewName = _filePrefix + newName + GetDelimiterType(enumerateDelimiterTypes) + enumerateCounter.ToString(enumerateFormat) + _fileSuffix;
                        break;
                }
            }
            else
            {
                previewName = _filePrefix + newName + _fileSuffix;
            }
        }
    }
}