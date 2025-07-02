using UnityEngine;

namespace _Project.Scripts
{
    public class PlayerCam : MonoBehaviour
    {
        [SerializeField] private float _sensX;
        [SerializeField] private float _sensY;
        [SerializeField] private Transform _orientation;
        private float _xRotation;
        private float _yRotation;
        private float _mouseX;
        private float _mouseY;
    
        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    
        void Update()
        {
            _mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * _sensX;
            _mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * _sensY;
            _yRotation += _mouseX;
            _xRotation -= _mouseY;
            _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);
            transform.rotation = Quaternion.Euler(_xRotation, _yRotation, 0f);
            _orientation.rotation = Quaternion.Euler(0f, _yRotation, 0f);
        }
    }
}
