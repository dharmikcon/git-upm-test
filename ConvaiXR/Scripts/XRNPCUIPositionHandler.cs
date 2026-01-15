using Convai.Scripts;
using UnityEngine;

/// <summary>
/// Handles the UI transformation for XR interactions, adjusting the UI position based on the player's camera distance from an NPC.
/// Also uses raycasting to determine the NPC the player is looking at.
/// </summary>
public class XRNPCUIPositionHandler : MonoBehaviour
{
    [SerializeField] private float _lerpSpeed;
    [SerializeField] private Vector3 _offset;
    [SerializeField] private float _cameraDistanceThreshold;
    [SerializeField] private float _raycastMaxDistance = 10f;

    private Camera _playerCamera;
    private ConvaiNPC _currentNPC;

    private void OnEnable()
    {
        //if (_currentNPC == null)
        //{
        //    _currentNPC = FindAnyObjectByType<ConvaiNPC>();
        //}
    }

    private void Start()
    {
        _playerCamera = Camera.main;
    }

    private void LateUpdate()
    {
        RaycastForNPC();

        if (_currentNPC != null)
        {
            UpdateUIPosition();
            FaceCamera();
        }
    }

    /// <summary>
    /// Performs a raycast from the center of the camera to find an NPC.
    /// If a new NPC is hit, updates the reference.
    /// </summary>
    private void RaycastForNPC()
    {
        if (_playerCamera == null)
            return;

        Ray ray = new Ray(_playerCamera.transform.position, _playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, _raycastMaxDistance))
        {
            ConvaiNPC foundNPC = hit.transform.GetComponent<ConvaiNPC>();
            if (foundNPC != null && foundNPC != _currentNPC)
            {
                _currentNPC = foundNPC;
                SetUIPosition(); // Snap UI instantly to new NPC
            }
        }
    }

    private void SetUIPosition()
    {
        if (_currentNPC == null)
            return;

        Transform npcTransform = _currentNPC.transform;
        Vector3 targetPosition = CalculateTargetPosition(npcTransform);
        transform.position = targetPosition;
    }

    private void UpdateUIPosition()
    {
        if (_currentNPC == null)
            return;

        Transform npcTransform = _currentNPC.transform;
        Vector3 targetPosition = CalculateTargetPosition(npcTransform);
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * _lerpSpeed);
    }

    private Vector3 CalculateTargetPosition(Transform npcTransform)
    {
        Vector3 leftOffset = new Vector3(-_offset.x, _offset.y, _offset.z);
        Vector3 rightOffset = new Vector3(_offset.x, _offset.y, _offset.z);

        Vector3 leftOffsetPosition = npcTransform.position + npcTransform.TransformDirection(leftOffset);
        Vector3 rightOffsetPosition = npcTransform.position + npcTransform.TransformDirection(rightOffset);

        float distanceToLeftOffset = Vector3.Distance(leftOffsetPosition, _playerCamera.transform.position);
        float distanceToRightOffset = Vector3.Distance(rightOffsetPosition, _playerCamera.transform.position);

        Vector3 dynamicOffset = DetermineDynamicOffset(distanceToLeftOffset, distanceToRightOffset);

        return npcTransform.position + npcTransform.TransformDirection(dynamicOffset);
    }

    private Vector3 DetermineDynamicOffset(float distanceToLeftOffset, float distanceToRightOffset)
    {
        Vector3 leftOffset = new Vector3(-_offset.x, _offset.y, _offset.z);
        Vector3 rightOffset = new Vector3(_offset.x, _offset.y, _offset.z);

        float threshold = 0.5f;

        if (distanceToLeftOffset < _cameraDistanceThreshold && distanceToRightOffset < _cameraDistanceThreshold)
        {
            float difference = Mathf.Abs(distanceToLeftOffset - distanceToRightOffset);
            return difference > threshold
                ? (distanceToLeftOffset > distanceToRightOffset ? leftOffset : rightOffset)
                : leftOffset;
        }
        else
        {
            return distanceToLeftOffset >= _cameraDistanceThreshold ? leftOffset : rightOffset;
        }
    }

    private void FaceCamera()
    {
        Vector3 direction = transform.position - _playerCamera.transform.position;
        transform.rotation = Quaternion.LookRotation(direction);
    }
}
