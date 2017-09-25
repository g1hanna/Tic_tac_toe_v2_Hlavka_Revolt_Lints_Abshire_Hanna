using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

//using TicTacToe.ConsoleApp.Model;

namespace CodingActivity_TicTacToe_ConsoleGame
{
    public class Gameboard
    {
        #region CLASSES
        /// <summary>
        /// An exception that occurs when a position outside the <see cref="Gameboard"/> 
        /// is used.
        /// </summary>
        public class GamePositionException : Exception
        {
            private int _invalidX;
            private int _invalidY;

            /// <summary>
            /// Creates a new gameboard position error. An error message is automatically generated
            /// </summary>
            /// <param name="invalidX">The x-position of the unaccepted space.</param>
            /// <param name="invalidY">The y-position of the unaccepted space.</param>
            public GamePositionException(int invalidX, int invalidY) :
                base(string.Format("Position [{0}, {1}] not on the gameboard.", invalidX, invalidY))
            {
                _invalidX = invalidX;
                _invalidY = invalidY;
            }
            /// <summary>
            /// Creates a new gameboard position error. The user must provide an error message.
            /// </summary>
            /// <param name="message">A user-defined error message.</param>
            /// <param name="invalidX">The x-position of the unaccepted space.</param>
            /// <param name="invalidY">The y-position of the unaccepted space.</param>
            public GamePositionException(string message, int invalidX, int invalidY) :
                base(message)
            {
                _invalidX = invalidX;
                _invalidY = invalidY;
            }

            /// <summary>
            /// Obtains the coordinates of an invalid position.
            /// </summary>
            /// <returns>A two-element array containing the x- and y-coordinates, respectively</returns>
            public int[] GetCoordinates()
            {
                return new int[2] { _invalidX, _invalidY };
            }
        }
        #endregion

        #region DELEGATES
        /// <summary>
        /// A method that determines whether a match has been made on the game board.
        /// </summary>
        /// <param name="posX">The starting X position of the pattern.</param>
        /// <param name="posY">The starting Y position of the pattern.</param>
        /// <returns></returns>
        private delegate bool gameBoardMatch(int posX, int posY);
        #endregion

        #region ENUMS

        public enum PlayerPiece
        {
            X,
            O,
            None
        }

        public enum GameboardState
        {
            NewRound,
            PlayerXTurn,
            PlayerOTurn,
            PlayerXWin,
            PlayerOWin,
            CatsGame
        }

        #endregion

        #region FIELDS

        private const int MAX_NUM_OF_ROWS_COLUMNS = 4;

        private PlayerPiece[,] _positionState;

        private GameboardState _currentRoundState;

        #endregion

        #region PROPERTIES

        public int MaxNumOfRowsColumns
        {
            get { return MAX_NUM_OF_ROWS_COLUMNS; }
        }

        public PlayerPiece[,] PositionState
        {
            get { return _positionState; }
            set { _positionState = value; }
        }

        public GameboardState CurrentRoundState
        {
            get { return _currentRoundState; }
            set { _currentRoundState = value; }
        }

        private PlayerPiece this[int x, int y]
        {
            get
            {
                if ((x < 4 && x >= 0) && (y < 4 && y >= 0))
                    return _positionState[x, y];
                else
                    throw new GamePositionException(x, y);
            }
            set
            {
                if ((x < 4 && x >= 0) && (y < 4 && y >= 0))
                    _positionState[x, y] = value;
                else
                    throw new GamePositionException(x, y);
            }
        } 
        #endregion

        #region CONSTRUCTORS

        public Gameboard()
        {
            _positionState = new PlayerPiece[MAX_NUM_OF_ROWS_COLUMNS, MAX_NUM_OF_ROWS_COLUMNS];

            InitializeGameboard();
        }

        #endregion

        #region METHODS

        /// <summary>
        /// fill the game board array with "None" enum values
        /// </summary>
        public void InitializeGameboard()
        {
            _currentRoundState = GameboardState.NewRound;

            //
            // Set all PlayerPiece array values to "None"
            //
            for (int row = 0; row < MAX_NUM_OF_ROWS_COLUMNS; row++)
            {
                for (int column = 0; column < MAX_NUM_OF_ROWS_COLUMNS; column++)
                {
                    _positionState[row, column] = PlayerPiece.None;
                }
            }
        }


