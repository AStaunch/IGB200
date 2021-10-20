using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SpriteManager;

public class MapManager : MonoBehaviour
{
    public Camera mapCamera;
    public GameObject mapScreen;
    // Start is called before the first frame update
    RoomData[] AllRooms;
    private bool isLoaded;

    void Start()
    {
        AllRooms = FindObjectsOfType<RoomData>();
        UnloadMenu();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C)) {
            if (isLoaded) {
                UnloadMenu();
            } else {
                LoadMenu();
            }
        }
    }

    void UnloadMenu() {
        Time.timeScale = 1;
        isLoaded = false;
        mapCamera.enabled = isLoaded;
        mapScreen.SetActive(isLoaded);
        foreach (RoomData Room in AllRooms) {
            Room.gameObject.layer = 2;
            Room.spriteRenderer.enabled = isLoaded;
            if (Room.Icon)  Room.Icon.SetActive(isLoaded);
        }

    }

    void LoadMenu() {
        Time.timeScale = 0;
        isLoaded = true;
        mapCamera.enabled = isLoaded;
        mapScreen.SetActive(isLoaded);
        foreach (RoomData Room in AllRooms) {
            Room.gameObject.layer = 5;
            Room.spriteRenderer.enabled = isLoaded;
            if (Room.isLoaded_) {
                Room.spriteRenderer.color = Color.white;
            } else if (Room.hasVisited_) {
                Room.spriteRenderer.color = Color.red;
            } else {
                Room.spriteRenderer.color = Color.green;
            }

            if(Room.hasVisited_ && Room.hasChest_ && Room.Icon) {
                Room.Icon.SetActive(isLoaded);
                Room.Icon.GetComponent<SpriteRenderer>().sprite = SpriteDict["ChestSprites"][0];
            } else {

            }
        }
    }
}
