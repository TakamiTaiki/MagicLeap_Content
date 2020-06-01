using UnityEngine;
using MagicLeap;
using UnityEngine.XR.MagicLeap;
using ML_C_Intensity = UnityEngine.XR.MagicLeap.MLInputControllerFeedbackIntensity;
using ML_C_Vibe = UnityEngine.XR.MagicLeap.MLInputControllerFeedbackPatternVibe;


public class ControllerManager : ControllerFeedbackExample
{
    private ControllerConnectionHandler controllerConnectionHandler;

    [SerializeField] Transform controllerTip;

    [SerializeField] Transform kitchen;
    [SerializeField] Transform floor;
    private MLInputController controller;

    public float a;


    void Start()
    {
        controllerConnectionHandler = GetComponent<ControllerConnectionHandler>();
        controller = controllerConnectionHandler.ConnectedController;
        MLInput.OnControllerButtonUp += HandleOnButtonUp;
        MLInput.OnControllerButtonDown += HandleOnButtonDown;
        MLInput.OnTriggerDown += HandleOnTriggerDown;
    }

    void Update()
    {
        //if (controller == null)
        //{
        //    controller = controllerConnectionHandler.ConnectedController;
        //    return;
        //}
        //float padPosY = controller.Touch1PosAndForce.y;
        //float force = controller.Touch1PosAndForce.z;

        //if (force > 0.8f)
        //{
        //    kitchen.position += Vector3.up * padPosY * a;
        //    floor.position += Vector3.up * padPosY * a;
        //}
    }

    public override void HandleOnButtonDown(byte controllerId, MLInputControllerButton button)
    {
        MLInputController controller = controllerConnectionHandler.ConnectedController;
        if (controller != null && controller.Id == controllerId && button == MLInputControllerButton.Bumper)
        {
            kitchen.position = controllerTip.position;
            Vector3 aim = kitchen.position - Camera.main.transform.position;
            aim.y = 0;
            var look = Quaternion.LookRotation(-aim);
            kitchen.localRotation = look;
        }
        else if (controller != null && controller.Id == controllerId && button == MLInputControllerButton.HomeTap)
            ;
    }

    public override void HandleOnButtonUp(byte controllerId, MLInputControllerButton button)
    {
        MLInputController controller = controllerConnectionHandler.ConnectedController;
        if (controller != null && controller.Id == controllerId &&
            button == MLInputControllerButton.Bumper)
        {
            controller.StartFeedbackPatternVibe(ML_C_Vibe.ForceUp, ML_C_Intensity.Medium);
        }
    }

    public override void HandleOnTriggerDown(byte controllerId, float value)
    {
        MLInputController controller = controllerConnectionHandler.ConnectedController;
        if (controller != null && controller.Id == controllerId)
        {
            ML_C_Intensity intensity = (ML_C_Intensity)((int)(value * 1.0f));
            controller.StartFeedbackPatternVibe(ML_C_Vibe.Buzz, intensity);

            Ray ray = new Ray(controllerTip.position, controllerTip.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 10.0f, 1 << 8))
            {
                Vector3 vec = hit.point - kitchen.position;
                vec.y = 0;
                kitchen.position += vec;

                Vector3 aim = kitchen.position - Camera.main.transform.position;
                aim.y = 0;
                var look = Quaternion.LookRotation(-aim);
                kitchen.localRotation = look;
            }
        }
    }
}