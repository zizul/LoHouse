using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;

public class SpriteCardSpawner : MonoBehaviour
{
    public GameObject cardPrefab;
    public string texturesDirectory = "Textures";
    public Transform parentTransform;
    public Vector3 spawnScale = Vector3.one;

    [Header("Card Properties")]
    public string cardTag = "Card";
    public LayerMask cardLayer = 0;
    public bool useCustomTagAndLayer = false;

    private List<string> allTextureFiles = new List<string>();
    private List<string> remainingTextureFiles = new List<string>();

    void Start()
    {
        LoadTextureFiles();
    }

    void LoadTextureFiles()
    {
        string path = Path.Combine(Application.dataPath, texturesDirectory);
        if (Directory.Exists(path))
        {
            allTextureFiles = Directory.GetFiles(path, "*.png").ToList();
            ResetRemainingTextures();
        }
        else
        {
            Debug.LogError($"Directory not found: {path}");
        }
    }

    void ResetRemainingTextures()
    {
        remainingTextureFiles = new List<string>(allTextureFiles);
        Debug.Log($"Texture pool reset. {remainingTextureFiles.Count} textures available.");
    }

    void OnEnable()
    {
        InputManager.KeyDownEvent += OnKeyDown;
    }

    void OnDisable()
    {
        InputManager.KeyDownEvent -= OnKeyDown;
    }

    void OnKeyDown(KeyCode key)
    {
        if (key == KeyCode.W && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
        {
            SpawnCard();
        }
    }

    string GetNextTexture()
    {
        if (remainingTextureFiles.Count == 0)
        {
            Debug.Log("All textures used, resetting pool...");
            ResetRemainingTextures();
        }

        int randomIndex = UnityEngine.Random.Range(0, remainingTextureFiles.Count);
        string selectedTexture = remainingTextureFiles[randomIndex];
        remainingTextureFiles.RemoveAt(randomIndex);

        Debug.Log($"Spawning card with texture: {Path.GetFileName(selectedTexture)}. {remainingTextureFiles.Count} textures remaining.");
        return selectedTexture;
    }

    void SpawnCard()
    {
        if (cardPrefab == null)
        {
            Debug.LogWarning("Card prefab is not assigned.");
            return;
        }

        if (allTextureFiles.Count == 0)
        {
            Debug.LogError("No texture files found in the specified directory.");
            return;
        }

        Vector3 spawnPosition = parentTransform.transform.position;
        Quaternion spawnRotation = parentTransform.transform.rotation;

        // Create the card at the anchor position and rotation
        GameObject newCard = Instantiate(cardPrefab, spawnPosition, spawnRotation, parentTransform);
        newCard.transform.localScale = spawnScale;

        // Apply custom tag and layer if enabled
        if (useCustomTagAndLayer)
        {
            if (!string.IsNullOrEmpty(cardTag))
            {
                try
                {
                    newCard.tag = cardTag;
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to set card tag: {e.Message}. Make sure the tag exists in Tags and Layers settings.");
                }
            }

            newCard.layer = (int)Mathf.Log(cardLayer.value, 2);
        }

        // Apply texture
        string textureFile = GetNextTexture();
        if (File.Exists(textureFile))
        {
            byte[] fileData = File.ReadAllBytes(textureFile);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(fileData);

            //Material newMaterial = new Material(Shader.Find("Sprite-Lit-Default"));
            //newMaterial.mainTexture = texture;
            //newCard.GetComponentInChildren<SpriteRenderer>().material.mainTexture = texture;

            //Material newMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            //newMaterial.mainTexture = texture;
            newCard.GetComponentInChildren<SpriteRenderer>().sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            newCard.GetComponentsInChildren<SpriteRenderer>().ToList().ForEach(x => x.sortingOrder = parentTransform.childCount);
            // Set card name to texture name and anchor for easier identification
            newCard.name = $"Card_{Path.GetFileNameWithoutExtension(textureFile)}";
        }

        Debug.Log($"Spawned card in hand: (Current count: {parentTransform.childCount})");
    }
}