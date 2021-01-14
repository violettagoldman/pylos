using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Pylos
{//rajouter fonction qui essaie de monter une bille à emplacement pour blocker un carré
    // de l'adversaire
    public class IA
    {
        private Position _positionToJump;
        private static readonly IA _ia = new IA();
        private BoardUI _board;

        public IA()
        {
            _positionToJump = null;
            _board = GameObject.Find("Board").GetComponent<BoardUI>();
        }

        
        public void PlayIA(Game game)
        {
            Position position = null;
            position = CheckJumpAndSquare(game);
            if (position != null)
            {
                game.Jump(position, _positionToJump);
                _positionToJump = null;
                return;
            }

            position = CheckSquare(game);
            if (position != null)
            {
                game.PlaceBall(position);
                _board.PlaceBall(new BallData(position.I, position.J, position.Stage));
                var list = game.BallFree();
                System.Random aleatoire = new System.Random();
                int i = aleatoire.Next(list.Count);
                game.RemoveSquare(list[i]);
                
                list = game.BallFree();
                if (!list.Any()) return;
                i = aleatoire.Next(list.Count);
                game.RemoveSquare(list[i]);
                return;
            }

            position = CheckSquareOpponent(game);
            if (position != null)
            {
                game.PlaceBall(position);
                _board.PlaceBall(new BallData(position.I, position.J, position.Stage));
                return;
            }
            position = CheckJump(game);
            if (position != null)
            {
                game.Jump(position, _positionToJump);
                _positionToJump = null;
                return;
            }
            game.PlaceBall(ChooseEmplacement(game));
            // _board.PlaceBall(new BallData(position.I, position.J, position.Stage));
        }
        
        public Position CheckJumpAndSquare(Game game)
        {
            var list=game.BallsCanJump();
            foreach (var p in list)
            {
                foreach (var pJump in game.EmplacementsToJump(p))
                {
                    Game g = game.Clone();
                    // g.Bs.TakeBallBoard(p, g.TabPlayer[g.ActualPlayer]);
                    // g.Bs.PlaceBallBoard(pJump, g.TabPlayer[g.ActualPlayer]);
                    g.Jump(p, pJump);
                    if (!g.Bs.Square(p)) continue;
                    _positionToJump = pJump;
                    return p;
                }
            }
            return null;
        }

        public Position CheckSquare(Game game)
        {
            List<Position> list=game.EmplacementsToPlay();
            foreach (var p in list)
            {
                Game g = game.Clone();
                g.Bs.PlaceBallBoard(p, g.TabPlayer[g.ActualPlayer]);
                    if (g.Bs.Square(p)) return p;
            }
            return null;
        }

        public Position CheckSquareOpponent(Game game)
        {
            Game g = game.Clone();
            if (game.TabPlayer[game.ActualPlayer].GetNbBalls() == 0) return null;
            g.SwitchPlayer();
            return CheckSquare(g);
        }
        
        public Position CheckJump(Game game)
        {
            var g = game.Clone();
            var list=g.BallsCanJump();
            if(!list.Any()) return null;
            System.Random aleatoire = new System.Random();
            int i = aleatoire.Next(list.Count);
            var listJump=g.EmplacementsToJump(list[i]);
            int j = aleatoire.Next(list.Count);
            _positionToJump = listJump[j];
            return list[i];
        }
        
        public Position ChooseEmplacement(Game game)
        {
            var list=game.Bs.EmplacementsToPlay();
            if(!list.Any()) return null;
            System.Random aleatoire = new System.Random();
            int i = aleatoire.Next(list.Count);
            return list[i];
        }

    }
}