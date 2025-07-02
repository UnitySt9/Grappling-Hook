using UnityEngine;
using System.Collections.Generic;

namespace _Project.Scripts
{
    public class GrapplingRope : MonoBehaviour
    {
        [Header("Rope Settings")]
        [SerializeField] private int _segmentCount = 20;
        [SerializeField] private float _segmentLength = 0.25f;
        [SerializeField] private float _ropeWidth = 0.1f;
        [SerializeField] private float _maxRopeLength = 10f;
        [SerializeField] private LayerMask _grappableLayers;
        [SerializeField] private LayerMask _whatIsGround;
        
        [Header("Swing Settings")]
        [SerializeField] private float _pullSpeed = 2f;
        
        private LineRenderer _lineRenderer;
        private List<RopeSegment> _ropeSegments = new List<RopeSegment>();
        private bool _isRopeActive = false;
        private float _minDetachDistance = 1.5f;
        private Vector3 _grapplePoint;
        private PlayerMovement _playerMovement;
        private Camera _mainCamera;

        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            _playerMovement = GetComponent<PlayerMovement>();
            _mainCamera = Camera.main;
            _lineRenderer.startWidth = _ropeWidth;
            _lineRenderer.endWidth = _ropeWidth;
            _lineRenderer.positionCount = 0;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                ShootGrapple();
            }
            
            if (Input.GetMouseButtonDown(1))
            {
                DetachGrapple();
            }
            
            if (_isRopeActive)
            {
                if (Vector3.Distance(transform.position, _grapplePoint) < _minDetachDistance)
                {
                    DetachGrapple();
                    return;
                }
                
                HandleSwingInput();
                SimulateRope();
                DrawRope();
            }
        }

        private void ShootGrapple()
        {
            if (_isRopeActive) 
                return;
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, _maxRopeLength, _grappableLayers))
            {
                _grapplePoint = hit.point;
                InitializeRope();
                _isRopeActive = true;
            }
        }

        private void DetachGrapple()
        {
            if (!_isRopeActive) 
                return;
            _isRopeActive = false;
            _lineRenderer.positionCount = 0;
        }

        private void InitializeRope()
        {
            _ropeSegments.Clear();
            Vector3 startPoint = transform.position;
            for (int i = 0; i < _segmentCount; i++)
            {
                _ropeSegments.Add(new RopeSegment(startPoint));
                startPoint.y -= _segmentLength;
            }
        }

        private void SimulateRope()
        {
          Vector3 gravity = Physics.gravity * 0.2f;
          for (int i = 0; i < _segmentCount; i++)
          {
              RopeSegment segment = _ropeSegments[i];
              Vector3 velocity = segment.currentPos - segment.previousPos;
              velocity *= 0.99f;
              segment.previousPos = segment.currentPos;
              segment.currentPos += velocity + gravity * Time.deltaTime;
          }
    
          for (int j = 0; j < 50; j++) 
          {
              ApplyConstraints();
          }
        }
        
        private void ApplyConstraints()
        {
            _ropeSegments[0].currentPos = transform.position;
            _ropeSegments[_segmentCount-1].currentPos = _grapplePoint;
            for (int i = 0; i < _segmentCount - 1; i++)
            {
                RopeSegment thisSeg = _ropeSegments[i];
                RopeSegment nextSeg = _ropeSegments[i+1];
                float dist = Vector3.Distance(thisSeg.currentPos, nextSeg.currentPos);
                float error = Mathf.Abs(dist - _segmentLength);
                Vector3 changeDir = Vector3.zero;

                if (dist > _segmentLength)
                {
                    changeDir = (thisSeg.currentPos - nextSeg.currentPos).normalized;
                }
                else if (dist < _segmentLength)
                {
                    changeDir = (nextSeg.currentPos - thisSeg.currentPos).normalized;
                }
        
                Vector3 changeAmount = changeDir * error;
        
                if (i != 0) 
                {
                    thisSeg.currentPos -= changeAmount * 0.5f;
                }
                nextSeg.currentPos += changeAmount * 0.5f;
            }
    
            for (int i = 1; i < _segmentCount - 1; i++)
            {
                if (Physics.SphereCast(_ropeSegments[i].previousPos, _ropeWidth * 0.5f, 
                        (_ropeSegments[i].currentPos - _ropeSegments[i].previousPos).normalized, 
                        out RaycastHit hit, 
                        Vector3.Distance(_ropeSegments[i].previousPos, _ropeSegments[i].currentPos),
                        _whatIsGround))
                {
                    _ropeSegments[i].currentPos = hit.point + hit.normal * (_ropeWidth * 0.5f);
                }
            }
        }

        private void DrawRope()
        {
            _lineRenderer.positionCount = _segmentCount;
            for (int i = 0; i < _segmentCount; i++)
            {
                _lineRenderer.SetPosition(i, _ropeSegments[i].currentPos);
            }
        }

        private void HandleSwingInput()
        {
            if (Input.GetKey(KeyCode.W))
            {
                Vector3 directionToGrapple = (_grapplePoint - transform.position).normalized;
                _playerMovement.AddSwingForce(directionToGrapple * _pullSpeed);
            }
        }
    }
}
