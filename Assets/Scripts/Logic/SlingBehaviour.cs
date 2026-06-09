using System;
using UnityEngine;

public class SlingBehaviour : MonoBehaviour
{
    [SerializeField] private float _lineLength = 5f;
    [SerializeField] private float _minAimingDist = 0.5f; // 발사로 인정할 최소 당김 거리 (이하는 조준 무시)
    [SerializeField] private float _maxAimingDist = 4f; // 세기가 최대가 되는 당김 거리 (이 이상은 상한 고정)
    [SerializeField] private float _minShotSpeed = 8f; // 최소 당김(_minAimingDist)일 때 발사 속도
    [SerializeField] private float _maxShotSpeed = 20f; // 최대 당김(_maxAimingDist)일 때 발사 속도
    [SerializeField] private float _magnetStrength = 15f;

    private Rigidbody2D _rigid;
    public Rigidbody2D Rigid => _rigid;
    private LineRenderer _line;

    private float _sqrMinAimingDist; // _minAimingDist 제곱 (IsValidAiming 비교용)

    private bool _isActiveSling;
    public bool IsActiveSling => _isActiveSling;

    public void Initialize(Rigidbody2D rigid)
    {
        _rigid = rigid;

        _sqrMinAimingDist = _minAimingDist * _minAimingDist;

        _line = GetComponentInChildren<LineRenderer>();
        _line.useWorldSpace = true;
        _line.positionCount = 2;
        _line.enabled = false;
    }

    // ============ ... ============

    public void SetActiveSling(bool active)
    {
        _isActiveSling = active;
    }

    // ============ 물리 모드 전환 ============

    public void SetPhysicsOnAnchor()
    {
        _rigid.linearVelocity = Vector2.zero;
        _rigid.bodyType = RigidbodyType2D.Kinematic;
    }

    public void SetPhysicsInAir()
    {
        _rigid.bodyType = RigidbodyType2D.Dynamic;
    }

    // ============ ... ============

    // 드래그 세기 0~1: _minAimingDist ~ _maxAimingDist 범위를 정규화.
    // ComputeShotVelocity의 t와 동일한 기준이므로 카메라 zoom이 발사 세기와 일치한다.
    public float ComupteDragStrength(Vector2 aimPos)
    {
        var dist = Vector2.Distance(Rigid.position, aimPos);
        return Mathf.Clamp01(dist / _maxAimingDist);
    }

    private Vector2 ComputeShotVelocity(Vector2 pullBackPos)
    {
        var origin = Rigid.position;
        var dir = (origin - pullBackPos).normalized;
        var dist = Vector2.Distance(pullBackPos, origin);
        var t = Mathf.Clamp01(dist / _maxAimingDist); // 당김 정도를 0 ~ 1로 정규화
        var speed = Mathf.Lerp(_minShotSpeed, _maxShotSpeed, t);
        var velocity = dir * speed;

        return velocity;
    }

    public Vector2 ComputePullBackPos(Vector2 targetPos)
    {
        var origin = Rigid.position;
        return origin - (targetPos - origin).normalized * _maxAimingDist;
    }

    // ============ 조준 ============

    // 발사로 인정할 만큼 충분히 당겼는지 (플레이어로부터의 월드 거리 기준).
    // PointerInput.DragThreshold(화면 px, 탭/드래그 구분)와는 별개의 게임플레이 게이트.
    // 비교만 하므로 sqrt(Vector2.Distance) 대신 sqrMagnitude로 양변 제곱 비교(_sqrMinAimingDist 캐싱).
    public bool IsValidAiming(Vector2 pullBackPos)
    {
        var sqrDist = (Rigid.position - pullBackPos).sqrMagnitude;
        return sqrDist >= _sqrMinAimingDist;
    }

    // 발사 방향으로의 직선. 당김 세기(0~1)에 비례해 길이가 늘어남.
    public void ShowAiming(Vector2 pullBackPos)
    {
        var origin = Rigid.position;
        var dir = (origin - pullBackPos).normalized;
        var length = _lineLength * ComupteDragStrength(pullBackPos);

        _line.SetPosition(0, origin);
        _line.SetPosition(1, origin + dir * length);
        _line.enabled = true;
    }

    public void HideAiming()
    {
        _line.enabled = false;
    }

    // ============ 위치 이동 ============

    // 목표 위치 방향으로 일정 속도로 이동. 부착 후 중심점으로 자연스럽게 끌려가는 후처리.
    public void Magnet(Vector2 target)
    {
        var anchorOffset = Rigid.position - (Vector2)transform.position;
        var adjustedTarget = target - anchorOffset;
        _rigid.position = Vector2.MoveTowards(_rigid.position, adjustedTarget, _magnetStrength * Time.fixedDeltaTime);
    }

    // ============ 발사 ============

    public void Shoot(Vector2 pullBackPos)
    {
        _rigid.linearVelocity = ComputeShotVelocity(pullBackPos);
    }

    public void ShootAt(Vector2 targetPos)
    {
        var pullBackPos = ComputePullBackPos(targetPos);
        
        Shoot(pullBackPos);
    }
}
