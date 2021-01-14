using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Pylos
{
  public class BoardAndStages
  {
    private readonly Stage [] _tabStages;
    private readonly ArrayList _listBalls;
    private readonly int _nbStages;

    public ArrayList ListBalls => _listBalls;

    private BoardUI _board;

    public BoardAndStages(int nbS)
    {
      _nbStages = nbS;
      _listBalls = new ArrayList();
      _tabStages = new Stage[_nbStages+1];
      Stage below = new Board(_nbStages+1);
      _tabStages[_nbStages] = below;
      for(var i=_nbStages ; i>0 ; i--){
        _tabStages[i-1] = new Stage(i);
        below.Above=_tabStages[i-1];
        _tabStages[i-1].Below=below;
        below = _tabStages[i-1]; 
      }
      below.Above=null;
      _board = GameObject.Find("Board").GetComponent<BoardUI>();
    }

    public Stage GetStage(int k){return _tabStages[k];}

    public void Display()
    {
      for(int i=0;i<_nbStages ; i++)
      {
        _tabStages[i].DisplayStage();
        Console.BackgroundColor = ConsoleColor.Black;
        Console.Write(Environment.NewLine);
        Console.WriteLine("Etage suivant");
      }
    }  
  

    public void PlaceBallBoard(Position position, Player p)
    {
      if (!_tabStages[position.Stage].PlayIsPossible(position.I, position.J)) return;
      Ball b = p.PlayBall();
      b.Position = position;
      _tabStages[position.Stage].AddBall(b,position.I,position.J);
      _listBalls.Add(b);
    }
    
    public void PlaceBallBoardClone(Position position, Player p)
    {
      Ball b = p.PlayBall();
      b.Position = position;
      _tabStages[position.Stage].AddBall(b,position.I,position.J);
      _listBalls.Add(b);
    }

    public void TakeBallBoard(Position position, Player p)
    {
      Console.WriteLine(_tabStages[position.Stage].CheckAbove(position.I,position.J));
      if (!_tabStages[position.Stage].CheckAbove(position.I, position.J)) return;
      Console.WriteLine("pièce retirée");
      Ball b = _tabStages[position.Stage].RemoveBall(position.I,position.J);
      b.Position = null;
      p.TakeBall(b);
      _listBalls.Remove(b);
      _board.RemoveBall(new BallData(position.I, position.J, position.Stage));
      Debug.Log("Out of takeballboard");
    }

    public List<Position> EmplacementsToPlay()
    {
      var list = new List<Position>();
      foreach(var stage in _tabStages){
        stage.AllPlaysPossibles(list, Array.IndexOf(_tabStages, stage));
      }
      return list;
    }

    public List<Position> EmplacementsToJump(Position position)
    { 
      
      var listFinale = new List<Position>();
      for(var l = position.Stage-1; l>0 ; l--){
        var list = new List<Position>();
        _tabStages[l].AllPlaysPossibles(list, l);
        //si l'étage est l'étage juste au dessus de la bille que l'on veut monter
        //on check que l'emplacement ne reposera pas sur son propre emplacement
        if (l == position.Stage - 1)
        {
          for(var i = 0; i<list.Count ; i++)
          {
            if (list[i].CheckSuperimposing(position)) listFinale.Add(list[i]);
          }
        }
        else
        {
          foreach (var variable in list)
          {
            listFinale.Add(variable);
          }
        }
        
      }
      return listFinale;
    }

    public List<Position> BallsFree(Player p)
    {
      var list = new List<Position>();
      foreach(var stage in _tabStages){
        stage.CanJump(list, Array.IndexOf(_tabStages, stage), p);
      }
      return list;
    }

    public List<Position> BallsCanJump(Player p)
    {
        var list = BallsFree(p);
        var listFinale = new List<Position>();
        for(var i = 0; i<list.Count ; i++){
          List<Position> listEmplacement = EmplacementsToJump(list[i]);
          if(listEmplacement.Any()){
            listFinale.Add(list[i]);
          }
        }
        foreach (var VARIABLE in listFinale)
        {
          Console.WriteLine("ball qui peuvent monter");
          VARIABLE.Display();
        }
        return listFinale;
    }

    public Player Victory()
    {
      return _tabStages[0].EmplacementEmpty(0, 0) ? null : _tabStages[0].GetBall(0,0).Player;
    }

    public bool Square(Position position)
    {
      return _tabStages[position.Stage].Square(position.I, position.J);
    }
    

    
  }
}