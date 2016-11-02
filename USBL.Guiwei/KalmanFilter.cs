using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace USBL.GuiWei
{
    public class KalmanFilter
    {
        private readonly static object SyncObject = new object();

        //静态接口，用于在程序域中任意位置操作KalmanFilter中的成员
        private static KalmanFilter _instance;
        //卡尔曼滤波次数
        int kalman_num = 1;
        //采样周期
        double Tcoh = 12;
        //噪声统计采样点数
        int Noise_evaluate_num = 16;
        //初始偏差
        double Initial_Error = 100;
        double wNoiseError;
        // 如果判为野值，则将flag_outliers置为true
        bool flag_outliers = false;
        //观测值
        double x_local, y_local, z_local;
        //前后两次信号差值
        double x_Local_Error, y_Local_Error, z_Local_Error;
        //状态转移矩阵
        Matrix FMat = new Matrix(6, 6);
        //量测矩阵
        Matrix HMat = new Matrix(3, 6);
        //过程方程中噪声矩阵
        Matrix DMat = new Matrix(6, 1);
        //过程方程中协方差矩阵
        Matrix QMat = new Matrix(6);
        //量测噪声协方差
        Matrix RMat = new Matrix(3);
        //卡尔曼滤波增益矩阵
        Matrix KMat = new Matrix(6, 3);
        //观测值矩阵
        Matrix Error_Velocity = new Matrix(3, 1);

        //初始协方差、状态变量
        Matrix P_Filter_0 = new Matrix(6);
        Matrix X_Filter_0 = new Matrix(6, 1);
        //预测协方差、预测状态量
        Matrix P_Predict = new Matrix(6);
        Matrix X_Predict = new Matrix(6, 1);
        //预测量测值、新息协方差
        Matrix Z_Predict = new Matrix(3, 1);
        Matrix S_Predict = new Matrix(3, 3);
        //滤波协方差、滤波状态量
        Matrix P_Filter = new Matrix(6);
        public Matrix X_Filter = new Matrix(6, 1);
        Matrix X_Filter_temp = new Matrix(6, 1);
        //上次滤波协方差、滤波状态量
        Matrix P_Filter_last = new Matrix(6);
        Matrix X_Filter_last = new Matrix(6, 1);

        //
        Matrix Gain_of_Z_Actual = new Matrix(3, 1);

        //存储前16次观测值
        List<double> Save_Error_x_Velocity = new List<double>();
        List<double> Save_Error_y_Velocity = new List<double>();
        List<double> Save_Error_z_Velocity = new List<double>();



        public static KalmanFilter GetInstance()
        {
            lock (SyncObject)
            {

                return _instance ?? (_instance = new KalmanFilter());
            }
        }

        public KalmanFilter()
        {
            //初始化时给FMat、HMat、QMat赋初值
            for (int i = 0; i < FMat.Row; i++)
            {
                for (int j = 0; j < FMat.Col; j++)
                {
                    if (i == j)
                    {
                        FMat[i, j] = 1;
                    }
                    else
                    {
                        if (j == i + 3)
                        {
                            FMat[i, j] = Tcoh;
                        }
                        else
                        {
                            FMat[i, j] = 0;
                        }
                    }
                }
            }

            for (int i = 0; i < HMat.Row; i++)
            {
                for (int j = 0; j < HMat.Col; j++)
                {
                    if (i == j)
                    {
                        HMat[i, j] = 1;
                    }
                    else
                    {
                        HMat[i, j] = 0;
                    }
                }
            }

            for (int i = 0; i < QMat.Row; i++)
            {
                for (int j = 0; j < QMat.Col; j++)
                {
                    if (i == j)
                    {
                        if (i < 3)
                            QMat[i, j] = Math.Pow(Tcoh, 4) / 4;
                        else
                            QMat[i, j] = Math.Pow(Tcoh, 2);

                    }
                    else
                    {
                        if (Math.Abs(i - j) == 3)
                            QMat[i, j] = Math.Pow(Tcoh, 3) / 2;
                        else
                            QMat[i, j] = 0;
                    }
                }
            }
            Save_Error_x_Velocity.Clear();
            Save_Error_y_Velocity.Clear();
            Save_Error_z_Velocity.Clear();
        }

        public void Kalman_Filter_Run(double x, double y, double z)
        {
            Init(x, y, z);
            if (kalman_num > Noise_evaluate_num - 1)
            {
                Kalman_Predit();
                Kalman_Outlier_Dealing();
                Kalman_Filter_Function();
                Kalman_Divergent_Overcoming();
            }

            X_Filter_last = X_Filter;
            P_Filter_last = P_Filter;
            kalman_num++;

        }

        public void Init(double x, double y, double z)
        {
            x_local = x;
            y_local = y;
            z_local = z;

            if (kalman_num > 1)
            {
                x_Local_Error = x_local - X_Filter_last[0, 0];
                y_Local_Error = y_local - X_Filter_last[1, 0];
                z_Local_Error = z_local - X_Filter_last[2, 0];
            }

            if (kalman_num < Noise_evaluate_num)
            {
                X_Filter[0, 0] = x_local;
                X_Filter[1, 0] = y_local;
                X_Filter[2, 0] = z_local;
                if (kalman_num == 1)
                {
                    X_Filter[3, 0] = 0;
                    X_Filter[4, 0] = 0;
                    X_Filter[5, 0] = 0;
                }
                else
                {
                    X_Filter[3, 0] = x_Local_Error / Tcoh;
                    X_Filter[4, 0] = y_Local_Error / Tcoh;
                    X_Filter[5, 0] = z_Local_Error / Tcoh;
                }
            }
            Save_Error_x_Velocity.Add(x_local);
            Save_Error_y_Velocity.Add(y_local);
            Save_Error_z_Velocity.Add(z_local);
            if (kalman_num > Noise_evaluate_num)
            {
                Save_Error_x_Velocity.RemoveAt(0);
                Save_Error_y_Velocity.RemoveAt(0);
                Save_Error_z_Velocity.RemoveAt(0);
            }
            //统计噪声协方差
            if (kalman_num > Noise_evaluate_num - 1)
            {
                double Var_x = Var(Save_Error_x_Velocity);
                double Var_y = Var(Save_Error_y_Velocity);
                double Var_z = Var(Save_Error_z_Velocity);
                for (int i = 0; i < RMat.Row; i++)
                {
                    for (int j = 0; j < RMat.Col; j++)
                    {
                        if (i == j)
                        {
                            switch (i)
                            {
                                case 0:
                                    RMat[i, j] = Var_x;
                                    break;
                                case 1:
                                    RMat[i, j] = Var_y;
                                    break;
                                case 2:
                                    RMat[i, j] = Var_z;
                                    break;
                                default:
                                    break;
                            }
                        }
                        else
                        {
                            RMat[i, j] = 0;
                        }
                    }
                }
            }




            if (kalman_num == Noise_evaluate_num)
            {
                wNoiseError = Math.Pow(Initial_Error, 2);
                //协方差初始化
                for (int i = 0; i < P_Filter_0.Col; i++)
                {
                    for (int j = 0; j < P_Filter_0.Row; j++)
                    {
                        if (i == j)
                        {
                            if (i < 3)
                            {
                                P_Filter_0[i, j] = wNoiseError;
                            }
                            else
                            {
                                P_Filter_0[i, j] = wNoiseError / Math.Pow(Tcoh, 2);
                            }
                        }
                        else
                        {
                            if (Math.Abs(i - j) == 3)
                                P_Filter_0[i, j] = wNoiseError / Tcoh;
                            else
                                P_Filter_0[i, j] = 0;
                        }

                    }
                }
                //状态量初始化
                X_Filter_0[0, 0] = x_local;
                X_Filter_0[1, 0] = y_local;
                X_Filter_0[2, 0] = z_local;
                X_Filter_0[3, 0] = x_Local_Error / Tcoh;
                X_Filter_0[4, 0] = y_Local_Error / Tcoh;
                X_Filter_0[5, 0] = z_Local_Error / Tcoh;
            }



            return;
        }

        //预测
        public void Kalman_Predit()
        {
            if (kalman_num == Noise_evaluate_num)
            {
                X_Predict = FMat * X_Filter_0;
                P_Predict = FMat * P_Filter_0 * (FMat.Transpose()) + QMat;
            }
            else
            {
                X_Predict = FMat * X_Filter_last;
                P_Predict = FMat * P_Filter_last * (FMat.Transpose()) + QMat;
            }
            Z_Predict = HMat * X_Predict;
            S_Predict = HMat * P_Predict * HMat.Transpose() + RMat;
            return;
        }

        //野值判断
        public void Kalman_Outlier_Dealing()
        {
            if (kalman_num > Noise_evaluate_num)
            {
                if (Math.Abs(x_local - Save_Error_x_Velocity[Noise_evaluate_num - 2]) > 15 || Math.Abs(y_local - Save_Error_y_Velocity[Noise_evaluate_num - 2]) > 15 || Math.Abs(z_local - Save_Error_z_Velocity[Noise_evaluate_num - 2]) > 15)
                {
                    x_local = Save_Error_x_Velocity[Noise_evaluate_num - 2] + X_Predict[3, 0] * Tcoh;
                    Save_Error_x_Velocity[Noise_evaluate_num - 1] = x_local;

                    y_local = Save_Error_y_Velocity[Noise_evaluate_num - 2] + X_Predict[4, 0] * Tcoh;
                    Save_Error_y_Velocity[Noise_evaluate_num - 1] = y_local;

                    z_local = Save_Error_z_Velocity[Noise_evaluate_num - 2] + X_Predict[5, 0] * Tcoh;
                    Save_Error_z_Velocity[Noise_evaluate_num - 1] = z_local;
                    flag_outliers = true;
                }
                else
                {
                    flag_outliers = false;
                }
            }
            return;
        }

        //滤波
        public void Kalman_Filter_Function()
        {
            KMat = P_Predict * HMat.Transpose() * S_Predict.Inverse();
            Error_Velocity[0, 0] = x_local;
            Error_Velocity[1, 0] = y_local;
            Error_Velocity[2, 0] = z_local;
            X_Filter_temp = X_Predict + KMat * (Error_Velocity - Z_Predict);
            P_Filter = P_Predict - KMat * S_Predict * KMat.Transpose();
            return;
        }

        //发散判断
        public void Kalman_Divergent_Overcoming()
        {
            if (kalman_num > Noise_evaluate_num)
            {
                if (flag_outliers == false)
                {
                    if (Math.Abs(X_Filter_temp[0, 0]) > Math.Abs(Error_Velocity[0, 0]))
                        Gain_of_Z_Actual[0, 0] = Math.Abs(X_Filter_temp[0, 0]) / Math.Abs(Error_Velocity[0, 0]);
                    else
                        Gain_of_Z_Actual[0, 0] = Math.Abs(Error_Velocity[0, 0]) / Math.Abs(X_Filter_temp[0, 0]);

                    if (Math.Abs(X_Filter_temp[1, 0]) > Math.Abs(Error_Velocity[1, 0]))
                        Gain_of_Z_Actual[1, 0] = Math.Abs(X_Filter_temp[1, 0]) / Math.Abs(Error_Velocity[1, 0]);
                    else
                        Gain_of_Z_Actual[1, 0] = Math.Abs(Error_Velocity[1, 0]) / Math.Abs(X_Filter_temp[1, 0]);

                    if (Math.Abs(X_Filter_temp[2, 0]) > Math.Abs(Error_Velocity[2, 0]))
                        Gain_of_Z_Actual[2, 0] = Math.Abs(X_Filter_temp[2, 0]) / Math.Abs(Error_Velocity[2, 0]);
                    else
                        Gain_of_Z_Actual[2, 0] = Math.Abs(Error_Velocity[2, 0]) / Math.Abs(X_Filter_temp[2, 0]);

                    if (Gain_of_Z_Actual[0, 0] > 10 || Gain_of_Z_Actual[1, 0] > 10 || Gain_of_Z_Actual[2, 0] > 10)
                    {
                        for (int i = 0; i < X_Filter.Row; i++)
                        {
                            if (i < 3)
                                X_Filter[i, 0] = Error_Velocity[i, 0];
                            else
                                X_Filter[i, 0] = 0;
                        }
                        kalman_num = 0;
                    }
                    else
                        X_Filter = X_Filter_temp;
                }
                else
                {
                    X_Filter = X_Filter_temp;//如果该观测点在前面判为野值，则不做滤波器是否发散的判断
                }
            }
            else
            {
                X_Filter = X_Filter_temp;//第16次不作野值判断，也不做是否发散判别
            }
            return;
        }

        //求随机数的方差
        double Var(List<double> data)
        {
            double[] v = new double[data.Count];
            data.CopyTo(v);
            double SumSqare = 0;
            for (int i = 0; i < v.Length; i++)
            {
                SumSqare = SumSqare + v[i] * v[i];
            }

            double Sum = 0;
            foreach (double d in v)
            {
                Sum = Sum + d;
            }

            double var = SumSqare / v.Length - Math.Pow(Sum / v.Length, 2);
            return var;
        }

        //
        public void Kalman_Filter_stop()
        {
            kalman_num = 0;
            Save_Error_x_Velocity.Clear();
            Save_Error_y_Velocity.Clear();
            Save_Error_z_Velocity.Clear();
        }

        public static KalmanFilter Instance
        {
            get { return GetInstance(); }
        }

    }
}

