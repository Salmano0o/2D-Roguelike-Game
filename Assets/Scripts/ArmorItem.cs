using UnityEngine;

public class ArmorItem : CellObject
{
    public int DefenseBonus = 1;

    public override bool PlayerWantsToEnter()
    {
        PlayerStats stats = GameManager.Instance.PlayerController.GetComponent<PlayerStats>();
        if (stats != null)
        {
            stats.Defense += DefenseBonus;
            Debug.Log("Armor acquired! Total Defense: " + stats.Defense);
        }

        Destroy(gameObject);
        return true;
    }
}