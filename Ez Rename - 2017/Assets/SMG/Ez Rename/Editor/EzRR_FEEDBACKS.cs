namespace SMG.EzRenamer.FEEDBACKS
{
	public class Title
	{
		public const string _00 = "Attention!";
		public const string _01 = "Renaming...";
		public const string _02 = "Replacing...";
		public const string _03 = "Inserting...";
		public const string _04 = "Removing...";
		public const string _05 = "Changing...";
		public const string _06 = "Sorting...";
	}
	
	public class Button
	{
		public const string _00 = "Close";
		public const string _01 = "Cancel";
		public const string _02 = "Continue";
	}
	
	public class General
	{
		public const string _00 = "There is no GameObject selected. Please, select one or more GameObjects and try again.";
		public const string _01 = "There is no File selected. Please, select one or more Files and try again.";
		public const string _02 = "The Files names cannot contain: <, >, :, \", /, \\, |, ? and *";
		public const string _03 = "The Files names cannot finish with SPACE or DOT.";
	}
	
	public class Rename
	{
		public const string _00 = "Wait until renaming is finished.";
		public const string _01 = "There's no GameObject or Asset selected. Select one of them before trying to copy their name.";
		public const string _02 = "One or more gameobjects has different parents and it may cause a problem on your Hierarchy. Do want to continue?";
		public const string _03 = "To prevent mistakes it isn't possible to rename files with different extensions. Please, select files with the same extensions and try again.";
		public const string _04 = "To prevent mistakes it isn't possible to rename with extensions .cs, .js and .shader.";
		public const string _05 = "It isn't possible to has files with the same name inside Project Folder. Enumerate has been activated and initial number is 0.";
	}
	
	public class Replace
	{
		public const string _00 = "Wait until replacing is finished.";
		public const string _01 = "You must enter a string to be replaced.";
	}
	
	public class Insert
	{
		public const string _00 = "Wait until inserting is finished.";
		public const string _01 = "You must enter a string to be inserted.";
	}
	
	public class Remove
	{
		public const string _00 = "Wait until removing is finished.";
	}
	
	public class CaseChange
	{
		public const string _00 = "Wait until changing is finished.";
	}
	
	public class Sort
	{
		public const string _00 = "Wait until sorting is finished.";
		public const string _01 = "To prevent mistakes it isn't possible to sort the children of multiple gameObjects.";
		public const string _02 = "To prevent mistakes it isn't possible to sort gameObjects that has different parents.";
	}
}