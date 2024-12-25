using UnityEngine;
using UnityEngine.UI; // UI 관련 코드

// 플레이어 캐릭터의 생명체로서의 동작을 담당
public class PlayerHealth : LivingEntity {
    public Slider healthSlider; // 체력을 표시할 UI 슬라이더

    public AudioClip deathClip; // 사망 소리
    public AudioClip hitClip; // 피격 소리
    public AudioClip itemPickupClip; // 아이템 습득 소리

    private AudioSource playerAudioPlayer; // 플레이어 소리 재생기
    private Animator playerAnimator; // 플레이어의 애니메이터

    private PlayerMovement playerMovement; // 플레이어 움직임 컴포넌트
    private PlayerShooter playerShooter; // 플레이어 슈터 컴포넌트

    private void Awake() {
        // 사용할 컴포넌트를 가져오기
        playerAudioPlayer = GetComponent<AudioSource>();
        playerAnimator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        playerShooter = GetComponent<PlayerShooter>();
    }

    protected override void OnEnable() {
        // LivingEntity의 OnEnable() 실행 (상태 초기화)
        base.OnEnable();

        healthSlider.gameObject.SetActive(true); // 체력 슬라이더 활성화
        healthSlider.maxValue = startingHealth; // 슬라이더의 최대값을 시작 체력으로
        healthSlider.value = health; // 슬라이더의 값도 시작 체력으로 초기화

        // 플레이어 조작을 받는 컴포넌트들 활성화
        playerMovement.enabled = true; // 플레이어 움직임 활성화
        playerShooter.enabled = true; // 플레이어 슈터 활성화
    }

    // 체력 회복
    public override void RestoreHealth(float newHealth) {
        // LivingEntity의 RestoreHealth() 실행 (체력 증가)
        base.RestoreHealth(newHealth);
        healthSlider.value = health; // 갱신된 체력으로 체력 슬라이더 갱신
    }

    // 데미지 처리
    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitDirection) {
        // LivingEntity의 OnDamage() 실행(데미지 적용)
        if(!dead) {
            playerAudioPlayer.PlayOneShot(hitClip); // 피격 소리 재생
        }
        base.OnDamage(damage, hitPoint, hitDirection);
        healthSlider.value = health; // 갱신된 체력으로 체력 슬라이더 갱신
    }

    // 사망 처리
    public override void Die() {
        // LivingEntity의 Die() 실행(사망 적용)
        base.Die();
        
        healthSlider.gameObject.SetActive(false); // 체력 슬라이더 비활성화
        playerAudioPlayer.PlayOneShot(deathClip); // 사망 소리 재생
        playerAnimator.SetTrigger("Die"); // 사망 애니메이션 재생

        playerMovement.enabled = false; // 플레이어 조작 불가
        playerShooter.enabled = false; // 플레이어 조작 불가
    }

    private void OnTriggerEnter(Collider other) {
        // 아이템과 충돌한 경우 해당 아이템을 사용하는 처리
        IItem item = other.GetComponent<IItem>();
        if (item != null) {
            item.Use(gameObject); // 아이템 사용
            playerAudioPlayer.PlayOneShot(itemPickupClip); // 아이템 습득 소리 재생
        }
    }
}