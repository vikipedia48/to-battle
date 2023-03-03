using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;


public enum UnitCategory
{
    No,
    General,
    LightInfantry,
    SpearInfantry,
    HeavyInfantry,
    MissileInfantry,
    LightCavalry,
    HeavyCavalry,
    MissileCavalry
}
public class UnitData
{
    public short id;
    public Order Order;
    public UnitCategory Category;
    public byte CombatSkill;
    public byte Morale;
    public short Soldiers;
    public bool IsFortified;
    public bool IsAlive;
    public Point Position;

    public static float MissileDamageFortifiedModifier = 0.75f;
    public static float MissileDamageRankModifier = 0.5f;
    public static byte BaseCombatSkill_General = 200;
    public static byte BaseCombatSkill_LightInfantry = 80;
    public static byte BaseCombatSkill_SpearInfantry = 100;
    public static byte BaseCombatSkill_HeavyInfantry = 140;
    public static byte BaseCombatSkill_MissileInfantry = 80;
    public static byte BaseCombatSkill_LightCavalry = 130;
    public static byte BaseCombatSkill_HeavyCavalry = 180;
    public static byte BaseCombatSkill_MissileCavalry = 110;
    public static byte BaseMorale_General = 200;
    public static byte BaseMorale_LightInfantry = 100;
    public static byte BaseMorale_SpearInfantry = 110;
    public static byte BaseMorale_HeavyInfantry = 120;
    public static byte BaseMorale_MissileInfantry = 100;
    public static byte BaseMorale_LightCavalry = 110;
    public static byte BaseMorale_HeavyCavalry = 130;
    public static byte BaseMorale_MissileCavalry = 110;
    
    public static short InitialSoldiers_General = 300;
    public static short InitialSoldiers_LightCavalry = 300;
    public static short InitialSoldiers_HeavyCavalry = 300;
    public static short InitialSoldiers_MissileCavalry = 300;
    public static short InitialSoldiers_LightInfantry = 400;
    public static short InitialSoldiers_HeavyInfantry = 400;
    public static short InitialSoldiers_SpearInfantry = 400;
    public static short InitialSoldiers_MissileInfantry = 400;
    
