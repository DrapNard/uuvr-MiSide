using System;
using BepInEx.Configuration;
using UnityEngine;

namespace Uuvr
{
    public class UuvrBehaviour : MonoBehaviour
    {
        /// <summary>
        /// Factory method to create an instance of the behavior and attach it to a new GameObject.
        /// </summary>
        /// <typeparam name="T">Type of the behavior to create.</typeparam>
        /// <param name="parent">Parent transform to attach the new GameObject.</param>
        /// <returns>An instance of the created behavior.</returns>
        public static T Create<T>(Transform parent) where T : UuvrBehaviour
        {
            return new GameObject(typeof(T).Name)
            {
                transform =
                {
                    parent = parent,
                    localPosition = Vector3.zero,
                    localRotation = Quaternion.identity
                }
            }.AddComponent<T>();
        }

        /// <summary>
        /// Unity's Awake method. Can be overridden by derived classes.
        /// </summary>
        protected virtual void Awake()
        {
            // Custom initialization logic for derived classes
            Debug.Log($"{GetType().Name}: Awake called.");
        }

        /// <summary>
        /// Called when the component is enabled. Subscribes to events.
        /// </summary>
        protected virtual void OnEnable()
        {
            try
            {
                // Subscribe to the BeforeRender event
                Application.onBeforeRender += OnBeforeRender;
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"Failed to register for BeforeRender: {exception.Message}");
            }

            // Subscribe to configuration changes
            ModConfiguration.Instance.Config.SettingChanged += ConfigOnSettingChanged;
        }

        /// <summary>
        /// Called when the component is disabled. Unsubscribes from events.
        /// </summary>
        protected virtual void OnDisable()
        {
            try
            {
                // Unsubscribe from the BeforeRender event
                Application.onBeforeRender -= OnBeforeRender;
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"Failed to unregister from BeforeRender: {exception.Message}");
            }

            // Unsubscribe from configuration changes
            ModConfiguration.Instance.Config.SettingChanged -= ConfigOnSettingChanged;
        }

        /// <summary>
        /// Event handler for configuration setting changes.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">Event arguments for the setting change.</param>
        private void ConfigOnSettingChanged(object? sender, SettingChangedEventArgs e)
        {
            OnSettingChanged();
        }

        /// <summary>
        /// Handles logic to execute before the render event.
        /// </summary>
        protected virtual void OnBeforeRender()
        {
            // Example: Sync object transform with a camera's transform
            if (Camera.main != null)
            {
                var mainCameraTransform = Camera.main.transform;
                transform.position = mainCameraTransform.position;
                transform.rotation = mainCameraTransform.rotation;

                Debug.Log($"{GetType().Name}: OnBeforeRender - Updated object transform to sync with the main camera.");
            }
            else
            {
                Debug.LogWarning($"{GetType().Name}: OnBeforeRender - No main camera available.");
            }
        }

        /// <summary>
        /// Handles logic to execute when a configuration setting changes.
        /// </summary>
        protected virtual void OnSettingChanged()
        {
            // Dynamically update the scale based on a setting
            var configEntry = ModConfiguration.Instance.Config.Bind<float>(
                "Settings",
                "ObjectScale",
                1f,
                "Scale of the object controlled by this component."
            );

            if (configEntry != null)
            {
                transform.localScale = Vector3.one * configEntry.Value;
                Debug.Log($"{GetType().Name}: OnSettingChanged - Applied new scale based on setting: {configEntry.Value}");
            }
            else
            {
                Debug.LogWarning($"{GetType().Name}: OnSettingChanged - Config entry is null or missing.");
            }
        }
    }
}
