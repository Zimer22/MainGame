using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class DynamicUI : MonoBehaviour
{
    public GameObject healthObj;
    public GameObject objectObj;
    public GameObject interactObj;
    TextMeshProUGUI healthText;
    TextMeshProUGUI selectedObjText;
    TextMeshProUGUI interactionsText;
    Player player;
    NetworkManager networkManager;
    void Start()
    {
        healthText = healthObj.GetComponent<TextMeshProUGUI>();
        selectedObjText = objectObj.GetComponent<TextMeshProUGUI>();
        interactionsText = interactObj.GetComponent<TextMeshProUGUI>();
    }
    public void ChangeSelectedObjectText(string selected)
    {
        if (selected != selectedObjText.text) // this avoids resetting the interactableHud every frame on the same object
        {
            selectedObjText.text = selected;
        }
    }
    public void EmptySelectedObjectText()
    {
        selectedObjText.text = string.Empty;
    }
    public void ChangeSelectedInteractText(string selected,string previnteraction = "", string nextinteraction = "")
    {
        if (selected != interactionsText.text)
        {
            interactionsText.text = previnteraction + "<br>>>" + selected + "<br>" + nextinteraction;
        }
    }
    public void EmptySelectedInteractText()
    {
        interactionsText.text = string.Empty;
    }
    public void InteractStartUi() // todo make interactable call this function, show progress till interaction done
    {

    }
    private void Update()
    {
        //if (lmao)
        //{
        //    player.FetchPlayerStats(out float h, out float s, out _, out _);
        //    healthText.SetText("Health: " + h);
        //}
    }
}
