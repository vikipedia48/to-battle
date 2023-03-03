using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Color = UnityEngine.Color;

public class Grid : MonoBehaviour
{
    public GameObject tilePrefab;

    [FormerlySerializedAs("lightInfantryPrefab")]
    public GameObject unitPrefab;

    public GameObject rankPrefab;
    public GameObject fortifyPrefab;
    public GameObject chargePrefab;
    public GameObject arrowsPrefab;
    public GameObject lossBubblePrefab;
    public Sprite rank3Image;
    public Sprite generalImage;
    public Sprite lightInfantryImage;
    public Sprite heavyInfantryImage;
    public Sprite spearInfantryImage;
    public Sprite missileInfantryImage;
    public Sprite lightCavalryImage;
    public Sprite heavyCavalryImage;
    public Sprite missileCavalryImage;

    public CameraController cameraControls;
    public GameObject MainCamera;
    public TextMeshProUGUI PlayerTurnText;
    public Button endTurnButton;
    public Text endTurnText;
    public GameObject InGamePanel;
    public GameObject UnitInfoPanel;
    public TextMeshProUGUI UnitInfoText;
    public Button attackButton;
    public RawImage attackButtonImage;
    public Texture attackIcon;
    public Button fortifyButton;
    public RawImage fortifyButtonImage;
    public Texture fortifyIcon;
    public Button fireButton;
    public RawImage fireButtonImage;
    public Texture fireIcon;
    public Texture cancelIcon;
    public GameObject confirmNewGamePanel;

    public AudioSource victorySound;
    public AudioSource clickSound;

    public int width, height;
    public short[,] tiles_units;

    public LinkedList<short>[] orders;
    public List<UnitData>[] playerUnits;
    public List<Unit>[] gameUnits;
    public SortedSet<(short, short)> unitsWhoHaveFoughtThisTurn;
    public HashSet<short>[] unitsRalliedByGeneral;
    public GameObject[,] TilesObjects;
    public Color[] teamColors;
    public List<Point> HighlightedTiles;

    public int turn;
    public bool disableSelecting = false;
    public bool animationPlaying;
    public bool gameOver = false;
    public short unitBeingDragged = -1;
    public short unitSelected;
    public OrderType currentSelectMode;

    public static Color unselectedColor = new Color(0.05f, 0.6f, 0.1f);
    public static Color selectedColor = new Color(0.1f, 0.3f, 1f);
    public static Color spearmenFortifiedColor = new Color(0.1f, 0.2f, 0.75f);
    public static Color attackColor = new Color(1f, 0.1f, 0.1f);
    public static Color beginningSelectedColor = new Color(0.5f, 0.5f, 0.2f);
    public static int ChargeDistance = 3;
    public static int FortifiedSpearmenMovementDistance = 2;
    public static float BaseCombatLosses = 10;
    public static float RelativeCombatLosses = 0.16f;
    public static float SpearBonusVsCavalry = 3;
    public static float FortifyBonus = 2;
    public static float BaseMorale = 100;
    public static float BaseCombatSkill = 100;
    public static byte MoraleAnimationTrigger = 40;
    public static float FlankedMoraleLoss = 0.15f;
    public static float BaseMissileInfantryDamage = 0.5f;
    public static float BaseMissileCavalryDamage = 0.4f;
    public static float MissileFireCenterTilePercentage = 0.6f;
    public static short MoraleLossUponSeeingUnitDeath = 5;
    public static int MoraleLossDistance = 1;
    public static float MoraleLossUponSeeingUnitRoute = 0.15f;
    public static float PercentageOfTeamsStartingSpace = 0.2f;
    public static float CombatSkillMultipliedByRank = 0.25f;
    public static float MoraleMultipliedByRank = 0.25f;
    public static short GeneralMoraleRaisingAmount = 30;
    public static short GeneralDeathMoraleLoss = 25;
    public static int GeneralMoraleRaisingRadius = 4; 
    public static float CavalryVsCavalryChargeFactor = 0.8f;

    #region ArrayGetters

    public Unit GetUnitGameObjectFromId(short id)
    {
        return gameUnits[id % 2][id / 2];
    }

    public short GetUnitIdFromTile(int x, int y)
    {
        return tiles_units[x, y];
    }

    public UnitData GetUnitDataFromId(short id)
    {
        return playerUnits[id % 2][id / 2];
    }

    public Point GetPointFromTileHash(int hash)
    {
        return new Point(hash / 399, hash % 399);
    }

    public int GetUnitsTeam(short id)
    {
        if (id == -1) return -1;
        return id % 2;
    }

    public int GetTeamOnTurn()
    {
        return turn % 2;
    }

    public bool IsTileOccupied(int i, int j)
    {
        if (i >= width || i < 0 || j >= height || j < 0) return false;
        return tiles_units[i, j] != -1;
    }

    public bool IsTileOccupiedByEnemy(int i, int j, int team)
    {
        if (i >= width || i < 0 || j >= height || j < 0) return false;
        if (tiles_units[i, j] == -1) return false;
        return tiles_units[i, j] % 2 != team;
    }

    public bool IsTileOccupiedByFriendly(int i, int j, int team)
    {
        if (i >= width || i < 0 || j >= height || j < 0) return false;
        if (tiles_units[i, j] == -1) return false;
        return tiles_units[i, j] % 2 == team;
    }

    public bool DoesPlayer1CurrentlyHaveFirstTurn()
    {
        return turn % 4 == 1;
    }

    #endregion

    #region Gui

    public void DrawArrowsFlying(Point pos1, Point pos2)
    {
        var arrowObject = Instantiate(arrowsPrefab,
            GetPositionFromTile(pos1), Quaternion.identity);
        
        arrowObject.GetComponent<Arrows>().pos1 = pos1;
        arrowObject.GetComponent<Arrows>().pos2 = pos2;
        StartCoroutine(arrowObject.GetComponent<Arrows>().FireAtPoint());
        //var delta = Math.Sqrt(Math.Pow(pos1.X - pos2.X, 2) + Math.Pow(pos1.Y - pos2.Y, 2));
        //var animationHeight = delta * ArrowAnimationHeightFactor;

    }
    public void ChangeWaveringAnimation(short id, bool wavering)
    {
        //GetUnitGameObjectFromId(id).GetComponent<Animator>().Play(wavering ? "UnitMorale" : "UnitNormal");
        StartCoroutine(GetUnitGameObjectFromId(id).ChangeWaveringAnimation(wavering));
    }

