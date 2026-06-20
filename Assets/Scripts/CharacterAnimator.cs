using UnityEngine;

// Lớp đệm cho ART sau này. Mọi script gameplay gọi qua đây để báo trạng thái
// (đi/đập/chết/hướng mặt). Khi CHƯA có Animator (bản xám) thì mọi lệnh tự bỏ qua,
// nên gắn hay không gắn script này đều không ảnh hưởng logic.
//
// Khi có art skeletal: tạo Animator Controller với các tham số
//   Bool "Walking", Trigger "Attack", Trigger "Die"
// rồi gán Animator + SpriteRenderer vào đây trên prefab.
public class CharacterAnimator : MonoBehaviour
{
    public Animator animator;             // gán khi đã có art; trống -> no-op
    public SpriteRenderer spriteToFlip;   // để lật hướng mặt (tuỳ chọn)

    static readonly int WalkingHash = Animator.StringToHash("Walking");
    static readonly int AttackHash = Animator.StringToHash("Attack");
    static readonly int DieHash = Animator.StringToHash("Die");

    public void SetWalking(bool on)
    {
        if (animator != null) animator.SetBool(WalkingHash, on);
    }

    public void TriggerAttack()
    {
        if (animator != null) animator.SetTrigger(AttackHash);
    }

    public void TriggerDie()
    {
        if (animator != null) animator.SetTrigger(DieHash);
    }

    public void FaceLeft(bool left)
    {
        if (spriteToFlip != null) spriteToFlip.flipX = left;
    }
}
