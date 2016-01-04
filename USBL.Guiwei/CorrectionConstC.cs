using System;
using System.Collections.Generic;

using System.Text;

using System.IO;

namespace USBL.GuiWei
{
    public class CorrectionConstC
    {
        //        private BinaryReader filereader;
        private StreamReader filereader;
        private double[] DepthVector;
        private double[] VeloVector;
        private int Depth_index = 0;
        private int Velo_index = 0;
        private float TravelTime;
        private double phi, gama, ArrayDepth, Heave;
        private int[] SVPd;
        private double[] SVPc;
        public double ResultX, ResultY, ResultH;


        public CorrectionConstC(float Time, double _phi, double _gama, double usz, double heave, int[] svpd,
            double[] svpc)
        {
            TravelTime = Time;
            phi = _phi;
            gama = _gama;
            ArrayDepth = usz;
            Heave = heave;
            SVPd = svpd;
            SVPc = svpc;
        }

        public void SoundSpeedMend()
        {
            int flag = 1;
            if (ArrayDepth < SVPd[0])
            {
                ArrayDepth = SVPd[0];
            }

            //找出其实深度在深度适量中的位置
            int Nstart = 0;
            for (int i = 0; i < SVPd.Length; i++)
            {
                if (ArrayDepth >= SVPd[i])
                {
                    Nstart = i;
                }
            }

            double[] DepthVector = new double[SVPd.Length - Nstart];
            double[] SpeedVector = new double[SVPd.Length - Nstart];
            Array.Copy(SVPd, Nstart, DepthVector, 0, SVPd.Length - Nstart);
            Array.Copy(SVPc, Nstart, SpeedVector, 0, SVPd.Length - Nstart);

            int Nsample = SpeedVector.Length;
            int Nlayer = 0;
            double theta0 = gama;

            double T, sumT = 0.0;
            double[] X = new double[Nsample - 1];
            T = (DepthVector[1] - ArrayDepth)/Math.Cos(theta0)/SpeedVector[0];
            sumT = T;
            if (theta0 == 0)
            {
                for (int m = 2; m < Nsample; m++)
                {
                    T = (DepthVector[m] - DepthVector[m - 1])/SpeedVector[m - 1];
                    sumT = sumT + T;
                    if (sumT > TravelTime)
                    {
                        T = TravelTime - (sumT - T);
                        Nlayer = m;
                        break;
                    }
                }
            }
            else
            {
                X[0] = T*SpeedVector[0]*Math.Sin(theta0);
                for (int m = 2; m < Nsample; m++)
                {
                    theta0 = Math.Asin(Math.Sin(theta0)/SpeedVector[m - 2]*SpeedVector[m - 1]);

                    //声线拐点，过了拐点后就不是有效探测点了
                    if (theta0 == Math.PI/2)
                    {
                        flag = 0;
                        break;
                    }
                    T = (DepthVector[m] - DepthVector[m - 1])/Math.Cos(theta0)/SpeedVector[m - 1];
                    X[m - 1] = (DepthVector[m] - DepthVector[m - 1])*Math.Tan(theta0);
                    sumT = sumT + T;
                    //判断是否超出给定的单程时间，如果超过，则散射点位于该层
                    if (sumT > TravelTime)
                    {
                        T = TravelTime - (sumT - T);
                        X[m - 1] = T*SpeedVector[m - 1]*Math.Sin(theta0);
                        Nlayer = m;
                        break;
                    }
                }
            }

            if (Nlayer == 0)
            {
                throw new Exception("Exceed max water Range!");
            }
            else if (!(flag == 0))
            {
                double R = 0.0;
                for (int i = 0; i < Nlayer; i++)
                {
                    R = R + X[i];
                }
                ResultX = R*Math.Cos(phi);
                ResultY = R*Math.Sin(phi);
                ResultH = DepthVector[Nlayer - 1] + T*SpeedVector[Nlayer - 1]*Math.Cos(theta0);
            }
            else
            {
                throw new Exception("Exceed valid Range!");
            }
        }
    }
}