    public void DisplayLossBubble(Point pos, short losses, Color color)
    {
        var bubble = Instantiate(lossBubblePrefab,
            GetPositionFromTile(pos), Quaternion.identity);
        bubble.GetComponent<LossBubble>().SetText("-"+losses);
        bubble.GetComponent<LossBubble>().SetColor(color);
        StartCoroutine(bubble.GetComponent<LossBubble>().DoAnimation());
    }
    private void UnitDiesOnScreen(short id)
    {
        StartCoroutine(GetUnitGameObjectFromId(id).Die());
    }    
    public void MoveUnitOnScreen(short id, List<Point> path, bool destroyAfterwards = false)
    {
        StartCoroutine(GetUnitGameObjectFromId(id).MoveToPath(path, destroyAfterwards));
    }

    private void AnimateFortification(GameObject fortifyObject, bool spawn)
    {
        StartCoroutine(fortifyObject.GetComponent<FortifyAnimation>().PikeAnimation(spawn));
    }
    public static Vector3 GetPositionFromTile(Point tile, float z = 0)
    {
        return new Vector3((0.5f + tile.X), (-0.5f - tile.Y), z);
    }

    private void ClearHighlightedTiles()
    {
        foreach (var v in HighlightedTiles)
        {
            ChangeTileColor(v.X, v.Y, unselectedColor);
        }

        HighlightedTiles.Clear();
    }

    public void HighlightTilesInRange(short id)
    {
        ClearHighlightedTiles();
        if (id < 0) return;
        var unit = GetUnitDataFromId(id);
        var distance = unit.GetMovementDistance();

        int startX = Math.Max(0, unit.Position.X - distance),
            endX = Math.Min(width-1, unit.Position.X + distance),
            startY = Math.Max(0, unit.Position.Y - distance),
            endY = Math.Min(height-1, unit.Position.Y + distance);

        for (var i = startX; i <= endX; ++i)
        {
            for (var j = startY; j <= endY; ++j)
            {
                var delta = Math.Sqrt(Math.Pow(unit.Position.X - i, 2) + Math.Pow(unit.Position.Y - j, 2));
                if (delta > distance || delta == 0) continue;
                HighlightedTiles.Add(new Point(i, j));
                ChangeTileColor(i, j,
                    (unit.Category == UnitCategory.SpearInfantry && unit.IsFortified &&
                     delta <= FortifiedSpearmenMovementDistance)
                        ? spearmenFortifiedColor
                        : selectedColor);
            }
        }
    }

    public void HighlightEnemiesInRange(short id, int distance = -1)
    {
        ClearHighlightedTiles();
        if (id < 0) return;
        var unit = GetUnitDataFromId(id);
        if (distance == -1) distance = unit.GetFireDistance();
        int startX = Math.Max(0, unit.Position.X - distance),
            endX = Math.Min(width-1, unit.Position.X + distance),
            startY = Math.Max(0, unit.Position.Y - distance),
            endY = Math.Min(height-1, unit.Position.Y + distance);
        for (var i = startX; i <= endX; ++i)
        {
            for (var j = startY; j <= endY; ++j)
            {
                var delta = Math.Sqrt(Math.Pow(unit.Position.X - i, 2) + Math.Pow(unit.Position.Y - j, 2));
                if (delta > distance || !IsTileOccupiedByEnemy(i, j, GetUnitsTeam(id))) continue;
                HighlightedTiles.Add(new Point(i, j));
                ChangeTileColor(i, j, selectedColor);
            }
        }
    }

    public void HighlightBeginningTiles()
    {
        ClearHighlightedTiles();
        int startX, endX;
        if (turn == 0)
        {
            startX = 0;
            endX = (int)(PercentageOfTeamsStartingSpace * width);
        }
        else
        {
            startX = width - (int)(PercentageOfTeamsStartingSpace * width);
            endX = width;
        }

        for (var i = startX; i < endX; ++i)
        {
            for (var j = 0; j < height; ++j)
            {
                HighlightedTiles.Add(new Point(i, j));
                ChangeTileColor(i, j, beginningSelectedColor);
            }
        }
    }

    public void ChangeTileColor(int i, int j, Color color)
    {
        TilesObjects[i, j].GetComponent<SpriteRenderer>().color = color;
    }

    public void HighlightFreeFireTile(short id)
    {
        if (id < 0) return;
        var pos = GetUnitDataFromId(id).Position;
        HighlightedTiles.Add(pos);
        ChangeTileColor(pos.X, pos.Y, attackColor);
    }

    public void OrderCancelled()
    {
        currentSelectMode = OrderType.No;
        ClearHighlightedTiles();
        ShowPanelForUnit(unitSelected);
        Draw_OrderCancelled(unitSelected);
    }

    public void Draw_OrderGiven(short id)
    {
        var color = GetUnitGameObjectFromId(id).GetComponent<SpriteRenderer>().color;
        GetUnitGameObjectFromId(id).GetComponent<SpriteRenderer>().color = new Color(
            color.r * Unit.OrderGiven_DarkerColor, color.g * Unit.OrderGiven_DarkerColor,
            color.b * Unit.OrderGiven_DarkerColor);
    }

    public void Draw_OrderCancelled(short id)
    {
        var color = GetUnitGameObjectFromId(id).GetComponent<SpriteRenderer>().color;
        GetUnitGameObjectFromId(id).GetComponent<SpriteRenderer>().color = new Color(
            color.r / Unit.OrderGiven_DarkerColor, color.g / Unit.OrderGiven_DarkerColor,
            color.b / Unit.OrderGiven_DarkerColor);
    }    
    public void HideEnemyUnits() // poziva se samo u prva dva turna
    {
        var enemyTeam = (turn == 0) ? 1 : 0;
        foreach (var team in playerUnits)
        {
            foreach (var v in team)
            {
                GetUnitGameObjectFromId(v.id).gameObject.SetActive(enemyTeam != GetUnitsTeam(v.id));
            }
        }
    }

