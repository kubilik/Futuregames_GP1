/*using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using PlayerScripts;

namespace General
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        
        private IScoreService _scoreService;
        private IInputService _inputService;
        private PlayerMovement _playerMovement;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
            
            GetServices();
        }
        
        private void GetServices()
        {
            try
            {
                _scoreService = ServiceLocator.GetService<IScoreService>();
            }
            catch (InvalidOperationException e)
            {
                Debug.LogWarning($"IScoreService not found. Ensure ScoreManager is initialized. Error: {e.Message}");
            }
            
            _playerMovement = FindObjectOfType<PlayerMovement>();
            if (_playerMovement == null)
            {
                Debug.LogError("PlayerMovement not found in the scene.");
            }
            
        }
        
        public void ResetGame()
        {
            Debug.Log("--- Game Reset Initiated ---");

            // 1. Reset Score
            if (_scoreService != null)
            {
                _scoreService.ResetScore();
                Debug.Log("Score system reset.");
            }
            
            // 2. Reset Player State (Assuming we move them to a start position)
            if (_playerMovement != null)
            {
                _playerMovement.SetPlayerState(PlayerState.IsIdle);
                Debug.Log("Player state reset to Idle.");
            }
            
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            
            Debug.Log($"Level '{SceneManager.GetActiveScene().name}' reloaded.");
        }
    }
}*/
