﻿namespace Grove.Gameplay.Abilities
{
  using System.Collections.Generic;
  using System.Linq;
  using Infrastructure;
  using Modifiers;

  [Copyable]
  public class SimpleAbilities : IStaticAbilities, IAcceptsCardModifier, IHashable
  {
    private readonly TrackableList<SimpleAbility> _abilities;
    private INotifyChangeTracker _changeTracker = new ChangeTrackerInitializationGuard();

    public SimpleAbilities(IEnumerable<Static> simpleAbilities)
    {
      _abilities = new TrackableList<SimpleAbility>(
        simpleAbilities.Select(x => new SimpleAbility(x)));      
    }

    private SimpleAbilities() {}

    public void Accept(ICardModifier modifier)
    {
      modifier.Apply(this);
    }

    public int CalculateHash(HashCalculator calc)
    {
      return calc.Calculate(_abilities);
    }

    public bool Deathtouch { get { return Has(Static.Deathtouch); } }
    public bool Defender { get { return Has(Static.Defender); } }
    public bool Fear { get { return Has(Static.Fear); } }
    public bool Flying { get { return Has(Static.Flying); } }
    public bool Haste { get { return Has(Static.Haste); } }
    public bool Hexproof { get { return Has(Static.Hexproof); } }
    public bool Indestructible { get { return Has(Static.Indestructible); } }
    public bool Lifelink { get { return Has(Static.Lifelink); } }
    public bool Shroud { get { return Has(Static.Shroud); } }
    public bool Trample { get { return Has(Static.Trample); } }
    public bool Unblockable { get { return Has(Static.Unblockable); } }
    public bool FirstStrike { get { return Has(Static.FirstStrike); } }
    public bool DoubleStrike { get { return Has(Static.DoubleStrike); } }
    public bool Reach { get { return Has(Static.Reach); } }
    public bool Vigilance { get { return Has(Static.Vigilance); } }
    public bool Swampwalk { get { return Has(Static.Swampwalk); } }
    public bool CannotAttack { get { return Has(Static.CannotAttack); } }
    public bool CannotBlock { get { return Has(Static.CannotBlock); } }
    public bool Islandwalk { get { return Has(Static.Islandwalk); } }
    public bool Mountainwalk { get { return Has(Static.Mountainwalk); } }
    public bool AssignsDamageAsThoughItWasntBlocked { get { return Has(Static.AssignsDamageAsThoughItWasntBlocked); } }
    public bool CanAttackOnlyIfDefenderHasIslands { get { return Has(Static.CanAttackOnlyIfDefenderHasIslands); } }
    public bool UnblockableIfDedenderHasArtifacts { get { return Has(Static.UnblockableIfDedenderHasArtifacts); } }

    public bool Flash { get { return Has(Static.Flash); } }
    public bool AttacksEachTurnIfAble { get { return Has(Static.AttacksEachTurnIfAble); } }
    public bool Forestwalk { get { return Has(Static.Forestwalk); } }

    public bool DoesNotUntap { get { return Has(Static.DoesNotUntap); } }

    public bool AnyEvadingAbility
    {
      get
      {
        return Fear || Flying || Trample || Unblockable || AssignsDamageAsThoughItWasntBlocked ||
          CanOnlyBeBlockedByCreaturesWithFlying || Swampwalk || Mountainwalk || Islandwalk;
      }
    }

    public bool CanOnlyBeBlockedByCreaturesWithFlying { get { return Has(Static.CanOnlyBeBlockedByCreaturesWithFlying); } }

    public bool Has(Static ability)
    {
      return _abilities.Any(x => x.IsEnabled && x.Value == ability);
    }

    public void Initialize(INotifyChangeTracker changeTracker, IHashDependancy hashDependancy)
    {
      _changeTracker = changeTracker;
      _abilities.Initialize(changeTracker, hashDependancy);

      foreach (var staticAbility in _abilities)
      {
        staticAbility.Initialize(changeTracker);
      }
    }

    public void Add(Static ability)
    {
      var simpleAbility = new SimpleAbility(ability);
      simpleAbility.Initialize(_changeTracker);
      _abilities.Add(simpleAbility);
    }

    public bool Remove(Static ability)
    {
      var staticAbility = _abilities
        .Where(x => x.Value == ability)
        .OrderBy(x => x.IsEnabled ? 0 : 1)
        .FirstOrDefault();

      if (staticAbility == null)
      {
        return false;
      }

      _abilities.Remove(staticAbility);
      return true;
    }

    public void Disable()
    {
      foreach (var staticAbility in _abilities)
      {
        staticAbility.Disable();
      }
    }

    public void Disable(Static ability)
    {
      var abilities = _abilities.Where(x => x.Value == ability);

      foreach (var simpleAbility in abilities)
      {
        simpleAbility.Disable();
      }
    }

    public void Enable(Static ability)
    {
      var abilities = _abilities.Where(x => x.Value == ability);

      foreach (var simpleAbility in abilities)
      {
        simpleAbility.Enable();
      }
    }

    public void Enable()
    {
      foreach (var staticAbility in _abilities)
      {
        staticAbility.Enable();
      }
    }
  }
}