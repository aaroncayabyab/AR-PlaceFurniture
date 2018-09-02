using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InteractManager : MonoBehaviour {

    Spawner spawner;

    //Object variables
    public GameObject bed;
    public GameObject chair;
    public GameObject sofa;
    public GameObject table;
    public GameObject cabinet;
    public GameObject rack;


    //UI variables
    public GameObject objectSelectionPanel;

    private void Awake()
    {
        if (spawner == null)
            spawner = GetComponent<Spawner>();
    }

    public void ToggleObjectSelection()
    {
        objectSelectionPanel.SetActive(!objectSelectionPanel.activeSelf);
    }

    public void SpawnBed()
    {
        spawner.SetSpawnObject(bed);
        ToggleObjectSelection();
    }

    public void SpawnChair()
    {
        spawner.SetSpawnObject(chair);
        ToggleObjectSelection();
    }

    public void SpawnSofa()
    {
        spawner.SetSpawnObject(sofa);
        ToggleObjectSelection();
    }

    public void SpawnTable()
    {
        spawner.SetSpawnObject(table);
        ToggleObjectSelection();
    }

    public void SpawnCabinet()
    {
        spawner.SetSpawnObject(cabinet);
        ToggleObjectSelection();
    }

    public void SpawnRack()
    {
        spawner.SetSpawnObject(rack);
        ToggleObjectSelection();
    }

    public void ResetLevel()
    {
        SceneManager.LoadScene(0);
    }

}
