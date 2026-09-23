using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Base Stats")]
    public int Strength = 1;
    public int Defense = 0;
    public int Speed = 10; // 10 Speed = 10 Energy restored per turn

    [Header("Energy System")]
    public int CurrentEnergy = 0;
    public int MoveEnergyCost = 10;

    [Header("Modifiers")]
    public float FoodMultiplier = 1.0f;

    public void AddTurnEnergy()
    {
        CurrentEnergy += Speed;
    }

    public bool CanAfford(int energyCost)
    {
        return CurrentEnergy >= energyCost;
    }

    public void SpendEnergy(int energyCost)
    {
        CurrentEnergy = Mathf.Max(0, CurrentEnergy - energyCost);
    }

    public int CalculateDamage(int incomingDamage)
    {
        return Mathf.Max(1, incomingDamage - Defense);
    }
}