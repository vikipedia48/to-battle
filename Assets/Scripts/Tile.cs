using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using Color = UnityEngine.Color;

public class Tile : MonoBehaviour
{
    private Grid gameLogic;
    
    private void Start()
    {
        gameLogic = GameObject.Find("GameLogic").GetComponent<Grid>();
    }

    private void OnMouseOver()
    {
        if (!Input.GetMouseButtonUp(0)) return;
        if (gameLogic.disableSelecting) return;
        if (gameLogic.UnitInfoPanel.activeSelf && Input.mousePosition.x < 200) return;
        var tilePoint = gameLogic.GetPointFromTileHash(Convert.ToInt32(gameObject.name));
        var unit = gameLogic.GetUnitIdFromTile(tilePoint.X, tilePoint.Y);
        if (gameLogic.turn < 2) // prva dva poteza je click and move uključeno
        {
            if (gameLogic.unitBeingDragged == -1)
            {
                if (unit == -1) return;
                gameLogic.unitBeingDragged = unit;
                gameLogic.GetUnitGameObjectFromId(unit).GetComponent<Draggable>().FollowCursor();
                gameLogic.clickSound.Play();
                return;
            }
            if (!gameLogic.HighlightedTiles.Contains(tilePoint) || gameLogic.IsTileOccupied(tilePoint.X, tilePoint.Y))
            {
                gameLogic.GetUnitGameObjectFromId(gameLogic.unitBeingDragged).GetComponent<Draggable>().StopFollowingCursor();
                StartCoroutine(gameLogic.GetUnitGameObjectFromId(gameLogic.unitBeingDragged).MoveToTile(gameLogic.GetUnitDataFromId(gameLogic.unitBeingDragged).Position));
                gameLogic.unitBeingDragged = -1;
                return;
            }
            gameLogic.MoveUnitData(gameLogic.unitBeingDragged, new Point(tilePoint.X, tilePoint.Y));
            var unitObject = gameLogic.GetUnitGameObjectFromId(gameLogic.unitBeingDragged);
            unitObject.GetComponent<Draggable>().StopFollowingCursor();
            unitObject.gameObject.transform.position =
                Grid.GetPositionFromTile(tilePoint);
            gameLogic.unitBeingDragged = -1;
            gameLogic.clickSound.Play();
            return;
        }
        switch (gameLogic.currentSelectMode)
        {
            case OrderType.No:
                gameLogic.ShowPanelForUnit(gameLogic.GetUnitIdFromTile(tilePoint.X,tilePoint.Y));
                break;
            case OrderType.Move:
                AddMoveOrderToThisTile();
                break;
            case OrderType.Fire:
                AddFireOrderForThisUnit();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

    }

    public void AddMoveOrderToThisTile()
    {
        var tileHash = Convert.ToInt32(gameObject.name);
        var id = gameLogic.unitSelected;
                
        if (gameLogic.HighlightedTiles.Contains(gameLogic.GetPointFromTileHash(tileHash)))
        {
            gameLogic.GetUnitDataFromId(id).Order = new Order(tileHash, OrderType.Move);
            gameLogic.orders[gameLogic.GetUnitsTeam(id)].AddLast(id);
            gameLogic.Draw_OrderGiven(id);
            gameLogic.clickSound.Play();
        }
        gameLogic.DeselectUnit();
    }

    public void AddFireOrderForThisUnit()
    {
        var tileHash = Convert.ToInt32(gameObject.name);
        var id = gameLogic.unitSelected;
        if (gameLogic.HighlightedTiles.Contains(gameLogic.GetPointFromTileHash(tileHash)))
        {
            gameLogic.GetUnitDataFromId(id).Order = new Order(gameLogic.GetUnitIdFromTile(tileHash/399,tileHash%399), OrderType.Fire);
            gameLogic.orders[gameLogic.GetUnitsTeam(id)].AddLast(id);
            gameLogic.Draw_OrderGiven(id);
            gameLogic.clickSound.Play();
        }
        gameLogic.DeselectUnit();
    }
}
