using UnityEngine;

public class CuttingCounter : BaseCounter
{
    public override void Interact(Player player)
    {
        //Placing Item
        if (!HasKitchenObject())
        {
            if (player.HasKitchenObject())
            {
                player.GetKitchenObject().SetKitchenObjectParent(this);
            }
        }
        else
        {
            //Grabbing item
            if (!player.HasKitchenObject())
            {
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }
}
