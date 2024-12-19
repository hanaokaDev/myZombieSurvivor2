using System.Collections;
using UnityEngine;

// 총을 구현한다
public class Gun : MonoBehaviour {
    // 총의 상태를 표현하는데 사용할 타입을 선언한다
    public enum State {
        Ready, // 발사 준비됨
        Empty, // 탄창이 빔
        Reloading // 재장전 중
    }

    public State state { get; private set; } // 현재 총의 상태

    public Transform fireTransform; // 총알이 발사될 위치

    public ParticleSystem muzzleFlashEffect; // 총구 화염 효과
    public ParticleSystem shellEjectEffect; // 탄피 배출 효과

    private LineRenderer bulletLineRenderer; // 총알 궤적을 그리기 위한 렌더러

    private AudioSource gunAudioPlayer; // 총 소리 재생기
    public AudioClip shotClip; // 발사 소리
    public AudioClip reloadClip; // 재장전 소리

    public float damage = 25; // 공격력
    private float fireDistance = 50f; // 사정거리

    public int ammoRemain = 100; // 남은 전체 탄약
    public int magCapacity = 25; // 탄창 용량
    public int magAmmo; // 현재 탄창에 남아있는 탄약


    public float timeBetFire = 0.12f; // 총알 발사 간격
    public float reloadTime = 1.8f; // 재장전 소요 시간
    private float lastFireTime; // 총을 마지막으로 발사한 시점


    private void Awake() {
        // 사용할 컴포넌트들의 참조를 가져오기
        gunAudioPlayer = GetComponent<AudioSource>();
        bulletLineRenderer = GetComponent<LineRenderer>();

        bulletLineRenderer.positionCount = 2; // 사용할 점은 2개. 총구위치와 총알이 맞은 위치
        bulletLineRenderer.enabled = false; // 인스펙터 렌더러 컴포넌트를 비활성화했지만, 코드에서도 확실하게 비활성화
    }

    private void OnEnable() {
        // 총 상태 초기화
        magAmmo = magCapacity; // 탄 최대용량으로 채우기
        state = State.Ready; // 발사준비상태
        lastFireTime = 0;
    }

    // 발사 시도
    public void Fire() {
        if(state == State.Ready && Time.time >= lastFireTime + timeBetFire) {
            lastFireTime = Time.time;
            Shot();
        }
    }

    // Ray를 활용한 실제 발사 처리
    private void Shot() {
        RaycastHit hit;
        Vector3 hitPosition = Vector3.zero; // 레이가 맞은 위치

        // 인수: 시작지점, 방향, 충돌정보 컨테이너, 사정거리
        if(Physics.Raycast(fireTransform.position, fireTransform.forward, out hit, fireDistance)) { // 레이가 다른 물체와 충돌했다면
            var target = hit.collider.GetComponent<IDamageable>();
            if(target != null) {
                target.OnDamage(damage, hit.point, hit.normal);
            }
            hitPosition = hit.point;
        } else {
            hitPosition = fireTransform.position + fireTransform.forward * fireDistance;
        }
        StartCoroutine(ShotEffect(hitPosition));
        magAmmo--;
        if (magAmmo <= 0) {
            state = State.Empty; // 탄창에 남은 탄알이 없다면 Empty 상태로 전환
        }
    }

    // 발사 이펙트와 소리를 재생하고 총알 궤적을 그린다
    private IEnumerator ShotEffect(Vector3 hitPosition) {
        muzzleFlashEffect.Play(); // 총구 화염 효과 재생
        shellEjectEffect.Play(); // 탄피 배출 효과 재생
        gunAudioPlayer.PlayOneShot(shotClip); // 총 발사 소리 재생
        bulletLineRenderer.SetPosition(0, fireTransform.position); // 라인렌더러의 첫번째 점은 총구의 위치
        bulletLineRenderer.SetPosition(1, hitPosition); // 라인렌더러의 두번째 점은 입력으로 들어온 충돌위치

        // 라인 렌더러를 활성화하여 총알 궤적을 그린다
        bulletLineRenderer.enabled = true;

        // 0.03초 동안 잠시 처리를 대기
        yield return new WaitForSeconds(0.03f);

        // 라인 렌더러를 비활성화하여 총알 궤적을 지운다
        bulletLineRenderer.enabled = false;
    }

    // 재장전 시도
    public bool Reload() {
        if(state == State.Reloading || ammoRemain <= 0 || magAmmo >= magCapacity) {
            return false;
        }
        StartCoroutine(ReloadRoutine());
        return true;
    }

    // 실제 재장전 처리를 진행
    private IEnumerator ReloadRoutine() {
        // 현재 상태를 재장전 중 상태로 전환
        state = State.Reloading;
        gunAudioPlayer.PlayOneShot(reloadClip); // 재장전 소리 재생
        
        // 재장전 소요 시간 만큼 처리를 쉬기
        yield return new WaitForSeconds(reloadTime);

        int ammoToFill = magCapacity - magAmmo; // 채워야 할 탄약 계산
        if(ammoRemain < ammoToFill) {
            ammoToFill = ammoRemain;
        }

        magAmmo += ammoToFill; // 탄창 채우기
        ammoRemain -= ammoToFill; // 남은 탄약에서 탄창에 넣은만큼 빼기

        // 총의 현재 상태를 발사 준비된 상태로 변경
        state = State.Ready;
    }
}