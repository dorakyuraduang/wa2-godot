using Godot;
using System;
using System.Diagnostics;
public struct Particle
{
	public bool Active;
	public int Type;
	public float PositionX;
	public float PositionY;
	public float SpeedScaleX;
	public float SpeedScaleY;
	public float ScaleX;
	public float ScaleY;
	public int Divergent;
	public Particle()
	{
		Active = false;
		PositionX = 0;
		PositionY = 0;
		ScaleX = 1;
		ScaleY = 1;
		Divergent = 0;
	}

}
[Tool]
public partial class Weather : Node2D

{
	float[] DriftTable = [
		0, 100, 200, 301, 401, 501, 601, 700, 799, 897, 995, 1092, 1189, 1284, 1379, 1474,
		1567, 1659, 1751, 1841, 1930, 2018, 2105, 2191, 2275, 2358, 2439, 2519, 2598, 2675, 2750, 2824,
		2896, 2966, 3034, 3101, 3166, 3229, 3289, 3348, 3405, 3460, 3513, 3563, 3612, 3658, 3702, 3744,
		3784, 3821, 3856, 3889, 3919, 3947, 3973, 3996, 4017, 4035, 4051, 4065, 4076, 4084, 4091, 4094,
		4096, 4094, 4091, 4084, 4076, 4065, 4051, 4035, 4017, 3996, 3973, 3947, 3919, 3889, 3856, 3821,
		3784, 3744, 3702, 3658, 3612, 3563, 3513, 3460, 3405, 3348, 3289, 3229, 3166, 3101, 3034, 2966,
		2896, 2824, 2750, 2675, 2598, 2519, 2439, 2358, 2275, 2191, 2105, 2018, 1930, 1841, 1751, 1659,
		1567, 1474, 1379, 1284, 1189, 1092, 995, 897, 799, 700, 601, 501, 401, 301, 200, 100,
		0,-100,-200,-301,-401,-501,-601,-700,-799,-897,-995,-1092,-1189,-1284,-1379,-1474,
		-1567,-1659,-1751,-1841,-1930,-2018,-2105,-2191,-2275,-2358,-2439,-2519,-2598,-2675,-2750,-2824,
		-2896,-2966,-3034,-3101,-3166,-3229,-3289,-3348,-3405,-3460,-3513,-3563,-3612,-3658,-3702,-3744,
		-3784,-3821,-3856,-3889,-3919,-3947,-3973,-3996,-4017,-4035,-4051,-4065,-4076,-4084,-4091,-4094,
		-4096,-4094,-4091,-4084,-4076,-4065,-4051,-4035,-4017,-3996,-3973,-3947,-3919,-3889,-3856,-3821,
		-3784,-3744,-3702,-3658,-3612,-3563,-3513,-3460,-3405,-3348,-3289,-3229,-3166,-3101,-3034,-2966,
		-2896,-2824,-2750,-2675,-2598,-2519,-2439,-2358,-2275,-2191,-2105,-2018,-1930,-1841,-1751,-1659,
		-1567,-1474,-1379,-1284,-1189,-1092,-995,-897,-799,-700,-601,-501,-401,-301,-200,-100
	   ];
	Particle[] Particles = new Particle[400];
	[Export]
	public bool emit;
	[Export]
	public int Type;
	[Export]
	public bool Spawn80PercentOnStart;
	[Export]
	public int U1;
	[Export]
	public int Frame;
	[Export]
	public int Mask;
	[Export]
	public int U3;
	[Export]
	public float CurSpeedX;
	[Export]
	public float CurSpeedY;
	[Export]
	public int CurTurbulence;
	[Export]
	public int SpeedX;
	[Export]
	public int SpeedY;
	[Export]
	public int ParticleCount;
	[Export]
	public int Turbulence;
	[Export]
	public bool IsRender;
	public int ActiveParticleCount;