        /// <summary>
        /// Determine if the game board position is taken
        /// </summary>
        /// <param name="gameboardPosition"></param>
        /// <returns>true if position is open</returns>
        public bool GameboardPositionAvailable(GameboardPosition gameboardPosition)
        {
            //
            // Confirm that the board position is empty
            // Note: gameboardPosition converted to array index by subtracting 1
            //

           if (this[gameboardPosition.Row - 1, gameboardPosition.Column - 1] == PlayerPiece.None)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Update the game board state if a player wins or a cat's game happens.
        /// </summary>
        /// <param name="winner">The winning player of the game.</param>
        public void UpdateGameboardState(out PlayerPiece winner)
        {
            //
            // Player X has won
            //
            if (ThreeInARow(PlayerPiece.X))
            {
                _currentRoundState = GameboardState.PlayerXWin;
                winner = PlayerPiece.X;
            }
            //
            // A player O has won
            //
            else if (ThreeInARow(PlayerPiece.O))
            {
                _currentRoundState = GameboardState.PlayerOWin;
                winner = PlayerPiece.O;
            }
            //
            // All positions filled
            //
            else if (IsCatsGame())
            {
                _currentRoundState = GameboardState.CatsGame;
                winner = PlayerPiece.None;
            }
            // Other situation
            else
            {
                winner = PlayerPiece.None;
            }
        }
        
        public bool IsCatsGame()
        {
            //
            // All positions on board are filled and no winner
            //
            for (int row = 0; row < 4; row++)
            {
                for (int column = 0; column < 4; column++)
                {
                    if (_positionState[row, column] == PlayerPiece.None)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Check for any three in a row.
        /// </summary>
        /// <param name="playerPieceToCheck">Player's game piece to check</param>
        /// <returns>true if a player has won</returns>
        private bool ThreeInARow(PlayerPiece playerPieceToCheck)
        {
            // set up game board patterns
            // row checker
            gameBoardMatch horizontalPattern = (int posX, int posY) =>
            {
                if ((posX < 0 || posX > 4) || (posY < 0 || posY > 1))
                    throw new GamePositionException(string.Format("Invalid coordinates ({0}, {1}). Match area exceeds board.",
                        posX, posY), posX, posY);

                return (_positionState[posX, posY] == playerPieceToCheck &&
                    _positionState[posX, posY + 1] == playerPieceToCheck &&
                    _positionState[posX, posY + 2] == playerPieceToCheck);
            };
            // column checker
            gameBoardMatch verticalPattern = (int posX, int posY) =>
            {
                if ((posY < 0 || posY > 4) || (posX < 0 || posX > 1))
                    throw new GamePositionException(string.Format("Invalid coordinates ({0}, {1}). Match area exceeds board.",
                        posX, posY), posX, posY);

                return (_positionState[posX, posY] == playerPieceToCheck &&
                    _positionState[posX + 1, posY] == playerPieceToCheck &&
                    _positionState[posX + 2, posY] == playerPieceToCheck);
            };
            // diagonal checker #1
            gameBoardMatch diagonalPatternClimb = (int posX, int posY) =>
            {
                if ((posX < 0 || posX > 1) || (posY < 0 || posY > 1))
                    throw new GamePositionException(string.Format("Invalid coordinates ({0}, {1}). Match area exceeds board.",
                        posX, posY), posX, posY);

                return (_positionState[posX, posY] == playerPieceToCheck &&
                _positionState[posX + 1, posY + 1] == playerPieceToCheck &&
                _positionState[posX + 2, posY + 2] == playerPieceToCheck);
            };
            // diagonal checker #2
            gameBoardMatch diagonalPatternFall = (int posX, int posY) =>
            {
                if ((posX < 0 || posX > 1) || (posY < 0 || posY > 1))
                    throw new GamePositionException(string.Format("Invalid coordinates ({0}, {1}). Match area exceeds board.",
                        posX, posY), posX, posY);

                return (_positionState[posX, posY + 2] == playerPieceToCheck &&
                _positionState[posX + 1, posY + 1] == playerPieceToCheck &&
                _positionState[posX + 2, posY] == playerPieceToCheck);
            };


            //
            // Check rows for player win
            //
            for (int row = 0; row < 4; row++)
            {
                if (horizontalPattern(row, 0) || horizontalPattern(row, 1)) return true;
            }

            //
            // Check columns for player win
            //
            for (int column = 0; column < 4; column++)
            {
                if (verticalPattern(0, column) || verticalPattern(1, column)) return true;
            }

            //
            // Check diagonals for player win
            //
            for (int row = 0; row < 2; row++)
            {
                for (int column = 0; column < 2; column++)
                {
                    if (diagonalPatternClimb(row, column) || diagonalPatternFall(row, column)) return true;
                }
            }

            //
            // No Player Has Won
            //

            return false;
        }

        /// <summary>
        /// Add player's move to the game board.
        /// </summary>
        /// <param name="gameboardPosition"></param>
        /// <param name="PlayerPiece"></param>
        public void SetPlayerPiece(GameboardPosition gameboardPosition, PlayerPiece PlayerPiece)
        {
            //
            // Row and column value adjusted to match array structure
            // Note: gameboardPosition converted to array index by subtracting 1
            //
            this[gameboardPosition.Row - 1, gameboardPosition.Column - 1] = PlayerPiece;

            //
            // Change game board state to next player
            //
            SetNextPlayer();
        }

        /// <summary>
        /// Switch the game board state to the next player.
        /// </summary>
        private void SetNextPlayer()
        {
            if (_currentRoundState == GameboardState.PlayerXTurn)
            {
                _currentRoundState = GameboardState.PlayerOTurn;
            }
            else
            {
                _currentRoundState = GameboardState.PlayerXTurn;
            }
        }

        #endregion
    }
}

