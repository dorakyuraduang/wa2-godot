using Godot;
using System;
using System.Diagnostics;
public enum ParticleFlags
{
    
}
public struct Particle
{
	public bool Active;
	public int Type;
	public int U1;
	public int U2;
	public float PositionX;
	public float PositionY;
	public int U3;
	public float SpeedScaleX;
	public float SpeedScaleY;
	public float ScaleX;
	public float ScaleY;
	public int Divergent;
	public int U4;
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
				// 第一次启动时：生成 80% 粒子
				if (Spawn80PercentOnStart)
				{
					Spawn80PercentOnStart = false;
					countLimit = ParticleCount * 4 / 5;

					for (int i = 0; i < countLimit; i++)
					{
						ActivateParticleType3(i, 1);
					}
				}
				else
				{
					countLimit = ParticleCount;
				}
				if (ActiveParticleCount < countLimit)
				{
					ActivateParticleType3(ActiveParticleCount, 1);
					ActiveParticleCount++;
				}
				if (ActiveParticleCount <= 0)
				{
					
					return;
				}
				for (int k = 0; k < 400; k++)
				{
					var p= Particles[k];
					if (!p.Active)
                    {
                        continue;
                    }
					if ((Mask & 0x800) != 0)
                    {
						//0x800估计
						float v69=0.000244140625f*DriftTable[(p.Divergent+64)%256]*p.SpeedScaleX*SpeedY;
						float v70=(p.U4-640)/(5120/(p.U1+1)-p.U3);

                    }
                    else
                    {
                        
                    }
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
	int ActivateParticleType3(int index, int dirY)
	{
		var p = Particles[index];
		p.Active = true;

		bool flag = true;
		int v3 = Random.Shared.Next(0, 32);
		p.U1=256;
		p.U2=0;

		// v3 == 0 的分支
		if (v3 == 0)
		{
			p.Type = Random.Shared.Next(0, 16);
			p.SpeedScaleX = (Random.Shared.Next(0, 100 * (64 - p.Type)) / 100f + 1f) * 0.5f;
			p.SpeedScaleY = (Random.Shared.Next(0, 100 * (64 - p.Type)) / 100f + 1f) * 0.5f;
			p.SpeedScaleX = Random.Shared.Next(0, 8) + 10;
			p.SpeedScaleY = Random.Shared.Next(0, 8) + 10;
		}

		// 处理 type = [192..239] 的分支
		if ((v3 > 0 && v3 <= 2) || (v3 == 0 && (Mask & 0x200) != 0))
		{
			p.Type = Random.Shared.Next(0, 48) + 192;

			p.SpeedScaleX = Random.Shared.Next(0, 4) + 5;
			p.SpeedScaleX = Random.Shared.Next(0, 4) + 5; // 覆盖第一次

			flag = (Mask & 0x400) != 0;
		}

		// flag 分支
		if (flag)
		{
			bool useU2Mode = (Mask & 0x800) != 0;

			p.Type = Random.Shared.Next(0, 8) + 248;

			if (useU2Mode)
			{
				p.U2 = Random.Shared.Next(0, 4) + 1;

				p.SpeedScaleX = Random.Shared.Next(0, 3) + 1;
				p.SpeedScaleY = (Random.Shared.Next(0, 3) - 2 * p.Type + 513) * 0.5f;
			}
			else
			{
				p.SpeedScaleX = (Random.Shared.Next(0, 100 * (256 - p.Type)) / 100f + 1) * 0.5f;
				p.SpeedScaleY = (Random.Shared.Next(0, 100 * (256 - p.Type)) / 100f + 1) * 0.5f;
			}
		}

		// ---------------- 位置随机计算 ----------------
		int t = p.Type;
		p.PositionX = Random.Shared.Next(0, 4 * (576 - t)) - 2 * (256 - t);
		p.U4=Random.Shared.Next(0, 4 * (576 - t)) - 2 * (256 - t);
		//根据粒子方向设置y坐标
		if (dirY > 0)
		{
			p.PositionY = Random.Shared.Next(0, 4 * (436 - t)) - 2 * (256 - t);
		}
		else
		{
			p.PositionY = -Random.Shared.Next(0, 100) - 2 * (256 - t);
		}

		U3 = p.Type;
		return Random.Shared.Next(0, 256);
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