    public void RevealAllUnits() // poziva se nakon drugog turna
    {
        foreach (var team in playerUnits)
        {
            foreach (var v in team)
            {
                GetUnitGameObjectFromId(v.id).gameObject.SetActive(true);
            }
        }
    }

    public void ShowPanelForUnit(short id)
    {
        if (id < 0)
        {
            DeselectUnit();
            return;
        }

        clickSound.Play();
        cameraControls.minClickableX = 200;
        unitSelected = id;
        var data = GetUnitDataFromId(id);

        UnitInfoText.text = data.CategoryToString() + "\n\nSoldiers : " + data.Soldiers + "\n\nMorale : " + data.Morale;
        attackButtonImage.texture = attackIcon;
        fireButtonImage.texture = fireIcon;
        fortifyButtonImage.texture = fortifyIcon;

        if (GetUnitsTeam(unitSelected) != GetTeamOnTurn())
        {
            attackButton.enabled = false;
            fortifyButton.enabled = false;
            fireButton.enabled = false;
            UnitInfoPanel.SetActive(true);
            return;
        }

        attackButton.enabled = true;
        fortifyButton.enabled = data.Category is not (UnitCategory.General or UnitCategory.HeavyCavalry
            or UnitCategory.LightCavalry
            or UnitCategory.MissileCavalry);
        fireButton.enabled = data.Category is UnitCategory.MissileInfantry or UnitCategory.MissileCavalry;

        var order = GetUnitDataFromId(id).Order;
        switch (order.type)
        {
            case OrderType.Move:
                attackButtonImage.texture = cancelIcon;
                var orderedTile = GetPointFromTileHash(order.target);
                HighlightedTiles.Add(orderedTile);
                ChangeTileColor(orderedTile.X, orderedTile.Y, attackColor);
                break;
            case OrderType.Fire:
                fireButtonImage.texture = cancelIcon;
                var target = GetUnitDataFromId((short)order.target).Position;
                HighlightedTiles.Add(target);
                ChangeTileColor(target.X, target.Y, attackColor);
                break;
            case OrderType.Fortify:
                fortifyButtonImage.texture = cancelIcon;
                break;
        }

        UnitInfoPanel.SetActive(true);
    }

    public void DeselectUnit()
    {
        unitSelected = -1;
        currentSelectMode = OrderType.No;
        ClearHighlightedTiles();
        UnitInfoPanel.SetActive(false);
        cameraControls.minClickableX = 0;
    }
    public IEnumerator PlayVictorySound()
    {
        while (animationPlaying)
        {
            yield return new WaitForSeconds(0.1f);
        }

        animationPlaying = true;
        victorySound.Play();
        animationPlaying = false;
    }

    #endregion

    #region GameLogic

        public List<Point> FindPath(Point start, Point destination, short[,] array, Func<int, int, bool> IsPassable)
    {
        var pathfinder = new AStarSearch(new SquareGrid(width, height), start, destination, IsPassable);
        return pathfinder.GetPath();
    }

    private List<Point> FindPathInThatDirection(Point start, Point destination, int team)
    {
        var path = FindPath(start, destination, tiles_units, (_, _) => true);
        for (var i = 1; i < path.Count; ++i)
        {
            if (IsTileOccupiedByEnemy(path[i].X, path[i].Y, team))
            {
                path = path.GetRange(0, i);
                break;
            }

            if (CheckIfTileStartsFight(path[i], team))
            {
                path = path.GetRange(0, i + 1);
                break;
            }
        }

        return path;
    }

    private void MoveUnit(short id, int target)
    {
        var unit = GetUnitDataFromId(id);
        var team = GetUnitsTeam(id);
        var start = GetUnitDataFromId(id).Position;
        var destination = GetPointFromTileHash(target);

        var path = FindPath(start, destination, tiles_units, (x, y) =>
        {
            if (start.X == x && start.Y == y) return true;

            if (destination.X == x && destination.Y == y) return true;

            if (IsTileOccupiedByEnemy(x, y, team)) return false;

            if (CheckIfTileStartsFight(new Point(x, y), team)) return false;
            
            return true;
        });
        if (path.Count == 0 || path.Count > unit.GetMovementDistance())
        {
            path = FindPathInThatDirection(start, destination, team);
        }

        while (path.Count > 0)
        {
            if (IsTileOccupied(path[^1].X, path[^1].Y)) path.RemoveAt(path.Count - 1);
            else break;
        }

        if (path.Count > 1) // path[0] je početni tile
        {
            if (!(unit.Category == UnitCategory.SpearInfantry && unit.IsFortified &&
                  path.Count-1 <= FortifiedSpearmenMovementDistance)) SetUnitFortification(id, false);
            var temp = path[0];
            MoveUnitData(id, path[^1]);
            var enemy = FindFirstEnemyInNeighbourTiles(id);
            if (enemy != -1 && path.Count > ChargeDistance)
            {
                var chargeObject = Instantiate(chargePrefab, GetUnitGameObjectFromId(id).gameObject.transform);
                if (temp.X > unit.Position.X) chargeObject.GetComponent<SpriteRenderer>().flipX = true;
            }
            MoveUnitOnScreen(id, path);
            if (unit.Category == UnitCategory.General) UpdateListOfUnitsRalliedByGeneral(team);

            var dist = GetDistanceFromGeneral(id);
            if (dist <= GeneralMoraleRaisingRadius)
            {
                if (unitsRalliedByGeneral[team].Add(id)) ChangeMorale(id, GeneralMoraleRaisingAmount);
            }
            else if (unitsRalliedByGeneral[team].Remove(id)) ChangeMorale(id, -GeneralMoraleRaisingAmount);
            //ScareFlankedEnemyUnits(id);

            if (enemy != -1)
            {
                if (!unitsWhoHaveFoughtThisTurn.Contains(SortedPair(id, enemy)))
                    GetIntoCombat(id, enemy, path.Count > ChargeDistance);
            }
            else if (unit.Category == UnitCategory.MissileCavalry) FireUnit(id, id);
        }
    }

