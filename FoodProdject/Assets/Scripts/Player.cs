using System;
using UnityEngine;
using UnityEngine.EventSystems;
public class Player : MonoBehaviour, IKitchenObjectParent
{
    public static Player Instance { get; private set; }
    [SerializeField] private float movementSpeed = 8f;
    [SerializeField] private float rotationSpeed = 20f;
    [SerializeField] private float playerCollisionSize = 0.3f;
    [SerializeField] private float interactionDistance = 2f;
    [SerializeField] private float playerCollisionHeight = 8f;
    [SerializeField] private LayerMask interactibleLayer;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private Transform kitchenObjectHoldPoint;

    private bool isWalking = false;
    private Vector3 lastInteractDirection;

    private BaseCounter selectedCounter;
    private KitchenObject kitchenObject;

    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public BaseCounter selectedCounter;
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There cant be more then one player instance in the scene at once!");
        }
        Instance = this;
    }

    private void Start()
    {
        gameInput.OnInteractAction += GameInput_OnActionInteract;
    }
    private void GameInput_OnActionInteract(object sender, System.EventArgs e)
    {
        if (selectedCounter != null)
        {
            selectedCounter.Interact(this);
        }
    }

    private void Update()
    {
        HandelMovement();
        HandelInteractions();
    }

    public bool IsWalking()
    {
        return isWalking;
    }

    private void HandelInteractions()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        Vector3 movementDirection = new Vector3(inputVector.x, 0f, inputVector.y);

        if (movementDirection != Vector3.zero)
        {
            lastInteractDirection = movementDirection;
        }
        if (Physics.Raycast(transform.position, lastInteractDirection, out RaycastHit raycastHit, interactionDistance, interactibleLayer))
        {
            if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter))
            {
                if (baseCounter != selectedCounter)
                {
                    SetSelectedCounter(baseCounter);
                }
            }
            else
            {
                SetSelectedCounter(null);
            }
        }
        else
        {
            SetSelectedCounter(null);
        }
    }

    private void HandelMovement()
    {
        float MoveDistance = movementSpeed * Time.deltaTime;

        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        Vector3 movementDirection = new Vector3(inputVector.x, 0f, inputVector.y);
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerCollisionHeight, playerCollisionSize, movementDirection, MoveDistance);

        if (!canMove)
        {
            Vector3 MovementDirectionX = new Vector3(movementDirection.x, 0, 0).normalized;
            canMove = movementDirection.x != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerCollisionHeight, playerCollisionSize, MovementDirectionX, MoveDistance);

            if (canMove)
            {
                movementDirection = MovementDirectionX;
            }
            else
            {
                Vector3 MovementDirectionZ = new Vector3(0, 0, movementDirection.z).normalized;
                canMove = movementDirection.z != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerCollisionHeight, playerCollisionSize, MovementDirectionZ, MoveDistance);

                if (canMove)
                {
                    movementDirection = MovementDirectionZ;
                }
            }
        }
        if (canMove)
        {
            transform.position += movementDirection * MoveDistance;
        }

        transform.forward = Vector3.Slerp(transform.forward, movementDirection, Time.deltaTime * rotationSpeed);

        isWalking = movementDirection != Vector3.zero;
    }

    private void SetSelectedCounter(BaseCounter selectedCounter)
    {
        this.selectedCounter = selectedCounter;

        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs
        {
            selectedCounter = selectedCounter
        });
    }
    public Transform GetKitchenFollowTransform()
    {
        return kitchenObjectHoldPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;
    }

    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }

    public void ClearKitchenObject()
    {
        kitchenObject = null;
    }

    public bool HasKitchenObject()
    {
        return kitchenObject != null;
    }
}