using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using System.IO;

namespace SMG.EzRenamer
{
	public class EzRR_ErrorCheckers : MonoBehaviour 
	{
		public static bool CheckForDifferentParents(GameObject[] gameObjectSelection)
		{
			List<Transform> _parents = new List<Transform>();
			for (int i = 0; i < gameObjectSelection.Length; i++)
			{
				if (!_parents.Contains(gameObjectSelection[i].transform.parent))
					_parents.Add(gameObjectSelection[i].transform.parent);

				if (_parents.Count > 1)
				{
					return true;
				}
			}
			
			return false;
		}
		
		public static bool CheckForDifferentTypes(Object[] objectsSelected)
		{
			List<string> _types = new List<string>();
			for (int i = 0; i < objectsSelected.Length; i++)
			{
				if (!_types.Contains(Path.GetExtension(AssetDatabase.GetAssetPath(objectsSelected[i])).ToString()))
					_types.Add(Path.GetExtension(AssetDatabase.GetAssetPath(objectsSelected[i])).ToString());

				if (_types.Count > 1)
				{
					return true;
				}
			}
			return false;
		}
		
		public static bool CheckForBlockedExtension(Object[] objectsSelected)
		{
			string _extension = "";
			_extension = Path.GetExtension(AssetDatabase.GetAssetPath(objectsSelected[0])).ToString();
			if (string.Equals(_extension, ".cs") || string.Equals(_extension, ".js") || string.Equals(_extension, ".shader"))
			{
				return true;
			}
			return false;
		}
		
		public static bool CheckForInvalidCharacters(string value)
		{
			// Verifies invalid characters
			if(value.Contains("<") || 
				value.Contains(">") || 
				value.Contains(":") || 
				value.Contains('"') || 
				value.Contains("/") || 
				value.Contains("\\") || 
				value.Contains("|") || 
				value.Contains("?") || 
				value.Contains("*"))
			{
				return true;
			}
			return false;
		}
		
		public static bool CheckForInvalidNameEndings(string value)
		{
			if(value.EndsWith(" ") || value.EndsWith("."))
			{
				return true;
			}
			return false;
		}
		
		public static bool CheckForStringNullOrEmpty(string value)
		{
			if(string.IsNullOrEmpty(value))
			{
				return true;
			}
			return false;
		}
	}
}