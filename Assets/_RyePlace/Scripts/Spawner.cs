using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.XR;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(ARSessionOrigin))]
public class Spawner : MonoBehaviour
{
    #region VARIABLES
    //Gameobjects
    public GameObject spawner;

    [SerializeField]
    private GameObject objectToSpawn; //Variable used to set selected spawned object from UI selection
    private GameObject spawnedObject; //Variable used to hold instantiated objectToSpawn


    //TEST-IN-EDITOR variables
    public GameObject ARTestPlane;
    public LayerMask ARTestLayer;

    [Space(10)]
    //UI
    public GameObject setupInstructions;
    public GameObject interactInstructions;
    public GameObject displayObjectSelection;
    public GameObject objectSelectionPanel;


    //Bool variable to handle behaviour. Used to trigger behavior once.
    bool isPlaneDetected = false;
    bool isObjectSpawned = false;

    //ARFoundation Variables
    ARSessionOrigin m_SessionOrigin;

    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    #endregion

    void Awake()
    {
        m_SessionOrigin = GetComponent<ARSessionOrigin>();


    }

    private void Start()
    {
        //Hide spawner when plane not detected
        spawner.SetActive(false);

        //Toggle UI Variables
        setupInstructions.SetActive(true);
        objectSelectionPanel.SetActive(false);
        displayObjectSelection.SetActive(false);
        interactInstructions.SetActive(false);

        //Only enable ARTestPlane when in Editor
        if (Application.isEditor)
        {
            ARTestPlane.SetActive(true);
        }
        else
        {
            ARTestPlane.SetActive(false);
        }
    }

    void Update()
    {
        //Only allow user to tap on screen that does not have UI
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if ((Input.touchCount > 0 || Input.GetMouseButton(0)) && isPlaneDetected == true)
            {
                if (spawnedObject == null && isObjectSpawned == false)
                {
                    spawnedObject = Instantiate(objectToSpawn) as GameObject;
                    ToggleSpawner(false);
                    spawnedObject.transform.position = spawner.transform.position;

                    //Display instructions when object is placed
                    StartCoroutine(DisplayInstructions());

                    isObjectSpawned = true;
                }
                else
                {
                    //Add LeanTouch Translate/Rotate components to allow spawned object to be moved and rotated along plane
                    if (spawnedObject.GetComponent<Lean.Touch.CustomLeanTranslate>() == null)
                    {
                        var customLeanTranslate = spawnedObject.AddComponent<Lean.Touch.CustomLeanTranslate>();
                        customLeanTranslate.sessionOrigin = m_SessionOrigin;
                        customLeanTranslate.hits = s_Hits;
                        customLeanTranslate.ARTestLayer = ARTestLayer;
                    }
                    if (spawnedObject.GetComponent<Lean.Touch.CustomLeanRotate>() == null)
                    {
                        spawnedObject.AddComponent<Lean.Touch.CustomLeanRotate>();
                    }
                }
            }
        }

        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Camera.main.pixelWidth * 0.5f, Camera.main.pixelHeight * 0.5f));

#if UNITY_EDITOR
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 500f, ARTestLayer) && isObjectSpawned == false)
        {
            OnPlaneDetect(hit.point);
        }
#else

        if (m_SessionOrigin.Raycast(ray, s_Hits, TrackableType.PlaneWithinInfinity) && isObjectSpawned == false)
        {
            Pose hitPose = s_Hits[0].pose;

            OnPlaneDetect(hitPose.position);
        }
#endif

    }

    void OnPlaneDetect(Vector3 hitPos)
    {
        if (isPlaneDetected == false)
        {
            setupInstructions.SetActive(false);
            objectSelectionPanel.SetActive(true);
            displayObjectSelection.SetActive(true);
            isPlaneDetected = true;
        }    


        spawner.SetActive(true);
        spawner.transform.position = hitPos;

        var cameraGroundPos = Camera.main.transform.position;
        cameraGroundPos.y = hitPos.y;

        spawner.transform.LookAt(cameraGroundPos);
    }


    //Use to display instructions over span of 5 seconds
    IEnumerator DisplayInstructions()
    {
        interactInstructions.SetActive(true);
        interactInstructions.GetComponent<Text>().text = "Tap and drag to move it around!";
        yield return new WaitForSeconds(2.5f);
        interactInstructions.GetComponent<Text>().text = "Rotate to see it from all angles!";
        yield return new WaitForSeconds(2.5f);
        interactInstructions.SetActive(false);

    }

    //Enable/Disable object positioner mesh, For use after plane detection
    void ToggleSpawner(bool value)
    {
        foreach (Transform item in spawner.transform)
        {
            item.gameObject.SetActive(value);
        }
    }


    //On object change, destroy currently spawned object if exists and re-enable object positioner
    public void SetSpawnObject(GameObject go)
    {
        if (objectToSpawn != null)
        {
            Destroy(spawnedObject);

            ToggleSpawner(true);
            isObjectSpawned = false;
        }

        objectToSpawn = go;
    }

}
