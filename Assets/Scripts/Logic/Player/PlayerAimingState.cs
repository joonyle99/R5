using UnityEngine;
using JoonyleGameDevKit;

public sealed class PlayerAimingState : StateBase<PlayerBehaviour>
{
    public override void Enter(PlayerBehaviour owner)
    {
        var pointerInput = owner.PointerInput;
        var pointerWorldPos = pointerInput.GetWorldPos;

        // owner.SlingBehaviour.ShowAiming(pointerWorldPos);
        // owner.SetFacingDirection((Vector2)owner.transform.position - pointerWorldPos);

        owner.PlayAnimation("Attach");
        owner.AlignToHand();
    }

    public override void Exit(PlayerBehaviour owner)
    {
        owner.SlingBehaviour.HideAiming();
        owner.ResetVisualScale();
    }

    public override void Update(PlayerBehaviour owner)
    {
        var pointerInput = owner.PointerInput;
        var pointerWorldPos = pointerInput.GetWorldPos;

        if (pointerInput.JustReleased)
        {
            if (owner.SlingBehaviour.IsValidAiming(pointerWorldPos))
            {
                owner.ChangeState<PlayerBoostingState>();
                owner.SlingBehaviour.Shoot(pointerWorldPos);
            }
            else
            {
                owner.ChangeState<PlayerAttachedState>();
            }

            return;
        }

        // 손가락은 안 뗐지만 더 이상 유효한 조준이 아니면(드래그를 안으로 되돌림) 조준 해제
        if (!pointerInput.IsDragging || !owner.SlingBehaviour.IsValidAiming(pointerWorldPos))
        {
            owner.ChangeState<PlayerAttachedState>();

            return;
        }

        var dragStrength = owner.SlingBehaviour.ComupteDragStrength(pointerWorldPos);
        var dragDir = (owner.Rigid.position - pointerWorldPos).normalized;

        var angle = Mathf.Atan2(dragDir.y, dragDir.x) * Mathf.Rad2Deg - 90f;
        owner.Rigid.MoveRotation(angle);
        owner.SetFacingDirection(dragDir.x);

        var defaultScaleX = owner.DefaultVisualScale.x;
        var scaleY = Mathf.Lerp(owner.DefaultVisualScale.y, owner.DefaultVisualScale.y * owner.StretchAmount, dragStrength);
        owner.SetVisualScale(defaultScaleX * owner.DefaultVisualScale.y / scaleY, scaleY);

        // 유효 조준 유지 중 — 매 프레임 조준선·방향 갱신 (어시스트 적용된 위치 기준)
        owner.SlingBehaviour.ShowAiming(pointerWorldPos);
    }
}
