using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HotbarHandler;

public static class SharedMemory
{
    public static Dictionary<AbstractDoor, bool> AllDoors;
    public static void UpdateDoor(AbstractDoor abstractDoor) {
        if (!AllDoors.ContainsKey(abstractDoor)) {
            AllDoors.Add(abstractDoor, abstractDoor.IsOpen);
        } else {
            AllDoors[abstractDoor] = abstractDoor.IsOpen;
        }
    }

    public static Dictionary<iPhysicsInterface, float[]> AllObjects;
    public static void UpdateObject(iPhysicsInterface abstractDoor, Vector3 position) {
        if (!AllObjects.ContainsKey(abstractDoor)) {
            AllObjects.Add(abstractDoor, new float[] { position.x, position.y, position.z });
        } else {
            AllObjects[abstractDoor] = new float[] { position.x, position.y, position.z };
        }
    }

    public static PlayerData LastState;
    public static void UpdatePlayer(PlayerEntity player, CheckPoint respawnLocation) {
        LastState.Health = player.MaxHealth_;
    }

    public static void SaveToFile() {

    }
    public static void LoadFromFile() {

    }
}

public class PlayerData
{
    public int Health;
    public bool[] ElementsUnlocked = new bool[8];
    public bool[] TemplatesUnlocked = new bool[5];
    public HotbarItem[] CurrentSpells = new HotbarItem[5];
    public Vector3 SavePoint;
}
