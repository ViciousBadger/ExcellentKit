using JetBrains.Annotations;

namespace ExcellentKit
{
    /// <summary>
    /// Emits Activate on the Start message and Deactivate on Destroy.
    /// </summary>
    public class SignalOnExist : SingleTrackingSignalEmitter
    {
        [UsedImplicitly]
        private void Start()
        {
            ActivateTracked();
        }

        [UsedImplicitly]
        private void OnDestroy()
        {
            // When a scene is being unloaded OnDestroy() will be called for all objects
            // but sending signals at this point would propably break a lot of things.
            // So, to avoid stupid bugs we check if the scene is loaded first.
            if (gameObject.scene.isLoaded)
            {
                DeactivateTracked();
            }
        }
    }
}
