using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class TabController : MonoBehaviour
{
    [SerializeField] private List<TabSelectButton> buttons = new();
    private UI_Deck deck;

    private void Awake()
    {
        deck = GetComponent<UI_Deck>();

        SelectTab(0);
    }

    public void SelectTab(int id)
    {
        StartCoroutine(SelectTabMenu(id));
    }

    public IEnumerator SelectTabMenu(int id)
    {
        foreach (TabSelectButton button in buttons)
        {
            button.SelectButton(button.id == id);
        }

        yield return null;

        deck.ResetScrolls();
    }
}
