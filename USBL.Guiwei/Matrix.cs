using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace USBL.GuiWei
{
    public class Matrix
    {
        double[,] m_data;

        public int Row
        {
            get
            {
                return m_data.GetLength(0);
            }
        }

        public int Col
        {
            get
            {
                return m_data.GetLength(1);
            }
        }


        //构造方阵
        public Matrix(int row)
        {
            m_data = new double[row, row];
        }

        public Matrix(int row, int col)
        {
            m_data = new double[row, col];
        }

        //复制构造函数
        public Matrix(Matrix m)
        {
            int row = m.Row;
            int col = m.Col;
            m_data = new double[row, col];
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    m_data[i, j] = m[i, j];
                }
            }
        }

        //分配矩阵大小，对于已含有内存的矩阵将清空
        public void SetSize(int row, int col)
        {
            m_data = new double[row, col];
        }

        //设置单位矩阵
        public void SetUnit()
        {
            for (int i = 0; i < m_data.GetLength(0); i++)
            {
                for (int j = 0; j < m_data.GetLength(1); j++)
                {
                    m_data[i, j] = ((i == j) ? 1 : 0);
                }
            }
        }

        //存取数据成员
        public double this[int row, int col]
        {
            get { return m_data[row, col]; }
            set { m_data[row, col] = value; }
        }

        //初等行变换，对调两行ri<-->rj
        public Matrix Exchange(int i, int j)
        {
            double temp;
            for (int k = 0; k < Col; k++)
            {
                temp = m_data[i, k];
                m_data[i, k] = m_data[j, k];
                m_data[j, k] = temp;
            }
            return this;
        }

        //初等变换 第index行乘以mul
        Matrix Multiple(int index, double mul)
        {
            for (int j = 0; j < Col; j++)
            {
                m_data[index, j] *= mul;
            }
            return this;
        }

        //初等行变换 第src行乘以mul加到第index行
        Matrix MultipleAdd(int index, int src, double mul)
        {
            for (int j = 0; j < Col; j++)
            {
                m_data[index, j] += m_data[src, j] * mul;
            }
            return this;
        }

        //transpose 转置
        public Matrix Transpose()
        {
            Matrix ret = new Matrix(Col, Row);
            for (int i = 0; i < Row; i++)
            {
                for (int j = 0; j < Col; j++)
                {
                    ret[j, i] = m_data[i, j];
                }
            }
            return ret;
        }

        //binary addition 矩阵加
        public static Matrix operator +(Matrix lhs, Matrix rhs)
        {
            if (lhs.Row != rhs.Row)//异常
            {
                System.Exception e = new Exception("相加的两个矩阵的行数不相等");
                throw e;
            }
            if (lhs.Col != rhs.Col)//异常
            {
                System.Exception e = new Exception("相加的两个矩阵的列数不相等");
                throw e;
            }
            int row = lhs.Row;
            int col = lhs.Col;
            Matrix ret = new Matrix(row, col);
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    ret[i, j] = lhs[i, j] + rhs[i, j];
                }
            }
            return ret;
        }

        //binary subtraction 矩阵减
        public static Matrix operator -(Matrix lhs, Matrix rhs)
        {
            if (lhs.Row != rhs.Row)//异常
            {
                System.Exception e = new Exception("相减的两个矩阵的行数不相等");
                throw e;
            }
            if (lhs.Col != rhs.Col)//异常
            {
                System.Exception e = new Exception("相减的两个矩阵的列数不相等");
                throw e;
            }
            int row = lhs.Row;
            int col = lhs.Col;
            Matrix ret = new Matrix(row, col);
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    ret[i, j] = lhs[i, j] - rhs[i, j];
                }
            }
            return ret;
        }

        //binary multiple 矩阵乘
        public static Matrix operator *(Matrix lhs, Matrix rhs)
        {
            if (lhs.Col != rhs.Row)//异常
            {
                System.Exception e = new Exception("相乘的两个矩阵的行列数不匹配");
                throw e;
            }
            Matrix ret = new Matrix(lhs.Row, rhs.Col);
            double temp;
            for (int i = 0; i < lhs.Row; i++)
            {
                for (int j = 0; j < rhs.Col; j++)
                {
                    temp = 0;
                    for (int k = 0; k < lhs.Col; k++)
                    {
                        temp += lhs[i, k] * rhs[k, j];
                    }
                    ret[i, j] = temp;
                }
            }
            return ret;
        }

        //binary division 矩阵除
        public static Matrix operator /(Matrix lhs, Matrix rhs)
        {
            return lhs * rhs.Inverse();
        }

        //unary addtion 单目加
        public static Matrix operator +(Matrix m)
        {
            Matrix ret = new Matrix(m);
            return ret;
        }

        //unary substraction 单目减
        public static Matrix operator -(Matrix m)
        {
            Matrix ret = new Matrix(m);
            for (int i = 0; i < ret.Row; i++)
            {
                for (int j = 0; j < ret.Col; j++)
                {
                    ret[i, j] = -ret[i, j];
                }
            }
            return ret;
        }

        //number multiple 数乘
        public static Matrix operator *(double d, Matrix m)
        {
            Matrix ret = new Matrix(m);
            for (int i = 0; i < ret.Row; i++)
            {
                for (int j = 0; j < ret.Col; j++)
                    ret[i, j] *= d;
            }
            return ret;
        }

        //number division 数除
        public static Matrix operator /(double d, Matrix m)
        {
            return d * m.Inverse();
        }

        //功能：返回列主元素的行号
        //参数：row为开始查找的行号
        //说明：在行号（row，Row）范围内查找row列中绝对值的元素，返回所在行号
        int Pivot(int row)
        {
            int index = row;
            for (int i = row + 1; i < Row; i++)
            {
                if (m_data[i, row] > m_data[index, row])
                    index = i;
            }
            return index;
        }

        //inversion 逆阵:使用矩阵的初等变换，列主元素消去法
        public Matrix Inverse()
        {
            if (Row != Col)//矩阵非方阵
            {
                System.Exception e = new Exception("求逆的矩阵不是方阵");
                throw e;
            }
            Matrix tmp = new Matrix(this);
            Matrix ret = new Matrix(Row);
            ret.SetUnit();
            int maxIndex;
            double dMul;
            for (int i = 0; i < Row; i++)
            {
                maxIndex = tmp.Pivot(i);
                if (tmp.m_data[maxIndex, i] == 0)
                {
                    System.Exception e = new Exception("求逆的矩阵的行列式的值等于0");
                    throw e;
                }
                if (maxIndex != i)
                {
                    tmp.Exchange(i, maxIndex);
                    ret.Exchange(i, maxIndex);
                }
                ret.Multiple(i, 1 / tmp[i, i]);
                tmp.Multiple(i, 1 / tmp[i, i]);
                for (int j = i + 1; j < Row; j++)
                {
                    dMul = -tmp[j, i] / tmp[i, i];
                    tmp.MultipleAdd(j, i, dMul);
                }
            }
            for (int i = Col - 1; i > 0; i--)
            {
                for (int j = i - 1; j >= 0; j--)
                {
                    dMul = -tmp[j, i] / tmp[i, i];
                    tmp.MultipleAdd(j, i, dMul);
                    ret.MultipleAdd(j, i, dMul);
                }
            }
            return ret;
        }
        //计算协方差


        //determine if the matrix is square:方阵
        public bool isquare()
        {
            return Row == Col;
        }

        //determin if the matrix is symmetric 对称阵
        public bool IsSymmetric()
        {
            if (Row != Col)
                return false;

            for (int i = 0; i < Row; i++)
                for (int j = i + 1; j < Col; j++)
                    if (m_data[i, j] != m_data[j, i])
                        return false;
            return true;
        }
        //一阶矩阵->实数
        /*public double ToDouble()
        {
            Trace.Assert(Row==1&&Col==1);
            return m_data[0, 0];
        }*/

        //conert to string 
        public override string ToString()
        {
            string s = "";
            for (int i = 0; i < Row; i++)
            {
                for (int j = 0; j < Col; j++)
                    s += string.Format("{0}", m_data[i, j]);
                s += "\r\n";
            }
            return s;
        }

    }
}
