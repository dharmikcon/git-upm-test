using Convai.Scripts.Setting_Panel_UI;
using UnityEngine;

namespace Convai.Scripts.Services.SettingsSystem
{
    public class ConvaiSettingsHandler : MonoBehaviour
    {
        [SerializeField] private ConvaiSettingPanel convaiSettingPanelPrefab;

        private ConvaiSettingPanel _panel;

        private void Awake()
        {
            _panel = Instantiate(convaiSettingPanelPrefab, transform);
            ConvaiServices.UISystem.OnSettingsOpened += ShowSettings;
            ConvaiServices.UISystem.OnSettingsClosed += HideSettings;
        }

        private void ShowSettings() => _panel.Show();
        private void HideSettings() => _panel.Hide();
    }
}
