/*  Author: Salick Talhah
 *  Date Created: February 14, 2021
 *  Last Updated: February 14, 2021
 *  Usage: Drag and drop the prefab this script is attached to into the Canvas as a child. You can also just drop this prefab anywhere in the scene and it will automatically find its rightful position within Canvas.
 *  Then use SetTargetPlayer() to add a player character object that the minimap camera will follow. Turn off the Minimap Marker layer on other cameras except the Minimap camera.
 *  Description: 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapScr : MonoBehaviour
{
    [Header("Manualset References")]                                //References that have to be manually set beyond the prefab- Either by drag and drop or calling a function that assigns it.
    [SerializeField] private Transform targetPlayerRef;             //The player chracter that the minimap will follow and center on.
    [SerializeField] private Transform[] registeredEnemyRefs;
    [SerializeField] private Transform[] registeredPickupRefs;
    [SerializeField] private Transform[] registeredCheckpointRefs;
    [SerializeField] private Transform[] registeredObjectiveRefs;

    [Header("Preset References")]                                   //References that the prefab should already have OR that the script will automatically find.
    [SerializeField] private GameObject canvasContainerRef;         //Drag and Drop canvas gameobject here.
    [SerializeField] private GameObject minimapMaskRef;             //Image mask used to shape the minimap
    [SerializeField] private GameObject minimapBorderRef;
    [SerializeField] private Transform miniMapCamContainerRef;      //Container object that holds the camera for the minimap view.
    [SerializeField] private Sprite playerMinimapMarkerSprite;      //The visual marker of how a player character will appear on the minimap.
    [SerializeField] private Sprite enemyMinimapMarkerSprite;       //The visual marker of how an enemy will appear on the minimap.
    [SerializeField] private Sprite pickupMinimapMarkerSprite;      //The visual marker of how a pickup will appear on the minimap.
    [SerializeField] private Sprite checkpointMinimapMarkerSprite;  //The visual marker of how a checkpoint will appear on the minimap.
    [SerializeField] private Sprite objectiveMinimapMarkerSprite;   //The visual marker of how an objective will appear on the minimap.

    [Header("Minimap Settings")]
    [SerializeField] private float camFollowSpeed = 10;             //How fast the minimap camera will follow the player. Note: this does not utilize smooth interpolation.

    [Tooltip("The minimap size will be this value multiplied by screen height.")]
    [SerializeField] private float miniMapSizeRatio = 0.2f;
    
    [SerializeField] private float miniMapZoom = 26;                //How much ground the minimap can cover. It's like an aerial view zoom effect.
    [SerializeField] private float miniMapIconSizes = 6;   
    //[SerializeField] private float playerIconSize = 6;            //This setting was added in case our player prefab scale differs from other objects and would need custom tweaking.
    [SerializeField] private float playerIconYRotation = 0;         //This setting was added in case our player prefab forward direction differs from other objects and would need custom tweaking.
    [SerializeField] private float camOverheadDistance = 30;
    [SerializeField] private float iconOverheadHeight = 2;          //Note: Player scale affects this
    [SerializeField] private bool rotateWithPlayer = false;         //Set whether or not the minimap rotates with player oriantation
    //[SerializeField] private bool bUseCircleMask;

    private Transform initialPlayerRef;                             //Used to compare with targetPlayerRef to check if targetPlayerRef has been changed.
    private Transform initialPlayerIconRef;
    private float miniMapSize = 256;                                //How big the minimap will appear in the screen.

    private void Awake() 
    {
        miniMapSize = Screen.height * miniMapSizeRatio;
        InsureCanvasExists();               //Insures that this object is within Canvas.
        ExtractCameraToRootHierarchy();     //Extract the Minimap Camera from within the prefab and unto the root of the scene hierarchy.
        ApplyMinimapLevelIcons();
    }

    private void Update() 
    {
        FollowTargetPlayerWithCam();        //Allows the minimap camera to follow the player movements. 

        //If the target player character has been changed: Create new Minimap icon for the new player character.
        if (HasTargetPlayerChanged()) 
        {
            //Destroy previous player icon.
            if (initialPlayerIconRef) 
            {
                Destroy(initialPlayerIconRef);
            }
            //Create new Minimap icon for the new player character.
            AddMinimapMarker(targetPlayerRef, MinimapMarker.PLAYER);
        }
    }

    //Insures that this object is within Canvas.
    private void InsureCanvasExists() 
    {
        bool canvasFound = false;

        //Check if this object is already in its proper place- as a child of Canvas object. Note: MinimapScr.cs is supposed to be attached to a prefab that is meant to go in the Canvas.
        if (this.transform.parent) 
        {
            if (this.transform.parent.GetComponent<Canvas>()) 
            {
                return;
            }
        }

        //Check if Canvas exists with the name 'Canvas'.
        if (GameObject.Find("Canvas")) 
        {
            if (GameObject.Find("Canvas").GetComponent<Canvas>()) 
            {
                canvasContainerRef = GameObject.Find("Canvas");
                this.transform.SetParent(canvasContainerRef.transform);
                this.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(miniMapSize, miniMapSize);
            }
        }

        //Check if Canvas exists with alias.
        else if (GameObject.Find("HUD"))
        {
            if (GameObject.Find("HUD").GetComponent<Canvas>()) 
            {
                canvasContainerRef = GameObject.Find("HUD");
                this.transform.SetParent(canvasContainerRef.transform);
                this.transform.SetSiblingIndex(0);
                canvasFound = true;
            }
        }
        else if (GameObject.Find("HUD (Desktop)"))
        {
            if (GameObject.Find("HUD (Desktop)").GetComponent<Canvas>()) 
            {
                canvasContainerRef = GameObject.Find("HUD (Desktop)");
                this.transform.SetParent(canvasContainerRef.transform);
                this.transform.SetSiblingIndex(0);
                canvasFound = true;
            }
        }
        else if (GameObject.Find("HUD (Mobile)"))
        {
            if (GameObject.Find("HUD (Mobile)").GetComponent<Canvas>()) 
            {
                canvasContainerRef = GameObject.Find("HUD (Mobile)");
                this.transform.SetParent(canvasContainerRef.transform);
                this.transform.SetSiblingIndex(0);
                canvasFound = true;
            }
        }

        //If Canvas object cannot be found: build one.
        if (!canvasFound) 
        {
            canvasContainerRef = new GameObject();
            canvasContainerRef.gameObject.AddComponent<Canvas>();
            canvasContainerRef.gameObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            canvasContainerRef.gameObject.AddComponent<CanvasScaler>();
            canvasContainerRef.gameObject.AddComponent<GraphicRaycaster>();
            canvasContainerRef.name = "Canvas";
            this.transform.SetParent(canvasContainerRef.transform);
            initialPlayerIconRef = canvasContainerRef.transform;
        }

        this.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(miniMapSize, miniMapSize);
        this.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(miniMapSize * -0.5f, miniMapSize * -0.5f);
        minimapMaskRef.GetComponent<RectTransform>().sizeDelta = new Vector2(miniMapSize - 3, miniMapSize - 3);
        minimapBorderRef.GetComponent<RectTransform>().sizeDelta = new Vector2(miniMapSize, miniMapSize);
    }

    //Extract the Minimap Camera from within the prefab and unto the root of the scene hierarchy.
    private void ExtractCameraToRootHierarchy() 
    {
        if (!miniMapCamContainerRef) 
        {
            Debug.LogError("[Error] miniMapCamContainerRef missing! Aborting operation...");
            return;
        }

        miniMapCamContainerRef.SetParent(null);

        if (miniMapCamContainerRef.GetChild(0).GetComponent<Camera>()) 
        {
            miniMapCamContainerRef.GetChild(0).GetComponent<Camera>().orthographicSize = miniMapZoom;
        }
        else 
        {
            Debug.LogError("[Error] Could not find Minimap Camera!");
        }

        if (targetPlayerRef) 
        {
            miniMapCamContainerRef.position = new Vector3(targetPlayerRef.transform.position.x, targetPlayerRef.transform.position.y + camOverheadDistance, targetPlayerRef.transform.position.z);
        }
    }

    //Allows the minimap camera to follow the player movements. 
    //Note: This is elected over simply having the camera as a child of the target player character to avoid potential issues of this object being destroyed along with the player character.
    private void FollowTargetPlayerWithCam() 
    {
        if (!targetPlayerRef) 
        {
            return;
        }

        if (!miniMapCamContainerRef) 
        {
            Debug.LogError("[Error] miniMapCamContainerRef missing! Aborting operation...");
            return;
        }

        //If Minimap Camera is too far from the player: teleport the camera to the player.
        //TODO remove. This part is only applicable in development stages; built games cannot drag and drop into the inspector during runtime.
        if (Vector2.Distance(new Vector2(miniMapCamContainerRef.position.x, miniMapCamContainerRef.position.z), new Vector2(targetPlayerRef.position.x, targetPlayerRef.position.z)) > 10 ||
            miniMapCamContainerRef.position.y < targetPlayerRef.position.y ||
            Vector3.Distance(miniMapCamContainerRef.position, targetPlayerRef.position) > (10 + camOverheadDistance)) 
        {
            miniMapCamContainerRef.position = new Vector3(targetPlayerRef.position.x, targetPlayerRef.position.y + camOverheadDistance, targetPlayerRef.position.z);
            return;
        }

        //Set minimap Camera to follow the player
        miniMapCamContainerRef.position = Vector3.MoveTowards(miniMapCamContainerRef.position, new Vector3(targetPlayerRef.position.x, targetPlayerRef.position.y + camOverheadDistance, targetPlayerRef.position.z), camFollowSpeed * Time.deltaTime);
        
        if (rotateWithPlayer) 
        {
            miniMapCamContainerRef.eulerAngles = new Vector3(0, targetPlayerRef.localEulerAngles.y + playerIconYRotation, 0);
        }

    }

    //Checks if the targetPlayerRef has changed since last checked.
    private bool HasTargetPlayerChanged() 
    {
        if (targetPlayerRef == initialPlayerRef) 
        {
            return false;
        }

        initialPlayerRef = targetPlayerRef;
        return true;
    }

    //Creates an icon that will represent the given target object on the minimap
    //Note: This marker object should only be seen by the minimap camera; turn off the Minimap Marker layer on other cameras.
    private void AddMinimapMarker(Transform _targetObj, MinimapMarker _markerType) 
    {
        GameObject minimapMarker;

        //Temp marker type check. Will need to be modified as more marker types are accomodated.
        if (!(_markerType == MinimapMarker.PLAYER || _markerType == MinimapMarker.ENEMY ||
              _markerType == MinimapMarker.PICKUP || _markerType == MinimapMarker.CHECKPOINT ||
              _markerType == MinimapMarker.OBJECTIVE)) {
            Debug.LogError("[Error] Invalid minimap marker type; Aborting operation...");
            return;
        }

        minimapMarker = new GameObject();
        minimapMarker.name = "Minimap Icon";
        minimapMarker.layer = LayerMask.NameToLayer("Minimap Marker");
        minimapMarker.AddComponent<SpriteRenderer>();

        switch (_markerType) 
        {
            case MinimapMarker.NONE:
                return;
            case MinimapMarker.PLAYER:
                if (playerMinimapMarkerSprite) 
                {
                    minimapMarker.transform.localScale = new Vector3(miniMapIconSizes, miniMapIconSizes, 1); 
                    minimapMarker.transform.position = new Vector3(_targetObj.position.x, _targetObj.position.y + iconOverheadHeight, _targetObj.position.z);
                    minimapMarker.transform.localEulerAngles = new Vector3(90, _targetObj.localEulerAngles.y, _targetObj.localEulerAngles.z);
                    minimapMarker.transform.SetParent(_targetObj);
                    minimapMarker.GetComponent<SpriteRenderer>().sprite = playerMinimapMarkerSprite;

                    //Custom settings
                    //minimapMarker.transform.localScale = new Vector3(playerIconSize, playerIconSize, 1);
                    if (playerIconYRotation != 0) 
                    {
                        minimapMarker.transform.localEulerAngles = new Vector3(90, playerIconYRotation, 0);
                    }
                }
                else 
                {
                    Debug.LogError("[Error] Player minimap marker material reference missing!");
                }
                break;
            case MinimapMarker.ENEMY:
                if (enemyMinimapMarkerSprite) 
                {
                    minimapMarker.transform.localScale = new Vector3(miniMapIconSizes, miniMapIconSizes, 1); 
                    minimapMarker.transform.position = new Vector3(_targetObj.position.x, _targetObj.position.y + iconOverheadHeight - 1, _targetObj.position.z);
                    minimapMarker.transform.localEulerAngles = new Vector3(90, _targetObj.localEulerAngles.y, _targetObj.localEulerAngles.z);
                    minimapMarker.transform.SetParent(_targetObj);
                    minimapMarker.GetComponent<SpriteRenderer>().sprite = enemyMinimapMarkerSprite;
                }
                else 
                {
                    Debug.LogError("[Error] Enemy minimap marker material reference missing!");
                }
                break;
            case MinimapMarker.PICKUP:
                if (pickupMinimapMarkerSprite) 
                {
                    minimapMarker.transform.localScale = new Vector3(miniMapIconSizes * 0.8f, miniMapIconSizes * 0.8f, 1); 
                    minimapMarker.transform.position = new Vector3(_targetObj.position.x, _targetObj.position.y + iconOverheadHeight - 2, _targetObj.position.z);
                    minimapMarker.transform.localEulerAngles = new Vector3(90, 0, 0);
                    minimapMarker.transform.SetParent(_targetObj);
                    minimapMarker.GetComponent<SpriteRenderer>().sprite = pickupMinimapMarkerSprite;
                }
                else 
                {
                    Debug.LogError("[Error] Pickup minimap marker material reference missing!");
                }
                break;
            case MinimapMarker.CHECKPOINT:
                if (checkpointMinimapMarkerSprite) 
                {
                    minimapMarker.transform.localScale = new Vector3(miniMapIconSizes, miniMapIconSizes, 1); 
                    minimapMarker.transform.position = new Vector3(_targetObj.position.x, _targetObj.position.y + iconOverheadHeight - 2, _targetObj.position.z);
                    minimapMarker.transform.localEulerAngles = new Vector3(90, 0, 0);
                    minimapMarker.transform.SetParent(_targetObj);
                    minimapMarker.GetComponent<SpriteRenderer>().sprite = checkpointMinimapMarkerSprite;
                }
                else 
                {
                    Debug.LogError("[Error] Checkpoint minimap marker material reference missing!");
                }
                break;
            case MinimapMarker.OBJECTIVE:
                if (objectiveMinimapMarkerSprite) 
                {
                    minimapMarker.transform.localScale = new Vector3(miniMapIconSizes, miniMapIconSizes, 1); 
                    minimapMarker.transform.position = new Vector3(_targetObj.position.x, _targetObj.position.y + iconOverheadHeight - 2, _targetObj.position.z);
                    minimapMarker.transform.localEulerAngles = new Vector3(90, 0, 0);
                    minimapMarker.transform.SetParent(_targetObj);
                    minimapMarker.GetComponent<SpriteRenderer>().sprite = objectiveMinimapMarkerSprite;
                }
                else 
                {
                    Debug.LogError("[Error] Objective minimap marker material reference missing!");
                }
                break;
            default:
                Debug.LogError("[Error] Invalid minimap marker type; Aborting operation...");
                return;
        }

    }

    //Apply minimap icons on player, enemy, pickups, and checkpoints
    private void ApplyMinimapLevelIcons() 
    {

        //Check if player exists, but doesn't have a minimap icon. If so: add icon.
        if (targetPlayerRef && !initialPlayerIconRef) 
        {
            HasTargetPlayerChanged();
            AddMinimapMarker(targetPlayerRef, MinimapMarker.PLAYER);
        }

        //Check if there are any enemies registered to be tracked on the minimap. If so: add icon(s).
        if (registeredEnemyRefs.Length > 0)
        {
            for (int enemyIndex = 0; enemyIndex < registeredEnemyRefs.Length;  enemyIndex++) 
            {
                if (registeredEnemyRefs[enemyIndex]) 
                {
                    AddMinimapMarker(registeredEnemyRefs[enemyIndex], MinimapMarker.ENEMY);
                }
            }
        }

        //Check if there are any pickups registered to be tracked on the minimap. If so: add icon(s).
        if (registeredPickupRefs.Length > 0) 
        {
            for (int pickupIndex = 0; pickupIndex < registeredPickupRefs.Length;  pickupIndex++) 
            {
                if (registeredPickupRefs[pickupIndex]) 
                {
                    AddMinimapMarker(registeredPickupRefs[pickupIndex], MinimapMarker.PICKUP);
                }
            }
        }

        //Check if there are any checkpoints registered to be tracked on the minimap. If so: add icon(s).
        if (registeredCheckpointRefs.Length > 0) 
        {
            for (int checkpointIndex = 0; checkpointIndex < registeredCheckpointRefs.Length;  checkpointIndex++) 
            {
                if (registeredCheckpointRefs[checkpointIndex]) 
                {
                    AddMinimapMarker(registeredCheckpointRefs[checkpointIndex], MinimapMarker.CHECKPOINT);
                }
            }
        }

        //Check if there are any checkpoints registered to be tracked on the minimap. If so: add icon(s).
        if (registeredObjectiveRefs.Length > 0) 
        {
            for (int objectiveIndex = 0; objectiveIndex < registeredObjectiveRefs.Length;  objectiveIndex++) 
            {
                if (registeredObjectiveRefs[objectiveIndex]) 
                {
                    AddMinimapMarker(registeredObjectiveRefs[objectiveIndex], MinimapMarker.OBJECTIVE);
                }
            }
        }
    }

    //Set new player character that the minimap cam will follow.
    public void SetTargetPlayer(Transform _insertPlayer) 
    {
        targetPlayerRef = _insertPlayer;
        miniMapCamContainerRef.position = new Vector3(targetPlayerRef.transform.position.x, targetPlayerRef.transform.position.y + 10, targetPlayerRef.transform.position.z);

        //If the target player character has been changed: Create new Minimap icon for the new player character.
        if (HasTargetPlayerChanged()) 
        {
            AddMinimapMarker(targetPlayerRef, MinimapMarker.PLAYER);
        }

        miniMapCamContainerRef.position = new Vector3(targetPlayerRef.transform.position.x, targetPlayerRef.transform.position.y + 30, targetPlayerRef.transform.position.z);
    }

    //Toggle whether or not the minimap is visible
    public void ToggleMiniMapVisibility() 
    {
        //If minimapMaskRef cannot be found, try to find it. If it still cannot be found return with error log.
        if (!minimapMaskRef) 
        {
            if (this.transform.GetChild(0)) 
            {
                if (this.transform.GetChild(0).gameObject.GetComponent<Mask>()) 
                {
                    minimapMaskRef = this.transform.GetChild(0).gameObject;
                }
            }
            if (!minimapMaskRef)
            { 
                Debug.LogError("[Error] Minimap mask reference missing! Aborting operation...");
                return;
            }
        }

        if (minimapMaskRef.activeSelf) 
        {
            minimapMaskRef.SetActive(false); //Turn off Minimap visual 
        }
        else 
        {
            minimapMaskRef.SetActive(true);  //Turn on Minimap visual
        }
    }

    //Set whether or not the minimap is visible.
    public void SetMiniMapVisibility(bool _set) 
    {
        //If minimapMaskRef cannot be found, try to find it. If it still cannot be found return with error log.
        if (!minimapMaskRef) 
        {
            if (this.transform.GetChild(0)) 
            {
                if (this.transform.GetChild(0).gameObject.GetComponent<Mask>()) 
                {
                    minimapMaskRef = this.transform.GetChild(0).gameObject;
                }
            }
            if (!minimapMaskRef)
            { 
                Debug.LogError("[Error] Minimap mask reference missing! Aborting operation...");
                return;
            }
        }

        //Set Minimap visual
        minimapMaskRef.SetActive(_set);
    }

    //Adjust how big the minimap will appear in the screen.
    public void SetMinimapSize(float _newMiniMapSize) 
    {
        if (!minimapMaskRef) 
        {
            Debug.LogError("[Error] minimapMaskRef missing! Aborting operation...");
            return;
        }

        if (_newMiniMapSize <= 0) 
        {
            Debug.LogError("[Error] Invalid minimap size! Aborting operation...");
            return;
        }

        miniMapSize = _newMiniMapSize;
        this.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(miniMapSize, miniMapSize);
        this.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(miniMapSize * -0.5f, miniMapSize * -0.5f);
        minimapMaskRef.GetComponent<RectTransform>().sizeDelta = new Vector2(miniMapSize, miniMapSize);
    }

    //Adjust how much ground the minimap can cover. Like an aerial view zoom effect.
    public void SetMinimapZoom(float _newZoomAmount) 
    {
        if (!miniMapCamContainerRef) 
        {
            Debug.LogError("[Error] miniMapCamContainerRef missing! Aborting operation...");
            return;
        }

        if (!miniMapCamContainerRef.GetChild(0).GetComponent<Camera>()) 
        {
            Debug.LogError("[Error] Could not find Minimap Camera! Aborting operation...");
            return;
        }

        if (_newZoomAmount <= 0) 
        {
            Debug.LogError("[Error] Invalid camera zoom amount! Aborting operation...");
            return;
        }

        miniMapZoom = _newZoomAmount;
        miniMapCamContainerRef.GetChild(0).GetComponent<Camera>().orthographicSize = miniMapZoom;
    }

    //Adjust the size of icons in the minimap
    public void SetIconSize(float _newMiniMapIconSize) 
    {
        if (!initialPlayerIconRef) 
        {
            return;
        }

        if (_newMiniMapIconSize <= 0) 
        {
            Debug.LogError("[Error] Invalid icon size! Aborting operation...");
            return;
        }

        miniMapIconSizes = _newMiniMapIconSize;
        initialPlayerIconRef.localScale = new Vector3(miniMapIconSizes, miniMapIconSizes, 1);

        //TODO adjust other icons here
    }

}
