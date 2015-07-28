using UnityEngine;
using System.Collections;

public class CombatZoneManager : MonoBehaviour {
	public Seq[] sequences;
	// Use this for initialization
	void Start () {
		var cz = GetComponent<CombatZone>();
		var count = 0;

		foreach (Seq seq in sequences){

			var sq = new Sequence();
			cz.spawnWaves[0].spawnSeqs.Add (sq);
			sq.delayNextEvent = seq.delay;
			sq.pos = seq.pos;
			for(int i=0; i<seq.amount; i ++){
				var e = new SpawnEvent();//sq.spawnEvents[i];
				e.enemyType = seq.eType;
				e.equipped = seq.equipped;
				e.paratrooper = seq.chute;
				e.offset = seq.offset;
				sq.spawnEvents.Add(e);
			}
			count++;
		}
	}
	

}
[System.Serializable]
public class Seq
{
	public int amount;
	public Vector2 pos;
	public CombatZone.EnemyType eType;
	public Enemy.Equipped equipped;
	public bool chute;
	public float delay;
	public Vector2 offset;
}
