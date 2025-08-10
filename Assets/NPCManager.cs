using UnityEngine;

public class NPCManager : MonoBehaviour
{
    public GameObject npc;
    public Transform spawnPoint1;
    public Transform spawnPoint2;
    public GameObject triggerZone2;
   
    
    void Start()
    {
        
        npc.transform.position = spawnPoint1.position;
        npc.SetActive(true);

        if (triggerZone2 != null)
            triggerZone2.SetActive(false);
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M)) 
        {
            MoveNPCToSecondPosition();
        }
    }

    public void MoveNPCToSecondPosition()
    {
        npc.transform.position = spawnPoint2.position;
        Debug.Log("NPC moved to position: " + npc.transform.position);

       
        npc.SetActive(false);

       
        if (triggerZone2 != null)
            triggerZone2.SetActive(true);
    }
}