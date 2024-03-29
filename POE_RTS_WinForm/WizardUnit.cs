﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POE_RTS_WinForm
{
  class WizardUnit : Unit, IUnit
  {
    public WizardUnit(string aName, int aPositionX, int aPositionY, int aHealth, int aSpeed, int aAttack, string aFaction, char aSymbol)

  : base(aName, aPositionX, aPositionY, aHealth, aSpeed, aAttack, aFaction, aSymbol)
    {
      this.attackRange = 1;

      this.xPosition = aPositionX;
      this.yPosition = aPositionY;
      this.health = aHealth;
      this.maxHealth = aHealth;
      this.speed = aSpeed;
      this.attack = aAttack;
      this.faction = aFaction;
      this.symbol = aSymbol;
      this.name = aName;
    }

    public int xPos
    {
      get
      {
        return base.xPosition;
      }
      set
      {
        if (value < 0)
        {
          base.xPosition = 0;
        }
        else if (value >= Map.gridSizeX)
        {
          base.xPosition = Map.gridSizeX - 1;
        }
        else
        {
          base.xPosition = value;
        }
      }
    }
    public int yPos
    {
      get
      {
        return base.yPosition;
      }
      set
      {
        if (value < 0)
        {
          base.yPosition = 0;
        }
        else if (value >= Map.gridSizeY)
        {
          base.yPosition = Map.gridSizeY - 1;
        }
        else
        {
          base.yPosition = value;
        }
      }
    }

    public int Health
    {
      get
      {
        return base.health;
      }
      set
      {
        base.health = value;
        if (base.health < 0)
        {
          KillUnit();
        }
        if (base.health > maxHealth)
        {
          base.health = maxHealth;
        }
      }
    }
    public int MaxHealth
    {
      get
      {
        return base.maxHealth;
      }
      //no set because it shouldn't be changed unless from an expansion
    }

    public int Speed
    {
      get
      {
        if (base.speed > 0)
        {
          return base.speed;
        }
        else
        {
          return 3;
        }
      }
      set
      {
        base.speed = value;
        if (speed < 0)
        {
          speed = 1;
        }
      }
    }

    public int Attack
    {
      get
      {
        return base.attack;
      }
      set
      {
        base.attack = value;
        if (base.attack < 0)
        {
          base.attack = 0;
        }
      }
    }
    public int AttackRange
    {
      get
      {
        return 1;
      }
      set
      {
        base.attackRange = 1;
      }
    }

    public string Faction
    {
      get
      {
        return "Neutral";
      }
      set
      {
        base.faction = "Neutral";
      }
    }
    public char Symbol
    {
      get
      {
        return 'W';
      }
      set
      {
        base.symbol = 'W';
      }
    }
    public bool IsAttacking
    {
      get { return base.isAttacking; }
      set { base.isAttacking = value; }
    }

    public override void Move(Direction direction)
    {
      switch (direction)
      {
        case Direction.Up:
          this.yPos += 1;
          break;
        case Direction.Down:
          this.yPos -= 1;
          break;
        case Direction.Left:
          this.xPos -= 1;
          break;
        case Direction.Right:
          this.xPos -= 1;
          break;
        default:
          break;
      }
    }

    public override void DamageUnit(int aAttack)
    {
      this.Health = this.Health - aAttack;
    }

    public override void EngageUnit(IUnit aTarget)
    {
      isAttacking = true;
      aTarget.Health -= this.Attack;
    }

    public override bool RangeCheck(IUnit aTarget)
    {
      if (aTarget is IPosition)
      {
        var lTarget = aTarget as IPosition;

        int differenceInXPosition = Math.Abs(this.xPosition - lTarget.xPos);
        int differenceInYPosition = Math.Abs(this.yPosition - lTarget.yPos);

        if (differenceInXPosition <= attackRange && differenceInYPosition <= attackRange)
        {
          return true;
        }
        else
        {
          return false;
        }
      }
      else
      {
        throw new ArgumentException("Unit doesn't have a Position");
      }
    }

    public override IUnit FindClosestEnemy(IUnit[,] aFieldToCheck)
    {
      Unit unitFound = null;

      int rangeToCheck = 1;
      int minRange;
      int maxRange;

      while (unitFound == null)
      {
        minRange = this.xPos - rangeToCheck;
        maxRange = this.xPos + rangeToCheck;

        if (minRange < 0)
        {
          minRange = 0;
        }
        if (maxRange > Map.gridSizeX)
        {
          maxRange = Map.gridSizeX;
        }

        //Check row
        for (int i = minRange; i < maxRange; i++)
        {
          if (aFieldToCheck[i, minRange] != null)
          {
            return aFieldToCheck[i, minRange];
          }
        }
        for (int i = minRange; i < maxRange; i++)
        {
          if (aFieldToCheck[i, maxRange - 1] != null)
          {
            return aFieldToCheck[i, maxRange - 1];
          }
        }

        minRange = this.yPos - rangeToCheck;
        maxRange = yPos + rangeToCheck;

        if (minRange < 0)
        {
          minRange = 0;
        }
        if (maxRange > Map.gridSizeY)
        {
          maxRange = Map.gridSizeY;
        }

        //Check column
        for (int i = minRange; i < maxRange; i++)
        {
          if (aFieldToCheck[i, maxRange - 1] != null)
          {
            return aFieldToCheck[i, maxRange - 1];
          }
        }
        for (int i = minRange; i < maxRange; i++)
        {
          if (aFieldToCheck[i, minRange] != null)
          {
            return aFieldToCheck[i, minRange];
          }
        }
        rangeToCheck++;
      }

      return null;
    }

    public void AOEDamage(List<Unit> units)
    {
      int minX = this.xPos - 1; 
      int minY = this.yPos - 1; 
                                
      int maxX = this.xPos + 1; 
      int maxY = this.yPos + 1; 

      for (int i = minY; i < maxY; i++)
      {
        for (int j = minX; j < maxX; j++)
        {
          foreach (Unit unit in units)
          {
            if (unit is IUnit && !(unit is WizardUnit))
            {
              IUnit lUnit = unit as IUnit;
              if (lUnit.xPos == j && lUnit.yPos == i)
              {
                EngageUnit(lUnit);
              }
            }
          }
        }
      }
    }

    public override void KillUnit()
    {
      this.isDead = true;
    }

    public override string ToString()
    {
      string text;
      text = $"{name}: {Environment.NewLine}" +
             $"Position:({xPosition},{yPosition}).{Environment.NewLine}" +
             $" Health: {health}.{Environment.NewLine}" +
             $" Attack: {attack}.{Environment.NewLine}" +
             $"Attack Range: {attackRange}.{Environment.NewLine}";

      if (isAttacking)
      {
        text += $"Is attacking.{Environment.NewLine}";
      }
      else
      {
        text += $"Is not attacking. {Environment.NewLine}";
      }

      text += $"Faction: {faction}.{Environment.NewLine}" +
              $" Symbol: {symbol}.{Environment.NewLine}";

      return text;
    }

    public override void SaveToFile()
    {
      var lClassSerialisation = new ClassSerialisation<WizardUnit>();

      lClassSerialisation.SaveClass(FileName, this);
    }
  }
}
