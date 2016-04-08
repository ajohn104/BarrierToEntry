using UnityEngine;
using System.Collections;

namespace BarrierToEntry {
    public class MatchController : MonoBehaviour {


        private Team team1 = new Team();
        private Team team2 = new Team();

        public Actor[] team1Members;
        public Actor[] team2Members;

        // Use this for initialization
        void Start() {
            team1.enemyTeam = team2;
            team2.enemyTeam = team1;
            team1.SetAsMembers(team1Members);
            team2.SetAsMembers(team2Members);
        }
    }
}