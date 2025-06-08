#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

/*
Ho ho ho
You have made it into the code of Alimad.Main
If ur looking for smth, you may also look at https://github.com/Alimadcorp/webbuildtools (Documentation)
*/

namespace Alimad.Main
{
    public class InstallWebBuildTools : EditorWindow
    {
        private bool notSelected = false;
        private List<BuildEntry> buildList = new List<BuildEntry>();
        private Vector2 scrollPos;
        private string outFolder = "";
        [MenuItem("Window/Alimad Co/Web Build Tools")]
        public static void ShowWindow()
        {
            GetWindow<InstallWebBuildTools>("Web Build Tools");
        }
        public void OnGUI()
        {
            GUILayout.Label("Alimad Web Build Tools", EditorStyles.boldLabel);
            if(notSelected && PlayerSettings.WebGL.template != "APPLICATION:MadWeb Template") { SetTemplate("APPLICATION:MadWeb Template"); notSelected = false; }
            if (GUILayout.Button("Install Templates"))
            {
                InstallTemplates();
                notSelected = true;
            }

            EditorGUILayout.LabelField("Multi Web Build List", EditorStyles.boldLabel);
            if (GUILayout.Button("Add New Build"))
            {
                buildList.Add(new BuildEntry(buildList.Count));
            }

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            int deleteIndex = 0;
            for (int i = 0; i < buildList.Count; i++)
            {
                EditorGUILayout.BeginVertical("box");

                buildList[i].version = "v" + (i + 1);
                GUILayout.Label(buildList[i].version);
                buildList[i].name = EditorGUILayout.TextField("Name:", buildList[i].name);

                EditorGUILayout.BeginHorizontal();
                buildList[i].address = EditorGUILayout.TextField("Location:", buildList[i].address);
                if (GUILayout.Button("Browse", GUILayout.Width(80)))
                {
                    string selectedFolder = EditorUtility.OpenFolderPanel("Select Build Folder", buildList[i].address, "");
                    if (!string.IsNullOrEmpty(selectedFolder))
                    {
                        buildList[i].address = selectedFolder;
                    }
                }
                EditorGUILayout.EndHorizontal();

                if (GUILayout.Button("Remove"))
                {
                    deleteIndex = i;
                }

                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndScrollView();
            if (deleteIndex > 0) { buildList.RemoveAt(deleteIndex); }

            GUILayout.Label("Select Output Folder");
            outFolder = EditorGUILayout.TextField("Location:", outFolder);
            if (GUILayout.Button("Browse", GUILayout.Width(80)))
            {
                string selectedFolder = EditorUtility.OpenFolderPanel("Select Output Folder", outFolder, "");
                if (!string.IsNullOrEmpty(selectedFolder))
                {
                    outFolder = selectedFolder;
                }
            }
            if (GUILayout.Button("Export Multi Web Build"))
            {
                ExportBuild();
            }
            
            if (GUILayout.Button("View Video Tutorial"))
            {
                Application.OpenURL("https://www.youtube.com/watch?v=Wr8rXc35Y0Y");
            }
        }
        public void SetTemplate(string tamplateName)
        {
            PlayerSettings.WebGL.template = tamplateName;
        }
        public string WBTFolder(bool arc)
        {
            string defaultPath = Application.dataPath + "/WebBuildTools"; // Ideal path
            if (Directory.Exists(defaultPath)) { return defaultPath + (arc ? "" : "/Templates"); }
            defaultPath = Application.dataPath + "/Unity Technologies/WebBuildTools"; // Look inside the unity techs folder
            if (Directory.Exists(defaultPath)) { return defaultPath + (arc ? "" : "/Templates"); }

            Debug.LogError("WebBuildTools folder not found in the Assets directory.");
            return null;
        }
        public void InstallTemplates()
        {
            string FPD = WBTFolder(false);   // Current folder of the web build assets
            Debug.Log(FPD);
            if (FPD == null)
            {
                EditorUtility.DisplayDialog("Error", "Assets/Web Build Tools not found in the project or are corrupted. Please consider reinstall", "OK");
                Application.OpenURL(WBTFolder(true) + "/Documentation/Documentation.html#howtoinstall");
                return;
            }
            try
            {
                // Look I am too stupid to do something cleaner than this
                if (!CopyFiles(FPD + "/MadWeb Template",
                EditorApplication.applicationPath.Replace("Unity.exe", "") + "Data/PlaybackEngines/WebGLSupport/BuildTools/WebGLTemplates/MadWeb Template")) { return; }
                if (!CopyFiles(FPD + "/MadWeb Template/TemplateData",
                EditorApplication.applicationPath.Replace("Unity.exe", "") + "Data/PlaybackEngines/WebGLSupport/BuildTools/WebGLTemplates/MadWeb Template/TemplateData")) { return; }
                if (!CopyFiles(FPD + "/MadWeb Light",
                EditorApplication.applicationPath.Replace("Unity.exe", "") + "Data/PlaybackEngines/WebGLSupport/BuildTools/WebGLTemplates/MadWeb Light")) { return; }
                if (!CopyFiles(FPD + "/MadWeb Light/TemplateData",
                EditorApplication.applicationPath.Replace("Unity.exe", "") + "Data/PlaybackEngines/WebGLSupport/BuildTools/WebGLTemplates/MadWeb Light/TemplateData")) { return; }
                if (!CopyFiles(FPD + "/MadWeb Color",
                EditorApplication.applicationPath.Replace("Unity.exe", "") + "Data/PlaybackEngines/WebGLSupport/BuildTools/WebGLTemplates/MadWeb Color")) { return; }
                if (!CopyFiles(FPD + "/MadWeb Color/TemplateData",
                EditorApplication.applicationPath.Replace("Unity.exe", "") + "Data/PlaybackEngines/WebGLSupport/BuildTools/WebGLTemplates/MadWeb Color/TemplateData")) { return; }
                SetTemplate("APPLICATION:MadWeb Template");
                EditorUtility.DisplayDialog("Install Web Build Tools", "Web Build Tools have been installed successfully! You can now export a web build or open README.md for more info.", "Yayy : )", "OK");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"An error occurred: {ex.Message}\n{ex.StackTrace}");
                EditorUtility.DisplayDialog("Error", $"An error occurred while installing Web Build Tools:\n{ex.Message}", "OK");
            }
        }