    public double GetDistanceFromGeneral(short id)
    {
        var generalUnit = GetUnitDataFromId((short)GetUnitsTeam(id));
        if (!generalUnit.IsAlive) return double.MaxValue;
        var position = GetUnitDataFromId(id).Position;
        return Math.Sqrt(Math.Pow(generalUnit.Position.X - position.X, 2) +
                         Math.Pow(generalUnit.Position.Y - position.Y, 2));
    }


    private bool CheckIfTileStartsFight(Point tile, int team)
    {
        if (IsTileOccupiedByEnemy(tile.X + 1, tile.Y, team)) return true;
        if (IsTileOccupiedByEnemy(tile.X - 1, tile.Y, team)) return true;
        if (IsTileOccupiedByEnemy(tile.X, tile.Y + 1, team)) return true;
        if (IsTileOccupiedByEnemy(tile.X, tile.Y - 1, team)) return true;
        return false;
    }

    public void GetIntoCombat(short attacker, short defender, bool charge)
    {
        //DrawSwords(GetUnitDataFromId(attacker).Position, GetUnitDataFromId(defender).Position);
        Fight(attacker, defender, charge);
        unitsWhoHaveFoughtThisTurn.Add(SortedPair(attacker, defender));
    }
    void ProcessOrder(short id)
    {
        var unit = GetUnitDataFromId(id);
        if (!unit.IsAlive) return;
        switch (unit.Order.type)
        {
            case OrderType.Move:
                MoveUnit(unit.id, unit.Order.target);
                break;
            case OrderType.Fortify:
                SetUnitFortification(unit.id, !GetUnitDataFromId(id).IsFortified);
                break;
            case OrderType.Fire:
                FireUnit(unit.id, unit.Order.target);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        GetUnitDataFromId(id).Order.type = OrderType.No;
    }

    public void PlayAllOrders()
    {
        disableSelecting = true;
        
        int firstTurn, secondTurn;
        if (DoesPlayer1CurrentlyHaveFirstTurn())
        {
            firstTurn = 0;
            secondTurn = 1;
        }
        else
        {
            secondTurn = 0;
            firstTurn = 1;
        }

        while (orders[0].Count > 0 || orders[1].Count > 0)
        {
            var currentUnit1 = orders[firstTurn].First;
            if (currentUnit1 != null)
            {
                ProcessOrder(currentUnit1.Value);
                orders[firstTurn].RemoveFirst();
            }

            var currentUnit2 = orders[secondTurn].First;
            if (currentUnit2 != null)
            {
                ProcessOrder(currentUnit2.Value);
                orders[secondTurn].RemoveFirst();
            }
        }

        foreach (var team in playerUnits)
        {
            foreach (var v in team)
            {
                if (v.IsAlive) FightEveryoneAround(v);
            }
        }

        unitsWhoHaveFoughtThisTurn.Clear();
        disableSelecting = false;
    }

    public void FightEveryoneAround(UnitData unit)
    {
        var pos = unit.Position;
        var team = GetUnitsTeam(unit.id);
        var flanked = 0;
        if (IsTileOccupiedByEnemy(pos.X + 1, pos.Y, team))
        {
            var unitThere = GetUnitIdFromTile(pos.X + 1, pos.Y);
            flanked++;
            if (!unitsWhoHaveFoughtThisTurn.Contains(SortedPair(unit.id, unitThere)))
                GetIntoCombat(unit.id, unitThere, false);
            if (!unit.IsAlive) return;
        }

        if (IsTileOccupiedByEnemy(pos.X - 1, pos.Y, team))
        {
            var unitThere = GetUnitIdFromTile(pos.X - 1, pos.Y);
            if (flanked++ > 0) ChangeMorale(unit.id, (int)(-FlankedMoraleLoss * BaseMorale * flanked));
            if (!unit.IsAlive) return;
            if (!unitsWhoHaveFoughtThisTurn.Contains(SortedPair(unit.id, unitThere)))
                GetIntoCombat(unit.id, unitThere, false);
            if (!unit.IsAlive) return;
        }

        if (IsTileOccupiedByEnemy(pos.X, pos.Y + 1, team))
        {
            var unitThere = GetUnitIdFromTile(pos.X, pos.Y + 1);
            if (flanked++ > 0) ChangeMorale(unit.id, (int)(-FlankedMoraleLoss * BaseMorale * flanked));
            if (!unit.IsAlive) return;
            if (!unitsWhoHaveFoughtThisTurn.Contains(SortedPair(unit.id, unitThere)))
                GetIntoCombat(unit.id, unitThere, false);
            if (!unit.IsAlive) return;
        }

        if (IsTileOccupiedByEnemy(pos.X, pos.Y - 1, team))
        {
            var unitThere = GetUnitIdFromTile(pos.X, pos.Y - 1);
            if (flanked++ > 0) ChangeMorale(unit.id, (int)(-FlankedMoraleLoss * BaseMorale * flanked));
            if (!unit.IsAlive) return;
            if (!unitsWhoHaveFoughtThisTurn.Contains(SortedPair(unit.id, unitThere)))
                GetIntoCombat(unit.id, unitThere, false);
        }
    }

    public void MoveUnitData(short id, Point destination)
    {
        if (IsTileOccupied(destination.X, destination.Y)) throw new ArgumentException();
        tiles_units[GetUnitDataFromId(id).Position.X, GetUnitDataFromId(id).Position.Y] = -1;
        tiles_units[destination.X, destination.Y] = id;
        GetUnitDataFromId(id).Position = destination;
    }

    private void FireUnit(short id, int target)
    {
        if (FindFirstEnemyInNeighbourTiles(id) != -1) return;

        var enemyUnit = GetUnitDataFromId((short)target);
        if (!enemyUnit.IsAlive) return;
        var enemyPos = enemyUnit.Position;
        var unit = GetUnitDataFromId(id);
        if (enemyUnit.id == id) // free fire
        {
            var team = GetUnitsTeam(id);
            var maxdist = unit.GetFireDistance();
            var dist = 1;
            while (dist <= maxdist)
            {
                var fixd = Math.Min(height - 1, unit.Position.Y + dist);
                for (var i = Math.Max(0, unit.Position.X - dist); i < Math.Min(width, unit.Position.X + dist + 1); ++i)
                {
                    if (IsTileOccupiedByEnemy(i, fixd, team))
                    {
                        if (Math.Sqrt(Math.Pow(enemyPos.X - unit.Position.X, 2) +
                                      Math.Pow(enemyPos.Y - unit.Position.Y, 2)) > maxdist) continue;
                        enemyUnit = GetUnitDataFromId(GetUnitIdFromTile(i, fixd));
                        enemyPos = enemyUnit.Position;
                        goto startfiring;
                    }
                }

                fixd = Math.Max(0, unit.Position.Y - dist);
                for (var i = Math.Max(0, unit.Position.X - dist); i < Math.Min(width, unit.Position.X + dist + 1); ++i)
                {
                    if (IsTileOccupiedByEnemy(i, fixd, team))
                    {
                        if (Math.Sqrt(Math.Pow(enemyPos.X - unit.Position.X, 2) +
                                      Math.Pow(enemyPos.Y - unit.Position.Y, 2)) > maxdist) continue;
                        enemyUnit = GetUnitDataFromId(GetUnitIdFromTile(i, fixd));
                        enemyPos = enemyUnit.Position;
                        goto startfiring;
                    }
                }

                fixd = Math.Min(width - 1, unit.Position.X + dist);
                for (var i = Math.Max(0, unit.Position.Y - dist + 1); i < Math.Min(width, unit.Position.Y + dist); ++i)
                {
                    if (IsTileOccupiedByEnemy(fixd, i, team))
                    {
                        if (Math.Sqrt(Math.Pow(enemyPos.X - unit.Position.X, 2) +
                                      Math.Pow(enemyPos.Y - unit.Position.Y, 2)) > maxdist) continue;
                        enemyUnit = GetUnitDataFromId(GetUnitIdFromTile(fixd,i));
                        enemyPos = enemyUnit.Position;
                        goto startfiring;
                    }
                }

                fixd = Math.Max(0, unit.Position.X - dist);
                for (var i = Math.Max(0, unit.Position.Y - dist + 1); i < Math.Min(width, unit.Position.Y + dist); ++i)
                {
                    if (IsTileOccupiedByEnemy(fixd, i, team))
                    {
                        if (Math.Sqrt(Math.Pow(enemyPos.X - unit.Position.X, 2) +
                                      Math.Pow(enemyPos.Y - unit.Position.Y, 2)) > maxdist) continue;
                        enemyUnit = GetUnitDataFromId(GetUnitIdFromTile(fixd,i));
                        enemyPos = enemyUnit.Position;
                        goto startfiring;
                    }
                }

                ++dist;
            }

            return;
        }

        startfiring: ;
        var delta = Math.Sqrt(Math.Pow(enemyPos.X - unit.Position.X, 2) + Math.Pow(enemyPos.Y - unit.Position.Y, 2));
        if (delta > unit.GetFireDistance() || delta <= 1) return;
        DrawArrowsFlying(unit.Position, enemyUnit.Position);
        short casualties = 0;
        if (unit.Category == UnitCategory.MissileInfantry)
            casualties = (short)(BaseMissileInfantryDamage * unit.Soldiers * unit.CombatSkill / 100);
        else if (unit.Category == UnitCategory.MissileCavalry)
            casualties = (short)(BaseMissileCavalryDamage * unit.Soldiers * unit.CombatSkill / 100);

        DamageUnit(enemyUnit.id,
            (short)(MissileFireCenterTilePercentage * casualties * enemyUnit.GetDamageFromMissileFire()));

        if (IsTileOccupied(enemyPos.X + 1, enemyPos.Y))
        {
            enemyUnit = GetUnitDataFromId(GetUnitIdFromTile(enemyPos.X + 1, enemyPos.Y));
            DamageUnit(GetUnitIdFromTile(enemyPos.X + 1, enemyPos.Y),
                (short)((1 - MissileFireCenterTilePercentage) / 4 * casualties * enemyUnit.GetDamageFromMissileFire()));
        }

        if (IsTileOccupied(enemyPos.X - 1, enemyPos.Y))
        {
            enemyUnit = GetUnitDataFromId(GetUnitIdFromTile(enemyPos.X - 1, enemyPos.Y));
            DamageUnit(GetUnitIdFromTile(enemyPos.X - 1, enemyPos.Y),
                (short)((1 - MissileFireCenterTilePercentage) / 4 * casualties * enemyUnit.GetDamageFromMissileFire()));
        }

        if (IsTileOccupied(enemyPos.X, enemyPos.Y + 1))
        {
            enemyUnit = GetUnitDataFromId(GetUnitIdFromTile(enemyPos.X, enemyPos.Y + 1));
            DamageUnit(GetUnitIdFromTile(enemyPos.X, enemyPos.Y + 1),
                (short)((1 - MissileFireCenterTilePercentage) / 4 * casualties * enemyUnit.GetDamageFromMissileFire()));
        }

        if (IsTileOccupied(enemyPos.X, enemyPos.Y - 1))
        {
            enemyUnit = GetUnitDataFromId(GetUnitIdFromTile(enemyPos.X, enemyPos.Y - 1));
            DamageUnit(GetUnitIdFromTile(enemyPos.X, enemyPos.Y - 1),
                (short)((1 - MissileFireCenterTilePercentage) / 4 * casualties * enemyUnit.GetDamageFromMissileFire()));
        }
    }

    public void DamageUnit(short id, short casualties)
    {
        if (id == -1) return;
        var unit = GetUnitDataFromId(id);
        DisplayLossBubble(unit.Position, Math.Min(unit.Soldiers, casualties), teamColors[GetUnitsTeam(id)]);
        if (unit.Soldiers <= casualties)
        {
            UnitDies(unit.id);
            return;
        }

        float moraleLoss = (float)casualties / (float)unit.GetInitialSize() * (float)BaseMorale;
        ChangeMorale(id, (byte)-moraleLoss);

        unit.Soldiers -= casualties;
        //unit.Morale -= (byte)moraleLoss;
    }

    


    private void SetUnitFortification(short id, bool state)
    {
        GetUnitDataFromId(id).IsFortified = state;
        var unitObject = GetUnitGameObjectFromId(id);
        //MainCamera.transform.position = GetPositionFromTile(GetUnitDataFromId(id).Position, -10);
            if (state)
            {
                var fortifyObject = Instantiate(fortifyPrefab, unitObject.transform);
                AnimateFortification(fortifyObject.gameObject,true);
            }
            else
            {
                Transform[] children = unitObject.GetComponentsInChildren<Transform>();
                foreach (var v in children)
                {
                    if (v.tag.Equals("fortify"))
                    {
                        AnimateFortification(v.gameObject, false);
                        return;
                    }
                }
            }
    }
    
    public void EndTurn()
    {
        if (gameOver)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            return;
        }

        DeselectUnit();
        foreach (var v in orders[GetTeamOnTurn()])
        {
            Draw_OrderCancelled(v);
        }

        if (GetTeamOnTurn() == 0) PlayerTurnText.text = "Player 2's Turn";
        else
        {
            endTurnButton.gameObject.SetActive(false);
            PlayAllOrders();
            var victory = CheckIfVictory();
            if (victory == 0)
            {
                endTurnButton.gameObject.SetActive(true);
                PlayerTurnText.text = "Player 1's Turn";
            }
            else
            {
                disableSelecting = true;
                endTurnButton.gameObject.SetActive(true);
                StartCoroutine(PlayVictorySound());
                // izvješće, napravi poslije možda
                if (victory == -1) PlayerTurnText.text = "DRAW";
                else if (victory == 1) PlayerTurnText.text = "PLAYER 1 WINS";
                else if (victory == 2) PlayerTurnText.text = "PLAYER 2 WINS";
                endTurnText.text = "New Game";
                gameOver = true;
                return;
            }
        }

        ++turn;
        if (turn == 1)
        {
            HideEnemyUnits();
            HighlightBeginningTiles();
        }
        else if (turn == 2)
        {
            RevealAllUnits();
            foreach (var team in gameUnits)
            {
                foreach (var v in team)
                {
                    Destroy(v.GetComponent<Draggable>());
                }
            }
            PopulateListOfUnitsRalliedByGeneral(0);
            PopulateListOfUnitsRalliedByGeneral(1);
        }

        var first = playerUnits[GetTeamOnTurn()].First(x => x.IsAlive);
        MainCamera.transform.position = GetPositionFromTile(first.Position,-10);
    }

