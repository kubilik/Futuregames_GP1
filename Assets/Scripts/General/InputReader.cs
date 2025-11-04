using UnityEngine;
using UnityEngine.InputSystem;
using System;

namespace General
{
    public enum ControlDevice
    {
        KeyboardMouse,
        Gamepad,
        Unknown 
    }
    
    public class InputReader : MonoBehaviour, IInputService, InputSystem_Actions.IPlayerActions, InputSystem_Actions.IUIActions
    {
        // --- IInputService Contract Members ---
        // RENAMED: Changed to OnControlStateChange to resolve CS0535
        public event Action<ControlDevice> OnControlSchemeChange;
        public event Action<ControlDevice> OnControlStateChange;
        public ControlDevice CurrentControlDevice { get; private set; } = ControlDevice.Unknown;
        public event Action<Vector2> OnMoveEvent;
        public event Action OnSprintStarted;
        public event Action OnSprintCanceled;
        public event Action<Vector2> OnSprintEvent;

        public event Action OnInteractEvent;
        
        // NEW: Pause event (matches IInputService)
        public event Action OnPauseEvent;
        // -------------------------------------
        
        private InputSystem_Actions _inputsInstance;

        private void Awake()
        {
            _inputsInstance = new InputSystem_Actions();
            _inputsInstance.Player.SetCallbacks(this);
            // NEW: Set callbacks for the UI action map
            _inputsInstance.UI.SetCallbacks(this); 
            
            try
            {
                ServiceLocator.RegisterService<IInputService>((IInputService)this);
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to register IInputService: " + e.Message);
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            if (Gamepad.all.Count > 0)
            {
                CurrentControlDevice = ControlDevice.Gamepad;
            }
            else
            {
                CurrentControlDevice = ControlDevice.KeyboardMouse;
            }
            Debug.Log($"Initial Device State: {CurrentControlDevice}");
        }
        private void OnEnable()
        {
            _inputsInstance.Player.Enable();
            // NEW: Enable UI map on start
            _inputsInstance.UI.Enable(); 
            InputSystem.onDeviceChange += OnInputDeviceChange;
        }
        private void OnDisable()
        {
            _inputsInstance.Player.Disable();
            // NEW: Disable UI map on cleanup
            _inputsInstance.UI.Disable(); 
            InputSystem.onDeviceChange -= OnInputDeviceChange;
        }
        private void OnInputDeviceChange(InputDevice device, InputDeviceChange change)
        {
            if (change == InputDeviceChange.Added || change == InputDeviceChange.Disconnected)
            {
                Debug.Log($"System device change: {device.displayName} was {change}. Next input will determine scheme.");
            }
        }
        
        #region Movement
        public void OnMove(InputAction.CallbackContext context)
        {
            if (context.started) 
            {
                CheckAndReportDevice(context);
            }
            
            OnMoveEvent?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                CheckAndReportDevice(context);
                OnSprintStarted?.Invoke();
            }
            
            if (context.canceled)
            {
                OnSprintCanceled?.Invoke();
            }
            
        }
        
        #endregion
        
        #region General Interaction
        public void OnInteract(InputAction.CallbackContext context)
        {
            if (context.started) 
            {
                OnInteractEvent?.Invoke();
                CheckAndReportDevice(context);
            }
        }
        #endregion
        
        #region UI Actions
        public void OnPauseGame(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                CheckAndReportDevice(context);
                Debug.Log($"[INPUT] Pause input received from {context.control.device.displayName}."); 
                OnPauseEvent?.Invoke();
            }
        }
        
        public void EnableUIInput(bool enabled)
        {
            if (enabled)
            {
                _inputsInstance.UI.Enable();
            }
            else
            {
                _inputsInstance.UI.Disable();
            }
            Debug.Log($"[Input] UI Input Set to: {enabled}");
        }
        
        public void DisablePlayerInput()
        {
            _inputsInstance.Player.Disable();
            // Enable UI input when player is paused/in a menu
            _inputsInstance.UI.Enable(); 
            Debug.Log("[Input] Player Input Disabled, UI Input Enabled.");
        }
        
        public void EnablePlayerInput()
        {
            // Disable UI input when returning to gameplay
            _inputsInstance.UI.Disable(); 
            // Enable the Player control map
            _inputsInstance.Player.Enable();
            Debug.Log("[Input] Player Input Enabled.");
        }
        
        #endregion
        
        private void CheckAndReportDevice(InputAction.CallbackContext context)
        {
            ControlDevice detectedDevice = GetDeviceFromControl(context.control);
            
            if (CurrentControlDevice != detectedDevice)
            {
                SetNewControlDevice(detectedDevice);
            }
        }
        private void SetNewControlDevice(ControlDevice newDevice)
        {
            CurrentControlDevice = newDevice;
            // RENAMED: Invoking the corrected event name
            OnControlStateChange?.Invoke(CurrentControlDevice);
            Debug.Log($"Control scheme switched to: {CurrentControlDevice}");
        }
        private ControlDevice GetDeviceFromControl(InputControl control)
        {
            // Check for Gamepad
            if (control.device is Gamepad)
            {
                return ControlDevice.Gamepad;
            }
            // Check for Keyboard
            else if (control.device is Keyboard || control.device is Mouse)
            {
                return ControlDevice.KeyboardMouse;
            }
            return ControlDevice.Unknown;
        }
        
        #region ImplementInFuture?
        public void OnLook(InputAction.CallbackContext context) { }
        public void OnJump(InputAction.CallbackContext context) { }
        public void OnAttack(InputAction.CallbackContext context) { }
        public void OnPrevious(InputAction.CallbackContext context) { }
        public void OnNext(InputAction.CallbackContext context) { }
        
        // UI Action Callbacks (Must exist to implement IUIActions)
        public void OnNavigate(InputAction.CallbackContext context) { }
        public void OnSubmit(InputAction.CallbackContext context) { }
        public void OnCancel(InputAction.CallbackContext context) { }
        public void OnPoint(InputAction.CallbackContext context) { }
        public void OnClick(InputAction.CallbackContext context) { }
        public void OnScrollWheel(InputAction.CallbackContext context) { }
        public void OnMiddleClick(InputAction.CallbackContext context) { }
        public void OnRightClick(InputAction.CallbackContext context) { }
        public void OnTrackedDevicePosition(InputAction.CallbackContext context) { }
        public void OnTrackedDeviceOrientation(InputAction.CallbackContext context) { }
            
        //public void OnControlSchemeChange(InputAction.CallbackContext context) { }
        #endregion
    }
}
