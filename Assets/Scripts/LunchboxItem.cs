using UnityEngine;

public class LunchboxItem : CellObject
{
    public float MultiplierBonus = 2.0f;

    public override bool PlayerWantsToEnter()
    {
        PlayerStats stats = GameManager.Instance.PlayerController.GetComponent<PlayerStats>();
        if (stats != null)
        {
            stats.FoodMultiplier = MultiplierBonus;
            Debug.Log("Lunchbox acquired! Food multiplier set to: " + stats.FoodMultiplier);
        }

        Destroy(gameObject);
        return true;
    }
}