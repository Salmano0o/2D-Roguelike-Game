using UnityEngine;

public class FoodObject : CellObject
{
    public int AmountGranted = 10;

    public override void PlayerEntered()
    {
        Destroy(gameObject);

        // Increase food amount in GameManager
        GameManager.Instance.ChangeFood(AmountGranted);
        Debug.Log("Food increased");
    }
}