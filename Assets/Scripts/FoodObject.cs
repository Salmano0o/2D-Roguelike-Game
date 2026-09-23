using UnityEngine;

public class FoodObject : CellObject
{
    public int AmountGranted = 10;

    public override void PlayerEntered()
    {
        PlayerStats stats = GameManager.Instance.PlayerController.GetComponent<PlayerStats>();
        int finalAmount = AmountGranted;

        if (stats != null)
        {
            finalAmount = Mathf.RoundToInt(AmountGranted * stats.FoodMultiplier);
        }

        Destroy(gameObject);

        // Increase food amount in GameManager
        GameManager.Instance.ChangeFood(finalAmount);
        Debug.Log("Food increased by: " + finalAmount);
    }
}