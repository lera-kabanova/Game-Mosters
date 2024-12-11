using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
/// <summary>
/// This script is attached to all the buttons in the right side on the screen
/// These are the buttons, that we use when we buy towers
/// </summary>
public class TowerBtn : MonoBehaviour
{
    [SerializeField]
    private GameObject towerPrefab;

    [SerializeField]
    private Sprite sprite;

    [SerializeField]
    private int price;

    [SerializeField]
    private TMP_Text priceTxt;


    public GameObject TowerPrefab
    {
        get
        {
            return towerPrefab;
        }
    }

    public Sprite Sprite
    {
        get
        {
            return sprite;
        }
    }

    public int Price
    {
        get
        {
            return price;
        }
    }

    private void Start()
    {
        priceTxt.text = Price + "$";

        GameManager.Instance.Changed += new CurrencyChanged(PriceCheck);
    }

    private void PriceCheck()
    {
        if (price <= GameManager.Instance.Currency)
        {
            GetComponent<Image>().color = Color.white;
            priceTxt.color = Color.white;
        }
        else
        {
            GetComponent<Image>().color = Color.grey;
            priceTxt.color = Color.grey;
        }
    }

    public void ShowInfo(string type)
    {
        string tooltip = string.Empty;

        switch (type)
        {
            case "Fire":
                tooltip = string.Format("<color=#84f542><size=20><b>Fire</b></size></color>");
                break;
            case "Frost":
                FrostTower frost = towerPrefab.GetComponentInChildren<FrostTower>();
                tooltip = string.Format("<color=#00ffffff><size=20><b>Frost</b></size></color>\nDamage: {0} \nProc: {1}% \nDebuff duration: {2}sec \nSlowing factor: {3}% \nHas a chance to slow dowm the target", frost.Damage, frost.Proc, frost.DebuffDuration, frost.SlowingFactor);
                break;
            case "Posion":
                tooltip = string.Format("<color=#00ff00ff><size=20><b>Poison</b></size></color>");
                break;
            case "Storm":
                tooltip = string.Format("<color=#add8e6ff><size=20><b>Storm</b></size></color>");
                break;

        }

        GameManager.Instance.SetTooltipText(type);
        GameManager.Instance.ShowStats();

    }
}
