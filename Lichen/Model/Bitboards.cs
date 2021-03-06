﻿using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Lichen.Model
{
    using Bitboard = UInt64;

    public static class Bitboards
    {
        public const int WHITE = 0;
        public const int BLACK = 1;

        //Diagonal Constants
        public const int FORWARD = 0;
        public const int BACKWARD = 1;

        public const int NUM_SQUARES = 64;

        public static readonly byte[,] SquareDistance = InitSquareDistances();

        public static readonly Bitboard[] SquareBitboards = InitSquareBitboards();

        public static readonly Bitboard[] RowBitboards = InitRowBitboards();
        public static readonly Bitboard[] ColumnBitboards = InitColumnBitboards();
        public static readonly Bitboard[,] DiagonalBitboards = InitDiagonalBitboards();

        public static readonly Bitboard[,] LineBitboards = InitLineBitboards();
        public static readonly Bitboard[,] BetweenBitboards = InitBetweenBitboards();

        public static readonly Bitboard[] KingBitboards = InitKingBitboards();
        public static readonly Bitboard[] KnightBitboards = InitKnightBitboards();
        public static readonly Bitboard[,] PawnBitboards = InitPawnBitboards();

        public static readonly Bitboard[] IsolatedPawnBitboards = InitIsolatedPawnBitboards();
        public static readonly Bitboard[,] PassedPawnBitboards = InitPassedPawnBitboards();
        public static readonly Bitboard[,] OutpostMasks = InitOutpostMasks();

        public static readonly CastleInfo[][] CastleInfo = InitCastleRookBitboards();
        public static readonly int[] EnPassentOffset = { -8, 8 };

        private const Bitboard DeBruijnSequence = 0x37E84A99DAE458F;
        private static readonly int[] MultiplyDeBruijnBitPosition =
        {
            0,  1, 17,  2, 18, 50,  3, 57,
            47, 19, 22, 51, 29,  4, 33, 58,
            15, 48, 20, 27, 25, 23, 52, 41,
            54, 30, 38,  5, 43, 34, 59,  8,
            63, 16, 49, 56, 46, 21, 28, 32,
            14, 26, 24, 40, 53, 37, 42,  7,
            62, 55, 45, 31, 13, 39, 36,  6,
            61, 44, 12, 35, 60, 11, 10,  9,
        };

        public static readonly Bitboard[] RookMagics =
        {
            0x4080019028834000UL, 0x2100144000210184UL, 0x1200120060809840UL, 0x0300081000600700UL,
            0x0200200600104408UL, 0x1300240003002218UL, 0x020008120002812CUL, 0x4180052140801900UL,
            0x3212002102008440UL, 0x2223802000400684UL, 0x4012004130620084UL, 0x20C4801001D80080UL,
            0x102B808034001800UL, 0x01020010495A0014UL, 0x04EF0041001C2200UL, 0x4C1200050882135CUL,
            0x0296818000204006UL, 0x0023850021084000UL, 0x0010450020001101UL, 0x0100090021003000UL,
            0x025931000801000CUL, 0x024E00804C000280UL, 0x4C078C0008102126UL, 0x0811620003006484UL,
            0x00A0800300204105UL, 0x0862030200406480UL, 0x0A1020030040B101UL, 0x1100D00100208900UL,
            0x02050011000C8800UL, 0x081A006200291004UL, 0x0241010400903A08UL, 0x3010DC06000480D1UL,
            0x329566400280008CUL, 0x4CD0002018400140UL, 0x0100801000802005UL, 0x4400201001000B02UL,
            0x62420008060010A0UL, 0x0402005012000804UL, 0x0A6010480C00092AUL, 0x0020928402000147UL,
            0x27A480C010288000UL, 0x0219A0005002C007UL, 0x0C10022000808010UL, 0x0E11224201120008UL,
            0x648A003460120018UL, 0x490200280C820011UL, 0x31C2220811040030UL, 0x0020048154020003UL,
            0x1004420B28810200UL, 0x220082004F511600UL, 0x0531006008C07500UL, 0x3A5270C00A02A200UL,
            0x0B01800400480080UL, 0x2A0400854D001900UL, 0x4461081026414400UL, 0x024D842110C18600UL,
            0x4052804222003102UL, 0x503627C003011281UL, 0x4001542000084101UL, 0x484BD81000200D01UL,
            0x181A0010780C6006UL, 0x2802000550840802UL, 0x2182821819300084UL, 0x2654038D06240142UL
        };

        public static readonly int[] RookShifts =
        {
            52, 53, 53, 53, 53, 53, 53, 52,
            53, 54, 54, 54, 54, 54, 54, 53,
            53, 54, 54, 54, 54, 54, 54, 53,
            53, 54, 54, 54, 54, 54, 54, 53,
            53, 54, 54, 54, 54, 54, 54, 53,
            53, 54, 54, 54, 54, 54, 54, 53,
            53, 54, 54, 54, 54, 54, 54, 53,
            52, 53, 53, 53, 53, 53, 53, 52
        };

        public static readonly Bitboard[] RookPremasks =
            MagicBitboardFactory.GenerateRookOccupancyBitboards();

        public static readonly Bitboard[][] RookMoves =
            MagicBitboardFactory.InitMagics(
                RookPremasks,
                RookMagics,
                RookShifts,
                MagicBitboardFactory.RookOffsets
            );

        public static readonly Bitboard[] BishopMagics =
        {
            0x081408081802A0C0UL, 0x10100A0244006500UL, 0x48681D810210FD04UL, 0x48681D810210FD04UL,
            0x0184042000C80401UL, 0x4220882018800382UL, 0x1920842C76402100UL, 0x5A220142021052C9UL,
            0x4810400222044108UL, 0x00D89030013100A1UL, 0x0001502C04C14416UL, 0x6888680951012005UL,
            0x0148040C20110A20UL, 0x12188A2820183434UL, 0x65058C040C1E9805UL, 0x2000221041045020UL,
            0x1084B0C11024110BUL, 0x31440218501C2042UL, 0x00D4098818C400A8UL, 0x10EB013024008004UL,
            0x0008806408A009A1UL, 0x4024800110101110UL, 0x34460144050CA20CUL, 0x407A2B5247080800UL,
            0x0F06B0A620200204UL, 0x01280841204B0545UL, 0x4128040148044090UL, 0x1084004004030002UL,
            0x401100400400C044UL, 0x5875020145028080UL, 0x14D900C0060210CAUL, 0x14D900C0060210CAUL,
            0x70102A0A4A107028UL, 0x000884103C60A253UL, 0x4602010A40500280UL, 0x0386010040A401C0UL,
            0x1024D40400A4C100UL, 0x2921044200010110UL, 0x00A4050C4A04240CUL, 0x2C09420A02448040UL,
            0x082410080440E848UL, 0x006444100C420928UL, 0x0083012110001302UL, 0x000432203100C800UL,
            0x104A102030402A01UL, 0x05812D1103030200UL, 0x1318886810C01084UL, 0x080D453200860200UL,
            0x4380881C10A4C000UL, 0x1A06004308088408UL, 0x3600020113880000UL, 0x4444948360881085UL,
            0x10A400610C240383UL, 0x044CC06901010265UL, 0x29400A6802019240UL, 0x00304A7801488510UL,
            0x07A1808848264000UL, 0x48A2F52201242081UL, 0x100C0202024A4800UL, 0x5A04682802208802UL,
            0x0000700410020210UL, 0x0013412034902988UL, 0x4C1920887D471402UL, 0x016E024408120140UL,
        };

        public static readonly int[] BishopShifts =
        {
            58, 59, 59, 59, 59, 59, 59, 58,
            59, 59, 59, 59, 59, 59, 59, 59,
            59, 59, 57, 57, 57, 57, 59, 59,
            59, 59, 57, 55, 55, 57, 59, 59,
            59, 59, 57, 55, 55, 57, 59, 59,
            59, 59, 57, 57, 57, 57, 59, 59,
            59, 59, 59, 59, 59, 59, 59, 59,
            58, 59, 59, 59, 59, 59, 59, 58
        };

        public static readonly Bitboard[] BishopPremasks =
            MagicBitboardFactory.GenerateBishopOccupancyBitboards();

        public static readonly Bitboard[][] BishopMoves =
            MagicBitboardFactory.InitMagics(
                BishopPremasks,
                BishopMagics,
                BishopShifts,
                MagicBitboardFactory.BishopOffsets
            );

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Bitboard GetRookMoveBitboard(int square, Bitboard occupancyBitboard)
        {
            Debug.Assert(square >= 0 && square < 64);
            var index = ((occupancyBitboard & RookPremasks[square]) * RookMagics[square]) >> RookShifts[square];
            return RookMoves[square][index];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Bitboard GetBishopMoveBitboard(int square, Bitboard occupancyBitboard)
        {
            Debug.Assert(square >= 0 && square < 64);
            var index = ((occupancyBitboard & BishopPremasks[square]) * BishopMagics[square]) >> BishopShifts[square];
            return BishopMoves[square][index];
        }

        private static byte[,] InitSquareDistances()
        {
            byte[,] result = new byte[NUM_SQUARES, NUM_SQUARES];
            for (int sq1 = 0; sq1 < NUM_SQUARES; sq1++)
            {
                for (int sq2 = 0; sq2 <= sq1; sq2++)
                {
                    int rowDist = Math.Abs(GetRow(sq1) - GetRow(sq2));
                    int colDist = Math.Abs(GetColumn(sq1) - GetColumn(sq2));
                    byte dist = (byte)Math.Max(colDist, rowDist);
                    result[sq1, sq2] = dist;
                    result[sq2, sq1] = dist;
                }
            }
            return result;
        }

        private static Bitboard[] InitSquareBitboards()
        {
            Bitboard[] result = new Bitboard[NUM_SQUARES];
            for (int square = 0; square < NUM_SQUARES; square++)
            {
                result[square] = 1UL << square;
            }
            return result;
        }

        private static Bitboard[] InitRowBitboards()
        {
            Bitboard[] result = new Bitboard[NUM_SQUARES];
            var rows = new Bitboard[] { 0xFF00000000000000,
                                        0x00FF000000000000,
                                        0x0000FF0000000000,
                                        0x000000FF00000000,
                                        0x00000000FF000000,
                                        0x0000000000FF0000,
                                        0x000000000000FF00,
                                        0x00000000000000FF };

            for (int square = 0; square < NUM_SQUARES; square++)
            {
                result[square] = rows[GetRow(square)];
            }
            return result;
        }

        private static Bitboard[] InitColumnBitboards()
        {
            Bitboard[] result = new Bitboard[NUM_SQUARES];
            var cols = new Bitboard[] { 0x8080808080808080,
                                        0x4040404040404040,
                                        0x2020202020202020,
                                        0x1010101010101010,
                                        0x0808080808080808,
                                        0x0404040404040404,
                                        0x0202020202020202,
                                        0x0101010101010101 };

            for (int square = 0; square < NUM_SQUARES; square++)
            {
                result[square] = cols[GetColumn(square)];
            }
            return result;
        }

        private static Bitboard[,] InitDiagonalBitboards()
        {
            Bitboard[,] result = new Bitboard[2, NUM_SQUARES];
            for (int i = 0; i < NUM_SQUARES; i++)
            {
                result[FORWARD, i] = GenerateDiagonalBitboard(i, 7);
                result[BACKWARD, i] = GenerateDiagonalBitboard(i, 9);
            }
            return result;
        }

        private static Bitboard GenerateDiagonalBitboard(int square, int offset)
        {
            Debug.Assert(square >= 0 && square < 64);

            Bitboard result = SquareBitboards[square];
            for (int i = 0; i < 2; i++) // Two passes, each in opposite directions
            {
                int curr = square;
                int next = curr + offset;
                while (next >= 0 && next < 64 && SquareDistance[curr, next] == 1)
                {
                    result |= SquareBitboards[next];
                    curr = next;
                    next += offset;
                }
                offset *= -1; // Reverse the ray direction
            }
            return result;
        }

        private static Bitboard[,] InitLineBitboards()
        {
            Bitboard[,] result = new Bitboard[NUM_SQUARES, NUM_SQUARES];

            for (int i = 0; i < NUM_SQUARES; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    Bitboard correctBitboard = 0;
                    if (RowBitboards[i] == RowBitboards[j])
                    {
                        correctBitboard = RowBitboards[i];
                    }
                    else if (ColumnBitboards[i] == ColumnBitboards[j])
                    {
                        correctBitboard = ColumnBitboards[i];
                    }
                    else if (DiagonalBitboards[FORWARD, i] == DiagonalBitboards[FORWARD, j])
                    {
                        correctBitboard = DiagonalBitboards[FORWARD, i];
                    }
                    else if (DiagonalBitboards[BACKWARD, i] == DiagonalBitboards[BACKWARD, j])
                    {
                        correctBitboard = DiagonalBitboards[BACKWARD, i];
                    }
                    result[i, j] = correctBitboard;
                    result[j, i] = correctBitboard;
                }
            }
            return result;
        }

        private static Bitboard[,] InitBetweenBitboards()
        {
            Bitboard[,] result = new Bitboard[NUM_SQUARES, NUM_SQUARES];
            for (int i = 0; i < NUM_SQUARES; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    int offset = 0;
                    if (RowBitboards[i] == RowBitboards[j])
                        offset = 1;
                    else if (ColumnBitboards[i] == ColumnBitboards[j])
                        offset = 8;
                    else if (DiagonalBitboards[FORWARD, i] == DiagonalBitboards[FORWARD, j])
                        offset = 7;
                    else if (DiagonalBitboards[BACKWARD, i] == DiagonalBitboards[BACKWARD, j])
                        offset = 9;
                    Bitboard currBitboard = 0;
                    if (offset != 0)
                    {
                        int currSquare = j + offset;
                        while (currSquare != i)
                        {
                            currBitboard |= Bitboards.SquareBitboards[currSquare];
                            currSquare += offset;
                        }
                    }
                    result[i, j] = currBitboard;
                    result[j, i] = currBitboard;
                }
            }
            return result;
        }

        private static Bitboard[] InitKingBitboards()
        {
            Bitboard[] result = new Bitboard[NUM_SQUARES];
            int[] offsets = { -9, -8, -7, -1, 1, 7, 8, 9 };
            for (int square = 0; square < NUM_SQUARES; square++)
            {
                result[square] = GenerateBitboardFromOffsets(square, 1, offsets);
            }
            return result;
        }

        private static Bitboard[] InitKnightBitboards()
        {
            Bitboard[] result = new Bitboard[NUM_SQUARES];
            int[] offsets = { 17, 15, 10, 6, -6, -10, -15, -17 };
            for (int square = 0; square < NUM_SQUARES; square++)
            {
                result[square] = GenerateBitboardFromOffsets(square, 5, offsets);
            }
            return result;
        }

        private static Bitboard[,] InitPawnBitboards()
        {
            Bitboard[,] result = new Bitboard[2, NUM_SQUARES];
            int[][] offsets = new int[2][];
            offsets[WHITE] = new int[] { 7, 9 };
            offsets[BLACK] = new int[] { -7, -9 };

            for (int i = 0; i < 64; i++)
            {
                if (i < Position.H8)
                {
                    result[WHITE, i] = GenerateBitboardFromOffsets(i, 1, offsets[WHITE]);
                }

                if (i > Position.A1)
                {
                    result[BLACK, i] = GenerateBitboardFromOffsets(i, 1, offsets[BLACK]);
                }
            }
            return result;
        }

        private static Bitboard[] InitIsolatedPawnBitboards()
        {
            Bitboard[] result = new Bitboard[NUM_SQUARES];

            for (int i=0;i<NUM_SQUARES;i++)
            {
                Bitboard current = 0;
                int column = GetColumn(i);
                if (column != 0) // Not A File
                {
                    current |= ColumnBitboards[i + 1];
                }
                if (column != 7) // Not H File
                {
                    current |= ColumnBitboards[i - 1];
                }
                result[i] = current;
            }

            return result;
        }

        private static Bitboard[,] InitPassedPawnBitboards()
        {
            Bitboard[,] result = new Bitboard[2, NUM_SQUARES];

            for (int i=0;i<NUM_SQUARES;i++)
            {
                Bitboard columnsBitboard = ColumnBitboards[i] | IsolatedPawnBitboards[i];
                columnsBitboard &= ~RowBitboards[i];

                Bitboard whiteMask = 0;
                int currRow = i + 8;
                while (currRow < NUM_SQUARES)
                {
                    whiteMask |= RowBitboards[currRow];
                    currRow += 8;
                }
                result[WHITE, i] = columnsBitboard & whiteMask;
                result[BLACK, i] = columnsBitboard & ~whiteMask;
            }

            return result;
        }

        private static Bitboard[,] InitOutpostMasks()
        {
            Bitboard[,] result = new Bitboard[2, NUM_SQUARES];

            for (int color = 0; color < 2; color++)
            {
                for (int square = 0; square < NUM_SQUARES; square++)
                {
                    result[color, square] = PassedPawnBitboards[color, square] & IsolatedPawnBitboards[square];
                }
            }

            return result;
        }

        public static Bitboard GenerateBitboardFromOffsets(int square, int maxDistance, int[] offsets)
        {
            Bitboard result = 0;
            foreach (int offset in offsets)
            {
                int currSquare = square + offset;
                if (currSquare >= 0 && currSquare < NUM_SQUARES && SquareDistance[currSquare, square] <= maxDistance)
                {
                    result |= SquareBitboards[currSquare];
                }
            }
            return result;
        }

        private static CastleInfo[][] InitCastleRookBitboards()
        {
            CastleInfo[][] result = new CastleInfo[2][];
            result[WHITE] = new CastleInfo[]
            {
                new CastleInfo(Position.E1, Position.G1, Position.H1, Position.F1, WHITE),
                new CastleInfo(Position.E1, Position.C1, Position.A1, Position.D1, WHITE)
            };

            result[BLACK] = new CastleInfo[]
            {
                new CastleInfo(Position.E8, Position.G8, Position.H8, Position.F8, BLACK),
                new CastleInfo(Position.E8, Position.C8, Position.A8, Position.D8, BLACK)
            };
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetColumn(int square)
        {
            Debug.Assert(square >= 0 && square < NUM_SQUARES);
            return 7 - square % 8;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetRow(int square)
        {
            Debug.Assert(square >= 0 && square < NUM_SQUARES);
            return 7 - square / 8;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AreAligned(int square1, int square2, int square3)
        {
            return (Bitboards.LineBitboards[square1, square2] & Bitboards.SquareBitboards[square3]) != 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CountBits(Bitboard bitboard)
        {
            int cnt = 0;
            while (bitboard != 0)
            {
                bitboard &= (bitboard - 1);
                cnt++;
            }
            return cnt;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PopLsb(ref Bitboard bitboard)
        {
            bitboard &= (bitboard - 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int BitScanForward(this Bitboard bitboard)
        {
            Debug.Assert(bitboard != 0);
            return MultiplyDeBruijnBitPosition[((ulong)((long)bitboard & -(long)bitboard) * DeBruijnSequence) >> 58];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetSquareFromName(string name)
        {
            int col = name[0] - 'a';
            int row = name[1] - '1';
            return 8 * row + (7 - col);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetNameFromSquare(int square)
        {
            Debug.Assert(square >= 0 && square < NUM_SQUARES);

            char rank = (char)('a' + (7 - square % 8));
            char file = (char)('1' + (square / 8));
            return rank.ToString() + file;
        }
    }
}
