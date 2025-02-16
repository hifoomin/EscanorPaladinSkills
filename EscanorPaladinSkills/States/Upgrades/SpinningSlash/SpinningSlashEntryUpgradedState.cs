/*
using EntityStates;
using PaladinMod.States;
using System;
using System.Collections.Generic;
using System.Text;

namespace EscanorPaladinSkills.States.Upgrades.SpinningSlash
{
    public class SpinningSlashEntryUpgradedState : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();

            if (isAuthority)
            {
                EntityState nextState = new SpinningSlashAirborneUpgradedState();
                if (characterMotor.isGrounded) nextState = new SpinningSlashGroundedUpgradedState();

                outer.SetNextState(nextState);
                return;
            }
        }
    }
}
*/