    public int CheckIfVictory()
    {
        var units1 = playerUnits[0].Count(x => x.IsAlive);
        var units2 = playerUnits[1].Count(x => x.IsAlive);

        if (units1 + units2 == 0) return -1;
        if (units1 == 0) return 2;
        if (units2 == 0) return 1;
        return 0;
    }

    public void AddFortifyOrderForSelectedUnit()
    {
        GetUnitDataFromId(unitSelected).Order = new Order(-1, OrderType.Fortify);
        AddOrder(unitSelected);
        Draw_OrderGiven(unitSelected);
        DeselectUnit();
    }
    public void RemoveOrder(short id)
    {
        orders[id % 2].Remove(id);
    }

    public void AddOrder(short id)
    {
        orders[id % 2].AddLast(id);
    }

    public void CancelOrder(short id)
    {
        RemoveOrder(id);
        GetUnitDataFromId(id).Order.type = OrderType.No;
    }
        public static (short item1, short item2) SortedPair(short item1, short item2)
    {
        if (item1 < item2) return (item1, item2);
        return (item2, item1);
    }

    public void Fight(short attacker, short defender, bool charge)
    {
        var units = new UnitData[2];
        units[0] = GetUnitDataFromId(attacker);
        units[1] = GetUnitDataFromId(defender);

        var casualties = new float[2];
        casualties[0] = units[1].Soldiers * RelativeCombatLosses + BaseCombatLosses;
        casualties[1] = units[0].Soldiers * RelativeCombatLosses + BaseCombatLosses;

        if (units[0].Category == UnitCategory.SpearInfantry && units[1].IsCavalry())
            casualties[0] /= SpearBonusVsCavalry;
        else if (units[1].Category == UnitCategory.SpearInfantry && units[0].IsCavalry())
            casualties[1] /= SpearBonusVsCavalry;

        if (units[0].IsFortified) casualties[0] /= FortifyBonus;
        if (units[1].IsFortified) casualties[1] /= FortifyBonus;

        if (charge) casualties[1] *= units[0].GetChargeBonus() * (units[0].IsCavalry() && units[1].IsCavalry() ? CavalryVsCavalryChargeFactor : 1);

        casualties[0] *= units[1].CombatSkill / BaseCombatSkill;
        casualties[1] *= units[0].CombatSkill / BaseCombatSkill;

        DamageUnit(units[0].id, (short)casualties[0]);
        DamageUnit(units[1].id, (short)casualties[1]);
    }

