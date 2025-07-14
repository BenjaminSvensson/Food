using UnityEngine;

public interface IKitchenObjectParent 
{
    public Transform GetKitchenFollowTransform();

    public void SetKitchenObject(KitchenObject kitchenObject);

    public KitchenObject GetKitchenObject();

    public void ClearKitchenObject();

    public bool HasKitchenObject();


}
