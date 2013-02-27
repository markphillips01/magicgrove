﻿namespace Grove.Core.Effects
{
  using System;
  using Infrastructure;

  [Copyable]
  public interface IDynamicParameter
  {
    void EvaluateOnResolve(Effect effect, Game game);
    void EvaluateOnInit(Effect effect, Game game);
  }

  public class DynParam<TOut> : IDynamicParameter
  {
    private readonly bool _evaluateOnInit;
    private readonly bool _evaluateOnResolve;
    private readonly Func<Effect, Game, TOut> _getter;
    private bool _isInitialized;
    private TOut _value;

    public DynParam(Func<Effect, Game, TOut> getter, bool evaluateOnInit = true, bool evaluateOnResolve = false)
    {
      _getter = getter;
      _evaluateOnInit = evaluateOnInit;
      _evaluateOnResolve = evaluateOnResolve;
    }

    public DynParam(TOut value)
    {
      Value = value;
    }

    public TOut Value
    {
      get
      {
        if (!_isInitialized)
          throw new InvalidOperationException("Parameter was not initialized, did you forget to register it?");

        return _value;
      }
      private set
      {
        _value = value;
        _isInitialized = true;
      }
    }


    public void EvaluateOnResolve(Effect effect, Game game)
    {
      if (_evaluateOnResolve)
        Evaluate(effect, game);
    }

    public void EvaluateOnInit(Effect effect, Game game)
    {
      if (_evaluateOnInit)
        Evaluate(effect, game);
    }

    private void Evaluate(Effect effect, Game game)
    {
      Value = _getter(effect, game);
    }

    public static implicit operator DynParam<TOut>(Func<Effect, Game, TOut> getter)
    {
      return new DynParam<TOut>(getter);
    }

    public static implicit operator DynParam<TOut>(Func<Effect, TOut> getter)
    {
      return new DynParam<TOut>((e, g) => getter(e));
    }

    public static implicit operator DynParam<TOut>(TOut value)
    {
      return new DynParam<TOut>(value);
    }

    public static implicit operator TOut(DynParam<TOut> param)
    {
      return param.Value;
    }
  }
}