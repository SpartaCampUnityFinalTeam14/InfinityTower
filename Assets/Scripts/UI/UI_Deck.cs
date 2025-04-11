using UnityEngine;

public class UI_Deck : UI
{
    [SerializeField] private UI_ChampionSelect championSelect;
    [SerializeField] private UI_TowerSelect towerSelect;

    public void init(bool isTower)
    {

    }

    public override void Clear()
    {
        base.Clear();

        championSelect.Clear();
    }
}
