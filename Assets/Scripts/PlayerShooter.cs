﻿using UnityEngine;

// 주어진 Gun 오브젝트를 쏘거나 재장전
// 알맞은 애니메이션을 재생하고 IK를 사용해 캐릭터 양손이 총에 위치하도록 조정
public class PlayerShooter : MonoBehaviour {
    public Gun gun; // 사용할 총
    public Transform gunPivot; // 총 배치의 기준점
    public Transform leftHandMount; // 총의 왼쪽 손잡이, 왼손이 위치할 지점
    public Transform rightHandMount; // 총의 오른쪽 손잡이, 오른손이 위치할 지점

    private PlayerInput playerInput; // 플레이어의 입력
    private Animator playerAnimator; // 애니메이터 컴포넌트

    private void Start() {
        // 사용할 컴포넌트들을 가져오기
        playerInput = GetComponent<PlayerInput>();
        playerAnimator = GetComponent<Animator>();
    }

    private void OnEnable() {
        // 슈터가 활성화될 때 총도 함께 활성화
        gun.gameObject.SetActive(true);
    }
    
    private void OnDisable() {
        // 슈터가 비활성화될 때 총도 함께 비활성화
        gun.gameObject.SetActive(false);
    }

    private void Update() {
        // 입력을 감지하고 총 발사하거나 재장전
        if(playerInput != null) {  
            if(playerInput.fire) {
                gun.Fire();
            }
            else if(playerInput.reload) {
                if(gun.Reload()) {
                    playerAnimator.SetTrigger("Reload");
                }
            }
        }
        UpdateUI(); // 남은 탄약 UI 갱신
    }

    // 남은 탄약 UI 갱신
    private void UpdateUI() {
        if (gun != null && UIManager.instance != null)
        {
            UIManager.instance.UpdateAmmoText(gun.magAmmo, gun.ammoRemain); // UI 매니저의 탄약 텍스트에 탄창의 탄약과 남은 전체 탄약을 표시
        }
    }

    // 애니메이터의 IK 갱신
    private void OnAnimatorIK(int layerIndex) {
        gunPivot.position = playerAnimator.GetIKHintPosition(AvatarIKHint.RightElbow); // 총의 기준점을 오른쪽 팔꿈치 위치로



        playerAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f); // 왼손 위치를 지정
        playerAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f); // 왼손 회전을 지정
        playerAnimator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandMount.position); // 왼손의 위치를 왼손잡이의 위치로
        playerAnimator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandMount.rotation); // 왼손의 회전을 왼손잡이의 회전으로

        playerAnimator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f); // 오른손 위치를 지정
        playerAnimator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f); // 오른손 회전을 지정
        playerAnimator.SetIKPosition(AvatarIKGoal.RightHand, rightHandMount.position); // 오른손의 위치를 오른손잡이의 위치로
        playerAnimator.SetIKRotation(AvatarIKGoal.RightHand, rightHandMount.rotation); // 오른손의 회전을 오른손잡이의 회전으로

    }
}