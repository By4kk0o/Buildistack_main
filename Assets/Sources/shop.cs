using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class shop : MonoBehaviour
{
    public GameObject money_go;
    public List<GameObject> shopItems = new List<GameObject>();
    public List<GameObject> customizationItems = new List<GameObject>();

    // check customization items unlock on customization menu open
    public void CheckCustomizationItemsUnlock()
    {
        foreach (var item in this.customizationItems)
        {
            int isUnlock = PlayerPrefs.GetInt("isUnlock" + item.name, 0);
            if (isUnlock == 1)
            {
                // active button
                item.GetComponent<Button>().interactable = true;
                // hide lock
                item.transform.GetChild(1).gameObject.SetActive(false);
            }
        }
    }

    // check if player can buy shopItems when opening the shop
    public void CheckShopItemsPriceAndUnlock()
    {
        foreach (var item in this.shopItems)
        {
            int isUnlock = PlayerPrefs.GetInt("isUnlock" + item.name, 0);
            if (isUnlock == 1)
            {
                // disable button
                item.GetComponent<Button>().interactable = false;
                // show validate image
                item.transform.GetChild(2).gameObject.SetActive(true);
                // hide text
                item.transform.GetChild(1).gameObject.SetActive(false);
            } else if (this.CanBuy(item))
            {
                // active button
                item.GetComponent<Button>().interactable = true;
                // change text color
                item.transform.GetChild(1).gameObject.GetComponent<Text>().color = Color.yellow;
            } else
            {
                // disable button
                item.GetComponent<Button>().interactable = false;
                // change text color
                item.transform.GetChild(1).gameObject.GetComponent<Text>().color = Color.red;
            }
        }
    }

    bool CanBuy(GameObject go)
    {
        int gold = PlayerPrefs.GetInt("MONEY", 0);
        int price = int.Parse(go.transform.GetChild(1).gameObject.GetComponent<Text>().text);
        if (gold >= price)
        {
            return true;
        } else
        {
            return false;
        }
    }

    // call before opening buy menu
    public void PreBuy(GameObject go)
    {
        int price = int.Parse(go.transform.GetChild(1).gameObject.GetComponent<Text>().text);
        PlayerPrefs.SetInt("PRICE", price);
        PlayerPrefs.SetString("TO_UNLOCK", "isUnlock" + go.name);
    }

    // call to unlock item
    public void Buy()
    {
        int moneyLeft = PlayerPrefs.GetInt("MONEY", 0) - PlayerPrefs.GetInt("PRICE", 0);
        PlayerPrefs.SetInt(PlayerPrefs.GetString("TO_UNLOCK"), 1);
        PlayerPrefs.SetInt("MONEY", moneyLeft);
        Debug.Log(PlayerPrefs.GetString("TO_UNLOCK"));
        Text text = this.money_go.GetComponent<Text>();
        text.text = moneyLeft.ToString();
        CheckShopItemsPriceAndUnlock();
    }
}
