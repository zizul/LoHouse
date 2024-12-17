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
            string textureFile = GetNextTexture();
            SpawnCard(GetSprite(textureFile), GenerateCardName(textureFile));
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

    public GameObject SpawnCard(Sprite cardSprite, string name)
    {
        if (cardPrefab == null)
        {
            Debug.LogWarning("Card prefab is not assigned.");
            return null;
        }

        if (allTextureFiles.Count == 0)
        {
            Debug.LogError("No texture files found in the specified directory.");
            return null;
        }

        // Spawn position and rotation
        Vector3 spawnPosition = parentTransform.position;
        Quaternion spawnRotation = parentTransform.rotation;

        // Instantiate the card
        GameObject newCard = Instantiate(cardPrefab, spawnPosition, spawnRotation, parentTransform);
        newCard.transform.localScale = spawnScale;

        // Set custom tag and layer
        if (useCustomTagAndLayer)
        {
            SetCustomTagAndLayer(newCard);
        }

        // Apply texture
        ApplyTextureToCard(newCard, cardSprite);

        // Set card name for easier identification
        newCard.name = name;

        Debug.Log($"Spawned card in hand: (Current count: {parentTransform.childCount})");
        return newCard;
    }

    void SetCustomTagAndLayer(GameObject card)
    {
        if (!string.IsNullOrEmpty(cardTag))
        {
            try
            {
                card.tag = cardTag;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to set card tag: {e.Message}. Make sure the tag exists in Tags and Layers settings.");
            }
        }

        if (cardLayer != 0)
        {
            card.layer = (int)Mathf.Log(cardLayer.value, 2);
        }
    }

    void ApplyTextureToCard(GameObject card, Sprite cardSprite)
    {
        if (cardSprite != null)
        {
            CardBehaviour cardBehaviour = card.GetComponent<CardBehaviour>();
            if (cardBehaviour != null)
            {
                cardBehaviour.SetCardFrontSprite(cardSprite);
            }
        }
    }

    private Sprite GetSprite(string textureFile)
    {
        if (!File.Exists(textureFile))
        {
            Debug.LogWarning("Texture file does not exist.");
            return null;
        }

        try
        {
            byte[] fileData = File.ReadAllBytes(textureFile);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(fileData);

            Sprite cardSprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            return cardSprite;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load texture to sprite: {e.Message}");
            return null;
        }
    }

    string GenerateCardName(string textureFile)
    {
        return File.Exists(textureFile)
            ? $"Card_{Path.GetFileNameWithoutExtension(textureFile)}"
            : "Card_Unknown";
    }
}