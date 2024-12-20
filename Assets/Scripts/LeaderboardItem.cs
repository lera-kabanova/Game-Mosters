using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class LeaderboardItem : MonoBehaviour
{
    public Text range;
    public Text usernameText;  // Текст для имени пользователя
    public Text earnedMoneyText;  // Текст для заработанных денег

    public void SetLeaderboardItem(string username, double earnedMoney)
    {
        usernameText.text = username;
        earnedMoneyText.text = earnedMoney.ToString("C");  // Форматирование как денежная сумма
    }
}
