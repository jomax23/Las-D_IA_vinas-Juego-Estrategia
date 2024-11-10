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

    public FieldObstacleGeneration fieldObstacleGenerator;

    // Start is called before the first frame update
    private void Start()
    {
        gm = GetComponent<GameMaster>();
        //fieldObstacleGenerator = GetComponent<FieldObstacleGeneration>();
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

        gm.purchasedItem = item;

        if (gm.playerTurn == 1 && item.cost <= gm.player1Gold)
        {
            if (fieldObstacleGenerator.arrayTile[0, fieldObstacleGenerator.generadorDelMapa.GetAltura() - 1].IsClear())
            {
                gm.player1Gold -= item.cost;
                BarrackItem itemBought = Instantiate(gm.purchasedItem, new Vector3(0, fieldObstacleGenerator.generadorDelMapa.GetAltura() - 1, -5), Quaternion.identity);
            }

            player1Menu.SetActive(false);
        }

        else if (gm.playerTurn == 2 && item.cost <= gm.player2Gold)
        {
            if (fieldObstacleGenerator.arrayTile[fieldObstacleGenerator.generadorDelMapa.GetAnchura() - 1, fieldObstacleGenerator.generadorDelMapa.GetAltura() - 1].IsClear())
            {
                gm.player2Gold -= item.cost;
                BarrackItem iitemBought = Instantiate(gm.purchasedItem, new Vector3(fieldObstacleGenerator.generadorDelMapa.GetAnchura() - 1, fieldObstacleGenerator.generadorDelMapa.GetAltura() - 1, -5), Quaternion.identity);
            }

            player2Menu.SetActive(false);
        }

        else
        {
            print("NOT ENOUGH GOLD");
            return;
        }

        gm.UpdateGoldText();

        if (gm.selectedUnit != null)
        {
            gm.selectedUnit.selected = false;
            gm.selectedUnit = null;
        }
        
        GetCreatableTiles(gm.playerTurn);
        
    }

    void GetCreatableTiles(int playerTurn)
    {
        
        /*
        foreach (Tile tile in FindObjectsOfType<Tile>())
        {
            if(tile.IsClear())
            {
                tile.SetCreatable();
            }
        }
        */
    }
}