        private bool CopyFiles(string sourcePath, string targetPath)
        {
            if (!Directory.Exists(sourcePath))
            {
                Debug.LogError($"Source directory not found: {sourcePath}");
                EditorUtility.DisplayDialog("Error", $"Source directory not found: {sourcePath}", "OK");
                return false;
            }
            if (!Directory.Exists(targetPath))
            {
                Directory.CreateDirectory(targetPath);
            }
            foreach (var file in Directory.GetFiles(sourcePath))
            {
                try
                {
                    string extension = Path.GetExtension(file);
                    if (extension != ".meta")
                    {
                        try{
                        string fileName = Path.GetFileName(file);
                        File.Copy(file, Path.Combine(targetPath, fileName), true);
                        }
                        catch(System.UnauthorizedAccessException ex){
                            // Cuz unity is installed in C://Program Files most of the time, we won't have access to it
                            EditorUtility.DisplayDialog("Error", $"Copy access denied to {targetPath}.", "OK");
                            Application.OpenURL(WBTFolder(true) + "/Documentation/Errors/ifnoaccess.txt");
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"Failed to copy file: {file}. Error: {ex.Message}");
                    return false;
                }
            }
            return true;
        }
        public void ExportBuild()
        {
            string sourceHTMLPath = WBTFolder(true) + "/Multi Web Build/";

            if (!File.Exists(sourceHTMLPath + "index.html"))
            {
                Debug.LogError($"Multi Web Build HTML file not found at: {sourceHTMLPath}index.html");
                return;
            }

            if (!Directory.Exists(outFolder))
            {
                EditorUtility.DisplayDialog("Error", "Output Folder does not exist", "OK");
                return;
            }
            if (Directory.GetFiles(outFolder).Length != 0)
            {
                if (!File.Exists(outFolder + "/index.html"))
                {
                    Debug.LogWarning("Copied files but output Folder was not empty");
                }
            }
            for (int i = 0; i < buildList.Count; i++)
            {
                if (!Directory.Exists(buildList[i].address))
                {
                    EditorUtility.DisplayDialog("Error", $"{buildList[i].address} does not exist", "OK");
                    return;
                }
                if (!File.Exists(buildList[i].address + "/index.html"))
                {
                    EditorUtility.DisplayDialog("Error", $"{buildList[i].address}/index.html does not exist", "OK");
                    return;
                }
                string currDir = outFolder + "/v" + (i + 1).ToString();
                if (!Directory.Exists(currDir)) { Directory.CreateDirectory(currDir); }
                if (!CopyFiles(buildList[i].address, currDir)) { EditorUtility.DisplayDialog("Error", $"Failed to copy {buildList[i].name}.", "OK"); }
                if (!Directory.Exists(currDir + "/TemplateData")) { Directory.CreateDirectory(currDir + "/TemplateData"); }
                if (!CopyFiles(buildList[i].address + "/TemplateData", currDir + "/TemplateData")) { EditorUtility.DisplayDialog("Error", $"Failed to copy {buildList[i].name}.", "OK"); }
                if (!Directory.Exists(currDir + "/Build")) { Directory.CreateDirectory(currDir + "/Build"); }
                if (!CopyFiles(buildList[i].address + "/Build", currDir + "/Build")) { EditorUtility.DisplayDialog("Error", $"Failed to copy {buildList[i].name}.", "OK"); }
                Debug.Log($"Copied {buildList[i].name}");
            }
            // Copy font
            try { File.Copy(sourceHTMLPath + "font.ttf", outFolder + "/font.ttf", true); }
            catch (System.Exception ex) { EditorUtility.DisplayDialog("Error", $"Failed to copy font.ttf", "OK"); Debug.LogError($"Failed to copy font.ttf. Error:{ex.Message}"); }
            Debug.Log("Build Copy Done");

            string toInsert = "";
            string targetHTMLPath = outFolder + "/index.html";
            string injectionPoint = "<!--HTML Injection Point-->";
            try
            {
                string htmlContent = File.ReadAllText(sourceHTMLPath + "index.html");

                for (int i = 0; i < buildList.Count; i++)
                {
                    string customHTML = $"<a href=\"#\" onclick=\"loadVersion('v{i + 1}')\">{buildList[i].name}</a><br><br>";
                    toInsert += customHTML + "\n";
                }
                if (htmlContent.Contains(injectionPoint))
                {
                    htmlContent = htmlContent.Replace(injectionPoint, injectionPoint + "\n" + toInsert);
                }
                else
                {
                    Debug.LogWarning("Injection point not found in the HTML file.");
                    return;
                }

                File.WriteAllText(targetHTMLPath, htmlContent, Encoding.UTF8);

                Debug.Log($"Multi Web Build saved at: {outFolder}");
                Application.OpenURL(outFolder);

                AssetDatabase.Refresh();
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error while injecting HTML: {ex.Message}");
            }
        }
    }
    [System.Serializable]
    class BuildEntry
    {
        public string version = "";       // Build version
        public string name = "";           // Build name
        public string address = "";        // Build folder location
        public BuildEntry(int l)
        {
            this.name = "Version " + (l + 1).ToString();
        }
    }
    [System.Serializable]
    class BuildListWrapper
    {
        public List<BuildEntry> builds;
    }
}
#endif