using Fusion;
using UnityEngine;

/**
 * This component represents a ball moving at a constant speed.
 */
public class Ball: NetworkBehaviour {
    [Networked] private TickTimer lifeTimer { get; set; }

    [SerializeField] float lifeTime = 5.0f;
    [SerializeField] float speed = 5.0f;

    public override void Spawned() {
        lifeTimer = TickTimer.CreateFromSeconds(Runner, lifeTime);
    }

    public override void FixedUpdateNetwork() {
        //Debug.Log(lifeTimer);
        if (lifeTimer.Expired(Runner))
            Runner.Despawn(Object);
        else
            transform.position += speed * transform.forward * Runner.DeltaTime;
    }
}