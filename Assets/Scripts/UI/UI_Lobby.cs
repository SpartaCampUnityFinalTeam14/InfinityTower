using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_Lobby : UI
{
    [SerializeField] private TextMeshProUGUI goldText;

    [SerializeField] private Button stageStartButton;
    [SerializeField] private Button championSelectButton;
    [SerializeField] private Button deckSelectButton;
    [SerializeField] private Button gachaButton;

    protected override void Awake()
    {
        base.Awake();

        goldText.text = SaveManager.Instance.playerData.gold.ToString();
        stageStartButton.onClick.AddListener(() => GameManager.Instance.LoadScene("KSM_Stage"));
        championSelectButton.onClick.AddListener(() => UIManager.Instance.ShowStackUI<UI_Deck>());
        deckSelectButton.onClick.AddListener(() => UIManager.Instance.ShowStackUI<UI_Deck>());
        gachaButton.onClick.AddListener(() => UIManager.Instance.ShowStackUI<UI_Gacha>());
    }
}
