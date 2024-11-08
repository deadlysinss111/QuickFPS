using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class FollowPlayer : MonoBehaviour
{
    private NavMeshAgent agent;
    private GameObject player;
    public float stopDistance = 2f;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        Follow();
    }
    public void Follow()
    {
        // Calculer la distance entre l'ennemi et le joueur
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        // Si la distance est sup�rieure � `stopDistance`, poursuivre le joueur
        if (distanceToPlayer > stopDistance)
        {
            agent.SetDestination(player.transform.position);
        }
        else
        {
            // Arr�ter de suivre le joueur si la distance est inf�rieure ou �gale � `stopDistance`
            agent.ResetPath(); // Annule la destination actuelle pour arr�ter l'agent
        }
    }
}
