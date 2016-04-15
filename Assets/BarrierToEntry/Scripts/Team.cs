using UnityEngine;
using System.Collections;
using System;

namespace BarrierToEntry
{
    public class Team
    {
        public static readonly Team NONE = new Team();

        private Actor[] _members = new Actor[0];
        public Actor[] members
        {
            get { return _members;  }
        }

        private Team _enemyTeam;
        public Team enemyTeam { 
            get
            {
                return _enemyTeam;
            }
            set
            {
                _enemyTeam = value;
                foreach(Actor member in members)
                {
                    member.enemyTeam = _enemyTeam;
                }
            }
        }
        
        public void SetAsMembers(Actor[] newMembers)
        {
            foreach(Actor actor in newMembers)
            {
                SetTeam(actor, this);
            }
        }
        
        public bool isSameTeam(Actor a, Actor b)
        {
            return a.team != NONE && b.team != NONE && a.team == b.team;
        }

        public static void SetTeam(Actor actor, Team team)
        {
            actor.team.RemoveMember(actor);
            team.AddMember(actor);
        }

        private void AddMember(Actor actor)
        {
            Actor[] newMembers = new Actor[_members.Length + 1];
            for(int i = 0; i < _members.Length; i++)
            {
                newMembers[i] = _members[i];
            }
            newMembers[newMembers.Length - 1] = actor;
            _members = newMembers;
            actor.team = this;
            actor.enemyTeam = this.enemyTeam;
        }

        private void RemoveMember(Actor actor)
        {
            int index = Array.IndexOf(_members, actor);
            if (index < 0) return;
            Actor[] newMembers = new Actor[_members.Length - 1];
            for(int i = 0; i < index; i++)
            {
                newMembers[i] = _members[i];
            }
            for(int j = index + 1; j < _members.Length; j++)
            {
                newMembers[j - 1] = _members[j];
            }
            _members = newMembers;
            actor.team = NONE;
            actor.enemyTeam = NONE;
        }
    }
}
