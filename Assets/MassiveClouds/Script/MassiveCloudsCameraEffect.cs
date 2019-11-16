#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR;

namespace Mewlist
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(MassiveClouds))]
    public class MassiveCloudsCameraEffect : MonoBehaviour
    {
        [SerializeField] private CameraEvent cameraEvent = CameraEvent.AfterSkybox;

        private CommandBuffer        commandBuffer;
        private CameraEvent          currentCameraEvent = CameraEvent.AfterSkybox;

        private Camera TargetCamera
        {
            get { return GetComponent<Camera>(); }
        }

        private MassiveClouds MassiveClouds
        {
            get { return GetComponent<MassiveClouds>(); }
        }
        
        private void SetupCamera()
        {
            currentCameraEvent = cameraEvent;
            TargetCamera.forceIntoRenderTexture = true;
            if ((TargetCamera.depthTextureMode & DepthTextureMode.Depth) == 0)
            {
                TargetCamera.depthTextureMode |= DepthTextureMode.Depth;
            }
        }

        private void Create()
        {
            Clear();
            SetupCamera();

            if (commandBuffer == null)
                commandBuffer = new CommandBuffer {name = "MassiveClouds"};
            TargetCamera.AddCommandBuffer(currentCameraEvent, commandBuffer);
        }

        private void OnPreRender()
        {
            if (commandBuffer == null) return;
            commandBuffer.Clear();
            MassiveClouds.BuildCommandBuffer(commandBuffer, BuiltinRenderTextureType.CameraTarget, BuiltinRenderTextureType.CameraTarget);            
        }

        private void Clear()
        {
            if (commandBuffer != null)
            {
                TargetCamera.RemoveCommandBuffer(currentCameraEvent, commandBuffer);
            }

            commandBuffer = null;
        }

        private void Update()
        {
            if (!MassiveClouds.enabled)
            {
                Clear();
                return;
            }
            if (commandBuffer == null) Create();
        }

        private void OnDisable()
        {
            Clear();
        }
    }
}