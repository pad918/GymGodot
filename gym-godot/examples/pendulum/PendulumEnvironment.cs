using Godot;
using System;
using System.Collections.Generic;

public partial class PendulumEnvironment : Environment
{
    [Export]
    Pendulum pendulum;

    [Export]
    MeshInstance3D arrow;

    float currentAction = 0;

    float theta = 0;

    float lastTheta = 0;

    float theta_dt = 0;

    public override void ApplyAction(List<int> action)
    {
        currentAction = Clip(action[0], -2, 2);
        pendulum.apply_torque_LOCAL(currentAction);

        // Scale arrow according to torque force
        arrow.Scale = new(Math.Abs(currentAction / 2), arrow.Scale.Y, currentAction / 2);
    }

    public override float[] GetObservation()
    {
        theta = pendulum.get_angle();

        // Unwrap the angle if needed
        if (Mathf.Sign(theta) != Mathf.Sign(lastTheta))
        {
            if (theta > Math.PI / 2)
                lastTheta = lastTheta + 2 * Mathf.Pi;
            if (theta < -Math.PI / 2)
                lastTheta = lastTheta - 2 * Mathf.Pi;
        }
        // Compute angular velocity
        theta_dt = (theta - lastTheta) * 10;
        lastTheta = theta;
        return [Mathf.Cos(theta), Mathf.Sin(theta), theta_dt];
    }

    public override float GetReward()
    {
        var cost = theta * theta + 0.1 * theta_dt * theta_dt + 0.001 * currentAction * currentAction;
        return -(float)cost;
    }

    public static float RandomRange(Random random, float min, float max)
    {
        return random.NextSingle() * (max - min) + min;
    }

    public override void Reset()
    {
        var rng = new Random();
        var rand_rot = RandomRange(rng, -Mathf.Pi, Mathf.Pi);
        pendulum.Translate(new Vector3(0, 6, 0));
        pendulum.Rotation = new Vector3(rand_rot, 0, 0);
        pendulum.Translate(new Vector3(0, -6, 0));
        pendulum.LinearVelocity = new Vector3(0, 0, 0);
        pendulum.AngularVelocity = new Vector3(0, 0, 0);
        lastTheta = pendulum.get_angle();
        _PhysicsProcess(0);
    }

    static float Clip(float value, float minValue, float maxValue) {
        if (value > maxValue) value = maxValue;
        else if (value < minValue) value = minValue;
        return value;
    }

}
