using System;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace chessGame
{
    internal class ChessGameProgram
    {
        static void Main(string[] args)
        {
            bool allTestSucsess = runTests();
            //bool allTestSucsess = true;
            if (allTestSucsess)
            {
                ChessGame chessBoard = new ChessGame();
                chessBoard.play();
            }
        }
        class Piece
        {
            string name;
            int id;
            public Piece(string name, int id)
            {
                this.name = name;
                this.id = id;
            }
            public int getId()
            {
                return this.id;
            }
            public void setId(int id)
            {
                this.id = id;
            }
            public string getName()
            {
                return name;
            }
            public void setName(string name)
            {
                this.name = name;
            }
            public string getToolType()
            {
                string toolType = "";
                if (this is Pawn)
                    toolType = "Pawn";
                else if (this is Rook)
                    toolType = "Rook";
                else if (this is Knight)
                    toolType = "Knight";
                else if (this is Bishop)
                    toolType = "Bishop";
                else if (this is King)
                    toolType = "King";
                else if (this is Queen)
                    toolType = "Queen";
                else
                    toolType = "Piece";
                return toolType;
            }
            public virtual int[] GetOptionalIndexes(Piece[] pieces)
            {
                return new int[0];
            }
            public int[] getAllIndexesOfAnami(Piece[] pieces)
            {
                bool isWhite = this.getName()[0] == 'W' ? true : false;
                string stringIndexes = "";
                for (int i = 0; i < pieces.Length; i++)
                {
                    if (pieces[i].getName()[0] == (isWhite ? 'B' : 'W'))
                    {
                        int[] indexes1 = new int[0];
                        indexes1 = pieces[i].GetOptionalIndexes(pieces);
                        for (int j = 0; j < indexes1.Length; j++)
                            stringIndexes += (indexes1[j]).ToString() + ",";
                    }
                }
                int[] indexes = convertStringIndexesToArray(stringIndexes);
                return indexes;
            }
            public int[] getAllIndexesOfMe(Piece[] pieces)
            {
                bool isWhite = this.getName()[0] == 'W' ? false : true;
                string stringIndexes = "";
                for (int i = 0; i < pieces.Length; i++)
                {
                    if (pieces[i].getName()[0] == (isWhite ? 'B' : 'W'))
                    {
                        int[] indexes1 = new int[0];
                        if (!(pieces[i] is King))
                            indexes1 = pieces[i].GetOptionalIndexes(pieces);
                        for (int j = 0; j < indexes1.Length; j++)
                        {
                            stringIndexes += (indexes1[j]).ToString() + ",";
                        }
                    }
                }
                int[] indexes = convertStringIndexesToArray(stringIndexes);
                return indexes;
            }
            public bool IsKingAnimeCanGetKild(Piece[] pieces)
            {
                bool isWhite = this.getName()[0] == 'W' ? true : false;
                int anmyKingIndex = 0;
                for (int i = 0; i < pieces.Length; i++)
                {
                    if (pieces[i].getName() == (isWhite ? "BK " : "WK "))
                        anmyKingIndex = pieces[i].getId();
                }
                int[] indexes1 = this.getAllIndexesOfMe(pieces);
                bool existInAnamyIndexes = false;
                for (int s = 0; s < indexes1.Length; s++)
                {
                    if (indexes1[s] == anmyKingIndex)
                        existInAnamyIndexes = true;
                }
                return existInAnamyIndexes;
            }
            public bool IsMyKingCanGetKild(Piece[] pieces)
            {
                bool isWhite = this.getName()[0] == 'W' ? true : false;
                int anmyKingIndex = 0;
                for (int i = 0; i < pieces.Length; i++)
                {
                    if (pieces[i].getName() == (!isWhite ? "BK " : "WK "))
                    {
                        anmyKingIndex = pieces[i].getId();
                        break;
                    }
                }
                int[] indexes1 = this.getAllIndexesOfAnami(pieces);
                bool existInAnamyIndexes = false;
                for (int s = 0; s < indexes1.Length; s++)
                {
                    if (indexes1[s] == anmyKingIndex)
                    {
                        existInAnamyIndexes = true;
                        break;
                    }
                }
                return existInAnamyIndexes;
            }
            public virtual Piece getCopy()
            {
                Piece resultPlayer = new Piece(this.name, this.id);
                return resultPlayer;
            }
        }
        class ChessGame
        {
            protected Piece[] pieces;
            bool isWhiteTurn;
            int countMoves;
            int countGamaseWithOutKillOrMovePawn;
            string userMsg;
            Piece[][] piecesHistory = new Piece[50][];
            public ChessGame()
            {
                this.pieces = new Piece[64];
                this.isWhiteTurn = true;
                this.countMoves = 0;
                this.gameStatus = "running";
                this.userMsg = "";
            }
            protected string gameStatus;
            public int getCountMoves()
            {
                return countMoves;
            }
            public void setCountMoves(int countMoves)
            {
                this.countMoves = countMoves;
            }
            public string getStatus()
            {
                return this.gameStatus;
            }
            public void setStatus(string status)
            {
                this.gameStatus = status;
            }
            public bool getIsWhiteTurn()
            {
                return this.isWhiteTurn;
            }
            public void setUserMsg(string userMsg)
            {
                this.userMsg = userMsg;
            }
            public void setIsWhiteTurn(bool state)
            {
                this.isWhiteTurn = state;
            }
            public void initBoard()
            {
                this.pieces = new Piece[64];
                int k = 0;
                for (int i = 0; i < 8; i++)
                {
                    if (i == 0 || i == 7)
                        initFirstOrLastLine(i);
                    for (int j = 0; j < 8; j++, k++)
                    {
                        if (i == 1)
                            this.pieces[k] = new Pawn("BP ", k, false, 0);
                        if (i == 6)
                            this.pieces[k] = new Pawn("WP ", k, false, 0);
                        if (i > 1 && i < 6)
                            this.pieces[k] = new Piece("-- ", k);
                    }
                }
            }
            public void SolgerKnghitKingmakeGameBeWithOneSolgerKnghitKingVS()
            {
                pieces[60] = new King("WK ", 60, true);
                pieces[4] = new King("BK ", 4, true);
                pieces[57] = new Knight("WN ", 57);
                pieces[1] = new Knight("BN ", 1);
                pieces[12] = new Pawn("WP ", 12, false, 0);
                for (int i = 0; i < pieces.Length; i++)
                {
                    if (i != 60 && i != 4 && i != 57 && i != 1 && i != 12)
                        this.pieces[i] = new Piece("-- ", i);
                }
            }
            public void initKingAndPawnvsKing()
            {
                pieces[60] = new King("WK ", 60, true);
                pieces[4] = new King("BK ", 4, true);
                pieces[52] = new Pawn("BP ", 52, false, 0);
                for (int i = 0; i < pieces.Length; i++)
                {
                    if (i != 60 && i != 4 && i != 52)
                        this.pieces[i] = new Piece("-- ", i);
                }
            }
            public void initFirstOrLastLine(int line)
            {
                for (int i = 0; i < 8; i++)
                {
                    int k = line == 0 ? i : (56 + i);
                    switch (i)
                    {
                        case 0: this.pieces[k] = new Rook(line == 0 ? "BR " : "WR ", k, true); break;
                        case 1: this.pieces[k] = new Knight(line == 0 ? "BN " : "WN ", k); break;
                        case 2: this.pieces[k] = new Bishop(line == 0 ? "BB " : "WB ", k); break;
                        case 3: this.pieces[k] = new Queen(line == 0 ? "BQ " : "WQ ", k); break;
                        case 4: this.pieces[k] = new King(line == 0 ? "BK " : "WK ", k, true); break;
                        case 5: this.pieces[k] = new Bishop(line == 0 ? "BB " : "WB ", k); break;
                        case 6: this.pieces[k] = new Knight(line == 0 ? "BN " : "WN ", k); break;
                        case 7: this.pieces[k] = new Rook(line == 0 ? "BR " : "WR ", k, true); break;
                    }
                }
            }
            public void printBoard()
            {
                string LettersIndexes = "ABCDEFGH";
                int indexBase64 = 0, DigitIndexes = 8;
                for (int i = 0; i <= 7; i++)
                {
                    if (i == 0)
                        Console.Write("   ");
                    Console.Write(LettersIndexes[i] + "  ");
                    if (i == 7)
                        Console.WriteLine();
                }
                for (int i = 0; i < 8; i++, DigitIndexes--)
                {
                    if (DigitIndexes != 8)
                        Console.Write(DigitIndexes + "  ");
                    for (int column = 0; column < 8; column++, indexBase64++)
                    {
                        if (i == 0 && column == 0)
                        {
                            if (DigitIndexes == 8 && column == 0)
                                Console.Write(DigitIndexes + "  ");
                        }
                        if (this.pieces[indexBase64] != null)
                            Console.Write(this.pieces[indexBase64].getName());
                    }
                    Console.WriteLine();
                }
            }
            public bool IsMoveIsCastling(Piece Piece, int indexTo)
            {
                bool result = false;
                if (!(Piece is King))
                    return result;
                King king = (King)Piece;
                bool isWhite = Piece.getName()[0] == 'W' ? true : false;
                int[] anamyIndexes = Piece.getAllIndexesOfAnami(this.pieces);
                if (king.getIsFirstMove())
                {
                    if (isWhite)
                    {
                        if (king.getId() == 60 && indexTo == 62)
                        {
                            if (pieces[63] is Rook)
                            {
                                Rook rook = (Rook)pieces[63];
                                if (rook.getIsFirstMove())
                                {
                                    if (!anamyIndexes.Contains(60) &&
                                        !anamyIndexes.Contains(61) &&
                                        !anamyIndexes.Contains(62))
                                    {
                                        if (pieces[61].getName() == "-- " &&
                                            pieces[62].getName() == "-- ")
                                        {
                                            Piece.setId(62);
                                            rook.setId(61);
                                            ((King)Piece).setIsFirstMove(false);
                                            rook.setIsFirstMove(false);
                                            pieces[62] = Piece;
                                            pieces[61] = rook;
                                            pieces[63] = new Piece("-- ", 63);
                                            pieces[60] = new Piece("-- ", 60);
                                            result = true;
                                        }
                                    }
                                }
                            }
                        }
                        else if (king.getId() == 60 && indexTo == 58)
                        {
                            if (pieces[56] is Rook)
                            {
                                Rook rook = (Rook)pieces[56];
                                if (rook.getIsFirstMove())
                                {
                                    if (!anamyIndexes.Contains(60) &&
                                        !anamyIndexes.Contains(59) &&
                                        !anamyIndexes.Contains(58) &&
                                        !anamyIndexes.Contains(57))
                                    {
                                        if (pieces[57].getName() == "-- " &&
                                            pieces[58].getName() == "-- " &&
                                            pieces[59].getName() == "-- ")
                                        {
                                            Piece.setId(58);
                                            rook.setId(59);
                                            ((King)Piece).setIsFirstMove(false);
                                            rook.setIsFirstMove(false);
                                            pieces[58] = Piece;
                                            pieces[59] = rook;
                                            pieces[56] = new Piece("-- ", 56);
                                            pieces[57] = new Piece("-- ", 57);
                                            pieces[60] = new Piece("-- ", 60);
                                            result = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (!isWhite)
                    {
                        if (king.getId() == 4 && indexTo == 6)
                        {
                            if (pieces[7] is Rook)
                            {
                                Rook rook = (Rook)pieces[7];
                                if (rook.getIsFirstMove())
                                {
                                    if (!anamyIndexes.Contains(4) &&
                                        !anamyIndexes.Contains(5) &&
                                        !anamyIndexes.Contains(6))
                                    {
                                        if (pieces[5].getName() == "-- " &&
                                            pieces[6].getName() == "-- ")
                                        {
                                            Piece.setId(6);
                                            rook.setId(5);
                                            rook.setIsFirstMove(false);
                                            king.setIsFirstMove(false);
                                            pieces[6] = Piece;
                                            pieces[5] = rook;
                                            pieces[4] = new Piece("-- ", 4);
                                            pieces[7] = new Piece("-- ", 7);
                                            result = true;
                                        }
                                    }
                                }
                            }
                        }
                        else if (king.getId() == 4 && indexTo == 2)
                        {

                            if (pieces[0] is Rook)
                            {
                                Rook rook = (Rook)pieces[0];
                                if (rook.getIsFirstMove())
                                {
                                    if (!anamyIndexes.Contains(1) &&
                                        !anamyIndexes.Contains(2) &&
                                        !anamyIndexes.Contains(3) &&
                                        !anamyIndexes.Contains(4))
                                    {
                                        if (pieces[1].getName() == "-- " &&
                                            pieces[2].getName() == "-- " &&
                                            pieces[3].getName() == "-- ")
                                        {
                                            Piece.setId(2);
                                            rook.setId(3);
                                            rook.setIsFirstMove(false);
                                            king.setIsFirstMove(false);
                                            pieces[2] = Piece;
                                            pieces[3] = rook;
                                            pieces[0] = new Piece("-- ", 0);
                                            pieces[1] = new Piece("-- ", 1);
                                            pieces[4] = new Piece("-- ", 4);
                                            result = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return result;
            }
            public bool IsPawnKillbyPasant(Piece Piece, int indexTo)
            {
                bool result = false;
                int playerLine = getLine(Piece.getId());
                int nextIndexline = getLine(indexTo);
                int playerIndex = Piece.getId();
                if (nextIndexline != 6 && nextIndexline != 3)
                    return false;
                if (Piece is Pawn)
                {
                    if (Piece.getName()[0] == 'W')
                    {
                        if (getLine(Piece.getId() + 1) == playerLine)
                        {
                            if (indexTo == (Piece.getId() - 7) && pieces[playerIndex - 7].getName()[0] == '-')
                            {
                                if ((this.pieces[playerIndex + 1] is Pawn) && this.pieces[playerIndex + 1].getName()[0] == 'B')
                                {
                                    Pawn pawn = (Pawn)pieces[playerIndex + 1];
                                    if (pawn.getJumpAtTorn() == this.countMoves - 1)
                                    {
                                        if (((Pawn)this.pieces[playerIndex + 1]).getIsJumpTow())
                                        {
                                            this.pieces[playerIndex] = new Piece("-- ", playerIndex);
                                            this.pieces[playerIndex + 1] = new Piece("-- ", (playerIndex + 1));
                                            this.pieces[playerIndex - 7] = new Pawn("WP ", (playerIndex - 7), true, 0);
                                            result = true;
                                        }
                                    }
                                }
                            }
                        }
                        if (getLine(Piece.getId() - 1) == playerLine)
                        {
                            if (indexTo == (Piece.getId() - 9) && pieces[playerIndex - 9].getName()[0] == '-')
                            {
                                if ((this.pieces[playerIndex - 1] is Pawn) && this.pieces[playerIndex - 1].getName()[0] == 'B')
                                {
                                    Pawn pawn = (Pawn)pieces[playerIndex - 1];
                                    if (pawn.getJumpAtTorn() == this.countMoves - 1)
                                    {
                                        if (((Pawn)this.pieces[playerIndex - 1]).getIsJumpTow())
                                        {
                                            this.pieces[playerIndex] = new Piece("-- ", (playerIndex));
                                            this.pieces[playerIndex - 1] = new Piece("-- ", (playerIndex - 1));
                                            this.pieces[playerIndex - 9] = new Pawn("WP ", (playerIndex - 9), true, 0);
                                            result = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (Piece.getName()[0] == 'B')
                    {
                        if (getLine(Piece.getId() - 1) == playerLine)
                        {
                            if (indexTo == (Piece.getId() + 7) && pieces[playerIndex + 7].getName()[0] == '-')
                            {
                                if ((this.pieces[playerIndex - 1] is Pawn) && this.pieces[playerIndex - 1].getName()[0] == 'W')
                                {
                                    Pawn pawn = (Pawn)pieces[playerIndex - 1];
                                    if (pawn.getJumpAtTorn() == this.countMoves - 1)
                                    {
                                        if (((Pawn)this.pieces[playerIndex - 1]).getIsJumpTow())
                                        {
                                            this.pieces[playerIndex] = new Piece("-- ", playerIndex);
                                            this.pieces[playerIndex - 1] = new Piece("-- ", (playerIndex - 1));
                                            this.pieces[playerIndex + 7] = new Pawn("BP ", (playerIndex + 7), true, 0);
                                            result = true;
                                        }
                                    }
                                }
                            }
                        }
                        if (getLine(Piece.getId() + 1) == playerLine)
                        {
                            if (indexTo == (Piece.getId() + 9) && pieces[playerIndex + 9].getName()[0] == '-')
                            {
                                if ((this.pieces[playerIndex + 1] is Pawn) && this.pieces[playerIndex + 1].getName()[0] == 'W')
                                {
                                    Pawn pawn = (Pawn)pieces[playerIndex + 1];
                                    if (pawn.getJumpAtTorn() == this.countMoves - 1)
                                    {
                                        if (((Pawn)this.pieces[playerIndex + 1]).getIsJumpTow())
                                        {
                                            this.pieces[playerIndex] = new Piece("-- ", (playerIndex));
                                            this.pieces[playerIndex + 1] = new Piece("-- ", (playerIndex + 1));
                                            this.pieces[playerIndex + 9] = new Pawn("BP ", (playerIndex + 9), true, 0);
                                            result = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return result;
            }
            public void manageHistory()
            {
                this.piecesHistory[this.countMoves] = this.getPlayersCopy();
            }
            public Piece[] getPlayersCopy()
            {
                Piece[] resultPplayers = new Piece[64];
                for (int i = 0; i < 64; i++)
                    resultPplayers[i] = this.pieces[i].getCopy();
                return resultPplayers;
            }
            public bool doMove(Piece Piece, int index)
            {
                bool isJumpTow = false;
                if (Piece is Pawn)
                {
                    if (Piece.getName()[0] == 'W')
                    {
                        if (getLine(Piece.getId()) == 2 && getLine(index) == 4)
                            isJumpTow = true;
                    }
                    else if (Piece.getName()[0] == 'B')
                    {
                        if (getLine(Piece.getId()) == 7 && getLine(index) == 5)
                            isJumpTow = true;
                    }
                }
                int origIndex = Piece.getId();
                Piece.setId(index);
                if (Piece is King)
                {
                    ((King)Piece).setIsFirstMove(false);
                }
                if (Piece is Rook)
                {
                    ((Rook)Piece).setIsFirstMove(false);
                }
                this.pieces[index] = Piece;
                this.pieces[origIndex] = new Piece("-- ", origIndex);
                if (isJumpTow)
                    ((Pawn)this.pieces[index]).setIsJumpTow(true);
                if (pieces[index] is Pawn)
                {
                    Pawn pawn = (Pawn)pieces[index];
                    pawn.setJumpAtTorn(this.countMoves);
                    pieces[index] = pawn;
                }
                return true;
            }
            public bool UpgradePawn(Piece Piece, int indexTo, string inputForTest)
            {
                string input = "";
                bool isUpgradeHappend = false;
                bool mainConditions = ((Piece is Pawn) &&
                            ((getLine(Piece.getId()) == 7) && Piece.getName()[0] == 'W' ||
                            (getLine(Piece.getId()) == 2) && Piece.getName()[0] == 'B'));
                if (!mainConditions)
                    return isUpgradeHappend;
                if (inputForTest.Length == 0)
                {
                    while (true)
                    {
                        Console.WriteLine("for apgrade to Knight prees 1 \nfor apgrade to Rook prees 2 \nfor apgrade to Bishop prees 3 \nfor apgrade to Queen prees 4 \n");
                        input = Console.ReadLine();
                        input = input.Trim();
                        if (input.Length == 1)
                        {
                            if (input == "1" || input == "2" || input == "3" || input == "4")
                                break;
                            else
                                WriteWithColor("input shold be between 1-4", "error");
                        }
                    }
                }
                else
                    input = "4";
                this.pieces[Piece.getId()] = new Piece("-- ", Piece.getId());
                switch (input)
                {
                    case "1":
                            this.pieces[indexTo] = new Knight((Piece.getName()[0] + "N ").ToString(), indexTo); isUpgradeHappend = true; break;
                    case "2":
                            this.pieces[indexTo] = new Rook((Piece.getName()[0] + "R ").ToString(), indexTo, false); isUpgradeHappend = true; break;
                    case "3":
                            this.pieces[indexTo] = new Bishop((Piece.getName()[0] + "B ").ToString(), indexTo); isUpgradeHappend = true; break;
                    case "4":
                            this.pieces[indexTo] = new Queen((Piece.getName()[0] + "Q ").ToString(), indexTo); isUpgradeHappend = true; break;
                }
                return isUpgradeHappend;
            }
            public bool IsOneOfPlayersCanSaveHisKing(Piece Piece)
            {
                bool canSave = false;
                bool isWhite = Piece.getName()[0] == 'W' ? false : true;
                for (int i = 0; i < pieces.Length; i++)
                {
                    if (pieces[i].getName()[0] == (isWhite ? 'W' : 'B'))
                    {
                        int[] indexes = pieces[i].GetOptionalIndexes(this.pieces);
                        for (int j = 0; j < indexes.Length; j++) {
                            if (this.isValidMove(this.pieces[i].getId(), indexes[j]))
                                canSave = true;
                            if (canSave)
                                break;
                        }
                    }
                    if (canSave)
                        break;
                }
                return canSave;
            }
            public void IsGameInDrewStatus(Piece Piece)
            {
                int[] anamyindexes = Piece.getAllIndexesOfAnami(this.pieces);
                if (anamyindexes.Length == 0)
                    this.setStatus("Draw because " + (this.isWhiteTurn ? "black" : "white") + " got no any Piece that can move");
                string wPlayers = ""; string bPlayers = "";
                for (int i = 0; i < this.pieces.Length; i++)
                {
                    if (this.pieces[i].getName()[0] == 'W')
                        wPlayers += this.pieces[i].getId() + ",";
                    else if (this.pieces[i].getName()[0] == 'B')
                        bPlayers += this.pieces[i].getId() + ",";
                }
                int[] arrayWPlayers = convertStringIndexesToArray(wPlayers);
                int[] arrayBPlayers = convertStringIndexesToArray(bPlayers);
                if (arrayBPlayers.Length == 1 && arrayWPlayers.Length == 1)
                    this.setStatus("Draw because we got king vs king");
                else if (arrayBPlayers.Length == 2 && arrayWPlayers.Length == 1)
                {
                    if ((this.pieces[arrayBPlayers[0]].getName() == "BK " ||
                         this.pieces[arrayBPlayers[0]].getName() == "BN " ||
                         this.pieces[arrayBPlayers[0]].getName() == "BB ") &&
                         (this.pieces[arrayBPlayers[1]].getName() == "BK " ||
                         this.pieces[arrayBPlayers[1]].getName() == "BN " ||
                         this.pieces[arrayBPlayers[1]].getName() == "BB "))
                        this.setStatus("Draw because black has not enough power to winn");
                }
                else if (arrayBPlayers.Length == 1 && arrayWPlayers.Length == 2)
                {
                    if ((this.pieces[arrayWPlayers[0]].getName() == "WK " ||
                         this.pieces[arrayWPlayers[0]].getName() == "WN " ||
                         this.pieces[arrayWPlayers[0]].getName() == "WB ") &&
                         (this.pieces[arrayWPlayers[1]].getName() == "WK " ||
                         this.pieces[arrayWPlayers[1]].getName() == "WN " ||
                         this.pieces[arrayWPlayers[1]].getName() == "WB "))
                        this.setStatus("Draw because white has not enough power to winn");
                }
                int samePlayers = 0;
                for (int i = 0; i < 100 && this.piecesHistory[i] != null; i++)
                {
                    if (this.isTheSame(this.pieces, this.piecesHistory[i]))
                        samePlayers++;
                }
                if (samePlayers >= 3)
                    this.setStatus("Draw because three repitition");
                if (this.countMoves > 1)
                {
                    int whiteCountToolsAcceptPawns = 0;
                    int blackCountToolsAcceptPawns = 0;
                    int whiteCountToolsAcceptPawnsH = 0;
                    int blackCountToolsAcceptPawnsH = 0;
                    bool whitePawnsMove = true;
                    bool blackPawnsMove = true;
                    for (int i = 0; i < this.pieces.Length; i++)
                    {
                        if (this.pieces[i].getName() == "WP ")
                        {
                            if (this.pieces[i].getName() != this.piecesHistory[countMoves - 1][i].getName())
                                whitePawnsMove = false;
                        }
                        if (this.pieces[i].getName() == "BP ")
                        {
                            if (this.pieces[i].getName() != this.piecesHistory[countMoves - 1][i].getName())
                                blackPawnsMove = false;
                        }
                        if ((this.pieces[i].getName()[0] == 'B') && (this.pieces[i].getName()[1] != 'P'))
                        {
                            if (pieces[i].getName()[1] != '-')
                                blackCountToolsAcceptPawns++;
                        }
                        if ((this.pieces[i].getName()[0] == 'W') && (this.pieces[i].getName()[1] != 'P'))
                        {
                            if (pieces[i].getName()[1] != '-')
                                whiteCountToolsAcceptPawns++;
                        }

                        if ((this.piecesHistory[countMoves - 1][i].getName()[0] == 'B') && (this.piecesHistory[countMoves - 1][i].getName()[1] != 'P'))
                            blackCountToolsAcceptPawnsH++;
                        if ((this.piecesHistory[countMoves - 1][i].getName()[0] == 'W') && (this.piecesHistory[countMoves - 1][i].getName()[1] != 'P'))
                            whiteCountToolsAcceptPawnsH++;
                    }
                    if ((whitePawnsMove && blackPawnsMove) &&
                        (blackCountToolsAcceptPawns == blackCountToolsAcceptPawnsH) &&
                        (whiteCountToolsAcceptPawns == whiteCountToolsAcceptPawnsH)
                        )
                        this.countGamaseWithOutKillOrMovePawn++;
                    else
                        this.countGamaseWithOutKillOrMovePawn = 0;
                    if (this.countGamaseWithOutKillOrMovePawn >= 20)
                        this.setStatus("Draw because 50 role");
                }
            }
            public bool isTheSame(Piece[] players1, Piece[] players2)
            {
                bool result = true;
                if (players1.Length != players2.Length)
                    return result;
                if (players1[0] == null || players2[0] == null)
                    return false;
                for (int i = 0; i < players2.Length; i++)
                {
                    if (players1[i].getName() != players2[i].getName() ||
                        players1[i].getId() != players2[i].getId())
                        result = false;
                    if ((players1[i] is King) && (players2[i] is King))
                    {
                        King king1 = (King)players1[i];
                        King king2 = (King)players2[i];
                        if (king1.getIsFirstMove() != king2.getIsFirstMove())
                            result = false;
                    }
                    if ((players1[i] is Rook) && (players2[i] is Rook))
                    {
                        Rook rook1 = (Rook)players1[i];
                        Rook rook2 = (Rook)players2[i];
                        if (rook1.getIsFirstMove() != rook2.getIsFirstMove())
                            result = false;
                    }
                }
                return result;
            }
            public bool isValidMove(int indexTo, int indexFrom)
            {
                bool result;
                Piece playerFrom = this.pieces[indexFrom];
                Piece playerTo = this.pieces[indexTo];
                superPlayer playerFromS = new superPlayer(playerFrom.getName(),playerFrom.getId(), playerFrom.getToolType());
                superPlayer playerToS = new superPlayer(playerTo.getName(), playerTo.getId(),playerTo.getToolType());

                if (this.pieces[indexFrom] is King)
                    playerFromS.setIsFirstMove(((King)this.pieces[indexFrom]).getIsFirstMove());
                else if (this.pieces[indexFrom] is Rook)
                    playerFromS.setIsFirstMove(((Rook)this.pieces[indexFrom]).getIsFirstMove());

                if (this.pieces[indexTo] is King)
                    playerToS.setIsFirstMove(((King)this.pieces[indexTo]).getIsFirstMove());
                else if (this.pieces[indexTo] is Rook)
                    playerToS.setIsFirstMove(((Rook)this.pieces[indexTo]).getIsFirstMove());
                if (playerFrom is Pawn)
                {
                    Pawn pawn = (Pawn)playerFrom;
                    playerFromS.setIsJumpTow(pawn.getIsJumpTow());
                    playerFromS.setJumpAtTorn(pawn.getJumpAtTorn());
                }
                if (playerTo is Pawn)
                {
                    Pawn pawn = (Pawn)playerTo;
                    playerToS.setIsJumpTow(pawn.getIsJumpTow());
                    playerToS.setJumpAtTorn(pawn.getJumpAtTorn());
                }
                this.doMove(this.pieces[indexTo], indexFrom);
                result = true;
                if (playerTo.IsMyKingCanGetKild(this.pieces))
                {
                    this.returnGameToLastPosition(playerFromS.getPlayerType(), playerFromS.getId(), playerFromS.getName(), playerFromS.getId(), playerFromS.getIsFirstMove(), playerFromS.getIsJumpTow(), playerFromS.getJumpAtTorn());
                    this.returnGameToLastPosition(playerToS.getPlayerType(), playerToS.getId(), playerToS.getName(), playerToS.getId(), playerToS.getIsFirstMove(), playerToS.getIsJumpTow(), playerToS.getJumpAtTorn());
                    result = false;
                }
                else
                {
                    this.returnGameToLastPosition(playerFromS.getPlayerType(), playerFromS.getId(), playerFromS.getName(), playerFromS.getId(), playerFromS.getIsFirstMove(), playerFromS.getIsJumpTow(), playerFromS.getJumpAtTorn());
                    this.returnGameToLastPosition(playerToS.getPlayerType(), playerToS.getId(), playerToS.getName(), playerToS.getId(), playerToS.getIsFirstMove(), playerToS.getIsJumpTow(), playerToS.getJumpAtTorn());
                }
                return result;
            }
            public bool isPlayerMoveHisPiece(Piece piece,int indexFrom,int indexTo)
            {
                return (piece.getName()[0] == (this.getIsWhiteTurn() ? 'W' : 'B') && (indexFrom != 0 || indexTo != 0));
            }
            public void handleTurn(string inputFortest = "")
            {
                bool isFirstTry = true;
                do
                {
                    int indexFrom = 0, indexTo = 0;
                    askUserForInput(inputFortest, isFirstTry, this.userMsg);
                    string input = inputFortest.Length == 0 ? GetInput(inputFortest) : inputFortest;
                    bool isValidInput = isValidateInput(input);
                    if (isValidInput)
                    {
                        int[] indexes = getIndexes(input);
                        indexFrom = indexes[0];
                        indexTo = indexes[1];
                    }
                    isFirstTry = false;
                    Piece Piece = this.pieces[indexFrom];
                    bool tornHppend = false;
                    bool isPlayerMoveHisPiece = this.isPlayerMoveHisPiece(Piece,indexFrom,indexTo);
                    if (isPlayerMoveHisPiece && isValidInput)
                    {
                        bool isValidMove = this.isValidMove(indexTo, indexFrom);
                        bool isMoveIsCastling = this.IsMoveIsCastling(Piece, indexTo);
                        bool isPawnKillbyPasant = IsPawnKillbyPasant(Piece, indexTo);
                        bool isPawnUpgraded = this.UpgradePawn(Piece, indexTo, inputFortest);
                        if ((!isMoveIsCastling) && (!isPawnKillbyPasant) && (!isPawnUpgraded))
                        {
                            int[] options = Piece.GetOptionalIndexes(this.pieces);
                            if (options.Contains(indexTo) && isValidMove)
                            {
                                this.doMove(Piece, indexTo);
                                tornHppend = true;
                            }
                        }
                        if (tornHppend || isMoveIsCastling || isPawnKillbyPasant || isPawnUpgraded)
                        {
                            this.manageHistory();
                            this.manageBordStatuses(Piece, inputFortest);
                            break;
                        }
                        if (inputFortest.Length > 1)
                            break;
                        WriteWithColor("Wrong input", "error");
                    }
                    else
                    {
                        if (inputFortest.Length > 0)
                            break;
                        WriteWithColor("Wrong input", "error");
                    }
                }
                while (this.getStatus() == "running");
            }
            public void returnGameToLastPosition(string toolType, int index, string p1Name, int p1iD, bool isfirstMove, bool isJumpTow, int jumpAtTorn)
            {
                if (toolType == "Pawn")
                    this.pieces[index] = new Pawn(p1Name, p1iD, isJumpTow, jumpAtTorn);
                else if (toolType == "Rook")
                    this.pieces[index] = new Rook(p1Name, p1iD, isfirstMove);
                else if (toolType == "Knight")
                    this.pieces[index] = new Knight(p1Name, p1iD);
                else if (toolType == "Bishop")
                    this.pieces[index] = new Bishop(p1Name, p1iD);
                else if (toolType == "King")
                    this.pieces[index] = new King(p1Name, p1iD, isfirstMove);
                else if (toolType == "Queen")
                    this.pieces[index] = new Queen(p1Name, p1iD);
                else if (toolType == "Piece")
                    this.pieces[index] = new Piece(p1Name, p1iD);
            }
            public void manageBordStatuses(Piece Piece, string inputFortest)
            {
                this.IsGameInDrewStatus(Piece);
                bool isKingAnimeCanGetKild = Piece.IsKingAnimeCanGetKild(this.pieces);
                if (isKingAnimeCanGetKild)
                {
                    bool isOneOfPlayersCanSaveHisKing = this.IsOneOfPlayersCanSaveHisKing(Piece);
                    if (!isOneOfPlayersCanSaveHisKing)
                        this.setStatus(this.isWhiteTurn ? "White winn" : "Black winn");
                    else
                    {
                        if (inputFortest.Length == 0)
                            WriteWithColor("Shach !!!", "error");
                    }
                }
            }
            public void play()
            {
                this.initBoard();
                this.manageHistory();
                this.setCountMoves(1);
                while (this.getStatus() == "running")
                {
                    this.printBoard();
                    this.setUserMsg("\n" + (this.getIsWhiteTurn() ? "White " : "Black ") + "Piece please enter a move: ");
                    this.handleTurn();
                    this.setIsWhiteTurn(!this.getIsWhiteTurn());
                    this.setCountMoves(this.getCountMoves() + 1);
                }
                this.printBoard();
                WriteWithColor(this.getStatus(), "info");
            }
        }
        class King: Piece
        {
            bool isFirstMove;
            public bool getIsFirstMove()
            {
                return this.isFirstMove;
            }
            public void setIsFirstMove(bool firstMove)
            {
                this.isFirstMove = firstMove;
            }
            public King(string name, int id, bool isFirstMove) : base(name, id)
            {
                this.isFirstMove = isFirstMove;
            }
            public override int[] GetOptionalIndexes(Piece[] pieces)
            {
                bool isWhite = this.getName()[0] == 'W' ? true : false;
                string stringIndexes = "";
                string stringIndexes1 = "";
                int[] chackOptions = new int[8];
                int k = 0;
                chackOptions[0] = this.getId() - 9; chackOptions[1] = this.getId() - 8;
                chackOptions[2] = this.getId() - 7; chackOptions[3] = this.getId() + 1;
                chackOptions[4] = this.getId() + 9; chackOptions[5] = this.getId() + 8;
                chackOptions[6] = this.getId() + 7; chackOptions[7] = this.getId() - 1;
                for (int d = 0; d < pieces.Length; d++)
                {
                    if (pieces[d].getName()[0] == (isWhite ? 'B' : 'W'))
                    {
                        int[] indexesS = new int[0];
                        if (!(pieces[d] is King))
                            indexesS = pieces[d].GetOptionalIndexes(pieces);
                        for (int r = 0; r < indexesS.Length; r++)
                            stringIndexes1 += (indexesS[r]).ToString() + ",";
                    }
                }
                int[] indexes1 = convertStringIndexesToArray(stringIndexes1);
                bool existInAnamyIndexes = false;
                for (int i = 0; i < chackOptions.Length; i++)
                {
                    if (chackOptions[i] >= 0 && chackOptions[i] < 64)
                    {

                        if (pieces[chackOptions[i]].getName() == "-- ")
                        {
                            existInAnamyIndexes = false;
                            for (int s = 0; s < indexes1.Length; s++)
                            {
                                if (indexes1[s] == chackOptions[i])
                                    existInAnamyIndexes = true;
                            }
                            if (!existInAnamyIndexes)
                            {
                                stringIndexes += (chackOptions[i]).ToString() + ",";
                                k++;
                                existInAnamyIndexes = false;
                            }
                        }
                        else
                        {
                            if (pieces[chackOptions[i]].getName()[0] == (isWhite ? 'B' : 'W'))
                            {
                                existInAnamyIndexes = false;
                                for (int s = 0; s < indexes1.Length; s++)
                                {
                                    if (indexes1[s] == chackOptions[i])
                                        existInAnamyIndexes = true;
                                }
                                if (!existInAnamyIndexes)
                                {
                                    stringIndexes += (chackOptions[i]).ToString() + ",";
                                    k++;
                                    existInAnamyIndexes = false;
                                }

                            }
                        }
                    }
                }
                int[] indexes = convertStringIndexesToArray(stringIndexes);
                return indexes;
            }
            public override King getCopy()
            {
                King resultPlayer = new King(this.getName(), this.getId(), this.getIsFirstMove());
                return resultPlayer;
            }
        }
        class Queen: Piece
        {
            public Queen(string name, int id) : base(name, id) { }
            public override int[] GetOptionalIndexes(Piece[] pieces)
            {
                Rook rook = new Rook(this.getName(), this.getId(), false);
                Bishop bishop = new Bishop(this.getName(), this.getId());
                int[] indexesOfRook = rook.GetOptionalIndexes(pieces);
                int[] indexesOfBishop = bishop.GetOptionalIndexes(pieces);
                int[] combined = indexesOfRook.Concat(indexesOfBishop).ToArray();
                return combined;
            }
            public override Queen getCopy()
            {
                Queen resultPiece = new Queen(this.getName(), this.getId());
                return resultPiece;
            }
        }
        class Rook: King
        {
            public Rook(string name, int id, bool isFirstMove) : base(name, id, isFirstMove)
            {
            }
            public override int[] GetOptionalIndexes(Piece[] pieces)
            {
                string stringIndexes = "";
                int k = 0;
                bool isWhite = this.getName()[0] == 'W' ? true : false;
                for (int i = this.getId() + 8; i < 64; i = i + 8, k++)
                {
                    if (pieces[i].getName() == "-- ")
                        stringIndexes += (i).ToString() + ",";
                    else
                    {
                        if (pieces[i].getName()[0] == (isWhite ? 'B' : 'W'))
                            stringIndexes += (i).ToString() + ",";
                        break;
                    }
                }
                k++;
                for (int i = this.getId() - 8; i >= 0; i = i - 8, k++)
                {
                    if (pieces[i].getName() == "-- ")
                        stringIndexes += (i).ToString() + ",";
                    else
                    {
                        if (pieces[i].getName()[0] == (isWhite ? 'B' : 'W'))
                            stringIndexes += (i).ToString() + ",";
                        break;
                    }
                }
                k++;
                for (int i = this.getId() + 1; i >= 0 && i <= 63; i++, k++)
                {
                    int plarerLine = getLine(this.getId());
                    if (plarerLine == getLine(i))
                    {
                        if (pieces[i].getName() == "-- ")
                            stringIndexes += (i).ToString() + ",";
                        else
                        {
                            if (pieces[i].getName()[0] == (isWhite ? 'B' : 'W'))
                                stringIndexes += (i).ToString() + ",";
                            break;
                        }
                    }
                    else
                        break;
                }
                k++;
                for (int i = this.getId() - 1; i >= 0 && i <= 63; i--, k++)
                {
                    int plarerLine = getLine(this.getId());
                    if (plarerLine == getLine(i))
                    {
                        if (pieces[i].getName() == "-- ")
                            stringIndexes += (i).ToString() + ",";
                        else
                        {
                            if (pieces[i].getName()[0] == (isWhite ? 'B' : 'W'))
                                stringIndexes += (i).ToString() + ",";
                            break;
                        }
                    }
                    else
                        break;
                }

                int[] indexes = convertStringIndexesToArray(stringIndexes);
                return indexes;
            }
            public override Rook getCopy()
            {
                Rook resultPice = new Rook(this.getName(), this.getId(), this.getIsFirstMove());
                return resultPice;
            }
        }
        class Bishop: Piece
        {
            public Bishop(string name, int id) : base(name, id) { }
            public override int[] GetOptionalIndexes(Piece[] pieces)
            {
                string stringIndexes = "";
                int k = 0;
                bool isWhite = this.getName()[0] == 'W' ? true : false;
                int playerLine = getLine(this.getId());
                for (int i = this.getId() + 9; i < 64; i = i + 9, k++)
                {
                    int nextMoveLine = getLine(i);
                    if (nextMoveLine == (playerLine - 1))
                    {
                        if (pieces[i].getName() == "-- ")
                            stringIndexes += (i).ToString() + ",";
                        else
                        {
                            if (pieces[i].getName()[0] == (isWhite ? 'B' : 'W'))
                                stringIndexes += (i).ToString() + ",";
                            break;
                        }
                    }
                    else
                        break;
                    playerLine--;
                }
                k++;
                playerLine = getLine(this.getId());
                for (int i = this.getId() - 9; i >= 0; i = i - 9, k++)
                {
                    int nextMoveLine = getLine(i);
                    if (nextMoveLine == (playerLine + 1))
                    {
                        if (pieces[i].getName() == "-- ")
                            stringIndexes += (i).ToString() + ",";
                        else
                        {
                            if (pieces[i].getName()[0] == (isWhite ? 'B' : 'W'))
                                stringIndexes += (i).ToString() + ",";
                            break;
                        }
                    }
                    else
                        break;
                    playerLine++;
                }
                k++;
                playerLine = getLine(this.getId());
                for (int i = this.getId() - 7; i >= 0; i -= 7, k++)
                {
                    int nextMoveLine = getLine(i);
                    if (nextMoveLine == (playerLine + 1))
                    {
                        if (pieces[i].getName() == "-- ")
                            stringIndexes += (i).ToString() + ",";
                        else
                        {
                            if (pieces[i].getName()[0] == (isWhite ? 'B' : 'W'))
                                stringIndexes += (i).ToString() + ",";
                            break;
                        }
                    }
                    else
                        break;
                    playerLine++;
                }
                k++;
                playerLine = getLine(this.getId());
                for (int i = this.getId() + 7; i < 64; i += 7, k++)
                {
                    int nextMoveLine = getLine(i);
                    if (nextMoveLine == (playerLine - 1))
                    {
                        if (pieces[i].getName() == "-- ")
                            stringIndexes += (i).ToString() + ",";
                        else
                        {
                            if (pieces[i].getName()[0] == (isWhite ? 'B' : 'W'))
                                stringIndexes += (i).ToString() + ",";
                            break;
                        }
                    }
                    else
                        break;
                    playerLine--;
                }
                int[] indexes = convertStringIndexesToArray(stringIndexes);
                return indexes;
            }
            public override Bishop getCopy()
            {
                Bishop resultPiece = new Bishop(this.getName(), this.getId());
                return resultPiece;
            }
        }
        class Knight: Piece
        {
            public Knight(string name, int id) : base(name, id) { }
            public override int[] GetOptionalIndexes(Piece[] pieces)
            {
                string stringIndexes = "";
                int k = 0, playerLine = getLine(this.getId());
                bool isWhite = this.getName()[0] == 'W' ? true : false;
                int[] ChackIndexes = new int[4];
                ChackIndexes[0] = this.getId() + 16; ChackIndexes[1] = this.getId() - 16;
                ChackIndexes[2] = this.getId() - 2; ChackIndexes[3] = this.getId() + 2;
                for (int i = 0; i < ChackIndexes.Length; i++)
                {
                    if (ChackIndexes[i] >= 0 && (i == 2 || i == 3))
                    {
                        if ((ChackIndexes[i] + 8) < 64)
                        {
                            if (playerLine == getLine(pieces[ChackIndexes[i]].getId()))
                                if (pieces[ChackIndexes[i] + 8].getName() == "-- ")
                                {
                                    stringIndexes += (ChackIndexes[i] + 8).ToString() + ",";
                                    k++;
                                }
                                else
                                {
                                    if (pieces[ChackIndexes[i] + 8].getName()[0] == (isWhite ? 'B' : 'W'))
                                    {
                                        stringIndexes += (ChackIndexes[i] + 8).ToString() + ",";
                                        k++;
                                    }
                                }
                        }
                        if ((ChackIndexes[i] - 8) >= 0)
                        {

                            if (pieces[ChackIndexes[i] - 8].getName() == "-- ")
                            {
                                stringIndexes += (ChackIndexes[i] - 8).ToString() + ",";
                                k++;
                            }
                            else
                            {
                                if (pieces[ChackIndexes[i] - 8].getName()[0] == (isWhite ? 'B' : 'W'))
                                {
                                    stringIndexes += (ChackIndexes[i] - 8).ToString() + ",";
                                    k++;
                                }
                            }
                        }
                    }
                    if (ChackIndexes[i] >= 0 && (i == 0 || i == 1))
                    {
                        if ((ChackIndexes[i] + 1) < 64)
                        {
                            if ((playerLine == getLine(pieces[ChackIndexes[i] + 1].getId()) - 2) ||
                                (playerLine == getLine(pieces[ChackIndexes[i] + 1].getId()) + 2))
                            {
                                if (pieces[ChackIndexes[i] + 1].getName() == "-- ")
                                {
                                    stringIndexes += (ChackIndexes[i] + 1).ToString() + ",";
                                    k++;
                                }
                                else
                                {
                                    if (pieces[ChackIndexes[i] + 1].getName()[0] == (isWhite ? 'B' : 'W'))
                                    {
                                        stringIndexes += (ChackIndexes[i] + 1).ToString() + ",";
                                        k++;
                                    }
                                }
                            }
                        }
                        if ((ChackIndexes[i] - 1) >= 0 && (ChackIndexes[i] - 1) < 64)
                        {
                            if ((
                                (playerLine == getLine(pieces[ChackIndexes[i] - 1].getId()) - 2) ||
                                (playerLine == getLine(pieces[ChackIndexes[i] - 1].getId()) + 2)))
                            {

                                if (pieces[ChackIndexes[i] - 1].getName() == "-- ")

                                {
                                    stringIndexes += (ChackIndexes[i] - 1).ToString() + ",";
                                    k++;
                                }
                                else
                                {
                                    if (pieces[ChackIndexes[i] - 1].getName()[0] == (isWhite ? 'B' : 'W'))
                                    {
                                        stringIndexes += (ChackIndexes[i] - 1).ToString() + ",";
                                        k++;
                                    }
                                }
                            }
                        }
                    }
                }
                int[] indexes = convertStringIndexesToArray(stringIndexes);
                return indexes;
            }
            public override Knight getCopy()
            {
                Knight resultPiece = new Knight(this.getName(), this.getId());
                return resultPiece;
            }
        }
        class Pawn: Piece
        {
            bool isJumpTow;
            int jumpAtTorn;
            public Pawn(string name, int id, bool isJumpTow, int jumpAtTorn) : base(name, id)
            {
                this.isJumpTow = isJumpTow;
                this.jumpAtTorn = jumpAtTorn;
            }
            public int getJumpAtTorn()
            {
                return this.jumpAtTorn;
            }
            public void setJumpAtTorn(int jumpAtTorn)
            {
                this.jumpAtTorn = jumpAtTorn;
            }
            public bool getIsJumpTow()
            {
                return isJumpTow;
            }
            public void setIsJumpTow(bool jumpTow)
            {
                isJumpTow = jumpTow;
            }
            public override int[] GetOptionalIndexes(Piece[] pieces)
            {
                string stringIndexes = "";
                int line = getLine(this.getId());
                if (this.getName()[0] == 'B')
                {
                    if (this.getId() > 7 && this.getId() < 16)
                    {
                        if (pieces[this.getId() + 8].getName() == "-- ")
                        {
                            stringIndexes += (this.getId() + 8).ToString() + ",";
                            if (pieces[this.getId() + 16].getName() == "-- ")
                                stringIndexes += (this.getId() + 16).ToString() + ",";
                        }
                    }
                    else if (this.getId() + 8 <= 56 && this.getId() + 7 <= 56 && this.getId() + 9 <= 56)
                    {
                        if (pieces[this.getId() + 8].getName() == "-- ")
                            stringIndexes += (this.getId() + 8).ToString() + ",";
                        if (getLine(pieces[this.getId() + 9].getId()) == line - 1)
                        {
                            if (pieces[this.getId() + 9].getName()[0] == 'W')
                                stringIndexes += (this.getId() + 9).ToString() + ",";
                        }
                        if (getLine(pieces[this.getId() + 7].getId()) == line - 1)
                        {
                            if (pieces[this.getId() + 7].getName()[0] == 'W')
                                stringIndexes += (this.getId() + 7).ToString() + ",";
                        }
                    }
                }
                else
                {
                    if (this.getId() > 47 && this.getId() < 56)
                    {
                        if (pieces[this.getId() - 8].getName() == "-- ")
                        {
                            stringIndexes += (this.getId() - 8).ToString() + ",";
                            if (pieces[this.getId() - 16].getName() == "-- ")
                                stringIndexes += (this.getId() - 16).ToString() + ",";
                        }
                    }
                    else if (getLine(this.getId()) != 1 && getLine(this.getId()) != 8)
                    {
                        if (pieces[this.getId() - 8].getName() == "-- ")
                            stringIndexes += (this.getId() - 8).ToString() + ",";
                        if (getLine(pieces[this.getId() - 9].getId()) == line + 1)
                        {
                            if (pieces[this.getId() - 9].getName()[0] == 'B')
                                stringIndexes += (this.getId() - 9).ToString() + ",";
                        }
                        if (getLine(pieces[this.getId() - 7].getId()) == line + 1)
                        {
                            if (pieces[this.getId() - 7].getName()[0] == 'B')
                                stringIndexes += (this.getId() - 7).ToString() + ",";
                        }
                    }
                }
                int[] indexes = convertStringIndexesToArray(stringIndexes);
                return indexes;
            }
            public override Pawn getCopy()
            {
                Pawn resultPiece = new Pawn(this.getName(), this.getId(), this.isJumpTow, this.jumpAtTorn);
                return resultPiece;
            }
        }
        class superPlayer: Pawn
        {
            bool isFirstMove;
            string playerType;
            public bool getIsFirstMove()
            {
                return this.isFirstMove;
            }
            public void setIsFirstMove(bool firstMove)
            {
                this.isFirstMove = firstMove;
            }
            public string getPlayerType()
            {
                return playerType;
            }
            public superPlayer(string name, int id,string playerType) : base(name, id, false, 0)
            {
                this.isFirstMove = false;
                this.playerType = playerType;
            }
        }
        class TestData
        {
            string[] arrayIndexes;
            string name;
            public void setName(string name)
            {
                this.name = name;
            }
            public string getName()
            {
                return this.name;
            }
            public void seArrayIndexes(string[] arrayIndexes)
            {
                this.arrayIndexes = arrayIndexes;
            }
            public TestData(string[] arrayIndexes)
            {
                this.arrayIndexes = arrayIndexes;
                this.name = this.getName();
            }
            public bool runTest()
            {
                bool result = false;
                ChessGame chessGame = new ChessGame();
                if (name == "test 6")
                    chessGame.SolgerKnghitKingmakeGameBeWithOneSolgerKnghitKingVS();
                else if (name == "test 7")
                    chessGame.SolgerKnghitKingmakeGameBeWithOneSolgerKnghitKingVS();
                else if (name == "test 10")
                    chessGame.initKingAndPawnvsKing();
                else
                    chessGame.initBoard();
                int k = 0;
                while (chessGame.getStatus() == "running" && k < this.arrayIndexes.Length)
                {
                    chessGame.handleTurn(this.arrayIndexes[k]);
                    chessGame.setIsWhiteTurn(!chessGame.getIsWhiteTurn());
                    chessGame.manageHistory();
                    chessGame.setCountMoves(chessGame.getCountMoves() + 1);
                    k++;
                }
                if (name == "test 1" || name == "test 3" || name == "test 4" || name == "test 8")
                    result = chessGame.getStatus() == "Black winn" ? true : false;
                else if (name == "test 5")
                    result = chessGame.getStatus() == "Draw because three repitition" ? true : false;
                else if (name == "test 6")
                    result = chessGame.getStatus() == "Draw because black has not enough power to winn" ? true : false;
                else if (name == "test 7")
                    result = chessGame.getStatus() == "Draw because white has not enough power to winn" ? true : false;
                else if (name == "test 9" || name == "test 2")
                    result = chessGame.getStatus() == "White winn" ? true : false;
                else if (name == "test 10")
                    result = chessGame.getStatus() == "Draw because we got king vs king" ? true : false;
                else if (name == "test 11")
                    result = chessGame.getStatus() == "Draw because 50 role" ? true : false;
                return result;
            }
        }
        public static bool runTests()
        {
            WriteWithColor("run tests", "success");
            bool allTestSecsess = true;
            TestData[] allTests = new TestData[11];
            allTests[0] = new TestData(new string[] { "h2h3","b7b5","h3h4","b5b4","a2a4","b4a3","a1a3","g7g5","a3a2","g5g4","a2a1","g4g3","b1a3","e7e6",
                                                      "a1b1","d8f6","h1h2","f8c5","b2b3","f6f2" });
            allTests[1] = new TestData(new string[] { "h2h4", "a7a5", "h4h5", "a5a4", "h5h6", "a4a3", "h6g7", "b7b6", "d2d4", "a8a4", "c1h6", "a4d4", "g7f8", "4" });
            allTests[2] = new TestData(new string[] { "a2a3","a7a5","b1c3","b7b6","d2d3","g8h6","c1h6","e7e6","d1c1","f8d6","c1d1","e8g8","d1b1","a8a7",
                                                      "b1a2","b8a6","g1h3","g8h8","g2g3","d8h4","f1g2","f7f6","a1d1","a7a8","d1a1","a8a7","e1g1","c8b7",
                                                      "a1b1","h4g3","b1c1","g3g2" });
            allTests[3] = new TestData(new string[] { "a2a3","a7a5","b1c3","b7b6","d2d3","g8h6","c1h6","e7e6","d1c1","f8d6","c1d1","e8g8","d1b1","a8a7",
                                                      "b1a2","b8a6","g1h3","g8h8","g2g3","d8h4","f1g2","f7f6","a1d1","a7a8","d1a1","a8a7","e1g1","c8b7",
                                                      "a1b1","h4g3","b1c1","g3d3","g1h1","d3c3","h1g1","c3c2","g1h1","c2c1","f2f3","a7a8","h3f2","a8a7",
                                                      "g2h3","a7a8","h3g4","c1f1" });
            allTests[4] = new TestData(new string[] { "b1c3", "b8c6", "a1b1", "a8b8", "b1a1", "b8a8", "a1b1", "a8b8", "b1a1", "b8a8", "a1b1", "a8b8" });
            allTests[5] = new TestData(new string[] { "b1c3", "e8e7", "c3d5", "e7e6", "e1e2", "e6d5" });
            allTests[6] = new TestData(new string[] { "e1e2", "e8e7", "e2d3", "b8c6", "d3c4", "c6b4", "c4b4" });
            allTests[7] = new TestData(new string[] { "a2a3", "e7e6", "a1a2", "d8f6", "a3a4", "f8c5", "a2a1", "c5f2" });
            allTests[8] = new TestData(new string[] { "b1c3", "a7a6", "f2f4", "a8a7", "f4f5", "a7a8", "d2d3", "e7e5", "f5e6", "h7h6", "c1h6", "a6a5", "e2e4",
                                                      "a8a7", "d1f3", "b7b6", "e1c1", "f8a3", "f3f7"});
            allTests[9] = new TestData(new string[] { "e1e2" });
            allTests[10] = new TestData(new string[] {"b1a3","b8a6","g1h3","a6c5","a3c4","g8f6","a2a3","a7a6","h3f4","c5b3","f4d3",
                                                      "a8a7","a1a2","a7a8","a2a1","a8a7","a1b1","h8g8","h1g1","f6e4","g1h1","a7a8",
                                                      "d3f4","e4g3","h1g1","g8h8","c4e5","a8a7" });
            for (int i = 0; i < allTests.Length; i++)
            {
                allTests[i].setName("test " + (i + 1));
                if (!allTests[i].runTest())
                {
                    allTestSecsess = false;
                    WriteWithColor(allTests[i].getName() + " faild", "error");   
                }
            }
            if (allTestSecsess)
                WriteWithColor("all tests success", "success");
            return allTestSecsess;
        }
        public static int getLine(int num)
        {
            int result = 0;
            if (num <= 7)
                result = 8;
            else if (num > 7 && num < 16)
                result = 7;
            else if (num > 15 && num < 24)
                result = 6;
            else if (num > 23 && num < 32)
                result = 5;
            else if (num > 31 && num < 40)
                result = 4;
            else if (num > 39 && num < 48)
                result = 3;
            else if (num > 47 && num < 56)
                result = 2;
            else if (num >= 56)
                result = 1;
            return result;
        }
        public static int[] convertStringIndexesToArray(string indexes)
        {
            if (indexes.Length == 0)
                return new int[0];
            int count = 0;
            for (int i = 0; i < indexes.Length; i++)
            {
                if (indexes[i] == ',')
                    count++;
            }
            int[] intIndexex = new int[count];
            int k = 0;
            string word = "";
            for (int s = 0; s <= indexes.Length - 1; s++)
            {
                if (indexes[s] != ',')
                    word += indexes[s];
                else
                {
                    intIndexex[k] = int.Parse(word);
                    k++;
                    word = "";
                }
            }
            return intIndexex;
        }
        public static string GetInput(string inputFortest)
        {
            string input = "";
            if (inputFortest.Length == 0)
            {
                input = Console.ReadLine();
            }
            return input;
        }
        public static int[] getIndexes(string input)
        {
            int[] indexes = new int[2];
            string moveFrom = "", moveTo = "";
            moveFrom = input[0].ToString().ToUpper() + input[1].ToString();
            moveTo = input[2].ToString().ToUpper() + input[3].ToString();
            indexes[0] = getIndex(moveFrom);
            indexes[1] = getIndex(moveTo);
            return indexes;
        }
        public static void askUserForInput(string inputFortest, bool isFirstTry, string msg)
        {
            if (inputFortest.Length == 0)
            {
                if (isFirstTry)
                    WriteWithColor(msg, "success");
                else
                    WriteWithColor(msg, "error");
            }
        }
        public static bool isValidateInput(string input)
        {
            bool isValid = false;
            if (input.Length != 4)
                return isValid;
            string allowdLetters = "ABCDEFGH", allowdDigits = "12345678", moveFrom = "", moveTo = "";
            moveFrom = input[0].ToString().ToUpper() + input[1].ToString();
            moveTo = input[2].ToString().ToUpper() + input[3].ToString();
            if (allowdLetters.Contains(moveFrom[0]) &&
                allowdDigits.Contains(moveFrom[1]) &&
                allowdLetters.Contains(moveTo[0]) &&
                allowdDigits.Contains(moveTo[1]))
                isValid = true;
            return isValid;

        }
        public static int getIndex(string index)
        {
            int row = 0, column = 0, result = 0;
            switch (index[1])
            {
                case '8': row = 0; break;
                case '7': row = 8; break;
                case '6': row = 16; break;
                case '5': row = 24; break;
                case '4': row = 32; break;
                case '3': row = 40; break;
                case '2': row = 48; break;
                case '1': row = 56; break;
            }
            switch (index[0])
            {
                case 'A': column = 0; break;
                case 'B': column = 1; break;
                case 'C': column = 2; break;
                case 'D': column = 3; break;
                case 'E': column = 4; break;
                case 'F': column = 5; break;
                case 'G': column = 6; break;
                case 'H': column = 7; break;
            }
            result = row + column;
            return result;
        }
        public static void WriteWithColor(string msg, string type = "")
        {
            if (type == "error")
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(msg);
                Console.ForegroundColor = ConsoleColor.White;
            }
            else if (type == "info")
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(msg);
                Console.ForegroundColor = ConsoleColor.White;
            }
            else if (type == "success")
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(msg);
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
    }
}