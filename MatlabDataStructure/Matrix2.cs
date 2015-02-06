using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatlabDataStructure
{
    public class Matrix2
    {
        private int[] _dims;
        private double[,] _arr;
        public bool isNull = false;

        public Matrix2(int dim1, int dim2, double? initialValue = null)
        {
            _dims = new int[2] {dim1, dim2};
            makeNewMatrix(initialValue);
        }

        public Matrix2(double[,] arr, int dim1, int dim2)
        {
            _dims = new int[2] { dim1, dim2 };
            _arr = new double[_dims[0], _dims[1]];
            for (int i = 0; i < _dims[0]; i++)
                for (int j = 0; j < _dims[1]; j++)
                    _arr[i,j] = arr[i,j];
        }

        public Matrix2(int? is_null)
        {
            isNull = true;
        }

        public int[] Dimensions
        {
            get { return _dims; }
        }

        public int Length
        {
            get { return _dims[0]; }
        }

        public int Width
        {
            get { return _dims[1]; }
        }

        public double[,] EntireData
        {
            get { return _arr; }
        }

        // allow callers to initialize
        public double this[int x, int y]
        {
            get { return _arr[x, y]; }
            set { _arr[x,y] = value; }
        }

        public static Matrix2 operator +(Matrix2 mat1, Matrix2 mat2)
        {
            if ((mat1.size(0) != mat2.size(0)) && (mat1.size(1) != mat2.size(1)))
                throw(new Exception("Both Operands should be of same size"));

            Matrix2 newMatrix = new Matrix2(mat1.size(0), mat1.size(1));

            for (int x = 0; x < mat1.size(0); x++)
                for (int y = 0; y < mat1.size(1); y++)
                    newMatrix[x,y] = mat1[x,y] + mat2[x,y];

            return newMatrix;
        }

        public static Matrix2 operator -(Matrix2 mat1, Matrix2 mat2)
        {
            if ((mat1.size(0) != mat2.size(0)) && (mat1.size(1) != mat2.size(1)))
                throw (new Exception("Both Operands should be of same size"));

            Matrix2 newMatrix = new Matrix2(mat1.size(0), mat1.size(1));

            for (int x = 0; x < mat1.size(0); x++)
                for (int y = 0; y < mat1.size(1); y++)
                    newMatrix[x, y] = mat1[x, y] - mat2[x, y];

            return newMatrix;
        }

        public static Matrix2 operator *(Matrix2 mat1, Matrix2 mat2)
        {
            if ((mat1.size(0) != mat2.size(0)) && (mat1.size(1) != mat2.size(1)))
                throw (new Exception("Both Operands should be of same size"));

            Matrix2 newMatrix = new Matrix2(mat1.size(0), mat1.size(1));

            for (int x = 0; x < mat1.size(0); x++)
                for (int y = 0; y < mat1.size(1); y++)
                    newMatrix[x, y] = mat1[x, y] * mat2[x, y];

            return newMatrix;
        }

        public static Matrix2 operator /(Matrix2 mat1, Matrix2 mat2)
        {
            if ((mat1.size(0) != mat2.size(0)) && (mat1.size(1) != mat2.size(1)))
                throw (new Exception("Both Operands should be of same size"));

            Matrix2 newMatrix = new Matrix2(mat1.size(0), mat1.size(1));

            for (int x = 0; x < mat1.size(0); x++)
                for (int y = 0; y < mat1.size(1); y++)
                    newMatrix[x, y] = mat1[x, y] / mat2[x, y];

            return newMatrix;
        }

        public int size(int val)
        {
            if (val >= 0 && val < _dims.Length)
            {
                return _dims[val];
            }else{
                throw (new Exception("Specified Value not in Matrix Range"));
            }
        }

        public string print(string firstLine)
        {
            string txt = (string)firstLine + ":\n";

            for (int i = 0; i < _dims[0]; i++)
            {
                for (int j = 0; j < _dims[1]; j++)
                    txt += _arr[i, j] + " ";
                txt += "\n";
            }
            return txt;
        }

        public void AddVal(double num)
        {
            for (int i = 0; i < _dims[0]; i++)
                for (int j = 0; j < _dims[1]; j++)
                    _arr[i,j] = _arr[i,j] + num;
        }

        public void SubVal(double num)
        {
            for (int i = 0; i < _dims[0]; i++)
                for (int j = 0; j < _dims[1]; j++)
                    _arr[i, j] = _arr[i, j] - num;
        }

        public void MultVal(double num)
        {
            for (int i = 0; i < _dims[0]; i++)
                for (int j = 0; j < _dims[1]; j++)
                    _arr[i, j] = _arr[i, j] * num;
        }

        public void DivVal(double num)
        {
            for (int i = 0; i < _dims[0]; i++)
                for (int j = 0; j < _dims[1]; j++)
                    _arr[i, j] = _arr[i, j] / num;
        }

        public Matrix2 Clone()
        {
            Matrix2 mat = new Matrix2(_dims[0], _dims[1]);
            for (int i = 0; i < _dims[0]; i++)
                for (int j = 0; j < _dims[1]; j++)
                    mat[i,j] = this[i,j];

            return mat;
        }

        private void makeNewMatrix(double? initialValue)
        {
            int a,b;

            a = (int)_dims[0];
            b = (int)_dims[1];
            _arr = new double[a,b];

            if (initialValue != null)
            {
                double initval = (double)initialValue;
                for (int i = 0; i < _dims[0]; i++)
                    for (int j = 0; j < _dims[1]; j++)
                        _arr[i,j] = initval;
            }
            else
            {
                Random rand = new Random();
                for (int i = 0; i < _dims[0]; i++)
                    for (int j = 0; j < _dims[1]; j++)
                    _arr[i,j] = rand.NextDouble();
            }
        }

        public Matrix1 getRow(int row){
            Matrix1 returned = new Matrix1(_dims[1]);
            if (row >= 0 && row < _dims[0])
            {
                for (int i = 0; i < _dims[1]; i++)
                    returned[i] = this[row,i];
            }
            else
            {
                throw (new Exception("Row is not in range"));
            }
            return returned;
        }

        public Matrix1 getCol(int col)
        {
            Matrix1 returned = new Matrix1(_dims[0]);
            if (col >= 0 && col < _dims[1])
            {
                for (int i = 0; i < _dims[0]; i++)
                    returned[i] = this[i, col];
            }
            else
            {
                throw (new Exception("Row is not in range"));
            }
            return returned;
        }

        public double sumRows(int col)
        {
            double sum = 0;
            if (col >= 0 && col < _dims[1])
            {
                for (int i = 0; i < _dims[0]; i++)
                    sum += _arr[i,col];
            }
            else
            {
                throw (new Exception("Col is not in range"));
            }
            return sum;
        }

        public double sumCols(int row)
        {
            double sum = 0;
            if (row >= 0 && row < _dims[0])
            {
                for (int i = 0; i < _dims[1]; i++)
                    sum += _arr[row,i];
            }
            else
            {
                throw (new Exception("Row is not in range"));
            }
            return sum;
        }
    }
}
