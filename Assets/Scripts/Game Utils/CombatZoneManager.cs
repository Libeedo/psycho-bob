using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CombatZoneManager : MonoBehaviour {
	public Wav[] waves;
	//public Seq[] sequences;
	// Use this for initialization
	void Start () {
		var cz = GetComponent<CombatZone>();
		var wCount = 0;
		foreach (Wav wav in waves) {
			var w = new Wave();
			cz.spawnWaves.Add(w);
			cz.spawnWaves[wCount].delayNextSeq = wav.delaySeq;
			foreach (Seq seq in wav.sequences) {

					var sq = new Sequence ();
					cz.spawnWaves [wCount].spawnSeqs.Add (sq);
					sq.delayNextEvent = seq.delay;
					sq.pos = seq.pos;
					for (int i=0; i<seq.amount; i ++) {
							var e = new SpawnEvent ();//sq.spawnEvents[i];
							e.enemyType = seq.eType;
							e.equipped = seq.equipped;
							e.paratrooper = seq.chute;
							Vector2 p = transform.position;
							e.offset = p + sq.pos + (seq.offset * i);
							sq.spawnEvents.Add (e);
					}
					
			}
			wCount++;
		}
	}
	

}
[System.Serializable]
public class Wav
{
	public float delaySeq;
	public List<Seq> sequences = new List<Seq> ();
}
[System.Serializable]
public class Seq
{
	public int amount;
	public Vector2 pos;//in relation to combatzone origin
	public CombatZone.EnemyType eType;
	public Enemy.Equipped equipped;
	public bool chute;
	public float delay;
	public Vector2 offset;
}