    public void PopulateListOfUnitsRalliedByGeneral(int team)
    {
        var unit = GetUnitDataFromId((short)team);
        for (var i = Math.Max(0, unit.Position.X - GeneralMoraleRaisingRadius); i <= Math.Min(width - 1,
                 unit.Position.X + GeneralMoraleRaisingRadius); ++i)
        {
            for (var j = Math.Max(0, unit.Position.Y - GeneralMoraleRaisingRadius); j <= Math.Min(height - 1,
                     unit.Position.Y + GeneralMoraleRaisingRadius); ++j)
            {
                var delta = Math.Sqrt(Math.Pow(unit.Position.X - i, 2) + Math.Pow(unit.Position.Y - j, 2));
                if (delta > GeneralMoraleRaisingRadius) continue;
                if (IsTileOccupiedByFriendly(i, j, team))
                {
                    var nearbyUnit = GetUnitIdFromTile(i, j);
                    unitsRalliedByGeneral[team].Add(nearbyUnit);
                    ChangeMorale(nearbyUnit, GeneralMoraleRaisingAmount);
                }
            }
        }
    }
    
    public void UpdateListOfUnitsRalliedByGeneral(int team)
    {
            var unit = GetUnitDataFromId((short)team); // general je uvijek id = 0 ili 1 
            var tempList = unitsRalliedByGeneral[team];
            unitsRalliedByGeneral[team].Clear();
                for (var i = Math.Max(0, unit.Position.X - GeneralMoraleRaisingRadius); i <= Math.Min(width - 1,
                         unit.Position.X + GeneralMoraleRaisingRadius); ++i)
                {
                    for (var j = Math.Max(0, unit.Position.Y - GeneralMoraleRaisingRadius); j <= Math.Min(height - 1,
                             unit.Position.Y + GeneralMoraleRaisingRadius); ++j)
                    {
                        var delta = Math.Sqrt(Math.Pow(unit.Position.X - i, 2) + Math.Pow(unit.Position.Y - j, 2));
                        if (delta > GeneralMoraleRaisingRadius) continue;
                        if (IsTileOccupiedByFriendly(i,j,team)) unitsRalliedByGeneral[team].Add(GetUnitIdFromTile(i,j));
                    }
                }
                foreach (var v in tempList)
                {
                    if (!unitsRalliedByGeneral[team].Contains(v)) ChangeMorale(v, -GeneralMoraleRaisingAmount);
                }

                foreach (var v in unitsRalliedByGeneral[team])
                {
                    if (!tempList.Contains(v)) ChangeMorale(v, GeneralMoraleRaisingAmount);
                }
    }


