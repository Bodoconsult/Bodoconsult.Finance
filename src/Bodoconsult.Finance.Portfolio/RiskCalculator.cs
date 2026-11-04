
///Get the covarianz matrix as arrayusing System;
///Get the covarianz matrix as ar
///
/// ///Calcuate risk///Risk in percent as double (0.1 means 10%)1 means 10%)1 means 10%///Get covariance matrix as arraystem.Data;
/// ///Calcuate risk///Risk in percent as double (0.1 means 10%)1 means 10%)1 means 10%///Get covariance matrix as arrayCstem.Data;

using System;
using System.Data;
using MathNet.Numerics.LinearAlgebra;

namespace Bodoconsult.Finance.Portfolio;

/// <summary>
/// Calculate the risk of an existing portfolio 
/// </summary>
//[ImplementPropertyChanged]
public class RiskCalculator
{
    /// <summary>
    /// Risk in percent (0.1 means 10%)
    /// </summary>
    public double Risk { get; private set; }

    /// <summary>
    /// Vector with risk values for the single portfolio items. Risk means standard deviation in Markowitz's sense.
    /// </summary>
    internal Vector<double> VectorA { get; private set; }

    /// <summary>
    /// Transponed risk vector
    /// </summary>
    internal Vector<double> VectorB { get; private set; }


    internal Matrix<double> CovarianceMatrix { get; private set; }

    /// <summary>
    /// Get the covariance matrix as array
    /// </summary>
    /// <returns>Covariance matrix as array</returns>
    public double[,] GetCovarianceMatrixAsArray()
    {
        return CovarianceMatrix.ToArray();
    }

    /// <summary>
    /// Load vector A and save . Then calculate vector B as transposed vector A
    /// </summary>
    /// <param name="vectorA"></param>
    public void LoadVectorA(Vector<double> vectorA)
    {
        VectorA = vectorA;
        VectorB = Matrix<double>.Build.DenseOfColumnVectors(VectorA).Transpose().Row(0);
    }

    /// <summary>
    /// Load vector A and save . Then calculate vector B as transposed vector A
    /// </summary>
    /// <param name="dt"></param>
    public void LoadVectorA(DataTable dt)
    {
        var erg = new double[dt.Rows.Count];

        for (var i = 0; i < dt.Rows.Count; i++)
        {
            erg[i] = Convert.ToDouble(dt.Rows[i][0]);
        }

        VectorA = Vector<double>.Build.DenseOfArray(erg);

        VectorB = Matrix<double>.Build.DenseOfColumnVectors(VectorA).Transpose().Row(0);
    }



    /// <summary>
    /// Load covariance matrix from a datatable with three columns
    /// </summary>
    /// <param name="matrix"></param>
    public void LoadCovarianceMatrix(Matrix<double> matrix)
    {
        CovarianceMatrix = matrix;
    }

    /// <summary>
    /// Load covariance matrix from a datatable with three columns (row-column-value)
    /// The square root of the datatables row count must be an integer number (resulting in a symmetric matrix)
    /// </summary>
    /// <param name="dt"></param>
    public void LoadCovarianceMatrix(DataTable dt)
    {

        if (dt == null || dt.Rows.Count == 0) throw new Exception("LoadCovarianceMatrix: datatable empty");

        var dim = (int)Math.Sqrt(dt.Rows.Count);

        var erg = new double[dim, dim];


        var valueAlt = -1;
        var row = -1;
        var column = 0;
        foreach (DataRow rowData in dt.Rows)
        {
            var value = Convert.ToInt32(rowData[0].ToString());

            if (value != valueAlt)
            {
                row++;
                valueAlt = value;
                column = 0;
            }

            //var column = Convert.ToInt32(rowData[1].ToString());

            erg[row, column] = Convert.ToDouble(rowData[2].ToString());
            column++;
        }

        CovarianceMatrix = Matrix<double>.Build.DenseOfArray(erg);
    }


    //public double[,] Zwerg;
    //public double[,] Zwerg1;

    public void Calculate()
    {
        var erg = Matrix<double>.Build.DenseOfRowVectors(VectorB).Multiply(CovarianceMatrix);

        //Zwerg = erg.ToArray();

        var erg1 = Matrix<double>.Build.DenseOfColumnVectors(VectorA).Multiply(erg);

        //Zwerg1 = erg1.ToArray();
        try
        {
            Risk = Math.Sqrt(erg1[0, 0]);
        }
        catch
        {
            Risk = 0;
        }
    }
}