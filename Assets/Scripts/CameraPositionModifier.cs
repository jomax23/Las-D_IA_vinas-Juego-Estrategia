using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPositionModifier : MonoBehaviour
{
    [Header("OFFSET CAMARA")]
    public CameraOffset cameraOffset;

    [Header("FACTOR DE ESCALADO")]
    public float scaleFactor;

    [Header("INSTANCIA AL OBJETO GENERADOR DEL CAMPO")]
    [SerializeField]
    private FieldObstacleGeneration generatedField;

    private float resolutionFactor = 16 / 9;
    //COMPONENTE CAMARA DEL OBJETO QUE LLEVA ESTE SCRIPT
    //private Camera camera;

    //COORDENADA Z DE LA CAMARA
    private int cameraZCoord = -10;

    private Vector3 cameraPosition;

    private int greaterDim;
    // Start is called before the first frame update
    void Start()
    {
        cameraPosition = new Vector3((generatedField.generadorDelMapa.GetAnchura() / 2) + cameraOffset.xOffset, (generatedField.generadorDelMapa.GetAltura() / 2) + cameraOffset.yOffset, cameraZCoord);
        transform.position = cameraPosition;

        Camera camera = GetComponent<Camera>();


        greaterDim = Mathf.Max(generatedField.generadorDelMapa.GetAnchura(), generatedField.generadorDelMapa.GetAltura());
        //greaterDim = generatedField.generadorDelMapa.GetAltura();

        //camera.orthographicSize = (greaterDim) * scaleFactor / 2;

        camera.orthographicSize = greaterDim / (scaleFactor * resolutionFactor);
        Debug.Log(camera.orthographicSize);


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

[System.Serializable]
public class CameraOffset
{
    public float xOffset;
    public float yOffset;
}