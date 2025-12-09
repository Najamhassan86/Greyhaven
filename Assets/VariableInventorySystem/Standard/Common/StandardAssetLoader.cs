using System;
using System.Collections;
using UnityEngine;

namespace VariableInventorySystem
{
    public class StandardAssetLoader
    {
        public virtual IEnumerator LoadAsync(IVariableInventoryAsset imageAsset, Action<Texture2D> onLoad)
        {
            string path = (imageAsset as StandardAsset)?.Path;
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("[StandardAssetLoader] Image path is null or empty!");
                onLoad(null);
                yield break;
            }

            //Try loading as Texture2D first
            var textureLoader = Resources.LoadAsync<Texture2D>(path);
            yield return textureLoader;
            
            Texture2D texture = textureLoader.asset as Texture2D;
            
            if (texture != null)
            {
                Debug.Log($"[StandardAssetLoader] Successfully loaded Texture2D from path: '{path}' ({texture.width}x{texture.height})");
                onLoad(texture);
                yield break;
            }
            
            //If Texture2D failed, try loading as Sprite and convert
            Debug.LogWarning($"[StandardAssetLoader] Texture2D not found at '{path}', trying Sprite...");
            var spriteLoader = Resources.LoadAsync<Sprite>(path);
            yield return spriteLoader;
            
                Sprite sprite = spriteLoader.asset as Sprite;
                if (sprite != null)
                {
                    //Convert Sprite to Texture2D
                    //If sprite is part of a texture, extract just the sprite area
                    if (sprite.texture != null)
                    {
                        Rect spriteRect = sprite.textureRect;
                        
                        //Create readable texture copy
                        RenderTexture renderTexture = RenderTexture.GetTemporary(
                            (int)spriteRect.width,
                            (int)spriteRect.height,
                            0,
                            RenderTextureFormat.Default,
                            RenderTextureReadWrite.Linear);
                        
                        Graphics.Blit(sprite.texture, renderTexture);
                        RenderTexture previous = RenderTexture.active;
                        RenderTexture.active = renderTexture;
                        
                        texture = new Texture2D((int)spriteRect.width, (int)spriteRect.height, TextureFormat.RGBA32, false);
                        texture.ReadPixels(new Rect(0, 0, spriteRect.width, spriteRect.height), 0, 0);
                        texture.Apply();
                        
                        RenderTexture.active = previous;
                        RenderTexture.ReleaseTemporary(renderTexture);
                        
                        Debug.Log($"[StandardAssetLoader] Loaded image '{path}' as Sprite and converted to Texture2D ({texture.width}x{texture.height})");
                        onLoad(texture);
                    }
                    else
                    {
                        Debug.LogError($"[StandardAssetLoader] Failed to get texture from Sprite at path: '{path}'");
                        onLoad(null);
                    }
                }
            else
            {
                //Try loading as Texture (generic) as last resort
                Debug.LogWarning($"[StandardAssetLoader] Sprite not found at '{path}', trying generic Texture...");
                var genericLoader = Resources.LoadAsync<Texture>(path);
                yield return genericLoader;
                
                Texture genericTexture = genericLoader.asset as Texture;
                if (genericTexture != null && genericTexture is Texture2D tex2D)
                {
                    texture = tex2D;
                    Debug.Log($"[StandardAssetLoader] Loaded generic Texture2D from path: '{path}' ({texture.width}x{texture.height})");
                    onLoad(texture);
                }
                else
                {
                    Debug.LogError($"[StandardAssetLoader] Failed to load image at path: '{path}'");
                    Debug.LogError($"[StandardAssetLoader] Tried: Texture2D, Sprite, and generic Texture - all failed.");
                    Debug.LogError($"[StandardAssetLoader] Make sure the file exists at: Assets/Resources/{path}.png");
                    Debug.LogError($"[StandardAssetLoader] Check import settings - Texture Type should be 'Default' or 'Sprite (2D and UI)'");
                    onLoad(null);
                }
            }
        }
    }
}
