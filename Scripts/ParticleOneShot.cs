using Godot;
using System;

public partial class ParticleOneShot : GpuParticles2D
{
	public void OnParticleFinished()
	{
		QueueRedraw();
	}
}
