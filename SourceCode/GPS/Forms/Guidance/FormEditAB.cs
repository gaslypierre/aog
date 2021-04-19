﻿using System;
using System.Windows.Forms;

namespace AgOpenGPS
{
    public partial class FormEditAB : Form
    {
        private readonly FormGPS mf = null;

        private double snapAdj = 0;

        public FormEditAB(Form callingForm)
        {
            //get copy of the calling main form
            mf = callingForm as FormGPS;

            InitializeComponent();

            this.Text = gStr.gsEditABLine;
            nudMinTurnRadius.Controls[0].Enabled = false;
        }

        private void FormEditAB_Load(object sender, EventArgs e)
        {
            nudMinTurnRadius.Value = (int)((double)Properties.Settings.Default.setAS_snapDistance * mf.cm2CmOrIn);
            label1.Text = mf.unitsInCm;
            btnCancel.Focus();
            lblHalfSnapFtM.Text = mf.unitsFtM;
            lblHalfWidth.Text = (mf.tool.toolWidth*0.5*mf.m2FtOrM).ToString("N2");
            tboxHeading.Text = Math.Round(glm.toDegrees(mf.ABLine.abHeading), 5).ToString();
        }

        private void tboxHeading_Enter(object sender, EventArgs e)
        {
            tboxHeading.Text = "";

            using (var form = new FormNumeric(0, 360, Math.Round(glm.toDegrees(mf.ABLine.abHeading), 5)))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tboxHeading.Text = ((double)form.ReturnValue).ToString();
                    mf.ABLine.abHeading = glm.toRadians((double)form.ReturnValue);
                    mf.ABLine.SetABLineByHeading();
                }
                else tboxHeading.Text = Math.Round(glm.toDegrees(mf.ABLine.abHeading), 5).ToString();

            }

            btnCancel.Focus();
        }

        private void nudMinTurnRadius_Enter(object sender, EventArgs e)
        {
            mf.KeypadToNUD((NumericUpDown)sender, this);
            btnCancel.Focus();
        }

        private void nudMinTurnRadius_ValueChanged(object sender, EventArgs e)
        {
            snapAdj = (double)nudMinTurnRadius.Value * mf.inOrCm2Cm * 0.01;
        }

        private void btnAdjRight_Click(object sender, EventArgs e)
        {
            mf.ABLine.MoveABLine(snapAdj);
        }

        private void btnAdjLeft_Click(object sender, EventArgs e)
        {
            mf.ABLine.MoveABLine(-snapAdj);
        }

        private void bntOk_Click(object sender, EventArgs e)
        {
            //index to last one. 
            int idx = mf.ABLine.numABLineSelected - 1;

            if (idx >= 0)
            {
                mf.ABLine.lineArr[idx].heading = mf.ABLine.abHeading;
                //calculate the new points for the reference line and points
                mf.ABLine.lineArr[idx].origin.easting = mf.ABLine.refPoint1.easting;
                mf.ABLine.lineArr[idx].origin.northing = mf.ABLine.refPoint1.northing;

                //sin x cos z for endpoints, opposite for additional lines
                mf.ABLine.lineArr[idx].ref1.easting = mf.ABLine.lineArr[idx].origin.easting - (Math.Sin(mf.ABLine.lineArr[idx].heading) *   1200);
                mf.ABLine.lineArr[idx].ref1.northing = mf.ABLine.lineArr[idx].origin.northing - (Math.Cos(mf.ABLine.lineArr[idx].heading) * 1200);
                mf.ABLine.lineArr[idx].ref2.easting = mf.ABLine.lineArr[idx].origin.easting + (Math.Sin(mf.ABLine.lineArr[idx].heading) *   1200);
                mf.ABLine.lineArr[idx].ref2.northing = mf.ABLine.lineArr[idx].origin.northing + (Math.Cos(mf.ABLine.lineArr[idx].heading) * 1200);
            }

            mf.FileSaveABLines();
            mf.ABLine.moveDistance = 0;

            mf.panelRight.Enabled = true;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            int last = mf.ABLine.numABLineSelected;
            mf.FileLoadABLines();

            mf.ABLine.numABLineSelected = last;
            mf.ABLine.refPoint1 = mf.ABLine.lineArr[mf.ABLine.numABLineSelected - 1].origin;
            mf.ABLine.abHeading = mf.ABLine.lineArr[mf.ABLine.numABLineSelected - 1].heading;
            mf.ABLine.SetABLineByHeading();
            mf.ABLine.isABLineSet = true;
            mf.ABLine.isABLineLoaded = true;
            mf.ABLine.moveDistance = 0;

            mf.panelRight.Enabled = true;
            Close();
        }

        private void btnSwapAB_Click(object sender, EventArgs e)
        {
            mf.ABLine.abHeading += Math.PI;
            if (mf.ABLine.abHeading > glm.twoPI) mf.ABLine.abHeading -= glm.twoPI;

            mf.ABLine.refABLineP1.easting = mf.ABLine.refPoint1.easting - (Math.Sin(mf.ABLine.abHeading) *   1200);
            mf.ABLine.refABLineP1.northing = mf.ABLine.refPoint1.northing - (Math.Cos(mf.ABLine.abHeading) * 1200);
            mf.ABLine.refABLineP2.easting = mf.ABLine.refPoint1.easting + (Math.Sin(mf.ABLine.abHeading) *   1200);
            mf.ABLine.refABLineP2.northing = mf.ABLine.refPoint1.northing + (Math.Cos(mf.ABLine.abHeading) * 1200);

            mf.ABLine.refPoint2.easting = mf.ABLine.refABLineP2.easting;
            mf.ABLine.refPoint2.northing = mf.ABLine.refABLineP2.northing;
            tboxHeading.Text = Math.Round(glm.toDegrees(mf.ABLine.abHeading), 5).ToString();
        }

        private void btnContourPriority_Click(object sender, EventArgs e)
        {
            if (mf.ABLine.isABLineSet)
            {
                mf.ABLine.SnapABLine();
            }
        }

        private void btnRightHalfWidth_Click(object sender, EventArgs e)
        {
            double dist = mf.tool.toolWidth;

            mf.ABLine.MoveABLine(dist * 0.5);
        }

        private void btnLeftHalfWidth_Click(object sender, EventArgs e)
        {
            double dist = mf.tool.toolWidth;

            mf.ABLine.MoveABLine(-dist*0.5);
        }

        private void btnNoSave_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void cboxDegrees_SelectedIndexChanged(object sender, EventArgs e)
        {
            mf.ABLine.abHeading = glm.toRadians(double.Parse(cboxDegrees.SelectedItem.ToString()));
            mf.ABLine.SetABLineByHeading();
            tboxHeading.Text = Math.Round(glm.toDegrees(mf.ABLine.abHeading), 5).ToString();
        }
    }
}
