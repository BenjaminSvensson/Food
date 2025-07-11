using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Player Player;
    private const string IsWalking = "IsWalking";
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        animator.SetBool(IsWalking, Player.IsWalking());
    }
}
