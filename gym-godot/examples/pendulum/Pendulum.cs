

using System;
using Godot;

public partial class Pendulum : RigidBody3D
{
    public void apply_torque_LOCAL(float strength) {
        ApplyTorqueImpulse(new Vector3(strength, 0, 0));
    }

    public float get_angle() {
        Vector3 pendulum_vec = GetNode<MeshInstance3D>("Cylinder2").GlobalTransform.Origin -
                                    GetNode<MeshInstance3D>("Cylinder1").GlobalTransform.Origin;
        float angle = new Vector3(0, 1, 0).AngleTo(pendulum_vec);
        angle = angle * Math.Sign(GetNode<MeshInstance3D>("Cylinder2").GlobalTransform.Origin.Z);
        return angle;
    }
}