    public void ChangeMorale(short id, int value)
    {
        if (id == -1) return;
        var unit = GetUnitDataFromId(id);
        if (!unit.IsAlive) return;
        var temp = unit.Morale + value;
        ChangeWaveringAnimation(unit.id, temp < MoraleAnimationTrigger);
        if (unit.Morale + value < 1)
        {
            unit.Morale = 0;
            UnitRoutes(unit.id);
        }

        unit.Morale = (byte)temp;
    }



    private void UnitRoutes(short id)
    {
        var unit = GetUnitDataFromId(id);
        var team = GetUnitsTeam(id);
        var pos = unit.Position;
        tiles_units[pos.X, pos.Y] = -1;

        var escapePoint = new Point();
        /*short up = -1, down = -1, left = -1, right = -1;
        if (pos.Y - 1 >= 0) up = GetUnitIdFromTile(pos.X, pos.Y - 1);
        if (pos.Y + 1 < height) down = GetUnitIdFromTile(pos.X, pos.Y + 1);
        if (pos.X - 1 >= 0) left = GetUnitIdFromTile(pos.X - 1, pos.Y);
        if (pos.X + 1 < width) right = GetUnitIdFromTile(pos.X + 1, pos.Y);

        if (left != -1 && GetUnitsTeam(left) != team && right != -1 && GetUnitsTeam(left) != team && up != -1 &&
            GetUnitsTeam(left) != team && down != -1 && GetUnitsTeam(left) != team)
        {
            UnitDies(id);
            return;
        }*/

        if (team == 0) escapePoint = new Point(0, pos.Y);
        else if (team == 1) escapePoint = new Point(width-1, pos.Y);
        var pathfinder = new AStarSearch(new SquareGrid(width, height), pos, escapePoint, (_,_) => true);
        var path = pathfinder.GetPath();
        path.Add(new Point(team == 0 ? -1 : width, pos.Y));
        
        
        MoveUnitOnScreen(id, path, true);
        //for (var i = Math.Max(0,Math.Min(escapePoint.X, unit.Position.X)); i < Math.Min(width))
        int startX, endX;
        if (team == 0)
        {
            startX = 0;
            endX = Math.Min(width - 1, pos.X + 1);
        }
        else
        {
            startX = Math.Max(0, pos.X - 1);
            endX = width - 1;
        }

        for (var i = startX; i <= endX; ++i)
        {
            for (var j = Math.Max(0, pos.Y - MoraleLossDistance);
                 j <= Math.Min(width - 1, pos.Y + MoraleLossDistance);
                 ++j)
            {
                if (GetUnitsTeam(GetUnitIdFromTile(i, j)) == team)
                    ChangeMorale(GetUnitIdFromTile(i, j), (byte)-MoraleLossUponSeeingUnitRoute);
            }
        }

        unit.IsAlive = false;
    }


    private void UnitDies(short id)
    {
        var unit = GetUnitDataFromId(id);
        var team = GetUnitsTeam(unit.id);
        unit.Soldiers = 0;
        unit.IsAlive = false;
        tiles_units[unit.Position.X, unit.Position.Y] = -1;
        UnitDiesOnScreen(id);
        for (var i = Math.Max(0, unit.Position.X - 1); i <= Math.Min(width - 1, unit.Position.X + 1); ++i)
        {
            for (var j = Math.Max(0, unit.Position.Y - 1); j <= Math.Min(height - 1, unit.Position.Y + 1); ++j)
            {
                if (i == 0 && j == 0) continue;
                if (IsTileOccupiedByFriendly(i,j,team)) ChangeMorale(GetUnitIdFromTile(i, j), -MoraleLossUponSeeingUnitDeath);
            }
        }

        if (unit.Category == UnitCategory.General)
        {
            foreach (var v in playerUnits[team])
            {
                if (v.IsAlive) ChangeMorale(v.id,-GeneralDeathMoraleLoss);
            }

            foreach (var v in unitsRalliedByGeneral[team])
            {
                ChangeMorale(v, -GeneralMoraleRaisingAmount);
            }
            unitsRalliedByGeneral[team].Clear();
        }
    }

    public short FindFirstEnemyInNeighbourTiles(short id)
    {
        var pos = GetUnitDataFromId(id).Position;
        var team = GetUnitsTeam(id);
        if (IsTileOccupiedByEnemy(pos.X + 1, pos.Y, team)) return GetUnitIdFromTile(pos.X + 1, pos.Y);
        if (IsTileOccupiedByEnemy(pos.X - 1, pos.Y, team)) return GetUnitIdFromTile(pos.X - 1, pos.Y);
        if (IsTileOccupiedByEnemy(pos.X, pos.Y + 1, team)) return GetUnitIdFromTile(pos.X, pos.Y + 1);
        if (IsTileOccupiedByEnemy(pos.X, pos.Y - 1, team)) return GetUnitIdFromTile(pos.X, pos.Y - 1);

        return -1;
    }

    #endregion

    #region OnClick

    public void AttackClicked()
    {
        clickSound.Play();
        var orderType = GetUnitDataFromId(unitSelected).Order.type;
        CancelOrder(unitSelected);
        if (orderType == OrderType.Move)
        {
            OrderCancelled();
            return;
        }

        HighlightTilesInRange(unitSelected);
        currentSelectMode = OrderType.Move;
    }

