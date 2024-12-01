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

    public int player1Gold = 0;
    public int player2Gold = 0;

    public int goldPerTurn = 10;

    public Text player1GoldText;
    public Text player2GoldText;

    public BarrackItem purchasedItem;

    [HideInInspector]
    public bool somethingIsMoving = false;

    private void Start()
    {
        //GetGoldIncome(1);
        UpdateGoldText();
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
        /*
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
        */

        if (playerTurn == 1)
        {
            player1Gold += goldPerTurn;
        }

        else
        {
            player2Gold += goldPerTurn;
        }

        UpdateGoldText();
    }
    private void Update()
    {

        if (somethingIsMoving == false)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                //Debug.Log(somethingIsMoving);
                //gm.SomethingIsMoving();
                EndTurn();
                GetGoldIncome(playerTurn);

            }

            if (selectedUnit != null)
            {
                selectedUnitSquare.SetActive(true);
                selectedUnitSquare.transform.position = selectedUnit.transform.position;
            }

            else
            {
                selectedUnitSquare.SetActive(false);
            }
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

        if(selectedUnit != null)
        {
            selectedUnit.selected = false;
            selectedUnit = null;
        }

        ResetTiles();

        GetComponent<Barrack>().CloseMenus();

        foreach (Tile tile in FindObjectsOfType<Tile>())
        {
            tile.ResetInfluences();
        }
        //ACTUALMENTE ESTO LO HE PUESTO EN OTRO LUGAR, PARA QUE SE EJECUTE CADA VEZ QUE EL USUARIO HA REALIZADO UNA ACCION COMO MOVERSE, ATACAR O CURAR, PERO SE PODRIA PONER AQUI, DEPENDE DE LO QUE QUERAIS

        /*
        foreach (Unit unit in FindObjectsOfType<Unit>())
        {
            unit.hasMoved = false;
            //unit.weaponIcon.SetActive(false);
            unit.hasAttacked = false;
            unit.SetInfluenceTiles();
        }
        */
    }
}
