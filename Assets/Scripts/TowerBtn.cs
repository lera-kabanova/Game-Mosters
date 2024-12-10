using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
public class TowerBtn : MonoBehaviour {

    /// <summary>
    /// The prefab that this button will spawn
    /// </summary>
    [SerializeField]
    private GameObject towerPrefab;

    /// <summary>
    /// A reference to the tower's sprite
    /// </summary>
    [SerializeField]
    private Sprite sprite;

    [SerializeField]
    private int price;

    [SerializeField]
    private TMP_Text priceTxt;

    /// <summary>
    /// Property for accessing the button's prefab
    /// </summary>
    public GameObject TowerPrefab
    {
        get
        {
            return towerPrefab;
        }
    }

    /// <summary>
    /// A reference for accessing the tower sprite
    /// </summary>
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
        if(price <= GameManager.Instance.Currency)
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
}