    public void FortifyClicked()
    {
        clickSound.Play();
        var unit = GetUnitDataFromId(unitSelected);
        var orderType = unit.Order.type;
        CancelOrder(unitSelected);
        if (orderType == OrderType.Fortify && !unit.IsFortified)
        {
            OrderCancelled();
            return;
        }

        AddFortifyOrderForSelectedUnit();
    }

    public void FireClicked()
    {
        clickSound.Play();
        var orderType = GetUnitDataFromId(unitSelected).Order.type;
        CancelOrder(unitSelected);
        if (orderType == OrderType.Fire)
        {
            OrderCancelled();
            return;
        }

        HighlightEnemiesInRange(unitSelected);
        HighlightFreeFireTile(unitSelected);
        currentSelectMode = OrderType.Fire;
    }
    public void NewGameClicked()
    {
        confirmNewGamePanel.SetActive(true);
    }

    public void ConfirmClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void CancelClicked()
    {
        confirmNewGamePanel.SetActive(false);
    }

    #endregion
    
    
    private void Start()
    {
        UnitInfoPanel.SetActive(false);
        InGamePanel.SetActive(false);
    }

    public void CommenceGame(LinkedList<UnitData> team1Units, Nation nation1, LinkedList<UnitData> team2Units,
        Nation nation2, int _width, int _height)
    {
        InGamePanel.SetActive(true);
        confirmNewGamePanel.SetActive(false);
        width = _width;
        height = _height;
        MainCamera.GetComponent<CameraController>().maxX = width;
        MainCamera.GetComponent<CameraController>().lowerY = -height;

        TilesObjects = new GameObject[width, height];
        tiles_units = new short[width, height];
        HighlightedTiles = new List<Point>();
        unitsWhoHaveFoughtThisTurn = new SortedSet<(short, short)>();
        orders = new[] { new LinkedList<short>(), new LinkedList<short>() };
        gameUnits = new[] { new List<Unit>(), new List<Unit>() };
        playerUnits = new[] { new List<UnitData>(), new List<UnitData>() };
        unitsRalliedByGeneral = new[] { new HashSet<short>(), new HashSet<short>() };

        for (var i = 0; i < width; ++i) // loadanje tileova
        {
            for (var j = 0; j < height; ++j)
            {
                tiles_units[i, j] = -1;
                var tile = Instantiate(tilePrefab,
                    GetPositionFromTile(new Point(i,j)),
                    Quaternion.identity);
                tile.GetComponent<SpriteRenderer>().color = unselectedColor;
                tile.name = Convert.ToString(399 * i + j);
                TilesObjects[i, j] = tile;
            }
        }

        team1Units.AddFirst(new UnitData(0, UnitCategory.General, 0));
        team2Units.AddFirst(new UnitData(1, UnitCategory.General, 0));

        var ids = new short[] { 0, 1 };
        var teamUnits = new[] { team1Units, team2Units };
        var nations = new[] { nation1, nation2 };
        var points = new[] { new Point(0, 0), new Point(width - 1, 0) };
        var increments = new[] { 1, -1 };
        teamColors = new[] { nation1.Colors[0], nation2.Colors[nations[0].Name.Equals(nations[1].Name) ? 1 : 0] };
        
        for (var team = 0; team < teamUnits.Length; ++team)
        {
            foreach (var v in teamUnits[team])
            {
                var combatSkill = v.GetBaseCombatSkill();
                var morale = v.GetBaseMorale();
                if (v.CombatSkill != 0)
                {
                    combatSkill *= (byte)(1 + v.CombatSkill * CombatSkillMultipliedByRank);
                    morale *= (byte)(1 + v.CombatSkill * MoraleMultipliedByRank);
                }

                combatSkill *= (byte)(1 + v.GetNationBonus(nations[team]));
                morale *= (byte)(1 + v.GetNationBonus(nations[team]));
                playerUnits[team].Add(new UnitData(ids[team], v.Category, combatSkill, morale, v.GetInitialSize(),
                    points[team]));
                tiles_units[points[team].X, points[team].Y] = ids[team];
                var u = Instantiate(unitPrefab,
                    GetPositionFromTile(points[team]),
                    Quaternion.identity);
                gameUnits[team].Add(u.GetComponent<Unit>());
                switch (v.Category)
                {
                    case UnitCategory.General:
                        u.GetComponent<SpriteRenderer>().sprite = generalImage;
                        break;
                    case UnitCategory.LightInfantry:
                        u.GetComponent<SpriteRenderer>().sprite = lightInfantryImage;
                        break;
                    case UnitCategory.SpearInfantry:
                        u.GetComponent<SpriteRenderer>().sprite = spearInfantryImage;
                        break;
                    case UnitCategory.HeavyInfantry:
                        u.GetComponent<SpriteRenderer>().sprite = heavyInfantryImage;
                        break;
                    case UnitCategory.MissileInfantry:
                        u.GetComponent<SpriteRenderer>().sprite = missileInfantryImage;
                        break;
                    case UnitCategory.LightCavalry:
                        u.GetComponent<SpriteRenderer>().sprite = lightCavalryImage;
                        break;
                    case UnitCategory.HeavyCavalry:
                        u.GetComponent<SpriteRenderer>().sprite = heavyCavalryImage;
                        break;
                    case UnitCategory.MissileCavalry:
                        u.GetComponent<SpriteRenderer>().sprite = missileCavalryImage;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                u.GetComponent<SpriteRenderer>().color = teamColors[team];
                if (v.CombatSkill != 0)
                {
                    var rankObject = Instantiate(rankPrefab, u.transform);
                    rankObject.transform.SetParent(u.transform, false);
                    if (v.CombatSkill == 2) rankObject.GetComponent<SpriteRenderer>().sprite = rank3Image;
                }

                ids[team] += 2;
                ++points[team].Y;
                if (points[team].Y >= height)
                {
                    points[team].Y = 0;
                    points[team].X += increments[team];
                }
            }
        }

        MainCamera.transform.position = GetPositionFromTile(new Point(0, 0), -10);
        turn = 0;
        currentSelectMode = OrderType.No;
        DeselectUnit();
        HideEnemyUnits();
        HighlightBeginningTiles();
        gameOver = false;
        disableSelecting = false;
    }
}