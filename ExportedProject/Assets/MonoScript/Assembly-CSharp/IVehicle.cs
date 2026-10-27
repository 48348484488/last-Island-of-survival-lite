using UnityEngine;
using gs.battle.scmsg;

public interface IVehicle
{
	float GetFuel();

	void SetFuel(float fuel);

	int GetSpeed();

	void SyncEngineSound(float volume);

	void KillOrStartEngine();

	int GetEngineSoundId();

	void OnDamage(float damage, Vector3 pos);

	Transform[] GetAttachPoints();

	bool IsBroken();

	float GetMaxFuel();

	void SetNeedSync(bool needSync);

	void OnSStopVehicleSound(SStopVehicleSound sStopVehicleSound);

	void SetHp(int hp);

	int GetHp();

	int GetMaxHp();

	void OnHit(float relativeSpeed);

	void PlayHornSound();

	void SetAllEffectVisible(bool visible);
}
