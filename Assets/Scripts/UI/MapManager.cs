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
    public static bool isActive;
    public static bool isOnlyMenu => !(HotbarHandler.isActive || PauseMenu.isActive || DialogueManager.isActive);

    void Start()
    {
        AllRooms = FindObjectsOfType<RoomData>();
        UnloadMenu();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && isOnlyMenu) {
            if (isActive) {
                UnloadMenu();
            } else {
                LoadMenu();
            }
        }
        if(Input.GetKeyDown(KeyCode.Escape) && isOnlyMenu) {
            UnloadMenu();
        }
    }

    void UnloadMenu() {
        Time.timeScale = 1;
        isActive = false;
        mapCamera.enabled = isActive;
        mapScreen.SetActive(isActive);
        foreach (RoomData Room in AllRooms) {
            Room.gameObject.layer = 2;
            Room.fill.SetActive(isActive);
            Room.spriteRenderer.enabled = isActive;
            if (Room.Icon)  Room.Icon.SetActive(isActive);
        }

    }
    void LoadMenu() {
        Time.timeScale = 0;
        isActive = true;
        mapCamera.enabled = isActive;
        mapScreen.SetActive(isActive);
        foreach (RoomData Room in AllRooms) {
            
            Room.gameObject.layer = 5;
            Room.spriteRenderer.enabled = isActive;
            Room.fill.SetActive(isActive);
            if (Room == RoomData.MostRecentlyLoaded) {
                Room.fill.GetComponent<SpriteRenderer>().color = Color.white;
            } else if (Room.hasVisited) {
                Room.fill.GetComponent<SpriteRenderer>().color = Color.red;
            } else {
                Room.fill.GetComponent<SpriteRenderer>().color = Color.green; 
            }

            if(Room.hasChest && Room.Icon) {
                Room.Icon.SetActive(isActive);
                if (Room.hasVisited) {
                    Room.Icon.GetComponent<SpriteRenderer>().sprite = SpriteDict["ChestSprites"][1];
                } else {
                    Room.Icon.GetComponent<SpriteRenderer>().sprite = SpriteDict["ChestSprites"][0];
                }                
            } else if (Room.hasFire && Room.Icon) {
                Room.Icon.SetActive(isActive);
                if (Room.bonfire.currentState_) {
                    Room.Icon.GetComponent<SpriteRenderer>().sprite = SpriteDict["BonfireSprites"][1];
                } else {
                    Room.Icon.GetComponent<SpriteRenderer>().sprite = SpriteDict["BonfireSprites"][0];
                }
            } else {

            }
        }
    }
}
