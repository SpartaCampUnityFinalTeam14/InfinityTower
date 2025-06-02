using UnityEngine;
using UnityEngine.UI;

public class CreditScroll : MonoBehaviour
{
    private bool isEnd;
    private ScrollRect scroll;
    [SerializeField] private Button ReturnButton;

    private void Awake()
    {
        scroll = GetComponent<ScrollRect>();

        ReturnButton.onClick.AddListener(ReturnToLobby);
    }

    private void Update()
    {
        if (!isEnd && scroll.verticalScrollbar.value > 0) 
        { 
            scroll.verticalScrollbar.value -= Time.deltaTime * 0.07f;
            if(scroll.verticalScrollbar.value <= 0) isEnd = true;
        }
    }

    void ReturnToLobby()
    {
        GameManager.Instance.LoadScene("KSM_Lobby");
    }
}