	public double FrameTimer;
	public override void _Ready()
	{
		// Reset();
	}
	public override void _PhysicsProcess(double delta)
	{
		if (!emit)
		{
			return;
		}
		if (SpeedX > CurSpeedX)
		{
			CurSpeedX = Math.Min(CurSpeedX + 1, SpeedX);
		}
		else if (SpeedX < CurSpeedX)
		{
			CurSpeedX = Math.Max(CurSpeedX - 1, SpeedX);
		}
		if (SpeedY > CurSpeedY)
		{
			CurSpeedY = Math.Min(CurSpeedY + 1, SpeedY);
		}
		else if (SpeedY < CurSpeedY)
		{
			CurSpeedY = Math.Max(CurSpeedY - 1, SpeedY);
		}
		if (CurTurbulence < Turbulence)
		{
			CurTurbulence = Math.Min(CurTurbulence + 1, Turbulence);
		}
		Frame++;
		int countLimit = 0;
		switch (Type)
		{
			case 1:
				{
					// 第一次启动时：生成 80% 粒子
					if (Spawn80PercentOnStart)
					{
						Spawn80PercentOnStart = false;
						countLimit = ParticleCount * 4 / 5;

						for (int i = 0; i < countLimit; i++)
						{
							ActivateParticleType1(i, Random.Shared.Next(0, 1280), Random.Shared.Next(0, 720));
						}
					}
					else
					{
						countLimit = ParticleCount;
					}

					// 控制生成节奏：粒子数未满 && 到达生成间隔
					int interval = Math.Max(4, (int)(100.0 / Math.Max(Math.Sqrt(Frame), 1.0)));
					if (ActiveParticleCount < countLimit && Frame % interval == 0)
					{
						int index = ActiveParticleCount++;
						if (index < Particles.Length)
						{
							ActivateParticleType1(index, Random.Shared.Next(0, 1280), -40 - Random.Shared.Next(0, 100));
						}
					}
					for (int i = 0; i < Particles.Length; i++)
					{
						Particle p = Particles[i];
						if (!p.Active)
						{
							continue;
						}
						p.PositionX += DriftTable[p.Divergent % 256] * p.SpeedScaleX * 0.000244140625f + p.SpeedScaleX * CurSpeedX / 10f;
						p.PositionY += p.SpeedScaleY * CurSpeedY / 10f;
						if (p.PositionY > 720)
						{
							if (ActiveParticleCount <= i)
							{
								ActiveParticleCount--;
								p.Active = false;
							}

							p.PositionY -= 752;
						}
						if (p.PositionX >= 1280)
						{
							if (ActiveParticleCount <= i)
							{
								ActiveParticleCount--;
								p.Active = false;
							}
							p.PositionX -= 1312;
						}
						if (p.PositionX < -32)
						{
							if (ActiveParticleCount <= i)
							{
								ActiveParticleCount--;
								p.Active = false;
							}
							p.PositionX += 1312;
						}
						if (IsRender)
						{

						}
						else
						{

						}
					}

				}
				break;
			case 3:
				if (Spawn80PercentOnStart)
				{
					Spawn80PercentOnStart = false;
					countLimit = ParticleCount * 4 / 5;

					for (int i = 0; i < countLimit; i++)
					{
						ActivateParticleType3(i,1);
					}
				}
				else
				{
					countLimit = ParticleCount;
				}
				break;
		}

	}
	void ActivateParticleType1(int index, int x, int y)
	{
		var p = Particles[index];
		p.Active = true;
		p.PositionX = x;
		p.PositionY = y;
		p.Type = Random.Shared.Next(20) != 0 ? Random.Shared.Next(1, 3) : 0;
		if (p.Type == 0)
		{
			p.SpeedScaleX = 8;
			p.SpeedScaleY = 8;
		}
		else
		{
			float scaleX = (float)(Random.Shared.Next(100 * (3 - p.Type)) / 100.0 + 1.0);
			float scaleY = (float)(Random.Shared.Next(100 * (3 - p.Type)) / 100.0 + 1.0);

			p.SpeedScaleX = scaleX * 0.5f;
			p.SpeedScaleY = scaleY * 0.5f;
			p.ScaleX = scaleX;
			p.ScaleY = scaleY;
		}
		p.Divergent = (byte)Random.Shared.Next(256) / 255;
	}
	void ActivateParticleType3(int index,int v)
	{
		var p = Particles[index];
		p.Active = true;
		int v3 = Random.Shared.Next(0, 32);
		if (v3 == 0)
        {
			p.Type = Random.Shared.Next(0, 16);
			p.SpeedScaleX = (Random.Shared.Next(0, 100 * (64 - p.Type)) / 100f + 1f)*0.5f;
			p.SpeedScaleY = (Random.Shared.Next(0, 100 * (64 - p.Type)) / 100f + 1f)*0.5f;;

        }
		// var p = Particles[index];
		// p.Active = true;
		// p.PositionX = x;
		// p.PositionY = y;
		// p.Type = Random.Shared.Next(20) != 0 ? Random.Shared.Next(1, 3) : 0;
		// if (p.Type == 0)
		// {
		// 	p.SpeedScaleX = 8;
		// 	p.SpeedScaleY = 8;
		// }
		// else
		// {
		// 	float scaleX = (float)(Random.Shared.Next(100 * (3 - p.Type)) / 100.0 + 1.0);
		// 	float scaleY = (float)(Random.Shared.Next(100 * (3 - p.Type)) / 100.0 + 1.0);

		// 	p.SpeedScaleX = scaleX * 0.5f;
		// 	p.SpeedScaleY = scaleY * 0.5f;
		// 	p.ScaleX = scaleX;
		// 	p.ScaleY = scaleY;
		// }
		// p.Divergent = (byte)Random.Shared.Next(256) / 255;
	}
	// public void Reset()
	// {
	// 	FrameTimer = 1.0 / Fps;
	// 	int spawnCount = 0;
	// 	if (percent80Count)
	// 	{
	// 		for (int i = 0; i < (int)(0.8 * Count); i++)
	// 		{

	// 		}
	// 	}
	// }
}
