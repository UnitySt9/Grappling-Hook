using System.Collections;
using UnityEngine;

namespace _Project.Scripts
{
    public class PlayerMovement : MonoBehaviour
    {
        private readonly KeyCode _jumpKey = KeyCode.Space;
        
        [Header("Movement")]
        [SerializeField] private float _moveSpeed;
        [SerializeField] private float _groundDrag;
        [SerializeField] private float _jumpForce;
        [SerializeField] private float _jumpCooldown;
        [SerializeField] private float _airMultiplier;
        private float _maxLinearSpeed;
        private float _maxAngularSpeed;
        private bool _isReadyToJump;
        private bool _isSwinging;
        
        [Header("Ground Check")]
        [SerializeField] private float _playerHeight;
        [SerializeField] private LayerMask _whatIsGround;
        [SerializeField] private Transform _orientation;
        private bool _isGrounded;
        private float _horizontalInput;
        private float _verticalInput;
        private Vector3 _moveDirection;
        private Rigidbody _rigidbody;
        private Coroutine _jumpResetCoroutine;

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.freezeRotation = true;
            _isReadyToJump = true;
            ResetMaxSpeeds();
        }

        private void Update()
        {
            _isGrounded = Physics.Raycast(transform.position, Vector3.down, _playerHeight * 0.5f + 0.3f, _whatIsGround);
            MyInput();
            SpeedControl();
            
            if (_isGrounded)
                _rigidbody.linearDamping = _groundDrag;
            else
                _rigidbody.linearDamping = 0;

            UpdateMaxSpeeds();
        }

        private void FixedUpdate()
        {
            MovePlayer();
        }

        private void MyInput()
        {
            _horizontalInput = Input.GetAxisRaw("Horizontal");
            _verticalInput = Input.GetAxisRaw("Vertical");
            if(Input.GetKey(_jumpKey) && _isReadyToJump && _isGrounded)
            {
                _isReadyToJump = false;
                Jump();
            }
        }

        private void MovePlayer()
        {
            _moveDirection = _orientation.forward * _verticalInput + _orientation.right * _horizontalInput;
            if(_isGrounded)
                _rigidbody.AddForce(_moveDirection.normalized * (_moveSpeed * 10f), ForceMode.Force);
            else if(!_isGrounded)
                _rigidbody.AddForce(_moveDirection.normalized * (_moveSpeed * 10f * _airMultiplier), ForceMode.Force);
        }

        private void SpeedControl()
        {
            Vector3 flatVel = new Vector3(_rigidbody.linearVelocity.x, 0f, _rigidbody.linearVelocity.z);
    
            if (_isSwinging)
            {
                if (_rigidbody.linearVelocity.magnitude > _moveSpeed)
                {
                    Vector3 limitedVel = _rigidbody.linearVelocity.normalized * _moveSpeed;
                    _rigidbody.linearVelocity = limitedVel;
                }
                _isSwinging = false;
            }
            else if(flatVel.magnitude > _moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * _moveSpeed;
                _rigidbody.linearVelocity = new Vector3(limitedVel.x, _rigidbody.linearVelocity.y, limitedVel.z);
            }
        }

        private void Jump()
        {
            _rigidbody.linearVelocity = new Vector3(_rigidbody.linearVelocity.x, 0f, _rigidbody.linearVelocity.z);
            _rigidbody.AddForce(transform.up * _jumpForce, ForceMode.Impulse);
    
            if (_jumpResetCoroutine != null)
                StopCoroutine(_jumpResetCoroutine);
    
            _jumpResetCoroutine = StartCoroutine(ResetJumpAfterDelay());
        }
        
        private IEnumerator ResetJumpAfterDelay()
        {
            yield return new WaitForSeconds(_jumpCooldown);
            _isReadyToJump = true;
            _jumpResetCoroutine = null;
        }

        private void ResetJump()
        {
            _isReadyToJump = true;
        }
        
        public void AddSwingForce(Vector3 force)
        {
            _isSwinging = true;
            _rigidbody.AddForce(force, ForceMode.Force);
        }

        private void UpdateMaxSpeeds()
        {
            float currentLinearSpeed = _rigidbody.linearVelocity.magnitude;
            float currentAngularSpeed = _rigidbody.angularVelocity.magnitude;

            if (currentLinearSpeed > _maxLinearSpeed)
            {
                _maxLinearSpeed = currentLinearSpeed;
            }

            if (currentAngularSpeed > _maxAngularSpeed)
            {
                _maxAngularSpeed = currentAngularSpeed;
            }
        }

        private void ResetMaxSpeeds()
        {
            _maxLinearSpeed = 0f;
            _maxAngularSpeed = 0f;
        }

        public float GetMaxLinearSpeed() => _maxLinearSpeed;
        public float GetMaxAngularSpeed() => _maxAngularSpeed;
    }
}
