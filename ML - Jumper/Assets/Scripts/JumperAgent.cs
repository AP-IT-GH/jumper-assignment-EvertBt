using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System;
using Random = UnityEngine.Random;

public class JumperAgent : Agent
{
    public Rigidbody rb;

    public Transform Obstacle;

    public float speedMultiplier; //random each episode

    public float jumpMultiplier = 1;

    private bool _collisionWithObstacle;

    public override void OnEpisodeBegin()
    {
        Debug.Log("Begin");
        this._collisionWithObstacle = false;
        // Reset obstacle location and speed
        Obstacle.localPosition = Vector3.zero;
        speedMultiplier = Random.Range(0.1f, 0.5f);
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        // Eigen positie
        sensor.AddObservation(this.transform.localPosition);
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Obstacle")
        {
            this._collisionWithObstacle = true;
        }
    }
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Acties, size = 2
        Obstacle.transform.Translate(speedMultiplier, 0, 0);

        float jumpSignal = Math.Abs(actionBuffers.ContinuousActions[0]);

        if (this.transform.localPosition.y < 0.2f)
        {
            this.rb.AddForce(0, jumpSignal * jumpMultiplier, 0);
        }

        // Obstacle falls from map
        if (Obstacle.localPosition.x > 20)
        {
            SetReward(1.0f);
            EndEpisode();
        }

        // Hit by obstacle?
        else if (_collisionWithObstacle)
        {
            EndEpisode();
        }
    }
}