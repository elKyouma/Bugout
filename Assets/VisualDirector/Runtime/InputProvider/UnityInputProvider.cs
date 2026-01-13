using System.Threading.Tasks;
using Unity.GraphToolkit.Samples.VisualNovelDirector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace VisualDirector
{
    public class UnityInputProvider : MonoBehaviour, IVisualDirectorInputProvider
    {
        private VisualDirectorInput _inputActions;
        private TaskCompletionSource<bool> _nextTcs;
        private TaskCompletionSource<int> _choiveTcs;
        private void Awake()
        {
            _inputActions = new VisualDirectorInput();
            if (_inputActions != null)
                _inputActions.Gameplay.Next.performed += OnNextPressed;
        }

        private void OnDestroy()
        {
            _inputActions.Gameplay.Next.performed -= OnNextPressed;
            _inputActions.Dispose();
        }

        public void OnChoiceButtonPressed(int buttonId) => _choiveTcs?.TrySetResult(buttonId);
        private void OnNextPressed(InputAction.CallbackContext _) => _nextTcs?.TrySetResult(true);
        private void OnEnable() => _inputActions.Enable();
        private void OnDisable() => _inputActions.Disable();

        public Task InputDetected()
        {
            if (_nextTcs == null || _nextTcs.Task.IsCompleted)
                _nextTcs = new TaskCompletionSource<bool>();
            return _nextTcs.Task;
        }

        public Task ChoiceDetected()
        {
            if (_choiveTcs == null || _choiveTcs.Task.IsCompleted)
                _choiveTcs = new TaskCompletionSource<int>();
            return _choiveTcs.Task;
        }
    }
}
