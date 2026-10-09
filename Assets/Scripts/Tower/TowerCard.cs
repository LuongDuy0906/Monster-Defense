using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerCard : MonoBehaviour
{
    public static event Action<TowerData> OnTowerSelected;
    
    [SerializeField] private Image towerImage;
    [SerializeField] private TMP_Text costText;

    private TowerData _towerData;

    public void Initialize(TowerData tower) 
    {
        _towerData = tower;
        towerImage.sprite = tower.sprite;
        costText.text = tower.cost.ToString();
    }

    public void PlaceTower()
    {
        OnTowerSelected?.Invoke(_towerData);
    }
}
