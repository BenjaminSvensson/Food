using System;
using UnityEngine;
public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }
    [SerializeField] private float MovementSpeed = 8f;
    [SerializeField] private float RotationSpeed = 20f;
    [SerializeField] private float PlayerCollisionSize = 0.3f;
    [SerializeField] private float InteractionDistance = 2f;
    [SerializeField] private float PlayerCollisionHeight = 8f; 
    [SerializeField] private LayerMask interactibleLayer;
    [SerializeField] private GameInput gameInput;

    private bool isWalking = false;
    private Vector3 lastInteractDirection;

    private ClearCounter selectedCounter;

    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public ClearCounter selectedCounter;
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
            selectedCounter.Interact();
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
        if (Physics.Raycast(transform.position, lastInteractDirection, out RaycastHit raycastHit, InteractionDistance, interactibleLayer))
        {
            if (raycastHit.transform.TryGetComponent(out ClearCounter clearCounter))
            {
                if (clearCounter != selectedCounter)
                {
                    SetSelectedCounter(clearCounter);
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
        float MoveDistance = MovementSpeed * Time.deltaTime;

        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        Vector3 movementDirection = new Vector3(inputVector.x, 0f, inputVector.y);
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * PlayerCollisionHeight, PlayerCollisionSize, movementDirection, MoveDistance);

        if (!canMove)
        {
            Vector3 MovementDirectionX = new Vector3(movementDirection.x, 0, 0).normalized;
            canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * PlayerCollisionHeight, PlayerCollisionSize, MovementDirectionX, MoveDistance);

            if (canMove)
            {
                movementDirection = MovementDirectionX;
            }
            else
            {
                Vector3 MovementDirectionZ = new Vector3(0, 0, movementDirection.z).normalized;
                canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * PlayerCollisionHeight, PlayerCollisionSize, MovementDirectionZ, MoveDistance);

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

        transform.forward = Vector3.Slerp(transform.forward, movementDirection, Time.deltaTime * RotationSpeed);

        isWalking = movementDirection != Vector3.zero;
    }

    private void SetSelectedCounter(ClearCounter selectedCounter)
    {
        this.selectedCounter = selectedCounter;

        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs
         {
             selectedCounter = selectedCounter
        });
    }
}