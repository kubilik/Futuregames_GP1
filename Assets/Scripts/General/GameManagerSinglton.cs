using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using PlayerScripts;
using UI;

namespace General
{
    public enum GameState
    {
        InGame,
        OnPause,
        GameOver,
        //PreGame
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        
        // State Management
        
        public event Action<GameState> OnGameStateChange;
        public GameState CurrentGameState { get; private set; } = GameState.InGame;
        
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
            
            SetGameState(GameState.InGame);
            
        }
        
        private void Start()
        {
            GetServices(); 
        }
        
        private void GetServices()
        {
        
            try
            {
                _inputService = ServiceLocator.GetService<IInputService>();
                
                _inputService.OnPauseEvent += TogglePause;
                
                Debug.Log("IInputService and Pause Event subscribed successfully.");
            }
            catch (InvalidOperationException e)
            {
                Debug.LogError("IInputService not found. Is InputReader registered in Awake? Error: " + e.Message);
            }
            
            _playerMovement = FindObjectOfType<PlayerMovement>();
        }
        
        public void TogglePause()
        {
            if (CurrentGameState == GameState.OnPause)
            {
                SetGameState(GameState.InGame);
            }
            else if (CurrentGameState == GameState.InGame)
            {
                SetGameState(GameState.OnPause);
            }
        }
        
        private void SetGameState(GameState newState)
        {
            if (CurrentGameState == newState) return;
            CurrentGameState = newState;
            
            StartCoroutine(ProcessStateChange(newState));
        
            OnGameStateChange?.Invoke(CurrentGameState);
        }
        
        private System.Collections.IEnumerator ProcessStateChange(GameState newState)
        {

            yield return null; 
        
            switch (newState)
            {
                case GameState.InGame:
                    Time.timeScale = 1f;
                    if (_inputService != null)
                    {
                        _inputService.EnablePlayerInput();
                        _inputService.EnableUIInput(false); 
                    }
                    Debug.Log("Game State: InGame. Time Scale set to 1.");
                    break;
        
                case GameState.OnPause:
                    Time.timeScale = 0f; 
                    if (_inputService != null)
                    {
                        _inputService.DisablePlayerInput();
                        _inputService.EnableUIInput(true); 
                    }
                    Debug.Log("Game State: OnPause. Time Scale set to 0.");
                    break;
            }
        }

        public void ResetGame()
        {
            Debug.Log("--- Game Reset Initiated ---");
            
            /*if (_scoreService != null)
            {
                _scoreService.ResetScore();
                Debug.Log("Score system reset.");
            }*/
            
            if (_playerMovement != null)
            {
                _playerMovement.SetPlayerState(PlayerState.IsIdle);
                Debug.Log("Player state reset to Idle.");
            }
            
            Time.timeScale = 1f; 
            
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            
            Debug.Log($"Level '{SceneManager.GetActiveScene().name}' reloaded.");
        }
    }
}
