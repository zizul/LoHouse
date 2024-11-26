using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class CardSpawner : MonoBehaviour
{
    public GameObject cardPrefab;
    public string texturesDirectory = "Textures";
    public Transform parentTransform;
    public Vector3 spawnScale = Vector3.one;

    [Header("Card Properties")]
    public string cardTag = "Card";
    public LayerMask cardLayer = 0;
    public bool useCustomTagAndLayer = false;
    public bool addMeshCollider = true;

    private List<string> allTextureFiles = new List<string>();
    private List<string> remainingTextureFiles = new List<string>();
    private List<GameObject> anchorPoints;
    private Dictionary<GameObject, int> anchorObjectCounts = new Dictionary<GameObject, int>();

    void Start()
    {
        LoadTextureFiles();
        RefreshAnchorPoints();
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

    void RefreshAnchorPoints()
    {
        anchorPoints = GameObject.FindGameObjectsWithTag("CardAnchor").ToList();
        anchorObjectCounts.Clear();

        foreach (GameObject anchor in anchorPoints)
        {
            anchorObjectCounts[anchor] = 0;
        }

        if (anchorPoints.Count == 0)
        {
            Debug.LogWarning("No CardAnchor points found in the scene!");
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
        if (key == KeyCode.Q && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
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

        int randomIndex = Random.Range(0, remainingTextureFiles.Count);
        string selectedTexture = remainingTextureFiles[randomIndex];
        remainingTextureFiles.RemoveAt(randomIndex);

        Debug.Log($"Spawning card with texture: {Path.GetFileName(selectedTexture)}. {remainingTextureFiles.Count} textures remaining.");
        return selectedTexture;
    }

    GameObject GetBestAnchorPoint()
    {
        if (anchorPoints == null || anchorPoints.Count == 0)
        {
            RefreshAnchorPoints();
            if (anchorPoints.Count == 0)
            {
                return null;
            }
        }

        // Remove any null anchors and their counts
        anchorPoints.RemoveAll(a => a == null);
        anchorObjectCounts = anchorObjectCounts
            .Where(kvp => kvp.Key != null)
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        // Find minimum count
        int minCount = anchorObjectCounts.Values.Min();

        // Get all anchors with minimum count
        var bestAnchors = anchorObjectCounts
            .Where(kvp => kvp.Value == minCount)
            .Select(kvp => kvp.Key)
            .ToList();

        // Return random anchor from those with minimum count
        return bestAnchors[Random.Range(0, bestAnchors.Count)];
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

        // Get best spawn position
        GameObject selectedAnchor = GetBestAnchorPoint();
        if (selectedAnchor == null)
        {
            Debug.LogError("No valid anchor points found!");
            return;
        }

        Vector3 spawnPosition = selectedAnchor.transform.position;
        Quaternion spawnRotation = selectedAnchor.transform.rotation;

        // Create the card at the anchor position and rotation
        GameObject newCard = Instantiate(cardPrefab, spawnPosition, spawnRotation, parentTransform);
        newCard.transform.localScale = spawnScale;

        // Increment anchor count
        anchorObjectCounts[selectedAnchor]++;

        // Apply custom tag and layer if enabled
        if (useCustomTagAndLayer)
        {
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

            newCard.layer = (int)Mathf.Log(cardLayer.value, 2);
        }

        // Add MeshCollider if enabled
        if (addMeshCollider)
        {
            MeshCollider meshCollider = newCard.GetComponent<MeshCollider>();
            if (meshCollider == null)
            {
                meshCollider = newCard.AddComponent<MeshCollider>();
            }
            meshCollider.convex = true;
            meshCollider.cookingOptions = MeshColliderCookingOptions.EnableMeshCleaning |
                                        MeshColliderCookingOptions.WeldColocatedVertices |
                                        MeshColliderCookingOptions.CookForFasterSimulation;
        }

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

            // Set card name to texture name and anchor for easier identification
            newCard.name = $"Card_{Path.GetFileNameWithoutExtension(textureFile)}_{selectedAnchor.name}";
        }

        Debug.Log($"Spawned card at anchor: {selectedAnchor.name} (Current count: {anchorObjectCounts[selectedAnchor]})");
    }

    void OnDrawGizmosSelected()
    {
        // Visualize anchor points and their counts
        if (anchorPoints != null)
        {
            foreach (GameObject anchor in anchorPoints)
            {
                if (anchor != null && anchorObjectCounts.ContainsKey(anchor))
                {
                    Gizmos.color = Color.yellow;
                    Vector3 textPosition = anchor.transform.position + Vector3.up * 0.5f;
                    // Draw a line representing the count
                    Gizmos.DrawLine(
                        anchor.transform.position,
                        anchor.transform.position + Vector3.up * (anchorObjectCounts[anchor] * 0.2f)
                    );
                }
            }
        }
    }
}