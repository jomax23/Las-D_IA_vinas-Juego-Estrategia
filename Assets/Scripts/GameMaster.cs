using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMaster : MonoBehaviour
{
    public Unit selectedUnit;

    public int playerTurn = 1; //PIKOLO = 1, VEGETA = 2

    public GameObject selectedUnitSquare;

    public Image playerIndicator;

    public Sprite player1Indicator;
    public Sprite player2Indicator;

    public int player1Gold = 100;
    public int player2Gold = 100;

    public Text player1GoldText;
    public Text player2GoldText;

    public BarrackItem purchasedItem;
    private void Start()
    {
        GetGoldIncome(1);
    }
    public void ResetTiles() {
        foreach (Tile tile in FindObjectsOfType<Tile>())
        {
            tile.Reset();
        }
    }

    public void UpdateGoldText()
    {
        player1GoldText.text = player1Gold.ToString();
        player2GoldText.text = player2Gold.ToString();
    }

    void GetGoldIncome(int playerTurn)
    {
        foreach(Village village in FindObjectsOfType<Village>())
        {
            if(village.playerNumber == playerTurn)
            {
                if(playerTurn == 1)
                {
                    player1Gold += village.goldPerTurn;
                }

                else
                {
                    player2Gold += village.goldPerTurn;
                }
            }
        }

        UpdateGoldText();
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            EndTurn();
        }

        if(selectedUnit != null)
        {
            selectedUnitSquare.SetActive(true);
            selectedUnitSquare.transform.position = selectedUnit.transform.position;
        }

        else
        {
            selectedUnitSquare.SetActive(false);
        }
           
    }

    void EndTurn()
    {
        if(playerTurn == 1)
        {
            playerTurn = 2;
            playerIndicator.sprite = player2Indicator;
        }

        else if(playerTurn == 2)
        {
            playerTurn = 1;
            playerIndicator.sprite = player1Indicator;
        }

        GetGoldIncome(playerTurn);

        if(selectedUnit != null)
        {
            selectedUnit.selected = false;
            selectedUnit = null;
        }

        ResetTiles();

        foreach(Unit unit in FindObjectsOfType<Unit>())
        {
            unit.hasMoved = false;
            unit.weaponIcon.SetActive(false);
            unit.hasAttacked = false;
        }

        GetComponent<Barrack>().CloseMenus();
    }
}
