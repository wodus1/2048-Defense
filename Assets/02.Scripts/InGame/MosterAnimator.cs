using UnityEditor.Animations;
using UnityEngine;

public class MonsterAnimator : MonoBehaviour //몬스터 애니메이션
{
    [SerializeField] private Animator animator;

    private readonly int hashIsMove = Animator.StringToHash("IsMove");
    private readonly int hashDie = Animator.StringToHash("IsDie");

    public void PlayMove()
    {
        animator.SetBool(hashIsMove, true);
    }

    public void PlayDie()
    {
        animator.SetBool(hashIsMove, false);
        animator.SetTrigger(hashDie);
    }
}
