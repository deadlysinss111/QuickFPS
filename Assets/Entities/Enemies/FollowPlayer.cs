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

        // Si la distance est supérieure à `stopDistance`, poursuivre le joueur
        if (distanceToPlayer > stopDistance)
        {
            agent.SetDestination(player.transform.position);
        }
        else
        {
            // Arrêter de suivre le joueur si la distance est inférieure ou égale à `stopDistance`
            agent.ResetPath(); // Annule la destination actuelle pour arrêter l'agent
        }
    }
}
