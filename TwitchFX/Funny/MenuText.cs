using IPA.Old;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TwitchFX
{
    public class MenuText
    {
        public static MenuText instance;

        // path to the file to load text from
        private const string FILE_PATH = "/UserData/CustomMenuText.txt";
        // path to load the font prefab from
        private const string FONT_PATH = "UserData/CustomMenuFont";
        // prefab to instantiate when creating the TextMeshPros
        public static GameObject textPrefab;
        // used if we can't load any custom entries
        public static readonly string[] DEFAULT_TEXT = { "BEAT", "SABER" };
        public static readonly Color defaultMainColor = Color.red;
        public static readonly Color defaultBottomColor = new Color(0, 0.5019608f, 1);

        // Store the text objects so when we leave the menu and come back, we aren't creating a bunch of them
        public static TextMeshPro mainText;
        public static TextMeshPro bottomText; // BOTTOM TEXT

        public System.Random random;

        public static readonly string[] SUDO = {"SUDO IS", "CUTE"};

        public static void SceneManagerOnActiveSceneChanged(Scene arg0, Scene arg1)
        {
            if (arg1.name.Contains("Menu")) // Only run in menu scene
            {
                setText(SUDO);
            }
        }

        private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
           
        }

        public static GameObject loadTextPrefab(string path)
        {
            GameObject prefab;
            string fontPath = Path.Combine(Environment.CurrentDirectory, path);
            if (!File.Exists(fontPath))
            {
                File.WriteAllBytes(fontPath, TwitchFX.neontubes_data.rawData);
            }
            AssetBundle fontBundle = AssetBundle.LoadFromFile(fontPath);
            prefab = fontBundle.LoadAsset<GameObject>("Text");
            if (prefab == null)
            {
                Console.WriteLine("[CustomMenuText] No text prefab found in the provided AssetBundle! Using NeonTubes.");
                AssetBundle beonBundle = AssetBundle.LoadFromMemory(TwitchFX.neontubes_data.rawData);
                prefab = beonBundle.LoadAsset<GameObject>("Text");
            }

            return prefab;
        }

        /// <summary>
        /// Replaces the logo in the main menu (which is an image and not text
        /// as of game version 0.12.0) with an editable TextMeshPro-based
        /// version. Performs only the necessary steps (if the logo has already
        /// been replaced, restores the text's position and color to default
        /// instead).
        /// Warning: Only call this function from the main menu scene!
        /// 
        /// Code generously donated by Kyle1413; edited some by Arti
        /// </summary>
        public static void replaceLogo()
        {
            // Since 0.13.0, we have to create our TextMeshPros differently! You can't change the font at runtime, so we load a prefab with the right font from an AssetBundle. This has the side effect of allowing for custom fonts, an oft-requested feature.
            if (textPrefab == null) textPrefab = loadTextPrefab(FONT_PATH);

            // Logo Top Pos : 0.63, 21.61, 24.82
            // Logo Bottom Pos : 0, 17.38, 24.82
            if (mainText == null) mainText = GameObject.Find("CustomMenuText")?.GetComponent<TextMeshPro>();
            if (mainText == null)
            {
                GameObject textObj = GameObject.Instantiate(textPrefab);
                textObj.name = "CustomMenuText";
                textObj.SetActive(false);
                mainText = textObj.GetComponent<TextMeshPro>();
                mainText.alignment = TextAlignmentOptions.Center;
                mainText.fontSize = 12;
                mainText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 2f);
                mainText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 2f);
                mainText.richText = true;
                textObj.transform.localScale *= 3.7f;
                mainText.overflowMode = TextOverflowModes.Overflow;
                mainText.enableWordWrapping = false;
                textObj.SetActive(true);
            }
            mainText.rectTransform.position = new Vector3(0f, 21.61f, 24.82f);
            mainText.color = defaultMainColor;
            mainText.text = "BEAT";

            if (bottomText == null) bottomText = GameObject.Find("CustomMenuText-Bot")?.GetComponent<TextMeshPro>();
            if (bottomText == null)
            {
                GameObject textObj2 = GameObject.Instantiate(textPrefab);
                textObj2.name = "CustomMenuText-Bot";
                textObj2.SetActive(false);
                bottomText = textObj2.GetComponent<TextMeshPro>();
                bottomText.alignment = TextAlignmentOptions.Center;
                bottomText.fontSize = 12;
                bottomText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 2f);
                bottomText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 2f);
                bottomText.richText = true;
                textObj2.transform.localScale *= 3.7f;
                bottomText.overflowMode = TextOverflowModes.Overflow;
                bottomText.enableWordWrapping = false;
                textObj2.SetActive(true);
            }
            bottomText.rectTransform.position = new Vector3(0f, 17f, 24.82f);
            bottomText.color = defaultBottomColor;
            mainText.text = "SABER";

            // Destroy Default Logo
            GameObject defaultLogo = FindUnityObjectsHelper.GetAllGameObjectsInLoadedScenes().Where(go => go.name == "Logo").FirstOrDefault();
            if (defaultLogo != null) GameObject.Destroy(defaultLogo);
        }

        /// <summary>
        /// Sets the text in the main menu (which normally reads BEAT SABER) to
        /// the text of your choice. TextMeshPro formatting can be used here.
        /// Additionally:
        /// - If the text is exactly 2 lines long, the first line will be
        ///   displayed in red, and the second will be displayed in blue.
        /// Warning: Only call this function from the main menu scene!
        /// </summary>
        /// <param name="lines">
        /// The text to display, separated by lines (from top to bottom).
        /// </param>
        public static void setText(string[] lines)
        {
            // Set up the replacement logo
            replaceLogo();

            if (lines.Length == 2)
            {
                mainText.text = lines[0];
                bottomText.text = lines[1];
            }
            else
            {
                // Hide the bottom line entirely; we're just going to use the main one
                bottomText.text = "";

                // Center the text vertically (halfway between the original positions)
                Vector3 newPos = mainText.transform.position;
                newPos.y = (newPos.y + bottomText.transform.position.y) / 2;
                mainText.transform.position = newPos;

                // Set text color to white by default (users can change it with formatting anyway)
                mainText.color = Color.white;

                // Set the text
                mainText.text = String.Join("\n", lines);
            }
        }
    }
}