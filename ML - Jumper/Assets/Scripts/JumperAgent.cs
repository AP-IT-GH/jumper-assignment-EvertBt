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

    public float jumpPower = 1;

    private bool _collisionWithObstacle;

    public override void OnEpisodeBegin()
    {
        this._collisionWithObstacle = false;
        // Reset obstacle location and speed
        Obstacle.localPosition = new Vector3(-6, 0.5f, 0);
        speedMultiplier = Random.Range(0.10f, 0.15f);
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
        
        // Jump
        if (this.transform.localPosition.y <= 0.18f && jumpSignal > 0.5f)
        {
            Jump(jumpSignal);
        }

        // Obstacle falls from map
        if (Obstacle.localPosition.x > 24)
        {
            SetReward(1.0f);
            EndEpisode();
        }
        else if (_collisionWithObstacle)
        {
            AddReward(-0.5f);
            EndEpisode();
        }        
    }

    private void Jump(float jumpSignal)
    {
        while (this.transform.localPosition.y <= 0.30f)
        {
            this.rb.AddForce(0, jumpSignal * jumpPower, 0);
        }
    }
}