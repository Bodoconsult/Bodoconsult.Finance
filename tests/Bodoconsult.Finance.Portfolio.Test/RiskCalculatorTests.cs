// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System;
using System.Data;
using NUnit.Framework;

namespace Bodoconsult.Finance.Portfolio.Test;

public class RiskCalculatorTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void TestCalculateWith2AssetsPortfolio()
    {

        double[] a = [1/3.0, 1/3.0, 1/3.0];

        var vectorA = MathNet.Numerics.LinearAlgebra.Vector<double>.Build.DenseOfArray(a);


        var dt = new DataTable();

        dt.Columns.Add("R", Type.GetType("System.Int32"));
        dt.Columns.Add("C", Type.GetType("System.Int32"));
        dt.Columns.Add("V", Type.GetType("System.Double"));

        // Row 1
        var workRow = dt.NewRow();
        workRow[0] = 0;
        workRow[1] = 0;
        workRow[2] = 0.0100;
        dt.Rows.Add(workRow);


        workRow = dt.NewRow();
        workRow[0] = 0;
        workRow[1] = 1;
        workRow[2] = 0.0018;
        dt.Rows.Add(workRow);


        workRow = dt.NewRow();
        workRow[0] = 0;
        workRow[1] = 2;
        workRow[2] = 0.0011;
        dt.Rows.Add(workRow);

        // Row 2
        workRow = dt.NewRow();
        workRow[0] = 1;
        workRow[1] = 0;
        workRow[2] = 0.0018;
        dt.Rows.Add(workRow);


        workRow = dt.NewRow();
        workRow[0] = 1;
        workRow[1] = 1;
        workRow[2] = 0.0109;
        dt.Rows.Add(workRow);


        workRow = dt.NewRow();
        workRow[0] = 1;
        workRow[1] = 2;
        workRow[2] = 0.0026;
        dt.Rows.Add(workRow);

        // Row 3
        workRow = dt.NewRow();
        workRow[0] = 2;
        workRow[1] = 0;
        workRow[2] = 0.0011;
        dt.Rows.Add(workRow);


        workRow = dt.NewRow();
        workRow[0] = 2;
        workRow[1] = 1;
        workRow[2] = 0.0026;
        dt.Rows.Add(workRow);


        workRow = dt.NewRow();
        workRow[0] = 2;
        workRow[1] = 2;
        workRow[2] = 0.0199;
        dt.Rows.Add(workRow);


        var rc = new RiskCalculator();
        rc.LoadVectorA(vectorA);

        rc.LoadCovarianceMatrix(dt);

        rc.Calculate();

        var risk = rc.Risk;

        Assert.IsTrue(risk- 0.037859388972001827 < 0.00000000000001);
    }
}