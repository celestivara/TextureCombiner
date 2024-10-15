using UnityEngine;
using UnityEditor;
using System.IO;

public class TextureCombiner : EditorWindow
{
    private Texture2D baseMap;
    private Texture2D smoothnessMap;
    private bool invertSmoothness = false;
    private string savePath = "";

    [MenuItem("Tools/Texture Combiner")]
    public static void ShowWindow()
    {
        GetWindow<TextureCombiner>("Texture Combiner");
    }

    private void OnGUI()
    {
        baseMap = (Texture2D)EditorGUILayout.ObjectField("Base Map", baseMap, typeof(Texture2D), false);
        smoothnessMap = (Texture2D)EditorGUILayout.ObjectField("Smoothness Map", smoothnessMap, typeof(Texture2D), false);
        invertSmoothness = EditorGUILayout.Toggle("Invert Smoothness", invertSmoothness);
        
        if (GUILayout.Button("Select Save Path"))
        {
            savePath = EditorUtility.SaveFilePanelInProject("Save Combined Texture", "CombinedTexture", "png", "Please enter a file name to save the texture to");
        }

        if (GUILayout.Button("Combine Textures"))
        {
            CombineTextures();
        }
    }

    private void CombineTextures()
    {
        if (baseMap == null || smoothnessMap == null || string.IsNullOrEmpty(savePath))
        {
            Debug.LogError("Please set all fields before combining textures.");
            return;
        }

        Texture2D combinedTexture = new Texture2D(baseMap.width, baseMap.height, TextureFormat.RGBA32, false);

        for (int y = 0; y < baseMap.height; y++)
        {
            for (int x = 0; x < baseMap.width; x++)
            {
                Color baseColor = baseMap.GetPixel(x, y);
                Color smoothnessColor = smoothnessMap.GetPixel(x, y);
                
                float smoothness = smoothnessColor.grayscale;
                if (invertSmoothness)
                {
                    smoothness = 1 - smoothness;
                }

                Color finalColor = new Color(baseColor.r, baseColor.g, baseColor.b, smoothness);
                combinedTexture.SetPixel(x, y, finalColor);
            }
        }

        combinedTexture.Apply();

        byte[] pngData = combinedTexture.EncodeToPNG();
        File.WriteAllBytes(savePath, pngData);

        AssetDatabase.Refresh();
        Debug.Log("Texture saved to: " + savePath);
    }
}