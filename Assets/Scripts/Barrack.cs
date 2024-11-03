using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Barrack : MonoBehaviour
{
    public Button player1ToggleButton;
    public Button player2ToggleButton;

    public GameObject player1Menu;
    public GameObject player2Menu;

    GameMaster gm;

    // Start is called before the first frame update
    private void Start()
    {
        gm = GetComponent<GameMaster>();
    }

    // Update is called once per frame
    private void Update()
    {
        if(gm.playerTurn == 1)
        {
            player1ToggleButton.interactable = true;
            player2ToggleButton.interactable = false;
        }
        else
        {
            player1ToggleButton.interactable = false;
            player2ToggleButton.interactable = true;
        }
    }

    public void ToggleMenu(GameObject menu)
    {
        menu.SetActive(!menu.activeSelf);
    }

    public void CloseMenus()
    {
        player1Menu.SetActive(false);
        player2Menu.SetActive(false);
    }

    public void BuyItem(BarrackItem item)
    {
        if (gm.playerTurn == 1 && item.cost <= gm.player1Gold) 
        {
            gm.player1Gold -= item.cost;
            player1Menu.SetActive(false);
        }

        else if(gm.playerTurn == 2 && item.cost <= gm.player2Gold)
        {
            gm.player2Gold -= item.cost;
            player2Menu.SetActive(false);
        }

        else
        {
            print("NOT ENOUGH GOLD");
            return;
        }

        gm.UpdateGoldText();

        gm.purchasedItem = item;

        if(gm.selectedUnit != null)
        {
            gm.selectedUnit.selected = false;
            gm.selectedUnit = null;
        }

        GetCreatableTiles();
    }

    void GetCreatableTiles()
    {
        foreach (Tile tile in FindObjectsOfType<Tile>())
        {
            if(tile.IsClear())
            {
                tile.SetCreatable();
            }
        }
    }
}
