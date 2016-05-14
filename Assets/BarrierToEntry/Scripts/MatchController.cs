using UnityEngine;
using System.Collections;

namespace BarrierToEntry {
    public class MatchController : MonoBehaviour {


        private Team team1 = new Team();
        private Team team2 = new Team();

        public Actor[] team1Members;
        public Actor[] team2Members;

        public GameObject AIPrefab;
        public GameObject PlayerPrefab;
        
        public GameObject Team1SpawnArea;
        public GameObject Team2SpawnArea;

        // Use this for initialization
        void Start() {
            SpawnTeams(5, 20);
            team1.enemyTeam = team2;
            team2.enemyTeam = team1;
            team1.SetAsMembers(team1Members);
            team2.SetAsMembers(team2Members);
            team1.SetTeamColor(ModelGenerator.BEAM_YODA);
            team2.SetTeamColor(ModelGenerator.BEAM_RED);
        }

        void SpawnTeams(int team1Amount, int team2Amount)
        {
            team1Members = new Actor[team1Amount];
            team2Members = new Actor[team2Amount];
            GameObject playerObject = Object.Instantiate(PlayerPrefab);
            Player player = playerObject.GetComponentInChildren<Player>();
            playerObject.transform.position = RandomInSpawn(Team1SpawnArea);
            playerObject.transform.LookAt(Team2SpawnArea.transform);
            team1Members[0] = player;
            for (int i = 1; i < team1Amount; i++)
            {
                GameObject aiObject = Object.Instantiate(AIPrefab);
                NPC npc = aiObject.GetComponentInChildren<NPC>();
                aiObject.transform.position = RandomInSpawn(Team1SpawnArea);
                aiObject.transform.LookAt(Team2SpawnArea.transform);
                team1Members[i] = npc;
                aiObject.name = "NPC Team 1 #" + i;
            }

            for (int j = 0; j < team2Amount; j++)
            {
                GameObject aiObject = Object.Instantiate(AIPrefab);
                NPC npc = aiObject.GetComponentInChildren<NPC>();
                aiObject.transform.position = RandomInSpawn(Team2SpawnArea);
                aiObject.transform.LookAt(Team1SpawnArea.transform);
                team2Members[j] = npc;
                aiObject.name = "NPC Team 2 #" + j;
            }
        }

        Vector3 RandomInSpawn(GameObject teamSpawn)
        {
            Vector3 cornerOne = Vector3.Scale(teamSpawn.transform.localScale / 2, new Vector3(1, 1, -1)) + teamSpawn.transform.position;
            Vector3 cornerTwo = Vector3.Scale(teamSpawn.transform.localScale / 2, new Vector3(-1, -1, 1)) + teamSpawn.transform.position;
            return new Vector3(Random.Range(cornerOne.x, cornerTwo.x), Mathf.Min(cornerOne.y,cornerTwo.y) ,Random.Range(cornerOne.z, cornerTwo.z));
        }
    }
}