using UnityEngine;
using Unity.Cinemachine;
public class CameraFocus : MonoBehaviour
{


    public CinemachineBrain Brain;
    public ICinemachineCamera CamA;
    public ICinemachineCamera CamB;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        CamA = GetComponent<CinemachineCamera>();
        CamB = GetComponent<CinemachineCamera>();



        //Override parameters
        int layer = 1; //The layer to override
        int priority = 1; //The priority of the
        float weight = 1f; //The weight of the new camera
        float blendTime = 0f; //The time it takes to blend between cameras
        Brain.SetCameraOverride(layer, priority, CamA, CamB, weight, blendTime);
    }

}
