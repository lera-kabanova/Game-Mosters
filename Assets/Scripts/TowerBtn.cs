using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

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
                FireTower fire = towerPrefab.GetComponentInChildren<FireTower>();
               tooltip = string.Format("<color=orange><size=35><b>Fire</b></size></color>\nDamage: {0} \nProc: {1}%\nDebuff duration: {2}sec \nTick time: {3} sec \nTick damage: {4}\nCan apply a DOT to the target", fire.Damage, fire.Proc, fire.DebuffDuration, fire.TickTime, fire.TickDamage);
                break;
            case "Frost":
                FrostTower frost = towerPrefab.GetComponentInChildren<FrostTower>();
                tooltip = string.Format("<color=#00ffffff><size=35><b>Frost</b></size></color>\nDamage: {0} \nProc: {1}%\nDebuff duration: {2}sec\nSlowing factor: {3}%\nHas a chance to slow down the target", frost.Damage, frost.Proc, frost.DebuffDuration, frost.SlowingFactor);
                break;
            case "Poison":
                PoisonTower poison = towerPrefab.GetComponentInChildren<PoisonTower>();
                tooltip = string.Format("<color=#00ff00ff><size=35><b>Poison</b></size></color>\nDamage: {0} \nProc: {1}%\nDebuff duration: {2}sec \nTick time: {3} sec \nSplash damage: {4}\nCan apply dripping poison", poison.Damage, poison.Proc, poison.DebuffDuration, poison.TickTime, poison.SplashDamage);
                break;
            case "Storm":
                StormTower storm = towerPrefab.GetComponentInChildren<StormTower>();
                tooltip = string.Format("<color=#add8e6ff><size=35><b>Storm</b></size></color>\nDamage: {0} \nProc: {1}%\nDebuff duration: {2}sec\n Has a chance to stunn the target", storm.Damage, storm.Proc, storm.DebuffDuration);
                break;

        }

        GameManager.Instance.SetTooltipText(tooltip);
        GameManager.Instance.ShowStats();

    }
}
