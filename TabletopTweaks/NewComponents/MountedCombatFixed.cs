﻿using Kingmaker.Blueprints;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Designers;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.Utility;
using UnityEngine;
using UnityEngine.Serialization;

namespace TabletopTweaks.NewComponents {
    [TypeId("6529138e5e96494f958a758ee21e451e")]
    class MountedCombatFixed : UnitFactComponentDelegate, IRulebookHandler<RuleAttackRoll>, IGlobalRulebookHandler<RuleAttackRoll>, ISubscriber, IGlobalSubscriber {

        public BlueprintBuff CooldownBuff {
            get {
                BlueprintBuffReference cooldownBuff = this.m_CooldownBuff;
                if (cooldownBuff == null) {
                    return null;
                }
                return cooldownBuff.Get();
            }
        }

        public void OnEventAboutToTrigger(RuleAttackRoll evt) {
        }

        public void OnEventDidTrigger(RuleAttackRoll evt) {
            //Main.LogDebug("MountedCombat - Attack Roll Check");
            if (base.Fact?.Owner == null) {
                //Main.LogDebug("MountedCombat - Owner == null");
                return;
            }
            if (evt.RuleAttackWithWeapon == null) {
                //Main.LogDebug("MountedCombat - RuleAttackWithWeapon == null");
                return;
            }
            if (evt.RuleAttackWithWeapon.Target != Owner.GetSaddledUnit()) {
                //Main.LogDebug("MountedCombat - Target != Owner.GetSaddledUnit()");
                return;
            }
            if (!evt.IsHit) {
                //Main.LogDebug($"MountedCombat - evt.IsHit - {evt.IsHit}");
                return;
            }
            if (Owner.HasFact(CooldownBuff)) {
                //Main.LogDebug($"MountedCombat - hasFact - CooldownBuff");
                return;
            }
            //Main.LogDebug("MountedCombat - Triggered");
            int dc = evt.D20 + evt.AttackBonus;
            bool success = GameHelper.TriggerSkillCheck(new RuleSkillCheck(base.Owner, StatType.SkillMobility, dc), null, false).Success;
            GameHelper.ApplyBuff(Owner, CooldownBuff, new Rounds?(1.Rounds()));
            if (success) {
                evt.AutoMiss = true;
                Main.LogDebug("MountedCombat - Result");
                Main.LogDebug($"IsAutomiss: {evt.AutoMiss}");
            }
        }

        [SerializeField]
        [FormerlySerializedAs("CooldownBuff")]
        public BlueprintBuffReference m_CooldownBuff;
    }
}
