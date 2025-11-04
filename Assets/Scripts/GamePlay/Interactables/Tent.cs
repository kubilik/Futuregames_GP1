using UnityEngine;
using Interfaces;
using PlayerScripts;

namespace GamePlay.Interactables
{
    // TentState enum provided by the user
    public enum TentState
    {
        Default, // All visuals disabled
        Normal,
        Damaged, // Renamed 'Destroyed' visual state to 'Damaged' for clarity
        Burned
    }
    
    public class Tent : MonoBehaviour , IInteractable
    {
        
        [Header("Visual States")]
        [Tooltip("The GameObject representing the healthy tent.")]
        [SerializeField] 
        private GameObject tentsNormalState;
        
        [Tooltip("The GameObject representing the damaged or destroyed tent.")]
        [SerializeField] 
        private GameObject tentsDamagedState;
        
        [Tooltip("The GameObject representing the burned tent.")]
        [SerializeField] 
        private GameObject tentsBurnedState;

        [Header("Current State")]
        // This is the private backing field for the tent's current state.
        // It is set to Normal by default, fulfilling the initialization requirement.
        [SerializeField] 
        private TentState _currentTentState = TentState.Normal;
        
        public string InteractionPrompt { get; } = "Tent";

        // Unity calls this when the script instance is being loaded.
        private void Awake()
        {
            // Ensure the correct visual state is set when the game starts.
            UpdateVisuals(_currentTentState);
        }

        // Method to switch the visuals based on the required state
        private void UpdateVisuals(TentState newState)
        {
            tentsNormalState.SetActive(false);
            tentsDamagedState.SetActive(false);
            tentsBurnedState.SetActive(false);
            
            switch (newState)
            {
                case TentState.Normal:
                    tentsNormalState.SetActive(true);
                    break;
                case TentState.Damaged:
                    tentsDamagedState.SetActive(true);
                    break;
                case TentState.Burned:
                    tentsBurnedState.SetActive(true);
                    break;
                case TentState.Default:
                    // Default state means all are off. No action needed here.
                    break;
            }

            // Update the internal state field
            _currentTentState = newState;
        }
        
        public void Interact(GameObject interactor, PlayerScripts.PlayerMovement playerMovement)
        {
            Debug.Log($"Player ({interactor.name}) is interacting with the {InteractionPrompt}. Current state: {_currentTentState}");
            
            // if (_currentTentState == TentState.Normal)
            // {
            //     UpdateVisuals(TentState.Damaged);
            // }
        }
        private void OnValidate()
        {
            if (tentsNormalState && tentsDamagedState && tentsBurnedState)
            {
                UpdateVisuals(_currentTentState);
            }
        }
    }
}
