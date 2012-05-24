﻿namespace Grove.Core
{
  using System;
  using Ai;
  using Costs;
  using Effects;
  using Infrastructure;
  using Messages;

  public class ActivatedAbility : Ability
  {
    private Func<Game, Card, ActivationParameters, bool> _timming = Timings.MainPhases;

    public ActivatedAbility()
    {
      UsesStack = true;
    }

    public bool ActivateOnlyAsSorcery { get; set; }
    protected Cost Cost { get; private set; }

    public ManaAmount ManaCost
    {
      get
      {
        var manaCost = Cost as TapOwnerPayMana;

        return manaCost == null
          ? ManaAmount.Zero
          : manaCost.Amount ?? ManaAmount.Zero;
      }
    }

    protected TurnInfo Turn { get { return Game.Turn; } }

    public void Activate(ActivationParameters activation)
    {
      Cost.Pay(activation.CostTarget, activation.X);

      var effect = EffectFactory.CreateEffect(this, activation.X);
      effect.Target = activation.EffectTarget;

      Publisher.Publish(new PlayerHasActivatedAbility{
        Ability = this,
        Target = activation.EffectTarget
      });

      if (UsesStack)
      {
        Stack.Push(effect);
        return;
      }

      effect.Resolve();
    }

    public override int CalculateHash(HashCalculator hashCalculator)
    {
      return hashCalculator.Calculate(
        Cost,
        EffectFactory,
        ActivateOnlyAsSorcery
        );
    }

    public virtual SpellPrerequisites CanActivate()
    {
      int? maxX = null;
      var canActivate = CanBeActivated(ref maxX);

      return canActivate
        ? new SpellPrerequisites{
          CanBeSatisfied = true,
          Description = Text,
          EffectTargetSelector = TargetSelector,
          CostTargetSelector = Cost.TargetSelector,
          XCalculator = Cost.XCalculator,
          MaxX = maxX,
          Timming = _timming
        }
        : new SpellPrerequisites{
          CanBeSatisfied = false
        };
    }

    public T GetEffect<T>() where T : Effect
    {
      return EffectFactory.CreateEffect(this) as T;
    }

    public void SetCost(ICostFactory costFactory)
    {
      Cost = costFactory.CreateCost(this);
    }

    public void Timing(Func<Game, Card, ActivationParameters, bool> predicate)
    {
      if (predicate != null)
        _timming = predicate;
    }

    private bool CanBeActivated(ref int? maxX)
    {
      return
        CanBeActivatedAtThisTime() &&
          Cost.CanPay(ref maxX);
    }

    private bool CanBeActivatedAtThisTime()
    {
      if (ActivateOnlyAsSorcery)
      {
        return Turn.Step.IsMain() &&
          OwningCard.Controller.IsActive &&
            Stack.IsEmpty;
      }

      return true;
    }

    public class Factory<T> : IActivatedAbilityFactory where T : ActivatedAbility, new()
    {
      public Action<T> Init = delegate { };
      public Game Game { get; set; }

      public ActivatedAbility Create(Card card)
      {
        var ability = new T();
        ability.OwningCard = card;
        ability.SourceCard = card;
        ability.Game = Game;

        Init(ability);

        return ability;
      }
    }
  }
}