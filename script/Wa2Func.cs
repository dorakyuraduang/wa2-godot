using Godot;
using System;
using System.Collections.Generic;
[GlobalClass]
public partial class Wa2Func : Node
{
	public static Dictionary<int, Action> FuncDic = new Dictionary<int, Action>{
		{0x80,printEx},
		{0x81,printEx2},
		{0x82,SetMessage},
		{0x83,SetMessageE},
		{0x84,EndMessage},
		{0x85,SetMessage2},
		{0x86,WaitMessage2},
		{0x87,K},
		{0x88,SetDemoMode},
		{0x89,VI},
		{0x8a,VV},
		{0x8b,VX},
		{0x8c,VW},
		{0x8d,VS},
		{0x8e,W},
		{0x8f,WR},
		{0x90,WN},
		{0x91,WNS},
		{0x92,B},
		{0x93,BC},
		{0x94,V},
		{0x95,H},
		{0x96,SetShake},
		{0x97,StopShake},
		{0x98,F},
		{0x99,FB},
		{0x9a,C},
		{0x9b,CW},
		{0x9c,CR},
		{0x9d,CRW},
		{0x9e,M},
		{0x9f,MS},
		{0xa0,MP},
		{0xa1,MV},
		{0xa2,MW},
		{0xa3,MLW},
		{0xa4,SE},
		{0xa5,SEP},
		{0xa6,SES},
		{0xa7,SEV},
		{0xa8,SEW},
		{0xa9,SEVW},
		{0xaa,SetTimeMode},
		{0xab,SetChromaMode},
		{0xac,SetEffctMode},
		{0xad,SetWeather},
		{0xae,ChangeWeather},
		{0xaf,ResetWeather},
		{0xb0,LoadBmp},
		{0xb1,LoadBmpAnime},
		{0xb2,SetBmpAvi},
		{0xb3,WaitBmpAvi},
		{0xb4,ReleaseBmp},
		{0xb5,WaitBmpAnime},
		{0xb6,SetBmpAnimePlay},
		{0xb7,SetBmpDisp},
		{0xb8,SetBmpLayer},
		{0xb9,SetBmpParam},
		{0xba,SetBmpRevParam},
		{0xbb,SetBmpBright},
		{0xbc,SetBmpMove},
		{0xbd,SetBmpPos},
		{0xbe,SetBmpZoom},
		{0xbf,SetBmpZoom2},
		{0xc0,SetBmpRoll},
		{0xc1,SetMovie},
		{0xc2,Wait},
		{0xc3,StartTimer},
		{0xc4,WaitTimer},
		{0xc5,GoTitle},
		{0xc6,GetGameFlag},
		{0xc7,SetGameFlag},
		{0xc8,LogOut},
		{0xc9,V_Flag},
		{0xca,H_Flag},
		{0xcb,Calender},
		{0xcc,GetTimer},
		{0xcd,GetSkip},
		{0xce,GetClick},
		{0xcf,runEX},
		{0xd0,SetSelectMess},
		{0xd1,SetSelect},
		{0xd2,S},
		{0xd3,Z},
		{0xd4,R},
		{0xd5,WSZ},
		{0xd6,StopSZR},
		{0xd7,VA},
		{0xd8,CS},
		{0xd9,CM},
		{0xda,CRS},
		{0xdb,SkipOFF},
		{0xdc,NevelMode},
		{0xdd,EroMode},
		{0xde,GetReplayMode},
		{0xdf,WN2},
		{0xe0,WNS2},
		{0xe1,B2},
		{0xe2,BC2},
		{0xe3,V2},
		{0xe4,H2},
		{0xe5,SetWeather2},
		{0xe6,ChangeWeather2},
		{0xe7,SetShake2},
		{0xe8,M2},
		{0xe9,NB},
		{0xea,NBR},
		{0xeb,VXV},
		{0xec,Wait2},
		{0xed,IfSkip},
		{0xee,GetSkip2},

	};
	public static void PlayBgm(int id, bool loopFlag = true, int volume = 255)
	{
		Wa2SoundMgr.Instance.PlayBgm(id, loopFlag, volume);
	}
	public static void StopBgm(int frame = 0)
	{
		Wa2SoundMgr.Instance.StopBgm(frame);
	}
	public static void PlaySysSe(AudioStream stream)
	{
		Wa2SoundMgr.Instance.PlaySysSe(stream);
	}
	public static void LoadScript(string name, uint pos = 0)
	{
		Wa2Script.LoadScript(name, pos);
	}
	public static void printEx() { }
	public static void printEx2() { }
	public static void SetMessage() { }
	public static void SetMessageE() { }
	public static void EndMessage() { }
	public static void SetMessage2() { }
	public static void WaitMessage2() { }
	public static void K() { }
	public static void SetDemoMode() { }
	public static void VI() { }
	public static void VV() { }
	public static void VX() { }
	public static void VW() { }
	public static void VS() { }
	public static void W() { }
	public static void WR() { }
	public static void WN() { }
	public static void WNS() { }
	public static void B() { }
	public static void BC() { }
	public static void V() { }
	public static void H() { }
	public static void SetShake() { }
	public static void StopShake() { }
	public static void F() { }
	public static void FB() { }
	public static void C() { }
	public static void CW() { }
	public static void CR() { }
	public static void CRW() { }
	public static void M() { }
	public static void MS() { }
	public static void MP() { }
	public static void MV() { }
	public static void MW() { }
	public static void MLW() { }
	public static void SE() { }
	public static void SEP() { }
	public static void SES() { }
	public static void SEV() { }
	public static void SEW() { }
	public static void SEVW() { }
	public static void SetTimeMode() { }
	public static void SetChromaMode() { }
	public static void SetEffctMode() { }
	public static void SetWeather() { }
	public static void ChangeWeather() { }
	public static void ResetWeather() { }
	public static void LoadBmp() { }
	public static void LoadBmpAnime() { }
	public static void SetBmpAvi() { }
	public static void WaitBmpAvi() { }
	public static void ReleaseBmp() { }
	public static void WaitBmpAnime() { }
	public static void SetBmpAnimePlay() { }
	public static void SetBmpDisp() { }
	public static void SetBmpLayer() { }
	public static void SetBmpParam() { }
	public static void SetBmpRevParam() { }
	public static void SetBmpBright() { }
	public static void SetBmpMove() { }
	public static void SetBmpPos() { }
	public static void SetBmpZoom() { }
	public static void SetBmpZoom2() { }
	public static void SetBmpRoll() { }
	public static void SetMovie() { }
	public static void Wait() { }
	public static void StartTimer() { }
	public static void WaitTimer() { }
	public static void GoTitle() { }
	public static void GetGameFlag() { }
	public static void SetGameFlag() { }
	public static void LogOut() { }
	public static void V_Flag() { }
	public static void H_Flag() { }
	public static void Calender() { }
	public static void GetTimer() { }
	public static void GetSkip() { }
	public static void GetClick() { }
	public static void runEX() { }
	public static void SetSelectMess() { }
	public static void SetSelect() { }
	public static void S() { }
	public static void Z() { }
	public static void R() { }
	public static void WSZ() { }
	public static void StopSZR() { }
	public static void VA() { }
	public static void CS() { }
	public static void CM() { }
	public static void CRS() { }
	public static void SkipOFF() { }
	public static void NevelMode() { }
	public static void EroMode() { }
	public static void GetReplayMode() { }
	public static void WN2() { }
	public static void WNS2() { }
	public static void B2() { }
	public static void BC2() { }
	public static void V2() { }
	public static void H2() { }
	public static void SetWeather2() { }
	public static void ChangeWeather2() { }
	public static void SetShake2() { }
	public static void M2() { }
	public static void NB() { }
	public static void NBR() { }
	public static void VXV() { }
	public static void Wait2() { }
	public static void IfSkip() { }
	public static void GetSkip2() { }
}
