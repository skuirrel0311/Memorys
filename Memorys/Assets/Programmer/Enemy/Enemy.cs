using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    GameObject player;

    void Start()
    {
        player = GameObject.Find("Player");

        GetComponent<NavMeshAgent>().SetDestination(player.transform.position);

        StartCoroutine("TracePlayer");
    }

    IEnumerator TracePlayer()
    {
        while(true)
        {
            GetComponent<NavMeshAgent>().SetDestination(player.transform.position);
            yield return new WaitForSeconds(5);
        }
    }


    void Update()
    {

    }
}
