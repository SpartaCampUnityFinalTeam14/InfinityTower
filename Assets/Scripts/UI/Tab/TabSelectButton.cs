using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class TabSelectButton : MonoBehaviour
{
    public int id = -1;
    [SerializeField] private GameObject panel;
    [SerializeField] private TabController tabController;
    [SerializeField] private Button button;
    [SerializeField] private Image buttonImage;

    private Color selectedColor = Color.white;
    private Color notSelectedColor = Color.gray;

    private void Awake()
    {
        button.onClick.AddListener(() => tabController.SelectTab(id));
    }

    public void SelectButton(bool flag)
    {
        buttonImage.color = flag ? selectedColor : notSelectedColor;
        panel.SetActive(flag);
    }
}