    public static int LightInfantryBaseCost = 200;
    public static int HeavyInfantryBaseCost = 500;
    public static int SpearInfantryBaseCost = 400;
    public static int MissileInfantryBaseCost = 400;
    public static int LightCavalryBaseCost = 400;
    public static int HeavyCavalryBaseCost = 700;
    public static int MissileCavalryBaseCost = 500;
    public static float Rank2CostMultiplier = 1.25f;
    public static float Rank3CostMultiplier = 1.5f;
    


public UnitData(short id, UnitCategory category, byte combatSkill, byte morale, short soldiers, Point position)
{
    this.id = id;
    Order = new Order(-1, OrderType.No);
    Category = category;
    CombatSkill = combatSkill;
    Morale = morale;
    Soldiers = soldiers;
    IsFortified = false;
    Position = position;
    IsAlive = true;
}

public UnitData(short id, UnitCategory category, byte rank)
{
    this.id = id;
    this.Category = category;
    this.CombatSkill = rank;
}

public bool IsCavalry()
{
    return Category == UnitCategory.General || Category == UnitCategory.LightCavalry ||
           Category == UnitCategory.HeavyCavalry || Category == UnitCategory.MissileCavalry;
}
public float GetNationBonus(Nation nation)
{
    return Category switch
    {
        UnitCategory.General => 0,
        UnitCategory.LightInfantry => nation.UnitBuffs[0],
        UnitCategory.HeavyInfantry => nation.UnitBuffs[1],
        UnitCategory.SpearInfantry => nation.UnitBuffs[2],
        UnitCategory.MissileInfantry => nation.UnitBuffs[3],
        UnitCategory.LightCavalry => nation.UnitBuffs[4],
        UnitCategory.HeavyCavalry => nation.UnitBuffs[5],
        UnitCategory.MissileCavalry => nation.UnitBuffs[6],
        _ => throw new ArgumentOutOfRangeException()
    };
}

public short GetInitialSize()
{
    return Category switch
    {
        UnitCategory.General => InitialSoldiers_General,
        UnitCategory.LightInfantry => InitialSoldiers_LightInfantry,
        UnitCategory.SpearInfantry => InitialSoldiers_SpearInfantry,
        UnitCategory.HeavyInfantry => InitialSoldiers_HeavyInfantry,
        UnitCategory.MissileInfantry => InitialSoldiers_MissileInfantry,
        UnitCategory.LightCavalry => InitialSoldiers_LightCavalry,
        UnitCategory.HeavyCavalry => InitialSoldiers_HeavyCavalry,
        UnitCategory.MissileCavalry => InitialSoldiers_MissileCavalry,
        _ => throw new ArgumentOutOfRangeException()
    };
}
public byte GetBaseCombatSkill()
{
    return Category switch
    {
        UnitCategory.General => BaseCombatSkill_General,
        UnitCategory.LightInfantry => BaseCombatSkill_LightInfantry,
        UnitCategory.SpearInfantry => BaseCombatSkill_SpearInfantry,
        UnitCategory.HeavyInfantry => BaseCombatSkill_HeavyInfantry,
        UnitCategory.MissileInfantry => BaseCombatSkill_MissileInfantry,
        UnitCategory.LightCavalry => BaseCombatSkill_LightCavalry,
        UnitCategory.HeavyCavalry => BaseCombatSkill_HeavyCavalry,
        UnitCategory.MissileCavalry => BaseCombatSkill_MissileCavalry,
        _ => throw new ArgumentOutOfRangeException()
    };
}
public byte GetBaseMorale()
{
    return Category switch
    {
        UnitCategory.General => BaseMorale_General,
        UnitCategory.LightInfantry => BaseMorale_LightInfantry,
        UnitCategory.SpearInfantry => BaseMorale_SpearInfantry,
        UnitCategory.HeavyInfantry => BaseMorale_HeavyInfantry,
        UnitCategory.MissileInfantry => BaseMorale_MissileInfantry,
        UnitCategory.LightCavalry => BaseMorale_LightCavalry,
        UnitCategory.HeavyCavalry => BaseMorale_HeavyCavalry,
        UnitCategory.MissileCavalry => BaseMorale_MissileCavalry,
        _ => throw new ArgumentOutOfRangeException()
    };
}

public int CalculateUnitCost()
{
    int rv = 0;
    switch (Category)
    {
        case UnitCategory.LightInfantry:
            rv = LightInfantryBaseCost;
            break;
        case UnitCategory.SpearInfantry:
            rv = SpearInfantryBaseCost;
            break;
        case UnitCategory.HeavyInfantry:
            rv = HeavyInfantryBaseCost;
            break;
        case UnitCategory.MissileInfantry:
            rv = MissileInfantryBaseCost;
            break;
        case UnitCategory.LightCavalry:
            rv = LightCavalryBaseCost;
            break;
        case UnitCategory.HeavyCavalry:
            rv = HeavyCavalryBaseCost;
            break;
        case UnitCategory.MissileCavalry:
            rv = MissileCavalryBaseCost;
            break;
        default:
            throw new ArgumentOutOfRangeException();
    }

    switch (CombatSkill) // u selectionu je combat skill između 0 i 2
    {
        case 0: 
            break;
        case 1:
            rv = (int) (rv*Rank2CostMultiplier);
            break;
        case 2:
            rv = (int)(rv * Rank3CostMultiplier);
            break;
        default:
            throw new ArgumentOutOfRangeException();
    }

    return rv;
}

public float GetDamageFromMissileFire()
{
    float fortified = (IsFortified) ? MissileDamageFortifiedModifier : 1;
    return Category switch
    {
        UnitCategory.General => 0.8f,
        UnitCategory.HeavyCavalry => 1.1f * CombatSkill/BaseCombatSkill_HeavyCavalry*MissileDamageRankModifier,
        UnitCategory.LightInfantry => fortified * 1.35f * CombatSkill/BaseCombatSkill_LightInfantry*MissileDamageRankModifier,
        UnitCategory.MissileInfantry => fortified * 1.25f * CombatSkill/BaseCombatSkill_MissileInfantry*MissileDamageRankModifier,
        UnitCategory.SpearInfantry => fortified * 1.2f * CombatSkill/BaseCombatSkill_SpearInfantry*MissileDamageRankModifier,
        UnitCategory.HeavyInfantry => fortified * 1 * CombatSkill/BaseCombatSkill_HeavyInfantry*MissileDamageRankModifier,
        UnitCategory.LightCavalry => 1.3f * CombatSkill/BaseCombatSkill_LightCavalry*MissileDamageRankModifier,
        UnitCategory.MissileCavalry => 1.3f * CombatSkill/BaseCombatSkill_MissileCavalry*MissileDamageRankModifier,
        _ => 0
    };
}

public byte GetMovementDistance()
{
    return Category switch
    {
        UnitCategory.HeavyInfantry => 4,
        UnitCategory.SpearInfantry => 4,
        UnitCategory.MissileInfantry => 5,
        UnitCategory.LightInfantry => 5,
        UnitCategory.General or UnitCategory.HeavyCavalry => 6,
        UnitCategory.MissileCavalry => 8,
        UnitCategory.LightCavalry => 9,
        _ => 0
    };
}

public float GetChargeBonus()
{
    return Category switch
    {
        UnitCategory.General or UnitCategory.HeavyCavalry => 2,
        UnitCategory.LightCavalry => 1.75f,
        UnitCategory.MissileCavalry => 1.5f,
        UnitCategory.LightInfantry or UnitCategory.MissileInfantry => 1.3f,
        UnitCategory.SpearInfantry or UnitCategory.HeavyInfantry => 1f,
        _ => throw new ArgumentOutOfRangeException()
    };
}

public byte GetFireDistance()
{
    return Category switch
    {
        UnitCategory.MissileInfantry => 12,
        UnitCategory.MissileCavalry => 12,
        _ => 0
    };
}

public string CategoryToString()
{
    return Category switch
    {
        UnitCategory.General => "General",
        UnitCategory.LightInfantry => "Skirmishers",
        UnitCategory.SpearInfantry => "Spearmen",
        UnitCategory.HeavyInfantry => "Men at arms",
        UnitCategory.MissileInfantry => "Archers",
        UnitCategory.LightCavalry => "Cavalry",
        UnitCategory.HeavyCavalry => "Knights",
        UnitCategory.MissileCavalry => "Horse Archers",
        _ => ""
    };
}


    
}


