
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using static UnityEditor.Experimental.GraphView.GraphView;
using System.Linq;

public class CardSpawner : MonoBehaviour
{
    public GameObject cardPrefab;
    public string texturesDirectory = "Textures";
    public Transform parentTransform;
    public Vector3 spawnPosition = Vector3.zero;
    public Vector3 spawnScale = Vector3.one;
    public string cardTag = "Card";           // Default tag for spawned cards
    public LayerMask cardLayer = 0;

    private List<string> allTextureFiles = new List<string>();
    private List<string> remainingTextureFiles = new List<string>();

    void OnEnable()
    {
        InputManager.KeyDownEvent += OnKeyDown;
    }

    void OnDisable()
    {
        InputManager.KeyDownEvent -= OnKeyDown;
    }

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

    void OnKeyDown(KeyCode key)
    {
        if (key == KeyCode.S && Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            SpawnCard();
        }
    }

    void SpawnCard()
    {
        if (cardPrefab == null)
        {
            Debug.LogWarning("Card prefab is not assigned.");
            return;
        }

        GameObject newCard = Instantiate(cardPrefab, spawnPosition, Quaternion.identity, parentTransform);
        newCard.transform.localScale = spawnScale;
        newCard.transform.localPosition = spawnPosition;

        if (!string.IsNullOrEmpty(cardTag))
        {
            try
            {
                newCard.tag = cardTag;
            }
            catch (UnityException e)
            {
                Debug.LogError($"Failed to set card tag: {e.Message}. Make sure the tag exists in Tags and Layers settings.");
            }
        }

        // Set layer
        newCard.layer = (int)Mathf.Log(cardLayer.value, 2); // Convert layer mask to layer index

        MeshCollider meshCollider = newCard.GetComponent<MeshCollider>();
        if (meshCollider == null)
        {
            meshCollider = newCard.AddComponent<MeshCollider>();
        }
        meshCollider.convex = true;
        meshCollider.cookingOptions = MeshColliderCookingOptions.EnableMeshCleaning |
                                    MeshColliderCookingOptions.WeldColocatedVertices |
                                    MeshColliderCookingOptions.CookForFasterSimulation;

        // Apply texture
        string textureFile = GetNextTexture();
        if (File.Exists(textureFile))
        {
            byte[] fileData = File.ReadAllBytes(textureFile);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(fileData);

            Material newMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            newMaterial.mainTexture = texture;
            newCard.GetComponent<Renderer>().material = newMaterial;

            // Set card name to texture name for easier identification
            newCard.name = $"Card_{Path.GetFileNameWithoutExtension(textureFile)}";
        }
    }
    private string GetNextTexture()
    {
        if (remainingTextureFiles.Count == 0)
        {
            Debug.Log("All textures used, resetting pool...");
            ResetRemainingTextures();
        }

        int randomIndex = Random.Range(0, remainingTextureFiles.Count);
        string selectedTexture = remainingTextureFiles[randomIndex];
        remainingTextureFiles.RemoveAt(randomIndex);

        Debug.Log($"Spawning card with texture: {Path.GetFileName(selectedTexture)}. {remainingTextureFiles.Count} textures remaining.");
        return selectedTexture;
    }
}