﻿namespace Grove.Core.Ai
{
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.Linq;

  public class AttackStrategy : IEnumerable<Card>
  {
    private readonly List<Card> _attackers;

    public AttackStrategy(
      int attackersLife,
      int defendersLife,
      IEnumerable<Card> attackerCandidates,
      IEnumerable<Card> blockerCandidates)
    {
      _attackers = ChooseAttackers(
        attackersLife,
        defendersLife,
        attackerCandidates.ToList(), blockerCandidates.ToList())
        .ToList();
    }

    public IEnumerator<Card> GetEnumerator()
    {
      return _attackers.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    private static IEnumerable<BlockAssignment> AssignEveryBlockerToEachAttacker(
      IEnumerable<ScoredCard> attackerCandidates,
      IEnumerable<ScoredCard> blockerCandidates,
      int defendersLife)
    {
      var assignments = new List<BlockAssignment>();

      foreach (var attackerCandidate in attackerCandidates)
      {
        var assignment = new BlockAssignment(attackerCandidate, defendersLife);
        foreach (var blockerCandidate in blockerCandidates)
        {
          assignment.TryAssignBlocker(blockerCandidate);
        }

        assignments.Add(assignment);
      }
      return assignments;
    }

    private static IEnumerable<Card> ChooseAttackers(int attackersLife, int defendersLife,
      List<Card> attackerCandidates, List<Card> blockerCandidates)
    {
      var assignments = AssignEveryBlockerToEachAttacker(
        ScoreCards(attackerCandidates),
        ScoreCards(blockerCandidates),
        defendersLife);      

      return (from blockAssignment in assignments
        where blockAssignment.Score > 0
        orderby blockAssignment.Score descending
        select blockAssignment.Attacker);
    }

    private static IEnumerable<ScoredCard> ScoreCards(IEnumerable<Card> cards)
    {
      return cards.Select(x => new ScoredCard{
        Card = x,
        Score = ScoreCalculator.CalculatePermanentScore(x)
      });
    }

    private class BlockAssignment
    {
      private readonly ScoredCard _attacker;
      private readonly List<ScoredCard> _blockers = new List<ScoredCard>();
      private readonly int _defendersLife;
      private readonly Lazy<int> _score;

      public BlockAssignment(ScoredCard attacker, int defendersLife)
      {
        _attacker = attacker;
        _defendersLife = defendersLife;
        _score = new Lazy<int>(CalculateScore);
      }

      public Card Attacker { get { return _attacker.Card; } }

      private int AttackerScore
      {
        get
        {
          if (Attacker.Has().Deathtouch)
            return 0;

          return CanAttackerBeKilled ? _attacker.Score : 0;
        }
      }

      private int BlockersScore
      {
        get
        {
          if (_blockers.Count == 0)
            return 0;
          
          if (Attacker.Has().Deathtouch)
          {            
            // attackers with deathtouch can kill every blocker
            // they can damage, they will either be blocked by 
            // lowest value creature or by creatures that
            // can kill and have lowest score
            return _blockers.Min(x => x.Score);
          }

          // approximate blocker score by sum of 
          // killed blocker scores when ordered from smaller
          // to greater scores          
          var todoDamage = Attacker.Power.Value;
          var score = 0;

          if (_blockers.Any(blocker => blocker.Card.LifepointsLeft > todoDamage))
            return 0;

          foreach (var blocker in _blockers.OrderBy(x => x.Score))
          {
            todoDamage -= blocker.Card.LifepointsLeft;
            score += blocker.Score;

            if (todoDamage <= 0)
              break;
          }

          return score;
        }
      }

      private bool CanAttackerBeKilled { get { return Attacker.LifepointsLeft <= _blockers.Sum(x => x.Card.Power.Value); } }

      private int DefendersLifeloss
      {
        get
        {
          if (_blockers.Count == 0)
            return Attacker.Power.Value;

          if (Attacker.Has().Trample)
          {
            var totalToughness = _blockers.Sum(x => x.Card.Toughness.Value);
            var diff = Attacker.Power.Value - totalToughness;

            return diff > 0 ? diff : 0;
          }

          return 0;
        }
      }

      public int Score { get { return _score.Value; } }

      public void TryAssignBlocker(ScoredCard scoredCard)
      {
        if (Attacker.CanBeBlockedBy(scoredCard.Card))
          _blockers.Add(scoredCard);
      }

      private int CalculateScore()
      {
        return BlockersScore + LifelossScore(_defendersLife) - AttackerScore;
      }

      private int LifelossScore(int defendersLife)
      {
        return ScoreCalculator.CalculateLifelossScore(defendersLife, DefendersLifeloss);
      }
    }

    private class ScoredCard
    {
      public Card Card { get; set; }
      public int Score { get; set; }
    }
  }
}