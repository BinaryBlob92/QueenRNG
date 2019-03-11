﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace QueenRng
{
    enum QueenAction { None, Scream, Stomp, StepBack };
    enum RngManipulation
    {
        None = 0x00,
        Run = 0x08,
        Run2 = 0x0C,
        ShootShield = 0x84,
    };

    class Program
    {
        static int[] Numbers = new int[] { 0xC6, 0x7E, 0x81, 0x6B, 0x4B, 0xFB, 0xE2, 0xFB, 0x54, 0xF6, 0xBD, 0xDF, 0x7C, 0x1C, 0xE1, 0x87, 0x01, 0xBF, 0x31, 0xDE, 0x56, 0x72, 0x0F, 0x47, 0x67, 0x66, 0x87, 0x59, 0xAA, 0x88, 0x3C, 0x59, 0xEA, 0x56, 0x13, 0x7B, 0xD2, 0x85, 0xA1, 0xD8, 0x3C, 0x54, 0x55, 0x2F, 0x37, 0xAE, 0x65, 0x5B, 0xDA, 0x02, 0x79, 0x98, 0xCC, 0xE3, 0x1A, 0x76, 0x8E, 0x5F, 0xD9, 0x99, 0x8F, 0x1F, 0x3F, 0x36, 0xEE, 0x43, 0x78, 0x4D, 0x0D, 0xFA, 0xBE, 0xA6, 0xDA, 0xE4, 0x86, 0x8E, 0xDC, 0x29, 0x6D, 0x4E, 0xFF, 0x56, 0xE1, 0x70, 0x20, 0xFB, 0x8F, 0xB1, 0x58, 0x05, 0x90, 0xC5, 0x09, 0xDC, 0x53, 0xCD, 0xAA, 0x3B, 0x48, 0x99, 0x52, 0xD3, 0x52, 0x9D, 0x06, 0x9F, 0xEA, 0xB5, 0xC2, 0x06, 0x13, 0x98, 0x49, 0xB2, 0x01, 0x1E, 0xAC, 0x32, 0x88, 0x31, 0x9C, 0x52, 0x46, 0x95, 0x71, 0x36, 0x8F, 0x57, 0xF6, 0x39, 0x1D, 0x16, 0xFA, 0x88, 0x74, 0xF5, 0x98, 0x7C, 0x17, 0x5C, 0x41, 0xBB, 0x6D, 0x71, 0x8E, 0x0F, 0x70, 0x59, 0xC7, 0x01, 0x1B, 0x2F, 0x33, 0x3D, 0x91, 0xC0, 0x1D, 0xA5, 0x0D, 0x0D, 0xAB, 0x33, 0x8D, 0x7E, 0x5E, 0x8F, 0x3E, 0xE6, 0x68, 0x74, 0xA6, 0x3A, 0xB1, 0xC3, 0x93, 0x11, 0xA8, 0x64, 0xC7, 0xDB, 0xCA, 0xE0, 0x60, 0xE1, 0xF3, 0xBF, 0x09, 0x00, 0x67, 0xA2, 0xE3, 0x25, 0xA0, 0x21, 0x31, 0x87, 0xD5, 0x62, 0xC5, 0xA8, 0x4F, 0x7E, 0x2E, 0x09, 0x6B, 0x94, 0x9F, 0xB0, 0x6D, 0xA9, 0x9E, 0x5A, 0x0B, 0x46, 0x70, 0x80, 0xB6, 0xCF, 0x47, 0x0C, 0xA6, 0xA5, 0x2A, 0xD8, 0xAC, 0xFB, 0xA0, 0xEB, 0xB7, 0x79, 0x24, 0x72, 0x23, 0x92, 0x48, 0x80, 0xC5, 0xA6, 0xA7, 0x85, 0xB7, 0xD7, 0x8C, 0x90, 0xE4, 0xAB, 0x63, 0x44, 0x52, 0x66, 0xE3, 0x9C, 0x33, 0x25, 0xF9, 0x5E };

        static void Main(string[] args)
        {
            var translation = new string[] { null, "A", "B", ">" };
            var durations = new List<int>();
            var sequenceStartRngManipulation = new RngManipulation[] { RngManipulation.None, RngManipulation.None, RngManipulation.None, RngManipulation.None, RngManipulation.None, RngManipulation.None, RngManipulation.None, RngManipulation.None };

            for (int startFrame = 0; startFrame < 256; startFrame++)
            {
                var queenActions = new List<QueenAction>();
                var currentFrame = startFrame;
                var queenAction = QueenAction.None;
                var stepBackCount = 0;

                for (int i = 0; stepBackCount < 9; i++)
                {
                    var rngManipulation = RngManipulation.None;
                    if ((i == 0 || queenActions[i - 1] == QueenAction.StepBack) && stepBackCount < sequenceStartRngManipulation.Length)
                    {
                        rngManipulation = sequenceStartRngManipulation[stepBackCount];
                    }

                    queenAction = GetQueenAction(currentFrame, queenAction, rngManipulation);
                    currentFrame += GetFrameCount(queenAction);
                    queenActions.Add(queenAction);

                    if (queenAction == QueenAction.StepBack)
                        stepBackCount++;
                }

                var duration = currentFrame - startFrame;
                Console.WriteLine("{0:X2} ({1}): {2}", startFrame, duration, string.Join("", queenActions.Select(x => translation[(int)x])));
                durations.Add(duration);
            }
            Console.WriteLine("RNG manip: {0}", string.Join(", ", sequenceStartRngManipulation));
            Console.WriteLine("Average: {0}", durations.Average());
            Console.WriteLine("Best: {0}", durations.Min());
            Console.WriteLine("Worst: {0}", durations.Max());
            Console.WriteLine("Median: {0}", durations.OrderBy(x => x).Skip(durations.Count / 2).First());
            
            Console.ReadKey();
        }

        static QueenAction GetQueenAction(int frame, QueenAction previousAction, RngManipulation rngManipulation = RngManipulation.None)
        {
            var index = frame + frame - 0x2C + (int)rngManipulation;
            if (previousAction == QueenAction.Stomp)
                index += 8;
            else if (previousAction == QueenAction.None || previousAction == QueenAction.StepBack)
                return (Numbers[index & 0xFF] & 0x01) == 0x01 ? QueenAction.Scream : QueenAction.Stomp;

            var number = Numbers[index & 0xFF] & 0x07;
            if (number < 5)
                return QueenAction.StepBack;
            else if (number < 7)
                return QueenAction.Scream;
            else
                return QueenAction.Stomp;
        }

        static int GetFrameCount(QueenAction queenAction)
        {
            switch (queenAction)
            {
                case QueenAction.Scream:
                    return 97;
                case QueenAction.Stomp:
                    return 73;
                case QueenAction.StepBack:
                    return 151;
                default:
                    return 0;
            }
        }
    }
}
