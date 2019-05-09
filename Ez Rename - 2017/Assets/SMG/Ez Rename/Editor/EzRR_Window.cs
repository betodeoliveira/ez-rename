using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using System.IO;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace SMG.EzRenamer
{
    public class EzRR_Window : EditorWindow
    {
        private Vector2 scrollPos;

        private static Texture2D ezRenameIcon;

        private EzRR_Rename rename;
        private EzRR_Replace replace;
        private EzRR_Insert insert;
        private EzRR_Remove remove;
        private EzRR_CaseChange caseChange;
        private EzRR_Sort sort;


        private void OnEnable()
        {
            rename = ScriptableObject.CreateInstance<EzRR_Rename>();
            replace = ScriptableObject.CreateInstance<EzRR_Replace>();
            insert = ScriptableObject.CreateInstance<EzRR_Insert>();
            remove = ScriptableObject.CreateInstance<EzRR_Remove>();
            caseChange = ScriptableObject.CreateInstance<EzRR_CaseChange>();
            sort = ScriptableObject.CreateInstance<EzRR_Sort>();
        }

        private void OnGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            EditorGUILayout.Space();
            rename.Draw();
            replace.Draw();
            insert.Draw();
            remove.Draw();
            caseChange.Draw();
            sort.Draw();
            EzRR_Style.DrawUILine(EzRR_Style.uiLineColor);
            EditorGUILayout.EndScrollView();
        }

        #region ========== Menu Items ==========================================
        [MenuItem("Window/Ez Rename/Open")]
        private static void OpenWindow()
        {
            GUIContent _titleContent = new GUIContent("Ez Rename");
            ezRenameIcon = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/SMG/Ez Rename/Editor/Editor Resources/Icons/ez-rename-icon.png", typeof(Texture2D));
            _titleContent.image = ezRenameIcon;
            EditorWindow _window = EditorWindow.GetWindow(typeof(EzRR_Window));
            _window.minSize = new Vector2(300, 415);
            _window.autoRepaintOnSceneChange = true;
            _window.titleContent = _titleContent;
            _window.Show();
        }

        [MenuItem("Window/Ez Rename/Guide")]
        private static void Guide()
        {
            Application.OpenURL("https://solomidgames.com/guides/ez-rename/quick-overview.html");
        }

        [MenuItem("Window/Ez Rename/Help")]
        private static void Help()
        {
            Application.OpenURL("mailto:help@solomidgames.com");
        }

        [MenuItem("Window/Ez Rename/Forum Thread")]
        private static void ForumThread()
        {
            Application.OpenURL("https://forum.unity.com/threads/released-ez-files-renamer.300182/");
        }

        [MenuItem("Window/Ez Rename/More Assets")]
        private static void MoreAssets()
        {
            Application.OpenURL("https://www.assetstore.unity3d.com/en/#!/search/page=1/sortby=popularity/query=publisher:11524");
        }

        [MenuItem("Window/Ez Rename/Website")]
        private static void Website()
        {
            Application.OpenURL("https://solomidgames.com");
        }

        [MenuItem("Window/Ez Rename/Follow us on Twitter")]
        private static void TwitterFollow()
        {
            Application.OpenURL("https://twitter.com/solomidgames");
        }

        // Open Window
        [MenuItem("GameObject/Ez Rename/Open Window", false, 48)]
        private static void MenuOpenWindow() { OpenWindow(); }
        // Sort Selection
        [MenuItem("GameObject/Ez Rename/Sort Selection/Name A_Z", false, 49)]
        private static void MenuSortSelectNameA_Z() { EzRR_Sort.ShortcutSortConfig(EzRR_Sort.SortOptions.nameA_Z, false); }
        [MenuItem("GameObject/Ez Rename/Sort Selection/Name Z_A", false, 50)]
        private static void MenuSortSelectNameZ_A() { EzRR_Sort.ShortcutSortConfig(EzRR_Sort.SortOptions.nameZ_A, false); }

        // Sort Children
        [MenuItem("GameObject/Ez Rename/Sort Children/Name A_Z", false, 49)]
        private static void MenuSortChildNameA_Z() { EzRR_Sort.ShortcutSortConfig(EzRR_Sort.SortOptions.nameA_Z, true); }
        [MenuItem("GameObject/Ez Rename/Sort Children/Name Z_A", false, 50)]
        private static void MenuSortChildNameZ_A() { EzRR_Sort.ShortcutSortConfig(EzRR_Sort.SortOptions.nameZ_A, true); }
        #endregion ======= Menu Items ==========================================
    }
}