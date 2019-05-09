using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using System.IO;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace SMG.EzRenamer
{
	public class EzRR_Insert : ScriptableObject 
	{
		private bool showOption = false;
		
		private enum InsertOptions
		{
			beginning,
			end,
			custom
		}
		private InsertOptions insertOption;
		
		private string stringToInsert;
		private int insertIndex;
		private string insertNewName;
		
		public void Draw()
		{
			EzRR_Style.DrawUILine(EzRR_Style.uiLineColor);
			EditorGUILayout.BeginHorizontal();
			showOption = EzRR_Style.DrawFoldoutHeader("Insert", showOption);
			EzRR_Style.DrawHelpButton("https://solomidgames.com/guides/ez-rename/insert.html");
			EditorGUILayout.EndHorizontal();
			if (showOption)
			{
				EditorGUI.indentLevel = 1;
				DrawInsert();
				EditorGUILayout.Space();
				EditorGUI.indentLevel = 0;
				DrawButtons();
			}
		}
		
		private void DrawInsert()
		{
			insertOption = (InsertOptions)EditorGUILayout.EnumPopup("Insert On", insertOption);
			if(insertOption == InsertOptions.custom)
			{
				insertIndex = EditorGUILayout.IntField("Start Index", insertIndex);
				if(insertIndex < 0)
					insertIndex = 0;
			}
				
			stringToInsert = EditorGUILayout.TextField("Insert:", stringToInsert);
		}
		
		private void DrawButtons()
		{
			EzRR_Style.DrawHeader("Do Insert On:");
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Hierarchy", "ButtonLeft", GUILayout.Height(EzRR_Style.mediumBtnHeight), GUILayout.MinWidth(100)))
            {
                DoInsertHierarchy();
            }
            if (GUILayout.Button("Project Folder", "ButtonRight", GUILayout.Height(EzRR_Style.mediumBtnHeight), GUILayout.MinWidth(100)))
            {
                DoInserProjectFolder();
            }
            EditorGUILayout.EndHorizontal();
		}
		
		private void DoInsertHierarchy()
		{
			GameObject[] _gameObjectsSelected = Selection.gameObjects;
			// Check for errors before continuing
			if(ErrorsOnHierarchy(_gameObjectsSelected))
				return;
			// Calculate the amount that each file will increase in the progress bar
			float _result = (float)_gameObjectsSelected.Length / 100f;
			Undo.RecordObjects(_gameObjectsSelected, "insert");
			for (int i = 0; i < _gameObjectsSelected.Length; i++)
			{
				EditorUtility.DisplayProgressBar(FEEDBACKS.Title._03, FEEDBACKS.Insert._00, _result * i);
				insertNewName = _gameObjectsSelected[i].name;
				if(insertOption == InsertOptions.beginning)
				{
					insertNewName = insertNewName.Insert(0, stringToInsert);
				}
				else if(insertOption == InsertOptions.end)
				{
					int _index = insertNewName.Length;
					insertNewName = insertNewName.Insert(_index, stringToInsert);
				}
				else
				{
					if(insertIndex > insertNewName.Length)
						insertIndex = insertNewName.Length;
					insertNewName = insertNewName.Insert(insertIndex, stringToInsert);
				}
				_gameObjectsSelected[i].name = insertNewName;
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
			// Verifies if the insert value is empty or null
			else if(EzRR_ErrorCheckers.CheckForStringNullOrEmpty(stringToInsert))
			{
				EditorUtility.DisplayDialog(FEEDBACKS.Title._00, FEEDBACKS.Insert._01, FEEDBACKS.Button._00);
				return true;
			}
			else
				return false;
		}
		
		private void DoInserProjectFolder()
		{
			Object[] _objectsSelected = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
			// Check for errors before continuing
			if(ErrorsOnProjectFolder(_objectsSelected))
				return;
			// Keep the path of the current file
			string _path;
			// Calculate the amount that each file will increase in the progress bar
			float _result = (float)_objectsSelected.Length / 100f;
			for (int i = 0; i < _objectsSelected.Length; i++)
			{
				EditorUtility.DisplayProgressBar(FEEDBACKS.Title._02, FEEDBACKS.Replace._00, _result * i);
				_path = AssetDatabase.GetAssetPath(_objectsSelected[i]);
				insertNewName = _objectsSelected[i].name;
				if(insertOption == InsertOptions.beginning)
				{
					insertNewName = insertNewName.Insert(0, stringToInsert);
				}
				else if(insertOption == InsertOptions.end)
				{
					int _index = insertNewName.Length;
					insertNewName = insertNewName.Insert(_index, stringToInsert);
				}
				else
				{
					if(insertIndex > insertNewName.Length)
						insertIndex = insertNewName.Length;
					insertNewName = insertNewName.Insert(insertIndex, stringToInsert);
				}
				if(EzRR_ErrorCheckers.CheckForInvalidNameEndings(insertNewName))
				{
					Debug.LogWarning("Skipping " + _objectsSelected[i].name + ":\n" + FEEDBACKS.General._03);
				}
				else
				{
					// If something happens during the replacing get the message and show it on console
					string _message = AssetDatabase.RenameAsset(_path, insertNewName);
					if(!string.IsNullOrEmpty(_message))
						Debug.LogWarning(_message);
				}
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
			// Verifies if the insert value is empty or null
			else if(EzRR_ErrorCheckers.CheckForStringNullOrEmpty(stringToInsert))
			{
				EditorUtility.DisplayDialog(FEEDBACKS.Title._00, FEEDBACKS.Insert._01, FEEDBACKS.Button._00);
				return true;
			}
			// Verifies invalid characters
			else if(EzRR_ErrorCheckers.CheckForInvalidCharacters(stringToInsert))
			{
				EditorUtility.DisplayDialog(FEEDBACKS.Title._00, FEEDBACKS.General._02, FEEDBACKS.Button._00);
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}