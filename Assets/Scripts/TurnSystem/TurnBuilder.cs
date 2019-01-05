using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnSystem {
  public class TurnBuilder {
    private List<Step> steps;
    private int maxTime;

    public TurnBuilder(int maxTime) {
      steps = new List<Step>();
      this.maxTime = maxTime;
    }

    public TurnBuilder InsertStep(int index, Step step) {
      steps.Insert(index, step);
      return this;
    }

    public TurnBuilder AddStep(Step step) {
      steps.Add(step);
      return this;
    }

    public TurnBuilder RemoveStep(int index) {
      steps.RemoveAt(index);
      return this;
    }

    public Turn Build() {
      return new Turn(steps, maxTime);
    }

    public int ExpectedTime() {
      int result = 0;
      steps.ForEach(step => result += step.ExpectedTime);
      return result;
    }

    public int ExpectedTimeStdDev() {
      int result = 0;
      steps.ForEach(step => result += step.ExpectedTimeStdDev * step.ExpectedTimeStdDev);
      return (int)Mathf.Sqrt(result);
    }
  }
}
