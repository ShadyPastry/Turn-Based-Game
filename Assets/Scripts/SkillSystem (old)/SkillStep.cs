using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkillSystem {
  public abstract class SkillStep : TurnSystem.Step {
    private const float waitAfterCompletion = 0.1f;

    private bool isInitalized;
    protected ISkillUser Owner { get; private set; }
    protected int SkillLevel { get; private set; }

    public virtual void Initialize(ISkillUser owner, int skillLevel) {
      if (isInitalized) {
        throw new System.Exception("Can only initialize a SkillStep once");
      }
      isInitalized = true;
      Owner = owner;
      SkillLevel = skillLevel;
    }

    protected sealed override IEnumerator StepBehavior() {
      string failureMessage = null;
      if (!CheckActionPrerequisites(ref failureMessage)) {
        //What if the failure is because previous steps in the turn altered state?  e.g. used too much TP
        throw new System.Exception(failureMessage);
      }

      yield return SkillEffect();

      yield return new WaitForSeconds(waitAfterCompletion);
    }
    protected abstract IEnumerator SkillEffect();
  }
}