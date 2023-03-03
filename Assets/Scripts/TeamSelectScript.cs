using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TeamSelectScript : MonoBehaviour
{

    public Grid gameWorldToBeCreated;

    public AudioSource clickSound;
    
    public TMP_InputField WidthInputField;
    public TextMeshProUGUI WidthPlaceholder;
    public TMP_InputField HeightInputField;
    public TextMeshProUGUI HeightPlaceholder;
    public TMP_InputField MaxGoldInputField;
    public TextMeshProUGUI MaxGoldPlaceholder;
    
    public GameObject TeamCreationPanel;
    
    public GameObject UnitCategoryChoosingPanel;
    public GameObject ArmyUnitsScrollview;
    public GameObject InputPanel;
    public GameObject SmoothGridLayoutElements;
    public GameObject UnitIconPrefab;
    [FormerlySerializedAs("Rank1Prefab")] public GameObject RankPrefab;
    public Button Team1SelectButton;
    public Button Team2SelectButton;

    //public Sprite GeneralSprite;
    public Sprite LightInfantrySprite;
    public Sprite HeavyInfantrySprite;
    public Sprite SpearInfantrySprite;
    public Sprite MissileInfantrySprite;
    public Sprite LightCavalrySprite;
    public Sprite HeavyCavalrySprite;
    public Sprite MissileCavalrySprite;
    public Sprite RankButtonSelectedSprite;
    public Sprite Rank1Sprite;
    public Sprite Rank2Sprite;

    public TextMeshProUGUI NationSelectedText;
    public TextMeshProUGUI GoldSpentText;
    
    public Button LightInfantryRank1Button;
    public Button LightInfantryRank2Button;
    public Button LightInfantryRank3Button;
    public TextMeshProUGUI LightInfantryNationBonus;
    public Button HeavyInfantryRank1Button;
    public Button HeavyInfantryRank2Button;
    public Button HeavyInfantryRank3Button;
    public TextMeshProUGUI HeavyInfantryNationBonus;
    public Button SpearInfantryRank1Button;
    public Button SpearInfantryRank2Button;
    public Button SpearInfantryRank3Button;
    public TextMeshProUGUI SpearInfantryNationBonus;
    public Button MissileInfantryRank1Button;
    public Button MissileInfantryRank2Button;
    public Button MissileInfantryRank3Button;
    public TextMeshProUGUI MissileInfantryNationBonus;
    public Button LightCavalryRank1Button;
    public Button LightCavalryRank2Button;
    public Button LightCavalryRank3Button;
    public TextMeshProUGUI LightCavalryNationBonus;
    public Button HeavyCavalryRank1Button;
    public Button HeavyCavalryRank2Button;
    public Button HeavyCavalryRank3Button;
    public TextMeshProUGUI HeavyCavalryNationBonus;
    public Button MissileCavalryRank1Button;
    public Button MissileCavalryRank2Button;
    public Button MissileCavalryRank3Button;
    public TextMeshProUGUI MissileCavalryNationBonus;
    public Button MissileCavalryBuyButton;

    public int choosingForTeam;
    public int[] nationSelected;
    public int[] ranksSelectedByPlayer;
    public LinkedList<UnitData>[,] UnitsBought;

    public short currentId = 0;
    //public int[] goldLeft;

    public static Nation[] Nations = new[]
    {
        new Nation("Byzantium", true, new []{0, 0.2f, 0, 0, 0, 0.3f, 0}, new []{new Color(0.415686f,0f,0.278431f), new Color(0.556863f ,0f ,0f)}),
        new Nation("Viking Army", false, new []{0.15f, 0.2f, 0.1f, 0.15f, 0, 0, 0}, new []{new Color(0.227451f ,0.337255f ,0.560784f), new Color(0.4f ,0.027451f ,0.027451f)}),
        new Nation("Caliphate", true, new []{0, 0, 0, 0, 0.25f, 0.15f, 0.1f}, new []{new Color(0.698039f ,0.627451f ,0.278431f), new Color(0.415686f ,0.25098f ,0.25098f)}),
        new Nation("England", false, new []{0, 0, 0, 0.4f, 0, 0.2f, 0}, new []{new Color(0.701961f ,0f ,0f), new Color(0.113725f ,0.113725f ,0.560784f)}),
        new Nation("France", false, new []{0, 0, 0.1f, 0.1f, 0, 0.4f, 0}, new []{new Color(0f ,0.282353f ,0.560784f), new Color(0.282353f ,0.560784f ,0.701961f)}),
        new Nation("Holy Roman Empire", false, new []{0, 0.1f, 0.1f, 0.1f, 0, 0.2f, 0}, new []{new Color(0.291176f ,0.291176f ,0.191176f), new Color(0.701961f ,0.588235f ,0f)}),
        new Nation("Hungary", true, new []{0.1f, 0, 0, 0.15f, 0.25f, 0, 0}, new []{new Color(0.701961f ,0.141176f ,0.141176f), new Color(0.196078f ,0.419608f ,0.0862745f)}),
        new Nation("Lombardy", false, new []{0, 0, 0.25f, 0.35f, 0, 0, 0}, new []{new Color(0f ,0.282353f ,0f), new Color(0.113725f ,0.337255f ,0.560784f)}),
        new Nation("Poland", true, new []{0, 0, 0, 0, 0.15f, 0.35f, 0}, new []{new Color(0.419608f ,0.168627f ,0.168627f), new Color(0.282353f ,0f ,0f)}),
        new Nation("Spain", false, new []{0.15f, 0, 0.05f, 0, 0.25f, 0.15f, 0}, new []{new Color(0.701961f ,0.588235f ,0f), new Color(0.701961f ,0.235294f ,0f)}),
        new Nation("Rus", true, new []{0.05f, 0.2f, 0.05f, 0.05f, 0.05f, 0.05f, 0.05f}, new []{new Color(0f ,0.188235f ,0.282353f), new Color(0.701961f ,0.560784f ,0.282353f)}),
        new Nation("Scotland", false, new []{0.1f, 0, 0.4f, 0.1f, 0, 0, 0}, new []{new Color(0f ,0f ,0.419608f), new Color(0.701961f ,0.588235f ,0f)}),
        new Nation("Ottomans", true, new []{0, 0, 0, 0.2f, 0.1f, 0, 0.2f}, new []{new Color(0.262745f ,0.560784f ,0.113725f), new Color(0.14902f ,0.168627f ,0.211765f)}),
        new Nation("Mongolia", true, new []{0, 0, 0, 0, 0.1f, 0, 0.4f}, new []{new Color(0.701961f ,0.701961f ,0.701961f), new Color(0.654902f ,0.701961f ,0.419608f)}),
        new Nation("Switzerland", false, new []{0, 0.1f, 0.5f, 0, 0, 0, 0}, new []{new Color(0.701961f ,0f ,0.113725f), new Color(1 ,1 ,1)})
    };
    public static int NumberOfNations = Nations.Length;
    public static int DefaultWidth = 30;
    public static int DefaultHeight = 30;
    public static int DefaultMaxGold = 10000;
    public static int MinimumWidth = 10;
    public static int MinimumHeight = 10;
    public static int MaxWidth = 300;
    public static int MaxHeight = 300;
    //public static int MaxUnits = MaxHeight * (int)(MaxWidth * Grid.PercentageOfTeamsStartingSpace);
    

    #region buttonOnClick

    public void WidthInputSelected()
    {
        WidthPlaceholder.color = new Color(0.196f, 0.196f, 0.196f, 0.50196f);
    }

    public void HeightInputSelected()
    {
        HeightPlaceholder.color = new Color(0.196f, 0.196f, 0.196f, 0.50196f);
    }

    public void MaxGoldInputSelected()
    {
        MaxGoldPlaceholder.color = new Color(0.196f, 0.196f, 0.196f, 0.50196f);
    }
    public void CheckmarkClicked()
    {
        clickSound.Play();
        DeselectTeam();
        var width = DefaultWidth;
        var height = DefaultHeight;
        var goldLimit = DefaultMaxGold;
        
        if (WidthInputField.text.Length != 0) width = Convert.ToInt32(WidthInputField.text);
        if (width > MaxWidth)
        {
            WidthInputField.text = "";
            WidthPlaceholder.color = new Color(1, 0, 0);
            WidthPlaceholder.text = "Cannot be >"+MaxWidth;
            return;
        }
        if (width < MinimumWidth)
        {
            WidthInputField.text = "";
            WidthPlaceholder.color = new Color(1, 0, 0);
            WidthPlaceholder.text = "Cannot be <"+MinimumWidth;
            return;
        }
        
        if (HeightInputField.text.Length != 0) height = Convert.ToInt32(HeightInputField.text);
        if (height > MaxHeight)
        {
            HeightInputField.text = "";
            HeightPlaceholder.color = new Color(1, 0, 0);
            HeightPlaceholder.text = "Cannot be >"+MaxHeight;
            return;
        }
        if (height < MinimumHeight)
        {
            HeightInputField.text = "";
            HeightPlaceholder.color = new Color(1, 0, 0);
            HeightPlaceholder.text = "Cannot be <"+MinimumHeight;
            return;
        }
        
        if (MaxGoldInputField.text.Length != 0) goldLimit = Convert.ToInt32(MaxGoldInputField.text);
        if (CalculateTeamCost(0) > goldLimit)
        {
            MaxGoldInputField.text = "";
            MaxGoldPlaceholder.color = new Color(1, 0, 0);
            MaxGoldPlaceholder.text = "Team 1 too expensive";
            return;
        }

        if (CalculateTeamCost(1) > goldLimit)
        {
            MaxGoldInputField.text = "";
            MaxGoldPlaceholder.color = new Color(1, 0, 0);
            MaxGoldPlaceholder.text = "Team 2 too expensive";
            return;
        }

        int maxUnits = (int)(width * Grid.PercentageOfTeamsStartingSpace) * height - 1;
        var team1 = UnitsBought[0, nationSelected[0]];
        var team2 = UnitsBought[1, nationSelected[1]];
        if (team1.Count > maxUnits || team2.Count > maxUnits)
        {
            MaxGoldInputField.text = "";
            MaxGoldPlaceholder.color = new Color(1, 0, 0);
            MaxGoldPlaceholder.text = "Too many units";
            return;
        }
        TeamCreationPanel.SetActive(false);
        gameWorldToBeCreated.CommenceGame(team1, Nations[nationSelected[0]], team2, Nations[nationSelected[1]], width, height);
        Destroy(this.gameObject);
    }
    
    public void AddLightInfantryClicked()
    {
        clickSound.Play();
        AddUnit(UnitCategory.LightInfantry);
    }
    public void AddHeavyInfantryClicked()
    {
        clickSound.Play();
        AddUnit(UnitCategory.HeavyInfantry);
    }
    public void AddSpearInfantryClicked()
    {
        clickSound.Play();
        AddUnit(UnitCategory.SpearInfantry);
    }
    public void AddMissileInfantryClicked()
    {
        clickSound.Play();
        AddUnit(UnitCategory.MissileInfantry);
    }
    public void AddLightCavalryClicked()
    {
        clickSound.Play();
        AddUnit(UnitCategory.LightCavalry);
    }
    public void AddHeavyCavalryClicked()
    {
        clickSound.Play();
        AddUnit(UnitCategory.HeavyCavalry);
    }
    public void AddMissileCavalryClicked()
    {
        clickSound.Play();
        AddUnit(UnitCategory.MissileCavalry);
    }
    
    public void RemoveUnit(int id)
    {
        var node = UnitsBought[choosingForTeam, nationSelected[choosingForTeam]].First;
        while (node != null)
        {
            var next = node.Next;
            if (node.Value.id == id)
            {
                UnitsBought[choosingForTeam, nationSelected[choosingForTeam]].Remove(node);
                return;
            }
            node = next;
        }

        throw new ArgumentOutOfRangeException();
    }
    public void ClickedTeam1()
    {
        clickSound.Play();
        if (choosingForTeam == 0)
        {
            DeselectTeam();
            return;
        }
        SelectTeam(0);
    }

    public void ClickedTeam2()
    {
        clickSound.Play();
        if (choosingForTeam == 1)
        {
            DeselectTeam();
            return;
        }
        SelectTeam(1);
    }

    public void ClickedTeam1LeftArrow()
    {
        clickSound.Play();
        ChangeSelectedTeam(0, false);
    }
    
    public void ClickedTeam1RightArrow()
    {
        clickSound.Play();
        ChangeSelectedTeam(0, true);
    }
    
    public void ClickedTeam2LeftArrow()
    {
        clickSound.Play();
        ChangeSelectedTeam(1, false);
    }
    
    public void ClickedTeam2RightArrow()
    {
        clickSound.Play();
        ChangeSelectedTeam(1, true);
    }
public void LightInfantryRank1Clicked()
    {
        clickSound.Play();
        ranksSelectedByPlayer[0] = 0;
        LightInfantryRank1Button.image.sprite = RankButtonSelectedSprite;
        LightInfantryRank2Button.image.sprite = null;
        LightInfantryRank3Button.image.sprite = null;
    }
    public void HeavyInfantryRank1Clicked()
    {
        clickSound.Play();
        ranksSelectedByPlayer[1] = 0;
        HeavyInfantryRank1Button.image.sprite = RankButtonSelectedSprite;
        HeavyInfantryRank2Button.image.sprite = null;
        HeavyInfantryRank3Button.image.sprite = null;
    }
    public void SpearInfantryRank1Clicked()
    {
        clickSound.Play();
        ranksSelectedByPlayer[2] = 0;
        SpearInfantryRank1Button.image.sprite = RankButtonSelectedSprite;
        SpearInfantryRank2Button.image.sprite = null;
        SpearInfantryRank3Button.image.sprite = null;
    }
    public void MissileInfantryRank1Clicked()
    {
        clickSound.Play();
        ranksSelectedByPlayer[3] = 0;
        MissileInfantryRank1Button.image.sprite = RankButtonSelectedSprite;
        MissileInfantryRank2Button.image.sprite = null;
        MissileInfantryRank3Button.image.sprite = null;
    }
    public void LightCavalryRank1Clicked()
    {
        clickSound.Play();
        ranksSelectedByPlayer[4] = 0;
        LightCavalryRank1Button.image.sprite = RankButtonSelectedSprite;
        LightCavalryRank2Button.image.sprite = null;
        LightCavalryRank3Button.image.sprite = null;
    }
    public void HeavyCavalryRank1Clicked()
    {
        clickSound.Play();
        ranksSelectedByPlayer[5] = 0;
        HeavyCavalryRank1Button.image.sprite = RankButtonSelectedSprite;
        HeavyCavalryRank2Button.image.sprite = null;
        HeavyCavalryRank3Button.image.sprite = null;
    }
    public void MissileCavalryRank1Clicked()
    {
        clickSound.Play();
        ranksSelectedByPlayer[6] = 0;
        MissileCavalryRank1Button.image.sprite = RankButtonSelectedSprite;
        MissileCavalryRank2Button.image.sprite = null;
        MissileCavalryRank3Button.image.sprite = null;
    }
    
    public void LightInfantryRank2Clicked()
    {
        clickSound.Play();
        ranksSelectedByPlayer[0] = 1;
        LightInfantryRank1Button.image.sprite = null;
        LightInfantryRank2Button.image.sprite = RankButtonSelectedSprite;
        LightInfantryRank3Button.image.sprite = null;
    }
    public void HeavyInfantryRank2Clicked()
    {
        clickSound.Play();
        ranksSelectedByPlayer[1] = 1;
        HeavyInfantryRank1Button.image.sprite = null;
        HeavyInfantryRank2Button.image.sprite = RankButtonSelectedSprite;
        HeavyInfantryRank3Button.image.sprite = null;
    }
    public void SpearInfantryRank2Clicked()
    {
        clickSound.Play();
        ranksSelectedByPlayer[2] = 1;
        SpearInfantryRank1Button.image.sprite = null;
        SpearInfantryRank2Button.image.sprite = RankButtonSelectedSprite;
        SpearInfantryRank3Button.image.sprite = null;
    }
    public void MissileInfantryRank2Clicked()
    {
        clickSound.Play();
        ranksSelectedByPlayer[3] = 1;
        MissileInfantryRank1Button.image.sprite = null;
        MissileInfantryRank2Button.image.sprite = RankButtonSelectedSprite;
        MissileInfantryRank3Button.image.sprite = null;
    }
    public void LightCavalryRank2Clicked()
    {
        clickSound.Play();
        ranksSelectedByPlayer[4] = 1;
        LightCavalryRank1Button.image.sprite = null;
        LightCavalryRank2Button.image.sprite = RankButtonSelectedSprite;
        LightCavalryRank3Button.image.sprite = null;
    }
    public void HeavyCavalryRank2Clicked()
    {
        clickSound.Play();
        ranksSelectedByPlayer[5] = 1;
        HeavyCavalryRank1Button.image.sprite = null;
        HeavyCavalryRank2Button.image.sprite = RankButtonSelectedSprite;
        HeavyCavalryRank3Button.image.sprite = null;
    }
    public void MissileCavalryRank2Clicked()
    {
        clickSound.Play();
        ranksSelectedByPlayer[6] = 1;
        MissileCavalryRank1Button.image.sprite = null;
        MissileCavalryRank2Button.image.sprite = RankButtonSelectedSprite;
        MissileCavalryRank3Button.image.sprite = null;
    }
    
    public void LightInfantryRank3Clicked()
    {
        clickSound.Play();
        ranksSelectedByPlayer[0] = 2;
        LightInfantryRank1Button.image.sprite = null;
        LightInfantryRank2Button.image.sprite = null;
        LightInfantryRank3Button.image.sprite = RankButtonSelectedSprite;
    }
    public void HeavyInfantryRank3Clicked()
    {
        clickSound.Play();
        ranksSelectedByPlayer[1] = 2;
        HeavyInfantryRank1Button.image.sprite = null;
        HeavyInfantryRank2Button.image.sprite = null;
        HeavyInfantryRank3Button.image.sprite = RankButtonSelectedSprite;
    }
    public void SpearInfantryRank3Clicked()
    {
        clickSound.Play();
        ranksSelectedByPlayer[2] = 2;
        SpearInfantryRank1Button.image.sprite = null;
        SpearInfantryRank2Button.image.sprite = null;
        SpearInfantryRank3Button.image.sprite = RankButtonSelectedSprite;
    }
    public void MissileInfantryRank3Clicked()
    {
        clickSound.Play();
        ranksSelectedByPlayer[3] = 2;
        MissileInfantryRank1Button.image.sprite = null;
        MissileInfantryRank2Button.image.sprite = null;
        MissileInfantryRank3Button.image.sprite = RankButtonSelectedSprite;
    }
    public void LightCavalryRank3Clicked()
    {
        clickSound.Play();
        ranksSelectedByPlayer[4] = 2;
        LightCavalryRank1Button.image.sprite = null;
        LightCavalryRank2Button.image.sprite = null;
        LightCavalryRank3Button.image.sprite = RankButtonSelectedSprite;
    }
    public void HeavyCavalryRank3Clicked()
    {
        clickSound.Play();
        ranksSelectedByPlayer[5] = 2;
        HeavyCavalryRank1Button.image.sprite = null;
        HeavyCavalryRank2Button.image.sprite = null;
        HeavyCavalryRank3Button.image.sprite = RankButtonSelectedSprite;
    }
    public void MissileCavalryRank3Clicked()
    {
        clickSound.Play();
        ranksSelectedByPlayer[6] = 2;
        MissileCavalryRank1Button.image.sprite = null;
        MissileCavalryRank2Button.image.sprite = null;
        MissileCavalryRank3Button.image.sprite = RankButtonSelectedSprite;
    }
    
    #endregion

    #region ui
    
    public void ChangeSelectedTeam(int player, bool right)
    {
        var newIndex = (right) ? nationSelected[player] + 1 : nationSelected[player] - 1;
        if (newIndex >= NumberOfNations) newIndex = 0;
        else if (newIndex < 0) newIndex = NumberOfNations - 1;
        nationSelected[player] = newIndex;
        var newNation = Nations[newIndex];
        if (player == 0)
        {
            Team1SelectButton.image.color = newNation.Colors[0];
        }
        else if (player == 1)
        {
            Team2SelectButton.image.color = (nationSelected[0] == nationSelected[1])
                ? newNation.Colors[1]
                : newNation.Colors[0];
        }
        

        if (choosingForTeam == player)
        {
            UpdateNationNameAndBonuses();
            ClearUnitsFromView();
            LoadUnitsIntoView();
            UpdateGoldSpent();
        }
    }
    public void DeselectTeam()
    {
        choosingForTeam = -1;
        ArmyUnitsScrollview.SetActive(false);
        UnitCategoryChoosingPanel.SetActive(false);
        InputPanel.SetActive(true);
    }

    public void SelectTeam(int team)
    {
        choosingForTeam = team;
        UpdateNationNameAndBonuses();
        ClearUnitsFromView();
        LoadUnitsIntoView();
        UpdateGoldSpent();
        ResetUnitRankButtons();
        UnitCategoryChoosingPanel.SetActive(true);
        ArmyUnitsScrollview.SetActive(true);
        InputPanel.SetActive(false);
    }

    private void UpdateGoldSpent()
    {
        var val = CalculateTeamCost(choosingForTeam);
        var maxGold = DefaultMaxGold;
        GoldSpentText.text = Convert.ToString(val);
        if (MaxGoldInputField.text.Length != 0) maxGold = Convert.ToInt32(MaxGoldInputField.text);
        if (val > maxGold) GoldSpentText.color = Color.red;
    }

    public void ResetUnitRankButtons()
    {
        Array.Clear(ranksSelectedByPlayer, 0, ranksSelectedByPlayer.Length);
        LightInfantryRank1Button.image.sprite = RankButtonSelectedSprite;
        HeavyInfantryRank1Button.image.sprite = RankButtonSelectedSprite;
        SpearInfantryRank1Button.image.sprite = RankButtonSelectedSprite;
        MissileInfantryRank1Button.image.sprite = RankButtonSelectedSprite;
        LightCavalryRank1Button.image.sprite = RankButtonSelectedSprite;
        HeavyCavalryRank1Button.image.sprite = RankButtonSelectedSprite;
        MissileCavalryRank1Button.image.sprite = RankButtonSelectedSprite;
        
        LightInfantryRank2Button.image.sprite = null;
        HeavyInfantryRank2Button.image.sprite = null;
        SpearInfantryRank2Button.image.sprite = null;
        MissileInfantryRank2Button.image.sprite = null;
        LightCavalryRank2Button.image.sprite = null;
        HeavyCavalryRank2Button.image.sprite = null;
        MissileCavalryRank2Button.image.sprite = null;
        
        LightInfantryRank3Button.image.sprite = null;
        HeavyInfantryRank3Button.image.sprite = null;
        SpearInfantryRank3Button.image.sprite = null;
        MissileInfantryRank3Button.image.sprite = null;
        LightCavalryRank3Button.image.sprite = null;
        HeavyCavalryRank3Button.image.sprite = null;
        MissileCavalryRank3Button.image.sprite = null;
        
    }

    public void ClearUnitsFromView()
    {
        foreach (Transform child in SmoothGridLayoutElements.transform)
        {
            Destroy(child.gameObject);
        }
    }
    public void LoadUnitsIntoView()
    {
        var nation = Nations[nationSelected[choosingForTeam]];
        var teamsColor = (nationSelected[0] == nationSelected[1] && choosingForTeam == 1) ? nation.Colors[1] : nation.Colors[0];

        foreach (var v in UnitsBought[choosingForTeam, nationSelected[choosingForTeam]])
        {
            var unit = Instantiate(UnitIconPrefab, SmoothGridLayoutElements.transform);
            switch (v.Category)
            {
                case UnitCategory.LightInfantry:
                    unit.GetComponent<Button>().image.sprite = LightInfantrySprite;
                    break;
                case UnitCategory.SpearInfantry:
                    unit.GetComponent<Button>().image.sprite = SpearInfantrySprite;
                    break;
                case UnitCategory.HeavyInfantry:
                    unit.GetComponent<Button>().image.sprite = HeavyInfantrySprite;
                    break;
                case UnitCategory.MissileInfantry:
                    unit.GetComponent<Button>().image.sprite = MissileInfantrySprite;
                    break;
                case UnitCategory.LightCavalry:
                    unit.GetComponent<Button>().image.sprite = LightCavalrySprite;
                    break;
                case UnitCategory.HeavyCavalry:
                    unit.GetComponent<Button>().image.sprite = HeavyCavalrySprite;
                    break;
                case UnitCategory.MissileCavalry:
                    unit.GetComponent<Button>().image.sprite = MissileCavalrySprite;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            unit.GetComponent<Button>().image.color = teamsColor;
            if (v.CombatSkill == 1)
            {
                var rankIcon = Instantiate(RankPrefab, unit.transform);
                rankIcon.GetComponent<RawImage>().texture = Rank1Sprite.texture;
            }
            else if (v.CombatSkill == 2)
            {
                var rankIcon = Instantiate(RankPrefab, unit.transform);
                rankIcon.GetComponent<RawImage>().texture = Rank2Sprite.texture;
            }
        }
    }

    public void UpdateNationNameAndBonuses()
    {
        var nation = Nations[nationSelected[choosingForTeam]];
        NationSelectedText.text = nation.Name;
        LightInfantryNationBonus.text = (nation.UnitBuffs[0] != 0) ? "+" + nation.UnitBuffs[0] * 100 + "%" : "";
        HeavyInfantryNationBonus.text = (nation.UnitBuffs[1] != 0) ? "+" + nation.UnitBuffs[1] * 100 + "%" : "";
        SpearInfantryNationBonus.text = (nation.UnitBuffs[2] != 0) ? "+" + nation.UnitBuffs[2] * 100 + "%" : "";
        MissileInfantryNationBonus.text = (nation.UnitBuffs[3] != 0) ? "+" + nation.UnitBuffs[3] * 100 + "%" : "";
        LightCavalryNationBonus.text = (nation.UnitBuffs[4] != 0) ? "+" + nation.UnitBuffs[4] * 100 + "%" : "";
        HeavyCavalryNationBonus.text = (nation.UnitBuffs[5] != 0) ? "+" + nation.UnitBuffs[5] * 100 + "%" : "";
        MissileCavalryNationBonus.text = (nation.UnitBuffs[6] != 0) ? "+" + nation.UnitBuffs[6] * 100 + "%" : "";
        MissileCavalryBuyButton.gameObject.SetActive(nation.HorseArchersEnabled);
    }
public void AddUnit(UnitCategory category)
    {
        var rank = 0;
        var button = Instantiate(UnitIconPrefab, SmoothGridLayoutElements.transform);
        switch (category)
        {
            case UnitCategory.LightInfantry:
                button.GetComponent<Button>().image.sprite = LightInfantrySprite;
                rank = ranksSelectedByPlayer[0];
                break;
            case UnitCategory.HeavyInfantry:
                button.GetComponent<Button>().image.sprite = HeavyInfantrySprite;
                rank = ranksSelectedByPlayer[1];
                break;
            case UnitCategory.SpearInfantry:
                button.GetComponent<Button>().image.sprite = SpearInfantrySprite;
                rank = ranksSelectedByPlayer[2];
                break;
            case UnitCategory.MissileInfantry:
                button.GetComponent<Button>().image.sprite = MissileInfantrySprite;
                rank = ranksSelectedByPlayer[3];
                break;
            case UnitCategory.LightCavalry:
                button.GetComponent<Button>().image.sprite = LightCavalrySprite;
                rank = ranksSelectedByPlayer[4];
                break;
            case UnitCategory.HeavyCavalry:
                button.GetComponent<Button>().image.sprite = HeavyCavalrySprite;
                rank = ranksSelectedByPlayer[5];
                break;
            case UnitCategory.MissileCavalry:
                button.GetComponent<Button>().image.sprite = MissileCavalrySprite;
                rank = ranksSelectedByPlayer[6];
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(category), category, null);
        }
        var unit = new UnitData(currentId, category, (byte)rank);
        UnitsBought[choosingForTeam, nationSelected[choosingForTeam]].AddLast(unit);
        var nation = Nations[nationSelected[choosingForTeam]];
        var teamsColor = (nationSelected[0] == nationSelected[1] && choosingForTeam == 1) ? nation.Colors[1] : nation.Colors[0];
        button.GetComponent<Button>().image.color = teamsColor;
        button.GetComponent<TeamSelectAddableUnit>().id = currentId++;
        if (rank == 1)
        {
            var rankObject = Instantiate(RankPrefab, button.transform);
            rankObject.GetComponent<RawImage>().texture = Rank1Sprite.texture;
        }
        else if (rank == 2)
        {
            var rankObject = Instantiate(RankPrefab, button.transform);
            rankObject.GetComponent<RawImage>().texture = Rank2Sprite.texture;
        }
        UpdateGoldSpent();
    }
    

    #endregion
    private void Start()
    {
        ranksSelectedByPlayer = new int[7];
        UnitsBought = new LinkedList<UnitData>[2,NumberOfNations];
        for (var i = 0; i < 2; ++i)
        {
            for (var j = 0; j < NumberOfNations; ++j)
            {
                UnitsBought[i, j] = new LinkedList<UnitData>();
                //UnitsBought[i,j].AddLast(new UnitData(currentId++, UnitCategory.General, 0)); // ne, ipak ne, nek se nadoda na kraju team selectiona
            }
        }
        
        nationSelected = new[] { 0, 1 };
        Team1SelectButton.image.color = Nations[0].Colors[0];
        Team2SelectButton.image.color = Nations[1].Colors[0];
        choosingForTeam = -1;

        WidthPlaceholder.text = Convert.ToString(DefaultWidth);
        HeightPlaceholder.text = Convert.ToString(DefaultHeight);
        MaxGoldPlaceholder.text = Convert.ToString(DefaultMaxGold);
        UnitCategoryChoosingPanel.SetActive(false);
        var height = Screen.height-256;
        var rect = ArmyUnitsScrollview.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(0, height);
        rect.offsetMin = new Vector2(200, rect.offsetMin.y);
        rect.offsetMax = new Vector2(0, rect.offsetMax.y);
        
        ArmyUnitsScrollview.SetActive(false);
    }




    public int CalculateTeamCost(int player)
    {
        int rv = 0;
        foreach (var v in UnitsBought[player, nationSelected[player]])
        {
            rv += v.CalculateUnitCost();
        }

        return rv;
    }



}
