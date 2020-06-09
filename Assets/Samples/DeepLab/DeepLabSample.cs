using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TensorFlowLite;
using System.Runtime.InteropServices;
//using UnityEngine.UIElements;

public class DeepLabSample : MonoBehaviour
{
    [SerializeField] string fileName = "deeplabv3_257_mv_gpu.tflite";
    [SerializeField] RawImage cameraView = null;
    [SerializeField] RawImage outputView = null;
    [SerializeField] ComputeShader compute = null;
    [SerializeField] Texture2D backTex = null;

    WebCamTexture webcamTexture;
    DeepLab deepLab;
    private Button logo; 
    void Start()
    {

        string path = Path.Combine(Application.streamingAssetsPath, fileName);
        deepLab = new DeepLab(path, compute);

        // Init camera
        string cameraName = WebCamUtil.FindName();
        webcamTexture = new WebCamTexture(cameraName, Screen.width, Screen.height, 30);
        webcamTexture.Play();
        cameraView.texture = webcamTexture;

        //Open Webpage on button click
        logo = GameObject.Find("Logo").GetComponent<Button>();
        logo.onClick.AddListener(webpage);
    }

    public void webpage()
    {
        Application.OpenURL("https://www.aimage.in");

    }
    void OnDestroy()
    {
        webcamTexture?.Stop();
        deepLab?.Dispose();
    }

    void Update()
    {

        var resizeOptions = deepLab.ResizeOptions;
        resizeOptions.rotationDegree = webcamTexture.videoRotationAngle;
        resizeOptions.flipY = false;
        deepLab.ResizeOptions = resizeOptions;

        deepLab.Invoke(webcamTexture);
        outputView.texture = deepLab.GetResultTexture2D();
        var pix = backTex.GetPixels32();
        System.Array.Reverse(pix, 0, pix.Length);
        outputView.texture = deepLab.GetResultImage2D(pix);

        cameraView.material = deepLab.transformMat;
    }

}
