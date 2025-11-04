using UnityEngine;
using General;
using System;

namespace UI
{
    public class UI_Manager : MonoBehaviour
    {
        [Tooltip("The root GameObject for the Pause Menu UI.")]
        [SerializeField] private GameObject pauseMenuPanel;
        
        private GameManager _gameManager;
        
        private IInputService _inputService;
        private ControlDevice _currentDevice = ControlDevice.Unknown;
        
        private void Start()
        {
            try
            {
                _gameManager = GameManager.Instance;
                
                if (_gameManager == null)
                {
                    Debug.LogError("UIManager failed to find GameManager Instance. Cannot subscribe to state changes.");
                    return;
                }
                
                _gameManager.OnGameStateChange += HandleGameStateChange;
                
                _inputService = ServiceLocator.GetService<IInputService>();
                _inputService.OnControlSchemeChange += HandleControlSchemeChange;
                
                HandleGameStateChange(_gameManager.CurrentGameState);
                HandleControlSchemeChange(_inputService.CurrentControlDevice);
            }
            catch (InvalidOperationException e)
            {
                Debug.LogError($"UIManager Initialization failed: {e.Message}");
            }
        }

        private void OnDestroy()
        {
            if (_gameManager != null)
            {
                _gameManager.OnGameStateChange -= HandleGameStateChange;
            }
            if (_inputService != null)
            {
                _inputService.OnControlSchemeChange -= HandleControlSchemeChange;
            }
        }
        
        private void HandleControlSchemeChange(ControlDevice newDevice)
        {
            _currentDevice = newDevice;
            Debug.Log($"UI Manager: Current input device set to {_currentDevice}");
            // TODO: Add logic here to trigger UI elements (like button icons)
        }

        private void HandleGameStateChange(GameState newState)
        {
            if (pauseMenuPanel == null)
            {
                Debug.LogWarning("Pause Menu Panel reference is missing in UIManager.");
                return;
            }

            if (newState == GameState.OnPause)
            {
                pauseMenuPanel.SetActive(true);
                Debug.Log("UI Manager: Pause Menu Opened.");
            }
            else
            {
                pauseMenuPanel.SetActive(false);
                Debug.Log("UI Manager: Pause Menu Closed.");
            }
        }
    }
}
