using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentData : MonoBehaviour
{
    public TMPro.TMP_Text playerNameText;
    public TMPro.TMP_InputField inputField;

    // Start is called before the first frame update
    void Start()
    {
        playerNameText.text = PlayerPrefs.GetString("playerName", "nobody");
    }

    public void SavePlayerName()
    {
        PlayerPrefs.SetString("playerName", inputField.text);
        playerNameText.text = inputField.text;
    }
}
