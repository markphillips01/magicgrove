﻿namespace Grove.Core
{
  using System.Linq;
  using Infrastructure;
  using Messages;

  [Copyable]
  public class Blocker : IHashable
  {
    private readonly Publisher _publisher;
    private readonly TrackableList<Damage> _assignedDamage;
    private readonly Trackable<Attacker> _attacker;
    private readonly Trackable<int> _damageAssignmentOrder;

    private Blocker() {}

    private Blocker(Card card, Attacker attacker, ChangeTracker changeTracker, Publisher publisher)
    {      
      Card = card;
      _attacker = new Trackable<Attacker>(attacker, changeTracker);
      _assignedDamage = new TrackableList<Damage>(changeTracker);
      _damageAssignmentOrder = new Trackable<int>(changeTracker);      
      _publisher = publisher;
    }

    public Attacker Attacker { get { return _attacker.Value; } private set { _attacker.Value = value; } }
    public Card Card { get; private set; }
    public Player Controller { get { return Card.Controller; } }

    public int DamageAssignmentOrder { get { return _damageAssignmentOrder.Value; } set { _damageAssignmentOrder.Value = value; } }

    public bool HasAssignedLeathalDamage
    {
      get
      {
        return Card.HasLeathalDamage ||
          _assignedDamage.Sum(x => x.Amount) + Card.Damage >= Card.Toughness ||
            _assignedDamage.Any(x => x.IsLeathal);
      }
    }

    private bool HasAttacker { get { return Attacker != null; } }

    public int LifepointsLeft { get { return Card.LifepointsLeft; } }
    public int Score { get { return Ai.ScoreCalculator.CalculatePermanentScore(Card); } }
    public int TotalDamageThisCanDeal { get { return Card.Power.Value; } }
    public int Toughness { get { return Card.Toughness.Value; } }

    public int CalculateHash(HashCalculator hashCalculator)
    {
      return hashCalculator.Calculate(
        Card,
        DamageAssignmentOrder,
        _assignedDamage);
    }

    public void AssignDamage(int amount)
    {
      var damage = new Damage(Attacker.Card, amount);
      _assignedDamage.Add(damage);
    }

    public void ClearAssignedDamage()
    {
      _assignedDamage.Clear();
    }

    public void DealAssignedDamage()
    {
      foreach (var damage in _assignedDamage)
      {
        Card.DealDamage(damage.Source, damage.Amount, isCombat: true);
      }

      ClearAssignedDamage();
    }

    public void DistributeDamageToAttacker()
    {
      if (Attacker != null)
        Attacker.AssignDamage(Card, TotalDamageThisCanDeal);
    }

    public void RemoveAttacker()
    {
      Attacker = null;
    }

    public void RemoveFromCombat()
    {
      _publisher.Publish(new RemovedFromCombat {Card = Card});

      if (HasAttacker)
      {
        Attacker.RemoveBlocker(this);
        Attacker = null;
      }      
    }

    [Copyable]
    public class Factory : IBlockerFactory
    {
      private readonly ChangeTracker _changeTracker;
      private readonly Publisher _publisher;

      private Factory() {}

      public Factory(ChangeTracker changeTracker, Publisher publisher)
      {
        _changeTracker = changeTracker;
        _publisher = publisher;
      }

      public Blocker Create(Card blocker, Attacker attacker)
      {
        return new Blocker(blocker, attacker, _changeTracker, _publisher);
      }
    }
  }
}