using UnityEngine;
using UnityEngine.InputSystem;

public class CameraPlayer : MonoBehaviour
{
    public float sensitivity = 2f;
    [Range(0f, 90f)][SerializeField] float yRotationLimit = 88f;

    Vector2 rotation = Vector2.zero;

    private void Update()
    {
        if (Mouse.current != null) // Vérifie si la souris est connectée
        {
            rotation.x += Mouse.current.delta.x.ReadValue() * sensitivity * Time.deltaTime;
            rotation.y += Mouse.current.delta.y.ReadValue() * sensitivity * Time.deltaTime;
            rotation.y = Mathf.Clamp(rotation.y, -yRotationLimit, yRotationLimit);

            var xQuat = Quaternion.AngleAxis(rotation.x, Vector3.up);
            var yQuat = Quaternion.AngleAxis(rotation.y, Vector3.left);

            Transform player = transform.parent.transform;
            player.localRotation = xQuat;
            transform.localRotation = yQuat;
        }
    }
}
