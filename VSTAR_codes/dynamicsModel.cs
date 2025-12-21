using System.Linq;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;
using System.Numerics;
using System.Threading.Tasks.Dataflow;
using System.Runtime.CompilerServices;

Start();

new int mass_dry = 13486; // kg
new int mass_prop = 12414 ; // kg


Quaternion q = TransformBlock.rotation; // grab initial quaternions
Vector3 w = rigidbody.angularVelocity;  // grab initial angular velocity
Vector3 tau = Vector3.zero; // initialize torque vector [need to figure this out when we have controllers setup]

Vector3 p = TransformBlock.position; // grab initial position
Vector3 v = rigidbody.velocity; // grab initial velocity




//create the initial rotational state vector, feed each element in individually because q is a struct and w is vector3
double[,] rot =
{
    {q.x},
    {q.y},
    {q.z},
    {q.w},
    {w.x},
    {w.y},
    {w.z}
};

FixedUpdate();

// perform rotation update
if (Input.GetKeyDown(KeyCode.Q))
{
    tau.X += 100; // apply torque about body x-axis
}

if (Input.GetKeyDown(KeyCode.W))
{
    tau.Y += 100; // apply torque about body y-axis
}

if (Input.GetKeyDown(KeyCode.E))
{
    tau.Z += 100; // apply torque about body z-axis
}
if (Input.GetKeyDown(KeyCode.A))
{
    tau.X -= 100; // apply torque about body x-axis
}

if (Input.GetKeyDown(KeyCode.S))
{
    tau.Y -= 100; // apply torque about body y-axis
}

if (Input.GetKeyDown(KeyCode.D))
{
    tau.Z -= 100; // apply torque about body z-axis
}

RotateOrion3 Spin = new RotateOrion3();
rot = Spin.Rotate3(rot, tau); 

