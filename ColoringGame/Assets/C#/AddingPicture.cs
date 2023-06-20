using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class AddingPicture : MonoBehaviour
{
    [Header("inputs data")]
    public TMP_InputField name_inputField;
    public TMP_Dropdown Category;
    public Toggle Acess_toggle;

    [Header("images lists")]
    public Transform LayersImages_transform;
    public Transform PreviewImages_transform;

    private Sprite PreviewImage;
    private List<Sprite> LayersImages = new List<Sprite>();

    private string PreviewPath;
    private List<string> LayersPathes = new List<string>();
    private List<string> originalLayersPathes = new List<string>();

    [Header("pathes")]
    public string folderName; // The name of the folder to create
    public string imagesPath = "Assets/PNGs/Images"; // Path to save the images
    public string originalsPath = "Assets/PNGs/ImagesOriginals"; // Path to save the original images

    public void CreatePrefab()
    {
        SaveSprites();

        // Create the prefab root GameObject and add the Picture component
        GameObject prefabRoot = new GameObject();
        Picture pictureComponent = prefabRoot.AddComponent<Picture>();

        // Set the Picture component properties
        Enums.ECategory category = GetSelectedCategory();
        pictureComponent.Name = name_inputField.text;
        pictureComponent.category = category;
        pictureComponent.acess = Acess_toggle.isOn ? Enums.EAcess.Premium : Enums.EAcess.Free;

        // Create child GameObjects for each layer sprite and assign SpriteRenderer components
        foreach (string _path in LayersPathes)
        {
            pictureComponent.Layers.Add(_path);
        }

        // Create child GameObjects for each original layer sprite and assign SpriteRenderer components
        foreach (string _path in originalLayersPathes)
        {
            pictureComponent.StandartLayers.Add(_path);
        }

        pictureComponent.MainImage = PreviewPath;

        // Save the prefab
        string prefabPath = "Assets/Resources/Picturies/" + pictureComponent.Name + ".prefab";
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(prefabRoot, prefabPath);

        Debug.Log("Picture prefab saved at: " + prefabPath);
    }


    private Enums.ECategory GetSelectedCategory()
    {
        Enums.ECategory category = Enums.ECategory.Animals;
        switch (Category.value)
        {
            case 0:
                category = Enums.ECategory.Animals;
                break;
            case 1:
                category = Enums.ECategory.Cars;
                break;
            case 2:
                category = Enums.ECategory.Peoples;
                break;
            case 3:
                category = Enums.ECategory.Things;
                break;
        }
        return category;
    }

    public void clearAllChoosedLayers()
    {
        int childCount = LayersImages_transform.transform.childCount;
        if (childCount > 0)
        {
            for (int i = childCount - 1; i >= 0; i--)
            {
                GameObject childObject = LayersImages_transform.transform.GetChild(i).gameObject;
                Destroy(childObject);
            }
        }

        LayersImages = new List<Sprite>();
    }

    public void SaveSprites()
    {
        string folderPath = Path.Combine(imagesPath, folderName);
        string originalsFolderPath = Path.Combine(originalsPath, folderName);

        // Create the folder if it doesn't exist
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // Create the originals folder if it doesn't exist
        if (!Directory.Exists(originalsFolderPath))
        {
            Directory.CreateDirectory(originalsFolderPath);
        }

        // Save the list of sprites
        foreach (Sprite sprite in LayersImages)
        {
            string spriteName = sprite.name;
            string spritePath = Path.Combine(folderPath, spriteName + ".png");
            FilesHandler.SaveSprite(sprite, spritePath);

            LayersPathes.Add(spritePath);


            string originalSpriteName = spriteName + "_original";
            string originalSpritePath = Path.Combine(originalsFolderPath, originalSpriteName + ".png");
            FilesHandler.SaveSprite(sprite, originalSpritePath);

            originalLayersPathes.Add(originalSpritePath);
        }

        // Save the single sprite
        string singleSpriteName = PreviewImage.name;
        string singleSpritePath = Path.Combine(originalsFolderPath, singleSpriteName + ".png");
        FilesHandler.SaveSprite(PreviewImage, singleSpritePath);
        PreviewPath = singleSpritePath;

        Debug.Log("PreviewPath: " + PreviewPath);
        Debug.Log("LayersPathes: " + LayersPathes.ToString());
    }

    ////////////\\\\\\\\\\\\\
    /// open file browser \\\
    ////////////\\\\\\\\\\\\\
    public void GetLayersImages()
    {
        string path = EditorUtility.OpenFilePanel("Overwrite with png", "", "png,jpg,jpeg");
        HandleLayersFilePath(path);
    }
    public void GetPreviewImages()
    {
        string path = EditorUtility.OpenFilePanel("Overwrite with png", "", "png,jpg,jpeg");
        HandlePreviewFilePath(path);
    }

    //////////\\\\\\\\\\\
    /// handle pathes \\\
    //////////\\\\\\\\\\\
    private void HandleLayersFilePath(string path)
    {
        if (path.Length > 0)
        {
            Image _image = Instantiate(new GameObject(), LayersImages_transform).AddComponent<Image>();

            Texture2D texture = FilesHandler.LoadTextureFromFile(path);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);

            sprite.name = Path.GetFileNameWithoutExtension(path);

            _image.sprite = sprite;
            LayersImages.Add(sprite);
        }
    }
    public void HandlePreviewFilePath(string path)
    {
        if (path != null)
        {
            int childCount = PreviewImages_transform.transform.childCount;
            if (childCount > 0)
            {
                for (int i = childCount - 1; i >= 0; i--)
                {
                    GameObject childObject = PreviewImages_transform.transform.GetChild(i).gameObject;
                    Destroy(childObject);
                }
            }

            Image _image = Instantiate(new GameObject(), PreviewImages_transform).AddComponent<Image>();

            Texture2D texture = FilesHandler.LoadTextureFromFile(path);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);

            sprite.name = Path.GetFileNameWithoutExtension(path);

            _image.sprite = sprite;

            PreviewImage = sprite;
        }
    }
}
