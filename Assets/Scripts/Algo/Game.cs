using System;
using System.Collections.Generic;
using System.Linq;
// using System.Reflection.Metadata;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pylos {
  public class Game
  {
    private Player[] _tabPlayer;
    private BoardAndStages _bs;
    private int _actualPlayer;
    public readonly int NbPlayers;
    private int _cptSquare;
    private BoardUI _board;
	  private IA _ia = new IA();
	  private int _nbIa = 3;

    public BoardAndStages Bs => _bs;
    public Player[] TabPlayer => _tabPlayer;


    public int ActualPlayer {
      get => _actualPlayer;
      set => _actualPlayer = value;
    }

    public Game(int size, int nbP, Player actualPlayer)
    {
      NbPlayers = nbP;
      _actualPlayer = 0;
      _bs = new BoardAndStages(size);
      _tabPlayer = new Player[nbP];

      int nbBalls = 0;
      for(int i=1 ; i<=size ; i++){
        nbBalls = nbBalls + i*i;
      }
      float nbBalls2 = (float)nbBalls/(float)NbPlayers;
      for(int i=0 ; i<NbPlayers ; i++){
        _tabPlayer[i] = new Player( i%2 > 0 ? (int)Math.Ceiling(nbBalls2) : (int)Math.Floor(nbBalls2),i);
      }
    }

    public Game()
    {
      _actualPlayer = 1;
      NbPlayers = 2;
      _bs = new BoardAndStages(4);
      _tabPlayer = new Player[2];
      _cptSquare = 0;
      _board = GameObject.Find("Board").GetComponent<BoardUI>();

      int nbBalls = 0;
      for(int i=1 ; i<=4 ; i++){
        nbBalls = nbBalls + i*i;
      }
      float nbBalls2 = (float)nbBalls/(float)NbPlayers;
      for(int i=0 ; i<NbPlayers ; i++){
        _tabPlayer[i] = new Player(i%2 > 0 ? (int)Math.Ceiling(nbBalls2) : (int)Math.Floor(nbBalls2), i);
      }
    }

    public Game Clone()
    {
      var g = new Game();
      foreach (Ball ball in _bs.ListBalls)
      {
        var p = ball.Position.Clone();
        g._bs.PlaceBallBoardClone(p, g._tabPlayer[ball.Player.Number]);
        g._actualPlayer = _actualPlayer;
      }
      return g;
    }

    //fonction à appeler en début du jeu, retourne le Game sur lequelle appeler les fonctions
    public Game InitGame(int nbIa)
    {
	    _nbIa = nbIa;
	    _ia = new IA();
      var g = new Game();
      g.NewTurn();
      return g;
    }

    public void NewTurn()
    {
      Debug.Log("New Turn");
      if (_bs.Victory() != null) {
        Debug.Log(_bs.Victory().Number);
        Debug.Log("has won !");
        return;
      }
      _board.ResetBallsColor();
      SwitchPlayer();
      _cptSquare = 0;
      var p = BallsCanJump();
      if (_tabPlayer[_actualPlayer].GetNbBalls() == 0 && !p.Any()) NewTurn();
	    if(_actualPlayer==_nbIa)
      {
       _ia.PlayIA(this);
        return;
      }
      _board.ShowFreeSpots(EmplacementsToPlay());
      
      _board.ShowMoveableBalls(BallsCanJump());
      
    }

    //Fonction a appeler quand on place une bille
    public void PlaceBall(Position position)
    {
      Debug.Log("Place ball");
      _bs.PlaceBallBoard(position, _tabPlayer[_actualPlayer]);
      _board.PlaceBall(new BallData(position.I, position.J, position.Stage));
      if(_bs.Victory() == null) { /*fonction fin de partie qui prend en argument _bs.Victory() */
        if (_bs.Square(position))
        {
          Debug.Log("We have a square, the player should remove 1 or 2 ball");
          _board.ShowRemoveableBalls(BallFree());
          _board.ShowFreeSpots(new List<Position>());
          return;
        }
      }
      NewTurn();
    }
    
    //fonction a appelé quand on enlève une bille après avoir fait un carré
    public void RemoveSquare(Position position)
    {
      Debug.Log("Remove a ball");
      _bs.TakeBallBoard(position, _tabPlayer[_actualPlayer]);
      ++_cptSquare;
      if (!BallFree().Any()||_cptSquare==2) {
        NewTurn();
        return;
      }
      Debug.Log("Still have to remove one");
      _board.ShowFreeSpots(new List<Position>());
      _board.ShowRemoveableBalls(BallFree());
      _board.ShowCancelButton();
    }
    
    //fonction a appéler pour savoir où une bille peut monter après avoir cliqué dessus
    public List<Position> EmplacementsToJump(Position position)
    {
      return _bs.EmplacementsToJump(position);
    }
    
    //fonction a appelé quand on valide le déplacement d'une bille
    public void Jump(Position lastEmplacement, Position newEmplacement)
    {
      Debug.Log("Move ball");
      _bs.TakeBallBoard(lastEmplacement, _tabPlayer[_actualPlayer]);
      PlaceBall(newEmplacement);
    }

    public List<Position> BallFree()
    {
      return _bs.BallsFree(_tabPlayer[_actualPlayer]);
    }

    public List<Position> EmplacementsToPlay()
    {
      return _bs.EmplacementsToPlay();
    }

    public List<Position> BallsCanJump()
    {
      return _bs.BallsCanJump(_tabPlayer[_actualPlayer]);
    }

    public void SwitchPlayer()
    {
     _actualPlayer = _actualPlayer == 0 ? 1 :  0;
    }

    public BoardAndStages GetBs(){return _bs;}
    public Player GetPlayer(int i){return _tabPlayer[i];}
    public int NumberBallPlayer()
    {
      return _tabPlayer[_actualPlayer].GetNbBalls();
    }
      
  }

}