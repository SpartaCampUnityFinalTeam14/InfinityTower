using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ArtifactSlot : MonoBehaviour
{
    ArtifactData data;

    [SerializeField] private Button selectButton;
    [SerializeField] private Image artifactImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI countText;

    public void Init(int id)
    {
        int rarity = id / 1000;

        data = DataManager.Instance.artifactDicts[rarity][id];

        nameText.text = data.name;
        SetCount(SaveManager.Instance.artifactSaveDict[id].count);
    }

    public void SetCount(int count)
    {
        countText.text = count.ToString();
    }
}
