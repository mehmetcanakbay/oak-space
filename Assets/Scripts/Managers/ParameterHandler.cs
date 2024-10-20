using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework.Constraints;
using Unity.VisualScripting;
using UnityEngine;

public class BlackHoleSceneParameterInfo {
    public int gridSizeX;
    public int gridSizeY;
    public int gridSizeZ;

    public float nodeSize;
    public float nodeOffset;

    public double distMultiplier;
    public double distortionStrength;

    public double blackHoleMass;
}
//TODO:
//This class only handles BlackHole at the moment. This should be changed
//so that it can be generalized for other scenes as well. I'm time limited at the moment
//so this will stay like this at the moment. very sad :(
public class ParameterHandler : MonoBehaviour
{
    //singleton time
    public static ParameterHandler Instance;
    [SerializeField] RectTransform parameterContainer; 
    Transform[] allParameters;

    SpacetimeGrid gridComponent;
    SpacetimeSpatialDistortions distortionComponent;
    SpacetimeGridLines gridLines;

    public BlackHoleSceneParameterInfo parameterInfo;

    BlackHole blackHole;
    // Start is called before the first frame update
    void Awake() {
        Instance = this;
    }
    void Start()
    {
        GameObject grid = GameObject.FindGameObjectWithTag("Grid");
        gridComponent = grid.GetComponent<SpacetimeGrid>();
        gridLines = grid.GetComponent<SpacetimeGridLines>();
        distortionComponent = grid.GetComponent<SpacetimeSpatialDistortions>();
        
        blackHole = GameObject.FindObjectsByType<BlackHole>(FindObjectsSortMode.None)[0];

        //I could generalize this and make it better but meh no time
        parameterInfo = new BlackHoleSceneParameterInfo();
        parameterInfo.gridSizeX = gridComponent.xSize;
        parameterInfo.gridSizeY = gridComponent.ySize;
        parameterInfo.gridSizeZ = gridComponent.zSize;

        parameterInfo.nodeSize = gridComponent.nodeSizes;
        parameterInfo.nodeOffset = gridComponent.offset;

        parameterInfo.distMultiplier = distortionComponent.distMultiplier;
        parameterInfo.distortionStrength = distortionComponent.distortionStrength;

        parameterInfo.blackHoleMass = blackHole.mass;
        ParameterTextField[] fields = GameObject.FindObjectsByType<ParameterTextField>(FindObjectsSortMode.InstanceID);
        for (int i = 0; i<fields.Length;i++) {
            fields[i].SetParamValue();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Reset() {
        SetAllParameters();

        //Resetting components
        distortionComponent.Reset();
        gridLines.Reset();
        CelestialObject[] objects;
        objects = GameObject.FindObjectsByType<CelestialObject>(FindObjectsSortMode.None);
        for (int i = 0; i<objects.Length;i++) {
            objects[i].Reset();
        }
    }

    void SetAllParameters() {
        ParameterTextField[] fields = GameObject.FindObjectsByType<ParameterTextField>(FindObjectsSortMode.InstanceID);
        for (int i = 0; i<fields.Length;i++) {
            fields[i].SetParameter();
        }


        gridComponent.xSize = parameterInfo.gridSizeX ;
        gridComponent.ySize = parameterInfo.gridSizeY ;
        gridComponent.zSize = parameterInfo.gridSizeZ ;

        gridComponent.nodeSizes = parameterInfo.nodeSize ;
        gridComponent.offset =parameterInfo.nodeOffset  ;

        distortionComponent.distMultiplier = parameterInfo.distMultiplier ;
        distortionComponent.distortionStrength = parameterInfo.distortionStrength  ;

        blackHole.mass = parameterInfo.blackHoleMass ;
    }
}
