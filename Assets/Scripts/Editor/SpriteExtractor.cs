using System.IO;
using System.Linq;
using Game;
using Game.Models;
using UnityEditor;
using UnityEngine;
using UtilityToolkit.Runtime;

namespace Editor
{
    public class SpriteExtractor : EditorWindow
    {
        [MenuItem("Assets/Extract Sprites to PNGs")]
        private static void ExtractSprites()
        {
            // Get the selected texture in the Project window
            CardSprites cardSprites = Selection.activeObject as CardSprites;

            if (cardSprites == null)
            {
                Debug.LogError("Please select a CardSprites asset first.");
                return;
            }

            string path = AssetDatabase.GetAssetPath(cardSprites);

            // Create an output directory
            string outputFolder = Path.Combine(Path.GetDirectoryName(path), cardSprites.name + "_Extracted");
            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }
            
            foreach (Card card in Card.FullDeck)
            {
                Sprite sprite = cardSprites.Get(card);
                if (sprite == null) continue;

                // 1. Cast dimensions to integers upfront so they match perfectly
                int width = (int)sprite.rect.width;
                int height = (int)sprite.rect.height;
                int x = (int)sprite.textureRect.x;
                int y = (int)sprite.textureRect.y;

                // 2. Create the texture using those exact integers
                Texture2D newTex = new Texture2D(width, height);
    
                // 3. Grab the pixels using the exact same dimensions
                Color[] pixels = sprite.texture.GetPixels(x, y, width, height);
    
                newTex.SetPixels(pixels);
                newTex.Apply();

                // Encode to PNG and save
                byte[] bytes = newTex.EncodeToPNG();
                string spritePath = Path.Combine(outputFolder, card + ".png");
                File.WriteAllBytes(spritePath, bytes);
    
                // Clean up memory
                DestroyImmediate(newTex);
            }

            AssetDatabase.Refresh();
            Debug.Log($"Successfully extracted sprites to: {outputFolder}");
        }

        [MenuItem("Assets/Assign New Sprites")]
        private static void AssignNewSprites()
        {
            CardSprites cardSprites = Selection.activeObject as CardSprites;

            if (cardSprites == null)
            {
                Debug.LogError("Please select a CardSprites asset first.");
                return;
            }

            string folderPath = "Assets/Configs/Board Card Sprites_Extracted";

            Sprite[] sprites = AssetDatabase.FindAssets("t:Sprite", new[] { folderPath })
                .Select(guid => AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GUIDToAssetPath(guid)))
                .ToArray();
            
            Debug.Log($"Found {sprites.Length} sprites in folder: {folderPath}");
            
            foreach (Sprite sprite in sprites)
            { 
                var option = Card.FullDeck.FirstOption(c => $"{c.ToString()}_0" == sprite.name);
                if (option.IsSome(out var card))
                {
                    cardSprites.Set(card, sprite);
                }
                else
                {
                    Debug.LogWarning($"No matching card found for sprite: {sprite.name} in Card.FullDeck");
                    return;
                }
            }
        }
    }
}