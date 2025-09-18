

using System.Collections.Generic;
using Godot;

public abstract partial class Environment : Node3D
{

    bool isDone = false;
    float cartPosition = 0;
    float lastCartPosition = 0;
    float poleAngle = 0;
    float lastPoleAngle = 0;

    public virtual void ApplyAction(List<int> action) {

    }
    public virtual float[] GetObservation() {
        return [];
    }
    public abstract float GetReward();

    public abstract void Reset();

    public bool IsDone() {
        return isDone;
    }

    //public abstract void _physics_process(float delta);

}