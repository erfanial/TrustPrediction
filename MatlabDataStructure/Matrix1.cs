using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatlabDataStructure
{
    public class Matrix1
    {
        private int[] _dims;
        private double[] _arr;
        public bool isNull = false;

        public Matrix1(int dim1, double? initialValue = null)
        {
            _dims = new int[1] {dim1};
            makeNewMatrix(initialValue);
        }

        public Matrix1(double[] arr)
        {
            _dims = new int[1] { (int) arr.Length };
            _arr = new double[_dims[0]];
            for (int i = 0; i < _dims[0]; i++)
                _arr[i] = arr[i];
        }

        public Matrix1(int? is_null)
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

        public double[] EntireData
        {
            get { return _arr; }
        }

        // allow callers to initialize
        public double this[int x]
        {
            get { return _arr[x]; }
            set { _arr[x] = value; }
        }

        public static Matrix1 operator +(Matrix1 mat1, Matrix1 mat2)
        {
            if (mat1.size(0) != mat2.size(0))
                throw (new Exception("Both Operands should be of same size"));

            Matrix1 newMatrix = new Matrix1(mat1.size(0));

            for (int x = 0; x < mat1.size(0); x++)
                newMatrix[x] = mat1[x] + mat2[x];

            return newMatrix;
        }

        public static Matrix1 operator -(Matrix1 mat1, Matrix1 mat2)
        {
            if (mat1.size(0) != mat2.size(0))
                throw (new Exception("Both Operands should be of same size"));

            Matrix1 newMatrix = new Matrix1(mat1.size(0));

            for (int x = 0; x < mat1.size(0); x++)
                newMatrix[x] = mat1[x] - mat2[x];

            return newMatrix;
        }

        public static Matrix1 operator *(Matrix1 mat1, Matrix1 mat2)
        {
            if (mat1.size(0) != mat2.size(0))
                throw (new Exception("Both Operands should be of same size"));

            Matrix1 newMatrix = new Matrix1(mat1.size(0));

            for (int x = 0; x < mat1.size(0); x++)
                newMatrix[x] = mat1[x] * mat2[x];

            return newMatrix;
        }

        public static Matrix1 operator /(Matrix1 mat1, Matrix1 mat2)
        {
            if (mat1.size(0) != mat2.size(0))
                throw (new Exception("Both Operands should be of same size"));

            Matrix1 newMatrix = new Matrix1(mat1.size(0));

            for (int x = 0; x < mat1.size(0); x++)
                newMatrix[x] = mat1[x] / mat2[x];

            return newMatrix;
        }

        public string print(string firstLine)
        {
            string txt = (string)firstLine + ":\n";

            for (int i = 0; i < _dims[0]; i++)
                txt += _arr[i] + " ";

            return txt;
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

        public void AddVal(double num)
        {
            for (int i = 0; i < _dims[0]; i++)
                _arr[i] = _arr[i] + num;
        }

        public void SubVal(double num)
        {
            for (int i = 0; i < _dims[0]; i++)
                _arr[i] = _arr[i] - num;
        }

        public void MultVal(double num)
        {
            for (int i = 0; i < _dims[0]; i++)
                _arr[i] = _arr[i] * num;
        }

        public void DivVal(double num)
        {
            for (int i = 0; i < _dims[0]; i++)
                _arr[i] = _arr[i] / num;
        }

        public Matrix1 Clone()
        {
            Matrix1 mat = new Matrix1(_dims[0]);
            for (int i = 1; i <= _dims[0]; i++)
                mat[i] = this[i];

            return mat;
        }
        
        private void makeNewMatrix(double? initialValue)
        {
            int a;

            a = (int)_dims[0];
            _arr = new double[a];

            if (initialValue != null)
            {
                double initval = (double)initialValue;
                for (int i = 0; i < _dims[0]; i++)
                    _arr[i] = initval;
            }
            else
            {
                Random rand = new Random();
                for (int i = 0; i < _dims[0]; i++)
                    _arr[i] = rand.NextDouble();
            }
        }

        public Matrix1 sort()
        {
            List<double> list = new List<double>(_dims[0]);
            for (int i = 0; i < _dims[0]; i++)
                list.Add(_arr[i]);
            list.Sort();

            Matrix1 uniqueMat = new Matrix1(list.Count);
            for (int i = 0; i < list.Count; i++)
                uniqueMat[i] = list[i];
            return uniqueMat;
        }

        public Matrix1 unique()
        {
            List<double> list = new List<double>(_dims[0]);
            for (int i = 0; i < _dims[0]; i++)
                list.Add(_arr[i]);
            list.Sort();

            List<double> uniqueList = new List<double>();
            double curItem = (double)list[0];
            uniqueList.Add(curItem);
            for (int i = 0; i < list.Count; i++)
                if(curItem != list[i])
                {
                    curItem = list[i];
                    uniqueList.Add(curItem);
                }

            Matrix1 uniqueMat = new Matrix1(uniqueList.Count);
            for (int i = 0; i < uniqueList.Count; i++)
                uniqueMat[i] = uniqueList[i];
            return uniqueMat;
        }

        public double sum()
        {
            double sum = 0;
            for (int i = 0; i < _dims[0]; i++)
                sum += _arr[i];
            return sum;
        }
    }
}
