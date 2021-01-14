using System;
using System.Collections.Generic;
 
 namespace Pylos {
  public class Player
  {   public readonly int Number;
      private int _nbBalls;
      private readonly List<Ball> _listBalls;

      public Player(int nbB, int number){
        Number = number;
        _nbBalls = nbB;
        _listBalls = new List<Ball>();
        for(int i=0 ; i<_nbBalls ; i++){
          _listBalls.Add(new Ball(this, Number, null));
        }
        Console.WriteLine("Joueur "+Number+" a "+_nbBalls+" billes.");
      }

      public Ball PlayBall(){
        Ball b = _listBalls[0];
        _nbBalls --;
        _listBalls.RemoveAt(0);
        return b;
      }

      public void TakeBall(Ball b){
        _nbBalls ++;
        _listBalls.Add(b);
      }
      
      public int GetNbBalls(){ return _nbBalls;}

      public void PlaceBall()
      {
          _nbBalls --;
      }

      public void RemoveBall()
      {
          _nbBalls++;
      }
  }
 }