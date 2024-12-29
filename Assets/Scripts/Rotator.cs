using UnityEngine;

// 게임 오브젝트를 지속적으로 회전하는 스크립트
public class Rotator : MonoBehaviour {
    public float rotationSpeed = 60f;

    private void Update() {
        // 이 스크립트가 부착된 게임 오브젝트의 Transform 컴포넌트에 영향을 미침
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
    }
}