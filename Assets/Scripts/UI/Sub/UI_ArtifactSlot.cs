using TMPro;
using UnityEditor.Experimental;
using UnityEngine;
using UnityEngine.UI;

public class UI_ArtifactSlot : MonoBehaviour
{
    public int id;
    ArtifactData data;

    [SerializeField] private Image artifactBackground;
    [SerializeField] private Color[] rarityColors;
    [SerializeField] private Button selectButton;
    [SerializeField] private Image artifactImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI countText;

    public void Init(int id)
    {
        this.id = id;
        int rarity = id / 1000;
        artifactBackground.color = rarityColors[rarity];
        data = DataManager.Instance.artifactDicts[rarity][id];
        Sprite sprite = Resources.Load<Sprite>($"Icons/Artifact/{data.sprite}");
        artifactImage.sprite = sprite;
        nameText.text = data.name;
        SetCount(SaveManager.Instance.artifactLevelDict[id].count);
    }

    public void SetCount(int count)
    {
        countText.text = count.ToString();
    }
}
