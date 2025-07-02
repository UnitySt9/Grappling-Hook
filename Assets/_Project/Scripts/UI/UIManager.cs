using UnityEngine;
using TMPro;

namespace _Project.Scripts
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private PlayerMovement _playerMovement;
        [SerializeField] private TMP_Text _maxSpeedText;
        [SerializeField] private TMP_Text _maxAngularSpeedText;

        private void Update()
        {
            UpdateSpeedUI();
        }

        private void UpdateSpeedUI()
        {
            if (_maxSpeedText != null)
            {
                _maxSpeedText.text = $"Speed: {_playerMovement.GetMaxLinearSpeed():F2} m/s";
            }

            if (_maxAngularSpeedText != null)
            {
                _maxAngularSpeedText.text = $"Angular Speed: {_playerMovement.GetMaxAngularSpeed():F2} rad/s";
            }
        }
    }
}
