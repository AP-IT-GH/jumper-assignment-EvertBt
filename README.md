1.	Maak een scene die het volgende bevat:
    -   Plane (Floor)
    -   Cube (Agent)
    -   Cube (Obstacle)
    -   Cube (Reward obstacle)

2.	Maak volgend script aan:
    -   JumperAgent.cs

3.	Kopieer volgende code in het script:
    ```cs
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

        public GameObject Obstacle;

        public GameObject Reward;

        private Transform _obj;

        public float speedMultiplier; //random each episode

        public float jumpPower = 1;

        private bool _collisionWithObstacle;

        private bool _collisionWithReward;

        private bool _isJumping = false;

        public override void OnEpisodeBegin()
        {
            if (_obj)
            {
                Destroy(_obj.gameObject);
            }

            if (Random.Range(0,2) > 0.5f)
            {
                _obj = Instantiate(Obstacle).GetComponent<Transform>();
            }
            else
            {
                _obj = Instantiate(Reward).GetComponent<Transform>();
            }
            _obj.gameObject.SetActive(true);
            _obj.parent = this.transform.parent;

            this._collisionWithReward = false;
            this._collisionWithObstacle = false;
            this._isJumping = false;
            // Reset obstacle location and speed
            _obj.localPosition = new Vector3(-14.5f, 0.8f, 0);
            speedMultiplier = Random.Range(0.10f, 0.15f);
        }
        public override void CollectObservations(VectorSensor sensor)
        {
            // Eigen positie
            sensor.AddObservation(this.transform.localPosition);
        }

        public void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "Obstacle")
            {
                this._collisionWithObstacle = true;
            }
            else if (collision.gameObject.tag == "Reward")
            {
                this._collisionWithReward = true;
            }
        }
        public override void OnActionReceived(ActionBuffers actionBuffers)
        {
            // Acties, size = 2
            _obj.Translate(speedMultiplier, 0, 0);

            float jumpSignal = Math.Abs(actionBuffers.ContinuousActions[0]);
            
            // Jump
            if (this.transform.localPosition.y <= 0.5f && jumpSignal > 0.5f && !this._isJumping)
            {
                this._isJumping = true;
            }

            // Obstacle falls from map
            if (_obj.localPosition.x > 15)
            {
                if (_obj.gameObject.tag == "Obstacle")
                {
                    SetReward(1.0f);
                }
                EndEpisode();
            }
            else if (_collisionWithObstacle)
            {
                AddReward(-0.5f);
                EndEpisode();
            }        
            else if (_collisionWithReward)
            {
                SetReward(1.0f);
                EndEpisode();
            }
        }

        private void FixedUpdate()
        {
            if (this._isJumping && this.transform.localPosition.y <= 0.60f)
            {
                this.rb.AddForce(0, jumpPower, 0);
            }
            else
            {
                this._isJumping = false;
            }
        }
    }
    ```

4.  Voeg volgende componenten toe aan de agent:
    -   JumperAgent.cs
    -   Behavior Parameters
    -   Rigidbody
    -   Decision Requester
    -   Ray Perceptor Sensor 3D

5.	Stel deze componenten in met volgende waardes:
    -   …

6.	Zorg ervoor dat volgende tags aan de overeenkomstige objecten hangt:
    | Object          | Tag      |
    |-----------------|----------|
    | Obstacle        | Obstacle |
    | Reward obstacle | Reward   |

7.	Zet beide obstacles op “onzichtbaar”

8.	Maak van alle objecten in deze scene een Prefab

9.	(Optional) Zet meerdere prefabs in de scene om parallel te trainen

10.	Start de training
