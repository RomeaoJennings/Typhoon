﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Typhoon.Model
{
    public struct CastleRights
    {
        readonly bool WhiteKing;
        readonly bool WhiteQueen;
        readonly bool BlackKing;
        readonly bool BlackQueen;

        public static CastleRights All { get { return new CastleRights(true, true, true, true); } }

        public CastleRights(bool whiteKing, bool whiteQueen, bool blackKing, bool blackQueen)
        {
            WhiteKing = whiteKing;
            WhiteQueen = whiteQueen;
            BlackKing = blackKing;
            BlackQueen = blackQueen;
        }

        public static CastleRights FromFEN(string fen)
        {
            bool whiteKing = fen.Contains("K");
            bool whiteQueen = fen.Contains("Q");
            bool blackKing = fen.Contains("k");
            bool blackQueen = fen.Contains("q");
            return new CastleRights(whiteKing, whiteQueen, blackKing, blackQueen);
        }

        public static bool operator==(CastleRights c1, CastleRights c2)
        {
            return c1.WhiteKing == c2.WhiteKing &&
                c1.WhiteQueen == c2.WhiteQueen &&
                c1.BlackKing == c2.BlackKing &&
                c1.BlackQueen == c2.BlackQueen;
        }

        public static bool operator !=(CastleRights c1, CastleRights c2)
        {
            return !(c1 == c2);
        }

        public override bool Equals(object obj)
        {
            if (typeof(CastleRights) == obj.GetType())
                return (CastleRights)obj == this;
            return false;
        }

        public override int GetHashCode()
        {
            return 
                (Convert.ToInt32(WhiteKing) + 1 << 28) |
                (Convert.ToInt32(WhiteQueen) + 1 << 20) |
                (Convert.ToInt32(BlackKing) + 1 << 12) |
                (Convert.ToInt32(BlackQueen) + 1 << 4);
        }
    }
}