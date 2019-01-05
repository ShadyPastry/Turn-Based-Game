using System.Collections;
using UnityEngine;

namespace SkillSystem {
  public sealed class Skill<T> where T : SkillStep, new() {
    public ISkillUser Owner { get; }
    public int MaxLevel { get; }

    public int Level { get; private set; } = 0;

    public Skill(ISkillUser owner, int maxLevel) {
      Owner = owner;
      MaxLevel = maxLevel;
    }

    public void LevelUp() {
      if (Level < MaxLevel) {
        Level += 1;
      }
    }

    public T Use() {
      T result = new T();
      result.Initialize(Owner, Level);
      return result;
    }
  }

//  public abstract class Skill {
//    public ISkillUser Owner { get; }
//    public int MaxLevel { get; }
//    public int Level { get; private set; } = 0;

//    public Skill(ISkillUser owner, int maxLevel) {
//      Owner = owner;
//      MaxLevel = maxLevel;
//    }

//    public void LevelUp() {
//      if (Level < MaxLevel) {
//        Level += 1;
//      }
//    }
//  }

//  public abstract class SkillStep2 : TurnSystem.Step {
//    private static readonly float waitAfterCompletion = 0.1f;

//    private readonly int level;

//    public SkillStep2(int level) {
//      this.level = level;
//    }

//    protected sealed override IEnumerator ActionInner() {
//      string failureMessage = null;
//      if (!CheckActionPrerequisites(ref failureMessage)) {
//        throw new System.Exception(failureMessage);
//      }

//      yield return SkillEffect(level);

//      yield return new WaitForSeconds(waitAfterCompletion);
//    }
//    protected abstract IEnumerator SkillEffect(int level);
//  }

//  [System.Obsolete("Not yet implemented")]
//  public abstract partial class SkillEnum {
//    //public static readonly SkillEnum skill1 = ...;
//    //public static readonly SkillEnum skill2 = ...;

//    private static readonly float waitAfterUse = 0.1f;

//    public TurnSystem.Step Use(ISkillUser user, int skillLevel) {
//      return SkillEffect(user, skillLevel);
//    }

//    //
//    //Creating new skills
//    //

//    //When creating a new skill, add the following to the partial class in a separate file
//    //1. Define a private class, NewSkill, inheriting from SkillEnum
//    //2. Inside of NewSkill, define a private class, NewSkillStep, inheriting from SkillEnumStep
//    //3. Define NewSkill.SkillEffect to return a new instance of NewSkillStep
//    //4. Add a public static readonly instance of NewSkill

//    protected abstract SkillEnumStep SkillEffect(ISkillUser user, int skillLevel);
//    protected abstract class SkillEnumStep : TurnSystem.Step {
//      protected sealed override IEnumerator ActionInner() {
//        //If prerequisites are not met, throw an exception (they should be checked by the client before performing the step)
//        string failureMessage = null; if (!CheckActionPrerequisites(ref failureMessage)) {
//          throw new System.Exception(failureMessage);
//        }

//        //Perform skill-specific behavior
//        yield return SkillEffect();

//        //Provide a little end lag after a skill is performed
//        yield return new WaitForSeconds(waitAfterUse);
//      }
//      protected abstract IEnumerator SkillEffect(); //Skill-specific behavior
//    }
//  }
}
