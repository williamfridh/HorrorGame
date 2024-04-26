using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/*BY: ALIN
The purpose of this script is to be ran RIGHT AFTER the map is done generating, for stuff like
handling spawnign the player, the monster, etcetera. you can attach and call
other scripts from here too.
*/

public class PostMapgenScript : MonoBehaviour{
    public GameObject SaiBallTester; //can be removed just for testing
    public GameObject escapeDoor;
    public GameObject RandomMapParent;
    public RandomMapHandler randscript;
    private int mapWidth, mapHeight, roomSize, escapeX, escapeZ, searchOffset;
    string direction;
    public void Main()
    {
        mapWidth = gameObject.GetComponent<RandomMapHandler>().MapWidth;
        mapHeight = gameObject.GetComponent<RandomMapHandler>().MapHeight;
        roomSize = gameObject.GetComponent<RandomMapHandler>().RoomSize;
        searchOffset = 0;
        Boolean found = false;
        while(!found){
            //down to up on left side
            for(int z = searchOffset; z<mapHeight - searchOffset; z++){
                if(randscript.gridHandler[searchOffset, z] != RandomMapHandler.Grid.EMPTY && !found){
                    escapeX = searchOffset;
                    escapeZ = z;
                    found = true;
                    direction = "left";
                }
            }
            //left to right on top side
            for(int x = searchOffset; x<mapWidth - searchOffset; x++){
                if(randscript.gridHandler[x, mapHeight - searchOffset - 1] != RandomMapHandler.Grid.EMPTY && !found){
                    escapeX = x;
                    escapeZ = mapHeight - searchOffset - 1;
                    found = true;
                    direction = "up";
                }
            }
            //down to up on right side
            for(int z = searchOffset; z<mapHeight - searchOffset; z++){
                if(randscript.gridHandler[mapWidth - searchOffset - 1, z] != RandomMapHandler.Grid.EMPTY && !found){
                    escapeX = mapWidth - searchOffset - 1;
                    escapeZ = z;
                    found = true;
                    direction = "right";
                }
            }
            //left to right on bottom side
            for(int x = searchOffset; x<mapWidth - searchOffset; x++){
                if(randscript.gridHandler[x, searchOffset] != RandomMapHandler.Grid.EMPTY && !found){
                    escapeX = x;
                    escapeZ = searchOffset;
                    found = true;
                    direction = "down";
                }
            }
            searchOffset++;    
        }
        Debug.Log("the door coords: (" + escapeX + ", " + escapeZ + ")");
        Instantiate(SaiBallTester, new Vector3(escapeX * roomSize, 0, escapeZ * roomSize), Quaternion.identity); //this can be deleted
        if(direction == "left"){
            Instantiate(escapeDoor, new Vector3(escapeX * roomSize - roomSize/2 + 0.31f, 0, escapeZ * roomSize),
            Quaternion.Euler(0, 90, 0)); 
        }
        else if(direction == "up"){
            Instantiate(escapeDoor, new Vector3(escapeX * roomSize, 0, escapeZ * roomSize + roomSize/2 - 0.31f),
            Quaternion.Euler(0, 180, 0));
        }
        else if(direction == "right"){
            Instantiate(escapeDoor, new Vector3(escapeX * roomSize + roomSize/2 - 0.31f, 0, escapeZ * roomSize),
            Quaternion.Euler(0, -90, 0));
        }
        else if(direction == "down"){
            Instantiate(escapeDoor, new Vector3(escapeX * roomSize, 0, escapeZ * roomSize - roomSize/2 + 0.31f),
            Quaternion.identity);
        }
    }   
}