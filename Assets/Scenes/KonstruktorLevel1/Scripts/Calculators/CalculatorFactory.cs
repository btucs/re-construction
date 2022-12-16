#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathUnits.Physics.Values;

public enum CalculatorEnum {
  None,
  AngleOnXAxisCalculator,
  CosineLawCalculator,
  ForceCalculator,
  PythagorasCalculator,
  TotalForceCalculator,
  WeightForceCalculator,

  HypothenuseAlphaCalculator,

  VectorCalculator,
  DrawForceCalculator,

  ReplacementModelCalculator,
  DrawEquilibriumCalculator,
  LineVolatilityCalculator,
  DrawInteractionCalculator,
}

public class CalculatorFactory {

  public AngleOnXAxisCalculator CreateAngleOnXAxisCalculator() {

    return new AngleOnXAxisCalculator();
  }

  public CosineLawCalculator CreateConsineLawCalculator() {

    return new CosineLawCalculator();
  }

  public ForceCalculator CreateForceCalculator() {

    return new ForceCalculator();
  }

  public PythagorasCalculator CreatePythagorasCalculator() {

    return new PythagorasCalculator();
  }

  public TotalForceCalculator CreateTotalForceCalculator() {

    return new TotalForceCalculator();
  }

  public WeightForceCalculator CreateWeightForceCalculator() {

    return new WeightForceCalculator();
  }

  public ForceSplittingCalculator CreateForceSplittingCalculator(DirectionEnum direction) {

    return new ForceSplittingCalculator(direction);
  }

  public ResultingForceCalculator CreateResultingForceCalculator(DirectionEnum direction) {

    return new ResultingForceCalculator(direction);
  }

  public VectorCalculator CreateVectorCalculator() {

    return new VectorCalculator();
  }

  public HypotenuseAlphaCalculator CreateHypotenuseAlphaCalculator() {

    return new HypotenuseAlphaCalculator();
  }

  public DrawForceCalculator CreateDrawForceCalculator() {

    return new DrawForceCalculator();
  }

  public ReplacementModelCalculator CreateReplacementModelCalculator() {

    return new ReplacementModelCalculator();
  }

  public DrawEquilibriumCalculator CreateDrawEquilibriumCalculator() {

    return new DrawEquilibriumCalculator();
  }

  public LineVolatilityCalculator CreateLineVolatilityCalculator() {

    return new LineVolatilityCalculator();
  }

  public DrawInteractionCalculator CreateDrawInteractionCalculator() {

    return new DrawInteractionCalculator();
  }

  public CalculatorAbstract<ScalarValue> GetScalarCalculator(CalculatorEnum calculator) {

    switch(calculator) {

      case CalculatorEnum.AngleOnXAxisCalculator:
        return CreateAngleOnXAxisCalculator();

      case CalculatorEnum.CosineLawCalculator:
        return CreateConsineLawCalculator();

      case CalculatorEnum.ForceCalculator:
        return CreateForceCalculator();

      case CalculatorEnum.PythagorasCalculator:
        return CreatePythagorasCalculator();

      case CalculatorEnum.TotalForceCalculator:
        return CreateTotalForceCalculator();

      case CalculatorEnum.WeightForceCalculator:
        return CreateWeightForceCalculator();


      case CalculatorEnum.HypothenuseAlphaCalculator:
        return CreateHypotenuseAlphaCalculator();

      default:
        return null;
    }
  }

  public CalculatorAbstract<VectorValue> GetVectorCalculator(CalculatorEnum calculator, float scale) {

    switch(calculator) {

      case CalculatorEnum.VectorCalculator:
        return CreateVectorCalculator();

      case CalculatorEnum.DrawForceCalculator:
        DrawForceCalculator drawForceCalculator =  CreateDrawForceCalculator();
        drawForceCalculator.scale = scale;

        return drawForceCalculator;

      case CalculatorEnum.DrawEquilibriumCalculator:
        return CreateDrawEquilibriumCalculator();

      case CalculatorEnum.LineVolatilityCalculator:
        return CreateLineVolatilityCalculator();

      case CalculatorEnum.DrawInteractionCalculator:
        return CreateDrawInteractionCalculator();

      default:
        return null;
    }
  }

  public AbstractNonCalculatableCalculator GetNonCalculatableCalculator(CalculatorEnum calculator) {

    switch(calculator) {

      case CalculatorEnum.ReplacementModelCalculator:
        return CreateReplacementModelCalculator();

      default:
        return null;
    }
  }
}
